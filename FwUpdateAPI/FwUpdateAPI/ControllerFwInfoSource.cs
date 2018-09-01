namespace FwUpdateAPI
{
	internal class ControllerFwInfoSource : FwInfoSource
	{
		private readonly SdkTbtBase _controller;

		public ControllerFwInfoSource(SdkTbtBase controller)
		{
			_controller = controller;
			base.Info = GetHwConfiguration();
		}

		public override byte[] Read(uint offset, uint length)
		{
			return _controller.ReadFirmware(offset, length);
		}

		public override uint DigitalSectionOffset()
		{
			return 0u;
		}
	}
}
