// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Buffs.BuffStorage
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Runtime.InteropServices;

#nullable disable
namespace Magicka.GameLogic.Entities.Buffs;

[StructLayout(LayoutKind.Explicit)]
public struct BuffStorage
{
  private static uint IDCounter;
  [FieldOffset(0)]
  public readonly uint UniqueID;
  [FieldOffset(4)]
  public BuffType BuffType;
  [FieldOffset(5)]
  public VisualCategory VisualCategory;
  [FieldOffset(8)]
  public Vector3 Color;
  [FieldOffset(20)]
  public float TTL;
  [FieldOffset(24)]
  public int Effect;
  [FieldOffset(28)]
  public bool SelfCasted;
  [FieldOffset(32 /*0x20*/)]
  public BuffBoostDamage BuffBoostDamage;
  [FieldOffset(32 /*0x20*/)]
  public BuffDealDamage BuffDealDamage;
  [FieldOffset(32 /*0x20*/)]
  public BuffResistance BuffResistance;
  [FieldOffset(32 /*0x20*/)]
  public BuffUndying BuffUndying;
  [FieldOffset(32 /*0x20*/)]
  public BuffBoost BuffBoost;
  [FieldOffset(32 /*0x20*/)]
  public BuffReduceAgro BuffReduceAgro;
  [FieldOffset(32 /*0x20*/)]
  public BuffModifyHitPoints BuffModifyHitPoints;
  [FieldOffset(32 /*0x20*/)]
  public BuffModifySpellTTL BuffModifySpellTTL;
  [FieldOffset(32 /*0x20*/)]
  public BuffModifySpellRange BuffModifySpellRange;

