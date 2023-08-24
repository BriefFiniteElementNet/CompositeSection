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
using System.Runtime.Serialization;
using System.Text;
using CompositeSection.Lib;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents an absolutely elastic material with a specific elastic modulus.
    /// </summary>
    [Obsolete("Not implemented fully yet")]
    public class ElasticMaterial : Material
    {
        #region Serialization Stuff

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            _es = (double)info.GetValue("_es", typeof(double));
        }

        /// <inheritdoc/>
        protected ElasticMaterial(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_es", _es);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticMatterial"/> class.
        /// </summary>
        public ElasticMaterial() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticMatterial"/> class.
        /// </summary>
        /// <param name="es">The es.</param>
        public ElasticMaterial(double es) : base()
        {
            _es = es;
        }

        #region fields

        /// <summary>
        /// The elastic modulus of material
        /// </summary>
        private double _es;

        /// <summary>
        /// The elastic modulus of material
        /// </summary>
        public double Es
        {
            get { return _es; }
            set { _es = value; }
        }

        #endregion

        #region IStressStrainCurve methods

        /// <inheritdoc/>
        public override double GetStress(double strain)
        {
            return strain * _es;
        }

        /// <inheritdoc/>
        public override double GetTangentElasticModulus(double strain)
        {
            return _es;
        }

        /// <inheritdoc/>
        public double IntegrateStressObsolete(double y0, double y1, double alpha, double beta, double fi, double e0,
            int r,
            int s)
        {
            if (r + s > 2)
                throw new InvalidOperationException();

            var sum = 0.5 * (y1 + y0);
            var neg = 0.5 * (y1 - y0);


            const double k1 =
                -0.774596669241483377035853079956479922166584341058318165317514753222696618387395806703857475371734703583260441372189929402637908087832729923135978349224240702213750958202698716256783906245777858513169283405612501838634682531972963691092925710263188052523534528101729260090115562126394576188;
            const double k2 = 0;
            const double k3 = -k1;


            const double w1 = 5.0 / 9.0;
            const double w2 = 8.0 / 9.0;
            const double w3 = 5.0 / 9.0;

            var z11 = sum + neg * k1;
            var z22 = sum + neg * k2;
            var z33 = sum + neg * k3;

            var y1r1 = alpha * z11 + beta; //x1^(r+1)
            var y2r1 = alpha * z22 + beta; //x2^(r+1)
            var y3r1 = alpha * z33 + beta; //x3^(r+1)

            var z1s = 1.0; //y1^s
            var z2s = 1.0; //y2^s
            var z3s = 1.0; //y3^s


            if (r == 1)
            {
                y1r1 *= y1r1;
                y2r1 *= y2r1;
                y3r1 *= y3r1;
            }

            if (s == 1)
            {
                z1s = z11;
                z2s = z22;
                z3s = z33;
            }

            var f1 = y1r1 * z1s * GetStress(fi * z11 + e0);
            var f2 = y2r1 * z2s * GetStress(fi * z22 + e0);
            var f3 = y3r1 * z3s * GetStress(fi * z33 + e0);


            var buf = 1.0 / (r + 1.0) * neg * (w1 * f1 + w2 * f2 + w3 * f3);

            return buf;
        }

        public override double IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0,
            int r, int s)
        {
            if (r + s > 3)
                throw new InvalidOperationException();

            var sum = 0.5 * (z1 + z0);
            var neg = 0.5 * (z1 - z0);


            const double k1 =
                -0.77459666924148337703585307995647992216658434105831816531751475322269661838739580;
            const double k2 = 0;
            const double k3 = -k1;


            const double w1 = 5.0 / 9.0;
            const double w2 = 8.0 / 9.0;
            const double w3 = 5.0 / 9.0;

            var z11 = sum + neg * k1;
            var z22 = sum + neg * k2;
            var z33 = sum + neg * k3;

            var y1r = 1.0;//y1^(r+1)
            var y2r = 1.0;//y2^(r+1)
            var y3r = 1.0;//y3^(r+1)

            var z1s = 1.0;//z1^s
            var z2s = 1.0;//z2^s
            var z3s = 1.0;//z3^s

            if (r >= 1)
            {
                y1r = alpha * z11 + beta;
                y2r = alpha * z22 + beta;
                y3r = alpha * z33 + beta;
            }

            if (r == 2)
            {
                y1r *= y1r;
                y2r *= y2r;
                y3r *= y3r;
            }

            if (s == 1)
            {
                z1s = z11;
                z2s = z22;
                z3s = z33;
            }

            var f1 = y1r * z1s * GetStress(fi * z11 + e0);
            var f2 = y2r * z2s * GetStress(fi * z22 + e0);
            var f3 = y3r * z3s * GetStress(fi * z33 + e0);

            var buf = neg * (w1 * f1 + w2 * f2 + w3 * f3);

            return buf;
        }

        /// <inheritdoc/>
        public double IntegrateTangentElasticModulusObsolete(double y0, double y1, double alpha, double beta, double fi,
            double e0, int r, int s)
        {
            if (r + s > 3)
                throw new InvalidOperationException();


            var sum = 0.5 * (y1 + y0);
            var neg = 0.5 * (y1 - y0);


            const double k1 =
                -0.774596669241483377035853079956479922166584341058318165317514753222696618387395806703857475371734703583260441372189929402637908087832729923135978349224240702213750958202698716256783906245777858513169283405612501838634682531972963691092925710263188052523534528101729260090115562126394576188;
            const double k2 = 0;
            const double k3 = -k1;


            const double w1 = 5.0 / 9.0;
            const double w2 = 8.0 / 9.0;
            const double w3 = 5.0 / 9.0;

            var y11 = sum + neg * k1;
            var y22 = sum + neg * k2;
            var y33 = sum + neg * k3;

            var x1r1 = alpha * y11 + beta; //x1^(r+1)
            var x2r1 = alpha * y22 + beta; //x2^(r+1)
            var x3r1 = alpha * y33 + beta; //x3^(r+1)

            var y1s = 1.0; //y1^s
            var y2s = 1.0; //y2^s
            var y3s = 1.0; //y3^s


            if (r == 1)
            {
                x1r1 *= x1r1;
                x2r1 *= x2r1;
                x3r1 *= x3r1;
            }

            if (r == 2)
            {
                x1r1 *= x1r1 * x1r1;
                x2r1 *= x2r1 * x2r1;
                x3r1 *= x3r1 * x3r1;
            }


            if (s == 1)
            {
                y1s = y11;
                y2s = y22;
                y3s = y33;
            }

            if (s == 2)
            {
                y1s = y11 * y11;
                y2s = y22 * y22;
                y3s = y33 * y33;
            }

            var f1 = x1r1 * y1s * GetTangentElasticModulus(fi * y11 + e0);
            var f2 = x2r1 * y2s * GetTangentElasticModulus(fi * y22 + e0);
            var f3 = x3r1 * y3s * GetTangentElasticModulus(fi * y33 + e0);

            var buf = 1.0 / (r + 1.0) * neg * (w1 * f1 + w2 * f2 + w3 * f3);

            return buf;
        }

        /// <inheritdoc/>
        public override double IntegrateTangentElasticModulus(double y0, double y1, double alpha, double beta, double fi,
            double e0,
            int r, int s)
        {
            if (r + s > 4)
                throw new InvalidOperationException();

            var sum = 0.5 * (y1 + y0);
            var neg = 0.5 * (y1 - y0);


            const double k1 =
                -0.77459666924148337703585307995647992216658434105831816531751475322269661838739580;
            const double k2 = 0;
            const double k3 = -k1;


            const double w1 = 5.0 / 9.0;
            const double w2 = 8.0 / 9.0;
            const double w3 = 5.0 / 9.0;

            var z11 = sum + neg * k1;
            var z22 = sum + neg * k2;
            var z33 = sum + neg * k3;

            var y11 = alpha * z11 + beta; //y1^r
            var y22 = alpha * z22 + beta; //y2^r
            var y33 = alpha * z33 + beta; //y3^r

            var y1r = y11; //y1^r
            var y2r = y22; //y2^r
            var y3r = y33; //y3^r

            var z1s = z11; //z1^s
            var z2s = z22; //z2^s
            var z3s = z33; //z3^s

            if (r == 0)
            {
                y1r = y2r = y3r = 1;
            }
            else
            {
                for (var i = 1; i < r; i++)
                {
                    y1r *= y11;
                    y2r *= y22;
                    y3r *= y33;
                }
            }


            if (s == 0)
            {
                z1s = z2s = z3s = 1;
            }
            else
            {
                for (var i = 1; i < s; i++)
                {
                    z1s *= z11;
                    z2s *= z22;
                    z3s *= z33;
                }
            }

            var f1 = y1r * z1s * GetTangentElasticModulus(fi * z11 + e0);
            var f2 = y2r * z2s * GetTangentElasticModulus(fi * z22 + e0);
            var f3 = y3r * z3s * GetTangentElasticModulus(fi * z33 + e0);

            var buf = neg * (w1 * f1 + w2 * f2 + w3 * f3);

            return buf;
        }

        /// <inheritdoc/>
        public override double[] GetWalls()
        {
            //return new double[0];

            return new double[] { -1, 1 };
        }

        public override Material Clone()
        {
            var buf = new ElasticMaterial();

            buf._es = this._es;

            buf.PositiveFailureStrain = this.PositiveFailureStrain;
            buf.NegativeFailureStrain = this.NegativeFailureStrain;

            return buf;
        }

        #endregion

        #region Static methods

        #endregion

    }
}