// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.MagickCamera
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Magicka.Graphics;

public class MagickCamera : Camera
{
  public static readonly float NEARCLIP = 105f;
  public static readonly float FARCLIP = 500f;
  public static readonly Vector3 CAMERAOFFSET = new Vector3(0.0f, 144f, 171f);
  public static readonly float DEFAULTFOV = MathHelper.ToRadians(5f);
  public static readonly float RATIO_16_9 = 1.77777779f;
  public static readonly float RATIO_4_3 = 1.33333337f;
  private float mDefaultMagnification = 1f;
  private AudioListener mListener;
  private float mMagnification = 1f;
  private float mTargetMagnification = 1f;
  private float mPlayerMagnification = 1f;
  private float mPlayerMagnificationTTL;
  private float mTime = 1f;
  private float mBiasTime = 10f;
  private float mTimeStart;
  private float mCameraShakeMagnitude;
  private float mStartCameraShakeTTL;
  private float mCameraShakeTTL;
  private CameraInterpolation mInterpolation = CameraInterpolation.Interpolated;
  private CameraBehaviour mCurrentBehaviour;
  private List<Magicka.GameLogic.Entities.Character> mPlayers;
  private List<Magicka.GameLogic.Entities.Character> mNetworkPlayers;
  private Vector3[] mPlayerDirections;
  private Entity mFollowing;
  private Vector3 mVelocity;
  private Vector3 mTargetPosition;
  private Vector3 mCurrentPosition;
  private JigLibX.Geometry.Plane[] mCollisionPlanesOld;
  private JigLibX.Geometry.Plane[] mCollisionPlanesNew;
  private Primitive mCameraMesh;
  private Box mCameraBox;
  private CollisionSkin mCollisionSkin;
  private bool mCollisionEnabled = true;
  private Vector3[] mFrustumCorners = new Vector3[8];
  private bool mCanMoveUp = true;
  private bool mCanMoveRight = true;
  private bool mCanMoveDown = true;
  private bool mCanMoveLeft = true;
  private bool mCanZoomIn = true;
  private bool mLockInput;
  private Vector3 mBias;
  private Vector3 mTargetBias;
  private bool mDynamicMagnify;
  private float mMinMagnification;
  private float mMaxMagnification;
  private SortedList<int, VisualEffectReference> mVisualEffects;
  private PlayState mPlayState;

  public MagickCamera(
    Vector3 iPosition,
    Vector3 iDirection,
    Vector3 iUp,
    float iFov,
    float iAspectRation,
    float iNearPlaneDistance,
    float iFarPlaneDistance)
    : base(iPosition, iDirection, iUp, iFov, iAspectRation, iNearPlaneDistance, iFarPlaneDistance)
  {
    this.mListener = new AudioListener();
    this.mPlayers = new List<Magicka.GameLogic.Entities.Character>(8);
    this.mNetworkPlayers = new List<Magicka.GameLogic.Entities.Character>(8);
    this.mPlayerDirections = new Vector3[4];
    this.mCollisionSkin = new CollisionSkin((Body) null);
    this.mCollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
    this.mCollisionPlanesOld = new JigLibX.Geometry.Plane[4];
    this.mCollisionPlanesNew = new JigLibX.Geometry.Plane[4];
    for (int prim = 0; prim < 4; ++prim)
    {
      this.mCollisionSkin.AddPrimitive((Primitive) new JigLibX.Geometry.Plane(Vector3.Down, new Vector3(0.0f, 1000f, 0.0f)), 1, new MaterialProperties());
      this.mCollisionPlanesOld[prim] = this.mCollisionSkin.GetPrimitiveOldWorld(prim) as JigLibX.Geometry.Plane;
      this.mCollisionPlanesNew[prim] = this.mCollisionSkin.GetPrimitiveNewWorld(prim) as JigLibX.Geometry.Plane;
    }
    this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
    this.mVisualEffects = new SortedList<int, VisualEffectReference>();
    this.mCollisionSkin.Tag = (object) this;
  }

