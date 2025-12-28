using System;
using System.Collections.Generic;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000408 RID: 1032
	public class GreaseLump : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001FCE RID: 8142 RVA: 0x000DF2A8 File Offset: 0x000DD4A8
		public static GreaseLump GetInstance()
		{
			if (GreaseLump.sCache.Count > 0)
			{
				GreaseLump result = GreaseLump.sCache[GreaseLump.sCache.Count - 1];
				GreaseLump.sCache.RemoveAt(GreaseLump.sCache.Count - 1);
				return result;
			}
			return new GreaseLump();
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x000DF2F8 File Offset: 0x000DD4F8
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			GreaseLump.sCache = new List<GreaseLump>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				GreaseLump.sCache.Add(new GreaseLump());
			}
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x000DF32C File Offset: 0x000DD52C
		public GreaseLump(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
			if (GreaseLump.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					GreaseLump.sModel = Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
				}
			}
		}

		// Token: 0x06001FD1 RID: 8145 RVA: 0x000DF394 File Offset: 0x000DD594
		private GreaseLump() : base(Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
		{
			if (GreaseLump.sModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					GreaseLump.sModel = Game.Instance.Content.Load<Model>("Models/Missiles/JudgementSprayMissile");
				}
			}
		}

		// Token: 0x06001FD2 RID: 8146 RVA: 0x000DF400 File Offset: 0x000DD600
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Grease can not be cast without an owner!");
		}

		// Token: 0x06001FD3 RID: 8147 RVA: 0x000DF40C File Offset: 0x000DD60C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				this.mTTL = 1.2f;
				SpellManager.Instance.AddSpellEffect(this);
				this.mOwner = (iOwner as Character);
				this.mPlayState = iPlayState;
				Vector3 translation = iOwner.CastSource.Translation;
				if (iOwner is Character)
				{
					translation = (iOwner as Character).GetLeftAttachOrientation().Translation;
				}
				Vector3 direction = iOwner.Direction;
				direction.Y = 0.5f;
				direction.Normalize();
				Vector3.Multiply(ref direction, 15f, out direction);
				this.mMissile = iOwner.GetMissileInstance();
				GreaseLump.SpawnLump(ref this.mMissile, iOwner, ref translation, ref direction);
				this.mMissile.FacingVelocity = false;
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
					spawnMissileMessage.Type = SpawnMissileMessage.MissileType.GreaseLump;
					spawnMissileMessage.Handle = this.mMissile.Handle;
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

		// Token: 0x06001FD4 RID: 8148 RVA: 0x000DF58C File Offset: 0x000DD78C
		internal static void SpawnLump(ref MissileEntity iMissile, ISpellCaster iOwner, ref Vector3 iPosition, ref Vector3 iVelocity)
		{
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			Damage iDamage = new Damage(AttackProperties.Damage, Elements.Earth, 100f, 1f);
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Collision;
			conditionCollection[0].Condition.Threshold = 0f;
			conditionCollection[0].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
			conditionCollection[0].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
			conditionCollection[0].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
			conditionCollection[0].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Hit;
			conditionCollection[1].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
			conditionCollection[1].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
			conditionCollection[1].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
			conditionCollection[1].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
			conditionCollection[1].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[3].Condition.EventConditionType = EventConditionType.Damaged;
			conditionCollection[3].Condition.Elements = Elements.Fire;
			conditionCollection[3].Condition.Hitpoints = 20f;
			conditionCollection[3].Add(new EventStorage(new PlayEffectEvent(GreaseLump.EFFECT, false, true)));
			conditionCollection[3].Add(new EventStorage(new PlaySoundEvent(Banks.Additional, GreaseLump.SOUND, false)));
			conditionCollection[3].Add(new EventStorage(new SplashEvent(iDamage, 3f)));
			conditionCollection[3].Add(new EventStorage(new CallbackEvent(new CallbackEvent.ItemCallbackFn(GreaseLump.OnCollision))));
			conditionCollection[3].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[2].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[2].Condition.Repeat = true;
			conditionCollection[2].Add(new EventStorage(new PlayEffectEvent(GreaseLump.PULSE, true)));
			iMissile.Initialize(iOwner as Entity, GreaseLump.sModel.Meshes[0].BoundingSphere.Radius * 0.75f, ref iPosition, ref iVelocity, GreaseLump.sModel, conditionCollection, false);
			iMissile.Body.AngularVelocity = new Vector3(0f, 0f, 2f * iMissile.Body.Mass);
			iMissile.Danger = 10f;
			iOwner.PlayState.EntityManager.AddEntity(iMissile);
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06001FD5 RID: 8149 RVA: 0x000DF90C File Offset: 0x000DDB0C
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06001FD6 RID: 8150 RVA: 0x000DF920 File Offset: 0x000DDB20
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mInterval -= iDeltaTime;
			if (this.mInterval < 0f)
			{
				this.mInterval = 0.15f;
				Vector3 position = this.mMissile.Position;
				position.Y = 0f;
				Vector3 up = Vector3.Up;
				AnimatedLevelPart animatedLevelPart = null;
				Grease.GreaseField instance = Grease.GreaseField.GetInstance(this.mPlayState);
				instance.Initialize(this.mOwner, animatedLevelPart, ref position, ref up);
				this.mPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Position = position;
					triggerActionMessage.Direction = up;
					if (animatedLevelPart != null)
					{
						triggerActionMessage.Arg = (int)animatedLevelPart.Handle;
					}
					else
					{
						triggerActionMessage.Arg = 65535;
					}
					triggerActionMessage.Id = (int)this.mOwner.Handle;
					triggerActionMessage.ActionType = TriggerActionType.SpawnGrease;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x000DFA2E File Offset: 0x000DDC2E
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			GreaseLump.sCache.Add(this);
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x000DFA58 File Offset: 0x000DDC58
		public static void OnCollision(Entity iItem, Entity iTarget)
		{
			Character character = iTarget as Character;
			if (character != null)
			{
				character.AddStatusEffect(new StatusEffect(StatusEffects.Greased, 10f, 1f, 1f, 1f));
			}
			Vector3 direction = iItem.Direction;
			Vector3 up = Vector3.Up;
			AnimatedLevelPart animatedLevelPart = null;
			for (int i = 0; i < GreaseLump.NUM_OF_FIELDS; i++)
			{
				float yaw = 3.1415927f / (float)GreaseLump.NUM_OF_FIELDS * (float)i;
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
				Vector3.Transform(ref direction, ref quaternion, out direction);
				float scaleFactor = (float)(1.0 + MagickaMath.Random.NextDouble() * 3.0);
				Vector3 vector;
				Vector3.Multiply(ref direction, scaleFactor, out vector);
				vector = iItem.Position + vector;
				vector.Y = 0f;
				Grease.GreaseField instance = Grease.GreaseField.GetInstance(iItem.PlayState);
				instance.Initialize((iItem as MissileEntity).Owner as ISpellCaster, animatedLevelPart, ref vector, ref up);
				iItem.PlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Position = vector;
					triggerActionMessage.Direction = direction;
					if (animatedLevelPart != null)
					{
						triggerActionMessage.Arg = (int)animatedLevelPart.Handle;
					}
					else
					{
						triggerActionMessage.Arg = 65535;
					}
					triggerActionMessage.Id = (int)(iItem as MissileEntity).Owner.Handle;
					triggerActionMessage.ActionType = TriggerActionType.SpawnGrease;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x0400221E RID: 8734
		private static List<GreaseLump> sCache;

		// Token: 0x0400221F RID: 8735
		public static readonly int EFFECT = "dungeons_grease_ball_impact".GetHashCodeCustom();

		// Token: 0x04002220 RID: 8736
		public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();

		// Token: 0x04002221 RID: 8737
		private static Model sModel;

		// Token: 0x04002222 RID: 8738
		private MissileEntity mMissile;

		// Token: 0x04002223 RID: 8739
		private Character mOwner;

		// Token: 0x04002224 RID: 8740
		public PlayState mPlayState;

		// Token: 0x04002225 RID: 8741
		private VisualEffectReference mEffect;

		// Token: 0x04002226 RID: 8742
		private Cue mCue;

		// Token: 0x04002227 RID: 8743
		private float mTTL;

		// Token: 0x04002228 RID: 8744
		private float mInterval;

		// Token: 0x04002229 RID: 8745
		private static readonly int NUM_OF_FIELDS = 0;

		// Token: 0x0400222A RID: 8746
		private static readonly int SOUND = "dungeon_slime_cube_death_big".GetHashCodeCustom();

		// Token: 0x0400222B RID: 8747
		private static readonly int PULSE = "dungeons_grease_ball".GetHashCodeCustom();
	}
}
