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
    public class SumTest
    {
        #region Int32
        [Test]
        public void NullSourceInt32NoSelector()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Sum());
        } 
        
        [Test]
        public void EmptySequenceInt32()
        {
            int[] source = { };
            Assert.AreEqual(0, source.Sum());
        }

        [Test]
        public void SimpleSumInt32()
        {
            int[] source = { 1, 3, 2 };
            Assert.AreEqual(6, source.Sum());
        }
        #endregion

        #region Int64
        [Test]
        public void NullSourceInt64NoSelector()
        {
            long[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Sum());
        }

        [Test]
        public void EmptySequenceInt64()
        {
            long[] source = { };
            Assert.AreEqual(0, source.Sum());
        }

        [Test]
        public void SimpleSumInt64()
        {
            long[] source = { 1, 3, 2 };
            Assert.AreEqual(6, source.Sum());
        }
        #endregion

        #region Single
        [Test]
        public void NullSourceSingleNoSelector()
        {
            float[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Sum());
        }

        [Test]
        public void EmptySequenceSingle()
        {
            float[] source = { };
            Assert.AreEqual(0, source.Sum());
        }

        [Test]
        public void SimpleSumSingle()
        {
            float[] source = { 1, 3, 2 };
            Assert.AreEqual(6, source.Sum());
        }

        [Test]
        public void SumWithNaNSingle()
        {
            float[] source = { 1, 3, float.NaN, 2 };
            Assert.IsNaN(source.Sum());
        }

        [Test]
        public void SumWithNaNNullableSingle()
        {
            float[] source = { 1, 3, float.NaN, 2 };
            Assert.IsNaN(source.Sum());
        }

        [Test]
        public void OverflowToNegativeInfinitySingle()
        {
            // Only test this once per type - the other overflow tests should be enough
            // for different method calls
            float[] source = { float.MinValue, float.MinValue };
            Assert.IsTrue(float.IsNegativeInfinity(source.Sum()));
        }

        [Test]
        public void OverflowToInfinitySingle()
        {
            float[] source = { float.MaxValue, float.MaxValue };
            Assert.IsTrue(float.IsPositiveInfinity(source.Sum()));
        }

        [Test]
        public void NonOverflowOfComputableSumSingle()
        {
            float[] source = { float.MaxValue, float.MaxValue,
                              -float.MaxValue, -float.MaxValue };
            // In a world where we summed using a float accumulator, the
            // result would be infinity.
            Assert.AreEqual(float.PositiveInfinity, source.Sum());
        }

        [Test]
        public void AccumulatorAccuracyForSingle()
        {
            // 20000000 and 20000004 are both exactly representable as
            // float values, but 20000001 is not. Therefore if we use
            // a float accumulator, we'll end up with 20000000. However,
            // if we use a double accumulator, we'll get the right value.
            float[] array = { 20000000f, 1f, 1f, 1f, 1f };
            Assert.AreEqual(20000000f, array.Sum());
        }
        #endregion

        #region Double
        [Test]
        public void NullSourceDoubleNoSelector()
        {
            double[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Sum());
        }

        [Test]
        public void EmptySequenceDouble()
        {
            double[] source = { };
            Assert.AreEqual(0, source.Sum());
        }

        [Test]
        public void SimpleSumDouble()
        {
            double[] source = { 1, 3, 2 };
            Assert.AreEqual(6, source.Sum());
        }

        [Test]
        public void SumWithNaNDouble()
        {
            double[] source = { 1, 3, double.NaN, 2 };
            Assert.IsNaN(source.Sum());
        }

        [Test]
        public void SumWithNaNNullableDouble()
        {
            double[] source = { 1, 3, double.NaN, 2 };
            Assert.IsNaN(source.Sum());
        }

        [Test]
        public void OverflowToNegativeInfinityDouble()
        {
            // Only test this once per type - the other overflow tests should be enough
            // for different method calls
            double[] source = { double.MinValue, double.MinValue };
            Assert.IsTrue(double.IsNegativeInfinity(source.Sum()));
        }

        [Test]
        public void OverflowToInfinityDouble()
        {
            double[] source = { double.MaxValue, double.MaxValue };
            Assert.IsTrue(double.IsPositiveInfinity(source.Sum()));
        }
        #endregion
    }
}