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

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents ultimate pressure and tension fibers location and height of each one.
    /// </summary>
    public class UltimateFibersSnapshot
    {
        public List<Point> TensionSensitiveFibers = new List<Point>();
        public List<double> TensionSensitiveHeights = new List<double>();

        public List<Point> PressureSensitiveFibers = new List<Point>();
        public List<double> PressureSensitiveHeights = new List<double>();

        public static UltimateFibersSnapshot Create(Section sec)
        {
            var buf = new UltimateFibersSnapshot();

            foreach (var elm in sec.SurfaceElements)
            {
                #region foreground

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.TensionSensitiveFibers.Add(pt);
                            buf.TensionSensitiveHeights.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);
                        }
                    }

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.PressureSensitiveFibers.Add(pt);
                            buf.PressureSensitiveHeights.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                        }
                    }
                }

                #endregion

                #region background

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.TensionSensitiveFibers.Add(pt);
                            buf.TensionSensitiveHeights.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);
                        }
                    }

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.PressureSensitiveFibers.Add(pt);
                            buf.PressureSensitiveHeights.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                        }
                    }
                }

                #endregion
            }

            foreach (var elm in sec.PolyLineElements)
            {
                #region foreground

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.TensionSensitiveFibers.Add(pt);
                            buf.TensionSensitiveHeights.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);
                        }
                    }

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.PressureSensitiveFibers.Add(pt);
                            buf.PressureSensitiveHeights.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                        }
                    }
                }

                #endregion

                #region background

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.TensionSensitiveFibers.Add(pt);
                            buf.TensionSensitiveHeights.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);
                        }
                    }

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        foreach (var pt in elm.Points)
                        {
                            buf.PressureSensitiveFibers.Add(pt);
                            buf.PressureSensitiveHeights.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                        }
                    }
                }

                #endregion
            }

            foreach (var elm in sec.FiberElements)
            {
                #region foreground

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        buf.TensionSensitiveFibers.Add(elm.Center);
                        buf.TensionSensitiveHeights.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);
                    }

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        buf.PressureSensitiveFibers.Add(elm.Center);
                        buf.PressureSensitiveHeights.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                    }
                }

                #endregion

                #region background

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                    {
                        buf.TensionSensitiveFibers.Add(elm.Center);
                        buf.TensionSensitiveHeights.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);
                    }

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                    {
                        buf.PressureSensitiveFibers.Add(elm.Center);
                        buf.PressureSensitiveHeights.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                    }
                }

                #endregion
            }

            if (!buf.TensionSensitiveFibers.Any() || !buf.PressureSensitiveFibers.Any())
            {
                throw new Exception();
            }

            return buf;
        }
    }
}