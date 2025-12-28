using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Statistics;
using Magicka.GameLogic.UI;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework.Input;

namespace Magicka.Storage
{
	// Token: 0x020002C0 RID: 704
	internal class SaveManager
	{
		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x000867A4 File Offset: 0x000849A4
		public static SaveManager Instance
		{
			get
			{
				if (SaveManager.sSingelton == null)
				{
					lock (SaveManager.sSingeltonLock)
					{
						if (SaveManager.sSingelton == null)
						{
							SaveManager.sSingelton = new SaveManager();
						}
					}
				}
				return SaveManager.sSingelton;
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x000867F8 File Offset: 0x000849F8
		private SaveManager()
		{
			if (!Directory.Exists("./SaveData"))
			{
				Directory.CreateDirectory("./SaveData");
			}
			StreamReader streamReader = File.OpenText("content/Data/col.dat");
			int num;
			int.TryParse(streamReader.ReadLine(), out num);
			if (num <= 0)
			{
				num = 10;
			}
			Defines.PLAYERCOLORS_UNLOCKED = num;
			streamReader.Close();
			if (Defines.PLAYERCOLORS_UNLOCKED > Defines.PLAYERCOLORS.Length)
			{
				Defines.PLAYERCOLORS_UNLOCKED = Defines.PLAYERCOLORS.Length;
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x00086875 File Offset: 0x00084A75
		public bool AlreadyLoaded
		{
			get
			{
				return this.mAlreadyLoaded;
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001532 RID: 5426 RVA: 0x0008687D File Offset: 0x00084A7D
		public KeyboardMouseController.Binding[] KeyBindings
		{
			get
			{
				return this.mKeyboardBindings;
			}
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x00086888 File Offset: 0x00084A88
		private void LoadSettings1430(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			instance.VSSettings.Read1370(iReader);
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.Particles = (SettingOptions)iReader.ReadByte();
			instance.ParticleLights = iReader.ReadBoolean();
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1410(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			instance.SteamGameLanguage = iReader.ReadString();
			instance.Language = (Language)iReader.ReadInt32();
			instance.VSync = iReader.ReadBoolean();
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x00086AF4 File Offset: 0x00084CF4
		private void LoadSettings1410(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			instance.VSSettings.Read1370(iReader);
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1410(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			instance.SteamGameLanguage = iReader.ReadString();
			instance.Language = (Language)iReader.ReadInt32();
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x00086D50 File Offset: 0x00084F50
		private void LoadSettings1370(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			instance.VSSettings.Read1370(iReader);
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			instance.SteamGameLanguage = iReader.ReadString();
			instance.Language = (Language)iReader.ReadInt32();
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x00086FAC File Offset: 0x000851AC
		private void LoadSettings1362(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			instance.SteamGameLanguage = iReader.ReadString();
			instance.Language = (Language)iReader.ReadInt32();
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x00087218 File Offset: 0x00085418
		private void LoadSettings1354(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			instance.SteamGameLanguage = iReader.ReadString();
			instance.Language = (Language)iReader.ReadInt32();
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00087484 File Offset: 0x00085684
		private void LoadSettings1350(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			KeyboardMouseController.Binding[] array = new KeyboardMouseController.Binding[17];
			for (int i = 0; i < 17; i++)
			{
				array[i].IsMouse = iReader.ReadBoolean();
				array[i].Button = iReader.ReadByte();
			}
			Array.Copy(array, KeyboardMouseController.mKeyboardBindings, array.Length);
			Array.Copy(array, this.mKeyboardBindings, array.Length);
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			LanguageManager.Instance.SetLanguage((Language)iReader.ReadInt32());
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x000876E8 File Offset: 0x000858E8
		private void LoadSettings1334(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			Microsoft.Xna.Framework.Input.Keys[] array = new Microsoft.Xna.Framework.Input.Keys[13];
			for (int i = 0; i < 13; i++)
			{
				array[i] = (Microsoft.Xna.Framework.Input.Keys)iReader.ReadInt32();
			}
			if (array[1] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[0] = new KeyboardMouseController.Binding(array[1]);
				this.mKeyboardBindings[0] = new KeyboardMouseController.Binding(array[1]);
			}
			if (array[6] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[1] = new KeyboardMouseController.Binding(array[6]);
				this.mKeyboardBindings[1] = new KeyboardMouseController.Binding(array[6]);
			}
			if (array[9] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[2] = new KeyboardMouseController.Binding(array[7]);
				this.mKeyboardBindings[2] = new KeyboardMouseController.Binding(array[7]);
			}
			if (array[2] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[3] = new KeyboardMouseController.Binding(array[2]);
				this.mKeyboardBindings[3] = new KeyboardMouseController.Binding(array[2]);
			}
			if (array[4] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[4] = new KeyboardMouseController.Binding(array[4]);
				this.mKeyboardBindings[4] = new KeyboardMouseController.Binding(array[4]);
			}
			if (array[5] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[5] = new KeyboardMouseController.Binding(array[5]);
				this.mKeyboardBindings[5] = new KeyboardMouseController.Binding(array[5]);
			}
			if (array[0] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[6] = new KeyboardMouseController.Binding(array[0]);
				this.mKeyboardBindings[6] = new KeyboardMouseController.Binding(array[0]);
			}
			if (array[3] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[7] = new KeyboardMouseController.Binding(array[3]);
				this.mKeyboardBindings[7] = new KeyboardMouseController.Binding(array[3]);
			}
			if (array[8] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[14] = new KeyboardMouseController.Binding(array[8]);
				this.mKeyboardBindings[14] = new KeyboardMouseController.Binding(array[8]);
			}
			if (array[9] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[13] = new KeyboardMouseController.Binding(array[9]);
				this.mKeyboardBindings[13] = new KeyboardMouseController.Binding(array[9]);
			}
			if (array[10] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[12] = new KeyboardMouseController.Binding(array[10]);
				this.mKeyboardBindings[12] = new KeyboardMouseController.Binding(array[10]);
			}
			if (array[11] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[10] = new KeyboardMouseController.Binding(array[11]);
				this.mKeyboardBindings[10] = new KeyboardMouseController.Binding(array[11]);
			}
			if (array[12] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[9] = new KeyboardMouseController.Binding(array[12]);
				this.mKeyboardBindings[9] = new KeyboardMouseController.Binding(array[12]);
			}
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			LanguageManager.Instance.SetLanguage((Language)iReader.ReadInt32());
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x00087BE8 File Offset: 0x00085DE8
		private void LoadSettings1330(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			iReader.ReadUInt16();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			Microsoft.Xna.Framework.Input.Keys[] array = new Microsoft.Xna.Framework.Input.Keys[13];
			for (int i = 0; i < 13; i++)
			{
				array[i] = (Microsoft.Xna.Framework.Input.Keys)iReader.ReadInt32();
			}
			if (array[1] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[0] = new KeyboardMouseController.Binding(array[1]);
				this.mKeyboardBindings[0] = new KeyboardMouseController.Binding(array[1]);
			}
			if (array[6] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[1] = new KeyboardMouseController.Binding(array[6]);
				this.mKeyboardBindings[1] = new KeyboardMouseController.Binding(array[6]);
			}
			if (array[9] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[2] = new KeyboardMouseController.Binding(array[7]);
				this.mKeyboardBindings[2] = new KeyboardMouseController.Binding(array[7]);
			}
			if (array[2] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[3] = new KeyboardMouseController.Binding(array[2]);
				this.mKeyboardBindings[3] = new KeyboardMouseController.Binding(array[2]);
			}
			if (array[4] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[4] = new KeyboardMouseController.Binding(array[4]);
				this.mKeyboardBindings[4] = new KeyboardMouseController.Binding(array[4]);
			}
			if (array[5] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[5] = new KeyboardMouseController.Binding(array[5]);
				this.mKeyboardBindings[5] = new KeyboardMouseController.Binding(array[5]);
			}
			if (array[0] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[6] = new KeyboardMouseController.Binding(array[0]);
				this.mKeyboardBindings[6] = new KeyboardMouseController.Binding(array[0]);
			}
			if (array[3] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[7] = new KeyboardMouseController.Binding(array[3]);
				this.mKeyboardBindings[7] = new KeyboardMouseController.Binding(array[3]);
			}
			if (array[8] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[14] = new KeyboardMouseController.Binding(array[8]);
				this.mKeyboardBindings[14] = new KeyboardMouseController.Binding(array[8]);
			}
			if (array[9] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[13] = new KeyboardMouseController.Binding(array[9]);
				this.mKeyboardBindings[13] = new KeyboardMouseController.Binding(array[9]);
			}
			if (array[10] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[12] = new KeyboardMouseController.Binding(array[10]);
				this.mKeyboardBindings[12] = new KeyboardMouseController.Binding(array[10]);
			}
			if (array[11] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[10] = new KeyboardMouseController.Binding(array[11]);
				this.mKeyboardBindings[10] = new KeyboardMouseController.Binding(array[11]);
			}
			if (array[12] != Microsoft.Xna.Framework.Input.Keys.None)
			{
				KeyboardMouseController.mKeyboardBindings[9] = new KeyboardMouseController.Binding(array[12]);
				this.mKeyboardBindings[9] = new KeyboardMouseController.Binding(array[12]);
			}
			for (int j = 0; j < 4; j++)
			{
				XInputController.Binding[] array2 = instance.XInputBindings[j];
				if (array2 == null)
				{
					array2 = (instance.XInputBindings[j] = new XInputController.Binding[24]);
				}
				for (int k = 0; k < 24; k++)
				{
					array2[k].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array2[k].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array3 = new DirectInputController.Binding[24];
				for (int m = 0; m < 24; m++)
				{
					array3[m].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array3[m].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array3);
			}
			LanguageManager.Instance.SetLanguage((Language)iReader.ReadInt32());
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x000880F0 File Offset: 0x000862F0
		private void LoadSettings1290(BinaryReader iReader)
		{
			GlobalSettings instance = GlobalSettings.Instance;
			instance.BloodAndGore = (SettingOptions)iReader.ReadByte();
			instance.DamageNumbers = (SettingOptions)iReader.ReadByte();
			instance.HealthBars = (SettingOptions)iReader.ReadByte();
			instance.SpellWheel = (SettingOptions)iReader.ReadByte();
			iReader.ReadByte();
			iReader.ReadInt32();
			iReader.ReadInt32();
			iReader.ReadBoolean();
			ResolutionData resolution;
			resolution.Width = (int)iReader.ReadInt16();
			resolution.Height = (int)iReader.ReadInt16();
			resolution.RefreshRate = (int)iReader.ReadInt16();
			instance.Resolution = resolution;
			instance.Fullscreen = iReader.ReadBoolean();
			instance.BloomQuality = (SettingOptions)iReader.ReadByte();
			instance.ShadowQuality = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = (SettingOptions)iReader.ReadByte();
			instance.DecalLimit = SettingOptions.Medium;
			instance.Particles = SettingOptions.Medium;
			instance.ParticleLights = true;
			instance.VolumeMusic = (int)iReader.ReadByte();
			instance.VolumeSound = (int)iReader.ReadByte();
			instance.GameName = iReader.ReadString();
			iReader.ReadUInt16();
			FilterData filter;
			FilterData.Read1400(iReader, out filter);
			instance.Filter = filter;
			for (int i = 0; i < 4; i++)
			{
				XInputController.Binding[] array = instance.XInputBindings[i];
				if (array == null)
				{
					array = (instance.XInputBindings[i] = new XInputController.Binding[24]);
				}
				for (int j = 0; j < 24; j++)
				{
					array[j].Type = (XInputController.Binding.BindingType)iReader.ReadByte();
					array[j].BindingIndex = iReader.ReadInt32();
				}
			}
			instance.DInputBindings.Clear();
			int num = iReader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				Guid key = new Guid(iReader.ReadBytes(16));
				DirectInputController.Binding[] array2 = new DirectInputController.Binding[24];
				for (int l = 0; l < 24; l++)
				{
					array2[l].Type = (DirectInputController.Binding.BindingType)iReader.ReadByte();
					array2[l].BindingIndex = iReader.ReadInt32();
				}
				instance.DInputBindings.Add(key, array2);
			}
			LanguageManager.Instance.SetLanguage((Language)iReader.ReadInt32());
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x000882FC File Offset: 0x000864FC
		public void LoadSettings()
		{
			string path = "./SaveData/Settings.sav";
			BinaryReader binaryReader = null;
			try
			{
				binaryReader = new BinaryReader(File.OpenRead(path));
				string text = binaryReader.ReadString();
				string[] array = text.Split(new char[]
				{
					'.'
				});
				ushort[] array2 = new ushort[4];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = ushort.Parse(array[i]);
				}
				ulong num = (ulong)array2[0] << 48 | (ulong)array2[1] << 32 | (ulong)array2[2] << 16 | (ulong)array2[3];
				if (num >= 281492156776448UL)
				{
					this.LoadSettings1430(binaryReader);
				}
				else if (num >= 281492156645376UL)
				{
					this.LoadSettings1410(binaryReader);
				}
				else if (num >= 281487862071296UL)
				{
					this.LoadSettings1370(binaryReader);
				}
				else if (num >= 281487862005762UL)
				{
					this.LoadSettings1362(binaryReader);
				}
				else if (num >= 281487861940228UL)
				{
					this.LoadSettings1354(binaryReader);
				}
				else if (num >= 281487861940224UL)
				{
					this.LoadSettings1350(binaryReader);
				}
				else if (num >= 281487861809156UL)
				{
					this.LoadSettings1334(binaryReader);
				}
				else if (num >= 281487861809152UL)
				{
					this.LoadSettings1330(binaryReader);
				}
				else if (num >= 281483567235072UL)
				{
					this.LoadSettings1290(binaryReader);
				}
				else
				{
					this.LoadSettings1290(binaryReader);
				}
			}
			catch
			{
				LanguageManager.Instance.SetLanguage(Language.eng);
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
			}
			if (binaryReader != null)
			{
				binaryReader.Close();
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x0008849C File Offset: 0x0008669C
		public void SaveSettings()
		{
			BinaryWriter binaryWriter = null;
			string path = "./SaveData/Settings.sav";
			try
			{
				binaryWriter = new BinaryWriter(File.Create(path));
				GlobalSettings instance = GlobalSettings.Instance;
				binaryWriter.Write(Application.ProductVersion);
				binaryWriter.Write((byte)instance.BloodAndGore);
				binaryWriter.Write((byte)instance.DamageNumbers);
				binaryWriter.Write((byte)instance.HealthBars);
				binaryWriter.Write((byte)instance.SpellWheel);
				instance.VSSettings.Write(binaryWriter);
				binaryWriter.Write((short)instance.Resolution.Width);
				binaryWriter.Write((short)instance.Resolution.Height);
				binaryWriter.Write((short)instance.Resolution.RefreshRate);
				binaryWriter.Write(instance.Fullscreen);
				binaryWriter.Write((byte)instance.BloomQuality);
				binaryWriter.Write((byte)instance.ShadowQuality);
				binaryWriter.Write((byte)instance.DecalLimit);
				binaryWriter.Write((byte)instance.Particles);
				binaryWriter.Write(instance.ParticleLights);
				binaryWriter.Write((byte)instance.VolumeMusic);
				binaryWriter.Write((byte)instance.VolumeSound);
				binaryWriter.Write(instance.GameName);
				FilterData.Write(binaryWriter, instance.Filter);
				for (int i = 0; i < 17; i++)
				{
					binaryWriter.Write(this.mKeyboardBindings[i].IsMouse);
					binaryWriter.Write(this.mKeyboardBindings[i].Button);
				}
				for (int j = 0; j < 4; j++)
				{
					XInputController.Binding[] array = instance.XInputBindings[j];
					for (int k = 0; k < 24; k++)
					{
						binaryWriter.Write((byte)array[k].Type);
						binaryWriter.Write(array[k].BindingIndex);
					}
				}
				binaryWriter.Write(instance.DInputBindings.Count);
				foreach (KeyValuePair<Guid, DirectInputController.Binding[]> keyValuePair in instance.DInputBindings)
				{
					binaryWriter.Write(keyValuePair.Key.ToByteArray());
					for (int l = 0; l < 24; l++)
					{
						binaryWriter.Write((byte)keyValuePair.Value[l].Type);
						binaryWriter.Write(keyValuePair.Value[l].BindingIndex);
					}
				}
				binaryWriter.Write(GlobalSettings.Instance.SteamGameLanguage);
				binaryWriter.Write((int)LanguageManager.Instance.CurrentLanguage);
				binaryWriter.Write(GlobalSettings.Instance.VSync);
				binaryWriter.Flush();
				binaryWriter.Close();
			}
			catch
			{
				if (binaryWriter != null)
				{
					binaryWriter.Close();
				}
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x0600153E RID: 5438 RVA: 0x0008876C File Offset: 0x0008696C
		public SaveData[] SaveSlots
		{
			get
			{
				if (this.mSaveSlots == null)
				{
					this.LoadCampaign();
				}
				return this.mSaveSlots;
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x00088782 File Offset: 0x00086982
		public SaveData[] MythosSaveSlots
		{
			get
			{
				if (this.mMythosSaveSlots == null)
				{
					this.LoadCampaign();
				}
				return this.mMythosSaveSlots;
			}
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00088798 File Offset: 0x00086998
		public void LoadCampaign()
		{
			BinaryReader binaryReader = null;
			try
			{
				if (this.mSaveSlots == null)
				{
					this.mSaveSlots = new SaveData[3];
				}
				string path = "./saveData/campaign.sav";
				binaryReader = new BinaryReader(File.OpenRead(path));
				ulong iVersion = 0UL;
				byte b = binaryReader.ReadByte();
				if (b == 255)
				{
					string text = binaryReader.ReadString();
					string[] array = text.Split(new char[]
					{
						'.'
					});
					ushort[] array2 = new ushort[4];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = ushort.Parse(array[i]);
					}
					iVersion = ((ulong)array2[0] << 48 | (ulong)array2[1] << 32 | (ulong)array2[2] << 16 | (ulong)array2[3]);
				}
				else
				{
					binaryReader.BaseStream.Seek(-1L, SeekOrigin.Current);
				}
				for (int j = 0; j < this.mSaveSlots.Length; j++)
				{
					if (binaryReader.ReadBoolean())
					{
						this.mSaveSlots[j] = SaveData.Read(iVersion, binaryReader, this.mSaveSlots[j]);
					}
				}
				binaryReader.Close();
			}
			catch
			{
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
			}
			try
			{
				if (this.mMythosSaveSlots == null)
				{
					this.mMythosSaveSlots = new SaveData[3];
				}
				string path2 = "./saveData/starscampaign.sav";
				binaryReader = new BinaryReader(File.OpenRead(path2));
				ulong iVersion2 = 0UL;
				byte b2 = binaryReader.ReadByte();
				if (b2 == 255)
				{
					string text2 = binaryReader.ReadString();
					string[] array3 = text2.Split(new char[]
					{
						'.'
					});
					ushort[] array4 = new ushort[4];
					for (int k = 0; k < array3.Length; k++)
					{
						array4[k] = ushort.Parse(array3[k]);
					}
					iVersion2 = ((ulong)array4[0] << 48 | (ulong)array4[1] << 32 | (ulong)array4[2] << 16 | (ulong)array4[3]);
				}
				else
				{
					binaryReader.BaseStream.Seek(-1L, SeekOrigin.Current);
				}
				for (int l = 0; l < this.mMythosSaveSlots.Length; l++)
				{
					if (binaryReader.ReadBoolean())
					{
						this.mMythosSaveSlots[l] = SaveData.Read(iVersion2, binaryReader, this.mMythosSaveSlots[l]);
					}
				}
				binaryReader.Close();
			}
			catch
			{
				if (binaryReader != null)
				{
					binaryReader.Close();
				}
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x000889CC File Offset: 0x00086BCC
		public void SaveCampaign()
		{
			string text = "./saveData/campaign.tmp";
			BinaryWriter binaryWriter = new BinaryWriter(File.Create(text));
			binaryWriter.Write(byte.MaxValue);
			binaryWriter.Write(Application.ProductVersion);
			for (int i = 0; i < 3; i++)
			{
				binaryWriter.Write(this.mSaveSlots[i] != null);
				if (this.mSaveSlots[i] != null)
				{
					this.mSaveSlots[i].Write(binaryWriter);
				}
			}
			binaryWriter.Flush();
			binaryWriter.Close();
			string text2 = "./saveData/campaign.sav";
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(text, text2);
			text = "./saveData/starscampaign.tmp";
			binaryWriter = new BinaryWriter(File.Create(text));
			binaryWriter.Write(byte.MaxValue);
			binaryWriter.Write(Application.ProductVersion);
			for (int j = 0; j < 3; j++)
			{
				binaryWriter.Write(this.mMythosSaveSlots[j] != null);
				if (this.mMythosSaveSlots[j] != null)
				{
					this.mMythosSaveSlots[j].Write(binaryWriter);
				}
			}
			binaryWriter.Flush();
			binaryWriter.Close();
			text2 = "./saveData/starscampaign.sav";
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(text, text2);
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00088AF0 File Offset: 0x00086CF0
		public void LoadLeaderBoards()
		{
			LevelNode[] challenges = LevelManager.Instance.Challenges;
			for (int i = 0; i < challenges.Length; i++)
			{
				string path = "./SaveData/" + challenges[i].FileName + ".sav";
				BinaryReader binaryReader = null;
				try
				{
					binaryReader = new BinaryReader(File.OpenRead(path));
					int num = binaryReader.ReadInt32();
					for (int j = 0; j < num; j++)
					{
						LeaderBoardData iData = default(LeaderBoardData);
						iData.Read(binaryReader);
						StatisticsManager.Instance.AddLocalEntry(i, iData);
					}
				}
				catch
				{
					if (binaryReader != null)
					{
						binaryReader.Close();
					}
				}
			}
			this.mAlreadyLoaded = true;
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00088B98 File Offset: 0x00086D98
		public void SaveLeaderBoards()
		{
			LevelNode[] challenges = LevelManager.Instance.Challenges;
			for (int i = 0; i < challenges.Length; i++)
			{
				string path = "./SaveData/" + challenges[i].FileName + ".sav";
				BinaryWriter binaryWriter = new BinaryWriter(File.Create(path));
				List<LeaderBoardData> list = StatisticsManager.Instance.Leaderboard(i);
				binaryWriter.Write(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Writer(binaryWriter);
				}
				binaryWriter.Flush();
				binaryWriter.Close();
			}
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00088C34 File Offset: 0x00086E34
		public void LoadPOPSData(ParadoxAccountSaveData iAccount)
		{
			lock (SaveManager.sFileLock)
			{
				if (File.Exists("./saveData/pops.sav"))
				{
					FileStream fileStream = File.OpenRead("./saveData/pops.sav");
					BinaryReader binaryReader = new BinaryReader(fileStream);
					iAccount.Read(binaryReader);
					binaryReader.Close();
					fileStream.Close();
				}
			}
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x00088C9C File Offset: 0x00086E9C
		public void SavePOPSData(ParadoxAccountSaveData iAccount)
		{
			lock (SaveManager.sFileLock)
			{
				FileStream fileStream = File.Create("./saveData/pops.sav");
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				iAccount.Write(binaryWriter);
				binaryWriter.Flush();
				binaryWriter.Close();
				fileStream.Close();
			}
		}

		// Token: 0x040016D6 RID: 5846
		private const string POPS_SAVE_FILE_NAME = "./saveData/pops.sav";

		// Token: 0x040016D7 RID: 5847
		private static SaveManager sSingelton;

		// Token: 0x040016D8 RID: 5848
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040016D9 RID: 5849
		private static volatile object sFileLock = new object();

		// Token: 0x040016DA RID: 5850
		private bool mAlreadyLoaded;

		// Token: 0x040016DB RID: 5851
		private SaveData[] mMythosSaveSlots;

		// Token: 0x040016DC RID: 5852
		private SaveData[] mSaveSlots;

		// Token: 0x040016DD RID: 5853
		private KeyboardMouseController.Binding[] mKeyboardBindings = new KeyboardMouseController.Binding[17];
	}
}
