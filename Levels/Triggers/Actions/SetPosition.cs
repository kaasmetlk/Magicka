// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SetPosition
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SetPosition(Trigger iTrigger, GameScene iScene, XmlNode iNode) : Action(iTrigger, iScene)
{
  public static readonly string PLAYER1 = "player1";
  public static readonly string PLAYER2 = "player2";
  public static readonly string PLAYER3 = "player3";
  public static readonly string PLAYER4 = "player4";
  protected string mID;
  protected int mIDHash;
  protected bool mFacingDirection;
  protected string mArea;
  protected int mAreaHash;

  public override void Initialize() => base.Initialize();

  protected override void Execute()
  {
    Entity entity = (Entity) null;
    int index = -1;
    if (this.mID.Equals(SetPosition.PLAYER1, StringComparison.OrdinalIgnoreCase))
      index = 0;
    else if (this.mID.Equals(SetPosition.PLAYER2, StringComparison.OrdinalIgnoreCase))
      index = 1;
    else if (this.mID.Equals(SetPosition.PLAYER3, StringComparison.OrdinalIgnoreCase))
      index = 2;
    else if (this.mID.Equals(SetPosition.PLAYER4, StringComparison.OrdinalIgnoreCase))
      index = 3;
    if (index != -1)
    {
      if (Magicka.Game.Instance.Players[index].Playing && Magicka.Game.Instance.Players[index].Avatar != null)
        entity = (Entity) Magicka.Game.Instance.Players[index].Avatar;
    }
    else
      entity = Entity.GetByID(this.mIDHash);
    if (entity == null)
      return;
    Matrix oLocator;
    this.mScene.GetLocator(this.mAreaHash, out oLocator);
    Vector3 pos1 = oLocator.Translation;
    Vector3 forward = oLocator.Forward;
    oLocator.Translation = new Vector3();
    Segment seg = new Segment();
    seg.Origin = pos1;
    ++seg.Origin.Y;
    seg.Delta.Y = -5f;
    Vector3 pos2;
    if (this.GameScene.LevelModel.CollisionSkin.SegmentIntersect(out float _, out pos2, out Vector3 _, seg))
    {
      pos1 = pos2;
      if (entity is Character character)
      {
        pos1.Y -= character.HeightOffset;
        if (this.mFacingDirection)
          character.CharacterBody.DesiredDirection = forward;
        else
          oLocator = entity.Body.Orientation with
          {
            Translation = new Vector3()
          };
      }
    }
    entity.Body.Velocity = new Vector3();
    entity.Body.MoveTo(pos1, oLocator);
  }

  public override void QuickExecute() => this.Execute();

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      this.mIDHash = this.mID.GetHashCodeCustom();
    }
  }

  public bool FacingDirection
  {
    get => this.mFacingDirection;
    set => this.mFacingDirection = value;
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaHash = this.mArea.GetHashCodeCustom();
    }
  }
}
