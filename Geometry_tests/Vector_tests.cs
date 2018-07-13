using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Vector_tests
    {

        [TestMethod]
        public void Vector_init_test()
        {
            Vector v = new Vector(1, 0);

            Assert.AreEqual(v.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 0, _Variables.EQUALS_TOLERANCE);

            v = new Vector(1, 1);
            Assert.AreEqual(v.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, 1);
            Assert.AreEqual(v.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, -1);
            Assert.AreEqual(v.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, -1, _Variables.EQUALS_TOLERANCE);
        }

        
        [TestMethod]
        public void Vector_rotate_test()
        {
            Vector v = new Vector(1, 0);
            Vector r = v.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(0, 1);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 0, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, 0);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, -1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(0, -1);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 0, _Variables.EQUALS_TOLERANCE);

            v = new Vector(1, 0);
            r = v.rotate(-Math.PI / 2);
            Assert.AreEqual(r.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, -1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(0, -1);
            r = r.rotate(-Math.PI / 2);
            Assert.AreEqual(r.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 0, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, 0);
            r = r.rotate(-Math.PI / 2);
            Assert.AreEqual(r.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(0, -1);
            r = r.rotate(-Math.PI / 2);
            Assert.AreEqual(r.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 0, _Variables.EQUALS_TOLERANCE);

            v = new Vector(1, 1);
            r = v.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, 1);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, -1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, -1);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, -1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(1, -1);
            r = r.rotate(Math.PI / 2);
            Assert.AreEqual(r.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Y, 1, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Vector_distance_test()
        {
            Vector v = new Vector(1, 0);
            double len = v.Length();
            Assert.AreEqual(len, 1, _Variables.EQUALS_TOLERANCE);

            v = new Vector(1, -1);
            len = v.Length();
            Assert.AreEqual(len, 1.414, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-1, -1);
            len = v.Length();
            Assert.AreEqual(len, 1.414, _Variables.EQUALS_TOLERANCE);

            v = new Vector(-2, -2);
            len = v.Length();
            Assert.AreEqual(len, 2.828, _Variables.EQUALS_TOLERANCE);
        }

    }
}
