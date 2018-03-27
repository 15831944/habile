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


[assembly: _Trx.CommandClass(typeof(DMTCommands.CadCommands))]
namespace DMTCommands
{
    public class CadCommands
    {
        //TODO
        //[_Trx.CommandMethod("debug1")]
        //public void debug1()
        //{
        //    _CONNECTION c = new _CONNECTION();
        //    Reinforcer program = new Reinforcer(ref c);
        //    program.run();
        //}
        

        [_Trx.CommandMethod("AEINFO")]
        public void info()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    string version = String.Format("{0}", System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToShortDateString());
                    c.ed.WriteMessage("\nDMT armeerimis programmi versioon: " + version + "\n");
                    c.ed.WriteMessage("\n[DONE]");
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
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
                    Reinforcer program = new Reinforcer(ref c);
                    program.run();
                    c.ed.WriteMessage("\n[DONE]");
                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage("\n" + de.Message);
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
            }
        }


        [_Trx.CommandMethod("AEB")]
        public void two()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    Tabler program = new Tabler(ref c);
                    program.bending();
                    c.ed.WriteMessage("\n[DONE]");
                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage("\n" + de.Message);
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
            }
        }


        [_Trx.CommandMethod("AEM")]
        public void three()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    Tabler program = new Tabler(ref c);
                    program.material();
                    c.ed.WriteMessage("\n[DONE]");
                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage("\n" + de.Message);
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
            }
        }


        [_Trx.CommandMethod("AEK")]
        public void four()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    Tabler program = new Tabler(ref c);
                    program.check();
                    c.ed.WriteMessage("\n[DONE]");

                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage("\n" + de.Message);
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
            }
        }


        [_Trx.CommandMethod("FindReplaceAE")]
        public void dummy()
        {
            findReplacer();
        }


        [_Trx.CommandMethod("AEFR")]
        public void findReplacer()
        {
            try
            {
                _CONNECTION c = new _CONNECTION();

                try
                {
                    FindReplacer program = new FindReplacer(ref c);
                    program.run();
                    c.ed.WriteMessage("\n[DONE]");

                }
                catch (DMTException de)
                {
                    c.ed.WriteMessage("\n" + de.Message);
                }
                catch (Exception ex)
                {
                    c.ed.WriteMessage("\n[ERROR] Unknown Exception");
                    c.ed.WriteMessage("\n[ERROR] " + ex.Message);
                    c.ed.WriteMessage("\n[ERROR] " + ex.TargetSite);
                }
                finally
                {
                    c.close();
                }
            }
            catch
            {
                _SWF.MessageBox.Show("\n[ERROR] Connection to BricsCad/AutoCad failed.");
            }
        }

    }
}