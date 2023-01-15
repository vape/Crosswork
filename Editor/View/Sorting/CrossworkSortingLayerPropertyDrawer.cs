using Crosswork.View.Sorting;
using UnityEditor;
using UnityEngine;

namespace Crosswork.Editor.View
{
    [CustomPropertyDrawer(typeof(CrossworkSortingLayer))]
    public class CrossworkSortingLayerPropertyDrawer : PropertyDrawer
    {
        private int[] cachedSortingLayerIds;
        private string[] cachedSortingLayerNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var idProp = property.FindPropertyRelative("Id");
            var current = 0;
            var config = CrossworkSortingConfig.Instance;

            if (config == null)
            {
                EditorGUI.HelpBox(position, "Failed to find sorting config", MessageType.Error);
                return;
            }

            if (config.Layers == null || config.Layers.Length == 0)
            {
                EditorGUI.HelpBox(position, "Config has no layers in it", MessageType.Warning);
                return;
            }

            if (cachedSortingLayerNames == null)
            {
                cachedSortingLayerIds = new int[config.Layers.Length];
                cachedSortingLayerNames = new string[config.Layers.Length];

                for (int i = 0; i < cachedSortingLayerNames.Length; ++i)
                {
                    cachedSortingLayerIds[i] = config.Layers[i].Id;
                    cachedSortingLayerNames[i] = config.Layers[i].Name;
                }
            }

            var foundAny = false;

            for (int i = 0; i < cachedSortingLayerIds.Length; ++i)
            {
                if (cachedSortingLayerIds[i] == idProp.intValue)
                {
                    foundAny = true;
                    current = i;
                    break;
                }
            }

            if (!foundAny)
            {
                idProp.intValue = cachedSortingLayerIds[0];
                current = 0;
            }

            var next = EditorGUI.Popup(position, label.text, current, cachedSortingLayerNames);
            if (next != current)
            {
                idProp.intValue = cachedSortingLayerIds[next];
            }
        }
    }
}
