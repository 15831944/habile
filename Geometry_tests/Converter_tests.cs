using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Converter_tests
    {

        [TestMethod]
        public void Converter_xy_to_la_test1()
        {
            Vector v = new Vector(5, 10);
            Polar pol = Converter.xy_to_la(v);

            Assert.AreEqual(pol.L, 11.1803, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, 1.1071, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_xy_to_la_test2()
        {
            Vector v = new Vector(-5, 10);
            Polar pol = Converter.xy_to_la(v);

            Assert.AreEqual(pol.L, 11.1803, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, Math.PI - 1.1071, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_xy_to_la_test3()
        {
            Vector v = new Vector(-5, -10);
            Polar pol = Converter.xy_to_la(v);

            Assert.AreEqual(pol.L, 11.1803, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, -Math.PI + 1.1071, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_xy_to_la_test4()
        {
            Vector v = new Vector(5, -10);
            Polar pol = Converter.xy_to_la(v);

            Assert.AreEqual(pol.L, 11.1803, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(pol.angle, -1.1071, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_la_to_xy_test1()
        {
            Polar pol = new Polar(11.1803, 1.1071);
            Vector v = Converter.la_to_xy(pol);

            Assert.AreEqual(v.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 10, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_la_to_xy_test2()
        {
            Polar pol = new Polar(11.1803, Math.PI - 1.1071);
            Vector v = Converter.la_to_xy(pol);

            Assert.AreEqual(v.X, -5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 10, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_la_to_xy_test3()
        {
            Polar pol = new Polar(11.1803, -Math.PI + 1.1071);
            Vector v = Converter.la_to_xy(pol);

            Assert.AreEqual(v.X, -5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, -10, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_la_to_xy_test4()
        {
            Polar pol = new Polar(11.1803, -1.1071);
            Vector v = Converter.la_to_xy(pol);

            Assert.AreEqual(v.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, -10, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_Wrap_test1()
        {
            double pii = Math.PI / 2;
            double pii2 = Converter.Wrap(pii);
            Assert.AreEqual(pii2, Math.PI/2, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_Wrap_test2()
        {
            double pii = Math.PI / 2 + (Math.PI * 4);
            double pii2 = Converter.Wrap(pii);
            Assert.AreEqual(pii2, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_Wrap_test3()
        {
            double pii = -Math.PI / 2 + (Math.PI * 4);
            double pii2 = Converter.Wrap(pii);
            Assert.AreEqual(pii2, -Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_Wrap_test4()
        {
            double pii = Math.PI / 2 + Math.PI;
            double pii2 = Converter.Wrap(pii);
            Assert.AreEqual(pii2, -Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_Wrap_test5()
        {
            double pii = Math.PI / 2 + Math.PI;
            double pii2 = Converter.Wrap(pii);
            Assert.AreEqual(pii2, -Math.PI / 2, _Variables.EQUALS_TOLERANCE);

            double pii3 = Converter.Wrap(pii2, Math.PI * 2, 0.0);
            Assert.AreEqual(pii3, Math.PI / 2 + Math.PI, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_DeltaAngle_test1()
        {
            Polar pol1 = new Polar(2.0, 0.0);
            Polar pol2 = new Polar(2.0, 3.0);

            double delta = Converter.AngleDelta(pol1.angle, pol2.angle);

            Assert.AreEqual(delta, 3.0, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_DeltaAngle_test2()
        {
            Polar pol1 = new Polar(2.0, 0.0);
            Polar pol2 = new Polar(2.0, -3.0);

            double delta = Converter.AngleDelta(pol1.angle, pol2.angle);

            Assert.AreEqual(delta, 3.0, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_DeltaAngle_test3()
        {
            Polar pol1 = new Polar(2.0, 3.0);
            Polar pol2 = new Polar(2.0, -3.0);

            double delta = Converter.AngleDelta(pol1.angle, pol2.angle);

            Assert.AreEqual(delta, 0.284, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_DeltaAngle_test4()
        {
            Polar pol1 = new Polar(2.0, 1.0);
            Polar pol2 = new Polar(2.0, 2.0707 + Math.PI / 2);

            double delta = Converter.AngleDelta(pol1.angle - Math.PI, pol2.angle);

            Assert.AreEqual(delta, 0.5, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_AngleDeltaClockwise_test1()
        {
            Vector v1 = new Vector(1.0, 0.0);
            Vector v2 = new Vector(-1.0, -1.0);

            double delta = Converter.AngleDeltaCW(v1, v2);

            Assert.AreEqual(delta, 3 * Math.PI / 4, _Variables.EQUALS_TOLERANCE);
        }

        [TestMethod]
        public void Converter_AngleDeltaClockwise_test2()
        {
            Vector v1 = new Vector(-1.0, -1.0);
            Vector v2 = new Vector(1.0, 0.0);

            double delta = Converter.AngleDeltaCW(v1, v2);

            Assert.AreEqual(delta, 5 * Math.PI / 4, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Converter_ToDeg_test1()
        {
            Polar pol = new Polar(2.0, 2.0);

            double deg = Converter.ToDeg(pol.angle);

            Assert.AreEqual(deg, 114.591559, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Converter_ToDeg_test2()
        {
            Polar pol = new Polar(2.0, Math.PI / 2 + Math.PI);

            double deg = Converter.ToDeg(pol.angle);
            Assert.AreEqual(deg, -90, _Variables.EQUALS_TOLERANCE);

            double deg2 = Converter.ToDeg(Converter.Wrap(pol.angle, Math.PI * 2, 0));
            Assert.AreEqual(deg2, 270, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void AngleHandle_delta_test_1()
        {
            double a1 = Math.PI / 4;
            double a2 = Math.PI / 2 + Math.PI / 4;

            double deltaCW = Converter.AngleDeltaCW(a1, a2);
            double deltaCCW = Converter.AngleDeltaCCW(a1, a2);
            double deltaMin = Converter.AngleDelta(a1, a2);

            Assert.AreEqual(deltaCW, 2 * Math.PI - Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaCCW, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaMin, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void AngleHandle_delta_test_2()
        {
            double a1 = Math.PI / 2 + Math.PI / 4;
            double a2 = Math.PI / 4;

            double deltaCW = Converter.AngleDeltaCW(a1, a2);
            double deltaCCW = Converter.AngleDeltaCCW(a1, a2);
            double deltaMin = Converter.AngleDelta(a1, a2);

            Assert.AreEqual(deltaCW, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaCCW, 2 * Math.PI - Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaMin, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void AngleHandle_delta_test_3()
        {
            Vector v1 = new Vector(1, 1);
            Vector v2 = new Vector(-1, 1);

            double deltaCW = Converter.AngleDeltaCW(v1, v2);
            double deltaCCW = Converter.AngleDeltaCCW(v1, v2);
            double deltaMin = Converter.AngleDelta(v1, v2);

            Assert.AreEqual(deltaCW, 2 * Math.PI - Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaCCW, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaMin, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void AngleHandle_delta_test_4()
        {
            Vector v1 = new Vector(-1, 1);
            Vector v2 = new Vector(1, 1);

            double deltaCW = Converter.AngleDeltaCW(v1, v2);
            double deltaCCW = Converter.AngleDeltaCCW(v1, v2);
            double deltaMin = Converter.AngleDelta(v1, v2);

            Assert.AreEqual(deltaCW, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaCCW, 2 * Math.PI - Math.PI / 2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(deltaMin, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }
    }
}