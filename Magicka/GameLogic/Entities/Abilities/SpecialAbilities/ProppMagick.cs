using System;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000365 RID: 869
	internal class ProppMagick : SpecialAbility
	{
		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06001A82 RID: 6786 RVA: 0x000B4318 File Offset: 0x000B2518
		public static ProppMagick Instance
		{
			get
			{
				if (ProppMagick.sSingelton == null)
				{
					lock (ProppMagick.sSingeltonLock)
					{
						if (ProppMagick.sSingelton == null)
						{
							ProppMagick.sSingelton = new ProppMagick();
						}
					}
				}
				return ProppMagick.sSingelton;
			}
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x000B436C File Offset: 0x000B256C
		static ProppMagick()
		{
			ProppMagick.sDamage = default(DamageCollection5);
			ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Arcane, 6000f, 1f));
			ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Damage, Elements.Fire, 2000f, 1f));
			ProppMagick.sDamage.AddDamage(new Damage(AttackProperties.Knockback, Elements.Earth, 200f, 3f));
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x000B442D File Offset: 0x000B262D
		private ProppMagick() : base(Animations.cast_spell1, "magick_proppmagick".GetHashCodeCustom())
		{
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x000B4444 File Offset: 0x000B2644
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Vector3 translation = iOwner.CastSource.Translation;
				translation.Y += 0.5f;
				MissileEntity missileInstance = iOwner.GetMissileInstance();
				Vector3 velocity = default(Vector3);
				float radians = (float)(SpecialAbility.RANDOM.NextDouble() - 0.5) * 1.5707964f;
				Vector3 direction = iOwner.Direction;
				Matrix matrix;
				Matrix.CreateRotationY(radians, out matrix);
				float num = 10f + (float)SpecialAbility.RANDOM.NextDouble() * 18f;
				velocity.Y = 18f;
				Vector3.TransformNormal(ref direction, ref matrix, out direction);
				velocity.X = direction.X * (float)Math.Sqrt((double)num);
				velocity.Z = direction.Z * (float)Math.Sqrt((double)num);
				float num2 = 6.2831855f * (float)SpecialAbility.RANDOM.NextDouble();
				float num3 = (float)Math.Acos((double)(2f * (float)SpecialAbility.RANDOM.NextDouble() - 1f));
				Vector3 angularVelocity = new Vector3((float)Math.Cos((double)num2) * (float)Math.Sin((double)num3), (float)Math.Sin((double)num2) * (float)Math.Sin((double)num3), (float)Math.Cos((double)num3));
				Vector3.Multiply(ref angularVelocity, 15f, out angularVelocity);
				num2 = 6.2831855f * (float)SpecialAbility.RANDOM.NextDouble();
				num3 = (float)Math.Acos((double)(2f * (float)SpecialAbility.RANDOM.NextDouble() - 1f));
				Vector3 lever = new Vector3((float)Math.Cos((double)num2) * (float)Math.Sin((double)num3), (float)Math.Sin((double)num2) * (float)Math.Sin((double)num3), (float)Math.Cos((double)num3));
				ProppMagick.Spawn(ref missileInstance, iOwner, ref translation, ref velocity, ref angularVelocity, ref lever);
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.ProppMagick;
					spawnMissileMessage.Handle = missileInstance.Handle;
					spawnMissileMessage.Item = 0;
					spawnMissileMessage.Owner = iOwner.Handle;
					spawnMissileMessage.Position = translation;
					spawnMissileMessage.Velocity = velocity;
					spawnMissileMessage.AngularVelocity = angularVelocity;
					spawnMissileMessage.Lever = lever;
					NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
				}
			}
			return false;
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x000B46E0 File Offset: 0x000B28E0
		internal static void Spawn(ref MissileEntity iMissile, ISpellCaster iOwner, ref Vector3 iPosition, ref Vector3 iVelocity, ref Vector3 iAngularVelocity, ref Vector3 iLever)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Collision;
			conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
			conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[0].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
			conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
			conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[2].Condition.Time = 8f;
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(ProppMagick.EXPLOSION_EFFECT, false, true)));
			conditionCollection[2].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.EXPLOSION_SOUND, false)));
			conditionCollection[2].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(4f, ProppMagick.sDamage)));
			conditionCollection[3].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[3].Condition.Repeat = true;
			conditionCollection[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, ProppMagick.SPELL_SOUND, true)));
			iMissile.Initialize(iOwner as Entity, 0.6f, ref iPosition, ref iVelocity, null, conditionCollection, false);
			iMissile.Body.AngularVelocity = iAngularVelocity;
			Vector3 vector = iLever;
			iMissile.SetProppMagickEffect(ProppMagick.SPELL_EFFECT, ref vector);
			iMissile.Danger = 30f;
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x04001CD4 RID: 7380
		private static ProppMagick sSingelton;

		// Token: 0x04001CD5 RID: 7381
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04001CD6 RID: 7382
		private static DamageCollection5 sDamage;

		// Token: 0x04001CD7 RID: 7383
		private static readonly int EXPLOSION_EFFECT = "magick_propp_explosion".GetHashCodeCustom();

		// Token: 0x04001CD8 RID: 7384
		private static readonly int SPELL_EFFECT = "magick_propp_spell".GetHashCodeCustom();

		// Token: 0x04001CD9 RID: 7385
		private static readonly int CHARGE_EFFECT = "magick_propp_charge".GetHashCodeCustom();

		// Token: 0x04001CDA RID: 7386
		private static readonly int SPELL_SOUND = "magick_propps_plasma_loop".GetHashCodeCustom();

		// Token: 0x04001CDB RID: 7387
		private static readonly int EXPLOSION_SOUND = "magick_propps_plasma_explosion".GetHashCodeCustom();
	}
}
