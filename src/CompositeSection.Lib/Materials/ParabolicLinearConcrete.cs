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
    /// Represents a class for general parabolic linear stress strain curve for concrete
    /// this model ignore stress in tension, a linear form from _eMin to _e0 and a parabolic form from _e0 to 0
    /// note that _eMin and _e0 are always negative strains
    /// </summary>
    [Serializable]
    public class ParabolicLinearConcrete : Material
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="ParabolicLinearConcrete"/> class from being created.
        /// </summary>
        private ParabolicLinearConcrete()
        {
        }


        #region IStressStrainCurve

        /// <inheritdoc/>
        public override double GetStress(double strain)
        {
            var etu = 0.0;

            if (_allowTension) etu = 0.1 * 1e6 * Fc / _d;

            if (strain > etu)
                return 0;

            if (strain > 0 && strain < etu)
                return _allowTension ? _d * strain : 0.0;

            if (strain < _eMin)
                return 0;

            if (strain <= _e0 && strain >= _eMin)
                return _a * strain + _b;

            return _c * strain * strain + _d * strain + _e;
        }

        /// <inheritdoc/>
        public override double GetTangentElasticModulus(double strain)
        {

            var etu = 0.0;

            if (_allowTension) etu = 0.1 * 1e6 * Fc / _d;

            if (strain > etu)
                return 0;

            if (strain > 0 && strain < etu)
                return _allowTension ? _d : 0.0;

            if (strain < _eMin)
                return 0;

            if (strain <= _e0 && strain >= _eMin)
                return _a;

            return 2 * _c * strain + _d;
        }

        /// <inheritdoc/>
        public override double IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0, int r, int s)
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

            var buf = neg*(w1*f1 + w2*f2 + w3*f3);

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

            var buf = neg*(w1*f1 + w2*f2 + w3*f3);


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

        /// <inheritdoc/>
        public override double[] GetWalls()
        {
            var etu = 0.0;

            if (_allowTension) etu = 0.1 * 1e6 * Fc / _d;

            if (_allowTension)
                return new double[] { _eMin, _e0, 0, etu };
            else
                return new double[] { _eMin, _e0, 0 };
        }

        /// <inheritdoc/>
        public override Material Clone()
        {
            var buf = new ParabolicLinearConcrete();

            buf._fc = this._fc;
            buf._eMin = this._eMin;
            buf._e0 = this._e0;

            buf._a = this._a;
            buf._b = this._b;
            buf._c = this._c;
            buf._d = this._d;
            buf._e = this._e;

            buf._allowTension = this.AllowTension;

            buf.PositiveFailureStrain = this.PositiveFailureStrain;
            buf.NegativeFailureStrain = this.NegativeFailureStrain;

            return buf;
        }

        #endregion

        #region Members

        private double _fc;
        private double _eMin;
        private double _e0;

        private double _a;
        private double _b;
        private double _c;
        private double _d;
        private double _e;

        private bool _allowTension;


        /// <summary>
        /// Gets the fc.
        /// </summary>
        /// <value>
        /// The fc in MPa.
        /// </value>
        public double Fc
        {
            get { return _fc; }
        }

        /// <summary>
        /// Determines wether caoncrete carry tension 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool AllowTension
        {
            get
            {
                return _allowTension;
            }
            set
            {
                _allowTension = value;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new parabolic stress strain curve related to specified fc.
        /// </summary>
        /// <param name="fc">The concrete specific strength in MPa.</param>
        /// <param name="isContant">if set to <c>true</c> the stress strain curve section in most negative strain interval will be constant otherwise will be linear. Refer to documentation for more information</param>
        /// <returns>new instance of <see cref="Material"/></returns>
        public static ParabolicLinearConcrete Create(double fc, bool isContant,bool allowTension=false)
        {
            if (fc < 0)
                throw new ArgumentOutOfRangeException("fc");

            var ks = 0.0;

            if (fc <= 15)
                ks = 1.0;

            if (fc > 15 && fc < 30)
                ks = 1 - (fc - 15) * 0.08 / 15.0;

            if (fc >= 30)
                ks = 0.92;

            var ec = 4700 * System.Math.Sqrt(fc) * 1e6;

            var fzeg = ks * fc;

            var e0 = -1.8 * fzeg / ec * 1e6;

            var alfa = (-0.15 * fzeg) / (e0 + 0.0038);

            var buf = new ParabolicLinearConcrete();
            buf._fc = fc;

            buf._eMin = -0.0038;
            buf._e0 = e0;

            if (!isContant)
            {
                buf._a = 1e6 * alfa;
                buf._b = 1e6 * (alfa * 0.0038 - fzeg * 0.85);
            }
            else
            {
                buf._a = 0;
                buf._b = -1e6 * fzeg;
            }

            buf._c = 1e6 * fzeg / (e0 * e0);
            buf._d = 1e6 * -2 * fzeg / e0;
            buf._e = 0;

            buf._allowTension = allowTension;
            return buf;
        }

        /// <summary>
        /// Creates a new parabolic stress strain curve related to specified fc.
        /// </summary>
        /// <param name="fc">The concrete specific strength in MPa.</param>
        /// <param name="ep0">The ep0.</param>
        /// <param name="isContant">if set to <c>true</c> the stress strain curve section in most negative strain interval will be constant otherwise will be linear. Refer to documentation for more information</param>
        /// <returns>
        /// new instance of <see cref="Material" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">fc</exception>
        public static ParabolicLinearConcrete CreateEc2(double fc,bool allowTension=false)
        {
            if (fc < 0)
                throw new ArgumentOutOfRangeException("fc");

            if (fc > 50)
                throw new ArgumentOutOfRangeException("fc");

            var fcd = fc;

            var e0 = -0.002;

            var buf = new ParabolicLinearConcrete();
            buf._fc = fc;

            buf._eMin = -0.0035;
            buf._e0 = e0;

            buf._a = 0;
            buf._b = -1e6*fcd;

            buf._c = 1e6*fcd/(e0*e0);
            buf._d = 1e6*-2*fcd/e0;
            buf._e = 0;


            buf._allowTension = allowTension;

            return buf;
        }

        /// <summary>
        /// Creates a new parabolic stress strain curve related to specified fc.
        /// </summary>
        /// <param name="fc">The concrete specific strength in MPa.</param>
        /// <param name="ec">The elastic modulus at 0 strain.</param>
        /// <param name="isContant">if set to <c>true</c> the stress strain curve section in most negative strain interval will be constant otherwise will be linear. Refer to documentation for more information</param>
        /// <returns>new instance of <see cref="Material"/></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">fc</exception>
        public static ParabolicLinearConcrete Create(double fc,double ec, bool isContant, bool allowTension = false)
        {
            if (fc < 0)
                throw new ArgumentOutOfRangeException("fc");

            var ks = 0.0;

            if (fc <= 15)
                ks = 1.0;

            if (fc > 15 && fc < 30)
                ks = 1 - (fc - 15) * 0.08 / 15.0;

            if (fc >= 30)
                ks = 0.92;

            //var ec = 4700 * System.Math.Sqrt(fc) * 1e6;

            var fzeg = ks * fc;

            var e0 = -1.8 * fzeg / ec * 1e6;

            var alfa = (-0.15 * fzeg) / (e0 + 0.0038);

            var buf = new ParabolicLinearConcrete();
            buf._fc = fc;

            buf._eMin = -0.0038;
            buf._e0 = e0;

            if (!isContant)
            {
                buf._a = 1e6 * alfa;
                buf._b = 1e6 * (alfa * 0.0038 - fzeg * 0.85);
            }
            else
            {
                buf._a = 0;
                buf._b = -1e6 * fzeg;
            }

            buf._c = 1e6 * fzeg / (e0 * e0);
            buf._d = 1e6 * -2 * fzeg / e0;
            buf._e = 0;

            buf._allowTension = allowTension;

            return buf;
        }

        /// <inheritdoc/>
        protected ParabolicLinearConcrete(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _fc = (double) info.GetValue("_fc", typeof (double));
            _eMin = (double) info.GetValue("_eMin", typeof (double));
            _e0 = (double) info.GetValue("_e0", typeof (double));
            _a = (double) info.GetValue("_a", typeof (double));
            _b = (double) info.GetValue("_b", typeof (double));
            _c = (double) info.GetValue("_c", typeof (double));
            _d = (double) info.GetValue("_d", typeof (double));
            _e = (double) info.GetValue("_e", typeof (double));
            _allowTension = (bool)info.GetValue("_allowTension", typeof(bool));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_fc", _fc);
            info.AddValue("_eMin", _eMin);
            info.AddValue("_e0", _e0);
            info.AddValue("_a", _a);
            info.AddValue("_b", _b);
            info.AddValue("_c", _c);
            info.AddValue("_d", _d);
            info.AddValue("_e", _e);
            info.AddValue("_allowTension", _allowTension);

            base.GetObjectData(info, context);
        }
    }
}
