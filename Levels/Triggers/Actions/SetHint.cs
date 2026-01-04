// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetHint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Localization;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SetHint(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  private const string KEYBOARD = "_key";
  private const string PAD = "_pad";
  private string mHint;
  private string mHintText;
  private int mHintHash;
  private bool mAppend;
  private TutorialManager.HintAnimations mAnimation;
  private TutorialManager.Position mHintPosition = TutorialManager.Position.BottomRight;

  public override void Initialize()
  {
    base.Initialize();
    string str = this.mHint;
    if (this.mAppend)
    {
      bool flag = false;
      Player[] players = Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
        {
          flag = !(players[index].Controller is KeyboardMouseController);
          break;
        }
      }
      str = !flag ? str + "_key" : str + "_pad";
    }
    this.mHintHash = str.ToLowerInvariant().GetHashCodeCustom();
    LanguageManager instance = LanguageManager.Instance;
    string iText = instance.GetString(this.mHintHash);
    string references = instance.ParseReferences(iText);
    this.mHintText = FontManager.Instance.GetFont(MagickaFont.Maiandra14).Wrap(references, 300, true);
  }

  protected override void Execute()
  {
    TutorialManager.Instance.SetHint(this.mHintHash, this.mHintText, this.mAnimation, this.mHintPosition);
  }

  public override void QuickExecute()
  {
  }

  public bool AppendController
  {
    get => this.mAppend;
    set => this.mAppend = value;
  }

  public string ID
  {
    get => this.mHint;
    set => this.mHint = value;
  }

  public TutorialManager.HintAnimations Animation
  {
    get => this.mAnimation;
    set => this.mAnimation = value;
  }

  public TutorialManager.Position Position
  {
    get => this.mHintPosition;
    set => this.mHintPosition = value;
  }
}
