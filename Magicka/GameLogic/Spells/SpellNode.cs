using System;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x0200039C RID: 924
	internal class SpellNode
	{
		// Token: 0x06001C44 RID: 7236 RVA: 0x000C0A4F File Offset: 0x000BEC4F
		public SpellNode(SpellNode parent)
		{
			this.parent = new WeakReference(parent);
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x06001C46 RID: 7238 RVA: 0x000C0A6C File Offset: 0x000BEC6C
		// (set) Token: 0x06001C45 RID: 7237 RVA: 0x000C0A63 File Offset: 0x000BEC63
		public Spell Content
		{
			get
			{
				return this.spell;
			}
			set
			{
				this.spell = value;
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06001C47 RID: 7239 RVA: 0x000C0A74 File Offset: 0x000BEC74
		public SpellNode Parent
		{
			get
			{
				return this.parent.Target as SpellNode;
			}
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x000C0A88 File Offset: 0x000BEC88
		public SpellNode GetChild(ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Center:
				return this.centerChild;
			case ControllerDirection.Right:
				return this.rightChild;
			case ControllerDirection.Up:
				return this.upChild;
			case ControllerDirection.Left:
				return this.leftChild;
			case ControllerDirection.Down:
				return this.downChild;
			}
			return null;
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x000C0AE8 File Offset: 0x000BECE8
		public void SetChild(ControllerDirection iDirection, SpellNode iChild)
		{
			switch (iDirection)
			{
			case ControllerDirection.Center:
				this.centerChild = iChild;
				return;
			case ControllerDirection.Right:
				this.rightChild = iChild;
				return;
			case ControllerDirection.Up:
				this.upChild = iChild;
				return;
			case ControllerDirection.UpRight:
			case ControllerDirection.Right | ControllerDirection.Left:
			case ControllerDirection.UpLeft:
			case ControllerDirection.Right | ControllerDirection.Up | ControllerDirection.Left:
				break;
			case ControllerDirection.Left:
				this.leftChild = iChild;
				return;
			case ControllerDirection.Down:
				this.downChild = iChild;
				break;
			default:
				return;
			}
		}

		// Token: 0x04001E78 RID: 7800
		private WeakReference parent;

		// Token: 0x04001E79 RID: 7801
		private SpellNode centerChild;

		// Token: 0x04001E7A RID: 7802
		private SpellNode rightChild;

		// Token: 0x04001E7B RID: 7803
		private SpellNode upChild;

		// Token: 0x04001E7C RID: 7804
		private SpellNode leftChild;

		// Token: 0x04001E7D RID: 7805
		private SpellNode downChild;

		// Token: 0x04001E7E RID: 7806
		private Spell spell;
	}
}
