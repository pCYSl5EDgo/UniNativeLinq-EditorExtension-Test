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

using NUnit.Framework;
using UniNativeLinq.TestSupport;
using Unity.Collections;

namespace UniNativeLinq.Tests
{
    class MinMaxByTest
    {
        [Test]
        public void ThrowingTest()
        {
            new MinByInt32Enumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, AsIs<int>>(new ThrowingEnumerable(), default, Allocator.Temp);
        }

        [Test]
        public void SimpleInt64Test()
        {
            using (var array = new NativeArray<long>(4, Allocator.Temp)
            {
                [0] = 0x2,
                [1] = 0x101,
                [2] = 0x1,
                [3] = 0x202,
            })
            {
                array.MinBy(x => (int)(x & 0xf)).AssertSequenceEqual(0x101L, 0x1L);
            }
        }

        [Test]
        public void SimpleDoubleTest()
        {
            using (var array = new NativeArray<double>(4, Allocator.Temp)
            {
                [0] = 2.0,
                [1] = 1.5,
                [2] = 1.0,
                [3] = 2.45,
            })
            {
                array.MinBy(x => (int)x).AssertSequenceEqual(1.5, 1.0);
            }
        }

        [Test]
        public void SimpleSingleTest()
        {
            using (var array = new NativeArray<float>(4, Allocator.Temp)
            {
                [0] = 2.0f,
                [1] = 1.5f,
                [2] = 1.0f,
                [3] = 2.45f,
            })
            {
                array.MinBy(x => (int)x).AssertSequenceEqual(1.5f, 1.0f);
            }
        }

        [Test]
        public void SimpleInt32Test()
        {
            using (var array = new NativeArray<int>(4, Allocator.Temp)
            {
                [0] = 0x2,
                [1] = 0x101,
                [2] = 0x1,
                [3] = 0x202,
            })
            {
                array.MinBy(x => (int)(x & 0xf)).AssertSequenceEqual(0x101, 0x1);
            }
        }
    }
}