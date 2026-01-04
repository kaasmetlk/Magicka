// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Message
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class Message
{
  private static readonly int OWNERHASH = "owner".GetHashCodeCustom();
  private static readonly float CAMERATIME = 2f;
  private string mOwner;
  private int mOwnerID;
  private MessageType mType;
  private Animations[] mAnimations;
  private Banks[] mBanks;
  private int[] mSounds;
  private Cue mCue;
  private bool mTurnToTarget;
  private float[] mMessageTTLs;
  private int[] mMessageIDs;
  private int[][] mMessageArgs;
  private string[] mMessages;
  private MagickaFont mFont = MagickaFont.Maiandra16;
  private int mCurrent;
  private bool mForceOnScreen;
  private Vector2 mMinBoxSize;
  private int mLineLength = 600;

  public Message(XmlNode iInput)
  {
    this.mTurnToTarget = true;
    this.mForceOnScreen = true;
    for (int i = 0; i < iInput.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iInput.Attributes[i];
      if (attribute.Name.Equals("character", StringComparison.OrdinalIgnoreCase))
      {
        this.mOwner = attribute.Value.ToLowerInvariant();
        this.mOwnerID = this.mOwner.GetHashCodeCustom();
      }
      else if (attribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
        this.mType = (MessageType) Enum.Parse(typeof (MessageType), attribute.Value, true);
      else if (attribute.Name.Equals("forceonscreen", StringComparison.OrdinalIgnoreCase))
        bool.TryParse(attribute.Value, out this.mForceOnScreen);
      else if (attribute.Name.Equals("facing", StringComparison.OrdinalIgnoreCase))
      {
        bool.TryParse(attribute.Value, out this.mTurnToTarget);
      }
      else
      {
        if (!attribute.Name.Equals("width", StringComparison.OrdinalIgnoreCase))
          throw new NotImplementedException();
        int result;
        if (int.TryParse(attribute.Value, out result))
          this.mLineLength = result;
      }
    }
    this.mMessageTTLs = new float[iInput.ChildNodes.Count];
    this.mMessageIDs = new int[iInput.ChildNodes.Count];
    this.mAnimations = new Animations[iInput.ChildNodes.Count];
    this.mSounds = new int[iInput.ChildNodes.Count];
    this.mBanks = new Banks[iInput.ChildNodes.Count];
    this.mMessageArgs = new int[iInput.ChildNodes.Count][];
    for (int index = 0; index < this.mAnimations.Length; ++index)
    {
      this.mAnimations[index] = Animations.talk_casual0;
      this.mBanks[index] = Banks.Characters;
    }
    int index1 = 0;
    foreach (XmlNode childNode in iInput.ChildNodes)
    {
      float num = -1f;
      foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode.Attributes)
      {
        if (attribute.Name.Equals("ttl", StringComparison.OrdinalIgnoreCase))
          num = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        else if (attribute.Name.Equals("anim", StringComparison.OrdinalIgnoreCase))
          this.mAnimations[index1] = (Animations) Enum.Parse(typeof (Animations), attribute.Value, true);
        else if (attribute.Name.Equals("sound", StringComparison.OrdinalIgnoreCase))
        {
          string[] strArray = attribute.Value.ToLowerInvariant().Split('/');
          if (strArray.Length == 1)
          {
            this.mSounds[index1] = strArray[0].GetHashCodeCustom();
          }
          else
          {
            this.mBanks[index1] = strArray.Length == 2 ? (Banks) Enum.Parse(typeof (Banks), strArray[0], true) : throw new Exception("Invalid syntax in \"Sound\" attribute!");
            this.mSounds[index1] = strArray[1].GetHashCodeCustom();
          }
        }
      }
      this.mMessageTTLs[index1] = num;
      string[] strArray1 = childNode.InnerText.ToLowerInvariant().Split(',');
      if (strArray1.Length > 1)
      {
        this.mMessageArgs[index1] = new int[strArray1.Length - 1];
        for (int index2 = 1; index2 < strArray1.Length; ++index2)
          this.mMessageArgs[index1][index2 - 1] = strArray1[index2].GetHashCodeCustom();
      }
      this.mMessageIDs[index1] = strArray1[0].GetHashCodeCustom();
      ++index1;
    }
  }

  public void Initialize()
  {
    this.mMessages = new string[this.mMessageIDs.Length];
    BitmapFont font = FontManager.Instance.GetFont(this.mFont);
    LanguageManager instance = LanguageManager.Instance;
    for (int index1 = 0; index1 < this.mMessageIDs.Length; ++index1)
    {
      string iText1 = instance.GetString(this.mMessageIDs[index1]);
      for (int index2 = iText1.IndexOf('#'); index2 >= 0; index2 = iText1.IndexOf('#'))
      {
        int num1 = iText1.IndexOf(';', index2 + 1);
        if (num1 > index2)
        {
          string str = iText1.Substring(index2, num1 - index2);
          int hashCodeCustom;
          try
          {
            int num2 = int.Parse(str.Substring(1));
            hashCodeCustom = this.mMessageArgs[index1][num2 - 1];
          }
          catch (FormatException ex)
          {
            hashCodeCustom = str.ToLowerInvariant().GetHashCodeCustom();
          }
          iText1 = $"{iText1.Substring(0, index2)}[c=1,1,1]{instance.GetString(hashCodeCustom)}[/c]{iText1.Substring(num1 + 1)}";
        }
        else
          break;
      }
      string iText2 = font.Wrap(iText1, this.mLineLength, true);
      this.mMessages[index1] = iText2;
      this.mMinBoxSize = Vector2.Max(this.mMinBoxSize, font.MeasureText(iText2, true));
    }
  }

  public int Owner => this.mOwnerID;

  public MessageType Type => this.mType;

  public string[] Text => this.mMessages;

  public bool TurnToTarget => this.mTurnToTarget;

  public void StopCue()
  {
    if (this.mCue == null)
      return;
    this.mCue.Stop(AudioStopOptions.Immediate);
  }

  public bool Advance(TextBox iBox, Scene iScene, Vector2 iScreenPosition)
  {
    if (this.mCurrent >= this.mMessageIDs.Length)
    {
      this.mCurrent = 0;
      iBox.Hide();
      return true;
    }
    if (this.mCue != null && !this.mCue.IsStopping)
      this.mCue.Stop(AudioStopOptions.AsAuthored);
    if (this.mSounds[this.mCurrent] != 0)
      this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent]);
    iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, iScreenPosition, this.mForceOnScreen, this.mOwnerID, this.mMessageTTLs[this.mCurrent]);
    ++this.mCurrent;
    return false;
  }

  public bool Advance(TextBox iBox, Scene iScene, Vector3 iWorldPosition, bool iFocusOwner)
  {
    Magicka.GameLogic.Entities.Character byId = Entity.GetByID(this.mOwnerID) as Magicka.GameLogic.Entities.Character;
    MagickCamera camera = iScene.Camera as MagickCamera;
    if (this.mCue != null && !this.mCue.IsStopping)
      this.mCue.Stop(AudioStopOptions.AsAuthored);
    if (this.mCurrent >= this.mMessageIDs.Length)
    {
      this.mCurrent = 0;
      iBox.Hide();
      if (byId != null)
      {
        if (iFocusOwner)
          camera.Release(Message.CAMERATIME);
        if (this.mAnimations[Math.Max(this.mMessageIDs.Length - 1, 0)] != Animations.None)
        {
          if (byId.SpecialIdleAnimation != Animations.None)
            byId.GoToAnimation(byId.SpecialIdleAnimation, 0.3f);
          else
            byId.GoToAnimation(Animations.idle, 0.3f);
        }
      }
      return true;
    }
    if (byId != null)
    {
      if (this.mAnimations[this.mCurrent] != Animations.None)
        byId.GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
      if (this.mSounds[this.mCurrent] != 0)
        this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent], byId.AudioEmitter);
      if (iFocusOwner)
      {
        camera.MoveTo(byId.Position, Message.CAMERATIME);
        camera.Magnification = 1.5f;
      }
      iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, this.mForceOnScreen, (Entity) byId, this.mMessageTTLs[this.mCurrent]);
    }
    else
    {
      this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent]);
      iBox.Initialize(iScene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, iWorldPosition, this.mForceOnScreen, this.mOwnerID, this.mMessageTTLs[this.mCurrent]);
    }
    ++this.mCurrent;
    return false;
  }

  public bool Advance(TextBox iBox, Entity iDefaultOwner, bool iFocusOwner)
  {
    Entity iOwner = this.mOwnerID != Message.OWNERHASH ? Entity.GetByID(this.mOwnerID) : iDefaultOwner;
    if (iOwner == null || iOwner.Dead)
    {
      ++this.mCurrent;
      return this.mCurrent >= this.mMessageIDs.Length || this.Advance(iBox, iDefaultOwner, iFocusOwner);
    }
    if (this.mCue != null && !this.mCue.IsStopping)
      this.mCue.Stop(AudioStopOptions.AsAuthored);
    MagickCamera magickCamera = (MagickCamera) null;
    if (iOwner != null)
      magickCamera = iOwner.PlayState.Scene.Camera as MagickCamera;
    if (this.mCurrent >= this.mMessageIDs.Length)
    {
      Magicka.GameLogic.Entities.Character character = iOwner as Magicka.GameLogic.Entities.Character;
      this.mCurrent = 0;
      iBox.Hide();
      if (iFocusOwner)
        magickCamera.Release(Message.CAMERATIME);
      if (character != null && !(character.CurrentState is DeadState) && this.mAnimations[Math.Max(this.mMessageIDs.Length - 1, 0)] != Animations.None)
      {
        if (character.SpecialIdleAnimation != Animations.None)
          character.GoToAnimation(character.SpecialIdleAnimation, 0.3f);
        else
          character.GoToAnimation(Animations.idle, 0.3f);
      }
      return true;
    }
    NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
    if (this.mAnimations[this.mCurrent] != Animations.None)
    {
      if (nonPlayerCharacter != null)
      {
        if (nonPlayerCharacter.AI.Events == null || nonPlayerCharacter.AI.Events.Length == 0 || nonPlayerCharacter.AI.CurrentEvent >= nonPlayerCharacter.AI.Events.Length)
          (iOwner as Magicka.GameLogic.Entities.Character).GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
      }
      else if (iOwner is Avatar && (iOwner as Avatar).Events == null)
        (iOwner as Magicka.GameLogic.Entities.Character).GoToAnimation(this.mAnimations[this.mCurrent], 0.3f);
    }
    if (this.mSounds[this.mCurrent] != 0)
      this.mCue = AudioManager.Instance.PlayCue(this.mBanks[this.mCurrent], this.mSounds[this.mCurrent], iOwner.AudioEmitter);
    if (iFocusOwner)
    {
      magickCamera.MoveTo(iOwner.Position, Message.CAMERATIME);
      magickCamera.Magnification = 1.5f;
    }
    if (iOwner != null)
      iBox.Initialize(iOwner.PlayState.Scene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, this.mForceOnScreen, iOwner, this.mMessageTTLs[this.mCurrent]);
    else
      iBox.Initialize(iOwner.PlayState.Scene, MagickaFont.Maiandra16, this.mMessages[this.mCurrent], this.mMinBoxSize, iOwner.Position, this.mForceOnScreen, iOwner.UniqueID, this.mMessageTTLs[this.mCurrent]);
    ++this.mCurrent;
    return false;
  }

  public void Reset() => this.mCurrent = 0;
}
