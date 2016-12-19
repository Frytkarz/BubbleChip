using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleChip
{
    public class Vector2Int
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Vector2Int()
        {
            x = 0;
            y = 0;
        }

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
