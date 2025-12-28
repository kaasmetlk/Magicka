using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;
using SteamWrapper;
using XNAnimation.Effects;

namespace Magicka
{
	// Token: 0x02000020 RID: 32
	internal static class Helper
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00006FEC File Offset: 0x000051EC
		internal static bool GetDLC(uint iID, out Helper.DLC oDLC)
		{
			for (int i = 0; i < Helper.DLC_ID_LOOKUP.Length; i++)
			{
				if (iID == Helper.DLC_ID_LOOKUP[i])
				{
					oDLC = (Helper.DLC)i;
					return true;
				}
			}
			oDLC = Helper.DLC.WizardHat;
			return false;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000701F File Offset: 0x0000521F
		internal static uint GetID(Helper.DLC iDLC)
		{
			return Helper.DLC_ID_LOOKUP[(int)iDLC];
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007028 File Offset: 0x00005228
		internal static bool CheckMagickDLC(MagickType iType)
		{
			if (iType != MagickType.MeteorS)
			{
				return iType != MagickType.Napalm || Helper.CheckDLC(Helper.DLC.Vietnam);
			}
			return Helper.CheckDLC(Helper.DLC.WizardHat);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007052 File Offset: 0x00005252
		internal static bool CheckDLCID(uint iAppID)
		{
			if (iAppID == SteamUtils.GetAppID())
			{
				return true;
			}
			if (iAppID == 901679U)
			{
				return Helper.CheckDLCID(73054U) & Helper.CheckDLCID(73055U) & Helper.CheckDLCID(73056U);
			}
			return SteamApps.BIsSubscribedApp(iAppID);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007090 File Offset: 0x00005290
		internal static bool CheckDLC(Helper.DLC iDLC)
		{
			uint iAppID = Helper.DLC_ID_LOOKUP[(int)iDLC];
			return Helper.CheckDLCID(iAppID);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000070AC File Offset: 0x000052AC
		public static void Swap<T>(ref T iA, ref T iB)
		{
			T t = iA;
			iA = iB;
			iB = t;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000070D4 File Offset: 0x000052D4
		public unsafe static DamageResult CircleDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, ref Vector3 iOrigin, float iRange, ref Damage iDamage)
		{
			Vector3 vector = default(Vector3);
			vector.Z = -1f;
			fixed (Damage* ptr = &iDamage)
			{
				return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref vector, iRange, 3.1415927f, ptr, 1);
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007114 File Offset: 0x00005314
		public unsafe static DamageResult CircleDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, ref Vector3 iOrigin, float iRange, ref DamageCollection5 iDamage)
		{
			Vector3 vector = default(Vector3);
			vector.Z = -1f;
			fixed (Damage* ptr = &iDamage.A)
			{
				return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref vector, iRange, 3.1415927f, ptr, 5);
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007158 File Offset: 0x00005358
		public unsafe static DamageResult ArcDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, ref Vector3 iOrigin, ref Vector3 iDirection, float iRange, float iSpread, ref Damage iDamage)
		{
			fixed (Damage* ptr = &iDamage)
			{
				return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, iSpread, ptr, 1);
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007180 File Offset: 0x00005380
		public unsafe static DamageResult ArcDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, ref Vector3 iOrigin, ref Vector3 iDirection, float iRange, float iSpread, ref DamageCollection5 iDamage)
		{
			fixed (Damage* ptr = &iDamage.A)
			{
				return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, iSpread, ptr, 5);
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000071B0 File Offset: 0x000053B0
		private unsafe static DamageResult ArcDamage(PlayState iPlayState, Entity iOwner, double iTimeStamp, Entity iIgnoreEntity, ref Vector3 iOrigin, ref Vector3 iDirection, float iRange, float iSpread, Damage* iDamage, int iNrOfDamages)
		{
			iSpread = Math.Min(iSpread, 3.1415927f);
			DamageResult damageResult = DamageResult.None;
			bool flag = false;
			for (int i = 0; i < iNrOfDamages; i++)
			{
				flag |= ((iDamage[i].Element & (Elements.Water | Elements.Steam)) == (Elements.Water | Elements.Steam));
			}
			float num = (float)Math.Cos((double)iSpread);
			List<Entity> entities = iPlayState.EntityManager.GetEntities(iOrigin, iRange, true);
			entities.Remove(iIgnoreEntity);
			for (int j = 0; j < entities.Count; j++)
			{
				IDamageable damageable = entities[j] as IDamageable;
				if (damageable != null && (!(damageable is Character) || !(damageable as Character).IsEthereal))
				{
					Vector3 position = damageable.Position;
					float num2;
					Vector3.Distance(ref iOrigin, ref position, out num2);
					Vector3 vector;
					Vector3.Subtract(ref position, ref iOrigin, out vector);
					vector.Y = 0f;
					float num3 = vector.Length();
					Vector3.Divide(ref vector, num3, out vector);
					if (num3 < 1E-06f || Vector3.Dot(vector, iDirection) >= num)
					{
						if (flag)
						{
							damageable.Electrocute(iOwner as IDamageable, 1f);
						}
						float num4 = 1f - (num2 - damageable.Radius) / iRange;
						if (num4 > 0f)
						{
							for (int k = 0; k < iNrOfDamages; k++)
							{
								Damage iDamage2 = iDamage[k];
								iDamage2.Magnitude *= num4;
								damageResult |= damageable.Damage(iDamage2, iOwner, iTimeStamp, iOrigin);
							}
						}
					}
				}
			}
			iPlayState.EntityManager.ReturnEntityList(entities);
			Vector3.Multiply(ref iDirection, iRange, out iDirection);
			Liquid.Freeze(iPlayState.Level.CurrentScene, ref iOrigin, ref iDirection, iSpread, 1f, iDamage, iNrOfDamages);
			return damageResult;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007384 File Offset: 0x00005584
		public static DamageResult Damage(this IDamageable t, Damage iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition)
		{
			if (iAttacker is Character)
			{
				float num;
				float num2;
				(iAttacker as Character).GetDamageModifier(iDamage.Element, out num, out num2);
				iDamage.Amount *= num;
				iDamage.Amount += num2;
			}
			Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNKE;
			Avatar avatar = iAttacker as Avatar;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				if (avatar == null)
				{
					iFeatures = Defines.DamageFeatures.NKE;
				}
				else if (!(avatar.Player.Gamer is NetworkGamer))
				{
					DamageRequestMessage damageRequestMessage = default(DamageRequestMessage);
					damageRequestMessage.TargetHandle = t.Handle;
					damageRequestMessage.AttackerHandle = avatar.Handle;
					Vector3 position = t.Position;
					Vector3.Subtract(ref iAttackPosition, ref position, out damageRequestMessage.RelativeAttackPosition);
					damageRequestMessage.Damage.A = iDamage;
					damageRequestMessage.TimeStamp = iTimeStamp;
					NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref damageRequestMessage);
					iFeatures = Defines.DamageFeatures.NKE;
				}
				else
				{
					iFeatures = Defines.DamageFeatures.Effects;
				}
			}
			else if (NetworkManager.Instance.State == NetworkState.Server && avatar != null)
			{
				if (avatar.Player.Gamer is NetworkGamer)
				{
					iFeatures = Defines.DamageFeatures.Effects;
				}
				else
				{
					DamageRequestMessage damageRequestMessage2 = default(DamageRequestMessage);
					damageRequestMessage2.TargetHandle = t.Handle;
					damageRequestMessage2.AttackerHandle = iAttacker.Handle;
					Vector3 position2 = t.Position;
					Vector3.Subtract(ref iAttackPosition, ref position2, out damageRequestMessage2.RelativeAttackPosition);
					damageRequestMessage2.Damage.A = iDamage;
					damageRequestMessage2.TimeStamp = iTimeStamp;
					NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref damageRequestMessage2);
				}
			}
			return t.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007508 File Offset: 0x00005708
		public static DamageResult Damage(this IDamageable t, DamageCollection5 iDamage, Entity iAttacker, double iTimeStamp, Vector3 iAttackPosition)
		{
			if (iAttacker is Character)
			{
				float num;
				float num2;
				(iAttacker as Character).GetDamageModifier(iDamage.A.Element, out num, out num2);
				iDamage.A.Amount = iDamage.A.Amount * num;
				iDamage.A.Amount = iDamage.A.Amount + num2;
				(iAttacker as Character).GetDamageModifier(iDamage.B.Element, out num, out num2);
				iDamage.B.Amount = iDamage.B.Amount * num;
				iDamage.B.Amount = iDamage.B.Amount + num2;
				(iAttacker as Character).GetDamageModifier(iDamage.C.Element, out num, out num2);
				iDamage.C.Amount = iDamage.C.Amount * num;
				iDamage.C.Amount = iDamage.C.Amount + num2;
				(iAttacker as Character).GetDamageModifier(iDamage.D.Element, out num, out num2);
				iDamage.D.Amount = iDamage.D.Amount * num;
				iDamage.D.Amount = iDamage.D.Amount + num2;
				(iAttacker as Character).GetDamageModifier(iDamage.E.Element, out num, out num2);
				iDamage.E.Amount = iDamage.E.Amount * num;
				iDamage.E.Amount = iDamage.E.Amount + num2;
			}
			Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNKE;
			Avatar avatar = iAttacker as Avatar;
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				if (avatar == null)
				{
					iFeatures = Defines.DamageFeatures.NKE;
				}
				else if (!(avatar.Player.Gamer is NetworkGamer))
				{
					DamageRequestMessage damageRequestMessage = default(DamageRequestMessage);
					damageRequestMessage.TargetHandle = t.Handle;
					damageRequestMessage.AttackerHandle = avatar.Handle;
					Vector3 position = t.Position;
					Vector3.Subtract(ref iAttackPosition, ref position, out damageRequestMessage.RelativeAttackPosition);
					damageRequestMessage.TimeStamp = iTimeStamp;
					damageRequestMessage.Damage = iDamage;
					NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref damageRequestMessage);
					iFeatures = Defines.DamageFeatures.NKE;
				}
				else
				{
					iFeatures = Defines.DamageFeatures.Effects;
				}
			}
			else if (NetworkManager.Instance.State == NetworkState.Server && avatar != null)
			{
				if (avatar.Player.Gamer is NetworkGamer)
				{
					iFeatures = Defines.DamageFeatures.Effects;
				}
				else
				{
					DamageRequestMessage damageRequestMessage2 = default(DamageRequestMessage);
					damageRequestMessage2.TargetHandle = t.Handle;
					damageRequestMessage2.AttackerHandle = avatar.Handle;
					Vector3 position2 = t.Position;
					Vector3.Subtract(ref iAttackPosition, ref position2, out damageRequestMessage2.RelativeAttackPosition);
					damageRequestMessage2.Damage = iDamage;
					damageRequestMessage2.TimeStamp = iTimeStamp;
					NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref damageRequestMessage2);
				}
			}
			return t.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000077A0 File Offset: 0x000059A0
		public static void SkinnedModelDeferredMaterialFromBasicEffect(SkinnedModelBasicEffect iEffect, out SkinnedModelDeferredAdvancedMaterial oMaterial)
		{
			oMaterial = default(SkinnedModelDeferredAdvancedMaterial);
			oMaterial.Alpha = iEffect.Alpha;
			oMaterial.Bloat = iEffect.Bloat;
			oMaterial.Colorize = iEffect.Colorize;
			oMaterial.CubeMapRotation = iEffect.CubeMapRotation;
			oMaterial.Damage = iEffect.Damage;
			oMaterial.DamageMap0 = iEffect.DamageMap0;
			oMaterial.DamageMap0Enabled = iEffect.DamageMap0Enabled;
			oMaterial.DamageMap1 = iEffect.DamageMap1;
			oMaterial.DamageMap1Enabled = iEffect.DamageMap1Enabled;
			oMaterial.DiffuseColor = iEffect.DiffuseColor;
			oMaterial.DiffuseMap0 = iEffect.DiffuseMap0;
			oMaterial.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
			oMaterial.DiffuseMap1 = iEffect.DiffuseMap1;
			oMaterial.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
			oMaterial.EmissiveAmount = iEffect.EmissiveAmount;
			oMaterial.NormalMap = iEffect.NormalMap;
			oMaterial.NormalMapEnabled = iEffect.NormalMapEnabled;
			oMaterial.NormalPower = iEffect.NormalPower;
			oMaterial.ProjectionMapMatrix = iEffect.Projection;
			oMaterial.SpecularAmount = iEffect.SpecularAmount;
			oMaterial.SpecularMapEnabled = iEffect.SpecularMapEnabled;
			oMaterial.SpecularPower = iEffect.SpecularPower;
			oMaterial.TintColor = iEffect.TintColor;
			oMaterial.UseSoftLightBlend = iEffect.UseSoftLightBlend;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000078D4 File Offset: 0x00005AD4
		public static void SkinnedModelDeferredMaterialFromBasicEffect(SkinnedModelBasicEffect iEffect, out SkinnedModelDeferredBasicMaterial oMaterial)
		{
			oMaterial.DiffuseColor = iEffect.DiffuseColor;
			oMaterial.TintColor = iEffect.TintColor;
			oMaterial.EmissiveAmount = iEffect.EmissiveAmount;
			oMaterial.SpecularAmount = iEffect.SpecularAmount;
			oMaterial.SpecularPower = iEffect.SpecularPower;
			oMaterial.Alpha = iEffect.Alpha;
			oMaterial.NormalPower = iEffect.NormalPower;
			oMaterial.Damage = iEffect.Damage;
			oMaterial.OverlayAlpha = iEffect.Diffuse1Alpha;
			oMaterial.DiffuseMap0Enabled = iEffect.DiffuseMap0Enabled;
			oMaterial.DiffuseMap1Enabled = iEffect.DiffuseMap1Enabled;
			oMaterial.SpecularMapEnabled = iEffect.SpecularMapEnabled;
			oMaterial.NormalMapEnabled = iEffect.NormalMapEnabled;
			oMaterial.DamageMap0Enabled = iEffect.DamageMap0Enabled;
			oMaterial.DamageMap1Enabled = iEffect.DamageMap1Enabled;
			oMaterial.UseSoftLightBlend = iEffect.UseSoftLightBlend;
			oMaterial.DiffuseMap0 = iEffect.DiffuseMap0;
			oMaterial.DiffuseMap1 = iEffect.DiffuseMap1;
			oMaterial.NormalMap = iEffect.NormalMap;
			oMaterial.MaterialMap = iEffect.SpecularMap;
			oMaterial.DamageMap0 = iEffect.DamageMap0;
			oMaterial.DamageMap1 = iEffect.DamageMap1;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000079EC File Offset: 0x00005BEC
		public static Texture2D TextureFromURL(string iURL, SurfaceFormat iFormat)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(iURL);
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			MemoryStream memoryStream = new MemoryStream();
			byte[] array = new byte[16384];
			for (;;)
			{
				int num = responseStream.Read(array, 0, array.Length);
				if (num <= 0)
				{
					break;
				}
				memoryStream.Write(array, 0, num);
			}
			responseStream.Close();
			responseStream.Dispose();
			memoryStream.Position = 0L;
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			TextureCreationParameters @default = TextureCreationParameters.Default;
			@default.Format = iFormat;
			Texture2D result;
			lock (graphicsDevice)
			{
				result = Texture2D.FromFile(graphicsDevice, memoryStream, @default);
			}
			memoryStream.Close();
			memoryStream.Dispose();
			return result;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00007ABC File Offset: 0x00005CBC
		public static bool ArrayEquals(byte[] iA, byte[] iB)
		{
			if (iA.Length != iB.Length)
			{
				return false;
			}
			for (int i = 0; i < iA.Length; i++)
			{
				if (iA[i] != iB[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00007AEC File Offset: 0x00005CEC
		public static int CountSetBits(uint iField)
		{
			return (int)(Helper.BitsSetTable256[(int)((UIntPtr)(iField & 255U))] + Helper.BitsSetTable256[(int)((UIntPtr)(iField >> 8 & 255U))] + Helper.BitsSetTable256[(int)((UIntPtr)(iField >> 16 & 255U))] + Helper.BitsSetTable256[(int)((UIntPtr)(iField >> 24))]);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00007B2C File Offset: 0x00005D2C
		public static int CountSetBits(ulong iField)
		{
			ulong num = iField;
			int num2 = 0;
			while (num > 0UL)
			{
				num &= num - 1UL;
				num2++;
			}
			return num2;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00007B50 File Offset: 0x00005D50
		public unsafe static int GetHashCodeCustom(this string iString)
		{
			IntPtr intPtr2;
			IntPtr intPtr = intPtr2 = iString;
			if (intPtr != 0)
			{
				intPtr2 = (IntPtr)((int)intPtr + RuntimeHelpers.OffsetToStringData);
			}
			char* ptr = intPtr2;
			char* ptr2 = ptr;
			int num = 352654597;
			int num2 = num;
			int* ptr3 = (int*)ptr2;
			for (int i = iString.Length; i > 0; i -= 4)
			{
				num = ((num << 5) + num + (num >> 27) ^ *ptr3);
				if (i <= 2)
				{
					break;
				}
				num2 = ((num2 << 5) + num2 + (num2 >> 27) ^ ptr3[1]);
				ptr3 += 2;
			}
			return num + num2 * 1566083941;
		}

		// Token: 0x040000A5 RID: 165
		private static readonly uint[] DLC_ID_LOOKUP = new uint[]
		{
			73030U,
			42918U
		};

		// Token: 0x040000A6 RID: 166
		private static byte[] BitsSetTable256 = new byte[]
		{
			0,
			1,
			1,
			2,
			1,
			2,
			2,
			3,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			1,
			2,
			2,
			3,
			2,
			3,
			3,
			4,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			2,
			3,
			3,
			4,
			3,
			4,
			4,
			5,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			3,
			4,
			4,
			5,
			4,
			5,
			5,
			6,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			4,
			5,
			5,
			6,
			5,
			6,
			6,
			7,
			5,
			6,
			6,
			7,
			6,
			7,
			7,
			8
		};

		// Token: 0x02000021 RID: 33
		internal enum DLC : byte
		{
			// Token: 0x040000A8 RID: 168
			WizardHat,
			// Token: 0x040000A9 RID: 169
			Vietnam
		}
	}
}
