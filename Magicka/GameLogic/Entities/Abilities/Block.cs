using System;
using Magicka.AI;
using Magicka.AI.Arithmetics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Abilities
{
	// Token: 0x0200031B RID: 795
	public class Block : Ability
	{
		// Token: 0x0600186B RID: 6251 RVA: 0x000A1ABD File Offset: 0x0009FCBD
		public Block(ContentReader iInput, AnimationClipAction[][] iAnimations) : base(iInput, iAnimations)
		{
			this.mArc = iInput.ReadSingle();
			this.mShield = iInput.ReadInt32();
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x000A1ADF File Offset: 0x0009FCDF
		public Block(Block iCloneSource) : base(iCloneSource)
		{
			this.mArc = iCloneSource.mArc;
			this.mShield = iCloneSource.mShield;
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x000A1B00 File Offset: 0x0009FD00
		protected override float Desirability(ref ExpressionArguments iArgs)
		{
			throw new NotImplementedException("Block must define a desirability expression!");
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000A1B0C File Offset: 0x0009FD0C
		public override void Update(Agent iAgent, float iDeltaTime)
		{
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000A1B0E File Offset: 0x0009FD0E
		public override bool InternalExecute(Agent iAgent)
		{
			iAgent.NPC.Block();
			return true;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000A1B1C File Offset: 0x0009FD1C
		public override float GetMaxRange(Agent iAgent)
		{
			return float.MaxValue;
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000A1B23 File Offset: 0x0009FD23
		public override float GetMinRange(Agent iAgent)
		{
			return 0f;
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x000A1B2A File Offset: 0x0009FD2A
		public override float GetArc(Agent iAgent)
		{
			return this.mArc;
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x000A1B32 File Offset: 0x0009FD32
		public override int[] GetWeapons()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x000A1B3C File Offset: 0x0009FD3C
		public override Vector3 GetDesiredDirection(Agent iAgent)
		{
			Vector3 position = iAgent.Owner.Position;
			Vector3 position2 = iAgent.CurrentTarget.Position;
			Vector3 result;
			Vector3.Subtract(ref position2, ref position, out result);
			float num = result.Length();
			if (num > 1E-06f)
			{
				Vector3.Divide(ref result, num, out result);
			}
			else
			{
				result.Z = 1f;
			}
			return result;
		}

		// Token: 0x04001A28 RID: 6696
		private float mArc;

		// Token: 0x04001A29 RID: 6697
		private int mShield;
	}
}
