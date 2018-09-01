using FwUpdateAPI;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace FwUpdateTool.WizardSteps
{
	public partial class FwUpdateProcessScreen : UserControl, IWizardScreen, INotifyPropertyChanged, IDisposable, IComponentConnector
	{
		private readonly DoubleAnimation _ani = new DoubleAnimation(1.0, 0.3, TimeSpan.FromMilliseconds(1000.0));

		private readonly FlashProgress _flashProgress = new FlashProgress();

		internal Image Logo;

		//private bool _contentLoaded;

		public bool CancelButtonActive => false;

		public bool NextButtonActive => false;

		public bool BackButtonActive => false;

		public string[] Title => new string[1]
		{
			"Flashing..."
		};

		public event PropertyChangedEventHandler PropertyChanged;

		public FwUpdateProcessScreen()
		{
			InitializeComponent();
			InitializeFlashAnimation();
			base.DataContext = _flashProgress;
		}

		private void InitializeFlashAnimation()
		{
			_ani.AutoReverse = true;
			_ani.Completed += ani_Completed;
			Logo.BeginAnimation(UIElement.OpacityProperty, _ani);
		}

		private void ani_Completed(object sender, EventArgs e)
		{
			Logo.BeginAnimation(UIElement.OpacityProperty, _ani);
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

		public void Dispose()
		{
			_flashProgress.Dispose();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/wizardsteps/fwupdateprocessscreen.xaml", UriKind.Relative);
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
				Logo = (Image)target;
			}
			else
			{
				_contentLoaded = true;
			}
		}
	}
}
