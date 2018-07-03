using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;


namespace dmt_find_replace
{
    class Program
    {
        static bool kodu = false; //THIS

        static string fileName = "summary_csv_rebar";


        public static void Main()
        {
            string script = fileName + @"_script.scr";

            string program = "";
            if (kodu) program = @"C:\Program Files\Autodesk\AutoCAD 2013\acad.exe";
            else program = @"C:\Program Files\Bricsys\BricsCAD V16 en_US\bricscad.exe";


            Console.WriteLine("Enter source folder:");
            string location = Console.ReadLine();


            Console.WriteLine(String.Empty);

            Dictionary<string, string> fr = new Dictionary<string, string>();
            bool runprogram = true;

            while (true)
            {
                Console.WriteLine("Find:");
                string find = Console.ReadLine();

                Console.WriteLine("Replace:");
                string replace = Console.ReadLine();

                fr[find] = replace;

                Console.WriteLine("Press ENTER to run script, press ESCAPE to exit... Press any key to add another parameter");
                ConsoleKeyInfo userinput = Console.ReadKey();
                if (userinput.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (userinput.Key == ConsoleKey.Escape)
                {
                    runprogram = false;
                    break;
                }
            }


            if (runprogram)
            {
                List<string> dwgs = getFiles(location, "*.DWG");

                createScriptFile(dwgs, script, fr);
                runAutocad(program, script);
                deleteScriptFile(script);
            }


            Console.WriteLine(String.Empty);
            Console.WriteLine("[done]");
            Console.ReadLine();
        }


        private static List<string> getFiles(string source, string ext)
        {
            List<string> files = new List<string>();

            if (!Directory.Exists(Path.GetDirectoryName(source))) return files;

            string[] importFiles = Directory.GetFiles(source, ext);

            foreach (string file in importFiles)
            {
                files.Add(file);
            }

            return files;
        }


        public static void createScriptFile(List<string> dwgs, string script,  Dictionary<string, string> fr)
        {
            StringBuilder txt = new StringBuilder();

            foreach (string dwg in dwgs)
            {
                txt.AppendLine("_.open \"" + dwg + "\"");

                txt.AppendLine("ZOOM e");
                
                foreach (string find in fr.Keys)
                {
                    txt.AppendLine("AEFR");
                    txt.AppendLine(find);
                    txt.AppendLine(fr[find]);
                }

                txt.AppendLine("_.qsave");
            }

            txt.AppendLine("_.quit");

            string scriptText = txt.ToString();

            if (File.Exists(script))
            {
                File.Delete(script);
            }

            File.AppendAllText(script, scriptText);
        }


        private static void runAutocad(string program, string script)
        {
            string excet = program;
            string args = "/b \"" + script + "\"";
            Process.Start(excet, args);
        }


        private static void deleteScriptFile(string script)
        {
            Console.WriteLine("Waiting for BricsCad to close...");
            System.Threading.Thread.Sleep(5000);

            string procc = "";
            if (kodu) procc = "acad";
            else procc = "bricscad";

            while (IsProcessOpen(procc))
            {
                System.Threading.Thread.Sleep(1000);
            }
            try
            {
                File.Delete(script);
            }
            catch
            {

            }
        }


        private static bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
