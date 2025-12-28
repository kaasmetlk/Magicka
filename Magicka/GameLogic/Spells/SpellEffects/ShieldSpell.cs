using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Buffs;
using Magicka.Gamers;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Spells.SpellEffects
{
	// Token: 0x0200039E RID: 926
	internal class ShieldSpell : SpellEffect
	{
		// Token: 0x06001C5A RID: 7258 RVA: 0x000C1078 File Offset: 0x000BF278
		public static void InitializeCache(int iSize)
		{
			ShieldSpell.sCache = new List<ShieldSpell>(iSize);
			for (int i = 0; i < iSize; i++)
			{
				ShieldSpell.sCache.Add(new ShieldSpell());
			}
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x000C10AC File Offset: 0x000BF2AC
		public static SpellEffect GetFromCache()
		{
			ShieldSpell shieldSpell = ShieldSpell.sCache[ShieldSpell.sCache.Count - 1];
			ShieldSpell.sCache.Remove(shieldSpell);
			return shieldSpell;
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x000C10DD File Offset: 0x000BF2DD
		public static void ReturnToCache(ShieldSpell iEffect)
		{
			ShieldSpell.sCache.Add(iEffect);
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x000C10EC File Offset: 0x000BF2EC
		public override void CastArea(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			base.CastArea(iSpell, iOwner, iFromStaff);
			this.mSpell = iSpell;
			Elements elements = this.mSpell.Element & ~Elements.Shield;
			this.mMinTTL = 0.5f;
			this.mHasCast = true;
			if (elements == Elements.None)
			{
				NetworkState state = NetworkManager.Instance.State;
				if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num = 5.5f;
					int num2 = 1000;
					Vector3 shieldcolor = Spell.SHIELDCOLOR;
					Vector3.Multiply(ref shieldcolor, 2f * (float)Math.Sqrt((double)iSpell[Elements.Shield]), out shieldcolor);
					Vector3 arcanecolor = Spell.ARCANECOLOR;
					Vector3.Multiply(ref arcanecolor, 2f * (float)Math.Sqrt((double)iSpell[Elements.Arcane]), out arcanecolor);
					Vector3 lifecolor = Spell.LIFECOLOR;
					Vector3.Multiply(ref lifecolor, 2f * (float)Math.Sqrt((double)iSpell[Elements.Life]), out lifecolor);
					Vector3.Add(ref shieldcolor, ref arcanecolor, out shieldcolor);
					Vector3.Add(ref shieldcolor, ref lifecolor, out shieldcolor);
					if (state == NetworkState.Client)
					{
						SpawnShieldRequestMessage spawnShieldRequestMessage;
						spawnShieldRequestMessage.OwnerHandle = iOwner.Handle;
						spawnShieldRequestMessage.Position = iOwner.Position;
						spawnShieldRequestMessage.Radius = num;
						spawnShieldRequestMessage.Direction = iOwner.Direction;
						spawnShieldRequestMessage.ShieldType = ShieldType.SPHERE;
						spawnShieldRequestMessage.HitPoints = (float)num2;
						NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref spawnShieldRequestMessage, 0);
						return;
					}
					Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
					if (state == NetworkState.Server)
					{
						SpawnShieldMessage spawnShieldMessage;
						spawnShieldMessage.Handle = fromCache.Handle;
						spawnShieldMessage.OwnerHandle = iOwner.Handle;
						spawnShieldMessage.Position = iOwner.Position;
						spawnShieldMessage.Radius = num;
						spawnShieldMessage.Direction = iOwner.Direction;
						spawnShieldMessage.ShieldType = ShieldType.SPHERE;
						spawnShieldMessage.HitPoints = (float)num2;
						NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref spawnShieldMessage);
					}
					fromCache.Initialize(iOwner, iOwner.Position, num, iOwner.Direction, ShieldType.SPHERE, (float)num2, shieldcolor);
					iOwner.PlayState.EntityManager.AddEntity(fromCache);
					return;
				}
			}
			else if ((elements & Elements.PhysicalElements) == Elements.None && ((elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life))
			{
				NetworkState state2 = NetworkManager.Instance.State;
				if ((state2 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state2 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num3 = 4f;
					float num4 = num3 * 6.2831855f;
					float num5 = num4 * 0.5f;
					int num6 = (int)num5;
					float num7 = (num5 + (float)num6) / (float)num6;
					float num8 = 6.2831855f / (float)num6;
					Vector3 position = iOwner.Position;
					Vector3 direction = iOwner.Direction;
					Vector3.Multiply(ref direction, num3, out direction);
					Quaternion quaternion;
					Quaternion.CreateFromYawPitchRoll(num8 * 0.5f, 0f, 0f, out quaternion);
					Vector3 origin;
					Vector3.Transform(ref direction, ref quaternion, out origin);
					Vector3 vector;
					Vector3.Normalize(ref origin, out vector);
					Vector3.Add(ref position, ref origin, out origin);
					Quaternion.CreateFromYawPitchRoll(num8 * 1f, 0f, 0f, out quaternion);
					Quaternion quaternion2;
					Quaternion.CreateFromYawPitchRoll(num8 * -0.5f, 0f, 0f, out quaternion2);
					Vector3 origin2;
					Vector3.Transform(ref direction, ref quaternion2, out origin2);
					Vector3 vector2;
					Vector3.Normalize(ref origin2, out vector2);
					Vector3.Add(ref position, ref origin2, out origin2);
					Quaternion.CreateFromYawPitchRoll(num8 * -1f, 0f, 0f, out quaternion2);
					Vector3 vector3;
					Vector3.Subtract(ref origin, ref origin2, out vector3);
					Vector3 vector4;
					Vector3.Subtract(ref origin2, ref origin, out vector4);
					float num9 = vector3.Length();
					Segment iSeg = default(Segment);
					iSeg.Delta.Y = -4f;
					this.mSpell = iSpell;
					DamageCollection5 damage;
					this.mSpell.CalculateDamage(SpellType.Shield, CastType.Area, out damage);
					iSeg.Origin = origin;
					float num10 = num4 * 0.5f - num9;
					float num11;
					Vector3 vector5;
					AnimatedLevelPart animatedLevelPart;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num11, out position, out vector5, out animatedLevelPart, iSeg))
					{
						if (state2 == NetworkState.Client)
						{
							SpawnMineRequestMessage spawnMineRequestMessage;
							spawnMineRequestMessage.OwnerHandle = iOwner.Handle;
							spawnMineRequestMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineRequestMessage.Position = position;
							spawnMineRequestMessage.Direction = vector;
							spawnMineRequestMessage.Scale = num7;
							spawnMineRequestMessage.Spell = iSpell;
							spawnMineRequestMessage.Damage = damage;
							spawnMineRequestMessage.Range = num10;
							spawnMineRequestMessage.NextDir = vector3;
							spawnMineRequestMessage.NextRotation = quaternion;
							spawnMineRequestMessage.Distance = num9;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref spawnMineRequestMessage, 0);
						}
						else
						{
							SpellMine instance = SpellMine.GetInstance();
							if (state2 == NetworkState.Server)
							{
								SpawnMineMessage spawnMineMessage;
								spawnMineMessage.Handle = instance.Handle;
								spawnMineMessage.OwnerHandle = iOwner.Handle;
								spawnMineMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
								spawnMineMessage.Position = position;
								spawnMineMessage.Direction = vector;
								spawnMineMessage.Scale = num7;
								spawnMineMessage.Spell = iSpell;
								spawnMineMessage.Damage = damage;
								NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage);
							}
							instance.Initialize(iOwner, position, vector, num7, num10, vector3, quaternion, num9, ref iSpell, ref damage, animatedLevelPart);
							iOwner.PlayState.EntityManager.AddEntity(instance);
						}
					}
					iSeg.Origin = origin2;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num11, out position, out vector5, out animatedLevelPart, iSeg))
					{
						if (state2 == NetworkState.Client)
						{
							SpawnMineRequestMessage spawnMineRequestMessage2;
							spawnMineRequestMessage2.OwnerHandle = iOwner.Handle;
							spawnMineRequestMessage2.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineRequestMessage2.Position = position;
							spawnMineRequestMessage2.Direction = vector2;
							spawnMineRequestMessage2.Scale = num7;
							spawnMineRequestMessage2.Spell = iSpell;
							spawnMineRequestMessage2.Damage = damage;
							spawnMineRequestMessage2.Range = num10;
							spawnMineRequestMessage2.NextDir = vector4;
							spawnMineRequestMessage2.NextRotation = quaternion2;
							spawnMineRequestMessage2.Distance = num9;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref spawnMineRequestMessage2, 0);
							return;
						}
						SpellMine instance2 = SpellMine.GetInstance();
						if (state2 == NetworkState.Server)
						{
							SpawnMineMessage spawnMineMessage2;
							spawnMineMessage2.Handle = instance2.Handle;
							spawnMineMessage2.OwnerHandle = iOwner.Handle;
							spawnMineMessage2.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineMessage2.Position = position;
							spawnMineMessage2.Direction = vector2;
							spawnMineMessage2.Scale = num7;
							spawnMineMessage2.Spell = iSpell;
							spawnMineMessage2.Damage = damage;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage2);
						}
						instance2.Initialize(iOwner, position, vector2, num7, num10, vector4, quaternion2, num9, ref iSpell, ref damage, animatedLevelPart);
						iOwner.PlayState.EntityManager.AddEntity(instance2);
						return;
					}
				}
			}
			else
			{
				NetworkState state3 = NetworkManager.Instance.State;
				if ((state3 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state3 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num12 = iSpell[Elements.Shield] + 3f;
					float num13 = num12 * 6.2831855f;
					float num14 = num13 * (1f / (Barrier.GetRadius((iSpell.Element & Elements.PhysicalElements) != Elements.None) * 2f));
					int num15 = (int)num14;
					float num16 = 1f + (num14 - (float)num15) / (float)num15;
					float num17 = 6.2831855f / (float)num15;
					Vector3 position2 = iOwner.Position;
					Vector3 direction2 = iOwner.Direction;
					Vector3.Multiply(ref direction2, num12, out direction2);
					Quaternion quaternion3;
					Quaternion.CreateFromYawPitchRoll(num17 * 0.5f, 0f, 0f, out quaternion3);
					Vector3 origin3;
					Vector3.Transform(ref direction2, ref quaternion3, out origin3);
					Vector3 vector6;
					Vector3.Normalize(ref origin3, out vector6);
					Vector3.Add(ref position2, ref origin3, out origin3);
					Quaternion.CreateFromYawPitchRoll(num17 * 1f, 0f, 0f, out quaternion3);
					Quaternion quaternion4;
					Quaternion.CreateFromYawPitchRoll(num17 * -0.5f, 0f, 0f, out quaternion4);
					Vector3 origin4;
					Vector3.Transform(ref direction2, ref quaternion4, out origin4);
					Vector3 vector7;
					Vector3.Normalize(ref origin4, out vector7);
					Vector3.Add(ref position2, ref origin4, out origin4);
					Quaternion.CreateFromYawPitchRoll(num17 * -1f, 0f, 0f, out quaternion4);
					Vector3 vector8;
					Vector3.Subtract(ref origin3, ref origin4, out vector8);
					Vector3 vector9;
					Vector3.Subtract(ref origin4, ref origin3, out vector9);
					float num18 = vector8.Length();
					Segment iSeg2 = default(Segment);
					iSeg2.Delta.Y = -4f;
					this.mSpell = iSpell;
					DamageCollection5 damage2;
					this.mSpell.CalculateDamage(SpellType.Shield, CastType.Area, out damage2);
					iSeg2.Origin = origin3;
					num13 -= num18;
					Barrier.HitListWithBarriers hitListWithBarriers = null;
					if (state3 != NetworkState.Client)
					{
						hitListWithBarriers = Barrier.HitListWithBarriers.GetFromCache();
					}
					bool flag = false;
					float num19 = num13 * 0.5f;
					float num20;
					Vector3 vector10;
					AnimatedLevelPart animatedLevelPart2;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num20, out position2, out vector10, out animatedLevelPart2, iSeg2))
					{
						if (state3 == NetworkState.Client)
						{
							SpawnBarrierRequestMessage spawnBarrierRequestMessage;
							spawnBarrierRequestMessage.OwnerHandle = iOwner.Handle;
							spawnBarrierRequestMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
							spawnBarrierRequestMessage.Position = position2;
							spawnBarrierRequestMessage.Direction = vector6;
							spawnBarrierRequestMessage.Scale = num16;
							spawnBarrierRequestMessage.Spell = iSpell;
							spawnBarrierRequestMessage.Damage = damage2;
							spawnBarrierRequestMessage.Range = num19;
							spawnBarrierRequestMessage.NextDir = vector8;
							spawnBarrierRequestMessage.NextRotation = quaternion3;
							spawnBarrierRequestMessage.Distance = num18;
							NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref spawnBarrierRequestMessage, 0);
						}
						else
						{
							Barrier fromCache2 = Barrier.GetFromCache(iOwner.PlayState);
							if (state3 == NetworkState.Server)
							{
								SpawnBarrierMessage spawnBarrierMessage;
								spawnBarrierMessage.Handle = fromCache2.Handle;
								spawnBarrierMessage.OwnerHandle = iOwner.Handle;
								spawnBarrierMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierMessage.Position = position2;
								spawnBarrierMessage.Direction = vector6;
								spawnBarrierMessage.Scale = num16;
								spawnBarrierMessage.Spell = iSpell;
								spawnBarrierMessage.Damage = damage2;
								spawnBarrierMessage.HitlistHandle = hitListWithBarriers.Handle;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage);
							}
							fromCache2.Initialize(iOwner, position2, vector6, num16, num19, vector8, quaternion3, num18, ref iSpell, ref damage2, hitListWithBarriers, animatedLevelPart2);
							iOwner.PlayState.EntityManager.AddEntity(fromCache2);
							flag = true;
						}
					}
					iSeg2.Origin = origin4;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num20, out position2, out vector10, out animatedLevelPart2, iSeg2))
					{
						if (state3 == NetworkState.Client)
						{
							SpawnBarrierRequestMessage spawnBarrierRequestMessage2;
							spawnBarrierRequestMessage2.OwnerHandle = iOwner.Handle;
							spawnBarrierRequestMessage2.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
							spawnBarrierRequestMessage2.Position = position2;
							spawnBarrierRequestMessage2.Direction = vector7;
							spawnBarrierRequestMessage2.Scale = num16;
							spawnBarrierRequestMessage2.Spell = iSpell;
							spawnBarrierRequestMessage2.Damage = damage2;
							spawnBarrierRequestMessage2.Range = num19;
							spawnBarrierRequestMessage2.NextDir = vector9;
							spawnBarrierRequestMessage2.NextRotation = quaternion4;
							spawnBarrierRequestMessage2.Distance = num18;
							NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref spawnBarrierRequestMessage2, 0);
						}
						else
						{
							Barrier fromCache3 = Barrier.GetFromCache(iOwner.PlayState);
							if (state3 == NetworkState.Server)
							{
								SpawnBarrierMessage spawnBarrierMessage2;
								spawnBarrierMessage2.Handle = fromCache3.Handle;
								spawnBarrierMessage2.OwnerHandle = iOwner.Handle;
								spawnBarrierMessage2.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierMessage2.Position = position2;
								spawnBarrierMessage2.Direction = vector7;
								spawnBarrierMessage2.Scale = num16;
								spawnBarrierMessage2.Spell = iSpell;
								spawnBarrierMessage2.Damage = damage2;
								spawnBarrierMessage2.HitlistHandle = hitListWithBarriers.Handle;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage2);
							}
							fromCache3.Initialize(iOwner, position2, vector7, num16, num19, vector9, quaternion4, num18, ref iSpell, ref damage2, hitListWithBarriers, animatedLevelPart2);
							iOwner.PlayState.EntityManager.AddEntity(fromCache3);
							flag = true;
						}
					}
					if (!flag && hitListWithBarriers != null)
					{
						hitListWithBarriers.Destroy();
					}
				}
				float iMagnitude = 0.25f + (float)MagickaMath.Random.NextDouble() * 0.5f;
				iOwner.PlayState.Camera.CameraShake(iMagnitude, 0.5f * iSpell[Elements.Shield]);
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000C1D0C File Offset: 0x000BFF0C
		public override void CastForce(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			Elements elements = iSpell.Element & ~Elements.Shield;
			this.mSpell = iSpell;
			base.CastForce(iSpell, iOwner, iFromStaff);
			this.mMinTTL = 0.5f;
			this.mHasCast = true;
			if (elements == Elements.None)
			{
				NetworkState state = NetworkManager.Instance.State;
				if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num = 5.5f;
					int num2 = 800;
					Vector3 shieldcolor = Spell.SHIELDCOLOR;
					Vector3 position = iOwner.Position;
					Vector3 direction = iOwner.Direction;
					Vector3.Multiply(ref direction, num - 2f, out direction);
					Vector3.Subtract(ref position, ref direction, out position);
					if (NetworkManager.Instance.State == NetworkState.Client)
					{
						SpawnShieldRequestMessage spawnShieldRequestMessage;
						spawnShieldRequestMessage.OwnerHandle = iOwner.Handle;
						spawnShieldRequestMessage.Position = position;
						spawnShieldRequestMessage.Radius = num;
						spawnShieldRequestMessage.Direction = iOwner.Direction;
						spawnShieldRequestMessage.ShieldType = ShieldType.DISC;
						spawnShieldRequestMessage.HitPoints = (float)num2;
						NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref spawnShieldRequestMessage, 0);
						return;
					}
					Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						SpawnShieldMessage spawnShieldMessage;
						spawnShieldMessage.Handle = fromCache.Handle;
						spawnShieldMessage.OwnerHandle = iOwner.Handle;
						spawnShieldMessage.Position = position;
						spawnShieldMessage.Radius = num;
						spawnShieldMessage.Direction = iOwner.Direction;
						spawnShieldMessage.ShieldType = ShieldType.DISC;
						spawnShieldMessage.HitPoints = (float)num2;
						NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref spawnShieldMessage);
					}
					fromCache.Initialize(iOwner, position, num, iOwner.Direction, ShieldType.DISC, (float)num2, shieldcolor);
					iOwner.PlayState.EntityManager.AddEntity(fromCache);
					return;
				}
			}
			else if ((elements & Elements.PhysicalElements) == Elements.None && ((elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life))
			{
				NetworkState state2 = NetworkManager.Instance.State;
				if ((state2 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state2 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num3 = 4f;
					float num4 = num3 * 6.2831855f;
					float num5 = num4 * 0.5f;
					int num6 = (int)num5;
					float num7 = 1f + (num5 - (float)num6) / (float)num6;
					float num8 = 6.2831855f / (float)num6;
					Vector3 position2 = iOwner.Position;
					Vector3 direction2 = iOwner.Direction;
					Vector3.Multiply(ref direction2, num3 - 3f, out direction2);
					Vector3.Subtract(ref position2, ref direction2, out position2);
					Vector3 direction3 = iOwner.Direction;
					Vector3.Multiply(ref direction3, num3, out direction3);
					Quaternion quaternion;
					Quaternion.CreateFromYawPitchRoll(num8 * 0.5f, 0f, 0f, out quaternion);
					Vector3 origin;
					Vector3.Transform(ref direction3, ref quaternion, out origin);
					Vector3 vector;
					Vector3.Normalize(ref origin, out vector);
					Vector3.Add(ref position2, ref origin, out origin);
					Quaternion.CreateFromYawPitchRoll(num8 * 1f, 0f, 0f, out quaternion);
					Quaternion quaternion2;
					Quaternion.CreateFromYawPitchRoll(num8 * -0.5f, 0f, 0f, out quaternion2);
					Vector3 origin2;
					Vector3.Transform(ref direction3, ref quaternion2, out origin2);
					Vector3 vector2;
					Vector3.Normalize(ref origin2, out vector2);
					Vector3.Add(ref position2, ref origin2, out origin2);
					Quaternion.CreateFromYawPitchRoll(num8 * -1f, 0f, 0f, out quaternion2);
					Vector3 vector3;
					Vector3.Subtract(ref origin, ref origin2, out vector3);
					Vector3 vector4;
					Vector3.Subtract(ref origin2, ref origin, out vector4);
					float num9 = vector3.Length();
					Segment iSeg = default(Segment);
					iSeg.Delta.Y = -4f;
					this.mSpell = iSpell;
					DamageCollection5 damage;
					this.mSpell.CalculateDamage(SpellType.Shield, CastType.Force, out damage);
					iSeg.Origin = origin;
					float num10 = num4 * 0.125f;
					float num11;
					Vector3 vector5;
					AnimatedLevelPart animatedLevelPart;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num11, out position2, out vector5, out animatedLevelPart, iSeg))
					{
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							SpawnMineRequestMessage spawnMineRequestMessage;
							spawnMineRequestMessage.OwnerHandle = iOwner.Handle;
							spawnMineRequestMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineRequestMessage.Position = position2;
							spawnMineRequestMessage.Direction = vector;
							spawnMineRequestMessage.Scale = num7;
							spawnMineRequestMessage.Spell = iSpell;
							spawnMineRequestMessage.Damage = damage;
							spawnMineRequestMessage.Range = num10;
							spawnMineRequestMessage.NextDir = vector3;
							spawnMineRequestMessage.NextRotation = quaternion;
							spawnMineRequestMessage.Distance = num9;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref spawnMineRequestMessage, 0);
						}
						else
						{
							SpellMine instance = SpellMine.GetInstance();
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								SpawnMineMessage spawnMineMessage;
								spawnMineMessage.Handle = instance.Handle;
								spawnMineMessage.OwnerHandle = iOwner.Handle;
								spawnMineMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
								spawnMineMessage.Position = position2;
								spawnMineMessage.Direction = vector;
								spawnMineMessage.Scale = num7;
								spawnMineMessage.Spell = iSpell;
								spawnMineMessage.Damage = damage;
								NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage);
							}
							instance.Initialize(iOwner, position2, vector, num7, num10, vector3, quaternion, num9, ref iSpell, ref damage, animatedLevelPart);
							iOwner.PlayState.EntityManager.AddEntity(instance);
						}
					}
					iSeg.Origin = origin2;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num11, out position2, out vector5, iSeg))
					{
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							SpawnMineRequestMessage spawnMineRequestMessage2;
							spawnMineRequestMessage2.OwnerHandle = iOwner.Handle;
							spawnMineRequestMessage2.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineRequestMessage2.Position = position2;
							spawnMineRequestMessage2.Direction = vector2;
							spawnMineRequestMessage2.Scale = num7;
							spawnMineRequestMessage2.Spell = iSpell;
							spawnMineRequestMessage2.Damage = damage;
							spawnMineRequestMessage2.Range = num10;
							spawnMineRequestMessage2.NextDir = vector4;
							spawnMineRequestMessage2.NextRotation = quaternion2;
							spawnMineRequestMessage2.Distance = num9;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref spawnMineRequestMessage2, 0);
							return;
						}
						SpellMine instance2 = SpellMine.GetInstance();
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							SpawnMineMessage spawnMineMessage2;
							spawnMineMessage2.Handle = instance2.Handle;
							spawnMineMessage2.OwnerHandle = iOwner.Handle;
							spawnMineMessage2.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
							spawnMineMessage2.Position = position2;
							spawnMineMessage2.Direction = vector2;
							spawnMineMessage2.Scale = num7;
							spawnMineMessage2.Spell = iSpell;
							spawnMineMessage2.Damage = damage;
							NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage2);
						}
						instance2.Initialize(iOwner, position2, vector2, num7, num10, vector4, quaternion2, num9, ref iSpell, ref damage, animatedLevelPart);
						iOwner.PlayState.EntityManager.AddEntity(instance2);
						return;
					}
				}
			}
			else
			{
				NetworkState state3 = NetworkManager.Instance.State;
				if ((state3 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state3 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
				{
					float num12 = 4f;
					float num13 = num12 * 6.2831855f;
					float num14 = num13 * (1f / (Barrier.GetRadius((iSpell.Element & Elements.PhysicalElements) != Elements.None) * 2f));
					int num15 = (int)num14;
					float num16 = 1f + (num14 - (float)num15) / (float)num15;
					float num17 = 6.2831855f / (float)num15;
					Vector3 position3 = iOwner.Position;
					if ((iSpell.Element & Elements.StatusEffects) == Elements.None && (iSpell.Element & Elements.Steam) == Elements.None && (iSpell.Element & Elements.Lightning) == Elements.None)
					{
						Vector3 direction4 = iOwner.Direction;
						Vector3.Multiply(ref direction4, num12 - 2f, out direction4);
						Vector3.Subtract(ref position3, ref direction4, out position3);
					}
					Vector3 direction5 = iOwner.Direction;
					Vector3.Multiply(ref direction5, num12, out direction5);
					Quaternion quaternion3;
					Quaternion.CreateFromYawPitchRoll(num17 * 0.5f, 0f, 0f, out quaternion3);
					Vector3 origin3;
					Vector3.Transform(ref direction5, ref quaternion3, out origin3);
					Vector3 vector6;
					Vector3.Normalize(ref origin3, out vector6);
					Vector3.Add(ref position3, ref origin3, out origin3);
					Quaternion.CreateFromYawPitchRoll(num17 * 1f, 0f, 0f, out quaternion3);
					Quaternion quaternion4;
					Quaternion.CreateFromYawPitchRoll(num17 * -0.5f, 0f, 0f, out quaternion4);
					Vector3 origin4;
					Vector3.Transform(ref direction5, ref quaternion4, out origin4);
					Vector3 vector7;
					Vector3.Normalize(ref origin4, out vector7);
					Vector3.Add(ref position3, ref origin4, out origin4);
					Quaternion.CreateFromYawPitchRoll(num17 * -1f, 0f, 0f, out quaternion4);
					Vector3 vector8;
					Vector3.Subtract(ref origin3, ref origin4, out vector8);
					Vector3 vector9;
					Vector3.Subtract(ref origin4, ref origin3, out vector9);
					float num18 = vector8.Length();
					Segment iSeg2 = default(Segment);
					iSeg2.Delta.Y = -4f;
					this.mSpell = iSpell;
					DamageCollection5 damage2;
					this.mSpell.CalculateDamage(SpellType.Shield, CastType.Force, out damage2);
					iSeg2.Origin = origin3;
					iSeg2.Origin.Y = iSeg2.Origin.Y + 1f;
					Barrier.HitListWithBarriers fromCache2 = Barrier.HitListWithBarriers.GetFromCache();
					bool flag = false;
					damage2.MultiplyMagnitude(0.25f);
					float num19 = num13 * 0.125f;
					float num20;
					Vector3 vector10;
					AnimatedLevelPart animatedLevelPart2;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num20, out position3, out vector10, out animatedLevelPart2, iSeg2))
					{
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							SpawnBarrierRequestMessage spawnBarrierRequestMessage;
							spawnBarrierRequestMessage.OwnerHandle = iOwner.Handle;
							spawnBarrierRequestMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
							spawnBarrierRequestMessage.Position = position3;
							spawnBarrierRequestMessage.Direction = vector6;
							spawnBarrierRequestMessage.Scale = num16;
							spawnBarrierRequestMessage.Spell = iSpell;
							spawnBarrierRequestMessage.Damage = damage2;
							spawnBarrierRequestMessage.Range = num19;
							spawnBarrierRequestMessage.NextDir = vector8;
							spawnBarrierRequestMessage.NextRotation = quaternion3;
							spawnBarrierRequestMessage.Distance = num18;
							NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref spawnBarrierRequestMessage, 0);
						}
						else
						{
							Barrier fromCache3 = Barrier.GetFromCache(iOwner.PlayState);
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								SpawnBarrierMessage spawnBarrierMessage;
								spawnBarrierMessage.Handle = fromCache3.Handle;
								spawnBarrierMessage.OwnerHandle = iOwner.Handle;
								spawnBarrierMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierMessage.Position = position3;
								spawnBarrierMessage.Direction = vector6;
								spawnBarrierMessage.Scale = num16;
								spawnBarrierMessage.Spell = iSpell;
								spawnBarrierMessage.Damage = damage2;
								spawnBarrierMessage.HitlistHandle = fromCache2.Handle;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage);
							}
							flag = true;
							fromCache3.Initialize(iOwner, position3, vector6, num16, num19, vector8, quaternion3, num18, ref iSpell, ref damage2, fromCache2, animatedLevelPart2);
							iOwner.PlayState.EntityManager.AddEntity(fromCache3);
						}
					}
					iSeg2.Origin = origin4;
					if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num20, out position3, out vector10, out animatedLevelPart2, iSeg2))
					{
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							SpawnBarrierRequestMessage spawnBarrierRequestMessage2;
							spawnBarrierRequestMessage2.OwnerHandle = iOwner.Handle;
							spawnBarrierRequestMessage2.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
							spawnBarrierRequestMessage2.Position = position3;
							spawnBarrierRequestMessage2.Direction = vector7;
							spawnBarrierRequestMessage2.Scale = num16;
							spawnBarrierRequestMessage2.Spell = iSpell;
							spawnBarrierRequestMessage2.Damage = damage2;
							spawnBarrierRequestMessage2.Range = num19;
							spawnBarrierRequestMessage2.NextDir = vector9;
							spawnBarrierRequestMessage2.NextRotation = quaternion4;
							spawnBarrierRequestMessage2.Distance = num18;
							NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref spawnBarrierRequestMessage2, 0);
						}
						else
						{
							Barrier fromCache4 = Barrier.GetFromCache(iOwner.PlayState);
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								SpawnBarrierMessage spawnBarrierMessage2;
								spawnBarrierMessage2.Handle = fromCache4.Handle;
								spawnBarrierMessage2.OwnerHandle = iOwner.Handle;
								spawnBarrierMessage2.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierMessage2.Position = position3;
								spawnBarrierMessage2.Direction = vector7;
								spawnBarrierMessage2.Scale = num16;
								spawnBarrierMessage2.Spell = iSpell;
								spawnBarrierMessage2.Damage = damage2;
								spawnBarrierMessage2.HitlistHandle = fromCache2.Handle;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage2);
							}
							flag = true;
							fromCache4.Initialize(iOwner, position3, vector7, num16, num13 * 0.125f, vector9, quaternion4, num18, ref iSpell, ref damage2, fromCache2, animatedLevelPart2);
							iOwner.PlayState.EntityManager.AddEntity(fromCache4);
						}
					}
					if (!flag)
					{
						fromCache2.Destroy();
					}
					float iMagnitude = 0.25f + (float)MagickaMath.Random.NextDouble() * 0.5f;
					iOwner.PlayState.Camera.CameraShake(iMagnitude, 0.5f * iSpell[Elements.Shield]);
				}
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000C298F File Offset: 0x000C0B8F
		public override void CastWeapon(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.mSpell = iSpell;
			this.mHasCast = false;
			base.CastWeapon(iSpell, iOwner, iFromStaff);
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000C29A8 File Offset: 0x000C0BA8
		internal override void AnimationEnd(ISpellCaster iOwner)
		{
			base.AnimationEnd(iOwner);
			Vector3 vector = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			float radius = iOwner.Radius;
			Vector3.Multiply(ref direction, radius, out direction);
			Vector3.Add(ref direction, ref vector, out vector);
			direction = iOwner.Direction;
			if (this.mCastType == CastType.Weapon)
			{
				Elements elements = this.mSpell.Element & ~Elements.Shield;
				this.mHasCast = true;
				if (elements == Elements.None)
				{
					NetworkState state = NetworkManager.Instance.State;
					if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
					{
						float num = 5.5f;
						int num2 = 800;
						Vector3 shieldcolor = Spell.SHIELDCOLOR;
						Vector3.Multiply(ref direction, num + 2f, out direction);
						Vector3.Add(ref vector, ref direction, out vector);
						if (NetworkManager.Instance.State == NetworkState.Client)
						{
							SpawnShieldRequestMessage spawnShieldRequestMessage;
							spawnShieldRequestMessage.OwnerHandle = iOwner.Handle;
							spawnShieldRequestMessage.Position = vector;
							spawnShieldRequestMessage.Radius = num;
							spawnShieldRequestMessage.Direction = iOwner.Direction;
							spawnShieldRequestMessage.ShieldType = ShieldType.WALL;
							spawnShieldRequestMessage.HitPoints = (float)num2;
							NetworkManager.Instance.Interface.SendMessage<SpawnShieldRequestMessage>(ref spawnShieldRequestMessage, 0);
							return;
						}
						Shield fromCache = Shield.GetFromCache(iOwner.PlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							SpawnShieldMessage spawnShieldMessage;
							spawnShieldMessage.Handle = fromCache.Handle;
							spawnShieldMessage.OwnerHandle = iOwner.Handle;
							spawnShieldMessage.Position = vector;
							spawnShieldMessage.Radius = num;
							spawnShieldMessage.Direction = iOwner.Direction;
							spawnShieldMessage.ShieldType = ShieldType.WALL;
							spawnShieldMessage.HitPoints = (float)num2;
							NetworkManager.Instance.Interface.SendMessage<SpawnShieldMessage>(ref spawnShieldMessage);
						}
						fromCache.Initialize(iOwner, vector, num, iOwner.Direction, ShieldType.WALL, (float)num2, shieldcolor);
						iOwner.PlayState.EntityManager.AddEntity(fromCache);
						return;
					}
				}
				else if ((elements & Elements.PhysicalElements) == Elements.None && ((elements & Elements.Arcane) == Elements.Arcane | (elements & Elements.Life) == Elements.Life))
				{
					NetworkState state2 = NetworkManager.Instance.State;
					if ((state2 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state2 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
					{
						int num3 = 2 + (int)Math.Max(this.mSpell.TotalMagnitude() * 0.5f, 1f);
						float num4 = 2.5f;
						float num5 = 1f;
						float num6 = num4 * (float)num3;
						float num7 = num4;
						vector += direction * num4;
						DamageCollection5 damage;
						this.mSpell.CalculateDamage(SpellType.Shield, CastType.Weapon, out damage);
						Segment iSeg = default(Segment);
						iSeg.Delta.Y = -4f;
						iSeg.Origin = vector;
						iSeg.Origin.Y = iSeg.Origin.Y + 1f;
						float num8;
						Vector3 vector2;
						Vector3 vector3;
						AnimatedLevelPart animatedLevelPart;
						if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num8, out vector2, out vector3, out animatedLevelPart, iSeg))
						{
							if (NetworkManager.Instance.State == NetworkState.Client)
							{
								SpawnMineRequestMessage spawnMineRequestMessage;
								spawnMineRequestMessage.OwnerHandle = iOwner.Handle;
								spawnMineRequestMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
								spawnMineRequestMessage.Position = vector2;
								spawnMineRequestMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
								spawnMineRequestMessage.Scale = num5;
								spawnMineRequestMessage.Spell = this.mSpell;
								spawnMineRequestMessage.Damage = damage;
								spawnMineRequestMessage.Range = num6;
								spawnMineRequestMessage.NextDir = direction * num7;
								spawnMineRequestMessage.NextRotation = Quaternion.Identity;
								spawnMineRequestMessage.Distance = num7;
								NetworkManager.Instance.Interface.SendMessage<SpawnMineRequestMessage>(ref spawnMineRequestMessage, 0);
								return;
							}
							SpellMine instance = SpellMine.GetInstance();
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								SpawnMineMessage spawnMineMessage;
								spawnMineMessage.Handle = instance.Handle;
								spawnMineMessage.OwnerHandle = iOwner.Handle;
								spawnMineMessage.AnimationHandle = ((animatedLevelPart == null) ? ushort.MaxValue : animatedLevelPart.Handle);
								spawnMineMessage.Position = vector2;
								spawnMineMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
								spawnMineMessage.Scale = num5;
								spawnMineMessage.Spell = this.mSpell;
								spawnMineMessage.Damage = damage;
								NetworkManager.Instance.Interface.SendMessage<SpawnMineMessage>(ref spawnMineMessage);
							}
							instance.Initialize(iOwner, vector2, new Vector3(direction.Z, 0f, -direction.X), num5, num6, direction * num7, Quaternion.Identity, num7, ref this.mSpell, ref damage, animatedLevelPart);
							iOwner.PlayState.EntityManager.AddEntity(instance);
							return;
						}
					}
				}
				else
				{
					NetworkState state3 = NetworkManager.Instance.State;
					if ((state3 != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state3 == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
					{
						int num9 = 2 + (int)Math.Max(this.mSpell.TotalMagnitude() * 0.5f, 1f);
						float num10 = 2.5f;
						float num11 = 1f;
						float num12 = num10 * (float)num9;
						float num13 = num10;
						vector += direction * num10;
						DamageCollection5 damage2;
						this.mSpell.CalculateDamage(SpellType.Shield, CastType.Weapon, out damage2);
						Segment iSeg2 = default(Segment);
						iSeg2.Delta.Y = -4f;
						iSeg2.Origin = vector;
						iSeg2.Origin.Y = iSeg2.Origin.Y + 2f;
						float num14;
						Vector3 vector4;
						Vector3 vector5;
						AnimatedLevelPart animatedLevelPart2;
						if (iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num14, out vector4, out vector5, out animatedLevelPart2, iSeg2))
						{
							if (NetworkManager.Instance.State == NetworkState.Client)
							{
								SpawnBarrierRequestMessage spawnBarrierRequestMessage;
								spawnBarrierRequestMessage.OwnerHandle = iOwner.Handle;
								spawnBarrierRequestMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierRequestMessage.Position = vector4;
								spawnBarrierRequestMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
								spawnBarrierRequestMessage.Scale = num11;
								spawnBarrierRequestMessage.Spell = this.mSpell;
								spawnBarrierRequestMessage.Damage = damage2;
								spawnBarrierRequestMessage.Range = num12;
								spawnBarrierRequestMessage.NextDir = direction * num13;
								spawnBarrierRequestMessage.NextRotation = Quaternion.Identity;
								spawnBarrierRequestMessage.Distance = num13;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierRequestMessage>(ref spawnBarrierRequestMessage, 0);
								return;
							}
							Barrier fromCache2 = Barrier.GetFromCache(iOwner.PlayState);
							Barrier.HitListWithBarriers fromCache3 = Barrier.HitListWithBarriers.GetFromCache();
							if (NetworkManager.Instance.State == NetworkState.Server)
							{
								SpawnBarrierMessage spawnBarrierMessage;
								spawnBarrierMessage.Handle = fromCache2.Handle;
								spawnBarrierMessage.OwnerHandle = iOwner.Handle;
								spawnBarrierMessage.AnimationHandle = ((animatedLevelPart2 == null) ? ushort.MaxValue : animatedLevelPart2.Handle);
								spawnBarrierMessage.Position = vector4;
								spawnBarrierMessage.Direction = new Vector3(direction.Z, 0f, -direction.X);
								spawnBarrierMessage.Scale = num11;
								spawnBarrierMessage.Spell = this.mSpell;
								spawnBarrierMessage.Damage = damage2;
								spawnBarrierMessage.HitlistHandle = fromCache3.Handle;
								NetworkManager.Instance.Interface.SendMessage<SpawnBarrierMessage>(ref spawnBarrierMessage);
							}
							fromCache2.Initialize(iOwner, vector4, new Vector3(direction.Z, 0f, -direction.X), num11, num12, direction * num13, Quaternion.Identity, num13, ref this.mSpell, ref damage2, fromCache3, animatedLevelPart2);
							iOwner.PlayState.EntityManager.AddEntity(fromCache2);
						}
					}
				}
			}
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000C3170 File Offset: 0x000C1370
		public override void CastSelf(Spell iSpell, ISpellCaster iOwner, bool iFromStaff)
		{
			this.mSpell = iSpell;
			Elements elements = iSpell.Element & ~Elements.Shield;
			base.CastSelf(iSpell, iOwner, iFromStaff);
			this.mMinTTL = 0.5f;
			this.mHasCast = true;
			if (elements == Elements.None)
			{
				if (NetworkManager.Instance.State != NetworkState.Client)
				{
					if (NetworkManager.Instance.State == NetworkState.Server)
					{
						CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
						characterActionMessage.Action = ActionType.SelfShield;
						characterActionMessage.Handle = iOwner.Handle;
						characterActionMessage.Param0I = (int)iSpell.Element;
						characterActionMessage.Param1F = iSpell[Elements.Earth];
						characterActionMessage.Param2F = iSpell[Elements.Ice];
						NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
					}
					iOwner.AddSelfShield(iSpell);
					return;
				}
			}
			else
			{
				bool flag = false;
				if ((elements & Elements.PhysicalElements) != Elements.None)
				{
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							CharacterActionMessage characterActionMessage2 = default(CharacterActionMessage);
							characterActionMessage2.Action = ActionType.SelfShield;
							characterActionMessage2.Handle = iOwner.Handle;
							characterActionMessage2.Param0I = (int)iSpell.Element;
							characterActionMessage2.Param1F = iSpell[Elements.Earth];
							characterActionMessage2.Param2F = iSpell[Elements.Ice];
							NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage2);
						}
						iOwner.AddSelfShield(iSpell);
					}
					flag = true;
				}
				if ((elements & (Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Steam)) != Elements.None && iOwner is Character)
				{
					float num = 1f;
					for (int i = 0; i < 11; i++)
					{
						Elements elements2 = Spell.ElementFromIndex(i);
						if ((elements2 & elements) == elements2)
						{
							num = Math.Max(this.mSpell[elements2], num);
						}
					}
					float iTTL = 15f + 5f * num;
					float iRadius = 0f + 1.5f * num;
					Resistance iResistance = default(Resistance);
					iResistance.Multiplier = 0f;
					if (!flag)
					{
						(iOwner as Character).RemoveSelfShield();
					}
					(iOwner as Character).ClearAura();
					if ((elements & Elements.Lightning) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Lightning;
						BuffStorage iBuff = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.LIGHTNINGCOLOR);
						int[] iTargetType = new int[1];
						AuraStorage auraStorage = new AuraStorage(new AuraBuff(iBuff), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.LIGHTNINGCOLOR, iTargetType, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage, true);
					}
					if ((elements & Elements.Fire) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Fire;
						BuffStorage iBuff2 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.FIRECOLOR);
						int[] iTargetType2 = new int[1];
						AuraStorage auraStorage2 = new AuraStorage(new AuraBuff(iBuff2), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.FIRECOLOR, iTargetType2, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage2, true);
					}
					if ((elements & Elements.Cold) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Cold;
						BuffStorage iBuff3 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.COLDCOLOR);
						int[] iTargetType3 = new int[1];
						AuraStorage auraStorage3 = new AuraStorage(new AuraBuff(iBuff3), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.COLDCOLOR, iTargetType3, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage3, true);
					}
					if ((elements & Elements.Steam) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Steam;
						BuffStorage iBuff4 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.STEAMCOLOR);
						int[] iTargetType4 = new int[1];
						AuraStorage auraStorage4 = new AuraStorage(new AuraBuff(iBuff4), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.STEAMCOLOR, iTargetType4, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage4, true);
					}
					if ((elements & Elements.Water) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Water;
						BuffStorage iBuff5 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.WATERCOLOR);
						int[] iTargetType5 = new int[1];
						AuraStorage auraStorage5 = new AuraStorage(new AuraBuff(iBuff5), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.WATERCOLOR, iTargetType5, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage5, true);
					}
					if ((elements & Elements.Life) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Life;
						BuffStorage iBuff6 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.LIFECOLOR);
						int[] iTargetType6 = new int[1];
						AuraStorage auraStorage6 = new AuraStorage(new AuraBuff(iBuff6), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.LIFECOLOR, iTargetType6, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage6, true);
					}
					if ((elements & Elements.Arcane) != Elements.None)
					{
						iResistance.ResistanceAgainst = Elements.Arcane;
						BuffStorage iBuff7 = new BuffStorage(new BuffResistance(iResistance), VisualCategory.Defensive, Spell.ARCANECOLOR);
						int[] iTargetType7 = new int[1];
						AuraStorage auraStorage7 = new AuraStorage(new AuraBuff(iBuff7), AuraTarget.All, AuraType.Buff, 0, iTTL, iRadius, VisualCategory.Defensive, Spell.ARCANECOLOR, iTargetType7, Factions.NONE);
						(iOwner as Character).AddAura(ref auraStorage7, true);
					}
				}
			}
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000C35F0 File Offset: 0x000C17F0
		public override bool CastUpdate(float iDeltaTime, ISpellCaster iOwner, out float oTurnSpeed)
		{
			oTurnSpeed = 0.1f;
			this.mMinTTL -= iDeltaTime;
			if (base.Active && (iOwner.AnimationController.HasFinished || this.mMinTTL < 0f))
			{
				this.DeInitialize(iOwner);
				return false;
			}
			return false;
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x000C363E File Offset: 0x000C183E
		public override void DeInitialize(ISpellCaster iOwner)
		{
			if (!base.Active)
			{
				return;
			}
			if (!this.mHasCast)
			{
				this.AnimationEnd(iOwner);
			}
			ShieldSpell.ReturnToCache(this);
			base.Active = false;
		}

		// Token: 0x04001E8E RID: 7822
		private static List<ShieldSpell> sCache;

		// Token: 0x04001E8F RID: 7823
		private bool mHasCast;
	}
}
