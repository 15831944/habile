using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;

//using _Ap = Autodesk.AutoCAD.ApplicationServices;
////using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
//using _Cm = Autodesk.AutoCAD.Colors;
//using _Db = Autodesk.AutoCAD.DatabaseServices;
//using _Ed = Autodesk.AutoCAD.EditorInput;
//using _Ge = Autodesk.AutoCAD.Geometry;
//using _Gi = Autodesk.AutoCAD.GraphicsInterface;
//using _Gs = Autodesk.AutoCAD.GraphicsSystem;
//using _Pl = Autodesk.AutoCAD.PlottingServices;
//using _Brx = Autodesk.AutoCAD.Runtime;
//using _Trx = Autodesk.AutoCAD.Runtime;
//using _Wnd = Autodesk.AutoCAD.Windows;

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

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    public static class Universal
    {
        public static void writeCadMessage(string errorMessage)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Ed.Editor ed = doc.Editor;

            ed.WriteMessage("\n" + errorMessage);
        }


        public static void programInit(List<string> blockNames, List<string> layerNames, _Db.Transaction trans)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.BlockTable blockTable = trans.GetObject(db.BlockTableId, _Db.OpenMode.ForRead) as _Db.BlockTable;
            _Db.LayerTable layerTable = trans.GetObject(db.LayerTableId, _Db.OpenMode.ForWrite) as _Db.LayerTable;

            foreach (string blockName in blockNames)
            {
                if (!blockTable.Has(blockName))
                {
                    throw new System.Exception("Selles failis puuduvad vajalikud blokkid");
                    //getBlockFromMaster(blockName, trans);
                }
            }

            foreach (string layerName in layerNames)
            {
                if (!layerTable.Has(layerName))
                {
                    createLayer(layerName, trans);
                }
            }
        }


        public static void createLayer(string layerName, _Db.Transaction trans)
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.LayerTable lt = trans.GetObject(db.LayerTableId, _Db.OpenMode.ForWrite) as _Db.LayerTable;
            _Db.LayerTableRecord newLayer = new _Db.LayerTableRecord();

            newLayer.Name = layerName;
            if (layerName == "Armatuur")
            {
                newLayer.Color = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 4);
            }
            else
            {
                newLayer.Color = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 6);
            }

            _Db.ObjectId layerId = lt.Add(newLayer);
            trans.AddNewlyCreatedDBObject(newLayer, true);
            db.Clayer = layerId;
        }


        public static void getBlockFromMaster(string blockName, _Db.Transaction trans)
        {
            try
            {
                _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
                _Db.Database destDb = doc.Database;

                _Ap.DocumentCollection dm = _Ap.Application.DocumentManager;
                _Ed.Editor ed = dm.MdiActiveDocument.Editor;
                _Db.Database sourceDb = new _Db.Database(false, true);

                string sourceFileName = @"C:\Brics_pealeehitus\master.dwg";
                if (!File.Exists(sourceFileName))
                {
                    sourceFileName = @"C:\Users\aleksandr.ess\Dropbox\DMT\Brics_testimine\master.dwg";
                    if (!File.Exists(sourceFileName))
                    {
                        sourceFileName = @"C:\Users\Alex\Dropbox\DMT\Brics_testimine\master.dwg";
                    }
                }

                sourceDb.ReadDwgFile(sourceFileName, System.IO.FileShare.Read, true, "");
                _Db.ObjectIdCollection blockIds = new _Db.ObjectIdCollection();

                _Db.TransactionManager tm = sourceDb.TransactionManager;
                using (_Db.Transaction myT = tm.StartTransaction())
                {
                    _Db.BlockTable bt = (_Db.BlockTable)tm.GetObject(sourceDb.BlockTableId, _Db.OpenMode.ForRead, false);

                    foreach (_Db.ObjectId btrId in bt)
                    {
                        _Db.BlockTableRecord btr = (_Db.BlockTableRecord)tm.GetObject(btrId, _Db.OpenMode.ForRead, false);

                        if (!btr.IsAnonymous && !btr.IsLayout) blockIds.Add(btrId);
                        btr.Dispose();
                    }
                }

                _Db.IdMapping mapping = new _Db.IdMapping();
                sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, _Db.DuplicateRecordCloning.Replace, false);
                sourceDb.Dispose();
            }
            catch
            {
                throw new System.Exception();
            }
        }

    }
}