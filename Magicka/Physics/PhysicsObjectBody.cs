using System;
using JigLibX.Collision;
using JigLibX.Physics;

namespace Magicka.Physics
{
	// Token: 0x020004F0 RID: 1264
	public class PhysicsObjectBody : Body
	{
		// Token: 0x0600255C RID: 9564 RVA: 0x0010FE68 File Offset: 0x0010E068
		public override void ProcessCollisionPoints(float dt)
		{
			if (!this.mReactToCharacters)
			{
				for (int i = 0; i < base.CollisionSkin.Collisions.Count; i++)
				{
					CollisionInfo collisionInfo = base.CollisionSkin.Collisions[i];
					if (collisionInfo.SkinInfo.Skin0 != null && collisionInfo.SkinInfo.Skin1 != null)
					{
						if (collisionInfo.SkinInfo.Skin0.Owner is CharacterBody)
						{
							collisionInfo.SkinInfo.IgnoreSkin1 = true;
						}
						if (collisionInfo.SkinInfo.Skin1.Owner is CharacterBody)
						{
							collisionInfo.SkinInfo.IgnoreSkin0 = true;
						}
					}
				}
			}
			base.ProcessCollisionPoints(dt);
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x0600255D RID: 9565 RVA: 0x0010FF17 File Offset: 0x0010E117
		// (set) Token: 0x0600255E RID: 9566 RVA: 0x0010FF1F File Offset: 0x0010E11F
		public bool ReactToCharacters
		{
			get
			{
				return this.mReactToCharacters;
			}
			set
			{
				this.mReactToCharacters = value;
			}
		}

		// Token: 0x040028DC RID: 10460
		private bool mReactToCharacters;
	}
}
