// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.UnlockMagick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class UnlockMagick : Action
{
  protected TextBox mTextBox;
  protected MagickType mMagickType;

  public UnlockMagick(Trigger iTrigger, GameScene iScene, XmlNode iNodE)
    : base(iTrigger, iScene)
  {
    this.mTextBox = new TextBox();
  }

  protected override void Execute()
  {
    this.mTextBox.Initialize(this.mScene.Scene, MagickaFont.Maiandra14, LanguageManager.Instance.GetString(BookOfMagick.MAGICK_PICKUP_LOC).Replace("#1;", $"[c=1,1,1]{LanguageManager.Instance.GetString(Magicka.GameLogic.Spells.Magick.NAME_LOCALIZATION[(int) this.mMagickType])}[/c]"), new Vector2(), new Vector2(), true, 0, 2f);
    DialogManager.Instance.AddTextBox(this.mTextBox);
    if (this.GameScene.PlayState.GameType == GameType.Versus)
    {
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing)
          SpellManager.Instance.UnlockMagick(players[index], this.mMagickType);
      }
    }
    else
      SpellManager.Instance.UnlockMagick(this.mMagickType, this.GameScene.PlayState.GameType);
  }

  public override void QuickExecute() => this.Execute();

  public MagickType MagickType
  {
    get => this.mMagickType;
    set => this.mMagickType = value;
  }
}
