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
using System.Runtime.Serialization;

namespace CompositeSection.Lib.Materials
{
    /// <summary>
    /// Represents a perfect elastic plastic behavior material
    /// </summary>
    [Serializable]
    public class PerfectElasticPlastic : Material
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="PerfectElasticPlastic"/> class from being created.
        /// </summary>
        private PerfectElasticPlastic()
        {
        }

        /// <inheritdoc/>
        public PerfectElasticPlastic(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _fy = (double)info.GetValue("_fy", typeof(double));
            _ey= (double)info.GetValue("_ey", typeof(double));
            _eu = (double)info.GetValue("_eu", typeof(double));
            _es = (double)info.GetValue("_es", typeof(double));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_fy", _fy);
            info.AddValue("_ey", _ey);
            info.AddValue("_eu", _eu);
            info.AddValue("_es", _es);

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// The yield stress, positive value
        /// </summary>
        private double _fy;

        /// <summary>
        /// The yield strain, corresponding to <see cref="_fy"/>
        /// </summary>
        private double _ey;


        /// <summary>
        /// The ultimate strain, positive value
        /// </summary>
        private double _eu;


        /// <summary>
        /// The elastic modulus of material
        /// </summary>
        private double _es;

        /// <inheritdoc/>
        public override double GetStress(double strain)
        {
            if (strain < -_eu || strain > _eu)
                return 0.0;

            if (strain < _ey && strain > -_ey)
                return strain * _es;

            if (strain < 0.0)
                return -_fy - 100 * (strain +_ey );
            else
                return _fy + 100 * (strain - _ey); ;
        }

        /// <inheritdoc/>
        public override double GetTangentElasticModulus(double strain)

        {
            if (strain < _ey && strain > -_ey)
                return _es;

            return 100.0;
        }

        /// <inheritdoc/>
        public override double IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0, int r, int s)
        {
            if (r + s > 3)
                throw new InvalidOperationException();

            var sum = 0.5 * (z1 + z0);
            var neg = 0.5 * (z1 - z0);


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
        public override double[] GetWalls()
        {
            return new double[] { -_eu, -_ey, _ey, _eu };
        }

        /// <inheritdoc/>
        public override Material Clone()
        {
            var buf = new PerfectElasticPlastic();
            
            buf._fy = this._fy;
            buf._es = this._es;
            buf._eu = this._eu;
            buf._ey = this._ey;

            buf.PositiveFailureStrain = this.PositiveFailureStrain;
            buf.NegativeFailureStrain = this.NegativeFailureStrain;

            return buf;
        }


        /// <summary>
        /// Creates a new perfect elastic - plastic stress strain curve for normal steel (E = 210 GPa, εu = 0.02) related to specified fy.
        /// </summary>
        /// <param name="fy">The yield stress in MPa</param>
        /// <param name="e">The elastic modulus.</param>
        /// <returns>
        /// Stress Strain Curve
        /// </returns>
        public static PerfectElasticPlastic Create(double fy, double e = 210e9, double eu = 0.02)
        {
            var es = e; //elastic modulus

            var buf = new PerfectElasticPlastic();

            buf._es = es;
            buf._fy = fy*1e6;
            buf._ey = buf._fy/es;
            buf._eu = Math.Max(eu, buf._ey) + 1e-8;
            

            return buf;
        }


        /// <inheritdoc/>
        public override double IntegrateTangentElasticModulus(double y0, double y1, double alpha, double beta, double fi, double e0,
            int r, int s)
        {
            if (r + s > 4)
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


            /** /
             var t = new List<double>();

             for (var i = 3; i < 7; i++)
             {
                 t.Add(this.GaussIntegrateOriginalTangentElasticModulus(y0, y1, alpha, beta, fi, e0, r, s, i ));
             }

             var maxDif = t.Select(i => Math.Abs(buf - i)).Max();

             if (Math.Abs(maxDif) > 1e-5)
                 Guid.NewGuid();
             /**/


            return buf;
        }


    }
}
