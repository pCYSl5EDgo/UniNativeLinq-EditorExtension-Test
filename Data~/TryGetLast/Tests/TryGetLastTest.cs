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

using System;
using NUnit.Framework;
using Unity.Collections;

namespace UniNativeLinq.Tests
{
    /// <summary>
    /// This class uses LinkedList[T] for most of the tests, as that doesn't implement IList[T] so won't
    /// go through any optimizations. There are then a couple of tests at the bottom for lists. It's
    /// possible this is overkill, particularly for the predicate tests where we actually test that
    /// it *isn't* optimized anyway... but it keeps the class consistent.
    /// </summary>
    [TestFixture]
    public class TryGetLastTest
    {
        [Test]
        public void NullSourceWithoutPredicate()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetLast(out _));
        }

        [Test]
        public void NullSourceWithPredicate()
        {
            int[] source = null;
            Func<int, bool> predicate = x => x > 3;
            Assert.Throws<ArgumentNullException>(() => source.TryGetLast(out _, predicate));
        }

        [Test]
        public void NullPredicate()
        {
            var source = new[] { 1, 3, 5 };
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetLast(out _, predicate));
        }

        [Test]
        public void EmptySequenceWithoutPredicate()
        {
            var source = Enumerable.Repeat(1, 0);
            Assert.IsFalse(source.TryGetLast(out _));
        }

        [Test]
        public void SingleElementSequenceWithoutPredicate()
        {
            var source = new[] { 5 };
            Assert.IsTrue(source.TryGetLast(out var x));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void MultipleElementSequenceWithoutPredicate()
        {
            var source = new[] { 5, 10 };
            Assert.IsTrue(source.TryGetLast(out var x));
            Assert.AreEqual(10, x);
        }

        [Test]
        public void EmptySequenceWithPredicate()
        {
            var source = new int[0];
            Assert.IsFalse(source.TryGetLast(out _, x => x > 3));
        }

        [Test]
        public void SingleElementSequenceWithMatchingPredicate()
        {
            var source = new[] { 5 };
            Assert.IsTrue(source.TryGetLast(out var y, x => x > 3));
            Assert.AreEqual(5, y);
        }

        [Test]
        public void SingleElementSequenceWithNonMatchingPredicate()
        {
            var source = new[] { 2 };
            Assert.IsFalse(source.TryGetLast(out _, x => x > 3));
        }

        [Test]
        public void MultipleElementSequenceWithNoPredicateMatches()
        {
            var source = new[] { 1, 2, 2, 1 };
            Assert.IsFalse(source.TryGetLast(out _, x => x > 3));
        }

        [Test]
        public void MultipleElementSequenceWithSinglePredicateMatch()
        {
            var source = new[] { 1, 2, 5, 2, 1 };
            Func<int, bool> predicate = x => x > 3;
            Assert.IsTrue(source.TryGetLast(out var y, predicate));
            Assert.AreEqual(5, y);
        }

        [Test]
        public void MultipleElementSequenceWithMultiplePredicateMatches()
        {
            var source = new[] { 1, 2, 5, 10, 2, 1 };
            Func<int, bool> predicate = x => x > 3;
            Assert.IsTrue(source.TryGetLast(out var y, predicate));
            Assert.AreEqual(10, y);
        }

        [Test]
        public void ListWithoutPredicateDoesntIterate()
        {
            using (var source = new NativeArray<int>(4, Allocator.Temp)
            {
                [0] = 1,
                [1] = 5,
                [2] = 10,
                [3] = 3
            })
            {
                Assert.IsTrue(source.TryGetLast(out var x));
                Assert.AreEqual(3, x);
            }
        }
    }
}