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
    public static class MathUtil
    {

        /// <summary>
        /// Gets the angle between moment vector and positive y direction in radians.
        /// </summary>
        /// <param name="force">The force.</param>
        /// <returns>The angle between moment vector and positive y direction</returns>
        public static double GetAlpha(Force force)
        {
            return Math.Atan2(force.Mz, force.My);
        }

        /// <summary>
        /// Gets a unit vector in same direction of moment of the <see cref="force"/>.
        /// </summary>
        /// <param name="force">The force.</param>
        public static VectorYZ GetMomentUnitVector(Force force)
        {
            var y = force.My;
            var z = force.Mz;

            var l = Math.Sqrt(y*y + z*z);

            var buf = new VectorYZ(y/l, z/l);

            return buf;
        }

        public static bool Equals(double v1, double v2, double tol)
        {
            if (tol.Equals(0.0))
                return v1.Equals(v2);

            return Math.Abs(v1 - v2) < tol;
        }

        /// <summary>
        /// Determines whether the v3 is in smaller between angel of v1 and v2.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        public static bool IsBetween(VectorYZ v1, VectorYZ v2, VectorYZ v3)
        {
            var l3 = Math.Sqrt(v3.Y*v3.Y + v3.Z*v3.Z);

            var sin = v3.Z / l3;
            var cos = v3.Y / l3;

            var v1p = new VectorYZ(cos * v1.Y + sin * v1.Z, -sin * v1.Y + cos * v1.Z);
            var v2p = new VectorYZ(cos * v2.Y + sin * v2.Z, -sin * v2.Y + cos * v2.Z);

            return v1p.Z*v2p.Z < 0 && v1p.Y > 0 && v2p.Y > 0;
        }

        public static double GetGama(VectorYZ v1, VectorYZ v2, VectorYZ v3)
        {
            if (!IsBetween(v1, v2, v3))
                throw new Exception();

            var l3 = Math.Sqrt(v3.Y * v3.Y + v3.Z * v3.Z);

            var sin = v3.Z / l3;
            var cos = v3.Y / l3;

            var v1p = new VectorYZ(cos * v1.Y + sin * v1.Z, -sin * v1.Y + cos * v1.Z);
            var v2p = new VectorYZ(cos * v2.Y + sin * v2.Z, -sin * v2.Y + cos * v2.Z);

            //a1 + gama * (a2 - a1) = a3
            //gama = (a3 - a1)/(a2 - a1)

            var a1 = Math.Atan2(v1p.Z, v1p.Y);
            var a2 = Math.Atan2(v2p.Z, v2p.Y);
            var a3 = 0;


            var gama = (a3 - a1)/(a2 - a1);

            return gama;
        }

        public static double GetBarArea(double barDiameter)
        {
            return Math.PI * barDiameter * barDiameter / 4;
        }
    }
}
