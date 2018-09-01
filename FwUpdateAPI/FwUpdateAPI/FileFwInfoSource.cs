using System;
using System.Linq;

namespace FwUpdateAPI
{
	internal class FileFwInfoSource : FwInfoSource
	{
		private readonly byte[] _image;

		private const int FarbSize = 3;

		public FileFwInfoSource(byte[] image)
		{
			_image = image;
			base.Info = GetHwConfiguration();
		}

		public override byte[] Read(uint offset, uint length)
		{
			return new ArraySegment<byte>(_image, (int)offset, (int)length).ToArray();
		}

		public override uint DigitalSectionOffset()
		{
			uint[] array = new uint[2]
			{
				0u,
				4096u
			};
			foreach (uint offset in array)
			{
				uint farb = GetFarb(offset);
				if (Utilities.ValidPointer(farb, 3))
				{
					return farb;
				}
			}
			throw new TbtException(TbtStatus.SDK_INVALID_IMAGE_FILE);
		}

		private uint GetFarb(uint offset)
		{
			byte[] array = Read(offset, 3u);
			Array.Resize(ref array, 4);
			return BitConverter.ToUInt32(array, 0);
		}
	}
}
