using System;
using Magicka.GameLogic.GameStates;
using Magicka.Levels;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020004E0 RID: 1248
	public class GreaseSplash : SpecialAbility
	{
		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06002510 RID: 9488 RVA: 0x0010BFF0 File Offset: 0x0010A1F0
		public static GreaseSplash Instance
		{
			get
			{
				if (GreaseSplash.sSingelton == null)
				{
					lock (GreaseSplash.sSingeltonLock)
					{
						if (GreaseSplash.sSingelton == null)
						{
							GreaseSplash.sSingelton = new GreaseSplash();
						}
					}
				}
				return GreaseSplash.sSingelton;
			}
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x0010C044 File Offset: 0x0010A244
		private GreaseSplash() : base(Animations.cast_magick_direct, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0010C058 File Offset: 0x0010A258
		public GreaseSplash(Animations iAnimation) : base(iAnimation, "#magick_grease".GetHashCodeCustom())
		{
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x0010C06B File Offset: 0x0010A26B
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Grease cannot be spawned without an owner!");
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x0010C078 File Offset: 0x0010A278
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			Vector3 direction = iOwner.Direction;
			Vector3 up = Vector3.Up;
			AnimatedLevelPart animatedLevelPart = null;
			for (int i = 0; i < GreaseSplash.NUM_OF_FIELDS; i++)
			{
				float yaw = 3.1415927f / (float)GreaseSplash.NUM_OF_FIELDS * (float)i;
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
				Vector3.Transform(ref direction, ref quaternion, out direction);
				float scaleFactor = (float)(1.0 + MagickaMath.Random.NextDouble() * (double)GreaseSplash.RANGE);
				Vector3 vector;
				Vector3.Multiply(ref direction, scaleFactor, out vector);
				vector = iOwner.Position + vector;
				vector.Y = 0f;
				Grease.GreaseField instance = Grease.GreaseField.GetInstance(iOwner.PlayState);
				instance.Initialize(iOwner, animatedLevelPart, ref vector, ref up);
				iOwner.PlayState.EntityManager.AddEntity(instance);
				if (NetworkManager.Instance.State == NetworkState.Server)
				{
					TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
					triggerActionMessage.Handle = instance.Handle;
					triggerActionMessage.Position = vector;
					triggerActionMessage.Direction = direction;
					if (animatedLevelPart != null)
					{
						triggerActionMessage.Arg = (int)animatedLevelPart.Handle;
					}
					else
					{
						triggerActionMessage.Arg = 65535;
					}
					triggerActionMessage.Id = (int)iOwner.Handle;
					triggerActionMessage.ActionType = TriggerActionType.SpawnGrease;
					NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
				}
			}
			return true;
		}

		// Token: 0x04002873 RID: 10355
		private static GreaseSplash sSingelton;

		// Token: 0x04002874 RID: 10356
		private static volatile object sSingeltonLock = new object();

		// Token: 0x04002875 RID: 10357
		private static readonly int NUM_OF_FIELDS = 7;

		// Token: 0x04002876 RID: 10358
		private static readonly float RANGE = 3f;
	}
}
