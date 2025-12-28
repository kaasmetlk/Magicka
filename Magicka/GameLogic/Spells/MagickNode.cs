using System;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x020005A4 RID: 1444
	public class MagickNode
	{
		// Token: 0x06002B2C RID: 11052 RVA: 0x00153786 File Offset: 0x00151986
		public MagickNode(MagickNode parent)
		{
			this.mParent = new WeakReference(parent);
			this.mChildren = new MagickNode[11];
		}

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06002B2E RID: 11054 RVA: 0x001537B0 File Offset: 0x001519B0
		// (set) Token: 0x06002B2D RID: 11053 RVA: 0x001537A7 File Offset: 0x001519A7
		public MagickType Content
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

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06002B2F RID: 11055 RVA: 0x001537B8 File Offset: 0x001519B8
		public MagickNode Parent
		{
			get
			{
				return this.mParent.Target as MagickNode;
			}
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x001537CC File Offset: 0x001519CC
		public MagickNode GetChild(Elements iElement)
		{
			int num = Spell.ElementIndex(iElement);
			if (num < 11)
			{
				return this.mChildren[num];
			}
			return null;
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x001537EF File Offset: 0x001519EF
		public void SetChild(Elements iElement, MagickNode iChild)
		{
			this.mChildren[Spell.ElementIndex(iElement)] = iChild;
		}

		// Token: 0x04002E67 RID: 11879
		private WeakReference mParent;

		// Token: 0x04002E68 RID: 11880
		private MagickNode[] mChildren;

		// Token: 0x04002E69 RID: 11881
		private MagickType mMagickType;
	}
}
