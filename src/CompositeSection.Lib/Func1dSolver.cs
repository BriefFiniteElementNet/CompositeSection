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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CompositeSection.Lib
{
    public class Func1DSolver
    {
        public double Max = double.NaN;

        public double Min = double.NaN;

        
        public Func1D F;

        /// <summary>
        /// The inputs history,
        /// inputs to the <see cref="F"/>.GetAxialForce() method
        /// </summary>
        public List<double> InputsHistory = new List<double>();

        /// <summary>
        /// The output history,
        /// outputs from the <see cref="F"/>.GetAxialForce() method
        /// </summary>
        public List<double> OutputHistory = new List<double>();

        /// <summary>
        /// The differentiate inputs history,
        /// inputs to the <see cref="F"/>.GetAxialForceDifferentiate() method
        /// </summary>
        public List<double> DInputsHistory = new List<double>();

        /// <summary>
        /// The differentiate output history,
        /// outputs from the <see cref="F"/>.GetAxialForceDifferentiate() method
        /// </summary>
        public List<double> DOutputHistory = new List<double>();


        public int HisCount{get { return InputsHistory.Count; }}

        /// <summary>
        /// The tolerance
        /// </summary>
        public double AbsoluteTolerance = 1e-6;


        /// <summary>
        /// Solves the equation of F(x) = y for x and returns x.
        /// </summary>
        /// <param name="y">The function result.</param>
        /// <param name="x">The x.</param>
        /// <returns>
        /// x
        /// </returns>
        public bool SolveObs(double y, out double x)
        {
            if (y > Max || y < Min)
            {
                x = double.NaN;
                return false;
            }

            Debug.Assert(OutputHistory.Count >= 3);

            var dn = double.MaxValue;
            var n1 = -1;//index of first

            var dn2 = double.MaxValue;
            var n2 = -1;//index of second

            var dn3 = double.MaxValue;
            var n3 = -1;//index of third

            #region nearest ones

            for (var i = 0; i < HisCount; i++)
            {
                var d2 = Math.Abs(OutputHistory[i] - y);

                if (d2 < dn)
                {
                    dn3 = dn2;
                    n3 = n2;

                    dn2 = dn;
                    n2 = n1;

                    dn = d2;
                    n1 = i;

                    goto done;
                }

                if (d2 < dn2)
                {
                    dn3 = dn2;
                    n3 = n2;

                    dn2 = d2;
                    n2 = i;

                    goto done;
                }

                if (d2 < dn3)
                {
                    dn3 = d2;
                    n3 = i;

                    goto done;
                }

                done:
                ;
            }

            #endregion

            var dm = Max - Min;

            if (dn/dm < AbsoluteTolerance)
            {
                x = InputsHistory[n1];
                return true;
            }

            // حالا سه تا نقطه داریم که مطمئنیم هر سه تاش هم مقدار دارن
            //یعنی اندیس هیچکدوم 1- نیست
            // پس اول کار حتما باید با سه تا نقطه تو 
            //InitiateHistory
            // شروع کنیم:
            //0.0, 0.5, 1.0

            offset = y;

            {
                var c = 0;

                var x1 = InputsHistory[n1];
                var y1 = OutputHistory[n1];
                var x2 = InputsHistory[n2];
                var y2 = OutputHistory[n2];


                while (c++ < 100)
                {
                    var newX = 0.0;
                    var newY = 0.0;

                    if (!GetNextGuessLinear(F, x1, y1, x2, y2, out newX, out newY))
                    {
                        x = 0;
                        return false;
                        throw new Exception();
                    }


                    if ((newY - offset) * (y1 - offset) > 0)
                    {
                        y1 = newY;
                        x1 = newX;
                    }

                    if ((newY - offset) * (y2 - offset) > 0)
                    {
                        y2 = newY;
                        x2 = newX;
                    }


                    if (!InputsHistory.Contains(newX))
                    {
                        InputsHistory.Add(newX);
                        OutputHistory.Add(newY);
                    }

                    if (Math.Abs((newY - y)/y) < AbsoluteTolerance)
                    {
                        x = newX;
                        return true;
                    }
                }

                x = 0;
                return false;
                
            }

        }

        public bool SolveObs2(double y, out double x)
        {
            if (y > Max || y < Min)
            {
                x = double.NaN;
                return false;
            }

            Debug.Assert(OutputHistory.Count >= 3);

            var dn = double.MaxValue;
            var n1 = -1;//index of first

            var dn2 = double.MaxValue;
            var n2 = -1;//index of second

            var dn3 = double.MaxValue;
            var n3 = -1;//index of third

            #region nearest ones

            for (var i = 0; i < HisCount; i++)
            {
                var d2 = Math.Abs(OutputHistory[i] - y);

                if (d2 < dn)
                {
                    dn3 = dn2;
                    n3 = n2;

                    dn2 = dn;
                    n2 = n1;

                    dn = d2;
                    n1 = i;

                    goto done;
                }

                if (d2 < dn2)
                {
                    dn3 = dn2;
                    n3 = n2;

                    dn2 = d2;
                    n2 = i;

                    goto done;
                }

                if (d2 < dn3)
                {
                    dn3 = d2;
                    n3 = i;

                    goto done;
                }

            done:
                ;
            }

            #endregion

            var dm = Max - Min;

            if (dn / dm < AbsoluteTolerance)
            {
                x = InputsHistory[n1];
                return true;
            }

            // حالا سه تا نقطه داریم که مطمئنیم هر سه تاش هم مقدار دارن
            //یعنی اندیس هیچکدوم 1- نیست
            // پس اول کار حتما باید با سه تا نقطه تو 
            //InitiateHistory
            // شروع کنیم:
            //0.0, 0.5, 1.0

            offset = y;

            {
                var c = 0;

                var x1 = InputsHistory[n1];
                var y1 = OutputHistory[n1];
                var x2 = InputsHistory[n2];
                var y2 = OutputHistory[n2];


                while (c++ < 100)
                {
                    var newX = 0.0;
                    var newY = 0.0;

                    if (!GetNextGuessLinear(F, x1, y1, x2, y2, out newX, out newY))
                    {
                        x = 0;
                        return false;
                        throw new Exception();
                    }


                    if ((newY - offset) * (y1 - offset) > 0)
                    {
                        y1 = newY;
                        x1 = newX;
                    }

                    if ((newY - offset) * (y2 - offset) > 0)
                    {
                        y2 = newY;
                        x2 = newX;
                    }


                    if (!InputsHistory.Contains(newX))
                    {
                        InputsHistory.Add(newX);
                        OutputHistory.Add(newY);
                    }

                    if (Math.Abs((newY - y) / y) < AbsoluteTolerance)
                    {
                        x = newX;
                        return true;
                    }
                }

                x = 0;
                return false;

            }

        }

        public bool Solve2(double y, out double x)
        {
            if (y > Max || y < Min)
            {
                x = double.NaN;
                return false;
            }

            offset = y;

            {
                var c = 0;

                var x1 = 0.0;
                var y1 = F.GetAxialForce(x1);
                var x2 = 1.0;
                var y2 = F.GetAxialForce(x2);


                while (c++ < 100)
                {
                    var newX = 0.0;
                    var newY = 0.0;

                    if (!GetNextGuessLinear(F, x1, y1, x2, y2, out newX, out newY))
                    {
                        x = 0;
                        return false;
                        throw new Exception();
                    }


                    if ((newY - offset) * (y1 - offset) > 0)
                    {
                        y1 = newY;
                        x1 = newX;
                    }

                    if ((newY - offset) * (y2 - offset) > 0)
                    {
                        y2 = newY;
                        x2 = newX;
                    }


                    if (!InputsHistory.Contains(newX))
                    {
                        InputsHistory.Add(newX);
                        OutputHistory.Add(newY);
                    }

                    if (Math.Abs((newY - y) / y) < AbsoluteTolerance)
                    {
                        x = newX;
                        return true;
                    }
                }

                x = 0;
                return false;

            }

        }


        public bool Solve(double y, out double x)
        {
            for (var i = 0; i < OutputHistory.Count; i++)
            {
                if (Math.Abs(OutputHistory[i] - y) < AbsoluteTolerance)
                {
                    x = InputsHistory[i];
                    return true;
                }
            }

            if (y > Max || y < Min)
            {
                x = double.NaN;
                return false;
            }

            offset = y;

            {
                var c = 0;

                var x1 = 0.0;
                var y1 = F.GetAxialForce(x1);
                var x2 = 1.0;
                var y2 = F.GetAxialForce(x2);

                for (int i = 0; i < OutputHistory.Count - 1; i++)
                {
                    var yi = OutputHistory[i];
                    var yi1 = OutputHistory[i + 1];

                    if (y >= Math.Min(yi, yi1) && y <= Math.Max(yi, yi1))
                    {
                        x1 = InputsHistory[i];
                        x2 = InputsHistory[i + 1];

                        y1 = OutputHistory[i];
                        y2 = OutputHistory[i + 1];

                        break;
                    }
                }


                while (c++ < 100)
                {
                    var newX = 0.0;
                    var newY = 0.0;

                    if (!GetNextGuessLinear(F, x1, y1, x2, y2, out newX, out newY))
                    {
                        x = 0;
                        return false;
                        throw new Exception();
                    }


                    if ((newY - offset) * (y1 - offset) > 0)
                    {
                        y1 = newY;
                        x1 = newX;
                    }

                    if ((newY - offset) * (y2 - offset) > 0)
                    {
                        y2 = newY;
                        x2 = newX;
                    }


                    if (!InputsHistory.Contains(newX))
                    {
                        lock (InputsHistory)
                        {
                            lock (OutputHistory)
                            {
                                InputsHistory.Add(newX);
                                OutputHistory.Add(newY);
                            }
                        }
                        
                    }

                    if (Math.Abs((newY - y) ) < AbsoluteTolerance)
                    {
                        x = newX;
                        return true;
                    }
                }

                x = 0;
                return false;

            }

        }

        private double offset;

        private bool GetNextGuessLinear(Func1D f, double x1, double y1, double x2, double y2,
            out double newX, out double newY)
        {
            var xMin = x1;
            var yMin = y1 - offset;

            var xMax = x2;
            var yMax = y2 - offset;

            if (x1 > x2)
            {
                xMax = x1;
                yMax = y1 - offset;

                xMin = x2;
                yMin = y2 - offset;
            }
            
            //if (yMin * yMax > 0)
            //    throw new InvalidOperationException();

            newX = 0;
            newY = 0;

            if (yMax.Equals(yMin))
                return false;

            newX = -yMin * (xMax - xMin) / (yMax - yMin) + xMin;

            if (newX > xMax || newX < xMin)
            {
                return false;
            }

            newY = f.GetAxialForce(newX);

            return true;
        }

        [Obsolete]
        public void InitiateHistory(params double[] xs)
        {
            if (xs.Length == 0)
                throw new Exception();

            var max = double.NegativeInfinity;
            var min = double.PositiveInfinity;


            foreach (var x in xs)
            {
                var y = F.GetAxialForce(x);

                if (y > max)
                    max = y;

                if (y < min)
                    min = y;

                if (!InputsHistory.Contains(x))
                {
                    InputsHistory.Add(x);
                    OutputHistory.Add(y);
                }

               
            }

            this.Max = max;
            this.Min = min;
        }

        public void AddGuesses(params double[] xs)
        {
            foreach (var x in xs)
            {
                var y = F.GetAxialForce(x);

                if (y > Max)
                    Max = y;

                if (y < Min)
                    Min = y;

                if (!InputsHistory.Contains(x))
                {
                    InputsHistory.Add(x);
                    OutputHistory.Add(y);
                }
                
            }
        }

        /// <summary>
        /// Analyses the <see cref="F"/> and calculates its <see cref="Func1D.Walls"/>.
        /// </summary>
        public void AnalyseForWalls()
        {
            //check for advanced stuff, have to find roots of dNx/dVal?

            //var epsilon = 1e-8;

            var count = 10;

            for (var i = 0; i <= count; i++)
            {
                var x = i/((double) count);

                var f = F.GetAxialForce(x);

                var df = F.GetAxialForceDifferentiate(x);

                InputsHistory.Add(x);

                OutputHistory.Add(f);

                DInputsHistory.Add(x);

                DOutputHistory.Add(df);
            }

            var dfAbsMax = DOutputHistory.Select(Math.Abs).Max();

            var tol = 1e-2*dfAbsMax;

            F.Walls.Add(Tuple.Create(0.0, F.GetAxialForce(0.0)));
            F.Walls.Add(Tuple.Create(1.0, F.GetAxialForce(1.0)));


            for (var i = 0; i < count; i++)
            {
                var dfi = DOutputHistory[i];
                var dfi1 = DOutputHistory[i+1];

                double root;

                if (dfi*dfi1 < 0)
                {
                    if (GetDifferentiateRoot(DInputsHistory[i], DInputsHistory[i + 1], tol, out root))
                    {
                        var fv = F.GetAxialForce(root);

                        lock (F.Walls)
                        {
                            F.Walls.Add(Tuple.Create(root, fv));    
                        }

                        lock (InputsHistory)
                        {
                            lock (OutputHistory)
                            {
                                InputsHistory.Add(root);
                                OutputHistory.Add(fv);
                            }
                        }

                        
                    }
                }
            }

            F.Walls.Sort(i => i.Item1);

            this.Max = OutputHistory.Max();
            this.Min = OutputHistory.Min();
        }

        /// <summary>
        /// Tries to find root of dNx/dVal in specified range
        /// </summary>
        /// <param name="v1">The v1 (between 0 and 1).</param>
        /// <param name="v2">The v2 (between 0 and 1).</param>
        /// <param name="tol">
        /// The amount of tolerance. This is a value not a coefficient.
        /// Value directly will be used.
        /// </param>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool GetDifferentiateRoot(double v1, double v2, double tol, out double root)
        {
            var x1 = Math.Min(v1, v2);
            var x2 = Math.Max(v1, v2);

            var y1 = F.GetAxialForceDifferentiate(x1);
            var y2 = F.GetAxialForceDifferentiate(x2);

            root = double.NaN;

            if (y1*y2 >= 0)
                return false;

            var count = 0;

            while (count++ < 100)
            {
                double newX, newY;

                if (!GetNextDifferentiateGuessLinear(F, x1, y1, x2, y2, out newX, out newY))
                    return false;

                if (newX < x1 || newX > x2)
                    throw new Exception();

                if (newY * y1 > 0)
                {
                    x1 = newX;
                    y1 = newY;
                }

                if (newY * y2 > 0)
                {
                    x2 = newX;
                    y2 = newY;
                }

                if (Math.Abs(newY) < tol)
                {
                    root = newX;
                    return true;
                }
            }

            return false;
        }

        private static bool GetNextDifferentiateGuessLinear(Func1D f, double xMin, double yMin, double xMax, double yMax,
            out double newLocation, out double newValue)
        {
            var x1 = xMin;
            var y1 = yMin;

            var x2 = xMax;
            var y2 = yMax;


            if (y1 * y2 > 0)
                throw new InvalidOperationException();

            newLocation = 0;
            newValue = 0;

            if (y2.Equals(y1))
                return false;

            newLocation = -y1 * (x2 - x1) / (y2 - y1) + x1;
            newValue = f.GetAxialForceDifferentiate(newLocation);

            return true;
        }
    }
}
