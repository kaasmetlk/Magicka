// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.TutorialManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Graphics;

internal class TutorialManager
{
  private const int MAX_TIMES_TIP_SHOWN = 3;
  private const int FIRST_INDEX_OF_SECOND_MAGICKS_TEXTURE = 37;
  public const int DIALOG_TEXT_WIDTH = 300;
  public const int HINT_WIDTH = 300;
  public const MagickaFont FONT = MagickaFont.Maiandra14;
  public static string[] TipsNames = new string[11]
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
  private string[] mTipsTexts = new string[11];
  private static TutorialManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private TutorialManager.DialogHintRenderData[] mDialogHintRenderData;
  private Elements mElementHint;
  private MagickType mMagickHint;
  private bool mIsActive;
  private TutorialManager.HintAnimations mAnimation;
  private string mDialogString;
  private float mDialogTimer;
  private int mTrigger;
  private int mHint;
  private int mNewHint;
  private string mNewString;
  private TutorialManager.Position mHintPosition;
  private TutorialManager.Position mNewHintPosition = TutorialManager.Position.BottomRight;
  private float mHintAlpha;
  private TutorialManager.HintRenderData[] mHintRenderData;
  private BitmapFont mFont;
  private Elements mEnabledElements = Elements.All;
  private bool[] mEnabledCastTypes = new bool[6]
  {
    true,
    true,
    true,
    true,
    true,
    true
  };
  private bool mPushEnabled = true;
  private bool mDarkening;
  private bool mFadeIn;
  private float mFadeTimer;
  private TutorialManager.FadeScreenRenderData[] mFadeRenderData;
  private Elements mNewEnabledElements = Elements.Basic | Elements.Ice | Elements.Steam | Elements.Poison;
  private Dictionary<int, bool> mFinishedHints = new Dictionary<int, bool>();
  private float mTime;
  private float mHoldOffInputTimer;
  private TutorialManager.Tips mTip = TutorialManager.Tips.None;
  private TutorialManager.Tips mNewTip = TutorialManager.Tips.None;
  private float mTipTimer;
  private TutorialManager.HintRenderData[] mTipRenderData;
  private TutorialManager.Position mTipPosition;
  private TutorialManager.Position mNewTipPosition = TutorialManager.Position.Top;
  private PlayState mPlayState;

  public static TutorialManager Instance
  {
    get
    {
      if (TutorialManager.mSingelton == null)
      {
        lock (TutorialManager.mSingeltonLock)
        {
          if (TutorialManager.mSingelton == null)
            TutorialManager.mSingelton = new TutorialManager();
        }
      }
      return TutorialManager.mSingelton;
    }
  }

