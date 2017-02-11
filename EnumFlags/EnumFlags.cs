using UnityEngine;


public class EnumFlags : PropertyAttribute {

	public string EnumName { get; private set; }

	public EnumFlags() {
	}

	public EnumFlags(string enumName) {
		EnumName = enumName;
	}
}
