using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

using G = Geometry;
using R = Reinforcement;

namespace Reinforcement_tests
{
    [TestClass]
    public class C_Raud_tests
    {
        [TestMethod]
        public void C_Raud_class_test()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point mainp = start.move(10.0, new G.Vector(1, 0));
            G.Point sidep = start.move(10.0, new G.Vector(-1, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side = new G.Line(sidep, start);

            R.C_Raud reinf = new R.C_Raud(main, side, 2, 8, "B500B");

            Assert.IsTrue(reinf is R.Raud);
            Assert.IsTrue(reinf is R.C_Raud);
        }

        [TestMethod]
        public void C_Raud_Init_test1()
        {
            G.Point start = new G.Point(0.0, 0.0);
            G.Point mainp = start.move(10.0, new G.Vector(1, 0));
            G.Point sidep = start.move(5.0, new G.Vector(-1, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side = new G.Line(sidep, start);

            R.C_Raud reinf = new R.C_Raud(main, side, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 0.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 0.0, 0.001);
            Assert.AreEqual(reinf.Rotation, 0.0, 0.001);

            Assert.AreEqual(reinf.A, 5, 0.001);
            Assert.AreEqual(reinf.B, 10, 0.001);
            Assert.AreEqual(reinf.U, Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.Length, 15.0, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }

        [TestMethod]
        public void C_Raud_Init_test2()
        {
            G.Point start = new G.Point(2.0, 2.0);
            G.Point mainp = start.move(5, new G.Vector(1, 1));
            G.Point sidep = start.move(10, new G.Vector(0, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side = new G.Line(sidep, start);

            R.C_Raud reinf = new R.C_Raud(main, side, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 2.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 2.0, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001);
            Assert.AreEqual(reinf.B, 5, 0.001);
            Assert.AreEqual(reinf.U, 3 * Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.Length, 15.0, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }

        [TestMethod]
        public void C_Raud_Init_test3()
        {
            G.Point start = new G.Point(6.0, 2.0);
            G.Point mainp = start.move(5, new G.Vector(1, -1));
            G.Point sidep = start.move(10, new G.Vector(0, 1));

            G.Line main = new G.Line(start, mainp);
            G.Line side = new G.Line(sidep, start);

            R.C_Raud reinf = new R.C_Raud(main, side, 2, 8, "B500B");

            Assert.AreEqual(reinf.StartPoint.X, 6.0, 0.001);
            Assert.AreEqual(reinf.StartPoint.Y, 2.0, 0.001);
            Assert.AreEqual(reinf.Rotation, Math.PI + 3 * Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.A, 10, 0.001);
            Assert.AreEqual(reinf.B, 5, 0.001);
            Assert.AreEqual(reinf.U, Math.PI / 4, 0.001);

            Assert.AreEqual(reinf.Length, 15.0, 0.001);
            Assert.AreEqual(reinf.Diameter, 8, 0.001);
            Assert.AreEqual(reinf.Materjal, "B500B");
        }
    }
}
