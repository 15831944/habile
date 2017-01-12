using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    public class LineSamePointException : Exception
    {
        public LineSamePointException() { }
        public LineSamePointException(string message) : base(message) { }
        public LineSamePointException(string message, Exception inner) : base(message, inner) { }
    }

    public class LineSameLineException : Exception
    {
        public LineSameLineException() { }
        public LineSameLineException(string message) : base(message) { }
        public LineSameLineException(string message, Exception inner) : base(message, inner) { }
    }

    public class LineNoIntersectionException : Exception
    {
        public LineNoIntersectionException() { }
        public LineNoIntersectionException(string message) : base(message) { }
        public LineNoIntersectionException(string message, Exception inner) : base(message, inner) { }
    }

    public class RegionNotValidException : Exception
    {
        public RegionNotValidException() { }
        public RegionNotValidException(string message) : base(message) { }
        public RegionNotValidException(string message, Exception inner) : base(message, inner) { }
    }

    public class RegionLineNotInRegionException : Exception
    {
        public RegionLineNotInRegionException() { }
        public RegionLineNotInRegionException(string message) : base(message) { }
        public RegionLineNotInRegionException(string message, Exception inner) : base(message, inner) { }
    }

    public class VectorZeroException : Exception
    {
        public VectorZeroException() { }
        public VectorZeroException(string message) : base(message) { }
        public VectorZeroException(string message, Exception inner) : base(message, inner) { }
    }

    public class EdgeNotInDefinedCornerException : Exception
    {
        public EdgeNotInDefinedCornerException() { }
        public EdgeNotInDefinedCornerException(string message) : base(message) { }
        public EdgeNotInDefinedCornerException(string message, Exception inner) : base(message, inner) { }
    }

    public class CornerNotInDefinedEdgeException : Exception
    {
        public CornerNotInDefinedEdgeException() { }
        public CornerNotInDefinedEdgeException(string message) : base(message) { }
        public CornerNotInDefinedEdgeException(string message, Exception inner) : base(message, inner) { }
    }
}
