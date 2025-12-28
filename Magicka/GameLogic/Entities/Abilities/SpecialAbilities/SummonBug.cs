using System;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000259 RID: 601
	internal class SummonBug : SpecialAbility
	{
		// Token: 0x0600128D RID: 4749 RVA: 0x00072677 File Offset: 0x00070877
		internal static void InitialzeCache(PlayState iPlayState)
		{
			SummonBug.sTemplate = iPlayState.Content.Load<CharacterTemplate>("data/characters/bugswarm");
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0007268E File Offset: 0x0007088E
		public SummonBug(Animations iAnimation) : base(iAnimation, "#specab_swarm".GetHashCodeCustom())
		{
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x000726A4 File Offset: 0x000708A4
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iPlayState);
				if (instance == null)
				{
					return false;
				}
				Vector3 direction = iOwner.Direction;
				Vector3 iPosition = iOwner.Position;
				Vector3.Multiply(ref direction, 4f, out direction);
				Vector3.Add(ref iPosition, ref direction, out iPosition);
				direction = iOwner.Direction;
				Vector3 origin;
				iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out origin, MovementProperties.Default);
				Segment iSeg = default(Segment);
				iSeg.Origin = origin;
				iSeg.Origin.Y = iSeg.Origin.Y + 1f;
				iSeg.Delta.Y = -10f;
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (iPlayState.Level.CurrentScene.SegmentIntersect(out num, out vector, out vector2, iSeg))
				{
					iPosition = vector;
				}
				instance.Initialize(SummonBug.sTemplate, iPosition, 0);
				if (iOwner is Character)
				{
					instance.Summoned(iOwner as Character);
				}
				instance.ForceAnimation(Animations.spawn);
				Agent ai = instance.AI;
				ai.SetOrder(Order.Attack, ReactTo.Attack | ReactTo.Proximity, Order.Attack, 0, 0, 0, null);
				ai.AlertRadius = 12f;
				Vector3 vector3 = default(Vector3);
				vector3.Y = 1f;
				Vector3 vector4 = default(Vector3);
				Matrix orientation;
				Matrix.CreateWorld(ref vector4, ref direction, ref vector3, out orientation);
				instance.CharacterBody.DesiredDirection = direction;
				instance.Body.Orientation = orientation;
				iPlayState.EntityManager.AddEntity(instance);
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
					if (iOwner != null)
					{
						triggerActionMessage.Scene = (int)iOwner.Handle;
					}
					triggerActionMessage.Bool0 = false;
					triggerActionMessage.Point1 = 170;
					triggerActionMessage.Point2 = 170;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			return true;
		}

		// Token: 0x0400114D RID: 4429
		private static CharacterTemplate sTemplate;
	}
}
