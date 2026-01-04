// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.CameraMove
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

internal class CameraMove(Trigger iTrigger, GameScene iScene) : Action(iTrigger, iScene)
{
  private bool mLockInput;
  private float mTime = 1f;
  private string mTarget;
  private int mTargetHash;
  private Vector3 mOffset;
  private float mMagnification = -1f;
  private CameraInterpolation mInterpolation = CameraInterpolation.Interpolated;
  private MagickCamera mCamera;

  public override void Initialize()
  {
    base.Initialize();
    this.mCamera = this.GameScene.PlayState.Camera;
  }

  protected override void Execute()
  {
    Matrix oLocator;
    Vector3 result = !this.GameScene.TryGetLocator(this.mTargetHash, out oLocator) ? this.mCamera.GroundPosition : oLocator.Translation;
    Vector3.Add(ref result, ref this.mOffset, out result);
    this.mCamera.MoveTo(result, this.mTime);
    this.mCamera.Interpolation = this.mInterpolation;
    if ((double) this.mMagnification > 0.0)
      this.mCamera.Magnification = this.mMagnification;
    this.mCamera.LockInput = this.mLockInput;
  }

  public override void QuickExecute()
  {
  }

  public bool LockInput
  {
    get => this.mLockInput;
    set => this.mLockInput = value;
  }

  public float Magnification
  {
    get => this.mMagnification;
    set => this.mMagnification = value;
  }

  public float Time
  {
    get => this.mTime;
    set => this.mTime = value;
  }

  public string Target
  {
    get => this.mTarget;
    set
    {
      this.mTarget = value;
      this.mTargetHash = this.mTarget.GetHashCodeCustom();
    }
  }

  public Vector3 Offset
  {
    get => this.mOffset;
    set => this.mOffset = value;
  }

  public CameraInterpolation Interpolation
  {
    get => this.mInterpolation;
    set => this.mInterpolation = value;
  }
}
