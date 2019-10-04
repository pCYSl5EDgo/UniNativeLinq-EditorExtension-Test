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
    public class AllTest
    {
        [Test]
        public void NullSource()
        {
            int[] src = null;
            Assert.Throws<ArgumentNullException>(() => src.All(x => x > 10));
        }

        [Test]
        public void NullPredicate()
        {
            int[] src = { 1, 3, 5 };
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => src.All(predicate));
        }

        [Test]
        public void EmptySequenceReturnsTrue()
        {
            Assert.IsTrue(new int[0].All(x => x > 0));
        }

        [Test]
        public void PredicateMatchingNoElements()
        {
            int[] src = { 1, 5, 20, 30 };
            Assert.IsFalse(src.All(x => x < 0));
        }

        [Test]
        public void PredicateMatchingSomeElements()
        {
            int[] src = { 1, 5, 8, 9 };
            Assert.IsFalse(src.All(x => x > 3));
        }

        [Test]
        public void PredicateMatchingAllElements()
        {
            int[] src = { 1, 5, 8, 9 };
            Assert.IsTrue(src.All(x => x > 0));
        }

        [Test]
        public void SequenceIsNotEvaluatedAfterFirstNonMatch()
        {
            int[] src = { 2, 10, 0, 3 };
            var query = src.Select(x => 10 / x);
            // This will finish at the second element (x = 10, so 10/x = 1)
            // It won't evaluate 10/0, which would throw an exception
            Assert.IsFalse(query.All(y => y > 2));
        }
    }
}