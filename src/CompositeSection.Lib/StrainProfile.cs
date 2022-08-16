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

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a strain profile which is linearly on section area
    /// Strain on every y,z is calculated by : ε(y,z) = ε₀ + κz . z + κy . y
    /// </summary>
    [Serializable]
    public struct StrainProfile
    {
        public StrainProfile(double kz, double ky, double e0)
        {
            _kz = kz;
            _e0 = e0;
            _ky = ky;
        }

        private double _ky;
        private double _kz;
        private double _e0;

        /// <summary>
        /// Gets or sets the E0.
        /// </summary>
        /// <value>
        /// The ε₀ in ε(y,z) = ε₀ + κz . z + κy . y.
        /// </value>
        public double E0
        {
            get { return _e0; }
            set { _e0 = value; }
        }

        /// <summary>
        /// Gets or sets the Kz.
        /// </summary>
        /// <value>
        /// The κz in ε(y,z) = ε₀ + κz . z + κy . y.
        /// </value>
        public double Kz
        {
            get { return _kz; }
            set { _kz = value; }
        }

        /// <summary>
        /// Gets or sets the Ky.
        /// </summary>
        /// <value>
        /// The κy in ε(y,z) = ε₀ + κz . z + κy . y.
        /// </value>
        public double Ky
        {
            get { return _ky; }
            set { _ky = value; }
        }

        /// <summary>
        /// Gets the strain at specified <see cref="location"/>.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>
        /// strain at specified <see cref="location"/>.
        /// </returns>
        public double GetStrainAt(Point location)
        {
            return _kz*location.Z + _ky*location.Y + _e0;
        }

        /// <summary>
        /// Gets the strain at specified location.
        /// </summary>
        /// <param name="y">The y component of location.</param>
        /// <param name="z">The z component of location.</param>
        /// <returns>strain at specified location.</returns>
        public double GetStrainAt(double y,double  z)
        {
            return _kz*z + _ky*y + _e0;
        }

    }
}
