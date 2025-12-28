using System;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.Items;
using XNAnimation;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000332 RID: 818
	internal class AddItemToAvatars : Action
	{
		// Token: 0x060018FE RID: 6398 RVA: 0x000A41F5 File Offset: 0x000A23F5
		public AddItemToAvatars(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x000A4200 File Offset: 0x000A2400
		public override void Initialize()
		{
			base.Initialize();
			try
			{
				this.mItem = base.GameScene.PlayState.Content.Load<Item>("data/items/wizard/" + this.mItemName);
			}
			catch (Exception)
			{
				try
				{
					this.mItem = base.GameScene.PlayState.Content.Load<Item>("data/items/npc/" + this.mItemName);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x000A4290 File Offset: 0x000A2490
		protected override void Execute()
		{
			if (this.mItem == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.mBone))
			{
				return;
			}
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					Avatar avatar = players[i].Avatar;
					if (avatar != null)
					{
						SkinnedModelBone skinnedModelBone = null;
						for (int j = 0; j < avatar.Model.SkeletonBones.Count; j++)
						{
							if (avatar.Model.SkeletonBones[j].Name.Equals(this.mBone, StringComparison.OrdinalIgnoreCase))
							{
								skinnedModelBone = avatar.Model.SkeletonBones[j];
								break;
							}
						}
						if (skinnedModelBone != null)
						{
							for (int k = 0; k < avatar.Equipment.Length; k++)
							{
								if (avatar.Equipment[k].AttachIndex < 0)
								{
									avatar.Equipment[k].Set(this.mItem, skinnedModelBone, null);
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x000A4396 File Offset: 0x000A2596
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06001902 RID: 6402 RVA: 0x000A439E File Offset: 0x000A259E
		// (set) Token: 0x06001903 RID: 6403 RVA: 0x000A43A6 File Offset: 0x000A25A6
		public string Item
		{
			get
			{
				return this.mItemName;
			}
			set
			{
				this.mItemName = value;
				this.mItemID = this.mItemName.GetHashCodeCustom();
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06001904 RID: 6404 RVA: 0x000A43C0 File Offset: 0x000A25C0
		// (set) Token: 0x06001905 RID: 6405 RVA: 0x000A43C8 File Offset: 0x000A25C8
		public string Bone
		{
			get
			{
				return this.mBone;
			}
			set
			{
				this.mBone = value;
			}
		}

		// Token: 0x04001AC5 RID: 6853
		private Item mItem;

		// Token: 0x04001AC6 RID: 6854
		private string mItemName;

		// Token: 0x04001AC7 RID: 6855
		private int mItemID;

		// Token: 0x04001AC8 RID: 6856
		private string mBone;
	}
}
