using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Logic_Tabler
{
    public class DrawingArea
    {
        public G.Area _area;

        public List<TableHead> _tableHeads;
        public List<ReinforcementMark> _marks;
        public List<Bending> _bendings;
        public List<TableRow> _rows;
        public List<TableSummary> _summarys;

        bool _valid = true;
        string _reason = "none";

        public bool Valid { get { return _valid; } }
        public string Reason { get { return _reason; } }


        public DrawingArea(G.Area area)
        {
            _area = area;

            _tableHeads = new List<TableHead>();
            _marks = new List<ReinforcementMark>();
            _bendings = new List<Bending>();
            _rows = new List<TableRow>();
            _summarys = new List<TableSummary>();

            _valid = true;
        }


        internal bool isInArea(G.Point p)
        {
            return _area.isPointInArea(p);
        }


        internal void addTableHead(TableHead th)
        {
            _tableHeads.Add(th);
        }


        internal void addMark(ReinforcementMark mk)
        {
            _marks.Add(mk);
        }


        internal void addBending(Bending bd)
        {
            _bendings.Add(bd);
        }

        
        internal void addRow(TableRow r)
        {
            _rows.Add(r);
        }

                        
        internal void addSummary(TableSummary p)
        {
            _summarys.Add(p);
        }


        internal void setInvalid(string reason)
        {
            _valid = false;
            _reason = reason;
        }

    }
}
