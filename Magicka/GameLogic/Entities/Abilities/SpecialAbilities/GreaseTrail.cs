using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200045F RID: 1119
	public class GreaseTrail : SpecialAbility, IAbilityEffect
	{
		// Token: 0x06002229 RID: 8745 RVA: 0x000F4B94 File Offset: 0x000F2D94
		public static GreaseTrail GetInstance()
		{
			if (GreaseTrail.sCache.Count > 0)
			{
				GreaseTrail result = GreaseTrail.sCache[GreaseTrail.sCache.Count - 1];
				GreaseTrail.sCache.RemoveAt(GreaseTrail.sCache.Count - 1);
				return result;
			}
			return new GreaseTrail();
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x000F4BE4 File Offset: 0x000F2DE4
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			GreaseTrail.sCache = new List<GreaseTrail>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				GreaseTrail.sCache.Add(new GreaseTrail());
			}
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x000F4C17 File Offset: 0x000F2E17
		public GreaseTrail(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x000F4C35 File Offset: 0x000F2E35
		private GreaseTrail() : base(Animations.cast_magick_sweep, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x000F4C54 File Offset: 0x000F2E54
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("GreaseTrail can not be cast without an owner!");
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x000F4C60 File Offset: 0x000F2E60
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				base.Execute(iOwner, iPlayState);
				this.mTTL = 20f;
				SpellManager.Instance.AddSpellEffect(this);
				this.mOwner = (iOwner as Character);
				this.mPlayState = iPlayState;
				this.mAnimationName = this.mOwner.CurrentAnimation;
				return true;
			}
			return false;
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x0600222F RID: 8751 RVA: 0x000F4D04 File Offset: 0x000F2F04
		public bool IsDead
		{
			get
			{
				return this.mTTL <= 0f;
			}
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x000F4D18 File Offset: 0x000F2F18
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mTTL -= iDeltaTime;
			this.mInterval -= iDeltaTime;
			if (this.mAnimationName != this.mOwner.CurrentAnimation)
			{
				this.mTTL = 0f;
			}
			if (this.mInterval < 0f)
			{
				this.mInterval = this.GREASE_INTERVAL;
				Vector3 position = this.mOwner.Position;
				position.X += (float)(MagickaMath.Random.NextDouble() * 3.0);
				position.Z += (float)(MagickaMath.Random.NextDouble() * 3.0);
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

		// Token: 0x06002231 RID: 8753 RVA: 0x000F4E8B File Offset: 0x000F308B
		public void OnRemove()
		{
			this.mTTL = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			GreaseTrail.sCache.Add(this);
		}

		// Token: 0x04002540 RID: 9536
		private static List<GreaseTrail> sCache;

		// Token: 0x04002541 RID: 9537
		public static readonly int SOUNDHASH = "magick_grease".GetHashCodeCustom();

		// Token: 0x04002542 RID: 9538
		private Character mOwner;

		// Token: 0x04002543 RID: 9539
		public PlayState mPlayState;

		// Token: 0x04002544 RID: 9540
		private VisualEffectReference mEffect;

		// Token: 0x04002545 RID: 9541
		private float mTTL;

		// Token: 0x04002546 RID: 9542
		private float mInterval;

		// Token: 0x04002547 RID: 9543
		private Animations mAnimationName;

		// Token: 0x04002548 RID: 9544
		private readonly float GREASE_INTERVAL = 0.35f;
	}
}
