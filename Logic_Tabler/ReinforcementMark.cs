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
    public class ReinforcementMark
    {
        G.Point _IP;
        string _fullMark;

        public G.Point IP { get { return _IP; } }

        int _Count;
        string _Position;

        string _Shape;
        int _Diameter;
        int _Other;

        public int Count { get { return _Count; } }
        public string Position { get { return _Position; } }
        public string Shape { get { return _Shape; } }
        public int Diameter { get { return _Diameter; } }
        public int Other { get { return _Other; } }

        bool _valid = true;
        string _reason = "none";

        public bool Valid { get { return _valid; } }
        public string Reason { get { return _reason; } }

        public ReinforcementMark(G.Point position, string mark)
        {
            _IP = position;
            _fullMark = mark;
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
            string mark = _fullMark;

            if (_fullMark.Contains(@"\P"))
            {
                mark = _fullMark.Split('\\')[0];
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
                _Count = number * count;
            }
            else
            {
                _Count = Int32.Parse(mark.Substring(0, temp));
            }

            temp = tot - numCount;
            _Position = mark.Substring(numCount, temp);

            temp = numShape - numCount;
            _Shape = mark.Substring(numCount, temp);

            temp = numDiam - numShape;
            _Diameter = Int32.Parse(mark.Substring(numShape, temp));

            temp = tot - numDiam;
            _Other = Int32.Parse(mark.Substring(numDiam, temp));
        }

        public override string ToString() // debug
        {
            string str = _Count + " " + _Position + " " + _Shape + " " + _Diameter + " " + _Other;
            return str;
        }
    }
}

