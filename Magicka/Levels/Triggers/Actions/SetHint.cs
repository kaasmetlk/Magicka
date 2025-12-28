using System;
using System.Xml;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.Gamers;
using Magicka.Graphics;
using Magicka.Localization;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000330 RID: 816
	internal class SetHint : Action
	{
		// Token: 0x060018ED RID: 6381 RVA: 0x000A4061 File Offset: 0x000A2261
		public SetHint(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x000A4074 File Offset: 0x000A2274
		public override void Initialize()
		{
			base.Initialize();
			string text = this.mHint;
			if (this.mAppend)
			{
				bool flag = false;
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing && !(players[i].Gamer is NetworkGamer))
					{
						flag = !(players[i].Controller is KeyboardMouseController);
						break;
					}
				}
				if (flag)
				{
					text += "_pad";
				}
				else
				{
					text += "_key";
				}
			}
			this.mHintHash = text.ToLowerInvariant().GetHashCodeCustom();
			LanguageManager instance = LanguageManager.Instance;
			string iText = instance.GetString(this.mHintHash);
			iText = instance.ParseReferences(iText);
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
			this.mHintText = font.Wrap(iText, 300, true);
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x000A4151 File Offset: 0x000A2351
		protected override void Execute()
		{
			TutorialManager.Instance.SetHint(this.mHintHash, this.mHintText, this.mAnimation, this.mHintPosition);
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x000A4175 File Offset: 0x000A2375
		public override void QuickExecute()
		{
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x060018F1 RID: 6385 RVA: 0x000A4177 File Offset: 0x000A2377
		// (set) Token: 0x060018F2 RID: 6386 RVA: 0x000A417F File Offset: 0x000A237F
		public bool AppendController
		{
			get
			{
				return this.mAppend;
			}
			set
			{
				this.mAppend = value;
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x060018F3 RID: 6387 RVA: 0x000A4188 File Offset: 0x000A2388
		// (set) Token: 0x060018F4 RID: 6388 RVA: 0x000A4190 File Offset: 0x000A2390
		public string ID
		{
			get
			{
				return this.mHint;
			}
			set
			{
				this.mHint = value;
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x060018F5 RID: 6389 RVA: 0x000A4199 File Offset: 0x000A2399
		// (set) Token: 0x060018F6 RID: 6390 RVA: 0x000A41A1 File Offset: 0x000A23A1
		public TutorialManager.HintAnimations Animation
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

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x060018F7 RID: 6391 RVA: 0x000A41AA File Offset: 0x000A23AA
		// (set) Token: 0x060018F8 RID: 6392 RVA: 0x000A41B2 File Offset: 0x000A23B2
		public TutorialManager.Position Position
		{
			get
			{
				return this.mHintPosition;
			}
			set
			{
				this.mHintPosition = value;
			}
		}

		// Token: 0x04001ABC RID: 6844
		private const string KEYBOARD = "_key";

		// Token: 0x04001ABD RID: 6845
		private const string PAD = "_pad";

		// Token: 0x04001ABE RID: 6846
		private string mHint;

		// Token: 0x04001ABF RID: 6847
		private string mHintText;

		// Token: 0x04001AC0 RID: 6848
		private int mHintHash;

		// Token: 0x04001AC1 RID: 6849
		private bool mAppend;

		// Token: 0x04001AC2 RID: 6850
		private TutorialManager.HintAnimations mAnimation;

		// Token: 0x04001AC3 RID: 6851
		private TutorialManager.Position mHintPosition = TutorialManager.Position.BottomRight;
	}
}
