// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.AchievementPopupWindow
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.IO;

#nullable disable
namespace Magicka.Achievements;

internal class AchievementPopupWindow : AchievementWindow
{
  public static readonly Vector3 DEFAULT_COLOR = new Vector3(0.9098039f);
  public static readonly Vector3 ACHIEVEMENT_COLOR = new Vector3(0.458823532f, 0.8980392f, 0.23137255f);
  public static readonly Vector3 POINTS_COLOR = new Vector3(1f);
  private static Texture2D sTexture;
  private static float sRectangleWidth;
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static GUIBasicEffect sEffect;
  private AchievementPopupWindow.RenderData[] mRenderData;
  private Text mUnlockedText;
  private Text mAchievementText;
  private Text mScoreText;
  private BitmapFont mAchievementFont;
  private BitmapFont mScoreFont;
  private string mAchiementCode;
  private float mTime;

  public AchievementPopupWindow()
  {
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (AchievementPopupWindow.sEffect == null)
    {
      lock (graphicsDevice)
      {
        TextureCreationParameters creationParameters = TextureCreationParameters.Default with
        {
          Format = SurfaceFormat.Dxt3
        };
        AchievementPopupWindow.sTexture = Texture2D.FromFile(graphicsDevice, "content/connectui/popup.png", creationParameters);
        AchievementPopupWindow.sEffect = new GUIBasicEffect(graphicsDevice, (EffectPool) null);
      }
      TextReader textReader = (TextReader) File.OpenText("content/connectui/popup.txt");
      int num = int.Parse(textReader.ReadLine());
      if (num < 0 || num > 1)
      {
        textReader.Close();
        throw new Exception("Invalid value in popup.txt");
      }
      textReader.ReadLine();
      Rectangle rectangle = new Rectangle();
      rectangle.X = int.Parse(textReader.ReadLine());
      rectangle.Y = int.Parse(textReader.ReadLine());
      rectangle.Width = int.Parse(textReader.ReadLine());
      rectangle.Height = int.Parse(textReader.ReadLine());
      AchievementPopupWindow.sRectangleWidth = (float) rectangle.Width;
      textReader.Close();
      Vector2 vector2 = new Vector2();
      vector2.X = 1f / (float) AchievementPopupWindow.sTexture.Width;
      vector2.Y = 1f / (float) AchievementPopupWindow.sTexture.Height;
      Vector4[] data = new Vector4[8];
      data[0].X = 0.0f;
      data[0].Y = 0.0f;
      data[0].Z = (float) rectangle.X * vector2.X;
      data[0].W = (float) rectangle.Y * vector2.Y;
      data[1].X = (float) rectangle.Width;
      data[1].Y = 0.0f;
      data[1].Z = (float) (rectangle.X + rectangle.Width) * vector2.X;
      data[1].W = (float) rectangle.Y * vector2.Y;
      data[2].X = (float) rectangle.Width;
      data[2].Y = (float) rectangle.Height;
      data[2].Z = (float) (rectangle.X + rectangle.Width) * vector2.X;
      data[2].W = (float) (rectangle.Y + rectangle.Height) * vector2.Y;
      data[3].X = 0.0f;
      data[3].Y = (float) rectangle.Height;
      data[3].Z = (float) rectangle.X * vector2.X;
      data[3].W = (float) (rectangle.Y + rectangle.Height) * vector2.Y;
      data[4].X = 0.0f;
      data[4].Y = 0.0f;
      data[4].Z = 0.0f;
      data[4].W = 0.0f;
      data[5].X = 41f;
      data[5].Y = 0.0f;
      data[5].Z = 1f;
      data[5].W = 0.0f;
      data[6].X = 41f;
      data[6].Y = 41f;
      data[6].Z = 1f;
      data[6].W = 1f;
      data[7].X = 0.0f;
      data[7].Y = 41f;
      data[7].Z = 0.0f;
      data[7].W = 1f;
      lock (graphicsDevice)
      {
        AchievementPopupWindow.sVertices = new VertexBuffer(graphicsDevice, data.Length * 4 * 4, BufferUsage.WriteOnly);
        AchievementPopupWindow.sVertices.SetData<Vector4>(data);
        AchievementPopupWindow.sVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
        {
          new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
          new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
        });
      }
    }
    this.mAchievementFont = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold);
    this.mScoreFont = FontManager.Instance.GetFont(MagickaFont.PDX_Points);
    this.mUnlockedText = new Text(64 /*0x40*/, this.mAchievementFont, TextAlign.Left, false);
    this.mAchievementText = new Text(128 /*0x80*/, this.mAchievementFont, TextAlign.Left, false);
    this.mScoreText = new Text(32 /*0x20*/, this.mScoreFont, TextAlign.Right, false);
    this.mRenderData = new AchievementPopupWindow.RenderData[3];
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index] = new AchievementPopupWindow.RenderData(this.mUnlockedText, this.mAchievementText, this.mScoreText);
  }

  public virtual void Show(AchievementData iAchievement)
  {
    this.mTime = 0.0f;
    this.mAchiementCode = iAchievement.Code;
    Point screenSize = RenderManager.Instance.ScreenSize;
    AchievementPopupWindow.sEffect.SetScreenSize(screenSize.X, screenSize.Y);
    this.mUnlockedText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.ACHIEVEMENT_UNLOCKED));
    this.mAchievementText.SetText(iAchievement.Name);
    this.mScoreText.SetText(AchievementsManager.Instance.GetTranslation(AchievementsManager.NUM_PP).Replace("%d", iAchievement.Points.ToString()));
    base.Show();
  }

  public override void Show() => throw new Exception("NEVER call this overload directly!");

  public override void Hide() => base.Hide();

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mTime += iDeltaTime;
    if (this.mVisible & (double) this.mTime > 3.0)
      this.Hide();
    AchievementPopupWindow.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.AchiementCode = this.mAchiementCode;
    iObject.Alpha = this.mAlpha;
    iObject.Position.X = (float) Math.Floor(((double) RenderManager.Instance.ScreenSize.X - (double) AchievementPopupWindow.sRectangleWidth) * 0.5);
    iObject.Position.Y = 32f;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  private class RenderData : IRenderableGUIObject
  {
    public string AchiementCode;
    public Vector2 Position;
    public float Alpha;
    private Text mUnlockedText;
    private Text mAchievementText;
    private Text mScoreText;

    public RenderData(Text iUnlockedText, Text iAchievementText, Text iScoreText)
    {
      this.mUnlockedText = iUnlockedText;
      this.mAchievementText = iAchievementText;
      this.mScoreText = iScoreText;
    }

    public void Draw(float iDeltaTime)
    {
      Vector4 vector4 = new Vector4();
      vector4.X = vector4.Y = vector4.Z = 1f;
      vector4.W = this.Alpha;
      AchievementPopupWindow.sEffect.Color = vector4;
      AchievementPopupWindow.sEffect.Texture = (Texture) AchievementPopupWindow.sTexture;
      Matrix matrix = new Matrix();
      matrix.M11 = 1f;
      matrix.M22 = 1f;
      matrix.M41 = this.Position.X;
      matrix.M42 = this.Position.Y;
      matrix.M44 = 1f;
      AchievementPopupWindow.sEffect.Transform = matrix;
      AchievementPopupWindow.sEffect.GraphicsDevice.Vertices[0].SetSource(AchievementPopupWindow.sVertices, 0, 16 /*0x10*/);
      AchievementPopupWindow.sEffect.GraphicsDevice.VertexDeclaration = AchievementPopupWindow.sVertexDeclaration;
      AchievementPopupWindow.sEffect.Begin();
      AchievementPopupWindow.sEffect.CurrentTechnique.Passes[0].Begin();
      AchievementPopupWindow.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      Texture2D achievementImage = AchievementsManager.Instance.GetAchievementImage(this.AchiementCode);
      if (achievementImage != null)
      {
        AchievementPopupWindow.sEffect.Texture = (Texture) achievementImage;
        matrix.M41 += 10f;
        matrix.M42 += 10f;
        AchievementPopupWindow.sEffect.Transform = matrix;
        AchievementPopupWindow.sEffect.CommitChanges();
        AchievementPopupWindow.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
      }
      Vector2 vector2 = FontManager.Instance.GetFont(MagickaFont.PDX_UI_Bold).MeasureText(this.mUnlockedText.Characters, true);
      vector4.X = AchievementPopupWindow.DEFAULT_COLOR.X;
      vector4.Y = AchievementPopupWindow.DEFAULT_COLOR.Y;
      vector4.Z = AchievementPopupWindow.DEFAULT_COLOR.Z;
      AchievementPopupWindow.sEffect.Color = vector4;
      this.mUnlockedText.Draw(AchievementPopupWindow.sEffect, this.Position.X + 71f, this.Position.Y + 19f);
      vector4.X = AchievementPopupWindow.ACHIEVEMENT_COLOR.X;
      vector4.Y = AchievementPopupWindow.ACHIEVEMENT_COLOR.Y;
      vector4.Z = AchievementPopupWindow.ACHIEVEMENT_COLOR.Z;
      AchievementPopupWindow.sEffect.Color = vector4;
      this.mAchievementText.Draw(AchievementPopupWindow.sEffect, (float) ((double) this.Position.X + 71.0 + (double) vector2.X + 10.0), this.Position.Y + 19f);
      vector4.X = AchievementPopupWindow.POINTS_COLOR.X;
      vector4.Y = AchievementPopupWindow.POINTS_COLOR.Y;
      vector4.Z = AchievementPopupWindow.POINTS_COLOR.Z;
      AchievementPopupWindow.sEffect.Color = vector4;
      this.mScoreText.Draw(AchievementPopupWindow.sEffect, this.Position.X + 770f, this.Position.Y + 17f);
      AchievementPopupWindow.sEffect.CurrentTechnique.Passes[0].End();
      AchievementPopupWindow.sEffect.End();
    }

    public int ZIndex => 2147483646;
  }
}
