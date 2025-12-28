using System;
using Magicka.GameLogic.Controls;
using Magicka.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI.Popup
{
	// Token: 0x02000150 RID: 336
	public abstract class MenuBasePopup
	{
		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060009DA RID: 2522 RVA: 0x0003BF97 File Offset: 0x0003A197
		// (set) Token: 0x060009DB RID: 2523 RVA: 0x0003BF9F File Offset: 0x0003A19F
		public bool CanDismiss
		{
			get
			{
				return this.mCanDismiss;
			}
			set
			{
				this.mCanDismiss = value;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060009DC RID: 2524 RVA: 0x0003BFA8 File Offset: 0x0003A1A8
		// (set) Token: 0x060009DD RID: 2525 RVA: 0x0003BFB0 File Offset: 0x0003A1B0
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

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060009DE RID: 2526 RVA: 0x0003BFB9 File Offset: 0x0003A1B9
		// (set) Token: 0x060009DF RID: 2527 RVA: 0x0003BFC1 File Offset: 0x0003A1C1
		public bool Enabled
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

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060009E0 RID: 2528 RVA: 0x0003BFCA File Offset: 0x0003A1CA
		// (set) Token: 0x060009E1 RID: 2529 RVA: 0x0003BFD7 File Offset: 0x0003A1D7
		public float Alpha
		{
			get
			{
				return this.mColour.W;
			}
			set
			{
				this.mColour.W = value;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060009E2 RID: 2530 RVA: 0x0003BFE5 File Offset: 0x0003A1E5
		// (set) Token: 0x060009E3 RID: 2531 RVA: 0x0003BFED File Offset: 0x0003A1ED
		public PopupAlign Alignment
		{
			get
			{
				return this.mAlignment;
			}
			set
			{
				this.SetAlignment(value);
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060009E4 RID: 2532 RVA: 0x0003BFF6 File Offset: 0x0003A1F6
		// (set) Token: 0x060009E5 RID: 2533 RVA: 0x0003BFFE File Offset: 0x0003A1FE
		public Action OnPositiveClick
		{
			get
			{
				return this.mOnPositiveClickDelegate;
			}
			set
			{
				this.mOnPositiveClickDelegate = value;
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060009E6 RID: 2534 RVA: 0x0003C007 File Offset: 0x0003A207
		// (set) Token: 0x060009E7 RID: 2535 RVA: 0x0003C00F File Offset: 0x0003A20F
		public Action OnNegativeClick
		{
			get
			{
				return this.mOnNegativeClickDelegate;
			}
			set
			{
				this.mOnNegativeClickDelegate = value;
			}
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0003C018 File Offset: 0x0003A218
		protected void SetAlignment(PopupAlign iAlignment)
		{
			this.mAlignment = iAlignment;
			Vector2 positionFromAlignment = this.GetPositionFromAlignment();
			this.mTransform.M41 = positionFromAlignment.X;
			this.mTransform.M42 = positionFromAlignment.Y;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0003C057 File Offset: 0x0003A257
		public void SetPosition(Vector2 iPosition)
		{
			this.mRelativePosition = iPosition / PopupSystem.REFERENCE_SIZE;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0003C06A File Offset: 0x0003A26A
		public void SetRelativePosition(Vector2 iPosition)
		{
			this.mRelativePosition = iPosition;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0003C074 File Offset: 0x0003A274
		protected Vector2 GetPositionFromAlignment()
		{
			Vector2 result = this.mPosition;
			Vector2 vector = this.mSize * this.mScale;
			if ((this.mAlignment & PopupAlign.Center) != PopupAlign.None)
			{
				result.Y -= vector.Y * 0.5f;
			}
			else if ((this.mAlignment & PopupAlign.Bottom) != PopupAlign.None)
			{
				result.Y -= vector.Y;
			}
			if ((this.mAlignment & PopupAlign.Center) != PopupAlign.None)
			{
				result.X -= vector.X * 0.5f;
			}
			else if ((this.mAlignment & PopupAlign.Right) != PopupAlign.None)
			{
				result.X -= vector.X;
			}
			return result;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0003C12A File Offset: 0x0003A32A
		public bool InsideBounds(float iX, float iY)
		{
			return iX >= this.mTopLeft.X && iY >= this.mTopLeft.Y && iX <= this.mBottomRight.X && iY <= this.mBottomRight.Y;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0003C169 File Offset: 0x0003A369
		public bool InsideBounds(MouseState iState)
		{
			return this.InsideBounds((float)iState.X, (float)iState.Y);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0003C181 File Offset: 0x0003A381
		public bool InsideBounds(ref Vector2 iPoint)
		{
			return this.InsideBounds(iPoint.X, iPoint.Y);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0003C195 File Offset: 0x0003A395
		public virtual void LanguageChanged()
		{
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0003C197 File Offset: 0x0003A397
		public virtual void Dismiss()
		{
			Singleton<PopupSystem>.Instance.DismissCurrentPopup();
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0003C1A3 File Offset: 0x0003A3A3
		public virtual void OnShow()
		{
			this.mPosition = this.mRelativePosition * PopupSystem.Resolution;
			this.mScale = PopupSystem.Resolution.Y / PopupSystem.REFERENCE_SIZE.Y;
			this.UpdateBoundingBox();
			this.mSelectedItem = -1;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x0003C1E3 File Offset: 0x0003A3E3
		public virtual void OnHide()
		{
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x0003C1E8 File Offset: 0x0003A3E8
		protected virtual void UpdateBoundingBox()
		{
			this.mTopLeft.X = this.mPosition.X + (float)this.mHitBox.X * this.mScale;
			this.mTopLeft.Y = this.mPosition.Y + (float)this.mHitBox.Y * this.mScale;
			this.mBottomRight.X = this.mPosition.X + (float)(this.mHitBox.X + this.mHitBox.Width) * this.mScale;
			this.mBottomRight.Y = this.mPosition.Y + (float)(this.mHitBox.Y + this.mHitBox.Height) * this.mScale;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x0003C2B8 File Offset: 0x0003A4B8
		public virtual void ResetHitArea()
		{
			this.mHitBox.X = 0;
			this.mHitBox.Y = 0;
			this.mHitBox.Width = (int)this.mSize.X;
			this.mHitBox.Height = (int)this.mSize.Y;
			this.UpdateBoundingBox();
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x0003C311 File Offset: 0x0003A511
		public virtual void SetHitArea(int iX, int iY, int iWidth, int iHeight)
		{
			this.mHitBox.X = iX;
			this.mHitBox.Y = iY;
			this.mHitBox.Width = iWidth;
			this.mHitBox.Height = iHeight;
			this.UpdateBoundingBox();
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x0003C34A File Offset: 0x0003A54A
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x0003C34C File Offset: 0x0003A54C
		public virtual void Draw(GUIBasicEffect iEffect)
		{
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x0003C34E File Offset: 0x0003A54E
		internal virtual void ControllerMouseAction(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x0003C350 File Offset: 0x0003A550
		internal virtual void ControllerMouseMove(Controller iSender, Point iScreenSize, MouseState iState, MouseState iOldState)
		{
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x0003C352 File Offset: 0x0003A552
		internal virtual void ControllerMovement(Controller iSender, ControllerDirection iDirection)
		{
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x0003C354 File Offset: 0x0003A554
		internal virtual void ControllerUp(Controller iSender)
		{
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x0003C356 File Offset: 0x0003A556
		internal virtual void ControllerDown(Controller iSender)
		{
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x0003C358 File Offset: 0x0003A558
		internal virtual void ControllerLeft(Controller iSender)
		{
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0003C35A File Offset: 0x0003A55A
		internal virtual void ControllerRight(Controller iSender)
		{
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x0003C35C File Offset: 0x0003A55C
		internal virtual void ControllerA(Controller iSender)
		{
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0003C35E File Offset: 0x0003A55E
		internal virtual void ControllerB(Controller iSender)
		{
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0003C360 File Offset: 0x0003A560
		internal virtual void ControllerX(Controller iSender)
		{
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0003C362 File Offset: 0x0003A562
		internal virtual void ControllerY(Controller iSender)
		{
		}

		// Token: 0x04000900 RID: 2304
		protected Vector2 mPosition = Vector2.Zero;

		// Token: 0x04000901 RID: 2305
		protected Vector2 mRelativePosition = new Vector2(0.5f, 0.5f);

		// Token: 0x04000902 RID: 2306
		protected float mScale = 1f;

		// Token: 0x04000903 RID: 2307
		protected Matrix mTransform = Matrix.Identity;

		// Token: 0x04000904 RID: 2308
		protected bool mEnabled = true;

		// Token: 0x04000905 RID: 2309
		protected bool mCanDismiss = true;

		// Token: 0x04000906 RID: 2310
		protected Vector4 mColour = Vector4.One;

		// Token: 0x04000907 RID: 2311
		protected Vector2 mSize = Vector2.Zero;

		// Token: 0x04000908 RID: 2312
		protected Vector2 mTopLeft = Vector2.Zero;

		// Token: 0x04000909 RID: 2313
		protected Vector2 mBottomRight = Vector2.Zero;

		// Token: 0x0400090A RID: 2314
		protected Rectangle mHitBox;

		// Token: 0x0400090B RID: 2315
		protected PopupAlign mAlignment = PopupAlign.Top | PopupAlign.Left;

		// Token: 0x0400090C RID: 2316
		protected int mSelectedItem = -1;

		// Token: 0x0400090D RID: 2317
		protected Action mOnPositiveClickDelegate;

		// Token: 0x0400090E RID: 2318
		protected Action mOnNegativeClickDelegate;
	}
}
