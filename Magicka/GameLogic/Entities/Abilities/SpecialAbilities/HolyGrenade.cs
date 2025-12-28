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
	// Token: 0x0200025A RID: 602
	public class HolyGrenade : SpecialAbility
	{
		// Token: 0x06001290 RID: 4752 RVA: 0x000728D0 File Offset: 0x00070AD0
		public HolyGrenade(Animations iAnimation) : base(iAnimation, "#specab_tsal_antioch".GetHashCodeCustom())
		{
			if (HolyGrenade.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					HolyGrenade.sModel = Game.Instance.Content.Load<Model>("Models/Missiles/holy_handgrenade");
				}
			}
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x00072938 File Offset: 0x00070B38
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
				HolyGrenade.SpawnGrenade(ref missileInstance, iOwner, ref translation, ref direction);
				missileInstance.FacingVelocity = false;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.HolyGrenade;
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

		// Token: 0x06001292 RID: 4754 RVA: 0x00072A7C File Offset: 0x00070C7C
		internal static void SpawnGrenade(ref MissileEntity iMissile, ISpellCaster iOwner, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Earth | Elements.Fire, 800f, 2f);
			Damage iDamage2 = new Damage(AttackProperties.Knockback, Elements.Earth, 750f, 2f);
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Damaged;
			conditionCollection[0].Condition.Elements = (Elements.Fire | Elements.Lightning | Elements.Arcane);
			conditionCollection[0].Condition.Hitpoints = 20f;
			conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(HolyGrenade.EFFECT, false, true)));
			conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, HolyGrenade.SOUND, false)));
			conditionCollection[0].Add(new EventStorage(new BlastEvent(6f, iDamage2)));
			conditionCollection[0].Add(new EventStorage(new BlastEvent(6f, iDamage)));
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[1].Condition.Time = 3f;
			conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(HolyGrenade.EFFECT, false, true)));
			conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, HolyGrenade.SOUND, false)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(6f, iDamage2)));
			conditionCollection[1].Add(new EventStorage(new BlastEvent(6f, iDamage)));
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[2].Condition.Repeat = true;
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(HolyGrenade.PULSE, true)));
			iMissile.Initialize(iOwner as Entity, HolyGrenade.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, HolyGrenade.sModel, conditionCollection, false);
			iMissile.Body.AngularVelocity = new Vector3(0f, 0f, 2f * iMissile.Body.Mass);
			iMissile.Danger = 30f;
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x0400114E RID: 4430
		private static Model sModel;

		// Token: 0x0400114F RID: 4431
		private static readonly int EFFECT = "holy_explosion".GetHashCodeCustom();

		// Token: 0x04001150 RID: 4432
		private static readonly int SOUND = "wep_handgrenade_explode".GetHashCodeCustom();

		// Token: 0x04001151 RID: 4433
		private static readonly int SOUND2 = "goblin_bomb_explosion".GetHashCodeCustom();

		// Token: 0x04001152 RID: 4434
		private static readonly int PULSE = "vietnam_grenade_trail".GetHashCodeCustom();
	}
}
