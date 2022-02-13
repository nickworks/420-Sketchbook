using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TreeFloat {
    public float atBase = 0;
    public float atTop = 0;
    public TreeFloat(float atBase, float atTop) {
        this.atBase = atBase;
        this.atTop = atTop;
    }
    public float Lerp(float p) {
        return Mathf.Lerp(atBase, atTop, p);
    }
} 
public class TreeRange : PropertyAttribute {
    public float min;
    public float max;
    public TreeRange(float min, float max) {
        this.min = min;
        this.max = max;
    }
}


[CustomPropertyDrawer(typeof(TreeRange))]
public class TreeRangeDrawer : PropertyDrawer {


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        // use the default property height, which takes into account the expanded state
        return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // First get the attribute since it contains the range for the slider
        TreeRange range = attribute as TreeRange;

        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if (property.propertyType == SerializedPropertyType.Generic) {


            Rect foldoutLabelPosition = position;
            foldoutLabelPosition.width = EditorGUIUtility.labelWidth;
            foldoutLabelPosition.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = EditorGUI.Foldout(foldoutLabelPosition, property.isExpanded, label.text, true);

            if (property.isExpanded) {

                EditorGUI.BeginProperty(position, label, property);

                SerializedProperty atBase = property.FindPropertyRelative("atBase");
                SerializedProperty atTop = property.FindPropertyRelative("atTop");
                //position.xMin += EditorGUIUtility.labelWidth;
                position.height = EditorGUIUtility.singleLineHeight;
                position.y += EditorGUIUtility.singleLineHeight;
                atTop.floatValue = EditorGUI.Slider(position, atTop.floatValue, range.min, range.max);
                position.y += EditorGUIUtility.singleLineHeight;
                atBase.floatValue = EditorGUI.Slider(position, atBase.floatValue, range.min, range.max);

                EditorGUI.EndProperty();

            }

        } else {
            EditorGUI.LabelField(position, label.text, "[TreeRange()] does not work with type " + property.propertyType.ToString());
        }

    }
}