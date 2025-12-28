using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities.CharacterStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x0200060D RID: 1549
	public class Grip : AnimationAction
	{
		// Token: 0x06002E81 RID: 11905 RVA: 0x00179290 File Offset: 0x00177490
		public Grip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			Matrix matrix = Matrix.CreateRotationY(3.1415927f);
			this.mType = (Grip.GripType)iInput.ReadByte();
			this.mRadius = iInput.ReadSingle();
			this.mBreakFreeTolerance = (int)iInput.ReadSingle();
			if (this.mBreakFreeTolerance == 0)
			{
				this.mBreakFreeTolerance = 10;
			}
			string value = iInput.ReadString();
			if (!string.IsNullOrEmpty(value))
			{
				this.mUseGripAttach = true;
				for (int i = 0; i < iSkeleton.Count; i++)
				{
					SkinnedModelBone skinnedModelBone = iSkeleton[i];
					if (skinnedModelBone.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						this.mGripAttach.mIndex = (int)skinnedModelBone.Index;
						this.mGripAttach.mBindPose = skinnedModelBone.InverseBindPoseTransform;
						Matrix.Invert(ref this.mGripAttach.mBindPose, out this.mGripAttach.mBindPose);
						Matrix.Multiply(ref matrix, ref this.mGripAttach.mBindPose, out this.mGripAttach.mBindPose);
						break;
					}
				}
			}
			value = iInput.ReadString();
			if (!string.IsNullOrEmpty(value))
			{
				for (int j = 0; j < iSkeleton.Count; j++)
				{
					SkinnedModelBone skinnedModelBone2 = iSkeleton[j];
					if (skinnedModelBone2.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						this.mTargetAttach.mIndex = (int)skinnedModelBone2.Index;
						this.mTargetAttach.mBindPose = skinnedModelBone2.InverseBindPoseTransform;
						Matrix.Invert(ref this.mTargetAttach.mBindPose, out this.mGripAttach.mBindPose);
						Matrix.Multiply(ref matrix, ref this.mTargetAttach.mBindPose, out this.mTargetAttach.mBindPose);
						break;
					}
				}
			}
			else
			{
				this.mTargetAttach.mIndex = -1;
			}
			this.mFinishOnGrip = iInput.ReadBoolean();
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x00179440 File Offset: 0x00177640
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			NonPlayerCharacter nonPlayerCharacter = iOwner as NonPlayerCharacter;
			if (nonPlayerCharacter == null || nonPlayerCharacter.IsGripping)
			{
				return;
			}
			Character character = nonPlayerCharacter.AI.CurrentTarget as Character;
			if (character == null || character.IsGripped || character.Dead || character.Drowning)
			{
				return;
			}
			if (character.IsSolidSelfShielded)
			{
				return;
			}
			int iTolerance = this.mBreakFreeTolerance;
			float radius = this.mRadius;
			Vector3 vector;
			if (this.mUseGripAttach)
			{
				vector = this.mGripAttach.mBindPose.Translation;
				Vector3.Transform(ref vector, ref iOwner.AnimationController.SkinnedBoneTransforms[this.mGripAttach.mIndex], out vector);
			}
			else
			{
				radius = iOwner.Radius;
				vector = nonPlayerCharacter.Position;
			}
			Vector3 position = character.Position;
			Vector3 vector2 = vector;
			Vector3 vector3 = position;
			vector2.Y = (vector3.Y = 0f);
			float num;
			Vector3.DistanceSquared(ref vector2, ref vector3, out num);
			if (num < (character.Radius + radius) * (character.Radius + radius))
			{
				Segment iSeg;
				iSeg.Origin = nonPlayerCharacter.Position;
				Vector3.Subtract(ref position, ref iSeg.Origin, out iSeg.Delta);
				float num2;
				Vector3 vector4;
				Vector3 vector5;
				if (!iOwner.PlayState.Level.CurrentScene.SegmentIntersect(out num2, out vector4, out vector5, iSeg))
				{
					List<Shield> shields = iOwner.PlayState.EntityManager.Shields;
					bool flag = true;
					int num3 = 0;
					while (num3 < shields.Count && flag)
					{
						if (shields[num3].SegmentIntersect(out vector4, iSeg, this.mRadius))
						{
							flag = false;
							break;
						}
						num3++;
					}
					if (flag)
					{
						switch (this.mType)
						{
						case Grip.GripType.Pickup:
							nonPlayerCharacter.GripCharacter(character, this.mType, this.mGripAttach.mIndex, iTolerance);
							goto IL_1F8;
						case Grip.GripType.Ride:
							nonPlayerCharacter.GripCharacter(character, this.mType, this.mTargetAttach.mIndex, iTolerance);
							goto IL_1F8;
						}
						nonPlayerCharacter.GripCharacter(character, this.mType, this.mTargetAttach.mIndex, iTolerance);
						IL_1F8:
						if (this.mFinishOnGrip)
						{
							nonPlayerCharacter.ChangeState(GrippingState.Instance);
						}
					}
				}
			}
		}

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06002E83 RID: 11907 RVA: 0x00179659 File Offset: 0x00177859
		public override bool UsesBones
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04003285 RID: 12933
		private float mRadius;

		// Token: 0x04003286 RID: 12934
		private Grip.GripType mType;

		// Token: 0x04003287 RID: 12935
		private int mBreakFreeTolerance;

		// Token: 0x04003288 RID: 12936
		private bool mUseGripAttach;

		// Token: 0x04003289 RID: 12937
		private BindJoint mGripAttach;

		// Token: 0x0400328A RID: 12938
		private BindJoint mTargetAttach;

		// Token: 0x0400328B RID: 12939
		private bool mFinishOnGrip;

		// Token: 0x0200060E RID: 1550
		public enum GripType
		{
			// Token: 0x0400328D RID: 12941
			Pickup,
			// Token: 0x0400328E RID: 12942
			Ride,
			// Token: 0x0400328F RID: 12943
			Hold
		}
	}
}
