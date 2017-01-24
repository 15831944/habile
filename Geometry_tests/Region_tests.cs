using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Region_tests
    {
        [TestMethod]
        public void Region_init_test_1()
        {
            List<Line> contours = new List<Line>();

            Point a = new Point(-1, -2);
            Point b = new Point(10, 0);
            Point c = new Point(10, 11);

            Line k = new Line(a, b);
            Line l = new Line(b, c);
            Line m = new Line(c, a);

            contours.Add(k);
            contours.Add(l);
            contours.Add(m);

            Region r = new Region(contours);

            Assert.AreEqual(r.Min_X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Max_X, 10, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Min_Y, -2, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Max_Y, 11, _Variables.EQUALS_TOLERANCE);

            Assert.AreEqual(r.edges.Count, 3);
            Assert.AreEqual(r.corners.Count, 3);
        }

        [TestMethod]
        public void Region_init_test_2()
        {
            List<Line> contours = new List<Line>();

            Point a = new Point(0, 0);
            Point b = new Point(5, 0);
            Point c = new Point(10, 0);
            Point d = new Point(15, 0);
            Point e = new Point(15, 10);
            Point f = new Point(0, 10);

            Line k = new Line(a, b);
            Line l = new Line(b, c);
            Line m = new Line(c, d);
            Line n = new Line(d, e);
            Line o = new Line(e, f);
            Line p = new Line(f, a);

            contours.Add(k);
            contours.Add(l);
            contours.Add(m);
            contours.Add(n);
            contours.Add(o);
            contours.Add(p);

            Region r = new Region(contours);

            Assert.AreEqual(r.edges.Count, 4);
            Assert.AreEqual(r.corners.Count, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(RegionNotValidException))]
        public void Region_init_fail_test()
        {
            List<Line> contours = new List<Line>();

            Point a = new Point(0, 0);
            Point b = new Point(10, 0);
            Point c = new Point(10, 10);

            Line k = new Line(a, b);
            Line l = new Line(b, c);

            contours.Add(k);
            contours.Add(l);

            Region r = new Region(contours);
        }

        [TestMethod]
        public void Region_isPointinRegion_test_simple()
        {
            List<Line> contours = new List<Line>();

            Point a = new Point(1, 0.5);
            Point b = new Point(7, 2);
            Point c = new Point(6, 5);
            Point d = new Point(2, 6);

            Line k = new Line(a, b);
            Line l = new Line(b, c);
            Line m = new Line(c, d);
            Line n = new Line(d, a);

            contours.Add(k);
            contours.Add(l);
            contours.Add(m);
            contours.Add(n);

            Region r = new Region(contours);

            Assert.AreEqual(r.Min_X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Max_X, 7, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Min_Y, 0.5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(r.Max_Y, 6, _Variables.EQUALS_TOLERANCE);

            Assert.AreEqual(r.edges.Count, 4);
            Assert.AreEqual(r.corners.Count, 4);

            Point t = new Point(0, 3);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(8, 3);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(3, -1);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(3, 8);
            Assert.IsFalse(r.isPointinRegion(t));
            
            t = new Point(1.5, 4.5);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(4, 1);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(6.5, 4);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(5, 5.5);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(4, 4);
            Assert.IsTrue(r.isPointinRegion(t));
        }

        [TestMethod]
        public void Region_isPointinRegion_test_twoRecs()
        {
            List<Line> contours = new List<Line>();

            Point a = new Point(0, 0);
            Point b = new Point(10, 0);
            Point c = new Point(10, 10);
            Point d = new Point(0, 10);

            Point a2 = new Point(5, 5);
            Point b2 = new Point(9, 5);
            Point c2 = new Point(9, 9);
            Point d2 = new Point(5, 9);

            Line k = new Line(a, b);
            Line l = new Line(b, c);
            Line m = new Line(c, d);
            Line n = new Line(d, a);

            Line k2 = new Line(a2, b2);
            Line l2 = new Line(b2, c2);
            Line m2 = new Line(c2, d2);
            Line n2 = new Line(d2, a2);

            contours.Add(k);
            contours.Add(l);
            contours.Add(m);
            contours.Add(n);

            contours.Add(k2);
            contours.Add(l2);
            contours.Add(m2);
            contours.Add(n2);

            Region r = new Region(contours);

            Assert.AreEqual(r.edges.Count, 8);
            Assert.AreEqual(r.corners.Count, 8);

            Point t = new Point(3, 7);
            Assert.IsTrue(r.isPointinRegion(t));

            t = new Point(3, 3);
            Assert.IsTrue(r.isPointinRegion(t));

            t = new Point(7, 3);
            Assert.IsTrue(r.isPointinRegion(t));

            t = new Point(7, 7);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(11, 9);
            Assert.IsFalse(r.isPointinRegion(t));

            t = new Point(0, -9);
            Assert.IsFalse(r.isPointinRegion(t));
        }

        [TestMethod]
        public void Region_isPointinRegion_test_bugz()
        {
            List<Line> contours = new List<Line>();

            Point a1 = new Point(9270.50355349, 6558.50334822);
            Point b1 = new Point(11590.50848401, 6558.50331297);
            Line l1 = new Line(a1, b1);
            contours.Add(l1);

            Point a2 = new Point(11590.50848401, 6558.50331297);
            Point b2 = new Point(11590.50851865, 8838.50209879);
            Line l2 = new Line(a2, b2);
            contours.Add(l2);

            Point a3 = new Point(11590.5082192, 8838.50209879);
            Point b3 = new Point(11590.50852169, 9038.50209879);
            Line l3 = new Line(a3, b3);
            contours.Add(l3);

            Point a4 = new Point(11590.50852169, 9038.50210220);
            Point b4 = new Point(11590.50852777, 9438.50356922);
            Line l4 = new Line(a4, b4);
            contours.Add(l4);

            Point a5 = new Point(11590.50852777, 9438.50356922);
            Point b5 = new Point(9270.50359726, 9438.50360448);
            Line l5 = new Line(a5, b5);
            contours.Add(l5);

            Point a6 = new Point(9270.50359726, 9438.50360448);
            Point b6 = new Point(9270.50357837, 6558.50334822);
            Line l6 = new Line(a6, b6);
            contours.Add(l6);

            Region r = new Region(contours);

            Assert.AreEqual(r.edges.Count, 4);
            Assert.AreEqual(r.corners.Count, 4);
        }
    }
}



