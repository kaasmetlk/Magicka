// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Abilities.SpecialAbilities.ArrowRain
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities;

public class ArrowRain : SpecialAbility, IAbilityEffect
{
  private static ArrowRain mSingelton;
  private static volatile object mSingeltonLock = new object();
  private readonly float RADIUS = 3.5f;
  private readonly uint NUM_OF_ARROWS;
  private readonly double INTESITY;
  private float LIFE_TIME = 4.5f;
  private float START_TIME;
  private bool mLaunched;
  private float mArrowSpawnTimer;
  private float mTTL;
  private int mLastArrowNum;
  private GameScene mScene;
  private PlayState mPlayState;
  private ISpellCaster mOwner;
  private Vector3 mPosition;
  private Elements[] mElements;
  private readonly Elements mAbilityElement;
  private MissileEntity me;

  public static ArrowRain Instance
  {
    get
    {
      if (ArrowRain.mSingelton == null)
      {
        lock (ArrowRain.mSingeltonLock)
        {
          if (ArrowRain.mSingelton == null)
            ArrowRain.mSingelton = new ArrowRain();
        }
      }
      return ArrowRain.mSingelton;
    }
  }

  public ArrowRain(Magicka.Animations iAnimation)
    : base(iAnimation, "#magick_meteors".GetHashCodeCustom())
  {
    this.mAbilityElement = Elements.None;
    this.NUM_OF_ARROWS = 10U;
    this.INTESITY = Math.Pow((double) (this.NUM_OF_ARROWS + 1U), 1.0 / (double) this.LIFE_TIME);
  }

  private ArrowRain()
    : base(Magicka.Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
  {
    this.mAbilityElement = Elements.None;
    this.NUM_OF_ARROWS = 10U;
    this.INTESITY = Math.Pow((double) (this.NUM_OF_ARROWS + 1U), 1.0 / (double) this.LIFE_TIME);
  }

  public ArrowRain(Magicka.Animations iAnimation, Elements[] iElements)
    : base(iAnimation, "#magick_meteors".GetHashCodeCustom())
  {
    this.mElements = iElements;
    foreach (Elements iElement in iElements)
    {
      if ((iElement & Elements.Lightning) != Elements.None)
      {
        this.NUM_OF_ARROWS = 7U;
        this.mAbilityElement = Elements.Lightning;
        break;
      }
      if ((iElement & Elements.Poison) != Elements.None)
      {
        this.NUM_OF_ARROWS = 15U;
        this.mAbilityElement = Elements.Poison;
        break;
      }
    }
    if (this.NUM_OF_ARROWS <= 0U)
    {
      this.NUM_OF_ARROWS = 10U;
      this.mAbilityElement = Elements.None;
    }
    this.INTESITY = Math.Pow((double) (this.NUM_OF_ARROWS + 1U), 1.0 / (double) this.LIFE_TIME);
  }

  public ArrowRain(Elements[] iElements)
    : base(Magicka.Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
  {
    this.mElements = iElements;
    foreach (Elements iElement in iElements)
    {
      if ((iElement & Elements.Lightning) != Elements.None)
      {
        this.NUM_OF_ARROWS = 7U;
        this.mAbilityElement = Elements.Lightning;
        break;
      }
      if ((iElement & Elements.Poison) != Elements.None)
      {
        this.NUM_OF_ARROWS = 15U;
        this.mAbilityElement = Elements.Poison;
        break;
      }
    }
    if (this.NUM_OF_ARROWS <= 0U)
    {
      this.NUM_OF_ARROWS = 10U;
      this.mAbilityElement = Elements.None;
    }
    this.INTESITY = Math.Pow((double) (this.NUM_OF_ARROWS + 1U), 1.0 / (double) this.LIFE_TIME);
  }

  public override bool Execute(Vector3 iPosition, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    if ((double) this.mTTL > 0.0)
    {
      this.mLaunched = false;
      this.START_TIME = 1.9f;
      this.mTTL = this.LIFE_TIME + this.START_TIME;
      return true;
    }
    this.mOwner = (ISpellCaster) null;
    this.mPlayState = iPlayState;
    return this.Execute();
  }

  public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return false;
    base.Execute(iOwner, iPlayState);
    if ((double) this.mTTL > 0.0)
    {
      this.mLaunched = false;
      this.START_TIME = 1.9f;
      this.mTTL = this.LIFE_TIME + this.START_TIME;
      return true;
    }
    this.mOwner = iOwner;
    this.mPlayState = iPlayState;
    return this.Execute();
  }

  private bool Execute()
  {
    this.mScene = this.mPlayState.Level.CurrentScene;
    this.mLaunched = false;
    this.START_TIME = 1.9f;
    this.mTTL = this.LIFE_TIME + this.START_TIME;
    this.mArrowSpawnTimer = 0.0f;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
    if (this.mOwner is Avatar)
    {
      Vector3 result = this.mOwner.Direction;
      Vector3.Multiply(ref result, 15f, out result);
      this.mPosition = this.mOwner.Position + result;
    }
    else if (this.mOwner is NonPlayerCharacter)
    {
      this.mPosition = (this.mOwner as NonPlayerCharacter).AI.LastTarget.Position;
    }
    else
    {
      this.mTTL = 0.0f;
      return false;
    }
    return true;
  }

  public void Launch()
  {
    this.mLaunched = true;
    ConditionCollection iConditions;
    lock (ProjectileSpell.sCachedConditions)
      iConditions = ProjectileSpell.sCachedConditions.Dequeue();
    iConditions.Clear();
    iConditions[0].Condition.EventConditionType = EventConditionType.Timer;
    iConditions[0].Condition.Time = 2.5f;
    iConditions[0].Add(new EventStorage(new RemoveEvent()));
    iConditions[1].Condition.EventConditionType = EventConditionType.Default;
    iConditions[1].Condition.Repeat = true;
    lock (ProjectileSpell.sCachedConditions)
      ProjectileSpell.sCachedConditions.Enqueue(iConditions);
    Spell oSpell = new Spell();
    if (this.mElements != null)
      Spell.DefaultSpell(this.mElements, out oSpell);
    this.me = this.mOwner == null ? MissileEntity.GetInstance(this.mPlayState) : this.mOwner.GetMissileInstance();
    Model model1 = new Model();
    Model model2 = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/goblin_arrow_0");
    Vector3 translation = (this.mOwner as Magicka.GameLogic.Entities.Character).GetLeftAttachOrientation().Translation;
    Vector3 iVelocity = new Vector3(0.01f, 50f, -0.01f);
    this.me.Initialize(this.mOwner as Entity, (Entity) null, 0.0f, model2.Meshes[0].BoundingSphere.Radius * 0.75f, ref translation, ref iVelocity, model2, iConditions, false);
    ProjectileSpell.SpawnMissile(ref this.me, model2, this.mOwner, 0.0f, ref translation, ref iVelocity, ref oSpell, 4f, 1);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
    {
      Type = SpawnMissileMessage.MissileType.Spell,
      Handle = this.me.Handle,
      Item = (ushort) 0,
      Owner = this.mOwner.Handle,
      Position = translation,
      Velocity = iVelocity,
      Spell = oSpell,
      Homing = 0.0f,
      Splash = 4f
    });
  }

