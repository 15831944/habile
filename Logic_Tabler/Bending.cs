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
    public class Bending : IEquatable<Bending>
    {
        G.Point _IP;
        string _BlockName;

        string _Position = "nul";
        string _Material = "nul";        
        int _Diameter = -9;
        string _Shape = "nul";

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
        public string BlockName { get { return _BlockName; } }

        public string Position { get { return _Position; } set { _Position = value; } }
        public string Material { get { return _Material; } set { _Material = value; } }
        
        public int Diameter { get { return _Diameter; } set { _Diameter = value; } }

        public int Length { get { return _Length; } set { _Diameter = value; } }

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

        public Bending(G.Point position, string blockName )
        {
            _IP = position;
            _BlockName = blockName;
        }

        public bool validator()
        {
            if (Material == "nul")
            {
                setInvalid("Error - BENDING BLOCK - Material = nul");
                return false;
            }

            if (Position == "nul")
            {
                setInvalid("Error - BENDING BLOCK - Position = nul");
                return false;
            }

            _Length = 0;

            if (_BlockName == "Raud_K")
            {
                _Length = (int)(A + B - R - R + (R * Math.PI));
            }
            else if (_BlockName == "Raud_U")
            {
                _Length = (A + B + C) * 2;
            }
            else if (_BlockName == "Raud_T")
            {
                _Length = (int)(A * Math.PI + B);
            }
            else if (_BlockName == "Raud_V")
            {
                _Length = (int)(A + B + C - R - R - R + 2 * R * Math.PI);
            }
            else if (_BlockName == "Raud_Y")
            {
                _Length = A + B + C + B + A;
            }
            else if (_BlockName == "Raud_W")
            {
                _Length = (int)(A + B + C + D + E - R - R - R + 2 * R * Math.PI);
            }
            else if (_BlockName == "Raud_Z3")
            {
                _Length = (int)(A * Math.PI - B);
            }
            else if (_BlockName == "Raud_Z4")
            {
                _Length = (int)(0.5 * A * Math.PI + 2 * B);
            }
            else if (_BlockName == "Raud_Z5")
            {
                _Length = A + B + C + B + A;
            }
            else if (_BlockName == "Raud_Z7")
            {
                _Length = A + B + C + D + C + B + A;
            }
            else if (_BlockName == "Raud_Z8")
            {
                _Length = D + A + B + C + B + A + D;
            }
            else if (_BlockName == "Raud_Z9")
            {
                _Length = A + A + B + C + C + D;
            }
            else
            {
                if (A > 0) _Length += A;
                if (B > 0) _Length += B;
                if (C > 0) _Length += C;
                if (D > 0) _Length += D;
                if (E > 0) _Length += E;
                if (F > 0) _Length += F;
                if (G > 0) _Length += G;
            }


            if (Length == 0)
            {
                setInvalid("Error - BENDING BLOCK - \"" + _Position.ToString() + "\" - Length = 0");
                return false;
            }

            try
            {
                alfa2();
            }
            catch
            {
                setInvalid("Error - BENDING BLOCK - \"" + _Position.ToString() + "\" - Position invalid");
                return false;
            }

            return true;
        }

        private void alfa2()
        {
            string mark = _Position;
            if (_Position.Contains(@"\P"))
            {
                mark = _Position.Split('\\')[0];
            }

            int tot = mark.Length;

            int numCount = 0;
            int numShape = 0;
            int numDiam = 0;

            for (int j = 0; j < tot; j++)
            {
                char cur = mark[j];

                if (Char.IsNumber(cur))
                {
                    numShape = j;
                    break;
                }
            }

            bool doubleDigit = false;
            for (int k = numShape; k < tot; k++)
            {
                char cur = mark[k];

                if (Char.IsNumber(cur))
                {
                    if (cur.Equals('1') && doubleDigit == false)
                    {
                        doubleDigit = true;
                        continue;
                    }
                    else if (cur.Equals('2') && doubleDigit == false)
                    {
                        doubleDigit = true;
                        continue;
                    }
                    else if (cur.Equals('3') && doubleDigit == false)
                    {
                        doubleDigit = true;
                        continue;
                    }
                    else
                    {
                        numDiam = k + 1;
                        break;
                    }
                }
            }

            int temp = numShape;
            _Shape = mark.Substring(numCount, temp);

            temp = numDiam - numShape;
            _Diameter = Int32.Parse(mark.Substring(numShape, temp));

            temp = tot - numDiam;
            int other = Int32.Parse(mark.Substring(numDiam, temp));
        }

        private void setInvalid(string reason)
        {
            _valid = false;
            _reason = reason;
        }

        public bool Equals(Bending other)
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
            return Equals(obj as Bending);
        }

        public static bool operator ==(Bending a, Bending b)
        {
            return object.Equals(a, b);
        }

        public static bool operator !=(Bending a, Bending b)
        {
            return !object.Equals(a, b);
        }
    }
}

