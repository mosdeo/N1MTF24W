using System;

namespace Battery
{
	[Flags]
	public enum BatteryChargeStatus : byte
	{
		High = 0x1,
		Low = 0x2,
		Critical = 0x4,
		Charging = 0x8,
		NoSystemBattery = 0x80,
		Unknown = byte.MaxValue
	}
}
