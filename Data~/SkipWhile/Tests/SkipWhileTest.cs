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
    [TestFixture]
    public class SkipWhileTest
    {
        readonly struct PredicateGreaterThan : IRefFunc<int, bool>
        {
            private readonly int compare;

            public PredicateGreaterThan(int compare)
            {
                this.compare = compare;
            }

            public bool Calc(ref int arg0)
            {
                return arg0 > compare;
            }
        }

        [Test]
        public void ExecutionIsDeferred()
        {
            new SkipWhileEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, PredicateGreaterThan>(new ThrowingEnumerable(), new PredicateGreaterThan(3));
        }

        [Test]
        public void NullSourceNoIndex()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.SkipWhile(new PredicateGreaterThan(10)));
        }

        [Test]
        public void NullPredicateNoIndex()
        {
            int[] source = { 1, 2 };
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => source.SkipWhile(predicate));
        }

        [Test]
        public void PredicateFailingFirstElement()
        {
            int[] source = { 5, 4, 4, 6, 5, 5 };
            source.SkipWhile(new PredicateGreaterThan(6)).AssertSequenceEqual(5, 4, 4, 6, 5, 5);
        }

        [Test]
        public void PredicateMatchingSomeElements()
        {
            int[] source = { 5, 4, 4, 6, 5, 5 };
            source.SkipWhile(x => x < 6).AssertSequenceEqual(6, 5, 5);
        }

        [Test]
        public void PredicateMatchingAllElements()
        {
            int[] source = { 5, 4, 4, 6, 5, 5 };
            source.SkipWhile(x => x < 100).AssertSequenceEqual();
        }
    }
}