//#define BRX_APP
#define ARX_APP

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
    class _CONNECTION
    {
        _Ap.Document _doc;
        _Db.Database _db;
        _Ed.Editor _ed;
        _Db.Transaction _trans;

        _Db.BlockTable _blockTable;
        _Db.BlockTableRecord _modelSpace;

        public _Ap.Document doc { get { return _doc; } }
        public _Db.Database db { get { return _db; } }
        public _Ed.Editor ed { get { return _ed; } }
        public _Db.Transaction trans { get { return _trans; } }

        public _Db.BlockTable blockTable { get { return _blockTable; } }
        public _Db.BlockTableRecord modelSpace { get { return _modelSpace; } }


        public _CONNECTION()
        {
            _doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _db = _doc.Database;
            _ed = _doc.Editor;
            _trans = _db.TransactionManager.StartTransaction();
            
            _blockTable = _trans.GetObject(_db.BlockTableId, _Db.OpenMode.ForRead) as _Db.BlockTable;
            _modelSpace = _trans.GetObject(_Db.SymbolUtilityServices.GetBlockModelSpaceId(_db), _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;
        }
        

        internal void close()
        {
            _ed.WriteMessage("\n[close connection]\n");
            _trans.Commit();
            _trans.Dispose();
            _ed.Regen();
        }

    }
}