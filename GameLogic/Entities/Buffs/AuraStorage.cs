// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.AuraStorage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

[StructLayout(LayoutKind.Explicit)]
public struct AuraStorage
{
  [FieldOffset(0)]
  public AuraTarget AuraTarget;
  [FieldOffset(1)]
  public AuraType AuraType;
  [FieldOffset(2)]
  public VisualCategory VisualCategory;
  [FieldOffset(4)]
  public Vector3 Color;
  [FieldOffset(16 /*0x10*/)]
  private float mPeriod;
  [FieldOffset(20)]
  public float TTL;
  [FieldOffset(24)]
  public int Effect;
  [FieldOffset(28)]
  public float Radius;
  [FieldOffset(32 /*0x20*/)]
  public int[] TargetType;
  [FieldOffset(36)]
  public Factions TargetFaction;
  [FieldOffset(40)]
  public AuraBuff AuraBuff;
  [FieldOffset(40)]
  public AuraDeflect AuraDeflect;
  [FieldOffset(40)]
  public AuraBoost AuraBoost;
  [FieldOffset(40)]
  public AuraLifeSteal AuraLifeSteal;
  [FieldOffset(40)]
  public AuraLove AuraLove;

  public AuraStorage(
    AuraBuff iAura,
    AuraTarget iAuraTarget,
    AuraType iAuraType,
    int iEffect,
    float iTTL,
    float iRadius,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    int[] iTargetType,
    Factions iTargetFaction)
    : this()
  {
    this.Color = iColor;
    this.AuraBuff = iAura;
    this.AuraTarget = iAuraTarget;
    this.AuraType = iAuraType;
    this.Effect = iEffect;
    this.TTL = iTTL;
    this.Radius = iRadius;
    this.VisualCategory = iVisualCategory;
    this.TargetType = iTargetType;
    this.TargetFaction = iTargetFaction;
  }

  public AuraStorage(
    AuraDeflect iAura,
    AuraTarget iAuraTarget,
    AuraType iAuraType,
    int iEffect,
    float iTTL,
    float iRadius,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    int[] iTargetType,
    Factions iTargetFaction)
    : this()
  {
    this.Color = iColor;
    this.AuraDeflect = iAura;
    this.AuraTarget = iAuraTarget;
    this.AuraType = iAuraType;
    this.Effect = iEffect;
    this.TTL = iTTL;
    this.Radius = iRadius;
    this.VisualCategory = iVisualCategory;
    this.TargetType = iTargetType;
    this.TargetFaction = iTargetFaction;
  }

  public AuraStorage(
    AuraBoost iAura,
    AuraTarget iAuraTarget,
    AuraType iAuraType,
    int iEffect,
    float iTTL,
    float iRadius,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    int[] iTargetType,
    Factions iTargetFaction)
    : this()
  {
    this.Color = iColor;
    this.AuraBoost = iAura;
    this.AuraTarget = iAuraTarget;
    this.AuraType = iAuraType;
    this.Effect = iEffect;
    this.TTL = iTTL;
    this.Radius = iRadius;
    this.VisualCategory = iVisualCategory;
    this.TargetType = iTargetType;
    this.TargetFaction = iTargetFaction;
  }

  public AuraStorage(
    AuraLifeSteal iAura,
    AuraTarget iAuraTarget,
    AuraType iAuraType,
    int iEffect,
    float iTTL,
    float iRadius,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    int[] iTargetType,
    Factions iTargetFaction)
    : this()
  {
    this.Color = iColor;
    this.AuraLifeSteal = iAura;
    this.AuraTarget = iAuraTarget;
    this.AuraType = iAuraType;
    this.Effect = iEffect;
    this.TTL = iTTL;
    this.Radius = iRadius;
    this.VisualCategory = iVisualCategory;
    this.TargetType = iTargetType;
    this.TargetFaction = iTargetFaction;
  }

  public AuraStorage(
    AuraLove iAura,
    AuraTarget iAuraTarget,
    AuraType iAuraType,
    int iEffect,
    float iTTL,
    float iRadius,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    int[] iTargetType,
    Factions iTargetFaction)
    : this()
  {
    this.Color = iColor;
    this.AuraLove = iAura;
    this.AuraTarget = iAuraTarget;
    this.AuraType = iAuraType;
    this.Effect = iEffect;
    this.TTL = iTTL;
    this.Radius = iRadius;
    this.VisualCategory = iVisualCategory;
    this.TargetType = iTargetType;
    this.TargetFaction = iTargetFaction;
  }

  public AuraStorage(ContentReader iInput)
    : this()
  {
    this.AuraTarget = (AuraTarget) iInput.ReadByte();
    this.AuraType = (AuraType) iInput.ReadByte();
    this.VisualCategory = (VisualCategory) iInput.ReadByte();
    this.Color = iInput.ReadVector3();
    string lowerInvariant = iInput.ReadString().ToLowerInvariant();
    this.Effect = !lowerInvariant.Equals("") ? lowerInvariant.GetHashCodeCustom() : 0;
    this.TTL = iInput.ReadSingle();
    this.Radius = iInput.ReadSingle();
    string[] strArray = iInput.ReadString().ToLowerInvariant().Split(',');
    int length = strArray.Length;
    this.TargetType = new int[length];
    for (int index = 0; index < length; ++index)
      this.TargetType[index] = strArray[index].GetHashCodeCustom();
    this.TargetFaction = (Factions) iInput.ReadInt32();
    switch (this.AuraType)
    {
      case AuraType.Buff:
        this.AuraBuff = new AuraBuff(iInput);
        break;
      case AuraType.Deflect:
        this.AuraDeflect = new AuraDeflect(iInput);
        break;
      case AuraType.Boost:
        this.AuraBoost = new AuraBoost(iInput);
        break;
      case AuraType.LifeSteal:
        this.AuraLifeSteal = new AuraLifeSteal(iInput);
        break;
      case AuraType.Love:
        this.AuraLove = new AuraLove(iInput);
        break;
    }
  }

  public void Execute(Character iOwner, float iDeltaTime)
  {
    this.TTL -= iDeltaTime;
    switch (this.AuraType)
    {
      case AuraType.Buff:
        this.mPeriod -= iDeltaTime;
        while ((double) this.mPeriod <= 0.0)
        {
          this.mPeriod += 0.25f;
          double num = (double) this.AuraBuff.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius, this.TargetType, this.TargetFaction);
        }
        break;
      case AuraType.Deflect:
        double num1 = (double) this.AuraDeflect.Execute((Entity) iOwner, iDeltaTime, this.AuraTarget, this.Effect, this.Radius);
        break;
      case AuraType.Boost:
        this.mPeriod -= iDeltaTime;
        while ((double) this.mPeriod <= 0.0)
        {
          this.mPeriod += 0.25f;
          double num2 = (double) this.AuraBoost.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
        }
        break;
      case AuraType.LifeSteal:
        this.mPeriod -= iDeltaTime;
        while ((double) this.mPeriod <= 0.0)
        {
          this.mPeriod += 0.25f;
          double num3 = (double) this.AuraLifeSteal.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
        }
        break;
      case AuraType.Love:
        this.mPeriod -= iDeltaTime;
        while ((double) this.mPeriod <= 0.0)
        {
          this.mPeriod += 0.25f;
          double num4 = (double) this.AuraLove.Execute(iOwner, this.AuraTarget, this.Effect, this.Radius);
        }
        break;
    }
  }
}
