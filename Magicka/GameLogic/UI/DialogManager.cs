using System;
using System.Collections.Generic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.GameLogic.UI
{
	// Token: 0x02000420 RID: 1056
	internal sealed class DialogManager
	{
		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060020B0 RID: 8368 RVA: 0x000E7BE8 File Offset: 0x000E5DE8
		public static DialogManager Instance
		{
			get
			{
				if (DialogManager.mSingelton == null)
				{
					lock (DialogManager.mSingeltonLock)
					{
						if (DialogManager.mSingelton == null)
						{
							DialogManager.mSingelton = new DialogManager();
						}
					}
				}
				return DialogManager.mSingelton;
			}
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x000E7C3C File Offset: 0x000E5E3C
		private DialogManager()
		{
			this.mTextBoxes = new TextBox[8];
			this.mActiveInteracts = new Interact[8];
			for (int i = 0; i < 8; i++)
			{
				this.mTextBoxes[i] = new TextBox();
			}
			this.mCutsceneText = new CutsceneText();
			this.mSubtitles = new Text(1024, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
			this.mSubtitles.DrawShadows = true;
			this.mSubtitles.ShadowsOffset = new Vector2(1f);
			this.mSubtitles.ShadowAlpha = 0.8f;
			GUIBasicEffect effect;
			lock (Game.Instance.GraphicsDevice)
			{
				effect = new GUIBasicEffect(Game.Instance.GraphicsDevice, null);
			}
			this.mRenderData = new DialogManager.RenderData[3];
			for (int j = 0; j < 3; j++)
			{
				DialogManager.RenderData renderData = new DialogManager.RenderData();
				this.mRenderData[j] = renderData;
				renderData.Effect = effect;
				renderData.Text = this.mSubtitles;
			}
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x000E7D6C File Offset: 0x000E5F6C
		public void SetDialogs(DialogCollection iDialogs)
		{
			this.mDialogs = iDialogs;
			for (int i = 0; i < 8; i++)
			{
				this.mActiveInteracts[i] = null;
			}
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x000E7D98 File Offset: 0x000E5F98
		internal void StartDialog(int iDialog, Vector2 iScreePosition, Controller iInteractor)
		{
			if (this.DialogActive(iDialog))
			{
				return;
			}
			Interact interact = this.mDialogs[iDialog].Pop();
			if (interact == null)
			{
				return;
			}
			int num = 0;
			while (num < 8 && (this.mActiveInteracts[num] != null || this.mTextBoxes[num].Visible))
			{
				num++;
			}
			if (num == 8)
			{
				num = 0;
			}
			this.mActiveInteracts[num] = interact;
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
			{
				this.mDrawCutsceneText = true;
				this.mActiveInteracts[num].Advance(this.mCutsceneText, iScreePosition, iInteractor);
				this.mCutsceneInteract = num;
			}
			else
			{
				this.mActiveInteracts[num].Advance(this.mTextBoxes[num], iScreePosition, iInteractor);
				this.mDrawCutsceneText = false;
			}
			this.mAwaitingInput = false;
			this.mHoldoffInputTimer = 0.2f;
			if (interact.LockGame)
			{
				PhysicsManager.Instance.Freeze();
				ControlManager.Instance.LimitInput(this);
			}
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x000E7E94 File Offset: 0x000E6094
		internal void StartDialog(int iDialog, Vector3 iWorldPosition, Controller iInteractor)
		{
			if (this.DialogActive(iDialog))
			{
				return;
			}
			Interact interact = this.mDialogs[iDialog].Pop();
			if (interact == null)
			{
				return;
			}
			int num = 0;
			while (num < 8 && (this.mActiveInteracts[num] != null || this.mTextBoxes[num].Visible))
			{
				num++;
			}
			if (num == 8)
			{
				num = 0;
			}
			this.mActiveInteracts[num] = interact;
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
			{
				this.mDrawCutsceneText = true;
				this.mActiveInteracts[num].Advance(this.mCutsceneText, iWorldPosition, iInteractor);
				this.mCutsceneInteract = num;
			}
			else
			{
				this.mActiveInteracts[num].Advance(this.mTextBoxes[num], iWorldPosition, iInteractor);
				this.mDrawCutsceneText = false;
			}
			this.mAwaitingInput = false;
			this.mHoldoffInputTimer = 0.2f;
			if (interact.LockGame)
			{
				PhysicsManager.Instance.Freeze();
				ControlManager.Instance.LimitInput(this);
			}
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x000E7F90 File Offset: 0x000E6190
		public void StartDialog(int iDialog, Entity iOwner, Controller iInteractor)
		{
			if (this.DialogActive(iDialog))
			{
				return;
			}
			Interact interact;
			if (this.mDialogs.ContainsKey(iDialog))
			{
				interact = this.mDialogs[iDialog].Pop();
			}
			else
			{
				interact = null;
			}
			if (interact == null)
			{
				return;
			}
			int num = 0;
			while (num < 8 && (this.mActiveInteracts[num] != null || this.mTextBoxes[num].Visible))
			{
				num++;
			}
			if (num == 8)
			{
				num = 0;
			}
			this.mActiveInteracts[num] = interact;
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
			{
				this.mDrawCutsceneText = true;
				this.mActiveInteracts[num].Advance(this.mCutsceneText, iOwner, iInteractor);
				this.mCutsceneInteract = num;
			}
			else
			{
				this.mActiveInteracts[num].Advance(this.mTextBoxes[num], iOwner, iInteractor);
				this.mDrawCutsceneText = false;
			}
			this.mAwaitingInput = false;
			this.mHoldoffInputTimer = 0.2f;
			if (interact.LockGame)
			{
				PhysicsManager.Instance.Freeze();
				ControlManager.Instance.LimitInput(this);
			}
		}

		// Token: 0x060020B6 RID: 8374 RVA: 0x000E80A0 File Offset: 0x000E62A0
		public void Update(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix)
		{
			this.mHoldoffInputTimer -= iDeltaTime;
			for (int i = 0; i < 8; i++)
			{
				this.mTextBoxes[i].Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
				if (this.mTextBoxes[i].Visible && this.mTextBoxes[i].Owner != null && this.mTextBoxes[i].Owner.Dead)
				{
					if (this.mActiveInteracts[i] != null)
					{
						this.mActiveInteracts[i].End();
						this.mActiveInteracts[i] = null;
					}
					this.mTextBoxes[i].Hide();
				}
			}
			for (int j = 0; j < this.mAdditionalTextBoxes.Count; j++)
			{
				this.mAdditionalTextBoxes[j].Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
				if (!this.mAdditionalTextBoxes[j].Visible)
				{
					this.mAdditionalTextBoxes.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < this.mMessageBoxes.Count; k++)
			{
				this.mMessageBoxes[k].Update(iDataChannel, iDeltaTime);
				if (this.mMessageBoxes[k].Dead)
				{
					this.mMessageBoxes.RemoveAt(k);
					k--;
				}
			}
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene && this.mDrawCutsceneText)
			{
				this.mCutsceneText.Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
			}
			if (this.mDrawSubtitles)
			{
				DialogManager.RenderData renderData = this.mRenderData[(int)iDataChannel];
				renderData.SubtitleHeight = this.mSubtitleHeight;
				GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, renderData);
			}
		}

		// Token: 0x060020B7 RID: 8375 RVA: 0x000E8240 File Offset: 0x000E6440
		public void Advance(Controller iSender)
		{
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
			{
				return;
			}
			if (this.mHoldoffInputTimer > 0f)
			{
				return;
			}
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Interactor == iSender)
				{
					if (NetworkManager.Instance.State != NetworkState.Offline)
					{
						DialogAdvanceMessage dialogAdvanceMessage = default(DialogAdvanceMessage);
						dialogAdvanceMessage.Interact = this.mActiveInteracts[i].Current;
						NetworkManager.Instance.Interface.SendMessage<DialogAdvanceMessage>(ref dialogAdvanceMessage);
					}
					if (this.mTextBoxes[i].Animating)
					{
						this.mTextBoxes[i].FinishAnimation(true);
					}
					else if (this.mActiveInteracts[i].Advance(this.mTextBoxes[i], iSender))
					{
						this.mDialogs.DialogFinished(this.mActiveInteracts[i].Parent.ID);
						this.mAwaitingInput = false;
						this.mHoldoffInputTimer = 0.2f;
						if (this.mActiveInteracts[i].LockGame)
						{
							PhysicsManager.Instance.UnFreeze();
							ControlManager.Instance.UnlimitInput(this);
						}
						this.mActiveInteracts[i] = null;
					}
				}
			}
		}

		// Token: 0x060020B8 RID: 8376 RVA: 0x000E8384 File Offset: 0x000E6584
		public void NetworkAdvance(int iInteract)
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Current == iInteract)
				{
					if (this.mTextBoxes[i].Animating)
					{
						this.mTextBoxes[i].FinishAnimation(true);
					}
					else if (this.mActiveInteracts[i].Advance(this.mTextBoxes[i], null))
					{
						this.mDialogs.DialogFinished(this.mActiveInteracts[i].Parent.ID);
						this.mAwaitingInput = false;
						this.mHoldoffInputTimer = 0.2f;
						if (this.mActiveInteracts[i].LockGame)
						{
							PhysicsManager.Instance.UnFreeze();
							ControlManager.Instance.UnlimitInput(this);
						}
						this.mActiveInteracts[i] = null;
					}
				}
			}
		}

		// Token: 0x060020B9 RID: 8377 RVA: 0x000E8458 File Offset: 0x000E6658
		public void Advance(TextBox iSender)
		{
			if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
			{
				if (this.mCutsceneText == iSender && this.mActiveInteracts[this.mCutsceneInteract] != null && this.mActiveInteracts[this.mCutsceneInteract].Advance(iSender, null))
				{
					this.mDialogs.DialogFinished(this.mActiveInteracts[this.mCutsceneInteract].Parent.ID);
					if (this.mActiveInteracts[this.mCutsceneInteract].LockGame)
					{
						PhysicsManager.Instance.UnFreeze();
						ControlManager.Instance.UnlimitInput(this);
					}
					this.mActiveInteracts[this.mCutsceneInteract] = null;
					this.mDrawCutsceneText = false;
				}
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					if (this.mTextBoxes[i] == iSender && this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Advance(iSender, null))
					{
						this.mDialogs.DialogFinished(this.mActiveInteracts[i].Parent.ID);
						if (this.mActiveInteracts[i].LockGame)
						{
							PhysicsManager.Instance.UnFreeze();
							ControlManager.Instance.UnlimitInput(this);
						}
						this.mActiveInteracts[i] = null;
					}
				}
			}
			for (int j = 0; j < this.mAdditionalTextBoxes.Count; j++)
			{
				this.mAdditionalTextBoxes[j].Hide();
			}
		}

		// Token: 0x060020BA RID: 8378 RVA: 0x000E85D0 File Offset: 0x000E67D0
		public void ControllerSelect(Controller iSender)
		{
			if (this.mHoldoffInputTimer > 0f)
			{
				return;
			}
			if (this.mMessageBoxes.Count > 0)
			{
				this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnSelect(iSender);
				return;
			}
			this.Advance(iSender);
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x000E8620 File Offset: 0x000E6820
		public void ControllerMove(Controller iSender, ControllerDirection iValue)
		{
			if (this.mHoldoffInputTimer > 0f | this.mMessageBoxes.Count == 0)
			{
				return;
			}
			Game.Instance.IsMouseVisible = false;
			this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnMove(iSender, iValue);
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x000E8676 File Offset: 0x000E6876
		public void ControllerType(Controller iSender, char iValue)
		{
			if (this.mHoldoffInputTimer > 0f | this.mMessageBoxes.Count == 0)
			{
				return;
			}
			this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnTextInput(iValue);
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x000E86B8 File Offset: 0x000E68B8
		public void ControllerMouseClick(Controller iSender, MouseState iNewState, MouseState iOldState)
		{
			MessageBox messageBox = this.mMessageBoxes[this.mMessageBoxes.Count - 1];
			messageBox.OnMouseClick(iNewState, iOldState);
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x000E86E8 File Offset: 0x000E68E8
		public void ControllerEsc(Controller iSender)
		{
			MessageBox messageBox = this.mMessageBoxes[this.mMessageBoxes.Count - 1];
			messageBox.ControllerEsc(iSender);
		}

		// Token: 0x060020BF RID: 8383 RVA: 0x000E8715 File Offset: 0x000E6915
		public void ControllerMouseRelease(Controller iSender, MouseState iNewState, MouseState iOldState)
		{
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x000E8718 File Offset: 0x000E6918
		public void ControllerMouseMove(Controller iSender, MouseState iNewState, MouseState iOldState)
		{
			MessageBox messageBox = this.mMessageBoxes[this.mMessageBoxes.Count - 1];
			messageBox.OnMouseMove(iNewState, iOldState);
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x000E8746 File Offset: 0x000E6946
		public void ShowSubtitles(string iSubtitles)
		{
			this.mSubtitles.SetText(iSubtitles);
			this.mSubtitleHeight = this.mSubtitles.Font.MeasureText(iSubtitles, true).Y;
			this.mDrawSubtitles = true;
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x000E8778 File Offset: 0x000E6978
		public void HideSubtitles()
		{
			this.mDrawSubtitles = false;
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060020C3 RID: 8387 RVA: 0x000E8784 File Offset: 0x000E6984
		public bool IsDialogActive
		{
			get
			{
				for (int i = 0; i < this.mActiveInteracts.Length; i++)
				{
					if (this.mActiveInteracts[i] != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060020C4 RID: 8388 RVA: 0x000E87B1 File Offset: 0x000E69B1
		public bool AwaitingInput
		{
			get
			{
				return this.mAwaitingInput | this.mMessageBoxes.Count > 0 | this.mHoldoffInputTimer > 0f;
			}
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x000E87D8 File Offset: 0x000E69D8
		public bool CanAdvance(Controller iSender)
		{
			for (int i = 0; i < this.mActiveInteracts.Length; i++)
			{
				if (this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Interactor == iSender)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060020C6 RID: 8390 RVA: 0x000E8815 File Offset: 0x000E6A15
		public bool MessageBoxActive
		{
			get
			{
				return this.mMessageBoxes.Count > 0;
			}
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060020C7 RID: 8391 RVA: 0x000E8825 File Offset: 0x000E6A25
		public BitmapFont SubtitleFont
		{
			get
			{
				return this.mSubtitles.Font;
			}
		}

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060020C8 RID: 8392 RVA: 0x000E8832 File Offset: 0x000E6A32
		public DialogCollection Dialogs
		{
			get
			{
				return this.mDialogs;
			}
		}

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060020C9 RID: 8393 RVA: 0x000E883A File Offset: 0x000E6A3A
		public bool HoldoffInput
		{
			get
			{
				return this.mHoldoffInputTimer > 0f;
			}
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x000E8849 File Offset: 0x000E6A49
		public bool IsDialogDone(int iID, int iInteractIndex)
		{
			return this.mDialogs.IsDialogDone(iID) && this.mDialogs[iID].InteractedMessages > iInteractIndex;
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x000E8870 File Offset: 0x000E6A70
		public InteractType GetDialogIconText(int iDialog)
		{
			if (!this.mDialogs.ContainsKey(iDialog))
			{
				return InteractType.Talk;
			}
			Interact interact = this.mDialogs[iDialog].Peek();
			if (interact == null)
			{
				return InteractType.Talk;
			}
			return interact.IconText;
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x000E88AC File Offset: 0x000E6AAC
		public TextBox GetTextBox()
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null)
				{
					return this.mTextBoxes[i];
				}
			}
			return null;
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x000E88DC File Offset: 0x000E6ADC
		public void EndAll()
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null)
				{
					this.mActiveInteracts[i].End();
				}
				this.mActiveInteracts[i] = null;
				this.mTextBoxes[i].FinishAnimation(false);
				this.mTextBoxes[i].Hide();
			}
			this.mCutsceneText.Hide();
			this.mCutsceneText.FinishAnimation(false);
			this.mAwaitingInput = false;
			PhysicsManager.Instance.UnFreeze();
			ControlManager.Instance.UnlimitInput(this);
		}

		// Token: 0x060020CE RID: 8398 RVA: 0x000E8964 File Offset: 0x000E6B64
		public void End(Avatar iAvatar)
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null && iAvatar.Player.Controller != null && this.mActiveInteracts[i].Interactor == iAvatar.Player.Controller)
				{
					this.mActiveInteracts[i].End();
					this.mActiveInteracts[i] = null;
					this.mTextBoxes[i].Hide();
				}
			}
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x000E89D4 File Offset: 0x000E6BD4
		public void End(int iDialog)
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Parent.ID == iDialog)
				{
					this.mActiveInteracts[i].End();
					this.mActiveInteracts[i] = null;
					this.mTextBoxes[i].Hide();
				}
			}
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x000E8A2F File Offset: 0x000E6C2F
		public void Reset()
		{
			this.EndAll();
			if (this.mDialogs != null)
			{
				this.mDialogs.Reset();
			}
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x000E8A4A File Offset: 0x000E6C4A
		public void AddTextBox(TextBox iTextBox)
		{
			this.mAdditionalTextBoxes.Add(iTextBox);
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x000E8A58 File Offset: 0x000E6C58
		public void AddMessageBox(MessageBox iMessageBox)
		{
			if (!(GameStateManager.Instance.CurrentState is PlayState) || !(GameStateManager.Instance.CurrentState as PlayState).IsPaused)
			{
				this.mHoldoffInputTimer = 0.2f;
			}
			this.mMessageBoxes.Add(iMessageBox);
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x000E8A98 File Offset: 0x000E6C98
		public bool DialogActive(int iDialog)
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mActiveInteracts[i] != null && this.mActiveInteracts[i].Parent.ID == iDialog)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400232C RID: 9004
		public const int MAXDIALOGS = 8;

		// Token: 0x0400232D RID: 9005
		public const float INPUT_COOLDOWN = 0.2f;

		// Token: 0x0400232E RID: 9006
		private static DialogManager mSingelton;

		// Token: 0x0400232F RID: 9007
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002330 RID: 9008
		private TextBox[] mTextBoxes;

		// Token: 0x04002331 RID: 9009
		private DialogCollection mDialogs;

		// Token: 0x04002332 RID: 9010
		private Interact[] mActiveInteracts;

		// Token: 0x04002333 RID: 9011
		private bool mAwaitingInput;

		// Token: 0x04002334 RID: 9012
		private float mHoldoffInputTimer;

		// Token: 0x04002335 RID: 9013
		private List<TextBox> mAdditionalTextBoxes = new List<TextBox>(10);

		// Token: 0x04002336 RID: 9014
		private List<MessageBox> mMessageBoxes = new List<MessageBox>(10);

		// Token: 0x04002337 RID: 9015
		private Text mSubtitles;

		// Token: 0x04002338 RID: 9016
		private float mSubtitleHeight;

		// Token: 0x04002339 RID: 9017
		private bool mDrawSubtitles;

		// Token: 0x0400233A RID: 9018
		private DialogManager.RenderData[] mRenderData;

		// Token: 0x0400233B RID: 9019
		private bool mDrawCutsceneText;

		// Token: 0x0400233C RID: 9020
		private int mCutsceneInteract;

		// Token: 0x0400233D RID: 9021
		private CutsceneText mCutsceneText;

		// Token: 0x02000421 RID: 1057
		private class RenderData : IRenderableGUIObject
		{
			// Token: 0x060020D5 RID: 8405 RVA: 0x000E8AE4 File Offset: 0x000E6CE4
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.Effect.SetScreenSize(screenSize.X, screenSize.Y);
				this.Effect.Color = new Vector4(1f);
				this.Effect.Begin();
				this.Effect.CurrentTechnique.Passes[0].Begin();
				this.Text.Draw(this.Effect, (float)screenSize.X * 0.5f, (float)screenSize.Y - (this.SubtitleHeight + 32f));
				this.Effect.CurrentTechnique.Passes[0].End();
				this.Effect.End();
			}

			// Token: 0x1700080D RID: 2061
			// (get) Token: 0x060020D6 RID: 8406 RVA: 0x000E8BAA File Offset: 0x000E6DAA
			public int ZIndex
			{
				get
				{
					return 2000;
				}
			}

			// Token: 0x0400233E RID: 9022
			public GUIBasicEffect Effect;

			// Token: 0x0400233F RID: 9023
			public Text Text;

			// Token: 0x04002340 RID: 9024
			public float SubtitleHeight;
		}
	}
}
