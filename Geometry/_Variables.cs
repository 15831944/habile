using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public static class _Variables
    {
        public static double EQUALS_TOLERANCE = 0.15; //Same point detection and tests
        public static double FLAT_CORNER_TOLERANCE = 0.02; //0.02 rad = 1 deg
        public static double VECTOR_ANGLE_TOLERANCE = 0.002; //0.02 rad = 1 deg
        public static double MOVE_DISTANCE = 0.02; //Detect "inside"
    }
}
