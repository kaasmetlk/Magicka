using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200053F RID: 1343
	public class Move : AnimationAction
	{
		// Token: 0x060027F5 RID: 10229 RVA: 0x00123E5A File Offset: 0x0012205A
		public Move(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mVelocity = iInput.ReadVector3();
		}

		// Token: 0x060027F6 RID: 10230 RVA: 0x00123E70 File Offset: 0x00122070
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			float speed = iOwner.AnimationController.Speed;
			Matrix orientation = iOwner.CharacterBody.Orientation;
			Vector3 additionalForce;
			Vector3.Transform(ref this.mVelocity, ref orientation, out additionalForce);
			Vector3.Multiply(ref additionalForce, speed, out additionalForce);
			iOwner.CharacterBody.AdditionalForce = additionalForce;
		}

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x060027F7 RID: 10231 RVA: 0x00123EBA File Offset: 0x001220BA
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04002B84 RID: 11140
		private Vector3 mVelocity;
	}
}
