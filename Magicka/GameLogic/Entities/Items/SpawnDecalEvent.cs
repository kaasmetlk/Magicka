using System;
using JigLibX.Geometry;
using Magicka.Graphics;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x0200049F RID: 1183
	public struct SpawnDecalEvent
	{
		// Token: 0x060023D9 RID: 9177 RVA: 0x00101C7D File Offset: 0x000FFE7D
		public SpawnDecalEvent(Decal iDecal, float iScale)
		{
			this.mScale = iScale;
			this.mDecal = iDecal;
			this.mTTL = 60f;
		}

		// Token: 0x060023DA RID: 9178 RVA: 0x00101C98 File Offset: 0x000FFE98
		public SpawnDecalEvent(Decal iDecal, int iScale, int iTTL)
		{
			this.mScale = (float)iScale;
			this.mDecal = iDecal;
			this.mTTL = (float)iTTL;
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x00101CB4 File Offset: 0x000FFEB4
		public SpawnDecalEvent(ContentReader iInput)
		{
			int num = iInput.ReadInt32();
			int num2 = iInput.ReadInt32();
			this.mDecal = (Decal)(num + num2 * 8);
			this.mScale = (float)iInput.ReadInt32();
			this.mTTL = 60f;
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x00101CF4 File Offset: 0x000FFEF4
		public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			Vector3 origin = iItem.Position;
			if (iPosition != null)
			{
				origin = iPosition.Value;
			}
			Segment iSeg = default(Segment);
			iSeg.Origin = origin;
			iSeg.Delta = new Vector3(0f, -iItem.Radius * 2f, 0f);
			float num;
			Vector3 vector;
			Vector3 vector2;
			AnimatedLevelPart iAnimation;
			if (iItem.PlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, out iAnimation, iSeg) && vector2.Y > 0.7f)
			{
				Vector2 vector3 = default(Vector2);
				vector3.X = (vector3.Y = this.mScale);
				Vector3 velocity = iItem.Body.Velocity;
				velocity.Y = 0f;
				velocity.Normalize();
				DecalManager.Instance.AddAlphaBlendedDecal(this.mDecal, iAnimation, ref vector3, ref vector, new Vector3?(velocity), ref vector2, this.mTTL, 1f);
			}
		}

		// Token: 0x040026EE RID: 9966
		private Decal mDecal;

		// Token: 0x040026EF RID: 9967
		private float mScale;

		// Token: 0x040026F0 RID: 9968
		private float mTTL;
	}
}
