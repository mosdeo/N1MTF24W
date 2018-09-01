using System;
using System.Collections.Generic;
using System.Linq;

namespace FwUpdateAPI
{
	internal class HostImageValidator : ImageValidator
	{
		private class HostCheckLocation : CheckLocation
		{
			public HwType Type
			{
				get;
				private set;
			}

			public HostCheckLocation(HwType type, uint offset, uint length, string description, byte mask = byte.MaxValue, Sections section = Sections.Digital)
				: base(offset, length, mask, section, TbtStatus.SDK_IMAGE_VALIDATION_ERROR, description)
			{
				Type = type;
			}
		}

		internal HostImageValidator(SdkTbtBase controller, byte[] image, Dictionary<Sections, FwInfo.SectionDetails> controllerSections, Dictionary<Sections, FwInfo.SectionDetails> fileSections, HwInfo hwInfo)
			: base(controller, image, controllerSections, fileSections, hwInfo)
		{
		}

		protected override IEnumerable<CheckLocation> GetCheckLocations(HwGeneration generation)
		{
			CheckLocation isHostCheckLocation = ImageValidator.GetIsHostCheckLocation();
			isHostCheckLocation.ErrorCode = TbtStatus.SDK_IMAGE_FOR_DEVICE_ERROR;
			List<CheckLocation> list = new List<CheckLocation>();
			list.Add(isHostCheckLocation);
			list.AddRange(GetHostCheckLocations(generation).Where(delegate(HostCheckLocation val)
			{
				if (val.Type != HwType._1Port)
				{
					return val.Type == base.HwInfo.Type;
				}
				return true;
			}));
			return list;
		}

		private static IEnumerable<HostCheckLocation> GetHostCheckLocations(HwGeneration gen)
		{
			switch (gen)
			{
			case HwGeneration.DSL6540_6340:
			case HwGeneration.JHL6540_6340:
				return new HostCheckLocation[8]
				{
					new HostCheckLocation(HwType._1Port, 5u, 2u, "Device ID", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 16u, 4u, "PCIe Settings", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 18u, 1u, "PA", Convert.ToByte("11001100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._2Ports, 19u, 1u, "PB", Convert.ToByte("11001100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._1Port, 289u, 1u, "Snk0", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 297u, 1u, "Snk1", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 310u, 1u, "Src0", 240, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 182u, 1u, "PA/PB (USB2)", Convert.ToByte("11000000", 2), Sections.Digital)
				};
			case HwGeneration.JHL6240:
				return new HostCheckLocation[6]
				{
					new HostCheckLocation(HwType._1Port, 5u, 2u, "Device ID", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 16u, 4u, "PCIe Settings", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 18u, 1u, "PA", Convert.ToByte("11001100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._1Port, 19u, 1u, "PB", Convert.ToByte("01000100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._1Port, 289u, 1u, "Snk0", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 182u, 1u, "PA/PB (USB2)", Convert.ToByte("11000000", 2), Sections.Digital)
				};
			case HwGeneration.JHL7540_7440_7340:
				return new HostCheckLocation[10]
				{
					new HostCheckLocation(HwType._1Port, 5u, 2u, "Device ID", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 16u, 4u, "PCIe Settings", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 18u, 1u, "PA", Convert.ToByte("11001100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._2Ports, 19u, 1u, "PB", Convert.ToByte("11001100", 2), Sections.DRAMUCode),
					new HostCheckLocation(HwType._1Port, 289u, 1u, "Snk0", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 297u, 1u, "Snk1", byte.MaxValue, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 310u, 1u, "Src0", 240, Sections.Digital),
					new HostCheckLocation(HwType._1Port, 182u, 1u, "PA/PB (USB2)", Convert.ToByte("11000000", 2), Sections.Digital),
					new HostCheckLocation(HwType._1Port, 94u, 1u, "Aux", 15, Sections.Digital),
					new HostCheckLocation(HwType._2Ports, 94u, 1u, "Aux (PB)", 16, Sections.Digital)
				};
			default:
				throw new TbtException(TbtStatus.SDK_UNKNOWN_CHIP);
			}
		}
	}
}
