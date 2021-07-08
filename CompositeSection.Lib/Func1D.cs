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
    public class Func1D
    {
        public static int EvaluationCount;

        public Func1D(Section section, CriticalStrainRange range)
        {
            Section = section;
            Range = range;
        }

        public Section Section;

        public CriticalStrainRange Range;

        public double GetAxialForce(double val)
        {
            EvaluationCount++;
            var str = Range.GetStrainProfile(val);
            return Section.GetSectionAxialForce(str);
        }

        public double GetAxialForceDifferentiate(double val)
        {
            //d(val)/d(N)
            var str = Range.GetStrainProfile(val);
            var stf = Section.GetSectionStiffness(str);

            var buf = -Range.Sin * (stf.RnxRky - Range.HingePosition.Y * stf.RnxRe0) +
                      Range.Cos * (stf.RnxRkz - Range.HingePosition.Z * stf.RnxRe0);

            buf *= Range.MaximumSlope - Range.MinimumSlope;

            return buf;
        }

        /// <summary>
        /// The local optimums of this functions.
        /// Between these values function is absolutely incremental or absolutely decremental
        /// </summary>
        public List<Tuple<double, double>> Walls = new List<Tuple<double, double>>();
    }
}
