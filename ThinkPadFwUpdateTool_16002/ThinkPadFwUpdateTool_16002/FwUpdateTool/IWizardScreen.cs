namespace FwUpdateTool
{
	public interface IWizardScreen
	{
		bool CancelButtonActive
		{
			get;
		}

		bool NextButtonActive
		{
			get;
		}

		bool BackButtonActive
		{
			get;
		}

		string[] Title
		{
			get;
		}
	}
}
