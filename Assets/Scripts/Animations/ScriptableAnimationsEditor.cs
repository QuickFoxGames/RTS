#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ScriptableAnimations))]
public class ScriptableAnimationsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScriptableAnimations scriptableAnimations = (ScriptableAnimations)target;
        if (GUILayout.Button("Convert Animation Clip"))
        {
            scriptableAnimations.ConvertClipToAnimationFrames();
        }
    }
}
#endif