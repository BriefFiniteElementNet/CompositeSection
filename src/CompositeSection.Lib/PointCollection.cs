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

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a collection of <see cref="Point"/>s.
    /// </summary>
    [Serializable]
    public class PointCollection : List<Point>
    {
        /// <summary>
        /// Creates a point collection from coordination array.
        /// </summary>
        /// <param name="coords">The coordinates.</param>
        /// <returns>generated point collection</returns>
        public static PointCollection FromCoordsArray(params double[] coords)
        {
            var buf = new PointCollection();

            if (coords.Length%2 != 0)
                throw new InvalidOperationException();


            for (int i = 0; i < coords.Length; i+=2)
            {
                var pt = new Point(coords[i], coords[i + 1]);
                buf.Add(pt);
            }


            return buf;
        }

        public PointCollection(IEnumerable<Point> points)
            : base(points)
        {

        }

        public PointCollection(params Point[] points)
            : base(points)
        {

        }

        public PointCollection()
            : base()
        {

        }

        public PointCollection(params double[] coords)
            : base()
        {
            this.BulkAdd(coords);
        }

        /// <summary>
        /// Adds a points with specified <see cref="x"/> and <see cref="y"/> coordinations.
        /// </summary>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public void Add(double y, double z)
        {
            Add(new Point(y, z));
        }

        public void BulkAdd(params double[] coords)
        {
            if (coords.Length%2 != 0)
                throw new Exception();

            var q = new Queue<double>(coords);

            while (q.Any())
            {
                this.Add(q.Dequeue(), q.Dequeue());
            }
        }

        /// <summary>
        /// Determines whether this instance is clockwise.
        /// </summary>
        /// <returns><c>true</c> if polygon vertices are in clockwise order</returns>
        public bool IsClockwise()
        {
            return GetArea() < 0;
        }


        /// <summary>
        /// Gets the area of polygon.
        /// </summary>
        /// <returns>area of polygon</returns>
        public double GetArea()
        {
            var a = 0.0;

            for (var i = 0; i < Count - 1; i++)
            {
                var bi = this[i].Y * this[i + 1].Z - this[i].Z * this[i + 1].Y;
                a += bi;
            }

            return a/2;
        }


        public double[] GetGeometricalProperties()
        {
            var lastPoint = this[this.Count - 1];

            if (lastPoint != this[0])
                throw new InvalidOperationException("First point and last point ot PolygonYz should put on each other");



            double a = 0.0, iz = 0.0, iy = 0.0, ixy = 0.0;


            var x = new double[this.Count];
            var y = new double[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                x[i] = this[i].Y;
                y[i] = this[i].Z;
            }

            var l = this.Count - 1;

            var ai = 0.0;

            for (var i = 0; i < l; i++)
            {
                ai = x[i] * y[i + 1] - x[i + 1] * y[i];
                a += ai;
                iy += (y[i] * y[i] + y[i] * y[i + 1] + y[i + 1] * y[i + 1]) * ai;
                iz += (x[i] * x[i] + x[i] * x[i + 1] + x[i + 1] * x[i + 1]) * ai;

                ixy += (x[i] * y[i + 1] + 2 * x[i] * y[i] + 2 * x[i + 1] * y[i + 1] + x[i + 1] * y[i]) * ai;

            }

            a = a * 0.5;
            iz = iz * 1 / 12.0;
            iy = iy * 1 / 12.0;
            ixy = ixy * 1 / 24.0;
            var j = iy + iz;
            //not sure which one is correct j = ix + iy or j = ixy :)!

            var buf = new double[] { iz, iy, j, a, a, a };

            if (a < 0)
                for (var i = 0; i < 6; i++)
                    buf[i] = -buf[i];

            return buf;
        }

        public PointCollection Clone()
        {
            var buf = new PointCollection();

            foreach (var p in this)
            {
                buf.Add(p);
            }

            return buf;
        }

        public void Add(params Point[] pts)
        {
            this.AddRange(pts);
        }

        /// <summary>
        /// Gets the perimeter of this point collection.
        /// </summary>
        /// <returns>the length of perimeter</returns>
        public double GetPerimeter()
        {
            var buf = 0.0;

            for (int i = 0; i < this.Count-1; i++)
            {
                var ith = this[i];
                var i1th = this[i + 1];

                var dy = i1th.Y - ith.Y;
                var dz = i1th.Z - ith.Z;

                buf += Math.Sqrt(dy*dy + dz*dz);
            }

            return buf;
        }
    }
}