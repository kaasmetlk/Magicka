// Decompiled with JetBrains decompiler
// Type: PolygonHead.Camera
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace PolygonHead;

public class Camera
{
  internal Camera.CameraState[] mStateBuffer;
  protected BoundingFrustum[] mViewFrustum;
  protected Vector3 mPosition;
  protected Vector3 mDirection;
  protected Vector3 mUp;
  protected float mFov;
  protected float mAspectRatio;
  protected float mNearPlaneDistance;
  protected float mFarPlaneDistance;
  protected float mSpeed;

  public Camera(
    Vector3 iPosition,
    Vector3 iDirection,
    Vector3 iUp,
    float iFov,
    float iAspectRation,
    float iNearPlaneDistance,
    float iFarPlaneDistance)
  {
    this.mStateBuffer = new Camera.CameraState[3];
    this.mViewFrustum = new BoundingFrustum[3];
    for (int index = 0; index < 3; ++index)
      this.mViewFrustum[index] = new BoundingFrustum(Matrix.Identity);
    this.Set(iPosition, iDirection, iUp, iFov, iAspectRation, iNearPlaneDistance, iFarPlaneDistance);
  }

  public void LerpTo(Vector3 iPosition)
  {
    Vector3.Lerp(ref this.mPosition, ref iPosition, this.mSpeed, out this.mPosition);
  }

  public void LerpTo(Vector3 iPosition, float iSpeed)
  {
    Vector3.Lerp(ref this.mPosition, ref iPosition, iSpeed, out this.mPosition);
  }

  public void Set(
    Vector3 iPosition,
    Vector3 iDirection,
    Vector3 iUp,
    float iFov,
    float iAspectRation,
    float iNearPlaneDistance,
    float iFarPlaneDistance)
  {
    this.mPosition = iPosition;
    this.mDirection = Vector3.Normalize(iDirection);
    this.mUp = iUp;
    this.mFov = iFov;
    this.mAspectRatio = iAspectRation;
    this.mNearPlaneDistance = iNearPlaneDistance;
    this.mFarPlaneDistance = iFarPlaneDistance;
  }

  public void GetViewMatrix(DataChannel iDataChannel, out Matrix oMatrix)
  {
    oMatrix = this.mStateBuffer[(int) iDataChannel].ViewMatrix;
  }

  public void GetProjectionMatrix(DataChannel iDataChannel, out Matrix oMatrix)
  {
    oMatrix = this.mStateBuffer[(int) iDataChannel].ProjectionMatrix;
  }

  public void GetViewProjectionMatrix(DataChannel iDataChannel, out Matrix oMatrix)
  {
    oMatrix = this.mStateBuffer[(int) iDataChannel].ViewProjectionMatrix;
  }

  public BoundingFrustum GetViewFrustum(DataChannel iDataChannel)
  {
    return this.mViewFrustum[(int) iDataChannel];
  }

  private void UpdateState(DataChannel iDataChannel)
  {
    this.mStateBuffer[(int) iDataChannel].Position = this.mPosition;
    this.mStateBuffer[(int) iDataChannel].EyeOfTheBeholder = this.EyeOfTheBeholder;
    this.mStateBuffer[(int) iDataChannel].Direction = this.mDirection;
    this.mStateBuffer[(int) iDataChannel].Up = this.mUp;
    Vector3 result;
    Vector3.Add(ref this.mPosition, ref this.mDirection, out result);
    Matrix.CreateLookAt(ref this.mPosition, ref result, ref this.mUp, out this.mStateBuffer[(int) iDataChannel].ViewMatrix);
    Matrix.CreatePerspectiveFieldOfView(this.mFov, this.mAspectRatio, this.mNearPlaneDistance, this.mFarPlaneDistance, out this.mStateBuffer[(int) iDataChannel].ProjectionMatrix);
    Matrix.Multiply(ref this.mStateBuffer[(int) iDataChannel].ViewMatrix, ref this.mStateBuffer[(int) iDataChannel].ProjectionMatrix, out this.mStateBuffer[(int) iDataChannel].ViewProjectionMatrix);
    this.mViewFrustum[(int) iDataChannel].Matrix = this.mStateBuffer[(int) iDataChannel].ViewProjectionMatrix;
  }

  public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.UpdateState(iDataChannel);
  }

  public virtual void SetPosition(ref Vector3 iPosition) => this.mPosition = iPosition;

  public Vector3 Position
  {
    get => this.mPosition;
    set => this.mPosition = value;
  }

  public virtual Vector3 EyeOfTheBeholder => this.mPosition;

  public Vector3 Direction
  {
    get => this.mDirection;
    set
    {
      this.mDirection = value;
      this.mDirection.Normalize();
    }
  }

  public Vector3 Up
  {
    get => this.mUp;
    set
    {
      this.mUp = value;
      this.mUp.Normalize();
    }
  }

  public float Fov
  {
    get => this.mFov;
    set => this.mFov = value;
  }

  public float AspectRation
  {
    get => this.mAspectRatio;
    set => this.mAspectRatio = value;
  }

  public float NearPlaneDistance
  {
    get => this.mNearPlaneDistance;
    set => this.mNearPlaneDistance = value;
  }

  public float Speed
  {
    get => this.mSpeed;
    set => this.mSpeed = value;
  }

  public float FarPlaneDistance
  {
    get => this.mFarPlaneDistance;
    set => this.mFarPlaneDistance = value;
  }

  public void Move(Vector3 movement)
  {
    this.mPosition += this.mDirection * movement.Z;
    Vector3 vector1 = Vector3.Normalize(Vector3.Cross(this.mDirection, this.mUp));
    this.mPosition += vector1 * movement.X;
    this.mPosition += Vector3.Normalize(Vector3.Cross(vector1, this.mDirection)) * movement.Y;
  }

  public void Rotate(float iYaw, float iPitch)
  {
    this.mDirection = Vector3.TransformNormal(this.mDirection, Matrix.CreateFromAxisAngle(this.mUp, iYaw));
    float num = (float) Math.Asin((double) this.mDirection.Y) + iPitch;
    if ((double) num > 1.4137166738510132)
      iPitch = 1.41371667f - num;
    else if ((double) num < -1.4137166738510132)
      iPitch = 1.41371667f - num;
    this.mDirection = Vector3.TransformNormal(this.mDirection, Matrix.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(this.mDirection, this.mUp)), iPitch));
  }

  public struct CameraState
  {
    public Matrix ViewMatrix;
    public Matrix ProjectionMatrix;
    public Matrix ViewProjectionMatrix;
    public Vector3 Position;
    public Vector3 EyeOfTheBeholder;
    public Vector3 Direction;
    public Vector3 Up;
  }
}
