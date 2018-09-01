using FwUpdateAPI;
using System.Collections.Generic;
using System.Linq;

namespace FwUpdateTool.WizardSteps
{
	internal class Controller : ISdk
	{
		private readonly SdkTbtController _sdkHandle;

		public SdkTbtBase SdkHandle => _sdkHandle;

		public string DisplayName
		{
			get;
			private set;
		}

		public List<Port> Ports
		{
			get;
			private set;
		}

		public bool IsSelectable
		{
			get;
			private set;
		}

		public Controller(SdkTbtController sdkHandle, IEnumerable<SdkTbtDevice> devices)
		{
			_sdkHandle = sdkHandle;
			DisplayName = _sdkHandle.ControllerId;
			IList<SdkTbtDevice> source = (devices as IList<SdkTbtDevice>) ?? devices.ToList();
			IEnumerable<IGrouping<uint, SdkTbtDevice>> enumerable = from device in source
			group device by device.PortNum into port
			select (port);
			Ports = new List<Port>();
			foreach (IGrouping<uint, SdkTbtDevice> item in enumerable)
			{
				IGrouping<uint, SdkTbtDevice> localPort = item;
				IOrderedEnumerable<SdkTbtDevice> currentDevices = from device in source
				where device.PortNum == localPort.Key
				orderby device.PositionInChain
				select device;
				Ports.Add(new Port(item.Key, currentDevices));
			}
			IsSelectable = Utilities.IsSupported(DisplayName);
		}
	}
}
