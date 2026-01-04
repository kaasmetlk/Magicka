// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxLoginWindow
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.Achievements;

internal class PdxLoginWindow : PdxWidgetWindow
{
  private static PdxLoginWindow sSingelton;
  private static volatile object sSingeltonLock = new object();
  private GUIBasicEffect mEffect;
  private PdxLoginWindow.RenderData[] mRenderData;
  private VertexBuffer mVertices;
  private VertexDeclaration mDeclaration;
  private float mProgress;
  private Text mErrorText;
  private PdxButton mCloseButton;
  private PdxButton mEnterButton;
  private Text mLoginHeaderText;
  private Text mUsernameText;
  private Text mPasswordText;
  private int mInputError;
  private Text mInputErrorText;
  private PdxTextBox mUsernameTextBox;
  private PdxTextBox mPasswordTextBox;

  public static PdxLoginWindow Instance
  {
    get
    {
      if (PdxLoginWindow.sSingelton == null)
      {
        lock (PdxLoginWindow.sSingeltonLock)
        {
          if (PdxLoginWindow.sSingelton == null)
            PdxLoginWindow.sSingelton = new PdxLoginWindow();
        }
      }
      return PdxLoginWindow.sSingelton;
    }
  }

  private PdxLoginWindow()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      this.mEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    Vector4[] vector4Array = new Vector4[16 /*0x10*/];
    int iIndex = 0;
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_middle);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_top);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_bottom);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.popup_input_error);
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, vector4Array.Length * 4 * 4, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(vector4Array);
      this.mDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.PDX_UI);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_Edit);
    this.mErrorText = new Text(512 /*0x0200*/, font1, TextAlign.Left, false);
    this.mErrorText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.ERROR_SERVICE_UNAVAILABLE));
    this.mCloseButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font1, AchievementsManager.BTN_CLOSE);
    this.mEnterButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[18], PdxWidgetWindow.sRectangles[19], font2, AchievementsManager.BTN_ENTER);
    this.mLoginHeaderText = new Text(64 /*0x40*/, font1, TextAlign.Center, false);
    this.mUsernameText = new Text(32 /*0x20*/, font1, TextAlign.Left, false);
    this.mPasswordText = new Text(32 /*0x20*/, font1, TextAlign.Left, false);
    this.mInputErrorText = new Text(64 /*0x40*/, font1, TextAlign.Left, false);
    this.mUsernameTextBox = new PdxTextBox(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[13], PdxWidgetWindow.sRectangles[12], PdxWidgetWindow.sRectangles[42], font2, 15);
    this.mUsernameTextBox.Active = true;
    this.mPasswordTextBox = new PdxTextBox(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[13], PdxWidgetWindow.sRectangles[12], PdxWidgetWindow.sRectangles[42], font2, 50);
    this.mPasswordTextBox.Mask = true;
    this.mRenderData = new PdxLoginWindow.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new PdxLoginWindow.RenderData();
  }

  public override void Show()
  {
    if (AchievementsManager.Instance.LoggedIn)
      return;
    if (!this.mVisible)
      AchievementsManager.Instance.SetLanguage(LanguageManager.Instance.CurrentLanguage);
    base.Show();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    if (!AchievementsManager.Instance.Busy & AchievementsManager.Instance.LoggedIn)
      this.Hide();
    this.mProgress = (float) (((double) this.mProgress + (double) iDeltaTime) % 2.0);
    this.mUsernameTextBox.Update(iDeltaTime);
    this.mPasswordTextBox.Update(iDeltaTime);
    string logInError = AchievementsManager.Instance.LogInError;
    if (string.IsNullOrEmpty(logInError))
    {
      if (this.mInputError != 0)
      {
        this.mInputError = 0;
        this.mInputErrorText.Clear();
      }
    }
    else
    {
      int hashCodeCustom = logInError.GetHashCodeCustom();
      if (hashCodeCustom != this.mInputError)
      {
        this.mInputError = hashCodeCustom;
        this.mInputErrorText.SetText(this.mInputErrorText.Font.Wrap(AchievementsManager.Instance.GetTranslation(this.mInputError), PdxWidgetWindow.sRectangles[6].Width - 45, true));
      }
    }
    PdxLoginWindow.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.Alpha = this.mAlpha;
    bool busy = AchievementsManager.Instance.Busy;
    iObject.DoProgress = busy | !this.mVisible;
    iObject.DoServiceUnavailable = AchievementsManager.Instance.Status != ServerRequestResult.SUCCESS;
    iObject.Progress = this.mProgress;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public override void OnLanguageChanged()
  {
    base.OnLanguageChanged();
    AchievementsManager instance = AchievementsManager.Instance;
    this.mErrorText.SetText(instance.GetTranslation(AchievementsManager.ERROR_SERVICE_UNAVAILABLE));
    this.mLoginHeaderText.SetText(instance.GetTranslation(AchievementsManager.LOGIN_HEADER));
    this.mUsernameText.SetText(instance.GetTranslation(AchievementsManager.USERNAME));
    this.mPasswordText.SetText(instance.GetTranslation(AchievementsManager.PASSWORD));
    this.mCloseButton.OnLanguageChanged();
    this.mEnterButton.OnLanguageChanged();
  }

  public override void OnMouseDown(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos))
      this.mCloseButton.State |= PdxButton.ButtonState.Down;
    if (this.mEnterButton.InsideBounds(ref iMousePos))
      this.mEnterButton.State |= PdxButton.ButtonState.Down;
    if (this.mUsernameTextBox.InsideBounds(ref iMousePos))
    {
      this.mPasswordTextBox.Active = false;
      this.mUsernameTextBox.Active = true;
    }
    else
    {
      if (!this.mPasswordTextBox.InsideBounds(ref iMousePos))
        return;
      this.mUsernameTextBox.Active = false;
      this.mPasswordTextBox.Active = true;
    }
  }

  public override void OnMouseUp(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos) & (this.mCloseButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
      this.Hide();
    this.mCloseButton.State &= ~PdxButton.ButtonState.Down;
    if (this.mEnterButton.InsideBounds(ref iMousePos) & (this.mEnterButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
      AchievementsManager.Instance.LogIn(this.mUsernameTextBox.String, this.mPasswordTextBox.String);
    this.mEnterButton.State &= ~PdxButton.ButtonState.Down;
  }

  public override void OnMouseMove(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos))
      this.mCloseButton.State |= PdxButton.ButtonState.Over;
    else
      this.mCloseButton.State &= ~PdxButton.ButtonState.Over;
    if (this.mEnterButton.InsideBounds(ref iMousePos))
      this.mEnterButton.State |= PdxButton.ButtonState.Over;
    else
      this.mEnterButton.State &= ~PdxButton.ButtonState.Over;
  }

  public override void OnKeyDown(KeyData iData)
  {
    switch (iData.Key)
    {
      case Keys.Tab:
        if (this.mUsernameTextBox.Active)
        {
          this.mUsernameTextBox.Active = false;
          this.mPasswordTextBox.Active = true;
          break;
        }
        this.mPasswordTextBox.Active = false;
        this.mUsernameTextBox.Active = true;
        break;
      case Keys.Enter:
        AchievementsManager.Instance.LogIn(this.mUsernameTextBox.String, this.mPasswordTextBox.String);
        break;
      case Keys.Left:
        if (this.mUsernameTextBox.Active)
        {
          --this.mUsernameTextBox.Cursor;
          break;
        }
        if (!this.mPasswordTextBox.Active)
          break;
        --this.mPasswordTextBox.Cursor;
        break;
      case Keys.Right:
        if (this.mUsernameTextBox.Active)
        {
          ++this.mUsernameTextBox.Cursor;
          break;
        }
        if (!this.mPasswordTextBox.Active)
          break;
        ++this.mPasswordTextBox.Cursor;
        break;
      case Keys.Delete:
        if (this.mUsernameTextBox.Active)
        {
          this.mUsernameTextBox.Delete();
          break;
        }
        if (!this.mPasswordTextBox.Active)
          break;
        this.mPasswordTextBox.Delete();
        break;
    }
  }

  public override void OnKeyPress(char iChar)
  {
    if (this.mUsernameTextBox.Active)
    {
      this.mUsernameTextBox.AppendChar(iChar);
    }
    else
    {
      if (!this.mPasswordTextBox.Active)
        return;
      this.mPasswordTextBox.AppendChar(iChar);
    }
  }

  private class RenderData : IRenderableGUIObject
  {
    public float Alpha;
    public bool DoProgress;
    public bool DoServiceUnavailable;
    private Matrix mTransform = Matrix.Identity;
    private static Vector4[] mTmpVertices;
    public float Progress;

    public RenderData()
    {
      if (PdxLoginWindow.RenderData.mTmpVertices != null)
        return;
      PdxLoginWindow.RenderData.mTmpVertices = new Vector4[4];
    }

    public void Draw(float iDeltaTime)
    {
      PdxLoginWindow instance = PdxLoginWindow.Instance;
      GUIBasicEffect mEffect = instance.mEffect;
      Point screenSize = RenderManager.Instance.ScreenSize;
      mEffect.SetScreenSize(screenSize.X, screenSize.Y);
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      vector4.W = this.Alpha;
      mEffect.Color = vector4;
      mEffect.Texture = (Texture) PdxWidgetWindow.sTexture;
      mEffect.TextureEnabled = true;
      mEffect.GraphicsDevice.Vertices[0].SetSource(instance.mVertices, 0, 16 /*0x10*/);
      mEffect.GraphicsDevice.VertexDeclaration = instance.mDeclaration;
      Vector2 vector2_1 = new Vector2();
      vector2_1.X = (float) ((screenSize.X - 800) / 2);
      vector2_1.Y = (float) ((screenSize.Y - 600) / 2);
      mEffect.Begin();
      mEffect.CurrentTechnique.Passes[0].Begin();
      this.DrawQuad(mEffect, vector2_1.X + 13f, vector2_1.Y + 118f, 0);
      this.DrawQuad(mEffect, vector2_1.X + 0.0f, vector2_1.Y + 0.0f, 1);
      this.DrawQuad(mEffect, vector2_1.X + 0.0f, vector2_1.Y + 446f, 2);
      Vector2 vector2_2 = new Vector2();
      if (this.DoProgress)
      {
        Rectangle sRectangle1 = PdxWidgetWindow.sRectangles[5];
        Rectangle sRectangle2 = PdxWidgetWindow.sRectangles[4];
        int num1 = (int) ((double) this.Progress * (double) sRectangle1.Width);
        if (num1 <= sRectangle1.Width)
        {
          sRectangle1.Width = num1;
          sRectangle2.X += num1;
          sRectangle2.Width -= num1;
          this.DrawQuad(mEffect, vector2_1.X + 330f, vector2_1.Y + 210f, ref sRectangle1);
          this.DrawQuad(mEffect, vector2_1.X + 330f + (float) num1, vector2_1.Y + 210f, ref sRectangle2);
        }
        else
        {
          int num2 = num1 - sRectangle1.Width;
          sRectangle2.Width = num2;
          sRectangle1.X += num2;
          sRectangle1.Width -= num2;
          this.DrawQuad(mEffect, vector2_1.X + 330f, vector2_1.Y + 210f, ref sRectangle2);
          this.DrawQuad(mEffect, vector2_1.X + 330f + (float) num2, vector2_1.Y + 210f, ref sRectangle1);
        }
      }
      else if (this.DoServiceUnavailable)
      {
        instance.mErrorText.Draw(mEffect, vector2_1.X + 200f, vector2_1.Y + 200f);
      }
      else
      {
        if (instance.mInputErrorText.Characters[0] != char.MinValue)
        {
          vector4.W = this.Alpha * this.Alpha;
          mEffect.Color = vector4;
          this.DrawQuad(mEffect, vector2_1.X + 530f, vector2_1.Y + 195f, 3);
          vector4.X = vector4.Y = vector4.Z = 0.0f;
          mEffect.Color = vector4;
          instance.mInputErrorText.Draw(mEffect, vector2_1.X + 560f, vector2_1.Y + 205f);
        }
        vector4.X = vector4.Y = vector4.Z = 1f;
        vector2_2.X = vector2_1.X + 400f;
        vector2_2.Y = vector2_1.Y + 170f;
        vector4.W = (float) ((double) this.Alpha * (double) this.Alpha * 0.87843137979507446);
        mEffect.Color = vector4;
        instance.mLoginHeaderText.Draw(mEffect, vector2_2.X, vector2_2.Y);
        vector2_2.X = vector2_1.X + 300f;
        vector2_2.Y = vector2_1.Y + 210f;
        vector4.W = (float) ((double) this.Alpha * (double) this.Alpha * 0.56470590829849243);
        mEffect.Color = vector4;
        instance.mUsernameText.Draw(mEffect, vector2_2.X, vector2_2.Y);
        vector2_2.Y = vector2_1.Y + 285f;
        instance.mPasswordText.Draw(mEffect, vector2_2.X, vector2_2.Y);
        vector2_2.X = vector2_1.X + 295f;
        vector2_2.Y = vector2_1.Y + 233f;
        instance.mUsernameTextBox.Position = vector2_2;
        instance.mUsernameTextBox.Draw(mEffect, this.Alpha * this.Alpha);
        vector2_2.X = vector2_1.X + 295f;
        vector2_2.Y = vector2_1.Y + 308f;
        instance.mPasswordTextBox.Position = vector2_2;
        instance.mPasswordTextBox.Draw(mEffect, this.Alpha * this.Alpha);
        vector2_2.X = vector2_1.X + 330f;
        vector2_2.Y = vector2_1.Y + 367f;
        instance.mEnterButton.Position = vector2_2;
        instance.mEnterButton.Draw(mEffect, this.Alpha * this.Alpha);
      }
      vector2_2.X = vector2_1.X + 580f;
      vector2_2.Y = vector2_1.Y + 85f;
      instance.mCloseButton.Position = vector2_2;
      instance.mCloseButton.Draw(mEffect, this.Alpha * this.Alpha);
      mEffect.CurrentTechnique.Passes[0].End();
      mEffect.End();
    }

    public int ZIndex => 2147483645;

    private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, int iId)
    {
      this.mTransform.M41 = iX;
      this.mTransform.M42 = iY;
      iEffect.Transform = this.mTransform;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iId * 4, 2);
    }

    private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, ref Rectangle iSourceRect)
    {
      this.mTransform.M41 = iX;
      this.mTransform.M42 = iY;
      iEffect.Transform = this.mTransform;
      iEffect.CommitChanges();
      PdxLoginWindow.RenderData.mTmpVertices[0].X = 0.0f;
      PdxLoginWindow.RenderData.mTmpVertices[0].Y = 0.0f;
      PdxLoginWindow.RenderData.mTmpVertices[0].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
      PdxLoginWindow.RenderData.mTmpVertices[0].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
      PdxLoginWindow.RenderData.mTmpVertices[1].X = (float) iSourceRect.Width;
      PdxLoginWindow.RenderData.mTmpVertices[1].Y = 0.0f;
      PdxLoginWindow.RenderData.mTmpVertices[1].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
      PdxLoginWindow.RenderData.mTmpVertices[1].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
      PdxLoginWindow.RenderData.mTmpVertices[2].X = (float) iSourceRect.Width;
      PdxLoginWindow.RenderData.mTmpVertices[2].Y = (float) iSourceRect.Height;
      PdxLoginWindow.RenderData.mTmpVertices[2].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
      PdxLoginWindow.RenderData.mTmpVertices[2].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
      PdxLoginWindow.RenderData.mTmpVertices[3].X = 0.0f;
      PdxLoginWindow.RenderData.mTmpVertices[3].Y = (float) iSourceRect.Height;
      PdxLoginWindow.RenderData.mTmpVertices[3].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
      PdxLoginWindow.RenderData.mTmpVertices[3].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
      iEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, PdxLoginWindow.RenderData.mTmpVertices, 0, 2);
    }
  }
}
