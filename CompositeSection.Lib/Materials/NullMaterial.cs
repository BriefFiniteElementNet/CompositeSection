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

namespace CompositeSection.Lib.Materials
{
    /// <summary>
    /// Represents a null material, for every strain the stress is zero
    /// </summary>
    public class NullMaterial:Material
    {
        public NullMaterial():base()
        {
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected NullMaterial(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        /// <inheritdoc/>
        public override double GetStress(double strain)
        {
            return 0;
        }

        /// <inheritdoc/>
        public override double GetTangentElasticModulus(double strain)
        {
            return 0;
        }

        /// <inheritdoc/>
        public override double[] GetWalls()
        {
            return new double[] {};
        }

        /// <inheritdoc/>
        public double IntegrateStressObsolete(double y0, double y1, double alpha, double beta, double fi, double e0, int r, int s)
        {
            return 0;
        }

        /// <inheritdoc/>
        public double IntegrateTangentElasticModulusObsolete(double y0, double y1, double alpha, double beta, double fi, double e0, int r,
            int s)
        {
            return 0;
        }

        /// <inheritdoc/>
        public override double IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0, int r, int s)
        {
            return 0;
        }

        /// <inheritdoc/>
        public override double IntegrateTangentElasticModulus(double y0, double y1, double alpha, double beta, double fi, double e0,
            int r, int s)
        {
            return 0;
        }

        /// <inheritdoc/>
        public override Material Clone()
        {
            var buf = new NullMaterial();

            buf.PositiveFailureStrain = this.PositiveFailureStrain;
            buf.NegativeFailureStrain = this.NegativeFailureStrain;

            return buf;
        }
    }
}
