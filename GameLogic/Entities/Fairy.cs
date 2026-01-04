// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Fairy
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Physics;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities;

internal class Fairy : Entity
{
  private const float TIME_UNTIL_NEXT_DIALOG_CAP = 60f;
  private Character mOwner;
  private Fairy.FairyState mCurrentState;
  private Vector3 mDeathPos;
  private Vector3 mRevivalTarget;
  private float mAnimPosition;
  private VisualEffectReference mEffectRef;
  private VisualEffectReference mCrashEffectRef;
  private float mTime;
  private float mDefaultOrbitRadius = 0.5f;
  private float mOrbitRadius = 0.5f;
  private float mWaitBeforeRevive;
  private bool mDeathsoundPlayed;
  private static float sTimeUntilNextDialog = 30f;
  private static float sDialogTimer;
  private int mLastDialogGreeting = -1;
  private int mLastDialogTip = -1;
  private bool mFirstCheckAfterCheckFail;
  private static readonly int[] GREETING_DIALOG_ID = new int[2]
  {
    "fairy_greet0".GetHashCodeCustom(),
    "fairy_greet1".GetHashCodeCustom()
  };
  private static readonly int[][] DIALOG_ID = new int[2][]
  {
    new int[5]
    {
      "fairy20".GetHashCodeCustom(),
      "fairy21".GetHashCodeCustom(),
      "fairy22".GetHashCodeCustom(),
      "fairy23".GetHashCodeCustom(),
      "fairy24".GetHashCodeCustom()
    },
    new int[5]
    {
      "fairy30".GetHashCodeCustom(),
      "fairy31".GetHashCodeCustom(),
      "fairy32".GetHashCodeCustom(),
      "fairy33".GetHashCodeCustom(),
      "fairy34".GetHashCodeCustom()
    }
  };
  private bool mActive;

  public bool Active
  {
    get => this.mActive;
    private set => this.mActive = value;
  }

  public float WaitBeforeRevive
  {
    get => this.mWaitBeforeRevive;
    set => this.mWaitBeforeRevive = value;
  }

  public static Fairy MakeFairy(PlayState iPlayState, Character iAvatar)
  {
    return new Fairy(iPlayState, iAvatar);
  }

  private Fairy(PlayState iPlayState, Character iOwner)
    : base(iPlayState)
  {
    this.mOwner = iOwner;
    this.Active = false;
    this.mBody = new Body();
    this.mBody.ApplyGravity = false;
    this.mBody.AllowFreezing = false;
    this.mCollision = new CollisionSkin(this.mBody);
    this.mBody.CollisionSkin = this.mCollision;
  }

  public void Initialize(PlayState iPlayState, bool iShowGreetingDialog)
  {
    this.Initialize("#ent_fairy".GetHashCodeCustom());
    this.mPlayState = iPlayState;
    this.mPlayState.EntityManager.AddEntity((Entity) this);
    if (!this.Active && iShowGreetingDialog)
      this.ShowGreetingDialog();
    this.Active = true;
    this.Play();
    if ((double) Fairy.sDialogTimer <= 0.0)
      Fairy.sDialogTimer = Fairy.sTimeUntilNextDialog;
    this.mDeathsoundPlayed = false;
  }

  private void Play()
  {
    this.mCurrentState = Fairy.FairyState.Orbiting;
    if (EffectManager.Instance.IsActive(ref this.mEffectRef))
      return;
    Matrix translation = Matrix.CreateTranslation(this.mOwner.Position);
    EffectManager.Instance.StartEffect("special_fairy".GetHashCodeCustom(), ref translation, out this.mEffectRef);
  }

