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
    class AnyTest
    {
        [Test]
        public void NullSourceWithoutPredicate()
        {
            int[] src = null;
            Assert.Throws<ArgumentNullException>(() => src.AsRefEnumerable().Any());
        }

        [Test]
        public void NullSourceWithPredicate()
        {
            int[] src = null;
            Assert.Throws<ArgumentNullException>(() => src.Any(x => x > 10));
        }

        [Test]
        public void NullPredicate()
        {
            int[] src = { 1, 3, 5 };
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => src.Any(predicate));
        }

        [Test]
        public void EmptySequenceWithoutPredicate()
        {
            Assert.IsFalse(new int[0].AsRefEnumerable().Any());
        }

        [Test]
        public void EmptySequenceWithPredicate()
        {
            Assert.IsFalse(new int[0].Any(x => x > 10));
        }

        [Test]
        public void NonEmptySequenceWithoutPredicate()
        {
            Assert.IsTrue(new int[1].AsRefEnumerable().Any());
        }

        [Test]
        public void NonEmptySequenceWithPredicateMatchingElement()
        {
            int[] src = { 1, 5, 20, 30 };
            Assert.IsTrue(src.Any(x => x > 10));
        }

        [Test]
        public void NonEmptySequenceWithPredicateNotMatchingElement()
        {
            int[] src = { 1, 5, 8, 9 };
            Assert.IsFalse(src.Any(x => x > 10));
        }

        [Test]
        public void SequenceIsNotEvaluatedAfterFirstMatch()
        {
            int[] src = { 10, 2, 0, 3 };
            var query = src.Select(x => 10 / x);
            // This will finish at the second element (x = 2, so 10/x = 5)
            // It won't evaluate 10/0, which would throw an exception
            Assert.IsTrue(query.Any(y => y > 2));
        }
    }
}