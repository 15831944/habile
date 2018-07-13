using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using G = Geometry;
using R = Reinforcement;

namespace Reinforcement_tests
{
    [TestClass]
    public class A_Raud_tests
    {

        [TestMethod]
        public void A_Raud_class_test()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point end = new G.Point(10.0, 0.0);
            G.Line line = new G.Line(start, end);

            R.A_Raud reinf = new R.A_Raud(line, 2, 8, "B500B");

            Assert.IsTrue(reinf is R.Raud);
            Assert.IsTrue(reinf is R.A_Raud);
        }


        [TestMethod]
        public void A_Raud_Init_test0()
        {
            G.Point start = new G.Point(100.0, 100.0);
            G.Point end = new G.Point(-2100.0, 2100.0);
            G.Line line = new G.Line(start, end);

            R.A_Raud reinf = new R.A_Raud(line, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 98.8110, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 101.0809, 0.001);
            Assert.AreEqual(reinf.Rotation, 2.4037, 0.001);

            Assert.AreEqual(reinf.A, 2970, 0.001); // ROUNDED

            Assert.AreEqual(reinf.Length, 2970, 0.001); // ROUNDED
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }


        [TestMethod]
        public void A_Raud_Init_test1()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point end = new G.Point(10.0, 0.0);
            G.Line line = new G.Line(start, end);

            R.A_Raud reinf = new R.A_Raud(line, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 0.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 0.0, 0.001);
            Assert.AreEqual(reinf.Rotation, 0.0, 0.001);

            Assert.AreEqual(reinf.A, 10.0, 0.001);

            Assert.AreEqual(reinf.Length, 10.0, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }


        [TestMethod]
        public void A_Raud_Init_test2()
        {
            G.Point start = new G.Point(10.0, 1.0);
            G.Point end = new G.Point(0.0, 1.0);
            G.Line line = new G.Line(start, end);

            R.A_Raud reinf = new R.A_Raud(line, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 10.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 1.0, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI, 0.001);

            Assert.AreEqual(reinf.A, 10.0, 0.001);

            Assert.AreEqual(reinf.Length, 10.0, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }


        [TestMethod]
        public void A_Raud_Init_test3()
        {
            G.Point start = new G.Point(10.0, 10.0);
            G.Point end = new G.Point(0.0, 0.0);
            G.Line line = new G.Line(start, end);

            R.A_Raud reinf = new R.A_Raud(line, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 8.535533, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 8.535533, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI + Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001); // ROUNDED

            Assert.AreEqual(reinf.Length, 10, 0.001); // ROUNDED
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }

    }
}
