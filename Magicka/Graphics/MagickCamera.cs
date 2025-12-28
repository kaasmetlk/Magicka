using System;
using System.Collections.Generic;
using System.IO;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.Graphics
{
	// Token: 0x02000149 RID: 329
	public class MagickCamera : Camera
	{
		// Token: 0x06000936 RID: 2358 RVA: 0x00039908 File Offset: 0x00037B08
		public MagickCamera(Vector3 iPosition, Vector3 iDirection, Vector3 iUp, float iFov, float iAspectRation, float iNearPlaneDistance, float iFarPlaneDistance) : base(iPosition, iDirection, iUp, iFov, iAspectRation, iNearPlaneDistance, iFarPlaneDistance)
		{
			this.mListener = new AudioListener();
			this.mPlayers = new List<Magicka.GameLogic.Entities.Character>(8);
			this.mNetworkPlayers = new List<Magicka.GameLogic.Entities.Character>(8);
			this.mPlayerDirections = new Vector3[4];
			this.mCollisionSkin = new CollisionSkin(null);
			this.mCollisionSkin.callbackFn += this.OnCollision;
			this.mCollisionPlanesOld = new JigLibX.Geometry.Plane[4];
			this.mCollisionPlanesNew = new JigLibX.Geometry.Plane[4];
			for (int i = 0; i < 4; i++)
			{
				this.mCollisionSkin.AddPrimitive(new JigLibX.Geometry.Plane(Vector3.Down, new Vector3(0f, 1000f, 0f)), 1, default(MaterialProperties));
				this.mCollisionPlanesOld[i] = (this.mCollisionSkin.GetPrimitiveOldWorld(i) as JigLibX.Geometry.Plane);
				this.mCollisionPlanesNew[i] = (this.mCollisionSkin.GetPrimitiveNewWorld(i) as JigLibX.Geometry.Plane);
			}
			this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
			this.mVisualEffects = new SortedList<int, VisualEffectReference>();
			this.mCollisionSkin.Tag = this;
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x00039AA8 File Offset: 0x00037CA8
		internal CameraBehaviour CurrentBehaviour
		{
			get
			{
				return this.mCurrentBehaviour;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000938 RID: 2360 RVA: 0x00039AB0 File Offset: 0x00037CB0
		public Entity TargetEntity
		{
			get
			{
				return this.mFollowing;
			}
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00039AB8 File Offset: 0x00037CB8
		public void SetPlayState(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00039AC4 File Offset: 0x00037CC4
		public void Init()
		{
			this.mTargetBias = default(Vector3);
			this.mBias = default(Vector3);
			base.Set(MagickCamera.CAMERAOFFSET, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, base.Fov, base.NearPlaneDistance, base.FarPlaneDistance);
			base.Rotate(0f, -MathHelper.ToRadians(40f));
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x00039B2C File Offset: 0x00037D2C
		private bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			CharacterBody characterBody = iSkin1.Owner as CharacterBody;
			if (this.mCollisionEnabled && !ControlManager.Instance.IsInputLimited && characterBody != null)
			{
				Avatar avatar = characterBody.Owner as Avatar;
				if (avatar != null && !avatar.Dead && (!(avatar.Player.Gamer is NetworkGamer) & (avatar.Events == null || avatar.CurrentEvent >= avatar.Events.Length)))
				{
					Vector3 vector;
					Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out vector);
					Vector3 normal = (iSkin0.GetPrimitiveNewWorld(iPrim0) as JigLibX.Geometry.Plane).Normal;
					float num;
					Vector3.Dot(ref normal, ref vector, out num);
					if (num > 0f)
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
					else if (num < -0.1f)
					{
						return false;
					}
					this.mCanZoomIn = false;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00039C4A File Offset: 0x00037E4A
		public void EnableDynamicMagnification(float iMinMagnification, float iMaxMagnification)
		{
			this.mDynamicMagnify = true;
			this.mMinMagnification = iMinMagnification;
			this.mMaxMagnification = iMaxMagnification;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00039C61 File Offset: 0x00037E61
		public void DisableDynamicMagnification()
		{
			this.mDynamicMagnify = false;
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x00039C6A File Offset: 0x00037E6A
		// (set) Token: 0x0600093F RID: 2367 RVA: 0x00039C72 File Offset: 0x00037E72
		public float DefaultMagnification
		{
			get
			{
				return this.mDefaultMagnification;
			}
			set
			{
				if (Math.Abs(this.mTargetMagnification - this.mDefaultMagnification) < 0.0001f)
				{
					this.mTargetMagnification = value;
				}
				this.mDefaultMagnification = value;
			}
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00039C9B File Offset: 0x00037E9B
		public void SetPlayerMagnification(float iMagnification, float iTTL)
		{
			this.mPlayerMagnification = iMagnification;
			this.mPlayerMagnificationTTL = iTTL;
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00039CAB File Offset: 0x00037EAB
		public void SetBias(ref Vector3 iBias, float iTime)
		{
			this.mTargetBias = iBias;
			this.mBiasTime = iTime;
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x00039CC0 File Offset: 0x00037EC0
		public override void SetPosition(ref Vector3 iPosition)
		{
			this.mCurrentPosition = iPosition;
			this.mTargetPosition = iPosition;
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00039CDC File Offset: 0x00037EDC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			for (int i = 0; i < this.mPlayers.Count; i++)
			{
				if (this.mPlayers[i].Dead)
				{
					if (float.IsNaN(this.mPlayers[i].UndyingTimer))
					{
						this.mPlayers.RemoveAt(i);
						i--;
					}
					else if (!this.mPlayers[i].Undying && this.mPlayers[i].UndyingTimer < 0f)
					{
						this.mPlayers.RemoveAt(i);
						i--;
					}
				}
			}
			for (int j = 0; j < this.mNetworkPlayers.Count; j++)
			{
				if (this.mNetworkPlayers[j].Dead)
				{
					if (float.IsNaN(this.mNetworkPlayers[j].UndyingTimer))
					{
						this.mNetworkPlayers.RemoveAt(j);
						j--;
					}
					else if (!this.mNetworkPlayers[j].Undying && this.mNetworkPlayers[j].UndyingTimer < 0f)
					{
						this.mNetworkPlayers.RemoveAt(j);
						j--;
					}
				}
			}
			switch (this.mCurrentBehaviour)
			{
			case CameraBehaviour.FollowPlayers:
			{
				Vector3 vector;
				bool centerPosition = this.GetCenterPosition(this.mPlayers, out vector);
				if (!centerPosition)
				{
					centerPosition = this.GetCenterPosition(this.mNetworkPlayers, out vector);
				}
				if (centerPosition)
				{
					Vector3 vector2;
					this.GetInfluenceVector(out vector2);
					Vector3.Add(ref vector, ref vector2, out this.mTargetPosition);
					Vector3.Add(ref this.mTargetPosition, ref this.mBias, out this.mTargetPosition);
				}
				else
				{
					this.mTargetPosition = this.mCurrentPosition;
				}
				this.mTime += (1f - this.mTime) * iDeltaTime;
				if (this.mCameraMesh != null || this.mCameraBox != null)
				{
					this.mTargetPosition = this.SnapPosition(this.mTargetPosition);
				}
				break;
			}
			case CameraBehaviour.FollowEntity:
				if (this.mFollowing == null)
				{
					this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
				}
				else
				{
					this.mTargetPosition = this.mFollowing.Position;
					if (this.mCameraMesh != null || this.mCameraBox != null)
					{
						this.mTargetPosition = this.SnapPosition(this.mTargetPosition);
					}
				}
				break;
			}
			Vector3.Lerp(ref this.mTargetBias, ref this.mBias, (float)Math.Pow(0.02, (double)(iDeltaTime / this.mBiasTime)), out this.mBias);
			Vector3 vector3 = default(Vector3);
			float num3;
			if (this.mTime > 0f)
			{
				switch (this.mInterpolation)
				{
				case CameraInterpolation.Linear:
				{
					Vector3 value;
					Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out value);
					float num = value.Length();
					float num2 = 100f * iDeltaTime / this.mTime;
					if (num2 < num)
					{
						Vector3.Multiply(ref value, num2 / num, out value);
						vector3 = this.mCurrentPosition + value;
					}
					else
					{
						vector3 = this.mTargetPosition;
					}
					break;
				}
				case CameraInterpolation.Interpolated:
					if (iDeltaTime <= 1E-45f)
					{
						vector3 = this.mCurrentPosition;
					}
					else
					{
						Vector3 vector4;
						Vector3.Subtract(ref this.mTargetPosition, ref this.mCurrentPosition, out vector4);
						Vector3 vector5;
						Vector3.Divide(ref this.mVelocity, iDeltaTime, out vector5);
						Vector3 vector6;
						Vector3.Multiply(ref vector4, 2f / this.mTime, out vector6);
						Vector3 vector7;
						Vector3.Multiply(ref vector5, 0.5f, out vector7);
						Vector3.Subtract(ref vector6, ref vector7, out vector6);
						Vector3.Add(ref vector5, ref vector6, out vector5);
						Vector3.Multiply(ref vector5, iDeltaTime, out vector5);
						if (vector5.LengthSquared() > vector4.LengthSquared())
						{
							vector3 = this.mTargetPosition;
						}
						else
						{
							Vector3.Add(ref this.mCurrentPosition, ref vector5, out vector3);
						}
					}
					break;
				}
				num3 = this.mTargetMagnification;
				if (this.mCurrentBehaviour == CameraBehaviour.FollowPlayers && this.mPlayerMagnificationTTL > 0f)
				{
					num3 *= this.mPlayerMagnification;
				}
				num3 = MathHelper.Lerp(num3, this.mMagnification, (float)Math.Pow(0.02, (double)(iDeltaTime / this.mTime)));
			}
			else
			{
				num3 = this.mTargetMagnification;
				vector3 = this.mTargetPosition;
			}
			this.mPlayerMagnificationTTL -= iDeltaTime;
			Vector3.Subtract(ref vector3, ref this.mCurrentPosition, out this.mVelocity);
			if (!this.mCanMoveUp)
			{
				Vector3 normal = this.mCollisionPlanesNew[0].Normal;
				float num4;
				Vector3.Dot(ref normal, ref this.mVelocity, out num4);
				Vector3.Multiply(ref normal, -num4, out normal);
				Vector3.Add(ref normal, ref this.mVelocity, out this.mVelocity);
			}
			if (!this.mCanMoveRight)
			{
				Vector3 normal2 = this.mCollisionPlanesNew[1].Normal;
				float num5;
				Vector3.Dot(ref normal2, ref this.mVelocity, out num5);
				Vector3.Multiply(ref normal2, -num5, out normal2);
				Vector3.Add(ref normal2, ref this.mVelocity, out this.mVelocity);
			}
			if (!this.mCanMoveDown)
			{
				Vector3 normal3 = this.mCollisionPlanesNew[2].Normal;
				float num6;
				Vector3.Dot(ref normal3, ref this.mVelocity, out num6);
				Vector3.Multiply(ref normal3, -num6, out normal3);
				Vector3.Add(ref normal3, ref this.mVelocity, out this.mVelocity);
			}
			if (!this.mCanMoveLeft)
			{
				Vector3 normal4 = this.mCollisionPlanesNew[3].Normal;
				float num7;
				Vector3.Dot(ref normal4, ref this.mVelocity, out num7);
				Vector3.Multiply(ref normal4, -num7, out normal4);
				Vector3.Add(ref normal4, ref this.mVelocity, out this.mVelocity);
			}
			Vector3.Add(ref this.mVelocity, ref this.mCurrentPosition, out this.mCurrentPosition);
			if (this.mDynamicMagnify && this.mCurrentBehaviour == CameraBehaviour.FollowPlayers)
			{
				Vector3 position = base.Position;
				Vector3 direction = base.Direction;
				float num8 = 0f;
				for (int k = 0; k < this.mPlayers.Count; k++)
				{
					Vector3 position2 = this.mPlayers[k].Position;
					Vector3.Subtract(ref position2, ref position, out this.mPlayerDirections[k]);
					this.mPlayerDirections[k].Normalize();
					float num9;
					Vector3.Dot(ref direction, ref this.mPlayerDirections[k], out num9);
					num9 = (float)Math.Acos((double)MathHelper.Clamp(num9, -1f, 1f));
					if (num9 >= num8)
					{
						num8 = num9;
					}
				}
				this.mTargetMagnification = MathHelper.Clamp(0.5f * MagickCamera.DEFAULTFOV / num8, this.mMinMagnification, this.mMaxMagnification);
			}
			float num10 = num3 - this.mMagnification;
			if (this.mCanZoomIn || num10 < 0f)
			{
				this.mMagnification += num10;
			}
			base.Position = this.mCurrentPosition + MagickCamera.CAMERAOFFSET;
			base.Fov = MagickCamera.DEFAULTFOV / this.mMagnification;
			if (this.mListener != null)
			{
				Vector3 groundPosition = this.GroundPosition;
				Vector3 vector8 = new Vector3(0f, 20f, 0f);
				Vector3.Add(ref vector8, ref groundPosition, out groundPosition);
				this.mListener.Position = groundPosition;
				this.mListener.Forward = Vector3.Down;
				this.mListener.Up = Vector3.Forward;
			}
			base.Update(iDataChannel, iDeltaTime);
			this.mViewFrustum[(int)iDataChannel].GetCorners(this.mFrustumCorners);
			for (int l = 0; l < 4; l++)
			{
				JigLibX.Geometry.Plane plane = this.mCollisionPlanesOld[l];
				JigLibX.Geometry.Plane plane2 = this.mCollisionPlanesNew[l];
				plane.D = plane2.D;
				plane.Normal = plane2.Normal;
				Vector3 pos = this.mFrustumCorners[l];
				Vector3 vector9 = this.mFrustumCorners[(l + 1) % 4];
				Vector3 vector10 = this.mFrustumCorners[l + 4];
				Vector3.Subtract(ref vector9, ref pos, out vector9);
				Vector3.Subtract(ref vector10, ref pos, out vector10);
				Vector3 n;
				Vector3.Cross(ref vector10, ref vector9, out n);
				n.Normalize();
				Vector3 vector11;
				Vector3.Multiply(ref n, 0.5f, out vector11);
				Vector3.Subtract(ref pos, ref vector11, out pos);
				plane2.SetPositionNormal(n, pos);
			}
			this.mCanMoveUp = true;
			this.mCanMoveRight = true;
			this.mCanMoveDown = true;
			this.mCanMoveLeft = true;
			this.mCanZoomIn = true;
			Vector3 forward = Vector3.Forward;
			for (int m = 0; m < this.mVisualEffects.Count; m++)
			{
				VisualEffectReference value2 = this.mVisualEffects.Values[m];
				EffectManager.Instance.UpdatePositionDirection(ref value2, ref this.mCurrentPosition, ref forward);
				if (value2.ID < 0)
				{
					this.mVisualEffects.RemoveAt(m--);
				}
				else
				{
					this.mVisualEffects[this.mVisualEffects.Keys[m]] = value2;
				}
			}
			if (this.mCameraShakeTTL > 0f && iDeltaTime > 1E-45f)
			{
				float num11;
				MathApproximation.FastSin(MathHelper.WrapAngle(this.mCameraShakeTTL * 60f), out num11);
				this.mCurrentPosition.Y = this.mCurrentPosition.Y + num11 * this.mCameraShakeMagnitude * (this.mCameraShakeTTL / this.mStartCameraShakeTTL) * 0.0333f;
			}
			this.mCameraShakeTTL -= iDeltaTime;
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0003A5D0 File Offset: 0x000387D0
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

		// Token: 0x06000945 RID: 2373 RVA: 0x0003A600 File Offset: 0x00038800
		private Vector3 SnapPosition(Vector3 iPosition)
		{
			Vector3 result = iPosition;
			if (this.mCameraBox != null)
			{
				this.mCameraBox.GetDistanceToPoint(out result, iPosition);
			}
			else if (this.mCameraMesh is TriangleMesh)
			{
				TriangleMesh triangleMesh = this.mCameraMesh as TriangleMesh;
				float num = float.MaxValue;
				Vector3 vector = default(Vector3);
				for (int i = 0; i < triangleMesh.GetNumTriangles(); i++)
				{
					IndexedTriangle triangle = triangleMesh.GetTriangle(i);
					Vector3 vector2;
					triangleMesh.GetVertex(triangle.GetVertexIndex(0), out vector2);
					Vector3 vector3;
					triangleMesh.GetVertex(triangle.GetVertexIndex(1), out vector3);
					Vector3 vector4;
					triangleMesh.GetVertex(triangle.GetVertexIndex(2), out vector4);
					Triangle triangle2 = new Triangle(ref vector2, ref vector3, ref vector4);
					float t;
					float t2;
					float num2 = Distance.PointTriangleDistanceSq(out t, out t2, ref iPosition, ref triangle2);
					if (num2 < num)
					{
						num = num2;
						triangle2.GetPoint(t, t2, out vector);
					}
				}
				result = vector;
			}
			return result;
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0003A6E0 File Offset: 0x000388E0
		private bool GetCenterPosition(List<Magicka.GameLogic.Entities.Character> iList, out Vector3 oPosition)
		{
			oPosition = default(Vector3);
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			float num4 = 0f;
			for (int i = 0; i < iList.Count; i++)
			{
				if (iList[i] != null && !iList[i].Dead)
				{
					num2++;
					Vector3 position = iList[i].Position;
					float num5;
					Vector3.Distance(ref oPosition, ref position, out num5);
					num4 += num5;
					Vector3.Add(ref oPosition, ref position, out oPosition);
					if (!iList[i].CharacterBody.IsLeavingGround && iList[i].CharacterBody.IsTouchingGround)
					{
						num3++;
						num += iList[i].Position.Y - (iList[i].Capsule.Length * 0.5f + iList[i].Capsule.Radius);
					}
				}
			}
			if (num2 > 0)
			{
				Vector3.Divide(ref oPosition, (float)num2, out oPosition);
				if (Math.Abs(num) >= 1E-45f)
				{
					oPosition.Y = num / (float)num3;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0003A800 File Offset: 0x00038A00
		private bool GetInfluenceVector(out Vector3 oVector)
		{
			oVector = default(Vector3);
			StaticList<Entity> entities = this.mPlayState.EntityManager.Entities;
			int num = 0;
			for (int i = 0; i < entities.Count; i++)
			{
				Magicka.GameLogic.Entities.Character character = entities[i] as Magicka.GameLogic.Entities.Character;
				if (character != null && !character.Dead && (character.Faction & Factions.FRIENDLY) != Factions.FRIENDLY && (!(character is Avatar) || (character as Avatar).Player.Gamer is NetworkGamer))
				{
					num++;
					Vector3 position = character.Position;
					Vector3.Subtract(ref position, ref this.mCurrentPosition, out position);
					float iSqrDist = position.LengthSquared();
					float influencePower = this.GetInfluencePower(iSqrDist);
					Vector3.Multiply(ref position, influencePower, out position);
					Vector3.Add(ref oVector, ref position, out oVector);
				}
			}
			if (num > 0)
			{
				Vector3.Divide(ref oVector, (float)num, out oVector);
				return true;
			}
			return false;
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0003A8D2 File Offset: 0x00038AD2
		private float GetInfluencePower(float iSqrDist)
		{
			return (float)Math.Pow(0.9977499842643738, (double)iSqrDist);
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000949 RID: 2377 RVA: 0x0003A8E5 File Offset: 0x00038AE5
		public AudioListener Listener
		{
			get
			{
				return this.mListener;
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x0003A8ED File Offset: 0x00038AED
		public CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollisionSkin;
			}
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x0600094B RID: 2379 RVA: 0x0003A8F5 File Offset: 0x00038AF5
		// (set) Token: 0x0600094C RID: 2380 RVA: 0x0003A8FD File Offset: 0x00038AFD
		public bool CollisionEnabled
		{
			get
			{
				return this.mCollisionEnabled;
			}
			set
			{
				this.mCollisionEnabled = value;
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0003A906 File Offset: 0x00038B06
		public void Release(float iTime)
		{
			this.Time = iTime;
			this.mCameraBox = null;
			this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
			this.mInterpolation = CameraInterpolation.Interpolated;
			this.mTargetMagnification = this.mDefaultMagnification;
			this.LockInput = false;
			this.mFollowing = null;
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0003A93E File Offset: 0x00038B3E
		public void Release_NoMagReset(float iTime)
		{
			this.Time = iTime;
			this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
			this.mInterpolation = CameraInterpolation.Interpolated;
			this.LockInput = false;
			this.mFollowing = null;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0003A963 File Offset: 0x00038B63
		public void Release(float iTime, bool iReleaseConfines)
		{
			if (!iReleaseConfines)
			{
				this.Time = iTime;
				this.mCurrentBehaviour = CameraBehaviour.FollowPlayers;
				this.mInterpolation = CameraInterpolation.Interpolated;
				this.mTargetMagnification = this.mDefaultMagnification;
				this.LockInput = false;
				this.mFollowing = null;
				return;
			}
			this.Release(iTime);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0003A99F File Offset: 0x00038B9F
		public void LockOn(Box iBox, float iTime)
		{
			this.Time = iTime;
			this.mCameraBox = iBox;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0003A9AF File Offset: 0x00038BAF
		public void AttachPlayers(Magicka.GameLogic.Entities.Character iFollower)
		{
			if (!this.mPlayers.Contains(iFollower))
			{
				this.mPlayers.Add(iFollower);
			}
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0003A9CB File Offset: 0x00038BCB
		public void AttachNetworkPlayers(Magicka.GameLogic.Entities.Character iFollower)
		{
			if (!this.mNetworkPlayers.Contains(iFollower))
			{
				this.mNetworkPlayers.Add(iFollower);
			}
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0003A9E7 File Offset: 0x00038BE7
		public void Follow(Entity iTarget)
		{
			this.mFollowing = iTarget;
			this.mCurrentBehaviour = CameraBehaviour.FollowEntity;
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0003A9F8 File Offset: 0x00038BF8
		public void CameraShake(Vector3 iPosition, float iMagnitude, float iTTL)
		{
			iPosition.Y = 0f;
			for (int i = 0; i < this.mPlayers.Count; i++)
			{
				Avatar avatar = this.mPlayers[i] as Avatar;
				if (!(avatar.Player.Gamer is NetworkGamer) && avatar.Player.Playing)
				{
					Vector3 position = this.mPlayers[i].Position;
					position.Y = 0f;
					float num;
					Vector3.Distance(ref iPosition, ref position, out num);
					Controller controller = avatar.Player.Controller;
					if (controller != null)
					{
						if (num < 1E-45f && num > -1E-45f)
						{
							controller.Rumble(1f, 1f);
						}
						else
						{
							Vector3 secondVector;
							Vector3.Subtract(ref iPosition, ref position, out secondVector);
							secondVector.Normalize();
							num = 1f - num / (iMagnitude * 20f);
							float num2 = MagickaMath.Angle(Vector3.Right, secondVector) / 3.1415927f;
							float num3 = MagickaMath.Angle(Vector3.Left, secondVector) / 3.1415927f;
							float iLeft = num * (1f - num3);
							float iRight = num * (1f - num2);
							controller.Rumble(iLeft, iRight);
						}
					}
				}
			}
			this.mCameraShakeMagnitude = iMagnitude;
			this.mStartCameraShakeTTL = iTTL;
			this.mCameraShakeTTL = iTTL;
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0003AB4C File Offset: 0x00038D4C
		public void CameraShake(float iMagnitude, float iTTL)
		{
			this.mCameraShakeMagnitude = iMagnitude;
			this.mStartCameraShakeTTL = iTTL;
			this.mCameraShakeTTL = iTTL;
		}

		// Token: 0x170001E5 RID: 485
		// (set) Token: 0x06000956 RID: 2390 RVA: 0x0003AB64 File Offset: 0x00038D64
		public float Time
		{
			set
			{
				this.mTimeStart = value;
				this.mTime = value;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000958 RID: 2392 RVA: 0x0003AB8A File Offset: 0x00038D8A
		// (set) Token: 0x06000957 RID: 2391 RVA: 0x0003AB81 File Offset: 0x00038D81
		public CameraInterpolation Interpolation
		{
			get
			{
				return this.mInterpolation;
			}
			set
			{
				this.mInterpolation = value;
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x0003AB92 File Offset: 0x00038D92
		// (set) Token: 0x0600095A RID: 2394 RVA: 0x0003AB9A File Offset: 0x00038D9A
		public float Magnification
		{
			get
			{
				return this.mTargetMagnification;
			}
			set
			{
				this.mTargetMagnification = value;
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0003ABA3 File Offset: 0x00038DA3
		public void SetPosition(Vector3 iPosition, bool iSnapToMesh)
		{
			if (iSnapToMesh && (this.mCameraMesh != null || this.mCameraBox != null))
			{
				iPosition = this.SnapPosition(iPosition);
			}
			this.mCurrentPosition = iPosition;
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x0003ABC8 File Offset: 0x00038DC8
		public Vector3 GroundPosition
		{
			get
			{
				return this.mCurrentPosition;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x0003ABD0 File Offset: 0x00038DD0
		// (set) Token: 0x0600095E RID: 2398 RVA: 0x0003ABD8 File Offset: 0x00038DD8
		public bool LockInput
		{
			get
			{
				return this.mLockInput;
			}
			set
			{
				this.mLockInput = value;
				if (value)
				{
					ControlManager.Instance.LimitInput(this);
					return;
				}
				ControlManager.Instance.UnlimitInput(this);
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0003ABFB File Offset: 0x00038DFB
		// (set) Token: 0x06000960 RID: 2400 RVA: 0x0003AC03 File Offset: 0x00038E03
		public Primitive SnapPrimitive
		{
			get
			{
				return this.mCameraMesh;
			}
			set
			{
				this.mCameraMesh = value;
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0003AC0C File Offset: 0x00038E0C
		public void MoveTo(Vector3 iTarget, float iMovementTime)
		{
			this.mCurrentBehaviour = CameraBehaviour.MoveToTarget;
			this.mTargetPosition = iTarget;
			this.Time = iMovementTime;
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0003AC23 File Offset: 0x00038E23
		public void ClearPlayers()
		{
			this.mPlayers.Clear();
			this.mNetworkPlayers.Clear();
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0003AC3C File Offset: 0x00038E3C
		public void RemoveEffect(int iHashCode)
		{
			if (this.mVisualEffects.ContainsKey(iHashCode))
			{
				VisualEffectReference visualEffectReference = this.mVisualEffects[iHashCode];
				EffectManager.Instance.Stop(ref visualEffectReference);
				this.mVisualEffects.Remove(iHashCode);
			}
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0003AC80 File Offset: 0x00038E80
		public void RemoveEffects()
		{
			for (int i = this.mVisualEffects.Count - 1; i >= 0; i--)
			{
				VisualEffectReference visualEffectReference = this.mVisualEffects.Values[i];
				EffectManager.Instance.Stop(ref visualEffectReference);
				this.mVisualEffects.RemoveAt(i);
			}
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0003ACD0 File Offset: 0x00038ED0
		public void StartVisualEffect(int iHashCode)
		{
			Vector3 forward = Vector3.Forward;
			VisualEffectReference value;
			EffectManager.Instance.StartEffect(iHashCode, ref this.mCurrentPosition, ref forward, out value);
			if (this.mVisualEffects.ContainsKey(iHashCode))
			{
				this.RemoveEffect(iHashCode);
			}
			this.mVisualEffects.Add(iHashCode, value);
		}

		// Token: 0x04000887 RID: 2183
		public static readonly float NEARCLIP = 105f;

		// Token: 0x04000888 RID: 2184
		public static readonly float FARCLIP = 500f;

		// Token: 0x04000889 RID: 2185
		public static readonly Vector3 CAMERAOFFSET = new Vector3(0f, 144f, 171f);

		// Token: 0x0400088A RID: 2186
		public static readonly float DEFAULTFOV = MathHelper.ToRadians(5f);

		// Token: 0x0400088B RID: 2187
		public static readonly float RATIO_16_9 = 1.7777778f;

		// Token: 0x0400088C RID: 2188
		public static readonly float RATIO_4_3 = 1.3333334f;

		// Token: 0x0400088D RID: 2189
		private float mDefaultMagnification = 1f;

		// Token: 0x0400088E RID: 2190
		private AudioListener mListener;

		// Token: 0x0400088F RID: 2191
		private float mMagnification = 1f;

		// Token: 0x04000890 RID: 2192
		private float mTargetMagnification = 1f;

		// Token: 0x04000891 RID: 2193
		private float mPlayerMagnification = 1f;

		// Token: 0x04000892 RID: 2194
		private float mPlayerMagnificationTTL;

		// Token: 0x04000893 RID: 2195
		private float mTime = 1f;

		// Token: 0x04000894 RID: 2196
		private float mBiasTime = 10f;

		// Token: 0x04000895 RID: 2197
		private float mTimeStart;

		// Token: 0x04000896 RID: 2198
		private float mCameraShakeMagnitude;

		// Token: 0x04000897 RID: 2199
		private float mStartCameraShakeTTL;

		// Token: 0x04000898 RID: 2200
		private float mCameraShakeTTL;

		// Token: 0x04000899 RID: 2201
		private CameraInterpolation mInterpolation = CameraInterpolation.Interpolated;

		// Token: 0x0400089A RID: 2202
		private CameraBehaviour mCurrentBehaviour;

		// Token: 0x0400089B RID: 2203
		private List<Magicka.GameLogic.Entities.Character> mPlayers;

		// Token: 0x0400089C RID: 2204
		private List<Magicka.GameLogic.Entities.Character> mNetworkPlayers;

		// Token: 0x0400089D RID: 2205
		private Vector3[] mPlayerDirections;

		// Token: 0x0400089E RID: 2206
		private Entity mFollowing;

		// Token: 0x0400089F RID: 2207
		private Vector3 mVelocity;

		// Token: 0x040008A0 RID: 2208
		private Vector3 mTargetPosition;

		// Token: 0x040008A1 RID: 2209
		private Vector3 mCurrentPosition;

		// Token: 0x040008A2 RID: 2210
		private JigLibX.Geometry.Plane[] mCollisionPlanesOld;

		// Token: 0x040008A3 RID: 2211
		private JigLibX.Geometry.Plane[] mCollisionPlanesNew;

		// Token: 0x040008A4 RID: 2212
		private Primitive mCameraMesh;

		// Token: 0x040008A5 RID: 2213
		private Box mCameraBox;

		// Token: 0x040008A6 RID: 2214
		private CollisionSkin mCollisionSkin;

		// Token: 0x040008A7 RID: 2215
		private bool mCollisionEnabled = true;

		// Token: 0x040008A8 RID: 2216
		private Vector3[] mFrustumCorners = new Vector3[8];

		// Token: 0x040008A9 RID: 2217
		private bool mCanMoveUp = true;

		// Token: 0x040008AA RID: 2218
		private bool mCanMoveRight = true;

		// Token: 0x040008AB RID: 2219
		private bool mCanMoveDown = true;

		// Token: 0x040008AC RID: 2220
		private bool mCanMoveLeft = true;

		// Token: 0x040008AD RID: 2221
		private bool mCanZoomIn = true;

		// Token: 0x040008AE RID: 2222
		private bool mLockInput;

		// Token: 0x040008AF RID: 2223
		private Vector3 mBias;

		// Token: 0x040008B0 RID: 2224
		private Vector3 mTargetBias;

		// Token: 0x040008B1 RID: 2225
		private bool mDynamicMagnify;

		// Token: 0x040008B2 RID: 2226
		private float mMinMagnification;

		// Token: 0x040008B3 RID: 2227
		private float mMaxMagnification;

		// Token: 0x040008B4 RID: 2228
		private SortedList<int, VisualEffectReference> mVisualEffects;

		// Token: 0x040008B5 RID: 2229
		private PlayState mPlayState;

		// Token: 0x0200014A RID: 330
		public class State
		{
			// Token: 0x06000967 RID: 2407 RVA: 0x0003AD79 File Offset: 0x00038F79
			public State(MagickCamera iMagickCamera)
			{
				this.mMagickCamera = iMagickCamera;
				this.UpdateState();
			}

			// Token: 0x06000968 RID: 2408 RVA: 0x0003AD90 File Offset: 0x00038F90
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

			// Token: 0x06000969 RID: 2409 RVA: 0x0003AE28 File Offset: 0x00039028
			public void ApplyState()
			{
				this.mMagickCamera.mMagnification = this.mMagnification;
				this.mMagickCamera.mTargetMagnification = this.mTargetMagnification;
				this.mMagickCamera.mTime = this.mTime;
				this.mMagickCamera.mCurrentBehaviour = this.mCurrentBehaviour;
				this.mMagickCamera.mTargetPosition = this.mTargetPosition;
				this.mMagickCamera.mCameraBox = this.mCameraBox;
				this.mMagickCamera.mPlayers.Clear();
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing)
					{
						this.mMagickCamera.mPlayers.Add(players[i].Avatar);
					}
				}
				this.mMagickCamera.mFollowing = this.mFollowing;
				this.mMagickCamera.mCurrentPosition = this.mCurrentPosition;
			}

			// Token: 0x0600096A RID: 2410 RVA: 0x0003AF08 File Offset: 0x00039108
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mMagnification);
				iWriter.Write(this.mTargetMagnification);
				iWriter.Write(this.mTime);
				iWriter.Write((byte)this.mCurrentBehaviour);
				iWriter.Write(this.mTargetPosition.X);
				iWriter.Write(this.mTargetPosition.Y);
				iWriter.Write(this.mTargetPosition.Z);
				iWriter.Write(this.mCurrentPosition.X);
				iWriter.Write(this.mCurrentPosition.Y);
				iWriter.Write(this.mCurrentPosition.Z);
			}

			// Token: 0x0600096B RID: 2411 RVA: 0x0003AFAC File Offset: 0x000391AC
			internal void Read(BinaryReader iReader)
			{
				this.mMagnification = iReader.ReadSingle();
				this.mTargetMagnification = iReader.ReadSingle();
				this.mTime = iReader.ReadSingle();
				this.mCurrentBehaviour = (CameraBehaviour)iReader.ReadByte();
				this.mCameraBox = null;
				this.mFollowing = null;
				this.mTargetPosition.X = iReader.ReadSingle();
				this.mTargetPosition.Y = iReader.ReadSingle();
				this.mTargetPosition.Z = iReader.ReadSingle();
				this.mCurrentPosition.X = iReader.ReadSingle();
				this.mCurrentPosition.Y = iReader.ReadSingle();
				this.mCurrentPosition.Z = iReader.ReadSingle();
			}

			// Token: 0x040008B6 RID: 2230
			private MagickCamera mMagickCamera;

			// Token: 0x040008B7 RID: 2231
			private float mMagnification;

			// Token: 0x040008B8 RID: 2232
			private float mTargetMagnification;

			// Token: 0x040008B9 RID: 2233
			private float mTime;

			// Token: 0x040008BA RID: 2234
			private CameraBehaviour mCurrentBehaviour;

			// Token: 0x040008BB RID: 2235
			private Entity mFollowing;

			// Token: 0x040008BC RID: 2236
			private Vector3 mTargetPosition;

			// Token: 0x040008BD RID: 2237
			private Vector3 mCurrentPosition;

			// Token: 0x040008BE RID: 2238
			private Box mCameraBox;
		}
	}
}
