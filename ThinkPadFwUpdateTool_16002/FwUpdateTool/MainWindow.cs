using Battery;
using FwUpdateAPI;
using FwUpdateTool.WizardSteps;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using TBT_Control;

namespace FwUpdateTool
{
	public partial class MainWindow : Window, INotifyPropertyChanged, IComponentConnector
	{
		private class InitException : Exception
		{
		}

		private readonly List<IWizardScreen> _pages = new List<IWizardScreen>();

		private Task _flashTask;

		private int _currentScreen;

		private bool _isDuringFwUpdate;

		public string ERROR1;

		public string Restart = "";

		public string Silent = "";

		public string Change = "";

		public string FileName;

		internal System.Windows.Controls.Button CancelButton;

		internal System.Windows.Controls.Button NextButton;

		internal System.Windows.Controls.Button BackButton;

		internal ContentControl PageContent;

		internal StackPanel Steps;

		private bool _contentLoaded;

		public string ImageFileName
		{
			private get;
			set;
		}

		public SdkTbtBase CurrentController
		{
			private get;
			set;
		}

		private string FlashingResult
		{
			get;
			set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public int PM_CHECK()
		{
			try
			{
				string text = "1.67.12.19";
				string[] array = new string[7];
				string[] array2 = new string[7];
				array2 = FileVersionInfo.GetVersionInfo("C:\\Windows\\Sysnative\\drivers\\ibmpmdrv.sys").FileVersion.Split('.');
				array = text.Split('.');
				if (int.Parse(array2[0]) <= int.Parse(array[0]))
				{
					if (int.Parse(array2[1]) <= int.Parse(array[1]))
					{
						if (int.Parse(array2[2]) <= int.Parse(array[2]))
						{
							if (int.Parse(array2[3]) <= int.Parse(array[3]) && int.Parse(array2[3]) != int.Parse(array[3]))
							{
								Console.WriteLine("PM Driver version is not the latest one,please update the PM driver first. ");
								Thread.Sleep(5000);
								return 3000;
							}
							Console.WriteLine("PM Driver version is OK. ");
							return 0;
						}
						Console.WriteLine("PM Driver version is OK. ");
						return 0;
					}
					Console.WriteLine("PM Driver version is OK. ");
					return 0;
				}
				Console.WriteLine("PM Driver version is OK. ");
				return 0;
			}
			catch (Exception)
			{
				Console.WriteLine("PM Driver version is not the latest one,please update the PM driver first. ");
				Thread.Sleep(5000);
				return 3000;
			}
		}

		public void start_window()
		{
			switch (System.Windows.MessageBox.Show("Before continuing please ensure the following:\nMake sure that the AC adapter is firmly connected to the system and outlet.\nMake sure that a charged battery pack is installed in the system.\nSave all open files and close all open applications.\nThe system will be rebooted after the firmware update process is completed.\nPlease do not suspend or hibernate your system while updating the Thunderbolt Firmware.\nPlease do not plug the Type C or Thunderbolt device to your system while updating the Thunderbolt Firmware.\n", "Thunderbolt Firmware Update", MessageBoxButton.YesNo, MessageBoxImage.Asterisk))
			{
			case MessageBoxResult.No:
				tbtoff();
				Environment.Exit(20);
				break;
			case MessageBoxResult.Yes:
				Lenovo_TVSU_Pop_Window();
				break;
			}
		}

		public void Lenovo_TVSU_Pop_Window()
		{
			if (System.Windows.MessageBox.Show("DO NOT POWER OFF DURING AN UPDATE.\nOtherwise, your system may be damaged.\nDo you want to continue?", "Thunderbolt Firmware Update", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.No)
			{
				start_window();
			}
		}

		public MainWindow()
		{
			tbtcheck();
			tbton();
			try
			{
				string[] array = Environment.GetCommandLineArgs().Skip(1).ToArray();
				Restart = array[0];
				Silent = array[1];
				if (Restart.ToUpper() == "-S" || Restart.ToUpper() == "/QN")
				{
					Change = Restart;
					if (Silent == "1" || Silent == "0")
					{
						Restart = Silent;
						Silent = Change;
						new Silent(Restart).Show();
					}
				}
				else
				{
					new Silent(Restart).Show();
				}
			}
			catch (Exception)
			{
				Silent = "";
				if (Restart.ToUpper() == "-S" || Restart.ToUpper() == "/QN")
				{
					Silent = "0";
					new Silent(Silent).Show();
				}
				else
				{
					start_window();
				}
			}
			try
			{
				InitializeComponent();
				List<SdkTbtController> list = LoadControllers().Values.ToList();
				List<SdkTbtDevice> devices = LoadDevices().Values.ToList();
				BuildWizard(list, devices);
				CurrentController = list.First();
				ConfigFwUpdateTask();
			}
			catch (Exception)
			{
				tbtoff();
				Environment.Exit(-1);
				Close();
			}
		}

		private static Dictionary<string, SdkTbtDevice> LoadDevices()
		{
			return SdkTbtDevice.GetDevicesFromWmi();
		}

		private static Dictionary<string, SdkTbtController> LoadControllers()
		{
			try
			{
				do
				{
					Dictionary<string, SdkTbtController> controllersFromWmi = SdkTbtController.GetControllersFromWmi();
					if (controllersFromWmi.Count != 0)
					{
						return controllersFromWmi;
					}
				}
				while (System.Windows.MessageBox.Show(FwUpdateAPI.Resources.NoControllerStringPart0 + "\n" + FwUpdateAPI.Resources.NoDeviceStringPart1 + "\n" + FwUpdateAPI.Resources.NoDeviceStringPart2GUI, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK);
				Environment.Exit(1);
			}
			catch (ManagementException)
			{
				System.Windows.MessageBox.Show(FwUpdateAPI.Resources.SWNotInstalled, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			catch (Exception ex2)
			{
				System.Windows.MessageBox.Show(ex2.Message, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			throw new InitException();
		}

		private void ConfigFwUpdateTask()
		{
			_flashTask = new Task(delegate
			{
				_isDuringFwUpdate = true;
				try
				{
					CurrentController.UpdateFirmwareFromFile(FileName);
					FlashingResult = FwUpdateAPI.Resources.FWUpdateSuccessMessage;
				}
				catch (TbtException ex)
				{
					FlashingResult = new TbtException(ex.ErrorCode, FwUpdateAPI.Resources.FWUpdateFailedMessage).Message;
				}
				catch (Exception ex2)
				{
					string text2 = new TbtException(TbtStatus.SDK_GENERAL_ERROR_CODE, ex2.Message).Message;
					if (!ex2.Message.Any())
					{
						text2 = text2 + "\n" + ex2.HResult;
					}
					FlashingResult = text2;
				}
			});
			_flashTask.ContinueWith(delegate
			{
				base.Dispatcher.Invoke(delegate
				{
					NextButton_Click(this, new RoutedEventArgs());
					UpdateCompletedScreen updateCompletedScreen = PageContent.Content as UpdateCompletedScreen;
					if (updateCompletedScreen != null)
					{
						updateCompletedScreen.ResultMessage.Text = FlashingResult;
					}
					_isDuringFwUpdate = false;
					string text = FlashingResult.Substring(9, 3);
					if (FlashingResult == "Firmware was updated successfully.")
					{
						if (Restart != "1")
						{
							tbt_reset();
							Thread.Sleep(3000);
							tbtoff();
							ProcessStartInfo startInfo = new ProcessStartInfo("shutdown.exe")
							{
								Arguments = "/r /f /t 8"
							};
							Environment.GetCommandLineArgs();
							Process.Start(startInfo);
							Console.WriteLine("Wait few seconds to force Thunderbolt device to be power off ... ");
							Environment.Exit(0);
						}
						else if (Restart == "1")
						{
							tbt_reset();
							Thread.Sleep(3000);
							tbtoff();
							Console.WriteLine("Wait few seconds to force Thunderbolt device to be power off... ");
							Environment.Exit(0);
						}
					}
					else if (FlashingResult != "Firmware was updated successfully.")
					{
						tbtoff();
						Environment.Exit(1);
						if (int.Parse(text.Substring(1, 1)) == 9)
						{
							string s = text.Substring(9, 2);
							Environment.GetCommandLineArgs();
							Environment.ExitCode = int.Parse(s);
							tbtoff();
							Close();
						}
						else
						{
							Environment.GetCommandLineArgs();
							Environment.ExitCode = int.Parse(text);
							tbtoff();
							Close();
						}
					}
				});
			});
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			if (_isDuringFwUpdate)
			{
				System.Windows.MessageBox.Show(FwUpdateAPI.Resources.CloseDuringUpdate, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
				e.Cancel = true;
			}
			else if (PageContent.Content is WelcomeScreen)
			{
				tbtoff();
				Environment.Exit(20);
			}
		}

		private void BuildWizard(List<SdkTbtController> controllers, List<SdkTbtDevice> devices)
		{
			LoadScreens(controllers, devices);
			PageContent.Content = _pages[_currentScreen];
			base.DataContext = PageContent.Content;
			((System.Windows.Controls.Button)Steps.Children[_currentScreen]).Style = (FindResource("CurrentWizardStepStyle") as Style);
		}

		private void LoadScreens(List<SdkTbtController> controllers, List<SdkTbtDevice> devices)
		{
			AddScreen(new WelcomeScreen());
			AddScreen(new FwUpdateProcessScreen());
			AddScreen(new UpdateCompletedScreen());
		}

		private void AddScreen(IWizardScreen screen)
		{
			_pages.Add(screen);
			StackPanel stackPanel = new StackPanel();
			string[] title = screen.Title;
			foreach (string text in title)
			{
				stackPanel.Children.Add(new TextBlock
				{
					Text = text,
					HorizontalAlignment = System.Windows.HorizontalAlignment.Center
				});
			}
			System.Windows.Controls.Button element = new System.Windows.Controls.Button
			{
				Content = stackPanel,
				Style = (FindResource("WizardStepStyle") as Style)
			};
			Steps.Children.Add(element);
		}

		public string path()
		{
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		public int Battery_Percent()
		{
			return (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100f);
		}

		public int battery_check()
		{
			int num = Battery_Percent();
			if (new Battery.PowerStatus().PowerLineStatus.ToString() == "Online")
			{
				return 1;
			}
			if (num < 25)
			{
				return 2;
			}
			return 0;
		}

		public void tbtoff()
		{
			new TBT_SCRQ().TbtIoControl(TBT_SCRQ.SCRQ_Command.TBT_FORCE_PWR_OFF);
			Thread.Sleep(2000);
		}

		public void tbton()
		{
			new TBT_SCRQ().TbtIoControl(TBT_SCRQ.SCRQ_Command.TBT_FORCE_PWR_ON);
			Console.WriteLine("Wait 7 seconds to force Thunderbolt device to be power on... ");
			Thread.Sleep(7000);
		}

		public void tbt_reset()
		{
			new TBT_SCRQ().TbtIoControl(TBT_SCRQ.SCRQ_Command.RESET_ALL_PDC);
			Console.WriteLine("Wait 7 seconds to force Thunderbolt device to be power on... ");
			Thread.Sleep(7000);
		}

		public void tbtcheck()
		{
			int num = PM_CHECK();
			if (num != 0)
			{
				Environment.Exit(num);
			}
			try
			{
				TBT_SCRQ tBT_SCRQ = new TBT_SCRQ();
				if (tBT_SCRQ.TbtIoControl(TBT_SCRQ.SCRQ_Command.QUERY_SCRQ) == 0)
				{
					Console.WriteLine("BIOS is not ready! Please update BIOS to the latest one");
					Thread.Sleep(1000);
					Environment.Exit(-1);
				}
				else
				{
					Console.WriteLine("The interface is checked and the BIOS supports the interface.");
					Thread.Sleep(1000);
					uint num2 = tBT_SCRQ.TbtIoControl(TBT_SCRQ.SCRQ_Command.QUERY_TBT_FORCE_PWR);
					if (num2 == 2147483648u || num2 == 0)
					{
						Console.WriteLine("Error, TBT_FORCE_PWR not supported! Please update BIOS first.");
						Thread.Sleep(1000);
						Environment.Exit(-1);
					}
					else
					{
						Console.WriteLine("The TBT_PWR is checked and the BIOS supports the function.");
						Thread.Sleep(1000);
						uint num3 = tBT_SCRQ.TbtIoControl(TBT_SCRQ.SCRQ_Command.QUERY_PDCRESET);
						if (num3 == 2147483648u || num3 == 0)
						{
							Console.WriteLine("Error, PDC_RESET not supported! Please update BIOS first.");
							Thread.Sleep(1000);
							Environment.Exit(-1);
						}
						else
						{
							Console.WriteLine("The PD Controller is checked and the BIOS supports the function.");
						}
					}
				}
			}
			catch (Win32Exception)
			{
				Console.WriteLine("Lenovo PM Driver is not installed on the system!");
				Thread.Sleep(1000);
				Environment.Exit(-1);
			}
		}

		public string prompt()
		{
			return System.Windows.MessageBox.Show("This process requires power to avoid an accident power-off during an update." + Environment.NewLine + "Connect the AC adapter to the system.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Hand).ToString();
		}

		public string Prompt_battrty_low()
		{
			return System.Windows.MessageBox.Show("This process requires power to avoid an accident power-off during an update." + Environment.NewLine + "Connect the AC adapter to the system and charge the battery on system over 25%.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Hand).ToString();
		}

		public void seterrorcode(string msg)
		{
			MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(msg, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Hand);
			ERROR1 = messageBoxResult.ToString();
			string text = msg.Substring(9, 2);
			if (PageContent.Content is SelectController)
			{
				if (msg.IndexOf("0x207") > 0)
				{
					messageBoxResult = System.Windows.MessageBox.Show("The Thunderbolt firmware \"TBT.bin\" file is not in the package. Please add the \"TBT.bin\" in the package.", FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Hand);
					ERROR1 = messageBoxResult.ToString();
					if (PageContent.Content is SelectController && ERROR1.ToString() == "OK")
					{
						tbtoff();
						Environment.Exit(207);
					}
				}
				else if (int.Parse(text.Substring(1, 1)) == 9 && ERROR1.ToString() == "OK")
				{
					text = msg.Substring(9, 2);
					Environment.Exit(int.Parse(text));
					Close();
				}
			}
			text = msg.Substring(9, 2);
			Environment.Exit(int.Parse(text));
			Close();
		}

		public void NextButton_Click(object sender, RoutedEventArgs re)
		{
			string[] fileSystemEntries = Directory.GetFileSystemEntries(path());
			for (int i = 0; i < fileSystemEntries.Length; i++)
			{
				string text = fileSystemEntries[i].ToUpper();
				if (text.IndexOf("TBT.BIN") > 0)
				{
					FileName = text;
				}
			}
			if (PageContent.Content is WelcomeScreen)
			{
				Thread.Sleep(1000);
				int num = battery_check();
				while (num == 0 || num == 2)
				{
					switch (num)
					{
					case 0:
						break;
					case 2:
						goto IL_00b6;
					default:
						continue;
					}
					string a = prompt();
					if (a == "OK")
					{
						num = battery_check();
					}
					else if (a != "OK")
					{
						tbt_reset();
						Thread.Sleep(3000);
						tbtoff();
						Environment.Exit(21);
						break;
					}
					continue;
					IL_00b6:
					string a2 = Prompt_battrty_low();
					if (a2 == "OK")
					{
						num = battery_check();
					}
					else if (a2 != "OK")
					{
						tbt_reset();
						Thread.Sleep(3000);
						tbtoff();
						Environment.Exit(22);
						break;
					}
				}
				if (num == 1)
				{
					try
					{
						CurrentController.ValidateImage(FileName);
						SafeModeWarning();
						_flashTask.Start();
					}
					catch (Exception ex)
					{
						string text2 = ex.Message;
						if (ex is ManagementException)
						{
							text2 = "WMI error: " + text2;
						}
						seterrorcode(text2);
						return;
					}
				}
			}
			((System.Windows.Controls.Button)Steps.Children[_currentScreen]).Style = (FindResource("WizardStepStyle") as Style);
			PageContent.Content = _pages[++_currentScreen];
			base.DataContext = PageContent.Content;
			((System.Windows.Controls.Button)Steps.Children[_currentScreen]).Style = (FindResource("CurrentWizardStepStyle") as Style);
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			((System.Windows.Controls.Button)Steps.Children[_currentScreen]).Style = (FindResource("WizardStepStyle") as Style);
			PageContent.Content = _pages[--_currentScreen];
			base.DataContext = PageContent.Content;
			((System.Windows.Controls.Button)Steps.Children[_currentScreen]).Style = (FindResource("CurrentWizardStepStyle") as Style);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			tbtoff();
			Environment.Exit(20);
		}

		public void OnPropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private void SafeModeWarning()
		{
			SdkTbtController sdkTbtController = CurrentController as SdkTbtController;
			if (sdkTbtController != null && sdkTbtController.IsInSafeMode)
			{
				System.Windows.MessageBox.Show(FwUpdateAPI.Resources.MinimalValidationInSafeMode, FwUpdateAPI.Resources.MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/FwUpdateTool;component/mainwindow.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocator);
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
				((MainWindow)target).Closing += MainWindow_OnClosing;
				break;
			case 2:
				CancelButton = (System.Windows.Controls.Button)target;
				CancelButton.Click += CancelButton_Click;
				break;
			case 3:
				NextButton = (System.Windows.Controls.Button)target;
				NextButton.Click += NextButton_Click;
				break;
			case 4:
				BackButton = (System.Windows.Controls.Button)target;
				BackButton.Click += BackButton_Click;
				break;
			case 5:
				PageContent = (ContentControl)target;
				break;
			case 6:
				Steps = (StackPanel)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
}
