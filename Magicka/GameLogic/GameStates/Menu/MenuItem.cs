using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead.Effects;

namespace Magicka.GameLogic.GameStates.Menu
{
	// Token: 0x0200029A RID: 666
	public abstract class MenuItem
	{
		// Token: 0x060013B9 RID: 5049 RVA: 0x00078AB4 File Offset: 0x00076CB4
		public MenuItem()
		{
			this.mEnabled = true;
			this.mColor = MenuItem.COLOR;
			this.mColorSelected = MenuItem.COLOR_SELECTED;
			this.mColorDisabled = MenuItem.COLOR_DISABLED;
			this.mTransform = Matrix.Identity;
			this.mAlpha = 1f;
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x060013BA RID: 5050 RVA: 0x00078B10 File Offset: 0x00076D10
		// (set) Token: 0x060013BB RID: 5051 RVA: 0x00078B18 File Offset: 0x00076D18
		public Vector2 Position
		{
			get
			{
				return this.mPosition;
			}
			set
			{
				this.mPosition = value;
				this.mTransform.M41 = this.mPosition.X;
				this.mTransform.M42 = this.mPosition.Y;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x060013BC RID: 5052 RVA: 0x00078B53 File Offset: 0x00076D53
		// (set) Token: 0x060013BD RID: 5053 RVA: 0x00078B5B File Offset: 0x00076D5B
		public float Scale
		{
			get
			{
				return this.mScale;
			}
			set
			{
				this.mScale = value;
				this.UpdateBoundingBox();
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x00078B6A File Offset: 0x00076D6A
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x00078B72 File Offset: 0x00076D72
		public float Alpha
		{
			get
			{
				return this.mAlpha;
			}
			set
			{
				this.mAlpha = value;
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00078B7B File Offset: 0x00076D7B
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x00078B83 File Offset: 0x00076D83
		public virtual bool Enabled
		{
			get
			{
				return this.mEnabled;
			}
			set
			{
				this.mEnabled = value;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x00078B8C File Offset: 0x00076D8C
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x00078B94 File Offset: 0x00076D94
		public virtual bool Selected
		{
			get
			{
				return this.mSelected;
			}
			set
			{
				this.mSelected = value;
			}
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00078BA0 File Offset: 0x00076DA0
		public bool InsideBounds(float iX, float iY)
		{
			return iX >= this.mTopLeft.X & iY >= this.mTopLeft.Y & iX <= this.mBottomRight.X & iY <= this.mBottomRight.Y;
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x00078BF4 File Offset: 0x00076DF4
		public bool InsideBounds(MouseState iState)
		{
			return (float)iState.X >= this.mTopLeft.X & (float)iState.Y >= this.mTopLeft.Y & (float)iState.X <= this.mBottomRight.X & (float)iState.Y <= this.mBottomRight.Y;
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x00078C64 File Offset: 0x00076E64
		public bool InsideBounds(ref Vector2 iPoint)
		{
			return iPoint.X >= this.mTopLeft.X & iPoint.Y >= this.mTopLeft.Y & iPoint.X <= this.mBottomRight.X & iPoint.Y <= this.mBottomRight.Y;
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x060013C7 RID: 5063 RVA: 0x00078CCC File Offset: 0x00076ECC
		public Vector2 TopLeft
		{
			get
			{
				return this.mTopLeft;
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x060013C8 RID: 5064 RVA: 0x00078CD4 File Offset: 0x00076ED4
		public Vector2 BottomRight
		{
			get
			{
				return this.mBottomRight;
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x060013C9 RID: 5065 RVA: 0x00078CDC File Offset: 0x00076EDC
		// (set) Token: 0x060013CA RID: 5066 RVA: 0x00078CE4 File Offset: 0x00076EE4
		public Vector4 Color
		{
			get
			{
				return this.mColor;
			}
			set
			{
				this.mColor = value;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x060013CB RID: 5067 RVA: 0x00078CED File Offset: 0x00076EED
		// (set) Token: 0x060013CC RID: 5068 RVA: 0x00078CF5 File Offset: 0x00076EF5
		public Vector4 ColorSelected
		{
			get
			{
				return this.mColorSelected;
			}
			set
			{
				this.mColorSelected = value;
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x060013CD RID: 5069 RVA: 0x00078CFE File Offset: 0x00076EFE
		// (set) Token: 0x060013CE RID: 5070 RVA: 0x00078D06 File Offset: 0x00076F06
		public Vector4 ColorDisabled
		{
			get
			{
				return this.mColorDisabled;
			}
			set
			{
				this.mColorDisabled = value;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x060013CF RID: 5071 RVA: 0x00078D0F File Offset: 0x00076F0F
		// (set) Token: 0x060013D0 RID: 5072 RVA: 0x00078D1C File Offset: 0x00076F1C
		public float ColorAlphas
		{
			get
			{
				return this.mColor.W;
			}
			set
			{
				this.mColor.W = value;
				this.mColorSelected.W = value;
				this.mColorDisabled.W = value;
			}
		}

		// Token: 0x060013D1 RID: 5073
		protected abstract void UpdateBoundingBox();

		// Token: 0x060013D2 RID: 5074
		public abstract void Draw(GUIBasicEffect iEffect);

		// Token: 0x060013D3 RID: 5075
		public abstract void Draw(GUIBasicEffect iEffect, float iScale);

		// Token: 0x060013D4 RID: 5076
		public abstract void LanguageChanged();

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x060013D5 RID: 5077 RVA: 0x00078D42 File Offset: 0x00076F42
		// (set) Token: 0x060013D6 RID: 5078 RVA: 0x00078D4A File Offset: 0x00076F4A
		public virtual object Tag { get; set; }

		// Token: 0x04001540 RID: 5440
		public static Vector4 COLOR_DISABLED = new Vector4(0f, 0f, 0f, 0.4f);

		// Token: 0x04001541 RID: 5441
		public static Vector4 COLOR = new Vector4(0.1f, 0.05f, 0f, 0.7f);

		// Token: 0x04001542 RID: 5442
		public static Vector4 COLOR_SELECTED = new Vector4(1.5f, 1.5f, 1.5f, 1f);

		// Token: 0x04001543 RID: 5443
		protected bool mEnabled;

		// Token: 0x04001544 RID: 5444
		protected bool mSelected;

		// Token: 0x04001545 RID: 5445
		protected Vector2 mPosition;

		// Token: 0x04001546 RID: 5446
		protected Matrix mTransform;

		// Token: 0x04001547 RID: 5447
		protected Vector4 mColor;

		// Token: 0x04001548 RID: 5448
		protected Vector4 mColorSelected;

		// Token: 0x04001549 RID: 5449
		protected Vector4 mColorDisabled;

		// Token: 0x0400154A RID: 5450
		protected float mAlpha;

		// Token: 0x0400154B RID: 5451
		protected Vector2 mTopLeft;

		// Token: 0x0400154C RID: 5452
		protected Vector2 mBottomRight;

		// Token: 0x0400154D RID: 5453
		protected float mScale = 1f;
	}
}
