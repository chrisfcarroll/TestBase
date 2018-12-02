﻿using System;
using System.Collections.Generic;

namespace TestBase.HttpClient.Fake
{
    static class ByteArraySearch {

        static readonly int [] Empty = new int [0];

        public static bool HasSubArray (this byte [] self, byte [] candidate)
        {
            if (IsEmptyLocate(self, candidate)) return false;

            var list = new List<int> ();

            for (int i = 0; i < self.Length; i++) {
                if (!IsMatch (self, i, candidate))
                    continue;

                return true;
            }

            return false;
        }

        static bool IsMatch (byte [] array, int position, byte [] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array [position + i] != candidate [i])
                    return false;

            return true;
        }

        static bool IsEmptyLocate (byte [] array, byte [] candidate)
        {
            return array            == null
                || candidate        == null
                || array.Length     == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }
}