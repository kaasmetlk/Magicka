// Decompiled with JetBrains decompiler
// Type: Magicka.Helper
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using XNAnimation.Effects;

#nullable disable
namespace Magicka;

internal static class Helper
{
  private static readonly uint[] DLC_ID_LOOKUP = new uint[2]
  {
    73030U,
    42918U
  };
  private static byte[] BitsSetTable256 = new byte[256 /*0x0100*/]
  {
    (byte) 0,
    (byte) 1,
    (byte) 1,
    (byte) 2,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 1,
    (byte) 2,
    (byte) 2,
    (byte) 3,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 2,
    (byte) 3,
    (byte) 3,
    (byte) 4,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 3,
    (byte) 4,
    (byte) 4,
    (byte) 5,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 4,
    (byte) 5,
    (byte) 5,
    (byte) 6,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 5,
    (byte) 6,
    (byte) 6,
    (byte) 7,
    (byte) 6,
    (byte) 7,
    (byte) 7,
    (byte) 8
  };

  internal static bool GetDLC(uint iID, out Helper.DLC oDLC)
  {
    for (int index = 0; index < Helper.DLC_ID_LOOKUP.Length; ++index)
    {
      if ((int) iID == (int) Helper.DLC_ID_LOOKUP[index])
      {
        oDLC = (Helper.DLC) index;
        return true;
      }
    }
    oDLC = Helper.DLC.WizardHat;
    return false;
  }

  internal static uint GetID(Helper.DLC iDLC) => Helper.DLC_ID_LOOKUP[(int) iDLC];

  internal static bool CheckMagickDLC(MagickType iType)
  {
    switch (iType)
    {
      case MagickType.MeteorS:
        return Helper.CheckDLC(Helper.DLC.WizardHat);
      case MagickType.Napalm:
        return Helper.CheckDLC(Helper.DLC.Vietnam);
      default:
        return true;
    }
  }

  internal static bool CheckDLCID(uint iAppID)
  {
    if ((int) iAppID == (int) SteamUtils.GetAppID())
      return true;
    return iAppID == 901679U ? Helper.CheckDLCID(73054U) & Helper.CheckDLCID(73055U) & Helper.CheckDLCID(73056U) : SteamApps.BIsSubscribedApp(iAppID);
  }

  internal static bool CheckDLC(Helper.DLC iDLC)
  {
    return Helper.CheckDLCID(Helper.DLC_ID_LOOKUP[(int) iDLC]);
  }

  public static void Swap<T>(ref T iA, ref T iB)
  {
    T obj = iA;
    iA = iB;
    iB = obj;
  }

