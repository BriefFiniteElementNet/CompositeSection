#region Copyright
// <copyright company="QIAU University" year="2015" >
//
//           CompositeSection: Studying composite sections behavior under
//                    monotonic axial load and biaxial bending 
//                         (Done as a masters thesis)
//
//                              _______________
//                             / o_________ o  /|
//                            /  /___  ___/   / |
//                           /      / /      /  |
//                          /  ____/ /___   /   |
//                         /  /_________/  /    ~
//                        /__o__________o_/
//                        |               |
//                        |   COMPOSITE   |
//                        |    SECTION    |
//                        ~               ~
//
//        (C) Copyright 2016, Regents of University of QIAU, Qazvin, Iran
//                           All Rights Reserved.
//
// Commercial usage of this software without written permission of copyright holder
// is strongly prohibited. 
// This software is as-is, WITHOUT ANY WARRANTY. For more information about usage, 
// editing, redistributing and warranties of this software see file copyright.txt 
// in the root folder.
// </copyright>
//
// <developers>
//    Ehsan Mohammad Ali (ehsan.ma@gmx.com) (Main Author, Done as Master's Thesis)
// </developers>
#endregion



using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositeSection.Lib
{
    public class NTetaCurveFinder
    {
        /// <summary>
        /// Section being analysed
        /// </summary>
        public Section Section;

        /// <summary>
        /// delta teta (degress)
        /// </summary>
        public double DeltaTeta = 1.0;

        /// <summary>
        /// count of different N (axial forces) between minimum and maximum axial force
        /// </summary>
        public int NCount;

        /// <summary>
        /// minimum axial force, sets by this class when user calls CreateSurface() method
        /// </summary>
        public double MinAxialForce;

        /// <summary>
        /// maximum axial force, sets by this class when user calls CreateSurface() method
        /// </summary>
        public double MaxAxialForce;

        /// <summary>
        /// List of solvers
        /// </summary>
        public List<List<Func1DSolver>> Solvers;

        /// <summary>
        /// tolerance for N
        /// </summary>
        public double Tolerance = 1e-4;

        /// <summary>
        /// count of failed tries
        /// </summary>
        public int FailedTries;

        /// <summary>
        /// count of successfull tries
        /// </summary>
        public int SuccessTries;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="additionalNsCoefs">additional N, if any. usualy uses for specific construction code for finding like 0.8Nmax etc</param>
        /// <returns></returns>
        public FailureSurface CreateSurface(params double[] additionalNsCoefs)
        {
            string msg;

            if (!Section.IsValidSection(out msg))
                throw new Exception(msg);


            var dirRanges = CriticalStrainCalculator.CalculateForSection(this.Section, DeltaTeta);

            var buf = new FailureSurface();

            var nMin = double.PositiveInfinity;
            var nMax = double.NegativeInfinity;


            Solvers = new List<List<Func1DSolver>>();

            foreach (var rngs in dirRanges)
            {
                var sl = new List<Func1DSolver>();
                Solvers.Add(sl);

                foreach (var rng in rngs)
                {
                    var slv = new Func1DSolver();
                    sl.Add(slv);
                    slv.F = new Func1D(Section, rng);



                    //nMin = Math.Min(nMin, slv.Min);
                    //nMax = slv.Max > nMax ? slv.Max : nMax;// Math.Max(nMax, slv.Max);
                }
            }


#if PARALLEL
            Parallel.ForEach(Solvers, slList =>
#else
            foreach (var slList in Solvers)
#endif

            {
                foreach (var slv in slList)
                {
                    slv.AnalyseForWalls();
                }
            }

#if PARALLEL
                );
#else

#endif

            foreach (var slList in Solvers)
            {
                foreach (var slv in slList)
                {
                    nMin = Math.Min(nMin, slv.Min);
                    nMax = Math.Max(nMax, slv.Max);
                }
            }

            MinAxialForce = nMin;
            MaxAxialForce = nMax;

            var absTol = (nMax - nMin)*Tolerance;

            Solvers.ForEach(i => i.ForEach(j => j.AbsoluteTolerance = absTol));

            var roots = new ConcurrentDictionary<int, double>();

            var aditionalNs = additionalNsCoefs.Select(i => i > 0 ? Math.Min(i , 1) * MaxAxialForce : -Math.Max(i, -1) * MinAxialForce).ToArray();

            var ns = Enumerable.Range(0, NCount).Select(i => (1.0 * i / (NCount - 1)) * (nMax - nMin) + nMin).Union(aditionalNs).Distinct().ToArray();

            Array.Sort(ns);

#if PARALLEL
            Parallel.ForEach(ns, targetN =>
#else
            foreach (var targetN in ns)
#endif
            {
                var lst = new List<FailurePoint>();

                var ring = new FailureRing() { Points = lst, TargetedAxialForce = targetN };

                buf.PointsNClassified.Add(lst);
                
                buf.Rings.Add(ring);

                for (var j = 0; j < Solvers.Count; j++)
                {
                    foreach (var solver in Solvers[j])
                    {
                        double x;

                        if ((solver.Max > targetN || MathUtil.Equals(solver.Max, targetN, absTol)) && //Greater or equal
                            (solver.Min < targetN || MathUtil.Equals(solver.Min, targetN, absTol))) //smaller or equal
                            if (solver.Solve(targetN, out x))
                            {
                                roots[GetHingHashCode(solver.F.Range)] = x;

                                var strain = solver.F.Range.GetStrainProfile(x);
                                var force = Section.GetSectionForces(strain);

                                var fp = new FailurePoint(force, strain);
                                fp.HingPosition = solver.F.Range.HingePosition;
                                fp.HingHeight = solver.F.Range.HingHeight;


                                lst.Add(fp);
                                SuccessTries++;
                            }
                            else
                            {
                                FailedTries++;
                            }
                    }
                }

                if (!lst.Any())
                    Guid.NewGuid();

            }
#if PARALLEL
                );
#else

#endif

            return buf;
        }

        [Obsolete("user CreateSurface(params double[])")]
        public FailureSurface CreateSurface(double targetN)
        {
            var sp = new Stopwatch();
            sp.Start();


            string msg;

            if (!Section.IsValidSection(out msg))
                throw new Exception(msg);

            var dirRanges = CriticalStrainCalculator.CalculateForSection(this.Section, DeltaTeta);

            var buf = new FailureSurface();

            var nMin = double.PositiveInfinity;
            var nMax = double.NegativeInfinity;

            Solvers = new List<List<Func1DSolver>>();

            foreach (var rngs in dirRanges)
            {
                var sl = new List<Func1DSolver>();
                Solvers.Add(sl);

                foreach (var rng in rngs)
                {
                    var slv = new Func1DSolver();
                    sl.Add(slv);
                    slv.F = new Func1D(Section, rng);
                    


                    //nMin = Math.Min(nMin, slv.Min);
                    //nMax = slv.Max > nMax ? slv.Max : nMax;// Math.Max(nMax, slv.Max);
                }
            }

            
#if PARALLEL
            Parallel.ForEach(Solvers, slList =>
#else
            foreach (var slList in Solvers)
#endif

            {
                foreach (var slv in slList)
                {
                    slv.AnalyseForWalls();
                }
            }

#if PARALLEL
                );
#else

#endif

            foreach (var slList in Solvers)
            {
                foreach (var slv in slList)
                {
                    nMin = Math.Min(nMin, slv.Min);
                    nMax = Math.Max(nMax, slv.Max);
                }
            }



            MinAxialForce = nMin;
            MaxAxialForce = nMax;

            if (targetN > nMax || targetN < nMin)
                throw new Exception();


            var absTol = (nMax - nMin) * Tolerance;

            Solvers.ForEach(i => i.ForEach(j => j.AbsoluteTolerance = absTol));

            var roots = new Dictionary<int, double>();

            //for (var i = 0; i < NCount; i++)
            {
                var lst = new List<FailurePoint>();
                buf.PointsNClassified.Add(lst);

                //var targetN = (1.0 * i / (NCount - 1)) * (nMax - nMin) + nMin;

                for (var j = 0; j < Solvers.Count; j++)
                {
                    foreach (var solver in Solvers[j])
                    {
                        double x;

                        if ((solver.Max > targetN || MathUtil.Equals(solver.Max, targetN, absTol)) &&//Greater or equal
                            (solver.Min < targetN || MathUtil.Equals(solver.Min, targetN, absTol)))//smaller or equal
                            if (solver.Solve(targetN, out x))
                            {
                                roots[GetHingHashCode(solver.F.Range)] = x;

                                var strain = solver.F.Range.GetStrainProfile(x);
                                var force = Section.GetSectionForces(strain);

                                var fp = new FailurePoint(force, strain);
                                fp.HingPosition = solver.F.Range.HingePosition;
                                fp.HingHeight = solver.F.Range.HingHeight;


                                lst.Add(fp);
                                SuccessTries++;
                            }
                            else
                            {
                                FailedTries++;
                            }
                    }
                }

                if (!lst.Any())
                    Guid.NewGuid();

            }

            return buf;
        }

        private int GetHingHashCode(CriticalStrainRange rng)
        {
            return rng.HingePosition.Y.GetHashCode() ^ 397 * rng.HingePosition.Z.GetHashCode() ^ 397 * rng.HingHeight.GetHashCode() ^ 397;
        }
    }
}
