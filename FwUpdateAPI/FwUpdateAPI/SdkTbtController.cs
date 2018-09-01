using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

namespace FwUpdateAPI
{
	public class SdkTbtController : SdkTbtBase
	{
		private Dictionary<Sections, FwInfo.SectionDetails> _sections;

		private string _vendorId;

		private string _modelId;

		private string _modelRevision;

		private string _nvmRevision;

		private string _customizedTiVersion;

		internal Dictionary<Sections, FwInfo.SectionDetails> Section => _sections ?? (_sections = new HostFwInfo(new ControllerFwInfoSource(this)).GetSectionInfo());

		public override string GetNeedPowerDownMessage => Resources.HostNeedsPowerDownMessage;

		public string ControllerId => base.Mo.GetPropertyValue("ControllerId").ToString();

		public uint Generation => (uint)base.Mo.GetPropertyValue("Generation");

		public ushort DeviceId => Convert.ToUInt16(base.Mo.GetPropertyValue("DeviceId"));

		public string SecurityLevel
		{
			get
			{
				if (IsInSafeMode)
				{
					return "N/A";
				}
				return ((uint)base.Mo.GetPropertyValue("SecurityLevel")).ToString();
			}
		}

		public string SupportsExternalGpu
		{
			get
			{
				if (IsInSafeMode)
				{
					return "N/A";
				}
				return ((bool)base.Mo.GetPropertyValue("SupportsExternalGpu")).ToString();
			}
		}

		public bool IsInSafeMode => (bool)base.Mo.GetPropertyValue("IsInSafeMode");

		public string VendorID
		{
			get
			{
				try
				{
					return _vendorId ?? (_vendorId = string.Format("0x{0:D4}", BitConverter.ToUInt16(ReadFirmware(Section[Sections.DROM].Offset + 16, 2u), 0).ToString("X")));
				}
				catch
				{
					return "N/A";
				}
			}
		}

		public string ModelId
		{
			get
			{
				try
				{
					return _modelId ?? (_modelId = string.Format("0x{0:D4}", BitConverter.ToUInt16(ReadFirmware(Section[Sections.DROM].Offset + 18, 2u), 0).ToString("X")));
				}
				catch
				{
					return "N/A";
				}
			}
		}

		public string ModelRevision
		{
			get
			{
				try
				{
					return _modelRevision ?? (_modelRevision = ReadFirmware(Section[Sections.DROM].Offset + 20, 1u)[0].ToString());
				}
				catch
				{
					return "N/A";
				}
			}
		}

		public string NVMRevision
		{
			get
			{
				try
				{
					return _nvmRevision ?? (_nvmRevision = ReadFirmware(Section[Sections.DROM].Offset + 21, 1u)[0].ToString());
				}
				catch
				{
					return "N/A";
				}
			}
		}

		public string CustomizedTIVersion
		{
			get
			{
				try
				{
					return _customizedTiVersion ?? (_customizedTiVersion = BitConverter.ToUInt16(ReadFirmware(Section[Sections.Ee2TarDma].Offset + 58, 2u), 0).ToString("X"));
				}
				catch
				{
					return "N/A";
				}
			}
		}

		public bool OsNativePciEnumeration => (bool)base.Mo.GetPropertyValue("OsNativePciEnumeration");

		public bool Rtd3Capable => (bool)base.Mo.GetPropertyValue("RTD3Capable");

		public SdkTbtController(ManagementObject mo)
			: base(mo)
		{
		}

		public static Dictionary<string, SdkTbtController> GetControllersFromWmi()
		{
			Utilities.StartService();
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\Intel\\Thunderbolt", "SELECT * FROM SdkTbtController"))
			{
				Dictionary<string, SdkTbtController> dictionary = new Dictionary<string, SdkTbtController>();
				foreach (ManagementObject item in managementObjectSearcher.Get().Cast<ManagementObject>())
				{
					SdkTbtController sdkTbtController = new SdkTbtController(item);
					dictionary.Add(sdkTbtController.ControllerId, sdkTbtController);
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
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] image = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
				HostFwInfo hostFwInfo = new HostFwInfo(new FileFwInfoSource(image));
				if (IsInSafeMode)
				{
					HwInfo hwInfo = FwInfoSource.HwConfiguration(DeviceId);
					if (hwInfo.Generation != hostFwInfo.Info.Generation)
					{
						throw new TbtException(TbtStatus.SDK_HW_GENERATION_MISMATCH);
					}
					if (hwInfo.Type != hostFwInfo.Info.Type)
					{
						throw new TbtException(TbtStatus.SDK_PORT_COUNT_MISMATCH);
					}
					if (!Utilities.GetImageIsHost(path))
					{
						throw new TbtException(TbtStatus.SDK_IMAGE_FOR_DEVICE_ERROR);
					}
				}
				else
				{
					HostFwInfo hostFwInfo2 = new HostFwInfo(new ControllerFwInfoSource(this));
					HostFwInfo hostFwInfo3 = new HostFwInfo(new FileFwInfoSource(image));
					if (hostFwInfo2.Info.Generation != hostFwInfo3.Info.Generation)
					{
						throw new TbtException(TbtStatus.SDK_HW_GENERATION_MISMATCH);
					}
					if (hostFwInfo2.Info.Type != hostFwInfo3.Info.Type)
					{
						throw new TbtException(TbtStatus.SDK_PORT_COUNT_MISMATCH);
					}
					new HostImageValidator(this, image, hostFwInfo2.GetSectionInfo(), hostFwInfo3.GetSectionInfo(), hostFwInfo2.Info).Validate();
					if (OsNativePciEnumeration != hostFwInfo3.OsNativePciEnumeration)
					{
						throw new TbtException(TbtStatus.SDK_NATIVE_MODE_MISMATCH);
					}
				}
			}
		}

		public override uint GetCurrentNvmVersion()
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			return base.GetCurrentNvmVersion();
		}

		public override string GetCurrentFullNvmVersion()
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			return base.GetCurrentFullNvmVersion();
		}

		public override byte[] I2CRead(uint port, uint offset, uint length)
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			return base.I2CRead(port, offset, length);
		}

		public override void I2CWrite(uint port, uint offset, byte[] data)
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			base.I2CWrite(port, offset, data);
		}

		[Obsolete("This method is obsolete, use I2CRead", true)]
		public override string GetCurrentPdVersion()
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			return "N/A";
		}

		public override byte[] ReadFirmware(uint offset, uint length)
		{
			if (IsInSafeMode)
			{
				throw new TbtException(TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE);
			}
			return base.ReadFirmware(offset, length);
		}
	}
}
