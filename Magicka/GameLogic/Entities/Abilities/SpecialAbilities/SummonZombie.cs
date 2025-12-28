using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003CF RID: 975
	public class SummonZombie : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06001DD8 RID: 7640 RVA: 0x000D2514 File Offset: 0x000D0714
		public static SummonZombie GetInstance()
		{
			if (SummonZombie.sCache.Count > 0)
			{
				SummonZombie result = SummonZombie.sCache[SummonZombie.sCache.Count - 1];
				SummonZombie.sCache.RemoveAt(SummonZombie.sCache.Count - 1);
				return result;
			}
			return new SummonZombie();
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x000D2564 File Offset: 0x000D0764
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			SummonZombie.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/zombie");
			SummonZombie.sCache = new List<SummonZombie>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				SummonZombie.sCache.Add(new SummonZombie());
			}
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x000D25AC File Offset: 0x000D07AC
		public SummonZombie(Animations iAnimation) : base(iAnimation, "#magick_sundead".GetHashCodeCustom())
		{
			this.mAudioEmitter = new AudioEmitter();
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x000D25CA File Offset: 0x000D07CA
		private SummonZombie() : base(Animations.cast_magick_direct, "#magick_sundead".GetHashCodeCustom())
		{
			this.mAudioEmitter = new AudioEmitter();
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x000D25E9 File Offset: 0x000D07E9
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute(iPosition);
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x000D2600 File Offset: 0x000D0800
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			Vector3.Add(ref position, ref direction, out position);
			return this.Execute(position);
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x000D2654 File Offset: 0x000D0854
		private bool Execute(Vector3 iPosition)
		{
			this.mTTL = 8.1f;
			this.mSpawnTimer = 0f;
			this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out this.mPosition, MovementProperties.Default);
			Segment iSeg = new Segment(this.mPosition, Vector3.Down * 3f);
			float num;
			Vector3 vector;
			Vector3 vector2;
			if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
			{
				iPosition = vector;
			}
			Matrix identity = Matrix.Identity;
			identity.M11 *= 4f;
			identity.M12 *= 4f;
			identity.M13 *= 4f;
			identity.M31 *= 4f;
			identity.M32 *= 4f;
			identity.M33 *= 4f;
			identity.Translation = iPosition;
			EffectManager.Instance.StartEffect(SummonZombie.EFFECT, ref identity, out this.mEffect);
			this.mAudioEmitter.Position = iPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			AudioManager.Instance.PlayCue(Banks.Spells, SummonZombie.SOUNDHASH, this.mAudioEmitter);
			SpellManager.Instance.AddSpellEffect(this);
			return true;
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x000D27C1 File Offset: 0x000D09C1
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x000D27D4 File Offset: 0x000D09D4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mSpawnTimer -= iDeltaTime;
			this.mAudioEmitter.Position = this.mPosition;
			this.mAudioEmitter.Up = Vector3.Up;
			this.mAudioEmitter.Forward = Vector3.Right;
			if (NetworkManager.Instance.State != NetworkState.Client && this.mSpawnTimer <= 0f)
			{
				this.mSpawnTimer += 2f;
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
				if (instance == null)
				{
					return;
				}
				Vector3 vector = this.mPosition;
				Vector3 vector2 = new Vector3(((float)SpecialAbility.RANDOM.NextDouble() - 0.5f) * 3f, 0f, ((float)SpecialAbility.RANDOM.NextDouble() - 0.5f) * 3f);
				Vector3.Add(ref vector, ref vector2, out vector2);
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector2, out vector, MovementProperties.Default);
				Segment iSeg = default(Segment);
				iSeg.Origin = vector;
				iSeg.Delta.Y = -10f;
				float num;
				Vector3 vector3;
				Vector3 vector4;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector3, out vector4, iSeg))
				{
					vector = vector3;
				}
				instance.Initialize(SummonZombie.sTemplate, vector, 0);
				if (this.mOwner is Character)
				{
					instance.Summoned(this.mOwner as Character);
				}
				instance.ForceAnimation(Animations.spawn);
				Agent ai = instance.AI;
				ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				ai.AlertRadius = 12f;
				Matrix orientation;
				Matrix.CreateRotationY((float)SpecialAbility.RANDOM.NextDouble() * 6.2831855f, out orientation);
				instance.Body.Orientation = orientation;
				this.mPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = instance.Type;
					triggerActionMessage.Id = instance.UniqueID;
					triggerActionMessage.Position = instance.Position;
					triggerActionMessage.Direction = orientation.Forward;
					triggerActionMessage.Bool0 = false;
					if (this.mOwner != null)
					{
						triggerActionMessage.Scene = (int)this.mOwner.Handle;
					}
					triggerActionMessage.Point1 = 170;
					triggerActionMessage.Point2 = 170;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x000D2A5E File Offset: 0x000D0C5E
		public void OnRemove()
		{
			EffectManager.Instance.Stop(ref this.mEffect);
			SummonZombie.sCache.Add(this);
		}

		// Token: 0x0400205C RID: 8284
		private const float TIME_BETWEEN_SPAWNS = 2f;

		// Token: 0x0400205D RID: 8285
		private const float MAGICK_TIME = 8.1f;

		// Token: 0x0400205E RID: 8286
		private static List<SummonZombie> sCache;

		// Token: 0x0400205F RID: 8287
		private static CharacterTemplate sTemplate;

		// Token: 0x04002060 RID: 8288
		public static readonly int SOUNDHASH = "magick_raise_dead".GetHashCodeCustom();

		// Token: 0x04002061 RID: 8289
		public static readonly int EFFECT = "magick_summonundead_ground".GetHashCodeCustom();

		// Token: 0x04002062 RID: 8290
		public static readonly int SUMMON_EFFECT = "magick_friendlysummon".GetHashCodeCustom();

		// Token: 0x04002063 RID: 8291
		private float mTTL;

		// Token: 0x04002064 RID: 8292
		private float mSpawnTimer;

		// Token: 0x04002065 RID: 8293
		private PlayState mPlayState;

		// Token: 0x04002066 RID: 8294
		private ISpellCaster mOwner;

		// Token: 0x04002067 RID: 8295
		private AudioEmitter mAudioEmitter;

		// Token: 0x04002068 RID: 8296
		private Vector3 mPosition;

		// Token: 0x04002069 RID: 8297
		private VisualEffectReference mEffect;
	}
}
