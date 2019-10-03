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
    public class SkipLastTest
    {
        [Test]
        public void ExecutionIsDeferred()
        {
            new SkipLastEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int>(new ThrowingEnumerable(), 10);
        }

        [Test]
        public void NullSource()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.SkipLast(10));
        }

        [Test]
        public void NegativeCount()
        {
            var skipLastEnumerable = Enumerable.Range(0, 5).SkipLast(-5);
            skipLastEnumerable.AssertSequenceEqual(0, 1, 2, 3, 4);
        }

        [Test]
        public void ZeroCount()
        {
            Enumerable.Range(0, 5).SkipLast(0).AssertSequenceEqual(0, 1, 2, 3, 4);
        }

        [Test]
        public void NegativeCountWithArray()
        {
            new int[] { 0, 1, 2, 3, 4 }.SkipLast(-5).AssertSequenceEqual(0, 1, 2, 3, 4);
        }

        [Test]
        public void ZeroCountWithArray()
        {
            new int[] { 0, 1, 2, 3, 4 }.SkipLast(0).AssertSequenceEqual(0, 1, 2, 3, 4);
        }

        [Test]
        public void CountShorterThanSource()
        {
            Enumerable.Range(0, 5).SkipLast(3).AssertSequenceEqual(0, 1);
        }

        [Test]
        public void CountEqualToSourceLength()
        {
            var rangeRepeatEnumerable = Enumerable.Range(0, 5);
            Assert.AreEqual(5L, rangeRepeatEnumerable.LongCount());
            var skipEnumerable = rangeRepeatEnumerable.SkipLast(5);
            Assert.AreEqual(0L, skipEnumerable.LongCount());
            skipEnumerable.AssertSequenceEqual();
        }

        [Test]
        public void CountGreaterThanSourceLength()
        {
            Enumerable.Range(0, 5).SkipLast(100).AssertSequenceEqual();
        }
    }
}