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
    public class TableBendingRow : IEquatable<TableBendingRow>
    {
        G.Point _IP;

        string _position = "nul";
        string _material = "nul";
        string _shape = "nul";

        int _count = 0;
        int _diameter = -9;

        int _length = 0;

        int _A = -1;
        int _B = -1;
        int _C = -1;
        int _D = -1;
        int _E = -1;
        int _F = -1;
        int _G = -1;

        string _U = "";
        string _V = "";

        int _R = -1;

        int _X = -1;
        int _Y = -1;

        public G.Point IP { get { return _IP; } }

        public string Position { get { return _position; } set { _position = value; } }
        public string Material { get { return _material; } set { _material = value; } }
        public string Shape { get { return _shape; } set { _shape = value; } }

        public int Count { get { return _count; } set { _count = value; } }
        public int Diameter { get { return _diameter; } set { _diameter = value; } }

        public int Length { get { return _length; } set { _length = value; } }

        public int A { get { return _A; } set { _A = value; } }
        public int B { get { return _B; } set { _B = value; } }
        public int C { get { return _C; } set { _C = value; } }
        public int D { get { return _D; } set { _D = value; } }
        public int E { get { return _E; } set { _E = value; } }
        public int F { get { return _F; } set { _F = value; } }
        public int G { get { return _G; } set { _G = value; } }

        public string U { get { return _U; } set { _U = value; } }
        public string V { get { return _V; } set { _V = value; } }

        public int R { get { return _R; } set { _R = value; } }

        public int X { get { return _X; } set { _X = value; } }
        public int Y { get { return _Y; } set { _Y = value; } }

        bool _valid = true;
        string _reason = "none";

        public bool Valid { get { return _valid; } }
        public string Reason { get { return _reason; } }


        public TableBendingRow(G.Point position)
        {
            _IP = position;
        }


        public TableBendingRow(BendingShape b)
        {
            _shape = b.Shape;
            _position = b.Position;
            _material = b.Material;

            _count = 0;
            _diameter = b.Diameter;
            _length = b.Length;

            _A = b.A;
            _B = b.B;
            _C = b.C;
            _D = b.D;
            _E = b.E;
            _F = b.F;
            _G = b.G;

            _U = b.U;
            _V = b.V;

            _R = b.R;

            _X = b.X;
            _Y = b.Y;
        }


        public bool validator()
        {
            if (Position == "nul")
            {
                setInvalid("[ERROR] - TABLE ROW - Position = nul");
                return false;
            }

            if (Material == "nul")
            {
                setInvalid("[ERROR] - TABLE ROW - Material = nul");
                return false;
            }

            if (Diameter < 1)
            {
                setInvalid("[ERROR] - TABLE ROW - Diameter");
                return false;
            }
            
            return true;
        }


        private void setInvalid(string reason)
        {
            _valid = false;
            _reason = reason;
        }


        public bool Equals(TableBendingRow other)
        {
            if (other == null) return false;
            return (this.Position == other.Position &&
                    this.Material == other.Material);
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as TableBendingRow);
        }


        public static bool operator ==(TableBendingRow a, TableBendingRow b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(TableBendingRow a, TableBendingRow b)
        {
            return !object.Equals(a, b);
        }

    }
}

