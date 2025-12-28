using System;
using Magicka.Network;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020002A9 RID: 681
	public class OverkillGrip : AnimationAction
	{
		// Token: 0x06001499 RID: 5273 RVA: 0x0007FB99 File Offset: 0x0007DD99
		public OverkillGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x0007FBA3 File Offset: 0x0007DDA3
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iOwner.GrippedCharacter != null && NetworkManager.Instance.State != NetworkState.Client)
			{
				iOwner.GrippedCharacter.OverKill();
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x0007FBC5 File Offset: 0x0007DDC5
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}
	}
}
