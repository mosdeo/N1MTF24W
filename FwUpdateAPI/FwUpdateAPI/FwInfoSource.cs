using System;

namespace FwUpdateAPI
{
	public abstract class FwInfoSource
	{
		public HwInfo Info
		{
			get;
			protected set;
		}

		public abstract byte[] Read(uint offset, uint length);

		public abstract uint DigitalSectionOffset();

		public static HwInfo HwConfiguration(ushort deviceId)
		{
			if ((uint)deviceId <= 5496u)
			{
				if ((uint)(deviceId - 5493) <= 1u)
				{
					return new HwInfo
					{
						Generation = HwGeneration.DSL6540_6340,
						Type = new HwType?(HwType._1Port)
					};
				}
				if ((uint)(deviceId - 5495) <= 1u)
				{
					return new HwInfo
					{
						Generation = HwGeneration.DSL6540_6340,
						Type = new HwType?(HwType._2Ports)
					};
				}
			}
			else
			{
				if ((uint)(deviceId - 5567) <= 1u)
				{
					return new HwInfo
					{
						Generation = HwGeneration.JHL6240,
						Type = new HwType?(HwType._1Port)
					};
				}
				switch (deviceId)
				{
				case 5597:
					return new HwInfo
					{
						Generation = HwGeneration.DSL6540_6340
					};
				case 5596:
					return new HwInfo
					{
						Generation = HwGeneration.JHL6240
					};
				case 5586:
				case 5587:
					return new HwInfo
					{
						Generation = HwGeneration.JHL6540_6340,
						Type = new HwType?(HwType._2Ports)
					};
				case 5593:
				case 5594:
					return new HwInfo
					{
						Generation = HwGeneration.JHL6540_6340,
						Type = new HwType?(HwType._1Port)
					};
				case 5598:
					return new HwInfo
					{
						Generation = HwGeneration.JHL6540_6340
					};
				case 5610:
				case 5611:
				case 5615:
					return new HwInfo
					{
						Generation = HwGeneration.JHL7540_7440_7340,
						Type = new HwType?(HwType._2Ports)
					};
				case 5607:
				case 5608:
					return new HwInfo
					{
						Generation = HwGeneration.JHL7540_7440_7340,
						Type = new HwType?(HwType._1Port)
					};
				}
			}
			throw new TbtException(TbtStatus.SDK_UNKNOWN_CHIP);
		}

		protected HwInfo GetHwConfiguration()
		{
			FwLocation fwLocation = new FwLocation
			{
				Offset = DigitalSectionOffset() + 5,
				Length = 2
			};
			return HwConfiguration(BitConverter.ToUInt16(Read(fwLocation.Offset, fwLocation.Length), 0));
		}
	}
}
