using System;
using PolygonHead;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x020002F1 RID: 753
	internal class MultiContextMenu : ContextMenu
	{
		// Token: 0x06001734 RID: 5940 RVA: 0x00095844 File Offset: 0x00093A44
		public MultiContextMenu(BitmapFont iFont, TextAlign iAlignment, int? iWidth) : base(iFont, iAlignment, iWidth)
		{
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x00095850 File Offset: 0x00093A50
		public int AddOption(int[] iLocValues)
		{
			lock (this.mNames)
			{
				this.mTextScales.Add(1f);
				this.mTexts.Add(new Text(64, this.mFont, this.mAlignment, false));
				this.mNames.Add(iLocValues);
			}
			this.LanguageChanged();
			return this.mTexts.Count - 1;
		}
	}
}
