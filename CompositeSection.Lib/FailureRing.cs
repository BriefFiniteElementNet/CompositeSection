using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompositeSection.Lib
{
    [Serializable]
    public class FailureRing
    {
        /// <summary>
        /// the points that forms the ring
        /// </summary>
        public List<FailurePoint> Points;

        /// <summary>
        /// axial force that targeted when creating the ring
        /// </summary>
        public double TargetedAxialForce;
    }
}
