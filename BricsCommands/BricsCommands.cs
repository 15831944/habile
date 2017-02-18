using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using SW = System.Windows.Forms;

//ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

//Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;

[assembly: CommandClass(typeof(commands.CadCommands))]
namespace commands
{
    public class CadCommands
    {
        [CommandMethod("AERDEBUG")]
        public void debuger()
        {
            Reinforcer.main();
        }

        [CommandMethod("AEINFO")]
        public void info()
        {
            SW.MessageBox.Show("Versioon: 07.02.2017\n");
        }

        [CommandMethod("AER")]
        public void one()
        {
            try
            {
                Reinforcer.main();
            }
            catch (System.Exception ex)
            {
                SW.MessageBox.Show("Viga\n" + ex.Message);
            }
        }

        [CommandMethod("AEB")]
        public void two()
        {
            try
            {
                Tabler program = new Tabler();
                program.main2();
            }
            catch (System.Exception ex)
            {
                SW.MessageBox.Show("Viga\n" + ex.Message);
            }
        }

        [CommandMethod("AEM")]
        public void three()
        {
            try
            {
                Tabler program = new Tabler();
                program.main3();
            }
            catch (System.Exception ex)
            {
                SW.MessageBox.Show("Viga\n" + ex.Message);
            }
        }

        [CommandMethod("AEK")]
        public void four()
        {
            try
            {
                Tabler program = new Tabler();
                program.main4();
            }
            catch (System.Exception ex)
            {
                SW.MessageBox.Show("Viga\n" + ex.Message);
            }
        }
    }
}