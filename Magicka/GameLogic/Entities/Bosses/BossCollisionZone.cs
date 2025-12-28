using System;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x02000035 RID: 53
	public class BossCollisionZone : Entity
	{
		// Token: 0x06000219 RID: 537 RVA: 0x0000E7AC File Offset: 0x0000C9AC
		public BossCollisionZone(PlayState iPlayState, IBoss iParent, params Primitive[] iPrimitives) : base(iPlayState)
		{
			this.mParent = iParent;
			this.mRadius = 100f;
			this.mBody = new Body();
			this.mCollision = new CollisionSkin(this.mBody);
			for (int i = 0; i < iPrimitives.Length; i++)
			{
				this.mCollision.AddPrimitive(iPrimitives[i], 1, new MaterialProperties(0f, 0f, 0f));
			}
			this.mCollision.ApplyLocalTransform(Transform.Identity);
			this.mBody.CollisionSkin = this.mCollision;
			this.mBody.Immovable = true;
			this.mBody.Tag = this;
			this.mBody.MoveTo(default(Vector3), Matrix.Identity);
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000E872 File Offset: 0x0000CA72
		public float HitPoints
		{
			get
			{
				return this.mParent.HitPoints;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000E87F File Offset: 0x0000CA7F
		public float MaxHitPoints
		{
			get
			{
				return this.mParent.MaxHitPoints;
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000E88C File Offset: 0x0000CA8C
		public override Vector3 CalcImpulseVelocity(Vector3 iDirection, float iElevation, float iMassPower, float iDistance)
		{
			return default(Vector3);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000E8A2 File Offset: 0x0000CAA2
		public void SetOrientation(ref Vector3 iPosition, ref Matrix iOrientation)
		{
			this.mBody.MoveTo(iPosition, iOrientation);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000E8BB File Offset: 0x0000CABB
		public new virtual void Initialize(int iUniqueID)
		{
			base.Initialize(iUniqueID);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000E8C4 File Offset: 0x0000CAC4
		public new virtual void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000E8CC File Offset: 0x0000CACC
		public override bool Dead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E8D0 File Offset: 0x0000CAD0
		public virtual bool SegmentIntersect(out Vector3 oPosition, Segment iSeg, float iSegmentRadius)
		{
			float num;
			Vector3 vector;
			return this.mCollision.SegmentIntersect(out num, out oPosition, out vector, iSeg);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E8EE File Offset: 0x0000CAEE
		public override bool ArcIntersect(out Vector3 oPosition, Vector3 iOrigin, Vector3 iDirection, float iRange, float iAngle, float iHeightDifference)
		{
			oPosition = default(Vector3);
			return false;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000E8F8 File Offset: 0x0000CAF8
		public override void Kill()
		{
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000E8FA File Offset: 0x0000CAFA
		public IBoss Owner
		{
			get
			{
				return this.mParent;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000225 RID: 549 RVA: 0x0000E902 File Offset: 0x0000CB02
		public override bool Removable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E905 File Offset: 0x0000CB05
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
		}

		// Token: 0x040001C0 RID: 448
		protected IBoss mParent;
	}
}
