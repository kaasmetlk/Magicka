using System;
using Magicka.GameLogic.Entities.Abilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020003CB RID: 971
	public class ThrowGrip : AnimationAction
	{
		// Token: 0x06001DC5 RID: 7621 RVA: 0x000D1E8C File Offset: 0x000D008C
		public ThrowGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
		}

		// Token: 0x06001DC6 RID: 7622 RVA: 0x000D1E98 File Offset: 0x000D0098
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution & iOwner.IsGripping)
			{
				NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
				Vector3 direction;
				if (nonPlayerCharacter != null)
				{
					ThrowGrip throwGrip = nonPlayerCharacter.AI.BusyAbility as ThrowGrip;
					if (throwGrip == null)
					{
						return;
					}
					throwGrip.CalcThrow(nonPlayerCharacter.AI, out direction);
				}
				else
				{
					direction = iOwner.Direction;
					direction.Y = 0.1f;
				}
				iOwner.GrippedCharacter.CharacterBody.AddImpulseVelocity(ref direction);
				iOwner.ReleaseAttachedCharacter();
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x000D1F09 File Offset: 0x000D0109
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}
	}
}
