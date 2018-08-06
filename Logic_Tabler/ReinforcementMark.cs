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
    public class ReinforcementMark : IEquatable<ReinforcementMark>
    {
        G.Point _IP;
        string _content;

        public G.Point IP { get { return _IP; } }

        int _count;
        string _position;

        string _shape;
        int _diameter;
        int _other;

        public string Content { get { return _content; } }

        public int Count { get { return _count; } }
        public string Position { get { return _position; } }
        public string Shape { get { return _shape; } }
        public int Diameter { get { return _diameter; } }
        public int Other { get { return _other; } }

        bool _valid = true;
        string _reason = "none";

        public bool Valid { get { return _valid; } }
        public string Reason { get { return _reason; } }


        public ReinforcementMark(G.Point position, string mark)
        {
            _IP = position;
            _content = mark;
        }


        public bool validate()
        {
            try
            {
                alfa();
            }
            catch
            {
                return false;
            }

            return true;
        }


        private void alfa()
        {
            string mark = _content;

            if (_content.Contains(@"\P"))
            {
                mark = _content.Split('\\')[0];
            }

            int tot = mark.Length;

            int numCount = 0;
            bool multiplier = false;
            int multiplierCount = 0;

            int numShape = 0;
            int numDiam = 0;

            for (int i = 0; i < tot; i++)
            {
                char cur = mark[i];
                if (!Char.IsNumber(cur))
                {
                    if (Char.Equals(cur, '*') && multiplier == false)
                    {
                        multiplier = true;
                        multiplierCount = i;
                    }
                    else
                    {
                        numCount = i;
                        break;
                    }
                }
            }

            for (int j = numCount; j < tot; j++)
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

            int temp = numCount;
            if (multiplier == true)
            {
                temp = numCount - multiplierCount - 1;
                int number = Int32.Parse(mark.Substring(0, multiplierCount));

                int count = Int32.Parse(mark.Substring(multiplierCount + 1, temp));
                _count = number * count;
            }
            else
            {
                _count = Int32.Parse(mark.Substring(0, temp));
            }

            temp = tot - numCount;
            _position = mark.Substring(numCount, temp);

            temp = numShape - numCount;
            _shape = mark.Substring(numCount, temp);

            temp = numDiam - numShape;
            _diameter = Int32.Parse(mark.Substring(numShape, temp));

            temp = tot - numDiam;
            _other = Int32.Parse(mark.Substring(numDiam, temp));
        }


        public override string ToString() // debug
        {
            string str = _count + " " + _position + " " + _shape + " " + _diameter + " " + _other;
            return str;
        }


        public bool Equals(ReinforcementMark other)
        {
            if (other == null) return false;
            return (this.IP == other.IP &&
                    this._content == other._content);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as ReinforcementMark);
        }


        public static bool operator ==(ReinforcementMark a, ReinforcementMark b)
        {
            return object.Equals(a, b);
        }


        public static bool operator !=(ReinforcementMark a, ReinforcementMark b)
        {
            return !object.Equals(a, b);
        }

    }
}

