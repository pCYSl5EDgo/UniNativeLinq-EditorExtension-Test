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
    public class CountTest
    {
        [Test]
        public void NonCollectionCount()
        {
            Assert.AreEqual(5, Enumerable.Range(2, 5).Count());
        }

        [Test]
        public void GenericOnlyCollectionCount()
        {
            Assert.AreEqual(5, Enumerable.Range(2, 5).Count());
        }

        [Test]
        public void SemiGenericCollectionCount()
        {
            Assert.AreEqual(5, Enumerable.Range(2, 5).Count());
        }

        [Test]
        public void PredicatedNullPredicateThrowsArgumentNullException()
        {
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>(() => new int[0].Where(predicate).Count());
        }

        [Test]
        public void PredicatedCount()
        {
            // Counts even numbers within 2, 3, 4, 5, 6
            Assert.AreEqual(3, Enumerable.Range(2, 5).Where(x => x % 2 == 0).Count());
        }

        [Test]
        [Ignore("Takes an enormous amount of time!")]
        public void Overflow()
        {
            var largeSequence = Enumerable.Range(0, int.MaxValue)
                                          .Concat(Enumerable.Range(0, 1));
            Assert.Throws<OverflowException>(() => largeSequence.Count());
        }

        [Test]
        [Ignore("Takes an enormous amount of time!")]
        public void OverflowWithPredicate()
        {
            var largeSequence = Enumerable.Range(0, int.MaxValue)
                                          .Concat(Enumerable.Range(0, 1));
            Assert.Throws<OverflowException>(() => largeSequence.Where(x => x >= 0).Count());
        }
    }
}