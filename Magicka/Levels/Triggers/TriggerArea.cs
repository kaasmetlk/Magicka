using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.Levels.Triggers
{
	// Token: 0x0200032A RID: 810
	public class TriggerArea
	{
		// Token: 0x060018C3 RID: 6339 RVA: 0x000A3918 File Offset: 0x000A1B18
		public TriggerArea(Box iBox)
		{
			if (iBox != null)
			{
				this.mCollisionSkin = new CollisionSkin(null);
				this.mCollisionSkin.callbackFn += this.OnCollision;
				this.mCollisionSkin.AddPrimitive(iBox, 1, new MaterialProperties(0f, 0.8f, 0.8f));
				iBox.GetCentre(out this.mCenter);
				this.mRadius = iBox.SideLengths.Length() * 0.5f;
			}
			this.mTypeCount = new Dictionary<int, int>(10);
			this.mFactionCount = new Dictionary<int, int>(5);
			this.mPresentCharacters = new StaticWeakList<Character>(256);
			this.mPresentEntities = new StaticWeakList<Entity>(256);
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x000A39D4 File Offset: 0x000A1BD4
		protected bool OnCollision(CollisionSkin iSkin0, int iPrim0, CollisionSkin iSkin1, int iPrim1)
		{
			if (iSkin1.Owner != null)
			{
				Entity iEntity = iSkin1.Owner.Tag as Entity;
				this.AddEntity(iEntity);
			}
			else if (iSkin0.Owner != null)
			{
				Entity iEntity2 = iSkin0.Owner.Tag as Entity;
				this.AddEntity(iEntity2);
			}
			return false;
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x000A3A24 File Offset: 0x000A1C24
		public int GetCount(int iType)
		{
			if (iType == TriggerArea.ANYID)
			{
				return this.mTotalCharacters;
			}
			int result;
			if (this.mTypeCount.TryGetValue(iType, out result))
			{
				return result;
			}
			return 0;
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x000A3A54 File Offset: 0x000A1C54
		public Vector3 GetRandomLocation()
		{
			Vector3 position = default(Vector3);
			Box box = (Box)this.mCollisionSkin.GetPrimitiveLocal(0);
			Vector3 sideLengths = box.SideLengths;
			position.X = MagickaMath.RandomBetween(0f, sideLengths.X);
			position.Y = MagickaMath.RandomBetween(0f, sideLengths.Y);
			position.Z = MagickaMath.RandomBetween(0f, sideLengths.Z);
			return Vector3.Transform(position, box.TransformMatrix);
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x000A3AD8 File Offset: 0x000A1CD8
		public int GetFactionCount(Factions iFaction)
		{
			int num = 0;
			for (int i = 0; i <= 15; i++)
			{
				int num2 = 1 << i;
				int num3;
				if ((num2 & (int)iFaction) != 0 && this.mFactionCount.TryGetValue(num2, out num3))
				{
					num += num3;
				}
			}
			return num;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000A3B18 File Offset: 0x000A1D18
		public virtual void Register()
		{
			this.Reset();
			if (!PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
			{
				PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
			}
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x000A3B66 File Offset: 0x000A1D66
		public void Reset()
		{
			this.mPresentEntities.Clear();
			this.mTotalEntities = 0;
			this.mPresentCharacters.Clear();
			this.mTotalCharacters = 0;
			this.mTypeCount.Clear();
			this.mFactionCount.Clear();
		}

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x060018CA RID: 6346 RVA: 0x000A3BA2 File Offset: 0x000A1DA2
		public StaticWeakList<Character> PresentCharacters
		{
			get
			{
				return this.mPresentCharacters;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x060018CB RID: 6347 RVA: 0x000A3BAA File Offset: 0x000A1DAA
		public StaticWeakList<Entity> PresentEntities
		{
			get
			{
				return this.mPresentEntities;
			}
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x000A3BB4 File Offset: 0x000A1DB4
		public static TriggerArea Read(ContentReader iInput)
		{
			Vector3 pos = iInput.ReadVector3();
			Vector3 sideLengths = iInput.ReadVector3();
			Quaternion quaternion = iInput.ReadQuaternion();
			Matrix orient;
			Matrix.CreateFromQuaternion(ref quaternion, out orient);
			return new TriggerArea(new Box(pos, orient, sideLengths));
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x060018CD RID: 6349 RVA: 0x000A3BEC File Offset: 0x000A1DEC
		public CollisionSkin CollisionSkin
		{
			get
			{
				return this.mCollisionSkin;
			}
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000A3BF4 File Offset: 0x000A1DF4
		internal virtual void UpdatePresent(EntityManager iManager)
		{
			this.Reset();
			StaticList<Entity> entities = iManager.Entities;
			Box box = this.mCollisionSkin.GetPrimitiveNewWorld(0) as Box;
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity = entities[i];
				Vector3 vector;
				if (box.GetDistanceToPoint(out vector, entity.Position) <= entity.Radius)
				{
					this.AddEntity(entity);
				}
			}
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x000A3C5C File Offset: 0x000A1E5C
		internal void AddEntity(Entity iEntity)
		{
			if (iEntity is Avatar && (iEntity as Avatar).IgnoreTriggers)
			{
				return;
			}
			this.mPresentEntities.Add(iEntity);
			this.mTotalEntities++;
			Character character = iEntity as Character;
			if (character != null)
			{
				this.mPresentCharacters.Add(character);
				if (!character.Dead || character.Template.Undying)
				{
					int num;
					if (!this.mTypeCount.TryGetValue(character.Type, out num))
					{
						num = 0;
					}
					this.mTypeCount[character.Type] = num + 1;
					for (int i = 0; i <= 15; i++)
					{
						Factions factions = (Factions)(1 << i);
						if ((factions & character.GetOriginalFaction) != Factions.NONE)
						{
							if (!this.mFactionCount.TryGetValue((int)factions, out num))
							{
								num = 0;
							}
							this.mFactionCount[(int)factions] = num + 1;
						}
					}
					this.mTotalCharacters++;
				}
			}
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000A3D3F File Offset: 0x000A1F3F
		internal Box Box
		{
			get
			{
				if (this.mCollisionSkin != null)
				{
					return this.mCollisionSkin.GetPrimitiveLocal(0) as Box;
				}
				return null;
			}
		}

		// Token: 0x04001A97 RID: 6807
		public static readonly int ANYID = "any".GetHashCodeCustom();

		// Token: 0x04001A98 RID: 6808
		protected CollisionSkin mCollisionSkin;

		// Token: 0x04001A99 RID: 6809
		protected int mTotalCharacters;

		// Token: 0x04001A9A RID: 6810
		protected StaticWeakList<Character> mPresentCharacters;

		// Token: 0x04001A9B RID: 6811
		protected int mTotalEntities;

		// Token: 0x04001A9C RID: 6812
		protected Vector3 mCenter;

		// Token: 0x04001A9D RID: 6813
		protected float mRadius;

		// Token: 0x04001A9E RID: 6814
		protected StaticWeakList<Entity> mPresentEntities;

		// Token: 0x04001A9F RID: 6815
		protected Dictionary<int, int> mTypeCount;

		// Token: 0x04001AA0 RID: 6816
		protected Dictionary<int, int> mFactionCount;
	}
}
