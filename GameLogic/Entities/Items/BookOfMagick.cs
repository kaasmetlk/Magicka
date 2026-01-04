// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.BookOfMagick
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public class BookOfMagick : Pickable
{
  private static Queue<BookOfMagick> sCache;
  private static Model sBookModel;
  private MagickType mMagick;
  public static readonly int TIMEOUT_EFFECT = "tome_timing_out".GetHashCodeCustom();
  public static readonly int DISAPPEAR_EFFECT = "tome_disappear".GetHashCodeCustom();
  public static readonly int MAGICK_PICKUP_LOC = "#magick_pick_up".GetHashCodeCustom();
  private static readonly int GLIMMER_EFFECT = "bookofmagick_glimmer".GetHashCodeCustom();
  private VisualEffectReference mEffect;
  protected float mRestingTimer = 1f;
  private string mPickUpString;
  private float mTimeOutTimer;
  private VisualEffectReference mTimeOutEffect;
  private static string[] sPickUpStrings;

  public static BookOfMagick GetInstance(PlayState iPlayState)
  {
    BookOfMagick instance = BookOfMagick.sCache.Dequeue();
    BookOfMagick.sCache.Enqueue(instance);
    return instance;
  }

  public static void InitializeCache(int iNr, PlayState iPlayState)
  {
    BookOfMagick.sCache = new Queue<BookOfMagick>(iNr);
    for (int index = 0; index < iNr; ++index)
      BookOfMagick.sCache.Enqueue(new BookOfMagick(iPlayState));
  }

  public BookOfMagick(PlayState iPlayState)
    : base(iPlayState)
  {
    if (BookOfMagick.sPickUpStrings == null)
    {
      BookOfMagick.sPickUpStrings = new string[35];
      for (int index = 1; index < BookOfMagick.sPickUpStrings.Length; ++index)
      {
        if (index >= Magicka.GameLogic.Spells.Magick.NAME_LOCALIZATION.Length)
        {
          Console.WriteLine("Magick name out of range! Fix MMagick.NAME_LOCALIZATION.");
        }
        else
        {
          BookOfMagick.sPickUpStrings[index] = LanguageManager.Instance.GetString(BookOfMagick.MAGICK_PICKUP_LOC);
          BookOfMagick.sPickUpStrings[index] = BookOfMagick.sPickUpStrings[index].Replace("#1;", $"[c=1,1,1]{LanguageManager.Instance.GetString(Magicka.GameLogic.Spells.Magick.NAME_LOCALIZATION[index])}[/c]");
        }
      }
    }
    if (BookOfMagick.sBookModel == null)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        BookOfMagick.sBookModel = Magicka.Game.Instance.Content.Load<Model>("Models/Items_Wizard/magickbook_major");
    }
    this.Model = BookOfMagick.sBookModel;
    VertexElement[] vertexElements;
    lock (Magicka.Game.Instance.GraphicsDevice)
      vertexElements = BookOfMagick.sBookModel.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
    int offsetInBytes = -1;
    for (int index = 0; index < vertexElements.Length; ++index)
    {
      if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position)
      {
        offsetInBytes = (int) vertexElements[index].Offset;
        break;
      }
    }
    if (offsetInBytes < 0)
      throw new Exception("No positions found");
    Vector3[] vector3Array = new Vector3[BookOfMagick.sBookModel.Meshes[0].MeshParts[0].NumVertices];
    BookOfMagick.sBookModel.Meshes[0].VertexBuffer.GetData<Vector3>(offsetInBytes, vector3Array, BookOfMagick.sBookModel.Meshes[0].MeshParts[0].StartIndex, vector3Array.Length, BookOfMagick.sBookModel.Meshes[0].MeshParts[0].VertexStride);
    this.mBoundingBox = BoundingBox.CreateFromPoints((IEnumerable<Vector3>) vector3Array);
    Vector3 result;
    Vector3.Subtract(ref this.mBoundingBox.Max, ref this.mBoundingBox.Min, out result);
    result.Y *= 2f;
    (this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = result;
    (this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = result;
    (this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = result;
    Vector3 vector3_1 = (this.mBoundingBox.Min + this.mBoundingBox.Max) * 0.5f;
    Vector3 vector3_2 = this.SetMass(50f);
    JigLibX.Math.Transform transform = new JigLibX.Math.Transform();
    Vector3.Negate(ref vector3_2, out transform.Position);
    Vector3.Add(ref transform.Position, ref vector3_1, out transform.Position);
    transform.Orientation = Matrix.Identity;
    this.mCollision.ApplyLocalTransform(transform);
    this.mBody.Immovable = true;
    this.mPickable = true;
    this.mPickUpString = BookOfMagick.sPickUpStrings[0];
    this.mDead = false;
  }

  public void Initialize(
    Vector3 iPosition,
    Matrix iOrientation,
    MagickType iMagick,
    bool iImmovable,
    Vector3 iVelocity,
    float iTimeout,
    int iUniqueID)
  {
    this.Initialize(iUniqueID);
    this.mRestingTimer = 1f;
    this.mDead = false;
    iOrientation.Translation = new Vector3();
    this.mBody.MoveTo(iPosition, iOrientation);
    this.mTimeOutTimer = iTimeout;
    this.mMagick = iMagick;
    this.mPickUpString = BookOfMagick.sPickUpStrings[(int) this.mMagick];
    iOrientation.Translation = iPosition;
    EffectManager.Instance.StartEffect(BookOfMagick.GLIMMER_EFFECT, ref iOrientation, out this.mEffect);
    this.mBody.Immovable = iImmovable;
    if (iImmovable)
      return;
    this.mBody.Velocity = iVelocity;
    this.mBody.EnableBody();
  }

  public MagickType Magick => this.mMagick;

  public string PickUpString => this.mPickUpString;

  public bool Resting => (double) this.mRestingTimer < 0.0;

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if ((double) this.mTimeOutTimer > 0.0)
    {
      this.mTimeOutTimer -= iDeltaTime;
      Vector3 position = this.Position;
      Vector3 right = Vector3.Right;
      bool flag = EffectManager.Instance.UpdatePositionDirection(ref this.mTimeOutEffect, ref position, ref right);
      if ((double) this.mTimeOutTimer <= 0.0)
      {
        EffectManager.Instance.StartEffect(BookOfMagick.DISAPPEAR_EFFECT, ref position, ref right, out VisualEffectReference _);
        if (NetworkManager.Instance.State != NetworkState.Client)
          this.Kill();
      }
      else if (!flag && (double) this.mTimeOutTimer <= 4.0)
        EffectManager.Instance.StartEffect(BookOfMagick.TIMEOUT_EFFECT, ref position, ref right, out this.mTimeOutEffect);
    }
    if (this.mBody.IsActive)
      this.mRestingTimer = 1f;
    else
      this.mRestingTimer -= iDeltaTime;
    Matrix orientation = this.mBody.Orientation with
    {
      Translation = this.mBody.Position
    };
    EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
  }

  public void Unlock(Player iPlayer)
  {
    if (this.mPlayState.GameType == GameType.Versus)
    {
      if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
      {
        Player[] players = Magicka.Game.Instance.Players;
        for (int index = 0; index < players.Length; ++index)
        {
          if ((players[index].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
            SpellManager.Instance.UnlockMagick(players[index], this.mMagick);
        }
      }
      else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
      {
        Player[] players = Magicka.Game.Instance.Players;
        for (int index = 0; index < players.Length; ++index)
        {
          if ((players[index].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
            SpellManager.Instance.UnlockMagick(players[index], this.mMagick);
        }
      }
      else
        SpellManager.Instance.UnlockMagick(iPlayer, this.mMagick);
    }
    else
    {
      SpellManager.Instance.UnlockMagick(this.mMagick, this.mPlayState.GameType);
      if (this.mPlayState.GameType == GameType.Campaign)
        TelemetryUtils.SendCollectSpellbook(this.mMagick);
    }
    this.Kill();
  }

  public override bool Dead => this.mDead;

  public override bool Removable => this.Dead;

  public override void Kill()
  {
    this.mDead = true;
    this.mTimeOutTimer = 0.0f;
    EffectManager.Instance.Stop(ref this.mEffect);
    EffectManager.Instance.Stop(ref this.mTimeOutEffect);
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
    if (this.mBody.Immovable || !this.mBody.IsActive)
      iPrediction = 0.0f;
    if (this.Resting)
      return;
    JigLibX.Math.Transform transform = this.mBody.Transform;
    TransformRate transformRate = this.mBody.TransformRate;
    transform.ApplyTransformRate(ref transformRate, iPrediction);
    oMsg.Features |= EntityFeatures.Position;
    oMsg.Position = transform.Position;
    oMsg.Features |= EntityFeatures.Orientation;
    Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
    oMsg.Features |= EntityFeatures.Velocity;
    oMsg.Velocity = this.mBody.Velocity;
  }

  public override bool Permanent => (double) this.mTimeOutTimer <= 0.0 && !this.Dead;
}
