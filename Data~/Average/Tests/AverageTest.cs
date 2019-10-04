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
    public class TryGetAverageTest
    {
        #region General Int32 tests
        [Test]
        public void NullSourceInt32NoSelector()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetAverage(out _));
        }

        [Test]
        public void NullSourceNullableInt32NoSelector()
        {
            int?[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.TryGetAverage(out _));
        }

        [Test]
        public void EmptySequenceInt32NoSelector()
        {
            int[] source = { };
            Assert.IsFalse(source.TryGetAverage(out _));
        }

        [Test]
        public void EmptySequenceNullableInt32NoSelector()
        {
            int?[] source = { };
            Assert.IsFalse(source.TryGetAverage(out var x));
            Assert.IsNull(x);
        }

        [Test]
        public void AllNullsSequenceNullableInt32NoSelector()
        {
            int?[] source = { null, null, null };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.IsNull(x);
        }

        [Test]
        public void SimpleTryGetAverageInt32NoSelector()
        {
            // Note that 7.5 is exactly representable as a double, so we
            // shouldn't need to worry about floating-point inaccuracies
            int[] source = { 5, 10, 0, 15 };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.AreEqual(7.5d, x);
        }

        [Test]
        public void SimpleTryGetAverageNullableInt32NoSelector()
        {
            int?[] source = { 5, 10, 0, 15 };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.AreEqual((double?)7.5d, x);
        }

        [Test]
        public void TryGetAverageIgnoresNullsNullableInt32NoSelector()
        {
            // The nulls here don't reduce the average
            int?[] source = { 5, null, 10, null, 0, null, 15 };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.AreEqual((double?)7.5d, x);
        }

        [Test]
        [Ignore("Takes an enormous amount of time!")]
        public void MoreThanInt32MaxValueElements()
        {
            try
            {
                var rangeRepeatEnumerable = Enumerable.Repeat(1, int.MaxValue);
                var repeatEnumerable = Enumerable.Repeat(1, 5);
                var query = rangeRepeatEnumerable
                    .Concat(repeatEnumerable);
                Assert.IsTrue(query.TryGetAverage(out var x));
                Assert.AreEqual(1d, x);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        #endregion

        #region Floating point fun
        [Test]
        public void SequenceContainingNan()
        {
            double[] array = { 1, 2, 3, double.NaN, 4, 5, 6 };
            Assert.IsTrue(array.TryGetAverage(out var x));
            Assert.IsNaN(x);
        }
        #endregion

        #region Overflow tests
        [Test]
        public void Int32DoesNotOverflowAtInt32MaxValue()
        {
            int[] array = { int.MaxValue, int.MaxValue,
                             -int.MaxValue, -int.MaxValue};
            Assert.IsTrue(array.TryGetAverage(out var x));
            Assert.AreEqual(0, x);
        }

        [Test]
        public void DoubleOverflowsToInfinity()
        {
            double[] source = { double.MaxValue, double.MaxValue,
                               -double.MaxValue, -double.MaxValue };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.IsTrue(double.IsPositiveInfinity(x));
        }

        [Test]
        public void DoubleOverflowsToNegativeInfinity()
        {
            double[] source = { -double.MaxValue, -double.MaxValue,
                                double.MaxValue, double.MaxValue };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.IsTrue(double.IsNegativeInfinity(x));
        }

        [Test]
        public void Int64KeepsPrecisionAtLargeValues()
        {
            // At long.MaxValue / 2, double precision doesn't get us
            // exact integers.
            long halfMax = long.MaxValue / 2;
            double halfMaxAsDouble = (double)halfMax;
            Assert.AreNotEqual(halfMax, (long)halfMaxAsDouble);

            long[] source = { halfMax, halfMax };
            Assert.IsTrue(source.TryGetAverage(out var x));
            Assert.AreEqual(halfMax, x);
        }
        #endregion
    }
}