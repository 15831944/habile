#define BRX_APP
//#define ARX_APP

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;

#if BRX_APP
    using _Ap = Bricscad.ApplicationServices;
    //using _Br = Teigha.BoundaryRepresentation;
    using _Cm = Teigha.Colors;
    using _Db = Teigha.DatabaseServices;
    using _Ed = Bricscad.EditorInput;
    using _Ge = Teigha.Geometry;
    using _Gi = Teigha.GraphicsInterface;
    using _Gs = Teigha.GraphicsSystem;
    using _Gsk = Bricscad.GraphicsSystem;
    using _Pl = Bricscad.PlottingServices;
    using _Brx = Bricscad.Runtime;
    using _Trx = Teigha.Runtime;
    using _Wnd = Bricscad.Windows;
    //using _Int = Bricscad.Internal;
#elif ARX_APP
    using _Ap = Autodesk.AutoCAD.ApplicationServices;
    //using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
    using _Cm = Autodesk.AutoCAD.Colors;
    using _Db = Autodesk.AutoCAD.DatabaseServices;
    using _Ed = Autodesk.AutoCAD.EditorInput;
    using _Ge = Autodesk.AutoCAD.Geometry;
    using _Gi = Autodesk.AutoCAD.GraphicsInterface;
    using _Gs = Autodesk.AutoCAD.GraphicsSystem;
    using _Pl = Autodesk.AutoCAD.PlottingServices;
    using _Brx = Autodesk.AutoCAD.Runtime;
    using _Trx = Autodesk.AutoCAD.Runtime;
    using _Wnd = Autodesk.AutoCAD.Windows;
#endif

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    partial class Reinforcer
    {
        _CONNECTION _c;


        public Reinforcer(ref _CONNECTION c)
        {
            _c = c;
        }


        public void run()
        {
            List<string> blockNames = new List<string>() { "Raud_A", "Raud_B", "Raud_C", "Raud_D", "Raud_E", "Raud_U", "Reinf_A_Raud", "Reinf_B_Raud", "Reinf_C_Raud", "Reinf_D_Raud", "Reinf_D_Raud_side", "Reinf_E_Raud2", "Reinf_U_Raud_side" };
            List<string> layerNames = new List<string>() { "K023TL", "Armatuur" };
            _SETUP init = new _SETUP(ref _c);
            init.start(blockNames, layerNames);

            getSettings();
            List<G.Line> polys = getGeometry();

            G.Point insertPoint = getBendingInsertionPoint();

            List<R.Raud> reinf = new List<R.Raud>();
            List<R.Raud_Array> reinf_array = new List<R.Raud_Array>();
            List<R.Raud> unique_reinf = new List<R.Raud>();

            L.ReinforcmentHandler logic = new L.ReinforcmentHandler(polys);
            logic.main(ref reinf, ref reinf_array, ref unique_reinf);
            
            output(reinf, reinf_array, unique_reinf, insertPoint);
        }
        

        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}