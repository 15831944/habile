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
    public class TableRow : IEquatable<TableRow>
    {
        G.Point _IP;

        string _Position = "nul";
        string _Material = "nul";

        int _Count = 0;
        int _Diameter = -9;

        int _Length = 0;

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

        public string Position { get { return _Position; } set { _Position = value; } }
        public string Material { get { return _Material; } set { _Material = value; } }

        public int Count { get { return _Count; } set { _Count = value; } }
        public int Diameter { get { return _Diameter; } set { _Diameter = value; } }

        public int Length { get { return _Length; } set { _Length = value; } }

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

        public TableRow(G.Point position)
        {
            _IP = position;
        }

        public TableRow(Bending b)
        {
            _Position = b.Position;
            _Material = b.Material;

            _Count = 0;
            _Diameter = b.Diameter;
            _Length = b.Length;

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


        public bool Equals(TableRow other)
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
            return Equals(obj as TableRow);
        }

        public static bool operator ==(TableRow a, TableRow b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(TableRow a, TableRow b)
        {
            return !object.Equals(a, b);
        }

    }
}

