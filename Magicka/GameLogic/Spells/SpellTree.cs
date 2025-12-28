using System;

namespace Magicka.GameLogic.Spells
{
	// Token: 0x0200041F RID: 1055
	internal class SpellTree
	{
		// Token: 0x060020AC RID: 8364 RVA: 0x000E7B29 File Offset: 0x000E5D29
		public SpellTree()
		{
			this.root = new SpellNode(null);
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x000E7B3D File Offset: 0x000E5D3D
		public void GoToRoot()
		{
			this.currentNode = this.root;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x000E7B4C File Offset: 0x000E5D4C
		public bool Move(ControllerDirection iDirection, out Spell oSpell)
		{
			SpellNode child = this.currentNode.GetChild(iDirection);
			oSpell = default(Spell);
			if (child == null)
			{
				oSpell = this.currentNode.Content;
				this.GoToRoot();
				return false;
			}
			this.currentNode = child;
			oSpell = this.currentNode.Content;
			return true;
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x000E7BA4 File Offset: 0x000E5DA4
		public SpellNode MoveAndAdd(ControllerDirection iDirection)
		{
			SpellNode spellNode = this.currentNode.GetChild(iDirection);
			if (spellNode == null)
			{
				spellNode = new SpellNode(this.currentNode);
				this.currentNode.SetChild(iDirection, spellNode);
			}
			this.currentNode = spellNode;
			return this.currentNode;
		}

		// Token: 0x0400232A RID: 9002
		private SpellNode root;

		// Token: 0x0400232B RID: 9003
		private SpellNode currentNode;
	}
}
