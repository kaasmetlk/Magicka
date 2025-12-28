using System;
using PolygonHead;

namespace Magicka.Achievements
{
	// Token: 0x020002B7 RID: 695
	internal abstract class AchievementWindow
	{
		// Token: 0x060014F5 RID: 5365 RVA: 0x00083336 File Offset: 0x00081536
		public virtual void Show()
		{
			this.mVisible = true;
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0008333F File Offset: 0x0008153F
		public virtual void Hide()
		{
			this.mVisible = false;
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x00083348 File Offset: 0x00081548
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mVisible)
			{
				this.mAlpha = Math.Min(this.mAlpha + iDeltaTime * 8f, 1f);
				return;
			}
			this.mAlpha = Math.Max(this.mAlpha - iDeltaTime * 8f, 0f);
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060014F8 RID: 5368 RVA: 0x0008339A File Offset: 0x0008159A
		public bool Visible
		{
			get
			{
				return this.mVisible | this.mAlpha > float.Epsilon;
			}
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x000833B0 File Offset: 0x000815B0
		public virtual void OnLanguageChanged()
		{
		}

		// Token: 0x04001658 RID: 5720
		protected const string UI_PATH = "content/connectui/";

		// Token: 0x04001659 RID: 5721
		protected float mAlpha;

		// Token: 0x0400165A RID: 5722
		protected bool mVisible;
	}
}
