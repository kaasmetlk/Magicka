// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuCutscene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.DRM;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Levels.Versus;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuCutscene : SubMenu
{
  private const float pagesAndMaskPolygonWidth = 2048f;
  private const float pagesAndMaskPolygonHeight = 2048f;
  private static SubMenuCutscene sSingelton;
  private static volatile object sSingeltonLock = new object();
  private float mapImagePolygonWidth = 2048f;
  private float mapImagePolygonHeight = 2048f;
  protected int mLevel;
  private VertexBuffer mVertices_Image;
  private VertexBuffer mVertices_Pages;
  private VertexDeclaration mVertexDeclaration;
  private Texture2D mPageTexture;
  private Texture2D mTexture;
  private Texture2D mMaskTexture;
  private Texture2D mTexture_Dungeons;
  private Texture2D mMaskTexture_Dungeons;
  private Texture2D mPageTexture_Dungeons;
  private bool mIsDungeons;
  private Texture2D mTexture_Dungeons2;
  private Texture2D mMaskTexture_Dungeons2;
  private Texture2D mPageTexture_Dungeons2;
  private bool mIsDungeons2;
  private bool mFinish;
  private bool mFinishing;
  private float mTime;
  private string mSubtitle;
  private Cue mCurrentCue;
  private bool mPlay;
  private bool mMap;
  private float mMapTimer;
  private Cutscene mCutscene;
  private Text mChapterNumber;
  private Text mChapterTitle;

  public static SubMenuCutscene Instance
  {
    get
    {
      if (SubMenuCutscene.sSingelton == null)
      {
        lock (SubMenuCutscene.sSingeltonLock)
        {
          if (SubMenuCutscene.sSingelton == null)
            SubMenuCutscene.sSingelton = new SubMenuCutscene();
        }
      }
      return SubMenuCutscene.sSingelton;
    }
  }

  private SubMenuCutscene()
  {
    this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap");
    this.mMaskTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask");
    this.mPageTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
    this.mTexture_Dungeons = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap_Dungeons");
    this.mMaskTexture_Dungeons = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask_Dungeons");
    if (this.mIsDungeons)
    {
      this.mapImagePolygonWidth = (float) this.mTexture_Dungeons.Width;
      this.mapImagePolygonHeight = (float) this.mTexture_Dungeons.Height;
    }
    this.mTexture_Dungeons2 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/CampaignMap_Dungeons2");
    this.mMaskTexture_Dungeons2 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/MapMask_Dungeons2");
    if (this.mIsDungeons2)
    {
      this.mapImagePolygonWidth = (float) this.mTexture_Dungeons2.Width;
      this.mapImagePolygonHeight = (float) this.mTexture_Dungeons2.Height;
    }
    this.mVertices_Pages = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 64 /*0x40*/, BufferUsage.WriteOnly);
    Vector4[] data = new Vector4[4];
    data[0].X = 0.0f;
    data[0].Y = 0.0f;
    data[0].Z = 0.0f;
    data[0].W = 0.0f;
    data[1].X = 2048f;
    data[1].Y = 0.0f;
    data[1].Z = 1f;
    data[1].W = 0.0f;
    data[2].X = 2048f;
    data[2].Y = 2048f;
    data[2].Z = 1f;
    data[2].W = 1f;
    data[3].X = 0.0f;
    data[3].Y = 2048f;
    data[3].Z = 0.0f;
    data[3].W = 1f;
    this.mVertices_Pages.SetData<Vector4>(data);
    if ((double) this.mapImagePolygonWidth != 2048.0 || (double) this.mapImagePolygonHeight != 2048.0)
      this.RebuildVertexes();
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    });
    this.mChapterNumber = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mChapterTitle = new Text(64 /*0x40*/, FontManager.Instance.GetFont(MagickaFont.MenuTitle), TextAlign.Center, false);
    this.mFinish = false;
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (iState.LeftButton == ButtonState.Pressed || !(Tome.Instance.CurrentState is Tome.OpenState))
      return;
    this.mFinish = true;
    if (this.mCurrentCue == null || !this.mCurrentCue.IsPlaying)
      return;
    this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
  }

  public override void ControllerMouseMove(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
  }

  public override void DrawNewAndOld(
    SubMenu iPreviousMenu,
    Viewport iCurrentLeftSide,
    Viewport iCurrentRightSide,
    Viewport iPreviousLeftSide,
    Viewport iPreviousRightSide)
  {
    if (iPreviousMenu != null & !this.mMap)
    {
      base.DrawNewAndOld(iPreviousMenu, iCurrentLeftSide, iCurrentRightSide, iPreviousLeftSide, iPreviousRightSide);
    }
    else
    {
      this.Draw(iCurrentLeftSide, iCurrentRightSide);
      this.DrawOld(iPreviousLeftSide, iPreviousRightSide);
    }
  }

  public void DrawOld(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.DrawChapter();
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    if (this.mCutscene == null)
      return;
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    if (this.mMap)
      this.DrawMap();
    else
      this.DrawChapter();
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private void DrawChapter()
  {
    this.mEffect.Color = MenuItem.COLOR;
    this.mChapterNumber.Draw(this.mEffect, 512f, 360f);
    this.mChapterTitle.Draw(this.mEffect, 512f, 480f);
  }

  public void SetImageDimensions(float width, float height)
  {
    float imagePolygonWidth = this.mapImagePolygonWidth;
    float imagePolygonHeight = this.mapImagePolygonHeight;
    this.mapImagePolygonWidth = width;
    this.mapImagePolygonHeight = height;
    if (this.mVertices_Image != null && (this.mVertices_Image == null || (double) width == (double) imagePolygonWidth && (double) height == (double) imagePolygonHeight))
      return;
    this.RebuildVertexes();
  }

  private void DrawMap()
  {
    bool flag = (double) this.mapImagePolygonWidth != 2048.0 || (double) this.mapImagePolygonHeight != 2048.0;
    this.mEffect.Color = Vector4.One;
    Matrix matrix1 = new Matrix();
    matrix1.M11 = 0.5f;
    matrix1.M22 = 0.5f;
    matrix1.M44 = 1f;
    this.mEffect.Transform = matrix1;
    this.mEffect.Texture = (Texture) this.mMaskTexture;
    this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
    this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
    this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
    this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Pages, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    Matrix matrix2 = new Matrix();
    Vector2 oPosition;
    float oZoom;
    this.mCutscene.GetCamera(this.mTime, out oPosition, out oZoom);
    matrix2.M11 = matrix2.M22 = oZoom;
    matrix2.M44 = 1f;
    matrix2.M41 = (float) ((double) this.mapImagePolygonWidth / 4.0 - (double) oPosition.X * (double) oZoom);
    matrix2.M42 = (float) ((double) this.mapImagePolygonHeight / 4.0 - (double) oPosition.Y * (double) oZoom);
    if (flag)
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Image, 0, 16 /*0x10*/);
    if (this.mIsDungeons || this.mIsDungeons2)
    {
      this.mEffect.Color = new Vector4(1f);
      this.mEffect.TextureScale = Vector2.One;
      this.mEffect.TextureEnabled = true;
    }
    this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
    this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
    this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
    this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
    if (this.mIsDungeons)
      this.mEffect.Texture = (Texture) this.mTexture_Dungeons;
    else if (this.mIsDungeons2)
      this.mEffect.Texture = (Texture) this.mTexture_Dungeons2;
    else
      this.mEffect.Texture = (Texture) this.mTexture;
    this.mEffect.Transform = matrix2;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    matrix1.M11 = 1f;
    matrix1.M22 = 1f;
    matrix1.M41 = -1024f;
    this.mEffect.Transform = matrix1;
    this.mEffect.Texture = (Texture) this.mPageTexture;
    if (flag)
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices_Pages, 0, 16 /*0x10*/);
    this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
    this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
    this.mEffect.CommitChanges();
    this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
    this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
    this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
    this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
  }

  public override void ControllerUp(Controller iSender)
  {
  }

  public override void ControllerDown(Controller iSender)
  {
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mFinish)
    {
      if (this.mFinishing)
        return;
      this.mFinishing = true;
      if (this.mCurrentCue != null && this.mCurrentCue.IsPlaying)
        this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
      DialogManager.Instance.HideSubtitles();
      RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
      RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 0.5f);
    }
    else if (!this.mMap)
    {
      this.mMapTimer -= iDeltaTime;
      if ((double) this.mMapTimer > 0.0)
        return;
      this.mMap = true;
      Tome.Instance.Riffle(PlaybackMode.Forward, 1);
      if (this.mCurrentCue != null && !this.mCurrentCue.IsPlaying)
        this.mCurrentCue.Play();
      if (string.IsNullOrEmpty(this.mSubtitle))
        return;
      DialogManager.Instance.ShowSubtitles(this.mSubtitle);
    }
    else
    {
      this.mTime += iDeltaTime;
      if ((((double) this.mTime > (double) this.mCutscene.Duration ? 1 : 0) & (this.mCurrentCue == null ? 1 : (this.mCurrentCue.IsStopping | this.mCurrentCue.IsStopped ? 1 : 0))) == 0)
        return;
      this.mFinish = true;
    }
  }

  private void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    Tome.Instance.PopMenuInstant();
    if (Tome.Instance.CurrentMenu is SubMenuEndGame)
      Tome.Instance.PopMenuInstant();
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.OnTransitionEnd);
    CampaignNode iLevel;
    if (this.mLevel < 0)
    {
      iLevel = LevelManager.Instance.DungeonsCampaign[this.mLevel * -1 - 1];
      this.mIsDungeons = false;
      this.mIsDungeons2 = false;
    }
    else
      iLevel = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign[this.mLevel] : LevelManager.Instance.VanillaCampaign[this.mLevel];
    SaveData iSaveSlot = (SaveData) null;
    if (NetworkManager.Instance.State != NetworkState.Client)
      iSaveSlot = SubMenuCampaignSelect_SaveSlotSelect.Instance.CurrentSaveData;
    bool iCustom = HackHelper.LicenseStatus == HackHelper.Status.Hacked || HackHelper.CheckLicense((LevelNode) iLevel) != HackHelper.License.Yes || LevelManager.Instance.CampaignIsHacked == HackHelper.Status.Hacked;
    Magicka.GameLogic.Player[] players = Magicka.Game.Instance.Players;
    for (int index = 0; index < players.Length; ++index)
    {
      if (players[index].Playing && HackHelper.CheckLicense(players[index].Gamer.Avatar) != HackHelper.License.Yes)
      {
        iCustom = true;
        break;
      }
    }
    bool flag = true;
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).HasEntered)
      flag = false;
    if (!flag)
      return;
    GameStateManager.Instance.PushState((GameState) new PlayState(iCustom, iLevel.FullFileName, SubMenuCharacterSelect.Instance.GameType, iLevel.SpawnPoint, iSaveSlot, (VersusRuleset.Settings) null));
  }

  public bool Play
  {
    get => this.mPlay;
    set
    {
      this.mPlay = value;
      if (!this.mPlay)
        return;
      this.mMap = false;
    }
  }

  public int Level
  {
    get => this.mLevel;
    set
    {
      this.mLevel = value;
      CampaignNode campaignNode;
      if (value < 0 && value >= -2)
      {
        this.mIsDungeons = value == -1;
        this.mIsDungeons2 = value == -2;
        campaignNode = LevelManager.Instance.DungeonsCampaign[value * -1 - 1];
      }
      else
        campaignNode = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign[value] : LevelManager.Instance.VanillaCampaign[value];
      if (this.mIsDungeons)
      {
        if (this.mTexture_Dungeons == null)
          this.SetImageDimensions(1704f, 652f);
        else
          this.SetImageDimensions((float) this.mTexture_Dungeons.Width, (float) this.mTexture_Dungeons.Height);
      }
      else if (this.mIsDungeons2)
      {
        if (this.mTexture_Dungeons2 == null)
          this.SetImageDimensions(1704f, 652f);
        else
          this.SetImageDimensions((float) this.mTexture_Dungeons2.Width, (float) this.mTexture_Dungeons2.Height);
      }
      else
        this.SetImageDimensions(2048f, 2048f);
      this.mCutscene = campaignNode.Cutscene;
      this.mChapterNumber.SetText(LanguageManager.Instance.GetString(campaignNode.Name.GetHashCodeCustom()));
      string iText = LanguageManager.Instance.GetString(campaignNode.ShortDescription);
      if (string.IsNullOrEmpty(iText) || iText.Length > 64 /*0x40*/)
        iText = "";
      this.mChapterTitle.SetText(this.mChapterTitle.Font.Wrap(iText, 800, true));
    }
  }

  private void RebuildVertexes()
  {
    if ((double) this.mapImagePolygonWidth == 2048.0 && (double) this.mapImagePolygonHeight == 2048.0)
    {
      this.mVertices_Image = (VertexBuffer) null;
    }
    else
    {
      this.mVertices_Image = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 64 /*0x40*/, BufferUsage.WriteOnly);
      Vector4[] data = new Vector4[4];
      data[0].X = 0.0f;
      data[0].Y = 0.0f;
      data[0].Z = 0.0f;
      data[0].W = 0.0f;
      data[1].X = this.mapImagePolygonWidth;
      data[1].Y = 0.0f;
      data[1].Z = 1f;
      data[1].W = 0.0f;
      data[2].X = this.mapImagePolygonWidth;
      data[2].Y = this.mapImagePolygonHeight;
      data[2].Z = 1f;
      data[2].W = 1f;
      data[3].X = 0.0f;
      data[3].Y = this.mapImagePolygonHeight;
      data[3].Z = 0.0f;
      data[3].W = 1f;
      this.mVertices_Image.SetData<Vector4>(data);
    }
  }

  public override void ControllerA(Controller iSender)
  {
    if (!(Tome.Instance.CurrentState is Tome.OpenState))
      return;
    this.mFinish = true;
    if (this.mCurrentCue == null)
      return;
    this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
  }

  public override void ControllerB(Controller iSender)
  {
  }

  public override void OnEnter()
  {
    base.OnEnter();
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.mFinishing = false;
    if (this.mPlay)
    {
      this.mMapTimer = 3f;
      if (this.mCutscene != null)
      {
        Point screenSize = RenderManager.Instance.ScreenSize;
        string oString;
        if (LanguageManager.Instance.TryGetString(this.mCutscene.SubTitles, out oString))
          this.mSubtitle = DialogManager.Instance.SubtitleFont.Wrap(oString, screenSize.X - 128 /*0x80*/, true);
        this.mCurrentCue = AudioManager.Instance.GetCue(this.mCutscene.DialogBank, this.mCutscene.Dialog);
        this.mFinish = false;
      }
      else
        this.mFinish = true;
      this.mPlay = false;
      this.mTime = 0.0f;
    }
    else
      this.mFinish = true;
  }
}
