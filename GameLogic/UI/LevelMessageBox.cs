// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.LevelMessageBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class LevelMessageBox : MessageBox
{
  private static LevelMessageBox sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int LOC_CUSTOM = "#notice_mod_content".GetHashCodeCustom();
  public static readonly int LOC_LOADING = "#network_23".GetHashCodeCustom();
  private static VertexBuffer sLevelVertexBuffer;
  private static VertexDeclaration sLevelVertexDeclaration;
  private static readonly int TITLE_WRAPPING = 336;
  private static readonly int DESCR_WRAPPING = 360;
  private static readonly float TITLE_PADDING = 8f;
  private static readonly float IMAGE_PADDING = 8f;
  private static readonly float ARROW_PADDING = 48f;
  private static readonly float OK_PADDING = 16f;
  private float mTitleFontHeight;
  private string mCurrentTexturePath;
  private Texture2D mCurrentTexture;
  private Texture2D mSelectedTexture;
  private float mTextureAlpha;
  private float mLevelTitleSpacing;
  private int mLevelSelection;
  private bool mAllowCustom;
  private HackHelper.License mCustomLevel = HackHelper.License.Pending;
  private Action<Controller, GameType, int, bool> mComplete;
  private GameType mLevelType = GameType.Campaign;
  private LevelMessageBox.Selections mSelection;
  private int mLoadingTextLength;
  private Text mLoadingText = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Left, true);
  private float mLoadingDotTimer = 0.5f;
  private Text mLevelTitle = new Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.MenuOption), TextAlign.Center, false);
  private Text mLevelDescription = new Text(1024 /*0x0400*/, FontManager.Instance.GetFont(MagickaFont.Maiandra18), TextAlign.Center, false);
  private MenuTextItem mOkItem;
  private MenuTextItem mCancelItem;
  private MenuImageItem mLeftArrow;
  private MenuImageItem mRightArrow;
  private Texture2D mCustomLevelTexture;
  private Text mCustomLevelText;
  private int mLastStep;

  public static LevelMessageBox Instance
  {
    get
    {
      if (LevelMessageBox.sSingelton == null)
      {
        lock (LevelMessageBox.sSingeltonLock)
        {
          if (LevelMessageBox.sSingelton == null)
            LevelMessageBox.sSingelton = new LevelMessageBox();
        }
      }
      return LevelMessageBox.sSingelton;
    }
  }

  static LevelMessageBox()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      LevelMessageBox.sLevelVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, Defines.QUAD_TEX_VERTS_C.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      LevelMessageBox.sLevelVertexBuffer.SetData<VertexPositionTexture>(Defines.QUAD_TEX_VERTS_C);
      LevelMessageBox.sLevelVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
  }

  private LevelMessageBox()
    : base(" ")
  {
    Texture2D texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    VertexPositionTexture[] data = new VertexPositionTexture[Defines.QUAD_TEX_VERTS_C.Length];
    Defines.QUAD_TEX_VERTS_C.CopyTo((Array) data, 0);
    data[0].TextureCoordinate.X = 1344f / (float) texture2D.Width;
    data[0].TextureCoordinate.Y = 160f / (float) texture2D.Height;
    data[1].TextureCoordinate.X = 1280f / (float) texture2D.Width;
    data[1].TextureCoordinate.Y = 160f / (float) texture2D.Height;
    data[2].TextureCoordinate.X = 1280f / (float) texture2D.Width;
    data[2].TextureCoordinate.Y = 96f / (float) texture2D.Height;
    data[3].TextureCoordinate.X = 1344f / (float) texture2D.Width;
    data[3].TextureCoordinate.Y = 96f / (float) texture2D.Height;
    VertexBuffer iVertices;
    VertexDeclaration iDeclaration;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      iVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      iVertices.SetData<VertexPositionTexture>(data);
      iDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
    this.mTextureAlpha = 0.0f;
    this.mLeftArrow = new MenuImageItem(new Vector2(this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f), MessageBox.sTexture, iVertices, iDeclaration, 1.57079637f, 0, VertexPositionTexture.SizeInBytes, 32f, 32f);
    this.mRightArrow = new MenuImageItem(new Vector2(this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f), MessageBox.sTexture, iVertices, iDeclaration, -1.57079637f, 0, VertexPositionTexture.SizeInBytes, 32f, 32f);
    this.mLevelTitleSpacing = this.mTitleFontHeight = (float) this.mLevelTitle.Font.LineHeight;
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mCustomLevelTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CustomLevel");
    this.mCustomLevelText = new Text(256 /*0x0100*/, FontManager.Instance.GetFont(MagickaFont.Maiandra16), TextAlign.Left, false);
    this.mCustomLevelText.SetText(this.mCustomLevelText.Font.Wrap(LanguageManager.Instance.GetString(LevelMessageBox.LOC_CUSTOM), 170, true));
    string iText = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
    this.mLoadingTextLength = iText.Length;
    this.mLoadingText.SetText(iText);
    Vector2 iPosition = new Vector2(this.mCenter.X - 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
    this.mOkItem = new MenuTextItem(SubMenu.LOC_OK, iPosition, this.mFont, TextAlign.Right);
    this.mOkItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mOkItem.ColorSelected = Vector4.One;
    iPosition = new Vector2(this.mCenter.X + 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
    this.mCancelItem = new MenuTextItem(SubMenu.LOC_CANCEL, iPosition, this.mFont, TextAlign.Left);
    this.mCancelItem.Color = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mCancelItem.ColorSelected = Vector4.One;
    this.mLevelTitle.DefaultColor = Defines.MESSAGEBOX_COLOR_DEFAULT;
    this.mLevelDescription.DefaultColor = Defines.MESSAGEBOX_COLOR_DEFAULT;
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    LevelNode[] levelNodeArray = this.mLevelType != GameType.Challenge ? LevelManager.Instance.Versus : LevelManager.Instance.Challenges;
    if (this.mLevelSelection >= 0 && this.mLevelSelection < levelNodeArray.Length)
    {
      LevelNode levelNode = levelNodeArray[this.mLevelSelection];
      string iText = this.mLevelTitle.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()), LevelMessageBox.TITLE_WRAPPING, true);
      this.mLevelTitle.SetText(iText);
      this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(iText, true).Y;
      this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
    }
    this.mCustomLevelText.SetText(this.mCustomLevelText.Font.Wrap(LanguageManager.Instance.GetString(LevelMessageBox.LOC_CUSTOM), 170, true));
    string iText1 = LanguageManager.Instance.GetString(LevelMessageBox.LOC_LOADING);
    this.mLoadingTextLength = iText1.Length;
    this.mLoadingText.SetText(iText1);
    this.mOkItem.LanguageChanged();
    this.mCancelItem.LanguageChanged();
  }

  public override void Show() => throw new InvalidOperationException();

  public void Show(
    GameType iType,
    bool iAllowCustom,
    Action<Controller, GameType, int, bool> iOnComplete)
  {
    this.mTextureAlpha = 0.0f;
    this.mComplete += iOnComplete;
    this.mAllowCustom = iAllowCustom;
    if (iType != this.mLevelType || !iAllowCustom && this.mCustomLevel != HackHelper.License.Yes)
    {
      this.mLevelSelection = 0;
      this.mLastStep = 1;
      LevelNode[] levelNodeArray;
      switch (iType)
      {
        case GameType.Challenge:
          levelNodeArray = LevelManager.Instance.Challenges;
          break;
        case GameType.StoryChallange:
          levelNodeArray = LevelManager.Instance.StoryChallanges;
          break;
        default:
          levelNodeArray = LevelManager.Instance.Versus;
          break;
      }
      HackHelper.License license = HackHelper.License.Pending;
      for (; this.mLevelSelection < levelNodeArray.Length; ++this.mLevelSelection)
      {
        license = HackHelper.CheckLicense(levelNodeArray[this.mLevelSelection]);
        switch (license)
        {
          case HackHelper.License.Pending:
          case HackHelper.License.Yes:
            goto label_10;
          case HackHelper.License.Custom:
            if (iAllowCustom)
              goto label_10;
            break;
        }
      }
label_10:
      this.mCustomLevel = license;
      LevelNode levelNode = levelNodeArray[this.mLevelSelection];
      string iText = this.mLevelTitle.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()), LevelMessageBox.TITLE_WRAPPING, true);
      this.mLevelTitle.SetText(iText);
      this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(iText, true).Y;
      if (levelNode.Name.Equals("#Location_debug", StringComparison.InvariantCultureIgnoreCase))
        this.mLevelDescription.SetText(levelNode.FileName);
      else
        this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
      this.mLevelType = iType;
      this.SetPreview(levelNode.LoadingImage);
    }
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mCenter.X = (float) screenSize.X * 0.5f;
    this.mCenter.Y = (float) screenSize.Y * 0.5f;
    this.mLeftArrow.Position = new Vector2(this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f);
    this.mRightArrow.Position = new Vector2(this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING, this.mCenter.Y + this.mSize.Y * 0.25f);
    this.mOkItem.Position = new Vector2(this.mCenter.X - 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
    this.mCancelItem.Position = new Vector2(this.mCenter.X + 16f, this.mCenter.Y + this.mSize.Y * 0.5f - this.mTitleFontHeight - LevelMessageBox.OK_PADDING);
    base.Show();
  }

  public override void Kill()
  {
    this.mComplete = (Action<Controller, GameType, int, bool>) null;
    base.Kill();
  }

  public override void OnTextInput(char iChar)
  {
  }

  private void SetPreview(string iPreviewFileName)
  {
    this.mCurrentTexturePath = this.mLevelType != GameType.Versus ? "Levels/Challenges/" + iPreviewFileName : "Levels/Versus/" + iPreviewFileName;
    this.mTextureAlpha = 0.0f;
    Magicka.Game.Instance.AddLoadTask(new Action(this.LoadTexture));
  }

  private void LoadTexture()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mCurrentTexture = Magicka.Game.Instance.Content.Load<Texture2D>(this.mCurrentTexturePath);
  }

  public Texture2D SelectedTexture => this.mSelectedTexture;

  private void ChangeLevelPreview(int iNewLevel)
  {
    LevelNode[] levelNodeArray = this.mLevelType != GameType.Challenge ? LevelManager.Instance.Versus : LevelManager.Instance.Challenges;
    this.mLevelSelection = iNewLevel < levelNodeArray.Length ? (iNewLevel >= 0 ? iNewLevel : levelNodeArray.Length - 1) : 0;
    LevelNode levelNode = levelNodeArray[this.mLevelSelection];
    string iText = this.mLevelTitle.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Name.GetHashCodeCustom()), LevelMessageBox.TITLE_WRAPPING, true);
    this.mLevelTitle.SetText(iText);
    this.mLevelTitleSpacing = this.mLevelTitle.Font.MeasureText(iText, true).Y;
    if (levelNode.Name.Equals("#Location_debug", StringComparison.InvariantCultureIgnoreCase))
      this.mLevelDescription.SetText(levelNode.FileName);
    else
      this.mLevelDescription.SetText(this.mLevelDescription.Font.Wrap(LanguageManager.Instance.GetString(levelNode.Description), LevelMessageBox.DESCR_WRAPPING, true));
    this.SetPreview(levelNode.LoadingImage);
  }

  private void ChangeSelection(int iStep)
  {
    this.mLastStep = iStep;
    int index = this.mLevelSelection + iStep;
    LevelNode[] levelNodeArray = this.mLevelType != GameType.Versus ? LevelManager.Instance.Challenges : LevelManager.Instance.Versus;
    HackHelper.License license = HackHelper.License.Pending;
    for (; this.mLevelSelection != index; index += iStep)
    {
      if (index >= levelNodeArray.Length)
        index -= levelNodeArray.Length;
      if (index < 0)
        index += levelNodeArray.Length;
      license = HackHelper.CheckLicense(levelNodeArray[index]);
      switch (license)
      {
        case HackHelper.License.Pending:
        case HackHelper.License.Yes:
          goto label_9;
        case HackHelper.License.Custom:
          if (this.mAllowCustom)
            goto label_9;
          break;
      }
    }
label_9:
    this.mCustomLevel = license;
    this.mLevelSelection = index;
    this.ChangeLevelPreview(this.mLevelSelection);
  }

  public override void OnMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (this.mSelection)
    {
      case LevelMessageBox.Selections.Level:
        if (iDirection == ControllerDirection.Left)
        {
          this.ChangeSelection(-1);
          break;
        }
        if (iDirection == ControllerDirection.Right)
        {
          this.ChangeSelection(1);
          break;
        }
        if (!(iDirection == ControllerDirection.Down | iDirection == ControllerDirection.Up))
          break;
        this.mSelection = LevelMessageBox.Selections.OkButton;
        this.mOkItem.Selected = true;
        this.mCancelItem.Selected = false;
        break;
      case LevelMessageBox.Selections.OkButton:
        if (iDirection == ControllerDirection.Up | iDirection == ControllerDirection.Down | iDirection == ControllerDirection.Left)
        {
          this.mSelection = LevelMessageBox.Selections.Level;
          this.mOkItem.Selected = false;
          this.mCancelItem.Selected = false;
          break;
        }
        if (iDirection != ControllerDirection.Right)
          break;
        this.mSelection = LevelMessageBox.Selections.CancelButton;
        this.mOkItem.Selected = false;
        this.mCancelItem.Selected = true;
        break;
      case LevelMessageBox.Selections.CancelButton:
        if (iDirection == ControllerDirection.Up)
        {
          this.mSelection = LevelMessageBox.Selections.Level;
          this.mOkItem.Selected = false;
          this.mCancelItem.Selected = false;
          break;
        }
        if (!(iDirection == ControllerDirection.Right | iDirection == ControllerDirection.Left))
          break;
        this.mSelection = LevelMessageBox.Selections.OkButton;
        this.mOkItem.Selected = true;
        this.mCancelItem.Selected = false;
        break;
    }
  }

  public override void OnSelect(Controller iSender)
  {
    switch (this.mSelection)
    {
      case LevelMessageBox.Selections.Level:
      case LevelMessageBox.Selections.OkButton:
        if (this.mCustomLevel != HackHelper.License.Yes && this.mCustomLevel != HackHelper.License.Custom)
          break;
        this.mSelectedTexture = this.mCurrentTexture;
        if (this.mComplete != null)
          this.mComplete(iSender, this.mLevelType, this.mLevelSelection, this.mCustomLevel != HackHelper.License.Yes);
        this.Kill();
        break;
      case LevelMessageBox.Selections.CancelButton:
        this.Kill();
        break;
    }
  }

  public override void OnMouseMove(MouseState iNewState, MouseState iOldState)
  {
    if ((double) iNewState.X <= (double) this.mCenter.X - (double) this.mSize.X * 0.5 || (double) iNewState.X >= (double) this.mCenter.X + (double) this.mSize.X * 0.5 || (double) iNewState.Y <= (double) this.mCenter.Y - (double) this.mSize.Y * 0.5 || (double) iNewState.Y >= (double) this.mCenter.Y + (double) this.mSize.Y * 0.5)
      return;
    if (this.mLeftArrow.InsideBounds(iNewState))
    {
      this.mOkItem.Selected = false;
      this.mCancelItem.Selected = false;
      this.mSelection = LevelMessageBox.Selections.Level;
      this.mLeftArrow.Selected = true;
      this.mRightArrow.Selected = false;
    }
    else if (this.mRightArrow.InsideBounds(iNewState))
    {
      this.mOkItem.Selected = false;
      this.mCancelItem.Selected = false;
      this.mSelection = LevelMessageBox.Selections.Level;
      this.mLeftArrow.Selected = false;
      this.mRightArrow.Selected = true;
    }
    else if (this.mOkItem.Enabled && this.mOkItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mOkItem.Selected = true;
      this.mCancelItem.Selected = false;
      this.mSelection = LevelMessageBox.Selections.OkButton;
      this.mLeftArrow.Selected = false;
      this.mRightArrow.Selected = false;
    }
    else if (this.mCancelItem.Enabled && this.mCancelItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mOkItem.Selected = false;
      this.mCancelItem.Selected = true;
      this.mSelection = LevelMessageBox.Selections.CancelButton;
      this.mLeftArrow.Selected = false;
      this.mRightArrow.Selected = false;
    }
    else
    {
      this.mOkItem.Selected = false;
      this.mCancelItem.Selected = false;
      this.mSelection = LevelMessageBox.Selections.Level;
      this.mLeftArrow.Selected = false;
      this.mRightArrow.Selected = false;
    }
  }

  public override void OnMouseClick(MouseState iNewState, MouseState iOldState)
  {
    if (iNewState.LeftButton != ButtonState.Released || iOldState.LeftButton != ButtonState.Pressed)
      return;
    if (this.mOkItem.Enabled && this.mOkItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mSelection = LevelMessageBox.Selections.OkButton;
      this.OnSelect((Controller) ControlManager.Instance.MenuController);
    }
    else if (this.mCancelItem.Enabled && this.mCancelItem.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mSelection = LevelMessageBox.Selections.CancelButton;
      this.OnSelect((Controller) ControlManager.Instance.MenuController);
    }
    else if (this.mLeftArrow.Enabled && this.mLeftArrow.InsideBounds((float) iNewState.X, (float) iNewState.Y))
    {
      this.mSelection = LevelMessageBox.Selections.Level;
      this.ChangeSelection(-1);
    }
    else
    {
      if (!this.mRightArrow.Enabled || !this.mRightArrow.InsideBounds((float) iNewState.X, (float) iNewState.Y))
        return;
      this.mSelection = LevelMessageBox.Selections.Level;
      this.ChangeSelection(1);
    }
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (this.mCustomLevel != HackHelper.License.Pending)
      return;
    HackHelper.License license = HackHelper.CheckLicense(this.mLevelType != GameType.Challenge ? LevelManager.Instance.Versus[this.mLevelSelection] : LevelManager.Instance.Challenges[this.mLevelSelection]);
    switch (license)
    {
      case HackHelper.License.No:
        this.ChangeSelection(this.mLastStep);
        return;
      case HackHelper.License.Custom:
        if (this.mAllowCustom)
          break;
        goto case HackHelper.License.No;
    }
    this.mCustomLevel = license;
  }

  public override void Draw(float iDeltaTime)
  {
    Matrix identity = Matrix.Identity;
    if (this.mCustomLevel == HackHelper.License.Custom)
    {
      identity.M11 = identity.M22 = identity.M33 = 1f;
      identity.M44 = 1f;
      identity.M41 = 0.0f;
      identity.M42 = this.mCenter.Y;
      MessageBox.sGUIBasicEffect.Transform = identity;
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      vector4.W = this.mAlpha * this.mTextureAlpha;
      MessageBox.sGUIBasicEffect.Color = vector4;
      MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
      MessageBox.sGUIBasicEffect.Texture = (Texture) MessageBox.sTexture;
      MessageBox.sGUIBasicEffect.TextureEnabled = true;
      MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(MessageBox.sVertexBuffer, 0, 16 /*0x10*/);
      MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = MessageBox.sVertexDeclaration;
      MessageBox.sGUIBasicEffect.Begin();
      MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].Begin();
      MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      vector4 = MenuItem.COLOR;
      vector4.W *= this.mTextureAlpha * this.mTextureAlpha;
      MessageBox.sGUIBasicEffect.Color = vector4;
      this.mCustomLevelText.Draw(MessageBox.sGUIBasicEffect, 20f, this.mCenter.Y - 200f);
      MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
      MessageBox.sGUIBasicEffect.End();
    }
    base.Draw(iDeltaTime);
    float iY1 = this.mCenter.Y - this.mSize.Y * 0.5f + this.mTitleFontHeight + LevelMessageBox.TITLE_PADDING;
    Vector2 vector2_1 = new Vector2();
    vector2_1.Y = iY1 + this.mTitleFontHeight * 0.5f;
    vector2_1.X = this.mCenter.X - this.mSize.X * 0.5f + LevelMessageBox.ARROW_PADDING;
    this.mLeftArrow.Position = vector2_1;
    vector2_1.X = this.mCenter.X + this.mSize.X * 0.5f - LevelMessageBox.ARROW_PADDING;
    this.mRightArrow.Position = vector2_1;
    if (this.mCustomLevel == HackHelper.License.Pending || this.mCustomLevel == HackHelper.License.No)
    {
      this.mLoadingDotTimer -= iDeltaTime;
      while ((double) this.mLoadingDotTimer < 0.0)
      {
        this.mLoadingDotTimer += 0.5f;
        int index;
        if (this.mLoadingText.Characters[this.mLoadingTextLength] == char.MinValue)
          index = this.mLoadingTextLength;
        else if (this.mLoadingText.Characters[this.mLoadingTextLength + 1] == char.MinValue)
          index = this.mLoadingTextLength + 1;
        else if (this.mLoadingText.Characters[this.mLoadingTextLength + 2] == char.MinValue)
        {
          index = this.mLoadingTextLength + 2;
        }
        else
        {
          index = -1;
          this.mLoadingText.Characters[this.mLoadingTextLength] = char.MinValue;
        }
        if (index > 0)
        {
          this.mLoadingText.Characters[index] = '.';
          this.mLoadingText.Characters[index + 1] = char.MinValue;
        }
        this.mLoadingText.MarkAsDirty();
      }
      Vector2 vector2_2 = this.mLoadingText.Font.MeasureText(this.mLoadingText.Characters, true, this.mLoadingTextLength);
      this.mLoadingText.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X - vector2_2.X * 0.5f, this.mCenter.Y - 50f);
    }
    else
    {
      MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
      MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
      this.mLevelTitle.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, iY1);
      float iY2 = iY1 + this.mLevelTitleSpacing;
      if (this.mCurrentTexture != null && !this.mCurrentTexture.IsDisposed)
      {
        this.mTextureAlpha = Math.Min(this.mTextureAlpha + iDeltaTime * 4f, 1f);
        lock (this.mCurrentTexture)
        {
          iY2 += (float) this.mCurrentTexture.Height * 0.25f + LevelMessageBox.IMAGE_PADDING;
          identity.M42 = iY2;
          identity.M41 = this.mCenter.X;
          identity.M11 = (float) this.mCurrentTexture.Width * 0.5f;
          identity.M22 = (float) this.mCurrentTexture.Height * 0.5f;
          MessageBox.sGUIBasicEffect.Transform = identity;
          MessageBox.sGUIBasicEffect.GraphicsDevice.Vertices[0].SetSource(LevelMessageBox.sLevelVertexBuffer, 0, VertexPositionTexture.SizeInBytes);
          MessageBox.sGUIBasicEffect.GraphicsDevice.VertexDeclaration = LevelMessageBox.sLevelVertexDeclaration;
          MessageBox.sGUIBasicEffect.VertexColorEnabled = false;
          MessageBox.sGUIBasicEffect.Texture = (Texture) this.mCurrentTexture;
          MessageBox.sGUIBasicEffect.TextureEnabled = true;
          MessageBox.sGUIBasicEffect.TextureScale = Vector2.One;
          MessageBox.sGUIBasicEffect.TextureOffset = Vector2.Zero;
          MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mTextureAlpha * this.mAlpha);
          MessageBox.sGUIBasicEffect.Saturation = 1f;
          MessageBox.sGUIBasicEffect.CommitChanges();
          MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
          if (this.mCustomLevel == HackHelper.License.Custom)
          {
            MessageBox.sGUIBasicEffect.Texture = (Texture) this.mCustomLevelTexture;
            MessageBox.sGUIBasicEffect.CommitChanges();
            MessageBox.sGUIBasicEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
          }
          iY2 += (float) this.mCurrentTexture.Height * 0.25f;
        }
      }
      else
        iY2 += 256f;
      MessageBox.sGUIBasicEffect.Color = new Vector4(1f, 1f, 1f, this.mAlpha);
      MessageBox.sGUIBasicEffect.VertexColorEnabled = true;
      this.mLevelDescription.Draw(MessageBox.sGUIBasicEffect, this.mCenter.X, iY2);
    }
    this.mOkItem.Alpha = this.mAlpha;
    this.mOkItem.Draw(MessageBox.sGUIBasicEffect);
    this.mCancelItem.Alpha = this.mAlpha;
    this.mCancelItem.Draw(MessageBox.sGUIBasicEffect);
    this.mLeftArrow.Draw(MessageBox.sGUIBasicEffect, 48f);
    this.mRightArrow.Draw(MessageBox.sGUIBasicEffect, 48f);
    MessageBox.sGUIBasicEffect.CurrentTechnique.Passes[0].End();
    MessageBox.sGUIBasicEffect.End();
  }

  private enum Selections
  {
    Level,
    OkButton,
    CancelButton,
  }
}
