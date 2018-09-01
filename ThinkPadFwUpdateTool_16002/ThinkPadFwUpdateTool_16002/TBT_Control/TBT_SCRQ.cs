using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace TBT_Control
{
	internal class TBT_SCRQ : IDisposable
	{
		public enum SCRQ_Command : uint
		{
			QUERY_SCRQ = 0u,
			QUERY_PDCRESET = 0x200,
			QUERY_TBT_FORCE_PWR = 768u,
			RESET_ALL_PDC = 2147484176u,
			RESET_PRIMARY_PDC = 2147484177u,
			RESET_SECONDARY_PDC = 2147484178u,
			TBT_FORCE_PWR_ON = 2147484417u,
			TBT_FORCE_PWR_OFF = 2147484418u
		}

		private const uint FILE_DEVICE_UNKNOWN = 34u;

		private const uint METHOD_BUFFERED = 0u;

		private const uint FILE_ANY_ACCESS = 0u;

		private uint IOCTL_PMDRV_SYSTEM_CONFIGURATION_REQUEST;

		private SafeFileHandle SCRQ_Handle;

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern SafeFileHandle CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.U4)] FileAccess access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DeviceIoControl(SafeFileHandle hDevice, uint IoControlCode, ref uint InBuffer, int nInBufferSize, ref uint OutBuffer, int nOutBufferSize, ref int pBytesReturned, IntPtr lpOverlapped);

		[DllImport("kernel32.dll")]
		private static extern void CloseHandle(SafeFileHandle hDevice);

		private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
		{
			return ((DeviceType & 0xFFFF) << 16) | ((Access & 3) << 14) | ((Function & 0xFFF) << 2) | (Method & 3);
		}

		public TBT_SCRQ()
		{
			IOCTL_PMDRV_SYSTEM_CONFIGURATION_REQUEST = CTL_CODE(34u, 2524u, 0u, 0u);
			SCRQ_Handle = CreateFile("\\\\.\\PMDRVS", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
			if (SCRQ_Handle.IsInvalid)
			{
				SCRQ_Handle = CreateFile("\\\\.\\IBMPmDrv", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
				if (SCRQ_Handle.IsInvalid)
				{
					throw new Win32Exception();
				}
			}
		}

		public uint TbtIoControl(SCRQ_Command command)
		{
			uint InBuffer = (uint)command;
			uint OutBuffer = 0u;
			int pBytesReturned = 0;
			DeviceIoControl(SCRQ_Handle, IOCTL_PMDRV_SYSTEM_CONFIGURATION_REQUEST, ref InBuffer, 4, ref OutBuffer, 4, ref pBytesReturned, IntPtr.Zero);
			return OutBuffer;
		}

		~TBT_SCRQ()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!SCRQ_Handle.IsClosed)
			{
				SCRQ_Handle.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}
}
