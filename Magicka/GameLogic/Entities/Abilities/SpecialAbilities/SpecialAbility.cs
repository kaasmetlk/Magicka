using System;
using System.Reflection;
using Magicka.GameLogic.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x0200003A RID: 58
	public abstract class SpecialAbility
	{
		// Token: 0x0600025B RID: 603 RVA: 0x0000F0B0 File Offset: 0x0000D2B0
		public SpecialAbility(Animations iAnimation, int iDisplayName)
		{
			this.mAnimation = iAnimation;
			this.mDisplayName = iDisplayName;
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000F0E4 File Offset: 0x0000D2E4
		public Animations Animation
		{
			get
			{
				return this.mAnimation;
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000F0EC File Offset: 0x0000D2EC
		public virtual bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			if (iOwner != null)
			{
				this.mTimeStamp = iOwner.PlayState.PlayTime;
			}
			return true;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000F103 File Offset: 0x0000D303
		public virtual bool Execute(ISpellCaster iOwner, Elements elements, PlayState iPlayState)
		{
			if (iOwner != null)
			{
				this.mTimeStamp = iOwner.PlayState.PlayTime;
			}
			return true;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000F11A File Offset: 0x0000D31A
		public virtual bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			return true;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000F120 File Offset: 0x0000D320
		private static Type GetAbilityType(string iName)
		{
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsSubclassOf(typeof(SpecialAbility)) && types[i].Name.Equals(iName, StringComparison.OrdinalIgnoreCase))
				{
					return types[i];
				}
			}
			return null;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000F170 File Offset: 0x0000D370
		public static SpecialAbility Read(ContentReader iInput)
		{
			string iName = iInput.ReadString();
			Type abilityType = SpecialAbility.GetAbilityType(iName);
			string value = iInput.ReadString();
			string text = iInput.ReadString();
			Animations animations = Animations.cast_self;
			if (!string.IsNullOrEmpty(value))
			{
				animations = (Animations)Enum.Parse(typeof(Animations), value, true);
			}
			Elements[] array = new Elements[iInput.ReadInt32()];
			Elements elements = Elements.None;
			if (array.Length > 0)
			{
				try
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (Elements)iInput.ReadInt32();
						elements |= array[i];
					}
					if (!string.IsNullOrEmpty(text))
					{
						int hashCodeCustom = text.ToLowerInvariant().GetHashCodeCustom();
						ConstructorInfo constructor = abilityType.GetConstructor(new Type[]
						{
							typeof(Animations),
							typeof(Elements[]),
							typeof(int)
						});
						return (SpecialAbility)constructor.Invoke(new object[]
						{
							animations,
							array,
							hashCodeCustom
						});
					}
					ConstructorInfo constructor2 = abilityType.GetConstructor(new Type[]
					{
						typeof(Animations),
						typeof(Elements[])
					});
					return (SpecialAbility)constructor2.Invoke(new object[]
					{
						animations,
						array
					});
				}
				catch (Exception)
				{
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				int hashCodeCustom2 = text.ToLowerInvariant().GetHashCodeCustom();
				ConstructorInfo constructor3 = abilityType.GetConstructor(new Type[]
				{
					typeof(Animations),
					typeof(int)
				});
				return (SpecialAbility)constructor3.Invoke(new object[]
				{
					animations,
					hashCodeCustom2
				});
			}
			ConstructorInfo constructor4 = abilityType.GetConstructor(new Type[]
			{
				typeof(Animations)
			});
			return (SpecialAbility)constructor4.Invoke(new object[]
			{
				animations
			});
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0000F3BC File Offset: 0x0000D5BC
		public int DisplayName
		{
			get
			{
				return this.mDisplayName;
			}
		}

		// Token: 0x040001D2 RID: 466
		private Animations mAnimation;

		// Token: 0x040001D3 RID: 467
		public static readonly int SOUND_MAGICK_FAIL = "magick_fail".GetHashCodeCustom();

		// Token: 0x040001D4 RID: 468
		protected static readonly Random RANDOM = new Random();

		// Token: 0x040001D5 RID: 469
		protected double mTimeStamp;

		// Token: 0x040001D6 RID: 470
		private long mLastExecute = DateTime.Now.Ticks;

		// Token: 0x040001D7 RID: 471
		private readonly int mDisplayName;
	}
}
