// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.HardwareInfoManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace Magicka.WebTools;

public static class HardwareInfoManager
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.HardwareInfoManager;
  private const ulong CPU_RAM_TO_GB = 1048576 /*0x100000*/;
  private const uint GPU_RAM_TO_GB = 1073741824 /*0x40000000*/;
  private const string SYNTAX_SERIAL_NUMBER = "SerialNumber";
  private const string SYNTAX_UNIQUE_ID = "UniqueId";
  private const string SYNTAX_NAME = "Name";
  private const string SYNTAX_CAPTION = "Caption";
  private const string SYNTAX_ADAPTER_RAM = "AdapterRAM";
  private const string SYNTAX_VISIBLE_MEMORY = "TotalVisibleMemorySize";
  private const string SYNTAX_LOGICAL_PROCESSOR = "NumberOfLogicalProcessors";
  private const string SYNTAX_DRIVER_VERSION = "DriverVersion";
  private const string COMPONENT_NAME_BASE_BOARD = "Win32_BaseBoard";
  private const string COMPONENT_NAME_BIOS = "Win32_BIOS";
  private const string COMPONENT_NAME_PROCESSOR = "Win32_Processor";
  private const string COMPONENT_NAME_DISK_DRIVE = "Win32_DiskDrive";
  private const string COMPONENT_NAME_OPERATING_SYSTEM = "Win32_OperatingSystem";
  private const string COMPONENT_NAME_VIDEO_CONTROLLER = "Win32_VideoController";
  public static readonly string BaseBoardSerial = string.Empty;
  public static readonly string BiosSerial = string.Empty;
  public static readonly string ProcessorUniqueId = string.Empty;
  public static readonly string DiskDriveSerial = string.Empty;
  public static readonly string OSSerial = string.Empty;
  public static readonly string DeviceUniqueId = string.Empty;
  public static readonly string OSVersion = string.Empty;
  public static readonly ulong SystemMemory = 0;
  public static readonly string GfxDevice = string.Empty;
  public static readonly uint GfxMem = 0;
  public static readonly string GfxDriver = string.Empty;
  public static readonly string CPUType = string.Empty;
  public static readonly uint LogicalProcessors = 0;

  static HardwareInfoManager()
  {
    HardwareInfoManager.BaseBoardSerial = HardwareInfoManager.GetComponent<string>("Win32_BaseBoard", "SerialNumber");
    HardwareInfoManager.BiosSerial = HardwareInfoManager.GetComponent<string>("Win32_BIOS", "SerialNumber");
    HardwareInfoManager.ProcessorUniqueId = HardwareInfoManager.GetComponent<string>("Win32_Processor", "UniqueId");
    HardwareInfoManager.DiskDriveSerial = HardwareInfoManager.GetComponent<string>("Win32_DiskDrive", "SerialNumber");
    HardwareInfoManager.OSSerial = HardwareInfoManager.GetComponent<string>("Win32_OperatingSystem", "SerialNumber");
    HardwareInfoManager.DeviceUniqueId = HardwareInfoManager.GetInt64HashCode(HardwareInfoManager.BaseBoardSerial + HardwareInfoManager.BiosSerial + HardwareInfoManager.ProcessorUniqueId + HardwareInfoManager.DiskDriveSerial + HardwareInfoManager.OSSerial).ToString("x");
    HardwareInfoManager.OSVersion = HardwareInfoManager.GetComponent<string>("Win32_OperatingSystem", "Caption");
    HardwareInfoManager.SystemMemory = (ulong) Math.Round((double) HardwareInfoManager.GetComponent<ulong>("Win32_OperatingSystem", "TotalVisibleMemorySize") / 1048576.0);
    HardwareInfoManager.GfxDevice = HardwareInfoManager.GetComponent<string>("Win32_VideoController", "Name");
    HardwareInfoManager.GfxMem = (uint) Math.Round((double) HardwareInfoManager.GetComponent<uint>("Win32_VideoController", "AdapterRAM") / 1073741824.0);
    HardwareInfoManager.GfxDriver = HardwareInfoManager.GetComponent<string>("Win32_VideoController", "DriverVersion");
    HardwareInfoManager.CPUType = HardwareInfoManager.GetComponent<string>("Win32_Processor", "Name");
    HardwareInfoManager.LogicalProcessors = HardwareInfoManager.GetComponent<uint>("Win32_Processor", "NumberOfLogicalProcessors");
  }

  private static T GetComponent<T>(string iHwClass, string iSyntax)
  {
    T component = default (T);
    try
    {
      using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + iHwClass).Get().GetEnumerator())
      {
        if (enumerator.MoveNext())
          component = (T) Convert.ChangeType(enumerator.Current[iSyntax], typeof (T));
      }
    }
    catch (Exception ex)
    {
      Logger.LogError(Logger.Source.HardwareInfoManager, $"{iHwClass} triggered an exception : {ex.Message}");
    }
    return component;
  }

  public static string GenerateUniqueSessionId()
  {
    long totalSeconds = (long) DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds;
    return HardwareInfoManager.DeviceUniqueId + totalSeconds.ToString("x");
  }

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

  private static long GetInt64HashCode(string iStrText)
  {
    long int64HashCode = 0;
    if (!string.IsNullOrEmpty(iStrText))
    {
      byte[] hash = new SHA256CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(iStrText));
      int64HashCode = BitConverter.ToInt64(hash, 0) ^ BitConverter.ToInt64(hash, 8) ^ BitConverter.ToInt64(hash, 24);
    }
    return int64HashCode;
  }
}
