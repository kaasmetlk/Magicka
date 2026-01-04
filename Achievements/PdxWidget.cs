// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.PdxWidget
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Achievements;

internal class PdxWidget : PdxWidgetWindow
{
  private static PdxWidget sSingelton;
  private static volatile object sSingeltonLock = new object();
  private GUIBasicEffect mEffect;
  private PdxWidget.RenderData[] mRenderData;
  private float mProgress;
  private VertexBuffer mVertices;
  private VertexDeclaration mDeclaration;
  private PdxButton mLogoutButton;
  private PdxButton mCloseButton;
  private PdxButton mLeftButton;
  private PdxButton mRightButton;
  private bool mDoScroll;
  private int mScroll;
  private bool mAchievements = true;
  private Rectangle mAchievementsRect;
  private Rectangle mGamesRect;
  private Text mRankText;
  private float mRankWidth;
  private Text mRankNumText;
  private float mRankNumWidth;
  private Text mAchievementsText;
  private Text mGamesText;
  private int mPopup = -1;
  private Vector2 mPopupPos;
  private Rectangle mAchievementPopupRect;
  private Rectangle mGamePopupRect;
  private bool mPopupAchievementEarned;
  private Text mPopupHeaderText;
  private Text mPopupBodyText;
  private Text mPopupDateText;
  private Text mPopupPointsText;

  public static PdxWidget Instance
  {
    get
    {
      if (PdxWidget.sSingelton == null)
      {
        lock (PdxWidget.sSingeltonLock)
        {
          if (PdxWidget.sSingelton == null)
            PdxWidget.sSingelton = new PdxWidget();
        }
      }
      return PdxWidget.sSingelton;
    }
  }

