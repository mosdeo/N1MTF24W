using FwUpdateAPI;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace FwUpdateTool.WizardSteps
{
	public class SelectController : UserControl, IWizardScreen, INotifyPropertyChanged, IComponentConnector
	{
		internal TreeView DevicesTree;

		private bool _contentLoaded;

		public bool CancelButtonActive => true;

		public bool NextButtonActive => DevicesTree.SelectedItem != null;

		public bool BackButtonActive => true;

		public string[] Title => new string[2]
		{
			"Select",
			"Controller/Device"
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public SelectController(List<SdkTbtController> controllers, List<SdkTbtDevice> devices)
		{
			InitializeComponent();
			List<Controller> list = new List<Controller>();
			foreach (SdkTbtController controller in controllers)
			{
				SdkTbtController localController = controller;
				IEnumerable<SdkTbtDevice> devices2 = from device in devices
				where device.ControllerId == localController.ControllerId
				select device;
				list.Add(new Controller(controller, devices2));
			}
			DevicesTree.ItemsSource = list;
		}

		private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
			if (mainWindow != null)
			{
				mainWindow.CurrentController = ((ISdk)DevicesTree.SelectedItem).SdkHandle;
				OnPropertyChanged("NextButtonActive");
			}
		}

		private void OnPropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/selectcontroller.xaml", UriKind.Relative);
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
				DevicesTree = (TreeView)target;
				DevicesTree.SelectedItemChanged += OnSelectedItemChanged;
			}
			else
			{
				_contentLoaded = true;
			}
		}
	}
}
