using System;
using System.Collections.Generic;

namespace FwUpdateAPI
{
	internal abstract class FwInfo
	{
		public class SectionDetails
		{
			public uint Offset
			{
				get;
				set;
			}

			public uint Length
			{
				get;
				set;
			}
		}

		private const uint DW = 4u;

		private const uint SizeOfSizeField = 2u;

		protected FwInfoSource Source
		{
			get;
			private set;
		}

		public HwInfo Info => Source.Info;

		protected FwInfo(FwInfoSource source)
		{
			Source = source;
		}

		public virtual Dictionary<Sections, SectionDetails> GetSectionInfo()
		{
			if (Source.Info == null)
			{
				throw new TbtException(TbtStatus.SDK_INTERNAL_ERROR, Resources.NoHWInfo);
			}
			Dictionary<Sections, SectionDetails> dictionary = new Dictionary<Sections, SectionDetails>();
			dictionary[Sections.Digital] = new SectionDetails
			{
				Offset = Source.DigitalSectionOffset()
			};
			dictionary[Sections.Digital].Length = SectionSize16Bit(dictionary[Sections.Digital].Offset);
			FwLocation fwLocation = new FwLocation
			{
				Offset = 117,
				Length = 4
			};
			uint num = BitConverter.ToUInt32(Source.Read(dictionary[Sections.Digital].Offset + fwLocation.Offset, fwLocation.Length), 0);
			dictionary[Sections.ArcParams] = new SectionDetails
			{
				Offset = dictionary[Sections.Digital].Offset + num
			};
			dictionary[Sections.ArcParams].Length = BitConverter.ToUInt32(Source.Read(dictionary[Sections.ArcParams].Offset, 4u), 0) * 4;
			FwLocation fwLocation2 = new FwLocation
			{
				Offset = 199,
				Length = 3
			};
			byte[] array = Source.Read(dictionary[Sections.Digital].Offset + fwLocation2.Offset, fwLocation2.Length);
			Array.Resize(ref array, 4);
			uint num2 = BitConverter.ToUInt32(array, 0);
			dictionary[Sections.Ee2TarDma] = new SectionDetails
			{
				Offset = dictionary[Sections.Digital].Offset + num2
			};
			uint num3 = BitConverter.ToUInt32(Source.Read((uint)(dictionary[Sections.ArcParams].Offset + ((Info.Generation < HwGeneration.JHL7540_7440_7340) ? 268 : 332)), 4u), 0);
			if (Utilities.ValidPointer(num3, 4))
			{
				dictionary[Sections.Pd] = new SectionDetails
				{
					Offset = num3 + dictionary[Sections.Digital].Offset
				};
			}
			FwLocation fwLocation3 = new FwLocation
			{
				Offset = 270,
				Length = 4
			};
			uint num4 = BitConverter.ToUInt32(Source.Read(dictionary[Sections.Digital].Offset + fwLocation3.Offset, fwLocation3.Length), 0);
			if (num4 == 0)
			{
				throw new TbtException(TbtStatus.SDK_NO_DROM_FOUND);
			}
			dictionary[Sections.DROM] = new SectionDetails
			{
				Offset = dictionary[Sections.Digital].Offset + num4
			};
			dictionary[Sections.DROM].Length = BitConverter.ToUInt32(Source.Read(dictionary[Sections.DROM].Offset, 4u), 0) * 4;
			return dictionary;
		}

		private uint SectionSize16Bit(uint sectionOffset)
		{
			return (uint)(BitConverter.ToUInt16(Source.Read(sectionOffset, 2u), 0) + 2);
		}

		protected uint SectionSize16BitDw(uint sectionOffset)
		{
			return (SectionSize16Bit(sectionOffset) - 2) * 4 + 2;
		}
	}
}
