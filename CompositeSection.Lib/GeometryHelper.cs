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
    public static class GeometryHelper
    {
        /// <summary>
        /// returns the corners of a closed rectangle which its center lies on origins (0,0)
        /// </summary>
        /// <param name="w">The width of rectangle (y direction).</param>
        /// <param name="h">The height of rectangle (z direction).</param>
        /// <returns>corners of a closed rectangle</returns>
        public static Point[] Rectangle(double h,double w)
        {
            var buf=new Point[5];

            var z = -h / 2;
            var y = -w / 2;

            buf[0]=new Point(y,z);
            buf[0]=new Point(y+w,z);
            buf[0]=new Point(y+w,z+h);
            buf[0]=new Point(y,z+h);
            buf[0]=new Point(y,z);

            return buf;
        }


    }
}
