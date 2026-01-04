// Decompiled with JetBrains decompiler
// Type: Magicka.Storage.SaveManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

#nullable disable
namespace Magicka.Storage;

internal class SaveManager
{
  private const string POPS_SAVE_FILE_NAME = "./saveData/pops.sav";
  private static SaveManager sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static volatile object sFileLock = new object();
  private bool mAlreadyLoaded;
  private SaveData[] mMythosSaveSlots;
  private SaveData[] mSaveSlots;
  private KeyboardMouseController.Binding[] mKeyboardBindings = new KeyboardMouseController.Binding[17];

  public static SaveManager Instance
  {
    get
    {
      if (SaveManager.sSingelton == null)
      {
        lock (SaveManager.sSingeltonLock)
        {
          if (SaveManager.sSingelton == null)
            SaveManager.sSingelton = new SaveManager();
        }
      }
      return SaveManager.sSingelton;
    }
  }

  private SaveManager()
  {
    if (!Directory.Exists("./SaveData"))
      Directory.CreateDirectory("./SaveData");
    StreamReader streamReader = File.OpenText("content/Data/col.dat");
    int result;
    int.TryParse(streamReader.ReadLine(), out result);
    if (result <= 0)
      result = 10;
    Defines.PLAYERCOLORS_UNLOCKED = result;
    streamReader.Close();
    if (Defines.PLAYERCOLORS_UNLOCKED <= Defines.PLAYERCOLORS.Length)
      return;
    Defines.PLAYERCOLORS_UNLOCKED = Defines.PLAYERCOLORS.Length;
  }

  public bool AlreadyLoaded => this.mAlreadyLoaded;

  public KeyboardMouseController.Binding[] KeyBindings => this.mKeyboardBindings;

