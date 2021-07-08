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
    /// Represents a structure for a square 3x3 matrix.
    /// </summary>
    /// <remarks>
    /// Order of members in 3x3 matrix is:
    /// 
    /// +           +
    /// | A   B   C |
    /// | D   E   F |
    /// | G   H   I |
    /// +           +
    /// </remarks>
    public struct Matrix3X3
    {
        #region Members

        public double A, B, C, D, E, F, G, H, I;

        #endregion

        #region Methods

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public double[,] ToArray()
        {
            var buf = new double[3, 3];

            //A B C
            //D E F
            //G H I

            buf[0, 0] = A;
            buf[0, 1] = B;
            buf[0, 2] = C;

            buf[1, 0] = D;
            buf[1, 1] = E;
            buf[1, 2] = F;

            buf[2, 0] = G;
            buf[2, 1] = H;
            buf[2, 2] = I;

            return buf;
        }

        /// <summary>
        /// Multiplies the specified instances.
        /// </summary>
        /// <param name="a">left matrix</param>
        /// <param name="b">right matrix</param>
        /// <returns>a * b</returns>
        public static Matrix3X3 Multiply(Matrix3X3 a, Matrix3X3 b)
        {
            var buf = new Matrix3X3();

            buf.A = a.A * b.A + a.B * b.D + a.C * b.G;
            buf.B = a.A * b.B + a.B * b.E + a.C * b.H;
            buf.C = a.A * b.C + a.B * b.F + a.C * b.I;

            buf.D = a.D * b.A + a.E * b.D + a.F * b.G;
            buf.E = a.D * b.B + a.E * b.E + a.F * b.H;
            buf.F = a.D * b.C + a.E * b.F + a.F * b.I;

            buf.G = a.G * b.A + a.H * b.D + a.I * b.G;
            buf.H = a.G * b.B + a.H * b.E + a.I * b.H;
            buf.I = a.G * b.C + a.H * b.F + a.I * b.I;

            return buf;
        }

        /// <summary>
        /// Multiplies the specified instances.
        /// </summary>
        /// <param name="a">left matrix</param>
        /// <param name="b">right vector</param>
        /// <returns>a * b</returns>
        public static Matrix3X1 Multiply(Matrix3X3 a, Matrix3X1 b)
        {
            var buf = new Matrix3X1();

            buf.A = a.A * b.A + a.B * b.B + a.C * b.C;
            buf.B = a.D * b.A + a.E * b.B + a.F * b.C;
            buf.C = a.G * b.A + a.H * b.B + a.I * b.C;

            return buf;
        }

        /// <summary>
        /// Subtracts the <see cref="b"/> from <see cref="a"/> and returns the result.
        /// </summary>
        /// <param name="a">a!</param>
        /// <param name="b">b!</param>
        /// <returns>a-b</returns>
        public static Matrix3X3 Subtract(Matrix3X3 a, Matrix3X3 b)
        {
            var buf = new Matrix3X3();

            buf.A = a.A - b.A;
            buf.B = a.B - b.B;
            buf.C = a.C - b.C;

            buf.D = a.D - b.D;
            buf.E = a.E - b.E;
            buf.F = a.F - b.F;

            buf.G = a.G - b.G;
            buf.H = a.H - b.H;
            buf.I = a.I - b.I;

            return buf;
        }

        /// <summary>
        /// Sums the <see cref="b"/> and <see cref="a"/> and returns the result.
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>a+b</returns>
        public static Matrix3X3 Sum(Matrix3X3 a, Matrix3X3 b)
        {
            var buf = new Matrix3X3();

            buf.A = a.A + b.A;
            buf.B = a.B + b.B;
            buf.C = a.C + b.C;

            buf.D = a.D + b.D;
            buf.E = a.E + b.E;
            buf.F = a.F + b.F;

            buf.G = a.G + b.G;
            buf.H = a.H + b.H;
            buf.I = a.I + b.I;

            return buf;
        }

        public Matrix3X3 Transpose()
        {
            var buf = new Matrix3X3();

            buf.A = this.A;
            buf.B = this.D;
            buf.C = this.G;

            buf.D = this.B;
            buf.E = this.E;
            buf.F = this.H;

            buf.G = this.C;
            buf.H = this.F;
            buf.I = this.I;

            return buf;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X3 operator *(Matrix3X3 a, Matrix3X3 b)
        {
            return Multiply(a, b);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X1 operator *(Matrix3X3 a, Matrix3X1 b)
        {
            return Multiply(a, b);
        }


        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X3 operator +(Matrix3X3 a, Matrix3X3 b)
        {
            return Sum(a, b);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Matrix3X3 operator -(Matrix3X3 a, Matrix3X3 b)
        {
            return Subtract(a, b);
        }
        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="Matrix3X3" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(Matrix3X3 other)
        {
            return A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C) && D.Equals(other.D) && E.Equals(other.E) && F.Equals(other.F) && G.Equals(other.G) && H.Equals(other.H) && I.Equals(other.I);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Matrix3X3 && Equals((Matrix3X3)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ C.GetHashCode();
                hashCode = (hashCode * 397) ^ D.GetHashCode();
                hashCode = (hashCode * 397) ^ E.GetHashCode();
                hashCode = (hashCode * 397) ^ F.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ H.GetHashCode();
                hashCode = (hashCode * 397) ^ I.GetHashCode();
                return hashCode;
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
        public static bool operator ==(Matrix3X3 left, Matrix3X3 right)
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
        public static bool operator !=(Matrix3X3 left, Matrix3X3 right)
        {
            return !left.Equals(right);
        }
    }
}
