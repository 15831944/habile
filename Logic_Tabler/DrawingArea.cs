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
        public List<BendingShape> _defaultMaterial;
        public List<TableBendingRow> _rows;
        public List<TableMaterialRow> _summarys;
        public List<ErrorPoint> _errors;

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
            _defaultMaterial = new List<BendingShape>();
            _rows = new List<TableBendingRow>();
            _summarys = new List<TableMaterialRow>();
            _errors = new List<ErrorPoint>();

            _valid = true;
        }


        public DrawingArea(bool pseudo)
        {
            _area = new G.Area(new G.Point(int.MinValue, int.MinValue), new G.Point(int.MaxValue, int.MaxValue));

            _tableHeads = new List<TableHead>();
            _marks = new List<ReinforcementMark>();
            _bendings = new List<BendingShape>();
            _defaultMaterial = new List<BendingShape>();
            _rows = new List<TableBendingRow>();
            _summarys = new List<TableMaterialRow>();
            _errors = new List<ErrorPoint>();

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


        internal void addMaterial(BendingShape df)
        {
            _defaultMaterial.Add(df);
        }


        internal void addRow(TableBendingRow r)
        {
            _rows.Add(r);
        }

                        
        internal void addSummary(TableMaterialRow p)
        {
            _summarys.Add(p);
        }


        internal void addError(ErrorPoint e)
        {
            _errors.Add(e);
        }


        internal void addErrors(List<ErrorPoint> e)
        {
            _errors.AddRange(e);
        }


        internal void setInvalid(string reason)
        {
            _valid = false;
            _reason = reason;
        }


        public G.Point getSummaryInsertionPoint()
        {
            if (_valid)
            {
                double tempX = _tableHeads[0].IP.X;
                double tempY = _tableHeads[0].IP.Y;

                foreach (TableBendingRow r in _rows)
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


        public void setErrorScale(double scale)
        {
            foreach (ErrorPoint e in _errors)
            {
                e.Scale = scale;
            }
        }

    }
}
