using System;
using System.Xml;
using Magicka.GameLogic;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000287 RID: 647
	internal class UnlockMagick : Action
	{
		// Token: 0x0600131C RID: 4892 RVA: 0x0007612C File Offset: 0x0007432C
		public UnlockMagick(Trigger iTrigger, GameScene iScene, XmlNode iNodE) : base(iTrigger, iScene)
		{
			this.mTextBox = new TextBox();
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x00076144 File Offset: 0x00074344
		protected override void Execute()
		{
			string text = LanguageManager.Instance.GetString(BookOfMagick.MAGICK_PICKUP_LOC);
			text = text.Replace("#1;", "[c=1,1,1]" + LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int)this.mMagickType]) + "[/c]");
			this.mTextBox.Initialize(this.mScene.Scene, MagickaFont.Maiandra14, text, default(Vector2), default(Vector2), true, 0, 2f);
			DialogManager.Instance.AddTextBox(this.mTextBox);
			if (base.GameScene.PlayState.GameType == GameType.Versus)
			{
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing)
					{
						SpellManager.Instance.UnlockMagick(players[i], this.mMagickType);
					}
				}
				return;
			}
			SpellManager.Instance.UnlockMagick(this.mMagickType, base.GameScene.PlayState.GameType);
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0007623E File Offset: 0x0007443E
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x00076246 File Offset: 0x00074446
		// (set) Token: 0x06001320 RID: 4896 RVA: 0x0007624E File Offset: 0x0007444E
		public MagickType MagickType
		{
			get
			{
				return this.mMagickType;
			}
			set
			{
				this.mMagickType = value;
			}
		}

		// Token: 0x040014D6 RID: 5334
		protected TextBox mTextBox;

		// Token: 0x040014D7 RID: 5335
		protected MagickType mMagickType;
	}
}
