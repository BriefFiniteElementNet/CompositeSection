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
using System.Text;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a utility class for dealing with section
    /// </summary>
    public static class SectionUtil
    {
        public static Section MoveToPlasticCenter(Section sec, double e0 = -0.0035+1e-5)
        {
            string msg;

            if (!sec.IsValidSection(out msg))
                throw new Exception(msg);

            var cl = sec.Clone();

            var s = new StrainProfile(0, 0, e0);

            var f = sec.GetSectionForces(s);

            var dy = -f.Mz/f.Nx;
            var dz = -f.My/f.Nx;

            if (MathUtil.Equals(f.Nx, 0, 1e-6))
                throw new Exception();


            foreach (var elm in cl.FiberElements)
            {
                elm.Center = new Point(elm.Center.Y + dy, elm.Center.Z + dz);
            }


            foreach (var elm in cl.SurfaceElements)
            {
                for (var i = 0; i < elm.Points.Count; i++)
                {
                    elm.Points[i] = new Point(elm.Points[i].Y + dy, elm.Points[i].Z + dz);
                }
            }

            foreach (var elm in cl.PolyLineElements)
            {
                for (var i = 0; i < elm.Points.Count; i++)
                {
                    elm.Points[i] = new Point(elm.Points[i].Y + dy, elm.Points[i].Z + dz);
                }
            }

            return cl;
        }

        public static void ScaleUltimateStrains(Section sec, double sc = 0.0001)
        {
            sc = 1 - sc;

            var allElms = new List<BaseElement>();
            allElms.AddRange(sec.FiberElements);
            allElms.AddRange(sec.PolyLineElements);
            allElms.AddRange(sec.SurfaceElements);

            foreach (var elm in allElms)
            {
                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                        elm.ForegroundMaterial.NegativeFailureStrain =
                            elm.ForegroundMaterial.NegativeFailureStrain.Value*sc;

                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                        elm.ForegroundMaterial.PositiveFailureStrain =
                            elm.ForegroundMaterial.PositiveFailureStrain.Value*sc;
                }


                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                        elm.BackgroundMaterial.NegativeFailureStrain =
                            elm.BackgroundMaterial.NegativeFailureStrain.Value*sc;

                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                        elm.BackgroundMaterial.PositiveFailureStrain =
                            elm.BackgroundMaterial.PositiveFailureStrain.Value*sc;
                }
            }
        }


#if PARALLEL
        public static bool IsParallelCompiled = true;
#else
        public static bool IsParallelCompiled = false;
#endif

#if DEBUG
        public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif
    }
}