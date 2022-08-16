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
    public class DTetaCurveFinder
    {
        public Section Section;

        public double DeltaTeta;

        public int DCount;


        public FailureSurface CreateSurface()
        {
            string msg;

            if (!Section.IsValidSection(out msg))
                throw new Exception(msg);


            var shot = UltimateFibersSnapshot.Create(Section);

            var dirRanges = CriticalStrainCalculator.CalculateForSection(this.Section, DeltaTeta);

            var buf = new FailureSurface();

            var lst = new List<FailurePoint>();


            var delta = 1.0/DCount;

            foreach (var rngs in dirRanges)
            {
                foreach (var rng in rngs)
                {
                    for (int i = 0; i <= DCount; i++)
                    {
                        var val = i * delta;

                        var strain = rng.GetStrainProfile(val);

                        var fp = new FailurePoint();
                        fp.Strain = strain;
                        fp.Force = Section.GetSectionForces(strain);
                        fp.HingPosition = rng.HingePosition;
                        fp.HingHeight = rng.HingHeight;

                        lst.Add(fp);
                    }
                }
               
            }

            buf.PointsNClassified.Add(lst);

            return buf;
        }
    }
}
