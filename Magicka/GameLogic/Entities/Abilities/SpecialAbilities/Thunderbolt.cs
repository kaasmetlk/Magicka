using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Bosses;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000253 RID: 595
	public class Thunderbolt : SpecialAbility
	{
		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001254 RID: 4692 RVA: 0x00070370 File Offset: 0x0006E570
		public static Thunderbolt Instance
		{
			get
			{
				if (Thunderbolt.mSingelton == null)
				{
					lock (Thunderbolt.mSingeltonLock)
					{
						if (Thunderbolt.mSingelton == null)
						{
							Thunderbolt.mSingelton = new Thunderbolt();
						}
					}
				}
				return Thunderbolt.mSingelton;
			}
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x000703C4 File Offset: 0x0006E5C4
		static Thunderbolt()
		{
			Thunderbolt.sDamage = default(Damage);
			Thunderbolt.sDamage.Amount = 5000f;
			Thunderbolt.sDamage.AttackProperty = (AttackProperties.Damage | AttackProperties.Knockdown | AttackProperties.Pushed);
			Thunderbolt.sDamage.Element = Elements.Lightning;
			Thunderbolt.sDamage.Magnitude = 1f;
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x00070445 File Offset: 0x0006E645
		private Thunderbolt() : base(Animations.cast_magick_direct, "#magick_thunderb".GetHashCodeCustom())
		{
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x0007045C File Offset: 0x0006E65C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			Vector3 iDirection = new Vector3((float)SpecialAbility.RANDOM.NextDouble(), 0f, (float)SpecialAbility.RANDOM.NextDouble());
			iDirection.Normalize();
			return this.Execute(iPosition, iDirection, null);
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x000704A2 File Offset: 0x0006E6A2
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mPlayState = iPlayState;
			return this.Execute(iOwner.Position, iOwner.Direction, iOwner);
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x000704C8 File Offset: 0x0006E6C8
		private bool Execute(Vector3 iPosition, Vector3 iDirection, ISpellCaster iOwner)
		{
			if (this.mPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL, iOwner.AudioEmitter);
				return false;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mTimeStamp = iOwner.PlayState.PlayTime;
				float num = 16f;
				Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
				Vector3 vector = iDirection;
				Vector3 origin = iPosition;
				origin.Y = 0f;
				Vector3 vector2;
				Vector3.Multiply(ref vector, num * 0.5f, out vector2);
				Vector3.Add(ref vector2, ref origin, out vector2);
				List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector2, num * 0.5f, false, true);
				entities.Remove(iOwner as Entity);
				bool flag = false;
				IDamageable damageable = null;
				Shield shield;
				if (this.mPlayState.EntityManager.IsProtectedByShield(iOwner as Entity, out shield))
				{
					flag = true;
					damageable = shield;
				}
				Vector3 vector3 = default(Vector3);
				Vector3 right = Vector3.Right;
				Segment segment = default(Segment);
				segment.Origin = origin;
				segment.Delta.Y = segment.Delta.Y + 22f;
				if (!flag)
				{
					segment.Origin = vector2;
					List<IDamageable> list = new List<IDamageable>(entities.Count);
					for (int i = 0; i < entities.Count; i++)
					{
						IDamageable damageable2 = entities[i] as IDamageable;
						if (damageable2 != null && (!(entities[i] is Character) || !(entities[i] as Character).IsEthereal) && (!(entities[i] is BossDamageZone) || !(entities[i] as BossDamageZone).IsEthereal) && !(entities[i] is MissileEntity) && !this.mPlayState.EntityManager.IsProtectedByShield(entities[i], out shield))
						{
							list.Add(damageable2);
						}
					}
					this.mPlayState.EntityManager.ReturnEntityList(entities);
					float num2 = float.MinValue;
					for (int j = 0; j < list.Count; j++)
					{
						float y = list[j].Body.CollisionSkin.WorldBoundingBox.Max.Y;
						if (y > num2)
						{
							damageable = list[j];
							num2 = y;
						}
					}
				}
				LightningBolt lightning = LightningBolt.GetLightning();
				Vector3 vector4 = new Vector3(0f, -1f, 0f);
				Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
				Vector3 position = this.mPlayState.Scene.Camera.Position;
				float iScale = 1f;
				if (damageable != null)
				{
					vector3 = damageable.Position;
					shield = (damageable as Shield);
					if (shield != null)
					{
						if (shield.ShieldType == ShieldType.SPHERE)
						{
							vector3.Y += damageable.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
						}
						else
						{
							vector3 += shield.Body.Orientation.Forward * shield.Radius;
						}
					}
					damageable.Damage(Thunderbolt.sDamage, iOwner as Entity, this.mTimeStamp, vector2);
					if (damageable is Avatar && damageable.HitPoints > 0f && !((damageable as Avatar).Player.Gamer is NetworkGamer))
					{
						AchievementsManager.Instance.AwardAchievement(this.mPlayState, "oneinamillion");
					}
				}
				else
				{
					vector2.X += (float)(MagickaMath.Random.NextDouble() - 0.5) * (num * 0.5f);
					vector2.Z += (float)(MagickaMath.Random.NextDouble() - 0.5) * (num * 0.5f);
					vector3 = vector2;
					vector4 = Vector3.Right;
				}
				vector2.Y += 40f;
				lightning.InitializeEffect(ref vector2, ref vector4, ref vector3, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.ThunderBolt;
					if (iOwner != null)
					{
						triggerActionMessage.Handle = iOwner.Handle;
					}
					if (damageable != null)
					{
						triggerActionMessage.Id = (int)damageable.Handle;
					}
					triggerActionMessage.Position = vector3;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
				VisualEffectReference visualEffectReference;
				EffectManager.Instance.StartEffect(Thunderbolt.EFFECT, ref vector3, ref right, out visualEffectReference);
				if (!(damageable is Shield))
				{
					Segment iSeg = default(Segment);
					iSeg.Origin = vector3;
					iSeg.Origin.Y = iSeg.Origin.Y + 1f;
					iSeg.Delta.Y = iSeg.Delta.Y - 10f;
					float num3;
					Vector3 vector5;
					Vector3 vector6;
					AnimatedLevelPart iAnimation;
					if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num3, out vector5, out vector6, out iAnimation, iSeg))
					{
						vector3 = vector5;
						DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, iAnimation, 4f, ref vector3, ref vector6, 60f);
					}
				}
				Thunderbolt.sAudioEmitter.Position = vector3;
				Thunderbolt.sAudioEmitter.Up = Vector3.Up;
				Thunderbolt.sAudioEmitter.Forward = Vector3.Right;
				AudioManager.Instance.PlayCue(Banks.Spells, Thunderbolt.SOUND, Thunderbolt.sAudioEmitter);
				this.mPlayState.Camera.CameraShake(vector3, 1.5f, 0.333f);
			}
			return true;
		}

		// Token: 0x0400110C RID: 4364
		public const float RANGE = 16f;

		// Token: 0x0400110D RID: 4365
		private static Thunderbolt mSingelton;

		// Token: 0x0400110E RID: 4366
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400110F RID: 4367
		public static readonly int EFFECT = "magick_thunderbolt".GetHashCodeCustom();

		// Token: 0x04001110 RID: 4368
		public static readonly int SOUND = "magick_thunderbolt".GetHashCodeCustom();

		// Token: 0x04001111 RID: 4369
		public static readonly Damage sDamage;

		// Token: 0x04001112 RID: 4370
		public static AudioEmitter sAudioEmitter = new AudioEmitter();

		// Token: 0x04001113 RID: 4371
		private PlayState mPlayState;

		// Token: 0x04001114 RID: 4372
		private new double mTimeStamp;
	}
}
