// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.AudioLocator
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace Magicka.Levels;

public struct AudioLocator
{
  private static readonly string DISTANCE_NORMALIZED_VAR_NAME = "NormalizedDistance";
  private static readonly string DISTANCE_VAR_NAME = "Distance";
  private static readonly string VOLUME_VAR_NAME = nameof (Volume);
  private static AudioEmitter sEmitter = new AudioEmitter();
  private int mID;
  private int mLocator;
  private bool mApply3D;
  private Banks mBank;
  private int mCueID;
  private Cue mCue;
  private float mRadius;
  private float mVolume;

  public AudioLocator(
    int iID,
    Banks iBank,
    int iCue,
    float iVolume,
    int iLocator,
    float iRadius,
    bool iApply3D)
  {
    this.mID = iID;
    this.mBank = iBank;
    this.mCueID = iCue;
    this.mCue = AudioManager.Instance.GetCue(iBank, iCue);
    this.mCue.SetVariable(AudioLocator.VOLUME_VAR_NAME, iVolume);
    this.mLocator = iLocator;
    this.mApply3D = iApply3D;
    this.mRadius = iRadius;
    this.mVolume = iVolume;
  }

  public void Play()
  {
    if (this.mCue.IsPaused)
      this.mCue.Resume();
    if (this.mCue.IsPlaying)
      return;
    if (this.mApply3D)
      this.mCue.Apply3D(AudioManager.Instance.getListener(), AudioLocator.sEmitter);
    if (this.mCue.IsStopped || this.mCue.IsStopping)
      return;
    this.mCue.Play();
  }

  public void Pause()
  {
    if (!this.mCue.IsPlaying)
      return;
    this.mCue.Pause();
  }

  public void Stop(AudioStopOptions iStopOptions)
  {
    if (!this.mCue.IsPlaying)
      return;
    this.mCue.Stop(iStopOptions);
  }

  public int Locator => this.mLocator;

  public bool Apply3D => this.mApply3D;

  public Banks Bank => this.mBank;

  public int CueID => this.mCueID;

  public Cue Cue => this.mCue;

  public float Radius => this.mRadius;

  public float Volume => this.mVolume;

  public void Update(GameScene iScene)
  {
    if (!this.mCue.IsPlaying)
      return;
    Matrix oLocator;
    iScene.GetLocator(this.mLocator, out oLocator);
    Vector3 translation = oLocator.Translation;
    Vector3 groundPosition = iScene.PlayState.Camera.GroundPosition;
    float result;
    Vector3.Distance(ref translation, ref groundPosition, out result);
    this.mCue.SetVariable(AudioLocator.DISTANCE_NORMALIZED_VAR_NAME, result / this.mRadius);
    if (this.mApply3D)
    {
      AudioLocator.sEmitter.Position = oLocator.Translation;
      AudioLocator.sEmitter.Forward = oLocator.Forward;
      AudioLocator.sEmitter.Up = oLocator.Up;
      this.mCue.Apply3D(iScene.PlayState.Camera.Listener, AudioLocator.sEmitter);
    }
    this.mCue.SetVariable(AudioLocator.DISTANCE_VAR_NAME, 0.0f);
  }

  public int ID => this.mID;
}
