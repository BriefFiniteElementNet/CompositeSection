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
    /// Represents a vector in Y-Z plane
    /// </summary>
    public struct VectorYZ
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VectorYZ"/> struct.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public VectorYZ(double y, double z):this()
        {
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y component of vector.
        /// </value>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The z component of vector.
        /// </value>
        public double Z { get; set; }


        /// <summary>
        /// Creates a VectorYZ from specified direction and length.
        /// </summary>
        /// <param name="teta">The direction angle.</param>
        /// <param name="l">The length of vector.</param>
        public static VectorYZ FromDirection(double teta, double l=1.0)
        {
            var sin = Math.Sin(teta);
            var cos = Math.Cos(teta);

            return new VectorYZ(l*cos, l*sin);
        }
    }
}
