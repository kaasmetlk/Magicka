using System;
using Magicka.GameLogic.Spells.SpellEffects;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020002F9 RID: 761
	public class CastSpell : AnimationAction
	{
		// Token: 0x06001793 RID: 6035 RVA: 0x0009B07C File Offset: 0x0009927C
		public CastSpell(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mFromStaff = iInput.ReadBoolean();
			if (!this.mFromStaff)
			{
				this.mSource = iInput.ReadString();
			}
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0009B0A6 File Offset: 0x000992A6
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution && iOwner.CurrentSpell == null)
			{
				iOwner.CastSpell(this.mFromStaff, this.mSource);
			}
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x0009B0C8 File Offset: 0x000992C8
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			SpellEffect currentSpell = iOwner.CurrentSpell;
			if (currentSpell != null)
			{
				currentSpell.AnimationEnd(iOwner);
			}
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x0009B0ED File Offset: 0x000992ED
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04001938 RID: 6456
		private bool mFromStaff;

		// Token: 0x04001939 RID: 6457
		private string mSource;
	}
}
