using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

using G = Geometry;
using R = Reinforcement;

namespace Reinforcement_tests
{
    [TestClass]
    public class E_Raud_tests
    {
        [TestMethod]
        public void E_Raud_class_test()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point mainp = start.move(5, new G.Vector(1, 0));
            G.Point side1p = start.move(10.0, new G.Vector(0, 1));
            G.Point side2p = mainp.move(15, new G.Vector(0, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side1 = new G.Line(side1p, start);
            G.Line side2 = new G.Line(mainp, side2p);

            R.E_Raud reinf = new R.E_Raud(main, side1, side2, 2, 8, "B500B");

            Assert.IsTrue(reinf is R.Raud);
            Assert.IsTrue(reinf is R.E_Raud);
        }

        [TestMethod]
        public void E_Raud_Init_test1()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point mainp = start.move(5, new G.Vector(1, 0));
            G.Point side1p = start.move(10, new G.Vector(-1, 1));
            G.Point side2p = mainp.move(15, new G.Vector(1, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side1 = new G.Line(side1p, start);
            G.Line side2 = new G.Line(mainp, side2p);

            R.E_Raud reinf = new R.E_Raud(main, side1, side2, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 0.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 0.0, 0.001);
            Assert.AreEqual(reinf.Rotation, 0.0, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001);
            Assert.AreEqual(reinf.B, 5, 0.001);
            Assert.AreEqual(reinf.C, 15, 0.001);
            Assert.AreEqual(reinf.U, Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.V, Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.X, 7.071, 0.001);
            Assert.AreEqual(reinf.Y, 10.606, 0.001);

            Assert.AreEqual(reinf.Length, 30, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }

        [TestMethod]
        public void E_Raud_Init_test2()
        {
            G.Point start = new G.Point(2.0, 2.0);
            G.Point mainp = start.move(5, new G.Vector(1, 1));
            G.Point side1p = start.move(10, new G.Vector(-1, 0));
            G.Point side2p = mainp.move(15, new G.Vector(-1, 0));

            G.Line main = new G.Line(start, mainp);
            G.Line side1 = new G.Line(side1p, start);
            G.Line side2 = new G.Line(mainp, side2p);

            R.E_Raud reinf = new R.E_Raud(main, side1, side2, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 2.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 2.0, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001);
            Assert.AreEqual(reinf.B, 5, 0.001);
            Assert.AreEqual(reinf.C, 15, 0.001);
            Assert.AreEqual(reinf.U, Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.V, 3 * Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.X, 7.071, 0.001);
            Assert.AreEqual(reinf.Y, 10.606, 0.001);

            Assert.AreEqual(reinf.Length, 30, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }

        [TestMethod]
        public void E_Raud_Init_test3()
        {
            G.Point start = new G.Point(6.0, 2.0);
            G.Point mainp = start.move(5, new G.Vector(1, -1));
            G.Point side1p = start.move(10, new G.Vector(1, 0));
            G.Point side2p = mainp.move(15, new G.Vector(1, 0));

            G.Line main = new G.Line(start, mainp);
            G.Line side1 = new G.Line(side1p, start);
            G.Line side2 = new G.Line(mainp, side2p);

            R.E_Raud reinf = new R.E_Raud(main, side1, side2, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 6.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 2.0, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI + 3 * Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001);
            Assert.AreEqual(reinf.B, 5, 0.001);
            Assert.AreEqual(reinf.C, 15, 0.001);
            Assert.AreEqual(reinf.U, 3 * Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.V, Math.PI / 4, 0.001);
            Assert.AreEqual(reinf.X, 7.071, 0.001);
            Assert.AreEqual(reinf.Y, 10.606, 0.001);

            Assert.AreEqual(reinf.Length, 30, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }
    }
}
