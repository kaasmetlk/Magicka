using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using Magicka.Storage;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200059F RID: 1439
	internal class AssignItem : Action
	{
		// Token: 0x06002AE8 RID: 10984 RVA: 0x00151954 File Offset: 0x0014FB54
		public AssignItem(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06002AE9 RID: 10985 RVA: 0x0015196C File Offset: 0x0014FB6C
		public override void Initialize()
		{
			base.Initialize();
			if (this.mItemID != AssignItem.DEFAULT_STAFF && this.mItemID != AssignItem.DEFAULT_WEAPON)
			{
				base.GameScene.PlayState.Content.Load<Item>("Data/Items/Wizard/" + this.mItem);
			}
		}

		// Token: 0x06002AEA RID: 10986 RVA: 0x001519BF File Offset: 0x0014FBBF
		protected override void Execute()
		{
			if (this.mAreaHash == AssignItem.ALLID && this.mPlayer == 0)
			{
				this.executeAny();
				return;
			}
			this.executeSpecified();
		}

		// Token: 0x06002AEB RID: 10987 RVA: 0x001519E4 File Offset: 0x0014FBE4
		public void executeAny()
		{
			Player[] players = Game.Instance.Players;
			if (this.mItemID == AssignItem.DEFAULT_STAFF)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && players[i].Avatar != null && !players[i].Avatar.Dead)
					{
						Avatar avatar = players[i].Avatar;
						string text = avatar.Template.Equipment[1].Item.Name;
						PlayerSaveData playerSaveData;
						if (this.mScene.PlayState.Info.Players.TryGetValue(avatar.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Staff))
						{
							text = playerSaveData.Staff;
						}
						Item.GetCachedWeapon(text.GetHashCodeCustom(), avatar.Equipment[1].Item);
						avatar.Player.Staff = text;
						if (this.mAnimation != Animations.None && !this.mQuickExecute)
						{
							avatar.Attack(this.mAnimation, true);
						}
					}
				}
				return;
			}
			if (this.mItemID == AssignItem.DEFAULT_WEAPON)
			{
				for (int j = 0; j < players.Length; j++)
				{
					if (players[j].Playing && players[j].Avatar != null && !players[j].Avatar.Dead)
					{
						Avatar avatar2 = players[j].Avatar;
						string text2 = avatar2.Template.Equipment[0].Item.Name;
						PlayerSaveData playerSaveData2;
						if (this.mScene.PlayState.Info.Players.TryGetValue(avatar2.Player.GamerTag, out playerSaveData2) && !string.IsNullOrEmpty(playerSaveData2.Weapon))
						{
							text2 = playerSaveData2.Weapon;
						}
						Item.GetCachedWeapon(text2.GetHashCodeCustom(), avatar2.Equipment[0].Item);
						avatar2.Player.Weapon = text2;
						if (this.mAnimation != Animations.None && !this.mQuickExecute)
						{
							avatar2.Attack(this.mAnimation, true);
						}
					}
				}
				return;
			}
			WeaponClass weaponClass = Item.GetCachedWeapon(this.mItemID).WeaponClass;
			for (int k = 0; k < players.Length; k++)
			{
				if (players[k].Playing && players[k].Avatar != null && !players[k].Avatar.Dead)
				{
					Avatar avatar3 = players[k].Avatar;
					if (weaponClass == WeaponClass.Staff)
					{
						Item.GetCachedWeapon(this.mItemID, avatar3.Equipment[1].Item);
						avatar3.Player.Staff = avatar3.Equipment[1].Item.Name;
					}
					else
					{
						Item.GetCachedWeapon(this.mItemID, avatar3.Equipment[0].Item);
						avatar3.Player.Weapon = avatar3.Equipment[0].Item.Name;
					}
					if (this.mAnimation != Animations.None && !this.mQuickExecute)
					{
						avatar3.Attack(this.mAnimation, true);
					}
				}
			}
		}

		// Token: 0x06002AEC RID: 10988 RVA: 0x00151CF8 File Offset: 0x0014FEF8
		public void executeSpecified()
		{
			Avatar avatar = null;
			if (this.mPlayer == 0)
			{
				TriggerArea triggerArea = base.GameScene.GetTriggerArea(this.mAreaHash);
				for (int i = 0; i < triggerArea.PresentCharacters.Count; i++)
				{
					avatar = (triggerArea.PresentCharacters[i] as Avatar);
					if (avatar != null)
					{
						break;
					}
				}
			}
			else
			{
				avatar = Game.Instance.Players[this.mPlayer - 1].Avatar;
			}
			if (avatar == null)
			{
				return;
			}
			if (this.mItemID == AssignItem.DEFAULT_STAFF)
			{
				if (!avatar.Dead)
				{
					Avatar avatar2 = avatar;
					string text = avatar2.Template.Equipment[1].Item.Name;
					PlayerSaveData playerSaveData;
					if (this.mScene.PlayState.Info.Players.TryGetValue(avatar2.Player.GamerTag, out playerSaveData) && !string.IsNullOrEmpty(playerSaveData.Staff))
					{
						text = playerSaveData.Staff;
					}
					WeaponClass weaponClass = Item.GetCachedWeapon(text.GetHashCodeCustom()).WeaponClass;
					Item.GetCachedWeapon(text.GetHashCodeCustom(), avatar2.Equipment[1].Item);
					avatar2.Player.Staff = text;
					if (this.mAnimation != Animations.None && !this.mQuickExecute)
					{
						avatar2.Attack(this.mAnimation, true);
						return;
					}
				}
			}
			else if (this.mItemID == AssignItem.DEFAULT_WEAPON)
			{
				if (!avatar.Dead)
				{
					Avatar avatar3 = avatar;
					string text2 = avatar3.Template.Equipment[0].Item.Name;
					PlayerSaveData playerSaveData2;
					if (this.mScene.PlayState.Info.Players.TryGetValue(avatar3.Player.GamerTag, out playerSaveData2) && !string.IsNullOrEmpty(playerSaveData2.Weapon))
					{
						text2 = playerSaveData2.Weapon;
					}
					WeaponClass weaponClass = Item.GetCachedWeapon(text2.GetHashCodeCustom()).WeaponClass;
					Item.GetCachedWeapon(text2.GetHashCodeCustom(), avatar3.Equipment[0].Item);
					avatar3.Player.Weapon = text2;
					if (this.mAnimation != Animations.None && !this.mQuickExecute)
					{
						avatar3.Attack(this.mAnimation, true);
						return;
					}
				}
			}
			else
			{
				WeaponClass weaponClass = Item.GetCachedWeapon(this.mItemID).WeaponClass;
				if (!avatar.Dead)
				{
					Avatar avatar4 = avatar;
					if (weaponClass == WeaponClass.Staff)
					{
						Item.GetCachedWeapon(this.mItemID, avatar4.Equipment[1].Item);
						avatar4.Player.Staff = avatar4.Equipment[1].Item.Name;
					}
					else
					{
						Item.GetCachedWeapon(this.mItemID, avatar4.Equipment[0].Item);
						avatar4.Player.Weapon = avatar4.Equipment[0].Item.Name;
					}
					if (this.mAnimation != Animations.None && !this.mQuickExecute)
					{
						avatar4.Attack(this.mAnimation, true);
					}
				}
			}
		}

		// Token: 0x06002AED RID: 10989 RVA: 0x00151FD7 File Offset: 0x001501D7
		public override void QuickExecute()
		{
			this.mQuickExecute = true;
			this.Execute();
			this.mQuickExecute = false;
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x06002AEE RID: 10990 RVA: 0x00151FED File Offset: 0x001501ED
		// (set) Token: 0x06002AEF RID: 10991 RVA: 0x00151FF5 File Offset: 0x001501F5
		public string ID
		{
			get
			{
				return this.mItem;
			}
			set
			{
				this.mItem = value;
				this.mItemID = value.GetHashCodeCustom();
			}
		}

		// Token: 0x17000A0D RID: 2573
		// (get) Token: 0x06002AF0 RID: 10992 RVA: 0x0015200A File Offset: 0x0015020A
		// (set) Token: 0x06002AF1 RID: 10993 RVA: 0x00152012 File Offset: 0x00150212
		public int Player
		{
			get
			{
				return this.mPlayer;
			}
			set
			{
				this.mPlayer = value;
			}
		}

		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06002AF2 RID: 10994 RVA: 0x0015201B File Offset: 0x0015021B
		// (set) Token: 0x06002AF3 RID: 10995 RVA: 0x00152023 File Offset: 0x00150223
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaHash = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06002AF4 RID: 10996 RVA: 0x0015203D File Offset: 0x0015023D
		// (set) Token: 0x06002AF5 RID: 10997 RVA: 0x00152045 File Offset: 0x00150245
		public Animations Animation
		{
			get
			{
				return this.mAnimation;
			}
			set
			{
				this.mAnimation = value;
			}
		}

		// Token: 0x04002E28 RID: 11816
		public static readonly int ALLID = "all".GetHashCodeCustom();

		// Token: 0x04002E29 RID: 11817
		private static readonly int DEFAULT_WEAPON = "weapon_default".GetHashCodeCustom();

		// Token: 0x04002E2A RID: 11818
		private static readonly int DEFAULT_STAFF = "staff_default".GetHashCodeCustom();

		// Token: 0x04002E2B RID: 11819
		private string mItem;

		// Token: 0x04002E2C RID: 11820
		private int mItemID;

		// Token: 0x04002E2D RID: 11821
		private int mPlayer;

		// Token: 0x04002E2E RID: 11822
		protected string mArea;

		// Token: 0x04002E2F RID: 11823
		protected int mAreaHash = AssignItem.ALLID;

		// Token: 0x04002E30 RID: 11824
		protected Animations mAnimation;

		// Token: 0x04002E31 RID: 11825
		private bool mQuickExecute;
	}
}
