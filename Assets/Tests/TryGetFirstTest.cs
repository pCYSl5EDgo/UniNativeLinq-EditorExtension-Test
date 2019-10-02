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

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class TryGetFirstTest
    {
        [Test]
        public void NullSourceWithoutPredicate()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetFirst(out _));
        }

        [Test]
        public void NullSourceWithPredicate()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetFirst(out _, x => x > 3));
        }

        [Test]
        public void NullSourceWithRefPredicate()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                bool Predicate(ref int x) => x > 3;

                source.TryGetFirst(out _, Predicate);
            });
        }

        [Test]
        public void NullPredicate()
        {
            int[] source = { 1, 3, 5 };
            Func<int, bool> Predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetFirst(out _, Predicate));
        }

        [Test]
        public void NullRefPredicate()
        {
            int[] source = { 1, 3, 5 };
            RefFunc<int, bool> Predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetFirst(out _, Predicate));
        }

        [Test]
        public void EmptySequenceWithoutPredicate()
        {
            int[] source = { };
            Assert.IsFalse(source.TryGetFirst(out _));
        }

        [Test]
        public void SingleElementSequenceWithoutPredicate()
        {
            int[] source = { 5 };
            Assert.IsTrue(source.TryGetFirst(out var x));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void MultipleElementSequenceWithoutPredicate()
        {
            int[] source = { 5, 10 };
            Assert.IsTrue(source.TryGetFirst(out var x));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void EmptySequenceWithPredicate()
        {
            int[] source = { };
            bool Predicate(int value) => value > 3;
            Assert.IsFalse(source.TryGetFirst(out _, Predicate));
        }

        [Test]
        public void SingleElementSequenceWithMatchingPredicate()
        {
            int[] source = { 5 };
            bool Predicate(int value) => value > 3;
            Assert.IsTrue(source.TryGetFirst(out var x, Predicate));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void SingleElementSequenceWithNonMatchingPredicate()
        {
            int[] source = { 2 };
            bool Predicate(int value) => value > 3;
            Assert.IsFalse(source.TryGetFirst(out _, Predicate));
        }

        [Test]
        public void MultipleElementSequenceWithNoPredicateMatches()
        {
            int[] source = { 1, 2, 2, 1 };
            bool Predicate(int value) => value > 3;
            Assert.IsFalse(source.TryGetFirst(out _, Predicate));
        }

        [Test]
        public void MultipleElementSequenceWithSinglePredicateMatch()
        {
            int[] source = { 1, 2, 5, 2, 1 };
            bool Predicate(int value) => value > 3;
            Assert.IsTrue(source.TryGetFirst(out var x, Predicate));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void MultipleElementSequenceWithMultiplePredicateMatches()
        {
            int[] source = { 1, 2, 5, 10, 2, 1 };
            bool Predicate(int value) => value > 3;
            Assert.IsTrue(source.TryGetFirst(out var x, Predicate));
            Assert.AreEqual(5, x);
        }

        [Test]
        public void EarlyOutAfterFirstElementWithoutPredicate()
        {
            int[] source = { 15, 1, 0, 3 };
            var query = source.Select(x => 10 / x);
            Assert.IsTrue(query.TryGetFirst(out var y));
            // We finish before getting as far as dividing by 0
            Assert.AreEqual(0, y);
        }

        [Test]
        public void EarlyOutAfterFirstElementWithPredicate()
        {
            bool Predicate(int z)
            {
                return z > 5;
            }

            int[] source = { 15, 1, 0, 3 };
            var query = source.Select(x => 10 / x);
            // We finish before getting as far as dividing by 0
            Assert.IsTrue(query.TryGetFirst(out var y, Predicate));
            Assert.AreEqual(10, y);
        }
    }
}