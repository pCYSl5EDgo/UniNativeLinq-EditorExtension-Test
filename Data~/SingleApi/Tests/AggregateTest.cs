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

using UniNativeLinq.TestSupport;
using System;
using NUnit.Framework;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class AggregateTest
    {
        [Test]
        public void NullSourceSeeded()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(3, (x, y) => x + y));
        }

        [Test]
        public void NullSourceSeededRef()
        {
            int[] source = null;
            var a = 3;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(ref a, (ref int x, ref int y) => x += y));
        }

        [Test]
        public void NullFuncSeeded()
        {
            int[] source = { 1, 3 };
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, null));
        }

        [Test]
        public void SeededAggregation()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;
            Func<int, int, int> func = (current, value) => current * 2 + value;
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            Assert.AreEqual(57, source.Aggregate(seed, func));
        }

        [Test]
        public void SeededAggregationRef()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;

            void Action(ref int current, ref int value)
            {
                current = current * 2 + value;
            }

            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            Assert.AreEqual(57, source.Aggregate(seed, Action));
        }

        [Test]
        public void NullSourceSeededWithResultSelector()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(3, (x, y) => x + y, result => result.ToInvariantString()));
        }

        [Test]
        public void NullFuncSeededWithResultSelector()
        {
            int[] source = { 1, 3 };
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, null, result => result.ToInvariantString()));
        }

        [Test]
        public void NullProjectionSeededWithResultSelector()
        {
            int[] source = { 1, 3 };
            Func<int, string> resultSelector = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, (x, y) => x + y, resultSelector));
        }

        [Test]
        public void SeededAggregationWithResultSelector()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;
            Func<int, int, int> func = (current, value) => current * 2 + value;
            Func<int, string> resultSelector = result => result.ToInvariantString();
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            // Result projection: 57.ToInvariantString() = "57"
            Assert.AreEqual("57", source.Aggregate(seed, func, resultSelector));
        }

        [Test]
        public void SeededAggregationWithResultSelectorRef()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;
            void Func(ref int current, ref int value) => current = current * 2 + value;

            string ResultSelector(ref int result) => result.ToInvariantString();
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            // Result projection: 57.ToInvariantString() = "57"
            Assert.AreEqual("57", source.Aggregate(seed, Func, ResultSelector));
        }

        [Test]
        public void DifferentSourceAndAccumulatorTypes()
        {
            int largeValue = 2000000000;
            int[] source = { largeValue, largeValue, largeValue };
            long sum = source.Aggregate(0L, (acc, value) => acc + value);
            Assert.AreEqual(6000000000L, sum);
            // Just to prove we haven't missed off a zero...
            Assert.IsTrue(sum > int.MaxValue);
        }

        [Test]
        public void DifferentSourceAndAccumulatorTypesRef()
        {
            int largeValue = 2000000000;
            int[] source = { largeValue, largeValue, largeValue };
            long sum = default;

            void Func(ref long acc, ref int value) => acc += value;

            source.Aggregate(ref sum, Func);
            Assert.AreEqual(6000000000L, sum);
            // Just to prove we haven't missed off a zero...
            Assert.IsTrue(sum > int.MaxValue);
        }

        [Test]
        public void EmptySequenceSeeded()
        {
            int[] source = { };
            Assert.AreEqual(5, source.Aggregate(5, (x, y) => x + y));
        }

        [Test]
        public void EmptySequenceSeededWithResultSelector()
        {
            int[] source = { };
            Assert.AreEqual("5", source.Aggregate(5, (x, y) => x + y, x => x.ToInvariantString()));
        }
    }
}