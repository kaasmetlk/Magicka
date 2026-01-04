// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.DisableCastType
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.Graphics;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class DisableCastType(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private CastType mCastType;

  protected override void Execute() => TutorialManager.Instance.DisableCastType(this.mCastType);

  public override void QuickExecute() => this.Execute();

  public CastType Type
  {
    get => this.mCastType;
    set => this.mCastType = value;
  }
}
