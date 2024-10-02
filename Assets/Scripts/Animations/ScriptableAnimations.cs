#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableAnimations", menuName = "Animation/ScriptableAnimations")]
public class ScriptableAnimations : ScriptableObject
{
    [SerializeField] private string m_filePath = "Assets/Animations/AnimationFrames.json";
    [SerializeField] private AnimationClip m_clip;
    private AnimationFrame[] m_animationFrames;
    public void ConvertClipToAnimationFrames()
    {
        if (m_clip == null)
        {
            Debug.LogError("AnimationClip is not assigned!");
            return;
        }
        var bindings = AnimationUtility.GetCurveBindings(m_clip);
        List<AnimationFrame> framesList = new List<AnimationFrame>();

        foreach (var binding in bindings)
        {
            Transform targetTransform = GetTransformByPath(binding.path);
            if (targetTransform == null) continue;

            var curve = AnimationUtility.GetEditorCurve(m_clip, binding);
            var keyframes = curve.keys;

            Vector3[] positions = new Vector3[keyframes.Length];
            float[] durations = new float[keyframes.Length - 1];

            for (int i = 0; i < keyframes.Length; i++)
            {
                positions[i] = targetTransform.position;

                if (i > 0)
                {
                    durations[i - 1] = keyframes[i].time - keyframes[i - 1].time;
                }
            }
            framesList.Add(new AnimationFrame
            {
                transformPath = binding.path,
                path = positions,
                animationName = m_clip.name,
                duration = durations
            });
        }
        m_animationFrames = framesList.ToArray();
        Debug.Log($"Converted {m_animationFrames.Length} animation frames.");

        SaveAnimationFramesToFile(m_filePath);
    }
    private Transform GetTransformByPath(string path)
    {
        string[] pathParts = path.Split('/');

        if (m_clip == null)
        {
            Debug.LogError("AnimationClip does not have a valid path or is not assigned.");
            return null;
        }

        Transform currentTransform = null;
        if (m_clip.isHumanMotion)
        {
            Animator animator = FindObjectOfType<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator found in the scene.");
                return null;
            }
            currentTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
        }
        else
        {
            GameObject rootObject = GameObject.Find(pathParts[0]);
            if (rootObject == null)
            {
                Debug.LogError($"GameObject '{pathParts[0]}' not found in the scene.");
                return null;
            }

            currentTransform = rootObject.transform;
        }

        for (int i = 1; i < pathParts.Length; i++)
        {
            if (currentTransform == null)
            {
                Debug.LogError($"Current Transform is null while processing part '{pathParts[i]}'.");
                return null;
            }

            currentTransform = currentTransform.Find(pathParts[i]);

            if (currentTransform == null)
            {
                Debug.LogError($"Transform '{pathParts[i]}' not found in path '{path}'");
                return null;
            }
        }
        return currentTransform;
    }
    private void SaveAnimationFramesToFile(string filePath)
    {
        string json = JsonUtility.ToJson(new AnimationFrameList { frames = m_animationFrames }, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Animation frames saved to {filePath}");
    }
    private void LoadAnimationFramesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Animation frames file does not exist!");
            return;
        }
        string json = File.ReadAllText(filePath);
        AnimationFrameList animationFrameList = JsonUtility.FromJson<AnimationFrameList>(json);
        m_animationFrames = animationFrameList.frames;
        Debug.Log($"Loaded {m_animationFrames.Length} animation frames from {filePath}");
    }
}
[System.Serializable]
public class AnimationFrameList
{
    public AnimationFrame[] frames;
}
[System.Serializable]
public class AnimationFrame
{
    public string animationName;
    public string transformPath;
    public Vector3[] path;
    public float[] duration;
}
#endif