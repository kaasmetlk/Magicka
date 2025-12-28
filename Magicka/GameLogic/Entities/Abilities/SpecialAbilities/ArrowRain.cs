using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.Spells.SpellEffects;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000185 RID: 389
	public class ArrowRain : SpecialAbility, IAbilityEffect
	{
		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x000473D4 File Offset: 0x000455D4
		public static ArrowRain Instance
		{
			get
			{
				if (ArrowRain.mSingelton == null)
				{
					lock (ArrowRain.mSingeltonLock)
					{
						if (ArrowRain.mSingelton == null)
						{
							ArrowRain.mSingelton = new ArrowRain();
						}
					}
				}
				return ArrowRain.mSingelton;
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x00047428 File Offset: 0x00045628
		public ArrowRain(Animations iAnimation) : base(iAnimation, "#magick_meteors".GetHashCodeCustom())
		{
			this.mAbilityElement = Elements.None;
			this.NUM_OF_ARROWS = 10U;
			this.INTESITY = Math.Pow(this.NUM_OF_ARROWS + 1U, (double)(1f / this.LIFE_TIME));
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00047490 File Offset: 0x00045690
		private ArrowRain() : base(Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
		{
			this.mAbilityElement = Elements.None;
			this.NUM_OF_ARROWS = 10U;
			this.INTESITY = Math.Pow(this.NUM_OF_ARROWS + 1U, (double)(1f / this.LIFE_TIME));
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x000474F8 File Offset: 0x000456F8
		public ArrowRain(Animations iAnimation, Elements[] iElements) : base(iAnimation, "#magick_meteors".GetHashCodeCustom())
		{
			this.mElements = iElements;
			foreach (Elements elements in iElements)
			{
				if ((elements & Elements.Lightning) != Elements.None)
				{
					this.NUM_OF_ARROWS = 7U;
					this.mAbilityElement = Elements.Lightning;
					break;
				}
				if ((elements & Elements.Poison) != Elements.None)
				{
					this.NUM_OF_ARROWS = 15U;
					this.mAbilityElement = Elements.Poison;
					break;
				}
			}
			if (this.NUM_OF_ARROWS <= 0U)
			{
				this.NUM_OF_ARROWS = 10U;
				this.mAbilityElement = Elements.None;
			}
			this.INTESITY = Math.Pow(this.NUM_OF_ARROWS + 1U, (double)(1f / this.LIFE_TIME));
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x000475B8 File Offset: 0x000457B8
		public ArrowRain(Elements[] iElements) : base(Animations.cast_magick_global, "#magick_meteors".GetHashCodeCustom())
		{
			this.mElements = iElements;
			foreach (Elements elements in iElements)
			{
				if ((elements & Elements.Lightning) != Elements.None)
				{
					this.NUM_OF_ARROWS = 7U;
					this.mAbilityElement = Elements.Lightning;
					break;
				}
				if ((elements & Elements.Poison) != Elements.None)
				{
					this.NUM_OF_ARROWS = 15U;
					this.mAbilityElement = Elements.Poison;
					break;
				}
			}
			if (this.NUM_OF_ARROWS <= 0U)
			{
				this.NUM_OF_ARROWS = 10U;
				this.mAbilityElement = Elements.None;
			}
			this.INTESITY = Math.Pow(this.NUM_OF_ARROWS + 1U, (double)(1f / this.LIFE_TIME));
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x00047678 File Offset: 0x00045878
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			if (this.mTTL > 0f)
			{
				this.mLaunched = false;
				this.START_TIME = 1.9f;
				this.mTTL = this.LIFE_TIME + this.START_TIME;
				return true;
			}
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute();
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x000476DC File Offset: 0x000458DC
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			base.Execute(iOwner, iPlayState);
			if (this.mTTL > 0f)
			{
				this.mLaunched = false;
				this.START_TIME = 1.9f;
				this.mTTL = this.LIFE_TIME + this.START_TIME;
				return true;
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			return this.Execute();
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0004774C File Offset: 0x0004594C
		private bool Execute()
		{
			this.mScene = this.mPlayState.Level.CurrentScene;
			this.mLaunched = false;
			this.START_TIME = 1.9f;
			this.mTTL = this.LIFE_TIME + this.START_TIME;
			this.mArrowSpawnTimer = 0f;
			SpellManager.Instance.AddSpellEffect(this);
			if (this.mOwner is Avatar)
			{
				Vector3 direction = this.mOwner.Direction;
				Vector3.Multiply(ref direction, 15f, out direction);
				this.mPosition = this.mOwner.Position + direction;
			}
			else
			{
				if (!(this.mOwner is NonPlayerCharacter))
				{
					this.mTTL = 0f;
					return false;
				}
				this.mPosition = (this.mOwner as NonPlayerCharacter).AI.LastTarget.Position;
			}
			return true;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x00047828 File Offset: 0x00045A28
		public void Launch()
		{
			this.mLaunched = true;
			ConditionCollection conditionCollection;
			lock (ProjectileSpell.sCachedConditions)
			{
				conditionCollection = ProjectileSpell.sCachedConditions.Dequeue();
			}
			conditionCollection.Clear();
			conditionCollection[0].Condition.EventConditionType = EventConditionType.Timer;
			conditionCollection[0].Condition.Time = 2.5f;
			conditionCollection[0].Add(new EventStorage(default(RemoveEvent)));
			conditionCollection[1].Condition.EventConditionType = EventConditionType.Default;
			conditionCollection[1].Condition.Repeat = true;
			lock (ProjectileSpell.sCachedConditions)
			{
				ProjectileSpell.sCachedConditions.Enqueue(conditionCollection);
			}
			Spell spell = default(Spell);
			if (this.mElements != null)
			{
				Spell.DefaultSpell(this.mElements, out spell);
			}
			if (this.mOwner != null)
			{
				this.me = this.mOwner.GetMissileInstance();
			}
			else
			{
				this.me = MissileEntity.GetInstance(this.mPlayState);
			}
			Model model = new Model();
			model = Game.Instance.Content.Load<Model>("Models/Missiles/goblin_arrow_0");
			Vector3 translation = (this.mOwner as Character).GetLeftAttachOrientation().Translation;
			Vector3 velocity = new Vector3(0.01f, 50f, -0.01f);
			this.me.Initialize(this.mOwner as Entity, null, 0f, model.Meshes[0].BoundingSphere.Radius * 0.75f, ref translation, ref velocity, model, conditionCollection, false);
			ProjectileSpell.SpawnMissile(ref this.me, model, this.mOwner, 0f, ref translation, ref velocity, ref spell, 4f, 1);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
				spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Spell;
				spawnMissileMessage.Handle = this.me.Handle;
				spawnMissileMessage.Item = 0;
				spawnMissileMessage.Owner = this.mOwner.Handle;
				spawnMissileMessage.Position = translation;
				spawnMissileMessage.Velocity = velocity;
				spawnMissileMessage.Spell = spell;
				spawnMissileMessage.Homing = 0f;
				spawnMissileMessage.Splash = 4f;
				NetworkManager.Instance.Interface.SendMessage<SpawnMissileMessage>(ref spawnMissileMessage);
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000BDF RID: 3039 RVA: 0x00047A90 File Offset: 0x00045C90
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00047AA4 File Offset: 0x00045CA4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			if (this.mTTL <= 0f)
			{
				this.me.Deinitialize();
			}
			if (this.mTTL < this.LIFE_TIME && !this.mLaunched)
			{
				this.Launch();
			}
			if (this.mLaunched)
			{
				this.mArrowSpawnTimer += iDeltaTime;
				int num = (int)(-1.0 + Math.Pow(this.INTESITY, (double)this.mArrowSpawnTimer));
				if (num != 0 && num != this.mLastArrowNum)
				{
					this.mLastArrowNum = num;
					Vector3 vector = this.mPosition;
					float num2 = (float)Math.Sqrt(SpecialAbility.RANDOM.NextDouble());
					float num3 = (float)(SpecialAbility.RANDOM.NextDouble() * 6.2831854820251465);
					float num4 = (float)((double)num2 * Math.Cos((double)num3));
					float num5 = (float)((double)num2 * Math.Sin((double)num3));
					vector.X += this.RADIUS * num4;
					vector.Z += this.RADIUS * num5;
					vector.Y += 146f;
					Vector3 vector2 = vector;
					vector2.Y = 0f;
					Vector3 velocity = new Vector3(0.01f, -20f, -0.01f);
					Spell spell = default(Spell);
					if (this.mElements != null)
					{
						Spell.DefaultSpell(this.mElements, out spell);
					}
					if ((this.mAbilityElement & Elements.Lightning) != Elements.None)
					{
						LightningBolt lightning = LightningBolt.GetLightning();
						lightning.AirToSurface = true;
						HitList iHitList = new HitList(16);
						DamageCollection5 damageCollection;
						spell.CalculateDamage(SpellType.Lightning, CastType.Force, out damageCollection);
						Flash.Instance.Execute(this.mPlayState.Scene, 0.075f);
						this.mPlayState.Camera.CameraShake(vector2, 0.5f, 0.075f);
						lightning.Cast(this.mOwner, vector, vector2 - vector, iHitList, Spell.LIGHTNINGCOLOR, 10f, ref damageCollection, new Spell?(spell), this.mPlayState);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
							triggerActionMessage.ActionType = TriggerActionType.LightningBolt;
							if (this.mOwner != null)
							{
								triggerActionMessage.Handle = this.mOwner.Handle;
							}
							triggerActionMessage.Position = vector;
							triggerActionMessage.Direction = vector2;
							triggerActionMessage.Spell = spell;
							NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
							return;
						}
					}
					else
					{
						MissileEntity missileEntity;
						if (this.mOwner != null)
						{
							missileEntity = this.mOwner.GetMissileInstance();
						}
						else
						{
							missileEntity = MissileEntity.GetInstance(this.mPlayState);
						}
						Model mdl = new Model();
						mdl = Game.Instance.Content.Load<Model>("Models/Missiles/goblin_arrow_0");
						ProjectileSpell.SpawnMissile(ref missileEntity, mdl, this.mOwner, 0f, ref vector, ref velocity, ref spell, 4f, 1);
						if (NetworkManager.Instance.State == NetworkState.Server)
						{
							SpawnMissileMessage spawnMissileMessage = default(SpawnMissileMessage);
							spawnMissileMessage.Type = SpawnMissileMessage.MissileType.Spell;
							spawnMissileMessage.Handle = missileEntity.Handle;
							spawnMissileMessage.Item = 0;
							spawnMissileMessage.Owner = this.mOwner.Handle;
							spawnMissileMessage.Position = vector;
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

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00047DFB File Offset: 0x00045FFB
		public void OnRemove()
		{
			this.mTTL = 0f;
			this.mScene.LightTargetIntensity = 1f;
		}

		// Token: 0x04000AD3 RID: 2771
		private static ArrowRain mSingelton;

		// Token: 0x04000AD4 RID: 2772
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000AD5 RID: 2773
		private readonly float RADIUS = 3.5f;

		// Token: 0x04000AD6 RID: 2774
		private readonly uint NUM_OF_ARROWS;

		// Token: 0x04000AD7 RID: 2775
		private readonly double INTESITY;

		// Token: 0x04000AD8 RID: 2776
		private float LIFE_TIME = 4.5f;

		// Token: 0x04000AD9 RID: 2777
		private float START_TIME;

		// Token: 0x04000ADA RID: 2778
		private bool mLaunched;

		// Token: 0x04000ADB RID: 2779
		private float mArrowSpawnTimer;

		// Token: 0x04000ADC RID: 2780
		private float mTTL;

		// Token: 0x04000ADD RID: 2781
		private int mLastArrowNum;

		// Token: 0x04000ADE RID: 2782
		private GameScene mScene;

		// Token: 0x04000ADF RID: 2783
		private PlayState mPlayState;

		// Token: 0x04000AE0 RID: 2784
		private ISpellCaster mOwner;

		// Token: 0x04000AE1 RID: 2785
		private Vector3 mPosition;

		// Token: 0x04000AE2 RID: 2786
		private Elements[] mElements;

		// Token: 0x04000AE3 RID: 2787
		private readonly Elements mAbilityElement;

		// Token: 0x04000AE4 RID: 2788
		private MissileEntity me;
	}
}
