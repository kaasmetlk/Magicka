// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.SpawnDecalEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Graphics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct SpawnDecalEvent
{
  private Decal mDecal;
  private float mScale;
  private float mTTL;

  public SpawnDecalEvent(Decal iDecal, float iScale)
  {
    this.mScale = iScale;
    this.mDecal = iDecal;
    this.mTTL = 60f;
  }

  public SpawnDecalEvent(Decal iDecal, int iScale, int iTTL)
  {
    this.mScale = (float) iScale;
    this.mDecal = iDecal;
    this.mTTL = (float) iTTL;
  }

  public SpawnDecalEvent(ContentReader iInput)
  {
    this.mDecal = (Decal) (iInput.ReadInt32() + iInput.ReadInt32() * 8);
    this.mScale = (float) iInput.ReadInt32();
    this.mTTL = 60f;
  }

  public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    Vector3 position = iItem.Position;
    if (iPosition.HasValue)
      position = iPosition.Value;
    Vector3 oPos;
    Vector3 oNrm;
    AnimatedLevelPart oAnimatedLevelPart;
    if (!iItem.PlayState.Level.CurrentScene.SegmentIntersect(out float _, out oPos, out oNrm, out oAnimatedLevelPart, new Segment()
    {
      Origin = position,
      Delta = new Vector3(0.0f, (float) (-(double) iItem.Radius * 2.0), 0.0f)
    }) || (double) oNrm.Y <= 0.699999988079071)
      return;
    Vector2 iScale = new Vector2();
    iScale.X = iScale.Y = this.mScale;
    Vector3 velocity = iItem.Body.Velocity with { Y = 0.0f };
    velocity.Normalize();
    DecalManager.Instance.AddAlphaBlendedDecal(this.mDecal, oAnimatedLevelPart, ref iScale, ref oPos, new Vector3?(velocity), ref oNrm, this.mTTL, 1f);
  }
}
