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
    /// Represents stiffness!
    /// </summary>
    public struct Stiffness
    {
        public static Stiffness Zero
        {
            get
            {
                return new Stiffness();
            }
        }

        #region fields

        private double _rnxRky;
        private double _rnxRkz;
        private double _rnxRe0;
        private double _rmzRky;
        private double _rmzRkz;
        private double _rmzRe0;
        private double _rmyRky;
        private double _rmyRkz;
        private double _rmyRe0;

        #endregion

        #region properties

        public double RnxRky
        {
            get { return _rnxRky; }
            set { _rnxRky = value; }
        }

        public double RnxRkz
        {
            get { return _rnxRkz; }
            set { _rnxRkz = value; }
        }

        public double RnxRe0
        {
            get { return _rnxRe0; }
            set { _rnxRe0 = value; }
        }

        public double RmzRky
        {
            get { return _rmzRky; }
            set { _rmzRky = value; }
        }

        public double RmzRkz
        {
            get { return _rmzRkz; }
            set { _rmzRkz = value; }
        }

        public double RmzRe0
        {
            get { return _rmzRe0; }
            set { _rmzRe0 = value; }
        }

        public double RmyRky
        {
            get { return _rmyRky; }
            set { _rmyRky = value; }
        }

        public double RmyRkz
        {
            get { return _rmyRkz; }
            set { _rmyRkz = value; }
        }

        public double RmyRe0
        {
            get { return _rmyRe0; }
            set { _rmyRe0 = value; }
        }

        #endregion

        public static Stiffness Sum(Stiffness s1, Stiffness s2)
        {
            var buf = new Stiffness();

            buf.RmyRe0 = s1.RmyRe0 + s2.RmyRe0;
            buf.RmyRky = s1.RmyRky + s2.RmyRky;
            buf.RmyRkz = s1.RmyRkz + s2.RmyRkz;

            buf.RmzRe0 = s1.RmzRe0 + s2.RmzRe0;
            buf.RmzRky = s1.RmzRky + s2.RmzRky;
            buf.RmzRkz = s1.RmzRkz + s2.RmzRkz;

            buf.RnxRe0 = s1.RnxRe0 + s2.RnxRe0;
            buf.RnxRky = s1.RnxRky + s2.RnxRky;
            buf.RnxRkz = s1.RnxRkz + s2.RnxRkz;

            return buf;
        }

        public static Stiffness Subtract(Stiffness s1, Stiffness s2)
        {
            var buf = new Stiffness();

            buf.RmyRe0 = s1.RmyRe0 - s2.RmyRe0;
            buf.RmyRky = s1.RmyRky - s2.RmyRky;
            buf.RmyRkz = s1.RmyRkz - s2.RmyRkz;

            buf.RmzRe0 = s1.RmzRe0 - s2.RmzRe0;
            buf.RmzRky = s1.RmzRky - s2.RmzRky;
            buf.RmzRkz = s1.RmzRkz - s2.RmzRkz;

            buf.RnxRe0 = s1.RnxRe0 - s2.RnxRe0;
            buf.RnxRky = s1.RnxRky - s2.RnxRky;
            buf.RnxRkz = s1.RnxRkz - s2.RnxRkz;

            return buf;
        }

        public static Stiffness DotDivide(Stiffness s1, Stiffness s2)
        {
            var buf = new Stiffness();

            buf.RmyRe0 = s1.RmyRe0 / s2.RmyRe0;
            buf.RmyRky = s1.RmyRky / s2.RmyRky;
            buf.RmyRkz = s1.RmyRkz / s2.RmyRkz;

            buf.RmzRe0 = s1.RmzRe0 / s2.RmzRe0;
            buf.RmzRky = s1.RmzRky / s2.RmzRky;
            buf.RmzRkz = s1.RmzRkz / s2.RmzRkz;

            buf.RnxRe0 = s1.RnxRe0 / s2.RnxRe0;
            buf.RnxRky = s1.RnxRky / s2.RnxRky;
            buf.RnxRkz = s1.RnxRkz / s2.RnxRkz;

            return buf;
        }
    }
}