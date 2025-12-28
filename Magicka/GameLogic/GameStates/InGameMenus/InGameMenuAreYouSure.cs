using System;
using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.InGameMenus
{
	// Token: 0x02000341 RID: 833
	internal class InGameMenuAreYouSure : InGameMenu
	{
		// Token: 0x0600196B RID: 6507 RVA: 0x000AB97C File Offset: 0x000A9B7C
		public override void LanguageChanged()
		{
			base.LanguageChanged();
			LanguageManager instance = LanguageManager.Instance;
			string text = "";
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				text = text + this.mFont.Wrap(instance.GetString(this.mTexts[i]), 300, true) + '\n';
			}
			this.mRUSureText.SetText(text);
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x000AB9E8 File Offset: 0x000A9BE8
		public InGameMenuAreYouSure(Action iYesCallback, params int[] iText)
		{
			this.mTexts = iText;
			this.mYesCallback = iYesCallback;
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
			this.AddMenuTextItem("#add_menu_yes".GetHashCodeCustom(), this.mFont, TextAlign.Center);
			this.AddMenuTextItem("#add_menu_no".GetHashCodeCustom(), this.mFont, TextAlign.Center);
			this.mRUSureText = new Text(512, this.mFont, TextAlign.Center, false);
			LanguageManager instance = LanguageManager.Instance;
			string text = "";
			for (int i = 0; i < this.mTexts.Length; i++)
			{
				text = text + this.mFont.Wrap(instance.GetString(this.mTexts[i]), 300, true) + '\n';
			}
			this.mRUSureText.SetText(text);
			this.mTextHeight = this.mFont.MeasureText(this.mRUSureText.Characters, true).Y;
			this.mBackgroundSize = new Vector2(400f, 300f);
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000ABAF0 File Offset: 0x000A9CF0
		public override void UpdatePositions()
		{
			Vector2 position = default(Vector2);
			position.X = InGameMenu.sScreenSize.X * 0.5f;
			position.Y = InGameMenu.sScreenSize.Y * 0.5f + 40f * InGameMenu.sScale;
			for (int i = 0; i < this.mMenuItems.Count; i++)
			{
				MenuItem menuItem = this.mMenuItems[i];
				menuItem.Scale = InGameMenu.sScale;
				menuItem.Position = position;
				position.Y += menuItem.BottomRight.Y - menuItem.TopLeft.Y;
			}
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x000ABB99 File Offset: 0x000A9D99
		protected override string IGetHighlightedButtonName()
		{
			if (this.mSelectedItem != 0)
			{
				return "no";
			}
			return "yes";
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x000ABBB0 File Offset: 0x000A9DB0
		protected override void IControllerSelect(Controller iSender)
		{
			if (this.mSelectedItem == 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
				this.mYesCallback.Invoke();
				return;
			}
			if (this.mSelectedItem > 0)
			{
				AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
				InGameMenu.PopMenu();
			}
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x000ABC01 File Offset: 0x000A9E01
		protected override void IControllerBack(Controller iSender)
		{
			InGameMenu.PopMenu();
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x000ABC08 File Offset: 0x000A9E08
		protected override void OnEnter()
		{
			if (InGameMenu.sController is KeyboardMouseController)
			{
				this.mSelectedItem = -1;
				return;
			}
			this.mSelectedItem = 1;
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x000ABC28 File Offset: 0x000A9E28
		protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
		{
			base.IDraw(iDeltaTime, ref iBackgroundSize);
			InGameMenu.sEffect.Color = this.mMenuItems[0].Color;
			this.mRUSureText.Draw(InGameMenu.sEffect, InGameMenu.sScreenSize.X * 0.5f, InGameMenu.sScreenSize.Y * 0.5f + 30f * InGameMenu.sScale - this.mTextHeight * InGameMenu.sScale, InGameMenu.sScale);
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x000ABCA6 File Offset: 0x000A9EA6
		protected override void OnExit()
		{
		}

		// Token: 0x04001BA1 RID: 7073
		private const string OPTION_YES = "yes";

		// Token: 0x04001BA2 RID: 7074
		private const string OPTION_NO = "no";

		// Token: 0x04001BA3 RID: 7075
		private int[] mTexts;

		// Token: 0x04001BA4 RID: 7076
		private Action mYesCallback;

		// Token: 0x04001BA5 RID: 7077
		private BitmapFont mFont;

		// Token: 0x04001BA6 RID: 7078
		private Text mRUSureText;

		// Token: 0x04001BA7 RID: 7079
		private float mTextHeight;
	}
}
