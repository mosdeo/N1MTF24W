using System;
using System.ComponentModel;
using System.Management;

namespace FwUpdateAPI
{
	public class FlashProgress : INotifyPropertyChanged, IDisposable
	{
		private uint _progress;

		private readonly ManagementEventWatcher _watcher;

		private readonly bool _registered = true;

		public uint Progress
		{
			get
			{
				return _progress;
			}
			private set
			{
				_progress = value;
				OnPropertyChanged("Progress");
			}
		}

		public bool Registered => _registered;

		public event PropertyChangedEventHandler PropertyChanged;

		public FlashProgress()
		{
			try
			{
				_watcher = new ManagementEventWatcher("root\\Intel\\Thunderbolt", "SELECT * FROM SdkFwUpdateProgress");
				_watcher.EventArrived += OnProgressDetected;
				_watcher.Start();
			}
			catch
			{
				_registered = false;
			}
		}

		private void OnProgressDetected(object sender, EventArrivedEventArgs e)
		{
			ManagementBaseObject newEvent = e.NewEvent;
			Progress = (uint)newEvent["Progress"];
		}

		private void OnPropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public void Dispose()
		{
			_watcher.Dispose();
		}
	}
}
