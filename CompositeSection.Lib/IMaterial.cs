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
using System.Runtime.Serialization;

namespace CompositeSection.Lib
{
    /// <summary>
    /// The only feature needed from materials is stress strain curve so this represents an interface for a general uni-axial stress strain curve
    /// </summary>
    [Obsolete()]
    public interface IMaterialObs:ISerializable
    {
        /*
        /// <summary>
        /// Gets the stress in defined <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        double GetStress(double strain);

        /// <summary>
        /// Gets the tangent elastic modulus at specified <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns></returns>
        double GetTangentElasticModulus(double strain);

        /// <summary>
        /// Gets the strains of edges of different regions in stress strain curve, sorted from small one to large one.
        /// </summary>
        /// <returns>The walls of regions</returns>
        double[] GetWalls();

        /// <summary>
        /// Calculates the:
        /// 
        ///    /  z₁
        ///   |
        ///   |  (α.z + β)ʳ zˢ σ(ε₀ + ϕ.z) dz
        ///   |
        ///  /  z₀
        /// 
        /// </summary>
        /// <param name="z0">The y₀</param>
        /// <param name="z1">The y₁</param>
        /// <param name="alpha">The α</param>
        /// <param name="beta">The β</param>
        /// <param name="fi">The ϕ</param>
        /// <param name="e0">The ε₀</param>
        /// <param name="r">The r</param>
        /// <param name="s">The s</param>
        /// <returns>Integration result regarding input parameters</returns>
        double IntegrateStress(double z0, double z1, double alpha, double beta, double fi, double e0, int r, int s);

        /// <summary>
        /// Calculates the:
        ///
        ///    /  z₁
        ///   |
        ///   |  (α.z + β)ʳ zˢ Et(ε₀ + ϕ.z) dz
        ///   |
        ///  /  z₀
        /// 
        /// Where Et is tangent modulus
        /// </summary>
        /// <param name="y0">The y₀</param>
        /// <param name="y1">The y₁</param>
        /// <param name="alpha">The α</param>
        /// <param name="beta">The β</param>
        /// <param name="fi">The ϕ</param>
        /// <param name="e0">The ε₀</param>
        /// <param name="r">The r</param>
        /// <param name="s">The s</param>
        /// <returns>Integration result regarding input parameters</returns>
        double IntegrateTangentElasticModulus(double y0, double y1, double alpha, double beta, double fi, double e0,
            int r, int s);

        /// <summary>
        /// Gets or sets the positive failure strain.
        /// </summary>
        /// <value>
        /// The positive failure strain threshold.
        /// If such a strain doesn't exists, set to null.
        /// </value>
        double? PositiveFailureStrain { set; get; }

        /// <summary>
        /// Gets or sets the negative failure strain.
        /// </summary>
        /// <value>
        /// The negative failure strain threshold.
        /// If such a strain doesn't exists, set to null.
        /// </value>
        double? NegativeFailureStrain { set; get; }


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone instance of this material</returns>
        Material Clone();
        */

    }
}
