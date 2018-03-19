using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public static class SummarHandler
    {
        public static List<DrawingArea> main(List<G.Area> areas, List<TableHead> heads, List<TableRow> rows, List<TableSummary> summarys)
        {
            List<DrawingArea> fields = sortData(areas, heads, rows, summarys);

            foreach (DrawingArea f in fields)
            {
                if (f._tableHeads.Count < 1)
                {
                    f.setInvalid("[WARNING] - Puudub [Painutustabel_pais]");
                    continue;
                }
                if (f._summarys.Count > 0)
                {
                    f.setInvalid("[WARNING] - Kaalu tabel on juba koostatud");
                    continue;
                }

                dosomeshitmagic(f);
            }

            return fields;
        }

        private static void dosomeshitmagic(DrawingArea field)
        {
            List<string> MaterialFilter = new List<string>();
            var DistinctItems = field._rows.GroupBy(x => x.Material).Select(y => y.First());
            foreach (TableRow item in DistinctItems)
            {
                MaterialFilter.Add(item.Material);
            }

            foreach (string mat in MaterialFilter)
            {
                List<TableRow> matList = field._rows.Where(x => x.Material == mat).ToList();

                List<int> DiameterFilter = new List<int>();
                DistinctItems = matList.GroupBy(x => x.Diameter).Select(y => y.First());
                foreach (TableRow item in DistinctItems)
                {
                    DiameterFilter.Add(item.Diameter);
                }

                double totWeight = 0;

                DiameterFilter.Sort();

                foreach (int diam in DiameterFilter)
                {
                    double radius = diam / 2;
                    double area = Math.PI * (Math.Pow(radius, 2));

                    List<TableRow> diamList = matList.Where(x => x.Diameter == diam).ToList();

                    TableSummary current = new TableSummary();
                    current.Text = "%%C" + diam.ToString();
                    current.Material = mat;
                    current.Weight = 0;
                    current.Units = "kg";

                    foreach (TableRow r in diamList)
                    {
                        double count = r.Count * r.Length * area;
                        double weight = count * 7850 / 1000000000;
                        current.Weight += weight;
                        totWeight += weight;
                    }

                    field.addSummary(current);
                }

                TableSummary total = new TableSummary();
                total.Text = "KOKKU";
                total.Material = mat;
                total.Weight = totWeight;
                total.Units = "kg";
                field.addSummary(total);
            }
        }


        public static G.Point getSummaryInsertionPoint(DrawingArea field)
        {
            if (field.Valid)
            {
                double tempX = field._tableHeads[0].IP.X;
                double tempY = field._tableHeads[0].IP.Y;

                foreach (TableRow r in field._rows)
                {
                    if (r.IP.Y < tempY)
                    {
                        tempY = r.IP.Y;
                    }
                }

                return new G.Point(tempX, tempY);
            }
            else
            {
                throw new Exception();
            }
        }

        private static List<DrawingArea> sortData(List<G.Area> areas, List<TableHead> heads, List<TableRow> rows, List<TableSummary> summarys)
        {
            List<DrawingArea> data = new List<DrawingArea>();

            foreach (G.Area cur in areas)
            {
                DrawingArea temp = new DrawingArea(cur);
                data.Add(temp);
            }

            foreach (TableHead head in heads)
            {
                foreach (DrawingArea cr in data)
                {
                    if (cr.isInArea(head.IP))
                    {
                        cr.addTableHead(head);
                        break;
                    }
                }
            }

            foreach (TableRow row in rows)
            {
                foreach (DrawingArea cr in data)
                {
                    if (cr.isInArea(row.IP))
                    {
                        cr.addRow(row);
                        break;
                    }
                }
            }

            foreach (TableSummary summary in summarys)
            {
                foreach (DrawingArea cr in data)
                {
                    if (cr.isInArea(summary.IP))
                    {
                        cr.addSummary(summary);
                        break;
                    }
                }
            }

            return data;
        }
    }
}
