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

#if PORTABLE
using CompositeSection.Lib.SerializationMocks;
#else
using System.Runtime.Serialization;
#endif

namespace CompositeSection.Lib
{
    /// <summary>
    /// The only feature needed from materials is stress strain curve so this represents an interface for a general uni-axial stress strain curve
    /// </summary>
    [Serializable]
    public abstract class Material: ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Material"/> class.
        /// </summary>
        protected Material()
        {
        }

        private string _label;

        /// <summary>
        /// Gets the stress in defined <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        public abstract double GetStress(double strain);

        /// <summary>
        /// Gets the tangent elastic modulus at specified <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns></returns>
        public abstract double GetTangentElasticModulus(double strain);

        /// <summary>
        /// Gets the strains of edges of different regions in stress strain curve, sorted from small one to large one.
        /// </summary>
        /// <returns>The walls of regions</returns>
        public abstract double[] GetWalls();

        /// <summary>
        /// Calculates the:
        /// 
        ///    /  z₁
        ///   |
        ///   |  (α.z + β)ʳ zˢ σ(ε₀ + ϕ.z) dz
        ///   |
        ///  /  z₀
        /// 
        /// </summary>
        /// <param name="z0">The z₀</param>
        /// <param name="z1">The z₁</param>
        /// <param name="alpha">The α</param>
        /// <param name="beta">The β</param>
        /// <param name="fi">The ϕ</param>
        /// <param name="e0">The ε₀</param>
        /// <param name="r">The r</param>
        /// <param name="s">The s</param>
        /// <returns>Integration result regarding input parameters</returns>
        public abstract double 
            IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0, int r, int s);

        /// <summary>
        /// Calculates the:
        ///
        ///    /  z₁
        ///   |
        ///   |  (α.z + β)ʳ zˢ Et(ε₀ + ϕ.z) dz
        ///   |
        ///  /  z₀
        /// 
        /// Where Et is tangent modulus
        /// </summary>
        /// <param name="y0">The z₀</param>
        /// <param name="y1">The z₁</param>
        /// <param name="alpha">The α</param>
        /// <param name="beta">The β</param>
        /// <param name="fi">The ϕ</param>
        /// <param name="e0">The ε₀</param>
        /// <param name="r">The r</param>
        /// <param name="s">The s</param>
        /// <returns>Integration result regarding input parameters</returns>
        public abstract double IntegrateTangentElasticModulus(double y0, double y1, double alpha, double beta, double fi, double e0,
            int r, int s);

        /// <summary>
        /// Gets or sets the positive failure strain.
        /// </summary>
        /// <value>
        /// The positive failure strain threshold.
        /// If such a strain doesn't exists, set to null.
        /// </value>
        public double? PositiveFailureStrain
        {
            set { _positiveFailureStrain = value; }
            get { return _positiveFailureStrain; }
        }

        /// <summary>
        /// Gets or sets the negative failure strain.
        /// </summary>
        /// <value>
        /// The negative failure strain threshold.
        /// If such a strain doesn't exists, set to null.
        /// </value>
        public double? NegativeFailureStrain
        {
            set { _negativeFailureStrain = value; }
            get { return _negativeFailureStrain; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        private double? _positiveFailureStrain;
        private double? _negativeFailureStrain;

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone instance of this material</returns>
        public abstract Material Clone();

        /// <inheritdoc/>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_positiveFailureStrain", _positiveFailureStrain);
            info.AddValue("_negativeFailureStrain", _negativeFailureStrain);
            info.AddValue("_label", _label);
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected Material(SerializationInfo info, StreamingContext context)
        {
            _positiveFailureStrain = (double?)info.GetValue("_positiveFailureStrain", typeof(double?));
            _negativeFailureStrain = (double?)info.GetValue("_negativeFailureStrain", typeof(double?));
            _label = (string)info.GetValue("_label", typeof(string));
        }
    }
}