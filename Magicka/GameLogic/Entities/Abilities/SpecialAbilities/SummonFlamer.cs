using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020002B2 RID: 690
	internal class SummonFlamer : SpecialAbility
	{
		// Token: 0x060014D9 RID: 5337 RVA: 0x00081D60 File Offset: 0x0007FF60
		internal static void InitializeCache(PlayState iPlayState)
		{
			SummonFlamer.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/conscious_blaze");
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00081D77 File Offset: 0x0007FF77
		public SummonFlamer(Animations iAnimation) : base(iAnimation, "#specab_tsal_blaze".GetHashCodeCustom())
		{
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x00081D8C File Offset: 0x0007FF8C
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mOwner = null;
			this.mPlayState = iPlayState;
			return this.Execute(iPosition, new Vector3((float)(MagickaMath.Random.NextDouble() - 0.5) * 2f, 0f, (float)(MagickaMath.Random.NextDouble() - 0.5) * 2f));
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x00081DF0 File Offset: 0x0007FFF0
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

		// Token: 0x060014DD RID: 5341 RVA: 0x00081E48 File Offset: 0x00080048
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
				instance.Initialize(SummonFlamer.sTemplate, this.mPosition, 0);
				if (this.mOwner is Character)
				{
					instance.Summoned(this.mOwner as Character, false, true);
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

		// Token: 0x0400163C RID: 5692
		private static CharacterTemplate sTemplate;

		// Token: 0x0400163D RID: 5693
		private PlayState mPlayState;

		// Token: 0x0400163E RID: 5694
		private ISpellCaster mOwner;

		// Token: 0x0400163F RID: 5695
		private Vector3 mPosition;

		// Token: 0x04001640 RID: 5696
		private Vector3 mDirection;
	}
}
