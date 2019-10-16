﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class SimpleEnumerableSingleApi : String2BoolArrayTuple, ISingleApi
    {
        string ISingleApi.Name
        {
            get => Name;
            set
            {
                if (RelatedEnumerableArray?.Length != 1)
                    RelatedEnumerableArray = new string[1];
                Name = RelatedEnumerableArray[0] = value;
            }
        }

        string ISingleApi.Description => Description;
        [field: SerializeField] public bool IsHided { get; set; }
        [field: SerializeField] public string[] RelatedEnumerableArray { get; private set; }
        [field:SerializeField]public string[] ExcludeEnumerableArray { get; internal set; }
        public IEnumerable<string> NameCollection => EnabledArray.Select(x => x.Enumerable);
        public int Count => EnabledArray.Length;
        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection => EnabledArray.Select(x => (x.Enumerable, x.Enabled));
        public IEnumerable<string> EnabledNameCollection => EnabledArray.Where(x => x.Enabled).Select(x => x.Enumerable);
        public bool TryGetEnabled(string name, out bool value)
        {
            for (var i = 0; i < EnabledArray.Length; i++)
            {
                ref var tuple = ref EnabledArray[i];
                if (tuple.Enumerable != name) continue;
                value = tuple.Enabled;
                return true;
            }
            value = default;
            return false;
        }

        public bool TrySetEnabled(string name, bool value)
        {
            for (var i = 0; i < EnabledArray.Length; i++)
            {
                ref var tuple = ref EnabledArray[i];
                if (tuple.Enumerable != name) continue;
                tuple.Enabled = value;
                EditorUtility.SetDirty(this);
                return true;
            }
            return false;
        }

        private bool fold;
        public void Draw(IEnumerableCollectionProcessor processor)
        {
            if (IsHided) return;
            foreach (var relatedEnumerable in RelatedEnumerableArray)
            {
                if (!processor.TryGetEnabled(relatedEnumerable, out var enabled))
                {
                    Debug.LogError(relatedEnumerable + " of " + Name);
                    throw new KeyNotFoundException();
                }
                if (!enabled) return;
            }
            if (!FoldoutUtility.Draw(ref fold, Name)) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = true;
                    }
                    EditorUtility.SetDirty(this);
                }
                else if (GUILayout.Button("Deselect All"))
                {
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = false;
                    }
                    EditorUtility.SetDirty(this);
                }
                else if (GUILayout.Button("Hide and Deselect All"))
                {
                    IsHided = true;
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = false;
                    }
                    EditorUtility.SetDirty(this);
                }
            }

            using (IndentScope.Create())
            {
                for (var i = 0; i < EnabledArray.Length; i++)
                {
                    ref var tuple = ref EnabledArray[i];
                    if (!processor.TryGetEnabled(tuple.Enumerable, out var targetEnabled) || !targetEnabled || (ExcludeEnumerableArray?.Contains(tuple.Enumerable) ?? false)) continue;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(new GUIContent(tuple.Enumerable, Description));
                        var guiContent = new GUIContent(tuple.Enumerable + " : " + tuple.Enabled, Description);
                        TrySetEnabled(tuple.Enumerable, EditorGUILayout.ToggleLeft(guiContent, tuple.Enabled, GUI.skin.button));
                    }
                }
            }
        }

        public int CompareTo(ISingleApi other)
        {
            if (other is null)
                return 1;
            var c = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return c == 0 ? string.Compare(Description, other.Description, StringComparison.Ordinal) : c;
        }
    }
}
