using System;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Gamers;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x020003D0 RID: 976
	public class RandomMine : SpecialAbility
	{
		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06001DE3 RID: 7651 RVA: 0x000D2AAC File Offset: 0x000D0CAC
		public static RandomMine Instance
		{
			get
			{
				if (RandomMine.mSingelton == null)
				{
					lock (RandomMine.mSingeltonLock)
					{
						if (RandomMine.mSingelton == null)
						{
							RandomMine.mSingelton = new RandomMine();
						}
					}
				}
				return RandomMine.mSingelton;
			}
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x000D2B00 File Offset: 0x000D0D00
		private RandomMine() : base(Animations.cast_magick_global, "#magick_reddit_protection".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x000D2B14 File Offset: 0x000D0D14
		public RandomMine(Animations iAnimation) : base(iAnimation, "#magick_reddit_protection".GetHashCodeCustom())
		{
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x000D2B27 File Offset: 0x000D0D27
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mDoDamage = (NetworkManager.Instance.State != NetworkState.Client);
			return this.Execute();
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000D2B4C File Offset: 0x000D0D4C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			Avatar avatar = iOwner as Avatar;
			if (avatar != null && !(avatar.Player.Gamer is NetworkGamer))
			{
				avatar.ConjureShield();
				switch (SpecialAbility.RANDOM.Next(7))
				{
				case 0:
					avatar.ConjureArcane();
					break;
				case 1:
					avatar.ConjureArcane();
					avatar.ConjureCold();
					break;
				case 2:
					avatar.ConjureArcane();
					avatar.ConjureWater();
					break;
				case 3:
					avatar.ConjureArcane();
					avatar.ConjureFire();
					break;
				case 4:
					avatar.ConjureArcane();
					avatar.ConjureLightning();
					break;
				case 5:
					avatar.ConjureArcane();
					avatar.ConjureFire();
					avatar.ConjureWater();
					break;
				case 6:
					avatar.ConjureLife();
					break;
				}
				if (NetworkManager.Instance.State != NetworkState.Offline)
				{
					CharacterActionMessage characterActionMessage = default(CharacterActionMessage);
					characterActionMessage.Handle = avatar.Handle;
					characterActionMessage.Action = ActionType.CastSpell;
					characterActionMessage.Param0I = 1;
					NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref characterActionMessage);
				}
				avatar.CastType = CastType.Force;
				avatar.CastSpell(true, null);
			}
			return this.Execute();
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x000D2C64 File Offset: 0x000D0E64
		private bool Execute()
		{
			return true;
		}

		// Token: 0x0400206A RID: 8298
		private static RandomMine mSingelton;

		// Token: 0x0400206B RID: 8299
		private static volatile object mSingeltonLock = new object();

		// Token: 0x0400206C RID: 8300
		private PlayState mPlayState;

		// Token: 0x0400206D RID: 8301
		private bool mDoDamage;
	}
}
