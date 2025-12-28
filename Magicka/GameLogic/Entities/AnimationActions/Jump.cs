using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000130 RID: 304
	internal class Jump : AnimationAction
	{
		// Token: 0x06000884 RID: 2180 RVA: 0x000370CC File Offset: 0x000352CC
		public Jump(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mElevation = iInput.ReadSingle();
			this.mElevation = MathHelper.ToRadians(this.mElevation);
			if (iInput.ReadBoolean())
			{
				this.mMinRange = new float?(iInput.ReadSingle());
			}
			if (iInput.ReadBoolean())
			{
				this.mMaxRange = new float?(iInput.ReadSingle());
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00037130 File Offset: 0x00035330
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution)
			{
				NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
				if (nonPlayerCharacter != null)
				{
					Vector3 position = iOwner.Position;
					Vector3 vector = default(Vector3);
					Vector3 vector2 = default(Vector3);
					if (nonPlayerCharacter.AI.CurrentTarget != null)
					{
						vector = nonPlayerCharacter.AI.CurrentTarget.Position;
						vector2 = nonPlayerCharacter.AI.CurrentTarget.Body.Velocity;
					}
					else
					{
						vector = iOwner.Direction;
						Vector3.Multiply(ref vector, this.mMinRange.GetValueOrDefault() + (this.mMaxRange.GetValueOrDefault() - this.mMinRange.GetValueOrDefault()) * 0.5f, out vector);
					}
					if (vector2.LengthSquared() > 1E-45f)
					{
						Vector3.Multiply(ref vector2, 0.5f, out vector2);
						Vector3.Add(ref vector, ref vector2, out vector);
					}
					Vector3 iDelta;
					Vector3.Subtract(ref vector, ref position, out iDelta);
					float num = iDelta.LengthSquared();
					if (num >= 1E-45f)
					{
						Vector3.Normalize(ref iDelta, out iDelta);
						num *= 1.1f;
						if (this.mMinRange != null)
						{
							num = Math.Max(this.mMinRange.Value, num);
						}
						if (this.mMaxRange != null)
						{
							num = Math.Min(this.mMaxRange.Value, num);
						}
						Vector3.Multiply(ref iDelta, num, out iDelta);
						nonPlayerCharacter.CharacterBody.AllowMove = true;
						nonPlayerCharacter.Jump(iDelta, this.mElevation);
					}
				}
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x00037294 File Offset: 0x00035494
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000808 RID: 2056
		private float mElevation;

		// Token: 0x04000809 RID: 2057
		private float? mMinRange;

		// Token: 0x0400080A RID: 2058
		private float? mMaxRange;
	}
}
