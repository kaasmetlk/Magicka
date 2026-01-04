// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnBoss
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using System;
using System.Reflection;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnBoss(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mBoss;
  private bool mGeneric;
  private bool mProp;
  private string mOnDeath;
  private int mOnDeathId;
  private string mOnDamage;
  private int mOnDamageId;
  private string mCharacterTemplate;
  private string mArea;
  private int mAreaHash;
  private float mHealthAppearDelay;
  private float mFreezeTime;
  private float mHealthBarWidth = 0.8f;
  private int mUniqueID;
  private string mUniqueName;
  private IBoss mBossRef;
  private BossFight mBossFight;
  private bool mDelayed;
  private int mMeshIdx = -1;

  public override void Initialize()
  {
    base.Initialize();
    if (this.mProp)
    {
      this.mBossRef = (IBoss) new PropBoss(this.GameScene.PlayState, this.mBoss, this.mUniqueID, this.mOnDeathId, this.mOnDamageId);
      this.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mBoss);
    }
    else if (this.mGeneric)
    {
      this.mBossRef = (IBoss) new GenericBoss(this.GameScene.PlayState, this.mBoss.GetHashCodeCustom(), this.mUniqueID, this.mMeshIdx);
      this.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mBoss);
    }
    else
      this.mBossRef = SpawnBoss.GetBoss(this.mBoss, this.GameScene.PlayState, this.mUniqueID);
    this.mBossFight = BossFight.Instance;
  }

  protected override void Execute()
  {
    if (!this.mBossFight.IsSetup)
      BossFight.Instance.Setup(this.GameScene.PlayState, this.mFreezeTime, this.mHealthAppearDelay, this.mHealthBarWidth);
    this.mBossFight.Initialize(this.mBossRef, this.mAreaHash, this.mUniqueID);
    if (this.mDelayed)
      return;
    this.mBossFight.Start();
  }

  public override void QuickExecute() => this.Execute();

  protected static Type GetBossType(string name)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
      {
        foreach (Type type in types[index].GetInterfaces())
        {
          if (type == typeof (IBoss))
            return types[index];
        }
      }
    }
    return (Type) null;
  }

  public static IBoss GetBoss(string iClassName, PlayState iPlayState, int iUniqueID)
  {
    return SpawnBoss.GetBossType(iClassName).GetConstructor(new Type[1]
    {
      typeof (PlayState)
    }).Invoke(new object[1]{ (object) iPlayState }) as IBoss;
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

  public string Boss
  {
    get => this.mBoss;
    set => this.mBoss = value;
  }

  public bool Prop
  {
    get => this.mProp;
    set => this.mProp = value;
  }

  public bool Generic
  {
    get => this.mGeneric;
    set => this.mGeneric = value;
  }

  public string Template
  {
    get => this.mCharacterTemplate;
    set => this.mCharacterTemplate = value;
  }

  public bool Delayed
  {
    get => this.mDelayed;
    set => this.mDelayed = value;
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

  public float HealthAppearDelay
  {
    get => this.mHealthAppearDelay;
    set => this.mHealthAppearDelay = value;
  }

  public float FreezeTime
  {
    get => this.mFreezeTime;
    set => this.mFreezeTime = value;
  }

  public float HealthBarWidth
  {
    get => this.mHealthBarWidth;
    set => this.mHealthBarWidth = value;
  }

  public int MeshId
  {
    get => this.mMeshIdx;
    set => this.mMeshIdx = value;
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
}
