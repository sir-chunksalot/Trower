using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		newposition.y += 18f;
		SerializedProperty data = property.FindPropertyRelative("rows");
		SerializedProperty xBounds = property.FindPropertyRelative("xBounds");
		SerializedProperty yBounds = property.FindPropertyRelative("yBounds");
		//data.rows[0][]
		for (int j = yBounds.intValue; j >= 0; j--)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
			newposition.height = 18f;
			if (row.arraySize != xBounds.intValue)
				row.arraySize = xBounds.intValue;
			newposition.width = position.width / 7;
			for (int i = 0; i < xBounds.intValue; i++)
			{
				EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
				newposition.x += newposition.width;
			}


			newposition.x = position.x;
			newposition.y += 18f;
		}
	}


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 18f * 8;
	}
}
