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
    partial class Reinforcer
    {
        private void getSettings()
        {
            _Db.TypedValue[] filterlist = new _Db.TypedValue[2];
            filterlist[0] = new _Db.TypedValue(0, "INSERT");
            filterlist[1] = new _Db.TypedValue(2, "Reinf_program_settings");
            _Ed.SelectionFilter filter = new _Ed.SelectionFilter(filterlist);

            _Ed.PromptSelectionOptions opts = new _Ed.PromptSelectionOptions();
            opts.MessageForAdding = "\nSelect Reinf_program_settings block: ";

            _Ed.PromptSelectionResult selection = _c.ed.GetSelection(opts, filter);
            if (selection.Status != _Ed.PromptStatus.OK) throw new DMTException("\n[ERROR] Reinf_program_settings - cancelled");
            if (selection.Value.Count != 1) throw new DMTException("\n[ERROR] Reinf_program_settings - too many in selection");


            _Db.ObjectId selectionId = selection.Value.GetObjectIds()[0];
            _Db.BlockReference selectionBR = _c.trans.GetObject(selectionId, _Db.OpenMode.ForWrite) as _Db.BlockReference;

            L._V_.Z_DRAWING_SCALE = selectionBR.ScaleFactors.X;

            foreach (_Db.ObjectId arId in selectionBR.AttributeCollection)
            {
                _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
                _Db.AttributeReference ar = obj as _Db.AttributeReference;

                if (ar != null)
                {
                    setProgramVariables(ar);
                }
            }
        }


        private void setProgramVariables(_Db.AttributeReference ar)
        {
            if (ar.Tag == "ARMATUURI_MARK")
            {
                L._V_.X_REINFORCEMENT_MARK = ar.TextString;
            }
            else
            {
                double number;
                string txt = ar.TextString;
                txt = txt.Replace('.', ',');
                bool parser = Double.TryParse(txt, out number);

                //A
                if (ar.Tag == "ELEMENDI_LAIUS")
                {
                    if (parser)
                    {
                        L._V_.X_ELEMENT_WIDTH = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] ELEMENDI_LAIUS - Setting invalid");
                    }
                }


                //B
                else if (ar.Tag == "KAITSEKIHT")
                {
                    if (parser)
                    {
                        L._V_.X_CONCRETE_COVER_1 = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] KAITSEKIHT - Setting invalid");
                    }
                }
                else if (ar.Tag == "KIHTIDE_ARV")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_NUMBER = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] KIHTIDE_ARV - Setting invalid");
                    }
                }


                //C
                else if (ar.Tag == "PÕHIARMATUURI_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_MAIN_DIAMETER = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] PÕHIARMATUURI_DIAM - Setting invalid");
                    }
                }
                else if (ar.Tag == "PÕHIARMATUURI_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] PÕHIARMATUURI_ANKURDUS - Setting invalid");
                    }
                }
                else if (ar.Tag == "PÕHIARMATUURI_ABISUURUS")
                {
                    if (parser)
                    {
                        L._V_.X_FIRST_PASS_CONSTRAINT = number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] PÕHIARMATUURI_ABISUURUS - Setting invalid");
                    }
                }


                //D
                else if (ar.Tag == "DIAGONAALI_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_DIAGONAL_DIAMETER = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] DIAGONAALI_DIAM - Setting invalid");
                    }
                }
                else if (ar.Tag == "DIAGONAALI_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_DIAGONAL_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] DIAGONAALI_ANKURDUS - Setting invalid");
                    }
                }


                //E
                else if (ar.Tag == "RANGIDE_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_DIAMETER = (int)number;

                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] RANGIDE_DIAM - Setting invalid");
                    }
                }
                else if (ar.Tag == "RANGIDE_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_SPACING = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] RANGIDE_SAMM - Setting invalid");
                    }
                }
                else if (ar.Tag == "RANGIDE_ABISUURUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_CONSTRAINT = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] RANGIDE_ABISUURUS - Setting invalid");
                    }
                }


                //F
                else if (ar.Tag == "HORISONTAAL_D_IO")
                {
                    if (parser)
                    {
                        if ((int)number == 0 || (int)number == 1)
                        {
                            L._V_.X_REINFORCEMENT_SIDE_D_CREATE = (int)number;
                        }
                        else
                        {
                            throw new DMTException("\n[ERROR] HORISONTAAL_D_IO - Setting invalid");
                        }
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] HORISONTAAL_D_IO - Setting invalid");
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_DIAMETER = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] HORISONTAAL_D_DIAM - Setting invalid");
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] HORISONTAAL_D_ANKURDUS - Setting invalid");
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_SPACING = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] HORISONTAAL_D_SAMM - Setting invalid");
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_PARAND")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_FIX = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] HORISONTAAL_D_PARAND - Setting invalid");
                    }
                }


                //E
                else if (ar.Tag == "VERTIKAAL_D_IO")
                {
                    if (parser)
                    {
                        if ((int)number == 0 || (int)number == 1)
                        {
                            L._V_.X_REINFORCEMENT_TOP_D_CREATE = (int)number;
                        }
                        else
                        {
                            throw new DMTException("\n[ERROR] VERTIKAAL_D_IO - Setting invalid");
                        }
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] VERTIKAAL_D_IO - Setting invalid");
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_DIAMETER = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] VERTIKAAL_D_DIAM - Setting invalid");
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] VERTIKAAL_D_ANKURDUS - Setting invalid");
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_SPACING = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] VERTIKAAL_D_SAMM - Setting invalid");
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_PARAND")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_FIX = (int)number;
                    }
                    else
                    {
                        throw new DMTException("\n[ERROR] VERTIKAAL_D_PARAND - Setting invalid");
                    }
                }
            }
        }


        private List<G.Line> getGeometry()
        {
            List<G.Line> polys = new List<G.Line>();

            _Ed.PromptSelectionResult userSelection = _c.doc.Editor.GetSelection();

            if (userSelection.Status != _Ed.PromptStatus.OK) throw new DMTException("\n[ERROR] Geometry - cancelled");

            _Ed.SelectionSet selectionSet = userSelection.Value;

            foreach (_Ed.SelectedObject currentObject in selectionSet)
            {
                if (currentObject == null) continue;
                _Db.Entity currentEntity = _c.trans.GetObject(currentObject.ObjectId, _Db.OpenMode.ForRead) as _Db.Entity;
                if (currentEntity == null) continue;

                if (currentEntity is _Db.Polyline)
                {
                    _Db.Polyline poly = _c.trans.GetObject(currentEntity.ObjectId, _Db.OpenMode.ForRead) as _Db.Polyline;
                    int points = poly.NumberOfVertices;

                    for (int i = 1; i < points; i++)
                    {
                        _Ge.Point2d p1 = poly.GetPoint2dAt(i - 1);
                        _Ge.Point2d p2 = poly.GetPoint2dAt(i);

                        G.Point new_p1 = new G.Point(p1.X, p1.Y);
                        G.Point new_p2 = new G.Point(p2.X, p2.Y);

                        if (new_p1 == new_p2) continue;

                        G.Line line = new G.Line(new_p1, new_p2);
                        polys.Add(line);
                    }

                    if (poly.Closed)
                    {
                        _Ge.Point2d p1 = poly.GetPoint2dAt(points - 1);
                        _Ge.Point2d p2 = poly.GetPoint2dAt(0);
                        G.Point new_p1 = new G.Point(p1.X, p1.Y);
                        G.Point new_p2 = new G.Point(p2.X, p2.Y);

                        if (new_p1 == new_p2) continue;

                        G.Line line = new G.Line(new_p1, new_p2);
                        polys.Add(line);
                    }
                }
            }

            if (polys.Count < 3) throw new DMTException("\n[ERROR] Geometry - less then 3");

            return polys;
        }


        private G.Point getBendingInsertionPoint()
        {
            _Ed.PromptPointOptions pickedPointOptions = new _Ed.PromptPointOptions("\nSelect Bending insertion point");
            _Ed.PromptPointResult pickedPoint = _c.doc.Editor.GetPoint(pickedPointOptions);
            if (pickedPoint.Status != _Ed.PromptStatus.OK) throw new DMTException("\n[ERROR] Bending insertion point - cancelled");

            _Ge.Point3d pt = pickedPoint.Value;
            G.Point picked = new G.Point(pt.X, pt.Y);

            return picked;
        }

    }
}