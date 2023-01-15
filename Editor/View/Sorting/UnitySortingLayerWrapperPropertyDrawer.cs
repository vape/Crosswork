using Crosswork.View.Sorting;
using UnityEditor;
using UnityEngine;

namespace Deszz.Simb.Sorting.Editor
{
    [CustomPropertyDrawer(typeof(UnitySortingLayerWrapper))]
    public class UnitySortingLayerWrapperPropertyDrawer : PropertyDrawer
    {
        private int[] cachedSortingLayerIds;
        private string[] cachedSortingLayerNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var idProp = property.FindPropertyRelative("Id");
            var current = 0;

            if (cachedSortingLayerNames == null)
            {
                cachedSortingLayerIds = new int[SortingLayer.layers.Length];
                cachedSortingLayerNames = new string[SortingLayer.layers.Length];

                for (int i = 0; i < cachedSortingLayerNames.Length; ++i)
                {
                    cachedSortingLayerIds[i] = SortingLayer.layers[i].id;
                    cachedSortingLayerNames[i] = SortingLayer.layers[i].name;
                }
            }

            for (int i = 0; i < cachedSortingLayerIds.Length; ++i)
            {
                if (cachedSortingLayerIds[i] == idProp.intValue)
                {
                    current = i;
                    break;
                }
            }

            var next = EditorGUI.Popup(position, label.text, current, cachedSortingLayerNames);
            if (next != current)
            {
                idProp.intValue = cachedSortingLayerIds[next];
            }
        }
    }
}
