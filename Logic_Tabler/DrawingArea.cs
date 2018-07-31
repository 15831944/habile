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
        public List<BendingShape> _bendings;
        public List<TableBendingRow> _rows;
        public List<TableMaterialRow> _summarys;

        bool _valid = true;
        string _reason = "none";

        public bool Valid { get { return _valid; } }
        public string Reason { get { return _reason; } }


        public DrawingArea(G.Area area)
        {
            _area = area;

            _tableHeads = new List<TableHead>();
            _marks = new List<ReinforcementMark>();
            _bendings = new List<BendingShape>();
            _rows = new List<TableBendingRow>();
            _summarys = new List<TableMaterialRow>();

            _valid = true;
        }


        public DrawingArea(bool pseudo)
        {
            _area = new G.Area(new G.Point(int.MinValue, int.MinValue), new G.Point(int.MaxValue, int.MaxValue));

            _tableHeads = new List<TableHead>();
            _marks = new List<ReinforcementMark>();
            _bendings = new List<BendingShape>();
            _rows = new List<TableBendingRow>();
            _summarys = new List<TableMaterialRow>();

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


        internal void addBending(BendingShape bd)
        {
            _bendings.Add(bd);
        }

        
        internal void addRow(TableBendingRow r)
        {
            _rows.Add(r);
        }

                        
        internal void addSummary(TableMaterialRow p)
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
