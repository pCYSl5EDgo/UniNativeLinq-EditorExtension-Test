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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;

namespace UniNativeLinq.TestSupport
{
    /// <summary>
    /// Class to help for deferred execution tests: it throw an exception
    /// if GetEnumerator is called.
    /// </summary>
    public unsafe struct ThrowingEnumerable : IRefEnumerable<ThrowingEnumerable.Enumerator, int>
    {
        public struct Enumerator : IRefEnumerator<int>
        {
            public bool MoveNext() => throw new InvalidOperationException();
            public void Reset() { }
            public ref int Current => throw new InvalidOperationException();
            public ref int TryGetNext(out bool success) => throw new InvalidOperationException();
            public bool TryMoveNext(out int value) => throw new InvalidOperationException();
            int IEnumerator<int>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() { }
        }

        public Enumerator GetEnumerator() => new Enumerator();

        public bool CanFastCount() => true;

        public bool Any() => false;

        public int Count() => 0;

        public long LongCount() => 0L;

        public long CopyTo(int* destination) => 0;

        public NativeEnumerable<int> ToNativeEnumerable(Allocator allocator) => default;

        public NativeArray<int> ToNativeArray(Allocator allocator) => default;

        public int[] ToArray() => Array.Empty<int>();

        public bool CanIndexAccess() => false;

        public ref int this[long index] => throw new InvalidOperationException();

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Check that the given function uses deferred execution.
        /// A "spiked" source is given to the function: the function
        /// call itself shouldn't throw an exception. However, using
        /// the result (by calling GetEnumerator() then MoveNext() on it) *should*
        /// throw InvalidOperationException.
        /// </summary>
        public static void AssertDeferred<T>
            (Func<ThrowingEnumerable, IEnumerable<T>> deferredFunction)
        {
            ThrowingEnumerable source = new ThrowingEnumerable();
            var result = deferredFunction(source);
            using (var iterator = result.GetEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() => iterator.MoveNext());
            }
        }
    }
}