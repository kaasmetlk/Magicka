// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ResumeAnimation
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class ResumeAnimation(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mAnimationName;
  private int[] mAnimationID;
  private float mSpeed = 1f;
  private float mLength;
  private bool mChildren = true;
  private bool? mLoop;

  protected override void Execute()
  {
    AnimatedLevelPart animatedLevelPart = this.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
    for (int index = 1; index < this.mAnimationID.Length; ++index)
      animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[index]);
    animatedLevelPart.Resume(this.mChildren, this.mLength, this.mSpeed, this.mLoop);
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

  public float Speed
  {
    get => this.mSpeed;
    set => this.mSpeed = value;
  }

  public float Length
  {
    get => this.mLength;
    set => this.mLength = value;
  }

  public bool Children
  {
    get => this.mChildren;
    set => this.mChildren = value;
  }

  public bool Loop
  {
    get => this.mLoop ?? false;
    set => this.mLoop = new bool?(value);
  }
}
