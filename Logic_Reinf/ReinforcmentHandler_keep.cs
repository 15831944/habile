using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;
using R = Reinforcement; 

namespace Logic_Reinf
{
    public partial class ReinforcmentHandler
    {
        private void keep(R.Raud reinf, G.Edge edge, G.Corner corner1, G.Corner corner2)
        {
            if (edge != null)
            {
                if (!setEdges.Keys.Contains(edge))
                {
                    setEdges[edge] = reinf;
                }
            }

            if (corner1 != null)
            {
                if (!setCorners.Keys.Contains(corner1))
                {
                    setCorners[corner1] = reinf;
                }
            }

            if (corner2 != null)
            {
                if (!setCorners.Keys.Contains(corner2))
                {
                    setCorners[corner2] = reinf;
                }
            }

            knownReinforcement.Add(reinf);
        }

        private void keep_double(R.Raud reinf, G.Edge edge)
        {
            if (edge != null)
            {
                if (!setEdges.Keys.Contains(edge))
                {
                    setEdges[edge] = reinf;
                }
            }
        }

        private void keep_array(R.Raud reinf, LineSegment ls)
        {
            if (ls != null)
            {
                if (!setLineSegment.Contains(ls))
                {
                    setLineSegment.Add(ls);
                }
            }
        }

        private void keep_replace(R.Raud nw, R.Raud old1, R.Raud old2)
        {
            ReplaceByValue(setEdges, old1, nw);
            ReplaceByValue(setEdges, old2, nw);
            ReplaceByValue(setCorners, old1, nw);
            ReplaceByValue(setCorners, old2, nw);

            for (int i = knownReinforcement.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(knownReinforcement[i], old1))
                {
                    knownReinforcement.RemoveAt(i);
                }
            }

            for (int i = knownReinforcement.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(knownReinforcement[i], old2))
                {
                    knownReinforcement.RemoveAt(i);
                }
            }

            knownReinforcement.Add(nw);
        }

        private void keep_replace(R.Raud n1, R.Raud n2, R.Raud old1, R.Raud old2)
        {
            ReplaceByValue(setEdges, old1, n1);
            ReplaceByValue(setEdges, old2, n1);
            ReplaceByValue(setCorners, old1, n2);
            ReplaceByValue(setCorners, old2, n2);


            for (int i = knownReinforcement.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(knownReinforcement[i], old1))
                {
                    knownReinforcement.RemoveAt(i);
                }
            }

            for (int i = knownReinforcement.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(knownReinforcement[i], old2))
                {
                    knownReinforcement.RemoveAt(i);
                }
            }

            knownReinforcement.Add(n1);
            knownReinforcement.Add(n2);
        }

        private void keep_remove(R.Raud a)
        {
            RemoveByValue(setEdges, a);
            RemoveByValue(setCorners, a);

            for (int i = knownReinforcement.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(knownReinforcement[i], a))
                {
                    knownReinforcement.RemoveAt(i);
                }
            }
        }

        private List<R.Raud> get_unique()
        {
            List<R.Raud> unique = new List<R.Raud>();
            
            foreach (R.Raud current in knownReinforcement)
            {
                if (!unique.Contains(current))
                {
                    unique.Add(current);
                }
            }

            foreach (R.Raud_Array current_array in knownArrayReinforcement)
            {
                foreach (R.Raud current in current_array.array)
                {
                    if (!unique.Contains(current))
                    {
                        unique.Add(current);
                    }
                }
            }

            return unique;
        }

        private static void ReplaceByValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue someValue, TValue someNewValue)
        {
            List<TKey> itemsToReplace = new List<TKey>();

            foreach (var pair in dictionary)
            {
                if (ReferenceEquals(pair.Value, someValue)) itemsToReplace.Add(pair.Key);
            }

            foreach (TKey item in itemsToReplace)
            {
                dictionary[item] = someNewValue;
            }
        }

        private static void RemoveByValue<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue someValue)
        {
            List<TKey> itemsToRemove = new List<TKey>();

            foreach (var pair in dictionary)
            {
                if (ReferenceEquals(pair.Value, someValue)) itemsToRemove.Add(pair.Key);
            }

            foreach (TKey item in itemsToRemove)
            {
                dictionary.Remove(item);
            }
        }
    }
}