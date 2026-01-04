// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnPhys
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnPhys(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private SpawnPhys.HandleAndHealth mSpawnedEntityHP;
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

  public override void Initialize()
  {
    base.Initialize();
    this.mTemplate = this.GameScene.PlayState.Content.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + this.mObject);
    if (this.mTemplate.MaxHitpoints > 0)
    {
      this.mEntity = (PhysicsEntity) new DamageablePhysicsEntity(this.GameScene.PlayState);
      this.mSpawnedEntityHP = new SpawnPhys.HandleAndHealth((int) this.mEntity.Handle, (float) this.mTemplate.MaxHitpoints);
    }
    else
      this.mEntity = new PhysicsEntity(this.GameScene.PlayState);
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
    this.GameScene.PlayState.EntityManager.AddEntity((Entity) this.mEntity);
  }

  public override void QuickExecute()
  {
  }

  public override void Update(float iDeltaTime)
  {
    if (this.mEntity is DamageablePhysicsEntity mEntity)
      this.mSpawnedEntityHP.Health = mEntity.HitPoints;
    base.Update(iDeltaTime);
  }

  protected override object Tag
  {
    get => (object) this.mSpawnedEntityHP;
    set => this.mSpawnedEntityHP = (SpawnPhys.HandleAndHealth) value;
  }

  protected override void WriteTag(BinaryWriter iWriter, object mTag)
  {
    iWriter.Write(((SpawnPhys.HandleAndHealth) mTag).Health);
  }

  protected override object ReadTag(BinaryReader iReader)
  {
    return (object) new SpawnPhys.HandleAndHealth((int) ushort.MaxValue, iReader.ReadSingle());
  }

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

  private struct HandleAndHealth(int iHandle, float iHealth)
  {
    public int Handle = iHandle;
    public float Health = iHealth;
  }
}