  public bool IsDead => (double) this.mTTL <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mTTL -= iDeltaTime;
    if ((double) this.mTTL <= 0.0)
      this.me.Deinitialize();
    if ((double) this.mTTL < (double) this.LIFE_TIME && !this.mLaunched)
      this.Launch();
    if (!this.mLaunched)
      return;
    this.mArrowSpawnTimer += iDeltaTime;
    int num1 = (int) (Math.Pow(this.INTESITY, (double) this.mArrowSpawnTimer) - 1.0);
    if (num1 == 0 || num1 == this.mLastArrowNum)
      return;
    this.mLastArrowNum = num1;
    Vector3 mPosition = this.mPosition;
    float num2 = (float) Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
    float num3 = (float) (SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
    float num4 = num2 * (float) Math.Cos((double) num3);
    float num5 = num2 * (float) Math.Sin((double) num3);
    mPosition.X += this.RADIUS * num4;
    mPosition.Z += this.RADIUS * num5;
    mPosition.Y += 146f;
    Vector3 iPosition = mPosition with { Y = 0.0f };
    Vector3 iVelocity = new Vector3(0.01f, -20f, -0.01f);
    Spell oSpell = new Spell();
    if (this.mElements != null)
      Spell.DefaultSpell(this.mElements, out oSpell);
    if ((this.mAbilityElement & Elements.Lightning) != Elements.None)
    {
      LightningBolt lightning = LightningBolt.GetLightning();
      lightning.AirToSurface = true;
      HitList iHitList = new HitList(16 /*0x10*/);
      DamageCollection5 oDamages;
      oSpell.CalculateDamage(SpellType.Lightning, CastType.Force, out oDamages);
      Flash.Instance.Execute(this.mPlayState.Scene, 0.075f);
      this.mPlayState.Camera.CameraShake(iPosition, 0.5f, 0.075f);
      lightning.Cast(this.mOwner, mPosition, iPosition - mPosition, iHitList, Spell.LIGHTNINGCOLOR, 10f, ref oDamages, new Spell?(oSpell), this.mPlayState);
      if (NetworkManager.Instance.State != NetworkState.Server)
        return;
      TriggerActionMessage iMessage = new TriggerActionMessage();
      iMessage.ActionType = TriggerActionType.LightningBolt;
      if (this.mOwner != null)
        iMessage.Handle = this.mOwner.Handle;
      iMessage.Position = mPosition;
      iMessage.Direction = iPosition;
      iMessage.Spell = oSpell;
      NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref iMessage);
    }
    else
    {
      MissileEntity iMissile = this.mOwner == null ? MissileEntity.GetInstance(this.mPlayState) : this.mOwner.GetMissileInstance();
      Model model = new Model();
      Model mdl = Magicka.Game.Instance.Content.Load<Model>("Models/Missiles/goblin_arrow_0");
      ProjectileSpell.SpawnMissile(ref iMissile, mdl, this.mOwner, 0.0f, ref mPosition, ref iVelocity, ref oSpell, 4f, 1);
      if (NetworkManager.Instance.State != NetworkState.Server)
        return;
      NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref new SpawnMissileMessage()
      {
        Type = SpawnMissileMessage.MissileType.Spell,
        Handle = iMissile.Handle,
        Item = (ushort) 0,
        Owner = this.mOwner.Handle,
        Position = mPosition,
        Velocity = iVelocity,
        Spell = oSpell,
        Homing = 0.0f,
        Splash = 4f
      });
    }
  }

  public void OnRemove()
  {
    this.mTTL = 0.0f;
    this.mScene.LightTargetIntensity = 1f;
  }
}
