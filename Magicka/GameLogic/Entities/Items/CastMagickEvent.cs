using System;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A8 RID: 1192
	public struct CastMagickEvent
	{
		// Token: 0x0600240A RID: 9226 RVA: 0x00102C80 File Offset: 0x00100E80
		public CastMagickEvent(ContentReader iInput)
		{
			string value = iInput.ReadString();
			this.MagickType = (MagickType)Enum.Parse(typeof(MagickType), value, true);
			Elements[] array = new Elements[iInput.ReadInt32()];
			this.CombinedElements = Elements.None;
			if (array.Length > 0)
			{
				try
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (Elements)iInput.ReadInt32();
						this.CombinedElements |= array[i];
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x00102D08 File Offset: 0x00100F08
		public CastMagickEvent(MagickType iType)
		{
			this.MagickType = iType;
			this.CombinedElements = Elements.None;
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x00102D18 File Offset: 0x00100F18
		public CastMagickEvent(MagickType iType, Elements elements)
		{
			this.MagickType = iType;
			this.CombinedElements = elements;
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x00102D28 File Offset: 0x00100F28
		public void Execute(Entity iItem, Entity iTarget)
		{
			Magick magick = default(Magick);
			magick.MagickType = this.MagickType;
			magick.Element = this.CombinedElements;
			ISpellCaster spellCaster = null;
			if (iItem is Item)
			{
				spellCaster = (iItem as Item).Owner;
			}
			else if (iItem is MissileEntity)
			{
				spellCaster = ((iItem as MissileEntity).Owner as ISpellCaster);
			}
			if (spellCaster != null)
			{
				if (magick.Effect is ITargetAbility)
				{
					(magick.Effect as ITargetAbility).Execute(spellCaster, iTarget, iItem.PlayState);
					return;
				}
				if (magick.Element != Elements.None)
				{
					magick.Effect.Execute(iItem as ISpellCaster, magick.Element, iItem.PlayState);
					return;
				}
				magick.Effect.Execute(spellCaster, iItem.PlayState);
				return;
			}
			else
			{
				if (!(iItem is ISpellCaster))
				{
					if (iItem != null)
					{
						magick.Effect.Execute(iItem.Position, iItem.PlayState);
					}
					return;
				}
				if (magick.Element == Elements.None)
				{
					magick.Effect.Execute(iItem as ISpellCaster, iItem.PlayState);
					return;
				}
				magick.Effect.Execute(iItem as ISpellCaster, magick.Element, iItem.PlayState);
				return;
			}
		}

		// Token: 0x04002710 RID: 10000
		public MagickType MagickType;

		// Token: 0x04002711 RID: 10001
		public Elements CombinedElements;
	}
}
