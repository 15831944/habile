using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Polar_tests
    {

        [TestMethod]
        public void Polar_init_test()
        {
            Polar pol = new Polar(1, 1.0);

            Assert.AreEqual(pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, 1.0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_init_wrapper_test()
        {
            Polar pol = new Polar(1, Math.PI * 6 + 0.3);

            Assert.AreEqual(pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, 0.3, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_init_wrapper_test2()
        {
            Polar pol = new Polar(1, Math.PI * 6 - 0.3);

            Assert.AreEqual(pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, -0.3, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_rotate_test1()
        {
            Polar pol = new Polar(1, 1.0);

            Polar new_pol = pol.rotate(1.0);

            Assert.AreEqual(new_pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_pol.angle, 2.0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_rotate_test2()
        {
            Polar pol = new Polar(1, 1.0);

            Polar new_pol = pol.rotate(-1.0);

            Assert.AreEqual(new_pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_pol.angle, 0.0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_rotate_wrapper_test1()
        {
            Polar pol = new Polar(1, 1.0);

            Polar new_pol = pol.rotate(3.0);

            Assert.AreEqual(new_pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_pol.angle, -2.28318, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Polar_rotate_wrapper_test2()
        {
            Polar pol = new Polar(1, 1.0);

            Polar new_pol = pol.rotate(-5.0);

            Assert.AreEqual(new_pol.L, 1.0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_pol.angle, 2.28318, _Variables.EQUALS_TOLERANCE);
        }

    }
}
