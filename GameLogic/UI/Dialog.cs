// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Dialog
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class Dialog
{
  private int mRepeatFrom;
  private int mNext;
  private int mInteracted;
  private Interact[] mMessages;
  private int mID;
  private Scene mScene;

  public Dialog(XmlNode iInput)
  {
    InteractType iDefaultIconText = InteractType.Talk;
    List<Interact> interactList = new List<Interact>();
    for (int i = 0; i < iInput.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iInput.Attributes[i];
      if (attribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
        this.mID = attribute.Value.ToLowerInvariant().GetHashCodeCustom();
      else if (attribute.Name.Equals("iconText", StringComparison.OrdinalIgnoreCase))
        iDefaultIconText = (InteractType) Enum.Parse(typeof (InteractType), attribute.Value, true);
      else
        this.mRepeatFrom = attribute.Name.Equals("repeatFrom", StringComparison.OrdinalIgnoreCase) ? int.Parse(attribute.Value) : throw new NotImplementedException();
    }
    for (int i = 0; i < iInput.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iInput.ChildNodes[i];
      if (!(childNode is XmlComment))
      {
        if (!childNode.Name.Equals("Interact", StringComparison.OrdinalIgnoreCase))
          throw new NotImplementedException();
        interactList.Add(new Interact(childNode, iDefaultIconText, this));
      }
    }
    this.mMessages = interactList.ToArray();
  }

  public void Initialize(Scene iScene)
  {
    this.mScene = iScene;
    for (int index = 0; index < this.mMessages.Length; ++index)
      this.mMessages[index].Initialize();
  }

  public InteractType IconText
  {
    get
    {
      return this.mNext < this.mMessages.Length ? this.mMessages[this.mNext].IconText : InteractType.Talk;
    }
  }

  public int ID => this.mID;

  public Scene Scene => this.mScene;

  public Interact Pop()
  {
    if (this.mNext >= this.mMessages.Length)
      return (Interact) null;
    Interact mMessage = this.mMessages[this.mNext];
    if (!mMessage.RepeatInfinitly && mMessage.Repetitions <= 1)
    {
      ++this.mNext;
      this.mInteracted = Math.Min(this.mInteracted + 1, this.mMessages.Length);
    }
    if (this.mNext >= this.mMessages.Length && this.mRepeatFrom >= 0)
    {
      this.mNext = this.mRepeatFrom;
      for (int index = 0; index < this.mMessages.Length; ++index)
        this.mMessages[index].Reset();
    }
    return mMessage;
  }

  public Interact Peek()
  {
    return this.mNext < this.mMessages.Length ? this.mMessages[this.mNext] : (Interact) null;
  }

  internal void Reset() => this.mNext = 0;

  public int Next => this.mNext;

  public int InteractedMessages => this.mInteracted;

  public class State
  {
    private int mNext;
    private int mFinished;
    private Interact.State[] mMessages;
    private Dialog mOwner;

    public State(Dialog iOwner)
    {
      this.mOwner = iOwner;
      this.mMessages = new Interact.State[this.mOwner.mMessages.Length];
      this.UpdateState();
    }

    public void UpdateState()
    {
      this.mNext = this.mOwner.mNext;
      this.mFinished = this.mOwner.mInteracted;
      for (int index = 0; index < this.mOwner.mMessages.Length; ++index)
        this.mMessages[index] = new Interact.State(this.mOwner.mMessages[index]);
    }

    public void ApplayState()
    {
      this.mOwner.mNext = this.mNext;
      this.mOwner.mInteracted = this.mFinished;
      for (int index = 0; index < this.mMessages.Length; ++index)
        this.mMessages[index].ApplyState();
    }

    internal Dialog Owner => this.mOwner;

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mNext);
      iWriter.Write(this.mFinished);
      for (int index = 0; index < this.mMessages.Length; ++index)
        this.mMessages[index].Write(iWriter);
    }

    internal void Read(BinaryReader iReader)
    {
      this.mNext = iReader.ReadInt32();
      this.mFinished = iReader.ReadInt32();
      for (int index = 0; index < this.mMessages.Length; ++index)
        this.mMessages[index].Read(iReader);
    }
  }
}
