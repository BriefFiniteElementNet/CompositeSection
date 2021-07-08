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
    [Serializable]
    public class FailureSurface
    {

        //public List<Triangle> Triangles;



        public FailureSurface()
        {
            //Points = new List<FailurePoint>();
            PointsNClassified = new List<List<FailurePoint>>();
        }

        public List<FailureRing> Rings=new List<FailureRing>();

        [Obsolete("use FailureSurface.Rings instead")]
        public List<List<FailurePoint>> PointsNClassified;

        public CompositeSection.Lib.FailureSurface ToOtherOne()
        {
            var buf = new CompositeSection.Lib.FailureSurface();
            buf.PointsNClassified = this.PointsNClassified;

            return buf;
        }

        public double GetSafetyFactor(Force force)
        {
            var alpha = Math.Atan2(force.Mz, force.My);


            var min = double.MaxValue;

            var minI = -1;
            var minJ = -1;

            for (var i = 0; i < PointsNClassified.Count; i++)
            {
                var avg = PointsNClassified[i].Average(j => j.Force.Nx);

                var d = Math.Abs(avg - force.Nx);

                if (d < min)
                {
                    minI = i;
                    min = d;
                }
                
            }

            if (minI == -1)
                return 0;

            var lst = PointsNClassified[minI];

            min = double.MaxValue;

            for (var j = 0; j < lst.Count; j++)
            {
                var f = lst[j].Force;

                var a1 = Math.Atan2(force.Mz, force.My);
                var a2 = Math.Atan2(f.Mz, f.My);

                var d = Math.Abs(a1 - a2);

                if (d < min)
                {
                    minJ = j;
                    min = d;
                }
            }

            if (minI == -1 && minJ == -1)
                return -1;

            var f1 = PointsNClassified[minI][minJ];

            var r1 = Math.Sqrt(f1.Force.My * f1.Force.My + f1.Force.Mz * f1.Force.Mz);

            var r2 = Math.Sqrt(force.My * force.My + force.Mz * force.Mz);

            return r1/r2;

            throw new NotImplementedException();
        }


      
    }
}
