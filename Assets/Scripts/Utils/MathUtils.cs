using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleChip
{
    public static class MathUtils
    {
        /// <summary>
        /// Silnia
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Factorial(this int value)
        {
                if (value <= 1)
                    return 1;
                else
                    return value * (value - 1).Factorial();
        }

        public static int SumTo(this int value)
        {
            if (value <= 1)
                return 1;
            else
                return value + (value - 1).SumTo();
        }
    }
}
