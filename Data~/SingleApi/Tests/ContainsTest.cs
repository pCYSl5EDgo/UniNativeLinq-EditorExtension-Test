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
    public class ContainsTest
    {
        [Test]
        public void NullSourceNoComparer()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Contains(44));
        }

        [Test]
        public void NullSourceWithComparer()
        {
            bool Target(ref long arg0, ref long arg1)
            {
                return true;
            }
            long[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Contains(57, Target));
        }

        [Test]
        public void NoMatchNoComparer()
        {
            // Default equality comparer is ordinal
            var source = new[] { 14, 29, -432 };
            Assert.IsFalse(source.Contains(-4));
        }

        [Test]
        public void MatchNoComparer()
        {
            // Default equality comparer is ordinal
            var source = new[] { 14, 29, -432 };
            // Clone the string to verify it's not just using reference identity
            Assert.IsTrue(source.Contains(-432));
        }

        [Test]
        public void NoMatchNullComparer()
        {
            // Default equality comparer is ordinal
            var source = new[] { 14, 29, -432 };
            Assert.Throws<ArgumentNullException>(() => source.Contains(12, null));
        }

        [Test]
        public void MatchNullComparer()
        {
            // Default equality comparer is ordinal
            var source = new[] { 14, 29, -432 };
            Assert.Throws<ArgumentNullException>(() => source.Contains(29, null));
        }

        [Test]
        public void ImmediateReturnWhenMatchIsFound()
        {
            int[] source = new[] { 10, 1, 5, 0 };
            var query = source.Select(x => 10 / x);
            // If we continued past 2, we'd see a division by zero exception
            Assert.IsTrue(query.Contains(2));
        }
    }
}