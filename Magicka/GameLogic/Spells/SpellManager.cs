using System;
using System.Collections.Generic;
using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Abilities.SpecialAbilities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Storage;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020002C1 RID: 705
	public class SpellManager
	{
		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001547 RID: 5447 RVA: 0x00088D18 File Offset: 0x00086F18
		public static SpellManager Instance
		{
			get
			{
				if (SpellManager.mSingelton == null)
				{
					lock (SpellManager.mSingeltonLock)
					{
						if (SpellManager.mSingelton == null)
						{
							SpellManager.mSingelton = new SpellManager();
						}
					}
				}
				return SpellManager.mSingelton;
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x00088D6C File Offset: 0x00086F6C
		private SpellManager()
		{
			this.mCombos = new Elements[35][];
			this.mSpellTree = new SpellTree();
			Spell content = default(Spell);
			this.mSpellTree.GoToRoot();
			SpellNode spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
			Spell.DefaultSpell(Elements.Life, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
			Spell.DefaultSpell(Elements.Water, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
			Spell.DefaultSpell(Elements.Cold, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
			Spell.DefaultSpell(Elements.Fire, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Up);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
			Spell.DefaultSpell(Elements.Arcane, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
			Spell.DefaultSpell(Elements.Lightning, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Right);
			Spell.DefaultSpell(Elements.Shield, out content);
			spellNode.Content = content;
			this.mSpellTree.GoToRoot();
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Down);
			spellNode = this.mSpellTree.MoveAndAdd(ControllerDirection.Left);
			Spell.DefaultSpell(Elements.Earth, out content);
			spellNode.Content = content;
			this.mMagickTree = new MagickTree();
			this.AddMagick(MagickType.Revive, new Elements[]
			{
				Elements.Life,
				Elements.Lightning
			});
			this.AddMagick(MagickType.Haste, new Elements[]
			{
				Elements.Lightning,
				Elements.Arcane,
				Elements.Fire
			});
			this.AddMagick(MagickType.Rain, new Elements[]
			{
				Elements.Water,
				Elements.Steam
			});
			this.AddMagick(MagickType.Grease, new Elements[]
			{
				Elements.Water,
				Elements.Earth,
				Elements.Life
			});
			this.AddMagick(MagickType.Teleport, new Elements[]
			{
				Elements.Lightning,
				Elements.Arcane,
				Elements.Lightning
			});
			this.AddMagick(MagickType.ThunderB, new Elements[]
			{
				Elements.Steam,
				Elements.Lightning,
				Elements.Arcane,
				Elements.Lightning
			});
			this.AddMagick(MagickType.Charm, new Elements[]
			{
				Elements.Life,
				Elements.Shield,
				Elements.Earth
			});
			this.AddMagick(MagickType.MeteorS, new Elements[]
			{
				Elements.Fire,
				Elements.Earth,
				Elements.Steam,
				Elements.Earth,
				Elements.Fire
			});
			this.AddMagick(MagickType.Fear, new Elements[]
			{
				Elements.Cold,
				Elements.Arcane,
				Elements.Shield
			});
			this.AddMagick(MagickType.SUndead, new Elements[]
			{
				Elements.Ice,
				Elements.Earth,
				Elements.Arcane,
				Elements.Cold
			});
			this.AddMagick(MagickType.Conflagration, new Elements[]
			{
				Elements.Steam,
				Elements.Fire,
				Elements.Steam,
				Elements.Fire,
				Elements.Steam
			});
			this.AddMagick(MagickType.SDeath, new Elements[]
			{
				Elements.Arcane,
				Elements.Cold,
				Elements.Ice,
				Elements.Cold,
				Elements.Arcane
			});
			this.AddMagick(MagickType.SPhoenix, new Elements[]
			{
				Elements.Life,
				Elements.Lightning,
				Elements.Fire
			});
			this.AddMagick(MagickType.Invisibility, new Elements[]
			{
				Elements.Arcane,
				Elements.Shield,
				Elements.Steam,
				Elements.Arcane
			});
			this.AddMagick(MagickType.SElemental, new Elements[]
			{
				Elements.Arcane,
				Elements.Shield,
				Elements.Earth,
				Elements.Steam,
				Elements.Arcane
			});
			this.AddMagick(MagickType.Blizzard, new Elements[]
			{
				Elements.Cold,
				Elements.Ice,
				Elements.Cold
			});
			this.AddMagick(MagickType.ThunderS, new Elements[]
			{
				Elements.Steam,
				Elements.Steam,
				Elements.Lightning,
				Elements.Arcane,
				Elements.Lightning
			});
			this.AddMagick(MagickType.Tornado, new Elements[]
			{
				Elements.Earth,
				Elements.Steam,
				Elements.Water,
				Elements.Steam
			});
			this.AddMagick(MagickType.Nullify, new Elements[]
			{
				Elements.Arcane,
				Elements.Shield
			});
			this.AddMagick(MagickType.Vortex, new Elements[]
			{
				Elements.Ice,
				Elements.Arcane,
				Elements.Ice,
				Elements.Shield,
				Elements.Ice
			});
			this.AddMagick(MagickType.Corporealize, new Elements[]
			{
				Elements.Arcane,
				Elements.Steam,
				Elements.Lightning,
				Elements.Shield,
				Elements.Arcane
			});
			this.AddMagick(MagickType.TimeWarp, new Elements[]
			{
				Elements.Cold,
				Elements.Shield
			});
			this.AddMagick(MagickType.CTD, new Elements[]
			{
				Elements.Lightning,
				Elements.Lightning,
				Elements.Fire,
				Elements.Life
			});
			this.AddMagick(MagickType.Napalm, new Elements[]
			{
				Elements.Steam,
				Elements.Earth,
				Elements.Life,
				Elements.Fire,
				Elements.Fire
			});
			this.AddMagick(MagickType.Portal, new Elements[]
			{
				Elements.Steam,
				Elements.Lightning,
				Elements.Shield
			});
			this.AddMagick(MagickType.TractorPull, new Elements[]
			{
				Elements.Earth,
				Elements.Arcane
			});
			this.AddMagick(MagickType.ProppMagick, new Elements[]
			{
				Elements.Fire,
				Elements.Steam,
				Elements.Arcane
			});
			this.AddMagick(MagickType.Levitate, new Elements[]
			{
				Elements.Steam,
				Elements.Arcane,
				Elements.Steam
			});
			this.AddMagick(MagickType.ChainLightning, new Elements[]
			{
				Elements.Lightning,
				Elements.Lightning,
				Elements.Lightning
			});
			this.AddMagick(MagickType.Confuse, new Elements[]
			{
				Elements.Arcane,
				Elements.Shield,
				Elements.Lightning
			});
			this.AddMagick(MagickType.Wave, new Elements[]
			{
				Elements.Earth,
				Elements.Steam,
				Elements.Earth,
				Elements.Steam,
				Elements.Earth
			});
			this.AddMagick(MagickType.PerformanceEnchantment, new Elements[]
			{
				Elements.Life,
				Elements.Fire,
				Elements.Lightning,
				Elements.Fire,
				Elements.Life
			});
			this.AddMagick(MagickType.JudgementSpray, new Elements[]
			{
				Elements.Ice,
				Elements.Ice,
				Elements.Arcane,
				Elements.Shield
			});
			this.AddMagick(MagickType.Amalgameddon, new Elements[]
			{
				Elements.Arcane,
				Elements.Water,
				Elements.Lightning
			});
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000894E8 File Offset: 0x000876E8
		private void AddMagick(MagickType iType, Elements[] iElements)
		{
			this.mMagickTree.GoToRoot();
			MagickNode magickNode = null;
			for (int i = 0; i < iElements.Length; i++)
			{
				magickNode = this.mMagickTree.MoveAndAdd(iElements[i]);
			}
			magickNode.Content = iType;
			this.mCombos[(int)magickNode.Content] = iElements;
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00089534 File Offset: 0x00087734
		public Spell Combine(StaticList<Spell> iSpells)
		{
			if (iSpells.Count == 0)
			{
				return default(Spell);
			}
			Spell spell = iSpells[0];
			if (spell.Element == Elements.All)
			{
				return spell;
			}
			for (int i = 1; i < iSpells.Count; i++)
			{
				spell += iSpells[i];
			}
			return spell;
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x0008958C File Offset: 0x0008778C
		public bool IsMagickAllowed(Player iPlayer, GameType iGameType, MagickType iMagick)
		{
			ulong num = 1UL << (int)iMagick;
			if (iGameType == GameType.Campaign | iGameType == GameType.Mythos)
			{
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && (players[i].UnlockedMagicks & num) != 0UL)
					{
						return true;
					}
				}
			}
			else if (iGameType == GameType.Challenge || iGameType == GameType.StoryChallange)
			{
				Player[] players2 = Game.Instance.Players;
				for (int j = 0; j < players2.Length; j++)
				{
					if (players2[j].Playing && (players2[j].UnlockedMagicks & num) != 0UL)
					{
						return true;
					}
				}
			}
			else
			{
				if (iPlayer == null)
				{
					return iMagick != MagickType.None;
				}
				return (iPlayer.UnlockedMagicks & 1UL << (int)iMagick) != 0UL;
			}
			return false;
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00089648 File Offset: 0x00087848
		public MagickType GetMagickType(Player iPlayer, PlayState iPlayState, StaticList<Spell> iSpells)
		{
			MagickType magickType = MagickType.None;
			this.mMagickTree.GoToRoot();
			for (int i = 0; i < iSpells.Count; i++)
			{
				this.mMagickTree.Move(iSpells[i].Element, out magickType);
			}
			if (magickType != MagickType.None && (this.IsMagickAllowed(iPlayer, iPlayState.GameType, magickType) || iPlayer == null))
			{
				return magickType;
			}
			return MagickType.None;
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x000896A8 File Offset: 0x000878A8
		public bool CombineMagick(Player iPlayer, GameType iGameType, StaticList<Spell> iSpells)
		{
			MagickType magickType = MagickType.None;
			bool flag = false;
			this.mMagickTree.GoToRoot();
			for (int i = 0; i < iSpells.Count; i++)
			{
				flag = this.mMagickTree.Move(iSpells[i].Element, out magickType);
				if (!flag)
				{
					break;
				}
			}
			if (flag && magickType != MagickType.None && (this.IsMagickAllowed(iPlayer, iGameType, magickType) || iPlayer == null))
			{
				iSpells.Clear();
				SpellMagickConverter spellMagickConverter = default(SpellMagickConverter);
				spellMagickConverter.Magick.Element = Elements.All;
				spellMagickConverter.Magick.MagickType = magickType;
				iSpells.Add(spellMagickConverter.Spell);
				return true;
			}
			return false;
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00089744 File Offset: 0x00087944
		public bool IsMagick(Player iPlayer, GameType iGameType, StaticList<Spell> iSpells)
		{
			MagickType magickType = MagickType.None;
			bool flag = false;
			this.mMagickTree.GoToRoot();
			for (int i = 0; i < iSpells.Count; i++)
			{
				flag = this.mMagickTree.Move(iSpells[i].Element, out magickType);
				if (!flag)
				{
					break;
				}
			}
			return flag && magickType != MagickType.None && (this.IsMagickAllowed(iPlayer, iGameType, magickType) || iPlayer == null);
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000897A8 File Offset: 0x000879A8
		public Spell HandleCombo(Player iPlayer)
		{
			StaticList<int> inputQueue = iPlayer.InputQueue;
			this.mSpellTree.GoToRoot();
			for (int i = 0; i < inputQueue.Count; i++)
			{
				Spell result;
				if (!this.mSpellTree.Move((ControllerDirection)inputQueue[i], out result))
				{
					if (inputQueue.Count <= 0)
					{
						return result;
					}
					inputQueue.RemoveAt(0);
				}
				else if (result.Element != Elements.None)
				{
					if (TutorialManager.Instance.IsElementEnabled(result.Element))
					{
						return result;
					}
					iPlayer.InputQueue.Clear();
				}
			}
			return default(Spell);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x00089838 File Offset: 0x00087A38
		public static bool InclusiveOpposites(Elements iA, Elements iB)
		{
			bool result = SpellManager.Opposites(iA, iB);
			if (((iA & Elements.Fire) == Elements.Fire || (iB & Elements.Fire) == Elements.Fire) && ((iA & Elements.Water) == Elements.Water || (iB & Elements.Water) == Elements.Water))
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x00089868 File Offset: 0x00087A68
		public static bool Opposites(Elements iA, Elements iB)
		{
			return (((iA & Elements.Arcane) == Elements.Arcane || (iB & Elements.Arcane) == Elements.Arcane) && ((iA & Elements.Life) == Elements.Life || (iB & Elements.Life) == Elements.Life)) || (((iA & Elements.Fire) == Elements.Fire || (iB & Elements.Fire) == Elements.Fire) && ((iA & Elements.Cold) == Elements.Cold || (iB & Elements.Cold) == Elements.Cold)) || (((iA & Elements.Earth) == Elements.Earth || (iB & Elements.Earth) == Elements.Earth) && ((iA & Elements.Lightning) == Elements.Lightning || (iB & Elements.Lightning) == Elements.Lightning)) || (((iA & Elements.Water) == Elements.Water || (iB & Elements.Water) == Elements.Water) && ((iA & Elements.Lightning) == Elements.Lightning || (iB & Elements.Lightning) == Elements.Lightning)) || ((iA & Elements.Shield) == Elements.Shield && (iB & Elements.Shield) == Elements.Shield);
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0008990C File Offset: 0x00087B0C
		public static bool Revertable(Elements iA, Elements iB)
		{
			return ((iA & Elements.Cold) == Elements.Cold && (iB & Elements.Steam) == Elements.Steam) || ((iB & Elements.Cold) == Elements.Cold && (iA & Elements.Steam) == Elements.Steam) || (((iA & Elements.Ice) == Elements.Ice && (iB & Elements.Fire) == Elements.Fire) || ((iB & Elements.Ice) == Elements.Ice && (iA & Elements.Fire) == Elements.Fire));
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0008996E File Offset: 0x00087B6E
		public static bool Combinable(Elements iA, Elements iB)
		{
			return ((iA & Elements.Water) == Elements.Water && (iB & Elements.Cold) == Elements.Cold) || ((iB & Elements.Water) == Elements.Water && (iA & Elements.Cold) == Elements.Cold) || (((iA & Elements.Water) == Elements.Water && (iB & Elements.Fire) == Elements.Fire) || ((iB & Elements.Water) == Elements.Water && (iA & Elements.Fire) == Elements.Fire));
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x000899A8 File Offset: 0x00087BA8
		public static int FindOpposites(StaticList<Spell> iList, int iLastIndex, Elements iElement)
		{
			for (int i = iLastIndex; i >= 0; i--)
			{
				if (SpellManager.Opposites(iElement, iList[i].Element))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x000899DC File Offset: 0x00087BDC
		public static int FindRevertables(StaticList<Spell> iList, int iLastIndex, Elements iElement)
		{
			for (int i = iLastIndex; i >= 0; i--)
			{
				if (SpellManager.Revertable(iElement, iList[i].Element))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x00089A10 File Offset: 0x00087C10
		public static int FindCombines(StaticList<Spell> iList, int iLastIndex, Elements iElement)
		{
			for (int i = iLastIndex; i >= 0; i--)
			{
				if (SpellManager.Combinable(iElement, iList[i].Element))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x00089A44 File Offset: 0x00087C44
		public static int FindDifferentElement(StaticList<Spell> iList, int iLastIndex, Elements iElement)
		{
			for (int i = iLastIndex; i >= 0; i--)
			{
				if (iElement != iList[i].Element)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x00089A74 File Offset: 0x00087C74
		public static void FindOppositesAndCombinables(Player iPlayer, Magicka.GameLogic.Entities.Character iOwner, StaticList<Spell> iList)
		{
			Elements elements = Elements.None;
			Elements elements2 = Elements.None;
			Elements iElementOut = Elements.None;
			for (int i = iList.Count - 1; i >= 0; i--)
			{
				Spell spell = iList[i];
				int num = SpellManager.FindOpposites(iList, i - 1, spell.Element);
				if (num >= 0)
				{
					elements = iList[num].Element;
					elements2 = iList[i].Element;
					if (iPlayer != null)
					{
						iPlayer.IconRenderer.TransformIconForRemoval(i, num);
					}
					iList.RemoveAt(num);
					i--;
					iList.RemoveAt(i);
				}
			}
			for (int j = iList.Count - 1; j >= 0; j--)
			{
				Spell a = iList[j];
				int num2 = SpellManager.FindRevertables(iList, j - 1, a.Element);
				if (num2 >= 0)
				{
					Spell value = a + iList[num2];
					elements = iList[num2].Element;
					elements2 = iList[j].Element;
					iElementOut = value.Element;
					if (iPlayer != null)
					{
						iPlayer.IconRenderer.TransformIconForCombine(value.Element, j, num2);
					}
					iList[num2] = value;
					iList.RemoveAt(j);
				}
			}
			for (int k = iList.Count - 1; k >= 0; k--)
			{
				Spell a2 = iList[k];
				int num3 = SpellManager.FindCombines(iList, k - 1, a2.Element);
				if (num3 >= 0)
				{
					Spell value2 = a2 + iList[num3];
					elements = iList[num3].Element;
					elements2 = iList[k].Element;
					iElementOut = value2.Element;
					if (iPlayer != null)
					{
						iPlayer.IconRenderer.TransformIconForCombine(value2.Element, k, num3);
					}
					iList[num3] = value2;
					iList.RemoveAt(k);
				}
			}
			if (elements != Elements.None && elements2 != Elements.None)
			{
				ChantSpellManager.Merge(iOwner, elements, elements2, iElementOut);
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00089C48 File Offset: 0x00087E48
		public bool TryAddToQueue(Player iPlayer, Magicka.GameLogic.Entities.Character iOwner, StaticList<Spell> iList, int iMaxCount, ref Spell iSpell)
		{
			if (iPlayer != null)
			{
				iPlayer.IconRenderer.AddIcon(iSpell.Element, iPlayer.Controller is DirectInputController);
			}
			ChantSpells chantSpells = new ChantSpells(iSpell.Element, iOwner);
			int num = iList.Count + 1;
			if (iList.Count < iMaxCount)
			{
				ChantSpellManager.Add(ref chantSpells);
				iList.Add(iSpell);
				SpellManager.FindOppositesAndCombinables(iPlayer, iOwner, iList);
			}
			else
			{
				Elements elements = Elements.None;
				Elements elements2 = Elements.None;
				Elements iElementOut = Elements.None;
				int num2 = SpellManager.FindOpposites(iList, iMaxCount - 1, iSpell.Element);
				if (num2 >= 0)
				{
					elements = iList[num2].Element;
					elements2 = iSpell.Element;
					if (iPlayer != null)
					{
						iPlayer.IconRenderer.TransformIconForRemoval(iMaxCount, num2);
					}
					iList.RemoveAt(num2);
				}
				else
				{
					num2 = SpellManager.FindRevertables(iList, iMaxCount - 1, iSpell.Element);
					if (num2 >= 0)
					{
						Spell value = iSpell + iList[num2];
						elements = iList[num2].Element;
						elements2 = iSpell.Element;
						iElementOut = value.Element;
						iList[num2] = value;
						if (iPlayer != null)
						{
							iPlayer.IconRenderer.TransformIconForCombine(value.Element, iMaxCount, num2);
						}
					}
					else
					{
						num2 = SpellManager.FindCombines(iList, iMaxCount - 1, iSpell.Element);
						if (num2 >= 0)
						{
							Spell value2 = iSpell + iList[num2];
							elements = iList[num2].Element;
							elements2 = iSpell.Element;
							iElementOut = value2.Element;
							iList[num2] = value2;
							if (iPlayer != null)
							{
								iPlayer.IconRenderer.TransformIconForCombine(value2.Element, iMaxCount, num2);
							}
						}
					}
				}
				ChantSpellManager.Add(ref chantSpells);
				if (elements != Elements.None && elements2 != Elements.None)
				{
					chantSpells.State = ChantSpellState.Orbiting;
					ChantSpellManager.Merge(iOwner, elements, elements2, iElementOut);
				}
			}
			return num == iList.Count;
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00089E21 File Offset: 0x00088021
		public void UnlockMagick(Player iPlayer, MagickType iMagick)
		{
			iPlayer.UnlockedMagicks |= 1UL << (int)iMagick;
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00089E38 File Offset: 0x00088038
		public void UnlockMagick(MagickType iMagick, GameType iGameType)
		{
			ulong num = 0UL;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				players[i].UnlockedMagicks |= 1UL << (int)iMagick;
				num |= players[i].UnlockedMagicks;
				if (iGameType == GameType.Campaign && (players[i].UnlockedMagicks & 8384510UL) == 8384510UL && players[i].Avatar != null)
				{
					AchievementsManager.Instance.AwardAchievement(players[i].Avatar.PlayState, "iputonmyrobeandwizar");
				}
			}
			if (iGameType == GameType.Campaign)
			{
				int num2 = Helper.CountSetBits(num & 8384510UL);
				for (int j = 0; j < SaveManager.Instance.SaveSlots.Length; j++)
				{
					SaveData saveData = SaveManager.Instance.SaveSlots[j];
					if (saveData != null)
					{
						int num3 = Helper.CountSetBits(saveData.UnlockedMagicks & 8384510UL);
						if (num3 > num2)
						{
							num2 = num3;
						}
					}
				}
				if ((iGameType == GameType.Campaign || iGameType == GameType.Mythos) && HackHelper.LicenseStatus != HackHelper.Status.Hacked)
				{
					SteamUserStats.IndicateAchievementProgress("iputonmyrobeandwizar", (uint)num2, (uint)Helper.CountSetBits(8384510UL));
					SteamUserStats.StoreStats();
				}
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00089F50 File Offset: 0x00088150
		public void AddSpellEffect(IAbilityEffect iSpellEffect)
		{
			if (!this.mEffects.Contains(iSpellEffect))
			{
				this.mEffects.Add(iSpellEffect);
			}
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00089F6C File Offset: 0x0008816C
		public bool IsEffectActive(Type iType)
		{
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				if (iType.IsInstanceOfType(this.mEffects[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00089FA8 File Offset: 0x000881A8
		public void ClearMagicks()
		{
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				if (this.mEffects[i] is SpecialAbility)
				{
					this.mEffects[i].OnRemove();
					this.mEffects.RemoveAt(i--);
				}
			}
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0008A000 File Offset: 0x00088200
		public void ClearEffects()
		{
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				this.mEffects[i].OnRemove();
			}
			this.mEffects.Clear();
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x0008A040 File Offset: 0x00088240
		public void Update(DataChannel iDataChannel, float iDeltaTime, PlayState iPlayState)
		{
			ChantSpellManager.Update(iDeltaTime);
			for (int i = 0; i < this.mEffects.Count; i++)
			{
				IAbilityEffect abilityEffect = this.mEffects[i];
				abilityEffect.Update(iDataChannel, iDeltaTime);
				if (abilityEffect.IsDead)
				{
					abilityEffect.OnRemove();
					this.mEffects.RemoveAt(i);
					i--;
				}
			}
			Portal.Update(iPlayState);
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x0008A0A2 File Offset: 0x000882A2
		public Elements[] GetMagickCombo(MagickType iType)
		{
			return this.mCombos[(int)iType];
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x0008A0AC File Offset: 0x000882AC
		public static bool Equatable(Elements[] iElements, StaticList<Spell> iSpellQue)
		{
			int num = Math.Min(iElements.Length, iSpellQue.Count);
			for (int i = 0; i < num; i++)
			{
				if ((iElements[i] & iSpellQue[i].Element) == Elements.None)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040016DE RID: 5854
		internal const ulong VANILLA_CAMPAIGN_MAGICKS = 8384510UL;

		// Token: 0x040016DF RID: 5855
		internal const ulong MYTHOS_CAMPAIGN_MAGICKS = 1040187402UL;

		// Token: 0x040016E0 RID: 5856
		private static SpellManager mSingelton;

		// Token: 0x040016E1 RID: 5857
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040016E2 RID: 5858
		private SpellTree mSpellTree;

		// Token: 0x040016E3 RID: 5859
		private MagickTree mMagickTree;

		// Token: 0x040016E4 RID: 5860
		private List<IAbilityEffect> mEffects = new List<IAbilityEffect>(32);

		// Token: 0x040016E5 RID: 5861
		private Elements[][] mCombos;
	}
}
