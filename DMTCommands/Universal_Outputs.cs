﻿using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;

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

//using _Ap = Bricscad.ApplicationServices;
////using _Br = Teigha.BoundaryRepresentation;
//using _Cm = Teigha.Colors;
//using _Db = Teigha.DatabaseServices;
//using _Ed = Bricscad.EditorInput;
//using _Ge = Teigha.Geometry;
//using _Gi = Teigha.GraphicsInterface;
//using _Gs = Teigha.GraphicsSystem;
//using _Gsk = Bricscad.GraphicsSystem;
//using _Pl = Bricscad.PlottingServices;
//using _Brx = Bricscad.Runtime;
//using _Trx = Teigha.Runtime;
//using _Wnd = Bricscad.Windows;
////using _Int = Bricscad.Internal;

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    class Universal
    {
        _CONNECTION _c;

        public Universal(ref _CONNECTION c)
        {
            _c = c;
        }


        public void start(List<string> blockNames, List<string> layerNames)
        {
            _Db.LayerTable layerTable = _c.trans.GetObject(_c.db.LayerTableId, _Db.OpenMode.ForWrite) as _Db.LayerTable;

            foreach (string blockName in blockNames)
            {
                if (!_c.blockTable.Has(blockName))
                {
                    //throw new DMTException("[ERROR] Selles failis puuduvad vajalikud blokkid!");
                    getBlockFromMaster(blockName);
                }
            }

            foreach (string layerName in layerNames)
            {
                if (!layerTable.Has(layerName))
                {
                    createLayer(layerName, layerTable);
                }
            }
        }


        private void createLayer(string layerName, _Db.LayerTable layerTable)
        {
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

            _Db.ObjectId layerId = layerTable.Add(newLayer);
            _c.trans.AddNewlyCreatedDBObject(newLayer, true);
            _c.db.Clayer = layerId;
        }


        private void getBlockFromMaster(string blockName)
        {
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


            using (_Db.Transaction sourceTrans = sourceDb.TransactionManager.StartTransaction())
            {
                _Db.BlockTable sourceBlockTable = sourceTrans.GetObject(sourceDb.BlockTableId, _Db.OpenMode.ForRead, false) as _Db.BlockTable;

                foreach (_Db.ObjectId sourceObject in sourceBlockTable)
                {
                    _Db.BlockTableRecord btr = sourceTrans.GetObject(sourceObject, _Db.OpenMode.ForRead, false) as _Db.BlockTableRecord;

                    if (!btr.IsAnonymous && !btr.IsLayout)
                    {
                        if (btr.Name == blockName)
                        {
                            blockIds.Add(sourceObject);
                            _c.ed.WriteMessage("\n" + blockName + " added");
                        }
                    }

                    btr.Dispose();
                }
            }

            _Db.IdMapping mapping = new _Db.IdMapping();
            sourceDb.WblockCloneObjects(blockIds, _c.blockTable.Id, mapping, _Db.DuplicateRecordCloning.Replace, false);
            sourceDb.Dispose();
        }

    }
}