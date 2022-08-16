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
//    Ehsan Mohammad Ali (ehsan.ma@gmx.com) (Main Author, Done as masters thesis)
// </developers>
#endregion



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a class for calculating critical strain for every section.
    /// </summary>
    public static class CriticalStrainCalculator
    {
        /// <summary>
        /// Represents a structure for two dimensional point in Lambda-Epsi1on system
        /// </summary>
        [DebuggerDisplay("{Lambda},{Epsi1on}")]
        public struct PointLambdaEpsi1on
        {
            public double Lambda;
            public double Epsi1on;
        }

        /// <summary>
        /// Calculates all critical ranges for section in <see cref="deltaTeta"/> intervals.
        /// </summary>
        /// <param name="sec">The sec.</param>
        /// <param name="deltaTeta">The delta teta.</param>
        /// <returns></returns>
        public static List<List<CriticalStrainRange>> CalculateForSection(Section sec, double deltaTeta)
        {
            var teta = 0.0;
            var buf = new List<List<CriticalStrainRange>>();
            var shot = UltimateFibersSnapshot.Create(sec);

            while (teta <= 180.0)
            {
                var b2 = new List<CriticalStrainRange>();

                buf.Add(b2);

                var t = teta*Math.PI/180.0;

                var sin = Math.Sin(t);
                var cos = Math.Cos(t);

                b2.AddRange(Calculate(shot, sin, cos));

                teta += deltaTeta;
            }

            return buf;
        }

        /// <summary>
        /// Calculates a set of failure strain ranges regarding specified direction
        /// </summary>
        /// <param name="snapshot">The ultimate fibers snapshot.</param>
        /// <param name="sin">The sin of angle.</param>
        /// <param name="cos">The cos of angle.</param>
        /// <returns></returns>
        public static List<CriticalStrainRange> Calculate(UltimateFibersSnapshot snapshot, double sin, double cos)
        {
            var tops = new List<PointLambdaEpsi1on>();
            var bots = new List<PointLambdaEpsi1on>();

            for (var i = 0; i < snapshot.PressureSensitiveFibers.Count; i++)
            {
                var p = new PointLambdaEpsi1on();

                var fib = snapshot.PressureSensitiveFibers[i];

                p.Lambda = -sin*fib.Y + cos*fib.Z;
                p.Epsi1on = snapshot.PressureSensitiveHeights[i];

                bots.Add(p);
            }

            for (var i = 0; i < snapshot.TensionSensitiveFibers.Count; i++)
            {
                var p = new PointLambdaEpsi1on();

                var fib = snapshot.TensionSensitiveFibers[i];

                p.Lambda = -sin*fib.Y + cos*fib.Z;
                p.Epsi1on = snapshot.TensionSensitiveHeights[i];

                tops.Add(p);
            }


            if (!snapshot.TensionSensitiveFibers.Any() || !snapshot.PressureSensitiveFibers.Any())
            {
                throw new Exception();
            }

            var allTops = tops.OrderBy(i => i.Lambda).ToList();
            var allBots = bots.OrderBy(i => i.Lambda).ToList();

            var newTops = new List<PointLambdaEpsi1on>();
            var newBots = new List<PointLambdaEpsi1on>();


            var geoTol = 1e-10;

            #region tops distinction

            //برای حذف تقاطی که دارای یک لاندا ولی اپسیـ1ـون متفاوت هستند
            for (var i = 0; i < allTops.Count ; i++)
            {
                if (newTops.Any(j => MathUtil.Equals(j.Lambda, allTops[i].Lambda, geoTol)))
                {
                    var idx = newTops.FindIndex(j => MathUtil.Equals(j.Lambda, allTops[i].Lambda, geoTol));

                    var npt = new PointLambdaEpsi1on();

                    npt.Lambda = allTops[i].Lambda;
                    npt.Epsi1on = Math.Min(newTops[idx].Epsi1on, allTops[i].Epsi1on);

                    newTops[idx] = npt;
                }
                else
                {
                    newTops.Add(allTops[i]);
                }
            }

            #endregion

            #region bots distinction

            //برای حذف تقاطی که دارای یک لاندا ولی اپسیـ1ـون متفاوت هستند
            for (var i = 0; i < allBots.Count ; i++)
            {
                if (newBots.Any(j => MathUtil.Equals(j.Lambda, allBots[i].Lambda, geoTol)))
                {
                    var idx = newBots.FindIndex(j => MathUtil.Equals(j.Lambda, allBots[i].Lambda, geoTol));

                    var npt = new PointLambdaEpsi1on();

                    npt.Lambda = allBots[i].Lambda;
                    npt.Epsi1on = Math.Max(newBots[idx].Epsi1on, allBots[i].Epsi1on);

                    newBots[idx] = npt;
                }
                else
                {
                    newBots.Add(allBots[i]);
                }
            }

            #endregion

            var buf = new List<CriticalStrainRange>();

            for (var i = 0; i < newTops.Count; i++)
            {
                var hng = newTops[i];

                var rng = GenerateForHing(newTops, newBots, newTops[i]);

                if (rng != null)
                {
                    var topsIdx =
                        tops.FindIndex(
                            (j, k) =>
                                MathUtil.Equals(k.Lambda, hng.Lambda, geoTol) &&
                                snapshot.TensionSensitiveHeights[j] == hng.Epsi1on);

                    if (topsIdx == -1)
                        throw new Exception();

                    rng.HingHeight = snapshot.TensionSensitiveHeights[topsIdx];
                    rng.HingePosition = snapshot.TensionSensitiveFibers[topsIdx];
                    rng.Sin = sin;
                    rng.Cos = cos;

                    buf.Add(rng);
                }
            }

            for (var i = 0; i < newBots.Count; i++)
            {
                var hng = newBots[i];

                var rng = GenerateForHing(newTops, newBots, newBots[i]);

                if (rng != null)
                {
                    var botsIdx =
                        bots.FindIndex(
                            (j, k) =>
                                MathUtil.Equals(k.Lambda, hng.Lambda, geoTol) &&  snapshot.PressureSensitiveHeights[j] == hng.Epsi1on);

                    if (botsIdx == -1)
                    {
                        //continue;
                        throw new Exception();
                    }

                    rng.HingHeight = snapshot.PressureSensitiveHeights[botsIdx];
                    rng.HingePosition = snapshot.PressureSensitiveFibers[botsIdx];
                    rng.Sin = sin;
                    rng.Cos = cos;

                    buf.Add(rng);
                }
            }


            var bufs = new List<CriticalStrainRange>();

            foreach (var c in buf)
            {
                if (!MathUtil.Equals(c.MaximumSlope, c.MinimumSlope, 1e-6))
                    bufs.Add(c);
                else
                {
                    
                }
            }

            return bufs;
        }


        /// <summary>
        /// Generates the critical range for a hinge which is on top or bottom.
        /// </summary>
        /// <param name="tops">All the top hinges.</param>
        /// <param name="bots">All the bottom hinges.</param>
        /// <param name="i">The number of top hinge.</param>
        /// <returns>generated range</returns>
        public static CriticalStrainRange GenerateForHing(List<PointLambdaEpsi1on> tops,
            List<PointLambdaEpsi1on> bots, PointLambdaEpsi1on fib)
        {
            //var fib = tops[i];

            var lh = double.MaxValue;
            var rh = double.MaxValue;

            var ll = double.MinValue;
            var rl = double.MinValue;

            if (bots.Any(j => j.Lambda < fib.Lambda))
                lh = bots.Where(j => j.Lambda < fib.Lambda)
                    .Select(j => (fib.Epsi1on - j.Epsi1on)/(fib.Lambda - j.Lambda))
                    .Min();

            if (tops.Any(j => j.Lambda < fib.Lambda))
                ll = tops.Where(j => j.Lambda < fib.Lambda)
                    .Select(j => (fib.Epsi1on - j.Epsi1on)/(fib.Lambda - j.Lambda))
                    .Max();


            if (tops.Any(j => j.Lambda > fib.Lambda))
                rh = tops.Where(j => j.Lambda > fib.Lambda)
                    .Select(j => (fib.Epsi1on - j.Epsi1on)/(fib.Lambda - j.Lambda))
                    .Min();

            if (bots.Any(j => j.Lambda > fib.Lambda))
                rl = bots.Where(j => j.Lambda > fib.Lambda)
                    .Select(j => (fib.Epsi1on - j.Epsi1on)/(fib.Lambda - j.Lambda))
                    .Max();

            var h = Math.Min(rh, lh);
            var l = Math.Max(rl, ll);

            var buf = new CriticalStrainRange();

            buf.MaximumSlope = h;
            buf.MinimumSlope = l;

            if (MathUtil.Equals(h, l, 1e-10))
                return null;

            if (h <= l)
                return null;

            if (Math.Abs(h) > 100 || Math.Abs(l) > 100)
                Guid.NewGuid();
            
             return buf;
        }
    }
}