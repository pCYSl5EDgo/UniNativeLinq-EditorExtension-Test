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
using UniNativeLinq.TestSupport;
using NUnit.Framework;
using Unity.Collections;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class JoinTest
    {
        [Test]
        public void ExecutionIsDeferred()
        {
            var outer = new ThrowingEnumerable();
            var inner = new ThrowingEnumerable();
            // No exception
            var join = new JoinEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, int, AsIs<int>, AsIs<int>, DefaultEqualityComparer<int>, int, AddInt>(outer, inner, default, default, default, default, Allocator.Temp);
        }

        struct AddInt : IRefFunc<int, int, int>
        {
            public int Calc(ref int arg0, ref int arg1) => arg0 + arg1;
        }

        [Test]
        public void OuterSequenceIsStreamed()
        {
            var outer = new[] { 10, 0, 2 }.Select(x => 10 / x);
            var inner = new[] { 1, 2, 3 };
            var query = outer.Join(inner, x => x, y => y, (x, y) => x == y, (x, y) => x + y);

            using (var iterator = query.GetEnumerator())
            {
                // First element is fine
                Assert.IsTrue(iterator.MoveNext());
                Assert.AreEqual(2, iterator.Current);

                // Attempting to get to the second element causes division by 0
                Assert.Throws<DivideByZeroException>(() => iterator.MoveNext());
            }
        }

        [Test]
        public void InnerSequenceIsBuffered()
        {
            var outer = new[] { 1, 2, 3 };
            var inner = new[] { 10, 0, 2 }.Select(x => 10 / x);
            var query = outer.Join(inner, x => x, y => y, (x, y) => x == y, (x, y) => x + y);

            Assert.Throws<DivideByZeroException>(() =>
            {
                using (var iterator = query.GetEnumerator())
                {
                    // Even though we could sensibly see the first element before anything
                    // is returned, that doesn't happen: the inner sequence is read completely
                    // before we start reading the outer sequence
                }
            });
        }

        [Test]
        public void SimpleJoin()
        {
            // We're going to join on the first character in the outer sequence item
            // being equal to the second character in the inner sequence item
            int[] outer = { 0x111, 0x233, 0x3ad };
            int[] inner = { 0x262, 0x143, 0x900, 0x200 };

            var query = outer.Join(inner,
                                   outerElement => outerElement >> 8,
                                   innerElement => innerElement >> 8,
                                   (int x, int y) => x == y,
                                   (outerElement, innerElement) => outerElement + innerElement);

            // Note: no matches for 0x3ad
            query.AssertSequenceEqual(0x254, 0x495, 0x433);
        }

        //[Test]
        //public void CustomComparer()
        //{
        //    // We're going to match the start of the outer sequence item
        //    // with the end of the inner sequence item, in a case-insensitive manner
        //    int[] outer = { "ABCxxx", "abcyyy", "defzzz", "ghizzz" };
        //    int[] inner = { "000abc", "111gHi", "222333" };

        //    var query = outer.Join(inner,
        //                           outerElement => outerElement.Subint(0, 3),
        //                           innerElement => innerElement.Subint(3),
        //                           (outerElement, innerElement) => outerElement + ":" + innerElement,
        //                           StringComparer.OrdinalIgnoreCase);
        //    query.AssertSequenceEqual("ABCxxx:000abc", "abcyyy:000abc", "ghizzz:111gHi");
        //}

        //[Test]
        //public void DifferentSourceTypes()
        //{
        //    int[] outer = { 5, 3, 7 };
        //    int[] inner = { "bee", "giraffe", "tiger", "badger", "ox", "cat", "dog" };

        //    var query = outer.Join(inner,
        //                           outerElement => outerElement,
        //                           innerElement => innerElement.Length,
        //                           (outerElement, innerElement) => outerElement + ":" + innerElement);
        //    query.AssertSequenceEqual("5:tiger", "3:bee", "3:cat", "3:dog", "7:giraffe");
        //}

        //// Note that LINQ to Objects ignores null keys for Join and GroupJoin
        //[Test]
        //public void NullKeys()
        //{
        //    int[] outer = { 0x111, "null", "nothing", 0x233 };
        //    int[] inner = { "nuff", 0x233 };
        //    var query = outer.Join(inner,
        //                           outerElement => outerElement.StartsWith("n") ? null : outerElement,
        //                           innerElement => innerElement.StartsWith("n") ? null : innerElement,
        //                           (outerElement, innerElement) => outerElement + ":" + innerElement);

        //    query.AssertSequenceEqual("second:second");
        //}
    }
}