// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.PlayAnimation
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class PlayAnimation(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mAnimationName;
  private int[] mAnimationID;
  private bool mResume;
  private float mSpeed = 1f;
  private float mStart = -1f;
  private float mEnd = -1f;
  private bool mChildren = true;
  private bool mLoop;

  protected override void Execute()
  {
    AnimatedLevelPart animatedLevelPart = this.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
    for (int index = 1; index < this.mAnimationID.Length; ++index)
      animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[index]);
    animatedLevelPart.Play(this.mChildren, this.mStart, this.mEnd, this.mSpeed, this.mLoop, this.mResume);
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

  public float Start
  {
    get => this.mStart;
    set => this.mStart = value;
  }

  public float End
  {
    get => this.mEnd;
    set => this.mEnd = value;
  }

  public bool Loop
  {
    get => this.mLoop;
    set => this.mLoop = value;
  }

  public bool Children
  {
    get => this.mChildren;
    set => this.mChildren = value;
  }

  public bool Resume
  {
    get => this.mResume;
    set => this.mResume = value;
  }
}
