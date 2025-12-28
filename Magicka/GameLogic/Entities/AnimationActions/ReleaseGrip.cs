using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000586 RID: 1414
	public class ReleaseGrip : AnimationAction
	{
		// Token: 0x06002A42 RID: 10818 RVA: 0x0014BF77 File Offset: 0x0014A177
		public ReleaseGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x0014BF84 File Offset: 0x0014A184
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			DamageCollection5 damageCollection = default(DamageCollection5);
			if (iOwner.GrippedCharacter != null)
			{
				iOwner.GrippedCharacter.SetCollisionDamage(ref damageCollection);
			}
			iOwner.ReleaseAttachedCharacter();
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06002A44 RID: 10820 RVA: 0x0014BFB4 File Offset: 0x0014A1B4
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}
	}
}
