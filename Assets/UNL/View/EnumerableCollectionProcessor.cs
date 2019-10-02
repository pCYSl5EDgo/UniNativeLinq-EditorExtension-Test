﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UniNativeLinq.Editor
{
    internal sealed class EnumerableCollectionProcessor : IEnumerableCollectionProcessor
    {
        private readonly StringBoolTuple[] types;
        private readonly Dictionary<string, StringBoolTuple> dictionary;
        private bool hasChanged;

        public EnumerableCollectionProcessor(StringBoolTuple[] types)
        {
            this.types = types ?? Array.Empty<StringBoolTuple>();
            Array.Sort(this.types, 0, this.types.Length, new Comparer());
            dictionary = this.types.ToDictionary(x => x.Enumerable, x => x);
        }

        sealed class Comparer : IComparer<StringBoolTuple>
        {
            public int Compare(StringBoolTuple x, StringBoolTuple y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                if (x.IsSpecial && !y.IsSpecial) return -1;
                if (!x.IsSpecial && y.IsSpecial) return 1;
                return string.Compare(x.Enumerable, y.Enumerable, StringComparison.Ordinal);
            }
        }

        public int Count => types.Length;

        public IEnumerable<string> NameCollection => types.Select(Selector);

        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection
        {
            get
            {
                (string Name, bool Enabled) Selector(StringBoolTuple _) => (_.Enumerable, _.Enabled);
                return types.Select(Selector);
            }
        }

        public IEnumerable<string> EnabledNameCollection => types.Where(Predicate).Select(Selector);

        private static bool Predicate(StringBoolTuple _) => _.Enabled;
        private static string Selector(StringBoolTuple _) => _.Enumerable;

        public bool TryGetEnabled(string name, out bool value)
        {
            if (!dictionary.TryGetValue(name, out var tuple)) return value = false;
            value = tuple.Enabled;
            return true;
        }

        public bool TrySetEnabled(string name, bool value)
        {
            if (!dictionary.TryGetValue(name, out var tuple)) return false;
            hasChanged = true;
            tuple.Enabled = value;
            EditorUtility.SetDirty(tuple);
            return true;
        }

        public bool IsSpecialType(string name, out bool value)
        {
            if (!dictionary.TryGetValue(name, out var tuple)) return value = false;
            value = tuple.IsSpecial;
            return true;
        }

        public uint GetGenericParameterCount(string name) => dictionary[name].GenericParameterCount;

        public IEnumerable<string> GetRelatedEnumerable(string name) => dictionary[name].RelatedEnumerableArray;
        public string GetElementType(string name) => dictionary[name].ElementType;

        public bool HasChanged => hasChanged;

        public void Apply() => hasChanged = false;
    }
}
