// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.CharacterCasting
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public class CharacterCasting(GameScene iScene) : Condition(iScene)
{
  private string mName = string.Empty;
  private int mID = -1;
  private CastType mCastType;
  private Elements mElement;

  protected override bool InternalMet(Character iSender)
  {
    bool flag = Entity.GetByID(this.mID) is Character byId && byId.CurrentSpell != null;
    if (flag)
    {
      SpellEffect currentSpell = byId.CurrentSpell;
      flag = flag & currentSpell.CastType == this.mCastType & currentSpell.Spell.ContainsElement(this.mElement);
    }
    return flag;
  }

  public string ID
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mID = this.mName.GetHashCodeCustom();
    }
  }

  public CastType CastType
  {
    get => this.mCastType;
    set => this.mCastType = value;
  }

  public Elements Element
  {
    get => this.mElement;
    set => this.mElement = value;
  }
}
