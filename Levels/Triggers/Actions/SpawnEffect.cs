// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.SpawnEffect
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using PolygonHead.ParticleEffects;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class SpawnEffect(Trigger iTrigger, GameScene iGameScene) : Action(iTrigger, iGameScene)
{
  private string mEffect;
  private int mEffectID;
  private string mName;
  private int mNameID;
  private string mArea;
  private int mAreaID;
  private float mRange;

  protected override void Execute() => this.Spawn();

  public override void QuickExecute()
  {
    if (EffectManager.Instance.GetEffect(this.mEffectID).Type == VisualEffectType.Single)
      return;
    this.Spawn();
  }

  protected void Spawn()
  {
    LevelModel.VisualEffectStorage iEffect;
    iEffect.ID = this.mNameID;
    iEffect.Effect = this.mEffectID;
    this.GameScene.GetLocator(this.mAreaID, out iEffect.Transform);
    iEffect.Range = this.mRange;
    this.GameScene.AddEffect(this.mNameID, ref iEffect);
  }

  public string Effect
  {
    get => this.mEffect;
    set
    {
      this.mEffect = value;
      this.mEffectID = this.mEffect.GetHashCodeCustom();
    }
  }

  public string Name
  {
    get => this.mName;
    set
    {
      this.mName = value;
      this.mNameID = this.mName.GetHashCodeCustom();
    }
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

  public float Range
  {
    get => this.mRange;
    set => this.mRange = value;
  }
}
