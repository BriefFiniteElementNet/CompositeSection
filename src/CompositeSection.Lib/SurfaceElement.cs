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
    /// Represents a surface element which is in polygonal form.
    /// </summary>
    [Serializable]
    public class SurfaceElement : BaseElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceElement"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public SurfaceElement(PointCollection points):base()
        {
            _points = points;
        }


        /// <inheritdoc />
        public override bool IsValidElement(out string message)
        {
            if (_points == null)
                throw new Exception("points");

            if (_points.Count < 3)
            {
                message = "points must be more than 3";
                return false;
            }

            if (!_points.Last().Equals(_points.First()))
            {
                message = "first and last points must be same";
                return false;
            }

            if (_points.IsClockwise())
            {
                message = "points must be in counter clockwise order";
                return false;
            }

            message = "";
            return true;
        }

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

        /// <inheritdoc />
        public override Force GetForce(StrainProfile strain)
        {
            if (this._points == null)
                throw new Exception();

            if (this._points.Count < 3)
                throw new Exception();

            if (this._points.First() != this._points.Last())
                throw new Exception("start and end points must be same");

            Point[] ptsp;


            bool isConstant = strain.Ky.Equals(0.0) && strain.Kz.Equals(0.0);

            var r = Math.Sqrt(strain.Kz * strain.Kz + strain.Ky * strain.Ky);

            var cos = strain.Kz / r;
            var sin = -strain.Ky / r;

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

                    ptp.Y = cos * pt.Y + sin * pt.Z;
                    ptp.Z = -sin * pt.Y + cos * pt.Z;

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
                buf.My = cos * frc.My + sin * frc.Mz;
                buf.Mz = -sin * frc.My + cos * frc.Mz;
                buf.Nx = frc.Nx;
            }

            return buf;
        }

        private Force GetForce(double r, double e0, Point[] points)
        {
            var ff = GetForce(r, e0, points, this.ForegroundMaterial); //foreground force
            var bf = GetForce(r, e0, points, this.BackgroundMaterial); //background force

            return Force.Subtract(ff, bf);
        }

        private Force GetForce(double r, double e0, Point[] points, Material ssc)
        {
            if (ssc.IsNullOrEmpty())
                return Force.Zero;

            var ff = new Force();

            var eps = ssc.GetWalls();

            double[] zs;


            if (r == 0)
            {
                zs = new[] { double.NegativeInfinity, double.PositiveInfinity };
            }
            else
            {
                zs = new double[eps.Length + 2];

                for (var i = 0; i < eps.Length; i++)
                    zs[i + 1] = (eps[i] - e0) / r;

                zs[0] = double.NegativeInfinity;
                zs[zs.Length - 1] = double.PositiveInfinity;
            }

            for (var i = 0; i < points.Length - 1; i++)
            {
                var isReverse = false;

                var p1 = points[i];
                var p2 = points[i + 1];

                if (p1.Z > p2.Z)
                {
                    isReverse = true;
                    p1 = points[i + 1];
                    p2 = points[i];
                }
                else
                {
                    if (p1.Z.Equals(p2.Z))
                        continue;
                }

                var alfa = (p1.Y - p2.Y) / (p1.Z - p2.Z);
                var beta = (p2.Y * p1.Z - p1.Y * p2.Z) / (p1.Z - p2.Z);

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

                var force = new Force();


                if (areInSameRegion)
                {
                    force.My += ssc.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, r: 0+1, s: 1);
                    force.Mz += 0.5 * ssc.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, r: 1+1, s: 0);
                    force.Nx += ssc.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, r: 0+1, s: 0);
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

                    force.My += ssc.IntegrateStress(p1.Z, zs[s], alfa, beta, r, e0, r: 0+1, s: 1);
                    force.Mz += 0.5 * ssc.IntegrateStress(p1.Z, zs[s], alfa, beta, r, e0, r: 1+1, s: 0);
                    force.Nx += ssc.IntegrateStress(p1.Z, zs[s], alfa, beta, r, e0, r: 0+1, s: 0);

                    for (var k = s; k < e; k++)
                    {
                        force.My += ssc.IntegrateStress(zs[k], zs[k + 1], alfa, beta, r, e0, r: 0+1, s: 1);
                        force.Mz += 0.5 * ssc.IntegrateStress(zs[k], zs[k + 1], alfa, beta, r, e0, r: 1+1, s: 0);
                        force.Nx += ssc.IntegrateStress(zs[k], zs[k + 1], alfa, beta, r, e0, r: 0+1, s: 0);
                    }

                    force.My += ssc.IntegrateStress(zs[e], zMax, alfa, beta, r, e0, r: 0+1, s: 1);
                    force.Mz += 0.5 * ssc.IntegrateStress(zs[e], zMax, alfa, beta, r, e0, r: 1+1, s: 0);
                    force.Nx += ssc.IntegrateStress(zs[e], zMax, alfa, beta, r, e0, r: 0+1, s: 0);
                }

                #endregion

                #endregion

                if (isReverse)
                    ff = Force.Subtract(ff, force);
                else
                    ff = Force.Sum(ff, force);
            }

            return ff;
        }

        private Stiffness GetStiffness(double r, double e0, Point[] points)
        {
            var ff = GetStiffness(r, e0, points, this.ForegroundMaterial); //foreground force
            var bf = GetStiffness(r, e0, points, this.BackgroundMaterial); //background force

            return Stiffness.Subtract(ff, bf);
        }


        /// <inheritdoc />
        public override Stiffness GetStiffness(StrainProfile strain)
        {
            if (this._points == null)
                throw new Exception();

            if (this._points.Count < 3)
                throw new Exception();

            if (this._points.First() != this._points.Last())
                throw new Exception();

            Point[] ptsp;

            var ff = new Stiffness();

            bool isConstant = strain.Ky.Equals(0.0) && strain.Kz.Equals(0.0);

            var r = Math.Sqrt(strain.Kz * strain.Kz + strain.Ky * strain.Ky);

            var cos = strain.Kz / r;
            var sin = -strain.Ky / r;

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

                    ptp.Y = cos * pt.Y + sin * pt.Z;
                    ptp.Z = -sin * pt.Y + cos * pt.Z;

                    ptsp[i] = ptp;
                }
            }

            var sff = GetStiffness(r, strain.E0, ptsp);

            var buf = new Stiffness();

            if (sin == 0 && cos == 1)
            {
                buf = sff;
            }
            else
            {
                var co = cos;
                var si = sin;

                buf.RmzRky =
                    co * (-sff.RmyRky * si + sff.RmzRky * co) - si * (-sff.RmyRkz * si + sff.RmzRkz * co);


                buf.RmzRkz =
                    co * (-sff.RmyRkz * si + sff.RmzRkz * co) + si * (-sff.RmyRky * si + sff.RmzRky * co);


                buf.RmzRe0 =
                    -sff.RmyRe0 * si + sff.RmzRe0 * co;


                buf.RmyRky =
                    co * (sff.RmyRky * co + sff.RmzRky * si) - si * (sff.RmyRkz * co + sff.RmzRkz * si);


                buf.RmyRkz =
                    co * (sff.RmyRkz * co + sff.RmzRkz * si) + si * (sff.RmyRky * co + sff.RmzRky * si);


                buf.RmyRe0 =
                    sff.RmyRe0 * co + sff.RmzRe0 * si;


                buf.RnxRky =
                    sff.RnxRky * co - sff.RnxRkz * si;


                buf.RnxRkz =
                    sff.RnxRky * si + sff.RnxRkz * co;


                buf.RnxRe0 =
                    sff.RnxRe0;
            }

            return buf;
        }

        private Stiffness GetStiffness(double r, double e0, Point[] points, Material stressStrainCurve)    
        {
            if (stressStrainCurve.IsNullOrEmpty())
                return Stiffness.Zero;

            var ff = new Stiffness();

            var eps = stressStrainCurve.GetWalls();

            double[] zs;


            if (r == 0)
            {
                zs = new[] { double.NegativeInfinity, double.PositiveInfinity };
            }
            else
            {
                zs = new double[eps.Length + 2];

                for (var i = 0; i < eps.Length; i++)
                    zs[i + 1] = (eps[i] - e0) / r;

                zs[0] = double.NegativeInfinity;
                zs[zs.Length - 1] = double.PositiveInfinity;
            }

            for (var i = 0; i < points.Length - 1; i++)
            {
                var isReverse = false;

                var p1 = points[i];
                var p2 = points[i + 1];

                if (p1.Z > p2.Z)
                {
                    isReverse = true;
                    p1 = points[i + 1];
                    p2 = points[i];
                }
                else
                {
                    if (p1.Z.Equals(p2.Z))
                        continue;
                }

                var alfa = (p1.Y - p2.Y) / (p1.Z - p2.Z);
                var beta = (p2.Y * p1.Z - p1.Y * p2.Z) / (p1.Z - p2.Z);

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

                var e = 0.0;    //r,s = 1,0
                var ey = 0.0;   //r,s = 2,0
                var ez = 0.0;   //r,s = 1,1
                var eyz = 0.0;  //r,s = 2,1
                var ey2 = 0.0;  //r,s = 3,0
                var ez2 = 0.0;  //r,s = 1,2
                

                if (areInSameRegion)
                {
                    e +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 1, s: 0);
                    ey +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 2, s: 0);
                    ez +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 1, s: 1);
                    eyz +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 2, s: 1);
                    ey2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 3, s: 0);
                    ez2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, p2.Z, alfa, beta, r, e0, r: 1, s: 2);
                }
                else
                {
                    var st = 0;
                    var en = 0;

                    #region determine s and e

                    for (var k = 0; k < zs.Length; k++)
                    {
                        if (zs[k] >= zMin)
                        {
                            st = k;
                            break;
                        }
                    }

                    for (var k = zs.Length - 1; k >= 0; k--)
                    {
                        if (zs[k] < zMax)
                        {
                            en = k;
                            break;
                        }
                    }

                    #endregion

                    e +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 1, s: 0);
                    ey +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 2, s: 0);
                    ez +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 1, s: 1);
                    eyz +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 2, s: 1);
                    ey2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 3, s: 0);
                    ez2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(p1.Z, zs[st], alfa, beta, r, e0, r: 1, s: 2);

                    for (var k = st; k < en; k++)
                    {
                        e += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 1, s: 0);

                        ey += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 2, s: 0);

                        ez += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 1, s: 1);
                        
                        eyz += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 2, s: 1);

                        ey2 += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 3, s: 0);
                        
                        ez2 += stressStrainCurve.IntegrateTangentElasticModulus(zs[k], zs[k + 1],
                            alfa, beta, r, e0, r: 1, s: 2);
                    }

                    e +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 1, s: 0);
                    ey +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 2, s: 0);
                    ez +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 1, s: 1);
                    eyz +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 2, s: 1);
                    ey2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 3, s: 0);
                    ez2 +=
                        stressStrainCurve.IntegrateTangentElasticModulus(zs[en], zMax, alfa, beta, r, e0, r: 1, s: 2);
                }

                #endregion

                //e *= 1.0;//r,s = 1,0
                ey *= 0.5;//r,s = 2,0
                //ez *= 1.0;//r,s = 1,1
                eyz *= 0.5;//r,s = 2,1
                ey2 *= 1.0/3.0;//r,s = 3,0
                //ez2 *= 1.0;//r,s = 1,2
                

                #endregion

                var stiff = new Stiffness();

                stiff.RmyRe0 = ez;
                stiff.RmyRky = eyz;
                stiff.RmyRkz = ez2;

                stiff.RmzRe0 = ey;
                stiff.RmzRky = ey2;
                stiff.RmzRkz = eyz;

                stiff.RnxRe0 = e;
                stiff.RnxRky = ey;
                stiff.RnxRkz = ez;

                if (isReverse)
                    ff = Stiffness.Subtract(ff, stiff);
                else
                    ff = Stiffness.Sum(ff, stiff);
            }

            return ff;
        }

        /// <inheritdoc />
        public override double GetAxialForce(StrainProfile strain)
        {
            if (this._points == null)
                throw new Exception();

            if (this._points.Count < 3)
                throw new Exception();

            if (this._points.First() != this._points.Last())
                throw new Exception();

            Point[] ptsp;


            bool isConstant = strain.Ky.Equals(0.0) && strain.Kz.Equals(0.0);

            var r = Math.Sqrt(strain.Kz * strain.Kz + strain.Ky * strain.Ky);

            var cos = strain.Kz / r;
            var sin = -strain.Ky / r;

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

                    ptp.Y = cos * pt.Y + sin * pt.Z;
                    ptp.Z = -sin * pt.Y + cos * pt.Z;

                    ptsp[i] = ptp;
                }
            }

            var frc = GetAxialForce(r, strain.E0, ptsp);

            return frc;
        }

        /// <inheritdoc />
        private double GetAxialForce(double r, double e0, Point[] points)
        {
            var ff = GetAxialForce(r, e0, points, this.ForegroundMaterial); //foreground force
            var bf = GetAxialForce(r, e0, points, this.BackgroundMaterial); //background force

            return ff - bf;
        }

        /// <inheritdoc />
        private double GetAxialForce(double r, double e0, Point[] points, Material ssc)
        {
            if (ssc.IsNullOrEmpty())
                return 0.0;

            var buf = 0.0;

            var eps = ssc.GetWalls();

            double[] zs;


            if (r == 0)
            {
                zs = new[] { double.NegativeInfinity, double.PositiveInfinity };
            }
            else
            {
                zs = new double[eps.Length + 2];

                for (var i = 0; i < eps.Length; i++)
                    zs[i + 1] = (eps[i] - e0) / r;

                zs[0] = double.NegativeInfinity;
                zs[zs.Length - 1] = double.PositiveInfinity;
            }

            for (var i = 0; i < points.Length - 1; i++)
            {
                var isReverse = false;

                var p1 = points[i];
                var p2 = points[i + 1];

                if (p1.Z > p2.Z)
                {
                    isReverse = true;
                    p1 = points[i + 1];
                    p2 = points[i];
                }
                else
                {
                    if (p1.Z.Equals(p2.Z))
                        continue;
                }

                var alfa = (p1.Y - p2.Y) / (p1.Z - p2.Z);
                var beta = (p2.Y * p1.Z - p1.Y * p2.Z) / (p1.Z - p2.Z);

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

                //var force = new Force();
                var fn = 0.0;

                if (areInSameRegion)
                {
                    fn += ssc.IntegrateStress(p1.Z, p2.Z, alfa, beta, r, e0, r: 0 + 1, s: 0);
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

                    fn += ssc.IntegrateStress(p1.Z, zs[s], alfa, beta, r, e0, r: 0 + 1, s: 0);

                    for (var k = s; k < e; k++)
                    {
                        fn += ssc.IntegrateStress(zs[k], zs[k + 1], alfa, beta, r, e0, r: 0 + 1, s: 0);
                    }

                    fn += ssc.IntegrateStress(zs[e], zMax, alfa, beta, r, e0, r: 0 + 1, s: 0);
                }

                #endregion

                #endregion

                if (isReverse)
                    buf -= fn; //Force.Subtract(ff, force);
                else
                    buf += fn;//Force.Sum(ff, force);
            }

            return buf;
        }

        public override BaseElement Clone()
        {
            var buf = new SurfaceElement();

            if (this.ForegroundMaterial != null)
                buf.ForegroundMaterial = this.ForegroundMaterial.Clone();

            if (this.BackgroundMaterial != null)
                buf.BackgroundMaterial = this.BackgroundMaterial.Clone();

            if (this._points != null)
                buf._points = this._points.Clone();


            return buf;
        }

                /// <inheritdoc />
               /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_points", _points);
        }

        /// <inheritdoc />
        protected SurfaceElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _points = (PointCollection)info.GetValue("_points", typeof(PointCollection));
        }

        /// <inheritdoc />
        public SurfaceElement()
            : base()
        {
            new PointCollection();
        }
    }
}