using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;


namespace summary_csv_rebar
{
    class Program
    {
        static bool kodu = false; //THIS

        static string fileName = "summary_csv_rebar";
        static string output_dir = @"temp_csv\";

        static void Main(string[] args)
        {
            string script = fileName + @"_script.scr";

            string program = "";
            if (kodu) program = @"C:\Program Files\Autodesk\AutoCAD 2013\acad.exe";
            else program = @"C:\Program Files\Bricsys\BricsCAD V16 en_US\bricscad.exe";
            

            Console.WriteLine("Enter source folder:");
            string location = Console.ReadLine();
            if (!location.EndsWith(@"\")) { location = location + @"\"; }
            string csv_location = location + output_dir;

            List<string> dwgs = getFiles(location, "*.DWG");

            createScriptFile(dwgs, script);
            runAutocad(program, script);
            deleteScriptFile(script);

            List<string> csvs = getFiles(csv_location, "*.CSV");
            List<string> parsed = parseCSVs(csvs);

            Console.WriteLine("Writing output...");
            string msg = output(location, parsed);

            Console.WriteLine(msg);
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


        private static void createScriptFile(List<string> dwgs, string script)
        {
            StringBuilder txt = new StringBuilder();
            
            foreach (string dwg in dwgs)
            {
                txt.AppendLine("_.open \"" + dwg + "\"");
                txt.AppendLine("CSV_REINF_SINGLE");
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


        private static List<string> parseCSVs(List<string> csvs)
        {
            List<string> file_rows = new List<string>();

            StreamReader reader = null;

            foreach (string csv in csvs)
            {
                file_rows.Add("");
                reader = new StreamReader(File.OpenRead(csv));
                readCSV(reader, ref file_rows);
            }

            return file_rows;
        }


        private static void readCSV(StreamReader reader, ref List<string> file_rows)
        {
            string line = reader.ReadLine();

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();

                if (line == null) continue;
                if (line == "") continue;

                file_rows.Add(line);
            }

            reader.Close();
        }


        private static string output(string location, List<string> data)
        {
            string extention = ".csv";

            int i = 1;

            string destination = location + fileName + extention;

            while (File.Exists(destination))
            {
                destination = location + fileName + i.ToString("D3") + extention;
                i++;
            }

            if (data == null || data.Count == 0) return "[ERROR] Data not found";
            if (!Directory.Exists(Path.GetDirectoryName(destination))) return "[ERROR] Directory not found";

            StringBuilder txt = new StringBuilder();

            txt.AppendLine("Project number;Element pos. number;Part name;material;exposure class;Profile;Quantity;units;Height H, mm;Length L, mm;Width B, mm;c.nom;REI ;mu0;empty;Length/pcs, m;Lentgh/total, m;Area netto, m2;Area brutto, m2;Weight;Weight/total;Comment;Dr.number;Dr.date;REV; Rev.date;Rev.comment;ORDER;Erection start;Actual er. start;Fabrication start;Actual fab. start;empty;empty ;GUID:");

            foreach (string row in data)
            {
                txt.AppendLine(row);
            }

            string csvText = txt.ToString();

            File.AppendAllText(destination, csvText);

            System.Threading.Thread.Sleep(100);

            try
            {
                Process.Start(destination);
            }
            catch
            {

            }

            return "[done]";
        }

    }
}