  private void Stop() => EffectManager.Instance.Stop(ref this.mEffectRef);

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.Active)
      return;
    base.Update(iDataChannel, iDeltaTime);
    bool flag = this.CheckDialogs(iDeltaTime);
    if (!this.mFirstCheckAfterCheckFail && !flag)
      this.mFirstCheckAfterCheckFail = true;
    switch (this.mCurrentState)
    {
      case Fairy.FairyState.Orbiting:
        if (this.mOwner != null && this.mOwner.Dead)
        {
          if ((double) this.mWaitBeforeRevive > 0.0)
            this.mWaitBeforeRevive -= iDeltaTime;
          if ((double) this.mWaitBeforeRevive <= 0.0)
          {
            this.mWaitBeforeRevive = 0.0f;
            this.FindRevivePosition();
            break;
          }
          this.OrbitAround(iDeltaTime);
          break;
        }
        this.OrbitAround(iDeltaTime);
        break;
      case Fairy.FairyState.FlyToRevivePosition:
        this.FlyTowardsTarget(iDeltaTime);
        break;
      case Fairy.FairyState.Reviving:
        this.ReviveAvatar();
        break;
      case Fairy.FairyState.WaitingForAvatar:
        if (!(this.mOwner is Avatar) || (this.mOwner as Avatar).Player.Avatar.Dead)
          break;
        this.Active = false;
        (this.mOwner as Avatar).Player.Avatar.TimedEthereal(0.0f, true);
        (this.mOwner as Avatar).Player.Avatar.TimedEthereal(5f, false);
        break;
    }
  }

  private void FindRevivePosition()
  {
    if (this.mPlayState.Camera.CurrentBehaviour == CameraBehaviour.FollowPlayers)
      this.mPlayState.Camera.Follow((Entity) this);
    this.mAnimPosition = 0.0f;
    this.mDeathPos = this.mBody.Position;
    this.mRevivalTarget = this.mOwner.CharacterBody.LastPositionOnGround;
    Vector3 oPoint;
    double nearestPosition = (double) this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref this.mRevivalTarget, out oPoint, MovementProperties.Default);
    this.mRevivalTarget = oPoint;
    this.mCurrentState = Fairy.FairyState.FlyToRevivePosition;
    this.mOrbitRadius = this.mDefaultOrbitRadius * 1f;
  }

  private void OrbitAround(float iDeltaTime)
  {
    this.mDefaultOrbitRadius = 0.8f;
    this.mOrbitRadius = this.mDefaultOrbitRadius;
    float num1 = (float) (6.2831854820251465 * 0.25) * this.mTime;
    float num2 = this.mOrbitRadius * (float) Math.Cos((double) num1);
    float num3 = this.mOrbitRadius * (float) Math.Sin((double) num1);
    float num4 = 0.25f * (float) Math.Sin((double) ((float) (6.2831854820251465 * 0.34999999403953552) * this.mTime)) + 1f;
    Vector3 position = this.mOwner.Position;
    Vector3 iPosition = new Vector3(position.X + num2, position.Y + num4, position.Z + num3);
    Vector3 direction = this.mOwner.Direction;
    this.mTime += iDeltaTime;
    this.mBody.Position = this.mOwner.Position;
    EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref iPosition, ref direction);
  }

  private void FlyTowardsTarget(float iDeltaTime)
  {
    this.mAnimPosition += iDeltaTime;
    float num1 = 2.1f;
    if ((double) num1 - (double) this.mAnimPosition < 1.3999999761581421 && !this.mDeathsoundPlayed)
    {
      this.mDeathsoundPlayed = true;
      AudioManager.Instance.PlayCue(Banks.Additional, "fairy_death".GetHashCodeCustom(), this.AudioEmitter);
    }
    if ((double) this.mAnimPosition >= (double) num1)
    {
      this.mCurrentState = Fairy.FairyState.Reviving;
    }
    else
    {
      Vector3 result = new Vector3();
      float num2 = ((double) this.mAnimPosition >= 1.0 ? (float) (1.0 - ((double) this.mAnimPosition - 1.0) / 1.0) : this.mAnimPosition / 1f) * 1.5f;
      float num3 = (float) (((double) this.mAnimPosition - 1.0) * 12.566370964050293);
      float num4 = ((double) this.mAnimPosition <= 2.0 ? ((double) this.mAnimPosition <= 1.0 ? 0.0f : (float) (((double) this.mAnimPosition - 1.0) / 1.0)) : (float) (1.0 - ((double) this.mAnimPosition - 1.0 - 1.0) / 0.10000000149011612)) * 2f;
      result.Y = this.mRevivalTarget.Y + num4;
      result.X = this.mRevivalTarget.X + (float) Math.Cos((double) num3) * num2;
      result.Z = this.mRevivalTarget.Z + (float) Math.Sin((double) num3) * num2;
      if ((double) this.mAnimPosition < 1.0)
        Vector3.SmoothStep(ref this.mDeathPos, ref result, this.mAnimPosition / 1f, out result);
      Vector3 forward = Vector3.Forward;
      EffectManager.Instance.UpdatePositionDirection(ref this.mEffectRef, ref result, ref forward);
      if ((double) this.mAnimPosition < 1.0)
        Vector3.SmoothStep(ref this.mDeathPos, ref this.mRevivalTarget, this.mAnimPosition / 1f, out result);
      else
        result = this.mRevivalTarget;
      this.mBody.Position = result;
    }
  }

  private void ReviveAvatar()
  {
    EffectManager.Instance.Stop(ref this.mEffectRef);
    Matrix translation = Matrix.CreateTranslation(this.mRevivalTarget);
    EffectManager.Instance.StartEffect("special_fairy_crash".GetHashCodeCustom(), ref translation, out this.mCrashEffectRef);
    if (this.mPlayState.Camera.CurrentBehaviour == CameraBehaviour.FollowEntity && this.mPlayState.Camera.TargetEntity == this)
      this.mPlayState.Camera.Release_NoMagReset(2f);
    Revive.GetInstance().Execute(this.mRevivalTarget, this.mPlayState);
    this.mCurrentState = Fairy.FairyState.WaitingForAvatar;
    Damage iDamage = new Damage(AttackProperties.Knockback, Elements.Earth, 100f, 2f);
    Vector3 position = this.mBody.Position;
    int num = (int) Helper.CircleDamage(this.mPlayState, (Entity) this, this.mPlayState.PlayTime, (Entity) this, ref position, 3f, ref iDamage);
    if (this.mOwner is Avatar)
      this.mPlayState.RemoveFairyFrom(this.mOwner as Avatar);
    this.mDeathsoundPlayed = false;
  }

  private void ShowGreetingDialog()
  {
    int index = MagickaMath.Random.Next(Fairy.GREETING_DIALOG_ID.Length);
    DialogManager.Instance.StartDialog(Fairy.GREETING_DIALOG_ID[index], (Entity) this, (Magicka.GameLogic.Controls.Controller) null);
  }

  private void ShowRandomDialog()
  {
    int index1 = this.mLastDialogGreeting;
    while (index1 == this.mLastDialogGreeting)
      index1 = MagickaMath.Random.Next(Fairy.DIALOG_ID.Length);
    int index2 = this.mLastDialogTip;
    while (index2 == this.mLastDialogTip)
      index2 = MagickaMath.Random.Next(Fairy.DIALOG_ID[0].Length);
    this.mLastDialogGreeting = index1;
    this.mLastDialogTip = index2;
    DialogManager.Instance.StartDialog(Fairy.DIALOG_ID[index1][index2], (Entity) this, (Magicka.GameLogic.Controls.Controller) null);
  }

  private bool CheckDialogs(float iDeltaTime)
  {
    if (this.mPlayState.IsInCutscene || this.mCurrentState != Fairy.FairyState.Orbiting)
    {
      if (this.mLastDialogGreeting != -1 && this.mLastDialogTip != -1)
      {
        int iDialog = Fairy.DIALOG_ID[this.mLastDialogGreeting][this.mLastDialogTip];
        if (DialogManager.Instance.DialogActive(iDialog))
          DialogManager.Instance.End(iDialog);
      }
      return false;
    }
    if (DialogManager.Instance.MessageBoxActive | DialogManager.Instance.IsDialogActive | DialogManager.Instance.AwaitingInput || ControlManager.Instance.IsInputLimited || AudioManager.Instance.Threat || BossFight.Instance.IsRunning || Credits.Instance.IsActive)
      return false;
    if (this.mFirstCheckAfterCheckFail)
    {
      Fairy.sDialogTimer = Math.Max(Fairy.sDialogTimer, 15f);
      this.mFirstCheckAfterCheckFail = false;
    }
    Fairy.sDialogTimer -= iDeltaTime;
    if ((double) Fairy.sDialogTimer <= 0.0)
    {
      this.ShowRandomDialog();
      if ((double) Fairy.sTimeUntilNextDialog < 60.0)
        Fairy.sTimeUntilNextDialog += 5f;
      Fairy.sDialogTimer = Fairy.sTimeUntilNextDialog;
    }
    return true;
  }

  public override bool Dead => false;

  public override bool Removable => !this.mActive;

  public override void Kill()
  {
  }

  internal override bool SendsNetworkUpdate(NetworkState iState) => false;

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    oMsg = new EntityUpdateMessage();
  }

  private enum FairyState
  {
    Orbiting,
    FlyToRevivePosition,
    Reviving,
    WaitingForAvatar,
  }
}
