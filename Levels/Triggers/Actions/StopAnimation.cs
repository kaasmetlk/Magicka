// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.StopAnimation
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class StopAnimation(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mAnimationName;
  private bool mStopAll = true;
  private int[] mAnimationID;

  protected override void Execute()
  {
    AnimatedLevelPart animatedLevelPart = this.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
    for (int index = 1; index < this.mAnimationID.Length; ++index)
      animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[index]);
    animatedLevelPart.Stop(this.mStopAll);
  }

  public override void QuickExecute()
  {
  }

  public string Name
  {
    get => this.mAnimationName;
    set
    {
      this.mAnimationName = value;
      string[] strArray = this.mAnimationName.Split('/');
      this.mAnimationID = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        this.mAnimationID[index] = strArray[index].GetHashCodeCustom();
    }
  }

  public bool Children
  {
    get => this.mStopAll;
    set => this.mStopAll = value;
  }
}
