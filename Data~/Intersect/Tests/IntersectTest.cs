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

#region Copyright and license information
//Copyright(C) 2019 pCYSl5EDgo
//
//This file is part of UniNativeLinq Test Repository 1.
//
//You can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
//
//UniNativeLinq Test Repository 1 is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along with UniNativeLinq Test Repository 1. If not, seehttp:www.gnu.org/licenses/.
#endregion

using UniNativeLinq.TestSupport;
using System;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class IntersectTest
    {
        [Test]
        public void NullFirstWithoutComparer()
        {
            int[] first = null;
            int[] second = { };
            Assert.Throws<ArgumentNullException>(() => first.Intersect(second));
        }

        [Test]
        public void NullSecondWithoutComparer()
        {
            int[] first = { };
            int[] second = null;
            Assert.Throws<ArgumentNullException>(() => first.Intersect(second));
        }

        [Test]
        public void NullFirstWithComparer()
        {
            int[] first = null;
            int[] second = { };
            Assert.Throws<ArgumentNullException>(() => first.Intersect(second, (ref int a, ref int b) => a == b));
        }

        [Test]
        public void NullSecondWithComparer()
        {
            int[] first = { };
            int[] second = null;
            Assert.Throws<ArgumentNullException>(() => first.Intersect(second, (ref int a, ref int b) => a == b));
        }

        [Test]
        public void NoComparerSpecified()
        {
            int[] first = { -10, 10, 5, 21, 5 };
            int[] second = { 5, 10, 90, 10 };
            first.Intersect(second).AssertSequenceEqual(10, 5);
        }

        [Test]
        public void NullComparerSpecified()
        {
            int[] first = { -10, 10, 5, 21, 5 };
            int[] second = { 5, 10, 90, 10 };
            first.Intersect(second).AssertSequenceEqual(10, 5);
        }

        [Test]
        public void CaseInsensitiveComparerSpecified()
        {
            int[] first = { -10, 10, 5, 21, 5 };
            int[] second = { 5, 10, 90, 10 };

            bool Comparer(ref int a, ref int b) => a == b || a == -b;
            var x = first.Intersect(second, Comparer);
            x.AssertSequenceEqual(-10, 5);
        }

        [Test]
        public void NoSequencesUsedBeforeIteration()
        {
            var first = new ThrowingEnumerable();
            var second = new ThrowingEnumerable();
            // No exceptions!
            var query = new SetOperationEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, IntersectOperation<ThrowingEnumerable, ThrowingEnumerable.Enumerator, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, DefaultEqualityComparer<int>>>(first, second, default, Allocator.Persistent);
            Assert.Throws<InvalidOperationException>(() => query.GetEnumerator());
        }

        [Test]
        public void SecondSequenceReadFullyOnFirstResultIteration()
        {
            int[] first = { 1 };
            var secondQuery = new[] { 10, 2, 0 }.Select(x => 10 / x);

            var query = first.Intersect(secondQuery);
            Assert.Throws<DivideByZeroException>(() => query.GetEnumerator());
        }

        [Test]
        public void FirstSequenceOnlyReadAsResultsAreRead()
        {
            var firstQuery = new[] { 10, 2, 0, 2 }.Select(x => 10 / x);
            int[] second = { 1 };

            var query = firstQuery.Intersect(second);
            Assert.Throws<DivideByZeroException>(() => query.GetEnumerator());
        }
    }
}