#region Copyright and license information
// Copyright 2010-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region Copyright and license information of UniNativeLinq
//Copyright(C) 2019 pCYSl5EDgo<https://github.com/pCYSl5EDgo>

//This file is part of UniNativeLinq-Test.

//UniNativeLinq is a dual license open-source Unity Editor extension software.

//You can redistribute it and/or modify it under the terms of the GNU General
//Public License as published by the Free Software Foundation, either version 3
//of the License, or (at your option) any later version.

//UniNativeLinq is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with UniNativeLinq. If not, see<http://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace UniNativeLinq.TestSupport
{
    /// <summary>
    /// This is originally from MoreLINQ:
    /// http://morelinq.googlecode.com
    /// 
    /// It may end up being expanded a bit though :)
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        /// Make testing even easier - a params array makes for readable tests :)
        /// The sequence is evaluated exactly once.
        /// </summary>
        public static void AssertSequenceEqual<T>(this IEnumerable<T> actual, params T[] expected)
        {
            // Working with a copy means we can look over it more than once.
            // We're safe to do that with the array anyway.
            List<T> copy = new List<T>(actual);
            Assert.AreEqual(expected.Length, copy.Count, "Expected counts to be equal");

            for (int i = 0; i < copy.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(expected[i], copy[i]))
                {
                    Assert.Fail("Expected sequences differ at index " + i + ": expected " + expected[i]
                                + "; was " + copy[i]);
                }
            }
        }
    }

    /// <summary>
    /// Simple class to let us convert an Int32 to a string using the invariant culture,
    /// without explicitly having to use CultureInfo.InvariantCulture everywhere.
    /// </summary>
    public static class Int32Extensions
    {
        public static string ToInvariantString(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}