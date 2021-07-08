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
    /// Represents a collection of <see cref="FiberElement"/>s.
    /// </summary>
    [Serializable]
    public class FiberElementCollection : ElementCollection<FiberElement>
    {
        /// <inheritdoc />
        public override ElementCollection<FiberElement> DeepClone()
        {
            var buf = new FiberElementCollection();

            foreach (var elm in this)
            {
                buf.Add(elm.Clone() as FiberElement);
            }

            return buf;
        }
    }
}