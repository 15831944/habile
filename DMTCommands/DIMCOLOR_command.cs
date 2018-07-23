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


namespace DMTCommands
{
    class DIMCOLOR_command
    {
        _CONNECTION _c;


        public DIMCOLOR_command(ref _CONNECTION c)
        {
            _c = c;
        }


        internal void run()
        {
            List<_Db.Dimension> dims = getAllDims();
            int c = changeDimColor(dims);
            write("Changed dimentions color count: " + c.ToString());
        }


        private List<_Db.Dimension> getAllDims()
        {
            List<_Db.Dimension> dims = new List<_Db.Dimension>();

            foreach (_Db.ObjectId btrId in _c.blockTable)
            {
                _Db.BlockTableRecord btr = _c.trans.GetObject(btrId, _Db.OpenMode.ForWrite) as _Db.BlockTableRecord;

                if (!(btr.IsFromExternalReference))
                {
                    foreach (_Db.ObjectId bid in btr)
                    {
                        _Db.Entity currentEntity = _c.trans.GetObject(bid, _Db.OpenMode.ForWrite, false) as _Db.Entity;

                        if (currentEntity == null)
                        {
                            continue;
                        }

                        if (currentEntity is _Db.Dimension)
                        {
                            _Db.Dimension dim = currentEntity as _Db.Dimension;
                            dims.Add(dim);
                        }
                    }
                }
            }

            return dims;
        }


        private int changeDimColor(List<_Db.Dimension> dims)
        {
            int count = 0;

            foreach (_Db.Dimension dim in dims)
            {
                if (dim.ColorIndex != 256 || dim.Dimclrt != _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 256) || dim.Dimtfill != 0 || dim.Dimtfillclr != _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 256))
                {
                    dim.ColorIndex = 256;
                    dim.Dimclrt = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 256);
                    dim.Dimtfill = 0;
                    dim.Dimtfillclr = _Cm.Color.FromColorIndex(_Cm.ColorMethod.None, 256);

                    count++;
                }
            }

            return count;
        }


        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}
