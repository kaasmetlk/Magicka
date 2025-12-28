using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020001FC RID: 508
	public class SummonUndead : SpecialAbility
	{
		// Token: 0x060010AD RID: 4269 RVA: 0x00068C24 File Offset: 0x00066E24
		internal static void InitializeCache(PlayState iPlayState)
		{
			SummonUndead.sTemplates = new CharacterTemplate[5];
			SummonUndead.sTemplates[0] = iPlayState.Content.Load<CharacterTemplate>("data/characters/ghoul");
			SummonUndead.sTemplates[1] = iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_wight_friendly");
			SummonUndead.sTemplates[2] = iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_arcane");
			SummonUndead.sTemplates[3] = iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_frost");
			SummonUndead.sTemplates[4] = iPlayState.Content.Load<CharacterTemplate>("data/characters/skeleton_darksoul_lightning");
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00068CAF File Offset: 0x00066EAF
		public SummonUndead(Animations iAnimation) : base(iAnimation, "#specab_spirit".GetHashCodeCustom())
		{
			this.mAudioEmitter = new AudioEmitter();
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00068CD0 File Offset: 0x00066ED0
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute(iPosition, new Vector3((float)(MagickaMath.Random.NextDouble() - 0.5) * 2f, 0f, (float)(MagickaMath.Random.NextDouble() - 0.5) * 2f));
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00068D34 File Offset: 0x00066F34
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			Vector3 position = iOwner.Position;
			Vector3 direction = iOwner.Direction;
			Vector3.Multiply(ref direction, 4f, out direction);
			Vector3.Add(ref position, ref direction, out position);
			return this.Execute(position, iOwner.Direction);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00068D8C File Offset: 0x00066F8C
		private bool Execute(Vector3 iPosition, Vector3 iDirection)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				this.mDirection = iDirection;
				this.mPosition = iPosition;
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(this.mPlayState);
				if (instance == null)
				{
					return false;
				}
				Vector3 vector = iPosition;
				vector.Y += 3f;
				this.mPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out this.mPosition, MovementProperties.Default);
				Segment iSeg = default(Segment);
				iSeg.Origin = this.mPosition;
				iSeg.Delta.Y = -10f;
				float num;
				Vector3 vector2;
				Vector3 vector3;
				if (this.mPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector2, out vector3, iSeg))
				{
					this.mPosition = vector2;
				}
				instance.Initialize(SummonUndead.sTemplates[SpecialAbility.RANDOM.Next(5)], this.mPosition, 0);
				if (this.mOwner is Character)
				{
					instance.Summoned(this.mOwner as Character, true);
				}
				instance.ForceAnimation(Animations.spawn);
				Agent ai = instance.AI;
				ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				ai.AlertRadius = 12f;
				Vector3 vector4 = default(Vector3);
				vector4.Y = 1f;
				Vector3 vector5 = default(Vector3);
				Matrix orientation;
				Matrix.CreateWorld(ref vector5, ref this.mDirection, ref vector4, out orientation);
				instance.CharacterBody.DesiredDirection = this.mDirection;
				instance.Body.Orientation = orientation;
				this.mPlayState.EntityManager.AddEntity(instance);
				ai.Owner.SpawnAnimation = Animations.spawn;
				ai.Owner.ChangeState(RessurectionState.Instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Template = instance.Type;
					triggerActionMessage.Id = instance.UniqueID;
					triggerActionMessage.Position = instance.Position;
					triggerActionMessage.Direction = orientation.Forward;
					if (this.mOwner != null)
					{
						triggerActionMessage.Scene = (int)this.mOwner.Handle;
					}
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.Point1 = 170;
					triggerActionMessage.Point2 = 170;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			return true;
		}

		// Token: 0x04000F42 RID: 3906
		private static CharacterTemplate[] sTemplates;

		// Token: 0x04000F43 RID: 3907
		private PlayState mPlayState;

		// Token: 0x04000F44 RID: 3908
		private ISpellCaster mOwner;

		// Token: 0x04000F45 RID: 3909
		private AudioEmitter mAudioEmitter;

		// Token: 0x04000F46 RID: 3910
		private Vector3 mPosition;

		// Token: 0x04000F47 RID: 3911
		private Vector3 mDirection;
	}
}
