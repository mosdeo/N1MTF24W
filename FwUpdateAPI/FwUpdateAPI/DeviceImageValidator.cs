using System;
using System.Collections.Generic;

namespace FwUpdateAPI
{
	internal class DeviceImageValidator : ImageValidator
	{
		internal DeviceImageValidator(SdkTbtBase controller, byte[] image, Dictionary<Sections, FwInfo.SectionDetails> controllerSections, Dictionary<Sections, FwInfo.SectionDetails> fileSections, HwInfo hwInfo)
			: base(controller, image, controllerSections, fileSections, hwInfo)
		{
		}

		protected override IEnumerable<CheckLocation> GetCheckLocations(HwGeneration generation)
		{
			CheckLocation isHostCheckLocation = ImageValidator.GetIsHostCheckLocation();
			isHostCheckLocation.ErrorCode = TbtStatus.SDK_IMAGE_FOR_HOST_ERROR;
			if ((uint)(generation - 3) <= 2u)
			{
				return new CheckLocation[2]
				{
					isHostCheckLocation,
					new CheckLocation(292u, 1u, byte.MaxValue, Sections.ArcParams, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, "X of N")
				};
			}
			if (generation == HwGeneration.JHL7540_7440_7340)
			{
				return new CheckLocation[1]
				{
					isHostCheckLocation
				};
			}
			throw new TbtException(TbtStatus.SDK_UNKNOWN_CHIP);
		}

		internal override void Validate()
		{
			base.Validate();
			if (base.HwInfo.Generation == HwGeneration.JHL7540_7440_7340)
			{
				CheckMultipleController();
			}
		}

		private void CheckMultipleController()
		{
			CheckLocation loc = new CheckLocation(14u, 2u, byte.MaxValue, Sections.DROM, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null);
			byte[] lengthBytes = ReadFromFile(loc);
			byte[] lengthBytes2 = ReadFromFw(loc);
			uint length = ExtractLength(lengthBytes);
			uint length2 = ExtractLength(lengthBytes2);
			byte[] drom = ReadFromFile(new CheckLocation(0u, length, byte.MaxValue, Sections.DROM, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null));
			byte[] drom2 = ReadFromFw(new CheckLocation(0u, length2, byte.MaxValue, Sections.DROM, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, null));
			if (GetMcData(drom) != GetMcData(drom2))
			{
				throw new TbtException(TbtStatus.SDK_IMAGE_VALIDATION_ERROR, Resources.ValidationFailedPart1 + "X of N\n" + Resources.ValidationFailedPart2);
			}
		}

		private uint ExtractLength(byte[] lengthBytes)
		{
			return GetBitField(BitConverter.ToUInt16(lengthBytes, 0), 0, 12) + 13;
		}

		private int? GetMcData(byte[] drom)
		{
			for (int i = 22; i < drom.Length; i += drom[i])
			{
				byte field = drom[i + 1];
				if (!Convert.ToBoolean(GetBitField(field, 7, 1)) && GetBitField(field, 0, 6) == 6)
				{
					return drom[i + 2];
				}
			}
			return null;
		}

		private uint GetBitField(uint field, int offset, int size)
		{
			field >>= offset;
			int num = (1 << size) - 1;
			return (uint)(field & num);
		}
	}
}
