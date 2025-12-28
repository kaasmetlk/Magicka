using System;
using Magicka.GameLogic.GameStates;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000456 RID: 1110
	public class Vortex : SpecialAbility
	{
		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x060021F1 RID: 8689 RVA: 0x000F3304 File Offset: 0x000F1504
		public static Vortex Instance
		{
			get
			{
				if (Vortex.sSingelton == null)
				{
					lock (Vortex.sSingeltonLock)
					{
						if (Vortex.sSingelton == null)
						{
							Vortex.sSingelton = new Vortex();
						}
					}
				}
				return Vortex.sSingelton;
			}
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x000F3358 File Offset: 0x000F1558
		private Vortex() : base(Animations.cast_magick_global, "#magick_vortex".GetHashCodeCustom())
		{
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x000F336C File Offset: 0x000F156C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			NetworkState state = NetworkManager.Instance.State;
			if ((state != NetworkState.Client && (!(iOwner is Avatar) || !((iOwner as Avatar).Player.Gamer is NetworkGamer))) || (state == NetworkState.Client && iOwner is Avatar && !((iOwner as Avatar).Player.Gamer is NetworkGamer)))
			{
				Vector3 vector = iOwner.Position + iOwner.Direction * 10f;
				VortexEntity instance = VortexEntity.GetInstance();
				instance.Initialize(iOwner, vector);
				iPlayState.EntityManager.AddEntity(instance);
				if (state != NetworkState.Offline)
				{
					SpawnVortexMessage spawnVortexMessage = default(SpawnVortexMessage);
					spawnVortexMessage.Handle = instance.Handle;
					spawnVortexMessage.Position = vector;
					spawnVortexMessage.OwnerHandle = iOwner.Handle;
					NetworkManager.Instance.Interface.SendMessage<SpawnVortexMessage>(ref spawnVortexMessage);
				}
			}
			return true;
		}

		// Token: 0x040024F4 RID: 9460
		private static Vortex sSingelton;

		// Token: 0x040024F5 RID: 9461
		private static volatile object sSingeltonLock = new object();
	}
}
