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
    /// Represents an abstract class for elements who creates the section
    /// </summary>
    [Serializable]
    public abstract class BaseElement:ISerializable
    {
        protected Material _foregroundMaterial;
        protected Material _backgroundMaterial;

        private string _label;

        /// <summary>
        /// Gets or sets the foreground material of this instance.
        /// </summary>
        /// <value>
        /// The material which is forming the foreground of this element.
        /// </value>
        /// <remarks>
        /// Sometimes elements are inside each other. for example rebars as fiber elements inside concrete as surface element.
        /// Actually bar element is occupied concrete so where the rebar exists concrete do not exists. The foreground material for rebars is steel in this case.
        /// </remarks>
        public Material ForegroundMaterial
        {
            get { return _foregroundMaterial; }
            set { _foregroundMaterial = value; }
        }

        /// <summary>
        /// Gets or sets the background material of this instance.
        /// </summary>
        /// <value>
        /// The material which is forming the background of this element.
        /// </value>
        /// <remarks>
        /// Sometimes elements are inside each other. for example rebars as fiber elements inside concrete as surface element.
        /// Actually bar element is occupied concrete so where the rebar exists concrete do not exists. The background material for rebars is concrete in this case.
        /// </remarks>
        public Material BackgroundMaterial
        {
            get { return _backgroundMaterial; }
            set { _backgroundMaterial = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// Determines whether is there any problem with definition of this element or not.
        /// If any problem exists it will further information will be inside <see cref="message"/>
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if no problem found for definition of this instance; <c>false</c> otherwise.</returns>
        public abstract bool IsValidElement(out string message);

        /// <summary>
        /// Gets the internal force of element due to defined <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns>force of element</returns>
        public abstract Force GetForce(StrainProfile strain);

        /// <summary>
        /// Gets the internal stiffness of element due to defined <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns>stiffness of element</returns>
        public abstract Stiffness GetStiffness(StrainProfile strain);

        /// <summary>
        /// Gets the internal axial force of element due to defined <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns>axial force of element</returns>
        public abstract double GetAxialForce(StrainProfile strain);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone version of this instance</returns>
        public abstract BaseElement Clone();

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_foregroundMaterial", _foregroundMaterial);
            info.AddValue("_backgroundMaterial", _backgroundMaterial);
            info.AddValue("_label", _label);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElement"/> class.
        /// Satisfied the ISerializer constructor.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected BaseElement(SerializationInfo info, StreamingContext context)
        {
            _foregroundMaterial = (Material)info.GetValue("_foregroundMaterial", typeof(Material));
            _backgroundMaterial = (Material)info.GetValue("_backgroundMaterial", typeof(Material));
            _label = (string)info.GetValue("_label", typeof(string));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElement"/> class.
        /// </summary>
        public BaseElement()
        {
        }
    }
}
