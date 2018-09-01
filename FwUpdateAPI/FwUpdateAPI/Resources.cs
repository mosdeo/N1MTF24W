using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace FwUpdateAPI
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("FwUpdateAPI.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		public static string ChipSizeError => ResourceManager.GetString("ChipSizeError", resourceCulture);

		public static string CloseDuringUpdate => ResourceManager.GetString("CloseDuringUpdate", resourceCulture);

		public static string CommandNotFound => ResourceManager.GetString("CommandNotFound", resourceCulture);

		public static string CommandNotSupportedOnDevice => ResourceManager.GetString("CommandNotSupportedOnDevice", resourceCulture);

		public static string CommandTableError => ResourceManager.GetString("CommandTableError", resourceCulture);

		public static string DeprecatedMethod => ResourceManager.GetString("DeprecatedMethod", resourceCulture);

		public static string DeviceNeedsPowerDownMessage => ResourceManager.GetString("DeviceNeedsPowerDownMessage", resourceCulture);

		public static string DeviceNotSupported => ResourceManager.GetString("DeviceNotSupported", resourceCulture);

		public static string DromMismatchError => ResourceManager.GetString("DromMismatchError", resourceCulture);

		public static string DromNotFound => ResourceManager.GetString("DromNotFound", resourceCulture);

		public static string EPUpdateNotSupported => ResourceManager.GetString("EPUpdateNotSupported", resourceCulture);

		public static string FileNotExists => ResourceManager.GetString("FileNotExists", resourceCulture);

		public static string FWUpdateFailedMessage => ResourceManager.GetString("FWUpdateFailedMessage", resourceCulture);

		public static string FWUpdateSuccessMessage => ResourceManager.GetString("FWUpdateSuccessMessage", resourceCulture);

		public static string GetFirmwareVersionFailed => ResourceManager.GetString("GetFirmwareVersionFailed", resourceCulture);

		public static string GetPdVersionFailed => ResourceManager.GetString("GetPdVersionFailed", resourceCulture);

		public static string HostNeedsPowerDownMessage => ResourceManager.GetString("HostNeedsPowerDownMessage", resourceCulture);

		public static string I2CAccessNotSupported => ResourceManager.GetString("I2CAccessNotSupported", resourceCulture);

		public static string ImageForDeviceError => ResourceManager.GetString("ImageForDeviceError", resourceCulture);

		public static string ImageForHostError => ResourceManager.GetString("ImageForHostError", resourceCulture);

		public static string IncompatibleHWGeneration => ResourceManager.GetString("IncompatibleHWGeneration", resourceCulture);

		public static string IncompatiblePortCount => ResourceManager.GetString("IncompatiblePortCount", resourceCulture);

		public static string IncorrectArgumentCount => ResourceManager.GetString("IncorrectArgumentCount", resourceCulture);

		public static string InvalidArgument => ResourceManager.GetString("InvalidArgument", resourceCulture);

		public static string InvalidControllerID => ResourceManager.GetString("InvalidControllerID", resourceCulture);

		public static string InvalidDeviceUUID => ResourceManager.GetString("InvalidDeviceUUID", resourceCulture);

		public static string InvalidImageFile => ResourceManager.GetString("InvalidImageFile", resourceCulture);

		public static string LoadControllersFailed => ResourceManager.GetString("LoadControllersFailed", resourceCulture);

		public static string LoadDevicesFailed => ResourceManager.GetString("LoadDevicesFailed", resourceCulture);

		public static string MaskIsntSupported => ResourceManager.GetString("MaskIsntSupported", resourceCulture);

		public static string MessageBoxCaption => ResourceManager.GetString("MessageBoxCaption", resourceCulture);

		public static string MinimalValidationInSafeMode => ResourceManager.GetString("MinimalValidationInSafeMode", resourceCulture);

		public static string ModelMismatchError => ResourceManager.GetString("ModelMismatchError", resourceCulture);

		public static string MultipleImagesFound => ResourceManager.GetString("MultipleImagesFound", resourceCulture);

		public static string NativeModeMismatch => ResourceManager.GetString("NativeModeMismatch", resourceCulture);

		public static string NoCommandSupplied => ResourceManager.GetString("NoCommandSupplied", resourceCulture);

		public static string NoControllerStringPart0 => ResourceManager.GetString("NoControllerStringPart0", resourceCulture);

		public static string NoDeviceStringPart1 => ResourceManager.GetString("NoDeviceStringPart1", resourceCulture);

		public static string NoDeviceStringPart2CMD => ResourceManager.GetString("NoDeviceStringPart2CMD", resourceCulture);

		public static string NoDeviceStringPart2GUI => ResourceManager.GetString("NoDeviceStringPart2GUI", resourceCulture);

		public static string NoDromInFileError => ResourceManager.GetString("NoDromInFileError", resourceCulture);

		public static string NoDromWarning => ResourceManager.GetString("NoDromWarning", resourceCulture);

		public static string NoEPStringPart0 => ResourceManager.GetString("NoEPStringPart0", resourceCulture);

		public static string NoEPUpdateSupport => ResourceManager.GetString("NoEPUpdateSupport", resourceCulture);

		public static string NoHWInfo => ResourceManager.GetString("NoHWInfo", resourceCulture);

		public static string NoMatchingDeviceFound => ResourceManager.GetString("NoMatchingDeviceFound", resourceCulture);

		public static string OutOfControllerSection => ResourceManager.GetString("OutOfControllerSection", resourceCulture);

		public static string OutOfFileSection => ResourceManager.GetString("OutOfFileSection", resourceCulture);

		public static string PdMismatchError => ResourceManager.GetString("PdMismatchError", resourceCulture);

		public static string ReadFirmwareFailed => ResourceManager.GetString("ReadFirmwareFailed", resourceCulture);

		public static string ReadI2CFailed => ResourceManager.GetString("ReadI2CFailed", resourceCulture);

		public static string RedwoodNotSupported => ResourceManager.GetString("RedwoodNotSupported", resourceCulture);

		public static string SafeModeError => ResourceManager.GetString("SafeModeError", resourceCulture);

		public static string SafeModeWarning => ResourceManager.GetString("SafeModeWarning", resourceCulture);

		public static string ServiceDoesntExist => ResourceManager.GetString("ServiceDoesntExist", resourceCulture);

		public static string SWNotInstalled => ResourceManager.GetString("SWNotInstalled", resourceCulture);

		public static string UnknownChip => ResourceManager.GetString("UnknownChip", resourceCulture);

		public static string ValidationFailedPart1 => ResourceManager.GetString("ValidationFailedPart1", resourceCulture);

		public static string ValidationFailedPart2 => ResourceManager.GetString("ValidationFailedPart2", resourceCulture);

		public static string VendorMismatchError => ResourceManager.GetString("VendorMismatchError", resourceCulture);

		public static string WriteI2CFailed => ResourceManager.GetString("WriteI2CFailed", resourceCulture);

		internal Resources()
		{
		}
	}
}
