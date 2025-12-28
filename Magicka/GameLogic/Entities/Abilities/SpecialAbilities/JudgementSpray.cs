using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004DF RID: 1247
	public class JudgementSpray : SpecialAbility, IAbilityEffect
	{
		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06002508 RID: 9480 RVA: 0x0010B57C File Offset: 0x0010977C
		public static JudgementSpray Instance
		{
			get
			{
				if (JudgementSpray.sSingelton == null)
				{
					lock (JudgementSpray.sSingeltonLock)
					{
						if (JudgementSpray.sSingelton == null)
						{
							JudgementSpray.sSingelton = new JudgementSpray();
						}
					}
				}
				return JudgementSpray.sSingelton;
			}
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x0010B5D8 File Offset: 0x001097D8
		public JudgementSpray() : base(Animations.cast_magick_global, "#specab_tsal_antioch".GetHashCodeCustom())
		{
			if (JudgementSpray.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					JudgementSpray.sModel = Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
				}
			}
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x0010B678 File Offset: 0x00109878
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				this.mOwner = iOwner;
				this.mTTL = this.TTL;
				SpellManager.Instance.AddSpellEffect(this);
				AudioManager.Instance.PlayCue(Banks.Additional, JudgementSpray.SOUND_FIRE, iOwner.AudioEmitter);
				return true;
			}
			return false;
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x0010B71C File Offset: 0x0010991C
		internal static void SpawnProjectile(ref MissileEntity m, ISpellCaster iOwner, ref Entity iTarget, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Cold, JudgementSpray.COLD_DAMAGE, 1f);
			Damage iDamage2 = new Damage(AttackProperties.Status, Elements.Cold, JudgementSpray.STATUS_DAMAGE, 1f);
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Damaged;
			conditionCollection[0].Condition.Elements = (Elements.Fire | Elements.Lightning | Elements.Arcane);
			conditionCollection[0].Condition.Hitpoints = 20f;
			conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
			conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
			conditionCollection[0].Add(new EventStorage(new DamageEvent(iDamage)));
			conditionCollection[0].Add(new EventStorage(new DamageEvent(iDamage2)));
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
			conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
			conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
			conditionCollection[1].Add(new EventStorage(new DamageEvent(iDamage)));
			conditionCollection[1].Add(new EventStorage(new DamageEvent(iDamage2)));
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[3].Condition.EventConditionType = EventConditionType.Collision;
			conditionCollection[3].Condition.Threshold = 0f;
			conditionCollection[3].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false)));
			conditionCollection[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
			conditionCollection[3].Add(new EventStorage(new DamageEvent(iDamage)));
			conditionCollection[3].Add(new EventStorage(new DamageEvent(iDamage2)));
			conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[4].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[4].Condition.Time = JudgementSpray.PROJECTILE_TTL;
			conditionCollection[4].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.EFFECT, false, true)));
			conditionCollection[4].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, JudgementSpray.SOUND_HIT, false)));
			conditionCollection[4].Add(new EventStorage(new DamageEvent(iDamage)));
			conditionCollection[4].Add(new EventStorage(new DamageEvent(iDamage2)));
			conditionCollection[4].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[2].Condition.Repeat = true;
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.PULSE, true)));
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(JudgementSpray.MISSILE, true)));
			m.Initialize(iOwner as Entity, iTarget, JudgementSpray.HOMING_EFFICIENCY, JudgementSpray.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, JudgementSpray.sModel, conditionCollection, false);
			m.Body.AngularVelocity = new Vector3(0f, 0f, 2f * m.Body.Mass);
			m.Danger = 30f;
			iOwner.PlayState.EntityManager.AddEntity(m);
			JudgementSpray.mMissiles.Add(m);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
			m.FacingVelocity = false;
			NetworkState state = NetworkManager.Instance.State;
			if (state != NetworkState.Offline && ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer))))
			{
				SpawnMissileMessage spawnMissileMessage;
				if (iTarget != null)
				{
					spawnMissileMessage = new SpawnMissileMessage
					{
						Type = SpawnMissileMessage.MissileType.JudgementMissile,
						Handle = m.Handle,
						Item = 0,
						Owner = iOwner.Handle,
						Target = iTarget.Handle,
						Position = iPosition,
						Velocity = iVelocity
					};
				}
				else
				{
					spawnMissileMessage = new SpawnMissileMessage
					{
						Type = SpawnMissileMessage.MissileType.JudgementMissile,
						Handle = m.Handle,
						Item = 0,
						Owner = iOwner.Handle,
						Target = 0,
						Position = iPosition,
						Velocity = iVelocity
					};
				}
				NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x0600250C RID: 9484 RVA: 0x0010BCA4 File Offset: 0x00109EA4
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x0600250D RID: 9485 RVA: 0x0010BCB8 File Offset: 0x00109EB8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mOwner == null)
			{
				return;
			}
			this.mTTL -= iDeltaTime;
			this.mInterval -= iDeltaTime;
			if (this.mInterval < 0f)
			{
				this.mInterval = this.TTL / (float)this.NUM_OF_PROJECTILES;
				List<Entity> entities = this.mOwner.PlayState.EntityManager.GetEntities(this.mOwner.Position, this.RANGE, false);
				Entity entity = null;
				float num = float.MaxValue;
				Vector3 position = this.mOwner.Position;
				foreach (Entity entity2 in entities)
				{
					if (entity2 is Character && entity2 != this.mOwner)
					{
						Vector3 position2 = entity2.Position;
						Vector3 vector = this.mOwner.Position - position2;
						vector.Normalize();
						Vector3 direction = this.mOwner.Direction;
						float num2 = MagickaMath.Angle(ref vector, ref direction);
						if (num2 < 3.926991f && num2 > 1.5707964f)
						{
							float num3;
							Vector3.Distance(ref position2, ref position, out num3);
							float num4 = this.OPTIMAL_DISTANCE - num3;
							if (Math.Abs(num4) < num)
							{
								entity = entity2;
								num = num4;
							}
						}
					}
				}
				Vector3 translation = this.mOwner.CastSource.Translation;
				if (this.mOwner is Character)
				{
					translation = (this.mOwner as Character).GetLeftAttachOrientation().Translation;
				}
				Vector3 direction2 = this.mOwner.Direction;
				Vector3 vector2 = new Vector3(0f, 1f, 0f);
				Vector3 vector3 = new Vector3(0f, -1f, 0f);
				this.mLeft = !this.mLeft;
				direction2.Y = 1f;
				direction2.Normalize();
				Vector3 vector4;
				if (this.mLeft)
				{
					Vector3.Cross(ref vector2, ref direction2, out vector4);
				}
				else
				{
					Vector3.Cross(ref vector3, ref direction2, out vector4);
				}
				float scaleFactor = (float)((double)(this.SPEED / 2f) + (0.5 + MagickaMath.Random.NextDouble()) * (double)this.SPEED / 2.0);
				Vector3.Multiply(ref vector4, scaleFactor, out vector4);
				MissileEntity missileInstance = this.mOwner.GetMissileInstance();
				JudgementSpray.SpawnProjectile(ref missileInstance, this.mOwner, ref entity, ref translation, ref vector4);
				this.mOwner.PlayState.EntityManager.ReturnEntityList(entities);
			}
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0010BF50 File Offset: 0x0010A150
		public void OnRemove()
		{
		}

		// Token: 0x0400285D RID: 10333
		private static volatile JudgementSpray sSingelton;

		// Token: 0x0400285E RID: 10334
		private static volatile object sSingeltonLock = new object();

		// Token: 0x0400285F RID: 10335
		private static List<MissileEntity> mMissiles = new List<MissileEntity>(128);

		// Token: 0x04002860 RID: 10336
		private static Model sModel;

		// Token: 0x04002861 RID: 10337
		private readonly float OPTIMAL_DISTANCE = 12f;

		// Token: 0x04002862 RID: 10338
		private readonly float RANGE = 20f;

		// Token: 0x04002863 RID: 10339
		private readonly float SPEED = 30f;

		// Token: 0x04002864 RID: 10340
		private readonly float TTL = 1f;

		// Token: 0x04002865 RID: 10341
		private static readonly float PROJECTILE_TTL = 2f;

		// Token: 0x04002866 RID: 10342
		private readonly int NUM_OF_PROJECTILES = 20;

		// Token: 0x04002867 RID: 10343
		private static float HOMING_EFFICIENCY = 0.55f;

		// Token: 0x04002868 RID: 10344
		private static readonly float COLD_DAMAGE = 273f;

		// Token: 0x04002869 RID: 10345
		private static readonly float STATUS_DAMAGE = 100f;

		// Token: 0x0400286A RID: 10346
		private ISpellCaster mOwner;

		// Token: 0x0400286B RID: 10347
		private float mTTL;

		// Token: 0x0400286C RID: 10348
		private float mInterval;

		// Token: 0x0400286D RID: 10349
		private bool mLeft;

		// Token: 0x0400286E RID: 10350
		private static readonly int EFFECT = "magick_judgementspray_hit".GetHashCodeCustom();

		// Token: 0x0400286F RID: 10351
		private static readonly int SOUND_HIT = "wep_handgrenade_explode".GetHashCodeCustom();

		// Token: 0x04002870 RID: 10352
		private static readonly int SOUND_FIRE = "woot_magick_judgment_fire".GetHashCodeCustom();

		// Token: 0x04002871 RID: 10353
		private static readonly int PULSE = "magick_judgementspray_trail".GetHashCodeCustom();

		// Token: 0x04002872 RID: 10354
		private static readonly int MISSILE = "magick_judgementspray_missile".GetHashCodeCustom();
	}
}
