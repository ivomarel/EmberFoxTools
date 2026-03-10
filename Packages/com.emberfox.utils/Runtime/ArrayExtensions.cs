using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    /// <summary>
    /// Shuffles the list in-place using Fisher–Yates.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list, System.Random rng = null)
    {
        rng ??= new System.Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}