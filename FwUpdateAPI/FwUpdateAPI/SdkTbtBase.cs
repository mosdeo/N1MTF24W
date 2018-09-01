using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace FwUpdateAPI
{
	public abstract class SdkTbtBase
	{
		public abstract string GetNeedPowerDownMessage
		{
			get;
		}

		protected ManagementObject Mo
		{
			get;
			private set;
		}

		protected SdkTbtBase(ManagementObject mo)
		{
			Mo = mo;
		}

		public abstract void ValidateImage(string path);

		public void UpdateFirmware(uint bufferSize, byte[] buffer)
		{
			ManagementBaseObject methodParameters = Mo.GetMethodParameters("UpdateFirmware");
			methodParameters["bufferSize"] = bufferSize;
			methodParameters["buffer"] = buffer;
			TbtStatus tbtStatus = (TbtStatus)Mo.InvokeMethod("UpdateFirmware", methodParameters, null).GetPropertyValue("ReturnValue");
			if (tbtStatus != 0)
			{
				throw new TbtException(tbtStatus, Resources.FWUpdateFailedMessage);
			}
		}

		public virtual uint GetCurrentNvmVersion()
		{
			ManagementBaseObject managementBaseObject = Mo.InvokeMethod("GetCurrentNvmVersion", null, null);
			TbtStatus tbtStatus = (TbtStatus)managementBaseObject.GetPropertyValue("ReturnValue");
			if (tbtStatus != 0)
			{
				throw new TbtException(tbtStatus, Resources.GetFirmwareVersionFailed);
			}
			return (uint)managementBaseObject.GetPropertyValue("nvmVersion");
		}

		public virtual string GetCurrentFullNvmVersion()
		{
			ManagementBaseObject managementBaseObject = Mo.InvokeMethod("GetCurrentFullNvmVersion", null, null);
			TbtStatus tbtStatus = (TbtStatus)managementBaseObject.GetPropertyValue("ReturnValue");
			switch (tbtStatus)
			{
			case TbtStatus.DEVICE_NOT_SUPPORTED:
				return "N/A";
			default:
				throw new TbtException(tbtStatus, Resources.GetFirmwareVersionFailed);
			case TbtStatus.SUCCESS_RESPONSE_CODE:
				return (string)managementBaseObject.GetPropertyValue("nvmVersion");
			}
		}

		public virtual byte[] I2CRead(uint port, uint offset, uint length)
		{
			ManagementBaseObject methodParameters = Mo.GetMethodParameters("I2CRead");
			methodParameters["port"] = port;
			methodParameters["offset"] = offset;
			methodParameters["length"] = length;
			ManagementBaseObject managementBaseObject = Mo.InvokeMethod("I2CRead", methodParameters, null);
			TbtStatus tbtStatus = (TbtStatus)managementBaseObject.GetPropertyValue("ReturnValue");
			if (tbtStatus != 0)
			{
				throw new TbtException(tbtStatus, Resources.ReadI2CFailed);
			}
			return (byte[])managementBaseObject.GetPropertyValue("data");
		}

		public virtual void I2CWrite(uint port, uint offset, byte[] data)
		{
			ManagementBaseObject methodParameters = Mo.GetMethodParameters("I2CWrite");
			methodParameters["port"] = port;
			methodParameters["offset"] = offset;
			methodParameters["data"] = data;
			TbtStatus tbtStatus = (TbtStatus)Mo.InvokeMethod("I2CWrite", methodParameters, null).GetPropertyValue("ReturnValue");
			if (tbtStatus != 0)
			{
				throw new TbtException(tbtStatus, Resources.WriteI2CFailed);
			}
		}

		[Obsolete("This method is obsolete, use I2CRead", true)]
		public virtual string GetCurrentPdVersion()
		{
			return "N/A";
		}

		public virtual byte[] ReadFirmware(uint offset, uint length)
		{
			ManagementBaseObject methodParameters = Mo.GetMethodParameters("ReadFirmware");
			methodParameters["length"] = 1;
			List<byte> list = new List<byte>();
			uint num = offset % 4u;
			for (uint num2 = offset - num; num2 < offset + length; num2 += 4)
			{
				methodParameters["offset"] = num2;
				ManagementBaseObject managementBaseObject = Mo.InvokeMethod("ReadFirmware", methodParameters, null);
				TbtStatus tbtStatus = (TbtStatus)managementBaseObject.GetPropertyValue("ReturnValue");
				if (tbtStatus != 0)
				{
					throw new TbtException(tbtStatus, Resources.ReadFirmwareFailed);
				}
				list.AddRange((byte[])managementBaseObject.GetPropertyValue("data"));
			}
			return list.Skip((int)num).Take((int)length).ToArray();
		}

		public void UpdateFirmwareFromFile(string filename)
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
				UpdateFirmware((uint)array.Length, array);
			}
		}
	}
}
