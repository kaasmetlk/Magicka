// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuIntro
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
using System;
using XNAnimation.Controllers;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuIntro : SubMenu
{
  private const float VLAD_DARK_TIME = 5f;
  private static SubMenuIntro sSingelton;
  private static volatile object sSingeltonLock = new object();
  private int mTextWidth = 896;
  public static readonly int MUSIC = "music_intro".GetHashCodeCustom();
  private int[] mSounds = new int[8]
  {
    "cuts_int0".GetHashCodeCustom(),
    "cuts_int1".GetHashCodeCustom(),
    "cuts_int2".GetHashCodeCustom(),
    "cuts_int3".GetHashCodeCustom(),
    "cuts_int4".GetHashCodeCustom(),
    "cuts_int5".GetHashCodeCustom(),
    "cuts_int6".GetHashCodeCustom(),
    "cuts_int7".GetHashCodeCustom()
  };
  private int[] mTexts = new int[8]
  {
    "#cutscene_intro0".GetHashCodeCustom(),
    "#cutscene_intro1".GetHashCodeCustom(),
    "#cutscene_intro2".GetHashCodeCustom(),
    "#cutscene_intro3".GetHashCodeCustom(),
    "#cutscene_intro4".GetHashCodeCustom(),
    "#cutscene_intro5".GetHashCodeCustom(),
    "#cutscene_intro6".GetHashCodeCustom(),
    "#cutscene_intro7".GetHashCodeCustom()
  };
  private float[] mTimes = new float[8]
  {
    17f,
    6f,
    20f,
    11f,
    17f,
    13f,
    9f,
    18f
  };
  private Text mTitle;
  private Text mDescription;
  private Texture2D mMaskTexture;
  private Texture2D mPageTexture;
  private Texture2D[] mTextures;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private int mTextureIndex;
  private Text mText;
  private Text mOldText;
  private bool mFinish;
  private bool mFinishing;
  private float mHackTimer;
  private bool mSwap;
  private Cue mCurrentCue;
  private int mIndex;
  private bool mPlay;

  public static SubMenuIntro Instance
  {
    get
    {
      if (SubMenuIntro.sSingelton == null)
      {
        lock (SubMenuIntro.sSingeltonLock)
        {
          if (SubMenuIntro.sSingelton == null)
            SubMenuIntro.sSingelton = new SubMenuIntro();
        }
      }
      return SubMenuIntro.sSingelton;
    }
  }

  private SubMenuIntro()
  {
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mTextures = new Texture2D[7]
      {
        null,
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s1"),
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s2"),
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s3"),
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s4"),
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s5"),
        Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_s6")
      };
      this.mMaskTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Cutscenes/intro_mask");
      this.mPageTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/ToM/tome_pages_0");
    }
    Vector4[] data = new Vector4[4 + 4 * this.mTextures.Length];
    int num = 0;
    data[num * 4].X = 0.0f;
    data[num * 4].Y = 0.0f;
    data[num * 4].Z = 0.5f;
    data[num * 4].W = 0.0f;
    data[num * 4 + 1].X = 1024f;
    data[num * 4 + 1].Y = 0.0f;
    data[num * 4 + 1].Z = 1f;
    data[num * 4 + 1].W = 0.0f;
    data[num * 4 + 2].X = 1024f;
    data[num * 4 + 2].Y = 1024f;
    data[num * 4 + 2].Z = 1f;
    data[num * 4 + 2].W = 0.5f;
    data[num * 4 + 3].X = 0.0f;
    data[num * 4 + 3].Y = 1024f;
    data[num * 4 + 3].Z = 0.5f;
    data[num * 4 + 3].W = 0.5f;
    for (int index = num + 1; index <= this.mTextures.Length; ++index)
    {
      if (this.mTextures[index - 1] != null)
      {
        Vector2 vector2 = new Vector2();
        vector2.X = 1f / (float) this.mTextures[index - 1].Width;
        vector2.Y = 1f / (float) this.mTextures[index - 1].Height;
        data[index * 4].X = -426f;
        data[index * 4].Y = -326f;
        data[index * 4].Z = 0.0f;
        data[index * 4].W = 0.0f;
        data[index * 4 + 1].X = 426f;
        data[index * 4 + 1].Y = -326f;
        data[index * 4 + 1].Z = 852f * vector2.X;
        data[index * 4 + 1].W = 0.0f;
        data[index * 4 + 2].X = 426f;
        data[index * 4 + 2].Y = 326f;
        data[index * 4 + 2].Z = 852f * vector2.X;
        data[index * 4 + 2].W = 652f * vector2.Y;
        data[index * 4 + 3].X = -426f;
        data[index * 4 + 3].Y = 326f;
        data[index * 4 + 3].Z = 0.0f;
        data[index * 4 + 3].W = 652f * vector2.Y;
      }
    }
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, data.Length * 4 * 4, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(data);
      this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    BitmapFont font1 = FontManager.Instance.GetFont(MagickaFont.MenuTitle);
    this.mTitle = new Text(32 /*0x20*/, font1, TextAlign.Center, false);
    this.mDescription = new Text(128 /*0x80*/, font1, TextAlign.Center, false);
    BitmapFont font2 = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    this.mText = new Text(512 /*0x0200*/, font2, TextAlign.Center, false);
    this.mOldText = new Text(512 /*0x0200*/, font2, TextAlign.Center, false);
    this.mFinish = false;
  }

  public override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    if (iState.LeftButton == ButtonState.Pressed || !(Tome.Instance.CurrentState is Tome.OpenState) || iOldState.LeftButton != ButtonState.Pressed)
      return;
    this.mFinish = true;
    if (this.mCurrentCue == null)
      return;
    this.mCurrentCue.Stop(AudioStopOptions.AsAuthored);
  }

  public void DrawOld(Viewport iLeftSide, Viewport iRightSide)
  {
    this.Draw(ref iRightSide, 1f, this.mOldText, this.mTextureIndex - 1);
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    if (this.mSwap)
    {
      Helper.Swap<Text>(ref this.mText, ref this.mOldText);
      this.mTextureIndex = this.mIndex;
      this.mSwap = false;
    }
    float amount = (float) ((1.0 - (double) this.mHackTimer / (double) this.mTimes[this.mTextureIndex]) / 0.800000011920929 - 0.125);
    double num = (double) MathHelper.Clamp(amount, 0.0f, 1f);
    float iTime = MathHelper.SmoothStep(0.0f, 1f, MathHelper.SmoothStep(0.0f, 1f, amount));
    this.Draw(ref iRightSide, iTime, this.mText, this.mTextureIndex);
  }

  private void Draw(ref Viewport iViewport, float iTime, Text iText, int iTextureIndex)
  {
    this.mEffect.GraphicsDevice.Viewport = iViewport;
    this.mEffect.Color = new Vector4(1f);
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    if (iTextureIndex >= 0 & iTextureIndex < this.mTextures.Length && this.mTextures[iTextureIndex] != null)
    {
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16 /*0x10*/);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      Matrix matrix1 = new Matrix();
      Matrix matrix2 = new Matrix();
      matrix2.M11 = matrix2.M22 = matrix2.M44 = 1f;
      matrix2.M41 = 512f;
      matrix2.M42 = 416f;
      this.mEffect.Transform = matrix2;
      this.mEffect.TextureOffset = new Vector2();
      this.mEffect.TextureScale = Vector2.One;
      this.mEffect.TextureEnabled = true;
      this.mEffect.Color = Vector4.One;
      this.mEffect.Texture = (Texture) this.mMaskTexture;
      this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
      this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 8, 2);
      this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.Zero;
      this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.SourceAlpha;
      switch (iTextureIndex)
      {
        case 3:
          SubMenuIntro.DrawImagePan1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        case 4:
          SubMenuIntro.DrawImageZoom1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        case 5:
          SubMenuIntro.DrawImageZoom2(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        default:
          SubMenuIntro.DrawImageNoAnimation(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
      }
      this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
      this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationAlpha;
      this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseDestinationAlpha;
      switch (iTextureIndex)
      {
        case 3:
          SubMenuIntro.DrawImagePan1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        case 4:
          SubMenuIntro.DrawImageZoom1(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        case 5:
          SubMenuIntro.DrawImageZoom2(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
        default:
          SubMenuIntro.DrawImageNoAnimation(iTime, this.mEffect, this.mTextures[iTextureIndex], 4 + 4 * iTextureIndex);
          break;
      }
      matrix2.M11 = 1f;
      matrix2.M22 = 1f;
      matrix2.M41 = 0.0f;
      matrix2.M42 = 0.0f;
      this.mEffect.Transform = matrix2;
      this.mEffect.Texture = (Texture) this.mPageTexture;
      this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.Alpha;
      this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      this.mEffect.CommitChanges();
      this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      this.mEffect.GraphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
      this.mEffect.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      this.mEffect.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      this.mEffect.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
      this.mEffect.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
    }
    else
    {
      this.mEffect.Color = MenuItem.COLOR;
      this.mTitle.Draw(this.mEffect, 512f, 360f);
      this.mDescription.Draw(this.mEffect, 512f, 480f);
    }
    this.mEffect.Color = new Vector4(0.1f, 0.1f, 0.1f, 0.9f);
    this.mEffect.CommitChanges();
    iText.Draw(this.mEffect, 512f, 768f);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  private static void DrawImageNoAnimation(
    float iTime,
    GUIBasicEffect iEffect,
    Texture2D iTexture,
    int iStartVertex)
  {
    iEffect.Texture = (Texture) iTexture;
    iEffect.TextureEnabled = true;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
  }

  private static void DrawImagePan1(
    float iTime,
    GUIBasicEffect iEffect,
    Texture2D iTexture,
    int iStartVertex)
  {
    iEffect.Texture = (Texture) iTexture;
    iEffect.TextureEnabled = true;
    iEffect.TextureOffset = new Vector2()
    {
      X = (float) (100.0 + (double) iTime * 600.0) / (float) iTexture.Width
    };
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
    iEffect.TextureOffset = new Vector2();
  }

  private static void DrawImageZoom1(
    float iTime,
    GUIBasicEffect iEffect,
    Texture2D iTexture,
    int iStartVertex)
  {
    iEffect.Texture = (Texture) iTexture;
    iEffect.TextureEnabled = true;
    iTime = 1f - iTime;
    iEffect.TextureOffset = new Vector2()
    {
      X = (float) ((double) iTime * (double) iTexture.Width * 0.25) / (float) iTexture.Width,
      Y = iTime * 64f / (float) iTexture.Height
    };
    iEffect.TextureScale = new Vector2()
    {
      X = 2f - iTime,
      Y = 2f - iTime
    };
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
    iEffect.TextureOffset = new Vector2();
    iEffect.TextureScale = new Vector2(1f);
  }

  private static void DrawImageZoom2(
    float iTime,
    GUIBasicEffect iEffect,
    Texture2D iTexture,
    int iStartVertex)
  {
    iEffect.Texture = (Texture) iTexture;
    iEffect.TextureEnabled = true;
    iEffect.TextureOffset = new Vector2()
    {
      X = iTime * 280f / (float) iTexture.Width,
      Y = iTime * 220f / (float) iTexture.Height
    };
    iEffect.TextureScale = new Vector2()
    {
      X = (float) (1.0 + (double) iTime * 0.10000000149011612),
      Y = (float) (1.0 + (double) iTime * 0.10000000149011612)
    };
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, iStartVertex, 2);
    iEffect.TextureOffset = new Vector2();
    iEffect.TextureScale = new Vector2(1f);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mHackTimer = Math.Max(this.mHackTimer - iDeltaTime, 0.0f);
    if (Tome.Instance.CurrentState is Tome.OpenState && (this.mIndex < 0 || this.mCurrentCue == null))
    {
      if (this.mIndex < 0)
      {
        if ((double) this.mHackTimer <= 0.0)
        {
          this.mIndex = 0;
          Tome.Instance.TargetLightIntensity = 1f;
          Tome.Instance.TargetLightVariationAmount = 0.2f;
          Tome.Instance.TargetLightVariationSpeed = 0.5f;
          this.mHackTimer = this.mTimes[this.mIndex];
        }
      }
      else
        this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[this.mIndex]);
    }
    if (this.mFinish)
    {
      if (this.mFinishing)
        return;
      this.mFinishing = true;
      RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
      RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 2f);
    }
    else if (this.mIndex < 0)
    {
      if ((double) this.mHackTimer > 5.0 || this.mCurrentCue != null)
        return;
      this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[0]);
      DialogManager.Instance.ShowSubtitles(DialogManager.Instance.SubtitleFont.Wrap(LanguageManager.Instance.GetString(this.mTexts[0]), RenderManager.Instance.ScreenSize.X * 8 / 10, true));
    }
    else
    {
      if ((double) this.mHackTimer > 0.0)
        return;
      DialogManager.Instance.HideSubtitles();
      lock (this.mTextures)
      {
        if (this.mIndex >= this.mTexts.Length - 1)
          return;
        int index = this.mIndex + 1;
        if (index == this.mTexts.Length - 1)
        {
          RenderManager.Instance.TransitionEnd += new TransitionEnd(this.OnTransitionEnd);
          RenderManager.Instance.BeginTransition(Transitions.Fade, Color.Black, 2f);
        }
        else
        {
          this.mOldText.SetText(this.mOldText.Font.Wrap(LanguageManager.Instance.GetString(this.mTexts[index]), this.mTextWidth, true));
          this.mCurrentCue = (Cue) null;
          this.mHackTimer = this.mTimes[index];
          this.mFinish = false;
          this.mSwap = true;
          Tome.Instance.Riffle(PlaybackMode.Forward, 1);
        }
        this.mIndex = index;
      }
    }
  }

  private void OnTransitionEnd(TransitionEffect iDeadTransition)
  {
    Tome.Instance.PopMenuInstant();
    if (Tome.Instance.CurrentMenu is SubMenuEndGame)
      Tome.Instance.PopMenuInstant();
    RenderManager.Instance.TransitionEnd -= new TransitionEnd(this.OnTransitionEnd);
    CampaignNode iLevel = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign[0] : LevelManager.Instance.VanillaCampaign[0];
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
    PlayState iGameState = new PlayState(iCustom, iLevel.FullFileName, SubMenuCampaignSelect_SaveSlotSelect.Instance.GameType, iLevel.SpawnPoint, iSaveSlot, (VersusRuleset.Settings) null);
    if (!this.mFinish && this.mIndex != -1)
    {
      this.mCurrentCue = AudioManager.Instance.PlayCue(Banks.WaveBank, this.mSounds[this.mIndex]);
      string iTip = LanguageManager.Instance.GetString(this.mTexts[this.mIndex]);
      iGameState.SetTip(iTip, false, this.mCurrentCue);
    }
    GameStateManager.Instance.PushState((GameState) iGameState);
  }

  public override void DrawNewAndOld(
    SubMenu iPreviousMenu,
    Viewport iCurrentLeftSide,
    Viewport iCurrentRightSide,
    Viewport iPreviousLeftSide,
    Viewport iPreviousRightSide)
  {
    if (iPreviousMenu != null & this.mIndex == 0 & !this.mSwap)
    {
      base.DrawNewAndOld(iPreviousMenu, iCurrentLeftSide, iCurrentRightSide, iPreviousLeftSide, iPreviousRightSide);
    }
    else
    {
      this.Draw(iCurrentLeftSide, iCurrentRightSide);
      this.DrawOld(iPreviousLeftSide, iPreviousRightSide);
    }
  }

  public bool Play
  {
    get => this.mPlay;
    set => this.mPlay = value;
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
    this.mFinish = false;
    this.mFinishing = false;
    if (!this.mPlay)
      return;
    CampaignNode campaignNode = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign[0] : LevelManager.Instance.VanillaCampaign[0];
    this.mTitle.SetText(LanguageManager.Instance.GetString(campaignNode.Name.GetHashCodeCustom()));
    this.mDescription.SetText(LanguageManager.Instance.GetString(campaignNode.Description));
    AudioManager.Instance.StopMusic();
    AudioManager.Instance.PlayMusic(Banks.Music, SubMenuIntro.MUSIC, new float?());
    Tome.Instance.TargetLightIntensity = 0.0f;
    Tome.Instance.TargetLightVariationAmount = 0.0f;
    Tome.Instance.TargetLightVariationSpeed = 1.5f;
    this.mIndex = -1;
    this.mTextureIndex = 0;
    this.mText.Clear();
    this.mCurrentCue = (Cue) null;
    this.mPlay = false;
    this.mHackTimer = 8f;
  }

  public override void OnExit()
  {
    base.OnExit();
    Tome.Instance.TargetLightIntensity = 1f;
    Tome.Instance.TargetLightVariationAmount = 0.2f;
    Tome.Instance.TargetLightVariationSpeed = 6f;
    DialogManager.Instance.HideSubtitles();
  }
}
