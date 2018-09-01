using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace FwUpdateTool
{
	public class App : Application
	{
		private static void ErrorMsgBox(string message)
		{
			MessageBox.Show(message, "Thunderbolt(TM) Firmware Update Utility", MessageBoxButton.OK, MessageBoxImage.Hand);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			base.OnStartup(e);
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName assemblyName = new AssemblyName(args.Name);
			if (assemblyName.Name.EndsWith(".resources"))
			{
				return null;
			}
			Application.Current.Shutdown(1);
			throw new Exception("Couldn't load assembly / DLL file: " + assemblyName.Name);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			ErrorMsgBox("Exception has been thrown. Closing application...\nException type:\t" + ex.GetType().FullName + "\nException message:\t" + ex.Message);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			base.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
