using System;
using System.IO;
using Magicka.Localization;
using Microsoft.Xna.Framework.Content;
using PolygonHead;

namespace Magicka.Graphics
{
	// Token: 0x020003F1 RID: 1009
	internal class FontManager
	{
		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06001ECD RID: 7885 RVA: 0x000D7720 File Offset: 0x000D5920
		public static FontManager Instance
		{
			get
			{
				if (FontManager.mSingelton == null)
				{
					lock (FontManager.mSingeltonLock)
					{
						if (FontManager.mSingelton == null)
						{
							FontManager.mSingelton = new FontManager();
						}
					}
				}
				return FontManager.mSingelton;
			}
		}

		// Token: 0x06001ECE RID: 7886 RVA: 0x000D7774 File Offset: 0x000D5974
		private FontManager()
		{
			this.mContent = new ContentManager(Game.Instance.Content.ServiceProvider, "content");
			this.mFonts = new BitmapFont[16];
			this.mFonts[0] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra14");
			this.mFonts[1] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra16");
			this.mFonts[1].Spacing = 1;
			this.mFonts[2] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Maiandra18");
			this.mFonts[2].Spacing = 1;
			this.mFonts[3] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross28");
			this.mFonts[3].Spacing = 1;
			this.mFonts[4] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross36");
			this.mFonts[4].Spacing = 1;
			this.mFonts[5] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Stonecross50");
			this.mFonts[5].Spacing = 1;
			this.mFonts[9] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/Versustext");
			this.mFonts[9].Spacing = 1;
			this.mFonts[6] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuDefault");
			this.mFonts[6].Spacing = 1;
			this.mFonts[7] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuOption");
			this.mFonts[8] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/MenuTitle");
			this.mFonts[10] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUIFont");
			this.mFonts[10].Spacing = 1;
			this.mFonts[11] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUISmallFont");
			this.mFonts[11].Spacing = 1;
			this.mFonts[12] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUISmallHeaderFont");
			this.mFonts[13] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXUIBoldFont");
			this.mFonts[13].Spacing = 1;
			this.mFonts[14] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXPointsFont");
			this.mFonts[15] = this.mContent.Load<BitmapFont>("Languages/ENG/Font/PDXEditFont");
		}

		// Token: 0x06001ECF RID: 7887 RVA: 0x000D79CC File Offset: 0x000D5BCC
		public void LoadFonts(Language iLanguage)
		{
			if (Directory.Exists("content/Languages/" + iLanguage.ToString() + "/font"))
			{
				lock (Game.Instance.GraphicsDevice)
				{
					this.mContent.Unload();
					this.mFonts[0].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Maiandra14"));
					this.mFonts[1].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Maiandra16"));
					this.mFonts[1].Spacing = 1;
					this.mFonts[2].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Maiandra18"));
					this.mFonts[2].Spacing = 1;
					this.mFonts[3].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Stonecross28"));
					this.mFonts[3].Spacing = 1;
					this.mFonts[4].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Stonecross36"));
					this.mFonts[4].Spacing = 1;
					this.mFonts[5].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Stonecross50"));
					this.mFonts[5].Spacing = 1;
					this.mFonts[9].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/Versustext"));
					this.mFonts[9].Spacing = 1;
					this.mFonts[6].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/MenuDefault"));
					this.mFonts[6].Spacing = 1;
					this.mFonts[7].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/MenuOption"));
					this.mFonts[8].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/MenuTitle"));
					this.mFonts[10].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXUIFont"));
					this.mFonts[10].Spacing = 1;
					this.mFonts[11].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXUISmallFont"));
					this.mFonts[11].Spacing = 1;
					this.mFonts[12].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXUISmallHeaderFont"));
					this.mFonts[13].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXUIBoldFont"));
					this.mFonts[13].Spacing = 1;
					this.mFonts[14].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXPointsFont"));
					this.mFonts[15].Read(this.mContent.Load<BitmapFont>("Languages/" + iLanguage.ToString() + "/Font/PDXEditFont"));
					return;
				}
			}
			this.LoadFonts(Language.eng);
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x000D7E00 File Offset: 0x000D6000
		public BitmapFont GetFont(MagickaFont iFont)
		{
			return this.mFonts[(int)iFont];
		}

		// Token: 0x04002144 RID: 8516
		private ContentManager mContent;

		// Token: 0x04002145 RID: 8517
		private BitmapFont[] mFonts;

		// Token: 0x04002146 RID: 8518
		private static FontManager mSingelton;

		// Token: 0x04002147 RID: 8519
		private static volatile object mSingeltonLock = new object();
	}
}
