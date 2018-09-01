using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace FwUpdateAPI
{
	public class SdkTbtDevice : SdkTbtBase
	{
		public override string GetNeedPowerDownMessage => Resources.DeviceNeedsPowerDownMessage;

		public string UUID => base.Mo.GetPropertyValue("UUID").ToString();

		public string ControllerId => base.Mo.GetPropertyValue("ControllerId").ToString();

		public uint PortNum => (uint)base.Mo.GetPropertyValue("PortNum");

		public uint PositionInChain => (uint)base.Mo.GetPropertyValue("PositionInChain");

		public string VendorName => base.Mo.GetPropertyValue("VendorName").ToString();

		public string ModelName => base.Mo.GetPropertyValue("ModelName").ToString();

		public ushort VendorId => (ushort)(uint)base.Mo.GetPropertyValue("VendorId");

		public ushort ModelId => (ushort)(uint)base.Mo.GetPropertyValue("ModelId");

		public byte ControllerNum => (byte)base.Mo.GetPropertyValue("ControllerNumber");

		public byte NumOfControllers => (byte)base.Mo.GetPropertyValue("NumberOfControllers");

		public bool Updatable => (bool)base.Mo.GetPropertyValue("Updatable");

		public byte LinkSpeed => (byte)base.Mo.GetPropertyValue("LinkSpeed");

		public SdkTbtDevice(ManagementObject mo)
			: base(mo)
		{
		}

		public static Dictionary<string, SdkTbtDevice> GetDevicesFromWmi()
		{
			Utilities.StartService();
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\Intel\\Thunderbolt", "SELECT * FROM SdkTbtDevice"))
			{
				Dictionary<string, SdkTbtDevice> dictionary = new Dictionary<string, SdkTbtDevice>();
				foreach (ManagementObject item in managementObjectSearcher.Get().Cast<ManagementObject>())
				{
					SdkTbtDevice sdkTbtDevice = new SdkTbtDevice(item);
					dictionary.Add(sdkTbtDevice.UUID, sdkTbtDevice);
				}
				return dictionary;
			}
		}

		public override void ValidateImage(string path)
		{
			if (!File.Exists(path))
			{
				throw new TbtException(TbtStatus.SDK_FILE_NOT_FOUND);
			}
			if (!Updatable)
			{
				throw new TbtException(TbtStatus.SDK_DEVICE_NOT_SUPPORTED);
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] image = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
				DeviceFwInfo deviceFwInfo = new DeviceFwInfo(new ControllerFwInfoSource(this));
				DeviceFwInfo deviceFwInfo2 = new DeviceFwInfo(new FileFwInfoSource(image));
				if (deviceFwInfo.Info.Generation != deviceFwInfo2.Info.Generation)
				{
					throw new TbtException(TbtStatus.SDK_HW_GENERATION_MISMATCH);
				}
				if (deviceFwInfo.Info.Type != deviceFwInfo2.Info.Type)
				{
					throw new TbtException(TbtStatus.SDK_PORT_COUNT_MISMATCH);
				}
				new DeviceImageValidator(this, image, deviceFwInfo.GetSectionInfo(), deviceFwInfo2.GetSectionInfo(), deviceFwInfo.Info).Validate();
			}
		}
	}
}
