// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.EnableLight
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead.Lights;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class EnableLight(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string[] mLights;
  private int[] mLightIDs;
  private LightTransitionType mEffect;
  private float mTime;

  protected override void Execute()
  {
    for (int index = 0; index < this.mLightIDs.Length; ++index)
      this.GameScene.LevelModel.Lights[this.mLightIDs[index]].Enable(this.GameScene.Scene, this.mEffect, this.mTime);
  }

  public override void QuickExecute()
  {
    for (int index = 0; index < this.mLightIDs.Length; ++index)
      this.GameScene.LevelModel.Lights[this.mLightIDs[index]].Enable(this.GameScene.Scene, LightTransitionType.None, 0.0f);
  }

  public string Id
  {
    get => string.Join(",", this.mLights);
    set
    {
      this.mLights = value.ToLowerInvariant().Split(',');
      this.mLightIDs = new int[this.mLights.Length];
      for (int index = 0; index < this.mLights.Length; ++index)
        this.mLightIDs[index] = this.mLights[index].GetHashCodeCustom();
    }
  }

  public LightTransitionType Effect
  {
    get => this.mEffect;
    set => this.mEffect = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
