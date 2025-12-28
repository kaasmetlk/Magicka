using System;
using Magicka.DRM;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Levels.Packs
{
	// Token: 0x020004FE RID: 1278
	internal class MagickPack : IPack
	{
		// Token: 0x060025CD RID: 9677 RVA: 0x00111A04 File Offset: 0x0010FC04
		public MagickPack(int iID, string iName, string iDesc, Texture2D iThumbTexture, Point iThumb, uint iAppID, MagickType[] iMagicks)
		{
			this.mID = iID;
			this.mAppID = iAppID;
			this.mNameID = iName.ToLowerInvariant().GetHashCodeCustom();
			this.mDescID = iDesc.ToLowerInvariant().GetHashCodeCustom();
			this.mThumbOffset.X = 64f * (float)iThumb.X / (float)iThumbTexture.Width;
			this.mThumbOffset.Y = 64f * (float)iThumb.Y / (float)iThumbTexture.Height;
			this.mMagicks = iMagicks;
			this.mIsUsed = DLC_StatusHelper.Instance.Item_IsUnused("MagickPack", this.mName, this.mAppID, false);
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x060025CE RID: 9678 RVA: 0x00111AC4 File Offset: 0x0010FCC4
		// (set) Token: 0x060025CF RID: 9679 RVA: 0x00111ACC File Offset: 0x0010FCCC
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

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x060025D0 RID: 9680 RVA: 0x00111AE0 File Offset: 0x0010FCE0
		// (set) Token: 0x060025D1 RID: 9681 RVA: 0x00111AE8 File Offset: 0x0010FCE8
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

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x060025D2 RID: 9682 RVA: 0x00111B27 File Offset: 0x0010FD27
		public bool IsUsed
		{
			get
			{
				return this.mIsUsed;
			}
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x00111B2F File Offset: 0x0010FD2F
		public void SetUsed(bool forceSave)
		{
			if (this.mLicense == HackHelper.License.Yes)
			{
				DLC_StatusHelper.Instance.Item_TrySetUsed("MagickPack", this.mName, forceSave);
				this.mIsUsed = true;
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x060025D4 RID: 9684 RVA: 0x00111B57 File Offset: 0x0010FD57
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x060025D5 RID: 9685 RVA: 0x00111B5F File Offset: 0x0010FD5F
		public int Name
		{
			get
			{
				return this.mNameID;
			}
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x060025D6 RID: 9686 RVA: 0x00111B67 File Offset: 0x0010FD67
		public int Descritpion
		{
			get
			{
				return this.mDescID;
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x060025D7 RID: 9687 RVA: 0x00111B6F File Offset: 0x0010FD6F
		public uint StoreURL
		{
			get
			{
				return this.mAppID;
			}
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x060025D8 RID: 9688 RVA: 0x00111B77 File Offset: 0x0010FD77
		public MagickType[] Magicks
		{
			get
			{
				return this.mMagicks;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x060025D9 RID: 9689 RVA: 0x00111B7F File Offset: 0x0010FD7F
		public Vector2 ThumbOffset
		{
			get
			{
				return this.mThumbOffset;
			}
		}

		// Token: 0x04002919 RID: 10521
		private HackHelper.License mLicense = HackHelper.License.Yes;

		// Token: 0x0400291A RID: 10522
		private bool mEnabled = true;

		// Token: 0x0400291B RID: 10523
		private bool mIsUsed;

		// Token: 0x0400291C RID: 10524
		private int mID;

		// Token: 0x0400291D RID: 10525
		private string mName;

		// Token: 0x0400291E RID: 10526
		private string mDescription;

		// Token: 0x0400291F RID: 10527
		private int mNameID;

		// Token: 0x04002920 RID: 10528
		private int mDescID;

		// Token: 0x04002921 RID: 10529
		private Vector2 mThumbOffset;

		// Token: 0x04002922 RID: 10530
		private uint mAppID;

		// Token: 0x04002923 RID: 10531
		private MagickType[] mMagicks;
	}
}
