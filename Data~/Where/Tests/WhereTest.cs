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

using UniNativeLinq.TestSupport;
using System;
using NUnit.Framework;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class WhereTest
    {
        readonly struct PredicateGreaterThan : IRefFunc<int, bool>, IWhereIndex<int>
        {
            private readonly int value;

            public PredicateGreaterThan(int value)
            {
                this.value = value;
            }

            public bool Calc(ref int arg0)
            {
                return arg0 > value;
            }

            public bool Calc(ref int value, long index)
            {
                return value > this.value;
            }
        }

        [Test]
        public void NullSourceThrowsNullArgumentException()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Where(x => x > 5));
        }

        [Test]
        public void NullPredicateThrowsNullArgumentException()
        {
            int[] source = { 1, 3, 7, 9, 10 };
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.Where(predicate));
        }

        [Test]
        public void WithIndexNullSourceThrowsNullArgumentException()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Where((x, index) => x > 5));
        }

        [Test]
        public void WithIndexNullPredicateThrowsNullArgumentException()
        {
            int[] source = { 1, 3, 7, 9, 10 };
            Func<int, long, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.Where(predicate));
        }

        [Test]
        public void SimpleFiltering()
        {
            int[] source = { 1, 3, 4, 2, 8, 1 };
            var result = source.Where(x => x < 4);
            result.AssertSequenceEqual(1, 3, 2, 1);
        }

        [Test]
        public void SimpleFilteringWithQueryExpression()
        {
            int[] source = { 1, 3, 4, 2, 8, 1 };
            var result = from x in source
                         where x < 4
                         select x;
            result.AssertSequenceEqual(1, 3, 2, 1);
        }

        [Test]
        public void EmptySource()
        {
            int[] source = new int[0];
            var result = source.Where(x => x < 4);
            result.AssertSequenceEqual();
        }

        [Test]
        public void ExecutionIsDeferred()
        {
            ThrowingEnumerable.AssertDeferred(src => new WhereEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, PredicateGreaterThan>(new ThrowingEnumerable(), new PredicateGreaterThan(0)));
        }

        [Test]
        public void WithIndexSimpleFiltering()
        {
            int[] source = { 1, 3, 4, 2, 8, 1 };
            var result = source.Where((x, index) => x < index);
            result.AssertSequenceEqual(2, 1);
        }

        [Test]
        public void WithIndexEmptySource()
        {
            int[] source = new int[0];
            var result = source.Where((x, index) => x < 4);
            result.AssertSequenceEqual();
        }

        [Test]
        public void WithIndexExecutionIsDeferred()
        {
            ThrowingEnumerable.AssertDeferred(src => new WhereIndexEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, PredicateGreaterThan>(new ThrowingEnumerable(), new PredicateGreaterThan(0)));
        }
    }
}