using System;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000142 RID: 322
	internal class SetDialogHint : Action
	{
		// Token: 0x0600090E RID: 2318 RVA: 0x00039620 File Offset: 0x00037820
		public SetDialogHint(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0003962C File Offset: 0x0003782C
		public override void Initialize()
		{
			base.Initialize();
			LanguageManager instance = LanguageManager.Instance;
			string iText = instance.GetString(this.mHintHash);
			iText = instance.ParseReferences(iText);
			BitmapFont font = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
			this.mHint = font.Wrap(iText, 300, true);
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0003967C File Offset: 0x0003787C
		protected override void Execute()
		{
			float? iScale = null;
			if (this.mScale > 0f)
			{
				iScale = new float?(this.mScale);
			}
			Vector2? iSize = null;
			if (this.mSize.X > 0f & this.mSize.Y > 0f)
			{
				iSize = new Vector2?(this.mSize);
			}
			if (this.mMagick != MagickType.None)
			{
				TutorialManager.Instance.SetMagickHint(this.mMagick, this.mHint, this.mTriggerID, iScale, iSize);
				return;
			}
			TutorialManager.Instance.SetElementHint(this.mElement, this.mHint, this.mTriggerID, iScale, iSize);
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0003972C File Offset: 0x0003792C
		public override void QuickExecute()
		{
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000912 RID: 2322 RVA: 0x0003972E File Offset: 0x0003792E
		// (set) Token: 0x06000913 RID: 2323 RVA: 0x00039736 File Offset: 0x00037936
		public new string Trigger
		{
			get
			{
				return this.mTrigger;
			}
			set
			{
				this.mTrigger = value;
				this.mTriggerID = this.mTrigger.GetHashCodeCustom();
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000914 RID: 2324 RVA: 0x00039750 File Offset: 0x00037950
		// (set) Token: 0x06000915 RID: 2325 RVA: 0x00039758 File Offset: 0x00037958
		public float Scale
		{
			get
			{
				return this.mScale;
			}
			set
			{
				this.mScale = value;
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000916 RID: 2326 RVA: 0x00039761 File Offset: 0x00037961
		// (set) Token: 0x06000917 RID: 2327 RVA: 0x00039769 File Offset: 0x00037969
		public Vector2 Size
		{
			get
			{
				return this.mSize;
			}
			set
			{
				this.mSize = value;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000918 RID: 2328 RVA: 0x00039772 File Offset: 0x00037972
		// (set) Token: 0x06000919 RID: 2329 RVA: 0x0003977A File Offset: 0x0003797A
		public Elements Element
		{
			get
			{
				return this.mElement;
			}
			set
			{
				this.mElement = value;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x0600091A RID: 2330 RVA: 0x00039783 File Offset: 0x00037983
		// (set) Token: 0x0600091B RID: 2331 RVA: 0x0003978B File Offset: 0x0003798B
		public MagickType Magick
		{
			get
			{
				return this.mMagick;
			}
			set
			{
				this.mMagick = value;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x0600091C RID: 2332 RVA: 0x00039794 File Offset: 0x00037994
		// (set) Token: 0x0600091D RID: 2333 RVA: 0x0003979C File Offset: 0x0003799C
		public int Custom
		{
			get
			{
				return (int)this.mMagick;
			}
			set
			{
				this.mMagick = (MagickType)value;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x0600091E RID: 2334 RVA: 0x000397A5 File Offset: 0x000379A5
		// (set) Token: 0x0600091F RID: 2335 RVA: 0x000397AD File Offset: 0x000379AD
		public string ID
		{
			get
			{
				return this.mHint;
			}
			set
			{
				this.mHint = value;
				this.mHintHash = value.ToLowerInvariant().GetHashCodeCustom();
			}
		}

		// Token: 0x04000870 RID: 2160
		private string mHint;

		// Token: 0x04000871 RID: 2161
		private int mHintHash;

		// Token: 0x04000872 RID: 2162
		private MagickType mMagick;

		// Token: 0x04000873 RID: 2163
		private Elements mElement;

		// Token: 0x04000874 RID: 2164
		private new string mTrigger;

		// Token: 0x04000875 RID: 2165
		private int mTriggerID;

		// Token: 0x04000876 RID: 2166
		private float mScale;

		// Token: 0x04000877 RID: 2167
		private Vector2 mSize;
	}
}
