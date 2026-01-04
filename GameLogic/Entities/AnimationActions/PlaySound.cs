// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.AnimationActions.PlaySound
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

#nullable disable
namespace Magicka.GameLogic.Entities.AnimationActions;

public class PlaySound : AnimationAction
{
  private string mSound;
  private Banks mBank;
  private int mSoundHash;

  public PlaySound(ContentReader iInput, SkinnedModelBoneCollection iSkeleton)
    : base(iInput, iSkeleton)
  {
    this.mSound = iInput.ReadString();
    this.mBank = (Banks) iInput.ReadInt32();
    this.mSoundHash = this.mSound.GetHashCodeCustom();
  }

  protected override void InternalExecute(Character iOwner, bool iFirstExecution)
  {
    if (!iFirstExecution)
      return;
    iOwner.AudioEmitter.Position = iOwner.Position;
    iOwner.AudioEmitter.Up = Vector3.Up;
    iOwner.AudioEmitter.Forward = iOwner.Direction;
    AudioManager.Instance.PlayCue(this.mBank, this.mSoundHash, iOwner.AudioEmitter);
  }

  public override bool UsesBones => false;
}
