using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Corner_tests
    {
        [TestMethod]
        public void Corner_init_test_1()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(10, 10);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, a, b);

            Assert.AreEqual(c.Angle, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Corner_init_test_2()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(10, 10);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, b, a);

            Assert.AreEqual(c.Angle, Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Corner_init_test_3()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(20, 10);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, a, b);

            Assert.AreEqual(c.Angle, Math.PI / 2 + Math.PI / 4, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Corner_init_test_4()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(0, 10);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, a, b);

            Assert.AreEqual(c.Angle, Math.PI / 4, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Corner_init_test_5()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(20, 0);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, a, b);

            Assert.AreEqual(c.Angle, Math.PI, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Corner_init_test_6()
        {
            //must provide valid edges
            Point a1 = new Point(0, 0);
            Point a2 = new Point(10, 0);
            Line al = new Line(a1, a2);
            Edge a = new Edge(al);

            Point b1 = new Point(10, 0);
            Point b2 = new Point(10, -10);
            Line bl = new Line(b1, b2);
            Edge b = new Edge(bl);

            Corner c = new Corner(a2, a, b);

            Assert.AreEqual(c.Angle, Math.PI + Math.PI / 2, _Variables.EQUALS_TOLERANCE);
        }
    }
}
