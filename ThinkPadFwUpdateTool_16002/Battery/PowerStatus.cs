using System.Runtime.InteropServices;

namespace Battery
{
	internal class PowerStatus
	{
		private struct SystemPowerStatus
		{
			public PowerLineStatus PowerLineStatus;

			public BatteryChargeStatus BatteryChargeStatus;

			public byte BatteryLifePercent;

			public byte Reserved;

			public int BatteryLifeRemaining;

			public int BatteryFullLifeTime;
		}

		private SystemPowerStatus _powerStatus;

		public PowerLineStatus PowerLineStatus => _powerStatus.PowerLineStatus;

		public BatteryChargeStatus BatteryChargeStatus => _powerStatus.BatteryChargeStatus;

		public float BatteryLifePercent => (float)(int)_powerStatus.BatteryLifePercent;

		public int BatteryLifeRemaining => _powerStatus.BatteryLifeRemaining;

		public int BatteryFullLifeTime => _powerStatus.BatteryFullLifeTime;

		[DllImport("kernel32")]
		private static extern void GetSystemPowerStatus(ref SystemPowerStatus powerStatus);

		public PowerStatus()
		{
			UpdatePowerInfo();
		}

		public void UpdatePowerInfo()
		{
			GetSystemPowerStatus(ref _powerStatus);
		}
	}
}
