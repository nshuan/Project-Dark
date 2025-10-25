using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace Dark.Scripts.Utils.Skeleton
{
    [CustomPropertyDrawer(typeof(SpineAnimationNameAttribute))]
    public class SpineAnimationNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SpineAnimationNameAttribute animAttr = (SpineAnimationNameAttribute)attribute;
            SerializedObject serializedObject = property.serializedObject;
            SerializedProperty skeletonProp = serializedObject.FindProperty(animAttr.skeletonFieldName);

            EditorGUI.BeginProperty(position, label, property);

            if (skeletonProp == null)
            {
                EditorGUI.LabelField(position, label.text, "⚠ Skeleton field not found");
            }
            else
            {
                SkeletonAnimation skeleton = skeletonProp.objectReferenceValue as SkeletonAnimation;

                if (skeleton != null && skeleton.SkeletonDataAsset != null)
                {
                    var data = skeleton.SkeletonDataAsset.GetSkeletonData(true);
                    if (data != null)
                    {
                        var anims = data.Animations;
                        string[] names = new string[anims.Count];
                        for (int i = 0; i < anims.Count; i++)
                            names[i] = anims.Items[i].Name;

                        int currentIndex = Mathf.Max(0, System.Array.IndexOf(names, property.stringValue));
                        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, names);
                        property.stringValue = names[newIndex];
                    }
                    else
                    {
                        EditorGUI.LabelField(position, label.text, "No skeleton data");
                    }
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Assign a SkeletonAnimation first");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}