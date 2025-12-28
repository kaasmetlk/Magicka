using System;
using System.Collections.Generic;
using Magicka.GameLogic;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Gamers;
using Magicka.Graphics.Effects;
using Magicka.Localization;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;

namespace Magicka.Graphics
{
	// Token: 0x02000337 RID: 823
	internal class TutorialManager
	{
		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x0600191B RID: 6427 RVA: 0x000A5440 File Offset: 0x000A3640
		public static TutorialManager Instance
		{
			get
			{
				if (TutorialManager.mSingelton == null)
				{
					lock (TutorialManager.mSingeltonLock)
					{
						if (TutorialManager.mSingelton == null)
						{
							TutorialManager.mSingelton = new TutorialManager();
						}
					}
				}
				return TutorialManager.mSingelton;
			}
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x000A54A0 File Offset: 0x000A36A0
		private TutorialManager()
		{
			GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
			TextBoxEffect textBoxEffect;
			GUIBasicEffect iEffect;
			IndexBuffer indexBuffer;
			VertexBuffer vertexBuffer;
			VertexDeclaration iVD;
			VertexBuffer vertexBuffer2;
			VertexDeclaration iIconDeclaration;
			lock (graphicsDevice)
			{
				textBoxEffect = new TextBoxEffect(graphicsDevice, Game.Instance.Content);
				iEffect = new GUIBasicEffect(graphicsDevice, null);
				indexBuffer = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.None, IndexElementSize.SixteenBits);
				vertexBuffer = new VertexBuffer(graphicsDevice, TextBox.VERTICES.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
				iVD = new VertexDeclaration(graphicsDevice, VertexPositionNormalTexture.VertexElements);
				indexBuffer.SetData<ushort>(TextBox.INDICES);
				vertexBuffer.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
				vertexBuffer2 = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
				vertexBuffer2.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
				iIconDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
			}
			Point screenSize = RenderManager.Instance.ScreenSize;
			textBoxEffect.BorderSize = 32f;
			textBoxEffect.ScreenSize = new Vector2((float)screenSize.X, (float)screenSize.Y);
			textBoxEffect.Texture = Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_say");
			this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
			this.mHintRenderData = new TutorialManager.HintRenderData[3];
			this.mTipRenderData = new TutorialManager.HintRenderData[3];
			this.mDialogHintRenderData = new TutorialManager.DialogHintRenderData[3];
			this.mFadeRenderData = new TutorialManager.FadeScreenRenderData[3];
			for (int i = 0; i < 3; i++)
			{
				this.mFadeRenderData[i] = new TutorialManager.FadeScreenRenderData(iEffect);
				this.mHintRenderData[i] = new TutorialManager.HintRenderData(iEffect, this.mFont, textBoxEffect, indexBuffer, vertexBuffer, iVD, vertexBuffer2, iIconDeclaration);
				this.mTipRenderData[i] = new TutorialManager.HintRenderData(iEffect, this.mFont, textBoxEffect, indexBuffer, vertexBuffer, iVD, vertexBuffer2, iIconDeclaration);
				this.mDialogHintRenderData[i] = new TutorialManager.DialogHintRenderData(iEffect, this.mFont, textBoxEffect, indexBuffer, vertexBuffer, iVD);
			}
			this.mHintPosition = TutorialManager.Position.BottomRight;
			this.mTipPosition = TutorialManager.Position.Top;
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x000A56F4 File Offset: 0x000A38F4
		public void Initialize(PlayState iPlayState)
		{
			this.mPlayState = iPlayState;
			this.mTipsTexts[0] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[0].GetHashCodeCustom());
			this.mTipsTexts[1] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[1].GetHashCodeCustom());
			this.mTipsTexts[2] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[2].GetHashCodeCustom());
			this.mTipsTexts[3] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[3].GetHashCodeCustom());
			this.mTipsTexts[4] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[4].GetHashCodeCustom());
			this.mTipsTexts[5] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[5].GetHashCodeCustom());
			this.mTipsTexts[6] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[6].GetHashCodeCustom());
			this.mTipsTexts[7] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[7].GetHashCodeCustom());
			this.mTipsTexts[8] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[8].GetHashCodeCustom());
			this.mTipsTexts[9] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[9].GetHashCodeCustom());
			this.mTipsTexts[10] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[10].GetHashCodeCustom());
			int iTargetLineWidth = (int)((double)GlobalSettings.Instance.Resolution.Width * 0.8);
			for (int i = 0; i < this.mTipsTexts.Length; i++)
			{
				this.mTipsTexts[i] = this.mFont.Wrap(this.mTipsTexts[i], iTargetLineWidth, true);
			}
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x000A58A4 File Offset: 0x000A3AA4
		public void UpdateResolution()
		{
			this.mTipsTexts[0] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[0].GetHashCodeCustom());
			this.mTipsTexts[1] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[1].GetHashCodeCustom());
			this.mTipsTexts[2] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[2].GetHashCodeCustom());
			this.mTipsTexts[3] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[3].GetHashCodeCustom());
			this.mTipsTexts[4] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[4].GetHashCodeCustom());
			this.mTipsTexts[5] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[5].GetHashCodeCustom());
			this.mTipsTexts[6] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[6].GetHashCodeCustom());
			this.mTipsTexts[7] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[7].GetHashCodeCustom());
			this.mTipsTexts[8] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[8].GetHashCodeCustom());
			this.mTipsTexts[9] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[9].GetHashCodeCustom());
			this.mTipsTexts[10] = LanguageManager.Instance.GetString(TutorialManager.TipsNames[10].GetHashCodeCustom());
			int iTargetLineWidth = (int)((double)GlobalSettings.Instance.Resolution.Width * 0.8);
			for (int i = 0; i < this.mTipsTexts.Length; i++)
			{
				this.mTipsTexts[i] = this.mFont.Wrap(this.mTipsTexts[i], iTargetLineWidth, true);
			}
			if (this.mTipTimer > 0f && this.mTip != TutorialManager.Tips.None)
			{
				for (int j = 0; j < 3; j++)
				{
					this.mTipRenderData[j].SetText(this.mTipsTexts[(int)this.mTip]);
				}
			}
			if (this.mPlayState.GenericHealthBar != null)
			{
				this.mPlayState.GenericHealthBar.UpdateResolution();
			}
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x000A5AA4 File Offset: 0x000A3CA4
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mHoldOffInputTimer -= iDeltaTime;
			this.mTime += iDeltaTime;
			if ((this.mHint != 0 | this.mNewHint != 0) && this.mHintAlpha >= 0f)
			{
				TutorialManager.HintRenderData hintRenderData = this.mHintRenderData[(int)iDataChannel];
				hintRenderData.HintPosition = this.mHintPosition;
				if (this.mHint != this.mNewHint)
				{
					this.mHintAlpha = Math.Max(this.mHintAlpha - iDeltaTime * 4f, 0f);
					if (this.mHintAlpha <= 0f)
					{
						this.mHint = this.mNewHint;
						this.mHintPosition = this.mNewHintPosition;
						this.mHintAlpha = 0f;
						for (int i = 0; i < 3; i++)
						{
							this.mHintRenderData[i].SetText(this.mNewString);
						}
					}
				}
				else
				{
					this.mHintAlpha = Math.Min(this.mHintAlpha + iDeltaTime * 4f, 1f);
				}
				hintRenderData.Alpha = this.mHintAlpha;
				hintRenderData.Time = this.mTime;
				hintRenderData.Animation = this.mAnimation;
				Player[] players = Game.Instance.Players;
				for (int j = 0; j < players.Length; j++)
				{
					if (players[j].Playing && !(players[j].Gamer is NetworkGamer))
					{
						hintRenderData.GamePad = !(players[j].Controller is KeyboardMouseController);
						break;
					}
				}
				this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, hintRenderData);
			}
			if (this.mIsActive && this.mDialogTimer >= 0f)
			{
				TutorialManager.DialogHintRenderData dialogHintRenderData = this.mDialogHintRenderData[(int)iDataChannel];
				if (!this.mIsActive)
				{
					this.mDialogTimer = Math.Max(this.mDialogTimer - iDeltaTime * 4f, 0f);
				}
				else
				{
					this.mDialogTimer = Math.Min(this.mDialogTimer + iDeltaTime * 4f, 1f);
				}
				dialogHintRenderData.Alpha = this.mDialogTimer;
				this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, dialogHintRenderData);
			}
			if (this.mDarkening)
			{
				TutorialManager.FadeScreenRenderData fadeScreenRenderData = this.mFadeRenderData[(int)iDataChannel];
				if (this.mFadeIn)
				{
					this.mFadeTimer = Math.Min(this.mFadeTimer + iDeltaTime * 4f, 1f);
				}
				else
				{
					this.mFadeTimer = Math.Max(this.mFadeTimer - iDeltaTime * 4f, 0f);
					this.mDarkening = (this.mFadeTimer > 0f);
				}
				fadeScreenRenderData.FadeAlpha = this.mFadeTimer;
				this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, fadeScreenRenderData);
			}
			if (this.mPlayState.GameType == GameType.Campaign && this.mPlayState.Level != null && this.mPlayState.Level.CurrentScene.ID != Defines.WC_3_HASH && this.mHint == 0)
			{
				this.mTipTimer -= iDeltaTime;
				if (this.mPlayState.IsInCutscene && this.mTipTimer > 0.25f)
				{
					this.mNewTip = TutorialManager.Tips.None;
					this.mTipTimer = 0.25f;
				}
				TutorialManager.HintRenderData hintRenderData2 = this.mTipRenderData[(int)iDataChannel];
				hintRenderData2.HintPosition = this.mTipPosition;
				if (((this.mNewTip != TutorialManager.Tips.None && this.mTipTimer <= 0f) || this.mNewTip == TutorialManager.Tips.WetLightning) && this.mTip != this.mNewTip)
				{
					SaveData.tip[] shownTips = this.mPlayState.Info.ShownTips;
					if ((this.mPlayState.PlayTime - shownTips[(int)this.mNewTip].timeStamp >= 180.0 || this.mNewTip == TutorialManager.Tips.WetLightning) && shownTips[(int)this.mNewTip].count < 3)
					{
						shownTips[(int)this.mNewTip].timeStamp = this.mPlayState.PlayTime;
						SaveData.tip[] array = shownTips;
						TutorialManager.Tips tips = this.mNewTip;
						array[(int)tips].count = array[(int)tips].count + 1;
						this.mTip = this.mNewTip;
						this.mTipPosition = this.mNewTipPosition;
						this.mTipTimer = 10f;
						for (int k = 0; k < 3; k++)
						{
							this.mTipRenderData[k].SetText(this.mTipsTexts[(int)this.mNewTip]);
						}
					}
					this.mNewTip = TutorialManager.Tips.None;
				}
				if (this.mTipTimer > 0f)
				{
					float alpha = 1f;
					if (this.mTipTimer >= 9.75f)
					{
						alpha = (10f - this.mTipTimer) * 4f;
					}
					else if (this.mTipTimer <= 0.25f)
					{
						alpha = this.mTipTimer * 4f;
					}
					hintRenderData2.Alpha = alpha;
					hintRenderData2.Time = this.mTipTimer;
					this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, hintRenderData2);
					return;
				}
				this.mTip = TutorialManager.Tips.None;
			}
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x000A5F84 File Offset: 0x000A4184
		public void Reset()
		{
			this.mEnabledElements = (Elements.Earth | Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Shield | Elements.Ice | Elements.Steam | Elements.Poison);
			for (int i = 0; i < this.mEnabledCastTypes.Length; i++)
			{
				this.mEnabledCastTypes[i] = true;
			}
			this.mPushEnabled = true;
			this.mNewEnabledElements = this.mEnabledElements;
			this.mNewHint = (this.mHint = 0);
			this.mHintAlpha = 0f;
			this.mDialogTimer = 0f;
			this.mElementHint = Elements.None;
			this.mMagickHint = MagickType.None;
			this.mIsActive = false;
			this.mDialogTimer = 0f;
			this.mFadeTimer = 0f;
			ControlManager.Instance.UnlimitInput(this);
			KeyboardHUD.Instance.Reset();
			this.mHoldOffInputTimer = 0f;
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06001921 RID: 6433 RVA: 0x000A603C File Offset: 0x000A423C
		public Elements EnabledElements
		{
			get
			{
				return this.mEnabledElements;
			}
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x000A6044 File Offset: 0x000A4244
		public void EnableCastType(CastType iCastType)
		{
			this.mEnabledCastTypes[(int)iCastType] = true;
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x000A604F File Offset: 0x000A424F
		public void DisableCastType(CastType iCastType)
		{
			this.mEnabledCastTypes[(int)iCastType] = false;
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x000A605A File Offset: 0x000A425A
		public bool IsCastTypeEnabled(CastType iCastType)
		{
			return this.mEnabledCastTypes[(int)iCastType];
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x000A6064 File Offset: 0x000A4264
		public void EnablePush()
		{
			this.mPushEnabled = true;
		}

		// Token: 0x06001926 RID: 6438 RVA: 0x000A606D File Offset: 0x000A426D
		public void DisablePush()
		{
			this.mPushEnabled = false;
		}

		// Token: 0x06001927 RID: 6439 RVA: 0x000A6076 File Offset: 0x000A4276
		public bool IsPushEnabled()
		{
			return this.mPushEnabled;
		}

		// Token: 0x06001928 RID: 6440 RVA: 0x000A6080 File Offset: 0x000A4280
		public void EnableElement(Elements iElement)
		{
			Elements iElement2 = iElement & ~this.mEnabledElements;
			this.mEnabledElements |= iElement;
			this.mNewEnabledElements = this.mEnabledElements;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					if (players[i].Controller is DirectInputController | players[i].Controller is XInputController)
					{
						players[i].SpellWheel.Enable(iElement2);
					}
					else
					{
						KeyboardHUD.Instance.Enable(iElement2);
					}
				}
			}
		}

		// Token: 0x06001929 RID: 6441 RVA: 0x000A6114 File Offset: 0x000A4314
		public void DisableElement(Elements iElement)
		{
			Elements iElement2 = iElement & this.mEnabledElements;
			this.mEnabledElements &= ~iElement;
			this.mNewEnabledElements = this.mEnabledElements;
			Player[] players = Game.Instance.Players;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].Playing)
				{
					if (players[i].Controller is DirectInputController | players[i].Controller is XInputController)
					{
						players[i].SpellWheel.Disable(iElement2);
					}
					else
					{
						KeyboardHUD.Instance.Disable(iElement2);
					}
				}
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x000A61A6 File Offset: 0x000A43A6
		public bool IsElementEnabled(Elements iElement)
		{
			return (this.mEnabledElements & iElement) != Elements.None;
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x000A61B8 File Offset: 0x000A43B8
		public void SetMagickHint(MagickType iMagickType, string iText, int iTrigger, float? iScale, Vector2? iSize)
		{
			this.mIsActive = true;
			this.mTrigger = iTrigger;
			this.mElementHint = Elements.None;
			this.mMagickHint = iMagickType;
			this.mDialogString = iText;
			this.mDialogTimer = 0f;
			this.EnableDark();
			if (iText == null)
			{
				return;
			}
			float num;
			if (iScale != null)
			{
				num = iScale.Value;
			}
			else
			{
				num = 1f;
			}
			Vector2 iSize2;
			if (iSize != null)
			{
				iSize2 = iSize.Value;
			}
			else
			{
				iSize2 = this.mFont.MeasureText(iText, true);
				Vector2 vector = new Vector2(num * TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, num * TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y);
				if (vector.Y > iSize2.Y)
				{
					iSize2.Y = vector.Y;
				}
				if (iMagickType != MagickType.None)
				{
					iSize2.X += vector.X;
				}
			}
			if (iMagickType != MagickType.None)
			{
				for (int i = 0; i < 3; i++)
				{
					this.mDialogHintRenderData[i].SetInfo(iMagickType, iText, num, iSize2);
				}
			}
			this.mHoldOffInputTimer = 1f;
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x000A62B8 File Offset: 0x000A44B8
		public void SetElementHint(Elements iElement, string iText, int iTrigger, float? iScale, Vector2? iSize)
		{
			if (iElement != Elements.None)
			{
				iText = LanguageManager.Instance.GetString(Defines.ELEMENT_STRINGS[Spell.ElementIndex(iElement)]) + "\n\n" + iText;
			}
			this.mIsActive = true;
			this.mTrigger = iTrigger;
			this.mElementHint = iElement;
			this.mMagickHint = MagickType.None;
			this.mDialogString = iText;
			this.EnableDark();
			if (iText == null)
			{
				return;
			}
			float num;
			if (iScale != null)
			{
				num = iScale.Value;
			}
			else
			{
				num = 1f;
			}
			if (iSize != null)
			{
				Vector2 iSize2 = iSize.Value;
			}
			else
			{
				Vector2 iSize2 = this.mFont.MeasureText(iText, true);
				Vector2 vector = new Vector2(num * TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, num * TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y);
				if (vector.Y > iSize2.Y)
				{
					iSize2.Y = vector.Y;
				}
				if (iElement != Elements.None)
				{
					iSize2.X += vector.X;
				}
				for (int i = 0; i < 3; i++)
				{
					this.mDialogHintRenderData[i].SetInfo(iElement, iText, num, iSize2);
				}
			}
			this.mHoldOffInputTimer = 1f;
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x000A63D4 File Offset: 0x000A45D4
		public void RemoveDialogHint()
		{
			if (!this.mIsActive | this.mHoldOffInputTimer > 0f)
			{
				return;
			}
			this.mIsActive = false;
			this.mElementHint = Elements.None;
			this.mMagickHint = MagickType.None;
			if (this.mTrigger != 0)
			{
				(GameStateManager.Instance.CurrentState as PlayState).Level.CurrentScene.ExecuteTrigger(this.mTrigger, null, false);
			}
			this.mTrigger = 0;
			this.DisableDark();
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x000A644B File Offset: 0x000A464B
		public bool IsDialogHintDone()
		{
			return !this.mIsActive;
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x000A6456 File Offset: 0x000A4656
		public void HideAll()
		{
			this.mIsActive = false;
			this.mElementHint = Elements.None;
			this.mMagickHint = MagickType.None;
			this.mTrigger = 0;
			this.DisableDark();
			this.mNewTip = TutorialManager.Tips.None;
			this.mTipTimer = 0f;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x000A648C File Offset: 0x000A468C
		public void SetHint(int iNewHintHash, string iNewString, TutorialManager.HintAnimations iAnimation, TutorialManager.Position iHintPosition)
		{
			this.mTime = 0f;
			this.mAnimation = iAnimation;
			this.mNewHint = iNewHintHash;
			this.mNewString = iNewString;
			this.mNewHintPosition = iHintPosition;
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x000A64B6 File Offset: 0x000A46B6
		public void RemoveHint()
		{
			this.mNewHint = 0;
			this.DisableDark();
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x000A64C5 File Offset: 0x000A46C5
		public void SetTip(TutorialManager.Tips iTip, TutorialManager.Position iTipPosition)
		{
			if (this.mNewTip != TutorialManager.Tips.None && iTip != TutorialManager.Tips.WetLightning && this.mTip != TutorialManager.Tips.Wet && iTip != TutorialManager.Tips.Dying)
			{
				return;
			}
			this.mNewTip = iTip;
			this.mNewHintPosition = iTipPosition;
		}

		// Token: 0x06001933 RID: 6451 RVA: 0x000A64F1 File Offset: 0x000A46F1
		public void RemoveTip()
		{
			this.mNewTip = TutorialManager.Tips.None;
			this.mTipTimer = 0f;
		}

		// Token: 0x06001934 RID: 6452 RVA: 0x000A6505 File Offset: 0x000A4705
		public void EnableDark()
		{
			this.mDarkening = true;
			this.mFadeIn = true;
		}

		// Token: 0x06001935 RID: 6453 RVA: 0x000A6515 File Offset: 0x000A4715
		public void DisableDark()
		{
			this.mFadeIn = false;
		}

		// Token: 0x04001AEB RID: 6891
		private const int MAX_TIMES_TIP_SHOWN = 3;

		// Token: 0x04001AEC RID: 6892
		private const int FIRST_INDEX_OF_SECOND_MAGICKS_TEXTURE = 37;

		// Token: 0x04001AED RID: 6893
		public const int DIALOG_TEXT_WIDTH = 300;

		// Token: 0x04001AEE RID: 6894
		public const int HINT_WIDTH = 300;

		// Token: 0x04001AEF RID: 6895
		public const MagickaFont FONT = MagickaFont.Maiandra14;

		// Token: 0x04001AF0 RID: 6896
		public static string[] TipsNames = new string[]
		{
			"#tu_text_hint_equipment_key",
			"#tu_text_hint_equipment_pad",
			"#tu_text_hint_wet_lightning",
			"#tu_text_hint_wet",
			"#tu_text_hint_cold",
			"#tu_text_hint_poison",
			"#tip09",
			"#tip10",
			"#tip15",
			"#tip17",
			"#tip18"
		};

		// Token: 0x04001AF1 RID: 6897
		private string[] mTipsTexts = new string[11];

		// Token: 0x04001AF2 RID: 6898
		private static TutorialManager mSingelton;

		// Token: 0x04001AF3 RID: 6899
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04001AF4 RID: 6900
		private TutorialManager.DialogHintRenderData[] mDialogHintRenderData;

		// Token: 0x04001AF5 RID: 6901
		private Elements mElementHint;

		// Token: 0x04001AF6 RID: 6902
		private MagickType mMagickHint;

		// Token: 0x04001AF7 RID: 6903
		private bool mIsActive;

		// Token: 0x04001AF8 RID: 6904
		private TutorialManager.HintAnimations mAnimation;

		// Token: 0x04001AF9 RID: 6905
		private string mDialogString;

		// Token: 0x04001AFA RID: 6906
		private float mDialogTimer;

		// Token: 0x04001AFB RID: 6907
		private int mTrigger;

		// Token: 0x04001AFC RID: 6908
		private int mHint;

		// Token: 0x04001AFD RID: 6909
		private int mNewHint;

		// Token: 0x04001AFE RID: 6910
		private string mNewString;

		// Token: 0x04001AFF RID: 6911
		private TutorialManager.Position mHintPosition;

		// Token: 0x04001B00 RID: 6912
		private TutorialManager.Position mNewHintPosition = TutorialManager.Position.BottomRight;

		// Token: 0x04001B01 RID: 6913
		private float mHintAlpha;

		// Token: 0x04001B02 RID: 6914
		private TutorialManager.HintRenderData[] mHintRenderData;

		// Token: 0x04001B03 RID: 6915
		private BitmapFont mFont;

		// Token: 0x04001B04 RID: 6916
		private Elements mEnabledElements = Elements.All;

		// Token: 0x04001B05 RID: 6917
		private bool[] mEnabledCastTypes = new bool[]
		{
			true,
			true,
			true,
			true,
			true,
			true
		};

		// Token: 0x04001B06 RID: 6918
		private bool mPushEnabled = true;

		// Token: 0x04001B07 RID: 6919
		private bool mDarkening;

		// Token: 0x04001B08 RID: 6920
		private bool mFadeIn;

		// Token: 0x04001B09 RID: 6921
		private float mFadeTimer;

		// Token: 0x04001B0A RID: 6922
		private TutorialManager.FadeScreenRenderData[] mFadeRenderData;

		// Token: 0x04001B0B RID: 6923
		private Elements mNewEnabledElements = Elements.Earth | Elements.Water | Elements.Cold | Elements.Fire | Elements.Lightning | Elements.Arcane | Elements.Life | Elements.Shield | Elements.Ice | Elements.Steam | Elements.Poison;

		// Token: 0x04001B0C RID: 6924
		private Dictionary<int, bool> mFinishedHints = new Dictionary<int, bool>();

		// Token: 0x04001B0D RID: 6925
		private float mTime;

		// Token: 0x04001B0E RID: 6926
		private float mHoldOffInputTimer;

		// Token: 0x04001B0F RID: 6927
		private TutorialManager.Tips mTip = TutorialManager.Tips.None;

		// Token: 0x04001B10 RID: 6928
		private TutorialManager.Tips mNewTip = TutorialManager.Tips.None;

		// Token: 0x04001B11 RID: 6929
		private float mTipTimer;

		// Token: 0x04001B12 RID: 6930
		private TutorialManager.HintRenderData[] mTipRenderData;

		// Token: 0x04001B13 RID: 6931
		private TutorialManager.Position mTipPosition;

		// Token: 0x04001B14 RID: 6932
		private TutorialManager.Position mNewTipPosition = TutorialManager.Position.Top;

		// Token: 0x04001B15 RID: 6933
		private PlayState mPlayState;

		// Token: 0x02000338 RID: 824
		[Flags]
		public enum HintAnimations
		{
			// Token: 0x04001B17 RID: 6935
			None = 0,
			// Token: 0x04001B18 RID: 6936
			Interact = 1,
			// Token: 0x04001B19 RID: 6937
			NavMagick = 2,
			// Token: 0x04001B1A RID: 6938
			Conjure = 4,
			// Token: 0x04001B1B RID: 6939
			ConjureWater = 8,
			// Token: 0x04001B1C RID: 6940
			ConjureLife = 16,
			// Token: 0x04001B1D RID: 6941
			ConjureShield = 32,
			// Token: 0x04001B1E RID: 6942
			ConjureCold = 64,
			// Token: 0x04001B1F RID: 6943
			ConjureLightning = 128,
			// Token: 0x04001B20 RID: 6944
			ConjureArcane = 256,
			// Token: 0x04001B21 RID: 6945
			ConjureEarth = 512,
			// Token: 0x04001B22 RID: 6946
			ConjureFire = 1024,
			// Token: 0x04001B23 RID: 6947
			CastForce = 2048,
			// Token: 0x04001B24 RID: 6948
			CastArea = 4096,
			// Token: 0x04001B25 RID: 6949
			CastSelf = 8192,
			// Token: 0x04001B26 RID: 6950
			CastMagick = 16384,
			// Token: 0x04001B27 RID: 6951
			Block = 32768,
			// Token: 0x04001B28 RID: 6952
			Attack = 65536
		}

		// Token: 0x02000339 RID: 825
		public enum Tips
		{
			// Token: 0x04001B2A RID: 6954
			None = -1,
			// Token: 0x04001B2B RID: 6955
			ItemKey,
			// Token: 0x04001B2C RID: 6956
			ItemPad,
			// Token: 0x04001B2D RID: 6957
			WetLightning,
			// Token: 0x04001B2E RID: 6958
			Wet,
			// Token: 0x04001B2F RID: 6959
			Cold,
			// Token: 0x04001B30 RID: 6960
			Poison,
			// Token: 0x04001B31 RID: 6961
			Heal,
			// Token: 0x04001B32 RID: 6962
			MagicksScroll,
			// Token: 0x04001B33 RID: 6963
			MagicksSpell,
			// Token: 0x04001B34 RID: 6964
			Beams,
			// Token: 0x04001B35 RID: 6965
			Dying,
			// Token: 0x04001B36 RID: 6966
			NR_OF_TIPS
		}

		// Token: 0x0200033A RID: 826
		public enum Position
		{
			// Token: 0x04001B38 RID: 6968
			TopLeft,
			// Token: 0x04001B39 RID: 6969
			Top,
			// Token: 0x04001B3A RID: 6970
			TopRight,
			// Token: 0x04001B3B RID: 6971
			CenterLeft,
			// Token: 0x04001B3C RID: 6972
			Center,
			// Token: 0x04001B3D RID: 6973
			CenterRight,
			// Token: 0x04001B3E RID: 6974
			BottomLeft,
			// Token: 0x04001B3F RID: 6975
			Bottom,
			// Token: 0x04001B40 RID: 6976
			BottomRight
		}

		// Token: 0x0200033B RID: 827
		protected class FadeScreenRenderData : IRenderableGUIObject
		{
			// Token: 0x06001937 RID: 6455 RVA: 0x000A65A4 File Offset: 0x000A47A4
			static FadeScreenRenderData()
			{
				lock (Game.Instance.GraphicsDevice)
				{
					TutorialManager.FadeScreenRenderData.sVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_TL.Length * TutorialManager.FadeScreenRenderData.sVertexStride, BufferUsage.None);
					TutorialManager.FadeScreenRenderData.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_TL);
				}
			}

			// Token: 0x06001938 RID: 6456 RVA: 0x000A662C File Offset: 0x000A482C
			public FadeScreenRenderData(GUIBasicEffect iEffect)
			{
				this.mEffect = iEffect;
			}

			// Token: 0x06001939 RID: 6457 RVA: 0x000A663C File Offset: 0x000A483C
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(TutorialManager.FadeScreenRenderData.sVertexBuffer, 0, TutorialManager.FadeScreenRenderData.sVertexStride);
				this.mEffect.GraphicsDevice.VertexDeclaration = TutorialManager.FadeScreenRenderData.sVertexDeclaration;
				Matrix identity = Matrix.Identity;
				identity.M11 *= (float)screenSize.X;
				identity.M22 *= (float)screenSize.Y;
				this.mEffect.Transform = identity;
				this.mEffect.TextureEnabled = false;
				this.mEffect.VertexColorEnabled = true;
				this.mEffect.Color = new Vector4(1f, 1f, 1f, 0.5f * this.FadeAlpha);
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x17000646 RID: 1606
			// (get) Token: 0x0600193A RID: 6458 RVA: 0x000A6788 File Offset: 0x000A4988
			public int ZIndex
			{
				get
				{
					return 1;
				}
			}

			// Token: 0x04001B41 RID: 6977
			public float FadeAlpha;

			// Token: 0x04001B42 RID: 6978
			private static int sVertexStride = VertexPositionColor.SizeInBytes;

			// Token: 0x04001B43 RID: 6979
			private GUIBasicEffect mEffect;

			// Token: 0x04001B44 RID: 6980
			private static VertexBuffer sVertexBuffer;

			// Token: 0x04001B45 RID: 6981
			private static VertexDeclaration sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);
		}

		// Token: 0x0200033C RID: 828
		private class HintRenderData : IRenderableGUIObject
		{
			// Token: 0x0600193B RID: 6459 RVA: 0x000A678C File Offset: 0x000A498C
			public HintRenderData(GUIBasicEffect iEffect, BitmapFont iFont, TextBoxEffect iBoxEffect, IndexBuffer iIB, VertexBuffer iVB, VertexDeclaration iVD, VertexBuffer iIconVertices, VertexDeclaration iIconDeclaration)
			{
				this.mIndexBuffer = iIB;
				this.mVertexBuffer = iVB;
				this.mVertexDeclaration = iVD;
				this.mBoxEffect = iBoxEffect;
				this.mEffect = iEffect;
				this.mColor = Vector4.One;
				this.mFont = iFont;
				this.mText = new Text(512, iFont, TextAlign.Left, false);
				this.mText.SetText("");
				this.HintPosition = TutorialManager.Position.BottomRight;
				this.mChar = new Text(1, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, true);
				this.mIconVertices = iIconVertices;
				this.mIconDeclaration = iIconDeclaration;
			}

			// Token: 0x0600193C RID: 6460 RVA: 0x000A682C File Offset: 0x000A4A2C
			public void Draw(float iDeltaTime)
			{
				Vector2 size = this.mSize;
				if (this.Animation != TutorialManager.HintAnimations.None)
				{
					size.X += 128f;
				}
				Point screenSize = RenderManager.Instance.ScreenSize;
				if (this.mDirty)
				{
					this.mText.SetText(this.mString);
					this.mDirty = false;
				}
				TutorialManager.Position hintPosition = this.HintPosition;
				float num;
				float num2;
				if (hintPosition != TutorialManager.Position.Top)
				{
					if (hintPosition != TutorialManager.Position.Center)
					{
						if (hintPosition != TutorialManager.Position.BottomRight)
						{
							num = 0f;
							num2 = 0f;
						}
						else
						{
							num = (float)Math.Floor((double)((float)screenSize.X - (float)screenSize.X * 0.05f - size.X * 0.5f));
							num2 = (float)Math.Floor((double)((float)screenSize.Y - (float)screenSize.Y * 0.05f - size.Y * 0.5f));
						}
					}
					else
					{
						num = (float)Math.Floor((double)((float)screenSize.X - (float)screenSize.X * 0.5f));
						num2 = (float)Math.Floor((double)((float)screenSize.Y - (float)screenSize.Y * 0.3f));
					}
				}
				else
				{
					num = (float)Math.Floor((double)((float)screenSize.X - (float)screenSize.X * 0.5f));
					num2 = (float)Math.Floor((double)((float)screenSize.Y - (float)screenSize.Y * 0.9f));
				}
				this.mBoxEffect.ScreenSize = new Vector2((float)screenSize.X, (float)screenSize.Y);
				this.mColor.W = this.Alpha * 0.5f;
				this.mBoxEffect.Color = this.mColor;
				this.mBoxEffect.Size = size;
				this.mBoxEffect.Scale = 1f;
				this.mBoxEffect.Position = new Vector2(num, num2);
				this.mBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				this.mBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				this.mBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mBoxEffect.Begin();
				this.mBoxEffect.CurrentTechnique.Passes[0].Begin();
				this.mBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
				this.mBoxEffect.CurrentTechnique.Passes[0].End();
				this.mBoxEffect.End();
				this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mColor.W = this.Alpha;
				this.mEffect.Color = this.mColor;
				this.mEffect.Begin();
				this.mEffect.CurrentTechnique.Passes[0].Begin();
				Vector2 vector = default(Vector2);
				vector.X = (float)Math.Floor((double)(num + size.X * 0.5f)) - 64f;
				vector.Y = num2;
				if (this.GamePad)
				{
					this.RenderIconGamePad(ref vector);
				}
				else
				{
					this.RenderIconKeyBoard(ref vector);
				}
				this.mBoxEffect.Color = this.mColor;
				this.mEffect.TextureOffset = default(Vector2);
				this.mEffect.TextureScale = new Vector2(1f);
				this.mText.Draw(this.mEffect, num - size.X * 0.5f, num2 - size.Y * 0.5f);
				this.mEffect.CurrentTechnique.Passes[0].End();
				this.mEffect.End();
			}

			// Token: 0x0600193D RID: 6461 RVA: 0x000A6BFC File Offset: 0x000A4DFC
			private void RenderIconKeyBoard(ref Vector2 iPos)
			{
				Texture2D texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/Tutorial");
				this.mEffect.Texture = texture2D;
				this.mEffect.TextureEnabled = true;
				this.mEffect.OverlayTextureEnabled = false;
				Vector2 vector = new Vector2(1f / (float)texture2D.Width, 1f / (float)texture2D.Height);
				Matrix matrix = default(Matrix);
				matrix.M41 = iPos.X;
				matrix.M42 = iPos.Y;
				matrix.M44 = 1f;
				Vector2 textureOffset = default(Vector2);
				Vector2 textureScale = default(Vector2);
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
				this.mEffect.VertexColorEnabled = false;
				float num = 0f;
				int i = 0;
				while (i < 17)
				{
					TutorialManager.HintAnimations hintAnimations = this.Animation & (TutorialManager.HintAnimations)(1 << i);
					if (hintAnimations <= TutorialManager.HintAnimations.ConjureArcane)
					{
						if (hintAnimations <= TutorialManager.HintAnimations.ConjureLife)
						{
							switch (hintAnimations)
							{
							case TutorialManager.HintAnimations.Interact:
								num += 1f;
								break;
							case TutorialManager.HintAnimations.NavMagick:
								num += 1.5f;
								break;
							case TutorialManager.HintAnimations.Interact | TutorialManager.HintAnimations.NavMagick:
								break;
							case TutorialManager.HintAnimations.Conjure:
								num += 1.5f;
								break;
							default:
								if (hintAnimations == TutorialManager.HintAnimations.ConjureWater || hintAnimations == TutorialManager.HintAnimations.ConjureLife)
								{
									goto IL_20D;
								}
								break;
							}
						}
						else if (hintAnimations <= TutorialManager.HintAnimations.ConjureCold)
						{
							if (hintAnimations == TutorialManager.HintAnimations.ConjureShield || hintAnimations == TutorialManager.HintAnimations.ConjureCold)
							{
								goto IL_20D;
							}
						}
						else if (hintAnimations == TutorialManager.HintAnimations.ConjureLightning || hintAnimations == TutorialManager.HintAnimations.ConjureArcane)
						{
							goto IL_20D;
						}
					}
					else
					{
						if (hintAnimations <= TutorialManager.HintAnimations.CastArea)
						{
							if (hintAnimations <= TutorialManager.HintAnimations.ConjureFire)
							{
								if (hintAnimations != TutorialManager.HintAnimations.ConjureEarth && hintAnimations != TutorialManager.HintAnimations.ConjureFire)
								{
									goto IL_23B;
								}
								goto IL_20D;
							}
							else if (hintAnimations != TutorialManager.HintAnimations.CastForce && hintAnimations != TutorialManager.HintAnimations.CastArea)
							{
								goto IL_23B;
							}
						}
						else if (hintAnimations <= TutorialManager.HintAnimations.CastMagick)
						{
							if (hintAnimations != TutorialManager.HintAnimations.CastSelf && hintAnimations != TutorialManager.HintAnimations.CastMagick)
							{
								goto IL_23B;
							}
						}
						else
						{
							if (hintAnimations == TutorialManager.HintAnimations.Block)
							{
								num += 1f;
								goto IL_23B;
							}
							if (hintAnimations != TutorialManager.HintAnimations.Attack)
							{
								goto IL_23B;
							}
							num += 1.5f;
							goto IL_23B;
						}
						num += 1.5f;
					}
					IL_23B:
					i++;
					continue;
					IL_20D:
					num += 2f;
					goto IL_23B;
				}
				num = this.Time % num;
				for (int j = 0; j < 17; j++)
				{
					if (num >= 0f)
					{
						TutorialManager.HintAnimations hintAnimations2 = this.Animation & (TutorialManager.HintAnimations)(1 << j);
						if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureArcane)
						{
							if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureLife)
							{
								switch (hintAnimations2)
								{
								case TutorialManager.HintAnimations.Interact:
									if (num < 1f)
									{
										matrix.M11 = 80f;
										matrix.M22 = 64f;
										textureScale.X = 80f * vector.X;
										textureScale.Y = 64f * vector.Y;
										if (num < 0.5f)
										{
											textureOffset.X = 432f * vector.X;
											textureOffset.Y = 0f * vector.Y;
										}
										else
										{
											textureOffset.X = 432f * vector.X;
											textureOffset.Y = 64f * vector.Y;
										}
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1f;
									goto IL_1483;
								case TutorialManager.HintAnimations.NavMagick:
									if (num < 1.5f)
									{
										matrix.M11 = 80f;
										matrix.M22 = 64f;
										textureScale.X = 80f * vector.X;
										textureScale.Y = 64f * vector.Y;
										textureOffset.X = 352f * vector.X;
										textureOffset.Y = 192f * vector.Y;
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1.5f;
									goto IL_1483;
								case TutorialManager.HintAnimations.Interact | TutorialManager.HintAnimations.NavMagick:
									goto IL_1483;
								case TutorialManager.HintAnimations.Conjure:
									if (num < 1.5f)
									{
										matrix.M11 = 48f;
										matrix.M22 = 48f;
										textureScale.X = 48f * vector.X;
										textureScale.Y = 48f * vector.Y;
										textureOffset.X = 384f * vector.X;
										textureOffset.Y = 96f * vector.Y;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										Matrix transform = matrix;
										transform.M41 -= 32f;
										this.mEffect.Transform = transform;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
										transform.M41 += 32f;
										this.mEffect.Transform = transform;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
										transform.M41 += 32f;
										this.mEffect.Transform = transform;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
										this.mEffect.Color = new Vector4(0f, 0f, 0f, 1f);
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Lightning)[0];
										this.mChar.MarkAsDirty();
										this.mChar.Draw(this.mEffect, iPos.X - 32f, iPos.Y - (float)(this.mChar.Font.LineHeight / 2));
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Arcane)[0];
										this.mChar.MarkAsDirty();
										this.mChar.Draw(this.mEffect, iPos.X, iPos.Y - (float)(this.mChar.Font.LineHeight / 2));
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Fire)[0];
										this.mChar.MarkAsDirty();
										this.mChar.Draw(this.mEffect, iPos.X + 32f, iPos.Y - (float)(this.mChar.Font.LineHeight / 2));
										this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
										this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
										this.mEffect.Color = new Vector4(1f);
									}
									num -= 1.5f;
									goto IL_1483;
								default:
									if (hintAnimations2 != TutorialManager.HintAnimations.ConjureWater && hintAnimations2 != TutorialManager.HintAnimations.ConjureLife)
									{
										goto IL_1483;
									}
									break;
								}
							}
							else if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureCold)
							{
								if (hintAnimations2 != TutorialManager.HintAnimations.ConjureShield && hintAnimations2 != TutorialManager.HintAnimations.ConjureCold)
								{
									goto IL_1483;
								}
							}
							else if (hintAnimations2 != TutorialManager.HintAnimations.ConjureLightning && hintAnimations2 != TutorialManager.HintAnimations.ConjureArcane)
							{
								goto IL_1483;
							}
						}
						else if (hintAnimations2 <= TutorialManager.HintAnimations.CastArea)
						{
							if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureFire)
							{
								if (hintAnimations2 != TutorialManager.HintAnimations.ConjureEarth && hintAnimations2 != TutorialManager.HintAnimations.ConjureFire)
								{
									goto IL_1483;
								}
							}
							else
							{
								if (hintAnimations2 == TutorialManager.HintAnimations.CastForce)
								{
									if (num < 1.5f)
									{
										matrix.M11 = 80f;
										matrix.M22 = 64f;
										textureScale.X = 80f * vector.X;
										textureScale.Y = 64f * vector.Y;
										if (num < 1f)
										{
											textureOffset.X = 432f * vector.X;
											textureOffset.Y = 0f * vector.Y;
										}
										else
										{
											textureOffset.X = 432f * vector.X;
											textureOffset.Y = 192f * vector.Y;
										}
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1.5f;
									goto IL_1483;
								}
								if (hintAnimations2 != TutorialManager.HintAnimations.CastArea)
								{
									goto IL_1483;
								}
								if (num < 1.5f)
								{
									Matrix transform2 = matrix;
									transform2.M41 -= 48f;
									transform2.M11 = 64f;
									transform2.M22 = 48f;
									textureScale.X = 64f * vector.X;
									textureScale.Y = 48f * vector.Y;
									if (num < 0.5f)
									{
										textureOffset.X = 368f * vector.X;
										textureOffset.Y = 0f * vector.Y;
									}
									else
									{
										textureOffset.X = 368f * vector.X;
										textureOffset.Y = 48f * vector.Y;
									}
									this.mEffect.Transform = transform2;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									transform2.M41 += 36f;
									transform2.M11 = 32f;
									transform2.M22 = 32f;
									textureScale.X = 32f * vector.X;
									textureScale.Y = 32f * vector.Y;
									textureOffset.X = 192f * vector.X;
									textureOffset.Y = 0f * vector.Y;
									this.mEffect.Transform = transform2;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									transform2.M41 += 44f;
									transform2.M11 = 80f;
									transform2.M22 = 64f;
									textureScale.X = 80f * vector.X;
									textureScale.Y = 64f * vector.Y;
									if (num < 1f)
									{
										textureOffset.X = 432f * vector.X;
										textureOffset.Y = 0f * vector.Y;
									}
									else
									{
										textureOffset.X = 432f * vector.X;
										textureOffset.Y = 192f * vector.Y;
									}
									this.mEffect.Transform = transform2;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1.5f;
								goto IL_1483;
							}
						}
						else if (hintAnimations2 <= TutorialManager.HintAnimations.CastMagick)
						{
							if (hintAnimations2 == TutorialManager.HintAnimations.CastSelf)
							{
								if (num < 1.5f)
								{
									matrix.M11 = 80f;
									matrix.M22 = 64f;
									textureScale.X = 80f * vector.X;
									textureScale.Y = 64f * vector.Y;
									if (num < 1f)
									{
										textureOffset.X = 432f * vector.X;
										textureOffset.Y = 0f * vector.Y;
									}
									else
									{
										textureOffset.X = 432f * vector.X;
										textureOffset.Y = 128f * vector.Y;
									}
									this.mEffect.Transform = matrix;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1.5f;
								goto IL_1483;
							}
							if (hintAnimations2 != TutorialManager.HintAnimations.CastMagick)
							{
								goto IL_1483;
							}
							if (num < 1.5f)
							{
								matrix.M11 = 128f;
								matrix.M22 = 48f;
								textureScale.X = 128f * vector.X;
								textureScale.Y = 48f * vector.Y;
								if (num < 1f)
								{
									textureOffset.X = 240f * vector.X;
									textureOffset.Y = 0f * vector.Y;
								}
								else
								{
									textureOffset.X = 240f * vector.X;
									textureOffset.Y = 48f * vector.Y;
								}
								this.mEffect.Transform = matrix;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							}
							num -= 1.5f;
							goto IL_1483;
						}
						else
						{
							if (hintAnimations2 == TutorialManager.HintAnimations.Block)
							{
								if (num < 1f)
								{
									matrix.M11 = 72f;
									matrix.M22 = 48f;
									textureScale.X = 72f * vector.X;
									textureScale.Y = 48f * vector.Y;
									if (num < 0.5f)
									{
										textureOffset.X = 368f * vector.X;
										textureOffset.Y = 144f * vector.Y;
									}
									else
									{
										textureOffset.X = 304f * vector.X;
										textureOffset.Y = 144f * vector.Y;
									}
									this.mEffect.Transform = matrix;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1f;
								goto IL_1483;
							}
							if (hintAnimations2 != TutorialManager.HintAnimations.Attack)
							{
								goto IL_1483;
							}
							if (num < 1.5f)
							{
								Matrix transform3 = matrix;
								transform3.M41 -= 48f;
								transform3.M11 = 64f;
								transform3.M22 = 48f;
								textureScale.X = 64f * vector.X;
								textureScale.Y = 48f * vector.Y;
								if (num < 0.5f)
								{
									textureOffset.X = 368f * vector.X;
									textureOffset.Y = 0f * vector.Y;
								}
								else
								{
									textureOffset.X = 368f * vector.X;
									textureOffset.Y = 48f * vector.Y;
								}
								this.mEffect.Transform = transform3;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								transform3.M41 += 36f;
								transform3.M11 = 32f;
								transform3.M22 = 32f;
								textureScale.X = 32f * vector.X;
								textureScale.Y = 32f * vector.Y;
								textureOffset.X = 192f * vector.X;
								textureOffset.Y = 0f * vector.Y;
								this.mEffect.Transform = transform3;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								transform3.M41 += 44f;
								transform3.M11 = 80f;
								transform3.M22 = 64f;
								textureScale.X = 80f * vector.X;
								textureScale.Y = 64f * vector.Y;
								if (num < 1f)
								{
									textureOffset.X = 432f * vector.X;
									textureOffset.Y = 0f * vector.Y;
								}
								else
								{
									textureOffset.X = 432f * vector.X;
									textureOffset.Y = 64f * vector.Y;
								}
								this.mEffect.Transform = transform3;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							}
							num -= 1.5f;
							goto IL_1483;
						}
						if (num < 2f)
						{
							matrix.M11 = 48f;
							matrix.M22 = 48f;
							textureScale.X = 48f * vector.X;
							textureScale.Y = 48f * vector.Y;
							if (num < 1.333f)
							{
								textureOffset.X = 384f * vector.X;
								textureOffset.Y = 96f * vector.Y;
							}
							else
							{
								textureOffset.X = 336f * vector.X;
								textureOffset.Y = 96f * vector.Y;
							}
							this.mEffect.Transform = matrix;
							this.mEffect.TextureScale = textureScale;
							this.mEffect.TextureOffset = textureOffset;
							this.mEffect.CommitChanges();
							this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							this.mEffect.Color = new Vector4(0f, 0f, 0f, 1f);
							TutorialManager.HintAnimations hintAnimations3 = (TutorialManager.HintAnimations)(1 << j);
							if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureCold)
							{
								if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureLife)
								{
									if (hintAnimations3 != TutorialManager.HintAnimations.ConjureWater)
									{
										if (hintAnimations3 == TutorialManager.HintAnimations.ConjureLife)
										{
											this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Life)[0];
										}
									}
									else
									{
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Water)[0];
									}
								}
								else if (hintAnimations3 != TutorialManager.HintAnimations.ConjureShield)
								{
									if (hintAnimations3 == TutorialManager.HintAnimations.ConjureCold)
									{
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Cold)[0];
									}
								}
								else
								{
									this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Shield)[0];
								}
							}
							else if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureArcane)
							{
								if (hintAnimations3 != TutorialManager.HintAnimations.ConjureLightning)
								{
									if (hintAnimations3 == TutorialManager.HintAnimations.ConjureArcane)
									{
										this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Arcane)[0];
									}
								}
								else
								{
									this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Lightning)[0];
								}
							}
							else if (hintAnimations3 != TutorialManager.HintAnimations.ConjureEarth)
							{
								if (hintAnimations3 == TutorialManager.HintAnimations.ConjureFire)
								{
									this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Fire)[0];
								}
							}
							else
							{
								this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Earth)[0];
							}
							this.mChar.MarkAsDirty();
							this.mChar.Draw(this.mEffect, iPos.X, iPos.Y - (float)(this.mChar.Font.LineHeight / 2));
							this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
							this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
							this.mEffect.Color = new Vector4(1f);
						}
						num -= 2f;
					}
					IL_1483:;
				}
			}

			// Token: 0x0600193E RID: 6462 RVA: 0x000A809C File Offset: 0x000A629C
			private void RenderIconGamePad(ref Vector2 iPos)
			{
				Texture2D texture2D = Game.Instance.Content.Load<Texture2D>("UI/HUD/Tutorial");
				this.mEffect.Texture = texture2D;
				this.mEffect.TextureEnabled = true;
				this.mEffect.OverlayTextureEnabled = false;
				Vector2 vector = new Vector2(1f / (float)texture2D.Width, 1f / (float)texture2D.Height);
				Matrix matrix = default(Matrix);
				matrix.M41 = iPos.X;
				matrix.M42 = iPos.Y;
				matrix.M44 = 1f;
				Vector2 textureOffset = default(Vector2);
				Vector2 textureScale = default(Vector2);
				this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
				this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
				this.mEffect.VertexColorEnabled = false;
				float num = 0f;
				int i = 0;
				while (i < 17)
				{
					TutorialManager.HintAnimations hintAnimations = this.Animation & (TutorialManager.HintAnimations)(1 << i);
					if (hintAnimations <= TutorialManager.HintAnimations.ConjureArcane)
					{
						if (hintAnimations <= TutorialManager.HintAnimations.ConjureLife)
						{
							switch (hintAnimations)
							{
							case TutorialManager.HintAnimations.Interact:
								num += 1f;
								break;
							case TutorialManager.HintAnimations.NavMagick:
								num += 1.5f;
								break;
							case TutorialManager.HintAnimations.Interact | TutorialManager.HintAnimations.NavMagick:
								break;
							case TutorialManager.HintAnimations.Conjure:
								num += 2f;
								break;
							default:
								if (hintAnimations == TutorialManager.HintAnimations.ConjureWater || hintAnimations == TutorialManager.HintAnimations.ConjureLife)
								{
									goto IL_20A;
								}
								break;
							}
						}
						else if (hintAnimations <= TutorialManager.HintAnimations.ConjureCold)
						{
							if (hintAnimations == TutorialManager.HintAnimations.ConjureShield || hintAnimations == TutorialManager.HintAnimations.ConjureCold)
							{
								goto IL_20A;
							}
						}
						else if (hintAnimations == TutorialManager.HintAnimations.ConjureLightning || hintAnimations == TutorialManager.HintAnimations.ConjureArcane)
						{
							goto IL_20A;
						}
					}
					else
					{
						if (hintAnimations <= TutorialManager.HintAnimations.CastArea)
						{
							if (hintAnimations <= TutorialManager.HintAnimations.ConjureFire)
							{
								if (hintAnimations != TutorialManager.HintAnimations.ConjureEarth && hintAnimations != TutorialManager.HintAnimations.ConjureFire)
								{
									goto IL_22C;
								}
								goto IL_20A;
							}
							else if (hintAnimations != TutorialManager.HintAnimations.CastForce && hintAnimations != TutorialManager.HintAnimations.CastArea)
							{
								goto IL_22C;
							}
						}
						else if (hintAnimations <= TutorialManager.HintAnimations.CastMagick)
						{
							if (hintAnimations != TutorialManager.HintAnimations.CastSelf && hintAnimations != TutorialManager.HintAnimations.CastMagick)
							{
								goto IL_22C;
							}
						}
						else
						{
							if (hintAnimations != TutorialManager.HintAnimations.Block && hintAnimations != TutorialManager.HintAnimations.Attack)
							{
								goto IL_22C;
							}
							num += 1f;
							goto IL_22C;
						}
						num += 1.5f;
					}
					IL_22C:
					i++;
					continue;
					IL_20A:
					num += 5f;
					goto IL_22C;
				}
				num = this.Time % num;
				for (int j = 0; j < 17; j++)
				{
					if (num >= 0f)
					{
						TutorialManager.HintAnimations hintAnimations2 = this.Animation & (TutorialManager.HintAnimations)(1 << j);
						if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureArcane)
						{
							if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureLife)
							{
								switch (hintAnimations2)
								{
								case TutorialManager.HintAnimations.Interact:
									if (num < 1f)
									{
										matrix.M11 = 64f;
										matrix.M22 = 64f;
										textureScale.X = 64f * vector.X;
										textureScale.Y = 64f * vector.Y;
										if (num < 0.5f)
										{
											textureOffset.X = 0f * vector.X;
											textureOffset.Y = 64f * vector.Y;
										}
										else
										{
											textureOffset.X = 0f * vector.X;
											textureOffset.Y = 192f * vector.Y;
										}
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1f;
									goto IL_14D9;
								case TutorialManager.HintAnimations.NavMagick:
									if (num < 1.5f)
									{
										matrix.M11 = 64f;
										matrix.M22 = 64f;
										textureScale.X = 64f * vector.X;
										textureScale.Y = 64f * vector.Y;
										if (num < 0.5f)
										{
											textureOffset.X = 64f * vector.X;
											textureOffset.Y = 64f * vector.Y;
										}
										else
										{
											textureOffset.X = 128f * vector.X;
											textureOffset.Y = 64f * vector.Y;
										}
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1.5f;
									goto IL_14D9;
								case TutorialManager.HintAnimations.Interact | TutorialManager.HintAnimations.NavMagick:
									goto IL_14D9;
								case TutorialManager.HintAnimations.Conjure:
									if (num < 2f)
									{
										matrix.M11 = 64f;
										matrix.M22 = 64f;
										textureScale.X = 64f * vector.X;
										textureScale.Y = 64f * vector.Y;
										textureOffset.X = 64f * vector.X;
										textureOffset.Y = 0f * vector.Y;
										this.mEffect.Transform = matrix;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 2f;
									goto IL_14D9;
								default:
									if (hintAnimations2 != TutorialManager.HintAnimations.ConjureWater && hintAnimations2 != TutorialManager.HintAnimations.ConjureLife)
									{
										goto IL_14D9;
									}
									break;
								}
							}
							else if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureCold)
							{
								if (hintAnimations2 != TutorialManager.HintAnimations.ConjureShield && hintAnimations2 != TutorialManager.HintAnimations.ConjureCold)
								{
									goto IL_14D9;
								}
							}
							else if (hintAnimations2 != TutorialManager.HintAnimations.ConjureLightning && hintAnimations2 != TutorialManager.HintAnimations.ConjureArcane)
							{
								goto IL_14D9;
							}
						}
						else if (hintAnimations2 <= TutorialManager.HintAnimations.CastArea)
						{
							if (hintAnimations2 <= TutorialManager.HintAnimations.ConjureFire)
							{
								if (hintAnimations2 != TutorialManager.HintAnimations.ConjureEarth && hintAnimations2 != TutorialManager.HintAnimations.ConjureFire)
								{
									goto IL_14D9;
								}
							}
							else
							{
								if (hintAnimations2 == TutorialManager.HintAnimations.CastForce)
								{
									if (num < 1.5f)
									{
										Matrix transform = matrix;
										transform.M42 -= 16f;
										transform.M11 = 64f;
										transform.M22 = 36f;
										textureScale.X = 64f * vector.X;
										textureScale.Y = 36f * vector.Y;
										textureOffset.X = 192f * vector.X;
										if (num < 0.5f)
										{
											textureOffset.Y = 128f * vector.Y;
										}
										else
										{
											textureOffset.Y = 192f * vector.Y;
										}
										this.mEffect.Transform = transform;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
										transform.M22 = 28f;
										transform.M42 += 32f;
										textureScale.Y = 28f * vector.Y;
										textureOffset.Y = 164f * vector.Y;
										this.mEffect.Transform = transform;
										this.mEffect.TextureScale = textureScale;
										this.mEffect.TextureOffset = textureOffset;
										this.mEffect.CommitChanges();
										this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									}
									num -= 1.5f;
									goto IL_14D9;
								}
								if (hintAnimations2 != TutorialManager.HintAnimations.CastArea)
								{
									goto IL_14D9;
								}
								if (num < 1.5f)
								{
									Matrix transform2 = matrix;
									transform2.M42 -= 16f;
									transform2.M11 = 64f;
									transform2.M22 = 36f;
									textureScale.X = 64f * vector.X;
									textureScale.Y = 36f * vector.Y;
									textureOffset.X = 128f * vector.X;
									if (num < 0.5f)
									{
										textureOffset.Y = 128f * vector.Y;
									}
									else
									{
										textureOffset.Y = 192f * vector.Y;
									}
									this.mEffect.Transform = transform2;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									transform2.M22 = 28f;
									transform2.M42 += 32f;
									textureScale.Y = 28f * vector.Y;
									textureOffset.Y = 164f * vector.Y;
									this.mEffect.Transform = transform2;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1.5f;
								goto IL_14D9;
							}
						}
						else if (hintAnimations2 <= TutorialManager.HintAnimations.CastMagick)
						{
							if (hintAnimations2 == TutorialManager.HintAnimations.CastSelf)
							{
								if (num < 1.5f)
								{
									matrix.M11 = 64f;
									matrix.M22 = 64f;
									textureScale.X = 64f * vector.X;
									textureScale.Y = 64f * vector.Y;
									if (num < 0.5f)
									{
										textureOffset.X = 0f * vector.X;
										textureOffset.Y = 64f * vector.Y;
									}
									else
									{
										textureOffset.X = 64f * vector.X;
										textureOffset.Y = 192f * vector.Y;
									}
									this.mEffect.Transform = matrix;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1.5f;
								goto IL_14D9;
							}
							if (hintAnimations2 != TutorialManager.HintAnimations.CastMagick)
							{
								goto IL_14D9;
							}
							if (num < 1.5f)
							{
								matrix.M11 = 64f;
								matrix.M22 = 64f;
								textureScale.X = 64f * vector.X;
								textureScale.Y = 64f * vector.Y;
								if (num < 0.5f)
								{
									textureOffset.X = 0f * vector.X;
									textureOffset.Y = 64f * vector.Y;
								}
								else
								{
									textureOffset.X = 64f * vector.X;
									textureOffset.Y = 128f * vector.Y;
								}
								this.mEffect.Transform = matrix;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							}
							num -= 1.5f;
							goto IL_14D9;
						}
						else
						{
							if (hintAnimations2 == TutorialManager.HintAnimations.Block)
							{
								if (num < 1f)
								{
									Matrix transform3 = matrix;
									transform3.M42 += 16f;
									transform3.M11 = 64f;
									transform3.M22 = 28f;
									textureScale.X = 64f * vector.X;
									textureScale.Y = 28f * vector.Y;
									textureOffset.X = 192f * vector.X;
									if (num < 0.5f)
									{
										textureOffset.Y = 164f * vector.Y;
									}
									else
									{
										textureOffset.Y = 228f * vector.Y;
									}
									this.mEffect.Transform = transform3;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
									transform3.M22 = 36f;
									transform3.M42 -= 32f;
									textureScale.Y = 36f * vector.Y;
									textureOffset.Y = 128f * vector.Y;
									this.mEffect.Transform = transform3;
									this.mEffect.TextureScale = textureScale;
									this.mEffect.TextureOffset = textureOffset;
									this.mEffect.CommitChanges();
									this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
								}
								num -= 1f;
								goto IL_14D9;
							}
							if (hintAnimations2 != TutorialManager.HintAnimations.Attack)
							{
								goto IL_14D9;
							}
							if (num < 1f)
							{
								matrix.M11 = 64f;
								matrix.M22 = 64f;
								textureScale.X = 64f * vector.X;
								textureScale.Y = 64f * vector.Y;
								if (num < 0.5f)
								{
									textureOffset.X = 0f * vector.X;
									textureOffset.Y = 64f * vector.Y;
								}
								else if (num < 1f)
								{
									textureOffset.X = 0f * vector.X;
									textureOffset.Y = 128f * vector.Y;
								}
								this.mEffect.Transform = matrix;
								this.mEffect.TextureScale = textureScale;
								this.mEffect.TextureOffset = textureOffset;
								this.mEffect.CommitChanges();
								this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							}
							num -= 1f;
							goto IL_14D9;
						}
						if (num < 5f)
						{
							Matrix transform4 = matrix;
							textureScale.X = 64f * vector.X;
							textureScale.Y = 64f * vector.Y;
							textureOffset.X = 128f * vector.X;
							textureOffset.Y = 0f * vector.Y;
							transform4.M11 = 64f;
							transform4.M22 = 64f;
							this.mEffect.Transform = transform4;
							this.mEffect.TextureScale = textureScale;
							this.mEffect.TextureOffset = textureOffset;
							this.mEffect.CommitChanges();
							this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
							textureOffset.X = 64f * vector.X;
							textureOffset.Y = 0f * vector.Y;
							if (num > 1f)
							{
								if (num < 2f)
								{
									TutorialManager.HintAnimations hintAnimations3 = (TutorialManager.HintAnimations)(1 << j);
									if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureCold)
									{
										if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureLife)
										{
											if (hintAnimations3 == TutorialManager.HintAnimations.ConjureWater)
											{
												goto IL_7D9;
											}
											if (hintAnimations3 != TutorialManager.HintAnimations.ConjureLife)
											{
												goto IL_D02;
											}
											goto IL_797;
										}
										else
										{
											if (hintAnimations3 == TutorialManager.HintAnimations.ConjureShield)
											{
												goto IL_7B8;
											}
											if (hintAnimations3 != TutorialManager.HintAnimations.ConjureCold)
											{
												goto IL_D02;
											}
										}
									}
									else if (hintAnimations3 <= TutorialManager.HintAnimations.ConjureArcane)
									{
										if (hintAnimations3 == TutorialManager.HintAnimations.ConjureLightning)
										{
											goto IL_7D9;
										}
										if (hintAnimations3 != TutorialManager.HintAnimations.ConjureArcane)
										{
											goto IL_D02;
										}
										goto IL_797;
									}
									else
									{
										if (hintAnimations3 == TutorialManager.HintAnimations.ConjureEarth)
										{
											goto IL_7B8;
										}
										if (hintAnimations3 != TutorialManager.HintAnimations.ConjureFire)
										{
											goto IL_D02;
										}
									}
									transform4.M41 += (num - 1f) * 18f;
									goto IL_D02;
									IL_7D9:
									transform4.M41 -= (num - 1f) * 18f;
									goto IL_D02;
									IL_7B8:
									transform4.M42 += (num - 1f) * 18f;
									goto IL_D02;
									IL_797:
									transform4.M42 -= (num - 1f) * 18f;
								}
								else if (num < 3f)
								{
									TutorialManager.HintAnimations hintAnimations4 = (TutorialManager.HintAnimations)(1 << j);
									if (hintAnimations4 <= TutorialManager.HintAnimations.ConjureCold)
									{
										if (hintAnimations4 <= TutorialManager.HintAnimations.ConjureLife)
										{
											if (hintAnimations4 == TutorialManager.HintAnimations.ConjureWater)
											{
												goto IL_8CA;
											}
											if (hintAnimations4 != TutorialManager.HintAnimations.ConjureLife)
											{
												goto IL_D02;
											}
										}
										else
										{
											if (hintAnimations4 == TutorialManager.HintAnimations.ConjureShield)
											{
												goto IL_8B2;
											}
											if (hintAnimations4 != TutorialManager.HintAnimations.ConjureCold)
											{
												goto IL_D02;
											}
											goto IL_8E2;
										}
									}
									else if (hintAnimations4 <= TutorialManager.HintAnimations.ConjureArcane)
									{
										if (hintAnimations4 == TutorialManager.HintAnimations.ConjureLightning)
										{
											goto IL_8CA;
										}
										if (hintAnimations4 != TutorialManager.HintAnimations.ConjureArcane)
										{
											goto IL_D02;
										}
									}
									else
									{
										if (hintAnimations4 == TutorialManager.HintAnimations.ConjureEarth)
										{
											goto IL_8B2;
										}
										if (hintAnimations4 != TutorialManager.HintAnimations.ConjureFire)
										{
											goto IL_D02;
										}
										goto IL_8E2;
									}
									transform4.M42 -= 18f;
									goto IL_D02;
									IL_8B2:
									transform4.M42 += 18f;
									goto IL_D02;
									IL_8CA:
									transform4.M41 -= 18f;
									goto IL_D02;
									IL_8E2:
									transform4.M41 += 18f;
								}
								else if (num < 4f)
								{
									TutorialManager.HintAnimations hintAnimations5 = (TutorialManager.HintAnimations)(1 << j);
									if (hintAnimations5 <= TutorialManager.HintAnimations.ConjureCold)
									{
										if (hintAnimations5 <= TutorialManager.HintAnimations.ConjureLife)
										{
											if (hintAnimations5 != TutorialManager.HintAnimations.ConjureWater)
											{
												if (hintAnimations5 == TutorialManager.HintAnimations.ConjureLife)
												{
													transform4.M41 -= (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
													transform4.M42 -= (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
												}
											}
											else
											{
												transform4.M41 -= (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
												transform4.M42 -= (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
											}
										}
										else if (hintAnimations5 != TutorialManager.HintAnimations.ConjureShield)
										{
											if (hintAnimations5 == TutorialManager.HintAnimations.ConjureCold)
											{
												transform4.M41 += (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
												transform4.M42 -= (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
											}
										}
										else
										{
											transform4.M41 += (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
											transform4.M42 += (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
										}
									}
									else if (hintAnimations5 <= TutorialManager.HintAnimations.ConjureArcane)
									{
										if (hintAnimations5 != TutorialManager.HintAnimations.ConjureLightning)
										{
											if (hintAnimations5 == TutorialManager.HintAnimations.ConjureArcane)
											{
												transform4.M41 += (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
												transform4.M42 -= (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
											}
										}
										else
										{
											transform4.M41 -= (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
											transform4.M42 += (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
										}
									}
									else if (hintAnimations5 != TutorialManager.HintAnimations.ConjureEarth)
									{
										if (hintAnimations5 == TutorialManager.HintAnimations.ConjureFire)
										{
											transform4.M41 += (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
											transform4.M42 += (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
										}
									}
									else
									{
										transform4.M41 -= (float)Math.Sin((double)((num - 3f) * 1.5707964f)) * 18f;
										transform4.M42 += (float)Math.Cos((double)((num - 3f) * 1.5707964f)) * 18f;
									}
								}
								else if (num < 5f)
								{
									TutorialManager.HintAnimations hintAnimations6 = (TutorialManager.HintAnimations)(1 << j);
									if (hintAnimations6 <= TutorialManager.HintAnimations.ConjureCold)
									{
										if (hintAnimations6 <= TutorialManager.HintAnimations.ConjureLife)
										{
											if (hintAnimations6 != TutorialManager.HintAnimations.ConjureWater)
											{
												if (hintAnimations6 != TutorialManager.HintAnimations.ConjureLife)
												{
													goto IL_D02;
												}
												goto IL_CB0;
											}
										}
										else
										{
											if (hintAnimations6 == TutorialManager.HintAnimations.ConjureShield)
											{
												goto IL_CC5;
											}
											if (hintAnimations6 != TutorialManager.HintAnimations.ConjureCold)
											{
												goto IL_D02;
											}
										}
										transform4.M42 -= 18f;
										goto IL_D02;
									}
									if (hintAnimations6 <= TutorialManager.HintAnimations.ConjureArcane)
									{
										if (hintAnimations6 != TutorialManager.HintAnimations.ConjureLightning)
										{
											if (hintAnimations6 != TutorialManager.HintAnimations.ConjureArcane)
											{
												goto IL_D02;
											}
											goto IL_CC5;
										}
									}
									else
									{
										if (hintAnimations6 == TutorialManager.HintAnimations.ConjureEarth)
										{
											goto IL_CB0;
										}
										if (hintAnimations6 != TutorialManager.HintAnimations.ConjureFire)
										{
											goto IL_D02;
										}
									}
									transform4.M42 += 18f;
									goto IL_D02;
									IL_CB0:
									transform4.M41 -= 18f;
									goto IL_D02;
									IL_CC5:
									transform4.M41 += 18f;
								}
							}
							IL_D02:
							this.mEffect.Transform = transform4;
							this.mEffect.TextureScale = textureScale;
							this.mEffect.TextureOffset = textureOffset;
							this.mEffect.CommitChanges();
							this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
						}
						num -= 5f;
					}
					IL_14D9:;
				}
			}

			// Token: 0x17000647 RID: 1607
			// (get) Token: 0x0600193F RID: 6463 RVA: 0x000A9591 File Offset: 0x000A7791
			public int ZIndex
			{
				get
				{
					return 103;
				}
			}

			// Token: 0x06001940 RID: 6464 RVA: 0x000A9595 File Offset: 0x000A7795
			public void SetText(string iString)
			{
				this.mSize = this.mFont.MeasureText(iString, true);
				this.mString = iString;
				this.mDirty = true;
			}

			// Token: 0x04001B46 RID: 6982
			public TutorialManager.Position HintPosition;

			// Token: 0x04001B47 RID: 6983
			public float Alpha;

			// Token: 0x04001B48 RID: 6984
			public TutorialManager.HintAnimations Animation;

			// Token: 0x04001B49 RID: 6985
			public bool GamePad;

			// Token: 0x04001B4A RID: 6986
			public float Time;

			// Token: 0x04001B4B RID: 6987
			private string mString;

			// Token: 0x04001B4C RID: 6988
			private Text mText;

			// Token: 0x04001B4D RID: 6989
			private bool mDirty;

			// Token: 0x04001B4E RID: 6990
			private Vector4 mColor;

			// Token: 0x04001B4F RID: 6991
			private BitmapFont mFont;

			// Token: 0x04001B50 RID: 6992
			private Vector2 mSize;

			// Token: 0x04001B51 RID: 6993
			private GUIBasicEffect mEffect;

			// Token: 0x04001B52 RID: 6994
			private TextBoxEffect mBoxEffect;

			// Token: 0x04001B53 RID: 6995
			private IndexBuffer mIndexBuffer;

			// Token: 0x04001B54 RID: 6996
			private VertexBuffer mVertexBuffer;

			// Token: 0x04001B55 RID: 6997
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x04001B56 RID: 6998
			private VertexBuffer mIconVertices;

			// Token: 0x04001B57 RID: 6999
			private VertexDeclaration mIconDeclaration;

			// Token: 0x04001B58 RID: 7000
			private Text mChar;
		}

		// Token: 0x0200033D RID: 829
		private class DialogHintRenderData : IRenderableGUIObject
		{
			// Token: 0x06001941 RID: 6465 RVA: 0x000A95B8 File Offset: 0x000A77B8
			static DialogHintRenderData()
			{
				VertexPositionTexture[] array = new VertexPositionTexture[8];
				array[0].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y);
				array[0].Position = new Vector3(0f, 0f, 0f);
				array[1].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y);
				array[1].Position = new Vector3(TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, 0f, 0f);
				array[2].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y);
				array[2].Position = new Vector3(TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y, 0f);
				array[3].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y);
				array[3].Position = new Vector3(0f, TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y, 0f);
				array[4].TextureCoordinate = new Vector2(0f, 0f);
				array[4].Position = new Vector3(0f, 0f, 0f);
				array[5].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.MAGICK_SIZE.X, 0f);
				array[5].Position = new Vector3(TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, 0f, 0f);
				array[6].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.MAGICK_SIZE.X, TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y);
				array[6].Position = new Vector3(TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y, 0f);
				array[7].TextureCoordinate = new Vector2(0f, TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y);
				array[7].Position = new Vector3(0f, TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y, 0f);
				lock (Game.Instance.GraphicsDevice)
				{
					TutorialManager.DialogHintRenderData.IMAGE_VB = new VertexBuffer(Game.Instance.GraphicsDevice, array.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
					TutorialManager.DialogHintRenderData.IMAGE_VD = new VertexDeclaration(Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
					TutorialManager.DialogHintRenderData.IMAGE_VB.SetData<VertexPositionTexture>(array);
				}
			}

			// Token: 0x06001942 RID: 6466 RVA: 0x000A9910 File Offset: 0x000A7B10
			public DialogHintRenderData(GUIBasicEffect iEffect, BitmapFont iFont, TextBoxEffect iBoxEffect, IndexBuffer iIB, VertexBuffer iVB, VertexDeclaration iVD)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					this.mElementsTexture = Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
					this.mMagicksTexture = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
					this.mMagicksTexture2 = Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks_2");
				}
				this.mIndexBuffer = iIB;
				this.mVertexBuffer = iVB;
				this.mVertexDeclaration = iVD;
				this.mTextBoxEffect = iBoxEffect;
				this.mGUIBasicEffect = iEffect;
				this.mColor = Vector4.One;
				this.mFont = iFont;
				this.mText = new Text(512, iFont, TextAlign.Left, false);
				this.mText.SetText("");
			}

			// Token: 0x06001943 RID: 6467 RVA: 0x000A99F8 File Offset: 0x000A7BF8
			public void Draw(float iDeltaTime)
			{
				Point screenSize = RenderManager.Instance.ScreenSize;
				if (this.mDirty)
				{
					this.mText.SetText(this.mString);
					this.mDirty = false;
				}
				Vector2 position;
				position.X = (float)screenSize.X * 0.5f + 0.5f;
				position.Y = (float)screenSize.Y * 0.4f + 0.5f;
				Vector2 screenSize2 = new Vector2((float)screenSize.X, (float)screenSize.Y);
				this.mTextBoxEffect.Position = position;
				this.mTextBoxEffect.Size = this.mSize;
				this.mTextBoxEffect.Scale = 1f;
				this.mTextBoxEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
				this.mTextBoxEffect.ScreenSize = screenSize2;
				this.mTextBoxEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				this.mTextBoxEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				this.mTextBoxEffect.GraphicsDevice.Indices = this.mIndexBuffer;
				this.mTextBoxEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
				this.mTextBoxEffect.Begin();
				this.mTextBoxEffect.CurrentTechnique.Passes[0].Begin();
				this.mTextBoxEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 36, 0, 18);
				this.mTextBoxEffect.CurrentTechnique.Passes[0].End();
				this.mTextBoxEffect.End();
				this.mTextBoxEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
				this.mGUIBasicEffect.SetScreenSize(screenSize.X, screenSize.Y);
				this.mGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(TutorialManager.DialogHintRenderData.IMAGE_VB, 0, VertexPositionTexture.SizeInBytes);
				this.mGUIBasicEffect.GraphicsDevice.VertexDeclaration = TutorialManager.DialogHintRenderData.IMAGE_VD;
				this.mGUIBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
				this.mGUIBasicEffect.TextureOffset = this.mTextureOffset;
				this.mGUIBasicEffect.TextureScale = this.mTextureScale;
				this.mGUIBasicEffect.TextureEnabled = true;
				this.mGUIBasicEffect.VertexColorEnabled = false;
				this.mGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
				Matrix identity = Matrix.Identity;
				identity.M11 = this.mScale;
				identity.M22 = this.mScale;
				identity.M41 = position.X - this.mSize.X * 0.5f;
				identity.M42 = position.Y - this.mSize.Y * 0.5f;
				this.mGUIBasicEffect.Transform = identity;
				if (this.mElement != Elements.None)
				{
					this.mGUIBasicEffect.Texture = this.mElementsTexture;
				}
				else if (this.mMagickType != MagickType.None)
				{
					if (this.mMagickType >= MagickType.Shrink)
					{
						this.mGUIBasicEffect.Texture = this.mMagicksTexture2;
					}
					else
					{
						this.mGUIBasicEffect.Texture = this.mMagicksTexture;
					}
				}
				this.mGUIBasicEffect.Begin();
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
				if (this.mElement != Elements.None | this.mMagickType != MagickType.None)
				{
					this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, this.mStartVertex, 2);
				}
				this.mGUIBasicEffect.VertexColorEnabled = true;
				this.mText.Draw(this.mGUIBasicEffect, position.X - (this.mSize.X * 0.5f - this.mTextOffset * this.mScale), position.Y - this.mSize.Y * 0.5f);
				this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
				this.mGUIBasicEffect.End();
				this.mGUIBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			}

			// Token: 0x17000648 RID: 1608
			// (get) Token: 0x06001944 RID: 6468 RVA: 0x000A9E36 File Offset: 0x000A8036
			public int ZIndex
			{
				get
				{
					return 202;
				}
			}

			// Token: 0x06001945 RID: 6469 RVA: 0x000A9E40 File Offset: 0x000A8040
			public void SetInfo(MagickType iMagickType, string iString, float iScale, Vector2 iSize)
			{
				this.mScale = iScale;
				this.mSize = iSize;
				this.mStartVertex = 4;
				if (iMagickType != MagickType.None)
				{
					this.mTextOffset = TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X + TutorialManager.DialogHintRenderData.TEXT_PADDING;
					int num = (int)iMagickType;
					if (num >= 37)
					{
						num -= 37;
						this.mTextureScale.X = 1f;
						this.mTextureScale.Y = 4f;
					}
					else
					{
						this.mTextureScale.X = 1f;
						this.mTextureScale.Y = 1f;
					}
					this.mTextureOffset.X = (float)(num % 5) * TutorialManager.DialogHintRenderData.MAGICK_SIZE.X;
					this.mTextureOffset.Y = (float)(num / 5) * TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y;
				}
				else
				{
					this.mTextOffset = 0f;
				}
				this.mMagickType = iMagickType;
				this.mElement = Elements.None;
				this.mString = iString;
				this.mDirty = true;
			}

			// Token: 0x06001946 RID: 6470 RVA: 0x000A9F2C File Offset: 0x000A812C
			public void SetInfo(Elements iElement, string iString, float iScale, Vector2 iSize)
			{
				this.mTextureScale.X = 1f;
				this.mTextureScale.Y = 1f;
				this.mScale = iScale;
				this.mSize = iSize;
				this.mStartVertex = 0;
				if (iElement != Elements.None)
				{
					int num = Spell.ElementIndex(iElement);
					this.mTextureOffset.X = (float)(num % 5) * TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X;
					this.mTextureOffset.Y = (float)(num / 5) * TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y;
					this.mTextOffset = TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X + TutorialManager.DialogHintRenderData.TEXT_PADDING;
				}
				else
				{
					this.mTextOffset = 0f;
				}
				this.mMagickType = MagickType.None;
				this.mElement = iElement;
				this.mString = iString;
				this.mDirty = true;
			}

			// Token: 0x04001B59 RID: 7001
			public static readonly Vector2 MAGICK_PX_SIZE = new Vector2(400f, 250f);

			// Token: 0x04001B5A RID: 7002
			public static readonly Vector2 MAGICK_SIZE = new Vector2(0.1953125f, 0.12207031f);

			// Token: 0x04001B5B RID: 7003
			public static readonly Vector2 ELEMENT_PX_SIZE = new Vector2(50f, 50f);

			// Token: 0x04001B5C RID: 7004
			public static readonly Vector2 ELEMENT_OFFSET = new Vector2(0f, 0.3046875f);

			// Token: 0x04001B5D RID: 7005
			public static readonly Vector2 ELEMENT_SIZE = new Vector2(0.09765625f, 0.09765625f);

			// Token: 0x04001B5E RID: 7006
			private static readonly VertexBuffer IMAGE_VB;

			// Token: 0x04001B5F RID: 7007
			private static readonly VertexDeclaration IMAGE_VD;

			// Token: 0x04001B60 RID: 7008
			public float Alpha;

			// Token: 0x04001B61 RID: 7009
			private string mString;

			// Token: 0x04001B62 RID: 7010
			private Text mText;

			// Token: 0x04001B63 RID: 7011
			private bool mDirty;

			// Token: 0x04001B64 RID: 7012
			private Vector4 mColor;

			// Token: 0x04001B65 RID: 7013
			private BitmapFont mFont;

			// Token: 0x04001B66 RID: 7014
			private Vector2 mSize;

			// Token: 0x04001B67 RID: 7015
			private float mScale;

			// Token: 0x04001B68 RID: 7016
			private GUIBasicEffect mGUIBasicEffect;

			// Token: 0x04001B69 RID: 7017
			private TextBoxEffect mTextBoxEffect;

			// Token: 0x04001B6A RID: 7018
			private IndexBuffer mIndexBuffer;

			// Token: 0x04001B6B RID: 7019
			private VertexBuffer mVertexBuffer;

			// Token: 0x04001B6C RID: 7020
			private VertexDeclaration mVertexDeclaration;

			// Token: 0x04001B6D RID: 7021
			private Vector2 mTextureOffset;

			// Token: 0x04001B6E RID: 7022
			private Vector2 mTextureScale;

			// Token: 0x04001B6F RID: 7023
			private Elements mElement;

			// Token: 0x04001B70 RID: 7024
			private MagickType mMagickType;

			// Token: 0x04001B71 RID: 7025
			private Texture2D mElementsTexture;

			// Token: 0x04001B72 RID: 7026
			private Texture2D mMagicksTexture;

			// Token: 0x04001B73 RID: 7027
			private Texture2D mMagicksTexture2;

			// Token: 0x04001B74 RID: 7028
			private int mStartVertex;

			// Token: 0x04001B75 RID: 7029
			private float mTextOffset;

			// Token: 0x04001B76 RID: 7030
			private static readonly float TEXT_PADDING = 6f;
		}
	}
}
