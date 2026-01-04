// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Items.PlaySoundEvent
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

#nullable disable
namespace Magicka.GameLogic.Entities.Items;

public struct PlaySoundEvent
{
  private static AudioEmitter sEmitter = new AudioEmitter();
  private static readonly string VAR_NAME = "Magnitude";
  private Banks mSoundBank;
  private int mSoundHash;
  private float mMagnitude;
  private bool mStopOnRemove;

  public PlaySoundEvent(Banks iBank, int iHash, bool iStopOnRemove, float iMagnitude)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iHash;
    this.mStopOnRemove = iStopOnRemove;
    this.mMagnitude = iMagnitude;
  }

  public PlaySoundEvent(Banks iBank, int iHash, bool iStopOnRemove)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iHash;
    this.mStopOnRemove = iStopOnRemove;
    this.mMagnitude = 1f;
  }

  public PlaySoundEvent(Banks iBank, int iHash)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iHash;
    this.mStopOnRemove = false;
    this.mMagnitude = 1f;
  }

  public PlaySoundEvent(Banks iBank, string iCue, bool iStopOnRemove, float iMagnitude)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iCue.GetHashCodeCustom();
    this.mStopOnRemove = iStopOnRemove;
    this.mMagnitude = iMagnitude;
  }

  public PlaySoundEvent(Banks iBank, string iCue, bool iStopOnRemove)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iCue.GetHashCodeCustom();
    this.mStopOnRemove = iStopOnRemove;
    this.mMagnitude = 1f;
  }

  public PlaySoundEvent(Banks iBank, int iHash, float iMagnitude)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iHash;
    this.mStopOnRemove = false;
    this.mMagnitude = iMagnitude;
  }

  public PlaySoundEvent(Banks iBank, string iCue)
  {
    this.mSoundBank = iBank;
    this.mSoundHash = iCue.GetHashCodeCustom();
    this.mStopOnRemove = false;
    this.mMagnitude = 1f;
  }

  public PlaySoundEvent(ContentReader iInput)
  {
    this.mSoundBank = (Banks) iInput.ReadInt32();
    this.mSoundHash = iInput.ReadString().GetHashCodeCustom();
    this.mMagnitude = iInput.ReadSingle();
    this.mStopOnRemove = iInput.ReadBoolean();
  }

  public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
  {
    AudioEmitter AE = iItem.AudioEmitter;
    if (iPosition.HasValue)
    {
      AE = PlaySoundEvent.sEmitter;
      AE.Position = iPosition.Value;
    }
    Cue iCue = AudioManager.Instance.PlayCue(this.mSoundBank, this.mSoundHash, AE);
    if (iCue == null)
      return;
    iCue.SetVariable(PlaySoundEvent.VAR_NAME, this.mMagnitude);
    if (!(iItem is MissileEntity) || !this.mStopOnRemove)
      return;
    (iItem as MissileEntity).AddCue(iCue);
  }

  public Banks SoundBank => this.mSoundBank;

  public int SoundHash => this.mSoundHash;
}
