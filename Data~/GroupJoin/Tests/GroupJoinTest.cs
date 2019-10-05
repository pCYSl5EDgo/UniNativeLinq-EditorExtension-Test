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
using UniNativeLinq.TestSupport;
using NUnit.Framework;
using Unity.Collections;

namespace UniNativeLinq.Tests
{
    [TestFixture]
    public class GroupJoinTest
    {
        [Test]
        public void ExecutionIsDeferred()
        {
            new GroupJoinEnumerable<ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, ThrowingEnumerable, ThrowingEnumerable.Enumerator, int, int, AsIs<int>, AsIs<int>, DefaultEqualityComparer<int>, int, LengthAdd>(new ThrowingEnumerable(), new ThrowingEnumerable(), default, default, default, default, Allocator.Temp);
        }

        readonly struct LengthAdd : IRefFunc<int, WhereIndexEnumerable<NativeEnumerable<int>, NativeEnumerable<int>.Enumerator, int, GroupJoinPredicate<int, int, DefaultEqualityComparer<int>>>, int>
        {
            public int Calc(ref int arg0, ref WhereIndexEnumerable<NativeEnumerable<int>, NativeEnumerable<int>.Enumerator, int, GroupJoinPredicate<int, int, DefaultEqualityComparer<int>>> arg1)
                => arg0 + arg1.Count();
        }

        [Test]
        public void SimpleGroupJoin()
        {
            // We're going to join on the first character in the outer sequence item
            // being equal to the second character in the inner sequence item
            int[] outer = { 100, 600, 300 };
            int[] inner = { 16, 32, 64, 96 };

            var query = GroupJoinFuncHelper.GroupJoin(
                outer,
                inner,
                OuterKey,
                InnerKey,
                (x, y) => x == y,
                (x, y) => x + y.Count(), Allocator.Temp);

            query.AssertSequenceEqual(101, 601, 301);
        }

        private static int OuterKey(int outerElement) => outerElement / 100;

        private static int InnerKey(int innerElement) => innerElement / 10;
    }
}