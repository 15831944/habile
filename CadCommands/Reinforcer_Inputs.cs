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

using G = Geometry;
using L = Logic_Reinf;

namespace commands
{
    static class Reinforcer_Inputs
    {
        public static bool getSettingsVariables()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            TypedValue[] filterlist = new TypedValue[2];
            filterlist[0] = new TypedValue(0, "INSERT");
            filterlist[1] = new TypedValue(2, "Reinf_program_settings");

            SelectionFilter filter = new SelectionFilter(filterlist);

            PromptSelectionOptions opts = new PromptSelectionOptions();
            opts.MessageForAdding = "\nSelect SETTINGS BLOCK: ";

            PromptSelectionResult selection = ed.GetSelection(opts, filter);

            if (selection.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nERROR - SETTINGS BLOCK not found");
                return false;
            }
            if (selection.Value.Count != 1)
            {
                ed.WriteMessage("\nERROR - Too many SETTINGS BLOCKs in selection");
                return false;
            }
            else
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    ObjectId selectionId = selection.Value.GetObjectIds()[0];
                    BlockReference selectionBR = trans.GetObject(selectionId, OpenMode.ForWrite) as BlockReference;

                    L._V_.Z_DRAWING_SCALE = selectionBR.ScaleFactors.X;

                    foreach (ObjectId arId in selectionBR.AttributeCollection)
                    {
                        DBObject obj = trans.GetObject(arId, OpenMode.ForWrite);
                        AttributeReference ar = obj as AttributeReference;
                        if (ar != null)
                        {
                            bool success = setProgramVariables(ar, ed);
                            if (!success) return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool setProgramVariables(AttributeReference ar, Editor ed)
        {
            if (ar.Tag == "ARMATUURI_MARK")
            {
                L._V_.X_REINFORCEMENT_MARK = ar.TextString;
            }
            else
            {
                double number;
                bool parser = Double.TryParse(ar.TextString, out number);

                //A
                if (ar.Tag == "ELEMENDI_LAIUS")
                {
                    if (parser)
                    {
                        L._V_.X_ELEMENT_WIDTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - ELEMENDI_LAIUS");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - KAITSEKIHT");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - KIHTIDE_ARV");
                        return false;
                    }
                }


                //C
                else if (ar.Tag == "PÕHIARMATUURI_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_MAIN_DIAMETER = (int)number;

                        if (number > 16)
                        {
                            L._V_.Y_REINFORCEMENT_MAIN_RADIUS = (int)number * 7;
                        }
                        else
                        {
                            L._V_.Y_REINFORCEMENT_MAIN_RADIUS = (int)number * 4;
                        }

                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_DIAM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_ANKURDUS");
                        return false;
                    }
                }
                //else if (ar.Tag == "PÕHIARMATUURI_RAADIUS")
                //{
                //    if (parser)
                //    {
                //        L._V_.X_REINFORCEMENT_MAIN_RADIUS = (int)number;
                //    }
                //    else
                //    {
                //        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_RAADIUS");
                //        return false;
                //    }
                //}
                else if (ar.Tag == "PÕHIARMATUURI_ABISUURUS")
                {
                    if (parser)
                    {
                        L._V_.X_FIRST_PASS_CONSTRAINT = number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_ABISUURUS");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - DIAGONAALI_DIAM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - DIAGONAALI_ANKURDUS");
                        return false;
                    }
                }


                //E
                else if (ar.Tag == "RANGIDE_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_DIAMETER = (int)number;

                        if (number > 16)
                        {
                            L._V_.Y_REINFORCEMENT_STIRRUP_RADIUS = (int)number * 7;
                        }
                        else
                        {
                            L._V_.Y_REINFORCEMENT_STIRRUP_RADIUS = (int)number * 4;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - RANGIDE_DIAM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - RANGIDE_SAMM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - RANGIDE_ABISUURUS");
                        return false;
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
                            ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_IO");
                            return false;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_IO");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_DIAM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_ANKURDUS");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_SAMM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_PARAND");
                        return false;
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
                            ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_IO");
                            return false;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_IO");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_DIAM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_ANKURDUS");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_SAMM");
                        return false;
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
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_PARAND");
                        return false;
                    }
                }
            }

            return true;
        }

        public static List<G.Line> getSelectedPolyLines()
        {
            List<G.Line> polys = new List<G.Line>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                PromptSelectionResult userSelection = doc.Editor.GetSelection();

                if (userSelection.Status == PromptStatus.OK)
                {
                    SelectionSet selectionSet = userSelection.Value;

                    foreach (SelectedObject currentObject in selectionSet)
                    {
                        if (currentObject != null)
                        {
                            Entity currentEntity = trans.GetObject(currentObject.ObjectId, OpenMode.ForRead) as Entity;

                            if (currentEntity != null)
                            {
                                if (currentEntity is Polyline)
                                {
                                    Polyline poly = trans.GetObject(currentEntity.ObjectId, OpenMode.ForRead) as Polyline;
                                    int points = poly.NumberOfVertices;

                                    for (int i = 1; i < points; i++)
                                    {
                                        Point2d p1 = poly.GetPoint2dAt(i - 1);
                                        Point2d p2 = poly.GetPoint2dAt(i);

                                        G.Point new_p1 = new G.Point(p1.X, p1.Y);
                                        G.Point new_p2 = new G.Point(p2.X, p2.Y);

                                        if (new_p1 == new_p2) continue;

                                        G.Line line = new G.Line(new_p1, new_p2);
                                        polys.Add(line);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return polys;
        }


        public static G.Point getBendingInsertionPoint()
        {
            G.Point picked;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                PromptPointResult pickedPoint;
                PromptPointOptions pickedPointOptions = new PromptPointOptions("\nSelect bending INSERTION POINT");

                pickedPoint = doc.Editor.GetPoint(pickedPointOptions);
                Point3d pt = pickedPoint.Value;

                picked = new G.Point(pt.X, pt.Y);
            }

            return picked;
        }
    }
}