using System;
using System.Collections.Generic;
using System.Linq;

namespace com.armatur.common.util
{
    public static class MathUtil
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public static int RandomRange(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static int Random(int max)
        {
            return _random.Next(max);
        }


        public static T selectByProbability<T>(this IEnumerable<T> data) where T : IWeighted
        {
            float max = data.Select(i => i.Weight).Sum();
            float rand = _random.NextFloat(0, max);

            T targetValue = default(T);
            float p = 0;

            bool found = false;
            foreach (var item in data)
            {

                if (rand >= p && rand < (p + item.Weight))
                {
                    // success
                    targetValue = (T)item;
                    found = true;
                    break;
                }

                p = p + item.Weight;
            }

            if (!found)
                throw new Exception("failed to select by probability");

            return targetValue;
        }


        public static T selectByProbability<T>(Dictionary<T, int> data)
        {
            int max = data.Select(i => i.Value).Sum();
            int rand = _random.Next(0, max);

            T targetValue = default(T);
            int p = 0;

            bool found = false;
            foreach (var item in data)
            {

                if (rand >= p && rand < (p + item.Value))
                {
                    // success
                    targetValue = (T)item.Key;
                    found = true;
                    break;
                }

                p = p + item.Value;
            }

            if (!found)
                throw new Exception("failed to select by probability");

            return targetValue;
        }
    }
}