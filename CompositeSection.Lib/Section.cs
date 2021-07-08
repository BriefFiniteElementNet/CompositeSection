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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace CompositeSection.Lib
{
    /// <summary>
    /// Represents a composite section
    /// </summary>
    [Serializable]
    public class Section :ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        public Section()
        {
            _fiberElements = new FiberElementCollection();
            _surfaceElements = new SurfaceElementCollection();
            _polyLineElements = new PolyLineElementCollection();
        }

        private FiberElementCollection _fiberElements;
        private SurfaceElementCollection _surfaceElements;
        private PolyLineElementCollection _polyLineElements;

        /// <summary>
        /// Gets or sets the fiber elements.
        /// </summary>
        /// <value>
        /// The fiber elements.
        /// </value>
        public FiberElementCollection FiberElements
        {
            get { return _fiberElements; }
            set { _fiberElements = value; }
        }

        /// <summary>
        /// Gets or sets the surface elements.
        /// </summary>
        /// <value>
        /// The surface elements.
        /// </value>
        public SurfaceElementCollection SurfaceElements
        {
            get { return _surfaceElements; }
            set { _surfaceElements = value; }
        }

        /// <summary>
        /// Gets or sets the polyline elements.
        /// </summary>
        /// <value>
        /// The polyline elements.
        /// </value>
        public PolyLineElementCollection PolyLineElements
        {
            get { return _polyLineElements; }
            set { _polyLineElements = value; }
        }


        /// <summary>
        /// Gets the section forces regarding <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns>The section Force</returns>
        public Force GetSectionForces(StrainProfile strain)
        {
            if (!string.IsNullOrEmpty(ValidateStrain(strain)))
                Guid.NewGuid();


            var srf = new Force();
            var pln = new Force();
            var fbr = new Force();


            foreach (var elm in SurfaceElements)
            {
                srf = Force.Sum(srf, elm.GetForce(strain));
            }

            foreach (var elm in PolyLineElements)
            {
                pln = Force.Sum(pln, elm.GetForce(strain));
            }

            foreach (var elm in FiberElements)
            {
                fbr = Force.Sum(fbr, elm.GetForce(strain));
            }

            return Force.Sum(srf, Force.Sum(fbr, pln));
        }

        public string ValidateStrain(StrainProfile profile)
        {
            var failed = 0;
            var failThreshold = 1;

            #region surfaces

            foreach (var elm in SurfaceElements)
            {
                var positiveFailures = new List<double>() {double.MaxValue};
                var negativeFailure = new List<double>() {double.MinValue};

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                }

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                }

                var max = positiveFailures.Min();
                var min = negativeFailure.Max();


                foreach (var pt in elm.Points)
                {
                    var str = profile.GetStrainAt(pt);

                    if (MathUtil.Equals(max, str, 1e-8) || MathUtil.Equals(min, str, 1e-8))
                        failThreshold++;
                    else if (str > max || str < min)
                        failed++;
                }
            }

            #endregion

            #region poly lines

            foreach (var elm in PolyLineElements)
            {
                var positiveFailures = new List<double>() { double.MaxValue };
                var negativeFailure = new List<double>() { double.MaxValue };

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                }

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                }

                var max = positiveFailures.Min();
                var min = negativeFailure.Max();


                foreach (var pt in elm.Points)
                {
                    var str = profile.GetStrainAt(pt);

                    if (MathUtil.Equals(max, str, 1e-8) || MathUtil.Equals(min, str, 1e-8))
                        failThreshold++;
                    else if (str > max || str < min)
                        failed++;
                }
            }

            #endregion

            #region fibers

            foreach (var elm in FiberElements)
            {
                var positiveFailures = new List<double>() {double.MaxValue};
                var negativeFailure = new List<double>() {double.MinValue};

                if (!elm.BackgroundMaterial.IsNullOrEmpty())
                {
                    if (elm.BackgroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.BackgroundMaterial.PositiveFailureStrain.Value);

                    if (elm.BackgroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.BackgroundMaterial.NegativeFailureStrain.Value);
                }

                if (!elm.ForegroundMaterial.IsNullOrEmpty())
                {
                    if (elm.ForegroundMaterial.PositiveFailureStrain.HasValue)
                        positiveFailures.Add(elm.ForegroundMaterial.PositiveFailureStrain.Value);

                    if (elm.ForegroundMaterial.NegativeFailureStrain.HasValue)
                        negativeFailure.Add(elm.ForegroundMaterial.NegativeFailureStrain.Value);
                }

                var max = positiveFailures.Min();
                var min = negativeFailure.Max();


                var str = profile.GetStrainAt(elm.Center);

                if (MathUtil.Equals(max, str, 1e-8) || MathUtil.Equals(min, str, 1e-8))
                    failThreshold++;
                else if (str > max || str < min)
                    failed++;
            }

            #endregion

            if (failed != 0)
                return "failed!";

            return null;
        }

        /// <summary>
        /// Gets the section stiffness regarding <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns>the section stiffness</returns>
        public Stiffness GetSectionStiffness(StrainProfile strain)
        {
            var buf = new Stiffness();

            var srf = new Stiffness();
            var pln = new Stiffness();
            var fbr = new Stiffness();

            foreach (var elm in SurfaceElements)
            {
                srf = Stiffness.Sum(srf, elm.GetStiffness(strain));
            }

            foreach (var elm in PolyLineElements)
            {
                pln = Stiffness.Sum(pln, elm.GetStiffness(strain));
            }

            foreach (var elm in FiberElements)
            {
                fbr = Stiffness.Sum(fbr, elm.GetStiffness(strain));
            }

            return Stiffness.Sum(srf, Stiffness.Sum(fbr, pln));
        }

        /// <summary>
        /// Gets the section axial force regarding <see cref="strain"/>.
        /// </summary>
        /// <param name="strain">The strain.</param>
        /// <returns></returns>
        public double GetSectionAxialForce(StrainProfile strain)
        {
            var buf = 0.0;

            foreach (var elm in SurfaceElements)
            {
                buf = buf + elm.GetAxialForce(strain);
            }

            foreach (var elm in PolyLineElements)
            {
                buf = buf + elm.GetAxialForce(strain);
            }

            foreach (var elm in FiberElements)
            {
                buf = buf + elm.GetAxialForce(strain);
            }

            return buf;
        }

        /// <summary>
        /// Determines whether is this Section a valid section.
        /// </summary>
        /// <param name="message">If is not valid section, this is a message containing further information on why this section is not a valid section.</param>
        /// <returns>true if this is valid section, false otherwise</returns>
        public bool IsValidSection(out string message)
        {
            if (_fiberElements != null)
                foreach (var e in _fiberElements)
                {
                    if (!e.IsValidElement(out message))
                        return false;
                }

            if (_surfaceElements != null)
                foreach (var e in _surfaceElements)
                {
                    if (!e.IsValidElement(out message))
                        return false;
                }

            if (_polyLineElements != null)
                foreach (var e in _polyLineElements)
                {
                    if (!e.IsValidElement(out message))
                        return false;
                }

            message = "";
            return true;
        }


        /// <summary>
        /// Elements should not have common part with each other, because this is usually a bug
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ValidateElementsCollision(out string message)
        {
            //surface elements should not have collision, note that if they touch eachother then no problem

            //polyline elements should not cross edge of surface elements, e.g a polyline element is whole inside a surface element or whole outside surface element
            //not possible to half inside and half outside of a surface element, not that touching is not problem

            
            throw new NotImplementedException();
        }


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>a clone version of this instance</returns>
        public Section Clone()
        {
            var buf = new Section();

            if (this._surfaceElements != null)
                buf._surfaceElements = this._surfaceElements.DeepClone() as SurfaceElementCollection;

            if (this._polyLineElements != null)
                buf._polyLineElements = this._polyLineElements.DeepClone() as PolyLineElementCollection;

            if (this._fiberElements != null)
                buf._fiberElements = this._fiberElements.DeepClone() as FiberElementCollection;


            return buf;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_fiberElements", _fiberElements);
            info.AddValue("_surfaceElements", _surfaceElements);
            info.AddValue("_polyLineElements", _polyLineElements);
        }

        protected Section(SerializationInfo info, StreamingContext context) 
        {
            _fiberElements = (FiberElementCollection)info.GetValue("_fiberElements", typeof(FiberElementCollection));
            _surfaceElements = (SurfaceElementCollection)info.GetValue("_surfaceElements", typeof(SurfaceElementCollection));
            _polyLineElements = (PolyLineElementCollection)info.GetValue("_polyLineElements", typeof(PolyLineElementCollection));
        }
    }
}