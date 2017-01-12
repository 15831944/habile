using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public static class TablerHandler
    {
        public static List<DrawingArea> main(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<Bending> bendings, List<TableRow> rows)
        {
            List<DrawingArea> fields = sortData(areas, heads, marks, bendings, rows);

            foreach (DrawingArea f in fields)
            {
                if (f._tableHeads.Count < 1)
                {
                    f.setInvalid("WARNING - Puudub Painutustabel_pais");
                    continue;
                }
                if (f._rows.Count > 0)
                {
                    f.setInvalid("WARNING - Tabel on juba koostatud");
                    continue;
                }

                foreach (Bending b in f._bendings)
                {
                    TableRow r = new TableRow(b);
                    f.addRow(r);
                }

                foreach (ReinforcementMark m in f._marks)
                {
                    bool found = false;
                    foreach (TableRow r in f._rows)
                    {
                        if (m.Position == r.Position)
                        {
                            r.Count += m.Count;
                            found = true;
                            break;
                        }
                    }

                    if (found == false)
                    {
                        if (m.Shape == "A")
                        {
                            Bending newBending = new Bending(m.IP, "Raud_A");
                            newBending.A = m.Other;
                            newBending.Material = "B500B";
                            newBending.Position = m.Position;

                            if (newBending.validator())
                            {
                                TableRow newRow = new TableRow(newBending);
                                newRow.Count += m.Count;
                                f.addRow(newRow);
                            }
                        }
                    }
                }

                f._rows = f._rows.OrderBy(b => b.Position).ToList();
            }

            return fields;
        }

        private static List<DrawingArea> sortData(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<Bending> bendings, List<TableRow> rows)
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

            foreach (ReinforcementMark mark in marks)
            {
                foreach (DrawingArea cr in data)
                {
                    if (cr.isInArea(mark.IP))
                    {
                        cr.addMark(mark);
                        break;
                    }
                }
            }

            foreach (Bending bending in bendings)
            {
                foreach (DrawingArea cr in data)
                {
                    if (cr.isInArea(bending.IP))
                    {
                        cr.addBending(bending);
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
            
            return data;
        }
    }
}
