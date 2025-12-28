using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x02000034 RID: 52
	public abstract class Entity
	{
		// Token: 0x060001F3 RID: 499 RVA: 0x0000DFD0 File Offset: 0x0000C1D0
		public static Entity GetByID(int iID)
		{
			Entity result = null;
			if (Entity.mUniqueEntities.TryGetValue(iID, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000DFF1 File Offset: 0x0000C1F1
		static Entity()
		{
			Entity.mInstances = new List<Entity>(512);
			Entity.mProtectedInstances = new ReadOnlyCollection<Entity>(Entity.mInstances);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000E020 File Offset: 0x0000C220
		protected Entity(PlayState iPlayState)
		{
			this.mAudioEmitter = new AudioEmitter();
			lock (Entity.mInstances)
			{
				this.mHandle = (ushort)Entity.mInstances.Count;
				Entity.mInstances.Add(this);
			}
			this.mPlayState = iPlayState;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000E098 File Offset: 0x0000C298
		public static Entity GetFromHandle(int iHandle)
		{
			if (iHandle >= Entity.mInstances.Count)
			{
				return null;
			}
			return Entity.mInstances[iHandle];
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000E0B4 File Offset: 0x0000C2B4
		protected void Initialize(int iUniqueID)
		{
			this.mInBoundUDPStamp.Clear();
			this.mOutBoundUDPStamp = -1;
			this.mUniqueID = iUniqueID;
			if (iUniqueID != 0)
			{
				Entity.mUniqueEntities[iUniqueID] = this;
			}
			this.mDead = false;
			this.mBody.EnableBody();
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000E0F0 File Offset: 0x0000C2F0
		public void SetUniqueID(int iUniqueID)
		{
			this.mUniqueID = iUniqueID;
			Entity.mUniqueEntities[iUniqueID] = this;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000E108 File Offset: 0x0000C308
		protected void Initialize()
		{
			this.mInBoundUDPStamp.Clear();
			this.mOutBoundUDPStamp = -1;
			this.AudioEmitter.Position = this.mBody.Position;
			this.AudioEmitter.Forward = this.mBody.Orientation.Forward;
			this.AudioEmitter.Up = Vector3.Up;
			this.mUniqueID = 0;
			this.mDead = false;
			this.mBody.EnableBody();
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000E184 File Offset: 0x0000C384
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000E18C File Offset: 0x0000C38C
		public int OutboundUDPStamp
		{
			get
			{
				return this.mOutBoundUDPStamp;
			}
			set
			{
				this.mOutBoundUDPStamp = value;
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000E198 File Offset: 0x0000C398
		public virtual void Deinitialize()
		{
			Entity entity;
			if (Entity.mUniqueEntities.TryGetValue(this.mUniqueID, out entity) && entity == this)
			{
				Entity.mUniqueEntities.Remove(this.mUniqueID);
			}
			this.mBody.DisableBody();
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000E1DC File Offset: 0x0000C3DC
		public virtual Matrix GetOrientation()
		{
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = this.mBody.Position;
			return orientation;
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000E208 File Offset: 0x0000C408
		public static ReadOnlyCollection<Entity> AllEntities
		{
			get
			{
				return Entity.mProtectedInstances;
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000E210 File Offset: 0x0000C410
		protected Vector3 SetMass(float mass)
		{
			PrimitiveProperties primitiveProperties = new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Mass, mass);
			float mass2;
			Vector3 result;
			Matrix matrix;
			Matrix bodyInertia;
			this.mCollision.GetMassProperties(primitiveProperties, out mass2, out result, out matrix, out bodyInertia);
			this.mBody.BodyInertia = bodyInertia;
			this.mBody.Mass = mass2;
			return result;
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000E255 File Offset: 0x0000C455
		public int UniqueID
		{
			get
			{
				return this.mUniqueID;
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000E260 File Offset: 0x0000C460
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			Matrix orientation = this.GetOrientation();
			this.mAudioEmitter.Position = orientation.Translation;
			this.mAudioEmitter.Forward = orientation.Forward;
			this.mAudioEmitter.Up = orientation.Up;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000E2AC File Offset: 0x0000C4AC
		public virtual Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			iDistance *= Math.Max(0f, (float)Math.Sqrt(1.0 - ((double)this.Body.Mass - (double)iMassPower * 0.25) / (double)iMassPower) * 2f);
			if (float.IsNaN(iDistance) || iDistance < 1E-06f)
			{
				return default(Vector3);
			}
			iDirection.Y = 0f;
			iDirection.Normalize();
			if (float.IsNaN(iDirection.X))
			{
				return default(Vector3);
			}
			Vector3 vector;
			Vector3.Multiply(ref iDirection, iDistance, out vector);
			Vector3 position = this.Position;
			vector.Y = (position.Y = 0f);
			Vector3.Add(ref vector, ref position, out vector);
			float num;
			Vector3.Distance(ref vector, ref position, out num);
			float num2 = iDirection.Y = (float)Math.Sin((double)iElevation);
			float num3 = (float)Math.Cos((double)iElevation);
			iDirection.X *= num3;
			iDirection.Z *= num3;
			float scaleFactor = 1.4f * (float)Math.Sqrt((double)(PhysicsManager.Instance.Simulator.Gravity.Y * -1f * num * num / (2f * (num * num2 / num3) * num3 * num3)));
			Vector3.Multiply(ref iDirection, scaleFactor, out iDirection);
			return iDirection;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000E414 File Offset: 0x0000C614
		public bool AddImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			Vector3 vector = this.CalcImpulseVelocity(iDirection, iElevation, iMassPower, iDistance);
			bool flag = vector.LengthSquared() > 1E-06f;
			if (flag)
			{
				this.AddImpulseVelocity(ref vector);
			}
			return flag;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000E448 File Offset: 0x0000C648
		protected virtual void AddImpulseVelocity(ref Vector3 iVelocity)
		{
			if (!this.mBody.Immovable)
			{
				this.mBody.SetActive();
				this.mBody.Velocity = iVelocity;
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000E474 File Offset: 0x0000C674
		public virtual bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			float value = this.Position.Y - iOrigin.Y;
			if (Math.Abs(value) > iHeightDifference)
			{
				oPosition = default(Vector3);
				return false;
			}
			iOrigin.Y = 0f;
			iDirection.Y = 0f;
			Vector3 position = this.Position;
			position.Y = 0f;
			Vector3 vector;
			Vector3.Subtract(ref iOrigin, ref position, out vector);
			float num = vector.Length();
			float num2 = this.mRadius;
			if (num - num2 > iRange)
			{
				oPosition = default(Vector3);
				return false;
			}
			Vector3.Divide(ref vector, num, out vector);
			float num3;
			Vector3.Dot(ref vector, ref iDirection, out num3);
			num3 = -num3;
			float num4 = (float)Math.Acos((double)num3);
			float num5 = -2f * num * num;
			float num6 = (float)Math.Acos((double)((num2 * num2 + num5) / num5));
			if (num4 - num6 < iAngle)
			{
				Vector3.Multiply(ref vector, num2, out vector);
				position = this.Position;
				Vector3.Add(ref position, ref vector, out oPosition);
				return true;
			}
			oPosition = default(Vector3);
			return false;
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000E572 File Offset: 0x0000C772
		public AudioEmitter AudioEmitter
		{
			get
			{
				return this.mAudioEmitter;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0000E57A File Offset: 0x0000C77A
		public PlayState PlayState
		{
			get
			{
				return this.mPlayState;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000E582 File Offset: 0x0000C782
		public virtual Body Body
		{
			get
			{
				return this.mBody;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000E58A File Offset: 0x0000C78A
		public virtual float Radius
		{
			get
			{
				return this.mRadius;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000E592 File Offset: 0x0000C792
		public virtual Vector3 Position
		{
			get
			{
				return this.mBody.Position;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000E5A0 File Offset: 0x0000C7A0
		public virtual Vector3 Direction
		{
			get
			{
				return this.mBody.Orientation.Forward;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
		public ushort Handle
		{
			get
			{
				return this.mHandle;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600020D RID: 525
		public abstract bool Dead { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600020E RID: 526
		public abstract bool Removable { get; }

		// Token: 0x0600020F RID: 527 RVA: 0x0000E5C8 File Offset: 0x0000C7C8
		public static void ClearHandles()
		{
			Entity.mInstances.Clear();
		}

		// Token: 0x06000210 RID: 528
		public abstract void Kill();

		// Token: 0x06000211 RID: 529 RVA: 0x0000E5D4 File Offset: 0x0000C7D4
		internal virtual bool SendsNetworkUpdate(NetworkState iState)
		{
			return iState == NetworkState.Server;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000E5DC File Offset: 0x0000C7DC
		internal bool GetNetworkUpdate(out EntityUpdateMessage oMsg, NetworkState iState, float iPrediction)
		{
			if (this.SendsNetworkUpdate(iState))
			{
				this.IGetNetworkUpdate(out oMsg, iPrediction);
				this.mOutBoundUDPStamp = (int)((ushort)(this.mOutBoundUDPStamp + 1));
				oMsg.UDPStamp = (ushort)this.mOutBoundUDPStamp;
				return oMsg.Features != EntityFeatures.None;
			}
			oMsg = default(EntityUpdateMessage);
			return false;
		}

		// Token: 0x06000213 RID: 531
		protected abstract void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction);

		// Token: 0x06000214 RID: 532 RVA: 0x0000E62C File Offset: 0x0000C82C
		internal void NetworkUpdate(SteamID iSender, ref EntityUpdateMessage iMsg)
		{
			ushort num;
			if (!this.mInBoundUDPStamp.TryGetValue(iSender.AsUInt64, out num) || (iMsg.UDPStamp >= num && iMsg.UDPStamp < num + 32767) || iMsg.UDPStamp < num - 32767)
			{
				this.mInBoundUDPStamp[iSender.AsUInt64] = iMsg.UDPStamp;
				this.INetworkUpdate(ref iMsg);
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000E695 File Offset: 0x0000C895
		internal void ForcedNetworkUpdate(SteamID iSender, ref EntityUpdateMessage iMsg)
		{
			this.mInBoundUDPStamp[iSender.AsUInt64] = iMsg.UDPStamp;
			this.INetworkUpdate(ref iMsg);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000E6B8 File Offset: 0x0000C8B8
		protected virtual void INetworkUpdate(ref EntityUpdateMessage iMsg)
		{
			bool flag = false;
			if (this.mBody != null)
			{
				if ((ushort)(iMsg.Features & EntityFeatures.Position) != 0)
				{
					flag |= (this.mBody.Position != iMsg.Position);
					this.mBody.Position = iMsg.Position;
				}
				if ((ushort)(iMsg.Features & EntityFeatures.Orientation) != 0)
				{
					Matrix matrix;
					Matrix.CreateFromQuaternion(ref iMsg.Orientation, out matrix);
					flag |= (this.mBody.Orientation != matrix);
					this.mBody.Orientation = matrix;
				}
				if (flag)
				{
					this.mBody.SetActive();
				}
				if ((ushort)(iMsg.Features & EntityFeatures.Velocity) != 0)
				{
					flag |= (this.mBody.Velocity != iMsg.Velocity);
					this.mBody.Velocity = iMsg.Velocity;
				}
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000E782 File Offset: 0x0000C982
		internal static bool TryGetFromHandle(ushort iHandle, out Entity oEntity)
		{
			if ((int)iHandle >= Entity.mInstances.Count)
			{
				oEntity = null;
				return false;
			}
			oEntity = Entity.mInstances[(int)iHandle];
			return true;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000E7A4 File Offset: 0x0000C9A4
		internal virtual float GetDanger()
		{
			return 0f;
		}

		// Token: 0x040001B3 RID: 435
		private static List<Entity> mInstances;

		// Token: 0x040001B4 RID: 436
		private static ReadOnlyCollection<Entity> mProtectedInstances;

		// Token: 0x040001B5 RID: 437
		private ushort mHandle;

		// Token: 0x040001B6 RID: 438
		protected int mUniqueID;

		// Token: 0x040001B7 RID: 439
		protected PlayState mPlayState;

		// Token: 0x040001B8 RID: 440
		protected bool mDead;

		// Token: 0x040001B9 RID: 441
		protected Body mBody;

		// Token: 0x040001BA RID: 442
		protected float mRadius;

		// Token: 0x040001BB RID: 443
		protected CollisionSkin mCollision;

		// Token: 0x040001BC RID: 444
		protected AudioEmitter mAudioEmitter;

		// Token: 0x040001BD RID: 445
		protected static Dictionary<int, Entity> mUniqueEntities = new Dictionary<int, Entity>(128);

		// Token: 0x040001BE RID: 446
		private Dictionary<ulong, ushort> mInBoundUDPStamp = new Dictionary<ulong, ushort>();

		// Token: 0x040001BF RID: 447
		private int mOutBoundUDPStamp = -1;
	}
}
