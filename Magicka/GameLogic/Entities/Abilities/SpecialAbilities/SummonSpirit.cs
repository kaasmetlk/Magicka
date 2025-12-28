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
	// Token: 0x02000258 RID: 600
	public class SummonSpirit : SpecialAbility
	{
		// Token: 0x06001288 RID: 4744 RVA: 0x0007233B File Offset: 0x0007053B
		internal static void InitializeCache(PlayState iPlayState)
		{
			SummonSpirit.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/treespirit");
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x00072352 File Offset: 0x00070552
		public SummonSpirit(Animations iAnimation) : base(iAnimation, "#specab_spirit".GetHashCodeCustom())
		{
			this.mAudioEmitter = new AudioEmitter();
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x00072370 File Offset: 0x00070570
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute(iPosition, new Vector3((float)(MagickaMath.Random.NextDouble() - 0.5) * 2f, 0f, (float)(MagickaMath.Random.NextDouble() - 0.5) * 2f));
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x000723D4 File Offset: 0x000705D4
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

		// Token: 0x0600128C RID: 4748 RVA: 0x0007242C File Offset: 0x0007062C
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
				instance.Initialize(SummonSpirit.sTemplate, this.mPosition, 0);
				if (this.mOwner is Character)
				{
					instance.Summoned(this.mOwner as Character);
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

		// Token: 0x04001147 RID: 4423
		private static CharacterTemplate sTemplate;

		// Token: 0x04001148 RID: 4424
		private PlayState mPlayState;

		// Token: 0x04001149 RID: 4425
		private ISpellCaster mOwner;

		// Token: 0x0400114A RID: 4426
		private AudioEmitter mAudioEmitter;

		// Token: 0x0400114B RID: 4427
		private Vector3 mPosition;

		// Token: 0x0400114C RID: 4428
		private Vector3 mDirection;
	}
}
