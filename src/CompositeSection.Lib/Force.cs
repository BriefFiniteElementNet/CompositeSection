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
    /// Represents a general force which consists of two moments about x and y direction and axial load parallel to x direction.
    /// </summary>
    [Serializable]
    public struct Force
    {
        /// <summary>
        /// Gets the zero force.
        /// </summary>
        /// <value>
        /// A force with zero components.
        /// </value>
        public static Force Zero
        {
            get { return new Force(0, 0, 0); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Force"/> struct.
        /// </summary>
        /// <param name="my">the My.</param>
        /// <param name="mz">The Mz.</param>
        /// <param name="nx">The Nx.</param>
        public Force(double my, double mz, double nx)
        {
            _my = my;
            _mz = mz;
            _nx = nx;
        }

        private double _my;
        private double _mz;
        private double _nx;

        /// <summary>
        /// Gets or sets moment about Y axis.
        /// </summary>
        /// <value>
        /// The moment about y axis.
        /// </value>
        public double My
        {
            get { return _my; }
            set { _my = value; }
        }

        /// <summary>
        /// Gets or sets the moment about Z axis.
        /// </summary>
        /// <value>
        /// The moment about x axis.
        /// </value>
        public double Mz
        {
            get { return _mz; }
            set { _mz = value; }
        }

        /// <summary>
        /// Gets or sets the axial force in x axis.
        /// </summary>
        /// <value>
        /// The normal force parallel to x direction.
        /// </value>
        public double Nx
        {
            get { return _nx; }
            set { _nx = value; }
        }

        /// <summary>
        /// Sums the specified forces.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        /// <returns>f1 + f2</returns>
        public static Force Sum(Force f1, Force f2)
        {
            return new Force(f1.My + f2.My, f1.Mz + f2.Mz, f1.Nx + f2.Nx);
        }

        /// <summary>
        /// Subtracts the specified forces.
        /// </summary>
        /// <param name="f1">The f1.</param>
        /// <param name="f2">The f2.</param>
        /// <returns>f1 - f2</returns>
        public static Force Subtract(Force f1, Force f2)
        {
            return new Force(f1.My - f2.My, f1.Mz - f2.Mz, f1.Nx - f2.Nx);
        }

        public bool Equals(Force other)
        {
            return _my.Equals(other._my) && _mz.Equals(other._mz) && _nx.Equals(other._nx);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Force && Equals((Force) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _my.GetHashCode();
                hashCode = (hashCode*397) ^ _mz.GetHashCode();
                hashCode = (hashCode*397) ^ _nx.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Force left, Force right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Force left, Force right)
        {
            return !left.Equals(right);
        }
    }
}
