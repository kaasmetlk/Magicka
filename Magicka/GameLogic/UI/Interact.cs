using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.Graphics;
using Microsoft.Xna.Framework;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000569 RID: 1385
	internal class Interact
	{
		// Token: 0x06002944 RID: 10564 RVA: 0x001440D4 File Offset: 0x001422D4
		public Interact(XmlNode iInput, InteractType iDefaultIconText, Dialog iParent)
		{
			this.mParent = iParent;
			this.mIconText = iDefaultIconText;
			List<Message> list = new List<Message>();
			for (int i = 0; i < iInput.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iInput.Attributes[i];
				if (xmlAttribute.Name.Equals("lock", StringComparison.OrdinalIgnoreCase))
				{
					this.mLock = bool.Parse(xmlAttribute.Value);
				}
				else if (xmlAttribute.Name.Equals("focusOwner", StringComparison.OrdinalIgnoreCase))
				{
					this.mFocusOwner = bool.Parse(xmlAttribute.Value);
				}
				else
				{
					if (!xmlAttribute.Name.Equals("count", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					this.mRepeat = int.Parse(xmlAttribute.Value);
				}
			}
			for (int j = 0; j < iInput.ChildNodes.Count; j++)
			{
				XmlNode xmlNode = iInput.ChildNodes[j];
				if (!(xmlNode is XmlComment))
				{
					if (!xmlNode.Name.Equals("message", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					list.Add(new Message(xmlNode));
				}
			}
			this.mMessages = list.ToArray();
		}

		// Token: 0x06002945 RID: 10565 RVA: 0x0014420C File Offset: 0x0014240C
		public void Initialize()
		{
			for (int i = 0; i < this.mMessages.Length; i++)
			{
				this.mMessages[i].Initialize();
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06002946 RID: 10566 RVA: 0x00144239 File Offset: 0x00142439
		public int Current
		{
			get
			{
				return this.mCurrent;
			}
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06002947 RID: 10567 RVA: 0x00144241 File Offset: 0x00142441
		public Controller Interactor
		{
			get
			{
				return this.mInteractor;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06002948 RID: 10568 RVA: 0x00144249 File Offset: 0x00142449
		public bool LockGame
		{
			get
			{
				return this.mLock;
			}
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06002949 RID: 10569 RVA: 0x00144251 File Offset: 0x00142451
		public InteractType IconText
		{
			get
			{
				return this.mIconText;
			}
		}

		// Token: 0x170009BA RID: 2490
		public Message this[int index]
		{
			get
			{
				return this.mMessages[index];
			}
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x00144263 File Offset: 0x00142463
		public bool Advance(TextBox iTextBox, Vector3 iWorldPosition, Controller iInteractor)
		{
			this.mWorldPosition = iWorldPosition;
			this.mLastOwner = null;
			this.mScreenPos = false;
			return this.Advance(iTextBox, iInteractor);
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x00144284 File Offset: 0x00142484
		public bool Advance(TextBox iTextBox, Vector2 iScreenPos, Controller iInteractor)
		{
			this.mWorldPosition.X = iScreenPos.X;
			this.mWorldPosition.Y = iScreenPos.Y;
			this.mWorldPosition.Z = 0f;
			this.mLastOwner = null;
			this.mScreenPos = true;
			return this.Advance(iTextBox, iInteractor);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x001442DB File Offset: 0x001424DB
		public bool Advance(TextBox iTextBox, Entity iDefaultOwner, Controller iInteractor)
		{
			this.mLastOwner = iDefaultOwner;
			this.mScreenPos = false;
			return this.Advance(iTextBox, iInteractor);
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x001442F4 File Offset: 0x001424F4
		public bool Advance(TextBox iTextBox, Controller iInteractor)
		{
			if (this.mRepeatCounter >= this.mRepeat && this.mRepeat > 0)
			{
				return true;
			}
			if (this.mInteractor == null && iInteractor != null)
			{
				this.mInteractor = iInteractor;
				ControlManager.Instance.LockPlayerInput(this.mInteractor);
			}
			bool flag;
			if (this.mLastOwner != null)
			{
				flag = this.mMessages[this.mCurrent].Advance(iTextBox, this.mLastOwner, this.mFocusOwner);
			}
			else if (this.mScreenPos)
			{
				flag = this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, new Vector2(this.mWorldPosition.X, this.mWorldPosition.Y));
			}
			else
			{
				flag = this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, this.mWorldPosition, this.mFocusOwner);
			}
			if (flag)
			{
				this.mCurrent++;
				if (this.mCurrent >= this.mMessages.Length)
				{
					this.mRepeatCounter++;
					this.mCurrent = 0;
					if (this.mInteractor != null)
					{
						ControlManager.Instance.UnlockPlayerInput(this.mInteractor);
						this.mInteractor = null;
					}
					return true;
				}
				if (this.mLastOwner != null)
				{
					this.mMessages[this.mCurrent].Advance(iTextBox, this.mLastOwner, this.mFocusOwner);
				}
				else if (this.mScreenPos)
				{
					this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, new Vector2(this.mWorldPosition.X, this.mWorldPosition.Y));
				}
				else
				{
					this.mMessages[this.mCurrent].Advance(iTextBox, this.mParent.Scene, this.mWorldPosition, this.mFocusOwner);
				}
			}
			return false;
		}

		// Token: 0x0600294F RID: 10575 RVA: 0x001444CC File Offset: 0x001426CC
		public void End()
		{
			if (this.mCurrent >= 0 && this.mCurrent < this.mMessages.Length)
			{
				this.mMessages[this.mCurrent].StopCue();
			}
			if (this.mInteractor != null)
			{
				ControlManager.Instance.UnlockPlayerInput(this.mInteractor);
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06002950 RID: 10576 RVA: 0x0014451C File Offset: 0x0014271C
		public int Repetitions
		{
			get
			{
				return this.mRepeat - this.mRepeatCounter;
			}
		}

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06002951 RID: 10577 RVA: 0x0014452B File Offset: 0x0014272B
		public Dialog Parent
		{
			get
			{
				return this.mParent;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06002952 RID: 10578 RVA: 0x00144533 File Offset: 0x00142733
		public bool RepeatInfinitly
		{
			get
			{
				return this.mRepeat <= 0;
			}
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x00144544 File Offset: 0x00142744
		public void Reset()
		{
			this.mCurrent = 0;
			this.mRepeatCounter = 0;
			for (int i = 0; i < this.mMessages.Length; i++)
			{
				this.mMessages[i].Reset();
			}
		}

		// Token: 0x04002CB0 RID: 11440
		private bool mLock;

		// Token: 0x04002CB1 RID: 11441
		private bool mFocusOwner = true;

		// Token: 0x04002CB2 RID: 11442
		private InteractType mIconText;

		// Token: 0x04002CB3 RID: 11443
		private Message[] mMessages;

		// Token: 0x04002CB4 RID: 11444
		private int mCurrent;

		// Token: 0x04002CB5 RID: 11445
		private int mRepeatCounter;

		// Token: 0x04002CB6 RID: 11446
		private int mRepeat = 1;

		// Token: 0x04002CB7 RID: 11447
		private Entity mLastOwner;

		// Token: 0x04002CB8 RID: 11448
		private Vector3 mWorldPosition;

		// Token: 0x04002CB9 RID: 11449
		private bool mScreenPos;

		// Token: 0x04002CBA RID: 11450
		private Controller mInteractor;

		// Token: 0x04002CBB RID: 11451
		private Dialog mParent;

		// Token: 0x0200056A RID: 1386
		public struct State
		{
			// Token: 0x06002954 RID: 10580 RVA: 0x0014457F File Offset: 0x0014277F
			public State(Interact iInteract)
			{
				this.mInteract = iInteract;
				this.mRepeatCounter = iInteract.mRepeatCounter;
				this.mCurrent = iInteract.mCurrent;
			}

			// Token: 0x06002955 RID: 10581 RVA: 0x001445A0 File Offset: 0x001427A0
			public void ApplyState()
			{
				for (int i = 0; i < this.mInteract.mMessages.Length; i++)
				{
					this.mInteract.mMessages[i].Reset();
				}
				this.mInteract.mRepeatCounter = this.mRepeatCounter;
				this.mInteract.mCurrent = this.mCurrent;
			}

			// Token: 0x06002956 RID: 10582 RVA: 0x001445F9 File Offset: 0x001427F9
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mCurrent);
				iWriter.Write(this.mRepeatCounter);
			}

			// Token: 0x06002957 RID: 10583 RVA: 0x00144613 File Offset: 0x00142813
			internal void Read(BinaryReader iReader)
			{
				this.mCurrent = iReader.ReadInt32();
				this.mRepeatCounter = iReader.ReadInt32();
			}

			// Token: 0x04002CBC RID: 11452
			private int mCurrent;

			// Token: 0x04002CBD RID: 11453
			private int mRepeatCounter;

			// Token: 0x04002CBE RID: 11454
			private Interact mInteract;
		}
	}
}
