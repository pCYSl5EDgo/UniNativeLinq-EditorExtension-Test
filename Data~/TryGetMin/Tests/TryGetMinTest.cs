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
    public class TryGetMinTest
    {
        #region Int32 tests
        [Test]
        public void NullInt32Source()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetMin(out var x));
        }

        [Test]
        public void NullInt32Selector()
        {
            byte[] source = { 0 };
            Func<byte, int> selector = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetMin(out var x, selector));
        }

        [Test]
        public void EmptySequenceInt32NoSelector()
        {
            int[] source = { };
            Assert.IsFalse(source.TryGetMin(out var x));
        }

        [Test]
        public void EmptySequenceInt32WithSelector()
        {
            byte[] source = { };
            Assert.IsFalse(source.TryGetMin(out var y, x => (int)x));
        }

        [Test]
        public void SimpleSequenceInt32NoSelector()
        {
            int[] source = { 5, 10, 6, 2, 13, 8 };
            Assert.IsTrue(source.TryGetMin(out var y));
            Assert.AreEqual(2, y);
        }
        #endregion

        #region Double tests
        // "Not a number" values have some interesting properties...
        [Test]
        public void SimpleSequenceDouble()
        {
            double[] source = { -2.5d, 2.5d, 0d };
            Assert.IsTrue(source.TryGetMin(out var y));
            Assert.AreEqual(-2.5d, y);
        }

        [Test]
        public void SequenceContainingBothInfinities()
        {
            double[] source = { 1d, double.PositiveInfinity, double.NegativeInfinity };
            Assert.IsTrue(source.TryGetMin(out var y));
            Assert.IsTrue(double.IsNegativeInfinity(y));
        }

        [Test]
        public void SequenceContainingNaN()
        {
            // Comparisons with NaN are odd, basically...
            double[] source = { 1d, double.PositiveInfinity, double.NaN, double.NegativeInfinity };
            // Enumerable.TryGetMin thinks that infinity is more than NaN
            Assert.IsTrue(source.TryGetMin(out var y));
            Assert.IsTrue(double.IsNegativeInfinity(y));
            // Math.TryGetMin thinks that NaN is more than infinity
            Assert.IsTrue(double.IsNaN(Math.Min(double.PositiveInfinity, double.NaN)));
        }
        #endregion
    }
}