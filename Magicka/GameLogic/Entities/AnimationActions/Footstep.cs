using System;
using Magicka.Audio;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x020001CC RID: 460
	public class Footstep : AnimationAction
	{
		// Token: 0x06000F99 RID: 3993 RVA: 0x00060CCB File Offset: 0x0005EECB
		public Footstep(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x00060CD8 File Offset: 0x0005EED8
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (iFirstExecution && iOwner.CharacterBody.IsTouchingGround && !iOwner.IsLevitating)
			{
				if (Footstep.sCueNames[(int)iOwner.CharacterBody.GroundMaterial] != 0)
				{
					Footstep.FootstepVariables iVariables;
					iVariables.Weight = iOwner.CharacterBody.Mass;
					iVariables.Depth = iOwner.WaterDepth;
					AudioManager.Instance.PlayCue<Footstep.FootstepVariables>(Banks.Footsteps, Footstep.sCueNames[(int)iOwner.CharacterBody.GroundMaterial], iVariables, iOwner.AudioEmitter);
				}
				if (Footstep.sEffectNames[(int)iOwner.CharacterBody.GroundMaterial] != 0)
				{
					Vector3 position = iOwner.Position;
					position.Y += iOwner.HeightOffset + iOwner.WaterDepth;
					Vector3 direction = iOwner.Direction;
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(Footstep.sEffectNames[(int)iOwner.CharacterBody.GroundMaterial], ref position, ref direction, out visualEffectReference);
				}
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000F9B RID: 3995 RVA: 0x00060DBB File Offset: 0x0005EFBB
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00060DC0 File Offset: 0x0005EFC0
		// Note: this type is marked as 'beforefieldinit'.
		static Footstep()
		{
			int[] array = new int[10];
			array[0] = "footstep_generic".GetHashCodeCustom();
			array[1] = "footstep_gravel".GetHashCodeCustom();
			array[2] = "footstep_grass".GetHashCodeCustom();
			array[3] = "footstep_wood".GetHashCodeCustom();
			array[4] = "footstep_snow".GetHashCodeCustom();
			array[5] = "footstep_stone".GetHashCodeCustom();
			array[6] = "footstep_mud".GetHashCodeCustom();
			array[7] = "footstep_generic".GetHashCodeCustom();
			array[8] = "footstep_water".GetHashCodeCustom();
			Footstep.sCueNames = array;
			Footstep.sEffectNames = new int[]
			{
				0,
				"footstep_gravel".GetHashCodeCustom(),
				"footstep_grass".GetHashCodeCustom(),
				0,
				"footstep_snow".GetHashCodeCustom(),
				0,
				"footstep_mud".GetHashCodeCustom(),
				0,
				"footstep_water".GetHashCodeCustom(),
				"footstep_lava".GetHashCodeCustom()
			};
		}

		// Token: 0x04000E18 RID: 3608
		private static readonly int[] sCueNames;

		// Token: 0x04000E19 RID: 3609
		private static readonly int[] sEffectNames;

		// Token: 0x020001CD RID: 461
		internal struct FootstepVariables : IAudioVariables
		{
			// Token: 0x06000F9D RID: 3997 RVA: 0x00060EAD File Offset: 0x0005F0AD
			public FootstepVariables(float iWeight, float iDepth)
			{
				this.Weight = iWeight;
				this.Depth = iDepth;
			}

			// Token: 0x06000F9E RID: 3998 RVA: 0x00060EBD File Offset: 0x0005F0BD
			public void AssignToCue(Cue iCue)
			{
				iCue.SetVariable(Footstep.FootstepVariables.VARIABLE_WEIGHT, this.Weight);
				iCue.SetVariable(Footstep.FootstepVariables.VARIABLE_DEPTH, this.Depth);
			}

			// Token: 0x04000E1A RID: 3610
			public static readonly string VARIABLE_WEIGHT = "Weight";

			// Token: 0x04000E1B RID: 3611
			public static readonly string VARIABLE_DEPTH = "WaterDepth";

			// Token: 0x04000E1C RID: 3612
			public float Weight;

			// Token: 0x04000E1D RID: 3613
			public float Depth;
		}
	}
}
