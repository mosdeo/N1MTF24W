using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FwUpdateTool.WizardSteps
{
	public class WelcomeScreen : UserControl, IWizardScreen, INotifyPropertyChanged, IComponentConnector
	{
		private bool _contentLoaded;

		public bool CancelButtonActive => true;

		public bool NextButtonActive => true;

		public bool BackButtonActive => false;

		public string[] Title => new string[1]
		{
			"Welcome"
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public WelcomeScreen()
		{
			InitializeComponent();
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
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/welcomescreen.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			_contentLoaded = true;
		}
	}
}