  internal CameraBehaviour CurrentBehaviour => this.mCurrentBehaviour;

  public Entity TargetEntity => this.mFollowing;

  public void SetPlayState(PlayState iPlayState) => this.mPlayState = iPlayState;

  public void Init()
  {
    this.mTargetBias = new Vector3();
    this.mBias = new Vector3();
    this.Set(MagickCamera.CAMERAOFFSET, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, this.Fov, this.NearPlaneDistance, this.FarPlaneDistance);
    this.Rotate(0.0f, -MathHelper.ToRadians(40f));
  }

  private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
  {
    CharacterBody owner1 = iSkin1.Owner as CharacterBody;
    if (!this.mCollisionEnabled || ControlManager.Instance.IsInputLimited || owner1 == null || !(owner1.Owner is Avatar owner2) || owner2.Dead || ((!(owner2.Player.Gamer is NetworkGamer) ? 1 : 0) & (owner2.Events == null ? 1 : (owner2.CurrentEvent >= owner2.Events.Length ? 1 : 0))) == 0)
      return false;
    Vector3 result1;
    Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out result1);
    Vector3 normal = (iSkin0.GetPrimitiveNewWorld(iPrim0) as JigLibX.Geometry.Plane).Normal;
    float result2;
    Vector3.Dot(ref normal, ref result1, out result2);
    if ((double) result2 > 0.0)
    {
      switch (iPrim0)
      {
        case 0:
          this.mCanMoveDown = false;
          break;
        case 1:
          this.mCanMoveLeft = false;
          break;
        case 2:
          this.mCanMoveUp = false;
          break;
        case 3:
          this.mCanMoveRight = false;
          break;
      }
    }
    else if ((double) result2 < -0.10000000149011612)
      return false;
    this.mCanZoomIn = false;
    return true;
  }

  public void EnableDynamicMagnification(float iMinMagnification, float iMaxMagnification)
  {
    this.mDynamicMagnify = true;
    this.mMinMagnification = iMinMagnification;
    this.mMaxMagnification = iMaxMagnification;
  }

  public void DisableDynamicMagnification() => this.mDynamicMagnify = false;

  public float DefaultMagnification
  {
    get => this.mDefaultMagnification;
    set
    {
      if ((double) System.Math.Abs(this.mTargetMagnification - this.mDefaultMagnification) < 9.9999997473787516E-05)
        this.mTargetMagnification = value;
      this.mDefaultMagnification = value;
    }
  }

  public void SetPlayerMagnification(float iMagnification, float iTTL)
  {
    this.mPlayerMagnification = iMagnification;
    this.mPlayerMagnificationTTL = iTTL;
  }

  public void SetBias(ref Vector3 iBias, float iTime)
  {
    this.mTargetBias = iBias;
    this.mBiasTime = iTime;
  }

  public override void SetPosition(ref Vector3 iPosition)
  {
    this.mCurrentPosition = iPosition;
    this.mTargetPosition = iPosition;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    for (int index = 0; index < this.mPlayers.Count; ++index)
    {
      if (this.mPlayers[index].Dead)
      {
        if (float.IsNaN(this.mPlayers[index].UndyingTimer))
        {
          this.mPlayers.RemoveAt(index);
          --index;
        }
        else if (!this.mPlayers[index].Undying && (double) this.mPlayers[index].UndyingTimer < 0.0)
        {
          this.mPlayers.RemoveAt(index);
          --index;
        }
      }
    }
    for (int index = 0; index < this.mNetworkPlayers.Count; ++index)
    {
      if (this.mNetworkPlayers[index].Dead)
      {
        if (float.IsNaN(this.mNetworkPlayers[index].UndyingTimer))
        {
          this.mNetworkPlayers.RemoveAt(index);
          --index;
        }
        else if (!this.mNetworkPlayers[index].Undying && (double) this.mNetworkPlayers[index].UndyingTimer < 0.0)
        {
          this.mNetworkPlayers.RemoveAt(index);
          --index;
        }
      }
    }
    switch (this.mCurrentBehaviour)
    {
      case CameraBehaviour.FollowPlayers:
        Vector3 oPosition;
        bool centerPosition = this.GetCenterPosition(this.mPlayers, out oPosition);
        if (!centerPosition)
          centerPosition = this.GetCenterPosition(this.mNetworkPlayers, out oPosition);
        if (centerPosition)
        {
          Vector3 oVector;
          this.GetInfluenceVector(out oVector);
          Vector3.Add(ref oPosition, ref oVector, out this.mTargetPosition);
          Vector3.Add(ref this.mTargetPosition, ref this.mBias, out this.mTargetPosition);
        }
        else
          this.mTargetPosition = this.mCurrentPosition;
        this.mTime += (1f - this.mTime) * iDeltaTime;
        if (this.mCameraMesh != null || this.mCameraBox != null)
        {
          this.mTargetPosition = this.SnapPosition(this.mTargetPosition);
          break;
        }
        break;
      case CameraBehaviour.FollowEntity:
        if (this.mFollowing == null)
        {
          this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
          break;
        }
        this.mTargetPosition = this.mFollowing.Position;
        if (this.mCameraMesh != null || this.mCameraBox != null)
        {
          this.mTargetPosition = this.SnapPosition(this.mTargetPosition);
          break;
        }
        break;
    }
    Vector3.Lerp(ref this.mTargetBias, ref this.mBias, (float) System.Math.Pow(0.02, (double) iDeltaTime / (double) this.mBiasTime), out this.mBias);
    Vector3 result1 = new Vector3();
    float num1;
    if ((double) this.mTime > 0.0)
    {
      switch (this.mInterpolation)
      {
        case CameraInterpolation.Linear:
          Vector3 result2;
          Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out result2);
          float num2 = result2.Length();
          float num3 = 100f * iDeltaTime / this.mTime;
          if ((double) num3 < (double) num2)
          {
            Vector3.Multiply(ref result2, num3 / num2, out result2);
            result1 = this.mCurrentPosition + result2;
            break;
          }
          result1 = this.mTargetPosition;
          break;
        case CameraInterpolation.Interpolated:
          if ((double) iDeltaTime <= 1.4012984643248171E-45)
          {
            result1 = this.mCurrentPosition;
            break;
          }
          Vector3 result3;
          Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out result3);
          Vector3 result4;
          Vector3.Divide(ref this.mVelocity, iDeltaTime, out result4);
          Vector3 result5;
          Vector3.Multiply(ref result3, 2f / this.mTime, out result5);
          Vector3 result6;
          Vector3.Multiply(ref result4, 0.5f, out result6);
          Vector3.Subtract(ref result5, ref result6, out result5);
          Vector3.Add(ref result4, ref result5, out result4);
          Vector3.Multiply(ref result4, iDeltaTime, out result4);
          if ((double) result4.LengthSquared() > (double) result3.LengthSquared())
          {
            result1 = this.mTargetPosition;
            break;
          }
          Vector3.Add(ref this.mCurrentPosition, ref result4, out result1);
          break;
      }
      float targetMagnification = this.mTargetMagnification;
      if (this.mCurrentBehaviour == CameraBehaviour.FollowPlayers && (double) this.mPlayerMagnificationTTL > 0.0)
        targetMagnification *= this.mPlayerMagnification;
      num1 = MathHelper.Lerp(targetMagnification, this.mMagnification, (float) System.Math.Pow(0.02, (double) iDeltaTime / (double) this.mTime));
    }
    else
    {
      num1 = this.mTargetMagnification;
      result1 = this.mTargetPosition;
    }
    this.mPlayerMagnificationTTL -= iDeltaTime;
    Vector3.Subtract(ref result1, ref this.mCurrentPosition, out this.mVelocity);
    if (!this.mCanMoveUp)
    {
      Vector3 result7 = this.mCollisionPlanesNew[0].Normal;
      float result8;
      Vector3.Dot(ref result7, ref this.mVelocity, out result8);
      Vector3.Multiply(ref result7, -result8, out result7);
      Vector3.Add(ref result7, ref this.mVelocity, out this.mVelocity);
    }
    if (!this.mCanMoveRight)
    {
      Vector3 result9 = this.mCollisionPlanesNew[1].Normal;
      float result10;
      Vector3.Dot(ref result9, ref this.mVelocity, out result10);
      Vector3.Multiply(ref result9, -result10, out result9);
      Vector3.Add(ref result9, ref this.mVelocity, out this.mVelocity);
    }
    if (!this.mCanMoveDown)
    {
      Vector3 result11 = this.mCollisionPlanesNew[2].Normal;
      float result12;
      Vector3.Dot(ref result11, ref this.mVelocity, out result12);
      Vector3.Multiply(ref result11, -result12, out result11);
      Vector3.Add(ref result11, ref this.mVelocity, out this.mVelocity);
    }
    if (!this.mCanMoveLeft)
    {
      Vector3 result13 = this.mCollisionPlanesNew[3].Normal;
      float result14;
      Vector3.Dot(ref result13, ref this.mVelocity, out result14);
      Vector3.Multiply(ref result13, -result14, out result13);
      Vector3.Add(ref result13, ref this.mVelocity, out this.mVelocity);
    }
    Vector3.Add(ref this.mVelocity, ref this.mCurrentPosition, out this.mCurrentPosition);
    if (this.mDynamicMagnify && this.mCurrentBehaviour == CameraBehaviour.FollowPlayers)
    {
      Vector3 position1 = this.Position;
      Vector3 direction = this.Direction;
      float num4 = 0.0f;
      for (int index = 0; index < this.mPlayers.Count; ++index)
      {
        Vector3 position2 = this.mPlayers[index].Position;
        Vector3.Subtract(ref position2, ref position1, out this.mPlayerDirections[index]);
        this.mPlayerDirections[index].Normalize();
        float result15;
        Vector3.Dot(ref direction, ref this.mPlayerDirections[index], out result15);
        result15 = (float) System.Math.Acos((double) MathHelper.Clamp(result15, -1f, 1f));
        if ((double) result15 >= (double) num4)
          num4 = result15;
      }
      this.mTargetMagnification = MathHelper.Clamp(0.5f * MagickCamera.DEFAULTFOV / num4, this.mMinMagnification, this.mMaxMagnification);
    }
    float num5 = num1 - this.mMagnification;
    if (this.mCanZoomIn || (double) num5 < 0.0)
      this.mMagnification += num5;
    this.Position = this.mCurrentPosition + MagickCamera.CAMERAOFFSET;
    this.Fov = MagickCamera.DEFAULTFOV / this.mMagnification;
    if (this.mListener != null)
    {
      Vector3 result16 = this.GroundPosition;
      Vector3 vector3 = new Vector3(0.0f, 20f, 0.0f);
      Vector3.Add(ref vector3, ref result16, out result16);
      this.mListener.Position = result16;
      this.mListener.Forward = Vector3.Down;
      this.mListener.Up = Vector3.Forward;
    }
    base.Update(iDataChannel, iDeltaTime);
    this.mViewFrustum[(int) iDataChannel].GetCorners(this.mFrustumCorners);
    for (int index = 0; index < 4; ++index)
    {
      JigLibX.Geometry.Plane plane1 = this.mCollisionPlanesOld[index];
      JigLibX.Geometry.Plane plane2 = this.mCollisionPlanesNew[index];
      plane1.D = plane2.D;
      plane1.Normal = plane2.Normal;
      Vector3 result17 = this.mFrustumCorners[index];
      Vector3 result18 = this.mFrustumCorners[(index + 1) % 4];
      Vector3 result19 = this.mFrustumCorners[index + 4];
      Vector3.Subtract(ref result18, ref result17, out result18);
      Vector3.Subtract(ref result19, ref result17, out result19);
      Vector3 result20;
      Vector3.Cross(ref result19, ref result18, out result20);
      result20.Normalize();
      Vector3 result21;
      Vector3.Multiply(ref result20, 0.5f, out result21);
      Vector3.Subtract(ref result17, ref result21, out result17);
      plane2.SetPositionNormal(result20, result17);
    }
    this.mCanMoveUp = true;
    this.mCanMoveRight = true;
    this.mCanMoveDown = true;
    this.mCanMoveLeft = true;
    this.mCanZoomIn = true;
    Vector3 forward = Vector3.Forward;
    for (int index = 0; index < this.mVisualEffects.Count; ++index)
    {
      VisualEffectReference iRef = this.mVisualEffects.Values[index];
      EffectManager.Instance.UpdatePositionDirection(ref iRef, ref this.mCurrentPosition, ref forward);
      if (iRef.ID < 0)
        this.mVisualEffects.RemoveAt(index--);
      else
        this.mVisualEffects[this.mVisualEffects.Keys[index]] = iRef;
    }
    if ((double) this.mCameraShakeTTL > 0.0 && (double) iDeltaTime > 1.4012984643248171E-45)
    {
      float oSin;
      MathApproximation.FastSin(MathHelper.WrapAngle(this.mCameraShakeTTL * 60f), out oSin);
      this.mCurrentPosition.Y += (float) ((double) oSin * (double) this.mCameraShakeMagnitude * ((double) this.mCameraShakeTTL / (double) this.mStartCameraShakeTTL) * 0.0333000011742115);
    }
    this.mCameraShakeTTL -= iDeltaTime;
  }

  public override Vector3 EyeOfTheBeholder
  {
    get
    {
      Vector3 result;
      Vector3.Multiply(ref this.mDirection, 200f, out result);
      Vector3.Add(ref this.mPosition, ref result, out result);
      return result;
    }
  }

  private Vector3 SnapPosition(Vector3 iPosition)
  {
    Vector3 closestBoxPoint = iPosition;
    if (this.mCameraBox != null)
    {
      double distanceToPoint = (double) this.mCameraBox.GetDistanceToPoint(out closestBoxPoint, iPosition);
    }
    else if (this.mCameraMesh is TriangleMesh)
    {
      TriangleMesh mCameraMesh = this.mCameraMesh as TriangleMesh;
      float num1 = float.MaxValue;
      Vector3 point = new Vector3();
      for (int iTriangle = 0; iTriangle < mCameraMesh.GetNumTriangles(); ++iTriangle)
      {
        IndexedTriangle triangle = mCameraMesh.GetTriangle(iTriangle);
        Vector3 result1;
        mCameraMesh.GetVertex(triangle.GetVertexIndex(0), out result1);
        Vector3 result2;
        mCameraMesh.GetVertex(triangle.GetVertexIndex(1), out result2);
        Vector3 result3;
        mCameraMesh.GetVertex(triangle.GetVertexIndex(2), out result3);
        Triangle rkTri = new Triangle(ref result1, ref result2, ref result3);
        float pfSParam;
        float pfTParam;
        float num2 = Distance.PointTriangleDistanceSq(out pfSParam, out pfTParam, ref iPosition, ref rkTri);
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          rkTri.GetPoint(pfSParam, pfTParam, out point);
        }
      }
      closestBoxPoint = point;
    }
    return closestBoxPoint;
  }

  private bool GetCenterPosition(List<Magicka.GameLogic.Entities.Character> iList, out Vector3 oPosition)
  {
    oPosition = new Vector3();
    float num1 = 0.0f;
    int num2 = 0;
    int num3 = 0;
    float num4 = 0.0f;
    for (int index = 0; index < iList.Count; ++index)
    {
      if (iList[index] != null && !iList[index].Dead)
      {
        ++num2;
        Vector3 position = iList[index].Position;
        float result;
        Vector3.Distance(ref oPosition, ref position, out result);
        num4 += result;
        Vector3.Add(ref oPosition, ref position, out oPosition);
        if (!iList[index].CharacterBody.IsLeavingGround && iList[index].CharacterBody.IsTouchingGround)
        {
          ++num3;
          num1 += iList[index].Position.Y - (iList[index].Capsule.Length * 0.5f + iList[index].Capsule.Radius);
        }
      }
    }
    if (num2 <= 0)
      return false;
    Vector3.Divide(ref oPosition, (float) num2, out oPosition);
    if ((double) System.Math.Abs(num1) >= 1.4012984643248171E-45)
      oPosition.Y = num1 / (float) num3;
    return true;
  }

  private bool GetInfluenceVector(out Vector3 oVector)
  {
    oVector = new Vector3();
    StaticList<Entity> entities = this.mPlayState.EntityManager.Entities;
    int num = 0;
    for (int iIndex = 0; iIndex < entities.Count; ++iIndex)
    {
      if (entities[iIndex] is Magicka.GameLogic.Entities.Character character && !character.Dead && (character.Faction & Factions.FRIENDLY) != Factions.FRIENDLY && (!(character is Avatar) || (character as Avatar).Player.Gamer is NetworkGamer))
      {
        ++num;
        Vector3 result = character.Position;
        Vector3.Subtract(ref result, ref this.mCurrentPosition, out result);
        float influencePower = this.GetInfluencePower(result.LengthSquared());
        Vector3.Multiply(ref result, influencePower, out result);
        Vector3.Add(ref oVector, ref result, out oVector);
      }
    }
    if (num <= 0)
      return false;
    Vector3.Divide(ref oVector, (float) num, out oVector);
    return true;
  }

  private float GetInfluencePower(float iSqrDist)
  {
    return (float) System.Math.Pow(0.99774998426437378, (double) iSqrDist);
  }

  public AudioListener Listener => this.mListener;

  public CollisionSkin CollisionSkin => this.mCollisionSkin;

  public bool CollisionEnabled
  {
    get => this.mCollisionEnabled;
    set => this.mCollisionEnabled = value;
  }

  public void Release(float iTime)
  {
    this.Time = iTime;
    this.mCameraBox = (Box) null;
    this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
    this.mInterpolation = CameraInterpolation.Interpolated;
    this.mTargetMagnification = this.mDefaultMagnification;
    this.LockInput = false;
    this.mFollowing = (Entity) null;
  }

  public void Release_NoMagReset(float iTime)
  {
    this.Time = iTime;
    this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
    this.mInterpolation = CameraInterpolation.Interpolated;
    this.LockInput = false;
    this.mFollowing = (Entity) null;
  }

  public void Release(float iTime, bool iReleaseConfines)
  {
    if (!iReleaseConfines)
    {
      this.Time = iTime;
      this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
      this.mInterpolation = CameraInterpolation.Interpolated;
      this.mTargetMagnification = this.mDefaultMagnification;
      this.LockInput = false;
      this.mFollowing = (Entity) null;
    }
    else
      this.Release(iTime);
  }

  public void LockOn(Box iBox, float iTime)
  {
    this.Time = iTime;
    this.mCameraBox = iBox;
  }

  public void AttachPlayers(Magicka.GameLogic.Entities.Character iFollower)
  {
    if (this.mPlayers.Contains(iFollower))
      return;
    this.mPlayers.Add(iFollower);
  }

  public void AttachNetworkPlayers(Magicka.GameLogic.Entities.Character iFollower)
  {
    if (this.mNetworkPlayers.Contains(iFollower))
      return;
    this.mNetworkPlayers.Add(iFollower);
  }

  public void Follow(Entity iTarget)
  {
    this.mFollowing = iTarget;
    this.mCurrentBehaviour = CameraBehaviour.FollowEntity;
  }

  public void CameraShake(Vector3 iPosition, float iMagnitude, float iTTL)
  {
    iPosition.Y = 0.0f;
    for (int index = 0; index < this.mPlayers.Count; ++index)
    {
      Avatar mPlayer = this.mPlayers[index] as Avatar;
      if (!(mPlayer.Player.Gamer is NetworkGamer) && mPlayer.Player.Playing)
      {
        Vector3 position = this.mPlayers[index].Position with
        {
          Y = 0.0f
        };
        float result1;
        Vector3.Distance(ref iPosition, ref position, out result1);
        Magicka.GameLogic.Controls.Controller controller = mPlayer.Player.Controller;
        if (controller != null)
        {
          if ((double) result1 < 1.4012984643248171E-45 && (double) result1 > -1.4012984643248171E-45)
          {
            controller.Rumble(1f, 1f);
          }
          else
          {
            Vector3 result2;
            Vector3.Subtract(ref iPosition, ref position, out result2);
            result2.Normalize();
            result1 = (float) (1.0 - (double) result1 / ((double) iMagnitude * 20.0));
            float num1 = MagickaMath.Angle(Vector3.Right, result2) / 3.14159274f;
            float num2 = MagickaMath.Angle(Vector3.Left, result2) / 3.14159274f;
            float iLeft = result1 * (1f - num2);
            float iRight = result1 * (1f - num1);
            controller.Rumble(iLeft, iRight);
          }
        }
      }
    }
    this.mCameraShakeMagnitude = iMagnitude;
    this.mStartCameraShakeTTL = iTTL;
    this.mCameraShakeTTL = iTTL;
  }

  public void CameraShake(float iMagnitude, float iTTL)
  {
    this.mCameraShakeMagnitude = iMagnitude;
    this.mStartCameraShakeTTL = iTTL;
    this.mCameraShakeTTL = iTTL;
  }

  public float Time
  {
    set => this.mTime = this.mTimeStart = value;
  }

  public CameraInterpolation Interpolation
  {
    set => this.mInterpolation = value;
    get => this.mInterpolation;
  }

  public float Magnification
  {
    get => this.mTargetMagnification;
    set => this.mTargetMagnification = value;
  }

  public void SetPosition(Vector3 iPosition, bool iSnapToMesh)
  {
    if (iSnapToMesh && (this.mCameraMesh != null || this.mCameraBox != null))
      iPosition = this.SnapPosition(iPosition);
    this.mCurrentPosition = iPosition;
  }

  public Vector3 GroundPosition => this.mCurrentPosition;

  public bool LockInput
  {
    get => this.mLockInput;
    set
    {
      this.mLockInput = value;
      if (value)
        ControlManager.Instance.LimitInput((object) this);
      else
        ControlManager.Instance.UnlimitInput((object) this);
    }
  }

  public Primitive SnapPrimitive
  {
    get => this.mCameraMesh;
    set => this.mCameraMesh = value;
  }

  public void MoveTo(Vector3 iTarget, float iMovementTime)
  {
    this.mCurrentBehaviour = CameraBehaviour.MoveToTarget;
    this.mTargetPosition = iTarget;
    this.Time = iMovementTime;
  }

  public void ClearPlayers()
  {
    this.mPlayers.Clear();
    this.mNetworkPlayers.Clear();
  }

  public void RemoveEffect(int iHashCode)
  {
    if (!this.mVisualEffects.ContainsKey(iHashCode))
      return;
    VisualEffectReference mVisualEffect = this.mVisualEffects[iHashCode];
    EffectManager.Instance.Stop(ref mVisualEffect);
    this.mVisualEffects.Remove(iHashCode);
  }

  public void RemoveEffects()
  {
    for (int index = this.mVisualEffects.Count - 1; index >= 0; --index)
    {
      VisualEffectReference iRef = this.mVisualEffects.Values[index];
      EffectManager.Instance.Stop(ref iRef);
      this.mVisualEffects.RemoveAt(index);
    }
  }

  public void StartVisualEffect(int iHashCode)
  {
    Vector3 forward = Vector3.Forward;
    VisualEffectReference oRef;
    EffectManager.Instance.StartEffect(iHashCode, ref this.mCurrentPosition, ref forward, out oRef);
    if (this.mVisualEffects.ContainsKey(iHashCode))
      this.RemoveEffect(iHashCode);
    this.mVisualEffects.Add(iHashCode, oRef);
  }

  public class State
  {
    private MagickCamera mMagickCamera;
    private float mMagnification;
    private float mTargetMagnification;
    private float mTime;
    private CameraBehaviour mCurrentBehaviour;
    private Entity mFollowing;
    private Vector3 mTargetPosition;
    private Vector3 mCurrentPosition;
    private Box mCameraBox;

    public State(MagickCamera iMagickCamera)
    {
      this.mMagickCamera = iMagickCamera;
      this.UpdateState();
    }

    public void UpdateState()
    {
      this.mMagnification = this.mMagickCamera.mMagnification;
      this.mTargetMagnification = this.mMagickCamera.mTargetMagnification;
      this.mTime = this.mMagickCamera.mTime;
      this.mCurrentBehaviour = this.mMagickCamera.mCurrentBehaviour;
      this.mCameraBox = this.mMagickCamera.mCameraBox;
      this.mFollowing = this.mMagickCamera.mFollowing;
      this.mTargetPosition = this.mMagickCamera.mTargetPosition;
      this.mCurrentPosition = this.mMagickCamera.mCurrentPosition;
    }

    public void ApplyState()
    {
      this.mMagickCamera.mMagnification = this.mMagnification;
      this.mMagickCamera.mTargetMagnification = this.mTargetMagnification;
      this.mMagickCamera.mTime = this.mTime;
      this.mMagickCamera.mCurrentBehaviour = this.mCurrentBehaviour;
      this.mMagickCamera.mTargetPosition = this.mTargetPosition;
      this.mMagickCamera.mCameraBox = this.mCameraBox;
      this.mMagickCamera.mPlayers.Clear();
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing)
          this.mMagickCamera.mPlayers.Add((Magicka.GameLogic.Entities.Character) players[index].Avatar);
      }
      this.mMagickCamera.mFollowing = this.mFollowing;
      this.mMagickCamera.mCurrentPosition = this.mCurrentPosition;
    }

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mMagnification);
      iWriter.Write(this.mTargetMagnification);
      iWriter.Write(this.mTime);
      iWriter.Write((byte) this.mCurrentBehaviour);
      iWriter.Write(this.mTargetPosition.X);
      iWriter.Write(this.mTargetPosition.Y);
      iWriter.Write(this.mTargetPosition.Z);
      iWriter.Write(this.mCurrentPosition.X);
      iWriter.Write(this.mCurrentPosition.Y);
      iWriter.Write(this.mCurrentPosition.Z);
    }

    internal void Read(BinaryReader iReader)
    {
      this.mMagnification = iReader.ReadSingle();
      this.mTargetMagnification = iReader.ReadSingle();
      this.mTime = iReader.ReadSingle();
      this.mCurrentBehaviour = (CameraBehaviour) iReader.ReadByte();
      this.mCameraBox = (Box) null;
      this.mFollowing = (Entity) null;
      this.mTargetPosition.X = iReader.ReadSingle();
      this.mTargetPosition.Y = iReader.ReadSingle();
      this.mTargetPosition.Z = iReader.ReadSingle();
      this.mCurrentPosition.X = iReader.ReadSingle();
      this.mCurrentPosition.Y = iReader.ReadSingle();
      this.mCurrentPosition.Z = iReader.ReadSingle();
    }
  }
}
