// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ScrollScore
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class ScrollScore(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mValueName;
  private string mAreaName;
  private int mAreaID;
  private string mOffset;
  private Vector3 mOffsetV;
  private float mTTL;
  private float mValuef;

  protected override void Execute()
  {
    Vector3 result = new Vector3();
    TriggerArea oArea;
    if (this.mScene.Level.CurrentScene.TryGetTriggerArea(this.mAreaID, out oArea))
    {
      result = oArea.GetRandomLocation();
    }
    else
    {
      Matrix oLocator;
      if (this.mScene.Level.CurrentScene.TryGetLocator(this.mAreaID, out oLocator))
        result = oLocator.Translation;
    }
    Vector3.Add(ref result, ref this.mOffsetV, out result);
    Vector3 one = Vector3.One;
    DamageNotifyer.Instance.AddNumber(this.mValuef, ref result, this.mTTL, false, ref one);
  }

  public override void QuickExecute() => this.Execute();

  public string Value
  {
    get => this.mValueName;
    set
    {
      this.mValueName = value;
      this.mValuef = float.Parse(this.mValueName, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
  }

  public float TTL
  {
    get => this.mTTL;
    set => this.mTTL = value;
  }

  public string Area
  {
    get => this.mAreaName;
    set
    {
      this.mAreaName = value;
      this.mAreaID = this.mAreaName.ToLowerInvariant().GetHashCodeCustom();
    }
  }

  public string Offset
  {
    get => this.mOffset;
    set
    {
      this.mOffset = value;
      string[] strArray = this.mOffset.Split(',');
      if (strArray.Length != 3)
        return;
      this.mOffsetV.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      this.mOffsetV.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
      this.mOffsetV.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
    }
  }
}
