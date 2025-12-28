using System;
using Microsoft.Xna.Framework.Content;
using XNAnimation;

namespace Magicka.GameLogic.Entities.AnimationActions
{
	// Token: 0x02000403 RID: 1027
	internal class DamageGrip : AnimationAction
	{
		// Token: 0x06001FA9 RID: 8105 RVA: 0x000DE2B0 File Offset: 0x000DC4B0
		public unsafe DamageGrip(ContentReader iInput, SkinnedModelBoneCollection iSkeleton) : base(iInput, iSkeleton)
		{
			this.mDamageToOwner = iInput.ReadBoolean();
			fixed (Damage* ptr = &this.mDamages.A)
			{
				int num = iInput.ReadInt32();
				if (num > 5)
				{
					throw new Exception("To many damages! Maximum allowed is " + 5 + ".");
				}
				for (int i = 0; i < num; i++)
				{
					ptr[i].AttackProperty = (AttackProperties)iInput.ReadInt32();
					ptr[i].Element = (Elements)iInput.ReadInt32();
					ptr[i].Amount = iInput.ReadSingle();
					ptr[i].Magnitude = iInput.ReadSingle();
				}
			}
		}

		// Token: 0x06001FAA RID: 8106 RVA: 0x000DE370 File Offset: 0x000DC570
		protected override void InternalExecute(Character iOwner, bool iFirstExecution)
		{
			if (this.mDamageToOwner)
			{
				iOwner.Damage(this.mDamages, iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
				return;
			}
			if (iOwner.IsGripping)
			{
				iOwner.GrippedCharacter.Damage(this.mDamages, iOwner, iOwner.PlayState.PlayTime, iOwner.Position);
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06001FAB RID: 8107 RVA: 0x000DE3D1 File Offset: 0x000DC5D1
		public override bool UsesBones
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040021F8 RID: 8696
		private bool mDamageToOwner;

		// Token: 0x040021F9 RID: 8697
		private DamageCollection5 mDamages;
	}
}
