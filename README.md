# EnumFlagsPropertyDrawer

[Flags]
public enum Days
{
	None = 0 << 0,
	Sunday = 1 << 1,
	Monday = 1 << 2,
	Tuesday = 1 << 3,
	Wednesday = 1 << 4,
	Thursday = 1 << 5,
	Friday = 1 << 6,
	Saturday = 1 << 7
}

[SerializeField, EnumFlags]
private Days weekdays;

// This lets you give the field a custom name in the inspector.
[EnumFlags("Custom Inspector Name")]
public Days weekend;