  private PdxWidget()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
      this.mEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
    Vector4[] vector4Array = new Vector4[84];
    int iIndex = 0;
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_0);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_1);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_2);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_3);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_4);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_5);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_6);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_7);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_8);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.points_number_9);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_middle);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_top);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_profilebox_bottom);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.frame_points);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.menu_highlight_left);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.menu_highlight_right);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.badge_achievement_off);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.badge_achievement_on);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.badge_game);
    PdxWidgetWindow.CreateVertices(vector4Array, ref iIndex, PdxWidgetWindow.Textures.popup_info);
    vector4Array[iIndex].X = 0.0f;
    vector4Array[iIndex].Y = 0.0f;
    vector4Array[iIndex].Z = 0.0f;
    vector4Array[iIndex].W = 0.0f;
    ++iIndex;
    vector4Array[iIndex].X = 1f;
    vector4Array[iIndex].Y = 0.0f;
    vector4Array[iIndex].Z = 1f;
    vector4Array[iIndex].W = 0.0f;
    ++iIndex;
    vector4Array[iIndex].X = 1f;
    vector4Array[iIndex].Y = 1f;
    vector4Array[iIndex].Z = 1f;
    vector4Array[iIndex].W = 1f;
    ++iIndex;
    vector4Array[iIndex].X = 0.0f;
    vector4Array[iIndex].Y = 1f;
    vector4Array[iIndex].Z = 0.0f;
    vector4Array[iIndex].W = 1f;
    ++iIndex;
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
    this.mAchievementsRect = PdxWidgetWindow.sRectangles[16 /*0x10*/];
    this.mGamesRect = PdxWidgetWindow.sRectangles[14];
    this.mGamePopupRect = PdxWidgetWindow.sRectangles[8];
    this.mAchievementPopupRect.Width = 75;
    this.mAchievementPopupRect.Height = 75;
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Small);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Small_Bold);
    BitmapFont font3 = FontManager.Instance.GetFont(MagickaFont.PDX_Points);
    this.mLogoutButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font1, AchievementsManager.BTN_LOGOUT);
    this.mCloseButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[40], PdxWidgetWindow.sRectangles[41], font1, AchievementsManager.BTN_CLOSE);
    this.mLeftButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[22], PdxWidgetWindow.sRectangles[23], (BitmapFont) null, 0);
    this.mLeftButton.State |= PdxButton.ButtonState.Over;
    this.mRightButton = new PdxButton(PdxWidgetWindow.sTexture, PdxWidgetWindow.sRectangles[24], PdxWidgetWindow.sRectangles[25], (BitmapFont) null, 0);
    this.mRightButton.State |= PdxButton.ButtonState.Over;
    this.mRankText = new Text(32 /*0x20*/, font1, TextAlign.Left, false);
    this.mRankNumText = new Text(32 /*0x20*/, font1, TextAlign.Left, false);
    this.mAchievementsText = new Text(32 /*0x20*/, font1, TextAlign.Center, false);
    this.mGamesText = new Text(32 /*0x20*/, font1, TextAlign.Center, false);
    this.mPopupHeaderText = new Text(64 /*0x40*/, font2, TextAlign.Left, false);
    this.mPopupBodyText = new Text(256 /*0x0100*/, font1, TextAlign.Left, false);
    this.mPopupDateText = new Text(64 /*0x40*/, font1, TextAlign.Right, false);
    this.mPopupPointsText = new Text(32 /*0x20*/, font3, TextAlign.Left, false);
    this.mRenderData = new PdxWidget.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new PdxWidget.RenderData();
  }

  public override void Show()
  {
    if (!AchievementsManager.Instance.LoggedIn)
      return;
    this.mAchievements = true;
    this.mScroll = 0;
    this.mDoScroll = AchievementsManager.Instance.Achievements.Count > 10;
    base.Show();
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mProgress = (float) (((double) this.mProgress + (double) iDeltaTime) % 2.0);
    Point screenSize = RenderManager.Instance.ScreenSize;
    this.mAchievementsRect.X = (screenSize.X - 800) / 2 + 180;
    this.mAchievementsRect.Y = (screenSize.Y - 600) / 2 + 119;
    this.mGamesRect.X = (screenSize.X - 800) / 2 + 397;
    this.mGamesRect.Y = (screenSize.Y - 600) / 2 + 119;
    PdxWidget.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.Alpha = this.mAlpha;
    iObject.Progress = this.mProgress;
    iObject.PopupAchievementEarned = this.mPopupAchievementEarned;
    iObject.DoProgress = AchievementsManager.Instance.Busy;
    iObject.Points = AchievementsManager.Instance.TotalPoints;
    iObject.DoScroll = this.mDoScroll;
    iObject.Scroll = this.mScroll;
    iObject.DoPopup = this.mPopup >= 0;
    iObject.MousePos = this.mPopupPos;
    iObject.Achievements = this.mAchievements;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public override void OnLanguageChanged()
  {
    base.OnLanguageChanged();
    this.mLogoutButton.OnLanguageChanged();
    this.mCloseButton.OnLanguageChanged();
  }

  public override void OnMouseDown(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos))
      this.mCloseButton.State |= PdxButton.ButtonState.Down;
    if (this.mLogoutButton.InsideBounds(ref iMousePos))
      this.mLogoutButton.State |= PdxButton.ButtonState.Down;
    if (this.mDoScroll)
    {
      if (this.mLeftButton.InsideBounds(ref iMousePos))
        this.mLeftButton.State |= PdxButton.ButtonState.Down;
      if (this.mRightButton.InsideBounds(ref iMousePos))
        this.mRightButton.State |= PdxButton.ButtonState.Down;
    }
    if (this.mAchievementsRect.Contains((int) iMousePos.X, (int) iMousePos.Y))
    {
      this.mAchievements = true;
      this.mScroll = 0;
      this.mDoScroll = AchievementsManager.Instance.Achievements.Count > 10;
    }
    else
    {
      if (!this.mGamesRect.Contains((int) iMousePos.X, (int) iMousePos.Y))
        return;
      this.mAchievements = false;
      this.mScroll = 0;
      this.mDoScroll = AchievementsManager.Instance.Games.Count > 10;
    }
  }

  public override void OnMouseUp(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos) & (this.mCloseButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
      this.Hide();
    this.mCloseButton.State &= ~PdxButton.ButtonState.Down;
    if (this.mLogoutButton.InsideBounds(ref iMousePos) & (this.mLogoutButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
    {
      AchievementsManager.Instance.LogOut();
      this.Hide();
    }
    this.mLogoutButton.State &= ~PdxButton.ButtonState.Down;
    if (this.mDoScroll)
    {
      if (this.mLeftButton.InsideBounds(ref iMousePos) & (this.mLeftButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
      {
        int num = this.mScroll - 10;
        this.mScroll = num < 0 ? 0 : num;
      }
      if (this.mRightButton.InsideBounds(ref iMousePos) & (this.mRightButton.State & PdxButton.ButtonState.Down) != PdxButton.ButtonState.Up)
      {
        int num1 = this.mScroll + 10;
        int num2 = this.mAchievements ? AchievementsManager.Instance.Achievements.Count : AchievementsManager.Instance.Games.Count;
        if (num1 < num2)
          this.mScroll = num1;
      }
    }
    this.mLeftButton.State &= ~PdxButton.ButtonState.Down;
    this.mRightButton.State &= ~PdxButton.ButtonState.Down;
  }

  public override void OnMouseMove(ref Vector2 iMousePos)
  {
    if (this.mCloseButton.InsideBounds(ref iMousePos))
      this.mCloseButton.State |= PdxButton.ButtonState.Over;
    else
      this.mCloseButton.State &= ~PdxButton.ButtonState.Over;
    if (this.mLogoutButton.InsideBounds(ref iMousePos))
      this.mLogoutButton.State |= PdxButton.ButtonState.Over;
    else
      this.mLogoutButton.State &= ~PdxButton.ButtonState.Over;
    AchievementsManager instance = AchievementsManager.Instance;
    Point screenSize = RenderManager.Instance.ScreenSize;
    screenSize.X = (screenSize.X - 800) / 2;
    screenSize.Y = (screenSize.Y - 600) / 2;
    int num1 = -1;
    Rectangle rectangle = this.mAchievements ? this.mAchievementPopupRect : this.mGamePopupRect;
    int num2 = this.mAchievements ? instance.Achievements.Count : instance.Games.Count;
    rectangle.Y = screenSize.Y + (this.mAchievements ? 204 : 170);
    for (int index1 = 0; index1 < 2; ++index1)
    {
      rectangle.X = screenSize.X + (this.mAchievements ? 78 : 55);
      for (int index2 = 0; index2 < 5; ++index2)
      {
        int num3 = index1 * 5 + index2 + this.mScroll;
        if (num3 >= 0 & num3 < num2 && rectangle.Contains((int) iMousePos.X, (int) iMousePos.Y))
        {
          num1 = num3;
          break;
        }
        rectangle.X += 137;
      }
      rectangle.Y += 137;
    }
    if (this.mPopup != num1)
    {
      this.mPopup = num1;
      if (num1 >= 0)
      {
        if (this.mAchievements)
        {
          AchievementData achievement = instance.Achievements[this.mPopup];
          this.mPopupHeaderText.SetText(achievement.Name);
          this.mPopupBodyText.SetText(this.mPopupBodyText.Font.Wrap(achievement.Desc, 238, true));
          this.mPopupDateText.SetText(achievement.DateAchieved.Year != 1970 ? AchievementsManager.Instance.GetTranslation(AchievementsManager.ACHIEVEMENT_EARNED).Replace("%s", achievement.DateAchieved.ToString()) : AchievementsManager.Instance.GetTranslation(AchievementsManager.EARNED_THIS_SESSION));
          this.mPopupPointsText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.NUM_PP).Replace("%d", achievement.Points.ToString()));
          this.mPopupAchievementEarned = achievement.Achieved;
        }
        else
        {
          GameData game = instance.Games[this.mPopup];
          this.mPopupHeaderText.SetText(game.Name);
          this.mPopupBodyText.SetText(this.mPopupBodyText.Font.Wrap(game.Desc, 238, true));
        }
      }
    }
    this.mPopupPos = iMousePos;
  }

  public override void OnKeyDown(KeyData iData)
  {
  }

  public override void OnKeyPress(char iChar)
  {
  }

  internal void OnProfileUpdate()
  {
    this.mRankText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.YOUR_RANK));
    this.mRankNumText.SetText(" " + AchievementsManager.Instance.Rank.ToString());
    this.mRankWidth = this.mRankText.Font.MeasureText(this.mRankText.Characters, true).X;
    this.mRankNumWidth = this.mRankNumText.Font.MeasureText(this.mRankNumText.Characters, true).X;
    string translation = AchievementsManager.Instance.GetTranslation(AchievementsManager.MENU_ACHIEVEMENTS);
    int length1 = translation.IndexOf("%d");
    string str = translation.Substring(0, length1) + AchievementsManager.Instance.AwardedAchievements.ToString() + translation.Substring(length1 + 2);
    int length2 = str.IndexOf("%d");
    this.mAchievementsText.SetText(str.Substring(0, length2) + AchievementsManager.Instance.Achievements.Count.ToString() + str.Substring(length2 + 2));
    this.mGamesText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.MENU_GAMES).Replace("%d", AchievementsManager.Instance.Games.Count.ToString()));
  }

  private class RenderData : IRenderableGUIObject
  {
    public float Alpha;
    public bool DoProgress;
    public float Progress;
    public Vector2 MousePos;
    public bool PopupAchievementEarned;
    public bool DoPopup;
    public bool Achievements;
    public bool DoScroll;
    public int Scroll;
    public int Points;
    private Matrix mTransform = Matrix.Identity;
    private Vector4[] mTmpVertices = new Vector4[4];

    public void Draw(float iDeltaTime)
    {
      PdxWidget instance = PdxWidget.Instance;
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
      Vector2 iTopLeft = new Vector2();
      iTopLeft.X = (float) ((screenSize.X - 800) / 2);
      iTopLeft.Y = (float) ((screenSize.Y - 600) / 2);
      mEffect.Begin();
      mEffect.CurrentTechnique.Passes[0].Begin();
      this.DrawQuad(mEffect, iTopLeft.X + 13f, iTopLeft.Y + 118f, 10);
      this.DrawQuad(mEffect, iTopLeft.X + 0.0f, iTopLeft.Y + 0.0f, 11);
      this.DrawQuad(mEffect, iTopLeft.X + 0.0f, iTopLeft.Y + 446f, 12);
      Vector2 vector2 = new Vector2();
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
          this.DrawQuad(mEffect, iTopLeft.X + 330f, iTopLeft.Y + 210f, ref sRectangle1);
          this.DrawQuad(mEffect, iTopLeft.X + 330f + (float) num1, iTopLeft.Y + 210f, ref sRectangle2);
        }
        else
        {
          int num2 = num1 - sRectangle1.Width;
          sRectangle2.Width = num2;
          sRectangle1.X += num2;
          sRectangle1.Width -= num2;
          this.DrawQuad(mEffect, iTopLeft.X + 330f, iTopLeft.Y + 210f, ref sRectangle2);
          this.DrawQuad(mEffect, iTopLeft.X + 330f + (float) num2, iTopLeft.Y + 210f, ref sRectangle1);
        }
      }
      else
      {
        vector4.W = this.Alpha * this.Alpha;
        mEffect.Color = vector4;
        this.DrawQuad(mEffect, iTopLeft.X + 278f, iTopLeft.Y + 42f, 13);
        if (this.Achievements)
          this.DrawQuad(mEffect, iTopLeft.X + 180f, iTopLeft.Y + 119f, 14);
        else
          this.DrawQuad(mEffect, iTopLeft.X + 397f, iTopLeft.Y + 119f, 15);
        vector4.W = this.Alpha * this.Alpha * this.Alpha;
        mEffect.Color = vector4;
        vector2.X = iTopLeft.X + 467f;
        vector2.Y = iTopLeft.Y + 45f;
        int points = this.Points;
        for (int index = 0; index < 6; ++index)
        {
          this.DrawQuad(mEffect, vector2.X, vector2.Y, points % 10);
          points /= 10;
          vector2.X -= 37f;
        }
        vector4.W = 0.627451f;
        mEffect.Color = vector4;
        vector2.X = iTopLeft.X + 391f - (float) Math.Floor(((double) instance.mRankWidth + (double) instance.mRankNumWidth) * 0.5);
        vector2.Y = iTopLeft.Y + 16f;
        instance.mRankText.Draw(mEffect, vector2.X, vector2.Y);
        vector4.W = 0.8784314f;
        mEffect.Color = vector4;
        vector2.X += instance.mRankWidth;
        instance.mRankNumText.Draw(mEffect, vector2.X, vector2.Y);
        vector4.W = this.Achievements ? 1f : 0.5f;
        mEffect.Color = vector4;
        instance.mAchievementsText.Draw(mEffect, iTopLeft.X + 295f, iTopLeft.Y + 128f);
        vector4.W = this.Achievements ? 0.5f : 1f;
        mEffect.Color = vector4;
        instance.mGamesText.Draw(mEffect, iTopLeft.X + 484f, iTopLeft.Y + 128f);
        if (this.DoScroll)
        {
          vector2.X = iTopLeft.X + 10f;
          vector2.Y = iTopLeft.Y + 240f;
          instance.mLeftButton.Position = vector2;
          instance.mLeftButton.Draw(mEffect, this.Alpha * this.Alpha);
          vector2.X = iTopLeft.X + 751f;
          vector2.Y = iTopLeft.Y + 240f;
          instance.mRightButton.Position = vector2;
          instance.mRightButton.Draw(mEffect, this.Alpha * this.Alpha);
        }
        mEffect.GraphicsDevice.Vertices[0].SetSource(instance.mVertices, 0, 16 /*0x10*/);
        mEffect.GraphicsDevice.VertexDeclaration = instance.mDeclaration;
        mEffect.Texture = (Texture) PdxWidgetWindow.sTexture;
        vector4.W = this.Alpha * this.Alpha;
        mEffect.Color = vector4;
        if (this.Achievements)
          this.DrawAchievements(mEffect, ref iTopLeft, instance);
        else
          this.DrawGames(mEffect, ref iTopLeft, instance);
      }
      vector2.X = iTopLeft.X + 140f;
      vector2.Y = iTopLeft.Y + 85f;
      instance.mLogoutButton.Position = vector2;
      instance.mLogoutButton.Draw(mEffect, this.Alpha * this.Alpha);
      vector2.X = iTopLeft.X + 580f;
      vector2.Y = iTopLeft.Y + 85f;
      instance.mCloseButton.Position = vector2;
      instance.mCloseButton.Draw(mEffect, this.Alpha * this.Alpha);
      mEffect.CurrentTechnique.Passes[0].End();
      mEffect.End();
    }

    public int ZIndex => 2147483644;

    private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, int iId)
    {
      this.mTransform.M11 = 1f;
      this.mTransform.M22 = 1f;
      this.mTransform.M41 = iX;
      this.mTransform.M42 = iY;
      iEffect.Transform = this.mTransform;
      iEffect.CommitChanges();
      iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iId * 4, 2);
    }

    private void DrawQuad(GUIBasicEffect iEffect, float iX, float iY, ref Rectangle iSourceRect)
    {
      this.mTransform.M11 = 1f;
      this.mTransform.M22 = 1f;
      this.mTransform.M41 = iX;
      this.mTransform.M42 = iY;
      iEffect.Transform = this.mTransform;
      iEffect.CommitChanges();
      this.mTmpVertices[0].X = 0.0f;
      this.mTmpVertices[0].Y = 0.0f;
      this.mTmpVertices[0].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
      this.mTmpVertices[0].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
      this.mTmpVertices[1].X = (float) iSourceRect.Width;
      this.mTmpVertices[1].Y = 0.0f;
      this.mTmpVertices[1].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
      this.mTmpVertices[1].W = (float) iSourceRect.Y * PdxWidgetWindow.sInvTextureSize.Y;
      this.mTmpVertices[2].X = (float) iSourceRect.Width;
      this.mTmpVertices[2].Y = (float) iSourceRect.Height;
      this.mTmpVertices[2].Z = (float) (iSourceRect.X + iSourceRect.Width) * PdxWidgetWindow.sInvTextureSize.X;
      this.mTmpVertices[2].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
      this.mTmpVertices[3].X = 0.0f;
      this.mTmpVertices[3].Y = (float) iSourceRect.Height;
      this.mTmpVertices[3].Z = (float) iSourceRect.X * PdxWidgetWindow.sInvTextureSize.X;
      this.mTmpVertices[3].W = (float) (iSourceRect.Y + iSourceRect.Height) * PdxWidgetWindow.sInvTextureSize.Y;
      iEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, this.mTmpVertices, 0, 2);
    }

    private void DrawAchievements(GUIBasicEffect iEffect, ref Vector2 iTopLeft, PdxWidget iOwner)
    {
      AchievementsManager instance = AchievementsManager.Instance;
      List<AchievementData> achievements = instance.Achievements;
      Vector2 vector2 = new Vector2();
      vector2.Y = 180f + iTopLeft.Y;
      for (int index1 = 0; index1 < 2; ++index1)
      {
        vector2.X = 55f + iTopLeft.X;
        for (int index2 = 0; index2 < 5; ++index2)
        {
          int index3 = index1 * 5 + index2 + this.Scroll;
          if (index3 >= 0 & index3 < achievements.Count)
          {
            AchievementData achievementData = achievements[index3];
            if (achievementData.Achieved)
              this.DrawQuad(iEffect, vector2.X, vector2.Y, 17);
            else
              this.DrawQuad(iEffect, vector2.X, vector2.Y, 16 /*0x10*/);
            Texture2D achievementImage = instance.GetAchievementImage(achievementData.Code);
            if (achievementImage != null)
            {
              iEffect.Texture = (Texture) achievementImage;
              this.mTransform.M11 = (float) achievementImage.Width;
              this.mTransform.M22 = (float) achievementImage.Height;
              this.mTransform.M41 = vector2.X + (float) Math.Floor((124.0 - (double) this.mTransform.M11) * 0.5);
              this.mTransform.M42 = vector2.Y + (float) Math.Floor((124.0 - (double) this.mTransform.M22) * 0.5);
              iEffect.Transform = this.mTransform;
              iEffect.CommitChanges();
              iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 80 /*0x50*/, 2);
              iEffect.Texture = (Texture) PdxWidgetWindow.sTexture;
            }
          }
          vector2.X += 137f;
        }
        vector2.Y += 137f;
      }
      if (!this.DoPopup)
        return;
      this.DrawQuad(iEffect, this.MousePos.X, this.MousePos.Y - 25f, 19);
      float num = 0.0f;
      if (this.PopupAchievementEarned)
      {
        iEffect.Color = new Vector4(0.458823532f, 0.8980392f, 0.23137255f, this.Alpha * this.Alpha);
        iOwner.mPopupDateText.Draw(iEffect, this.MousePos.X + 251f, this.MousePos.Y - 15f);
        num += 25f;
        iEffect.Color = new Vector4(this.Alpha * this.Alpha);
      }
      iOwner.mPopupHeaderText.Draw(iEffect, this.MousePos.X + 29f, (float) ((double) num + (double) this.MousePos.Y - 16.0));
      iOwner.mPopupBodyText.Draw(iEffect, this.MousePos.X + 29f, (float) ((double) num + (double) this.MousePos.Y - 16.0 + (double) iOwner.mPopupHeaderText.Font.LineHeight + 2.0));
      iOwner.mPopupPointsText.Draw(iEffect, this.MousePos.X + 29f, this.MousePos.Y + 85f);
    }

    private void DrawGames(GUIBasicEffect iEffect, ref Vector2 iTopLeft, PdxWidget iOwner)
    {
      AchievementsManager instance = AchievementsManager.Instance;
      List<GameData> games = instance.Games;
      Vector2 vector2 = new Vector2();
      vector2.Y = 170f + iTopLeft.Y;
      for (int index1 = 0; index1 < 2; ++index1)
      {
        vector2.X = 55f + iTopLeft.X;
        for (int index2 = 0; index2 < 5; ++index2)
        {
          int index3 = index1 * 5 + index2 + this.Scroll;
          if (index3 >= 0 & index3 < games.Count)
          {
            GameData gameData = games[index3];
            this.DrawQuad(iEffect, vector2.X, vector2.Y, 18);
            Texture2D gameImage = instance.GetGameImage(gameData.Code);
            if (gameImage != null)
            {
              iEffect.Texture = (Texture) gameImage;
              this.mTransform.M11 = (float) gameImage.Width;
              this.mTransform.M22 = (float) gameImage.Height;
              this.mTransform.M41 = vector2.X + 17f;
              this.mTransform.M42 = vector2.Y + 15f;
              iEffect.Transform = this.mTransform;
              iEffect.CommitChanges();
              iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 80 /*0x50*/, 2);
              iEffect.Texture = (Texture) PdxWidgetWindow.sTexture;
            }
          }
          vector2.X += 137f;
        }
        vector2.Y += 137f;
      }
      if (!this.DoPopup)
        return;
      this.DrawQuad(iEffect, this.MousePos.X, this.MousePos.Y - 25f, 19);
      iOwner.mPopupHeaderText.Draw(iEffect, this.MousePos.X + 29f, this.MousePos.Y - 16f);
      iOwner.mPopupBodyText.Draw(iEffect, this.MousePos.X + 29f, (float) ((double) this.MousePos.Y - 16.0 + (double) iOwner.mPopupHeaderText.Font.LineHeight + 2.0));
    }
  }
}
