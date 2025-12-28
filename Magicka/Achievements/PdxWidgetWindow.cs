using System;
using System.IO;
using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.Achievements
{
	// Token: 0x020002B8 RID: 696
	internal abstract class PdxWidgetWindow : AchievementWindow
	{
		// Token: 0x060014FB RID: 5371 RVA: 0x000833BA File Offset: 0x000815BA
		protected PdxWidgetWindow()
		{
			PdxWidgetWindow.EnsureInitialized();
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x000833C8 File Offset: 0x000815C8
		private static void EnsureInitialized()
		{
			if (PdxWidgetWindow.sTexture == null)
			{
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					PdxWidgetWindow.sTexture = Texture2D.FromFile(graphicsDevice, "content/connectui/widget.png");
				}
				PdxWidgetWindow.sInvTextureSize.X = 1f / (float)PdxWidgetWindow.sTexture.Width;
				PdxWidgetWindow.sInvTextureSize.Y = 1f / (float)PdxWidgetWindow.sTexture.Height;
				TextReader textReader = File.OpenText("content/connectui/widget.txt");
				int num = int.Parse(textReader.ReadLine());
				if (43 != num)
				{
					throw new Exception("Invalid nr of textures in Widget.png!");
				}
				PdxWidgetWindow.sRectangles = new Rectangle[num];
				for (int i = 0; i < num; i++)
				{
					string text = textReader.ReadLine();
					text = text.Substring(0, text.Length - 4);
					PdxWidgetWindow.Textures textures = (PdxWidgetWindow.Textures)Enum.Parse(typeof(PdxWidgetWindow.Textures), text, true);
					PdxWidgetWindow.sRectangles[(int)textures].X = int.Parse(textReader.ReadLine());
					PdxWidgetWindow.sRectangles[(int)textures].Y = int.Parse(textReader.ReadLine());
					PdxWidgetWindow.sRectangles[(int)textures].Width = int.Parse(textReader.ReadLine());
					PdxWidgetWindow.sRectangles[(int)textures].Height = int.Parse(textReader.ReadLine());
				}
			}
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x0008353C File Offset: 0x0008173C
		protected static void CreateVertices(Vector4[] iVertices, ref int iIndex, PdxWidgetWindow.Textures iTexture)
		{
			PdxWidgetWindow.EnsureInitialized();
			PdxWidgetWindow.CreateVertices(iVertices, ref iIndex, ref PdxWidgetWindow.sRectangles[(int)iTexture]);
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x00083558 File Offset: 0x00081758
		public static void CreateVertices(Vector4[] iVertices, ref int iIndex, ref Rectangle iSourceRect)
		{
			PdxWidgetWindow.EnsureInitialized();
			iVertices[iIndex].X = 0f;
			iVertices[iIndex].Y = 0f;
			iVertices[iIndex].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
			iVertices[iIndex].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
			iIndex++;
			iVertices[iIndex].X = (float)iSourceRect.Width;
			iVertices[iIndex].Y = 0f;
			iVertices[iIndex].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
			iVertices[iIndex].W = (float)iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
			iIndex++;
			iVertices[iIndex].X = (float)iSourceRect.Width;
			iVertices[iIndex].Y = (float)iSourceRect.Height;
			iVertices[iIndex].Z = (float)(iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
			iVertices[iIndex].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
			iIndex++;
			iVertices[iIndex].X = 0f;
			iVertices[iIndex].Y = (float)iSourceRect.Height;
			iVertices[iIndex].Z = (float)iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
			iVertices[iIndex].W = (float)(iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
			iIndex++;
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0008372E File Offset: 0x0008192E
		public static void GetTexture(PdxWidgetWindow.Textures iTexture, out Texture2D oTexture, out Rectangle oRect)
		{
			PdxWidgetWindow.EnsureInitialized();
			oTexture = PdxWidgetWindow.sTexture;
			oRect = PdxWidgetWindow.sRectangles[(int)iTexture];
		}

		// Token: 0x06001500 RID: 5376
		public abstract void OnMouseDown(ref Vector2 iMousePos);

		// Token: 0x06001501 RID: 5377
		public abstract void OnMouseUp(ref Vector2 iMousePos);

		// Token: 0x06001502 RID: 5378
		public abstract void OnMouseMove(ref Vector2 iMousePos);

		// Token: 0x06001503 RID: 5379
		public abstract void OnKeyDown(KeyData iData);

		// Token: 0x06001504 RID: 5380
		public abstract void OnKeyPress(char iChar);

		// Token: 0x0400165B RID: 5723
		protected static Texture2D sTexture;

		// Token: 0x0400165C RID: 5724
		protected static Vector2 sInvTextureSize;

		// Token: 0x0400165D RID: 5725
		protected static Rectangle[] sRectangles;

		// Token: 0x020002B9 RID: 697
		public enum Textures
		{
			// Token: 0x0400165F RID: 5727
			frame_profilebox_middle,
			// Token: 0x04001660 RID: 5728
			frame_profilebox_top,
			// Token: 0x04001661 RID: 5729
			frame_profilebox_bottom,
			// Token: 0x04001662 RID: 5730
			popup_info,
			// Token: 0x04001663 RID: 5731
			progress_indicator_off,
			// Token: 0x04001664 RID: 5732
			progress_indicator_on,
			// Token: 0x04001665 RID: 5733
			popup_input_error,
			// Token: 0x04001666 RID: 5734
			popup_input_info,
			// Token: 0x04001667 RID: 5735
			badge_game,
			// Token: 0x04001668 RID: 5736
			frame_points,
			// Token: 0x04001669 RID: 5737
			badge_achievement_on,
			// Token: 0x0400166A RID: 5738
			badge_achievement_off,
			// Token: 0x0400166B RID: 5739
			textfield_focus,
			// Token: 0x0400166C RID: 5740
			textfield_notfocus,
			// Token: 0x0400166D RID: 5741
			menu_highlight_right,
			// Token: 0x0400166E RID: 5742
			menu_right,
			// Token: 0x0400166F RID: 5743
			menu_highlight_left,
			// Token: 0x04001670 RID: 5744
			menu_left,
			// Token: 0x04001671 RID: 5745
			button_big_empty_off,
			// Token: 0x04001672 RID: 5746
			button_big_empty_on,
			// Token: 0x04001673 RID: 5747
			button_big_empty_off_d,
			// Token: 0x04001674 RID: 5748
			button_big_empty_on_d,
			// Token: 0x04001675 RID: 5749
			button_arrow_left_off,
			// Token: 0x04001676 RID: 5750
			button_arrow_left_on,
			// Token: 0x04001677 RID: 5751
			button_arrow_right_off,
			// Token: 0x04001678 RID: 5752
			button_arrow_right_on,
			// Token: 0x04001679 RID: 5753
			button_arrow_left_off_d,
			// Token: 0x0400167A RID: 5754
			button_arrow_left_on_d,
			// Token: 0x0400167B RID: 5755
			button_arrow_right_off_d,
			// Token: 0x0400167C RID: 5756
			button_arrow_right_on_d,
			// Token: 0x0400167D RID: 5757
			points_number_0,
			// Token: 0x0400167E RID: 5758
			points_number_1,
			// Token: 0x0400167F RID: 5759
			points_number_2,
			// Token: 0x04001680 RID: 5760
			points_number_3,
			// Token: 0x04001681 RID: 5761
			points_number_4,
			// Token: 0x04001682 RID: 5762
			points_number_5,
			// Token: 0x04001683 RID: 5763
			points_number_6,
			// Token: 0x04001684 RID: 5764
			points_number_7,
			// Token: 0x04001685 RID: 5765
			points_number_8,
			// Token: 0x04001686 RID: 5766
			points_number_9,
			// Token: 0x04001687 RID: 5767
			button_empty_off,
			// Token: 0x04001688 RID: 5768
			button_empty_on,
			// Token: 0x04001689 RID: 5769
			textfield_cursor,
			// Token: 0x0400168A RID: 5770
			NR_OF_TEXTURES
		}
	}
}
