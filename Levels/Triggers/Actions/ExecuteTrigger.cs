// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.ExecuteTrigger
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public class ExecuteTrigger(Magicka.Levels.Triggers.Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private string mTriggerName;
  private int mTriggerId;
  private Queue<Character> mArgs = new Queue<Character>(10);

  public override void OnTrigger(Character iArg)
  {
    base.OnTrigger(iArg);
    this.mArgs.Enqueue(iArg);
  }

  protected override void Execute()
  {
    this.GameScene.ExecuteTrigger(this.mTriggerId, this.mArgs.Dequeue(), false);
  }

  public override void QuickExecute()
  {
  }

  public string Trigger
  {
    get => this.mTriggerName;
    set
    {
      this.mTriggerName = value;
      this.mTriggerId = this.mTriggerName.GetHashCodeCustom();
    }
  }

  protected override object Tag
  {
    get => (object) new Queue<Character>((IEnumerable<Character>) this.mArgs);
    set => this.mArgs = new Queue<Character>((IEnumerable<Character>) (value as Queue<Character>));
  }

  protected override void WriteTag(BinaryWriter iWriter, object iTag)
  {
    Queue<Character> characterQueue = iTag as Queue<Character>;
    iWriter.Write(characterQueue.Count);
    foreach (Character character in characterQueue)
    {
      if (character != null)
        iWriter.Write(character.UniqueID);
      else
        iWriter.Write(0);
    }
  }

  protected override object ReadTag(BinaryReader iReader)
  {
    int capacity = iReader.ReadInt32();
    Queue<Character> characterQueue = new Queue<Character>(capacity);
    for (int index = 0; index < capacity; ++index)
    {
      int iID = iReader.ReadInt32();
      characterQueue.Enqueue(Entity.GetByID(iID) as Character);
    }
    return (object) characterQueue;
  }
}
