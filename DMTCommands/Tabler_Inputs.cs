using System;
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
    partial class Tabler
    {
        //
        //DRAWING AREA
        //
        private List<G.Area> getAllAreas(string blockName)
        {
            List<G.Area> areas = new List<G.Area>();

            List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
            areas = getBoxAreas(blocks);

            if (areas.Count < 1) throw new DMTException("[ERROR] " + blockName + " not found");
            return areas;
        }


        private List<G.Area> getBoxAreas(List<_Db.BlockReference> blocks)
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
        private List<T.TableHead> getAllTableHeads(string blockName)
        {
            List<T.TableHead> heads = new List<T.TableHead>();

            List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
            heads = getTableHeadData(blocks);

            if (heads.Count < 1) throw new DMTException("[ERROR] " + blockName + " not found");
            return heads;
        }


        private List<T.TableHead> getTableHeadData(List<_Db.BlockReference> blocks)
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
        //MARKS
        //
        private List<T.ReinforcementMark> getAllMarks(string layer)
        {
            List<T.ReinforcementMark> marks = new List<T.ReinforcementMark>();

            List<_Db.MText> allTexts = getAllText(layer);
            marks = getMarkData(allTexts);

            if (marks.Count < 1) throw new DMTException("[ERROR] Reinforcement marks not found");
            return marks;
        }


        private List<T.ReinforcementMark> getMarkData(List<_Db.MText> txts)
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
                    write("[WARNING] VIIDE - \"" + txt.Contents + "\" - could not read!");
                }
            }

            return parse;
        }



        //
        //BENDING
        //
        public List<T.Bending> getAllBendings(List<string> bendingNames)
        {
            List<T.Bending> bendings = new List<T.Bending>();

            List<_Db.BlockReference> blocks = new List<_Db.BlockReference>();
            foreach (string name in bendingNames)
            {
                List<_Db.BlockReference> block = getAllBlockReference(name);
                blocks.AddRange(block);
            }

            bendings = getBendingData(blocks);

            return bendings;
        }


        private List<T.Bending> getBendingData(List<_Db.BlockReference> blocks)
        {
            List<T.Bending> parse = new List<T.Bending>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.Bending current = new T.Bending(insp, block.Name);

                foreach (_Db.ObjectId arId in block.AttributeCollection)
                {
                    _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
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
                    write(current.Reason);
                }
            }

            return parse;
        }


        private void setBendingParameters(_Db.AttributeReference ar, T.Bending bending)
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
        //TABLE ROWS
        //
        public List<T.TableRow> getAllTableRows(string blockName)
        {
            List<T.TableRow> rows = new List<T.TableRow>();

            List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
            rows = getRowData(blocks);

            return rows;
        }


        private List<T.TableRow> getRowData(List<_Db.BlockReference> blocks)
        {
            List<T.TableRow> parse = new List<T.TableRow>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableRow current = new T.TableRow(insp);

                foreach (_Db.ObjectId arId in block.AttributeCollection)
                {
                    _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
                    _Db.AttributeReference ar = obj as _Db.AttributeReference;
                    setRowAttribute(ar, current);
                }

                parse.Add(current);
            }

            return parse;
        }


        private void setRowAttribute(_Db.AttributeReference ar, T.TableRow row)
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
        //SUMMARY
        //        
        public List<T.TableSummary> getAllTableSummarys(string blockName)
        {
            List<T.TableSummary> sums = new List<T.TableSummary>();

            List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
            sums = getSummaryData(blocks);

            return sums;
        }


        private List<T.TableSummary> getSummaryData(List<_Db.BlockReference> blocks)
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
        private List<_Db.BlockReference> getAllBlockReference(string blockName)
        {
            List<_Db.BlockReference> refs = new List<_Db.BlockReference>();
            
            if (_c.blockTable.Has(blockName))
            {
                foreach (_Db.ObjectId id in _c.modelSpace)
                {
                    _Db.DBObject currentEntity = _c.trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.DBObject;
                    if (currentEntity == null) continue;

                    if (currentEntity is _Db.BlockReference)
                    {
                        _Db.BlockReference blockRef = currentEntity as _Db.BlockReference;

                        _Db.BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = _c.trans.GetObject(blockRef.DynamicBlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }
                        else
                        {
                            block = _c.trans.GetObject(blockRef.BlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }

                        if (block == null) continue;
                        
                        if (block.Name == blockName)
                        {
                            refs.Add(blockRef);
                        }
                    }
                }
            }

            return refs;
        }


        private List<_Db.MText> getAllText(string layer)
        {
            List<_Db.MText> txt = new List<_Db.MText>();

            foreach (_Db.ObjectId id in _c.modelSpace)
            {
                _Db.Entity currentEntity = _c.trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.Entity;
                if (currentEntity == null) continue;

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

            return txt;
        }

    }
}