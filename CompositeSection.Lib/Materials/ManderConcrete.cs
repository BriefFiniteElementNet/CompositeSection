using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompositeSection.Lib.Materials
{
    /// <summary>
    /// Concrete model based on Mander formula for ectangular sections
    /// </summary>
    [Serializable]
    [Obsolete("Still in development")]
    public class ManderConcrete: Material
    {

        /// <summary>
        /// if set to true, tensile strength considered, false otherwise
        /// </summary>
        public bool ConsiderTensileStress = false;

        public ManderConcrete(double fco, double sumAi2, double s, double bo, double ho, double @as, double fyw, double pz, double py)
        {
            this.fco = fco;
            this.sumAi2 = sumAi2;
            this.s = s;
            this.bo = bo;
            this.ho = ho;
            this.@as = @as;
            this.fyw = fyw;
            this.pz = pz;
            this.py = py;

            recalcPolynomialParameters();
        }

        private void recalcPolynomialParameters()
        {
            //due to integration problems we have to change to polynomial form
            //we define 5 conditions:
            //f(0)=0,f'(0)=Ec,f(ecc)=fcc,f'(ecc)=0 for first part

            double Ec, fcc;

            


            {

                {
                    Ec = 5000.0 * Math.Sqrt(fco);
                    double fact1 = 1.0 - (sumAi2 / (6.0 * bo * ho));
                    double fact2 = 1.0 - (s / (2.0 * bo));
                    double fact3 = 1.0 - (s / (2.0 * ho));
                    double fact4 = 1.0 / (1.0 - (As / (bo * ho)));
                    double ke = fact1 * fact2 * fact3 * fact4;

                    double fex = ke * pz * fyw;
                    double fey = ke * py * fyw;
                    double fe = (fex + fey) / 2.0;
                    double lambdac = 2.254 * Math.Sqrt(1.0 + (7.94 * fe / fco)) - (2.0 * fe / fco) - 1.254;
                    fcc = lambdac * fco;
                    double eco = 0.002;
                    ecc = eco * (1.0 + 5.0 * (lambdac - 1.0));

                    double Esec = fcc / ecc;

                    double r = R = Ec / (Ec - Esec);

                    {
                        double pshmin = Math.Min(pz, py);
                        double alfase = fact1 * fact2 * fact3;
                        double we = alfase * pshmin * fyw / fcc;
                        ecu = 0.0035 + 0.07 * Math.Sqrt(we);

                        if (ecu > 0.018) ecu = 0.018;
                    }
                }

                /*{
                    var det = -ecc * ecc * ecc * ecc;

                    a0 = 0;
                    a1 = Ec;

                    a3 = Ec * ecc * ecc - 2 * ecc * (Ec * ecc - fcc);
                    a2 = Ec * ecc * ecc * ecc - 3 * ecc * ecc * (Ec * ecc - fcc);

                    a3 /= det;
                    a2 /= det;
                }*/
            }

            {//first part

                var mtx = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(5);
                var right = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(5, 1);


                mtx.SetRow(0, new double[] { 0, 0, 0, 0, 1 });//f(0) = 0
                right.SetRow(0, new double[] { 0 });

                mtx.SetRow(1, new double[] { 0, 0, 0, 1, 0 });//f'(0) = Ec
                right.SetRow(1, new double[] { 1e6*Ec });

                mtx.SetRow(2, new double[] { 0.0625 * ecc * ecc * ecc * ecc, -0.125 * ecc * ecc * ecc, 0.25 * ecc * ecc, -0.5 * ecc, 1 });//f(-ecc/2) = ...
                right.SetRow(2, new double[] { (1e6*GetExactStress(-ecc/2)) });

                mtx.SetRow(3, new double[] { ecc * ecc * ecc * ecc,- ecc * ecc * ecc, ecc * ecc, -ecc, 1 });//f(-ecc) = -fcc
                right.SetRow(3, new double[] { -1e6*fcc });

                mtx.SetRow(4, new double[] { -4 * ecc * ecc * ecc, 3 * ecc * ecc, -2 * ecc, 1, 0 });//f'(-ecc) = 0
                right.SetRow(4, new double[] { 0 });

                var t = mtx.Inverse() * right;
                var ttt = mtx.Solve(right);

                a4 = ttt[0, 0];
                a3 = ttt[1, 0];
                a2 = ttt[2, 0];
                a1 = ttt[3, 0];
                a0 = ttt[4, 0];
            }

            {//second part

                var mtx = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(5);
                var right = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(5, 1);


                var fnc = new Func<double, double[]>(strainn => new double[] {
                    strainn * strainn * strainn * strainn,
                    strainn * strainn * strainn,
                    strainn * strainn,
                    strainn,
                    1 });

                var fpnc = new Func<double, double[]>(strainn => new double[] {
                    4*strainn * strainn * strainn ,
                    3*strainn * strainn,
                    2*  strainn,
                    1,
                    0 });



                mtx.SetRow(0, fnc(-ecc));//f(-ecc) = -fcc
                right.SetRow(0, new double[] { -1e6*fcc });

                mtx.SetRow(1, fpnc(-ecc));//f'(-ecc) = 0
                right.SetRow(1, new double[] { 0 });

                var e2 = (ecu + ecc) / 2;

                mtx.SetRow(2, fnc(-e2));//f(-e2) = ...
                right.SetRow(2, new double[] { 1e6*GetExactStress(-e2) });

                mtx.SetRow(3, fnc(-ecu));//f(-ecu) = -fcu
                right.SetRow(3, new double[] { 1e6 * GetExactStress(-ecu) });

                mtx.SetRow(4, fpnc(-ecu));//f'(-e2) = ...
                right.SetRow(4, new double[] { -1e6 * GetExactTangentElasticModulus(-ecu) });

                var t = mtx.Inverse() * right;
                var ttt = mtx.Solve(right);

                b4 = ttt[0, 0];
                b3 = ttt[1, 0];
                b2 = ttt[2, 0];
                b1 = ttt[3, 0];
                b0 = ttt[4, 0];
            }


            this.NegativeFailureStrain = -ecu;
            
        }

        #region properties
        private double ecc, ecu;
        private double etu = 0.00014;

        private double R;

        private double fco, sumAi2, s, bo, ho, @as, fyw, pz, py;

        private double a0, a1, a2, a3, a4;//sigma = a[i]*e^i for first part
        private double b0, b1, b2, b3, b4;//sigma = b[i]*e^i for second part
        

        /// <summary>
        /// Unconfined (characteristic strength) specific strength of concrete, in [MPa], for example 30 for C30 concrete
        /// </summary>
        /// <example>
        /// new ManderConcrete(){Fco = 30);//for C30 with 30 MPa
        /// </example>
        public double Fco
        {
            get { return fco; }
            private set
            {
                fco = value;
            }
        }

        /// <summary>
        /// in [mm^2],Ai is distance between longitudinal rebars and sumAi2 is sum ai*ai along section
        /// </summary>
        public double SumAi2
        {
            get { return sumAi2; }
            private set
            {
                sumAi2 = value;
            }
        }

        /// <summary>
        /// in [mm], s is confinement spacing (space of stirups)
        /// </summary>
        public double S
        {
            get
            {
                return s;
            }

            private set
            {
                s = value;
            }
        }

        /// <summary>
        /// in [mm],distance between confined concrete, b is total section width and bo = b-2* concrete cover
        /// </summary>
        public double Bo
        {
            get { return bo; }
            private set
            {
                bo = value;
            }
        }
        /// <summary>
        /// in [mm],distance between confined concrete, h is total section height and ho = h-2* concrete cover
        /// </summary>
        public double Ho
        {
            get { return ho; }
            private set
            {
                ho = value;
            }
        }
        /// <summary>
        /// in [mm^2] Longitudinal steel, total of section
        /// </summary>
        public double As
        {
            get { return @as; }
            private set
            {
                @as = value;
            }
        }
        /// <summary>
        /// in [Mpa] confinement steel yield stress
        /// </summary>
        public double Fyw
        {
            get { return fyw; }
            private set
            {
                fyw = value;
            }
        }

        /// <summary>
        /// volumentric ratio of confinement to concrete 
        /// </summary>
        /// <remarks>
        /// take 1 meter legth from element and find volüme of confinement in z direction and divide by element 1 meter length volume
        /// </remarks>
        public double Pz
        {
            get { return pz; }
            private set
            {
                pz = value;
            }
        }

        /// <summary>
        /// volumentric ratio of confinement to concrete 
        /// </summary>
        /// <remarks>
        /// take 1 meter legth from element and find volüme of confinement in y direction and divide by element 1 meter length volume
        /// </remarks>
        public double Py
        {
            get { return py; }
            private set
            {
                py = value;
            }
        }

        #endregion

        public double GetExactStress(double strain)
        {

            
            var ec = -strain;


            if (ec < 0)
                return 0;

            double fc = 0;

            //start
            double fact1 = 1.0 - (sumAi2 / (6.0 * bo * ho));
            double fact2 = 1.0 - (s / (2.0 * bo));
            double fact3 = 1.0 - (s / (2.0 * ho));
            double fact4 = 1.0 / (1.0 - (As / (bo * ho)));
            double ke = fact1 * fact2 * fact3 * fact4;

            double fex = ke * pz * fyw;
            double fey = ke * py * fyw;
            double fe = (fex + fey) / 2.0;
            double lambdac = 2.254 * Math.Sqrt(1.0 + (7.94 * fe / fco)) - (2.0 * fe / fco) - 1.254;
            double fcc = lambdac * fco;
            double Ec = 5000.0 * Math.Sqrt(fco);
            double eco = 0.002;
            double ecc = eco * (1.0 + 5.0 * (lambdac - 1.0));

            ecc = eco * (1.0 + 5.0 * (lambdac - 1.0));

            double Esec = fcc / ecc;

            double r = Ec / (Ec - Esec);

            double x = ec / ecc;


            fc = (fcc * x * r) / (r - 1.0 + Math.Pow(x, r));

            return -fc;
        }

        public double GetExactTangentElasticModulus(double strain)
        {
            var ec = -strain;
            double fc = 0;

            //start
            double fact1 = 1.0 - (sumAi2 / (6.0 * bo * ho));
            double fact2 = 1.0 - (s / (2.0 * bo));
            double fact3 = 1.0 - (s / (2.0 * ho));
            double fact4 = 1.0 / (1.0 - (As / (bo * ho)));
            double ke = fact1 * fact2 * fact3 * fact4;

            double fex = ke * pz * fyw;
            double fey = ke * py * fyw;
            double fe = (fex + fey) / 2.0;
            double lambdac = 2.254 * Math.Sqrt(1.0 + (7.94 * fe / fco)) - (2.0 * fe / fco) - 1.254;
            double fcc = lambdac * fco;
            double Ec = 5000.0 * Math.Sqrt(fco);
            double eco = 0.002;
            double ecc = eco * (1.0 + 5.0 * (lambdac - 1.0));
            double Esec = fcc / ecc;

            

            double r = Ec / (Ec - Esec);

            double x = ec / ecc;

            //E = df/dε = df/dx * dx/dε 
            //f = F/G
            //df = (F.dG-G.dF)/G^2

            //F = fcc*x*r
            //dF = fcc*r*dx
            //G = (r - 1.0 + Math.Pow(x, r))
            //dG = R * Math.Pow(x, R-1)

            //df/dx = (F.dG/dx-G.dF/dx)/G^2

            //E = df/dε = df/dx * dx/dε 


            var F = (fcc * x * r);
            var dFdx =  fcc * r;

            var G = (r - 1.0 + Math.Pow(x, r));
            var dGdx = (r - 1.0 + r*Math.Pow(x, r-1));

            var dfdx = (F * dGdx - G * dFdx) / (G * G);
            var dxde = 1 / ecc;

            fc = (fcc * x * r) / (r - 1.0 + Math.Pow(x, r));

            var E = dfdx * dxde;

            return E;
        }


        public override Material Clone()
        {
            throw new NotImplementedException();
        }

        public override double GetStress(double strain)
        {
            var x = strain;

            var buf = 0.0;

            

            if (strain > etu)
                buf = 0;

            if (strain > 0 && strain <= etu)
                buf = ConsiderTensileStress ? a1 * x + a0 : 0;

            if (strain < 0 && strain > -ecc)
                buf = a4 * x * x * x * x + a3 * x * x * x + a2 * x * x + a1 * x + a0;

            if (strain <= -ecc && strain >= -ecu)
                buf = b4 * x * x * x * x + b3 * x * x * x + b2 * x * x + b1 * x + b0;

            if (strain < -ecu)
                buf = 0;

            //yet buf is MPa

            //buf *= 10e6;

            return buf;
        }

        public override double GetTangentElasticModulus(double strain)
        {
            var x = strain;

            var buf = 0.0;


            //if (ConsiderTensileStress)
            //    if(strain>0 && strain > ;

            var etu = 0.00014;

            if (strain > etu)
                buf = 0;

            if (strain > 0 && strain <= etu)
                buf = ConsiderTensileStress ? a1 : 0;

            if (strain <= 0 && strain > -ecc)
                buf = 4 * a4 * x * x * x + 3 * a3 * x * x + 2 * a2 * x + a1;

            if (strain <= -ecc && strain >= -ecu)
                buf = 4 * b4 * x * x * x + 3 * b3 * x * x + 2 * b2 * x + b1;

            //buf *= 10e6;

            return buf;
        }


        public override double[] GetWalls()
        {
            if(ConsiderTensileStress)
                return new double[] { -ecu, -ecc, 0, etu };
            else
                return new double[] { -ecu, -ecc, 0};
        }

        private static double Pow(double @base,int power)
        {
            switch(power)
            {
                case 0: return 1;
                case 1: return @base;
                case 2: return @base* @base;
                case 3: return @base* @base* @base;
                case 4: return @base * @base * @base * @base;
                case 5: return @base * @base * @base * @base * @base;
                case 6: return @base * @base * @base * @base * @base * @base;
            }

            return Math.Pow(@base, power);
        }

        public override double IntegrateStress(double z0, double z1, double alpha, double beta, double phi, double e0, int r, int s)
        {
            if (r + s > 3)
                throw new InvalidOperationException();

            if (z1 < z0)
                throw new Exception();


            var sum = 0.5 * (z1 + z0);
            var neg = 0.5 * (z1 - z0);

            var n = 4;

            var wi = new double[] { 0.6521451548625461, 0.6521451548625461, 0.3478548451374538, 0.3478548451374538 };
            var ki = new double[] { -0.3399810435848563, 0.3399810435848563, -0.8611363115940526, 0.8611363115940526 };


            var buff = 0.0;

            for (var i = 0; i < n; i++)
            {
                var z = sum + neg * ki[i];

                var y_r = Pow(alpha * z + beta, r);
                var z_s = Pow(z, s);

                var strain = phi * z + e0;
                var stress = GetStress(strain);

                var f = y_r * z_s * stress;
                buff += neg * f * wi[i];
            }


            var zii = new double[4];

            for (var i = 0; i < n; i++)
                zii[i] = sum + neg * ki[i];

            /*
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
            */

            var yir = new double[n];

            for (var i = 0; i < n; i++)
                yir[i] = Pow(alpha * zii[i] + beta, r);


            var zis = new double[n];

            for (var i = 0; i < n; i++)
                zis[i] = Pow(zii[i], s);

            /*
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
            }*/


            var fi = new double[n];

            for (var i = 0; i < n; i++)
                fi[i] = yir[i] * zis[i] * GetStress(phi * zii[i] + e0);

            //var f1 = y1r * z1s * GetStress(phi * z11 + e0);
            //var f2 = y2r * z2s * GetStress(phi * z22 + e0);
            //var f3 = y3r * z3s * GetStress(phi * z33 + e0);

            //var buf = neg * (w1 * f1 + w2 * f2 + w3 * f3);
            var buf = 0.0;

            for(var i=0;i<n;i++)
            {
                buf += wi[i] * fi[i];
            }

            buf *= neg;

            return buff;
        }

        public override double IntegrateTangentElasticModulus(double z0, double z1, double alpha, double beta, double phi, double e0, int r, int s)
        {
            if (r + s > 4)
                throw new InvalidOperationException();

            var sum = 0.5 * (z1 + z0);
            var neg = 0.5 * (z1 - z0);

            var n = 4;

            var wi = new double[] { 0.6521451548625461, 0.6521451548625461, 0.3478548451374538, 0.3478548451374538 };
            var ki = new double[] { -0.3399810435848563, 0.3399810435848563, -0.8611363115940526, 0.8611363115940526 };


            var zi = new double[4];

            for (var i = 0; i < n; i++)
                zi[i] = sum + neg * ki[i];

            var yir = new double[n];//(alpha*z+betta) ^ r

            for (var i = 0; i < n; i++)
                yir[i] = Pow(alpha * zi[i] + beta, r);


            var zis = new double[n];//z[i]^s

            for (var i = 0; i < n; i++)
                zis[i] = Pow(zi[i], s);

            var fi = new double[n];

            for (var i = 0; i < n; i++)
                fi[i] = yir[i] * zis[i] * GetTangentElasticModulus(phi * zi[i] + e0);

            
            var buf = 0.0;

            for (var i = 0; i < n; i++)
            {
                buf += wi[i] * fi[i];
            }

            buf *= neg;

            return buf;
        }
    }
}
