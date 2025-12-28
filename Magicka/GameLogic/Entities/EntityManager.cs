using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities
{
	// Token: 0x020005CC RID: 1484
	public class EntityManager
	{
		// Token: 0x06002C56 RID: 11350 RVA: 0x0015CD4C File Offset: 0x0015AF4C
		public EntityManager(PlayState iPlayState)
		{
			this.mEntities = new StaticObjectList<Entity>(512);
			this.mShields = new List<Shield>(256);
			this.mBarriers = new List<Barrier>(256);
			this.mPlayState = iPlayState;
			MissileEntity.InitializeCache(128, this.mPlayState);
			NonPlayerCharacter.InitializeCache(192, this.mPlayState);
			ElementalEgg.InitializeCache(128, this.mPlayState);
			this.mQuadGrid = new List<Entity>[16, 16];
			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					this.mQuadGrid[i, j] = new List<Entity>(512);
				}
			}
			this.mQuaryLists = new Queue<List<Entity>>();
			this.mQuaryLists.Enqueue(new List<Entity>(512));
			this.mQuaryLists.Enqueue(new List<Entity>(512));
			this.mQuaryLists.Enqueue(new List<Entity>(512));
			this.mQuaryLists.Enqueue(new List<Entity>(512));
		}

		// Token: 0x06002C57 RID: 11351 RVA: 0x0015CE68 File Offset: 0x0015B068
		private void PlaceEntitiesInGrid()
		{
			Vector2 vector = default(Vector2);
			for (int i = 0; i < this.mEntities.Count; i++)
			{
				Entity entity = this.mEntities[i];
				vector.X = entity.Position.X * 0.125f;
				vector.Y = entity.Position.Z * 0.125f;
				float num = entity.Radius * 0.125f;
				float num2 = Math.Min(num, 8f);
				int num3 = (int)Math.Floor((double)(vector.X - num2));
				int num4 = (int)Math.Floor((double)(vector.X + num2));
				for (int j = num3; j <= num4; j++)
				{
					float num5;
					if (Math.Abs((float)j - vector.X) < Math.Abs((float)(j + 1) - vector.X))
					{
						num5 = (float)Math.Sqrt((double)(num * num - ((float)j - vector.X) * ((float)j - vector.X)));
					}
					else
					{
						num5 = (float)Math.Sqrt((double)(num * num - ((float)(j + 1) - vector.X) * ((float)(j + 1) - vector.X)));
					}
					if (float.IsNaN(num5))
					{
						num5 = num;
					}
					int num6 = (int)Math.Floor((double)(vector.Y - Math.Min(num5, num2)));
					int num7 = (int)Math.Floor((double)(vector.Y + Math.Min(num5, num2)));
					int num8 = j % 16;
					if (num8 < 0)
					{
						num8 += 16;
					}
					for (int k = num6; k <= num7; k++)
					{
						int num9 = k % 16;
						if (num9 < 0)
						{
							num9 += 16;
						}
						List<Entity> list = this.mQuadGrid[num8, num9];
						lock (list)
						{
							list.Add(entity);
						}
					}
				}
			}
		}

		// Token: 0x06002C58 RID: 11352 RVA: 0x0015D058 File Offset: 0x0015B258
		public void UpdateQuadGrid()
		{
			lock (this.mQuadGrid)
			{
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						this.mQuadGrid[i, j].Clear();
					}
				}
				this.PlaceEntitiesInGrid();
			}
		}

		// Token: 0x06002C59 RID: 11353 RVA: 0x0015D0C0 File Offset: 0x0015B2C0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (iDataChannel == DataChannel.None)
			{
				return;
			}
			lock (this.mEntities)
			{
				for (int i = 0; i < this.mEntities.Count; i++)
				{
					Entity entity = this.mEntities[i];
					entity.Update(iDataChannel, iDeltaTime);
				}
			}
		}

		// Token: 0x06002C5A RID: 11354 RVA: 0x0015D124 File Offset: 0x0015B324
		public void UpdateNetwork(int iClientIndex, float iPrediction)
		{
			int num = 0;
			NetworkState state = NetworkManager.Instance.State;
			NetworkInterface @interface = NetworkManager.Instance.Interface;
			for (int i = 0; i < this.mEntities.Count; i++)
			{
				Entity entity = this.mEntities[i];
				EntityUpdateMessage entityUpdateMessage;
				if (entity.GetNetworkUpdate(out entityUpdateMessage, state, iPrediction))
				{
					num++;
					entityUpdateMessage.Handle = entity.Handle;
					@interface.SendUdpMessage<EntityUpdateMessage>(ref entityUpdateMessage, iClientIndex);
				}
			}
		}

		// Token: 0x06002C5B RID: 11355 RVA: 0x0015D198 File Offset: 0x0015B398
		public void RemoveDeadEntities()
		{
			NetworkManager instance = NetworkManager.Instance;
			for (int i = 0; i < this.mEntities.Count; i++)
			{
				Entity entity = this.mEntities[i];
				if (entity.Removable)
				{
					if (instance.State == NetworkState.Server)
					{
						EntityRemoveMessage entityRemoveMessage;
						entityRemoveMessage.Handle = entity.Handle;
						instance.Interface.SendMessage<EntityRemoveMessage>(ref entityRemoveMessage);
					}
					this.RemoveEntity(i, entity);
					i--;
				}
			}
		}

		// Token: 0x06002C5C RID: 11356 RVA: 0x0015D208 File Offset: 0x0015B408
		public void ReturnEntityList(List<Entity> iList)
		{
			if (iList == null)
			{
				throw new ArgumentException("Argument cannot be null", "iList");
			}
			if (this.mQuaryLists.Contains(iList))
			{
				throw new Exception("This list is already present in the cache!");
			}
			lock (this.mQuaryLists)
			{
				this.mQuaryLists.Enqueue(iList);
			}
		}

		// Token: 0x06002C5D RID: 11357 RVA: 0x0015D274 File Offset: 0x0015B474
		public IDamageable GetClosestIDamageable(IDamageable iCaster, Vector3 iCenter, float iRadius, bool iIgnoreProtectedEntities)
		{
			int num = (int)Math.Floor((double)((iCenter.X - iRadius) * 0.125f));
			int num2 = (int)Math.Floor((double)((iCenter.X + iRadius) * 0.125f));
			int num3 = (int)Math.Floor((double)((iCenter.Z - iRadius) * 0.125f));
			int num4 = (int)Math.Floor((double)((iCenter.Z + iRadius) * 0.125f));
			if (num2 - num > 16)
			{
				num = 0;
				num2 = 15;
			}
			if (num4 - num3 > 16)
			{
				num3 = 0;
				num4 = 15;
			}
			float num5 = float.MaxValue;
			IDamageable result = null;
			Segment iSeg;
			iSeg.Origin = iCenter;
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					int num6 = i % 16;
					int num7 = j % 16;
					if (num6 < 0)
					{
						num6 += 16;
					}
					if (num7 < 0)
					{
						num7 += 16;
					}
					List<Entity> list = this.mQuadGrid[num6, num7];
					for (int k = 0; k < list.Count; k++)
					{
						IDamageable damageable = list[k] as IDamageable;
						if (damageable != null && !(damageable.Dead | damageable == iCaster))
						{
							Vector3 position = damageable.Position;
							float num8;
							Vector3.DistanceSquared(ref position, ref iCenter, out num8);
							float num9 = iRadius + damageable.Radius;
							num9 *= num9;
							if (num8 <= num9 && num8 < num5)
							{
								bool flag = false;
								if (iIgnoreProtectedEntities)
								{
									for (int l = 0; l < this.mShields.Count; l++)
									{
										iSeg.Delta = damageable.Position;
										Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
										Vector3 vector;
										if (this.mShields[l].SegmentIntersect(out vector, iSeg, 0.5f))
										{
											flag = true;
											break;
										}
									}
								}
								if (!flag)
								{
									result = damageable;
									num5 = num8;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06002C5E RID: 11358 RVA: 0x0015D45A File Offset: 0x0015B65A
		public List<Entity> GetEntities(Vector3 iCenter, float iRadius, bool iIgnoreProtectedEntities)
		{
			return this.GetEntities(iCenter, iRadius, iIgnoreProtectedEntities, false);
		}

		// Token: 0x06002C5F RID: 11359 RVA: 0x0015D468 File Offset: 0x0015B668
		public List<Entity> GetEntities(Vector3 iCenter, float iRadius, bool iIgnoreProtectedEntities, bool iIgnoreYAxis)
		{
			List<Entity> list;
			lock (this.mQuaryLists)
			{
				if (this.mQuaryLists.Count > 0)
				{
					list = this.mQuaryLists.Dequeue();
				}
				else
				{
					list = new List<Entity>(512);
				}
			}
			list.Clear();
			if (iIgnoreYAxis)
			{
				iCenter.Y = 0f;
			}
			int num = (int)Math.Floor((double)((iCenter.X - iRadius) * 0.125f));
			int num2 = (int)Math.Floor((double)((iCenter.X + iRadius) * 0.125f));
			int num3 = (int)Math.Floor((double)((iCenter.Z - iRadius) * 0.125f));
			int num4 = (int)Math.Floor((double)((iCenter.Z + iRadius) * 0.125f));
			if (num2 - num > 16)
			{
				num = 0;
				num2 = 15;
			}
			if (num4 - num3 > 16)
			{
				num3 = 0;
				num4 = 15;
			}
			lock (this.mQuadGrid)
			{
				for (int i = num; i <= num2; i++)
				{
					for (int j = num3; j <= num4; j++)
					{
						int num5 = i % 16;
						int num6 = j % 16;
						if (num5 < 0)
						{
							num5 += 16;
						}
						if (num6 < 0)
						{
							num6 += 16;
						}
						List<Entity> list2 = this.mQuadGrid[num5, num6];
						int count = list2.Count;
						for (int k = 0; k < count; k++)
						{
							Entity entity = list2[k];
							if (!entity.Dead)
							{
								Vector3 position = entity.Position;
								if (iIgnoreYAxis)
								{
									position.Y = 0f;
								}
								float num7;
								Vector3.DistanceSquared(ref position, ref iCenter, out num7);
								float num8 = iRadius + entity.Radius;
								num8 *= num8;
								if (num7 <= num8 && !list.Contains(entity))
								{
									list.Add(entity);
								}
							}
						}
					}
				}
			}
			if (iIgnoreProtectedEntities)
			{
				Segment iSeg;
				iSeg.Origin = iCenter;
				for (int l = 0; l < list.Count; l++)
				{
					Shield shield = list[l] as Shield;
					if (shield != null)
					{
						for (int m = 0; m < list.Count; m++)
						{
							if (l != m)
							{
								iSeg.Delta = list[m].Position;
								Vector3.Subtract(ref iSeg.Delta, ref iSeg.Origin, out iSeg.Delta);
								IDamageable damageable = list[m] as IDamageable;
								Vector3 vector;
								if (damageable == null | shield.SegmentIntersect(out vector, iSeg, 0.5f))
								{
									float num7;
									Vector3.DistanceSquared(ref vector, ref iCenter, out num7);
									if (num7 > 1E-06f)
									{
										if (m < l)
										{
											l--;
										}
										list.RemoveAt(m--);
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06002C60 RID: 11360 RVA: 0x0015D73C File Offset: 0x0015B93C
		public void AddEntity(Entity iEntity)
		{
			lock (this.mEntities)
			{
				if (this.mEntities.Contains(iEntity))
				{
					return;
				}
				this.mEntities.Add(iEntity);
			}
			if (iEntity is PhysicsEntity)
			{
				(iEntity as PhysicsEntity).OnSpawn();
			}
			if (iEntity is Shield)
			{
				this.mShields.Add(iEntity as Shield);
				return;
			}
			if (iEntity is Barrier)
			{
				this.mBarriers.Add(iEntity as Barrier);
			}
		}

		// Token: 0x06002C61 RID: 11361 RVA: 0x0015D7D4 File Offset: 0x0015B9D4
		private void RemoveEntity(int iIndex, Entity iEntity)
		{
			this.mEntities.RemoveAt(iIndex);
			iEntity.Deinitialize();
			if (iEntity is Shield)
			{
				this.mShields.Remove(iEntity as Shield);
				return;
			}
			if (iEntity is Barrier)
			{
				this.mBarriers.Remove(iEntity as Barrier);
			}
		}

		// Token: 0x06002C62 RID: 11362 RVA: 0x0015D828 File Offset: 0x0015BA28
		public void Clear()
		{
			while (this.mEntities.Count > 0)
			{
				this.RemoveEntity(this.mEntities.Count - 1, this.mEntities[this.mEntities.Count - 1]);
			}
			this.Shields.Clear();
			this.Barriers.Clear();
		}

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06002C63 RID: 11363 RVA: 0x0015D886 File Offset: 0x0015BA86
		public StaticList<Entity> Entities
		{
			get
			{
				return this.mEntities;
			}
		}

		// Token: 0x06002C64 RID: 11364 RVA: 0x0015D890 File Offset: 0x0015BA90
		public void GetCharacters(ref List<Character> chars)
		{
			int count = this.mEntities.Count;
			int i = 0;
			while (i < count)
			{
				Character character;
				try
				{
					character = (this.mEntities[i] as Character);
				}
				catch
				{
					goto IL_32;
				}
				goto IL_27;
				IL_32:
				i++;
				continue;
				IL_27:
				if (character != null)
				{
					chars.Add(character);
					goto IL_32;
				}
				goto IL_32;
			}
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06002C65 RID: 11365 RVA: 0x0015D8E8 File Offset: 0x0015BAE8
		public List<Barrier> Barriers
		{
			get
			{
				return this.mBarriers;
			}
		}

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06002C66 RID: 11366 RVA: 0x0015D8F0 File Offset: 0x0015BAF0
		public List<Shield> Shields
		{
			get
			{
				return this.mShields;
			}
		}

		// Token: 0x06002C67 RID: 11367 RVA: 0x0015D8F8 File Offset: 0x0015BAF8
		public void ClearAndStore(List<Entity> iStoreTarget)
		{
			for (int i = 0; i < this.mEntities.Count; i++)
			{
				Entity entity = this.mEntities[i];
				entity.Body.DisableBody();
				if (!(entity is Avatar))
				{
					if (iStoreTarget != null && !entity.Removable)
					{
						if (entity is PhysicsEntity)
						{
							iStoreTarget.Add(entity);
							goto IL_C6;
						}
						NonPlayerCharacter nonPlayerCharacter = entity as NonPlayerCharacter;
						if (nonPlayerCharacter != null && !nonPlayerCharacter.IsSummoned && (nonPlayerCharacter.HitPoints > 0f || nonPlayerCharacter.Undying))
						{
							nonPlayerCharacter.AI.Disable();
							iStoreTarget.Add(entity);
							goto IL_C6;
						}
						Pickable pickable = entity as Pickable;
						if (pickable != null && pickable.IsPickable)
						{
							iStoreTarget.Add(entity);
							goto IL_C6;
						}
						ElementalEgg elementalEgg = entity as ElementalEgg;
						if (elementalEgg != null && !elementalEgg.IsSummoned)
						{
							iStoreTarget.Add(entity);
							goto IL_C6;
						}
					}
					entity.Deinitialize();
				}
				IL_C6:;
			}
			this.mEntities.Clear();
		}

		// Token: 0x06002C68 RID: 11368 RVA: 0x0015D9EC File Offset: 0x0015BBEC
		public bool IsProtectedByShield(Entity iEntity, out Shield oShield)
		{
			Vector3 position = iEntity.Position;
			for (int i = 0; i < this.mShields.Count; i++)
			{
				Shield shield = this.mShields[i];
				Vector3 position2 = shield.Position;
				float num;
				Vector3.DistanceSquared(ref position, ref position2, out num);
				if (shield.ShieldType == ShieldType.SPHERE & num < shield.Radius * shield.Radius)
				{
					oShield = shield;
					return true;
				}
			}
			oShield = null;
			return false;
		}

		// Token: 0x04002FFC RID: 12284
		public const int MAXENTITIES = 512;

		// Token: 0x04002FFD RID: 12285
		public const int QUADCELLSIZE = 8;

		// Token: 0x04002FFE RID: 12286
		public const int QUADCELLS = 16;

		// Token: 0x04002FFF RID: 12287
		private StaticList<Entity> mEntities;

		// Token: 0x04003000 RID: 12288
		private List<Entity>[,] mQuadGrid;

		// Token: 0x04003001 RID: 12289
		private List<Shield> mShields;

		// Token: 0x04003002 RID: 12290
		private List<Barrier> mBarriers;

		// Token: 0x04003003 RID: 12291
		private Queue<List<Entity>> mQuaryLists;

		// Token: 0x04003004 RID: 12292
		private PlayState mPlayState;
	}
}
