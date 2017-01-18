//using System;
//using System.Text;
//using System.Collections;
//using System.Linq;
//using System.IO;
//using System.Diagnostics;
//using System.Collections.Generic;
//using SWF = System.Windows.Forms;

//using Autodesk.AutoCAD.Runtime;
//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.EditorInput;

//using R = Reinforcement;
//using G = Geometry;
//using L = Logic_Reinf;

//namespace commands
//{
//    static class Reinforcer
//    {
//        public static void main()
//        {
//            bool settings = Reinforcer_Inputs.getSettingsVariables();
//            if (settings == false)
//            {
//                Universal.writeCadMessage("ERROR - SETTINGS BLOCK");
//                return;
//            }

//            List<G.Line> polys = Reinforcer_Inputs.getSelectedPolyLines();
//            if (!(polys.Count > 2))
//            {
//                Universal.writeCadMessage("ERROR - GEOMETRY");
//                return;
//            }

//            G.Point insertPoint = Reinforcer_Inputs.getBendingInsertionPoint();
//            if (insertPoint == null)
//            {
//                Universal.writeCadMessage("ERROR - BENDING INSERTION POINT");
//                return;
//            }

//            List<R.Raud> reinf = new List<R.Raud>();
//            List<R.Raud_Array> reinf_array = new List<R.Raud_Array>();
//            List<R.Raud> unique_reinf = new List<R.Raud>();

//            L.ReinforcmentHandler logic = new L.ReinforcmentHandler(polys);
//            logic.main(ref reinf, ref reinf_array, ref unique_reinf);

//            Reinforcer_Outputs.main(reinf, reinf_array, unique_reinf, insertPoint);
//            Universal.writeCadMessage("PROGRAM FINISED");
//        }
//    }
//}