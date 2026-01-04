// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.CompanyState
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates;

internal class CompanyState : GameState
{
  private static CompanyState mSingelton;
  private static volatile object mSingeltonLock = new object();
  private ContentManager mContentManager;
  private RenderableQuad mParadox;
  private RenderableQuad mArrowhead;
  private RenderableQuad mPieces;
  private RenderableQuad mMagicka;
  private float mCountDown;
  private bool mSkip;
  private CompanyState.UpdateState mCurrentUpdate;

  public static CompanyState Instance
  {
    get
    {
      if (CompanyState.mSingelton == null)
      {
        lock (CompanyState.mSingeltonLock)
        {
          if (CompanyState.mSingelton == null)
            CompanyState.mSingelton = new CompanyState();
        }
      }
      return CompanyState.mSingelton;
    }
  }

  public void Initialize()
  {
    Point screenSize = RenderManager.Instance.ScreenSize;
    screenSize.X /= 2;
    screenSize.Y /= 2;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    this.mContentManager = new ContentManager(Magicka.Game.Instance.Content.ServiceProvider, "content");
    this.mArrowhead = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/arrowhead"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
    this.mArrowhead.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mParadox = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/paradox"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
    this.mParadox.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mMagicka = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/magicka"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
    this.mMagicka.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    float num = (float) GlobalSettings.Instance.Resolution.Width / this.mMagicka.Width;
    this.mMagicka.Scale = (double) num < 1.0 ? num : 1f;
    this.mPieces = new RenderableQuad(this.mContentManager.Load<Texture2D>("UI/Company/pieces"), graphicsDevice, PivotPosition.CENTER, RenderType.GUI, this.mScene, 2);
    this.mPieces.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mMagicka.Visible = false;
    this.mArrowhead.Visible = false;
    this.mPieces.Visible = false;
    this.mCountDown = 3f;
    this.mCurrentUpdate = new CompanyState.UpdateState(this.UpdateParadox);
  }

  public CompanyState()
    : base(new Camera(Vector3.Zero, Vector3.Forward, Vector3.Up, MagickCamera.DEFAULTFOV, 1.77777779f, 100f, 500f))
  {
  }

  public override void OnEnter()
  {
    Tome instance;
    Magicka.Game.Instance.AddLoadTask((Action) (() => instance = Tome.Instance));
    Point screenSize = RenderManager.Instance.ScreenSize;
    screenSize.X /= 2;
    screenSize.Y /= 2;
    this.mArrowhead.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mParadox.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mMagicka.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
    this.mPieces.Position = new Vector2((float) screenSize.X, (float) screenSize.Y);
  }

  public override void OnExit()
  {
    this.mContentManager.Dispose();
    ControlManager.Instance.ClearControllers();
    Tome.Instance.SetCameraAnimation(Tome.CameraAnimation.Wake_Up);
    Tome.Instance.LightIntensity = 0.0f;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    base.Update(iDataChannel, iDeltaTime);
    this.mCurrentUpdate(iDataChannel, iDeltaTime);
  }

  private void UpdateParadox(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mParadox.Update(iDataChannel, iDeltaTime);
    this.mCountDown -= iDeltaTime;
    if ((double) this.mCountDown > 0.0 && !this.mSkip)
      return;
    if (this.mParadox.Visible && !this.mSkip)
    {
      this.mParadox.Fade(0.5f);
    }
    else
    {
      this.mParadox.Visible = false;
      this.mCurrentUpdate -= new CompanyState.UpdateState(this.UpdateParadox);
      this.mCountDown = 3f;
      this.mCurrentUpdate += new CompanyState.UpdateState(this.UpdateArrowhead);
      this.mArrowhead.Fade(1f);
      this.mSkip = false;
    }
  }

  private void UpdateArrowhead(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mArrowhead.Update(iDataChannel, iDeltaTime);
    this.mCountDown -= iDeltaTime;
    if ((double) this.mCountDown > 0.0 && !this.mSkip)
      return;
    if (this.mArrowhead.Visible && !this.mSkip)
    {
      this.mArrowhead.Fade(0.5f);
    }
    else
    {
      this.mArrowhead.Visible = false;
      this.mCurrentUpdate -= new CompanyState.UpdateState(this.UpdateArrowhead);
      this.mCountDown = 3f;
      this.mCurrentUpdate += new CompanyState.UpdateState(this.UpdatePieces);
      this.mPieces.Fade(1f);
      this.mSkip = false;
    }
  }

  private void UpdatePieces(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mPieces.Update(iDataChannel, iDeltaTime);
    this.mCountDown -= iDeltaTime;
    if ((double) this.mCountDown > 0.0 && !this.mSkip)
      return;
    if (this.mPieces.Visible && !this.mSkip)
    {
      this.mPieces.Fade(0.5f);
    }
    else
    {
      this.mPieces.Visible = false;
      this.mCurrentUpdate -= new CompanyState.UpdateState(this.UpdatePieces);
      this.mCountDown = 3f;
      this.mCurrentUpdate += new CompanyState.UpdateState(this.UpdateMagicka);
      this.mMagicka.Fade(1f);
      this.mSkip = false;
    }
  }

  private void UpdateMagicka(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mMagicka.Update(iDataChannel, iDeltaTime);
    this.mCountDown -= iDeltaTime;
    if ((double) this.mCountDown > 0.0 && !this.mSkip)
      return;
    if ((double) this.mMagicka.Alpha == 1.0)
    {
      if (this.mSkip)
        this.mMagicka.Fade(0.2f);
      else
        this.mMagicka.Fade(1f);
    }
    if (this.mMagicka.Visible)
      return;
    this.StartGame();
  }

  private void StartGame() => GameStateManager.Instance.ChangeState((GameState) MenuState.Instance);

  public void SkipScreen() => this.mSkip = true;

  public delegate void UpdateState(DataChannel iDataChannel, float iDeltaTime);
}
