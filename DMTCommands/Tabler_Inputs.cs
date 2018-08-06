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
        //
        //DRAWING AREA
        //
        private List<G.Area> getAllAreas(List<string> blockNames)
        {
            List<G.Area> areas = new List<G.Area>();

            foreach (string blockName in blockNames)
            { 
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
                List<G.Area> temp = getBoxAreas(blocks);
                areas.AddRange(temp);
            }

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
        private List<T.TableHead> getAllTableHeads(List<string> blockNames)
        {
            List<T.TableHead> heads = new List<T.TableHead>();

            foreach (string blockName in blockNames)
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
                List<T.TableHead> temp = getTableHeadData(blocks);
                heads.AddRange(temp);
            }                       

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

                string lang = getAttributeValue(block, "Keele valik");
                if (lang != null) current.setLanguange(lang);

                parse.Add(current);
            }

            return parse;
        }


        private string getAttributeValue(_Db.BlockReference block, string attribute)
        {
            _Db.DynamicBlockReferencePropertyCollection aa = block.DynamicBlockReferencePropertyCollection;
            foreach (_Db.DynamicBlockReferenceProperty a in aa)
            {
                if (a.PropertyName == attribute) return a.Value.ToString();
            }

            return null;
        }


        //
        //MARKS
        //
        private List<T.ReinforcementMark> getAllMarks()
        {
            List<_Db.MText> allTexts = getAllText(markLayer);
            List<T.ReinforcementMark> marks = getMarkData(allTexts);

            return marks;
        }


        private List<T.ReinforcementMark> getMarkData(List<_Db.MText> txts)
        {
            List<T.ReinforcementMark> parse = new List<T.ReinforcementMark>();

            foreach (_Db.MText txt in txts)
            {
                G.Point insp = new G.Point(txt.Location.X, txt.Location.Y);
                T.ReinforcementMark current = new T.ReinforcementMark(insp, txt.Contents);
                parse.Add(current);
            }

            return parse;
        }


        //
        //BENDING
        //
        public List<T.BendingShape> getAllBendings(List<string> blockNames)
        {
            List<T.BendingShape> bendings = new List<T.BendingShape>();

            foreach (string blockName in blockNames)
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
                List<T.BendingShape> temp = getBendingData(blocks);
                bendings.AddRange(temp);
            }            

            return bendings;
        }


        private List<T.BendingShape> getBendingData(List<_Db.BlockReference> blocks)
        {
            List<T.BendingShape> parse = new List<T.BendingShape>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.BendingShape current = new T.BendingShape(insp, block.Name);

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


        private void setBendingParameters(_Db.AttributeReference ar, T.BendingShape bending)
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
        public List<T.TableBendingRow> getAllTableRows(List<string> blockNames)
        {
            List<T.TableBendingRow> rows = new List<T.TableBendingRow>();

            foreach (string blockName in blockNames)
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
                List<T.TableBendingRow> temp = getRowData(blocks);
                rows.AddRange(temp);
            }

            return rows;
        }


        private List<T.TableBendingRow> getRowData(List<_Db.BlockReference> blocks)
        {
            List<T.TableBendingRow> parse = new List<T.TableBendingRow>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableBendingRow current = new T.TableBendingRow(insp);

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


        private void setRowAttribute(_Db.AttributeReference ar, T.TableBendingRow row)
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
        public List<T.TableMaterialRow> getAllTableSummarys(List<string> blockNames)
        {
            List<T.TableMaterialRow> sums = new List<T.TableMaterialRow>();

            foreach (string blockName in blockNames)
            {
                List<_Db.BlockReference> blocks = getAllBlockReference(blockName);
                List<T.TableMaterialRow> temp = getSummaryData(blocks);
                sums.AddRange(temp);
            }

            return sums;
        }


        private List<T.TableMaterialRow> getSummaryData(List<_Db.BlockReference> blocks)
        {
            List<T.TableMaterialRow> parse = new List<T.TableMaterialRow>();

            foreach (_Db.BlockReference block in blocks)
            {
                G.Point insp = new G.Point(block.Position.X, block.Position.Y);
                T.TableMaterialRow temp = new T.TableMaterialRow(insp);
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
            List<_Db.MText> txts = new List<_Db.MText>();

            foreach (_Db.ObjectId id in _c.modelSpace)
            {
                _Db.Entity currentEntity = _c.trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.Entity;

                if (currentEntity == null) continue;

                if (currentEntity is _Db.MText)
                {
                    _Db.MText mtxt = currentEntity as _Db.MText;

                    if (mtxt.Layer == layer)
                    {
                        txts.Add(mtxt);
                    }
                }
                else if (currentEntity is _Db.DBText)
                {
                    _Db.DBText dbt = currentEntity as _Db.DBText;
                    if (dbt.Layer == layer)
                    {
                        _Db.MText mtxt = new _Db.MText();
                        mtxt.Contents = dbt.TextString;
                        mtxt.Location = dbt.Position;
                        txts.Add(mtxt);
                    }
                }
                else if (currentEntity is _Db.MLeader)
                {
                    _Db.MLeader ml = currentEntity as _Db.MLeader;
                    if (ml.Layer == layer)
                    {
                        if (ml.ContentType == _Db.ContentType.MTextContent)
                        {
                            _Db.MText mtxt = ml.MText;
                            txts.Add(mtxt);
                        }
                    }
                }
            }

            return txts;
        }

    }
}