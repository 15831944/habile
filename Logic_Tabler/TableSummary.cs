using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using G = Geometry;

namespace Logic_Tabler
{
    public class TableSummary
    {
        G.Point _IP;

        string _text;
        string _material;
        double _weight;
        string _units;

        public string Text { get { return _text; } set { _text = value; } }
        public string Material { get { return _material; } set { _material = value; } }
        public double Weight { get { return _weight; } set { _weight = value; } }
        public string Units { get { return _units; } set { _units = value; } }

        public G.Point IP { get { return _IP; } }


        public TableSummary(G.Point position)
        {
            _IP = position;
        }


        public TableSummary()
        {

        }

    }
}
