// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.PhysAnimation
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class PhysAnimation(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mPhysName;
  private int mPhysNameID;
  private string mAnimationNames;
  private int[] mAnimationID;
  private float mSpeed = 1f;
  private float mStart;
  private float mEnd;
  private bool mChildren = true;

  protected override void Execute()
  {
    DamageablePhysicsEntity byId = Entity.GetByID(this.mPhysNameID) as DamageablePhysicsEntity;
    AnimatedLevelPart animatedLevelPart = this.GameScene.LevelModel.GetAnimatedLevelPart(this.mAnimationID[0]);
    for (int index = 1; index < this.mAnimationID.Length; ++index)
      animatedLevelPart = animatedLevelPart.GetChild(this.mAnimationID[index]);
    PhysAnimationControl physAnimCont;
    physAnimCont.AnimatedLevelPart = animatedLevelPart;
    physAnimCont.End = this.mEnd;
    physAnimCont.Start = this.mStart;
    physAnimCont.Speed = this.mSpeed;
    physAnimCont.Children = this.mChildren;
    byId.AddAnimation(physAnimCont);
  }

  public override void QuickExecute() => this.Execute();

  public string Id
  {
    get => this.mPhysName;
    set
    {
      this.mPhysName = value;
      this.mPhysNameID = this.mPhysName.GetHashCodeCustom();
    }
  }

  public string Name
  {
    get => this.mAnimationNames;
    set
    {
      this.mAnimationNames = value;
      string[] strArray = this.mAnimationNames.Split('/');
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

  public bool Children
  {
    get => this.mChildren;
    set => this.mChildren = value;
  }
}
