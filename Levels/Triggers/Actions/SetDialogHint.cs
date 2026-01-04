// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetDialogHint
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SetDialogHint(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mHint;
  private int mHintHash;
  private MagickType mMagick;
  private Elements mElement;
  private string mTrigger;
  private int mTriggerID;
  private float mScale;
  private Vector2 mSize;

  public override void Initialize()
  {
    base.Initialize();
    LanguageManager instance = LanguageManager.Instance;
    string iText = instance.GetString(this.mHintHash);
    string references = instance.ParseReferences(iText);
    this.mHint = FontManager.Instance.GetFont(MagickaFont.Maiandra14).Wrap(references, 300, true);
  }

  protected override void Execute()
  {
    float? iScale = new float?();
    if ((double) this.mScale > 0.0)
      iScale = new float?(this.mScale);
    Vector2? iSize = new Vector2?();
    if ((double) this.mSize.X > 0.0 & (double) this.mSize.Y > 0.0)
      iSize = new Vector2?(this.mSize);
    if (this.mMagick != MagickType.None)
      TutorialManager.Instance.SetMagickHint(this.mMagick, this.mHint, this.mTriggerID, iScale, iSize);
    else
      TutorialManager.Instance.SetElementHint(this.mElement, this.mHint, this.mTriggerID, iScale, iSize);
  }

  public override void QuickExecute()
  {
  }

  public string Trigger
  {
    get => this.mTrigger;
    set
    {
      this.mTrigger = value;
      this.mTriggerID = this.mTrigger.GetHashCodeCustom();
    }
  }

  public float Scale
  {
    get => this.mScale;
    set => this.mScale = value;
  }

  public Vector2 Size
  {
    get => this.mSize;
    set => this.mSize = value;
  }

  public Elements Element
  {
    get => this.mElement;
    set => this.mElement = value;
  }

  public MagickType Magick
  {
    get => this.mMagick;
    set => this.mMagick = value;
  }

  public int Custom
  {
    get => (int) this.mMagick;
    set => this.mMagick = (MagickType) value;
  }

  public string ID
  {
    get => this.mHint;
    set
    {
      this.mHint = value;
      this.mHintHash = value.ToLowerInvariant().GetHashCodeCustom();
    }
  }
}
