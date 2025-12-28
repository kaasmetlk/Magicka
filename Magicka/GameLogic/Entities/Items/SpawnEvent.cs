using System;
using Magicka.AI;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A4 RID: 1188
	public struct SpawnEvent
	{
		// Token: 0x060023F9 RID: 9209 RVA: 0x00102290 File Offset: 0x00100490
		public SpawnEvent(int iType)
		{
			this.Type = iType;
			this.IdleAnimation = Animations.None;
			this.SpawnAnimation = Animations.None;
			this.Order = Order.Attack;
			this.ReactTo = ReactTo.None;
			this.Reaction = Order.None;
			this.Health = 1f;
			this.Rotation = 0f;
			this.Offset = default(Vector3);
		}

		// Token: 0x060023FA RID: 9210 RVA: 0x001022EC File Offset: 0x001004EC
		public SpawnEvent(int iType, Order iOrder, ReactTo iReactTo, Order iReaction)
		{
			this.Type = iType;
			this.IdleAnimation = Animations.None;
			this.SpawnAnimation = Animations.None;
			this.Order = iOrder;
			this.ReactTo = iReactTo;
			this.Reaction = iReaction;
			this.Health = 1f;
			this.Rotation = 0f;
			this.Offset = default(Vector3);
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x00102348 File Offset: 0x00100548
		public SpawnEvent(int iType, Order iOrder, ReactTo iReactTo, Order iReaction, float iHealth)
		{
			this.Type = iType;
			this.IdleAnimation = Animations.None;
			this.SpawnAnimation = Animations.None;
			this.Order = iOrder;
			this.ReactTo = iReactTo;
			this.Reaction = iReaction;
			this.Health = iHealth;
			this.Rotation = 0f;
			this.Offset = default(Vector3);
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x001023A0 File Offset: 0x001005A0
		public SpawnEvent(ContentReader iInput)
		{
			string text = iInput.ReadString();
			this.Type = text.GetHashCodeCustom();
			this.IdleAnimation = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
			this.SpawnAnimation = (Animations)Enum.Parse(typeof(Animations), iInput.ReadString(), true);
			this.Health = iInput.ReadSingle();
			this.Order = (Order)iInput.ReadByte();
			this.ReactTo = (ReactTo)iInput.ReadByte();
			this.Reaction = (Order)iInput.ReadByte();
			this.Rotation = MathHelper.ToRadians(iInput.ReadSingle());
			this.Offset = iInput.ReadVector3();
			iInput.ContentManager.Load<CharacterTemplate>("Data/Characters/" + text);
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x00102468 File Offset: 0x00100668
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			NonPlayerCharacter instance = NonPlayerCharacter.GetInstance(iItem.PlayState);
			CharacterTemplate cachedTemplate = CharacterTemplate.GetCachedTemplate(this.Type);
			Matrix orientation = iItem.Body.Orientation;
			Matrix matrix;
			Matrix.CreateRotationY(this.Rotation, out matrix);
			Matrix.Multiply(ref matrix, ref orientation, out orientation);
			Vector3 position = iItem.Position;
			Vector3 vector;
			Vector3.TransformNormal(ref this.Offset, ref orientation, out vector);
			Vector3.Add(ref vector, ref position, out position);
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnNPC;
				triggerActionMessage.Handle = instance.Handle;
				triggerActionMessage.Template = this.Type;
				triggerActionMessage.Id = 0;
				triggerActionMessage.Position = position;
				triggerActionMessage.Direction = orientation.Forward;
				triggerActionMessage.Bool0 = false;
				triggerActionMessage.Point0 = 0;
				triggerActionMessage.Point1 = 0;
				triggerActionMessage.Point2 = (int)this.SpawnAnimation;
				triggerActionMessage.Point3 = (int)this.IdleAnimation;
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			instance.Initialize(cachedTemplate, position, 0);
			if (iItem.PlayState.Level.CurrentScene.RuleSet != null)
			{
				if (iItem is Character)
				{
					instance.Faction = (iItem as Character).Faction;
				}
				if (iItem.PlayState.Level.CurrentScene.RuleSet is SurvivalRuleset)
				{
					(iItem.PlayState.Level.CurrentScene.RuleSet as SurvivalRuleset).AddedCharacter(instance, true);
				}
			}
			instance.Body.Orientation = orientation;
			instance.CharacterBody.DesiredDirection = orientation.Forward;
			instance.HitPoints = this.Health * instance.MaxHitPoints;
			instance.AI.SetOrder(this.Order, this.ReactTo, this.Reaction, 0, 0, 0, null);
			if (this.SpawnAnimation != Animations.None && this.SpawnAnimation != Animations.idle && this.SpawnAnimation != Animations.idle_agg)
			{
				instance.SpawnAnimation = this.SpawnAnimation;
				instance.ChangeState(RessurectionState.Instance);
			}
			if (this.IdleAnimation != Animations.None)
			{
				instance.SpecialIdleAnimation = this.IdleAnimation;
			}
			iItem.PlayState.EntityManager.AddEntity(instance);
		}

		// Token: 0x040026FF RID: 9983
		public int Type;

		// Token: 0x04002700 RID: 9984
		public Animations IdleAnimation;

		// Token: 0x04002701 RID: 9985
		public Animations SpawnAnimation;

		// Token: 0x04002702 RID: 9986
		public Order Order;

		// Token: 0x04002703 RID: 9987
		public ReactTo ReactTo;

		// Token: 0x04002704 RID: 9988
		public Order Reaction;

		// Token: 0x04002705 RID: 9989
		public float Health;

		// Token: 0x04002706 RID: 9990
		public float Rotation;

		// Token: 0x04002707 RID: 9991
		public Vector3 Offset;
	}
}