  private void LoadSettings1430(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    instance.VSSettings.Read1370(iReader);
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.Particles = (SettingOptions) iReader.ReadByte();
    instance.ParticleLights = iReader.ReadBoolean();
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1410(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num = iReader.ReadInt32();
    for (int index3 = 0; index3 < num; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    instance.SteamGameLanguage = iReader.ReadString();
    instance.Language = (Language) iReader.ReadInt32();
    instance.VSync = iReader.ReadBoolean();
  }

  private void LoadSettings1410(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    instance.VSSettings.Read1370(iReader);
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1410(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num = iReader.ReadInt32();
    for (int index3 = 0; index3 < num; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    instance.SteamGameLanguage = iReader.ReadString();
    instance.Language = (Language) iReader.ReadInt32();
  }

  private void LoadSettings1370(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    instance.VSSettings.Read1370(iReader);
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num = iReader.ReadInt32();
    for (int index3 = 0; index3 < num; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    instance.SteamGameLanguage = iReader.ReadString();
    instance.Language = (Language) iReader.ReadInt32();
  }

  private void LoadSettings1362(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num2 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num2; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    instance.SteamGameLanguage = iReader.ReadString();
    instance.Language = (Language) iReader.ReadInt32();
  }

  private void LoadSettings1354(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num2 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num2; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    instance.SteamGameLanguage = iReader.ReadString();
    instance.Language = (Language) iReader.ReadInt32();
  }

  private void LoadSettings1350(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    KeyboardMouseController.Binding[] sourceArray = new KeyboardMouseController.Binding[17];
    for (int index = 0; index < 17; ++index)
    {
      sourceArray[index].IsMouse = iReader.ReadBoolean();
      sourceArray[index].Button = iReader.ReadByte();
    }
    Array.Copy((Array) sourceArray, (Array) KeyboardMouseController.mKeyboardBindings, sourceArray.Length);
    Array.Copy((Array) sourceArray, (Array) this.mKeyboardBindings, sourceArray.Length);
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num2 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num2; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    LanguageManager.Instance.SetLanguage((Language) iReader.ReadInt32());
  }

  private void LoadSettings1334(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    Microsoft.Xna.Framework.Input.Keys[] keysArray = new Microsoft.Xna.Framework.Input.Keys[13];
    for (int index = 0; index < 13; ++index)
      keysArray[index] = (Microsoft.Xna.Framework.Input.Keys) iReader.ReadInt32();
    if (keysArray[1] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[0] = new KeyboardMouseController.Binding(keysArray[1]);
      this.mKeyboardBindings[0] = new KeyboardMouseController.Binding(keysArray[1]);
    }
    if (keysArray[6] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[1] = new KeyboardMouseController.Binding(keysArray[6]);
      this.mKeyboardBindings[1] = new KeyboardMouseController.Binding(keysArray[6]);
    }
    if (keysArray[9] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[2] = new KeyboardMouseController.Binding(keysArray[7]);
      this.mKeyboardBindings[2] = new KeyboardMouseController.Binding(keysArray[7]);
    }
    if (keysArray[2] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[3] = new KeyboardMouseController.Binding(keysArray[2]);
      this.mKeyboardBindings[3] = new KeyboardMouseController.Binding(keysArray[2]);
    }
    if (keysArray[4] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[4] = new KeyboardMouseController.Binding(keysArray[4]);
      this.mKeyboardBindings[4] = new KeyboardMouseController.Binding(keysArray[4]);
    }
    if (keysArray[5] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[5] = new KeyboardMouseController.Binding(keysArray[5]);
      this.mKeyboardBindings[5] = new KeyboardMouseController.Binding(keysArray[5]);
    }
    if (keysArray[0] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[6] = new KeyboardMouseController.Binding(keysArray[0]);
      this.mKeyboardBindings[6] = new KeyboardMouseController.Binding(keysArray[0]);
    }
    if (keysArray[3] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[7] = new KeyboardMouseController.Binding(keysArray[3]);
      this.mKeyboardBindings[7] = new KeyboardMouseController.Binding(keysArray[3]);
    }
    if (keysArray[8] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[14] = new KeyboardMouseController.Binding(keysArray[8]);
      this.mKeyboardBindings[14] = new KeyboardMouseController.Binding(keysArray[8]);
    }
    if (keysArray[9] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[13] = new KeyboardMouseController.Binding(keysArray[9]);
      this.mKeyboardBindings[13] = new KeyboardMouseController.Binding(keysArray[9]);
    }
    if (keysArray[10] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[12] = new KeyboardMouseController.Binding(keysArray[10]);
      this.mKeyboardBindings[12] = new KeyboardMouseController.Binding(keysArray[10]);
    }
    if (keysArray[11] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[10] = new KeyboardMouseController.Binding(keysArray[11]);
      this.mKeyboardBindings[10] = new KeyboardMouseController.Binding(keysArray[11]);
    }
    if (keysArray[12] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[9] = new KeyboardMouseController.Binding(keysArray[12]);
      this.mKeyboardBindings[9] = new KeyboardMouseController.Binding(keysArray[12]);
    }
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num2 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num2; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    LanguageManager.Instance.SetLanguage((Language) iReader.ReadInt32());
  }

  private void LoadSettings1330(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    int num2 = (int) iReader.ReadUInt16();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    Microsoft.Xna.Framework.Input.Keys[] keysArray = new Microsoft.Xna.Framework.Input.Keys[13];
    for (int index = 0; index < 13; ++index)
      keysArray[index] = (Microsoft.Xna.Framework.Input.Keys) iReader.ReadInt32();
    if (keysArray[1] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[0] = new KeyboardMouseController.Binding(keysArray[1]);
      this.mKeyboardBindings[0] = new KeyboardMouseController.Binding(keysArray[1]);
    }
    if (keysArray[6] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[1] = new KeyboardMouseController.Binding(keysArray[6]);
      this.mKeyboardBindings[1] = new KeyboardMouseController.Binding(keysArray[6]);
    }
    if (keysArray[9] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[2] = new KeyboardMouseController.Binding(keysArray[7]);
      this.mKeyboardBindings[2] = new KeyboardMouseController.Binding(keysArray[7]);
    }
    if (keysArray[2] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[3] = new KeyboardMouseController.Binding(keysArray[2]);
      this.mKeyboardBindings[3] = new KeyboardMouseController.Binding(keysArray[2]);
    }
    if (keysArray[4] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[4] = new KeyboardMouseController.Binding(keysArray[4]);
      this.mKeyboardBindings[4] = new KeyboardMouseController.Binding(keysArray[4]);
    }
    if (keysArray[5] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[5] = new KeyboardMouseController.Binding(keysArray[5]);
      this.mKeyboardBindings[5] = new KeyboardMouseController.Binding(keysArray[5]);
    }
    if (keysArray[0] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[6] = new KeyboardMouseController.Binding(keysArray[0]);
      this.mKeyboardBindings[6] = new KeyboardMouseController.Binding(keysArray[0]);
    }
    if (keysArray[3] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[7] = new KeyboardMouseController.Binding(keysArray[3]);
      this.mKeyboardBindings[7] = new KeyboardMouseController.Binding(keysArray[3]);
    }
    if (keysArray[8] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[14] = new KeyboardMouseController.Binding(keysArray[8]);
      this.mKeyboardBindings[14] = new KeyboardMouseController.Binding(keysArray[8]);
    }
    if (keysArray[9] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[13] = new KeyboardMouseController.Binding(keysArray[9]);
      this.mKeyboardBindings[13] = new KeyboardMouseController.Binding(keysArray[9]);
    }
    if (keysArray[10] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[12] = new KeyboardMouseController.Binding(keysArray[10]);
      this.mKeyboardBindings[12] = new KeyboardMouseController.Binding(keysArray[10]);
    }
    if (keysArray[11] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[10] = new KeyboardMouseController.Binding(keysArray[11]);
      this.mKeyboardBindings[10] = new KeyboardMouseController.Binding(keysArray[11]);
    }
    if (keysArray[12] != Microsoft.Xna.Framework.Input.Keys.None)
    {
      KeyboardMouseController.mKeyboardBindings[9] = new KeyboardMouseController.Binding(keysArray[12]);
      this.mKeyboardBindings[9] = new KeyboardMouseController.Binding(keysArray[12]);
    }
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num3 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num3; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    LanguageManager.Instance.SetLanguage((Language) iReader.ReadInt32());
  }

  private void LoadSettings1290(BinaryReader iReader)
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.BloodAndGore = (SettingOptions) iReader.ReadByte();
    instance.DamageNumbers = (SettingOptions) iReader.ReadByte();
    instance.HealthBars = (SettingOptions) iReader.ReadByte();
    instance.SpellWheel = (SettingOptions) iReader.ReadByte();
    int num1 = (int) iReader.ReadByte();
    iReader.ReadInt32();
    iReader.ReadInt32();
    iReader.ReadBoolean();
    ResolutionData resolutionData;
    resolutionData.Width = (int) iReader.ReadInt16();
    resolutionData.Height = (int) iReader.ReadInt16();
    resolutionData.RefreshRate = (int) iReader.ReadInt16();
    instance.Resolution = resolutionData;
    instance.Fullscreen = iReader.ReadBoolean();
    instance.BloomQuality = (SettingOptions) iReader.ReadByte();
    instance.ShadowQuality = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = (SettingOptions) iReader.ReadByte();
    instance.DecalLimit = SettingOptions.Medium;
    instance.Particles = SettingOptions.Medium;
    instance.ParticleLights = true;
    instance.VolumeMusic = (int) iReader.ReadByte();
    instance.VolumeSound = (int) iReader.ReadByte();
    instance.GameName = iReader.ReadString();
    int num2 = (int) iReader.ReadUInt16();
    FilterData oData;
    FilterData.Read1400(iReader, out oData);
    instance.Filter = oData;
    for (int index1 = 0; index1 < 4; ++index1)
    {
      XInputController.Binding[] bindingArray = instance.XInputBindings[index1] ?? (instance.XInputBindings[index1] = new XInputController.Binding[24]);
      for (int index2 = 0; index2 < 24; ++index2)
      {
        bindingArray[index2].Type = (XInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index2].BindingIndex = iReader.ReadInt32();
      }
    }
    instance.DInputBindings.Clear();
    int num3 = iReader.ReadInt32();
    for (int index3 = 0; index3 < num3; ++index3)
    {
      Guid key = new Guid(iReader.ReadBytes(16 /*0x10*/));
      DirectInputController.Binding[] bindingArray = new DirectInputController.Binding[24];
      for (int index4 = 0; index4 < 24; ++index4)
      {
        bindingArray[index4].Type = (DirectInputController.Binding.BindingType) iReader.ReadByte();
        bindingArray[index4].BindingIndex = iReader.ReadInt32();
      }
      instance.DInputBindings.Add(key, bindingArray);
    }
    LanguageManager.Instance.SetLanguage((Language) iReader.ReadInt32());
  }

  public void LoadSettings()
  {
    string path = "./SaveData/Settings.sav";
    BinaryReader iReader = (BinaryReader) null;
    try
    {
      iReader = new BinaryReader((Stream) File.OpenRead(path));
      string[] strArray = iReader.ReadString().Split('.');
      ushort[] numArray = new ushort[4];
      for (int index = 0; index < strArray.Length; ++index)
        numArray[index] = ushort.Parse(strArray[index]);
      ulong num = (ulong) ((long) numArray[0] << 48 /*0x30*/ | (long) numArray[1] << 32 /*0x20*/ | (long) numArray[2] << 16 /*0x10*/) | (ulong) numArray[3];
      if (num >= 281492156776448UL /*0x01000400030000*/)
        this.LoadSettings1430(iReader);
      else if (num >= 281492156645376UL /*0x01000400010000*/)
        this.LoadSettings1410(iReader);
      else if (num >= 281487862071296UL /*0x01000300070000*/)
        this.LoadSettings1370(iReader);
      else if (num >= 281487862005762UL /*0x01000300060002*/)
        this.LoadSettings1362(iReader);
      else if (num >= 281487861940228UL /*0x01000300050004*/)
        this.LoadSettings1354(iReader);
      else if (num >= 281487861940224UL /*0x01000300050000*/)
        this.LoadSettings1350(iReader);
      else if (num >= 281487861809156UL /*0x01000300030004*/)
        this.LoadSettings1334(iReader);
      else if (num >= 281487861809152UL /*0x01000300030000*/)
        this.LoadSettings1330(iReader);
      else if (num >= 281483567235072UL /*0x01000200090000*/)
        this.LoadSettings1290(iReader);
      else
        this.LoadSettings1290(iReader);
    }
    catch
    {
      LanguageManager.Instance.SetLanguage(Language.eng);
      iReader?.Close();
    }
    iReader?.Close();
  }

  public void SaveSettings()
  {
    BinaryWriter iWriter = (BinaryWriter) null;
    string path = "./SaveData/Settings.sav";
    try
    {
      iWriter = new BinaryWriter((Stream) File.Create(path));
      GlobalSettings instance = GlobalSettings.Instance;
      iWriter.Write(Application.ProductVersion);
      iWriter.Write((byte) instance.BloodAndGore);
      iWriter.Write((byte) instance.DamageNumbers);
      iWriter.Write((byte) instance.HealthBars);
      iWriter.Write((byte) instance.SpellWheel);
      instance.VSSettings.Write(iWriter);
      iWriter.Write((short) instance.Resolution.Width);
      iWriter.Write((short) instance.Resolution.Height);
      iWriter.Write((short) instance.Resolution.RefreshRate);
      iWriter.Write(instance.Fullscreen);
      iWriter.Write((byte) instance.BloomQuality);
      iWriter.Write((byte) instance.ShadowQuality);
      iWriter.Write((byte) instance.DecalLimit);
      iWriter.Write((byte) instance.Particles);
      iWriter.Write(instance.ParticleLights);
      iWriter.Write((byte) instance.VolumeMusic);
      iWriter.Write((byte) instance.VolumeSound);
      iWriter.Write(instance.GameName);
      FilterData.Write(iWriter, instance.Filter);
      for (int index = 0; index < 17; ++index)
      {
        iWriter.Write(this.mKeyboardBindings[index].IsMouse);
        iWriter.Write(this.mKeyboardBindings[index].Button);
      }
      for (int index1 = 0; index1 < 4; ++index1)
      {
        XInputController.Binding[] xinputBinding = instance.XInputBindings[index1];
        for (int index2 = 0; index2 < 24; ++index2)
        {
          iWriter.Write((byte) xinputBinding[index2].Type);
          iWriter.Write(xinputBinding[index2].BindingIndex);
        }
      }
      iWriter.Write(instance.DInputBindings.Count);
      foreach (KeyValuePair<Guid, DirectInputController.Binding[]> dinputBinding in instance.DInputBindings)
      {
        iWriter.Write(dinputBinding.Key.ToByteArray());
        for (int index = 0; index < 24; ++index)
        {
          iWriter.Write((byte) dinputBinding.Value[index].Type);
          iWriter.Write(dinputBinding.Value[index].BindingIndex);
        }
      }
      iWriter.Write(GlobalSettings.Instance.SteamGameLanguage);
      iWriter.Write((int) LanguageManager.Instance.CurrentLanguage);
      iWriter.Write(GlobalSettings.Instance.VSync);
      iWriter.Flush();
      iWriter.Close();
    }
    catch
    {
      iWriter?.Close();
    }
  }

  public SaveData[] SaveSlots
  {
    get
    {
      if (this.mSaveSlots == null)
        this.LoadCampaign();
      return this.mSaveSlots;
    }
  }

  public SaveData[] MythosSaveSlots
  {
    get
    {
      if (this.mMythosSaveSlots == null)
        this.LoadCampaign();
      return this.mMythosSaveSlots;
    }
  }

  public void LoadCampaign()
  {
    BinaryReader iReader = (BinaryReader) null;
    try
    {
      if (this.mSaveSlots == null)
        this.mSaveSlots = new SaveData[3];
      iReader = new BinaryReader((Stream) File.OpenRead("./saveData/campaign.sav"));
      ulong iVersion = 0;
      if (iReader.ReadByte() == byte.MaxValue)
      {
        string[] strArray = iReader.ReadString().Split('.');
        ushort[] numArray = new ushort[4];
        for (int index = 0; index < strArray.Length; ++index)
          numArray[index] = ushort.Parse(strArray[index]);
        iVersion = (ulong) ((long) numArray[0] << 48 /*0x30*/ | (long) numArray[1] << 32 /*0x20*/ | (long) numArray[2] << 16 /*0x10*/) | (ulong) numArray[3];
      }
      else
        iReader.BaseStream.Seek(-1L, SeekOrigin.Current);
      for (int index = 0; index < this.mSaveSlots.Length; ++index)
      {
        if (iReader.ReadBoolean())
          this.mSaveSlots[index] = SaveData.Read(iVersion, iReader, this.mSaveSlots[index]);
      }
      iReader.Close();
    }
    catch
    {
      iReader?.Close();
    }
    try
    {
      if (this.mMythosSaveSlots == null)
        this.mMythosSaveSlots = new SaveData[3];
      iReader = new BinaryReader((Stream) File.OpenRead("./saveData/starscampaign.sav"));
      ulong iVersion = 0;
      if (iReader.ReadByte() == byte.MaxValue)
      {
        string[] strArray = iReader.ReadString().Split('.');
        ushort[] numArray = new ushort[4];
        for (int index = 0; index < strArray.Length; ++index)
          numArray[index] = ushort.Parse(strArray[index]);
        iVersion = (ulong) ((long) numArray[0] << 48 /*0x30*/ | (long) numArray[1] << 32 /*0x20*/ | (long) numArray[2] << 16 /*0x10*/) | (ulong) numArray[3];
      }
      else
        iReader.BaseStream.Seek(-1L, SeekOrigin.Current);
      for (int index = 0; index < this.mMythosSaveSlots.Length; ++index)
      {
        if (iReader.ReadBoolean())
          this.mMythosSaveSlots[index] = SaveData.Read(iVersion, iReader, this.mMythosSaveSlots[index]);
      }
      iReader.Close();
    }
    catch
    {
      iReader?.Close();
    }
  }

  public void SaveCampaign()
  {
    string str1 = "./saveData/campaign.tmp";
    BinaryWriter iWriter1 = new BinaryWriter((Stream) File.Create(str1));
    iWriter1.Write(byte.MaxValue);
    iWriter1.Write(Application.ProductVersion);
    for (int index = 0; index < 3; ++index)
    {
      iWriter1.Write(this.mSaveSlots[index] != null);
      if (this.mSaveSlots[index] != null)
        this.mSaveSlots[index].Write(iWriter1);
    }
    iWriter1.Flush();
    iWriter1.Close();
    string str2 = "./saveData/campaign.sav";
    if (File.Exists(str2))
      File.Delete(str2);
    File.Move(str1, str2);
    string str3 = "./saveData/starscampaign.tmp";
    BinaryWriter iWriter2 = new BinaryWriter((Stream) File.Create(str3));
    iWriter2.Write(byte.MaxValue);
    iWriter2.Write(Application.ProductVersion);
    for (int index = 0; index < 3; ++index)
    {
      iWriter2.Write(this.mMythosSaveSlots[index] != null);
      if (this.mMythosSaveSlots[index] != null)
        this.mMythosSaveSlots[index].Write(iWriter2);
    }
    iWriter2.Flush();
    iWriter2.Close();
    string str4 = "./saveData/starscampaign.sav";
    if (File.Exists(str4))
      File.Delete(str4);
    File.Move(str3, str4);
  }

  public void LoadLeaderBoards()
  {
    LevelNode[] challenges = LevelManager.Instance.Challenges;
    for (int iChallengeIndex = 0; iChallengeIndex < challenges.Length; ++iChallengeIndex)
    {
      string path = $"./SaveData/{challenges[iChallengeIndex].FileName}.sav";
      BinaryReader iReader = (BinaryReader) null;
      try
      {
        iReader = new BinaryReader((Stream) File.OpenRead(path));
        int num = iReader.ReadInt32();
        for (int index = 0; index < num; ++index)
        {
          LeaderBoardData iData = new LeaderBoardData();
          iData.Read(iReader);
          StatisticsManager.Instance.AddLocalEntry(iChallengeIndex, iData);
        }
      }
      catch
      {
        iReader?.Close();
      }
    }
    this.mAlreadyLoaded = true;
  }

  public void SaveLeaderBoards()
  {
    LevelNode[] challenges = LevelManager.Instance.Challenges;
    for (int iChallengeIndex = 0; iChallengeIndex < challenges.Length; ++iChallengeIndex)
    {
      BinaryWriter iWriter = new BinaryWriter((Stream) File.Create($"./SaveData/{challenges[iChallengeIndex].FileName}.sav"));
      List<LeaderBoardData> leaderBoardDataList = StatisticsManager.Instance.Leaderboard(iChallengeIndex);
      iWriter.Write(leaderBoardDataList.Count);
      for (int index = 0; index < leaderBoardDataList.Count; ++index)
        leaderBoardDataList[index].Writer(iWriter);
      iWriter.Flush();
      iWriter.Close();
    }
  }

  public void LoadPOPSData(ParadoxAccountSaveData iAccount)
  {
    lock (SaveManager.sFileLock)
    {
      if (!File.Exists("./saveData/pops.sav"))
        return;
      FileStream input = File.OpenRead("./saveData/pops.sav");
      BinaryReader iReader = new BinaryReader((Stream) input);
      iAccount.Read(iReader);
      iReader.Close();
      input.Close();
    }
  }

  public void SavePOPSData(ParadoxAccountSaveData iAccount)
  {
    lock (SaveManager.sFileLock)
    {
      FileStream output = File.Create("./saveData/pops.sav");
      BinaryWriter iWriter = new BinaryWriter((Stream) output);
      iAccount.Write(iWriter);
      iWriter.Flush();
      iWriter.Close();
      output.Close();
    }
  }
}
