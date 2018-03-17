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
    static class Tabler_Inputs
    {
        //
        //DRAWING AREA
        //
        public static List<G.Area> getAllAreas(string blockName)
        {
            List<G.Area> areas = new List<G.Area>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName, trans);
                areas = getBoxAreas(blocks, trans);
            }

            return areas;
        }


        private static List<G.Area> getBoxAreas(List<_Db.BlockReference> blocks, _Db.Transaction trans)
        {
            List<G.Area> parse = new List<G.Area>();

            foreach (_Db.BlockReference block in blocks)
            {
                _Ge.Point3d IP = block.Position;
                G.Point min = new G.Point(IP.X, IP.Y);

                double dX = 0;
                double dY = 0;

                _Db.DynamicBlockReferencePropertyCollection aa = block.DynamicBlockReferencePropertyCollection;
                foreach (_Db.DynamicBlockReferenceProperty a in aa)
                {
                    if (a.PropertyName == "X Suund") dX = (double)a.Value;
                    else if (a.PropertyName == "Y Suund") dY = (double)a.Value;
                }

                if (dX != 0 || dY != 0)
                {
                    G.Point max = new G.Point(IP.X + dX, IP.Y + dY);
                    G.Area area = new G.Area(min, max);

                    parse.Add(area);
                }
            }

            return parse;
        }


        //
        //TABLE HEADS
        //
        public static List<T.TableHead> getAllTableHeads(string blockName)
        {
            List<T.TableHead> heads = new List<T.TableHead>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName, trans);
                heads = getTableHeadData(blocks, trans);
            }

            return heads;
        }


        private static List<T.TableHead> getTableHeadData(List<_Db.BlockReference> blocks, _Db.Transaction trans)
        {
            List<T.TableHead> parse = new List<T.TableHead>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                double scale = block.ScaleFactors.Y;
                T.TableHead current = new T.TableHead(insp, scale);
                parse.Add(current);
            }

            return parse;
        }


        //
        //TABLE ROWS
        //
        public static List<T.TableRow> getAllTableRows(string blockName)
        {
            List<T.TableRow> rows = new List<T.TableRow>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName, trans);
                rows = getRowData(blocks, trans);
            }

            return rows;
        }


        private static List<T.TableRow> getRowData(List<_Db.BlockReference> blocks, _Db.Transaction trans)
        {
            List<T.TableRow> parse = new List<T.TableRow>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableRow current = new T.TableRow(insp);

                foreach (_Db.ObjectId arId in block.AttributeCollection)
                {
                    _Db.DBObject obj = trans.GetObject(arId, _Db.OpenMode.ForWrite);
                    _Db.AttributeReference ar = obj as _Db.AttributeReference;
                    setRowParameters(ar, current);
                }

                parse.Add(current);
            }

            return parse;
        }


        private static void setRowParameters(_Db.AttributeReference ar, T.TableRow row)
        {
            if (ar != null)
            {
                if (ar.Tag == "Klass") { row.Material = ar.TextString; }
                else if (ar.Tag == "Pos") { row.Position = ar.TextString; }
                else
                {
                    int temp = 99999;
                    Int32.TryParse(ar.TextString, out temp);

                    if (ar.Tag == "Diam") { row.Diameter = temp; }
                    else if (ar.Tag == "tk") { row.Count = temp; }
                    else if (ar.Tag == "Pikkus") { row.Length = temp; }
                }
            }
        }


        //
        //BENDING
        //
        public static List<T.Bending> getAllBendings(List<string> bendingNames)
        {
            List<T.Bending> bendings = new List<T.Bending>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(bendingNames, trans);
                bendings = getBendingData(blocks, trans);
            }

            return bendings;
        }


        private static List<T.Bending> getBendingData(List<_Db.BlockReference> blocks, _Db.Transaction trans)
        {
            List<T.Bending> parse = new List<T.Bending>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.Bending current = new T.Bending(insp, block.Name);

                foreach (_Db.ObjectId arId in block.AttributeCollection)
                {
                    _Db.DBObject obj = trans.GetObject(arId, _Db.OpenMode.ForWrite);
                    _Db.AttributeReference ar = obj as _Db.AttributeReference;
                    setBendingParameters(ar, current);
                }

                current.validator();

                if (current.Valid)
                {
                    parse.Add(current);
                }
                else
                {
                    Universal.writeCadMessage(current.Reason);
                }
            }

            return parse;
        }


        private static void setBendingParameters(_Db.AttributeReference ar, T.Bending bending)
        {
            if (ar != null)
            {
                if (ar.Tag == "Positsioon") { bending.Position = ar.TextString; }
                else if (ar.Tag == "Teraseklass") { bending.Material = ar.TextString; }
                else if (ar.Tag == "U") { bending.U = ar.TextString; }
                else if (ar.Tag == "V") { bending.V = ar.TextString; }
                else
                {
                    int temp = 99999;
                    Int32.TryParse(ar.TextString, out temp);

                    if (ar.Tag == "A") { bending.A = temp; }
                    else if (ar.Tag == "B") { bending.B = temp; }
                    else if (ar.Tag == "C") { bending.C = temp; }
                    else if (ar.Tag == "D") { bending.D = temp; }
                    else if (ar.Tag == "E") { bending.E = temp; }
                    else if (ar.Tag == "F") { bending.F = temp; }
                    else if (ar.Tag == "G") { bending.G = temp; }
                    else if (ar.Tag == "R") { bending.R = temp; }
                    else if (ar.Tag == "X") { bending.X = temp; }
                    else if (ar.Tag == "Y") { bending.Y = temp; }
                }
            }
        }


        //
        //MARKS
        //
        public static List<T.ReinforcementMark> getAllMarks(string layer)
        {
            List<T.ReinforcementMark> marks = new List<T.ReinforcementMark>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.MText> allTexts = getAllText(layer, trans);
                marks = getMarkData(allTexts, trans);
            }

            return marks;
        }


        private static List<T.ReinforcementMark> getMarkData(List<_Db.MText> txts, _Db.Transaction trans)
        {
            List<T.ReinforcementMark> parse = new List<T.ReinforcementMark>();

            foreach (_Db.MText txt in txts)
            {
                G.Point insp = new G.Point(txt.Location.X, txt.Location.Y);
                T.ReinforcementMark current = new T.ReinforcementMark(insp, txt.Contents);

                if (current.validate())
                {
                    parse.Add(current);
                }
                else
                {
                    Universal.writeCadMessage("WARNING - VIIDE - \"" + txt.Contents + "\" - could not read");
                }
            }

            return parse;
        }


        //
        //SUMMARY
        //        
        public static List<T.TableSummary> getAllTableSummarys(string blockName)
        {
            List<T.TableSummary> sums = new List<T.TableSummary>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName, trans);
                sums = getSummaryData(blocks, trans);
            }

            return sums;
        }


        private static List<T.TableSummary> getSummaryData(List<_Db.BlockReference> blocks, _Db.Transaction trans)
        {
            List<T.TableSummary> parse = new List<T.TableSummary>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableSummary temp = new T.TableSummary(insp);
                parse.Add(temp);
            }

            return parse;
        }


        //
        //UNIVERSAL
        //
        private static List<_Db.MText> getAllText(string layer, _Db.Transaction trans)
        {
            List<_Db.MText> txt = new List<_Db.MText>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.BlockTableRecord btr = trans.GetObject(_Db.SymbolUtilityServices.GetBlockModelSpaceId(db), _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

            foreach (_Db.ObjectId id in btr)
            {
                _Db.Entity currentEntity = trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.Entity;

                if (currentEntity != null)
                {
                    if (currentEntity is _Db.MText)
                    {
                        _Db.MText br = currentEntity as _Db.MText;
                        if (br.Layer == layer)
                        {
                            txt.Add(br);
                        }
                    }

                    if (currentEntity is _Db.MLeader)
                    {
                        _Db.MLeader br = currentEntity as _Db.MLeader;
                        if (br.Layer == layer)
                        {
                            if (br.ContentType == _Db.ContentType.MTextContent)
                            {
                                _Db.MText leaderText = br.MText;
                                txt.Add(leaderText);
                            }
                        }
                    }
                }
            }

            return txt;
        }


        private static List<_Db.BlockReference> getAllBlockReference(string blockName, _Db.Transaction trans)
        {
            List<_Db.BlockReference> refs = new List<_Db.BlockReference>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.BlockTable bt = trans.GetObject(db.BlockTableId, _Db.OpenMode.ForRead) as _Db.BlockTable;

            if (bt.Has(blockName))
            {
                _Db.BlockTableRecord btr = trans.GetObject(_Db.SymbolUtilityServices.GetBlockModelSpaceId(db), _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

                foreach (_Db.ObjectId id in btr)
                {
                    _Db.DBObject currentEntity = trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.DBObject;

                    if (currentEntity == null)
                    {
                        continue;
                    }

                    else if (currentEntity is _Db.BlockReference)
                    {
                        _Db.BlockReference blockRef = currentEntity as _Db.BlockReference;

                        _Db.BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = trans.GetObject(blockRef.DynamicBlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }
                        else
                        {
                            block = trans.GetObject(blockRef.BlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }

                        if (block != null)
                        {
                            if (block.Name == blockName)
                            {
                                refs.Add(blockRef);
                            }
                        }
                    }
                }
            }

            return refs;
        }


        private static List<_Db.BlockReference> getAllBlockReference(List<string> blockNames, _Db.Transaction trans)
        {
            List<_Db.BlockReference> refs = new List<_Db.BlockReference>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            _Db.BlockTable bt = trans.GetObject(db.BlockTableId, _Db.OpenMode.ForRead) as _Db.BlockTable;

            _Db.BlockTableRecord btr = trans.GetObject(_Db.SymbolUtilityServices.GetBlockModelSpaceId(db), _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

            foreach (_Db.ObjectId id in btr)
            {
                _Db.DBObject currentEntity = trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.DBObject;

                if (currentEntity == null)
                {
                    continue;
                }

                else if (currentEntity is _Db.BlockReference)
                {
                    _Db.BlockReference blockRef = currentEntity as _Db.BlockReference;

                    _Db.BlockTableRecord block = null;
                    if (blockRef.IsDynamicBlock)
                    {
                        block = trans.GetObject(blockRef.DynamicBlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                    }
                    else
                    {
                        block = trans.GetObject(blockRef.BlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                    }

                    if (block != null)
                    {
                        if (blockNames.Contains(block.Name))
                        {
                            refs.Add(blockRef);
                        }
                    }
                }
            }

            return refs;
        }

    }
}