using FwUpdateAPI;

namespace FwUpdateTool.WizardSteps
{
	internal class Device : ISdk
	{
		private readonly SdkTbtDevice _sdkHandle;

		public SdkTbtBase SdkHandle => _sdkHandle;

		public string DisplayName
		{
			get;
			private set;
		}

		public bool IsSelectable
		{
			get;
			private set;
		}

		public Device(SdkTbtDevice sdkHandle)
		{
			_sdkHandle = sdkHandle;
			DisplayName = $"{_sdkHandle.VendorName}, {_sdkHandle.ModelName}";
			if (_sdkHandle.NumOfControllers > 1)
			{
				DisplayName += $": {_sdkHandle.ControllerNum}/{_sdkHandle.NumOfControllers}";
			}
			IsSelectable = (_sdkHandle.Updatable && Utilities.IsSupported(_sdkHandle.ControllerId));
		}
	}
}
