using System;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200066F RID: 1647
	public class Fireflask : SpecialAbility
	{
		// Token: 0x060031BD RID: 12733 RVA: 0x00198C2C File Offset: 0x00196E2C
		public Fireflask(Animations iAnimation) : base(iAnimation, "#specab_fireflask".GetHashCodeCustom())
		{
			if (Fireflask.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					Fireflask.sModel = Game.Instance.Content.Load<Model>("Models/Items_Wizard/weapon_fireflask");
				}
			}
		}

		// Token: 0x060031BE RID: 12734 RVA: 0x00198C94 File Offset: 0x00196E94
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				Vector3 translation = iOwner.CastSource.Translation;
				if (iOwner is Character)
				{
					translation = (iOwner as Character).GetLeftAttachOrientation().Translation;
				}
				Vector3 direction = iOwner.Direction;
				direction.Y = 1f;
				direction.Normalize();
				Vector3.Multiply(ref direction, 13f, out direction);
				MissileEntity missileInstance = iOwner.GetMissileInstance();
				Fireflask.SpawnGrenade(ref missileInstance, iOwner, ref translation, ref direction);
				missileInstance.FacingVelocity = false;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.FireFlask;
					spawnMissileMessage.Handle = missileInstance.Handle;
					spawnMissileMessage.Item = 0;
					spawnMissileMessage.Owner = iOwner.Handle;
					spawnMissileMessage.Position = translation;
					spawnMissileMessage.Velocity = direction;
					NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060031BF RID: 12735 RVA: 0x00198DD8 File Offset: 0x00196FD8
		internal static void SpawnGrenade(ref MissileEntity iMissile, ISpellCaster iOwner, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Fire, 470f, 1f);
			Damage iDamage2 = new Damage(AttackProperties.Status, Elements.Fire, 30f, 1f);
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Damaged;
			conditionCollection[0].Condition.Elements = (Elements.Fire | Elements.Lightning | Elements.Arcane);
			conditionCollection[0].Condition.Hitpoints = 20f;
			conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(Fireflask.EFFECT, false, true)));
			conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fireflask.SOUND, false)));
			conditionCollection[0].Add(new EventStorage(new BlastEvent(2f, iDamage2)));
			conditionCollection[0].Add(new EventStorage(new BlastEvent(2f, iDamage)));
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[1].Condition.Time = 3f;
			conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(Fireflask.EFFECT, false, true)));
			conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fireflask.SOUND, false)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(2f, iDamage2)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(2f, iDamage)));
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[3].Condition.EventConditionType = EventConditionType.Collision;
			conditionCollection[3].Condition.Threshold = 20f;
			conditionCollection[3].Add(new EventStorage(new PlayEffectEvent(Fireflask.EFFECT, false)));
			conditionCollection[3].Add(new EventStorage(new PlaySoundEvent(Banks.Spells, Fireflask.SOUND, false)));
			conditionCollection[3].Add(new EventStorage(new BlastEvent(2f, iDamage2)));
			conditionCollection[3].Add(new EventStorage(new BlastEvent(2f, iDamage)));
			conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[2].Condition.Repeat = true;
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(Fireflask.PULSE, true)));
			iMissile.Initialize(iOwner as Entity, Fireflask.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, Fireflask.sModel, conditionCollection, false);
			iMissile.Body.AngularVelocity = new Vector3(0f, 0f, 2f * iMissile.Body.Mass);
			iMissile.Danger = 30f;
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x04003628 RID: 13864
		private static Model sModel;

		// Token: 0x04003629 RID: 13865
		private static readonly int EFFECT = "fireflask_explosion".GetHashCodeCustom();

		// Token: 0x0400362A RID: 13866
		private static readonly int SOUND = "spell_fire_force".GetHashCodeCustom();

		// Token: 0x0400362B RID: 13867
		private static readonly int SOUND2 = "spell_fire_projectile".GetHashCodeCustom();

		// Token: 0x0400362C RID: 13868
		private static readonly int PULSE = "vietnam_grenade_trail".GetHashCodeCustom();
	}
}
