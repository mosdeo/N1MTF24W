using System;
using System.Collections.Generic;
using System.Linq;

namespace FwUpdateAPI
{
	internal abstract class ImageValidator
	{
		private readonly Dictionary<Sections, FwInfo.SectionDetails> _fileSections;

		private readonly SdkTbtBase _controller;

		private readonly byte[] _image;

		private readonly Dictionary<Sections, FwInfo.SectionDetails> _controllerSections;

		protected HwInfo HwInfo
		{
			get;
			private set;
		}

		protected abstract IEnumerable<CheckLocation> GetCheckLocations(HwGeneration generation);

		public static CheckLocation GetIsHostCheckLocation()
		{
			return new CheckLocation(16u, 1u, Convert.ToByte("00000010", 2), Sections.Digital, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null);
		}

		protected ImageValidator(SdkTbtBase controller, byte[] image, Dictionary<Sections, FwInfo.SectionDetails> controllerSections, Dictionary<Sections, FwInfo.SectionDetails> fileSections, HwInfo hwInfo)
		{
			_controller = controller;
			_image = image;
			_controllerSections = controllerSections;
			_fileSections = fileSections;
			HwInfo = hwInfo;
		}

		internal virtual void Validate()
		{
			ValidateChipSize();
			ComparePdExistence();
			CompareDROM();
			Compare(GetCheckLocations(HwInfo.Generation));
		}

		private void ValidateChipSize()
		{
			CheckLocation checkLocation = new CheckLocation(69u, 1u, Convert.ToByte("00000111", 2), Sections.Digital, TbtStatus.SDK_CHIP_SIZE_ERROR, null);
			byte num = ReadFromFw(checkLocation)[0];
			byte b = ReadFromFile(checkLocation)[0];
			if (num < b)
			{
				throw new TbtException(checkLocation.ErrorCode);
			}
		}

		private void ComparePdExistence()
		{
			CheckLocation checkLocation = new CheckLocation(268u, 4u, byte.MaxValue, Sections.ArcParams, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null);
			bool num = Utilities.ValidPointer(BitConverter.ToUInt32(ReadFromFw(checkLocation), 0), (int)checkLocation.Length);
			bool flag = Utilities.ValidPointer(BitConverter.ToUInt32(ReadFromFile(checkLocation), 0), (int)checkLocation.Length);
			if (num != flag)
			{
				throw new TbtException(TbtStatus.SDK_PD_MISMATCH);
			}
		}

		private void CompareDROM()
		{
			List<CheckLocation> checkLocations = new List<CheckLocation>
			{
				new CheckLocation(16u, 2u, byte.MaxValue, Sections.DROM, TbtStatus.SDK_VENDOR_MISMATCH, null),
				new CheckLocation(18u, 2u, byte.MaxValue, Sections.DROM, TbtStatus.SDK_MODEL_MISMATCH, null)
			};
			Compare(checkLocations);
		}

		private void Compare(IEnumerable<CheckLocation> checkLocations)
		{
			foreach (CheckLocation checkLocation in checkLocations)
			{
				byte[] first = ReadFromFw(checkLocation);
				byte[] second = ReadFromFile(checkLocation);
				if (!first.SequenceEqual(second))
				{
					if (checkLocation.Description != null && checkLocation.Description.Any())
					{
						throw new TbtException(checkLocation.ErrorCode, Resources.ValidationFailedPart1 + checkLocation.Description + "\n" + Resources.ValidationFailedPart2);
					}
					throw new TbtException(checkLocation.ErrorCode);
				}
			}
		}

		protected byte[] ReadFromFw(CheckLocation loc)
		{
			return ApplyMask(_controller.ReadFirmware(_controllerSections[loc.Section].Offset + loc.Offset, loc.Length), loc.Mask);
		}

		protected byte[] ReadFromFile(CheckLocation loc)
		{
			return ApplyMask(_image.Skip((int)(_fileSections[loc.Section].Offset + loc.Offset)).Take((int)loc.Length).ToArray(), loc.Mask);
		}

		private static byte[] ApplyMask(byte[] val, byte mask)
		{
			if (mask != 255)
			{
				val[0] &= mask;
			}
			return val;
		}
	}
}
