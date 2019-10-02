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

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class SelectTest
    {
        [Test]
        public void NullProjectionThrowsNullArgumentException()
        {
            int[] source = { 1, 3, 7, 9, 10 };
            Func<int, int> projection = null;
            Assert.Throws<ArgumentNullException>(() => source.Select(projection));
        }

        [Test]
        public void WithIndexNullSourceThrowsNullArgumentException()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Select((x, index) => x + index));
        }

        [Test]
        public void WithIndexNullPredicateThrowsNullArgumentException()
        {
            int[] source = { 1, 3, 7, 9, 10 };
            Func<int, long, int> projection = null;
            Assert.Throws<ArgumentNullException>(() => source.Select(projection));
        }

        [Test]
        public void SimpleProjection()
        {
            int[] source = { 1, 5, 2 };
            var result = source.Select(x => x * 2);
            result.AssertSequenceEqual(2, 10, 4);
        }

        [Test]
        public void SimpleProjectionWithQueryExpression()
        {
            int[] source = { 1, 5, 2 };
            var result = from x in source
                         select x * 2;
            result.AssertSequenceEqual(2, 10, 4);
        }

        [Test]
        public void EmptySource()
        {
            int[] source = new int[0];
            var result = source.Select(x => x * 2);
            result.AssertSequenceEqual();
        }

        [Test]
        public void ExecutionIsDeferred()
        {
            ThrowingEnumerable.AssertDeferred(src => 
                new SelectEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, int, DelegateFuncToStructOperatorAction<int, int>>(src, new DelegateFuncToStructOperatorAction<int, int>(x => x * 2)));
        }

        [Test]
        public void WithIndexSimpleProjection()
        {
            int[] source = { 1, 5, 2 };
            var result = source.Select((x, index) => x + index * 10);
            result.AssertSequenceEqual(1, 15, 22);
        }

        [Test]
        public void WithIndexEmptySource()
        {
            int[] source = new int[0];
            var result = source.Select((x, index) => x + index);
            result.AssertSequenceEqual();
        }

        [Test]
        public void WithIndexExecutionIsDeferred()
        {
            ThrowingEnumerable.AssertDeferred(src =>
                new SelectIndexEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, long, DelegateFuncToSelectIndexStructOperator<int, long>>(src, new DelegateFuncToSelectIndexStructOperator<int, long>((x, index) => x + index)));
        }

        [Test]
        public void SideEffectsInProjection()
        {
            int[] source = new int[3]; // Actual values won't be relevant
            int count = 0;
            var query = source.Select(x => count++);
            query.AssertSequenceEqual(0, 1, 2);
            query.AssertSequenceEqual(3, 4, 5);
            count = 10;
            query.AssertSequenceEqual(10, 11, 12);
        }
    }
}