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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CompositeSection.Lib
{
    public class NAlphaCurveFinder
    {
        public Section Section;

        /// <summary>
        /// The delta Alpha in degress
        /// </summary>
        public double DeltaAlpha;

        public int NCount;

        public double Tolerance = 1e-3;

        [Obsolete("for historical porpuses")]
        public FailureSurface CreateSurface(double axial)
        {
            string msg;

            if (!Section.IsValidSection(out msg))
                throw new Exception(msg);

            var prg = 0;


            var sec = Section;
            var ns = NCount;
            var deltaAlpha = DeltaAlpha * Math.PI / 180.0;
            NTetaCurveFinder fndr;

            var nteta = (fndr = new NTetaCurveFinder() { DeltaTeta = DeltaAlpha * 0.5, Section = sec, NCount = ns }).CreateSurface(axial);

            var shot = UltimateFibersSnapshot.Create(sec);

            //for (int idx = 0; idx < nteta.PointsNClassified.Count; idx++)
            {
                var lst = nteta.PointsNClassified[0].ToList();

                var nx = lst.Average(i => i.Force.Nx);

                var avgMy = lst.Average(i => i.Force.My);
                var avgMz = lst.Average(i => i.Force.Mz);


                lst.Sort(new AngleCompairer() {CentralForce = new Force(avgMy, avgMz, nx)});

                var lst2 = new List<FailurePoint>();

                var alpha = -Math.PI;

                do
                {

                    FailurePoint p1, p2;

                    if (GetTheBestPoint(lst, shot, alpha, nx, out p1))
                    {
                        var impvd = Improve(p1, alpha, nx, sec);

                        var targetAlpha = MathUtil.GetAlpha(impvd.Force) * 180 / Math.PI;

                        lst2.Add(impvd);
                    }
                    else if (GetMatchingPoints(lst, shot, alpha, nx, out p1, out p2))
                    {
                        var inptd = Interpolate(p1, p2, alpha, nx, sec);
                        var impvd = Improve(inptd, alpha, nx, sec);

                        var targetAlpha = MathUtil.GetAlpha(impvd.Force) * 180 / Math.PI;

                        lst2.Add(impvd);

                        //break;
                    }
                    else
                    {

                    }

                } while ((alpha += deltaAlpha) <= Math.PI);

                nteta.PointsNClassified[0] = lst2;
            }

            AnalyseNAlphaCurve(nteta, deltaAlpha);
            return nteta;
        }

        /// <summary>
        /// gets the underlying ntheta curve finder, do not change any thing on it, only for reading
        /// </summary>
        public NTetaCurveFinder nTetaCurveFinder
        {
            get; private set;
        }


        /// <summary>
        /// Creates the surface
        /// </summary>
        /// <param name="additionalNsCoefs">additional Ns. these are coef for nmax (if negative nmin) for situations like ring of 0.85 NMax or 0.8 Nmax should be added to output</param>
        /// <returns></returns>
        public FailureSurface CreateSurface(params double[] additionalNsCoefs)
        {
            string msg;

            if (!Section.IsValidSection(out msg))
                throw new Exception(msg);

            var prg = 0;


            var sec = Section;
            var ns = NCount;
            var deltaAlpha = DeltaAlpha*Math.PI/180.0;
            NTetaCurveFinder fndr;

            var nteta = (fndr = new NTetaCurveFinder() { DeltaTeta = DeltaAlpha * 0.5, Section = sec, NCount = ns ,Tolerance = Tolerance}).CreateSurface(additionalNsCoefs);

            var shot = UltimateFibersSnapshot.Create(sec);

            for (int idx = 0; idx < nteta.PointsNClassified.Count; idx++)
            {
                var lst = nteta.PointsNClassified[idx].ToList();

                var nx = lst.Average(i => i.Force.Nx);

                var avgMy = lst.Average(i => i.Force.My);
                var avgMz = lst.Average(i => i.Force.Mz);

                lst.Sort(new AngleCompairer() { CentralForce = new Force(avgMy, avgMz, nx) });

                var lst2 = new List<FailurePoint>();


                if (idx == 0 || idx == nteta.PointsNClassified.Count - 1)//either top point or buttom points
                {
                    lst2.AddRange(lst);
                    continue;
                }

                var alpha = -Math.PI;

                var failed = 0;

                do
                {

                    FailurePoint p1, p2;

                    if (GetTheBestPoint(lst, shot, alpha, nx, out p1))
                    {
                        var impvd = Improve(p1, alpha, nx, sec);

                        var targetAlpha = MathUtil.GetAlpha(impvd.Force) * 180 / Math.PI;

                        lst2.Add(impvd);
                    }
                    else if (GetMatchingPoints(lst, shot, alpha, nx, out p1, out p2))
                    {
                        var inptd = Interpolate(p1, p2, alpha, nx, sec);
                        var impvd = Improve(inptd, alpha, nx, sec);

                        var targetAlpha = MathUtil.GetAlpha(impvd.Force) * 180 / Math.PI;

                        lst2.Add(impvd);

                        //break;
                    }
                    else
                    {
                        failed++;
                    }

                } while ((alpha += deltaAlpha) <= Math.PI);

                nteta.PointsNClassified[idx] = lst2;
            }

            AnalyseNAlphaCurve(nteta, deltaAlpha);
            return nteta;
        }

        /// <summary>
        /// sort of post process
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="deltaAlpha"></param>
        private void AnalyseNAlphaCurve(FailureSurface surface, double deltaAlpha)
        {
            var flags = new List<bool>();
            var alphas = new List<double>();

            for (double alpha = -Math.PI; alpha < Math.PI; alpha += deltaAlpha)
            {
                flags.Add(false);
                alphas.Add(alpha);
            }

            var errs = new List<double>();

            for (var i = 1; i < surface.PointsNClassified.Count - 1; i++)
            {
                var lst = surface.PointsNClassified[i];

                for (var j = 0; j < flags.Count; j++)
                {
                    var alpha = alphas[i];

                    var neares = lst.Min(k => Math.Abs(MathUtil.GetAlpha(k.Force) - alpha));

                    var deg = neares * 180.0 / Math.PI;

                    errs.Add(Math.Abs(deg));

                    if (!MathUtil.Equals(neares, 0, 0.00001))
                    {
                        Guid.NewGuid();
                    }

                }
            }

            errs.Sort();
            errs.Reverse();



            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the best existing point if exists within a tolerance.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="snapShot">The snap shot.</param>
        /// <param name="targetAlpha">The target alpha.</param>
        /// <param name="targetNx">The target nx.</param>
        /// <param name="p1">The p1.</param>
        /// <returns></returns>
        public bool GetTheBestPoint(List<FailurePoint> points, UltimateFibersSnapshot snapShot, double targetAlpha,
            double targetNx, out FailurePoint p1)
        {
            var vt = VectorYZ.FromDirection(targetAlpha);

            for (var i = 0; i < points.Count - 1; i++)
            {
                var vi = MathUtil.GetMomentUnitVector(points[i].Force);

                if (MathUtil.Equals(vi.Y, vt.Y, 1e-3) && MathUtil.Equals(vi.Z, vt.Z, 1e-3))
                {
                    p1 = points[i];
                    return true;
                }
            }

            p1 = new FailurePoint();
            return false;
        }

        /// <summary>
        /// Gets the two matching points which are left and right of target Alpha.
        /// </summary>
        /// <param name="points">The all points with same Nx.</param>
        /// <param name="snapShot">The snap shot.</param>
        /// <param name="targetAlpha">The target alpha.</param>
        /// <param name="targetNx">The target nx.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool GetMatchingPoints(List<FailurePoint> points,UltimateFibersSnapshot snapShot, double targetAlpha,double targetNx, out FailurePoint p1,
            out FailurePoint p2)
        {
            var lst = points.ToList();

            lst.Add(lst[0]);

            var alpha = targetAlpha;

            var flag2 = false;

            var vt = VectorYZ.FromDirection(alpha);

            var cnt2 = 0;

            for (var i = 0; i < lst.Count - 1; i++)
            {
                var vi = MathUtil.GetMomentUnitVector(lst[i].Force);
                var vi1 = MathUtil.GetMomentUnitVector(lst[i + 1].Force);
                

                if (MathUtil.IsBetween(vi, vi1, vt))
                {
                    flag2 = true;

                    if (CanInterpolate(lst[i], lst[i + 1]))
                    {
                        p1 = lst[i];
                        p2 = lst[i + 1];

                        return true;
                    }
                    else
                    {
                        //find left and right of point

                        var f1 = lst[i];
                        var f2 = lst[i + 1];
                        FailurePoint f3;

                        cnt2 = 0;

                        while (Bisect(targetNx, f1, f2, snapShot, out f3) && cnt2++<100)
                        {
                            var v1 = MathUtil.GetMomentUnitVector(f1.Force);
                            var v2 = MathUtil.GetMomentUnitVector(f2.Force);
                            var v3 = MathUtil.GetMomentUnitVector(f3.Force);

                            var flag = false;

                            if (MathUtil.IsBetween(v1, v3, vt))
                            {
                                flag = true;
                                if (CanInterpolate(f1, f3))
                                {
                                    p1 = f1;
                                    p2 = f3;

                                    return true;
                                }
                                else
                                {
                                    f1 = f1;
                                    f2 = f3;
                                }
                            }

                            if (MathUtil.IsBetween(v3, v2, vt))
                            {
                                flag = true;
                                if (CanInterpolate(f3, f2))
                                {
                                    p1 = f3;
                                    p2 = f2;

                                    return true;
                                }
                                else
                                {
                                    f1 = f3;
                                    f2 = f2;
                                }
                            }

                            if (!flag)
                                Guid.NewGuid();

                        }

                        
                    }
                }
            }

            p1 = p2 = new FailurePoint();
            return false;


            throw new NotImplementedException();
        }

        public bool Bisect(double targetNx, FailurePoint p1, FailurePoint p2, UltimateFibersSnapshot snapShot,
            out FailurePoint p3)
        {
            var sin3 = 0.0; //TODO: Be fixed
            var cos3 = 0.0; //TODO: Be fixed

            {
                var s1 = p1.Strain;
                var r1 = Math.Sqrt(s1.Ky*s1.Ky + s1.Kz*s1.Kz);
                var sin1 = -s1.Ky / r1;
                var cos1 = s1.Kz / r1;

                var s2 = p2.Strain;
                var r2 = Math.Sqrt(s2.Ky * s2.Ky + s2.Kz * s2.Kz);
                var sin2 = -s2.Ky / r2;
                var cos2 = s2.Kz / r2;


                var v1 = new VectorYZ(p1.Strain.Ky, p1.Strain.Kz);
                var v2 = new VectorYZ(p2.Strain.Ky, p2.Strain.Kz);

                var v3 = new VectorYZ(0.5*(p1.Strain.Ky + p2.Strain.Ky), 0.5*(p1.Strain.Kz + p2.Strain.Kz));

                var l3 = Math.Sqrt(v3.Y * v3.Y + v3.Z * v3.Z);
                var l1 = Math.Sqrt(v1.Y * v1.Y + v3.Z * v1.Z);


                sin3 = -v3.Y/l3;
                cos3 = v3.Z/l3;
            }


            var ts = CriticalStrainCalculator.Calculate(snapShot, sin3, cos3);

            FailurePoint? m = null; //نقطه جدید با این نقطه مچ هست //این نقطه یا پی 1 هست یا پی 2

            CriticalStrainRange tRng = null; //اون رنجی که با بالایی همخونی داره

            foreach (var range in ts)
            {
                //چک کنیم با کدوم نقطه درسته

                if (MathUtil.Equals(range.HingHeight, p1.HingHeight, 0) &&
                    range.HingePosition.Equals(p1.HingPosition))
                {
                    m = p1;
                    tRng = range;
                    break;
                }

                if (MathUtil.Equals(range.HingHeight, p2.HingHeight, 0) &&
                    range.HingePosition.Equals(p2.HingPosition))
                {
                    m = p2;
                    tRng = range;
                    break;
                }

            }

            if (!m.HasValue)
            {
                //failed to find it!
                p3 = new FailurePoint();

                return false;
            }



            var f2 = new Func1D(Section, tRng);
            var f = new Func1DSolver() {F = f2};
            f.AnalyseForWalls();

            double x;

            if (!f.Solve(targetNx, out x))
            {
                p3 = new FailurePoint();
                return false;
            }

            var strain = tRng.GetStrainProfile(x);

            var force = Section.GetSectionForces(strain);

            p3 = new FailurePoint(force, strain, tRng.HingePosition, tRng.HingHeight);

            return true;
        }

       

        public static bool CanInterpolate(FailurePoint a, FailurePoint b)
        {
            return !(!a.HingPosition.Equals(b.HingPosition) || !a.HingHeight.Equals(b.HingHeight));
        }

        /// <summary>
        /// Interpolates the specified f1.
        /// </summary>
        /// <param name="a">The f1.</param>
        /// <param name="b">The b.</param>
        /// <param name="targetAlpha">The target alpha, in radian.</param>
        /// <param name="targetAxialForce">The target axial force.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public static FailurePoint Interpolate(FailurePoint a, FailurePoint b, double targetAlpha,
            double targetAxialForce, Section section)
        {
            if (!a.HingPosition.Equals(b.HingPosition) || !a.HingHeight.Equals(b.HingHeight))
                throw new InvalidOperationException();

            var tet = targetAlpha; //*Math.PI/180.0;

            var sin = Math.Sin(tet);
            var cos = Math.Cos(tet);

            var yh = a.HingPosition.Y;
            var zh = a.HingPosition.Z;

            var aa = Math.Atan2(a.Force.Mz, a.Force.My);
            var ab = Math.Atan2(b.Force.Mz, b.Force.My);

            if (Math.Abs(Math.Abs(targetAlpha) - Math.PI) < 1e-5)
            {
                Guid.NewGuid();
            }

            var gama = (tet - aa)/(ab - aa);

            var kyt = a.Strain.Ky + gama*(b.Strain.Ky - a.Strain.Ky);
            var kzt = a.Strain.Kz + gama*(b.Strain.Kz - a.Strain.Kz);
            var e0t = a.HingHeight - kyt*yh - kzt*zh;

            var buf = new FailurePoint();
            buf.HingPosition = a.HingPosition;
            buf.HingHeight = a.HingHeight;

            buf.Strain = new StrainProfile(kzt, kyt, e0t);
            buf.Force = section.GetSectionForces(buf.Strain);

            var at = Math.Atan2(buf.Force.Mz, buf.Force.My);


            var err = Math.Abs((at - targetAlpha)/targetAlpha);


            return buf;
        }

        public static FailurePoint Improve(FailurePoint a, double targetAlpha, double targetAxialForce, Section section)
        {
            return a;

            var tet = targetAlpha;

            var sin = Math.Sin(tet);
            var cos = Math.Cos(tet);

            var yh = a.HingPosition.Y;
            var zh = a.HingPosition.Z;

            var stf = section.GetSectionStiffness(a.Strain);


            var rmytkyt = stf.RmyRky + stf.RmyRe0*yh;
            var rmytkzt = stf.RmyRkz + stf.RmyRe0*zh;

            var rmztkyt = stf.RmzRky + stf.RmzRe0*yh;
            var rmztkzt = stf.RmzRkz + stf.RmzRe0*zh;


            var rnxtkyt = stf.RnxRky - stf.RnxRe0*yh;
            var rnxtkzt = stf.RnxRkz - stf.RnxRe0*zh;

            var c = -sin;
            var e = cos;


            var b11 = -c*rmytkyt - e*rmztkyt;
            var b12 = -c*rmytkzt - e*rmztkzt;

            var b21 = -rnxtkyt;
            var b22 = -rnxtkzt;

            var det = b11*b22 - b12*b21;

            var c11 = b22/det;
            var c22 = b11/det;

            var c21 = -b21/det;
            var c12 = -b12/det;

            var my = a.Force.My;
            var mz = a.Force.Mz;
            var nx = a.Force.Nx;

            var dky = c11*(c*my + e*mz) + c12*(nx - targetAxialForce);
            var dkz = c21*(c*my + e*mz) + c22*(nx - targetAxialForce);

            var kyt = a.Strain.Ky + dky;
            var kzt = a.Strain.Kz + dkz;
            var e0t = a.HingHeight - kyt*yh - kzt*zh;

            var buf = new FailurePoint();
            buf.Strain = new StrainProfile(kzt, kyt, e0t);
            buf.Force = section.GetSectionForces(buf.Strain);
            buf.HingHeight = a.HingHeight;
            buf.HingPosition = a.HingPosition;

            var ba = Math.Atan2(buf.Force.Mz, buf.Force.My);
            var aa = Math.Atan2(a.Force.Mz, a.Force.My);

            var err1 = Math.Abs((aa - targetAlpha)/targetAlpha);
            var err2 = Math.Abs((ba - targetAlpha)/targetAlpha);

            return buf;
        }


    }

}