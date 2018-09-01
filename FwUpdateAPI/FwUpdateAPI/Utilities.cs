using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;

namespace FwUpdateAPI
{
	public static class Utilities
	{
		public class DeviceInformation
		{
			public ushort VendorId
			{
				get;
				set;
			}

			public ushort ModelId
			{
				get;
				set;
			}
		}

		public const string NA = "N/A";

		private const string ServiceName = "ThunderboltService";

		public static uint GetImageNvmVersion(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				int count = (int)fileStream.Length;
				byte[] array = new BinaryReader(fileStream).ReadBytes(count);
				uint offset = new DeviceFwInfo(new FileFwInfoSource(array)).GetSectionInfo()[Sections.Digital].Offset;
				return array[10 + offset];
			}
		}

		public static string GetImageFullNvmVersion(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				int count = (int)fileStream.Length;
				byte[] array = new BinaryReader(fileStream).ReadBytes(count);
				uint offset = new DeviceFwInfo(new FileFwInfoSource(array)).GetSectionInfo()[Sections.Digital].Offset;
				return $"{array[offset + 10]:X}.{array[offset + 9]:X2}";
			}
		}

		public static string NvmVersionToString(uint version)
		{
			return version.ToString("X");
		}

		public static string SafeGetVersion(Func<string> func)
		{
			try
			{
				return func();
			}
			catch (TbtException ex)
			{
				TbtStatus errorCode = ex.ErrorCode;
				if (errorCode != TbtStatus.INVALID_OPERATION_IN_SAFE_MODE && errorCode != TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE)
				{
					throw;
				}
				return "N/A";
			}
		}

		[Obsolete("This method is obsolete, for TI use GetImageTIPdVersion", true)]
		public static string GetImagePdVersion(string path)
		{
			return GetImageTIPdVersion(path);
		}

		public static string GetImageTIPdVersion(string path)
		{
			byte[] array = default(byte[]);
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				array = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
			}
			Dictionary<Sections, FwInfo.SectionDetails> sectionInfo = new DeviceFwInfo(new FileFwInfoSource(array)).GetSectionInfo();
			if (!sectionInfo.ContainsKey(Sections.Pd))
			{
				return "N/A";
			}
			string text;
			if (Encoding.ASCII.GetString(array.Skip((int)(sectionInfo[Sections.Pd].Offset + 48)).Take(4).ToArray()) == "ACE1")
			{
				string[] array2 = BitConverter.ToString(array.Skip((int)(sectionInfo[Sections.Pd].Offset + 52)).Take(3).ToArray(), 0).Split('-');
				text = $"{array2[1]}.{array2[0]}.{array2[2]}";
			}
			else
			{
				string @string = Encoding.ASCII.GetString(array);
				Match match = Regex.Match(@string, "TPS6598. HW.{5}FW", RegexOptions.Singleline);
				if (!match.Success)
				{
					return "N/A";
				}
				text = @string.Substring(match.Index + match.Length, 10);
			}
			text = text.TrimStart('0');
			if (text[0] == '.')
			{
				text = "0" + text;
			}
			return text;
		}

		public static string GetTIPdInfo(SdkTbtBase controller)
		{
			string text = BitConverter.ToInt32(controller.I2CRead(1u, 15u, 4u), 0).ToString("X");
			string text2 = text.Insert(text.Length - 2, ".");
			return text2.Insert(text2.Length - 5, ".");
		}

		public static DeviceInformation GetImageDeviceInformation(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				byte[] imageBuffer = binaryReader.ReadBytes((int)fileStream.Length);
				DeviceFwInfo deviceFwInfo = new DeviceFwInfo(new FileFwInfoSource(imageBuffer));
				Dictionary<Sections, FwInfo.SectionDetails> fileSections = deviceFwInfo.GetSectionInfo();
				if (!fileSections.ContainsKey(Sections.DROM))
				{
					throw new TbtException(TbtStatus.SDK_NO_DROM_IN_FILE_ERROR);
				}
				CheckLocation arg = new CheckLocation(16u, 2u, byte.MaxValue, Sections.DROM, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null);
				CheckLocation arg2 = new CheckLocation(18u, 2u, byte.MaxValue, Sections.DROM, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null);
				Func<CheckLocation, byte[]> func = (CheckLocation loc) => imageBuffer.Skip((int)(fileSections[loc.Section].Offset + loc.Offset)).Take((int)loc.Length).ToArray();
				return new DeviceInformation
				{
					VendorId = BitConverter.ToUInt16(func(arg), 0),
					ModelId = BitConverter.ToUInt16(func(arg2), 0)
				};
			}
		}

		public static bool GetImageIsHost(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
				Dictionary<Sections, FwInfo.SectionDetails> sectionInfo = new DeviceFwInfo(new FileFwInfoSource(array)).GetSectionInfo();
				CheckLocation isHostCheckLocation = ImageValidator.GetIsHostCheckLocation();
				return Convert.ToBoolean(array.Skip((int)(sectionInfo[isHostCheckLocation.Section].Offset + isHostCheckLocation.Offset)).Take((int)isHostCheckLocation.Length).ToArray()[0] & isHostCheckLocation.Mask);
			}
		}

		public static bool GetImageOsNativePciEnumerationStatus(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return new HostFwInfo(new FileFwInfoSource(new BinaryReader(fileStream).ReadBytes((int)fileStream.Length))).OsNativePciEnumeration;
			}
		}

		[Obsolete("All of the currently supported host controllers support EP update", true)]
		public static bool SupportsDeviceUpdate(string controllerId)
		{
			return true;
		}

		[Obsolete("All of the currently supported controllers support FW update", true)]
		public static bool HostUpdateSupported(string controllerId)
		{
			return true;
		}

		public static bool IsSupported(string controllerId)
		{
			try
			{
				GetHwConfiguration(controllerId);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static HwInfo GetHwConfiguration(string controllerId)
		{
			int startIndex = controllerId.IndexOf("DEV_") + "DEV_".Length;
			return FwInfoSource.HwConfiguration(ushort.Parse(controllerId.Substring(startIndex, 4), NumberStyles.AllowHexSpecifier));
		}

		internal static bool ValidPointer(uint pointer, int pointerSize)
		{
			bool flag = BitConverter.GetBytes(pointer).Take(pointerSize).All((byte b) => b == 255);
			if (pointer != 0)
			{
				return !flag;
			}
			return false;
		}

		internal static bool StartService()
		{
			ServiceController serviceController = GetServiceController();
			if (serviceController == null)
			{
				return false;
			}
			if (serviceController.Status != ServiceControllerStatus.Running)
			{
				try
				{
					serviceController.Start();
					serviceController.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(10000000L));
					return true;
				}
				catch
				{
					return false;
				}
			}
			return true;
		}

		private static ServiceController GetServiceController()
		{
			if (IsServiceExists())
			{
				return new ServiceController("ThunderboltService");
			}
			throw new TbtException(TbtStatus.SDK_SERVICE_NOT_FOUND);
		}

		private static bool IsServiceExists()
		{
			return ServiceController.GetServices().FirstOrDefault((ServiceController s) => s.ServiceName == "ThunderboltService") != null;
		}
	}
}
