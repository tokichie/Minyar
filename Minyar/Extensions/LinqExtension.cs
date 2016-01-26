using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Extensions {
    public static class LinqExtension {
        public static double Median(this IEnumerable<double> source) {
            if (source.Count() == 0) {
                throw new InvalidOperationException("Cannot compute median for an empty set.");
            }

            var sortedList = from number in source
                             orderby number
                             select number;

            int itemIndex = (int)sortedList.Count() / 2;

            if (sortedList.Count() % 2 == 0) {
                // Even number of items.
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            } else {
                // Odd number of items.
                return sortedList.ElementAt(itemIndex);
            }
        }

        public static int Median(this IEnumerable<int> source) {
            return (int)(from num in source select (double) num).Median();
        }

        public static double Median<T>(this IEnumerable<T> numbers, Func<T, double> selector) {
            return (from num in numbers select selector(num)).Median();
        }

        public static int Median<T>(this IEnumerable<T> numbers, Func<T, int> selector) {
            return (from num in numbers select selector(num)).Median();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng) {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng) {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++) {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
