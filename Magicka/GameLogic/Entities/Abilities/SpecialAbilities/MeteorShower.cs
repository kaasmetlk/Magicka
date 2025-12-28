using System;
using Magicka.Audio;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020005AC RID: 1452
	public class MeteorShower : SpecialAbility, IAbilityEffect
	{
		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06002B78 RID: 11128 RVA: 0x00156FB4 File Offset: 0x001551B4
		public static MeteorShower Instance
		{
			get
			{
				if (MeteorShower.mSingelton == null)
				{
					lock (MeteorShower.mSingeltonLock)
					{
						if (MeteorShower.mSingelton == null)
						{
							MeteorShower.mSingelton = new MeteorShower();
						}
					}
				}
				return MeteorShower.mSingelton;
			}
		}

		// Token: 0x06002B79 RID: 11129 RVA: 0x00157008 File Offset: 0x00155208
		private MeteorShower() : base(Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
		{
		}

		// Token: 0x06002B7A RID: 11130 RVA: 0x0015701C File Offset: 0x0015521C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (iPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
				return false;
			}
			if (this.mTTL > 0f)
			{
				this.mPosition = iPosition;
				this.mTTL = 17.5f;
				return true;
			}
			this.mPosition = iPosition;
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute();
		}

		// Token: 0x06002B7B RID: 11131 RVA: 0x0015708C File Offset: 0x0015528C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (iPlayState.Level.CurrentScene.Indoors)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, SpecialAbility.SOUND_MAGICK_FAIL);
				return false;
			}
			if (this.mTTL > 0f)
			{
				this.mTTL = 17.5f;
				return true;
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			return this.Execute();
		}

		// Token: 0x06002B7C RID: 11132 RVA: 0x001570F8 File Offset: 0x001552F8
		private bool Execute()
		{
			this.mScene = this.mPlayState.Level.CurrentScene;
			this.mTTL = 17.5f;
			this.mScene.LightTargetIntensity = 0.4f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mRumble != null && !this.mRumble.IsPlaying)
			{
				this.mRumble = AudioManager.Instance.PlayCue(Banks.Spells, MeteorShower.SOUND_RUMBLE);
			}
			return true;
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06002B7D RID: 11133 RVA: 0x0015716E File Offset: 0x0015536E
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002B7E RID: 11134 RVA: 0x00157180 File Offset: 0x00155380
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (NetworkManager.Instance.State != NetworkState.Client && this.mTTL > 2.5f)
			{
				this.mMeteorSpawnTimer -= iDeltaTime;
				if (this.mMeteorSpawnTimer <= 0f)
				{
					AudioManager.Instance.PlayCue(Banks.Spells, "magick_meteor_preblast".GetHashCodeCustom());
					Vector3 vector = (this.mOwner != null) ? this.mOwner.Position : this.mPosition;
					this.mMeteorSpawnTimer += 0.4f + (float)SpecialAbility.RANDOM.NextDouble() * 0.4f;
					int num = SpecialAbility.RANDOM.Next(2) + 1;
					for (int i = 0; i < num; i++)
					{
						Vector3 position = vector;
						float num2 = (float)Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
						float num3 = (float)(SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
						float num4 = (float)((double)num2 * Math.Cos((double)num3));
						float num5 = (float)((double)num2 * Math.Sin((double)num3));
						position.X += 20f * num4;
						position.Z += 20f * num5;
						position.Y += 146f;
						Vector3 velocity = new Vector3(0.01f, -20f, -0.01f);
						Spell spell = default(Spell);
						spell.Element = (Elements.Earth | Elements.Fire);
						spell.EarthMagnitude = 5f;
						spell.FireMagnitude = 5f;
						MissileEntity missileEntity;
						if (this.mOwner != null)
						{
							missileEntity = this.mOwner.GetMissileInstance();
						}
						else
						{
							missileEntity = MissileEntity.GetInstance(this.mPlayState);
						}
						ProjectileSpell.SpawnMissile(ref missileEntity, this.mOwner, 0f, ref position, ref velocity, ref spell, 4f, 1);
						AudioManager.Instance.PlayCue(Banks.Spells, MeteorShower.SOUND_PREBLAST, missileEntity.AudioEmitter);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
							spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Spell;
							spawnMissileMessage.Handle = missileEntity.Handle;
							spawnMissileMessage.Item = 0;
							spawnMissileMessage.Owner = this.mOwner.Handle;
							spawnMissileMessage.Position = position;
							spawnMissileMessage.Velocity = velocity;
							spawnMissileMessage.Spell = spell;
							spawnMissileMessage.Homing = 0f;
							spawnMissileMessage.Splash = 4f;
							NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
						}
					}
				}
			}
		}

		// Token: 0x06002B7F RID: 11135 RVA: 0x001573FF File Offset: 0x001555FF
		public static void SpawnMissile(MissileEntity iMissile, Entity iOwner, ref Vector3 iPosition, ref Vector3 iVelocity, float iRadius)
		{
			iMissile.Initialize(iOwner, iRadius, ref iPosition, ref iVelocity, MeteorShower.MODEL, MeteorShower.CONDITIONS, true);
			iMissile.PlayState.EntityManager.AddEntity(iMissile);
		}

		// Token: 0x06002B80 RID: 11136 RVA: 0x00157428 File Offset: 0x00155628
		public void OnRemove()
		{
			this.mTTL = 0f;
			this.mScene.LightTargetIntensity = 1f;
			if (this.mRumble != null && !this.mRumble.IsStopping)
			{
				this.mRumble.Stop(AudioStopOptions.AsAuthored);
			}
		}

		// Token: 0x04002F16 RID: 12054
		private const float TIME_BETWEEN_METEORS = 0.4f;

		// Token: 0x04002F17 RID: 12055
		private const float LIFE_TIME = 17.5f;

		// Token: 0x04002F18 RID: 12056
		private static MeteorShower mSingelton;

		// Token: 0x04002F19 RID: 12057
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002F1A RID: 12058
		public static readonly ConditionCollection CONDITIONS;

		// Token: 0x04002F1B RID: 12059
		public static readonly Model MODEL;

		// Token: 0x04002F1C RID: 12060
		private static readonly int SOUND_RUMBLE = "magick_meteor_rumble".GetHashCodeCustom();

		// Token: 0x04002F1D RID: 12061
		private static readonly int SOUND_PREBLAST = "magick_meteor_preblast".GetHashCodeCustom();

		// Token: 0x04002F1E RID: 12062
		private float mMeteorSpawnTimer;

		// Token: 0x04002F1F RID: 12063
		private float mTTL;

		// Token: 0x04002F20 RID: 12064
		private GameScene mScene;

		// Token: 0x04002F21 RID: 12065
		private PlayState mPlayState;

		// Token: 0x04002F22 RID: 12066
		private ISpellCaster mOwner;

		// Token: 0x04002F23 RID: 12067
		private Vector3 mPosition;

		// Token: 0x04002F24 RID: 12068
		private Cue mRumble;
	}
}
