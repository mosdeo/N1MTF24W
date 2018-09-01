using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FwUpdateTool.WizardSteps
{
	public class ConfirmationScreen : UserControl, IWizardScreen, INotifyPropertyChanged, IComponentConnector
	{
		private bool _isHost;

		private string _nvmVersion;

		private string _nvmFileVersion;

		private string _pdVersion;

		private string _pdFileVersion;

		private bool? _osNativePciEnumeration;

		private bool _osNativePciEnumerationFile;

		internal Button StartButton;

		private bool _contentLoaded;

		public bool IsHost
		{
			get
			{
				return _isHost;
			}
			set
			{
				_isHost = value;
				OnPropertyChanged("IsHost");
			}
		}

		public string NvmVersion
		{
			get
			{
				return _nvmVersion;
			}
			set
			{
				_nvmVersion = value;
				OnPropertyChanged("NvmVersion");
			}
		}

		public string NvmFileVersion
		{
			get
			{
				return _nvmFileVersion;
			}
			set
			{
				_nvmFileVersion = value;
				OnPropertyChanged("NvmFileVersion");
			}
		}

		public string PdVersion
		{
			get
			{
				return _pdVersion;
			}
			set
			{
				_pdVersion = value;
				OnPropertyChanged("PdVersion");
			}
		}

		public string PdFileVersion
		{
			get
			{
				return _pdFileVersion;
			}
			set
			{
				_pdFileVersion = value;
				OnPropertyChanged("PdFileVersion");
			}
		}

		public bool? OsNativePciEnumeration
		{
			get
			{
				return _osNativePciEnumeration;
			}
			set
			{
				_osNativePciEnumeration = value;
				OnPropertyChanged("OsNativePciEnumeration");
			}
		}

		public bool OsNativePciEnumerationFile
		{
			get
			{
				return _osNativePciEnumerationFile;
			}
			set
			{
				_osNativePciEnumerationFile = value;
				OnPropertyChanged("OsNativePciEnumerationFile");
			}
		}

		public bool CancelButtonActive => true;

		public bool NextButtonActive => false;

		public bool BackButtonActive => true;

		public string[] Title => new string[1]
		{
			"Confirmation"
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public ConfirmationScreen()
		{
			InitializeComponent();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			(Application.Current.MainWindow as MainWindow)?.NextButton_Click(sender, e);
		}

		private void OnPropertyChanged(string name)
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
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/confirmationscreen.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				StartButton = (Button)target;
				StartButton.Click += StartButton_Click;
			}
			else
			{
				_contentLoaded = true;
			}
		}
	}
}
