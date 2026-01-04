// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Magick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class Magick(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  protected string mID;
  protected int mIDHash;
  protected MagickType mMagickType;
  protected Vector3 mPosition;

  protected override void Execute()
  {
    if (this.mIDHash == 0)
      new Magicka.GameLogic.Spells.Magick() { MagickType = this.mMagickType }.Effect.Execute(this.mPosition, this.GameScene.PlayState);
    else
      new Magicka.GameLogic.Spells.Magick() { MagickType = this.mMagickType }.Effect.Execute((ISpellCaster) (Entity.GetByID(this.mIDHash) as Character), this.GameScene.PlayState);
  }

  public override void QuickExecute() => this.Execute();

  public MagickType MagickType
  {
    get => this.mMagickType;
    set => this.mMagickType = value;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      if (!string.IsNullOrEmpty(this.mID))
        this.mIDHash = this.mID.GetHashCodeCustom();
      else
        this.mIDHash = 0;
    }
  }

  public Vector3 Position
  {
    get => this.mPosition;
    set => this.mPosition = value;
  }
}
