using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Geometry;

namespace Geometry_tests
{
    [TestClass]
    public class Line_test
    {
        [TestMethod]
        public void Line_init_test()
        {
            Point a = new Point(-1, -2);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Assert.AreEqual(k.Start.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(k.Start.Y, -2, _Variables.EQUALS_TOLERANCE);

            Assert.AreEqual(k.End.X, 10, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(k.End.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        [ExpectedException(typeof(LineSamePointException))]
        public void Line_init_fail_test()
        {
            Point a = new Point(-1, -2);
            Point b = new Point(-1, -2);

            Line k = new Line(a, b);
        }


        [TestMethod]
        public void Line_getCenterPoint_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 10);

            Line k = new Line(a, b);

            Point c = k.getCenterPoint();
            Assert.AreEqual(c.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(c.Y, 5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getCenterPoint_test2()
        {
            Point a = new Point(10, 10);
            Point b = new Point(0, 0);

            Line k = new Line(a, b);

            Point c = k.getCenterPoint();
            Assert.AreEqual(c.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(c.Y, 5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getCenterPoint_test3()
        {
            Point a = new Point(10, -20);
            Point b = new Point(-20, 10);

            Line k = new Line(a, b);

            Point c = k.getCenterPoint();
            Assert.AreEqual(c.X, -5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(c.Y, -5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getDirectionVector_test1()
        {
            Point a = new Point(-1, -2);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Vector v = k.getDirectionVector();
            Assert.AreEqual(v.X, 0.9839, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, 0.1789, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getDirectionVector_test2()
        {
            Point a = new Point(10, 0);
            Point b = new Point(-1, -2);

            Line k = new Line(a, b);

            Vector v = k.getDirectionVector();
            Assert.AreEqual(v.X, -0.9839, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(v.Y, -0.1789, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getOffsetVector_test1()
        {
            Point a = new Point(-1, -2);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Vector o = k.getOffsetVector();
            Assert.AreEqual(o.X, -0.1789, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(o.Y, 0.9839, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getOffsetVector_test2()
        {
            Point a = new Point(10, 0);
            Point b = new Point(-1, -2);

            Line k = new Line(a, b);

            Vector o = k.getOffsetVector();
            Assert.AreEqual(o.X, 0.1789, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(o.Y, -0.9839, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_offset_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.Offset(1.5);

            Assert.AreEqual(new_k.Start.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.Start.Y, 1.5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.X, 10, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.Y, 1.5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_offset_test2()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.Offset(-1.5);

            Assert.AreEqual(new_k.Start.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.Start.Y, -1.5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.X, 10, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.Y, -1.5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_Extend_Start_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);
            
            Line new_k = k.extendStart(1.0);
            Point new_a = new_k.Start;

            Assert.AreEqual(new_a.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_a.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_Extend_Start_test2()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.extendStart(-1.0);
            Point new_a = new_k.Start;

            Assert.AreEqual(new_a.X, 1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_a.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_Extend_End_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.extendEnd(1.0);
            Point new_b = new_k.End;

            Assert.AreEqual(new_b.X, 11, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_b.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_Extend_End_test2()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.extendEnd(-1.0);
            Point new_b = new_k.End;

            Assert.AreEqual(new_b.X, 9, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_b.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_Extend_Double_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Line k = new Line(a, b);

            Line new_k = k.extendDouble(1.0);
            
            Assert.AreEqual(new_k.Start.X, -1, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.Start.Y, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.X, 11, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(new_k.End.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getIntersectionPoint_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Point c = new Point(5, -5);
            Point d = new Point(5, 5);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Point x = Line.getIntersectionPoint(k, m);
            Assert.AreEqual(x.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getIntersectionPoint_test2()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 0);

            Point c = new Point(5, -5);
            Point d = new Point(5, 5);

            Line k = new Line(c, d);
            Line m = new Line(a, b);

            Point x = Line.getIntersectionPoint(k, m);
            Assert.AreEqual(x.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getIntersectionPoint_test3()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 10);

            Point c = new Point(10, 0);
            Point d = new Point(0, 10);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Point x = Line.getIntersectionPoint(k, m);
            Assert.AreEqual(x.X, 5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Y, 5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        [ExpectedException(typeof(LineNoIntersectionException))]
        public void Line_getIntersectionPoint_test4()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 10);

            Point c = new Point(-10, -1);
            Point d = new Point(-1, -10);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Point x = Line.getIntersectionPoint(k, m);
        }


        [TestMethod]
        public void Line_getIntersectionPoint_test5()
        {
            Point a = new Point(0, 0);
            Point b = new Point(-10, -10);

            Point c = new Point(-10, 0);
            Point d = new Point(0, -10);

            Line k = new Line(a, b);
            Line m = new Line(c, d);
            
            Point x = Line.getIntersectionPoint(k, m);
            Assert.AreEqual(x.X, -5, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Y, -5, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_getIntersectionPoint_test6()
        {
            Point a = new Point(0, 0);
            Point b = new Point(10, 10);

            Point c = new Point(-10, -10);
            Point d = new Point(0, 0);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Point x = Line.getIntersectionPoint(k, m);
            Assert.AreEqual(x.X, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Y, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_rotation_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(1000, 1000);

            Line k = new Line(a, b);
            Polar p = Converter.xy_to_la(k.getDirectionVector());

            Line z = k.rotation(k.Start, -p.angle);

            Assert.AreEqual(z.Length(), 1414.2135, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_rotation_test2()
        {
            Point a = new Point(200, 500);
            Point b = new Point(1184.8078, 673.6482);
            Point c = new Point(396.9616, 534.7296);
            Point d = new Point(1677.2116, 760.4723);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Polar p1 = Converter.xy_to_la(k.getDirectionVector());

            Line z = k.rotation(k.Start, -p1.angle);
            Line x = m.rotation(k.Start, -p1.angle);

            double z_dY = z.End.Y - z.Start.Y;
            double x_dY = x.End.Y - x.Start.Y;

            Assert.AreEqual(z.Length(), 1000, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Length(), 1300, _Variables.EQUALS_TOLERANCE);

            Assert.AreEqual(z_dY, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x_dY, 0, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_rotation_test3()
        {
            Point a = new Point(200, 500);
            Point b = new Point(1184.8078, 673.6482);
            Point c = new Point(388.2791, 583.9700);
            Point d = new Point(1668.5292, 809.7127);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            Polar p1 = Converter.xy_to_la(k.getDirectionVector());

            Line z = k.rotation(k.Start, -p1.angle);
            Line x = m.rotation(k.Start, -p1.angle);

            double z_dY = z.End.Y - z.Start.Y;
            double x_dY = x.End.Y - x.Start.Y;
            double zx_dY = x.End.Y - z.Start.Y;
            double zex_dY = x.End.Y - z.End.Y;

            Assert.AreEqual(z.Length(), 1000, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x.Length(), 1300, _Variables.EQUALS_TOLERANCE);

            Assert.AreEqual(z_dY, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(x_dY, 0, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(zx_dY, 50, _Variables.EQUALS_TOLERANCE);
            Assert.AreEqual(zex_dY, 50, _Variables.EQUALS_TOLERANCE);
        }


        [TestMethod]
        public void Line_areLinesColinear_test1()
        {
            Point a = new Point(0, 0);
            Point b = new Point(1000, 0);
            Point c = new Point(500, 0);
            Point d = new Point(2500, 0);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            bool y = Line.areLinesOnSameLine(k, m);
            bool z = Line.areLinesCoLinear(k, m);

            Assert.AreEqual(y, true);
            Assert.AreEqual(z, true);
        }


        [TestMethod]
        public void Line_areLinesColinear_test2()
        {
            Point a = new Point(200, 500);
            Point b = new Point(1184.8078, 673.6482);
            Point c = new Point(396.9616, 534.7296);
            Point d = new Point(1677.2116, 760.4723);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            bool y = Line.areLinesOnSameLine(k, m);
            bool z = Line.areLinesCoLinear(k, m);

            Assert.AreEqual(y, true);
            Assert.AreEqual(z, true);
        }


        [TestMethod]
        public void Line_areLinesColinear_test3()
        {
            Point a = new Point(0, 0);
            Point b = new Point(1000, 0);
            Point c = new Point(1500, 0);
            Point d = new Point(2500, 0);

            Line k = new Line(a, b);
            Line m = new Line(c, d);

            bool y = Line.areLinesOnSameLine(k, m);
            bool z = Line.areLinesCoLinear(k, m);

            Assert.AreEqual(y, true);
            Assert.AreEqual(z, false);
        }

    }
}

