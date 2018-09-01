using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FwUpdateTool.WizardSteps
{
	public partial class UpdateCompletedScreen : UserControl, IWizardScreen, INotifyPropertyChanged, IComponentConnector
	{
		internal TextBlock ResultMessage;

		//private bool _contentLoaded;

		public bool CancelButtonActive => false;

		public bool NextButtonActive => false;

		public bool BackButtonActive => false;

		public string[] Title => new string[1]
		{
			"Done"
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public UpdateCompletedScreen()
		{
			InitializeComponent();
            InitializeComponentsCustom();
        }

		public void OnPropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected void OnPageChange()
		{
			OnPropertyChanged("CancelButtonActive");
			OnPropertyChanged("NextButtonActive");
			OnPropertyChanged("BackButtonActive");
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponentsCustom()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/updatecompletedscreen.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				ResultMessage = (TextBlock)target;
			}
			else
			{
				_contentLoaded = true;
			}
		}
	}
}
