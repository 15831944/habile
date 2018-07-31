using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public class HandlerBending
    {

        public HandlerBending()
        {

        }


        public List<DrawingArea> main(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<BendingShape> bendings, List<TableBendingRow> rows)
        {
            List<DrawingArea> fields = sortData(areas, heads, marks, bendings, rows);

            foreach (DrawingArea f in fields)
            {
                if (f._tableHeads.Count < 1)
                {
                    f.setInvalid("[WARNING] - Painutustabel_pais - Puudub, ala jääb vahele");
                    continue;
                }
                if (f._tableHeads.Count > 1)
                {
                    f.setInvalid("[WARNING] - Painutustabel_pais - Rohkem kui 1, ala jääb vahele");
                    continue;
                }
                if (f._rows.Count > 0)
                {
                    f.setInvalid("[WARNING] - Tabel on juba koostatud");
                    continue;
                }
                if (f._marks.Count == 0)
                {
                    f.setInvalid("[WARNING] - Raamjoone vahel pole ühtegi viidet");
                    continue;
                }

                string defaultMaterial = "B500B";

                foreach (BendingShape b in f._bendings)
                {
                    TableBendingRow r = new TableBendingRow(b);
                    f.addRow(r);
                }

                try
                {
                    defaultMaterial = f._rows.GroupBy(i => i.Material).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                }
                catch
                {

                }
                

                foreach (ReinforcementMark m in f._marks)
                {
                    bool found = false;
                    foreach (TableBendingRow r in f._rows)
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
                            BendingShape newBending = new BendingShape(m.IP, "Raud_A");
                            newBending.A = m.Other;
                            newBending.Material = defaultMaterial;
                            newBending.Position = m.Position;

                            if (newBending.validator())
                            {
                                TableBendingRow newRow = new TableBendingRow(newBending);
                                newRow.Count += m.Count;
                                f.addRow(newRow);
                            }
                        }
                    }
                }

                f._rows = f._rows.OrderBy(b => b.Shape).ThenBy(s => s.Diameter).ThenBy(s => s.Length).ToList();
            }

            return fields;
        }


        private List<DrawingArea> sortData(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<BendingShape> bendings, List<TableBendingRow> rows)
        {
            List<DrawingArea> data = new List<DrawingArea>();

            if (areas.Count != 0)
            {
                foreach (G.Area cur in areas)
                {
                    DrawingArea temp = new DrawingArea(cur);
                    data.Add(temp);
                }
            }
            else
            {
                DrawingArea temp = new DrawingArea(true);
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

            foreach (BendingShape bending in bendings)
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

            foreach (TableBendingRow row in rows)
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
