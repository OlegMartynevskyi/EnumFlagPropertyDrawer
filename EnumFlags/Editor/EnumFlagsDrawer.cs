using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;


[CustomPropertyDrawer (typeof(EnumFlags))]
public sealed class EnumFlagsDrawer : PropertyDrawer
{
	FieldInfo enumField;
	int rowCount;
	bool foldout;

	static readonly int fontSize = 12;
	static readonly float LABEL_HEIGHT = 20.0f;
	static readonly float TOGGLE_HEIGHT = 18.0f;

	static readonly GUIStyle toggleButtonNormalStyle;
	static readonly GUIStyle toggleButtonToggledStyle;

	static EnumFlagsDrawer ()
	{
		toggleButtonNormalStyle = "Button";
		toggleButtonToggledStyle = new GUIStyle (toggleButtonNormalStyle);
		toggleButtonToggledStyle.normal.background = toggleButtonToggledStyle.active.background;
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{		
		float height = 0.0f;
		if (enumField != null && enumField.FieldType.IsEnum) {
			height += LABEL_HEIGHT;
		}
		if (foldout) {
			height += rowCount * TOGGLE_HEIGHT;
		}
		return height > 0 ? height : base.GetPropertyHeight (property, label);
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{		
		EditorGUI.BeginProperty (position, label, property);	
		enumField = property.serializedObject.targetObject.GetType ().GetField (property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		if (enumField != null && enumField.FieldType.IsEnum) {
			EnumFlags enumFlags = (EnumFlags)attribute;
			string labelText = string.IsNullOrEmpty (enumFlags.EnumName) ? enumField.Name : enumFlags.EnumName;
			foldout = EditorGUI.Foldout (new Rect (position.x, position.y, position.width, LABEL_HEIGHT), foldout, labelText);
			if (foldout) {
				Type enumType = enumField.FieldType;
				string[] enumNames = Enum.GetNames (enumType);
				int[] enumValues = (int[])Enum.GetValues (enumType); 

				if (enumValues.Length > 0) {
					//Skip zero
					int togglesShift = enumValues [0] > 0 ? 0 : 1; 
					bool[] toggles = new bool[enumValues.Length - togglesShift];
					if (toggles.Length > 0) {					
						for (int i = 0; i < toggles.Length; ++i) {				
							toggles [i] = (property.intValue & enumValues [i + togglesShift]) == enumValues [i + togglesShift];  
						}

						int index = 0;
						int row = 0;
						float totalWidth = 0.0f;
						EditorGUI.BeginChangeCheck ();
						do {							
							float toggleWidth = enumNames [index + togglesShift].Length * fontSize;
							if (totalWidth + toggleWidth > position.width) {
								++row;
								totalWidth = 0;
							}
							Rect togglePosition = new Rect (position.x + totalWidth, position.y + LABEL_HEIGHT + row * TOGGLE_HEIGHT, toggleWidth, TOGGLE_HEIGHT);
							if (GUI.Button (togglePosition, enumNames [index + togglesShift], toggles [index] ? toggleButtonToggledStyle : toggleButtonNormalStyle)) {
								toggles [index] = !toggles [index];
							}
							++index;
							totalWidth += toggleWidth;
						} while(index < toggles.Length);

						if (EditorGUI.EndChangeCheck ()) {
							property.intValue = 0;
							for (int i = 0; i < toggles.Length; ++i) {
								if (toggles [i])
									property.intValue |= enumValues [i + togglesShift];								
							}
						}
						rowCount = row + 1;
					}
				}
			}
		}
		EditorGUI.EndProperty ();
	}
}