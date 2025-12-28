using System;
using JigLibX.Geometry;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000176 RID: 374
	public class Crouch : AnimationAction
	{
		// Token: 0x06000B66 RID: 2918 RVA: 0x00044218 File Offset: 0x00042418
		public Crouch(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mRadius = iInput.ReadSingle();
			this.mLength = iInput.ReadSingle();
			if (this.mLength <= 0f)
			{
				this.mLength = 0.01f;
			}
			if (this.mRadius <= 0f)
			{
				this.mRadius = 0.01f;
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x00044278 File Offset: 0x00042478
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				Capsule capsule = iOwner.Body.CollisionSkin.GetPrimitiveNewWorld(0) as Capsule;
				if (capsule.Length != this.mLength || capsule.Radius != this.mRadius)
				{
					iOwner.SetCapsuleForm(this.mLength, this.mRadius);
				}
			}
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x000442CD File Offset: 0x000424CD
		public override void Kill(Character iOwner)
		{
			base.Kill(iOwner);
			iOwner.ResetCapsuleForm();
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x000442DC File Offset: 0x000424DC
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000A59 RID: 2649
		private float mRadius;

		// Token: 0x04000A5A RID: 2650
		private float mLength;
	}
}
