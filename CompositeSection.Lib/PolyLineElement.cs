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
using System.Linq;
using System.Runtime.Serialization;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a polyline lement
    /// </summary>
    [Serializable]
    public class PolyLineElement : BaseElement
    {
        private PointCollection _points;

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public PointCollection Points
        {
            get { return _points; }
            set { _points = value; }
        }

        /// <summary>
        /// Gets or sets the thickness.
        /// </summary>
        /// <value>
        /// The thickness of element in [m] dimension
        /// </value>
        public double Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        private double _thickness;

        /// <inheritdoc />
        public override bool IsValidElement(out string message)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Force GetForce(StrainProfile strain)
        {
            if (this._points == null)
                throw new Exception();

            if (this._points.Count < 2)
                throw new Exception();

            Point[] ptsp;


            bool isConstant = strain.Ky.Equals(0.0) && strain.Kz.Equals(0.0);

            var r = Math.Sqrt(strain.Kz*strain.Kz + strain.Ky*strain.Ky);

            var cos = strain.Kz/r;
            var sin = -strain.Ky/r;

            if (isConstant)
            {
                cos = 1.0;
                sin = 0.0;
                r = 0;
            }

            if (isConstant)
            {
                ptsp = _points.ToArray();
            }
            else
            {
                ptsp = new Point[_points.Count];

                for (var i = 0; i < _points.Count; i++)
                {
                    var pt = _points[i];
                    var ptp = new Point();

                    ptp.Y = cos*pt.Y + sin*pt.Z;
                    ptp.Z = -sin*pt.Y + cos*pt.Z;

                    ptsp[i] = ptp;
                }
            }

            var frc = GetForce(r, strain.E0, ptsp);

            var buf = new Force();

            if (isConstant)
            {
                buf = frc;
            }
            else
            {
                buf.My = cos*frc.My + sin*frc.Mz;
                buf.Mz = -sin*frc.My + cos*frc.Mz;
                buf.Nx = frc.Nx;
            }

            //buf.Nx *= _thickness;
            //buf.My *= _thickness;
            //buf.Mz *= _thickness;

            return buf;
        }


        private Force GetForce(double r, double e0, Point[] points)
        {
            var ff = GetForce(r, e0, points, this.ForegroundMaterial); //foreground force
            var bf = GetForce(r, e0, points, this.BackgroundMaterial); //background force

            return Force.Subtract(ff, bf);
        }

        /// <summary>
        /// Gets the force!
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="e0">The e0.</param>
        /// <param name="points">The points.</param>
        /// <param name="ssc">The SSC.</param>
        /// <returns></returns>
        private Force GetForce(double r, double e0, Point[] points, Material ssc)
        {
            if (ssc.IsNullOrEmpty())
                return Force.Zero;

            var ff = new Force();

            var eps = ssc.GetWalls();

            double[] zs;


            if (r.Equals(0.0))
            {
                zs = new[] {double.NegativeInfinity, double.PositiveInfinity};
            }
            else
            {
                zs = new double[eps.Length + 2];

                for (var i = 0; i < eps.Length; i++)
                    zs[i + 1] = (eps[i] - e0)/r;

                zs[0] = double.NegativeInfinity;
                zs[zs.Length - 1] = double.PositiveInfinity;
            }


            for (var i = 0; i < points.Length - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];

                if (p1.Z > p2.Z)
                {
                    p1 = points[i + 1];
                    p2 = points[i];
                }


                var force = new Force();

                if (p1.Z.Equals(p2.Z))
                {
                    //اینجا باید درست هندل بشه
                    //و نباید دو نقطه با زد مساوی به ادامه راه پیدا کنه


                    var l = Math.Abs(p2.Y - p1.Y);
                    var cy = 0.5*(p2.Y + p1.Y);
                    var cz = p1.Z; //equals p2.Z
                    var t = _thickness;
                    var strain = e0 + r*cz;
                    var stress = ssc.GetStress(strain);

                    var a = t*l;

                    force.Nx = a*stress;
                    force.My = a*stress*cz;
                    force.Mz = a*stress*cy;

                    goto done;
                }

                

                #region integrating force

                var zMin = p1.Z;
                var zMax = p2.Z;


                var areInSameRegion = false;

                #region determining whether both line points are in same region

                for (var k = 0; k < zs.Length - 1; k++)
                {
                    if (zMin >= zs[k] && zMin < zs[k + 1])
                    {
                        areInSameRegion = zMax >= zs[k] && zMax <= zs[k + 1];
                        break;
                    }
                }

                #endregion

                #region integrating over this line

                var aa = (p2.Z - p1.Z)/(p1.Y*p2.Z - p2.Y*p1.Z);
                var bb = (p1.Y - p2.Y)/(p1.Y*p2.Z - p2.Y*p1.Z);
                //this line's equation is: aa * y + bb * z = 1

                if (areInSameRegion)
                {
                    force = Force.Sum(GetSliceForce(p1, p2, ssc, r, e0), force);
                }
                else
                {
                    var s = 0;
                    var e = 0;

                    #region determine s and e

                    for (var k = 0; k < zs.Length; k++)
                    {
                        if (zs[k] >= zMin)
                        {
                            s = k;
                            break;
                        }
                    }

                    for (var k = zs.Length - 1; k >= 0; k--)
                    {
                        if (zs[k] < zMax)
                        {
                            e = k;
                            break;
                        }
                    }

                    #endregion

                    {
                        //from p1.z to zs[s]

                        var p1p = new Point(); //new p1 that should be passed to GetSliceForce method
                        var p2p = new Point(); //new p2 that should be passed to GetSliceForce method

                        p1p = p1;
                        p2p.Z = zs[s];
                        p2p.Y = (1.0 - bb*p2p.Z)/aa;

                        force = Force.Sum(GetSliceForce(p1p, p2p, ssc, r, e0), force);
                    }

                    for (var k = s; k < e; k++)
                    {
                        //from zs[k] to zs[k+1]

                        var p1p = new Point(); //new p1 that should be passed to GetSliceForce method
                        var p2p = new Point(); //new p2 that should be passed to GetSliceForce method

                        p1p.Z = zs[k];
                        p2p.Z = zs[k + 1];

                        p1p.Y = (1.0 - bb*p1p.Z)/aa;
                        p2p.Y = (1.0 - bb*p2p.Z)/aa;

                        force = Force.Sum(GetSliceForce(p1p, p2p, ssc, r, e0), force);
                    }

                    {
                        //from zs[e] to zmax

                        var p1p = new Point(); //new p1 that should be passed to GetSliceForce method
                        var p2p = new Point(); //new p2 that should be passed to GetSliceForce method

                        p1p.Z = zs[e];
                        p2p = p2;

                        p1p.Y = (1.0 - bb*p1p.Z)/aa;

                        force = Force.Sum(GetSliceForce(p1p, p2p, ssc, r, e0), force);
                    }
                }

                #endregion

                #endregion

                done:

                ff = Force.Sum(ff, force);
            }

            return ff;
        }


        /// <summary>
        /// Gets the force of a line element with same thickness as current instance with start location <see cref="p1"/> and end location <see cref="p2"/> is 
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="mat">The mat.</param>
        /// <param name="r">The r (strain slope).</param>
        /// <param name="e0">The e0 (strain at origins).</param>
        /// <returns></returns>
        private Force GetSliceForce(Point p1, Point p2, Material mat, double r, double e0)
        {
            if (p1.Z > p2.Z)
                throw new Exception();

            var dy = p2.Y - p1.Y;
            var dz = p2.Z - p1.Z;

            var alfa = (p1.Y - p2.Y)/(p1.Z - p2.Z);
            var beta = (p2.Y*p1.Z - p1.Y*p2.Z)/(p1.Z - p2.Z);

            var l = Math.Sqrt(dy*dy + dz*dz);
            var sc = this._thickness*l/Math.Abs(dz);

            


            var buf = new Force();

            buf.Nx = sc*mat.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, 0, 0);
            buf.My = sc*mat.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, 0, 1);
            buf.Mz = sc*mat.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, 1, 0);

            if (Math.Abs(dz) < 1e-8 * Math.Abs(dy))
            {
                //where dz << dy then alfa and beta increase insanely and it causes rounding errors.
                //to overcome this, in cases that dz << dy (or line is almost parallel to y axis) we assume constant stress along line
                //and the compute Mz with that assumption. Not that only Mz should be corrected, because only in Mz our 'r' is not zero (remember (alpha*z+beta)^r).

                //var strn = r * p1.Z + e0;
                var f = mat.GetStress(r*p1.Z + e0)*l*_thickness;
                buf.Mz = (p1.Y + dy/2)*f;

                //Guid.NewGuid();
            }

            return buf;
        }

        /// <inheritdoc />
        public override Stiffness GetStiffness(StrainProfile strain)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override double GetAxialForce(StrainProfile strain)
        {
            return GetForce(strain).Nx;
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override BaseElement Clone()
        {
            var buf = new PolyLineElement();

            if (this.ForegroundMaterial != null)
                buf.ForegroundMaterial = this.ForegroundMaterial.Clone();

            if (this.BackgroundMaterial != null)
                buf.BackgroundMaterial = this.BackgroundMaterial.Clone();

            throw new NotImplementedException();
            return buf;
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_points", _points);
            info.AddValue("_thickness", _thickness);
        }

        /// <inheritdoc />
        protected PolyLineElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _points = (PointCollection)info.GetValue("_points", typeof(PointCollection));
            _thickness = (double)info.GetValue("_thickness", typeof(double));
        }

        /// <inheritdoc />
        public PolyLineElement()
            : base()
        {
        }

        public PolyLineElement(PointCollection points)
            : base()
        {
            _points = points;
        }
    }
    
}