  public static unsafe DamageResult CircleDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    ref Vector3 iOrigin,
    float iRange,
    ref Magicka.GameLogic.Damage iDamage)
  {
    Vector3 iDirection = new Vector3();
    iDirection.Z = -1f;
    fixed (Magicka.GameLogic.Damage* iDamage1 = &iDamage)
      return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, 3.14159274f, iDamage1, 1);
  }

  public static unsafe DamageResult CircleDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    ref Vector3 iOrigin,
    float iRange,
    ref DamageCollection5 iDamage)
  {
    Vector3 iDirection = new Vector3();
    iDirection.Z = -1f;
    fixed (Magicka.GameLogic.Damage* iDamage1 = &iDamage.A)
      return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, 3.14159274f, iDamage1, 5);
  }

  public static unsafe DamageResult ArcDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iRange,
    float iSpread,
    ref Magicka.GameLogic.Damage iDamage)
  {
    fixed (Magicka.GameLogic.Damage* iDamage1 = &iDamage)
      return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, iSpread, iDamage1, 1);
  }

  public static unsafe DamageResult ArcDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iRange,
    float iSpread,
    ref DamageCollection5 iDamage)
  {
    fixed (Magicka.GameLogic.Damage* iDamage1 = &iDamage.A)
      return Helper.ArcDamage(iPlayState, iOwner, iTimeStamp, iIgnoreEntity, ref iOrigin, ref iDirection, iRange, iSpread, iDamage1, 5);
  }

  private static unsafe DamageResult ArcDamage(
    PlayState iPlayState,
    Entity iOwner,
    double iTimeStamp,
    Entity iIgnoreEntity,
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iRange,
    float iSpread,
    Magicka.GameLogic.Damage* iDamage,
    int iNrOfDamages)
  {
    iSpread = Math.Min(iSpread, 3.14159274f);
    DamageResult damageResult = DamageResult.None;
    bool flag = false;
    for (int index = 0; index < iNrOfDamages; ++index)
      flag |= (iDamage[index].Element & (Elements.Water | Elements.Steam)) == (Elements.Water | Elements.Steam);
    float num1 = (float) Math.Cos((double) iSpread);
    List<Entity> entities = iPlayState.EntityManager.GetEntities(iOrigin, iRange, true);
    entities.Remove(iIgnoreEntity);
    for (int index1 = 0; index1 < entities.Count; ++index1)
    {
      if (entities[index1] is IDamageable t && (!(t is Character) || !(t as Character).IsEthereal))
      {
        Vector3 position = t.Position;
        float result1;
        Vector3.Distance(ref iOrigin, ref position, out result1);
        Vector3 result2;
        Vector3.Subtract(ref position, ref iOrigin, out result2);
        result2.Y = 0.0f;
        float num2 = result2.Length();
        Vector3.Divide(ref result2, num2, out result2);
        if ((double) num2 < 9.9999999747524271E-07 || (double) Vector3.Dot(result2, iDirection) >= (double) num1)
        {
          if (flag)
            t.Electrocute(iOwner as IDamageable, 1f);
          float num3 = (float) (1.0 - ((double) result1 - (double) t.Radius) / (double) iRange);
          if ((double) num3 > 0.0)
          {
            for (int index2 = 0; index2 < iNrOfDamages; ++index2)
            {
              Magicka.GameLogic.Damage iDamage1 = iDamage[index2];
              iDamage1.Magnitude *= num3;
              damageResult |= t.Damage(iDamage1, iOwner, iTimeStamp, iOrigin);
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

  public static DamageResult Damage(
    this IDamageable t,
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition)
  {
    if (iAttacker is Character)
    {
      float oMultiplyer;
      float oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.Element, out oMultiplyer, out oBias);
      iDamage.Amount *= oMultiplyer;
      iDamage.Amount += oBias;
    }
    Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNKE;
    Avatar avatar = iAttacker as Avatar;
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      if (avatar == null)
        iFeatures = Defines.DamageFeatures.NKE;
      else if (!(avatar.Player.Gamer is NetworkGamer))
      {
        DamageRequestMessage iMessage = new DamageRequestMessage();
        iMessage.TargetHandle = t.Handle;
        iMessage.AttackerHandle = avatar.Handle;
        Vector3 position = t.Position;
        Vector3.Subtract(ref iAttackPosition, ref position, out iMessage.RelativeAttackPosition);
        iMessage.Damage.A = iDamage;
        iMessage.TimeStamp = iTimeStamp;
        NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref iMessage);
        iFeatures = Defines.DamageFeatures.NKE;
      }
      else
        iFeatures = Defines.DamageFeatures.Effects;
    }
    else if (NetworkManager.Instance.State == NetworkState.Server && avatar != null)
    {
      if (avatar.Player.Gamer is NetworkGamer)
      {
        iFeatures = Defines.DamageFeatures.Effects;
      }
      else
      {
        DamageRequestMessage iMessage = new DamageRequestMessage();
        iMessage.TargetHandle = t.Handle;
        iMessage.AttackerHandle = iAttacker.Handle;
        Vector3 position = t.Position;
        Vector3.Subtract(ref iAttackPosition, ref position, out iMessage.RelativeAttackPosition);
        iMessage.Damage.A = iDamage;
        iMessage.TimeStamp = iTimeStamp;
        NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref iMessage);
      }
    }
    return t.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public static DamageResult Damage(
    this IDamageable t,
    DamageCollection5 iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition)
  {
    if (iAttacker is Character)
    {
      float oMultiplyer;
      float oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.A.Element, out oMultiplyer, out oBias);
      iDamage.A.Amount *= oMultiplyer;
      iDamage.A.Amount += oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.B.Element, out oMultiplyer, out oBias);
      iDamage.B.Amount *= oMultiplyer;
      iDamage.B.Amount += oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.C.Element, out oMultiplyer, out oBias);
      iDamage.C.Amount *= oMultiplyer;
      iDamage.C.Amount += oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.D.Element, out oMultiplyer, out oBias);
      iDamage.D.Amount *= oMultiplyer;
      iDamage.D.Amount += oBias;
      (iAttacker as Character).GetDamageModifier(iDamage.E.Element, out oMultiplyer, out oBias);
      iDamage.E.Amount *= oMultiplyer;
      iDamage.E.Amount += oBias;
    }
    Defines.DamageFeatures iFeatures = Defines.DamageFeatures.DNKE;
    Avatar avatar = iAttacker as Avatar;
    if (NetworkManager.Instance.State == NetworkState.Client)
    {
      if (avatar == null)
        iFeatures = Defines.DamageFeatures.NKE;
      else if (!(avatar.Player.Gamer is NetworkGamer))
      {
        DamageRequestMessage iMessage = new DamageRequestMessage();
        iMessage.TargetHandle = t.Handle;
        iMessage.AttackerHandle = avatar.Handle;
        Vector3 position = t.Position;
        Vector3.Subtract(ref iAttackPosition, ref position, out iMessage.RelativeAttackPosition);
        iMessage.TimeStamp = iTimeStamp;
        iMessage.Damage = iDamage;
        NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref iMessage);
        iFeatures = Defines.DamageFeatures.NKE;
      }
      else
        iFeatures = Defines.DamageFeatures.Effects;
    }
    else if (NetworkManager.Instance.State == NetworkState.Server && avatar != null)
    {
      if (avatar.Player.Gamer is NetworkGamer)
      {
        iFeatures = Defines.DamageFeatures.Effects;
      }
      else
      {
        DamageRequestMessage iMessage = new DamageRequestMessage();
        iMessage.TargetHandle = t.Handle;
        iMessage.AttackerHandle = avatar.Handle;
        Vector3 position = t.Position;
        Vector3.Subtract(ref iAttackPosition, ref position, out iMessage.RelativeAttackPosition);
        iMessage.Damage = iDamage;
        iMessage.TimeStamp = iTimeStamp;
        NetworkManager.Instance.Interface.SendMessage<DamageRequestMessage>(ref iMessage);
      }
    }
    return t.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
  }

  public static void SkinnedModelDeferredMaterialFromBasicEffect(
    SkinnedModelBasicEffect iEffect,
    out SkinnedModelDeferredAdvancedMaterial oMaterial)
  {
    oMaterial = new SkinnedModelDeferredAdvancedMaterial();
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

  public static void SkinnedModelDeferredMaterialFromBasicEffect(
    SkinnedModelBasicEffect iEffect,
    out SkinnedModelDeferredBasicMaterial oMaterial)
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

  public static Texture2D TextureFromURL(string iURL, SurfaceFormat iFormat)
  {
    Stream responseStream = WebRequest.Create(iURL).GetResponse().GetResponseStream();
    MemoryStream textureStream = new MemoryStream();
    byte[] buffer = new byte[16384 /*0x4000*/];
    while (true)
    {
      int count = responseStream.Read(buffer, 0, buffer.Length);
      if (count > 0)
        textureStream.Write(buffer, 0, count);
      else
        break;
    }
    responseStream.Close();
    responseStream.Dispose();
    textureStream.Position = 0L;
    GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
    TextureCreationParameters creationParameters = TextureCreationParameters.Default with
    {
      Format = iFormat
    };
    Texture2D texture2D;
    lock (graphicsDevice)
      texture2D = Texture2D.FromFile(graphicsDevice, (Stream) textureStream, creationParameters);
    textureStream.Close();
    textureStream.Dispose();
    return texture2D;
  }

  public static bool ArrayEquals(byte[] iA, byte[] iB)
  {
    if (iA.Length != iB.Length)
      return false;
    for (int index = 0; index < iA.Length; ++index)
    {
      if ((int) iA[index] != (int) iB[index])
        return false;
    }
    return true;
  }

  public static int CountSetBits(uint iField)
  {
    return (int) Helper.BitsSetTable256[(IntPtr) (iField & (uint) byte.MaxValue)] + (int) Helper.BitsSetTable256[(IntPtr) (iField >> 8 & (uint) byte.MaxValue)] + (int) Helper.BitsSetTable256[(IntPtr) (iField >> 16 /*0x10*/ & (uint) byte.MaxValue)] + (int) Helper.BitsSetTable256[(IntPtr) (iField >> 24)];
  }

  public static int CountSetBits(ulong iField)
  {
    ulong num1 = iField;
    int num2 = 0;
    while (num1 > 0UL)
    {
      num1 &= num1 - 1UL;
      ++num2;
    }
    return num2;
  }

  public static unsafe int GetHashCodeCustom(this string iString)
  {
    fixed (char* chPtr = iString)
    {
      int num1 = 352654597 /*0x15051505*/;
      int num2 = num1;
      int* numPtr = (int*) chPtr;
      for (int length = iString.Length; length > 0; length -= 4)
      {
        num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
        if (length > 2)
        {
          num2 = (num2 << 5) + num2 + (num2 >> 27) ^ numPtr[1];
          numPtr += 2;
        }
        else
          break;
      }
      return num1 + num2 * 1566083941;
    }
  }

  internal enum DLC : byte
  {
    WizardHat,
    Vietnam,
  }
}
