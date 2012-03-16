﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;

namespace System.Net.Http.Formatting
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> that can be used to compare header field
    /// based on their quality values. This applies to values used in accept-charset, accept-encoding, 
    /// accept-language and related header fields with similar syntax rules.
    /// </summary>
    internal class StringWithQualityHeaderValueComparer : IComparer<StringWithQualityHeaderValue>
    {
        private static readonly StringWithQualityHeaderValueComparer _qualityComparer =
            new StringWithQualityHeaderValueComparer();

        private StringWithQualityHeaderValueComparer()
        {
        }

        public static StringWithQualityHeaderValueComparer QualityComparer
        {
            get { return _qualityComparer; }
        }

        /// <summary>
        /// Compares two <see cref="StringWithQualityHeaderValue"/> based on their quality value (a.k.a their "q-value").
        /// Values with identical q-values are considered equal (i.e the result is 0) with the exception of wild-card
        /// values (i.e. a value of "*") which are considered less than non-wild-card values. This allows to sort
        /// a sequence of <see cref="StringWithQualityHeaderValue"/> following their q-values ending up with any
        /// wild-cards at the end.
        /// </summary>
        /// <param name="stringWithQuality1">The first value to compare.</param>
        /// <param name="stringWithQuality2">The second value to compare</param>
        /// <returns>The result of the comparison.</returns>
        public int Compare(StringWithQualityHeaderValue stringWithQuality1,
                           StringWithQualityHeaderValue stringWithQuality2)
        {
            Contract.Assert(stringWithQuality1 != null);
            Contract.Assert(stringWithQuality2 != null);

            double quality1 = stringWithQuality1.Quality.HasValue ? stringWithQuality1.Quality.Value : 1.0;
            double quality2 = stringWithQuality2.Quality.HasValue ? stringWithQuality2.Quality.Value : 1.0;

            double qualityDifference = quality1 - quality2;
            if (qualityDifference < 0)
            {
                return -1;
            }
            else if (qualityDifference > 0)
            {
                return 1;
            }

            if (!String.Equals(stringWithQuality1.Value, stringWithQuality2.Value, StringComparison.OrdinalIgnoreCase))
            {
                if (String.Equals(stringWithQuality1.Value, "*", StringComparison.OrdinalIgnoreCase))
                {
                    return -1;
                }
                else if (String.Equals(stringWithQuality2.Value, "*", StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
