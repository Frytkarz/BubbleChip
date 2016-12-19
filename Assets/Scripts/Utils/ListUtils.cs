using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleChip
{
    public static class ListUtils
    {
        /// <summary>
        /// Wyciąga randowmową pozycję z listy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Random<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
