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
    partial class TABLE_command
    {
        _CONNECTION _c;

        List<string> boxName = new List<string>() { "Drawing_Area" };
        List<string> tableHead = new List<string>() { "Painutustabel_pais" };
        List<string> tableBendingRow = new List<string>() { "Painutustabel_rida" };
        List<string> tableMaterialRow = new List<string>() { "PainutusKokkuvõte" };

        string markLayer = "K023TL";
        string kontrollLayer = "_AUTO_KONTROLL_";
        string bendingLayer = "K004";
        string materialLayer = "K004";

        List<string> bendingShapes = new List<string>() { "Raud_A",  "Raud_A_DEFAULT", "Raud_B", "Raud_C", "Raud_D", "Raud_E", "Raud_F", "Raud_G", "Raud_H", "Raud_J", "Raud_K", "Raud_L", "Raud_M", "Raud_N", "Raud_R", "Raud_S", "Raud_T",
                                                         "Raud_U", "Raud_V", "Raud_W", "Raud_X", "Raud_Y", "Raud_Z1", "Raud_Z2", "Raud_Z3", "Raud_Z4", "Raud_Z5", "Raud_Z6", "Raud_Z7", "Raud_Z8", "Raud_Z9" };

        public TABLE_command(ref _CONNECTION c)
        {
            _c = c;        
        }


        public void bending()
        {            
            _SETUP init = new _SETUP(ref _c);
            init.initBlocks(tableBendingRow);

            //List<string> layerNames = new List<string>() { bendingLayer };
            //init.layers(layerNames);

            List<G.Area> areas = getAllAreas(boxName);
            List<T.TableHead> heads = getAllTableHeads(tableHead);
            List<T.ReinforcementMark> marks = getAllMarks();

            List<T.BendingShape> bendings = getAllBendings(bendingShapes);
            List<T.TableBendingRow> rows = getAllTableRows(tableBendingRow);

            T.HandlerBending logic = new T.HandlerBending();
            List<T.DrawingArea> data = logic.main(areas, heads, marks, bendings, rows);

            bending_output(data, tableBendingRow[0]);
        }


        public void material()
        {
            _SETUP init = new _SETUP(ref _c);
            init.initBlocks(tableMaterialRow);

            //List<string> layerNames = new List<string>() { materialLayer };
            //init.layers(layerNames);

            List<G.Area> areas = getAllAreas(boxName);
            List<T.TableHead> heads = getAllTableHeads(tableHead);

            List<T.TableBendingRow> rows = getAllTableRows(tableBendingRow);
            List<T.TableMaterialRow> summarys = getAllTableSummarys(tableMaterialRow);

            T.HandlerMaterial logic = new T.HandlerMaterial();
            List<T.DrawingArea> data = logic.main(areas, heads, rows, summarys);

            material_output(data, tableMaterialRow[0]);
        }


        public void check()
        {
            _SETUP init = new _SETUP(ref _c);

            //KONTROLL LAYER LUUAKSE HILJEM PROGRAMMIS, KUI ON VAJA
            //List<string> layerNames = new List<string>() { kontrollLayer };
            //init.initLayers(layerNames);

            List<G.Area> areas = getAllAreas(boxName);
            List<T.TableHead> heads = getAllTableHeads(tableHead);
            List<T.ReinforcementMark> marks = getAllMarks();

            List<T.BendingShape> bendings = getAllBendings(bendingShapes);
            List<T.TableBendingRow> rows = getAllTableRows(tableBendingRow);
            List<T.TableMaterialRow> summarys = getAllTableSummarys(tableMaterialRow);

            T.HandlerChecker logic = new T.HandlerChecker();
            List<T.DrawingArea> data = logic.main(areas, heads, marks, bendings, rows, summarys);

            checker_output(init, data);
        }


        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}