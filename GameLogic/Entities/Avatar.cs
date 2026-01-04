// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Avatar
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Achievements;
using Magicka.AI;
using Magicka.AI.AgentStates;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Entities.AnimationActions;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Magicka.PathFinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using SteamWrapper;
using System;
using System.Collections.Generic;
using XNAnimation;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class Avatar : Character, IAI
{
  private static List<Avatar> mCache = new List<Avatar>(16 /*0x10*/);
  public static readonly int WIZARDHASH = "wizard".GetHashCodeCustom();
  private static readonly int UNARMED = "weapon_unarmed".GetHashCodeCustom();
  private Avatar.OutlineRenderData[] mOutlineRenderData;
  private Avatar.AfterImageRenderData[] mAfterImageRenderData;
  private Avatar.TargettingRenderData[] mTargettingRenderData;
  private float mDeadAge;
  private Magicka.GameLogic.Player mPlayer;
  private Stack<CharacterActionMessage> mQueuedNetActions = new Stack<CharacterActionMessage>(16 /*0x10*/);
  private int mChantedSpells;
  private CastType mCastButtonPressed;
  private bool mLeftTriggerActive;
  private bool mRightTriggerActive;
  private bool mCanChant;
  private ControllerDirection mChantDirection;
  private bool mMove;
  private bool mPolymorphed;
  private int mBoosts;
  private float mBoostCooldown;
  private bool mChantingMagick;
  private float mShowStaffSpecialAbilityNotifierTimer;
  private string mSpecialAbilityName;
  private Queue<MissileEntity> mMissileCache = new Queue<MissileEntity>(32 /*0x20*/);
  private Vector3 mDesiredInputDirection;
  private bool mChargeUnlocked = true;
  private float mAfterImageTimer;
  private float mAfterImageIntensity;
  private float mTargettingAlpha;
  private bool mHeavyWeaponReload;
  private Fairy mFairy;
  private bool mIgnoreTriggers;
  private Stack<IAIState> mStates = new Stack<IAIState>(5);
  private AIEvent[] mEvents;
  private List<PathNode> mPath = new List<PathNode>(512 /*0x0200*/);

  public static void ClearCache() => Avatar.mCache.Clear();

  public static void InitializeCache(PlayState iPlayState)
  {
    Avatar.mCache.Clear();
    for (int index = 0; index < 16 /*0x10*/; ++index)
      Avatar.mCache.Add(new Avatar(iPlayState));
  }

  public static Avatar GetFromCache(Magicka.GameLogic.Player iPlayer)
  {
    if (iPlayer == null)
      throw new ArgumentException("iPlayer cannot be null", nameof (iPlayer));
    Avatar fromCache = Avatar.mCache[0];
    Avatar.mCache.RemoveAt(0);
    fromCache.mPlayer = iPlayer;
    return fromCache;
  }

  public static Avatar GetFromCache(Magicka.GameLogic.Player iPlayer, ushort iHandle)
  {
    if (iPlayer == null)
      throw new ArgumentException("iPlayer cannot be null", nameof (iPlayer));
    Avatar fromHandle = Entity.GetFromHandle((int) iHandle) as Avatar;
    Avatar.mCache.Remove(fromHandle);
    fromHandle.mPlayer = iPlayer;
    return fromHandle;
  }

  internal Fairy RevivalFairy
  {
    get => this.mFairy;
    set => this.mFairy = value;
  }

  private Avatar(PlayState iPlayState)
    : base(iPlayState)
  {
    this.mPlayer = (Magicka.GameLogic.Player) null;
    for (int index = 0; index < 32 /*0x20*/; ++index)
      this.mMissileCache.Enqueue(new MissileEntity(iPlayState));
    this.mOutlineRenderData = new Avatar.OutlineRenderData[3];
    for (int index = 0; index < this.mOutlineRenderData.Length; ++index)
    {
      this.mOutlineRenderData[index] = new Avatar.OutlineRenderData();
      this.mOutlineRenderData[index].mSkeleton = this.mRenderData[index].mSkeleton;
    }
    this.mAfterImageRenderData = new Avatar.AfterImageRenderData[3];
    Matrix[][] iSkeleton = new Matrix[5][];
    for (int index = 0; index < iSkeleton.Length; ++index)
      iSkeleton[index] = new Matrix[80 /*0x50*/];
    for (int index = 0; index < 3; ++index)
      this.mAfterImageRenderData[index] = new Avatar.AfterImageRenderData(iSkeleton);
    this.mTargettingRenderData = new Avatar.TargettingRenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mTargettingRenderData[index] = new Avatar.TargettingRenderData(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/AimAid"), (Texture2D) null);
    this.mFairy = Fairy.MakeFairy(iPlayState, (Character) this);
  }

  private int PlayerIndex
  {
    get
    {
      for (int playerIndex = 0; playerIndex < Magicka.Game.Instance.Players.Length; ++playerIndex)
      {
        if (Magicka.Game.Instance.Players[playerIndex] == this.mPlayer)
          return playerIndex;
      }
      return -1;
    }
  }

  public void CheckInventory()
  {
    if (this.mPlayState.Inventory.Owner != null && this.mPlayState.Inventory.Owner != this || this.mPolymorphed || this.Equipment[0].Item.Type == Avatar.UNARMED | this.Equipment[1].Item.Type == Avatar.UNARMED)
      return;
    if (this.mPlayState.Inventory.Active)
      this.mPlayState.Inventory.Close((Character) this);
    else
      this.mPlayState.Inventory.ShowInventory(this.Equipment[0].Item, this.Equipment[1].Item, (Character) this);
  }

  internal Vector3 DesiredInputDirection => this.mDesiredInputDirection;

  internal void GenerateForceFeedback(float iLeft, float iRight)
  {
    if (this.Player.Controller == null)
      return;
    this.Player.Controller.Rumble(iLeft, iRight);
  }

  public override void Initialize(CharacterTemplate iTemplate, Vector3 iPosition, int iUniqueID)
  {
    this.Initialize(iTemplate, this.Player.ID, iPosition, iUniqueID);
  }

  public override void Initialize(
    CharacterTemplate iTemplate,
    int iRandomOverride,
    Vector3 iPosition,
    int iUniqueID)
  {
    this.mStates.Clear();
    this.mStates.Push((IAIState) AIStateIdle.Instance);
    this.mHeavyWeaponReload = false;
    this.mEvents = (AIEvent[]) null;
    if (iTemplate.ID == Magicka.GameLogic.Player.ALUCART_UNIQUE_ID)
      this.SetUniqueID(Magicka.GameLogic.Player.ALUCART_UNIQUE_ID);
    if (this.mPlayState.Level.ForceCamera)
      this.mPlayState.Camera.AttachPlayers((Character) this);
    else if (!(this.Player.Gamer is NetworkGamer))
      this.mPlayState.Camera.AttachPlayers((Character) this);
    else if (this.Player.Gamer is NetworkGamer)
      this.mPlayState.Camera.AttachNetworkPlayers((Character) this);
    base.Initialize(iTemplate, iRandomOverride, iPosition, iUniqueID);
    Level.AvatarItem[] additionalAvatarItems = this.PlayState.Level.AdditionalAvatarItems;
    for (int index1 = 0; index1 < additionalAvatarItems.Length; ++index1)
    {
      SkinnedModelBone iBone = (SkinnedModelBone) null;
      for (int index2 = 0; index2 < this.mModel.SkeletonBones.Count; ++index2)
      {
        if (this.mModel.SkeletonBones[index2].Name.Equals(additionalAvatarItems[index1].Bone, StringComparison.OrdinalIgnoreCase))
        {
          iBone = this.mModel.SkeletonBones[index2];
          break;
        }
      }
      if (iBone != null && additionalAvatarItems[index1].Item != null)
      {
        for (int index3 = 0; index3 < this.mEquipment.Length; ++index3)
        {
          if (this.mEquipment[index3].AttachIndex < 0)
          {
            this.mEquipment[index3].Set(additionalAvatarItems[index1].Item, iBone, new Vector3?());
            break;
          }
        }
      }
    }
    if (!this.mPolymorphed)
    {
      if (!string.IsNullOrEmpty(this.Player.Weapon))
        Item.GetCachedWeapon(this.Player.Weapon.GetHashCodeCustom(), this.mEquipment[0].Item);
      if (!string.IsNullOrEmpty(this.Player.Staff))
        Item.GetCachedWeapon(this.Player.Staff.GetHashCodeCustom(), this.mEquipment[1].Item);
    }
    for (int index = 0; index < this.mOutlineRenderData.Length; ++index)
      this.mOutlineRenderData[index].SetMeshDirty();
    this.mTurnSpeedMax = this.mTurnSpeed = 12f;
    this.mMove = false;
    this.mPolymorphed = false;
    Magicka.GameLogic.Player player = this.Player;
    this.mBoosts = 0;
    this.mBoostCooldown = 0.0f;
    this.mCanChant = true;
    this.mLeftTriggerActive = false;
    this.mRightTriggerActive = false;
    this.mShowStaffSpecialAbilityNotifierTimer = 0.0f;
    this.mDeadAge = 0.0f;
    this.mChantedSpells = 0;
    if (player.Controller != null)
      player.Controller.Invert(false);
    this.ResetAfterImages();
    this.mCastButtonPressed = CastType.None;
    player.IconRenderer.SetCapacity(5);
    player.IconRenderer.Clear();
    player.InputQueue.Clear();
    if (this.Equipment[1].Item.HasSpecialAbility)
    {
      this.mSpecialAbilityName = LanguageManager.Instance.GetString(this.Equipment[1].Item.SpecialAbilityName);
      this.mShowStaffSpecialAbilityNotifierTimer = 3f;
    }
    this.mFaction |= (Factions) (256 /*0x0100*/ << player.ID);
    this.mFaction |= player.Team;
    if (this.mPlayState.Level.CurrentScene != null && this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
      this.mFaction &= ~Factions.FRIENDLY;
    switch (player.Team)
    {
      case Factions.TEAM_RED:
        this.mMaterial.TintColor = Defines.TEAMCOLORS[0];
        break;
      case Factions.TEAM_BLUE:
        this.mMaterial.TintColor = Defines.TEAMCOLORS[1];
        break;
      default:
        this.mMaterial.TintColor = Defines.PLAYERCOLORS[(int) player.Color];
        break;
    }
    this.SetImmortalTime(0.0f);
    this.Player.Ressing = false;
    this.mDoNotRender = false;
  }

  public override void Deinitialize()
  {
    base.Deinitialize();
    if (!Avatar.mCache.Contains(this))
      Avatar.mCache.Add(this);
    this.mPlayState.Inventory.Close((Character) this);
  }

  public bool FindPickUp(bool iCheckDirection, Pickable iItem)
  {
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 2.5f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Pickable pickable && pickable.IsPickable)
      {
        Vector3 vector1 = this.Position with { Y = 0.0f };
        Vector3 result1 = pickable.Position with
        {
          Y = 0.0f
        };
        Vector3.Subtract(ref result1, ref vector1, out result1);
        float num = result1.Length();
        if ((double) num <= (double) this.Radius && iItem == pickable)
        {
          this.mPlayState.EntityManager.ReturnEntityList(entities);
          return true;
        }
        if (iCheckDirection)
        {
          Vector3.Divide(ref result1, num, out result1);
          vector1 = this.CharacterBody.Direction;
          float result2;
          Vector3.Dot(ref vector1, ref result1, out result2);
          if ((double) result2 > 0.0 && iItem == pickable)
          {
            this.mPlayState.EntityManager.ReturnEntityList(entities);
            return true;
          }
        }
        else if (iItem == pickable)
        {
          this.mPlayState.EntityManager.ReturnEntityList(entities);
          return true;
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    return false;
  }

  public Pickable FindPickUp(bool iCheckDirection)
  {
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 2.5f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Pickable pickUp && pickUp.IsPickable)
      {
        Vector3 vector1 = this.Position with { Y = 0.0f };
        Vector3 result1 = pickUp.Position with { Y = 0.0f };
        Vector3.Subtract(ref result1, ref vector1, out result1);
        float num = result1.Length();
        if ((double) num <= (double) this.Radius)
        {
          this.mPlayState.EntityManager.ReturnEntityList(entities);
          return pickUp;
        }
        if (iCheckDirection)
        {
          Vector3.Divide(ref result1, num, out result1);
          vector1 = this.CharacterBody.Direction;
          float result2;
          Vector3.Dot(ref vector1, ref result1, out result2);
          if ((double) result2 > 0.0)
          {
            this.mPlayState.EntityManager.ReturnEntityList(entities);
            return pickUp;
          }
        }
        else
        {
          this.mPlayState.EntityManager.ReturnEntityList(entities);
          return pickUp;
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    return (Pickable) null;
  }

  public Interactable FindInteractable(bool iCheckDirection)
  {
    SortedList<int, Trigger> triggers = this.mPlayState.Level.CurrentScene.Triggers;
    for (int index = 0; index < triggers.Count; ++index)
    {
      if (triggers.Values[index] is Interactable interactable && interactable.Enabled)
      {
        Vector3 position = this.Position with { Y = 0.0f };
        Locator locator = interactable.Locator;
        Vector3 translation = locator.Transform.Translation with
        {
          Y = 0.0f
        };
        float result1;
        Vector3.DistanceSquared(ref translation, ref position, out result1);
        float radius = this.Capsule.Radius;
        if ((double) result1 <= (double) radius * (double) radius)
          return interactable;
        if ((double) result1 <= (double) locator.Radius * (double) locator.Radius)
        {
          if (!iCheckDirection)
            return interactable;
          Vector3 result2;
          Vector3.Subtract(ref translation, ref position, out result2);
          Vector3 direction = this.CharacterBody.Direction;
          result2.Normalize();
          float result3;
          Vector3.Dot(ref direction, ref result2, out result3);
          if ((double) result3 > 0.699999988079071)
            return interactable;
        }
      }
    }
    return (Interactable) null;
  }

  public Character FindCharacter(bool iCheckDirection)
  {
    float num1 = float.MaxValue;
    Character character1 = (Character) null;
    List<Entity> entities = this.mPlayState.EntityManager.GetEntities(this.Position, 3f, false);
    entities.Remove((Entity) this);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Character character2 && character2.InteractText != InteractType.None)
      {
        Vector3 vector1 = this.Position with { Y = 0.0f };
        Vector3 result1 = character2.Position with
        {
          Y = 0.0f
        };
        Vector3.Subtract(ref result1, ref vector1, out result1);
        if (iCheckDirection)
        {
          result1.Normalize();
          vector1 = this.CharacterBody.Direction;
          float result2;
          Vector3.Dot(ref vector1, ref result1, out result2);
          if ((double) result2 >= 0.699999988079071)
          {
            this.mPlayState.EntityManager.ReturnEntityList(entities);
            return character2;
          }
        }
        else
        {
          float num2 = result1.LengthSquared();
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            character1 = character2;
          }
        }
      }
    }
    this.mPlayState.EntityManager.ReturnEntityList(entities);
    return character1;
  }

  public override MissileEntity GetMissileInstance()
  {
    MissileEntity missileInstance = this.mMissileCache.Dequeue();
    this.mMissileCache.Enqueue(missileInstance);
    return missileInstance;
  }

  public override bool IsBlocking
  {
    set
    {
      if (this.mPolymorphed)
        return;
      base.IsBlocking = value;
    }
  }

  public string SpecialAbilityName => this.mSpecialAbilityName;

  public bool NotifySpecialAbility
  {
    get
    {
      if (this.mPolymorphed)
        return false;
      if ((double) this.mShowStaffSpecialAbilityNotifierTimer > 0.0 && this.mEquipment[1].Item.HasSpecialAbility)
        return true;
      return this.mEquipment[1].Item.CooldownHintTime && !string.IsNullOrEmpty(this.mSpecialAbilityName);
    }
  }

  internal override bool Polymorphed
  {
    get => this.mPolymorphed;
    set => this.mPolymorphed = value;
  }

  public void ResetAfterImages()
  {
    this.mAfterImageIntensity = -1f;
    this.mAfterImageTimer = 0.0f;
  }

  public bool ChantingMagick => this.mChantingMagick && this.MagickType != MagickType.None;

  public MagickType MagickType
  {
    get => SpellManager.Instance.GetMagickType(this.Player, this.mPlayState, this.SpellQueue);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mQueuedNetActions.Count > 0)
    {
      CharacterActionMessage iMsg = this.mQueuedNetActions.Pop();
      this.NetworkAction(ref iMsg);
    }
    if (this.mEvents != null)
    {
      if (this.CurrentEvent < this.mEvents.Length | (double) this.CurrentEventDelay >= 1.4012984643248171E-45 | float.IsNaN(this.CurrentEventDelay))
      {
        this.mStates.Peek().OnExecute((IAI) this, iDeltaTime);
        this.CurrentStateAge += iDeltaTime;
      }
      else
        this.mIgnoreTriggers = false;
    }
    else
      this.mIgnoreTriggers = false;
    if (this.mCurrentSpell == null && this.CastButton(CastType.Weapon) && this.CastType == CastType.None || this.mCurrentSpell == null && this.CastButton(CastType.Self) && this.CastType == CastType.None)
    {
      this.mCastButtonPressed = CastType.None;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.CastSpell,
          Param0I = 0
        });
    }
    if (!this.Dead)
    {
      if (GlobalSettings.Instance.HealthBars != SettingOptions.Off & ((double) this.mInvisibilityTimer <= 0.0 | this.mPlayState.GameType != GameType.Versus))
      {
        Vector4 vector4 = new Vector4(1f, 0.0f, 0.0f, 1f);
        if (this.HasStatus(StatusEffects.Poisoned))
        {
          vector4.X = 0.0f;
          vector4.Y = 1f;
        }
        float iTimeSinceLastDamage = Math.Min(this.mTimeSinceLastDamage, this.mTimeSinceLastStatusDamage);
        Vector3 position = this.Position;
        position.Y -= this.Capsule.Length * 0.5f + this.Capsule.Radius;
        Healthbars.Instance.AddHealthBar(position, this.NormalizedHitPoints, this.mTemplate.Radius, 1.25f, iTimeSinceLastDamage, false, new Vector4?(vector4), new Vector2?());
      }
      if ((double) this.mShowStaffSpecialAbilityNotifierTimer > 0.0)
        this.mShowStaffSpecialAbilityNotifierTimer -= iDeltaTime;
      if (SpellManager.Instance.GetMagickType(this.Player, this.PlayState, this.SpellQueue) == MagickType.None)
        this.mChantingMagick = false;
    }
    if (!this.IsInvisibile & !this.mDoNotRender & ((double) this.CharacterBody.SpeedMultiplier > (double) this.TimeWarpModifier | (double) this.mAfterImageIntensity > -1.0))
    {
      Avatar.AfterImageRenderData iObject = this.mAfterImageRenderData[(int) iDataChannel];
      if (iObject.MeshDirty)
      {
        ModelMesh mesh = this.mModel.Model.Meshes[0];
        ModelMeshPart meshPart = mesh.MeshParts[0];
        iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart);
        iObject.Color = this.mMaterial.TintColor;
      }
      int count = this.mModel.SkeletonBones.Count;
      this.mAfterImageTimer -= iDeltaTime;
      this.mAfterImageIntensity -= iDeltaTime;
      while ((double) this.mAfterImageTimer <= 0.0)
      {
        this.mAfterImageTimer += 0.05f;
        if ((double) (this.mBody.Velocity with { Y = 0.0f }).LengthSquared() > 1.0 / 1000.0 & (double) this.CharacterBody.SpeedMultiplier > 1.0)
        {
          while ((double) this.mAfterImageIntensity <= 0.0)
            this.mAfterImageIntensity += 0.05f;
        }
        for (int index = iObject.mSkeleton.Length - 1; index > 0; --index)
          Array.Copy((Array) iObject.mSkeleton[index - 1], (Array) iObject.mSkeleton[index], count);
        Array.Copy((Array) this.mAnimationController.SkinnedBoneTransforms, (Array) iObject.mSkeleton[0], count);
      }
      iObject.mIntensity = this.mAfterImageIntensity / 0.05f;
      iObject.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
      this.mPlayState.Scene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) iObject);
    }
    else
    {
      this.mAfterImageTimer = 0.0f;
      this.mAfterImageIntensity = -1f;
    }
    base.Update(iDataChannel, iDeltaTime);
    if ((double) this.mBoostCooldown > 0.0)
      this.mBoostCooldown -= iDeltaTime;
    if (this.Dead)
      this.mDeadAge += iDeltaTime;
    this.mCanChant = this.mCurrentState == IdleState.Instance || this.mCurrentState == MoveState.Instance || this.mCurrentState == CastState.Instance || this.mCurrentState == ChargeState.Instance && this.CastType == CastType.None;
    if (this.SpellQueue.Count > 0)
      this.CharacterBody.SpeedMultiplier *= (float) (1.0 - (double) this.SpellQueue.Count / (double) this.SpellQueue.Capacity * 0.60000002384185791);
    if (!this.Dead & !this.mDoNotRender & !this.PlayState.IsInCutscene && (double) this.mInvisibilityTimer <= 0.0 | this.mPlayState.GameType != GameType.Versus)
    {
      Avatar.OutlineRenderData iObject = this.mOutlineRenderData[(int) iDataChannel];
      iObject.Forced = this.IsInvisibile;
      if (iObject.MeshDirty)
      {
        ModelMesh mesh = this.mModel.Model.Meshes[0];
        ModelMeshPart meshPart = mesh.MeshParts[0];
        iObject.SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, SkinnedModelDeferredEffect.TYPEHASH);
        iObject.Color = this.mMaterial.TintColor;
      }
      iObject.mBoundingSphere = this.mRenderData[(int) iDataChannel].mBoundingSphere;
      this.mPlayState.Scene.AddProjection(iDataChannel, (IProjectionObject) iObject);
    }
    this.mTargettingAlpha = this.Player.Gamer is NetworkGamer || !(this.mCurrentState is ChargeState) || this.Player.Controller is KeyboardMouseController ? Math.Max(this.mTargettingAlpha - iDeltaTime * 4f, 0.0f) : Math.Min(this.mTargettingAlpha + iDeltaTime * 4f, 1f);
    if ((double) this.mTargettingAlpha > 0.0)
    {
      Avatar.TargettingRenderData iObject = this.mTargettingRenderData[(int) iDataChannel];
      Matrix iTransform = new Matrix();
      Vector3 result1 = this.Direction;
      Vector3 up = Vector3.Up;
      Vector3 result2;
      Vector3.Cross(ref result1, ref up, out result2);
      result2.Normalize();
      Vector3.Cross(ref up, ref result2, out result1);
      iTransform.Backward = up;
      iTransform.Right = result2;
      iTransform.Up = result1;
      Vector3 position = this.Position;
      position.Y += this.HeightOffset;
      iTransform.Translation = position;
      iObject.Glow = 1f;
      iObject.Alpha = this.mTargettingAlpha;
      iTransform.M11 *= 1.25f;
      iTransform.M12 *= 1.25f;
      iTransform.M13 *= 1.25f;
      iTransform.M21 *= 2.5f;
      iTransform.M22 *= 2.5f;
      iTransform.M23 *= 2.5f;
      iObject.TextureScale = new Vector2(0.25f, 1f);
      iObject.TextureOffset = new Vector2(0.6125f, 0.0f);
      iTransform.M31 *= 2f;
      iTransform.M32 *= 2f;
      iTransform.M33 *= 2f;
      iTransform.M44 = 1f;
      iObject.SetPosition(ref iTransform);
      this.mPlayState.Scene.AddProjection(iDataChannel, (IProjectionObject) iObject);
    }
    if (!this.IsCharmed || !(this.mCurrentState is CastState) && !(this.mCurrentState is ChargeState))
      return;
    Vector3 movement = this.CharacterBody.Movement;
    Vector3 oVector;
    if ((double) movement.LengthSquared() <= 9.9999999747524271E-07 || !this.InCharmDirection(ref movement, out oVector))
      return;
    this.CharacterBody.Movement = oVector;
  }

  public void UpdateMouseDirection(Vector2 iValue, bool iInverted)
  {
    if (this.mEvents != null && this.CurrentEvent < this.mEvents.Length || this.mHypnotized)
      return;
    if (float.IsNaN(iValue.X) || float.IsNaN(iValue.Y))
      iValue = new Vector2();
    if (ControlManager.Instance.IsInputLimited || ControlManager.Instance.IsPlayerInputLocked(this.Player.ID))
    {
      this.CharacterBody.Movement = new Vector3();
    }
    else
    {
      float d = iValue.LengthSquared();
      if ((double) d < 9.9999999747524271E-07 && !this.IsPanicing && !this.IsFeared && !this.IsStumbling)
      {
        this.CharacterBody.Movement = new Vector3();
        this.CharacterBody.DesiredDirection = this.CharacterBody.Transform.Orientation.Forward;
      }
      else
      {
        Vector3 result = !iInverted ? new Vector3(iValue.X, 0.0f, iValue.Y) : new Vector3(-iValue.X, 0.0f, -iValue.Y);
        if ((double) d > 9.9999999747524271E-07)
        {
          float num = (float) Math.Sqrt((double) d);
          Vector3.Divide(ref result, num, out result);
        }
        this.mDesiredInputDirection = result;
        Vector3 oVector;
        if (this.IsCharmed && (this.mCurrentState is CastState || this.mCurrentState is ChargeState) && this.InCharmDirection(ref result, out oVector))
          result = oVector;
        if (!this.mMove)
        {
          if (!this.IsPanicing && !this.IsFeared && (double) d > 0.10000000149011612)
            this.CharacterBody.Movement = result;
          result = new Vector3();
        }
        if (this.IsStumbling)
        {
          Vector3 movement = this.CharacterBody.Movement;
          Vector3.Multiply(ref result, -1f, out result);
          Vector3.Add(ref movement, ref result, out result);
          this.CharacterBody.Movement = result;
        }
        if (this.IsPanicing && (double) d > 0.0)
        {
          Vector3 movement = this.CharacterBody.Movement;
          result *= 1f - this.mPanic;
          Vector3.Add(ref movement, ref result, out result);
          this.CharacterBody.Movement = result;
        }
        if (this.IsFeared)
          return;
        this.CharacterBody.Movement = result;
      }
    }
  }

  public void UpdatePadDirection(Vector2 iValue, bool iInverted)
  {
    if (this.mEvents != null && this.CurrentEvent < this.mEvents.Length || this.mHypnotized)
      return;
    if (ControlManager.Instance.IsInputLimited || ControlManager.Instance.IsPlayerInputLocked(this.Player.ID))
    {
      this.CharacterBody.Movement = new Vector3();
    }
    else
    {
      float num = iValue.LengthSquared();
      Vector3 result = !iInverted ? new Vector3(iValue.X, 0.0f, -iValue.Y) : new Vector3(-iValue.X, 0.0f, iValue.Y);
      this.mDesiredInputDirection = result;
      if (this.IsCharmed && (this.mCurrentState is CastState || this.mCurrentState is ChargeState))
      {
        if ((double) num <= 9.9999999747524271E-07)
          result = this.Direction;
        Vector3 oVector;
        if (this.InCharmDirection(ref result, out oVector))
          result = oVector;
      }
      else if ((double) num < 9.9999999747524271E-07 && !this.IsPanicing && !this.IsFeared && !this.IsStumbling)
      {
        this.CharacterBody.Movement = new Vector3();
        this.CharacterBody.DesiredDirection = this.CharacterBody.Transform.Orientation.Forward;
        return;
      }
      if (this.IsStumbling)
      {
        Vector3 movement = this.CharacterBody.Movement;
        Vector3.Multiply(ref result, -1f, out result);
        Vector3.Add(ref movement, ref result, out result);
        this.CharacterBody.Movement = result;
      }
      if (this.IsPanicing && (double) num > 0.0)
      {
        Vector3 movement = this.CharacterBody.Movement;
        result *= 1f - this.mPanic;
        Vector3.Add(ref movement, ref result, out result);
        this.CharacterBody.Movement = result;
      }
      if (this.IsFeared)
        return;
      this.CharacterBody.Movement = result;
    }
  }

  private bool InCharmDirection(ref Vector3 iDirection, out Vector3 oVector)
  {
    Vector3 position1 = this.CharmOwner.Position;
    Vector3 position2 = this.Position;
    Vector3 direction = this.Direction;
    Vector3 result1;
    Vector3.Subtract(ref position1, ref position2, out result1);
    Vector2 vector1 = new Vector2(result1.X, result1.Z);
    vector1.Normalize();
    float num1 = MagickaMath.Angle(vector1);
    Vector2 vector2 = new Vector2(iDirection.X, iDirection.Z);
    vector2.Normalize();
    float num2 = MagickaMath.Angle(vector2);
    Vector2 vector3 = new Vector2(direction.X, direction.Z);
    vector3.Normalize();
    float num3 = MagickaMath.Angle(vector3);
    float num4 = num2 - num1;
    float num5 = num3 - num1;
    float num6 = 0.3926991f;
    if ((double) Math.Abs(num5) < (double) num6 && (double) Math.Abs(num4) < (double) num6)
    {
      Matrix result2;
      if ((double) num4 >= 0.0)
        Matrix.CreateRotationY(-1.57079637f, out result2);
      else
        Matrix.CreateRotationY(1.57079637f, out result2);
      Vector3.Transform(ref iDirection, ref result2, out oVector);
      return true;
    }
    oVector = new Vector3();
    return false;
  }

  public bool ChargeUnlocked
  {
    get => this.mChargeUnlocked;
    set => this.mChargeUnlocked = value;
  }

  public void ForcePressed()
  {
    if (this.Equipment[1].Item.WeaponClass != WeaponClass.Staff || !TutorialManager.Instance.IsCastTypeEnabled(CastType.Force) || this.mSpellQueue.Count == 0 && !TutorialManager.Instance.IsPushEnabled() || !(this.mCastButtonPressed == CastType.None & this.CastType == CastType.None))
      return;
    this.CastType = CastType.Force;
    this.mCastButtonPressed = CastType.Force;
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Handle = this.Handle,
      Action = ActionType.CastSpell,
      Param0I = (int) this.mCastButtonPressed
    });
  }

  public void ForceReleased()
  {
    this.mChargeUnlocked = true;
    if (this.mCastButtonPressed != CastType.Force)
      return;
    this.mCastButtonPressed = CastType.None;
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Handle = this.Handle,
      Action = ActionType.CastSpell,
      Param0I = 0
    });
  }

  public void AreaPressed()
  {
    if (this.Equipment[1].Item.WeaponClass != WeaponClass.Staff || !TutorialManager.Instance.IsCastTypeEnabled(CastType.Area) || this.mSpellQueue.Count == 0 && !TutorialManager.Instance.IsPushEnabled() || !(this.mCastButtonPressed == CastType.None & this.CastType == CastType.None))
      return;
    this.CastType = CastType.Area;
    this.mCastButtonPressed = CastType.Area;
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Handle = this.Handle,
      Action = ActionType.CastSpell,
      Param0I = (int) this.mCastButtonPressed
    });
  }

  public void AreaReleased()
  {
    if (this.mCastButtonPressed != CastType.Area)
      return;
    this.mCastButtonPressed = CastType.None;
    if (NetworkManager.Instance.State == NetworkState.Offline)
      return;
    NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
    {
      Handle = this.Handle,
      Action = ActionType.CastSpell,
      Param0I = 0
    });
  }

  public virtual void MouseMove() => this.mMove = true;

  public virtual void MouseMoveStop() => this.mMove = false;

  public override int Type => Avatar.WIZARDHASH;

  public override bool IsInAEvent
  {
    get => this.mEvents != null && this.CurrentEvent < this.mEvents.Length;
  }

  public override bool IsImmortal
  {
    get => this.IsInAEvent | this.mPlayState.IsInCutscene | base.IsImmortal;
  }

  public override CastType CastType
  {
    set
    {
      if (this.mPolymorphed)
        return;
      base.CastType = value;
    }
  }

  public bool CastButton(CastType iType) => this.mCastButtonPressed == iType;

  internal void ConjureEarth()
  {
    this.HandleCombo(ControllerDirection.Down);
    this.HandleCombo(ControllerDirection.Left);
  }

  internal void ConjureWater()
  {
    this.HandleCombo(ControllerDirection.Left);
    this.HandleCombo(ControllerDirection.Up);
  }

  internal void ConjureCold()
  {
    this.HandleCombo(ControllerDirection.Right);
    this.HandleCombo(ControllerDirection.Up);
  }

  internal void ConjureFire()
  {
    this.HandleCombo(ControllerDirection.Right);
    this.HandleCombo(ControllerDirection.Down);
  }

  internal void ConjureLightning()
  {
    this.HandleCombo(ControllerDirection.Left);
    this.HandleCombo(ControllerDirection.Down);
  }

  internal void ConjureArcane()
  {
    this.HandleCombo(ControllerDirection.Up);
    this.HandleCombo(ControllerDirection.Right);
  }

  internal void ConjureLife()
  {
    this.HandleCombo(ControllerDirection.Up);
    this.HandleCombo(ControllerDirection.Left);
  }

  internal void ConjureShield()
  {
    this.HandleCombo(ControllerDirection.Down);
    this.HandleCombo(ControllerDirection.Right);
  }

  internal void HandleCombo(ControllerDirection iDirection)
  {
    if (this.IsInAEvent || this.Polymorphed || this.mCurrentState is ChargeState)
      return;
    Magicka.GameLogic.Player player = this.Player;
    this.mChantDirection = iDirection;
    player.InputQueue.Add((int) iDirection);
    Spell iSpell = SpellManager.Instance.HandleCombo(player);
    if (iSpell.Element != Elements.None)
    {
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Action = ActionType.ConjureElement,
          Handle = this.Handle,
          Param0I = (int) iSpell.Element
        });
      if ((((iSpell.Element & Elements.Lightning) != Elements.Lightning || !this.HasStatus(StatusEffects.Wet) ? 0 : (!this.HasPassiveAbility(Item.PassiveAbilities.WetLightning) ? 1 : 0)) | ((iSpell.Element & Elements.Fire) != Elements.Fire ? 0 : (this.HasStatus(StatusEffects.Greased) ? 1 : 0))) != 0)
      {
        if (this.HasStatus(StatusEffects.Wet))
          TutorialManager.Instance.SetTip(TutorialManager.Tips.WetLightning, TutorialManager.Position.Top);
        DamageCollection5 oDamages;
        iSpell.CalculateDamage(SpellType.Lightning, CastType.Self, out oDamages);
        oDamages.MultiplyMagnitude(0.5f);
        int num = (int) this.Damage(oDamages, (Entity) this, this.PlayState.PlayTime, this.Position);
        player.InputQueue.Clear();
        return;
      }
      if (SpellManager.Instance.TryAddToQueue(this.Player, (Character) this, this.mSpellQueue, 5, ref iSpell))
        ++this.mChantedSpells;
      AudioManager.Instance.PlayCue(Banks.UI, Defines.SOUNDS_UI_ELEMENT[Spell.ElementIndex(iSpell.Element)]);
      player.InputQueue.Clear();
    }
    if (!this.mCanChant)
      this.mChantDirection = ControllerDirection.Center;
    this.mChantingMagick = SpellManager.Instance.IsMagick(this.Player, this.mPlayState.GameType, this.SpellQueue);
    if (!this.mChantingMagick)
      return;
    TutorialManager.Instance.SetTip(TutorialManager.Tips.MagicksSpell, TutorialManager.Position.Top);
  }

  public void Action()
  {
    if (this.mCurrentState is CastState || this.mCurrentState is ChargeState || this.mPolymorphed || DialogManager.Instance.AwaitingInput || DialogManager.Instance.HoldoffInput)
      return;
    if (!AudioManager.Instance.Threat)
    {
      Character character = this.FindCharacter(!(this.Player.Controller is KeyboardMouseController));
      if (character != null && character.Interact((Character) this, this.Player.Controller))
      {
        if (!(!(this.Player.Gamer is NetworkGamer) & NetworkManager.Instance.State != NetworkState.Offline))
          return;
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Action = ActionType.Interact,
          Handle = this.Handle,
          TargetHandle = character.Handle
        });
        return;
      }
    }
    Pickable pickUp = this.FindPickUp(!(this.Player.Controller is KeyboardMouseController));
    if (pickUp != null)
      this.PickUp(pickUp);
    Interactable interactable = this.FindInteractable(!(this.Player.Controller is KeyboardMouseController));
    if (interactable == null || AudioManager.Instance.Threat && interactable.InteractType == InteractType.Talk)
      return;
    interactable.Interact((Character) this);
  }

  public void Interact()
  {
    if (this.mCurrentState is CastState | this.mCurrentState is ChargeState || DialogManager.Instance.AwaitingInput || DialogManager.Instance.HoldoffInput)
      return;
    this.FindInteractable(!(this.Player.Controller is KeyboardMouseController))?.Interact((Character) this);
  }

  public void Boost()
  {
    if (this.mCurrentState is ChargeState || this.mPolymorphed)
      return;
    if (TutorialManager.Instance.IsCastTypeEnabled(CastType.Magick) && (SpellManager.Instance.CombineMagick(this.Player, this.mPlayState.GameType, this.mSpellQueue) || this.mSpellQueue.Count > 0 && this.mSpellQueue[0].Element == Elements.All))
    {
      this.CastType = CastType.Magick;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.CastSpell,
          Param0I = 5
        });
      if (!Magicka.GameLogic.Spells.Magick.IsInstant(new SpellMagickConverter()
      {
        Spell = this.mSpell
      }.Magick.MagickType) || !(this.HasStatus(StatusEffects.Frozen) | this.mCurrentState is FlyingState | this.mCurrentState is KnockDownState))
        return;
      this.CastSpell(true, "");
    }
    else if (this.IsEntangled | this.IsGripped)
    {
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.BreakFree
        });
      this.BreakFree();
      if (this.CurrentState is CastState)
        return;
      this.GoToAnimation(Magicka.Animations.spec_entangled_attack, 0.1f);
    }
    else
    {
      if (BoostState.Instance.ShieldToBoost((Character) this) == null && !(this.IsSelfShielded & !this.IsSolidSelfShielded))
        return;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.Boost
        });
      ++this.mBoosts;
      if (this.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ShieldBoost)
        ++this.mBoosts;
      this.mBoostCooldown = 0.25f;
      if (this.CurrentState != BoostState.Instance)
        this.mBoosts = 1;
      for (int index = 0; index < this.mAuras.Count; ++index)
      {
        AuraStorage aura = this.mAuras[index].Aura;
        if (aura.AuraType == AuraType.Boost)
          this.mBoosts *= (int) aura.AuraBoost.Magnitude;
      }
      for (int index = 0; index < this.Equipment[1].Item.Auras.Count; ++index)
      {
        AuraStorage aura = this.Equipment[1].Item.Auras[index].Aura;
        if (aura.AuraType == AuraType.Boost)
          this.mBoosts *= (int) aura.AuraBoost.Magnitude;
      }
      for (int index = 0; index < this.mBuffs.Count; ++index)
      {
        if (this.mBuffs[index].BuffType == BuffType.Boost)
          this.mBoosts *= (int) this.mBuffs[index].BuffBoost.Amount;
      }
    }
  }

  public void Attack()
  {
    if (this.mCurrentSpell != null || this.mCurrentState is CastState || this.mCurrentState is ChargeState || ControlManager.Instance.IsInputLimited || this.Equipment[0].Item.IsCoolingdown)
      return;
    if (this.SpellQueue.Count > 0 && this.SpellQueue[0].Element != Elements.All || this.Equipment[0].Item.SpellCharged)
    {
      if (this.CastType != CastType.None || this.IsGripped || !TutorialManager.Instance.IsCastTypeEnabled(CastType.Weapon))
        return;
      this.mCastButtonPressed = CastType.Weapon;
      this.CastType = CastType.Weapon;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.CastSpell,
          Param0I = (int) this.mCastButtonPressed
        });
      this.PlayState.TooFancyForFireballs[this.Player.ID] = false;
    }
    else
    {
      this.PlayState.TooFancyForFireballs[this.Player.ID] = false;
      if (this.mGripType == Grip.GripType.Pickup && this.IsGripped)
        this.mBreakFreeCounter += (int) this.mBreakFreeStrength;
      if (this.mAttacking && (double) this.mAnimationController.Time <= (double) this.mAnimationController.AnimationClip.Duration - 0.10000000149011612)
        return;
      if (this.mEquipment[0].Item.WeaponClass == WeaponClass.Heavy)
      {
        if (this.mAttacking)
          return;
        if (this.mHeavyWeaponReload)
          this.Attack(Magicka.Animations.attack_melee1, this.WieldingGun);
        else
          this.Attack(Magicka.Animations.attack_melee0, this.WieldingGun);
        this.mHeavyWeaponReload = !this.mHeavyWeaponReload;
      }
      else if (MagickaMath.Random.NextDouble() < 0.5)
        this.Attack(Magicka.Animations.attack_melee1, this.WieldingGun);
      else
        this.Attack(Magicka.Animations.attack_melee0, this.WieldingGun);
    }
  }

  public void Special()
  {
    if (ControlManager.Instance.IsInputLimited & this.mCurrentSpell == null)
      return;
    if (this.SpellQueue.Count > 0)
    {
      if (!TutorialManager.Instance.IsCastTypeEnabled(CastType.Self) || this.CastType != CastType.None)
        return;
      this.mCastButtonPressed = CastType.Self;
      this.CastType = CastType.Self;
      if (this.HasStatus(StatusEffects.Frozen) || this.mCurrentState is FlyingState || this.mCurrentState is PushedState || this.mCurrentState is KnockDownState)
        this.CastSpell(true, "");
      if (NetworkManager.Instance.State == NetworkState.Offline)
        return;
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = this.Handle,
        Action = ActionType.CastSpell,
        Param0I = (int) this.mCastButtonPressed
      });
    }
    else
    {
      if (this.mCurrentState is CastState | this.mCurrentState is ChargeState || !this.mEquipment[this.mSourceOfSpellIndex].Item.HasSpecialAbility)
        return;
      if (NetworkManager.Instance.State != NetworkState.Offline)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Param0I = this.mSourceOfSpellIndex,
          Action = ActionType.ItemSpecial
        });
      this.mEquipment[this.mSourceOfSpellIndex].Item.ExecuteSpecialAbility();
    }
  }

  public override void Entangle(float iMagnitude)
  {
    base.Entangle(iMagnitude);
    this.mBoosts = 0;
  }

  public void SpecialRelease()
  {
    if (this.mCastButtonPressed != CastType.Self)
      return;
    this.mCastButtonPressed = CastType.None;
  }

  public void AttackRelease()
  {
    if (this.CastType == CastType.None & this.mCastButtonPressed == CastType.Weapon)
      this.mCastButtonPressed = CastType.None;
    if (!this.WieldingGun)
      return;
    this.mNextAttackAnimation = Magicka.Animations.None;
  }

  public bool WieldingGun => this.mEquipment[0].Item.IsGunClass;

  internal ControllerDirection ChantDirection => this.mChantDirection;

  public override int Boosts
  {
    get => this.mBoosts;
    set => this.mBoosts = value;
  }

  public override float BoostCooldown => this.mBoostCooldown;

  public override bool IsAggressive => AudioManager.Instance.Threat;

  public bool LeftTrigger => this.mLeftTriggerActive;

  public bool RightTrigger => this.mRightTriggerActive;

  public Magicka.GameLogic.Player Player
  {
    get => this.mPlayer;
    set => this.mPlayer = value;
  }

  public override void BloatKill(Elements iElement, Entity iKiller)
  {
    base.BloatKill(iElement, iKiller);
    if (this.Player.Controller == null)
      return;
    this.Player.Controller.Rumble(1f, 1f);
  }

  public override void CastSpell(bool iFromStaff, string iJoint)
  {
    if (this.mSpell.Element != Elements.All)
      this.PlayState.TooFancyForFireballs[this.Player.ID] = false;
    if (this.mSpell.Element != Elements.All && this.CastType == CastType.Weapon)
    {
      Item obj = this.Equipment[0].Item;
      if (obj.SpellCharged && this.SpellQueue.Count == 0)
      {
        obj.RetrieveSpell().Cast(iFromStaff, (ISpellCaster) this, this.CastType);
      }
      else
      {
        for (int iIndex = 0; iIndex < this.SpellQueue.Count; ++iIndex)
        {
          Spell spell = this.SpellQueue[iIndex];
          obj.TryAddToQueue(ref spell, false);
        }
        this.Player.IconRenderer.Clear();
        this.SpellQueue.Clear();
        this.mChantedSpells = 0;
        if (this.Player.Gamer is NetworkGamer)
          return;
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "theenchanter");
      }
    }
    else
      base.CastSpell(iFromStaff, iJoint);
  }

  public override void CombineSpell()
  {
    this.Player.IconRenderer.Clear();
    this.mSpell = SpellManager.Instance.Combine(this.Player.SpellQueue);
    this.mChantedSpells = 0;
    this.Player.SpellQueue.Clear();
  }

  private void DropMagicks()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    int num = 34;
    for (int iMagick = 0; iMagick < num; ++iMagick)
    {
      if (((long) this.Player.UnlockedMagicks & (long) (1UL << iMagick)) != 0L && iMagick != 1)
      {
        BookOfMagick instance = BookOfMagick.GetInstance(this.mPlayState);
        Vector3 iVelocity = new Vector3((float) (Character.sRandom.NextDouble() - 0.5) * 3f, (float) Character.sRandom.NextDouble() * 7f, (float) (Character.sRandom.NextDouble() - 0.5) * 3f);
        instance.Initialize(this.Position, this.Body.Orientation, (MagickType) iMagick, false, iVelocity, 20f, 0);
        this.mPlayState.EntityManager.AddEntity((Entity) instance);
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          TriggerActionMessage iMessage = new TriggerActionMessage();
          iMessage.ActionType = TriggerActionType.SpawnMagick;
          iMessage.Handle = instance.Handle;
          iMessage.Template = iMagick;
          iMessage.Position = instance.Position;
          iMessage.Direction = iVelocity;
          iMessage.Time = 20f;
          iMessage.Point0 = 0;
          iMessage.Bool0 = false;
          Matrix orientation = instance.Body.Orientation;
          Quaternion.CreateFromRotationMatrix(ref orientation, out iMessage.Orientation);
          NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
        }
      }
    }
  }

  public override void Terminate(bool iKillItems, bool iIsKillPlane, bool iNetwork)
  {
    base.Terminate(iKillItems, iIsKillPlane, iNetwork);
    if (iNetwork)
      return;
    if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
    {
      (this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
      if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
      {
        this.DropMagicks();
        this.Player.UnlockedMagicks = 0UL;
        this.Player.IconRenderer.TomeMagick = MagickType.None;
      }
    }
    this.Die();
  }

  public override void Drown()
  {
    base.Drown();
    this.mPlayState.SetDiedInLevel();
    this.Die();
  }

  public override void OverKill()
  {
    base.OverKill();
    this.mPlayState.SetDiedInLevel();
    if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
    {
      (this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
      if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
      {
        this.DropMagicks();
        this.Player.UnlockedMagicks = 0UL;
        this.Player.IconRenderer.TomeMagick = MagickType.None;
      }
    }
    this.Die();
  }

  public override void Die()
  {
    base.Die();
    this.mPlayState.SetDiedInLevel();
    if (this.Player.Color == (byte) 7 && !(this.Player.Gamer is NetworkGamer))
      AchievementsManager.Instance.AwardAchievement(this.PlayState, "omgtheykilledyellow");
    for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
    {
      Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[index];
      Avatar avatar = player.Avatar;
      if (player.Playing && !avatar.Dead && avatar.UniqueID != this.UniqueID && avatar.IsInvisibile && !(avatar.Player.Gamer is NetworkGamer))
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "betteryouthanme");
    }
    this.Player.Weapon = (string) null;
    this.Player.Staff = (string) null;
    this.mPlayState.Inventory.Close((Character) this);
    if (this.mPlayState.Level.CurrentScene.RuleSet is VersusRuleset)
    {
      (this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).OnPlayerDeath(this.Player);
      if ((this.mPlayState.Level.CurrentScene.RuleSet as VersusRuleset).DropMagicks)
      {
        this.DropMagicks();
        this.Player.UnlockedMagicks = 0UL;
        this.Player.IconRenderer.TomeMagick = MagickType.None;
      }
    }
    if (!Avatar.mCache.Contains(this))
      Avatar.mCache.Add(this);
    if (!(this.Name == "wizard_alucart"))
      return;
    this.KillEverybody();
  }

  public void Slay()
  {
  }

  public void KillEverybody()
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    foreach (Magicka.GameLogic.Player player in Magicka.Game.Instance.Players)
    {
      if (player.Avatar != null && !player.Avatar.mDead)
        player.Avatar.Kill();
    }
  }

  public void SetTransform(Matrix iTransform)
  {
    this.mBody.MoveTo(iTransform.Translation + new Vector3(0.0f, this.Capsule.Length * 0.5f + this.Capsule.Radius, 0.0f), iTransform);
  }

  internal override void NetworkAction(ref CharacterActionMessage iMsg)
  {
    if (this.mPlayer == null)
    {
      this.mQueuedNetActions.Push(iMsg);
    }
    else
    {
      if (this.mQueuedNetActions.Count > 0)
      {
        CharacterActionMessage iMsg1 = this.mQueuedNetActions.Pop();
        this.NetworkAction(ref iMsg1);
      }
      switch (iMsg.Action)
      {
        case ActionType.ConjureElement:
          Elements param0I1 = (Elements) iMsg.Param0I;
          Spell oSpell;
          Spell.DefaultSpell(param0I1, out oSpell);
          if (param0I1 == Elements.Lightning & this.HasStatus(StatusEffects.Wet) | param0I1 == Elements.Fire & this.HasStatus(StatusEffects.Greased))
          {
            DamageCollection5 oDamages;
            oSpell.CalculateDamage(SpellType.Shield, CastType.Self, out oDamages);
            int num = (int) this.Damage(oDamages, (Entity) this, iMsg.TimeStamp, this.Position);
            break;
          }
          if (SpellManager.Instance.TryAddToQueue((Magicka.GameLogic.Player) null, (Character) this, this.mSpellQueue, 5, ref oSpell))
            ++this.mChantedSpells;
          AudioManager.Instance.PlayCue(Banks.UI, Defines.SOUNDS_UI_ELEMENT[Spell.ElementIndex(oSpell.Element)]);
          break;
        case ActionType.CastSpell:
          CastType param0I2 = (CastType) iMsg.Param0I;
          if (param0I2 == CastType.Magick)
          {
            SpellManager.Instance.CombineMagick(this.Player, this.mPlayState.GameType, this.mSpellQueue);
            if (this.HasStatus(StatusEffects.Frozen) || this.mCurrentState is FlyingState)
              this.CastSpell(true, "");
          }
          else
            this.mCastButtonPressed = param0I2;
          if (param0I2 != CastType.None)
          {
            this.CastType = param0I2;
            if (!this.HasStatus(StatusEffects.Frozen) && !(this.mCurrentState is FlyingState) && !(this.mCurrentState is PushedState) && !(this.mCurrentState is KnockDownState))
              break;
            this.CastSpell(true, "");
            break;
          }
          this.mChargeUnlocked = true;
          break;
        case ActionType.PickUp:
          this.InternalPickUp(Entity.GetFromHandle((int) iMsg.TargetHandle) as Pickable);
          break;
        case ActionType.PickUpRequest:
          this.PickUp(Entity.GetFromHandle((int) iMsg.TargetHandle) as Pickable);
          break;
        case ActionType.Interact:
          (Entity.GetFromHandle((int) iMsg.TargetHandle) as Character).Interact((Character) this, this.Player.Controller);
          break;
        case ActionType.Boost:
          ++this.mBoosts;
          if (this.Equipment[1].Item.PassiveAbility.Ability == Item.PassiveAbilities.ShieldBoost)
            ++this.mBoosts;
          this.mBoostCooldown = 0.25f;
          if (this.CurrentState != BoostState.Instance)
            this.mBoosts = 1;
          for (int index = 0; index < this.mAuras.Count; ++index)
          {
            AuraStorage aura = this.mAuras[index].Aura;
            if (aura.AuraType == AuraType.Boost)
              this.mBoosts *= (int) aura.AuraBoost.Magnitude;
          }
          for (int index = 0; index < this.Equipment[1].Item.Auras.Count; ++index)
          {
            AuraStorage aura = this.Equipment[1].Item.Auras[index].Aura;
            if (aura.AuraType == AuraType.Boost)
              this.mBoosts *= (int) aura.AuraBoost.Magnitude;
          }
          for (int index = 0; index < this.mBuffs.Count; ++index)
          {
            if (this.mBuffs[index].BuffType == BuffType.Boost)
              this.mBoosts *= (int) this.mBuffs[index].BuffBoost.Amount;
          }
          break;
        case ActionType.ItemSpecial:
          this.mEquipment[iMsg.Param0I].Item.ExecuteSpecialAbility();
          break;
        case ActionType.Magick:
          if (iMsg.Param3I == 39)
          {
            Polymorph.PolymorphAvatar(this, iMsg.Param0I, ref Polymorph.sTemporaryDataHolder);
            break;
          }
          base.NetworkAction(ref iMsg);
          break;
        case ActionType.EventComplete:
          if (this.mEvents == null || this.CurrentEvent >= this.mEvents.Length)
            break;
          int index1 = iMsg.Param0I << 16 /*0x10*/;
          AIEventType aiEventType = (AIEventType) (iMsg.Param0I >> 16 /*0x10*/);
          if (index1 != this.CurrentEvent || aiEventType != this.mEvents[index1].EventType)
            break;
          switch (aiEventType)
          {
            case AIEventType.Move:
              this.CurrentEventDelay = this.mEvents[index1].MoveEvent.Delay;
              break;
            case AIEventType.Animation:
              this.CurrentEventDelay = this.mEvents[index1].AnimationEvent.Delay;
              break;
            case AIEventType.Face:
              this.CurrentEventDelay = this.mEvents[index1].FaceEvent.Delay;
              break;
            case AIEventType.Kill:
              this.CurrentEventDelay = this.mEvents[index1].KillEvent.Delay;
              break;
            case AIEventType.Loop:
              this.CurrentEventDelay = this.mEvents[index1].LoopEvent.Delay;
              break;
          }
          ++this.CurrentEvent;
          break;
        default:
          base.NetworkAction(ref iMsg);
          break;
      }
    }
  }

  public Character Owner => (Character) this;

  public bool IgnoreTriggers
  {
    get => this.mIgnoreTriggers;
    set => this.mIgnoreTriggers = value;
  }

  public AIEvent[] Events
  {
    get => this.mEvents;
    set
    {
      this.mEvents = value;
      if (value == null || value.Length == 0)
      {
        this.ChangeState((BaseState) IdleState.Instance);
        this.GoToAnimation(Magicka.Animations.idle, 0.05f);
      }
      this.CurrentEvent = 0;
      this.CurrentEventDelay = 0.0f;
      DialogManager.Instance.End(this);
    }
  }

  public int CurrentEvent { get; set; }

  public bool LoopEvents => false;

  public Vector3 WayPoint { get; set; }

  public float CurrentEventDelay { get; set; }

  public List<PathNode> Path => this.mPath;

  public float CurrentStateAge { get; set; }

  public void PushState(IAIState iNewState)
  {
    this.CurrentStateAge = 0.0f;
    IAIState aiState = this.mStates.Peek();
    if (iNewState == aiState)
      return;
    aiState.OnExit((IAI) this);
    this.mStates.Push(iNewState);
    iNewState.OnEnter((IAI) this);
  }

  public void PopState()
  {
    this.CurrentStateAge = 0.0f;
    this.mStates.Pop().OnExit((IAI) this);
    this.mStates.Peek().OnEnter((IAI) this);
  }

  public void PickUp(Pickable iPickable)
  {
    if (this.Polymorphed)
      return;
    NetworkState state = NetworkManager.Instance.State;
    if (state == NetworkState.Client && !(this.Player.Gamer is NetworkGamer))
    {
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Handle = this.Handle,
        Action = ActionType.PickUpRequest,
        TargetHandle = iPickable.Handle
      }, 0);
    }
    else
    {
      if (iPickable is Item && iPickable.Removable)
        return;
      if (state == NetworkState.Server)
        NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
        {
          Handle = this.Handle,
          Action = ActionType.PickUp,
          TargetHandle = iPickable.Handle
        });
      this.InternalPickUp(iPickable);
    }
  }

  private void InternalPickUp(Pickable iPickable)
  {
    Item iBItem = iPickable as Item;
    BookOfMagick bookOfMagick = iPickable as BookOfMagick;
    if (iBItem != null)
    {
      this.mAllowAttackRotate = false;
      this.mAttacking = true;
      if (this.mPlayer != null && this.mPlayer.Controller != null && !(this.mPlayer.Controller is KeyboardMouseController))
        TutorialManager.Instance.SetTip(TutorialManager.Tips.ItemPad, TutorialManager.Position.Top);
      else
        TutorialManager.Instance.SetTip(TutorialManager.Tips.ItemKey, TutorialManager.Position.Top);
      if (iBItem.OnPickup != 0)
        this.mPlayState.Level.CurrentScene.ExecuteTrigger(iBItem.OnPickup, (Character) this, false);
      this.Player.ObtainedTextBox.Initialize(this.mPlayState.Scene, MagickaFont.Maiandra14, iBItem.PickUpString, new Vector2(), true, (Entity) this, 2f);
      if (iBItem.WeaponClass == WeaponClass.Staff)
      {
        Item iAItem = this.mEquipment[1].Item;
        if (iAItem.IsBound)
          return;
        Item.Swap(iAItem, iBItem);
        this.mNextAttackAnimation = Magicka.Animations.pickup_staff;
        if ((double) iAItem.DespawnTime > 0.0)
          iBItem.Despawn(20f);
        iAItem.Despawn(0.0f);
        Quaternion rotation;
        Vector3 translation;
        iAItem.Transform.Decompose(out Vector3 _, out rotation, out translation);
        Matrix result;
        Matrix.CreateFromQuaternion(ref rotation, out result);
        iBItem.Body.MoveTo(translation, result);
        iBItem.Body.EnableBody();
        result.Translation = translation;
        iBItem.Transform = result;
        this.Player.Staff = this.mEquipment[1].Item.Name;
        if (this.mEquipment[1].Item.HasSpecialAbility)
        {
          this.mSpecialAbilityName = LanguageManager.Instance.GetString(this.mEquipment[1].Item.SpecialAbilityName);
          this.mShowStaffSpecialAbilityNotifierTimer = 3f;
        }
      }
      else
      {
        Item iAItem = this.mEquipment[0].Item;
        if (iAItem.IsBound)
          return;
        Item.Swap(iAItem, iBItem);
        this.mNextAttackAnimation = Magicka.Animations.pickup_weapon;
        this.mHeavyWeaponReload = false;
        if ((double) iAItem.DespawnTime > 0.0)
          iBItem.Despawn(20f);
        iAItem.Despawn(0.0f);
        Quaternion rotation;
        Vector3 translation;
        iAItem.Transform.Decompose(out Vector3 _, out rotation, out translation);
        Matrix result;
        Matrix.CreateFromQuaternion(ref rotation, out result);
        iBItem.Body.MoveTo(translation, result);
        iBItem.Body.EnableBody();
        result.Translation = translation;
        iBItem.Transform = result;
        this.Player.Weapon = this.mEquipment[0].Item.Name;
      }
      DialogManager.Instance.AddTextBox(this.Player.ObtainedTextBox);
      if (iBItem.PreviousOwner is Avatar && (iBItem.PreviousOwner as Avatar).UniqueID != this.mUniqueID && !(this.Player.Gamer is NetworkGamer))
        AchievementsManager.Instance.AwardAchievement(this.PlayState, "finderskeepers");
      iBItem.PreviousOwner = (Entity) null;
      iBItem.Body.Immovable = false;
    }
    else
    {
      if (bookOfMagick == null)
        return;
      bookOfMagick.Unlock(this.Player);
      this.Player.IconRenderer.TomeMagick = bookOfMagick.Magick;
      this.Player.ObtainedTextBox.Initialize(this.mPlayState.Scene, MagickaFont.Maiandra14, bookOfMagick.PickUpString, new Vector2(), true, (Entity) this, 2f);
      if (bookOfMagick.OnPickup != 0)
        this.mPlayState.Level.CurrentScene.ExecuteTrigger(bookOfMagick.OnPickup, (Character) this, false);
      DialogManager.Instance.AddTextBox(this.Player.ObtainedTextBox);
      this.mAllowAttackRotate = false;
      this.mAttacking = true;
      this.mNextAttackAnimation = Magicka.Animations.pickup_magick;
    }
  }

  public override DamageResult InternalDamage(
    Magicka.GameLogic.Damage iDamage,
    Entity iAttacker,
    double iTimeStamp,
    Vector3 iAttackPosition,
    Defines.DamageFeatures iFeatures)
  {
    DamageResult damageResult = base.InternalDamage(iDamage, iAttacker, iTimeStamp, iAttackPosition, iFeatures);
    if (this.mPlayer != null && !(this.mPlayer.Gamer is NetworkGamer) && (damageResult & DamageResult.OverKilled) == DamageResult.None && ((damageResult & DamageResult.Damaged) == DamageResult.Damaged || (damageResult & DamageResult.Hit) == DamageResult.Hit) && this.mPlayer.Controller != null)
      this.mPlayer.Controller.Rumble(0.5f, 0.5f);
    return damageResult;
  }

  internal override float GetDanger()
  {
    return !(this.mCurrentState is ChargeState) ? base.GetDanger() : 10f;
  }

  internal virtual void GetPolymorphValues(out Polymorph.AvatarPolymorphData iData)
  {
    iData = new Polymorph.AvatarPolymorphData();
    iData.Type = this.mType;
    iData.Faction = this.Faction;
    iData.NormalizedHP = this.mHitPoints / this.mMaxHitPoints;
    iData.TimeSinceLastDamage = this.mTimeSinceLastDamage;
    iData.TimeSinceLastStatusDamage = this.mTimeSinceLastStatusDamage;
    iData.DesiredDirection = this.CharacterBody.DesiredDirection;
    iData.IsEthereal = this.IsEthereal;
    iData.FearedBy = this.mFearedBy;
    iData.FearPosition = this.mFearPosition;
    iData.FearTimer = this.mFearTimer;
    iData.CharmOwner = this.mCharmOwner;
    iData.CharmEffect = this.mCharmEffect;
    iData.CharmTimer = this.mCharmTimer;
    iData.Hypnotized = this.mHypnotized;
    iData.HypnotizeEffect = this.mHypnotizeEffect;
    iData.HypnotizeDirection = this.mHypnotizeDirection;
    Array.Copy((Array) this.mStatusEffects, (Array) Polymorph.AvatarPolymorphData.sTempStatusEffects, this.mStatusEffects.Length);
  }

  internal void ApplyPolymorphValues(ref Polymorph.AvatarPolymorphData iData)
  {
    this.mType = iData.Type;
    this.Faction = iData.Faction;
    this.mHitPoints = iData.NormalizedHP * this.mMaxHitPoints;
    this.mTimeSinceLastDamage = iData.TimeSinceLastDamage;
    this.mTimeSinceLastStatusDamage = iData.TimeSinceLastStatusDamage;
    this.CharacterBody.DesiredDirection = iData.DesiredDirection;
    if (iData.IsEthereal)
      this.Ethereal(true, 1f, 1f);
    this.mFearedBy = iData.FearedBy;
    this.mFearPosition = iData.FearPosition;
    this.mFearTimer = iData.FearTimer;
    this.mCharmOwner = iData.CharmOwner;
    this.mCharmEffect = iData.CharmEffect;
    this.mCharmTimer = iData.CharmTimer;
    this.mHypnotized = iData.Hypnotized;
    this.mHypnotizeEffect = iData.HypnotizeEffect;
    this.mHypnotizeDirection = iData.HypnotizeDirection;
    foreach (StatusEffect tempStatusEffect in Polymorph.AvatarPolymorphData.sTempStatusEffects)
    {
      int num = (int) this.AddStatusEffect(tempStatusEffect);
    }
  }

  internal override bool SendsNetworkUpdate(NetworkState iState)
  {
    return this.Player != null && !(this.Player.Gamer is NetworkGamer) || iState == NetworkState.Server;
  }

  protected override void INetworkUpdate(ref EntityUpdateMessage iMsg)
  {
    if (this.Player == null)
      return;
    base.INetworkUpdate(ref iMsg);
  }

  protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
  {
    base.IGetNetworkUpdate(out oMsg, iPrediction);
    if (this.Player.Gamer is NetworkGamer)
    {
      oMsg.Features &= ~EntityFeatures.Position;
      oMsg.Features &= ~EntityFeatures.Direction;
      oMsg.Features &= ~EntityFeatures.GenericFloat;
      oMsg.Features &= ~EntityFeatures.Etherealized;
    }
    if (NetworkManager.Instance.State == NetworkState.Server)
      return;
    oMsg.Features &= ~EntityFeatures.Damageable;
    oMsg.Features &= ~EntityFeatures.StatusEffected;
    oMsg.Features &= ~EntityFeatures.SelfShield;
  }

  public void ForceSetNotDead() => this.mDead = false;

  internal void RequestForcedSyncingOfPlayers()
  {
    if (this.mPlayer == null || NetworkManager.Instance.State == NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<RequestForcedPlayerStatusSync>(ref new RequestForcedPlayerStatusSync()
    {
      Handle = (ushort) this.mPlayer.ID
    }, 0, P2PSend.Reliable);
  }

  protected class OutlineRenderData : IProjectionObject
  {
    public BoundingSphere mBoundingSphere;
    protected int mEffect;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public Vector3 Color;
    public Matrix[] mSkeleton;
    public bool Forced;
    protected int mVerticesHash;
    protected bool mMeshDirty = true;

    public int Effect => this.mEffect;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, Texture2D iDepthMap)
    {
      SkinnedModelDeferredEffect modelDeferredEffect = iEffect as SkinnedModelDeferredEffect;
      float rimLightBias = modelDeferredEffect.RimLightBias;
      float rimLightGlow = modelDeferredEffect.RimLightGlow;
      modelDeferredEffect.RimLightBias = 0.0f;
      modelDeferredEffect.RimLightGlow = 0.0f;
      modelDeferredEffect.DiffuseMap0Enabled = false;
      modelDeferredEffect.DiffuseMap1Enabled = false;
      modelDeferredEffect.SpecularMapEnabled = false;
      modelDeferredEffect.Bloat = -0.0333f;
      modelDeferredEffect.Bones = this.mSkeleton;
      modelDeferredEffect.SpecularAmount = 0.0f;
      modelDeferredEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.None;
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.None;
      if (this.Forced)
      {
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.Always;
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBias = -0.1f;
      }
      else
      {
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.Greater;
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
        modelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
        modelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Increment;
        modelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      }
      modelDeferredEffect.CommitChanges();
      modelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
      modelDeferredEffect.DiffuseColor = this.Color;
      modelDeferredEffect.EmissiveAmount = 1f;
      modelDeferredEffect.Bloat = 0.0f;
      if (this.Forced)
      {
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0.0f;
      }
      else
      {
        modelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
        modelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
        modelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
        modelDeferredEffect.GraphicsDevice.RenderState.DepthBias = -1f / 400f;
      }
      modelDeferredEffect.CommitChanges();
      modelDeferredEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      modelDeferredEffect.GraphicsDevice.RenderState.DepthBias = 0.0f;
      modelDeferredEffect.Bloat = 0.0f;
      modelDeferredEffect.RimLightBias = rimLightBias;
      modelDeferredEffect.RimLightGlow = rimLightGlow;
      modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
      modelDeferredEffect.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      modelDeferredEffect.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
      modelDeferredEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Always;
      modelDeferredEffect.GraphicsDevice.RenderState.ReferenceStencil = 0;
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
      modelDeferredEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
      modelDeferredEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
    }

    public bool MeshDirty => this.mMeshDirty;

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
      this.mMeshDirty = false;
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
    }
  }

  protected class AfterImageRenderData : IRenderableAdditiveObject
  {
    public BoundingSphere mBoundingSphere;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    protected VertexBuffer mVertexBuffer;
    protected IndexBuffer mIndexBuffer;
    public float mIntensity;
    public Vector3 Color;
    public Matrix[][] mSkeleton;
    private SkinnedModelDeferredBasicMaterial mMaterial;
    protected int mVerticesHash;
    protected bool mMeshDirty = true;

    public AfterImageRenderData(Matrix[][] iSkeleton) => this.mSkeleton = iSkeleton;

    public bool MeshDirty => this.mMeshDirty;

    public int Effect => SkinnedModelDeferredEffect.TYPEHASH;

    public int Technique => 2;

    public VertexBuffer Vertices => this.mVertexBuffer;

    public IndexBuffer Indices => this.mIndexBuffer;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      if (iEffect.GraphicsDevice.RenderState.ColorWriteChannels == ColorWriteChannels.Alpha)
      {
        iEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
        iEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      }
      else
      {
        SkinnedModelDeferredEffect iEffect1 = iEffect as SkinnedModelDeferredEffect;
        iEffect1.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
        iEffect1.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
        this.mMaterial.AssignToEffect(iEffect1);
        iEffect1.Colorize = new Vector4(Character.ColdColor, 1f);
        float num1 = 0.333f;
        float num2 = (float) (0.33300000429153442 / ((double) this.mSkeleton.Length + 1.0));
        float num3 = num1 + this.mIntensity * num2;
        for (int index = 0; index < this.mSkeleton.Length && (double) num3 > 0.0; ++index)
        {
          if (index != 0)
          {
            iEffect1.Alpha = num3;
            iEffect1.Bones = this.mSkeleton[index];
            iEffect1.CommitChanges();
            iEffect1.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
            num3 -= num2;
          }
        }
        iEffect1.Colorize = new Vector4();
      }
    }

    public void SetMeshDirty() => this.mMeshDirty = true;

    public void SetMesh(VertexBuffer iVertices, IndexBuffer iIndices, ModelMeshPart iMeshPart)
    {
      this.mMeshDirty = false;
      Helper.SkinnedModelDeferredMaterialFromBasicEffect(iMeshPart.Effect as SkinnedModelBasicEffect, out this.mMaterial);
      this.mVertexBuffer = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndexBuffer = iIndices;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      for (int index1 = 0; index1 < this.mSkeleton.Length; ++index1)
      {
        Matrix[] matrixArray = this.mSkeleton[index1];
        for (int index2 = 0; index2 < matrixArray.Length; ++index2)
          matrixArray[index2].M11 = matrixArray[index2].M22 = matrixArray[index2].M33 = matrixArray[index2].M44 = float.NaN;
      }
    }
  }

  protected class TargettingRenderData(
    GraphicsDevice iDevice,
    Texture2D iTexture,
    Texture2D iNormalMap) : ProjectionObject(iDevice, iTexture, iNormalMap)
  {
    public float Alpha;

    public override void Draw(Effect iEffect, Texture2D iDepthMap)
    {
      ProjectionEffect projectionEffect = iEffect as ProjectionEffect;
      projectionEffect.DepthMap = iDepthMap;
      projectionEffect.PixelSize = new Vector2(1f / (float) iDepthMap.Width, 1f / (float) iDepthMap.Height);
      projectionEffect.Alpha = this.Alpha;
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
      projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      projectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
      base.Draw(iEffect, iDepthMap);
      projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 3;
      base.Draw(iEffect, iDepthMap);
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
      projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
    }
  }
}