  public BuffStorage(ContentReader iInput)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.SelfCasted = false;
    this.BuffType = (BuffType) iInput.ReadByte();
    this.VisualCategory = (VisualCategory) iInput.ReadByte();
    this.Color = iInput.ReadVector3();
    this.TTL = iInput.ReadSingle();
    string lowerInvariant = iInput.ReadString().ToLowerInvariant();
    this.Effect = !lowerInvariant.Equals("") ? lowerInvariant.GetHashCodeCustom() : 0;
    switch (this.BuffType)
    {
      case BuffType.BoostDamage:
        this.BuffBoostDamage = new BuffBoostDamage(iInput);
        break;
      case BuffType.DealDamage:
        this.BuffDealDamage = new BuffDealDamage(iInput);
        break;
      case BuffType.Resistance:
        this.BuffResistance = new BuffResistance(iInput);
        break;
      case BuffType.Undying:
        this.BuffUndying = new BuffUndying(iInput);
        break;
      case BuffType.Boost:
        this.BuffBoost = new BuffBoost(iInput);
        break;
      case BuffType.ReduceAgro:
        this.BuffReduceAgro = new BuffReduceAgro(iInput);
        break;
      case BuffType.ModifyHitPoints:
        this.BuffModifyHitPoints = new BuffModifyHitPoints(iInput);
        break;
      case BuffType.ModifySpellTTL:
        this.BuffModifySpellTTL = new BuffModifySpellTTL(iInput);
        break;
      case BuffType.ModifySpellRange:
        this.BuffModifySpellRange = new BuffModifySpellRange(iInput);
        break;
    }
  }

  public BuffStorage(BuffBoostDamage iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(
    BuffBoostDamage iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.BoostDamage;
    this.BuffBoostDamage = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(BuffDealDamage iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(
    BuffDealDamage iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.DealDamage;
    this.BuffDealDamage = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(BuffResistance iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(
    BuffResistance iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.Resistance;
    this.BuffResistance = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(BuffUndying iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(
    BuffUndying iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.Undying;
    this.BuffUndying = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(BuffBoost iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(BuffBoost iBuff, float iTTL, VisualCategory iVisualCategory, Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.Boost;
    this.BuffBoost = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(BuffReduceAgro iBuff, VisualCategory iVisualCategory, Vector3 iColor)
    : this(iBuff, 0.5f, iVisualCategory, iColor)
  {
  }

  public BuffStorage(
    BuffReduceAgro iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.ReduceAgro;
    this.BuffReduceAgro = iBuff;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(
    BuffModifyHitPoints iBuff,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iHitPointsMultiplyer,
    float iHitPointsModifier)
    : this(iBuff, 0.5f, iVisualCategory, iColor, iHitPointsMultiplyer, iHitPointsModifier)
  {
  }

  public BuffStorage(
    BuffModifyHitPoints iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iHitPointsMultiplyer,
    float iHitPointsModifier)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.ModifyHitPoints;
    this.BuffModifyHitPoints = iBuff;
    this.BuffModifyHitPoints.HitPointsMultiplier = iHitPointsMultiplyer;
    this.BuffModifyHitPoints.HitPointsModifier = iHitPointsModifier;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(
    BuffModifySpellTTL iBuff,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iTTLMultiplyer,
    float iTTLModifier)
    : this(iBuff, 0.5f, iVisualCategory, iColor, iTTLMultiplyer, iTTLModifier)
  {
  }

  public BuffStorage(
    BuffModifySpellTTL iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iTTLMultiplyer,
    float iTTLModifier)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.ModifySpellTTL;
    this.BuffModifySpellTTL = iBuff;
    this.BuffModifySpellTTL.TTLMultiplier = iTTLMultiplyer;
    this.BuffModifySpellTTL.TTLModifier = iTTLModifier;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public BuffStorage(
    BuffModifySpellRange iBuff,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iSpellRangeMultiplyer,
    float iSpellRangeModifier)
    : this(iBuff, 0.5f, iVisualCategory, iColor, iSpellRangeMultiplyer, iSpellRangeModifier)
  {
  }

  public BuffStorage(
    BuffModifySpellRange iBuff,
    float iTTL,
    VisualCategory iVisualCategory,
    Vector3 iColor,
    float iSpellRangeMultiplyer,
    float iSpellRangeModifier)
    : this()
  {
    this.UniqueID = BuffStorage.IDCounter++;
    this.Color = iColor;
    this.BuffType = BuffType.ModifyHitPoints;
    this.BuffModifySpellRange = iBuff;
    this.BuffModifySpellRange.RangeMultiplier = iSpellRangeMultiplyer;
    this.BuffModifySpellRange.RangeModifier = iSpellRangeModifier;
    this.TTL = iTTL;
    this.VisualCategory = iVisualCategory;
  }

  public void Execute(Character iOwner, float iDeltatime)
  {
    this.TTL -= iDeltatime;
    switch (this.BuffType)
    {
      case BuffType.BoostDamage:
        double num1 = (double) this.BuffBoostDamage.Execute(iOwner);
        break;
      case BuffType.DealDamage:
        double num2 = (double) this.BuffDealDamage.Execute(iOwner, iDeltatime);
        break;
      case BuffType.Resistance:
        double num3 = (double) this.BuffResistance.Execute(iOwner);
        break;
      case BuffType.Undying:
        double num4 = (double) this.BuffUndying.Execute(iOwner);
        break;
      case BuffType.Boost:
        double num5 = (double) this.BuffBoost.Execute(iOwner);
        break;
      case BuffType.ReduceAgro:
        double num6 = (double) this.BuffReduceAgro.Execute(iOwner);
        break;
      case BuffType.ModifyHitPoints:
        double num7 = (double) this.BuffModifyHitPoints.Execute(iOwner);
        break;
      case BuffType.ModifySpellTTL:
        double num8 = (double) this.BuffModifySpellTTL.Execute(iOwner);
        break;
      case BuffType.ModifySpellRange:
        double num9 = (double) this.BuffModifySpellRange.Execute(iOwner);
        break;
    }
  }
}
