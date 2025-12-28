using System;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x02000644 RID: 1604
	internal class MagickTree
	{
		// Token: 0x060030AB RID: 12459 RVA: 0x0018ED24 File Offset: 0x0018CF24
		public MagickTree()
		{
			this.root = new MagickNode(null);
		}

		// Token: 0x060030AC RID: 12460 RVA: 0x0018ED38 File Offset: 0x0018CF38
		public void GoToRoot()
		{
			this.currentNode = this.root;
		}

		// Token: 0x060030AD RID: 12461 RVA: 0x0018ED48 File Offset: 0x0018CF48
		public bool Move(Elements iElement, out MagickType oMagick)
		{
			MagickNode child = this.currentNode.GetChild(iElement);
			oMagick = MagickType.None;
			if (child == null)
			{
				oMagick = this.currentNode.Content;
				this.GoToRoot();
				return false;
			}
			this.currentNode = child;
			oMagick = this.currentNode.Content;
			return true;
		}

		// Token: 0x060030AE RID: 12462 RVA: 0x0018ED94 File Offset: 0x0018CF94
		public MagickNode MoveAndAdd(Elements iElement)
		{
			MagickNode magickNode = this.currentNode.GetChild(iElement);
			if (magickNode == null)
			{
				magickNode = new MagickNode(this.currentNode);
				this.currentNode.SetChild(iElement, magickNode);
			}
			this.currentNode = magickNode;
			return this.currentNode;
		}

		// Token: 0x040034A9 RID: 13481
		private MagickNode root;

		// Token: 0x040034AA RID: 13482
		private MagickNode currentNode;
	}
}
