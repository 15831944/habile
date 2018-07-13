using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using G = Geometry;

namespace Reinforcement
{
    public class Raud_Array
    {
        internal List<Raud> _array;
        internal G.Point _IP;
        internal int _CC;

        public List<Raud> array { get { return _array; } }
        public G.Point IP { get { return _IP; } }


        public Raud_Array(int CC)
        {
            _array = new List<Raud>();
            _CC = CC;
        }


        public void add_one(Raud one)
        {
            _array.Add(one);
            int index = (int)_array.Count / 2;
            _IP = _array[index].IP;
        }


        public override string ToString()
        {
            if (_array.Count > 0)
            {
                return _array.Count.ToString() + _array[0].ToStringNoCount() + @"\PS=" + _CC.ToString();
            }
            else
            {
                return "empty array ToString Call";
            }
        }

    }
}

