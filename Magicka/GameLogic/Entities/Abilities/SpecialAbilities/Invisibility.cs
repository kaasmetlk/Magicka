using System;
using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200017F RID: 383
	public class Invisibility : SpecialAbility
	{
		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x000460C8 File Offset: 0x000442C8
		public static Invisibility Instance
		{
			get
			{
				if (Invisibility.mSingelton == null)
				{
					lock (Invisibility.mSingeltonLock)
					{
						if (Invisibility.mSingelton == null)
						{
							Invisibility.mSingelton = new Invisibility();
						}
					}
				}
				return Invisibility.mSingelton;
			}
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0004611C File Offset: 0x0004431C
		public Invisibility(Animations iAnimation) : base(iAnimation, "#magick_invisibility".GetHashCodeCustom())
		{
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0004612F File Offset: 0x0004432F
		private Invisibility() : base(Animations.cast_magick_self, "#magick_invisibility".GetHashCodeCustom())
		{
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00046143 File Offset: 0x00044343
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("Invisibility have to be cast by a character!");
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0004614F File Offset: 0x0004434F
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			base.Execute(iOwner, iPlayState);
			if (iOwner is Character)
			{
				AudioManager.Instance.PlayCue(Banks.Spells, Invisibility.SOUNDHASH, iOwner.AudioEmitter);
				(iOwner as Character).SetInvisible(15f);
				return true;
			}
			return false;
		}

		// Token: 0x04000AAE RID: 2734
		public const float INVISIBILITY_TIME = 15f;

		// Token: 0x04000AAF RID: 2735
		private static Invisibility mSingelton;

		// Token: 0x04000AB0 RID: 2736
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04000AB1 RID: 2737
		public static readonly int SOUNDHASH = "magick_invisibility".GetHashCodeCustom();

		// Token: 0x04000AB2 RID: 2738
		public static readonly int INVISIBILITY_EFFECT = "invisibility".GetHashCodeCustom();
	}
}
