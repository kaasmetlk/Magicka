using System;
using System.Collections.Generic;
using Magicka.Misc;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.CoreFramework
{
	// Token: 0x02000673 RID: 1651
	public class GameConsole : Singleton<GameConsole>
	{
		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x060031D4 RID: 12756 RVA: 0x00199B18 File Offset: 0x00197D18
		// (set) Token: 0x060031D5 RID: 12757 RVA: 0x00199B20 File Offset: 0x00197D20
		public bool Enabled
		{
			get
			{
				return this.mEnabled;
			}
			set
			{
				this.mEnabled = value;
			}
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x00199B29 File Offset: 0x00197D29
		public void Init(BitmapFont iFont)
		{
		}

		// Token: 0x060031D7 RID: 12759 RVA: 0x00199B2B File Offset: 0x00197D2B
		public void PushInput(GameConsole.DisplayTextDelegate iDelegate, int iPriority)
		{
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x00199B2D File Offset: 0x00197D2D
		public void PushInput(GameConsole.DisplayTextDelegate iDelegate)
		{
			this.PushInput(iDelegate, 1000);
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x00199B3B File Offset: 0x00197D3B
		public void Draw(GUIBasicEffect iEffect, float iPosX, float iPoxY)
		{
		}

		// Token: 0x0400363E RID: 13886
		private const int MAX_TEXT_SIZE = 5000;

		// Token: 0x0400363F RID: 13887
		public const int DEFAULT_PRIORITY = 1000;

		// Token: 0x04003640 RID: 13888
		private List<GameConsole.Input> mTextInputs = new List<GameConsole.Input>();

		// Token: 0x04003641 RID: 13889
		private Text mText;

		// Token: 0x04003642 RID: 13890
		private object mListLock = new object();

		// Token: 0x04003643 RID: 13891
		private bool mEnabled = true;

		// Token: 0x02000674 RID: 1652
		// (Invoke) Token: 0x060031DC RID: 12764
		public delegate string DisplayTextDelegate();

		// Token: 0x02000675 RID: 1653
		private class Input
		{
			// Token: 0x060031DF RID: 12767 RVA: 0x00199B62 File Offset: 0x00197D62
			public Input(int iPriority, GameConsole.DisplayTextDelegate iDelegate)
			{
				this.Priority = iPriority;
				this.Delegate = iDelegate;
			}

			// Token: 0x04003644 RID: 13892
			public readonly int Priority;

			// Token: 0x04003645 RID: 13893
			public readonly GameConsole.DisplayTextDelegate Delegate;
		}
	}
}
