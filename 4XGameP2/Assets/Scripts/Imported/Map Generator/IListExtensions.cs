// Copyright (c) 2022 Nuno Fachada
// Distributed under the MIT License (See accompanying file LICENSE_CODE or copy
// at http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;

namespace ImportedGenerator
{
    public static class IListExtensions
    {
        private static readonly Random random;

        static IListExtensions()
        {
            random = new Random();
        }

        // Fisher–Yates shuffling.
        // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                T aux;
                int j = random.Next(i + 1);
                aux = list[j];
                list[j] = list[i];
                list[i] = aux;
            }
        }
    }
}