using System;
using System.IO;
using Magicka.DRM;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Levels.Packs
{
	// Token: 0x020004FD RID: 1277
	internal class ItemPack : IPack
	{
		// Token: 0x060025BF RID: 9663 RVA: 0x001117F0 File Offset: 0x0010F9F0
		public ItemPack(int iID, string iName, string iDesc, Texture2D iThumbTexture, Point iThumb, uint iAppID, string[] iItems)
		{
			this.mID = iID;
			this.mAppID = iAppID;
			this.mName = iName;
			this.mDescription = iDesc;
			this.mNameID = iName.ToLowerInvariant().GetHashCodeCustom();
			this.mDescID = iDesc.ToLowerInvariant().GetHashCodeCustom();
			this.mThumbOffset.X = 64f * (float)iThumb.X / (float)iThumbTexture.Width;
			this.mThumbOffset.Y = 64f * (float)iThumb.Y / (float)iThumbTexture.Height;
			this.mItems = new string[iItems.Length];
			this.mItemIDs = new int[iItems.Length];
			for (int i = 0; i < this.mItems.Length; i++)
			{
				string text = iItems[i].ToLowerInvariant();
				this.mItems[i] = text.Substring("content".Length + 1, text.Length - 4 - ("content".Length + 1));
				text = Path.GetFileNameWithoutExtension(text);
				this.mItemIDs[i] = text.GetHashCodeCustom();
			}
			this.mIsUsed = DLC_StatusHelper.Instance.Item_IsUnused("ItemPack", this.mName, this.mAppID, false);
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x060025C0 RID: 9664 RVA: 0x00111935 File Offset: 0x0010FB35
		// (set) Token: 0x060025C1 RID: 9665 RVA: 0x0011193D File Offset: 0x0010FB3D
		public HackHelper.License License
		{
			get
			{
				return this.mLicense;
			}
			set
			{
				this.mLicense = value;
				if (value != HackHelper.License.Yes)
				{
					this.mEnabled = false;
				}
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x060025C2 RID: 9666 RVA: 0x00111951 File Offset: 0x0010FB51
		// (set) Token: 0x060025C3 RID: 9667 RVA: 0x0011195C File Offset: 0x0010FB5C
		public bool Enabled
		{
			get
			{
				return this.mEnabled;
			}
			set
			{
				bool flag = this.mEnabled;
				this.mEnabled = (value & this.mLicense == HackHelper.License.Yes);
				if (flag != this.mEnabled)
				{
					PackMan.Instance.OnPackEnabledChanged(this, this.mEnabled);
				}
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x060025C4 RID: 9668 RVA: 0x0011199B File Offset: 0x0010FB9B
		public bool IsUsed
		{
			get
			{
				return this.mIsUsed;
			}
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x001119A3 File Offset: 0x0010FBA3
		public void SetUsed(bool forceSave)
		{
			if (this.mLicense == HackHelper.License.Yes)
			{
				DLC_StatusHelper.Instance.Item_TrySetUsed("ItemPack", this.mName, forceSave);
				this.mIsUsed = true;
			}
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x060025C6 RID: 9670 RVA: 0x001119CB File Offset: 0x0010FBCB
		public uint StoreURL
		{
			get
			{
				return this.mAppID;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x060025C7 RID: 9671 RVA: 0x001119D3 File Offset: 0x0010FBD3
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x060025C8 RID: 9672 RVA: 0x001119DB File Offset: 0x0010FBDB
		public int Name
		{
			get
			{
				return this.mNameID;
			}
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x060025C9 RID: 9673 RVA: 0x001119E3 File Offset: 0x0010FBE3
		public int Descritpion
		{
			get
			{
				return this.mDescID;
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x060025CA RID: 9674 RVA: 0x001119EB File Offset: 0x0010FBEB
		public string[] Items
		{
			get
			{
				return this.mItems;
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x060025CB RID: 9675 RVA: 0x001119F3 File Offset: 0x0010FBF3
		public int[] ItemIDs
		{
			get
			{
				return this.mItemIDs;
			}
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x060025CC RID: 9676 RVA: 0x001119FB File Offset: 0x0010FBFB
		public Vector2 ThumbOffset
		{
			get
			{
				return this.mThumbOffset;
			}
		}

		// Token: 0x0400290D RID: 10509
		private HackHelper.License mLicense = HackHelper.License.Yes;

		// Token: 0x0400290E RID: 10510
		private bool mEnabled = true;

		// Token: 0x0400290F RID: 10511
		private bool mIsUsed;

		// Token: 0x04002910 RID: 10512
		private int mID;

		// Token: 0x04002911 RID: 10513
		private string mName;

		// Token: 0x04002912 RID: 10514
		private string mDescription;

		// Token: 0x04002913 RID: 10515
		private Vector2 mThumbOffset;

		// Token: 0x04002914 RID: 10516
		private uint mAppID;

		// Token: 0x04002915 RID: 10517
		private int mNameID;

		// Token: 0x04002916 RID: 10518
		private int mDescID;

		// Token: 0x04002917 RID: 10519
		private string[] mItems;

		// Token: 0x04002918 RID: 10520
		private int[] mItemIDs;
	}
}
