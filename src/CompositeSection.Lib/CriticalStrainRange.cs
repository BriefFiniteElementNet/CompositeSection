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
    /// Represents a range for critical strains which are in failure threshold in Hinge location
    /// </summary>
    public class CriticalStrainRange
    {
        /// <summary>
        /// The sin and cos, for knowing direction.
        /// </summary>
        public double Sin, Cos;

        /// <summary>
        /// The hinge position.
        /// </summary>
        public Point HingePosition;

        /// <summary>
        /// The hinge height.
        /// </summary>
        public double HingHeight;

        /// <summary>
        /// The maximum slope in defined direction by sin and cos.
        /// </summary>
        public double MaximumSlope;

        /// <summary>
        /// The minimum slope in defined direction by sin and cos.
        /// </summary>
        public double MinimumSlope;

        /// <summary>
        /// Generates the strain profile from specified <see cref="val"/>
        /// </summary>
        /// <param name="val">A value in [0,1] range.</param>
        /// <returns>Appropriated strain profile</returns>
        public StrainProfile GetStrainProfile(double val)
        {
            if (val < 0 || val > 1)
                throw new ArgumentException();

            if (MaximumSlope <= MinimumSlope)
                throw new InvalidOperationException();

            var s = MinimumSlope + (MaximumSlope - MinimumSlope)*val;

            var buf = new StrainProfile();

            buf.Kz = Cos*s;
            buf.Ky = -Sin*s;
            buf.E0 = HingHeight - buf.Kz*HingePosition.Z - buf.Ky*HingePosition.Y;

            return buf;
        }

    }
}
