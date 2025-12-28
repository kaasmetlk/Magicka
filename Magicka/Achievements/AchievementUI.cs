using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.Achievements
{
	// Token: 0x020004E7 RID: 1255
	public class AchievementUI
	{
		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06002543 RID: 9539 RVA: 0x0010F51C File Offset: 0x0010D71C
		public static AchievementUI Instance
		{
			get
			{
				if (AchievementUI.sSingelton == null)
				{
					lock (AchievementUI.sSingeltonLock)
					{
						if (AchievementUI.sSingelton == null)
						{
							AchievementUI.sSingelton = new AchievementUI();
						}
					}
				}
				return AchievementUI.sSingelton;
			}
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x0010F570 File Offset: 0x0010D770
		private AchievementUI()
		{
			string str = "achievementsUI/";
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			this.mPopupTexture = Texture2D.FromFile(graphicsDevice, str + "popup.png");
			TextReader textReader = File.OpenText(str + "popup.txt");
			int num = int.Parse(textReader.ReadLine());
			if (num < 0 || num > 1)
			{
				textReader.Close();
				throw new Exception("Invalid value in popup.txt");
			}
			textReader.ReadLine();
			this.mPopupRect.X = int.Parse(textReader.ReadLine());
			this.mPopupRect.Y = int.Parse(textReader.ReadLine());
			this.mPopupRect.Width = int.Parse(textReader.ReadLine());
			this.mPopupRect.Height = int.Parse(textReader.ReadLine());
			textReader.Close();
			this.mWidgetTexture = Texture2D.FromFile(graphicsDevice, str + "widget.png");
			textReader = File.OpenText(str + "widget.txt");
			num = int.Parse(textReader.ReadLine());
			for (int i = 0; i < num; i++)
			{
				textReader.ReadLine().ToLowerInvariant();
				Rectangle rectangle = default(Rectangle);
				rectangle.X = int.Parse(textReader.ReadLine());
				rectangle.Y = int.Parse(textReader.ReadLine());
				rectangle.Width = int.Parse(textReader.ReadLine());
				rectangle.Height = int.Parse(textReader.ReadLine());
			}
		}

		// Token: 0x040028A6 RID: 10406
		private static AchievementUI sSingelton;

		// Token: 0x040028A7 RID: 10407
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040028A8 RID: 10408
		private Rectangle mPopupRect;

		// Token: 0x040028A9 RID: 10409
		private Texture2D mPopupTexture;

		// Token: 0x040028AA RID: 10410
		private Texture2D mWidgetTexture;

		// Token: 0x020004E8 RID: 1256
		private class WidgetRenderData : IRenderableGUIObject
		{
			// Token: 0x06002546 RID: 9542 RVA: 0x0010F6F4 File Offset: 0x0010D8F4
			public void Draw(float iDeltaTime)
			{
				throw new NotImplementedException();
			}

			// Token: 0x170008A8 RID: 2216
			// (get) Token: 0x06002547 RID: 9543 RVA: 0x0010F6FB File Offset: 0x0010D8FB
			public int ZIndex
			{
				get
				{
					throw new NotImplementedException();
				}
			}
		}

		// Token: 0x020004E9 RID: 1257
		private class PopupRenderData : IRenderableGUIObject
		{
			// Token: 0x06002549 RID: 9545 RVA: 0x0010F70A File Offset: 0x0010D90A
			public void Draw(float iDeltaTime)
			{
				throw new NotImplementedException();
			}

			// Token: 0x170008A9 RID: 2217
			// (get) Token: 0x0600254A RID: 9546 RVA: 0x0010F711 File Offset: 0x0010D911
			public int ZIndex
			{
				get
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
