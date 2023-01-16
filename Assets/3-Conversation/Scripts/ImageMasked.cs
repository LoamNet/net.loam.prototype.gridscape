using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// Apply this to a child under the mask
/// </summary>
[System.Serializable]
public class ImageMasked : Image
{
    [SerializeField] public CompareFunction compareFunction;

    public override Material materialForRendering
    {
        get
        {
            Material mat = new Material(base.materialForRendering);
            mat.SetInt("_StencilComp", (int)compareFunction);
            return mat;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ImageMasked))]
    [UnityEditor.CanEditMultipleObjects] // If safe
    public class CustomMaskEditor : UnityEditor.Editor
    {
        UnityEditor.SerializedProperty property;
        private void OnEnable()
        {
            property = serializedObject.FindProperty("compareFunction");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}
