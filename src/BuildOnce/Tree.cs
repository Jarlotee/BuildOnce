using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildOnce
{
    public class Tree<T> : HashSet<Tree<T>>
    {
        public T Key { get; set; }

        public string ToString(int level = 0)
        {
            var result = Key != null ? Key.ToString().PadLeft(Key.ToString().Length + level * 2) : string.Empty;

            if (Count > 0)
            {
                result += Environment.NewLine + this.Select(t => t.ToString(level + 1)).Aggregate((a, b) => a + Environment.NewLine + b);
            }

            return result;
        }
    }
}
