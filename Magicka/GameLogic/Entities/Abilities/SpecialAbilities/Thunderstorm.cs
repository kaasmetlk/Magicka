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
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000610 RID: 1552
	public class Thunderstorm : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06002E8A RID: 11914 RVA: 0x001798CC File Offset: 0x00177ACC
		public static Thunderstorm Instance
		{
			get
			{
				if (Thunderstorm.mSingelton == null)
				{
					lock (Thunderstorm.mSingeltonLock)
					{
						if (Thunderstorm.mSingelton == null)
						{
							Thunderstorm.mSingelton = new Thunderstorm();
						}
					}
				}
				return Thunderstorm.mSingelton;
			}
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x00179920 File Offset: 0x00177B20
		private Thunderstorm() : base(Animations.cast_magick_global, "#magick_thunders".GetHashCodeCustom())
		{
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x0017994C File Offset: 0x00177B4C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (iPlayState.Level.CurrentScene.Indoors)
			{
				return false;
			}
			this.mTimeStamp = 0.0;
			this.mPerfectStorm = true;
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				this.mPlayersAliveAtStart[i] = false;
				if (Game.Instance.Players[i].Playing && Game.Instance.Players[i].Avatar != null)
				{
					this.mPlayersAliveAtStart[i] = !Game.Instance.Players[i].Avatar.Dead;
					if (Game.Instance.Players[i].Avatar.Dead)
					{
						this.mPerfectStorm = false;
					}
				}
			}
			this.mPosition = iPosition;
			this.mOwner = null;
			this.mPlayState = iPlayState;
			this.mIndoor = this.mPlayState.Level.CurrentScene.Indoors;
			this.mRain.Execute(iPosition, iPlayState);
			this.mBoltTTL = 2f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mAmbience != null && !this.mAmbience.IsPlaying)
			{
				this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.AMBIENCE);
			}
			return true;
		}

		// Token: 0x06002E8D RID: 11917 RVA: 0x00179A90 File Offset: 0x00177C90
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (iPlayState.Level.CurrentScene.Indoors)
			{
				return false;
			}
			this.mTimeStamp = iOwner.PlayState.PlayTime;
			this.mPerfectStorm = true;
			for (int i = 0; i < Game.Instance.Players.Length; i++)
			{
				this.mPlayersAliveAtStart[i] = false;
				if (Game.Instance.Players[i].Playing && Game.Instance.Players[i].Avatar != null)
				{
					this.mPlayersAliveAtStart[i] = !Game.Instance.Players[i].Avatar.Dead;
					if (Game.Instance.Players[i].Avatar.Dead)
					{
						this.mPerfectStorm = false;
					}
				}
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			this.mIndoor = this.mPlayState.Level.CurrentScene.Indoors;
			this.mRain.Execute(iOwner, iPlayState);
			this.mBoltTTL = 2f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mAmbience != null && !this.mAmbience.IsPlaying)
			{
				this.mAmbience = AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.AMBIENCE);
			}
			return true;
		}

		// Token: 0x06002E8E RID: 11918 RVA: 0x00179BD8 File Offset: 0x00177DD8
		public void CheckPerfectStorm()
		{
			if (!this.IsDead)
			{
				for (int i = 0; i < Game.Instance.Players.Length; i++)
				{
					if (Game.Instance.Players[i].Playing && Game.Instance.Players[i].Avatar != null && this.mPlayersAliveAtStart[i] == Game.Instance.Players[i].Avatar.Dead)
					{
						this.mPerfectStorm = false;
					}
				}
			}
		}

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x06002E8F RID: 11919 RVA: 0x00179C51 File Offset: 0x00177E51
		public bool IsDead
		{
			get
			{
				return this.mRain.IsDead;
			}
		}

		// Token: 0x06002E90 RID: 11920 RVA: 0x00179C60 File Offset: 0x00177E60
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mIndoor)
			{
				return;
			}
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mBoltTTL -= iDeltaTime;
				this.CheckPerfectStorm();
				if (this.mBoltTTL <= 0f)
				{
					Vector3 vector = (this.mOwner != null) ? this.mOwner.Position : this.mPosition;
					this.mBoltTTL = 0.5f + (float)SpecialAbility.RANDOM.NextDouble() * 0.5f;
					int num = SpecialAbility.RANDOM.Next(2) + 1;
					for (int i = 0; i < num; i++)
					{
						Vector3 vector2 = vector;
						float num2 = (float)Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
						float num3 = (float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f;
						float num4 = (float)((double)num2 * Math.Cos((double)num3));
						float num5 = (float)((double)num2 * Math.Sin((double)num3));
						vector2.X += 20f * num4;
						vector2.Z += 20f * num5;
						Vector3 vector3;
						this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector2, out vector3, MovementProperties.All);
						vector2 = vector3;
						IDamageable damageable = null;
						float num6 = float.MinValue;
						List<Entity> entities = this.mPlayState.EntityManager.GetEntities(vector2, 5f, true, true);
						for (int j = 0; j < entities.Count; j++)
						{
							IDamageable damageable2 = entities[j] as IDamageable;
							if (damageable2 != null && (!(entities[j] is Character) || !(entities[j] as Character).IsEthereal) && (!(entities[j] is BossDamageZone) || !((entities[j] as BossDamageZone).Owner is Grimnir2)) && !(entities[j] is MissileEntity))
							{
								float y = entities[j].Body.CollisionSkin.WorldBoundingBox.Max.Y;
								if (y > num6)
								{
									damageable = (entities[j] as IDamageable);
									num6 = y;
								}
							}
						}
						this.mPlayState.EntityManager.ReturnEntityList(entities);
						Flash.Instance.Execute(this.mPlayState.Scene, 0.125f);
						LightningBolt lightning = LightningBolt.GetLightning();
						Vector3 vector4 = new Vector3(0f, -1f, 0f);
						Vector3 lightningcolor = Spell.LIGHTNINGCOLOR;
						float iScale = 1f;
						if (damageable != null)
						{
							vector2 = damageable.Position;
							vector = vector2;
							vector.Y += 20f;
							Shield shield = damageable as Shield;
							if (shield != null)
							{
								if (shield.ShieldType == ShieldType.SPHERE)
								{
									vector2.Y += damageable.Body.CollisionSkin.WorldBoundingBox.Max.Y * 0.5f;
								}
								else
								{
									vector2 += shield.Body.Orientation.Forward * shield.Radius;
								}
							}
							damageable.Damage(Thunderstorm.sDamage, this.mOwner as Entity, this.mTimeStamp, vector);
							if (damageable is Avatar && damageable.HitPoints > 0f && !((damageable as Avatar).Player.Gamer is NetworkGamer))
							{
								AchievementsManager.Instance.AwardAchievement(this.mPlayState, "oneinamillion");
							}
						}
						Vector3 position = this.mPlayState.Camera.Position;
						vector = vector2;
						vector.Y += 40f;
						lightning.InitializeEffect(ref vector, ref vector4, ref vector2, ref position, ref lightningcolor, false, iScale, 1f, this.mPlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.ThunderBolt;
							if (this.mOwner != null)
							{
								triggerActionMessage.Handle = this.mOwner.Handle;
							}
							if (damageable != null)
							{
								triggerActionMessage.Id = (int)damageable.Handle;
							}
							triggerActionMessage.Position = vector2;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
						}
						Vector3 right = Vector3.Right;
						VisualEffectReference visualEffectReference;
						EffectManager.Instance.StartEffect(Thunderstorm.EFFECT, ref vector2, ref right, out visualEffectReference);
						if (!(damageable is Shield))
						{
							Segment iSeg = default(Segment);
							iSeg.Origin = vector2;
							iSeg.Delta.Y = iSeg.Delta.Y - 10f;
							float num7;
							Vector3 vector5;
							Vector3 vector6;
							AnimatedLevelPart iAnimation;
							if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num7, out vector5, out vector6, out iAnimation, iSeg))
							{
								vector2 = vector5;
								DecalManager.Instance.AddAlphaBlendedDecal(Decal.Scorched, iAnimation, 4f, ref vector2, ref vector6, 60f);
							}
						}
						Thunderstorm.mAudioEmitter.Position = vector;
						Thunderstorm.mAudioEmitter.Up = Vector3.Up;
						Thunderstorm.mAudioEmitter.Forward = Vector3.Right;
						AudioManager.Instance.PlayCue(Banks.Spells, Thunderstorm.SOUND, Thunderstorm.mAudioEmitter);
						this.mPlayState.Camera.CameraShake(vector, 1.2f, 0.333f);
					}
				}
			}
		}

		// Token: 0x06002E91 RID: 11921 RVA: 0x0017A174 File Offset: 0x00178374
		public void OnRemove()
		{
			this.mBoltTTL = 0f;
			if (this.mPerfectStorm)
			{
				AchievementsManager.Instance.AwardAchievement(this.mPlayState, "theperfectstorm");
			}
			if (this.mAmbience != null && !this.mAmbience.IsStopping)
			{
				this.mAmbience.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x04003296 RID: 12950
		private const float TIME_BETWEEN_BOLTS = 0.5f;

		// Token: 0x04003297 RID: 12951
		private const float RANGE = 5f;

		// Token: 0x04003298 RID: 12952
		private static Thunderstorm mSingelton;

		// Token: 0x04003299 RID: 12953
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400329A RID: 12954
		private bool[] mPlayersAliveAtStart = new bool[4];

		// Token: 0x0400329B RID: 12955
		private bool mPerfectStorm;

		// Token: 0x0400329C RID: 12956
		protected static readonly int EFFECT = Thunderbolt.EFFECT;

		// Token: 0x0400329D RID: 12957
		protected static readonly int SOUND = Thunderbolt.SOUND;

		// Token: 0x0400329E RID: 12958
		protected static readonly int AMBIENCE = "magick_thunderstorm".GetHashCodeCustom();

		// Token: 0x0400329F RID: 12959
		public static readonly Damage sDamage = Thunderbolt.sDamage;

		// Token: 0x040032A0 RID: 12960
		protected static AudioEmitter mAudioEmitter = new AudioEmitter();

		// Token: 0x040032A1 RID: 12961
		private float mBoltTTL;

		// Token: 0x040032A2 RID: 12962
		private Rain mRain = Rain.Instance;

		// Token: 0x040032A3 RID: 12963
		private bool mIndoor;

		// Token: 0x040032A4 RID: 12964
		private ISpellCaster mOwner;

		// Token: 0x040032A5 RID: 12965
		private Vector3 mPosition;

		// Token: 0x040032A6 RID: 12966
		private PlayState mPlayState;

		// Token: 0x040032A7 RID: 12967
		private Cue mAmbience;
	}
}
