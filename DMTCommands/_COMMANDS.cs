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


[assembly: _Trx.CommandClass(typeof(DMTCommands.CadCommands))]
namespace DMTCommands
{
    public class CadCommands
    {
        [_Trx.CommandMethod("AEINFO")]
        public void info()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    string version = String.Format("{0}", System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToShortDateString());
                    c.ed.WriteMessage("DMT armeerimis programmi versioon: " + version);
                }
                catch
                {
                    c.ed.WriteMessage("[ERROR] unknown exception");
                }
                finally
                {
                    c.ed.WriteMessage("\n[done]");
                    c.close();
                }                                
            }
            catch
            {
                _SWF.MessageBox.Show("Connection to BricsCad/AutoCad failed.");
            }            
        }


        [_Trx.CommandMethod("AER")]
        public void one()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    Reinforcer program = new Reinforcer(c);
                    program.run();
                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage(de.Message);
                }
                catch
                {
                    c.ed.WriteMessage("[ERROR] unknown exception");
                }
                finally
                {
                    c.ed.WriteMessage("\n[done]");
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("Connection to BricsCad/AutoCad failed.");
            }
        }


        [_Trx.CommandMethod("AEB")]
        public void two()
        {
            try
            {
                Tabler program = new Tabler();
                program.main2();
            }
            catch (System.Exception ex)
            {
                _SWF.MessageBox.Show("Viga\n" + ex.Message);
            }
        }


        [_Trx.CommandMethod("AEM")]
        public void three()
        {
            try
            {
                Tabler program = new Tabler();
                program.main3();
            }
            catch (System.Exception ex)
            {
                _SWF.MessageBox.Show("Viga\n" + ex.Message);
            }
        }


        [_Trx.CommandMethod("AEK")]
        public void four()
        {
            try
            {
                Tabler program = new Tabler();
                program.main4();
            }
            catch (System.Exception ex)
            {
                _SWF.MessageBox.Show("Viga\n" + ex.Message);
            }
        }

    }
}