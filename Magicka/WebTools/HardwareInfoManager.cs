using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Magicka.CoreFramework;

namespace Magicka.WebTools
{
	// Token: 0x02000092 RID: 146
	public static class HardwareInfoManager
	{
		// Token: 0x0600043E RID: 1086 RVA: 0x00014870 File Offset: 0x00012A70
		static HardwareInfoManager()
		{
			HardwareInfoManager.BaseBoardSerial = HardwareInfoManager.GetComponent<string>("Win32_BaseBoard", "SerialNumber");
			HardwareInfoManager.BiosSerial = HardwareInfoManager.GetComponent<string>("Win32_BIOS", "SerialNumber");
			HardwareInfoManager.ProcessorUniqueId = HardwareInfoManager.GetComponent<string>("Win32_Processor", "UniqueId");
			HardwareInfoManager.DiskDriveSerial = HardwareInfoManager.GetComponent<string>("Win32_DiskDrive", "SerialNumber");
			HardwareInfoManager.OSSerial = HardwareInfoManager.GetComponent<string>("Win32_OperatingSystem", "SerialNumber");
			string iStrText = string.Concat(new string[]
			{
				HardwareInfoManager.BaseBoardSerial,
				HardwareInfoManager.BiosSerial,
				HardwareInfoManager.ProcessorUniqueId,
				HardwareInfoManager.DiskDriveSerial,
				HardwareInfoManager.OSSerial
			});
			HardwareInfoManager.DeviceUniqueId = HardwareInfoManager.GetInt64HashCode(iStrText).ToString("x");
			HardwareInfoManager.OSVersion = HardwareInfoManager.GetComponent<string>("Win32_OperatingSystem", "Caption");
			HardwareInfoManager.SystemMemory = (ulong)Math.Round(HardwareInfoManager.GetComponent<ulong>("Win32_OperatingSystem", "TotalVisibleMemorySize") / 1048576.0);
			HardwareInfoManager.GfxDevice = HardwareInfoManager.GetComponent<string>("Win32_VideoController", "Name");
			HardwareInfoManager.GfxMem = (uint)Math.Round(HardwareInfoManager.GetComponent<uint>("Win32_VideoController", "AdapterRAM") / 1073741824.0);
			HardwareInfoManager.GfxDriver = HardwareInfoManager.GetComponent<string>("Win32_VideoController", "DriverVersion");
			HardwareInfoManager.CPUType = HardwareInfoManager.GetComponent<string>("Win32_Processor", "Name");
			HardwareInfoManager.LogicalProcessors = HardwareInfoManager.GetComponent<uint>("Win32_Processor", "NumberOfLogicalProcessors");
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00014A58 File Offset: 0x00012C58
		private static T GetComponent<T>(string iHwClass, string iSyntax)
		{
			T result = default(T);
			try
			{
				ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + iHwClass);
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						result = (T)((object)Convert.ChangeType(managementObject[iSyntax], typeof(T)));
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(Logger.Source.HardwareInfoManager, iHwClass + " triggered an exception : " + ex.Message);
			}
			return result;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00014B10 File Offset: 0x00012D10
		public static string GenerateUniqueSessionId()
		{
			long num = (long)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds;
			return HardwareInfoManager.DeviceUniqueId + num.ToString("x");
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00014B50 File Offset: 0x00012D50
		public static void Print()
		{
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Base board serial : " + HardwareInfoManager.BaseBoardSerial);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "BIOS serial : " + HardwareInfoManager.BiosSerial);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Processor unique id : " + HardwareInfoManager.ProcessorUniqueId);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Disk drive serial : " + HardwareInfoManager.DiskDriveSerial);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "OS serial : " + HardwareInfoManager.OSSerial);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Device unique id : " + HardwareInfoManager.DeviceUniqueId.ToString());
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "OS Version : " + HardwareInfoManager.OSVersion);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "System Memory : " + HardwareInfoManager.SystemMemory.ToString());
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Gfx Device : " + HardwareInfoManager.GfxDevice);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Gfx Mem : " + HardwareInfoManager.GfxMem.ToString());
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Gfx Driver : " + HardwareInfoManager.GfxDriver);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "CPU Type : " + HardwareInfoManager.CPUType);
			Logger.LogDebug(Logger.Source.HardwareInfoManager, "Logical Processors : " + HardwareInfoManager.LogicalProcessors.ToString());
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x00014C8C File Offset: 0x00012E8C
		private static long GetInt64HashCode(string iStrText)
		{
			long result = 0L;
			if (!string.IsNullOrEmpty(iStrText))
			{
				byte[] bytes = Encoding.Unicode.GetBytes(iStrText);
				SHA256 sha = new SHA256CryptoServiceProvider();
				byte[] value = sha.ComputeHash(bytes);
				long num = BitConverter.ToInt64(value, 0);
				long num2 = BitConverter.ToInt64(value, 8);
				long num3 = BitConverter.ToInt64(value, 24);
				result = (num ^ num2 ^ num3);
			}
			return result;
		}

		// Token: 0x040002A4 RID: 676
		private const Logger.Source LOGGER_SOURCE = Logger.Source.HardwareInfoManager;

		// Token: 0x040002A5 RID: 677
		private const ulong CPU_RAM_TO_GB = 1048576UL;

		// Token: 0x040002A6 RID: 678
		private const uint GPU_RAM_TO_GB = 1073741824U;

		// Token: 0x040002A7 RID: 679
		private const string SYNTAX_SERIAL_NUMBER = "SerialNumber";

		// Token: 0x040002A8 RID: 680
		private const string SYNTAX_UNIQUE_ID = "UniqueId";

		// Token: 0x040002A9 RID: 681
		private const string SYNTAX_NAME = "Name";

		// Token: 0x040002AA RID: 682
		private const string SYNTAX_CAPTION = "Caption";

		// Token: 0x040002AB RID: 683
		private const string SYNTAX_ADAPTER_RAM = "AdapterRAM";

		// Token: 0x040002AC RID: 684
		private const string SYNTAX_VISIBLE_MEMORY = "TotalVisibleMemorySize";

		// Token: 0x040002AD RID: 685
		private const string SYNTAX_LOGICAL_PROCESSOR = "NumberOfLogicalProcessors";

		// Token: 0x040002AE RID: 686
		private const string SYNTAX_DRIVER_VERSION = "DriverVersion";

		// Token: 0x040002AF RID: 687
		private const string COMPONENT_NAME_BASE_BOARD = "Win32_BaseBoard";

		// Token: 0x040002B0 RID: 688
		private const string COMPONENT_NAME_BIOS = "Win32_BIOS";

		// Token: 0x040002B1 RID: 689
		private const string COMPONENT_NAME_PROCESSOR = "Win32_Processor";

		// Token: 0x040002B2 RID: 690
		private const string COMPONENT_NAME_DISK_DRIVE = "Win32_DiskDrive";

		// Token: 0x040002B3 RID: 691
		private const string COMPONENT_NAME_OPERATING_SYSTEM = "Win32_OperatingSystem";

		// Token: 0x040002B4 RID: 692
		private const string COMPONENT_NAME_VIDEO_CONTROLLER = "Win32_VideoController";

		// Token: 0x040002B5 RID: 693
		public static readonly string BaseBoardSerial = string.Empty;

		// Token: 0x040002B6 RID: 694
		public static readonly string BiosSerial = string.Empty;

		// Token: 0x040002B7 RID: 695
		public static readonly string ProcessorUniqueId = string.Empty;

		// Token: 0x040002B8 RID: 696
		public static readonly string DiskDriveSerial = string.Empty;

		// Token: 0x040002B9 RID: 697
		public static readonly string OSSerial = string.Empty;

		// Token: 0x040002BA RID: 698
		public static readonly string DeviceUniqueId = string.Empty;

		// Token: 0x040002BB RID: 699
		public static readonly string OSVersion = string.Empty;

		// Token: 0x040002BC RID: 700
		public static readonly ulong SystemMemory = 0UL;

		// Token: 0x040002BD RID: 701
		public static readonly string GfxDevice = string.Empty;

		// Token: 0x040002BE RID: 702
		public static readonly uint GfxMem = 0U;

		// Token: 0x040002BF RID: 703
		public static readonly string GfxDriver = string.Empty;

		// Token: 0x040002C0 RID: 704
		public static readonly string CPUType = string.Empty;

		// Token: 0x040002C1 RID: 705
		public static readonly uint LogicalProcessors = 0U;
	}
}
