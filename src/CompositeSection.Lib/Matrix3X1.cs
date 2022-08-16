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



namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a structure for a 3x1 matrix (vector with length of 3)
    /// </summary>
    /// <remarks>
    /// Order of members in 3x3 matrix is:
    /// 
    /// +   +
    /// | A |
    /// | B |
    /// | C |
    /// +   +
    /// </remarks>
    public struct Matrix3X1
    {
        #region Members

        /// <summary>
        /// First member
        /// </summary>
        public double A;

        /// <summary>
        /// Second member
        /// </summary>
        public double B;

        /// <summary>
        /// Third member
        /// </summary>
        public double C;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the vector as an array.
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new[] { A, B, C };
        }


        /// <summary>
        /// Adds specified vectors.
        /// </summary>
        /// <param name="a">the a.</param>
        /// <param name="b">The b.</param>
        /// <returns>a+b</returns>
        public static Matrix3X1 Add(Matrix3X1 a, Matrix3X1 b)
        {
            return new Matrix3X1() { A = a.A + b.A, B = a.B + b.B, C = a.C + b.C };
        }

        /// <summary>
        /// Subtracts specified vectors.
        /// </summary>
        /// <param name="a">the a.</param>
        /// <param name="b">The b.</param>
        /// <returns>a-b</returns>
        public static Matrix3X1 Subtract(Matrix3X1 a, Matrix3X1 b)
        {
            return new Matrix3X1() { A = a.A - b.A, B = a.B - b.B, C = a.C - b.C };
        }

        /// <summary>
        /// Negates the specified matrix.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>-a</returns>
        public static Matrix3X1 Negate(Matrix3X1 a)
        {
            return new Matrix3X1() { A = -a.A, B = -a.B, C = -a.C };
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X1 operator +(Matrix3X1 a, Matrix3X1 b)
        {
            return Add(a, b);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X1 operator -(Matrix3X1 a, Matrix3X1 b)
        {
            return Subtract(a, b);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X1 operator -(Matrix3X1 a)
        {
            return Negate(a);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3X1"/> struct.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <param name="c">c.</param>
        public Matrix3X1(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }


        #region Equality members

        public bool Equals(Matrix3X1 other)
        {
            return A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Matrix3X1 && Equals((Matrix3X1)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ C.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Matrix3X1 left, Matrix3X1 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3X1 left, Matrix3X1 right)
        {
            return !left.Equals(right);
        }

        #endregion

    }
}
