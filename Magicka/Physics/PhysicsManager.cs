using System;
using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Magicka.Physics
{
	// Token: 0x02000095 RID: 149
	public class PhysicsManager
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00015ED0 File Offset: 0x000140D0
		public static PhysicsManager Instance
		{
			get
			{
				if (PhysicsManager.mSingelton == null)
				{
					lock (PhysicsManager.mSingeltonLock)
					{
						if (PhysicsManager.mSingelton == null)
						{
							PhysicsManager.mSingelton = new PhysicsManager();
						}
					}
				}
				return PhysicsManager.mSingelton;
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00015F24 File Offset: 0x00014124
		private PhysicsManager()
		{
			this.CreateSimulator();
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00015F34 File Offset: 0x00014134
		private void CreateSimulator()
		{
			this.mSimulator = new PhysicsSystem();
			this.mSimulator.Gravity = Vector3.Down * 9.82f * 2f;
			this.mSimulator.CollisionSystem = new CollisionSystemSAP();
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new HollowSphereCapsule());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new HollowSphereSphere());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new HollowSphereBox());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new HollowSphereStaticMesh());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new CustomCapsuleStaticMesh());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new CustomCapsulePlane());
			this.mSimulator.CollisionSystem.RegisterCollDetectFunctor(new CustomCapsuleCapsule());
			this.mSimulator.EnableFreezing = true;
			this.mSimulator.AllowedPenetration = 0.001f;
			this.mSimulator.SolverType = PhysicsSystem.Solver.Normal;
			this.mSimulator.CollisionSystem.UseSweepTests = true;
			this.mSimulator.IsShockStepEnabled = false;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00016058 File Offset: 0x00014258
		public void Update(float iDeltaTime)
		{
			if (this.mIsFrozen)
			{
				return;
			}
			this.mSimulator.Integrate(iDeltaTime);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00016070 File Offset: 0x00014270
		public void Clear()
		{
			while (this.mSimulator.Bodies.Count > 0)
			{
				this.mSimulator.Bodies[0].DisableBody();
			}
			while (this.mSimulator.CollisionSystem.CollisionSkins.Count > 0)
			{
				this.mSimulator.CollisionSystem.RemoveCollisionSkin(this.mSimulator.CollisionSystem.CollisionSkins[0]);
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x000160E9 File Offset: 0x000142E9
		public void Freeze()
		{
			this.mIsFrozen = true;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x000160F2 File Offset: 0x000142F2
		public void UnFreeze()
		{
			this.mIsFrozen = false;
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x000160FB File Offset: 0x000142FB
		public PhysicsSystem Simulator
		{
			get
			{
				return this.mSimulator;
			}
		}

		// Token: 0x040002EF RID: 751
		private PhysicsSystem mSimulator;

		// Token: 0x040002F0 RID: 752
		private static PhysicsManager mSingelton;

		// Token: 0x040002F1 RID: 753
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040002F2 RID: 754
		private bool mIsFrozen;
	}
}
