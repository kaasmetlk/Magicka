using System;
using Magicka.GameLogic.GameStates;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200060F RID: 1551
	public class Tornado : SpecialAbility
	{
		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06002E84 RID: 11908 RVA: 0x0017965C File Offset: 0x0017785C
		public static Tornado Instance
		{
			get
			{
				if (Tornado.sSingelton == null)
				{
					lock (Tornado.sSingeltonLock)
					{
						if (Tornado.sSingelton == null)
						{
							Tornado.sSingelton = new Tornado();
						}
					}
				}
				return Tornado.sSingelton;
			}
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x001796B0 File Offset: 0x001778B0
		private Tornado() : base(Animations.cast_magick_direct, "#magick_tornado".GetHashCodeCustom())
		{
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x001796C4 File Offset: 0x001778C4
		public Tornado(Animations iAnimation) : base(iAnimation, "#magick_tornado".GetHashCodeCustom())
		{
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x001796D8 File Offset: 0x001778D8
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 vector;
				iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref iPosition, out vector, MovementProperties.Default);
				iPosition = vector;
				Matrix identity = Matrix.Identity;
				identity.Translation = iPosition;
				TornadoEntity instance = TornadoEntity.GetInstance();
				instance.Initialize(iPlayState, identity, null);
				iPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Id = 0;
					triggerActionMessage.Position = iPosition;
					Quaternion.CreateFromRotationMatrix(ref identity, out triggerActionMessage.Orientation);
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			return true;
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x00179790 File Offset: 0x00177990
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (NetworkManager.Instance.State != NetworkState.Client)
			{
				Vector3 vector = iOwner.Position;
				Vector3 direction = iOwner.Direction;
				Vector3.Multiply(ref direction, 4f, out direction);
				Vector3.Add(ref direction, ref vector, out vector);
				Vector3 vector2;
				iPlayState.Level.CurrentScene.NavMesh.GetNearestPosition(ref vector, out vector2, MovementProperties.Default);
				vector = vector2;
				Matrix identity = Matrix.Identity;
				identity.Translation = vector;
				identity.Forward = direction;
				TornadoEntity instance = TornadoEntity.GetInstance();
				instance.Initialize(iPlayState, identity, iOwner);
				iPlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.ActionType = TriggerActionType.SpawnTornado;
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Id = (int)iOwner.Handle;
					triggerActionMessage.Position = vector;
					Quaternion.CreateFromRotationMatrix(ref identity, out triggerActionMessage.Orientation);
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			return true;
		}

		// Token: 0x04003290 RID: 12944
		public const float MAGICK_TTL = 15f;

		// Token: 0x04003291 RID: 12945
		private static Tornado sSingelton;

		// Token: 0x04003292 RID: 12946
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04003293 RID: 12947
		public static readonly int AMBIENCE = "magick_tornado_rumble".GetHashCodeCustom();

		// Token: 0x04003294 RID: 12948
		public static readonly int EFFECT = "magick_tornado".GetHashCodeCustom();

		// Token: 0x04003295 RID: 12949
		public static readonly int HIT_EFFECT = "magick_tornado_hit".GetHashCodeCustom();
	}
}
