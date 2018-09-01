using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FwUpdateTool.WizardSteps
{
	public partial class SelectImageScreen : UserControl, IWizardScreen, INotifyPropertyChanged, IComponentConnector
	{
		internal Button BrowseButton;

		internal TextBox FilePath;

		//private bool _contentLoaded;

		public bool CancelButtonActive => true;

		public bool NextButtonActive => true;

		public bool BackButtonActive => true;

		public string[] Title => new string[1]
		{
			"Select FW Image"
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public SelectImageScreen()
		{
			InitializeComponent();
            InitializeComponentsCustom();
        }

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				FilePath.Text = openFileDialog.FileName;
			}
		}

		private void OnContentChanged(object sender, TextChangedEventArgs e)
		{
			MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
			if (mainWindow != null)
			{
				mainWindow.ImageFileName = FilePath.Text;
			}
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
		public void InitializeComponentsCustom()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/selectimagescreen.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				BrowseButton = (Button)target;
				BrowseButton.Click += BrowseButton_Click;
				break;
			case 2:
				FilePath = (TextBox)target;
				FilePath.TextChanged += OnContentChanged;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