  private TutorialManager()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    TextBoxEffect iBoxEffect;
    GUIBasicEffect iEffect;
    IndexBuffer iIB;
    VertexBuffer iVB;
    VertexDeclaration iVD;
    VertexBuffer iIconVertices;
    VertexDeclaration iIconDeclaration;
    lock (graphicsDevice)
    {
      iBoxEffect = new TextBoxEffect(graphicsDevice, Magicka.Game.Instance.Content);
      iEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
      iIB = new IndexBuffer(graphicsDevice, TextBox.INDICES.Length * 2, BufferUsage.None, IndexElementSize.SixteenBits);
      iVB = new VertexBuffer(graphicsDevice, TextBox.VERTICES.Length * VertexPositionNormalTexture.SizeInBytes, BufferUsage.None);
      iVD = new VertexDeclaration(graphicsDevice, VertexPositionNormalTexture.VertexElements);
      iIB.SetData<ushort>(TextBox.INDICES);
      iVB.SetData<VertexPositionNormalTexture>(TextBox.VERTICES);
      iIconVertices = new VertexBuffer(graphicsDevice, 4 * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      iIconVertices.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
      iIconDeclaration = new VertexDeclaration(graphicsDevice, VertexPositionTexture.VertexElements);
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    iBoxEffect.BorderSize = 32f;
    iBoxEffect.ScreenSize = new Vector2((float) screenSize.X, (float) screenSize.Y);
    iBoxEffect.Texture = (Texture) Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Dialog_say");
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra14);
    this.mHintRenderData = new TutorialManager.HintRenderData[3];
    this.mTipRenderData = new TutorialManager.HintRenderData[3];
    this.mDialogHintRenderData = new TutorialManager.DialogHintRenderData[3];
    this.mFadeRenderData = new TutorialManager.FadeScreenRenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mFadeRenderData[index] = new TutorialManager.FadeScreenRenderData(iEffect);
      this.mHintRenderData[index] = new TutorialManager.HintRenderData(iEffect, this.mFont, iBoxEffect, iIB, iVB, iVD, iIconVertices, iIconDeclaration);
      this.mTipRenderData[index] = new TutorialManager.HintRenderData(iEffect, this.mFont, iBoxEffect, iIB, iVB, iVD, iIconVertices, iIconDeclaration);
      this.mDialogHintRenderData[index] = new TutorialManager.DialogHintRenderData(iEffect, this.mFont, iBoxEffect, iIB, iVB, iVD);
    }
    this.mHintPosition = TutorialManager.Position.BottomRight;
    this.mTipPosition = TutorialManager.Position.Top;
  }

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
    int iTargetLineWidth = (int) ((double) GlobalSettings.Instance.Resolution.Width * 0.8);
    for (int index = 0; index < this.mTipsTexts.Length; ++index)
      this.mTipsTexts[index] = this.mFont.Wrap(this.mTipsTexts[index], iTargetLineWidth, true);
  }

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
    int iTargetLineWidth = (int) ((double) GlobalSettings.Instance.Resolution.Width * 0.8);
    for (int index = 0; index < this.mTipsTexts.Length; ++index)
      this.mTipsTexts[index] = this.mFont.Wrap(this.mTipsTexts[index], iTargetLineWidth, true);
    if ((double) this.mTipTimer > 0.0 && this.mTip != TutorialManager.Tips.None)
    {
      for (int index = 0; index < 3; ++index)
        this.mTipRenderData[index].SetText(this.mTipsTexts[(int) this.mTip]);
    }
    if (this.mPlayState.GenericHealthBar == null)
      return;
    this.mPlayState.GenericHealthBar.UpdateResolution();
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mHoldOffInputTimer -= iDeltaTime;
    this.mTime += iDeltaTime;
    if (this.mHint != 0 | this.mNewHint != 0 && (double) this.mHintAlpha >= 0.0)
    {
      TutorialManager.HintRenderData iObject = this.mHintRenderData[(int) iDataChannel];
      iObject.HintPosition = this.mHintPosition;
      if (this.mHint != this.mNewHint)
      {
        this.mHintAlpha = Math.Max(this.mHintAlpha - iDeltaTime * 4f, 0.0f);
        if ((double) this.mHintAlpha <= 0.0)
        {
          this.mHint = this.mNewHint;
          this.mHintPosition = this.mNewHintPosition;
          this.mHintAlpha = 0.0f;
          for (int index = 0; index < 3; ++index)
            this.mHintRenderData[index].SetText(this.mNewString);
        }
      }
      else
        this.mHintAlpha = Math.Min(this.mHintAlpha + iDeltaTime * 4f, 1f);
      iObject.Alpha = this.mHintAlpha;
      iObject.Time = this.mTime;
      iObject.Animation = this.mAnimation;
      Player[] players = Magicka.Game.Instance.Players;
      for (int index = 0; index < players.Length; ++index)
      {
        if (players[index].Playing && !(players[index].Gamer is NetworkGamer))
        {
          iObject.GamePad = !(players[index].Controller is KeyboardMouseController);
          break;
        }
      }
      this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    if (this.mIsActive && (double) this.mDialogTimer >= 0.0)
    {
      TutorialManager.DialogHintRenderData iObject = this.mDialogHintRenderData[(int) iDataChannel];
      this.mDialogTimer = this.mIsActive ? Math.Min(this.mDialogTimer + iDeltaTime * 4f, 1f) : Math.Max(this.mDialogTimer - iDeltaTime * 4f, 0.0f);
      iObject.Alpha = this.mDialogTimer;
      this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    if (this.mDarkening)
    {
      TutorialManager.FadeScreenRenderData iObject = this.mFadeRenderData[(int) iDataChannel];
      if (this.mFadeIn)
      {
        this.mFadeTimer = Math.Min(this.mFadeTimer + iDeltaTime * 4f, 1f);
      }
      else
      {
        this.mFadeTimer = Math.Max(this.mFadeTimer - iDeltaTime * 4f, 0.0f);
        this.mDarkening = (double) this.mFadeTimer > 0.0;
      }
      iObject.FadeAlpha = this.mFadeTimer;
      this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
    }
    if (this.mPlayState.GameType != GameType.Campaign || this.mPlayState.Level == null || this.mPlayState.Level.CurrentScene.ID == Defines.WC_3_HASH || this.mHint != 0)
      return;
    this.mTipTimer -= iDeltaTime;
    if (this.mPlayState.IsInCutscene && (double) this.mTipTimer > 0.25)
    {
      this.mNewTip = TutorialManager.Tips.None;
      this.mTipTimer = 0.25f;
    }
    TutorialManager.HintRenderData iObject1 = this.mTipRenderData[(int) iDataChannel];
    iObject1.HintPosition = this.mTipPosition;
    if ((this.mNewTip != TutorialManager.Tips.None && (double) this.mTipTimer <= 0.0 || this.mNewTip == TutorialManager.Tips.WetLightning) && this.mTip != this.mNewTip)
    {
      SaveData.tip[] shownTips = this.mPlayState.Info.ShownTips;
      if ((this.mPlayState.PlayTime - shownTips[(int) this.mNewTip].timeStamp >= 180.0 || this.mNewTip == TutorialManager.Tips.WetLightning) && shownTips[(int) this.mNewTip].count < 3)
      {
        shownTips[(int) this.mNewTip].timeStamp = this.mPlayState.PlayTime;
        ++shownTips[(int) this.mNewTip].count;
        this.mTip = this.mNewTip;
        this.mTipPosition = this.mNewTipPosition;
        this.mTipTimer = 10f;
        for (int index = 0; index < 3; ++index)
          this.mTipRenderData[index].SetText(this.mTipsTexts[(int) this.mNewTip]);
      }
      this.mNewTip = TutorialManager.Tips.None;
    }
    if ((double) this.mTipTimer > 0.0)
    {
      float num = 1f;
      if ((double) this.mTipTimer >= 9.75)
        num = (float) ((10.0 - (double) this.mTipTimer) * 4.0);
      else if ((double) this.mTipTimer <= 0.25)
        num = this.mTipTimer * 4f;
      iObject1.Alpha = num;
      iObject1.Time = this.mTipTimer;
      this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject1);
    }
    else
      this.mTip = TutorialManager.Tips.None;
  }

  public void Reset()
  {
    this.mEnabledElements = Elements.Basic | Elements.Ice | Elements.Steam | Elements.Poison;
    for (int index = 0; index < this.mEnabledCastTypes.Length; ++index)
      this.mEnabledCastTypes[index] = true;
    this.mPushEnabled = true;
    this.mNewEnabledElements = this.mEnabledElements;
    this.mNewHint = this.mHint = 0;
    this.mHintAlpha = 0.0f;
    this.mDialogTimer = 0.0f;
    this.mElementHint = Elements.None;
    this.mMagickHint = MagickType.None;
    this.mIsActive = false;
    this.mDialogTimer = 0.0f;
    this.mFadeTimer = 0.0f;
    ControlManager.Instance.UnlimitInput((object) this);
    KeyboardHUD.Instance.Reset();
    this.mHoldOffInputTimer = 0.0f;
  }

  public Elements EnabledElements => this.mEnabledElements;

  public void EnableCastType(CastType iCastType) => this.mEnabledCastTypes[(int) iCastType] = true;

  public void DisableCastType(CastType iCastType)
  {
    this.mEnabledCastTypes[(int) iCastType] = false;
  }

  public bool IsCastTypeEnabled(CastType iCastType) => this.mEnabledCastTypes[(int) iCastType];

  public void EnablePush() => this.mPushEnabled = true;

  public void DisablePush() => this.mPushEnabled = false;

  public bool IsPushEnabled() => this.mPushEnabled;

  public void EnableElement(Elements iElement)
  {
    Elements iElement1 = iElement & ~this.mEnabledElements;
    this.mEnabledElements |= iElement;
    this.mNewEnabledElements = this.mEnabledElements;
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        if (players[index].Controller is DirectInputController | players[index].Controller is XInputController)
          players[index].SpellWheel.Enable(iElement1);
        else
          KeyboardHUD.Instance.Enable(iElement1);
      }
    }
  }

  public void DisableElement(Elements iElement)
  {
    Elements iElement1 = iElement & this.mEnabledElements;
    this.mEnabledElements &= ~iElement;
    this.mNewEnabledElements = this.mEnabledElements;
    Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing)
      {
        if (players[index].Controller is DirectInputController | players[index].Controller is XInputController)
          players[index].SpellWheel.Disable(iElement1);
        else
          KeyboardHUD.Instance.Disable(iElement1);
      }
    }
  }

  public bool IsElementEnabled(Elements iElement)
  {
    return (this.mEnabledElements & iElement) != Elements.None;
  }

  public void SetMagickHint(
    MagickType iMagickType,
    string iText,
    int iTrigger,
    float? iScale,
    Vector2? iSize)
  {
    this.mIsActive = true;
    this.mTrigger = iTrigger;
    this.mElementHint = Elements.None;
    this.mMagickHint = iMagickType;
    this.mDialogString = iText;
    this.mDialogTimer = 0.0f;
    this.EnableDark();
    if (iText == null)
      return;
    float iScale1 = !iScale.HasValue ? 1f : iScale.Value;
    Vector2 iSize1;
    if (iSize.HasValue)
    {
      iSize1 = iSize.Value;
    }
    else
    {
      iSize1 = this.mFont.MeasureText(iText, true);
      Vector2 vector2 = new Vector2(iScale1 * TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, iScale1 * TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y);
      if ((double) vector2.Y > (double) iSize1.Y)
        iSize1.Y = vector2.Y;
      if (iMagickType != MagickType.None)
        iSize1.X += vector2.X;
    }
    if (iMagickType != MagickType.None)
    {
      for (int index = 0; index < 3; ++index)
        this.mDialogHintRenderData[index].SetInfo(iMagickType, iText, iScale1, iSize1);
    }
    this.mHoldOffInputTimer = 1f;
  }

  public void SetElementHint(
    Elements iElement,
    string iText,
    int iTrigger,
    float? iScale,
    Vector2? iSize)
  {
    if (iElement != Elements.None)
      iText = $"{LanguageManager.Instance.GetString(Defines.ELEMENT_STRINGS[Spell.ElementIndex(iElement)])}\n\n{iText}";
    this.mIsActive = true;
    this.mTrigger = iTrigger;
    this.mElementHint = iElement;
    this.mMagickHint = MagickType.None;
    this.mDialogString = iText;
    this.EnableDark();
    if (iText == null)
      return;
    float iScale1 = !iScale.HasValue ? 1f : iScale.Value;
    if (iSize.HasValue)
    {
      Vector2 vector2_1 = iSize.Value;
    }
    else
    {
      Vector2 iSize1 = this.mFont.MeasureText(iText, true);
      Vector2 vector2_2 = new Vector2(iScale1 * TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, iScale1 * TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y);
      if ((double) vector2_2.Y > (double) iSize1.Y)
        iSize1.Y = vector2_2.Y;
      if (iElement != Elements.None)
        iSize1.X += vector2_2.X;
      for (int index = 0; index < 3; ++index)
        this.mDialogHintRenderData[index].SetInfo(iElement, iText, iScale1, iSize1);
    }
    this.mHoldOffInputTimer = 1f;
  }

  public void RemoveDialogHint()
  {
    if (!this.mIsActive | (double) this.mHoldOffInputTimer > 0.0)
      return;
    this.mIsActive = false;
    this.mElementHint = Elements.None;
    this.mMagickHint = MagickType.None;
    if (this.mTrigger != 0)
      (GameStateManager.Instance.CurrentState as PlayState).Level.CurrentScene.ExecuteTrigger(this.mTrigger, (Magicka.GameLogic.Entities.Character) null, false);
    this.mTrigger = 0;
    this.DisableDark();
  }

  public bool IsDialogHintDone() => !this.mIsActive;

  public void HideAll()
  {
    this.mIsActive = false;
    this.mElementHint = Elements.None;
    this.mMagickHint = MagickType.None;
    this.mTrigger = 0;
    this.DisableDark();
    this.mNewTip = TutorialManager.Tips.None;
    this.mTipTimer = 0.0f;
  }

  public void SetHint(
    int iNewHintHash,
    string iNewString,
    TutorialManager.HintAnimations iAnimation,
    TutorialManager.Position iHintPosition)
  {
    this.mTime = 0.0f;
    this.mAnimation = iAnimation;
    this.mNewHint = iNewHintHash;
    this.mNewString = iNewString;
    this.mNewHintPosition = iHintPosition;
  }

  public void RemoveHint()
  {
    this.mNewHint = 0;
    this.DisableDark();
  }

  public void SetTip(TutorialManager.Tips iTip, TutorialManager.Position iTipPosition)
  {
    if (this.mNewTip != TutorialManager.Tips.None && iTip != TutorialManager.Tips.WetLightning && this.mTip != TutorialManager.Tips.Wet && iTip != TutorialManager.Tips.Dying)
      return;
    this.mNewTip = iTip;
    this.mNewHintPosition = iTipPosition;
  }

  public void RemoveTip()
  {
    this.mNewTip = TutorialManager.Tips.None;
    this.mTipTimer = 0.0f;
  }

  public void EnableDark()
  {
    this.mDarkening = true;
    this.mFadeIn = true;
  }

  public void DisableDark() => this.mFadeIn = false;

  [Flags]
  public enum HintAnimations
  {
    None = 0,
    Interact = 1,
    NavMagick = 2,
    Conjure = 4,
    ConjureWater = 8,
    ConjureLife = 16, // 0x00000010
    ConjureShield = 32, // 0x00000020
    ConjureCold = 64, // 0x00000040
    ConjureLightning = 128, // 0x00000080
    ConjureArcane = 256, // 0x00000100
    ConjureEarth = 512, // 0x00000200
    ConjureFire = 1024, // 0x00000400
    CastForce = 2048, // 0x00000800
    CastArea = 4096, // 0x00001000
    CastSelf = 8192, // 0x00002000
    CastMagick = 16384, // 0x00004000
    Block = 32768, // 0x00008000
    Attack = 65536, // 0x00010000
  }

  public enum Tips
  {
    None = -1, // 0xFFFFFFFF
    ItemKey = 0,
    ItemPad = 1,
    WetLightning = 2,
    Wet = 3,
    Cold = 4,
    Poison = 5,
    Heal = 6,
    MagicksScroll = 7,
    MagicksSpell = 8,
    Beams = 9,
    Dying = 10, // 0x0000000A
    NR_OF_TIPS = 11, // 0x0000000B
  }

  public enum Position
  {
    TopLeft,
    Top,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    Bottom,
    BottomRight,
  }

  protected class FadeScreenRenderData : IRenderableGUIObject
  {
    public float FadeAlpha;
    private static int sVertexStride = VertexPositionColor.SizeInBytes;
    private GUIBasicEffect mEffect;
    private static VertexBuffer sVertexBuffer;
    private static VertexDeclaration sVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(VertexPositionColor.VertexElements);

    static FadeScreenRenderData()
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        TutorialManager.FadeScreenRenderData.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_COL_VERTS_TL.Length * TutorialManager.FadeScreenRenderData.sVertexStride, BufferUsage.None);
        TutorialManager.FadeScreenRenderData.sVertexBuffer.SetData<VertexPositionColor>(Defines.QUAD_COL_VERTS_TL);
      }
    }

    public FadeScreenRenderData(GUIBasicEffect iEffect) => this.mEffect = iEffect;

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(TutorialManager.FadeScreenRenderData.sVertexBuffer, 0, TutorialManager.FadeScreenRenderData.sVertexStride);
      this.mEffect.GraphicsDevice.VertexDeclaration = TutorialManager.FadeScreenRenderData.sVertexDeclaration;
      Matrix identity = Matrix.Identity;
      identity.M11 *= (float) screenSize.X;
      identity.M22 *= (float) screenSize.Y;
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

    public int ZIndex => 1;
  }

  private class HintRenderData : IRenderableGUIObject
  {
    public TutorialManager.Position HintPosition;
    public float Alpha;
    public TutorialManager.HintAnimations Animation;
    public bool GamePad;
    public float Time;
    private string mString;
    private Text mText;
    private bool mDirty;
    private Vector4 mColor;
    private BitmapFont mFont;
    private Vector2 mSize;
    private GUIBasicEffect mEffect;
    private TextBoxEffect mBoxEffect;
    private IndexBuffer mIndexBuffer;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    private VertexBuffer mIconVertices;
    private VertexDeclaration mIconDeclaration;
    private Text mChar;

    public HintRenderData(
      GUIBasicEffect iEffect,
      BitmapFont iFont,
      TextBoxEffect iBoxEffect,
      IndexBuffer iIB,
      VertexBuffer iVB,
      VertexDeclaration iVD,
      VertexBuffer iIconVertices,
      VertexDeclaration iIconDeclaration)
    {
      this.mIndexBuffer = iIB;
      this.mVertexBuffer = iVB;
      this.mVertexDeclaration = iVD;
      this.mBoxEffect = iBoxEffect;
      this.mEffect = iEffect;
      this.mColor = Vector4.One;
      this.mFont = iFont;
      this.mText = new Text(512 /*0x0200*/, iFont, TextAlign.Left, false);
      this.mText.SetText("");
      this.HintPosition = TutorialManager.Position.BottomRight;
      this.mChar = new Text(1, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, true);
      this.mIconVertices = iIconVertices;
      this.mIconDeclaration = iIconDeclaration;
    }

    public void Draw(float iDeltaTime)
    {
      Vector2 mSize = this.mSize;
      if (this.Animation != TutorialManager.HintAnimations.None)
        mSize.X += 128f;
      Point screenSize = RenderManager.Instance.ScreenSize;
      if (this.mDirty)
      {
        this.mText.SetText(this.mString);
        this.mDirty = false;
      }
      float x;
      float y;
      switch (this.HintPosition)
      {
        case TutorialManager.Position.Top:
          x = (float) Math.Floor((double) screenSize.X - (double) screenSize.X * 0.5);
          y = (float) Math.Floor((double) screenSize.Y - (double) screenSize.Y * 0.89999997615814209);
          break;
        case TutorialManager.Position.Center:
          x = (float) Math.Floor((double) screenSize.X - (double) screenSize.X * 0.5);
          y = (float) Math.Floor((double) screenSize.Y - (double) screenSize.Y * 0.30000001192092896);
          break;
        case TutorialManager.Position.BottomRight:
          x = (float) Math.Floor((double) screenSize.X - (double) screenSize.X * 0.05000000074505806 - (double) mSize.X * 0.5);
          y = (float) Math.Floor((double) screenSize.Y - (double) screenSize.Y * 0.05000000074505806 - (double) mSize.Y * 0.5);
          break;
        default:
          x = 0.0f;
          y = 0.0f;
          break;
      }
      this.mBoxEffect.ScreenSize = new Vector2((float) screenSize.X, (float) screenSize.Y);
      this.mColor.W = this.Alpha * 0.5f;
      this.mBoxEffect.Color = this.mColor;
      this.mBoxEffect.Size = mSize;
      this.mBoxEffect.Scale = 1f;
      this.mBoxEffect.Position = new Vector2(x, y);
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
      Vector2 iPos = new Vector2();
      iPos.X = (float) Math.Floor((double) x + (double) mSize.X * 0.5) - 64f;
      iPos.Y = y;
      if (this.GamePad)
        this.RenderIconGamePad(ref iPos);
      else
        this.RenderIconKeyBoard(ref iPos);
      this.mBoxEffect.Color = this.mColor;
      this.mEffect.TextureOffset = new Vector2();
      this.mEffect.TextureScale = new Vector2(1f);
      this.mText.Draw(this.mEffect, x - mSize.X * 0.5f, y - mSize.Y * 0.5f);
      this.mEffect.CurrentTechnique.Passes[0].End();
      this.mEffect.End();
    }

    private void RenderIconKeyBoard(ref Vector2 iPos)
    {
      Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Tutorial");
      this.mEffect.Texture = (Texture) texture2D;
      this.mEffect.TextureEnabled = true;
      this.mEffect.OverlayTextureEnabled = false;
      Vector2 vector2_1 = new Vector2(1f / (float) texture2D.Width, 1f / (float) texture2D.Height);
      Matrix matrix1 = new Matrix();
      matrix1.M41 = iPos.X;
      matrix1.M42 = iPos.Y;
      matrix1.M44 = 1f;
      Vector2 vector2_2 = new Vector2();
      Vector2 vector2_3 = new Vector2();
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
      this.mEffect.VertexColorEnabled = false;
      float num1 = 0.0f;
      for (int index = 0; index < 17; ++index)
      {
        switch (this.Animation & (TutorialManager.HintAnimations) (1 << index))
        {
          case TutorialManager.HintAnimations.Interact:
            ++num1;
            break;
          case TutorialManager.HintAnimations.NavMagick:
            num1 += 1.5f;
            break;
          case TutorialManager.HintAnimations.Conjure:
            num1 += 1.5f;
            break;
          case TutorialManager.HintAnimations.ConjureWater:
          case TutorialManager.HintAnimations.ConjureLife:
          case TutorialManager.HintAnimations.ConjureShield:
          case TutorialManager.HintAnimations.ConjureCold:
          case TutorialManager.HintAnimations.ConjureLightning:
          case TutorialManager.HintAnimations.ConjureArcane:
          case TutorialManager.HintAnimations.ConjureEarth:
          case TutorialManager.HintAnimations.ConjureFire:
            num1 += 2f;
            break;
          case TutorialManager.HintAnimations.CastForce:
          case TutorialManager.HintAnimations.CastArea:
          case TutorialManager.HintAnimations.CastSelf:
          case TutorialManager.HintAnimations.CastMagick:
            num1 += 1.5f;
            break;
          case TutorialManager.HintAnimations.Block:
            ++num1;
            break;
          case TutorialManager.HintAnimations.Attack:
            num1 += 1.5f;
            break;
        }
      }
      float num2 = this.Time % num1;
      for (int index = 0; index < 17; ++index)
      {
        if ((double) num2 >= 0.0)
        {
          switch (this.Animation & (TutorialManager.HintAnimations) (1 << index))
          {
            case TutorialManager.HintAnimations.Interact:
              if ((double) num2 < 1.0)
              {
                matrix1.M11 = 80f;
                matrix1.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              --num2;
              continue;
            case TutorialManager.HintAnimations.NavMagick:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 80f;
                matrix1.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                vector2_2.X = 352f * vector2_1.X;
                vector2_2.Y = 192f * vector2_1.Y;
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.Conjure:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 48f;
                matrix1.M22 = 48f;
                vector2_3.X = 48f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                vector2_2.X = 384f * vector2_1.X;
                vector2_2.Y = 96f * vector2_1.Y;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                Matrix matrix2 = matrix1;
                matrix2.M41 -= 32f;
                this.mEffect.Transform = matrix2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix2.M41 += 32f;
                this.mEffect.Transform = matrix2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix2.M41 += 32f;
                this.mEffect.Transform = matrix2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                this.mEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, 1f);
                this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Lightning)[0];
                this.mChar.MarkAsDirty();
                this.mChar.Draw(this.mEffect, iPos.X - 32f, iPos.Y - (float) (this.mChar.Font.LineHeight / 2));
                this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Arcane)[0];
                this.mChar.MarkAsDirty();
                this.mChar.Draw(this.mEffect, iPos.X, iPos.Y - (float) (this.mChar.Font.LineHeight / 2));
                this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Fire)[0];
                this.mChar.MarkAsDirty();
                this.mChar.Draw(this.mEffect, iPos.X + 32f, iPos.Y - (float) (this.mChar.Font.LineHeight / 2));
                this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
                this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
                this.mEffect.Color = new Vector4(1f);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.ConjureWater:
            case TutorialManager.HintAnimations.ConjureLife:
            case TutorialManager.HintAnimations.ConjureShield:
            case TutorialManager.HintAnimations.ConjureCold:
            case TutorialManager.HintAnimations.ConjureLightning:
            case TutorialManager.HintAnimations.ConjureArcane:
            case TutorialManager.HintAnimations.ConjureEarth:
            case TutorialManager.HintAnimations.ConjureFire:
              if ((double) num2 < 2.0)
              {
                matrix1.M11 = 48f;
                matrix1.M22 = 48f;
                vector2_3.X = 48f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                if ((double) num2 < 1.3329999446868896)
                {
                  vector2_2.X = 384f * vector2_1.X;
                  vector2_2.Y = 96f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 336f * vector2_1.X;
                  vector2_2.Y = 96f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                this.mEffect.Color = new Vector4(0.0f, 0.0f, 0.0f, 1f);
                switch ((TutorialManager.HintAnimations) (1 << index))
                {
                  case TutorialManager.HintAnimations.ConjureWater:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Water)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureLife:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Life)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureShield:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Shield)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureCold:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Cold)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureLightning:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Lightning)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureArcane:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Arcane)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureEarth:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Earth)[0];
                    break;
                  case TutorialManager.HintAnimations.ConjureFire:
                    this.mChar.Characters[0] = KeyboardMouseController.KeyToString(KeyboardBindings.Fire)[0];
                    break;
                }
                this.mChar.MarkAsDirty();
                this.mChar.Draw(this.mEffect, iPos.X, iPos.Y - (float) (this.mChar.Font.LineHeight / 2));
                this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
                this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
                this.mEffect.Color = new Vector4(1f);
              }
              num2 -= 2f;
              continue;
            case TutorialManager.HintAnimations.CastForce:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 80f;
                matrix1.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 1.0)
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 192f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastArea:
              if ((double) num2 < 1.5)
              {
                Matrix matrix3 = matrix1;
                matrix3.M41 -= 48f;
                matrix3.M11 = 64f;
                matrix3.M22 = 48f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 368f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 368f * vector2_1.X;
                  vector2_2.Y = 48f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix3;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix3.M41 += 36f;
                matrix3.M11 = 32f;
                matrix3.M22 = 32f;
                vector2_3.X = 32f * vector2_1.X;
                vector2_3.Y = 32f * vector2_1.Y;
                vector2_2.X = 192f * vector2_1.X;
                vector2_2.Y = 0.0f * vector2_1.Y;
                this.mEffect.Transform = matrix3;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix3.M41 += 44f;
                matrix3.M11 = 80f;
                matrix3.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 1.0)
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 192f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix3;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastSelf:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 80f;
                matrix1.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 1.0)
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 128f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastMagick:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 128f;
                matrix1.M22 = 48f;
                vector2_3.X = 128f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                if ((double) num2 < 1.0)
                {
                  vector2_2.X = 240f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 240f * vector2_1.X;
                  vector2_2.Y = 48f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.Block:
              if ((double) num2 < 1.0)
              {
                matrix1.M11 = 72f;
                matrix1.M22 = 48f;
                vector2_3.X = 72f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 368f * vector2_1.X;
                  vector2_2.Y = 144f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 304f * vector2_1.X;
                  vector2_2.Y = 144f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              --num2;
              continue;
            case TutorialManager.HintAnimations.Attack:
              if ((double) num2 < 1.5)
              {
                Matrix matrix4 = matrix1;
                matrix4.M41 -= 48f;
                matrix4.M11 = 64f;
                matrix4.M22 = 48f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 48f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 368f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 368f * vector2_1.X;
                  vector2_2.Y = 48f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix4;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix4.M41 += 36f;
                matrix4.M11 = 32f;
                matrix4.M22 = 32f;
                vector2_3.X = 32f * vector2_1.X;
                vector2_3.Y = 32f * vector2_1.Y;
                vector2_2.X = 192f * vector2_1.X;
                vector2_2.Y = 0.0f * vector2_1.Y;
                this.mEffect.Transform = matrix4;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix4.M41 += 44f;
                matrix4.M11 = 80f;
                matrix4.M22 = 64f;
                vector2_3.X = 80f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 1.0)
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 0.0f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 432f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix4;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            default:
              continue;
          }
        }
      }
    }

    private void RenderIconGamePad(ref Vector2 iPos)
    {
      Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Tutorial");
      this.mEffect.Texture = (Texture) texture2D;
      this.mEffect.TextureEnabled = true;
      this.mEffect.OverlayTextureEnabled = false;
      Vector2 vector2_1 = new Vector2(1f / (float) texture2D.Width, 1f / (float) texture2D.Height);
      Matrix matrix1 = new Matrix();
      matrix1.M41 = iPos.X;
      matrix1.M42 = iPos.Y;
      matrix1.M44 = 1f;
      Vector2 vector2_2 = new Vector2();
      Vector2 vector2_3 = new Vector2();
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mIconVertices, 0, VertexPositionTexture.SizeInBytes);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mIconDeclaration;
      this.mEffect.VertexColorEnabled = false;
      float num1 = 0.0f;
      for (int index = 0; index < 17; ++index)
      {
        switch (this.Animation & (TutorialManager.HintAnimations) (1 << index))
        {
          case TutorialManager.HintAnimations.Interact:
            ++num1;
            break;
          case TutorialManager.HintAnimations.NavMagick:
            num1 += 1.5f;
            break;
          case TutorialManager.HintAnimations.Conjure:
            num1 += 2f;
            break;
          case TutorialManager.HintAnimations.ConjureWater:
          case TutorialManager.HintAnimations.ConjureLife:
          case TutorialManager.HintAnimations.ConjureShield:
          case TutorialManager.HintAnimations.ConjureCold:
          case TutorialManager.HintAnimations.ConjureLightning:
          case TutorialManager.HintAnimations.ConjureArcane:
          case TutorialManager.HintAnimations.ConjureEarth:
          case TutorialManager.HintAnimations.ConjureFire:
            num1 += 5f;
            break;
          case TutorialManager.HintAnimations.CastForce:
          case TutorialManager.HintAnimations.CastArea:
          case TutorialManager.HintAnimations.CastSelf:
          case TutorialManager.HintAnimations.CastMagick:
            num1 += 1.5f;
            break;
          case TutorialManager.HintAnimations.Block:
          case TutorialManager.HintAnimations.Attack:
            ++num1;
            break;
        }
      }
      float num2 = this.Time % num1;
      for (int index = 0; index < 17; ++index)
      {
        if ((double) num2 >= 0.0)
        {
          switch (this.Animation & (TutorialManager.HintAnimations) (1 << index))
          {
            case TutorialManager.HintAnimations.Interact:
              if ((double) num2 < 1.0)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 192f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              --num2;
              continue;
            case TutorialManager.HintAnimations.NavMagick:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 64f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 128f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.Conjure:
              if ((double) num2 < 2.0)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                vector2_2.X = 64f * vector2_1.X;
                vector2_2.Y = 0.0f * vector2_1.Y;
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 2f;
              continue;
            case TutorialManager.HintAnimations.ConjureWater:
            case TutorialManager.HintAnimations.ConjureLife:
            case TutorialManager.HintAnimations.ConjureShield:
            case TutorialManager.HintAnimations.ConjureCold:
            case TutorialManager.HintAnimations.ConjureLightning:
            case TutorialManager.HintAnimations.ConjureArcane:
            case TutorialManager.HintAnimations.ConjureEarth:
            case TutorialManager.HintAnimations.ConjureFire:
              if ((double) num2 < 5.0)
              {
                Matrix matrix2 = matrix1;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                vector2_2.X = 128f * vector2_1.X;
                vector2_2.Y = 0.0f * vector2_1.Y;
                matrix2.M11 = 64f;
                matrix2.M22 = 64f;
                this.mEffect.Transform = matrix2;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                vector2_2.X = 64f * vector2_1.X;
                vector2_2.Y = 0.0f * vector2_1.Y;
                if ((double) num2 > 1.0)
                {
                  if ((double) num2 < 2.0)
                  {
                    switch ((TutorialManager.HintAnimations) (1 << index))
                    {
                      case TutorialManager.HintAnimations.ConjureWater:
                      case TutorialManager.HintAnimations.ConjureLightning:
                        matrix2.M41 -= (float) (((double) num2 - 1.0) * 18.0);
                        break;
                      case TutorialManager.HintAnimations.ConjureLife:
                      case TutorialManager.HintAnimations.ConjureArcane:
                        matrix2.M42 -= (float) (((double) num2 - 1.0) * 18.0);
                        break;
                      case TutorialManager.HintAnimations.ConjureShield:
                      case TutorialManager.HintAnimations.ConjureEarth:
                        matrix2.M42 += (float) (((double) num2 - 1.0) * 18.0);
                        break;
                      case TutorialManager.HintAnimations.ConjureCold:
                      case TutorialManager.HintAnimations.ConjureFire:
                        matrix2.M41 += (float) (((double) num2 - 1.0) * 18.0);
                        break;
                    }
                  }
                  else if ((double) num2 < 3.0)
                  {
                    switch ((TutorialManager.HintAnimations) (1 << index))
                    {
                      case TutorialManager.HintAnimations.ConjureWater:
                      case TutorialManager.HintAnimations.ConjureLightning:
                        matrix2.M41 -= 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureLife:
                      case TutorialManager.HintAnimations.ConjureArcane:
                        matrix2.M42 -= 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureShield:
                      case TutorialManager.HintAnimations.ConjureEarth:
                        matrix2.M42 += 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureCold:
                      case TutorialManager.HintAnimations.ConjureFire:
                        matrix2.M41 += 18f;
                        break;
                    }
                  }
                  else if ((double) num2 < 4.0)
                  {
                    switch ((TutorialManager.HintAnimations) (1 << index))
                    {
                      case TutorialManager.HintAnimations.ConjureWater:
                        matrix2.M41 -= (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 -= (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureLife:
                        matrix2.M41 -= (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 -= (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureShield:
                        matrix2.M41 += (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 += (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureCold:
                        matrix2.M41 += (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 -= (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureLightning:
                        matrix2.M41 -= (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 += (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureArcane:
                        matrix2.M41 += (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 -= (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureEarth:
                        matrix2.M41 -= (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 += (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureFire:
                        matrix2.M41 += (float) Math.Cos(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        matrix2.M42 += (float) Math.Sin(((double) num2 - 3.0) * 1.5707963705062866) * 18f;
                        break;
                    }
                  }
                  else if ((double) num2 < 5.0)
                  {
                    switch ((TutorialManager.HintAnimations) (1 << index))
                    {
                      case TutorialManager.HintAnimations.ConjureWater:
                      case TutorialManager.HintAnimations.ConjureCold:
                        matrix2.M42 -= 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureLife:
                      case TutorialManager.HintAnimations.ConjureEarth:
                        matrix2.M41 -= 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureShield:
                      case TutorialManager.HintAnimations.ConjureArcane:
                        matrix2.M41 += 18f;
                        break;
                      case TutorialManager.HintAnimations.ConjureLightning:
                      case TutorialManager.HintAnimations.ConjureFire:
                        matrix2.M42 += 18f;
                        break;
                    }
                  }
                }
                this.mEffect.Transform = matrix2;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 5f;
              continue;
            case TutorialManager.HintAnimations.CastForce:
              if ((double) num2 < 1.5)
              {
                Matrix matrix3 = matrix1;
                matrix3.M42 -= 16f;
                matrix3.M11 = 64f;
                matrix3.M22 = 36f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 36f * vector2_1.Y;
                vector2_2.X = 192f * vector2_1.X;
                vector2_2.Y = (double) num2 >= 0.5 ? 192f * vector2_1.Y : 128f * vector2_1.Y;
                this.mEffect.Transform = matrix3;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix3.M22 = 28f;
                matrix3.M42 += 32f;
                vector2_3.Y = 28f * vector2_1.Y;
                vector2_2.Y = 164f * vector2_1.Y;
                this.mEffect.Transform = matrix3;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastArea:
              if ((double) num2 < 1.5)
              {
                Matrix matrix4 = matrix1;
                matrix4.M42 -= 16f;
                matrix4.M11 = 64f;
                matrix4.M22 = 36f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 36f * vector2_1.Y;
                vector2_2.X = 128f * vector2_1.X;
                vector2_2.Y = (double) num2 >= 0.5 ? 192f * vector2_1.Y : 128f * vector2_1.Y;
                this.mEffect.Transform = matrix4;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix4.M22 = 28f;
                matrix4.M42 += 32f;
                vector2_3.Y = 28f * vector2_1.Y;
                vector2_2.Y = 164f * vector2_1.Y;
                this.mEffect.Transform = matrix4;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastSelf:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 64f * vector2_1.X;
                  vector2_2.Y = 192f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.CastMagick:
              if ((double) num2 < 1.5)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                else
                {
                  vector2_2.X = 64f * vector2_1.X;
                  vector2_2.Y = 128f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              num2 -= 1.5f;
              continue;
            case TutorialManager.HintAnimations.Block:
              if ((double) num2 < 1.0)
              {
                Matrix matrix5 = matrix1;
                matrix5.M42 += 16f;
                matrix5.M11 = 64f;
                matrix5.M22 = 28f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 28f * vector2_1.Y;
                vector2_2.X = 192f * vector2_1.X;
                vector2_2.Y = (double) num2 >= 0.5 ? 228f * vector2_1.Y : 164f * vector2_1.Y;
                this.mEffect.Transform = matrix5;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                matrix5.M22 = 36f;
                matrix5.M42 -= 32f;
                vector2_3.Y = 36f * vector2_1.Y;
                vector2_2.Y = 128f * vector2_1.Y;
                this.mEffect.Transform = matrix5;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              --num2;
              continue;
            case TutorialManager.HintAnimations.Attack:
              if ((double) num2 < 1.0)
              {
                matrix1.M11 = 64f;
                matrix1.M22 = 64f;
                vector2_3.X = 64f * vector2_1.X;
                vector2_3.Y = 64f * vector2_1.Y;
                if ((double) num2 < 0.5)
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 64f * vector2_1.Y;
                }
                else if ((double) num2 < 1.0)
                {
                  vector2_2.X = 0.0f * vector2_1.X;
                  vector2_2.Y = 128f * vector2_1.Y;
                }
                this.mEffect.Transform = matrix1;
                this.mEffect.TextureScale = vector2_3;
                this.mEffect.TextureOffset = vector2_2;
                this.mEffect.CommitChanges();
                this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
              }
              --num2;
              continue;
            default:
              continue;
          }
        }
      }
    }

    public int ZIndex => 103;

    public void SetText(string iString)
    {
      this.mSize = this.mFont.MeasureText(iString, true);
      this.mString = iString;
      this.mDirty = true;
    }
  }

  private class DialogHintRenderData : IRenderableGUIObject
  {
    public static readonly Vector2 MAGICK_PX_SIZE = new Vector2(400f, 250f);
    public static readonly Vector2 MAGICK_SIZE = new Vector2(25f / 128f, 0.122070313f);
    public static readonly Vector2 ELEMENT_PX_SIZE = new Vector2(50f, 50f);
    public static readonly Vector2 ELEMENT_OFFSET = new Vector2(0.0f, 39f / 128f);
    public static readonly Vector2 ELEMENT_SIZE = new Vector2(25f / 256f, 25f / 256f);
    private static readonly VertexBuffer IMAGE_VB;
    private static readonly VertexDeclaration IMAGE_VD;
    public float Alpha;
    private string mString;
    private Text mText;
    private bool mDirty;
    private Vector4 mColor;
    private BitmapFont mFont;
    private Vector2 mSize;
    private float mScale;
    private GUIBasicEffect mGUIBasicEffect;
    private TextBoxEffect mTextBoxEffect;
    private IndexBuffer mIndexBuffer;
    private VertexBuffer mVertexBuffer;
    private VertexDeclaration mVertexDeclaration;
    private Vector2 mTextureOffset;
    private Vector2 mTextureScale;
    private Elements mElement;
    private MagickType mMagickType;
    private Texture2D mElementsTexture;
    private Texture2D mMagicksTexture;
    private Texture2D mMagicksTexture2;
    private int mStartVertex;
    private float mTextOffset;
    private static readonly float TEXT_PADDING = 6f;

    static DialogHintRenderData()
    {
      VertexPositionTexture[] data = new VertexPositionTexture[8];
      data[0].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y);
      data[0].Position = new Vector3(0.0f, 0.0f, 0.0f);
      data[1].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y);
      data[1].Position = new Vector3(TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, 0.0f, 0.0f);
      data[2].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y);
      data[2].Position = new Vector3(TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X, TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y, 0.0f);
      data[3].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.X, TutorialManager.DialogHintRenderData.ELEMENT_OFFSET.Y + TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y);
      data[3].Position = new Vector3(0.0f, TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.Y, 0.0f);
      data[4].TextureCoordinate = new Vector2(0.0f, 0.0f);
      data[4].Position = new Vector3(0.0f, 0.0f, 0.0f);
      data[5].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.MAGICK_SIZE.X, 0.0f);
      data[5].Position = new Vector3(TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, 0.0f, 0.0f);
      data[6].TextureCoordinate = new Vector2(TutorialManager.DialogHintRenderData.MAGICK_SIZE.X, TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y);
      data[6].Position = new Vector3(TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X, TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y, 0.0f);
      data[7].TextureCoordinate = new Vector2(0.0f, TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y);
      data[7].Position = new Vector3(0.0f, TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.Y, 0.0f);
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        TutorialManager.DialogHintRenderData.IMAGE_VB = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
        TutorialManager.DialogHintRenderData.IMAGE_VD = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
        TutorialManager.DialogHintRenderData.IMAGE_VB.SetData<VertexPositionTexture>(data);
      }
    }

    public DialogHintRenderData(
      GUIBasicEffect iEffect,
      BitmapFont iFont,
      TextBoxEffect iBoxEffect,
      IndexBuffer iIB,
      VertexBuffer iVB,
      VertexDeclaration iVD)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        this.mElementsTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/hud");
        this.mMagicksTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
        this.mMagicksTexture2 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks_2");
      }
      this.mIndexBuffer = iIB;
      this.mVertexBuffer = iVB;
      this.mVertexDeclaration = iVD;
      this.mTextBoxEffect = iBoxEffect;
      this.mGUIBasicEffect = iEffect;
      this.mColor = Vector4.One;
      this.mFont = iFont;
      this.mText = new Text(512 /*0x0200*/, iFont, TextAlign.Left, false);
      this.mText.SetText("");
    }

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      if (this.mDirty)
      {
        this.mText.SetText(this.mString);
        this.mDirty = false;
      }
      Vector2 vector2_1;
      vector2_1.X = (float) ((double) screenSize.X * 0.5 + 0.5);
      vector2_1.Y = (float) ((double) screenSize.Y * 0.40000000596046448 + 0.5);
      Vector2 vector2_2 = new Vector2((float) screenSize.X, (float) screenSize.Y);
      this.mTextBoxEffect.Position = vector2_1;
      this.mTextBoxEffect.Size = this.mSize;
      this.mTextBoxEffect.Scale = 1f;
      this.mTextBoxEffect.Color = new Vector4(1f, 1f, 1f, this.Alpha);
      this.mTextBoxEffect.ScreenSize = vector2_2;
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
      this.mGUIBasicEffect.Transform = Matrix.Identity with
      {
        M11 = this.mScale,
        M22 = this.mScale,
        M41 = vector2_1.X - this.mSize.X * 0.5f,
        M42 = vector2_1.Y - this.mSize.Y * 0.5f
      };
      if (this.mElement != Elements.None)
        this.mGUIBasicEffect.Texture = (Texture) this.mElementsTexture;
      else if (this.mMagickType != MagickType.None)
        this.mGUIBasicEffect.Texture = this.mMagickType < MagickType.Shrink ? (Texture) this.mMagicksTexture : (Texture) this.mMagicksTexture2;
      this.mGUIBasicEffect.Begin();
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      if (this.mElement != Elements.None | this.mMagickType != MagickType.None)
        this.mGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, this.mStartVertex, 2);
      this.mGUIBasicEffect.VertexColorEnabled = true;
      this.mText.Draw(this.mGUIBasicEffect, vector2_1.X - (float) ((double) this.mSize.X * 0.5 - (double) this.mTextOffset * (double) this.mScale), vector2_1.Y - this.mSize.Y * 0.5f);
      this.mGUIBasicEffect.CurrentTechnique.Passes[0].End();
      this.mGUIBasicEffect.End();
      this.mGUIBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
    }

    public int ZIndex => 202;

    public void SetInfo(MagickType iMagickType, string iString, float iScale, Vector2 iSize)
    {
      this.mScale = iScale;
      this.mSize = iSize;
      this.mStartVertex = 4;
      if (iMagickType != MagickType.None)
      {
        this.mTextOffset = TutorialManager.DialogHintRenderData.MAGICK_PX_SIZE.X + TutorialManager.DialogHintRenderData.TEXT_PADDING;
        int num = (int) iMagickType;
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
        this.mTextureOffset.X = (float) (num % 5) * TutorialManager.DialogHintRenderData.MAGICK_SIZE.X;
        this.mTextureOffset.Y = (float) (num / 5) * TutorialManager.DialogHintRenderData.MAGICK_SIZE.Y;
      }
      else
        this.mTextOffset = 0.0f;
      this.mMagickType = iMagickType;
      this.mElement = Elements.None;
      this.mString = iString;
      this.mDirty = true;
    }

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
        this.mTextureOffset.X = (float) (num % 5) * TutorialManager.DialogHintRenderData.ELEMENT_SIZE.X;
        this.mTextureOffset.Y = (float) (num / 5) * TutorialManager.DialogHintRenderData.ELEMENT_SIZE.Y;
        this.mTextOffset = TutorialManager.DialogHintRenderData.ELEMENT_PX_SIZE.X + TutorialManager.DialogHintRenderData.TEXT_PADDING;
      }
      else
        this.mTextOffset = 0.0f;
      this.mMagickType = MagickType.None;
      this.mElement = iElement;
      this.mString = iString;
      this.mDirty = true;
    }
  }
}
