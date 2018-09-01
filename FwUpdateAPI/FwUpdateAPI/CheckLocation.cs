namespace FwUpdateAPI
{
	internal class CheckLocation
	{
		public const byte FullMask = byte.MaxValue;

		public Sections Section
		{
			get;
			private set;
		}

		public uint Offset
		{
			get;
			private set;
		}

		public uint Length
		{
			get;
			private set;
		}

		public byte Mask
		{
			get;
			private set;
		}

		public TbtStatus ErrorCode
		{
			get;
			set;
		}

		public string Description
		{
			get;
			private set;
		}

		public CheckLocation(uint offset, uint length, byte mask = byte.MaxValue, Sections section = Sections.Digital, TbtStatus errorCode = TbtStatus.SDK_IMAGE_VALIDATION_ERROR, string description = null)
		{
			Section = section;
			Offset = offset;
			Length = length;
			if (mask != 255 && length > 1)
			{
				throw new TbtException(TbtStatus.SDK_INTERNAL_ERROR, Resources.MaskIsntSupported);
			}
			Mask = mask;
			ErrorCode = errorCode;
			Description = description;
		}
	}
}
