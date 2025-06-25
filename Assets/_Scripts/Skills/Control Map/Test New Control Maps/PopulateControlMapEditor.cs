using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControlMap_v2))]
public class PopulateControlMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Populate")) { 
            var map = (ControlMap_v2)target;
            var inputs = System.Enum.GetValues(typeof(SkillInput));

            map.inputIcons = new();

            foreach (var input in inputs)
            {
                map.inputIcons.Add(new SkillInputIcon
                {
                    input = (SkillInput)input,
                    fallbackText = input.ToString()
                });
            }

            EditorUtility.SetDirty(map);
        }
    }
}
