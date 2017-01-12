using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using SWF = System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;

namespace commands
{
    static class Tabler_Inputs
    {
        //
        //DRAWING AREA
        //
        public static List<G.Area> getAllAreas(string blockName)
        {
            List<G.Area> areas = new List<G.Area>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<BlockReference> blocks = getAllBlockReference(blockName, trans);
                areas = getBoxAreas(blocks, trans);
            }

            return areas;
        }

        private static List<G.Area> getBoxAreas(List<BlockReference> blocks, Transaction trans)
        {
            List<G.Area> parse = new List<G.Area>();

            foreach (BlockReference block in blocks)
            {
                Point3d IP = block.Position;
                G.Point min = new G.Point(IP.X, IP.Y);

                double dX = 0;
                double dY = 0;

                DynamicBlockReferencePropertyCollection aa = block.DynamicBlockReferencePropertyCollection;
                foreach (DynamicBlockReferenceProperty a in aa)
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

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<BlockReference> blocks = getAllBlockReference(blockName, trans);
                heads = getTableHeadData(blocks, trans);
            }

            return heads;
        }

        private static List<T.TableHead> getTableHeadData(List<BlockReference> blocks, Transaction trans)
        {
            List<T.TableHead> parse = new List<T.TableHead>();

            foreach (BlockReference block in blocks)
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

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<BlockReference> blocks = getAllBlockReference(blockName, trans);
                rows = getRowData(blocks, trans);
            }

            return rows;
        }

        private static List<T.TableRow> getRowData(List<BlockReference> blocks, Transaction trans)
        {
            List<T.TableRow> parse = new List<T.TableRow>();

            foreach (BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableRow current = new T.TableRow(insp);

                foreach (ObjectId arId in block.AttributeCollection)
                {
                    DBObject obj = trans.GetObject(arId, OpenMode.ForWrite);
                    AttributeReference ar = obj as AttributeReference;
                    setRowParameters(ar, current);
                }

                parse.Add(current);
            }

            return parse;
        }

        private static void setRowParameters(AttributeReference ar, T.TableRow row)
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

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<BlockReference> blocks = getAllBlockReference(bendingNames, trans);
                bendings = getBendingData(blocks, trans);
            }

            return bendings;
        }

        private static List<T.Bending> getBendingData(List<BlockReference> blocks, Transaction trans)
        {
            List<T.Bending> parse = new List<T.Bending>();

            foreach (BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.Bending current = new T.Bending(insp, block.Name);

                foreach (ObjectId arId in block.AttributeCollection)
                {
                    DBObject obj = trans.GetObject(arId, OpenMode.ForWrite);
                    AttributeReference ar = obj as AttributeReference;
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

        private static void setBendingParameters(AttributeReference ar, T.Bending bending)
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

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<MText> allTexts = getAllText(layer, trans);
                marks = getMarkData(allTexts, trans);
            }

            return marks;
        }

        private static List<T.ReinforcementMark> getMarkData(List<MText> txts, Transaction trans)
        {
            List<T.ReinforcementMark> parse = new List<T.ReinforcementMark>();

            foreach (MText txt in txts)
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

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                List<BlockReference> blocks = getAllBlockReference(blockName, trans);
                sums = getSummaryData(blocks, trans);
            }

            return sums;
        }

        private static List<T.TableSummary> getSummaryData(List<BlockReference> blocks, Transaction trans)
        {
            List<T.TableSummary> parse = new List<T.TableSummary>();

            foreach (BlockReference block in blocks)
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
        private static List<MText> getAllText(string layer, Transaction trans)
        {
            List<MText> txt = new List<MText>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTableRecord btr = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;

            foreach (ObjectId id in btr)
            {
                Entity currentEntity = trans.GetObject(id, OpenMode.ForWrite, false) as Entity;

                if (currentEntity != null)
                {
                    if (currentEntity is MText)
                    {
                        MText br = currentEntity as MText;
                        if (br.Layer == layer)
                        {
                            txt.Add(br);
                        }
                    }

                    if (currentEntity is MLeader)
                    {
                        MLeader br = currentEntity as MLeader;
                        if (br.Layer == layer)
                        {
                            if (br.ContentType == ContentType.MTextContent)
                            {
                                MText leaderText = br.MText;
                                txt.Add(leaderText);
                            }
                        }
                    }
                }
            }

            return txt;
        }

        private static List<BlockReference> getAllBlockReference(string blockName, Transaction trans)
        {
            List<BlockReference> refs = new List<BlockReference>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            if (bt.Has(blockName))
            {
                BlockTableRecord btr = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;

                foreach (ObjectId id in btr)
                {
                    DBObject currentEntity = trans.GetObject(id, OpenMode.ForWrite, false) as DBObject;

                    if (currentEntity == null)
                    {
                        continue;
                    }

                    else if (currentEntity is BlockReference)
                    {
                        BlockReference blockRef = currentEntity as BlockReference;

                        BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = trans.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        }
                        else
                        {
                            block = trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
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

        private static List<BlockReference> getAllBlockReference(List<string> blockNames, Transaction trans)
        {
            List<BlockReference> refs = new List<BlockReference>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            BlockTableRecord btr = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite) as BlockTableRecord;

            foreach (ObjectId id in btr)
            {
                DBObject currentEntity = trans.GetObject(id, OpenMode.ForWrite, false) as DBObject;

                if (currentEntity == null)
                {
                    continue;
                }

                else if (currentEntity is BlockReference)
                {
                    BlockReference blockRef = currentEntity as BlockReference;

                    BlockTableRecord block = null;
                    if (blockRef.IsDynamicBlock)
                    {
                        block = trans.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    }
                    else
                    {
                        block = trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
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