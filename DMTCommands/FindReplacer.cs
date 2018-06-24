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
    partial class FindReplacer
    {
        _CONNECTION _c;


        public FindReplacer(ref _CONNECTION c)
        {
            _c = c;
        }


        public void run()
        {
            _Ed.PromptStringOptions pStrOpts = new _Ed.PromptStringOptions("\nFIND: ");
            pStrOpts.AllowSpaces = true;
            _Ed.PromptResult pr = _c.ed.GetString(pStrOpts);
            if (pr.Status != _Ed.PromptStatus.OK) return;
            string find = pr.StringResult;

            pStrOpts = new _Ed.PromptStringOptions("\nREPLACE: ");
            pStrOpts.AllowSpaces = true;
            pr = _c.ed.GetString(pStrOpts);
            if (pr.Status != _Ed.PromptStatus.OK) return;
            string replace = pr.StringResult;

            renameText(find, replace);
            renameBlockInside(find, replace);
            renameBlockFields(find, replace);
            renameLayout(find, replace);
        }


        public void renameText(string find, string replace)
        {
            foreach (_Db.ObjectId id in _c.modelSpace)
            {
                _Db.Entity currentEntity = _c.trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.Entity;

                if (currentEntity == null)
                {
                    continue;
                }
                if (currentEntity is _Db.MText)
                {
                    _Db.MText txt = currentEntity as _Db.MText;
                    txt.Contents = txt.Contents.Replace(find, replace);
                }
                if (currentEntity is _Db.DBText)
                {
                    _Db.DBText txt = currentEntity as _Db.DBText;
                    txt.TextString = txt.TextString.Replace(find, replace);
                }
            }
        }


        public void renameBlockInside(string find, string replace)
        {
            foreach (_Db.ObjectId btrId in _c.blockTable)
            {
                _Db.BlockTableRecord btr = _c.trans.GetObject(btrId, _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

                foreach (_Db.ObjectId bid in btr)
                {
                    _Db.Entity currentEntity = _c.trans.GetObject(bid, _Db.OpenMode.ForWrite) as _Db.Entity;

                    if (currentEntity == null)
                    {
                        continue;
                    }
                    if (currentEntity is _Db.MText)
                    {
                        _Db.MText txt = currentEntity as _Db.MText;
                        txt.Contents = txt.Contents.Replace(find, replace);
                        txt.DowngradeOpen();
                    }
                    if (currentEntity is _Db.DBText)
                    {
                        _Db.DBText txt = currentEntity as _Db.DBText;
                        txt.TextString = txt.TextString.Replace(find, replace);
                    }
                }
            }
        }


        public void renameBlockFields(string find, string replace)
        {
            foreach (_Db.ObjectId btrId in _c.blockTable)
            {
                _Db.BlockTableRecord btr = _c.trans.GetObject(btrId, _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

                _Db.ObjectIdCollection blockRefIds = btr.GetBlockReferenceIds(true, true);

                foreach (_Db.ObjectId btRfId in blockRefIds)
                {
                    _Db.BlockReference br = _c.trans.GetObject(btRfId, _Db.OpenMode.ForWrite) as _Db.BlockReference;

                    if (br != null)
                    {
                        foreach (_Db.ObjectId arId in br.AttributeCollection)
                        {
                            _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
                            _Db.AttributeReference ar = obj as _Db.AttributeReference;

                            if (ar != null)
                            {
                                ar.TextString = ar.TextString.Replace(find, replace);
                            }
                        }
                    }
                }
            }
        }


        public void renameLayout(string find, string replace)
        {
            List<string> layouts = new List<string>();

            _Db.DBDictionary lays = _c.trans.GetObject(_c.db.LayoutDictionaryId, _Db.OpenMode.ForWrite) as _Db.DBDictionary;

            foreach (_Db.DBDictionaryEntry item in lays)
            {
                if (item.Key.Contains(find))
                {
                    string name = item.Key;
                    layouts.Add(name);
                }
            }

            foreach (string lay in layouts)
            {
                _Db.LayoutManager lm = _Db.LayoutManager.Current;
                string newname = lay.Replace(find, replace);
                lm.RenameLayout(lay, newname);
            }
        }

    }
}
