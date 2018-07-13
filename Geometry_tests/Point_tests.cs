using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Point_tests
    {

        [TestMethod]
        public void Point_init_test()
        {
            Point a = new Point(1, 2);
            Assert.AreEqual(a.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(a.Y, 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_equals_test1()
        {
            Point a = new Point(1, 2);
            Point b = new Point(1, 2);
            Assert.IsTrue(a == b);
        }


        [TestMethod]
        public void Point_equals_test2()
        {
            Point a = new Point(1, 2);
            Point b = new Point(2, 2);
            Assert.IsFalse (a == b);
        }


        [TestMethod]
        public void Point_equals_test3()
        {
            Point a = new Point(1, 2);
            Point b = new Point(1, 2);
            Assert.IsFalse(a != b);
        }


        [TestMethod]
        public void Point_equals_test4()
        {
            Point a = new Point(1, 2);
            Point b = new Point(2, 2);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void Point_equals_test5()
        {
            Point a = new Point(1.000001, 1.9999);
            Point b = new Point(1, 2);
            Assert.IsTrue(a == b);
        }


        [TestMethod]
        public void Point_move_test1()
        {
            Point a = new Point(1, 2);
            Vector v = new Vector(1, 0);
            double d = 3;

            Point n = a.move(d, v);
            Assert.AreEqual(n.X, 4, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test2()
        {
            Point a = new Point(1, 2);
            Vector v = new Vector(-1, 0);
            double d = 3;

            Point n = a.move(d, v);
            Assert.AreEqual(n.X, -2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, 2, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test3()
        {
            Point a = new Point(1, 2);
            Vector v = new Vector(0, 1);
            double d = 3;

            Point n = a.move(d, v);
            Assert.AreEqual(n.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, 5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test4()
        {
            Point a = new Point(1, 2);
            Vector v = new Vector(0, -1);
            double d = 3;

            Point n = a.move(d, v);
            Assert.AreEqual(n.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, -1, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test5()
        {
            Point a = new Point(1, 1);
            Vector v = new Vector(1, 1);
            double d = 3;

            Point n = a.move(d, v);

            Assert.AreEqual(n.X, 3.121, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, 3.121, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test6()
        {
            Point a = new Point(1, 1);
            Vector v = new Vector(-1, 1);
            double d = 3;

            Point n = a.move(d, v);

            Assert.AreEqual(n.X, -1.121, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, 3.121, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test7()
        {
            Point a = new Point(1, 1);
            Vector v = new Vector(-1, -1);
            double d = 3;

            Point n = a.move(d, v);

            Assert.AreEqual(n.X, -1.121, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, -1.121, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_move_test8()
        {
            Point a = new Point(1, 1);
            Vector v = new Vector(1, -1);
            double d = 3;

            Point n = a.move(d, v);

            Assert.AreEqual(n.X, 3.121, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(n.Y, -1.121, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_distance_test1()
        {
            Point a = new Point(1, 1);
            Point b = new Point(2, 2);

            double dist = a.distanceTo(b);

            Assert.AreEqual(dist, 1.414, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_distance_test2()
        {
            Point a = new Point(-1, -1);
            Point b = new Point(2, 2);

            double dist = a.distanceTo(b);

            Assert.AreEqual(dist, 4.243, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Point_distance_test3()
        {
            Point a = new Point(1, -1);
            Point b = new Point(2, 2);

            double dist = a.distanceTo(b);

            Assert.AreEqual(dist, 3.162, _Variables.EQUALS_TOLERANCE);
        }

    }
}