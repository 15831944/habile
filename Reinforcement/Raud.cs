using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Reinforcement
{
    public abstract class Raud : IEquatable<Raud>
    {
        internal G.Point _StartPoint;
        internal G.Point _EndPoint;
        internal G.Point _IP;

        double _Rotation;

        int _Number;
        int _Diameter;
        string _Materjal;

        public G.Point StartPoint { get { return _StartPoint; } }
        public G.Point EndPoint { get { return _EndPoint; } }
        public G.Point IP { get { return _IP; } }
        public double Rotation { get { return _Rotation; } }
        public int Number { get { return _Number; } }
        public int Diameter { get { return _Diameter; } }
        public string Materjal { get { return _Materjal; } }

        public Raud(G.Line main, int nr, int d, string teras)
        {
            G.Vector dir = main.getDirectionVector();
            G.Polar pol = G.Converter.xy_to_la(dir);

            _Rotation = G.Converter.Wrap(pol.angle, Math.PI * 2, 0.0);

            _Number = nr;
            _Diameter = d;
            _Materjal = teras;

            double shorterLength = shorter(main.Length());
            double originalLength = main.Length();
            double delta = (originalLength - shorterLength) / 2;

            _StartPoint = main.Start.move(delta, dir);
            _EndPoint = _StartPoint.move(shorterLength, dir);
        }

        internal int shorter(double len)
        {
            return (int)Math.Floor(len / 5 + 0.01) * 5;
        }

        public abstract override string ToString();
        public abstract string ToStringNoCount();
        public abstract bool Equals(Raud other);
        public abstract override bool Equals(object obj);
    }
}