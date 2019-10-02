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

namespace UniNativeLinq.Tests
{
#if !LINQBRIDGE && !DOTNET35_ONLY
    [TestFixture]
    public class AdjustedZipTest
    {
        [Test]
        public void NullFirst()
        {
            int[] first = null;
            int[] second = { };
            Func<int, int, int> resultSelector = (x, y) => x * (y + 1);
            Assert.Throws<ArgumentNullException>(() => first.AdjustedZip(second, resultSelector));
        }

        [Test]
        public void NullSecond()
        {
            int[] first = { };
            int[] second = null;
            Func<int, int, int> resultSelector = (x, y) => x * (y + 1);
            Assert.Throws<ArgumentNullException>(() => first.AdjustedZip(second, resultSelector));
        }

        [Test]
        public void NullResultSelector()
        {
            int[] first = { };
            int[] second = { };
            Func<int, int, int> resultSelector = null;
            Assert.Throws<ArgumentNullException>(() => first.AdjustedZip(second, resultSelector));
        }

        readonly struct AddOperator : IRefAction<int, int, int>
        {
            public void Execute(ref int arg0, ref int arg1, ref int arg2)
            {
                arg2 = arg0 + arg1;
            }
        }

        [Test]
        public void ExecutionIsDeferred()
        {
            var first = new ThrowingEnumerable();
            var second = new ThrowingEnumerable();
            new AdjustedZipEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, int, AddOperator>(first, second);
        }

        [Test]
        public void ShortFirst()
        {
            int[] first = { 0, 1, 2 };
            var second = Enumerable.Range(5, 10);
            Func<int, int, int> resultSelector = (x, y) => x * (y + 1);
            var query = first.AdjustedZip(second, resultSelector);
            query.AssertSequenceEqual(0 * 6, 1 * 7, 2 * 8);
        }

        [Test]
        public void ShortSecond()
        {
            int[] first = { 0, 1, 2, 3, 4 };
            var second = Enumerable.Range(5, 3);
            Func<int, int, int> resultSelector = (x, y) => x * (y + 1);
            var query = first.AdjustedZip(second, resultSelector);
            query.AssertSequenceEqual(0, 7, 16);
        }

        [Test]
        public void EqualLengthSequences()
        {
            int[] first = { 0, 1, 2 };
            var second = Enumerable.Range(5, 3);
            Func<int, int, int> resultSelector = (x, y) => x * (y + 1);
            var query = first.AdjustedZip(second, resultSelector);
            query.AssertSequenceEqual(0, 7, 16);
        }

        [Test]
        public void AdjacentElements()
        {
            int[] elements = { 0, 1, 2, 3, 4 };
            var query = elements.AdjustedZip(elements.Skip(1), (x, y) => x + y);
            query.AssertSequenceEqual(1, 3, 5, 7);
        }
    }
#endif
}