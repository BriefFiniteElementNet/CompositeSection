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
using System.Runtime.Serialization;
using System.Xml.Schema;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a point (fiber) element
    /// </summary>
    [Serializable]
    public class FiberElement : BaseElement
    {

        private bool _adnvancedCalculations;

        private Point _center;

        private double _area;


        public Point Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public double Area
        {
            get { return _area; }
            set { _area = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adnvanced calculations on fibers should be applied or not.
        /// </summary>
        /// <value>
        /// <c>true</c> if [adnvanced calculations]; otherwise, <c>false</c>.
        /// </value>
        public bool AdnvancedCalculations
        {
            get { return _adnvancedCalculations; }
            set { _adnvancedCalculations = value; }
        }

        /*
        #region geometric fields

        /// <summary>
        /// Determines wether geometrical fields should be recalculated or not.
        /// </summary>
        internal bool IsDirty()
        {
            return lastCenter.HasValue && _center.Equals(lastCenter.Value);
        }

        internal void CalculateGeometricProperties()
        {
            if (!IsDirty())
                return;

            throw new NotImplementedException();
        }

        private Point? lastCenter;

        /// <summary>
        /// First moment of area about X axis
        /// </summary>
        /// <remarks>
        /// 
        ///       /
        /// Qx=  |  y . dA
        ///     / A
        /// 
        /// </remarks>
        internal double Qx;

        /// <summary>
        /// First moment of area about Y axis
        /// </summary>
        /// <remarks>
        /// 
        ///       /
        /// Qy=  |  x . dA
        ///     / A
        /// 
        /// </remarks>
        internal double Qy;

        /// <summary>
        /// Second moment of area about X axis
        /// </summary>
        /// <remarks>
        /// 
        ///       /
        /// Ix=  |  y^2 . dA
        ///     / A
        /// 
        /// </remarks>
        internal double Ix;

        /// <summary>
        /// Second moment of area about Y axis
        /// </summary>
        /// <remarks>
        /// 
        ///       /
        /// Iy=  |  x^2 . dA
        ///     / A
        /// 
        /// </remarks>
        internal double Iy;

        /// <summary>
        /// Second moment of area about Y axis
        /// </summary>
        /// <remarks>
        /// 
        ///        /
        /// Ixy=  |  x . y . dA
        ///      / A
        /// 
        /// </remarks>
        internal double Ixy;

        /// <summary>
        /// Some sort of area moment!
        /// </summary>
        /// <remarks>
        /// 
        ///         /
        /// Ix2y=  |  x^2 . y . dA
        ///       / A
        /// 
        /// </remarks>
        internal double Ix2y;

        /// <summary>
        /// Some sort of area moment!
        /// </summary>
        /// <remarks>
        /// 
        ///         /
        /// Ixy2=  |  x . y^2 . dA
        ///       / A
        /// 
        /// </remarks>
        internal double Ixy2;

        /// <summary>
        /// Some sort of area moment!
        /// </summary>
        /// <remarks>
        /// 
        ///        /
        /// Ix3=  |  x^3 . dA
        ///      / A
        /// 
        /// </remarks>
        internal double Ix3;

        /// <summary>
        /// Some sort of area moment!
        /// </summary>
        /// <remarks>
        /// 
        ///         /
        /// Iy3=  |  y^3 . dA
        ///       / A
        ///  
        /// </remarks>
        internal double Iy3;

        #endregion
        */

        /// <inheritdoc />
        public override bool IsValidElement(out string message)
        {
            message = "";
            return true;
        }

        /// <inheritdoc />
        public override Force GetForce(StrainProfile strain)
        {
            //if (IsDirty())
            //    CalculateGeometricProperties();

            if (_adnvancedCalculations)
                return GetForceAdvanced(strain);

            var str = strain.GetStrainAt(this.Center);

            var sig = 0.0;

            if (!ForegroundMaterial.IsNullOrEmpty())
                sig += ForegroundMaterial.GetStress(str);

            if (!BackgroundMaterial.IsNullOrEmpty())
                sig -= BackgroundMaterial.GetStress(str);


            var buf = new Force();

            buf.Nx = sig*_area;
            buf.My = sig*_area*_center.Z;
            buf.Mz = sig*_area*_center.Y;

            return buf;
        }


        private Force GetForceAdvanced(StrainProfile strain)
        {
            var buf = new Force();

            if (!ForegroundMaterial.IsNullOrEmpty())
                buf = Force.Sum(buf, GetForceAdvanced(strain, ForegroundMaterial));

            if (!BackgroundMaterial.IsNullOrEmpty())
                buf = Force.Subtract(buf, GetForceAdvanced(strain, BackgroundMaterial));

            return buf;
        }

        private double GetAxialForceAdvanced(StrainProfile strain)
        {
            var buf = 0.0;

            if (!ForegroundMaterial.IsNullOrEmpty())
                buf += GetAxialForceAdvanced(strain, ForegroundMaterial);

            if (!BackgroundMaterial.IsNullOrEmpty())
                buf += GetAxialForceAdvanced(strain, BackgroundMaterial);

            return buf;
        }

        /// <summary>
        /// Gets the force with advanced calculations.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private Force GetForceAdvanced(StrainProfile strain,Material mat)
        {
            var er0 = mat.GetTangentElasticModulus(strain.GetStrainAt(this.Center));
            var sigmar0 = mat.GetStress(strain.GetStrainAt(this.Center));

            var a = er0 * strain.Ky;
            var b = er0 * strain.Kz;
            var c = sigmar0 - (er0*strain.Ky*Center.Y + er0*strain.Kz*Center.Z);

            var r = Math.Sqrt(this.Area / Math.PI);

            var yb = this.Center.Y;
            var zb = this.Center.Z;

            var i00 = _area;
            var i10 = yb * i00;
            var i01 = zb * i00;

            var i11 = yb * zb * i00;

            var i02 = i00 * (r * r / 4 + zb * zb);
            var i20 = i00 * (r * r / 4 + yb * yb);

            var buf = new Force();

            buf.Nx = a * i10 + b * i01 + c * i00;
            buf.Mz = a * i20 + b * i11 + c * i10;
            buf.My = a * i11 + b * i02 + c * i01;

            return buf;
        }

        private double GetAxialForceAdvanced(StrainProfile strain, Material mat)
        {
            var er0 = mat.GetTangentElasticModulus(strain.GetStrainAt(this.Center));
            var sigmar0 = mat.GetStress(strain.GetStrainAt(this.Center));

            var a = er0 * strain.Ky;
            var b = er0 * strain.Kz;
            var c = sigmar0 - (er0 * strain.Ky * Center.Y + er0 * strain.Kz * Center.Z);

            var yb = this.Center.Y;
            var zb = this.Center.Z;

            var i00 = _area;
            var i10 = yb * i00;
            var i01 = zb * i00;

            var nx = a * i10 + b * i01 + c * i00;

            return nx;
        }

        private Stiffness GetStiffnessAdvanced(StrainProfile strain)
        {
            var buf = new Stiffness();

            if (!ForegroundMaterial.IsNullOrEmpty())
                buf = Stiffness.Sum(buf, GetStiffnessAdvanced(strain, ForegroundMaterial));

            if (!BackgroundMaterial.IsNullOrEmpty())
                buf = Stiffness.Subtract(buf, GetStiffnessAdvanced(strain, BackgroundMaterial));

            return buf;
        }

        private Stiffness GetStiffnessAdvanced(StrainProfile strain,Material mat)
        {
            var er0 = mat.GetTangentElasticModulus(strain.GetStrainAt(this.Center));
            
            var r = Math.Sqrt(this.Area / Math.PI);

            var yb = this.Center.Y;
            var zb = this.Center.Z;

            var i00 = _area;
            var i10 = yb * i00;
            var i01 = zb * i00;

            var i11 = yb * zb * i00;

            var i02 = i00 * (r * r / 4 + zb*zb);
            var i20 = i00 * (r * r / 4 + yb*yb);

            var buf = new Stiffness();

            buf.RmyRky = er0 * i11;
            buf.RmyRkz = er0 * i02;
            buf.RmyRe0 = er0 * i01;
            
            buf.RmzRky = er0 * i20;
            buf.RmzRkz = er0 * i11;
            buf.RmzRe0 = er0 * i10;
            
            buf.RnxRky = er0 * i10;
            buf.RnxRkz = er0 * i01;
            buf.RnxRe0 = er0 * i00;

            return buf;
        }

        /// <inheritdoc />
        public override Stiffness GetStiffness(StrainProfile strain)
        {
            if (_adnvancedCalculations)
                return GetStiffnessAdvanced(strain);

            //if (IsDirty())
            //    CalculateGeometricProperties();

            var et = 0.0;

            var str = strain.GetStrainAt(this.Center);

            if (!ForegroundMaterial.IsNullOrEmpty())
                et += ForegroundMaterial.GetTangentElasticModulus(str);

            if (!BackgroundMaterial.IsNullOrEmpty())
                et -= BackgroundMaterial.GetTangentElasticModulus(str);

            var a = this.Area;
            var y = this.Center.Y;
            var z = this.Center.Z;

            var e = a*et;
            var ey = e*y;
            var ez = e*z;
            var ey2 = e*y*y;
            var ez2 = e*z*z;
            var eyz = e*y*z;

            var stiff = new Stiffness();

            stiff.RmyRky = eyz;
            stiff.RmyRkz = ez2;
            stiff.RmyRe0 = ez;

            stiff.RmzRky = ey2;
            stiff.RmzRkz = eyz;
            stiff.RmzRe0 = ey;

            stiff.RnxRky = ey;
            stiff.RnxRkz = ez;
            stiff.RnxRe0 = e;

            return stiff;
        }

        /// <inheritdoc />
        public override double GetAxialForce(StrainProfile strain)
        {
            if (_adnvancedCalculations)
                return GetAxialForceAdvanced(strain);

            var backNull = BackgroundMaterial is Materials.NullMaterial || BackgroundMaterial == null;
            var forNull = ForegroundMaterial is Materials.NullMaterial || ForegroundMaterial == null;

            if (backNull && forNull)
                return 0.0;

            var st = strain.GetStrainAt(_center);

            var bs = 0.0;

            if (BackgroundMaterial != null)
                bs = BackgroundMaterial.GetStress(st);


            var fs = 0.0;

            if (ForegroundMaterial != null)
                fs = ForegroundMaterial.GetStress(st);

            return (fs - bs) * _area;
        }

        /// <inheritdoc />
        public override BaseElement Clone()
        {
            var buf = (FiberElement) MemberwiseClone();

            if (this.ForegroundMaterial != null)
                buf.ForegroundMaterial = this.ForegroundMaterial.Clone();

            if (this.BackgroundMaterial != null)
                buf.BackgroundMaterial = this.BackgroundMaterial.Clone();

            return buf;
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_center", _center);
            info.AddValue("_adnvancedCalculations", _adnvancedCalculations);
            info.AddValue("_area", _area);
        }

        /// <inheritdoc />
        protected FiberElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _center = (Point)info.GetValue("_center", typeof(Point));
            _adnvancedCalculations = (bool)info.GetValue("_adnvancedCalculations", typeof(bool));
            _area = (double)info.GetValue("_area", typeof(double));
        }

        /// <inheritdoc />
        public FiberElement()
            : base()
        {
        }
    }
}
