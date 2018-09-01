using System;

namespace FwUpdateAPI
{
	public enum HwGeneration
	{
		[Obsolete("Unsupported HW", true)]
		DSL4510_4410,
		[Obsolete("Unsupported HW", true)]
		DSL5520_5320,
		[Obsolete("Unsupported HW", true)]
		DSL5110,
		DSL6540_6340,
		JHL6240,
		JHL6540_6340,
		JHL7540_7440_7340
	}
}
