// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnAnimProp
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System;
using System.IO;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public sealed class SpawnAnimProp(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private SpawnAnimProp.HandleAndHealth mSpawnedEntityHP;
  private string mArea;
  private int mAreaId;
  private string mObject;
  private string mOnDeath;
  private int mOnDeathId;
  private string mOnDamage;
  private int mOnDamageId;
  private string mUniqueName;
  private int mUniqueID;
  private PhysicsEntity mEntity;
  private PhysicsEntityTemplate mTemplate;
  private string mAnimationStr = "";
  private Animations mAnimation;
  private bool mIsDamageable;
  private bool mIsAffectedByGravity = true;

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaId = this.mArea.GetHashCodeCustom();
    }
  }

  public string Object
  {
    get => this.mObject;
    set => this.mObject = value;
  }

  public string OnDeath
  {
    get => this.mOnDeath;
    set
    {
      this.mOnDeath = value;
      this.mOnDeathId = this.mOnDeath.GetHashCodeCustom();
    }
  }

  public string OnDamage
  {
    get => this.mOnDamage;
    set
    {
      this.mOnDamage = value;
      this.mOnDamageId = this.mOnDamage.GetHashCodeCustom();
    }
  }

  public string ID
  {
    get => this.mUniqueName;
    set
    {
      this.mUniqueName = value;
      this.mUniqueID = this.mUniqueName.GetHashCodeCustom();
    }
  }

  public string Animation
  {
    get => this.mAnimationStr;
    set
    {
      this.mAnimationStr = value;
      if (!string.IsNullOrEmpty(this.mAnimationStr))
        this.mAnimationStr = this.mAnimationStr.Trim().ToLower();
      if (!string.IsNullOrEmpty(this.mAnimationStr))
      {
        try
        {
          this.mAnimation = (Animations) Enum.Parse(typeof (Animations), this.mAnimationStr);
        }
        catch (ArgumentException ex)
        {
          this.mAnimation = Animations.None;
        }
      }
      else
        this.mAnimation = Animations.None;
    }
  }

  public string Damageable
  {
    set
    {
      string str1 = value;
      if (string.IsNullOrEmpty(str1))
      {
        this.mIsDamageable = false;
      }
      else
      {
        string str2 = str1.Trim();
        if (string.IsNullOrEmpty(str2))
        {
          this.mIsDamageable = false;
        }
        else
        {
          string lower = str2.ToLower();
          if (string.Compare(lower, "true") == 0)
            this.mIsDamageable = true;
          else if (string.Compare(lower, "false") == 0)
            this.mIsDamageable = false;
          else
            this.mIsDamageable = false;
        }
      }
    }
  }

  public string Gravity
  {
    set
    {
      string str1 = value;
      if (string.IsNullOrEmpty(str1))
      {
        this.mIsDamageable = false;
      }
      else
      {
        string str2 = str1.Trim();
        if (string.IsNullOrEmpty(str2))
        {
          this.mIsAffectedByGravity = false;
        }
        else
        {
          string lower = str2.ToLower();
          if (string.Compare(lower, "true") == 0)
            this.mIsAffectedByGravity = true;
          else if (string.Compare(lower, "false") == 0)
            this.mIsAffectedByGravity = false;
          else
            this.mIsAffectedByGravity = false;
        }
      }
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this.mTemplate = this.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/AnimatedProps/" + this.mObject);
    if (this.mTemplate.AnimationClips == null || this.mTemplate.AnimationClips.Length == 0 || this.mTemplate.Skeleton == null)
    {
      this.mEntity = this.mTemplate.MaxHitpoints <= 0 ? new PhysicsEntity(this.GameScene.PlayState) : (PhysicsEntity) new DamageablePhysicsEntity(this.GameScene.PlayState);
    }
    else
    {
      this.mEntity = (PhysicsEntity) new AnimatedPhysicsEntity(this.GameScene.PlayState);
      (this.mEntity as AnimatedPhysicsEntity).IsDamageable = this.mIsDamageable;
      (this.mEntity as AnimatedPhysicsEntity).IsAffectedByGravity = this.mIsAffectedByGravity;
    }
    if (this.mTemplate.MaxHitpoints <= 0 || !this.mIsDamageable)
      return;
    this.mSpawnedEntityHP = new SpawnAnimProp.HandleAndHealth((int) this.mEntity.Handle, (float) this.mTemplate.MaxHitpoints);
  }

  protected override void Execute()
  {
    Matrix oLocator;
    this.GameScene.GetLocator(this.mAreaId, out oLocator);
    this.mEntity.Initialize(this.mTemplate, oLocator, this.mUniqueID);
    if (this.mEntity is DamageablePhysicsEntity mEntity)
    {
      mEntity.OnDeath = this.mOnDeathId;
      mEntity.OnDamage = this.mOnDamageId;
    }
    if (this.mEntity is AnimatedPhysicsEntity)
      (this.mEntity as AnimatedPhysicsEntity).ForceAnimation(this.mAnimation);
    this.GameScene.PlayState.EntityManager.AddEntity((Entity) this.mEntity);
  }

  public override void QuickExecute()
  {
  }

  public override void Update(float iDeltaTime)
  {
    if (this.mEntity is AnimatedPhysicsEntity mEntity)
      this.mSpawnedEntityHP.Health = mEntity.HitPoints;
    base.Update(iDeltaTime);
  }

  protected override object Tag
  {
    get => (object) this.mSpawnedEntityHP;
    set => this.mSpawnedEntityHP = (SpawnAnimProp.HandleAndHealth) value;
  }

  protected override void WriteTag(BinaryWriter iWriter, object mTag)
  {
    iWriter.Write(((SpawnAnimProp.HandleAndHealth) mTag).Health);
  }

  protected override object ReadTag(BinaryReader iReader)
  {
    return (object) new SpawnAnimProp.HandleAndHealth((int) ushort.MaxValue, iReader.ReadSingle());
  }

  private struct HandleAndHealth(int iHandle, float iHealth)
  {
    public int Handle = iHandle;
    public float Health = iHealth;
  }
}
