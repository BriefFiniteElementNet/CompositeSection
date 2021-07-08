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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a point in Y-Z coordination system
    /// </summary>
    [DebuggerDisplay("{Y}, {Z}")]
    [Serializable]
    public struct Point:ISerializable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> structure.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="z">The Z.</param>
        public Point(double y, double z)
        {
            Z = z;
            Y = y;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The Z component
        /// </summary>
        public double Z;

        /// <summary>
        /// The Y Component
        /// </summary>
        public double Y;

        #endregion

        #region Equality Suff

        public bool Equals(Point other)
        {
            return Z.Equals(other.Z) && Y.Equals(other.Y);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Point && Equals((Point) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Z.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        #endregion

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }

        private Point(SerializationInfo info, StreamingContext context)
        {
            Y = info.GetDouble("Y");
            Z = info.GetDouble("Z");
        }


    }
}