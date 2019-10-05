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
using UniNativeLinq.TestSupport;
using Unity.Collections;
using UnityEngine;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class GroupByTest
    {
        [Test]
        public void ExecutionIsPartiallyDeferred()
        {
            // No exception yet...
            var _ = new GroupByEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, int, AsIs<int>, int, AsIs<int>, DefaultEqualityComparer<int>>(new ThrowingEnumerable(), Allocator.Temp, GroupByDisposeOptions.Recursive);
            // Note that for LINQ to Objects, calling GetEnumerator() starts iterating
            // over the input sequence, so we're not testing that...
        }

        [Test]
        public void SequenceIsReadFullyBeforeFirstResultReturned()
        {
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            // Final projection will throw
            var query = numbers.Select(x => 10 / x);

            var groups = query.GroupBy(x => x);
            // Either GetEnumerator or MoveNext will throw. See blog post for details.
            Assert.Throws<DivideByZeroException>(() =>
            {
                using (var iterator = groups.GetEnumerator())
                {
                    iterator.MoveNext();
                }
            });
        }

        [Test]
        public void SimpleGroupBy()
        {
            float[] source = { 3.5f, 5.2f, 3.9f, 5.9f, 4.5f };
            var groups = source.GroupBy(x => (int)x);

            var list = groups.ToArray();
            Assert.AreEqual(3, list.Length);

            list[0].AssertSequenceEqual(3.5f, 3.9f);
            Assert.AreEqual(3, list[0].Key);

            list[1].AssertSequenceEqual(5.2f, 5.9f);
            Assert.AreEqual(5, list[1].Key);

            list[2].AssertSequenceEqual(4.5f);
            Assert.AreEqual(4, list[2].Key);
        }

        [Test]
        public void GroupByWithElementProjection()
        {
            double[] source = { 3.5, 5.2, 3.9, 5.9, 4.5 };
            var groups = source.GroupBy(x => (int)x, x =>
            {
                int v = (int)(x * 10) - 10 * ((int)x);
                Debug.Log(v);
                return v;
            });

            var list = groups.ToArray();
            Assert.AreEqual(3, list.Length);

            list[0].AssertSequenceEqual(5, 9);
            Assert.AreEqual(3, list[0].Key);

            list[1].AssertSequenceEqual(2, 9);
            Assert.AreEqual(5, list[1].Key);

            list[2].AssertSequenceEqual(5);
            Assert.AreEqual(4, list[2].Key);
        }
    }
}