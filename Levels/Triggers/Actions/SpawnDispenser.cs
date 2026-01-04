// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnDispenser
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class SpawnDispenser(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mArea;
  private int mAreaID;
  private Dispensers mModel;
  private string[] mTypeID;
  private int[] mTypeIDHash;
  private int[] mAmount;
  private bool mActive = true;
  private float mTime;
  private string mID;
  private int mIDHash;

  public override void Initialize()
  {
    base.Initialize();
    for (int index = 0; index < this.mTypeID.Length; ++index)
      this.GameScene.PlayState.Content.Load<CharacterTemplate>("Data/Characters/" + this.mTypeID[index]);
  }

  protected override void Execute()
  {
    Dispenser fromCache = Dispenser.GetFromCache(this.GameScene.PlayState);
    Matrix oLocator;
    this.GameScene.GetLocator(this.mAreaID, out oLocator);
    fromCache.Initialize(oLocator, this.mModel, this.mTypeIDHash, this.mAmount, this.mTime, this.mActive);
    this.GameScene.PlayState.EntityManager.AddEntity((Entity) fromCache);
  }

  public override void QuickExecute() => this.Execute();

  public float GetTotalHitPoins()
  {
    float totalHitPoins = 0.0f;
    for (int index1 = 0; index1 < this.mTypeIDHash.Length; ++index1)
    {
      int index2 = this.mAmount.Length;
      if (index2 >= index1)
        index2 = 0;
      CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.mTypeIDHash[index1]);
      totalHitPoins += cachedTemplate.MaxHitpoints * (float) this.mAmount[index2];
    }
    return totalHitPoins;
  }

  public string Area
  {
    get => this.mArea;
    set
    {
      this.mArea = value;
      this.mAreaID = this.mArea.GetHashCodeCustom();
    }
  }

  public Dispensers Model
  {
    get => this.mModel;
    set => this.mModel = value;
  }

  public string Types
  {
    set
    {
      this.mTypeID = value.Split(',');
      this.mTypeIDHash = new int[this.mTypeID.Length];
      for (int index = 0; index < this.mTypeIDHash.Length; ++index)
        this.mTypeIDHash[index] = this.mTypeID[index].GetHashCodeCustom();
    }
  }

  public string Amount
  {
    set
    {
      string[] strArray = value.Split(',');
      this.mAmount = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        this.mAmount[index] = int.Parse(strArray[index]);
    }
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }

  public string ID
  {
    get => this.mID;
    set
    {
      this.mID = value;
      this.mIDHash = this.mID.GetHashCodeCustom();
    }
  }

  public bool Active
  {
    get => this.mActive;
    set => this.mActive = value;
  }
}
