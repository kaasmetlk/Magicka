// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.DropDownBox`1
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Localization;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal class DropDownBox<T> : DropDownBox
{
  private int?[] mNameIDs;
  private T[] mValues;

  public event Action<DropDownBox, T> ValueChanged;

  public DropDownBox(BitmapFont iFont, T[] iValues, int?[] iNames, int iWidth)
    : base(iFont, new string[iValues.Length], iWidth)
  {
    if (iNames != null && iNames.Length != iValues.Length)
      throw new ArgumentException("iNames must contain the same number of elements os iValues! If no localization is to be used, pass null instead.", nameof (iNames));
    this.mValues = iValues;
    this.mNameIDs = iNames;
    this.LanguageChanged();
  }

  public override void LanguageChanged()
  {
    if (this.mNameIDs != null)
    {
      LanguageManager instance = LanguageManager.Instance;
      for (int index = 0; index < this.mNames.Length; ++index)
      {
        if (this.mNameIDs[index].HasValue)
          this.mNames[index] = instance.GetString(this.mNameIDs[index].Value);
        else
          this.mNames[index] = this.mValues[index].ToString();
      }
    }
    else
    {
      for (int index = 0; index < this.mNames.Length; ++index)
        this.mNames[index] = this.mValues[index].ToString();
    }
    base.LanguageChanged();
  }

  protected override void OnSelectedIndexChanged()
  {
    base.OnSelectedIndexChanged();
    if (this.ValueChanged == null)
      return;
    this.ValueChanged((DropDownBox) this, this.mValues[this.mSelectedIndex]);
  }

  public T SelectedValue => this.mValues[this.mSelectedIndex];

  public T[] Values => this.mValues;

  public void SetNewValue(T iNewValue)
  {
    for (int index = 0; index < this.mValues.Length; ++index)
    {
      if (this.mValues[index].Equals((object) iNewValue))
      {
        this.SelectedIndex = index;
        break;
      }
    }
  }
}
