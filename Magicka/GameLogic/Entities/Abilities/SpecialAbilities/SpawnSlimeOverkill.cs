using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.Misc;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Abilities.SpecialAbilities
{
	// Token: 0x02000184 RID: 388
	public class SpawnSlimeOverkill : SpawnSlime
	{
		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000BCD RID: 3021 RVA: 0x000470F4 File Offset: 0x000452F4
		public new static SpawnSlimeOverkill Instance
		{
			get
			{
				if (SpawnSlimeOverkill.sSingelton == null)
				{
					lock (SpawnSlimeOverkill.sSingeltonLock)
					{
						if (SpawnSlimeOverkill.sSingelton == null)
						{
							SpawnSlimeOverkill.sSingelton = new SpawnSlimeOverkill();
						}
					}
				}
				return SpawnSlimeOverkill.sSingelton;
			}
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00047148 File Offset: 0x00045348
		private SpawnSlimeOverkill() : base(Animations.cast_magick_direct)
		{
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x00047152 File Offset: 0x00045352
		public SpawnSlimeOverkill(Animations iAnimation) : base(iAnimation)
		{
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0004715B File Offset: 0x0004535B
		private SpawnSlimeOverkill(Elements[] elements) : base(Animations.cast_magick_direct, elements)
		{
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00047166 File Offset: 0x00045366
		public SpawnSlimeOverkill(Animations iAnimation, Elements[] elements) : base(iAnimation, elements)
		{
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00047170 File Offset: 0x00045370
		public override bool Execute(Vector3 iPosition, PlayState iPlayState)
		{
			throw new Exception("New slime cannot be spawned without a parent (iOwner)!");
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0004717C File Offset: 0x0004537C
		public override bool Execute(ISpellCaster iOwner, PlayState iPlayState)
		{
			return this.Execute(iOwner, this.mElements, iPlayState);
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0004718C File Offset: 0x0004538C
		public override bool Execute(ISpellCaster iOwner, Elements elements, PlayState iPlayState)
		{
			if (iOwner != null)
			{
				this.mTimeStamp = iOwner.PlayState.PlayTime;
			}
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return false;
			}
			this.mOwner = iOwner;
			this.mPlayState = iPlayState;
			List<Pair<SpawnSlime.SlimeSize, Elements>> list = new List<Pair<SpawnSlime.SlimeSize, Elements>>();
			string name;
			switch (name = (iOwner as Character).Name)
			{
			case "dungeons_slimecube_poison_large":
			{
				Elements second;
				if (elements != Elements.None)
				{
					second = base.RandomElement(elements);
				}
				else
				{
					second = Elements.Poison;
				}
				for (int i = 0; i < 8; i++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second));
				}
				goto IL_225;
			}
			case "dungeons_slimecube_poison_medium":
			case "dungeons_slimecube_medium":
			case "dungeons_slimecube_poison":
			{
				Elements second2;
				if (elements != Elements.None)
				{
					second2 = base.RandomElement(elements);
				}
				else
				{
					second2 = Elements.Poison;
				}
				for (int j = 0; j < 3; j++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second2));
				}
				goto IL_225;
			}
			case "dungeons_slimecube_arcane":
			{
				Elements second3;
				if (elements != Elements.None)
				{
					second3 = base.RandomElement(elements);
				}
				else
				{
					second3 = Elements.Arcane;
				}
				for (int k = 0; k < 3; k++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second3));
				}
				goto IL_225;
			}
			case "dungeons_acolyte_a":
			{
				Elements second4;
				if (elements != Elements.None)
				{
					second4 = base.RandomElement(elements);
				}
				else
				{
					second4 = Elements.Poison;
				}
				for (int l = 0; l < 3; l++)
				{
					list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second4));
				}
				goto IL_225;
			}
			case "dungeons_acolyte_c":
			{
				Elements second5;
				if (elements != Elements.None)
				{
					second5 = base.RandomElement(elements);
				}
				else
				{
					second5 = Elements.Poison;
				}
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Medium, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
				list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second5));
				goto IL_225;
			}
			}
			Elements second6 = Elements.Poison;
			list.Add(new Pair<SpawnSlime.SlimeSize, Elements>(SpawnSlime.SlimeSize.Small, second6));
			IL_225:
			base.CreateEntities(list);
			return true;
		}

		// Token: 0x04000AD1 RID: 2769
		private static SpawnSlimeOverkill sSingelton;

		// Token: 0x04000AD2 RID: 2770
		private static volatile object sSingeltonLock = new object();
	}
}
