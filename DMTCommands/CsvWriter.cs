//#define BRX_APP
#define ARX_APP

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
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
    class CsvWriter
    {
        struct Rebar
        {
            public string diam;
            public string mat;
            public string mass;
            public string unit;
        };


        struct Mesh
        {
            public string size;
            public string mat;
            public double area;
            public string unit;
        };


        _CONNECTION _c;

        static string[] titleBlockName = { "kirjanurk_h_EN" };
        static string[] titleNameAttribute = { "DRAWING_NR" };

        static string[] rebarMaterialBlockName = { "PainutusKokkuvõte" };
        static string[] rebarFilterOutWords = { "KOKKU" };
        static string[] rebarDiamAttribute = { "Diam" };
        static string[] rebarMatAttribute = { "Klass" };
        static string[] rebarMassAttribute = { "Mass" };
        static string[] rebarUnitAttribute = { "Ühik" };

        static string[] meshMaterialBlockName = { "materjalid_1" };
        static string[] meshFilterWords = { "MESH" };
        static string[] meshNameAttribute = { "NIMETUS" };
        static string[] meshSizeAttribute = { "TÄHIS" };
        static string[] meshMatAttribute = { "STANDARD" };
        static string[] meshAreaAttribute = { "HULK" };
        static string[] meshUnitAttribute = { "ÜHIK" };

        string output_dir = @"temp_csv\";


        public CsvWriter(ref _CONNECTION c)
        {
            _c = c;            
        }
        

        public void run()
        {
            string panelName = getPanelName();
            List<Rebar> rebar = getReinforcementData();
            List<Mesh> mesh = getMeshData();

            output(panelName, rebar, mesh);

        }


        private void output(string panelName, List<Rebar> rebar, List<Mesh> mesh)
        {
            string fileName = createFileName();
            createFolder(fileName);
            
            writeToCSV(fileName , panelName, rebar, mesh);
        }


        private string getPanelName()
        {
            List<_Db.BlockReference> titleblocks = getallBlocks(titleBlockName);
            if (titleblocks.Count < 1) throw new DMTException("[ERROR] Titleblock - NOT FOUND");

            List<string> names = getAllAttributeValues(titleblocks, titleNameAttribute);
            if (names.Count < 1) throw new DMTException("[ERROR] Titleblock - Name attribute not found");

            names = names.OrderBy(x => x.Length).ToList();

            return names.First();
        }


        private List<Rebar> getReinforcementData()
        {
            List<Rebar> reb = new List<Rebar>();

            List<_Db.BlockReference> rebarMaterialBlocks = getallBlocks(rebarMaterialBlockName);

            if (rebarMaterialBlocks.Count > 0)
            {
                rebarMaterialBlocks = filterOutByAttribute(rebarMaterialBlocks, rebarDiamAttribute, rebarFilterOutWords);

                List<string> diams = getAllAttributeValues(rebarMaterialBlocks, rebarDiamAttribute);
                if (rebarMaterialBlocks.Count != diams.Count) throw new DMTException("[ERROR] Rebar Material Block - Diam - attribute error");

                List<string> mats = getAllAttributeValues(rebarMaterialBlocks, rebarMatAttribute);
                if (rebarMaterialBlocks.Count != mats.Count) throw new DMTException("[ERROR] Rebar Material Block - Material - attribute error");

                List<string> masses = getAllAttributeValues(rebarMaterialBlocks, rebarMassAttribute);
                if (rebarMaterialBlocks.Count != masses.Count) throw new DMTException("[ERROR] Rebar Material Block - Mass - attribute error");

                List<string> units = getAllAttributeValues(rebarMaterialBlocks, rebarUnitAttribute);
                if (rebarMaterialBlocks.Count != units.Count) throw new DMTException("[ERROR] Rebar Material Block - Unit - attribute error");

                for (int i = 0; i < rebarMaterialBlocks.Count; i++)
                {
                    Rebar r;
                    r.diam = diams[i].ToUpper().Replace("%%C", "D");
                    r.mat = mats[i];
                    r.mass = masses[i];
                    r.unit = units[i];
                    reb.Add(r);
                }

                reb = reb.OrderBy(x => x.diam).ToList();
                reb = reb.OrderBy(x => x.diam.Length).ToList();
                reb = reb.OrderBy(x => x.mat).ToList();
            }
            else
            {
                write("[WARNING] Rebar Material Block - NOT FOUND");
            }

            return reb;
        }


        private List<Mesh> getMeshData()
        {
            List<Mesh> mesh = new List<Mesh>();

            List<_Db.BlockReference> meshMaterialBlocks = getallBlocks(meshMaterialBlockName);

            if (meshMaterialBlocks.Count > 0)
            {
                meshMaterialBlocks = filterByAttribute(meshMaterialBlocks, meshNameAttribute, meshFilterWords);

                List<string> sizes = getAllAttributeValues(meshMaterialBlocks, meshSizeAttribute);
                if (meshMaterialBlocks.Count != sizes.Count) throw new DMTException("[ERROR] Mesh Material Block - Size - attribute error");

                List<string> mats = getAllAttributeValues(meshMaterialBlocks, meshMatAttribute);
                if (meshMaterialBlocks.Count != mats.Count) throw new DMTException("[ERROR] Mesh Material Block - Materjal - attribute error");

                List<string> areas = getAllAttributeValues(meshMaterialBlocks, meshAreaAttribute);
                if (meshMaterialBlocks.Count != areas.Count) throw new DMTException("[ERROR] Mesh Material Block - Area - attribute error");

                List<string> units = getAllAttributeValues(meshMaterialBlocks, meshUnitAttribute);
                if (meshMaterialBlocks.Count != units.Count) throw new DMTException("[ERROR] Mesh Material Block - Unit - attribute error");

                for (int i = 0; i < meshMaterialBlocks.Count; i++)
                {
                    Mesh m;
                    m.size = sizes[i].ToUpper().Replace("#", "").Replace("%%C", "").Replace(" CC=", "-");
                    m.mat = mats[i];
                    m.area = parseArea(areas[i]);
                    m.unit = units[i];
                    mesh.Add(m);
                }

                mesh = mesh.OrderBy(x => x.area).ToList();
            }
            else
            {
                write("[WARNING] Mesh Material Block - NOT FOUND");
            }

            return mesh;
        }


        private double parseArea(string area)
        {
            string[] areas = area.Split(';');

            double value = 0;

            foreach (string a in areas)
            {
                try
                {
                    double temp = Double.Parse(a.Replace(',', '.'));
                    value = value + temp;
                }
                catch
                {
                    throw new DMTException("[ERROR] Mesh Material Block - Area - value error");
                }                
            }

            return value;
        }


        private string createFileName()
        {
            _Db.HostApplicationServices hs = _Db.HostApplicationServices.Current;
            string dwg_path = hs.FindFile(_c.doc.Name, _c.doc.Database, _Db.FindFileHint.Default);
            string dwg_dir = Path.GetDirectoryName(dwg_path);
            string dwg_name = Path.GetFileNameWithoutExtension(dwg_path);

            if (!dwg_dir.EndsWith(@"\")) { dwg_dir = dwg_dir + @"\"; }
            string csv_dir = dwg_dir + output_dir;
            string csv_path = csv_dir + dwg_name + ".csv";

            return csv_path;
        }


        private void createFolder(string fullPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            Thread.Sleep(10);

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                throw new DMTException("[ERROR] IO - Directory - creation failed");
            }

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch
                {
                    throw new DMTException("[ERROR] IO - File - file in use or access denied");
                }
                
            }

            Thread.Sleep(10);

            if (File.Exists(fullPath))
            {
                throw new DMTException("[ERROR] IO - File - file in use or access denied");
            }
        }


        private void writeToCSV(string fullPath, string panelName, List<Rebar> rebar, List<Mesh> mesh)
        {
            StringBuilder txt = new StringBuilder();

            write(fullPath);
            txt.AppendLine("alexi programmi ajutine file");
            txt.AppendLine(";");
            txt.AppendLine(";" + panelName + ";");

            foreach (Rebar r in rebar)
            {
                txt.AppendLine(";" + "Rebars;" + r.diam + ";" + r.mat + ";" + ";" + ";" + r.mass + ";" + r.unit + @"/total");
            }

            foreach (Mesh m in mesh)
            {
                txt.AppendLine(";" + "Mesh;" + m.size + ";" + m.mat + ";" + ";" + ";" + m.area + ";" + m.unit + @"/total");
            }

            string csvText = txt.ToString();

            File.AppendAllText(fullPath, csvText);

        }


        private List<_Db.BlockReference> filterOutByAttribute(List<_Db.BlockReference> blocks, string[] attribute, string[] filter)
        {
            List<_Db.BlockReference> filteredBlocks = new List<_Db.BlockReference>();

            foreach (_Db.BlockReference block in blocks)
            {
                string value = getAttributeValue(block, attribute);

                if (value != null)
                {
                    if (filter.Contains(value) == false)
                    {
                        filteredBlocks.Add(block);
                    }
                }
            }

            return filteredBlocks;
        }


        private List<_Db.BlockReference> filterByAttribute(List<_Db.BlockReference> blocks, string[] attribute, string[] filter)
        {
            List<_Db.BlockReference> filteredBlocks = new List<_Db.BlockReference>();

            foreach (_Db.BlockReference block in blocks)
            {
                string value = getAttributeValue(block, attribute);

                if (value != null)
                {
                    if (filter.Contains(value))
                    {
                        filteredBlocks.Add(block);
                    }
                }
            }

            return filteredBlocks;
        }


        private List<string> getAllAttributeValues(List<_Db.BlockReference> blocks, string[] attribute)
        {
            List<string> values = new List<string>();

            foreach (_Db.BlockReference block in blocks)
            {
                string value = getAttributeValue(block, attribute);

                if (value != null)
                {
                    values.Add(value);
                }                
            }

            return values;
        }


        private string getAttributeValue(_Db.BlockReference block, string[] attribute)
        {
            foreach (_Db.ObjectId arId in block.AttributeCollection)
            {
                _Db.DBObject obj = _c.trans.GetObject(arId, _Db.OpenMode.ForWrite);
                _Db.AttributeReference ar = obj as _Db.AttributeReference;

                if (ar != null)
                {
                    if (attribute.Contains(ar.Tag))
                    {
                        return ar.TextString;
                    }
                }
            }

            return null;
        }

        
        private List<_Db.BlockReference> getallBlocks(string[] blockNames)
        {
            List<_Db.BlockReference> blocks = new List<_Db.BlockReference>();

            foreach (string name in blockNames)
            {
                List<_Db.BlockReference> block = getAllBlockReference(name);
                blocks.AddRange(block);
            }

            return blocks;
        }


        private List<_Db.BlockReference> getAllBlockReference(string blockName)
        {
            List<_Db.BlockReference> refs = new List<_Db.BlockReference>();

            if (_c.blockTable.Has(blockName))
            {
                foreach (_Db.ObjectId id in _c.modelSpace)
                {
                    _Db.DBObject currentEntity = _c.trans.GetObject(id, _Db.OpenMode.ForWrite, false) as _Db.DBObject;
                    if (currentEntity == null) continue;

                    if (currentEntity is _Db.BlockReference)
                    {
                        _Db.BlockReference blockRef = currentEntity as _Db.BlockReference;

                        _Db.BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = _c.trans.GetObject(blockRef.DynamicBlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }
                        else
                        {
                            block = _c.trans.GetObject(blockRef.BlockTableRecord, _Db.OpenMode.ForRead) as _Db.BlockTableRecord;
                        }

                        if (block == null) continue;

                        if (block.Name == blockName)
                        {
                            refs.Add(blockRef);
                        }
                    }
                }
            }

            return refs;
        }


        private void write(string message)
        {
            _c.ed.WriteMessage("\n" + message);
        }

    }
}