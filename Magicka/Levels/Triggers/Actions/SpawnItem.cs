using System;
using Magicka.GameLogic.Entities.Items;
using Magicka.Network;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000015 RID: 21
	public class SpawnItem : Action
	{
		// Token: 0x0600008B RID: 139 RVA: 0x00006424 File Offset: 0x00004624
		public SpawnItem(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00006438 File Offset: 0x00004638
		public override void Initialize()
		{
			base.Initialize();
			this.mPlaceholder = new Item(base.GameScene.PlayState, null);
			base.GameScene.PlayState.Content.Load<Item>("Data/Items/Wizard/" + this.mItem);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00006488 File Offset: 0x00004688
		protected override void Execute()
		{
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				return;
			}
			Matrix matrix;
			base.GameScene.GetLocator(this.mAreaHash, out matrix);
			Magicka.GameLogic.Entities.Items.Item.GetCachedWeapon(this.mItemHash, this.mPlaceholder);
			this.mPlaceholder.OnPickup = this.mOnPickUp;
			this.mPlaceholder.Detach();
			Vector3 translation = matrix.Translation;
			Matrix orientation = matrix;
			orientation.Translation = default(Vector3);
			this.mPlaceholder.Body.MoveTo(translation, orientation);
			base.GameScene.PlayState.EntityManager.AddEntity(this.mPlaceholder);
			this.mPlaceholder.Body.Immovable = !this.mPhysicsEnabled;
			this.mPlaceholder.IgnoreTractorPull = this.mIgnoreTractorPull;
			if (!string.IsNullOrEmpty(this.mIDString))
			{
				this.mPlaceholder.SetUniqueID(this.mIDHash);
			}
			this.mPlaceholder.Body.EnableBody();
			AnimatedLevelPart animatedLevelPart;
			if (this.mAttachToAnimatedLevelPartID != 0 && this.mScene.Level.CurrentScene.LevelModel.AnimatedLevelParts.TryGetValue(this.mAttachToAnimatedLevelPartID, out animatedLevelPart))
			{
				animatedLevelPart.AddEntity(this.mPlaceholder);
				this.mPlaceholder.AnimatedLevelPartID = this.mAttachToAnimatedLevelPartID;
			}
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.SpawnItem;
				triggerActionMessage.Handle = this.mPlaceholder.Handle;
				triggerActionMessage.Template = this.mItemHash;
				triggerActionMessage.Position = this.mPlaceholder.Position;
				triggerActionMessage.Bool0 = this.mPhysicsEnabled;
				triggerActionMessage.Bool1 = this.mIgnoreTractorPull;
				triggerActionMessage.Point0 = this.mOnPickUp;
				triggerActionMessage.Point1 = this.mAttachToAnimatedLevelPartID;
				triggerActionMessage.Direction = default(Vector3);
				Quaternion.CreateFromRotationMatrix(ref orientation, out triggerActionMessage.Orientation);
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006682 File Offset: 0x00004882
		public override void QuickExecute()
		{
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00006684 File Offset: 0x00004884
		// (set) Token: 0x06000090 RID: 144 RVA: 0x0000668C File Offset: 0x0000488C
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000091 RID: 145 RVA: 0x000066A6 File Offset: 0x000048A6
		// (set) Token: 0x06000092 RID: 146 RVA: 0x000066AE File Offset: 0x000048AE
		public string Item
		{
			get
			{
				return this.mItem;
			}
			set
			{
				this.mItem = value;
				this.mItemHash = this.mItem.GetHashCodeCustom();
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000066C8 File Offset: 0x000048C8
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000066D0 File Offset: 0x000048D0
		public bool IgnoreTractorPull
		{
			get
			{
				return this.mIgnoreTractorPull;
			}
			set
			{
				this.mIgnoreTractorPull = value;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000095 RID: 149 RVA: 0x000066D9 File Offset: 0x000048D9
		// (set) Token: 0x06000096 RID: 150 RVA: 0x000066E1 File Offset: 0x000048E1
		public string AttachTo
		{
			get
			{
				return this.mAttachToAnimatedLevelPart;
			}
			set
			{
				this.mAttachToAnimatedLevelPart = value;
				this.mAttachToAnimatedLevelPartID = this.mAttachToAnimatedLevelPart.GetHashCodeCustom();
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000066FB File Offset: 0x000048FB
		// (set) Token: 0x06000098 RID: 152 RVA: 0x00006703 File Offset: 0x00004903
		public int Nr
		{
			get
			{
				return this.mNr;
			}
			set
			{
				this.mNr = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000099 RID: 153 RVA: 0x0000670C File Offset: 0x0000490C
		// (set) Token: 0x0600009A RID: 154 RVA: 0x00006714 File Offset: 0x00004914
		public bool PhysicsEnabled
		{
			get
			{
				return this.mPhysicsEnabled;
			}
			set
			{
				this.mPhysicsEnabled = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600009B RID: 155 RVA: 0x0000671D File Offset: 0x0000491D
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00006725 File Offset: 0x00004925
		public string OnPickup
		{
			get
			{
				return this.mOnPickupTrigger;
			}
			set
			{
				this.mOnPickupTrigger = value;
				this.mOnPickUp = this.mOnPickupTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000673F File Offset: 0x0000493F
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00006747 File Offset: 0x00004947
		public string ID
		{
			get
			{
				return this.mIDString;
			}
			set
			{
				this.mIDString = value;
				this.mIDHash = this.mIDString.GetHashCodeCustom();
			}
		}

		// Token: 0x0400007A RID: 122
		private string mArea;

		// Token: 0x0400007B RID: 123
		private int mAreaHash;

		// Token: 0x0400007C RID: 124
		private string mItem;

		// Token: 0x0400007D RID: 125
		private int mItemHash;

		// Token: 0x0400007E RID: 126
		private string mOnPickupTrigger;

		// Token: 0x0400007F RID: 127
		private int mOnPickUp;

		// Token: 0x04000080 RID: 128
		private int mNr = 1;

		// Token: 0x04000081 RID: 129
		private bool mPhysicsEnabled;

		// Token: 0x04000082 RID: 130
		private bool mIgnoreTractorPull;

		// Token: 0x04000083 RID: 131
		private int mAttachToAnimatedLevelPartID;

		// Token: 0x04000084 RID: 132
		private string mAttachToAnimatedLevelPart;

		// Token: 0x04000085 RID: 133
		private Item mPlaceholder;

		// Token: 0x04000086 RID: 134
		private string mIDString;

		// Token: 0x04000087 RID: 135
		private int mIDHash;
	}
}
