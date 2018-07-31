using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;

namespace Logic_Tabler
{
    public static class HandlerChecker
    {

        public static List<ErrorPoint> main(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<BendingShape> bendings, List<TableBendingRow> rows, List<TableMaterialRow> summarys)
        {
            List<ErrorPoint> errors = new List<ErrorPoint>();
            List<DrawingArea> fields = sortData(areas, heads, marks, bendings, rows, summarys);

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

                List<ErrorPoint> currentErrors = dosomeshitmagic(f);
                errors.AddRange(currentErrors);
            }

            return errors;
        }


        private static List<ErrorPoint> dosomeshitmagic(DrawingArea field)
        {
            List<ErrorPoint> errors = new List<ErrorPoint>();

            var dublicateTableRows = field._rows
                    .GroupBy(i => i.Position)
                    .Where(g => g.Count() > 1)
                    .Select(j => j.First());

            foreach (TableBendingRow r in dublicateTableRows)
            {
                ErrorPoint er = new ErrorPoint(r.IP, "[VIGA] - TABEL - DUBLICATE - " + r.Position, field._tableHeads[0].Scale);
                errors.Add(er);
            }

            var dublicateBendings = field._bendings
                .GroupBy(i => i.Position)
                .Where(g => g.Count() > 1)
                .Select(j => j.First());

            foreach (BendingShape b in dublicateBendings)
            {
                ErrorPoint er = new ErrorPoint(b.IP, "[VIGA] - PAINUTUS - DUBLICATE - " + b.Position, field._tableHeads[0].Scale);
                errors.Add(er);
            }

            foreach (TableBendingRow r in field._rows)
            {
                if (r.Count == 0)
                {
                    ErrorPoint er = new ErrorPoint(r.IP, "[VIGA] - TABEL - KOGUS -> 0", field._tableHeads[0].Scale);
                    errors.Add(er);
                }
            }

            foreach (ReinforcementMark m in field._marks)
            {
                bool found = false;
                foreach (TableBendingRow r in field._rows)
                {
                    if (m.Position == r.Position)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    ErrorPoint er = new ErrorPoint(m.IP, "[VIGA] - VIIDE - EI LEIA TABELIST - " + m.Position, field._tableHeads[0].Scale);
                    errors.Add(er);
                }
            }

            foreach (ReinforcementMark m in field._marks)
            {
                bool found = false;
                foreach (BendingShape b in field._bendings)
                {
                    if (m.Position == b.Position)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false && m.Shape != "A")
                {
                    ErrorPoint er = new ErrorPoint(m.IP, "[VIGA] - VIIDE - EI LEIA PAINUTUST - " + m.Position, field._tableHeads[0].Scale);
                    errors.Add(er);
                }
            }
            
            foreach (ReinforcementMark m in field._marks)
            {
                bool found = false;
                foreach (ReinforcementMark n in field._marks)
                {
                    if (ReferenceEquals(m, n)) continue;

                    if (m.IP == n.IP)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == true)
                {
                    ErrorPoint er = new ErrorPoint(m.IP, "[VIGA] - VIIDE - TOPELT VIIDE - " + m.Position, field._tableHeads[0].Scale);
                    if (!errors.Contains(er)) errors.Add(er);
                }
            }

            foreach (BendingShape b in field._bendings)
            {
                bool found = false;
                foreach (TableBendingRow r in field._rows)
                {
                    if (b.Position == r.Position)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    ErrorPoint er = new ErrorPoint(b.IP, "[VIGA] - PAINUTUS - EI LEIA TABELIST - " + b.Position, field._tableHeads[0].Scale);
                    errors.Add(er);
                }
            }

            foreach (BendingShape b in field._bendings)
            {
                bool found = false;
                foreach (ReinforcementMark m in field._marks)
                {
                    if (b.Position == m.Position)
                    {
                        found = true;
                        break;
                    }
                    
                }

                if (found == false)
                {
                    ErrorPoint er = new ErrorPoint(b.IP, "[VIGA] - PAINUTUS - EI LEIA VIIDET - " + b.Position, field._tableHeads[0].Scale);
                    errors.Add(er);
                }
            }

            return errors;
        }


        public static G.Point getSummaryInsertionPoint(DrawingArea field)
        {
            if (field.Valid)
            {
                double tempX = field._tableHeads[0].IP.X;
                double tempY = field._tableHeads[0].IP.Y;

                foreach (TableBendingRow r in field._rows)
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


        private static List<DrawingArea> sortData(List<G.Area> areas, List<TableHead> heads, List<ReinforcementMark> marks, List<BendingShape> bendings, List<TableBendingRow> rows, List<TableMaterialRow> summarys)
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

            foreach (TableMaterialRow summary in summarys)
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
