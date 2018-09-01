using System;
using System.Collections.Generic;

namespace FwUpdateAPI
{
	public class TbtException : Exception
	{
		private static readonly Dictionary<TbtStatus, string> SdkStatusMessages = new Dictionary<TbtStatus, string>
		{
			{
				TbtStatus.SDK_NO_COMMAND_SUPPLIED,
				Resources.NoCommandSupplied
			},
			{
				TbtStatus.SDK_COMMAND_NOT_FOUND,
				Resources.CommandNotFound
			},
			{
				TbtStatus.SDK_ARGUMENT_COUNT_MISMATCH,
				Resources.IncorrectArgumentCount
			},
			{
				TbtStatus.SDK_INVALID_CONTROLLER_ID,
				Resources.InvalidControllerID
			},
			{
				TbtStatus.SDK_INVALID_DEVICE_UUID,
				Resources.InvalidDeviceUUID
			},
			{
				TbtStatus.SDK_FILE_NOT_FOUND,
				Resources.FileNotExists
			},
			{
				TbtStatus.SDK_SERVICE_NOT_FOUND,
				Resources.ServiceDoesntExist
			},
			{
				TbtStatus.SDK_LOAD_CONTROLLERS_ERROR,
				Resources.LoadControllersFailed
			},
			{
				TbtStatus.SDK_LOAD_DEVICES_ERROR,
				Resources.LoadDevicesFailed
			},
			{
				TbtStatus.SDK_INVALID_OPERATION_IN_SAFE_MODE,
				Resources.SafeModeError
			},
			{
				TbtStatus.SDK_DEVICE_NOT_SUPPORTED,
				Resources.DeviceNotSupported
			},
			{
				TbtStatus.SDK_UNKNOWN_CHIP,
				Resources.UnknownChip
			},
			{
				TbtStatus.SDK_INVALID_IMAGE_FILE,
				Resources.InvalidImageFile
			},
			{
				TbtStatus.SDK_HW_GENERATION_MISMATCH,
				Resources.IncompatibleHWGeneration
			},
			{
				TbtStatus.SDK_PORT_COUNT_MISMATCH,
				Resources.IncompatiblePortCount
			},
			{
				TbtStatus.SDK_CHIP_SIZE_ERROR,
				Resources.ChipSizeError
			},
			{
				TbtStatus.SDK_IMAGE_FOR_HOST_ERROR,
				Resources.ImageForHostError
			},
			{
				TbtStatus.SDK_IMAGE_FOR_DEVICE_ERROR,
				Resources.ImageForDeviceError
			},
			{
				TbtStatus.SDK_PD_MISMATCH,
				Resources.PdMismatchError
			},
			{
				TbtStatus.SDK_NO_DROM_IN_FILE_ERROR,
				Resources.NoDromInFileError
			},
			{
				TbtStatus.SDK_VENDOR_MISMATCH,
				Resources.VendorMismatchError
			},
			{
				TbtStatus.SDK_MODEL_MISMATCH,
				Resources.ModelMismatchError
			},
			{
				TbtStatus.SDK_NO_MATCHING_DEVICE_FOUND,
				Resources.NoMatchingDeviceFound
			},
			{
				TbtStatus.SDK_MULTIPLE_IMAGES_FOUND,
				Resources.MultipleImagesFound
			},
			{
				TbtStatus.SDK_COMMAND_IS_NOT_SUPPORTED_ON_DEVICE,
				Resources.CommandNotSupportedOnDevice
			},
			{
				TbtStatus.SDK_DEPRECATED_METHOD,
				Resources.DeprecatedMethod
			},
			{
				TbtStatus.SDK_INVALID_ARGUMENT,
				Resources.InvalidArgument
			},
			{
				TbtStatus.SDK_NO_DROM_FOUND,
				Resources.DromNotFound
			},
			{
				TbtStatus.SDK_NATIVE_MODE_MISMATCH,
				Resources.NativeModeMismatch
			}
		};

		public TbtStatus ErrorCode
		{
			get;
			private set;
		}

		public string TbtMessage
		{
			get;
			private set;
		}

		public TbtException(TbtStatus code, string message)
			: base(ErrorMessage(code) + " " + message)
		{
			ErrorCode = code;
			TbtMessage = message;
		}

		public TbtException(TbtStatus code)
			: base(ErrorMessage(code) + (SdkStatusMessages.ContainsKey(code) ? (" " + SdkStatusMessages[code]) : ""))
		{
			ErrorCode = code;
			if (SdkStatusMessages.ContainsKey(code))
			{
				TbtMessage = SdkStatusMessages[code];
			}
		}

		public static string ErrorMessage(TbtStatus code)
		{
			return $"Error: 0x{(int)code:X} {code}";
		}
	}
}
