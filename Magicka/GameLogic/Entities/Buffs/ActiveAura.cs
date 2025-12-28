using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JigLibX.Geometry;
using Magicka.Graphics;
using Magicka.Levels;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.Entities.Buffs
{
	// Token: 0x020002FC RID: 764
	public struct ActiveAura
	{
		// Token: 0x06001797 RID: 6039 RVA: 0x0009B0F0 File Offset: 0x000992F0
		public unsafe void Execute(Character iOwner, float iDeltaTime)
		{
			this.Aura.Execute(iOwner, iDeltaTime);
			if (this.Aura.VisualCategory != VisualCategory.None)
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime, 1f);
				this.mRotation = MathHelper.WrapAngle(this.mRotation + iDeltaTime * 0.2f);
				Vector4 vector = default(Vector4);
				vector.X = this.Aura.Color.X;
				vector.Y = this.Aura.Color.Y;
				vector.Z = this.Aura.Color.Z;
				Vector2 vector2 = default(Vector2);
				vector2.X = (vector2.Y = 1.5f);
				Vector3 vector3 = default(Vector3);
				vector3.Y = 1f;
				Vector3 position = iOwner.Position;
				Segment iSeg = default(Segment);
				iSeg.Delta.Y = -8f;
				GameScene currentScene = iOwner.PlayState.Level.CurrentScene;
				Vector3 value = new Vector3(0f, 0f, 1f);
				Quaternion quaternion;
				Quaternion.CreateFromYawPitchRoll(this.mRotation, 0f, 0f, out quaternion);
				Vector3.Transform(ref value, ref quaternion, out value);
				DecalManager instance = DecalManager.Instance;
				float yaw = 1.0471976f;
				Matrix matrix = default(Matrix);
				matrix.M11 = -1.5f;
				matrix.M23 = 1.5f;
				matrix.M32 = 1f;
				matrix.M44 = 1f;
				Matrix.Transform(ref matrix, ref quaternion, out matrix);
				Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f, out quaternion);
				fixed (ulong* ptr = &this.Decals.FixedElementField)
				{
					DecalManager.DecalReference* ptr2 = (DecalManager.DecalReference*)ptr;
					float num = this.mAlpha;
					if (iOwner.PlayState.IsInCutscene)
					{
						this.mAlpha = 0f;
					}
					for (int i = 0; i < 6; i++)
					{
						Vector3.Transform(ref value, ref quaternion, out value);
						Vector3.Multiply(ref value, this.Aura.Radius, out iSeg.Origin);
						Vector3.Add(ref iSeg.Origin, ref position, out iSeg.Origin);
						iSeg.Origin.Y = iSeg.Origin.Y + 4f;
						Matrix.Transform(ref matrix, ref quaternion, out matrix);
						float num2;
						Vector3 vector4;
						Vector3 vector5;
						if (!currentScene.SegmentIntersect(out num2, out vector4, out vector5, iSeg))
						{
							iSeg.GetPoint(0.5f, out vector4);
						}
						matrix.M41 = vector4.X;
						matrix.M42 = vector4.Y;
						matrix.M43 = vector4.Z;
						if (!instance.SetDecal(ref ptr2[i], 1f, ref matrix, this.mAlpha * 0.666f))
						{
							instance.AddAlphaBlendedDecal(Decal.AuraOffensive + (int)this.Aura.VisualCategory - 1, null, ref vector2, ref vector4, new Vector3?(value), ref vector3, 1f, ref vector, out ptr2[i]);
						}
					}
					this.mAlpha = num;
				}
			}
		}

		// Token: 0x0400194B RID: 6475
		private const int NR_OF_DECALS = 6;

		// Token: 0x0400194C RID: 6476
		public AuraStorage Aura;

		// Token: 0x0400194D RID: 6477
		public float StartTTL;

		// Token: 0x0400194E RID: 6478
		public bool SelfCasted;

		// Token: 0x0400194F RID: 6479
		private float mRotation;

		// Token: 0x04001950 RID: 6480
		private float mAlpha;

		// Token: 0x04001951 RID: 6481
		[FixedBuffer(typeof(ulong), 6)]
		public ActiveAura.<Decals>e__FixedBuffer7 Decals;

		// Token: 0x04001952 RID: 6482
		public VisualEffectReference mEffect;

		// Token: 0x020002FD RID: 765
		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 48)]
		public struct <Decals>e__FixedBuffer7
		{
			// Token: 0x04001953 RID: 6483
			public ulong FixedElementField;
		}
	}
}
