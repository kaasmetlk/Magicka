using System;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x0200053C RID: 1340
	public class JormungandrCollisionZone : BossCollisionZone
	{
		// Token: 0x060027DC RID: 10204 RVA: 0x0012343D File Offset: 0x0012163D
		public JormungandrCollisionZone(PlayState iPlayState, IBoss iParent, params Primitive[] iPrimitives) : base(iPlayState, iParent, iPrimitives)
		{
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x060027DD RID: 10205 RVA: 0x00123448 File Offset: 0x00121648
		public override Vector3 Position
		{
			get
			{
				Transform transform;
				this.mCollision.GetPrimitiveLocal(16).GetTransform(out transform);
				Vector3 position = transform.Position;
				position.Y = 1f;
				return position;
			}
		}
	}
}
