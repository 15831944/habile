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


namespace DMTCommands
{
    class VIIDE_command
    {
        _CONNECTION _c;

        static string[] blockNames = { "viide" };
        static string[] viideTopAttribute = { "ÜLEMINE_TEKST" };
        static string[] viideBottomAttribute = { "ALUMINE_TEKST" };

        struct Viide
        {
            public string txt;
            public _Ge.Point3d ip;
            public int number;
            public string item;
        };


        public VIIDE_command(ref _CONNECTION c)
        {
            _c = c;
        }


        internal void run()
        {
            List<_Db.BlockReference> blocks = getAllBlocks(blockNames);

            if (blocks.Count == 0)
            {
                string names = string.Join(", ", blockNames.ToArray());
                throw new DMTException("[ERROR] - (" + names + ") - not found");
            }

            List<Viide> data = getData(blocks);
            if (data.Count == 0) throw new DMTException("[ERROR] Viga andmete lugemisel ");

            _Ge.Point3d ip = getPoint();

            output(data, ip);

        }


        private _Ge.Point3d getPoint()
        {
            _Ed.PromptPointOptions pPtOpts = new _Ed.PromptPointOptions("Select output location: ");
            _Ed.PromptPointResult pPtRes = _c.ed.GetPoint(pPtOpts);

            if (pPtRes.Status != _Ed.PromptStatus.OK) throw new DMTException("[ERROR] cancelled");

            return pPtRes.Value;
        }


        private List<_Db.BlockReference> getAllBlocks(string[] blockNames)
        {
            List<_Db.BlockReference> blocks = new List<_Db.BlockReference>();

            foreach (string name in blockNames)
            {
                List<_Db.BlockReference> block = getAllBlockReference(name);
                blocks.AddRange(block);
            }

            return blocks;
        }


        private List<Viide> getData(List<_Db.BlockReference> blocks)
        {
            List<Viide> data = new List<Viide>();

            foreach (_Db.BlockReference block in blocks)
            {
                string v1 = getAttributeValue(block, viideTopAttribute);
                string v2 = getAttributeValue(block, viideBottomAttribute);

                if (v1 == "" && v2 == "")
                {
                    write("[WARNING] viite väärtused tühjad");
                    continue;
                }

                string viide = v1 + " " + v2;
                int number = 1;
                string item = "";

                bool valid = validate(viide, ref number, ref item);
                if (valid)
                {
                    Viide v;
                    v.ip = block.Position;
                    v.txt = viide;
                    v.item = item;
                    v.number = number;
                    data.Add(v);
                }
            }

            return data;
        }

    
        private bool validate(string viide, ref int number, ref string item)
        {
            item = viide.ToUpper();
            return true;
        }


        private void output(List<Viide> data, _Ge.Point3d ip)
        {
            _Ge.Point3d currentPoint = ip;
            double size = 3;
            double scale = 40;
            double delta = scale * size;

            currentPoint = new _Ge.Point3d(ip.X, currentPoint.Y - delta, 0);

            foreach (Viide v in data)
            {
                string outputText = v.item + " [" + v.number + "]";
                insertText(currentPoint, outputText, scale);

                currentPoint = new _Ge.Point3d(ip.X, currentPoint.Y - delta, 0);
            }
        }


        private void insertText(_Ge.Point3d ip, string txt, double scale)
        {
            _Db.MText mText = new _Db.MText();
            mText.SetDatabaseDefaults();
            mText.TextHeight = 2.5 * scale;
            mText.Contents = txt;
            mText.Location = ip;
            
            _c.modelSpace.AppendEntity(mText);
            _c.trans.AddNewlyCreatedDBObject(mText, true);
        }


        private string getAttributeValue(_Db.BlockReference block, string[] attribute)
        {
            foreach (_Db.ObjectId arId in block.AttributeCollection)
            {
                _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
                _Db.AttributeReference ar = obj as _Db.AttributeReference;

                if (ar != null)
                {
                    if (attribute.Contains(ar.Tag))
                    {
                        return ar.TextString;
                    }
                }
            }

            return "";
        }


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


        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}
