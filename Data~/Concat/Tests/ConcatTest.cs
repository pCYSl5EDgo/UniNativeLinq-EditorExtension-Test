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

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class ConcatTest
    {
        [Test]
        public void SimpleConcatenation()
        {
            int[] first = { 0, 4 };
            int[] second = { 8, 20 };
            first.Concat(second).AssertSequenceEqual(0, 4, 8, 20);
        }

        [Test]
        public void NullFirstThrowsNullArgumentException()
        {
            int[] first = null;
            int[] second = { -100 };
            Assert.Throws<ArgumentNullException>(() => first.Concat(second));
        }

        [Test]
        public void NullSecondThrowsNullArgumentException()
        {
            int[] first = { -100 };
            int[] second = null;
            Assert.Throws<ArgumentNullException>(() => first.Concat(second));
        }

        [Test]
        public void FirstSequenceIsntAccessedBeforeFirstUse()
        {
            var first = new ThrowingEnumerable();
            int[] second = { 5 };
            // No exception yet...
            var query = new ConcatEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, ArrayEnumerable<int>, ArrayEnumerable<int>.Enumerator, int>(first, second.AsRefEnumerable());
            // Still no exception...
            using (var iterator = query.GetEnumerator())
            {
                // Now it will go bang
                Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
            }
        }

        [Test]
        public void SecondSequenceIsntAccessedBeforeFirstUse()
        {
            int[] first = { 5 };
            var second = new ThrowingEnumerable();
            // No exception yet...
            var query = new ConcatEnumerable<ArrayEnumerable<int>, ArrayEnumerable<int>.Enumerator, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int>(first.AsRefEnumerable(), second);
            // Still no exception...
            using (var iterator = query.GetEnumerator())
            {
                // First element is fine...
                Assert.IsTrue(iterator.MoveNext());
                Assert.AreEqual(5, iterator.Current);
                // Now it will go bang, as we move into the second sequence
                Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
            }
        }
    }
}