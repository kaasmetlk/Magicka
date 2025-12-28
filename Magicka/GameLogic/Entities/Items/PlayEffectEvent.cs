using System;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004A1 RID: 1185
	public struct PlayEffectEvent
	{
		// Token: 0x060023E1 RID: 9185 RVA: 0x00101E9D File Offset: 0x0010009D
		public PlayEffectEvent(int iHash)
		{
			this.mFollow = false;
			this.mEffectHash = iHash;
			this.mWorldAlign = false;
		}

		// Token: 0x060023E2 RID: 9186 RVA: 0x00101EB4 File Offset: 0x001000B4
		public PlayEffectEvent(string iName)
		{
			this.mEffectHash = iName.GetHashCodeCustom();
			this.mFollow = false;
			this.mWorldAlign = false;
		}

		// Token: 0x060023E3 RID: 9187 RVA: 0x00101ED0 File Offset: 0x001000D0
		public PlayEffectEvent(int iHash, bool iFollow)
		{
			this.mEffectHash = iHash;
			this.mFollow = iFollow;
			this.mWorldAlign = false;
		}

		// Token: 0x060023E4 RID: 9188 RVA: 0x00101EE7 File Offset: 0x001000E7
		public PlayEffectEvent(int iHash, bool iFollow, bool iWorldAlign)
		{
			this.mEffectHash = iHash;
			this.mFollow = iFollow;
			this.mWorldAlign = iWorldAlign;
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x00101EFE File Offset: 0x001000FE
		public PlayEffectEvent(string iName, bool iFollow)
		{
			this.mEffectHash = iName.GetHashCodeCustom();
			this.mFollow = iFollow;
			this.mWorldAlign = false;
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x00101F1A File Offset: 0x0010011A
		public PlayEffectEvent(ContentReader iInput)
		{
			this.mFollow = iInput.ReadBoolean();
			this.mWorldAlign = iInput.ReadBoolean();
			this.mEffectHash = iInput.ReadString().ToLowerInvariant().GetHashCodeCustom();
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x00101F4C File Offset: 0x0010014C
		public void Execute(Entity iItem, Entity iTarget, ref Vector3? iPosition)
		{
			Vector3 vector = iItem.Position;
			if (iPosition != null)
			{
				vector = iPosition.Value;
			}
			Vector3 vector2 = new Vector3(0f, 1f, 0f);
			if (!this.mWorldAlign)
			{
				vector2 = iItem.GetOrientation().Forward;
			}
			VisualEffectReference iEffect;
			EffectManager.Instance.StartEffect(this.mEffectHash, ref vector, ref vector2, out iEffect);
			if (this.mFollow)
			{
				if (iItem is MissileEntity)
				{
					(iItem as MissileEntity).AddEffectReference(this.mEffectHash, iEffect);
					return;
				}
				if (iItem is Item)
				{
					(iItem as Item).AddEffectReference(this.mEffectHash, iEffect);
				}
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x060023E8 RID: 9192 RVA: 0x00101FF5 File Offset: 0x001001F5
		public int EffectHash
		{
			get
			{
				return this.mEffectHash;
			}
		}

		// Token: 0x040026F4 RID: 9972
		private int mEffectHash;

		// Token: 0x040026F5 RID: 9973
		private bool mFollow;

		// Token: 0x040026F6 RID: 9974
		private bool mWorldAlign;
	}
}
