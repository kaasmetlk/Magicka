using System;
using System.Xml;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Spells;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000483 RID: 1155
	public class Magick : Action
	{
		// Token: 0x060022F5 RID: 8949 RVA: 0x000FB8DE File Offset: 0x000F9ADE
		public Magick(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000FB8E8 File Offset: 0x000F9AE8
		protected override void Execute()
		{
			if (this.mIDHash == 0)
			{
				Magick magick = default(Magick);
				magick.MagickType = this.mMagickType;
				magick.Effect.Execute(this.mPosition, base.GameScene.PlayState);
				return;
			}
			Character iOwner = Entity.GetByID(this.mIDHash) as Character;
			Magick magick2 = default(Magick);
			magick2.MagickType = this.mMagickType;
			magick2.Effect.Execute(iOwner, base.GameScene.PlayState);
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x000FB970 File Offset: 0x000F9B70
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x060022F8 RID: 8952 RVA: 0x000FB978 File Offset: 0x000F9B78
		// (set) Token: 0x060022F9 RID: 8953 RVA: 0x000FB980 File Offset: 0x000F9B80
		public MagickType MagickType
		{
			get
			{
				return this.mMagickType;
			}
			set
			{
				this.mMagickType = value;
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x060022FA RID: 8954 RVA: 0x000FB989 File Offset: 0x000F9B89
		// (set) Token: 0x060022FB RID: 8955 RVA: 0x000FB991 File Offset: 0x000F9B91
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				if (!string.IsNullOrEmpty(this.mID))
				{
					this.mIDHash = this.mID.GetHashCodeCustom();
					return;
				}
				this.mIDHash = 0;
			}
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x060022FC RID: 8956 RVA: 0x000FB9C0 File Offset: 0x000F9BC0
		// (set) Token: 0x060022FD RID: 8957 RVA: 0x000FB9C8 File Offset: 0x000F9BC8
		public Vector3 Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				this.mPosition = value;
			}
		}

		// Token: 0x0400261A RID: 9754
		protected string mID;

		// Token: 0x0400261B RID: 9755
		protected int mIDHash;

		// Token: 0x0400261C RID: 9756
		protected MagickType mMagickType;

		// Token: 0x0400261D RID: 9757
		protected Vector3 mPosition;
	}
}
