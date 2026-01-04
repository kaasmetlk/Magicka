// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Interact
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class Interact
{
  private bool mLock;
  private bool mFocusOwner = true;
  private InteractType mIconText;
  private Message[] mMessages;
  private int mCurrent;
  private int mRepeatCounter;
  private int mRepeat = 1;
  private Entity mLastOwner;
  private Vector3 mWorldPosition;
  private bool mScreenPos;
  private Controller mInteractor;
  private Dialog mParent;

  public Interact(XmlNode iInput, InteractType iDefaultIconText, Dialog iParent)
  {
    this.mParent = iParent;
    this.mIconText = iDefaultIconText;
    List<Message> messageList = new List<Message>();
    for (int i = 0; i < iInput.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iInput.Attributes[i];
      if (attribute.Name.Equals("lock", StringComparison.OrdinalIgnoreCase))
        this.mLock = bool.Parse(attribute.Value);
      else if (attribute.Name.Equals("focusOwner", StringComparison.OrdinalIgnoreCase))
        this.mFocusOwner = bool.Parse(attribute.Value);
      else
        this.mRepeat = attribute.Name.Equals("count", StringComparison.OrdinalIgnoreCase) ? int.Parse(attribute.Value) : throw new NotImplementedException();
    }
    for (int i = 0; i < iInput.ChildNodes.Count; ++i)
    {
      XmlNode childNode = iInput.ChildNodes[i];
      if (!(childNode is XmlComment))
      {
        if (!childNode.Name.Equals("message", StringComparison.OrdinalIgnoreCase))
          throw new NotImplementedException();
        messageList.Add(new Message(childNode));
      }
    }
    this.mMessages = messageList.ToArray();
  }

  public void Initialize()
  {
    for (int index = 0; index < this.mMessages.Length; ++index)
      this.mMessages[index].Initialize();
  }

  public int Current => this.mCurrent;

  public Controller Interactor => this.mInteractor;

  public bool LockGame => this.mLock;

  public InteractType IconText => this.mIconText;

  public Message this[int index] => this.mMessages[index];

  public bool Advance(TextBox iTextBox, Vector3 iWorldPosition, Controller iInteractor)
  {
    this.mWorldPosition = iWorldPosition;
    this.mLastOwner = (Entity) null;
    this.mScreenPos = false;
    return this.Advance(iTextBox, iInteractor);
  }

  public bool Advance(TextBox iTextBox, Vector2 iScreenPos, Controller iInteractor)
  {
    this.mWorldPosition.X = iScreenPos.X;
    this.mWorldPosition.Y = iScreenPos.Y;
    this.mWorldPosition.Z = 0.0f;
    this.mLastOwner = (Entity) null;
    this.mScreenPos = true;
    return this.Advance(iTextBox, iInteractor);
  }

  public bool Advance(TextBox iTextBox, Entity iDefaultOwner, Controller iInteractor)
  {
    this.mLastOwner = iDefaultOwner;
    this.mScreenPos = false;
    return this.Advance(iTextBox, iInteractor);
  }

  public bool Advance(TextBox iTextBox, Controller iInteractor)
  {
    if (this.mRepeatCounter >= this.mRepeat && this.mRepeat > 0)
      return true;
    if (this.mInteractor == null && iInteractor != null)
    {
      this.mInteractor = iInteractor;
      ControlManager.Instance.LockPlayerInput(this.mInteractor);
    }
    if (this.mLastOwner == null ? (!this.mScreenPos ? this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, this.mWorldPosition, this.mFocusOwner) : this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, new Vector2(this.mWorldPosition.X, this.mWorldPosition.Y))) : this.mMessages[this.mCurrent].Advance(iTextBox, this.mLastOwner, this.mFocusOwner))
    {
      ++this.mCurrent;
      if (this.mCurrent >= this.mMessages.Length)
      {
        ++this.mRepeatCounter;
        this.mCurrent = 0;
        if (this.mInteractor != null)
        {
          ControlManager.Instance.UnlockPlayerInput(this.mInteractor);
          this.mInteractor = (Controller) null;
        }
        return true;
      }
      if (this.mLastOwner != null)
        this.mMessages[this.mCurrent].Advance(iTextBox, this.mLastOwner, this.mFocusOwner);
      else if (this.mScreenPos)
        this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, new Vector2(this.mWorldPosition.X, this.mWorldPosition.Y));
      else
        this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, this.mWorldPosition, this.mFocusOwner);
    }
    return false;
  }

  public void End()
  {
    if (this.mCurrent >= 0 && this.mCurrent < this.mMessages.Length)
      this.mMessages[this.mCurrent].StopCue();
    if (this.mInteractor == null)
      return;
    ControlManager.Instance.UnlockPlayerInput(this.mInteractor);
  }

  public int Repetitions => this.mRepeat - this.mRepeatCounter;

  public Dialog Parent => this.mParent;

  public bool RepeatInfinitly => this.mRepeat <= 0;

  public void Reset()
  {
    this.mCurrent = 0;
    this.mRepeatCounter = 0;
    for (int index = 0; index < this.mMessages.Length; ++index)
      this.mMessages[index].Reset();
  }

  public struct State
  {
    private int mCurrent;
    private int mRepeatCounter;
    private Interact mInteract;

    public State(Interact iInteract)
    {
      this.mInteract = iInteract;
      this.mRepeatCounter = iInteract.mRepeatCounter;
      this.mCurrent = iInteract.mCurrent;
    }

    public void ApplyState()
    {
      for (int index = 0; index < this.mInteract.mMessages.Length; ++index)
        this.mInteract.mMessages[index].Reset();
      this.mInteract.mRepeatCounter = this.mRepeatCounter;
      this.mInteract.mCurrent = this.mCurrent;
    }

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mCurrent);
      iWriter.Write(this.mRepeatCounter);
    }

    internal void Read(BinaryReader iReader)
    {
      this.mCurrent = iReader.ReadInt32();
      this.mRepeatCounter = iReader.ReadInt32();
    }
  }
}
