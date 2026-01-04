// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.HighlightAnim
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class HighlightAnim(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mID;
  private int[] mIDHash;
  private float mTime = 1f;

  protected override void Execute()
  {
    AnimatedLevelPart animatedLevelPart = this.GameScene.LevelModel.GetAnimatedLevelPart(this.mIDHash[0]);
    for (int index = 1; index < this.mIDHash.Length; ++index)
      animatedLevelPart = animatedLevelPart.GetChild(this.mIDHash[index]);
    animatedLevelPart.Highlight(this.mTime);
  }

  public override void QuickExecute()
  {
  }

  public string Name
  {
    get => this.mID;
    set
    {
      this.mID = value;
      string[] strArray = this.mID.Split('/');
      this.mIDHash = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
        this.mIDHash[index] = strArray[index].GetHashCodeCustom();
    }
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }
}
