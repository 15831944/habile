﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace habile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(@"C:\Program Files\Bricsys\BricsCAD V16 en_US\bricscad.exe"))
            {
                string run = @"C:\Program Files\Bricsys\BricsCAD V16 en_US\bricscad.exe";
                string netload = @"C:\Users\aleksandr.ess\Dropbox\CODE\habile\BricsCommands\bin\Debug\BricsCommands.dll";
                string dwg = @"C:\Users\aleksandr.ess\Dropbox\DMT\ERM\training3.dwg";
                string script = "script1.scr";

                createScriptFile(script, netload, dwg);
                runAutocad(run, script);
            }
            else if (File.Exists(@"C:\Program Files\Autodesk\AutoCAD 2013\acad.exe"))
            {
                string run = @"C:\Program Files\Autodesk\AutoCAD 2013\acad.exe";
                string netload = @"C:\Users\Alex\Dropbox\CODE\habile\CadCommands\bin\Debug\CadCommands.dll";
                string dwg = @"C:\Users\Alex\Dropbox\DMT\ERM\training3.dwg";
                string script = "script1.scr";

                createScriptFile(script, netload, dwg);
                runAutocad(run, script);
            }
            else if (File.Exists(@"C:\Program Files\Autodesk\AutoCAD 2014\acad.exe"))
            {
                string run = @"C:\Program Files\Autodesk\AutoCAD 2014\acad.exe";
                string netload = @"C:\Users\Alex\Dropbox\CODE\habile\CadCommands\bin\Debug\CadCommands.dll";
                string dwg = @"C:\Users\Alex\Dropbox\DMT\ERM\training.dwg";
                string script = "script1.scr";

                createScriptFile(script, netload, dwg);
                runAutocad(run, script);
            }
            else
            {
                Console.WriteLine("Viga programmi käivitamisel");
                Console.ReadLine();
            }
        }

        public static void createScriptFile(string script, string netload, string dwg)
        {
            StringBuilder scriptText = new StringBuilder();

            scriptText.AppendLine("_.open \"" + dwg + "\"");
            scriptText.AppendLine("NETLOAD \"" + netload + "\"");

            if (File.Exists(script))
            {
                File.Delete(script);
            }

            File.AppendAllText(script, scriptText.ToString());
        }

        public static void runAutocad(string program, string script)
        {
            string excet = program;
            string args = "/b \"" + script + "\" ";
            Process.Start(excet, args);
        }
    }
}
