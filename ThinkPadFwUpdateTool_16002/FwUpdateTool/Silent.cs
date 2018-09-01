using Battery;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TBT_Control;

namespace FwUpdateTool
{
	public class Silent : Form
	{
		public int Stop;

		private string PathContent;

		private string FilePath;

		private string FileName;

		private string FileUPPERCASE;

		private string path;

		private IContainer components;

		public void BatteryCheck()
		{
			while (true)
			{
				string a = new Battery.PowerStatus().PowerLineStatus.ToString();
				int num = (int)(SystemInformation.PowerStatus.BatteryLifePercent * 100f);
				if (a != "Online")
				{
					Stop = 1;
					if (MessageBox.Show("This process requires power to avoid an accident power-off during an update." + Environment.NewLine + "Connect the AC adapter to the system.", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand).ToString() != "OK")
					{
						Stop = 1;
						tbtoff();
						Environment.Exit(21);
					}
					Stop = 1;
				}
				else if (num < 25)
				{
					Stop = 1;
					if (MessageBox.Show("This process requires power to avoid an accident power-off during an update." + Environment.NewLine + "Connect the AC adapter to the system and charge the battery on system over 25%.", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand).ToString() != "OK")
					{
						Stop = 1;
						tbtoff();
						Environment.Exit(22);
					}
					Stop = 1;
				}
				else if (a == "Online")
				{
					Stop = 0;
				}
			}
		}

		public Silent(string restart)
		{
			InitializeComponent();
			string str = "FwUpdateCmd EnumControllers";
			string text = "";
			string text2 = "";
			string text3 = "";
			Process process = new Process();
			Process process2 = new Process();
			new Process();
			new Thread(BatteryCheck).Start();
			PathContent = AppDomain.CurrentDomain.BaseDirectory;
			FilePath = PathContent;
			string[] fileSystemEntries = Directory.GetFileSystemEntries(FilePath);
			foreach (string text4 in fileSystemEntries)
			{
				FileUPPERCASE = text4.ToUpper();
				if (FileUPPERCASE.IndexOf("TBT.BIN") > 0)
				{
					FileName = FileUPPERCASE;
				}
			}
			do
			{
				path = FileName;
				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.Arguments = "/c " + str;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.Start();
				text = process.StandardOutput.ReadToEnd();
				text = text.Substring(0, text.Length - 2);
				process.Close();
				Thread.Sleep(2000);
			}
			while (Stop != 0);
			Console.WriteLine("The process is started. Please wait for the few minutes for updating the Thunderbolt firmware.");
			text2 = "FwUpdateCmd FWUpdate \"" + text + "\" \"" + path + "\"";
			process2.StartInfo.FileName = "cmd.exe";
			process2.StartInfo.Arguments = "/c " + text2;
			process2.StartInfo.UseShellExecute = false;
			process2.StartInfo.RedirectStandardInput = true;
			process2.StartInfo.RedirectStandardOutput = true;
			process2.StartInfo.RedirectStandardError = true;
			process2.StartInfo.CreateNoWindow = true;
			process2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process2.Start();
			while (true)
			{
				text3 = process2.StandardOutput.ReadToEnd();
				if (text3.IndexOf("0x") > 0)
				{
					text3 = text3.Substring(text3.IndexOf("0x"), 6);
					Console.WriteLine("Due to errors, the update process is stopped. The error code is:" + text3);
					tbtoff();
					Console.WriteLine("Wait few seconds to force Thunderbolt device to be power off... ");
					Thread.Sleep(3000);
					Environment.Exit(1);
				}
				while (text3.IndexOf("G3") != 0)
				{
					if (restart != "1")
					{
						Console.WriteLine("The Thunderbolt Firmware update proccess is finished. The system will reboot now.");
						ProcessStartInfo startInfo = new ProcessStartInfo("shutdown.exe")
						{
							Arguments = "/r /f /t 5"
						};
						tbt_reset();
						Thread.Sleep(3000);
						tbtoff();
						Console.WriteLine("Wait few seconds to force Thunderbolt device to be power off... ");
						Thread.Sleep(3000);
						Environment.GetCommandLineArgs();
						Process.Start(startInfo);
						Environment.Exit(0);
					}
					else if (restart == "1")
					{
						Console.WriteLine("The Thunderbolt Firmware update proccess is finished.");
						tbt_reset();
						Thread.Sleep(3000);
						tbtoff();
						Console.WriteLine("Wait few seconds to force Thunderbolt device to be power off... ");
						Thread.Sleep(3000);
						Environment.Exit(0);
					}
				}
			}
		}

		public void tbt_reset()
		{
			new TBT_SCRQ().TbtIoControl(TBT_SCRQ.SCRQ_Command.RESET_ALL_PDC);
			Console.WriteLine("Wait few seconds to force Thunderbolt device to be reseted... ");
			Thread.Sleep(3000);
		}

		public void tbtoff()
		{
			new TBT_SCRQ().TbtIoControl(TBT_SCRQ.SCRQ_Command.TBT_FORCE_PWR_OFF);
		}

		private void Silent_Load(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(284, 261);
			base.Name = "Silent";
			Text = "Silent";
			base.Load += new System.EventHandler(Silent_Load);
			ResumeLayout(false);
		}
	}
}
