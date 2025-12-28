using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A0 RID: 1184
	public struct CameraShakeEvent
	{
		// Token: 0x060023DD RID: 9181 RVA: 0x00101DE3 File Offset: 0x000FFFE3
		public CameraShakeEvent(float iTTL, float iMagnitude)
		{
			this.mTTL = iTTL;
			this.mMagnitude = iMagnitude;
			this.mFromPosition = false;
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x00101DFA File Offset: 0x000FFFFA
		public CameraShakeEvent(float iTTL, float iMagnitude, bool iFromPosition)
		{
			this.mTTL = iTTL;
			this.mMagnitude = iMagnitude;
			this.mFromPosition = iFromPosition;
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x00101E11 File Offset: 0x00100011
		public CameraShakeEvent(ContentReader iInput)
		{
			this.mTTL = iInput.ReadSingle();
			this.mMagnitude = iInput.ReadSingle();
			this.mFromPosition = iInput.ReadBoolean();
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x00101E38 File Offset: 0x00100038
		public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			if (this.mFromPosition)
			{
				Vector3 iPosition2 = iItem.Position;
				if (iPosition != null)
				{
					iPosition2 = iPosition.Value;
				}
				iItem.PlayState.Camera.CameraShake(iPosition2, this.mMagnitude, this.mTTL);
				return;
			}
			iItem.PlayState.Camera.CameraShake(this.mMagnitude, this.mTTL);
		}

		// Token: 0x040026F1 RID: 9969
		private float mTTL;

		// Token: 0x040026F2 RID: 9970
		private float mMagnitude;

		// Token: 0x040026F3 RID: 9971
		private bool mFromPosition;
	}
}
