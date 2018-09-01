using FwUpdateAPI;
using System.Collections.Generic;
using System.Linq;

namespace FwUpdateTool.WizardSteps
{
	internal class Port
	{
		public string DisplayName
		{
			get;
			private set;
		}

		public List<Device> Devices
		{
			get;
			private set;
		}

		public bool IsSelectable => false;

		public Port(uint index, IOrderedEnumerable<SdkTbtDevice> currentDevices)
		{
			DisplayName = "Port " + (index + 1) + ":";
			Devices = new List<Device>();
			foreach (SdkTbtDevice currentDevice in currentDevices)
			{
				Devices.Add(new Device(currentDevice));
			}
		}
	}
}
