using System;
using System.Collections.Generic;

namespace FwUpdateAPI
{
	internal class HostFwInfo : FwInfo
	{
		[Flags]
		private enum SectionReadBit
		{
			DPIN = 0x1,
			DPOUT = 0x2,
			CP = 0x4,
			ARC = 0x8,
			LC = 0x10,
			IRAM = 0x20,
			DRAM = 0x40
		}

		public bool OsNativePciEnumeration => (base.Source.Read(base.GetSectionInfo()[Sections.Digital].Offset + 123, 1u)[0] & 0x20) != 0;

		public HostFwInfo(FwInfoSource source)
			: base(source)
		{
		}

		public override Dictionary<Sections, SectionDetails> GetSectionInfo()
		{
			Dictionary<Sections, SectionDetails> sectionInfo = base.GetSectionInfo();
			HwGeneration generation = base.Source.Info.Generation;
			if ((uint)(generation - 3) > 3u)
			{
				throw new TbtException(TbtStatus.SDK_UNKNOWN_CHIP);
			}
			FwLocation fwLocation = new FwLocation
			{
				Offset = sectionInfo[Sections.Digital].Offset + 2,
				Length = 1
			};
			SectionReadBit sectionReadBit = (SectionReadBit)base.Source.Read(fwLocation.Offset, fwLocation.Length)[0];
			if (sectionReadBit.HasFlag(SectionReadBit.DRAM))
			{
				var obj = new[]
				{
					new
					{
						name = "CP",
						flag = SectionReadBit.CP
					},
					new
					{
						name = "HDPOut",
						flag = SectionReadBit.DPOUT
					},
					new
					{
						name = "HDPIn",
						flag = SectionReadBit.DPIN
					},
					new
					{
						name = "LC",
						flag = SectionReadBit.LC
					},
					new
					{
						name = "ARC",
						flag = SectionReadBit.ARC
					},
					new
					{
						name = "IRAM",
						flag = SectionReadBit.IRAM
					}
				};
				FwLocation fwLocation2 = new FwLocation
				{
					Offset = sectionInfo[Sections.Digital].Offset + 3,
					Length = 2
				};
				uint num = sectionInfo[Sections.Digital].Offset + BitConverter.ToUInt16(base.Source.Read(fwLocation2.Offset, fwLocation2.Length), 0);
				var array = obj;
				foreach (var anon in array)
				{
					if (sectionReadBit.HasFlag(anon.flag))
					{
						uint num2 = SectionSize16BitDw(num);
						num += num2;
					}
				}
				sectionInfo[Sections.DRAMUCode] = new SectionDetails
				{
					Offset = num
				};
				sectionInfo[Sections.DRAMUCode].Length = SectionSize16BitDw(sectionInfo[Sections.DRAMUCode].Offset);
			}
			return sectionInfo;
		}
	}
}
