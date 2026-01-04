// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.Main.SubMenuEndGame
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu.Main;

internal class SubMenuEndGame : SubMenu
{
  private const float SCREENSHOT_WIDTH = 800f;
  private const float SCREENSHOT_HEIGHT = 640f;
  private static SubMenuEndGame sSingelton;
  private static volatile object sSingeltonLock = new object();
  private bool mIsCampaign;
  private SaveData mSaveSlot;
  private Texture2D mScreenShot;
  private Vector4[] mVertices;
  private VertexDeclaration mVertexDeclaration;
  private OptionsMessageBox mRUSureMessageBox;

  public static SubMenuEndGame Instance
  {
    get
    {
      if (SubMenuEndGame.sSingelton == null)
      {
        lock (SubMenuEndGame.sSingeltonLock)
        {
          if (SubMenuEndGame.sSingelton == null)
            SubMenuEndGame.sSingelton = new SubMenuEndGame();
        }
      }
      return SubMenuEndGame.sSingelton;
    }
  }

  private SubMenuEndGame()
  {
    this.mRUSureMessageBox = new OptionsMessageBox("#add_menu_rus".GetHashCodeCustom(), new int[2]
    {
      "#add_menu_yes".GetHashCodeCustom(),
      "#add_menu_no".GetHashCodeCustom()
    });
    this.mRUSureMessageBox.Select += new Action<OptionsMessageBox, int>(this.RUSureCallback);
    this.mMenuTitle = new Text(32 /*0x20*/, FontManager.Instance.GetFont(MagickaFont.MenuDefault), TextAlign.Center, false);
    this.mPosition = new Vector2(256f, 386f);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    });
    this.mVertices = new Vector4[4];
    this.mVertices[0].X = -400f;
    this.mVertices[0].Y = -320f;
    this.mVertices[0].Z = 0.0f;
    this.mVertices[1].X = 400f;
    this.mVertices[1].Y = -320f;
    this.mVertices[1].Z = 1f;
    this.mVertices[2].X = 400f;
    this.mVertices[2].Y = 320f;
    this.mVertices[2].Z = 1f;
    this.mVertices[3].X = -400f;
    this.mVertices[3].Y = 320f;
    this.mVertices[3].Z = 0.0f;
    this.CreditsEnd = false;
  }

  private void RUSureCallback(MessageBox iSender, int iOption)
  {
    if (iOption != 0)
      return;
    if (this.mIsCampaign && this.mSaveSlot != null)
    {
      --this.mSaveSlot.Level;
      this.mSaveSlot.TotalPlayTime -= (int) PlayState.RecentPlayState.PlayTime;
      SaveManager.Instance.SaveCampaign();
    }
    PlayState.RecentPlayState.Restart((object) this, RestartType.StartOfLevel);
    Tome.Instance.CloseTomeInstant();
    GameStateManager.Instance.PushState((GameState) PlayState.RecentPlayState);
    if (NetworkManager.Instance.State != NetworkState.Server)
      return;
    NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
    {
      IntendedMenu = MenuSelectMessage.MenuType.Statistics,
      Option = 1,
      Param0I = 0
    });
  }

  public void AddMenuTextItem(string iText)
  {
    Vector2 mPosition = this.mPosition;
    BitmapFont font = FontManager.Instance.GetFont(MagickaFont.MenuDefault);
    mPosition.Y += (float) (font.LineHeight * this.mMenuItems.Count);
    this.mMenuItems.Add((MenuItem) new MenuTextItem(iText + " (NonLoc)", mPosition, font, TextAlign.Center));
  }

  public bool CreditsEnd { get; set; }

  public override void ControllerA(Controller iSender)
  {
    if (this.mScreenShot != null || this.CreditsEnd)
      return;
    if (this.mSelectedPosition == 2)
    {
      Tome.Instance.PopMenu();
    }
    else
    {
      base.ControllerA(iSender);
      if (!this.mIsCampaign & this.mSelectedPosition == 0)
        this.mSelectedPosition = 2;
      if (this.mSelectedPosition == 1)
      {
        Tome.Instance.PopMenuInstant();
        this.mRUSureMessageBox.SelectedIndex = 1;
        this.mRUSureMessageBox.Show();
      }
      else
      {
        if (NetworkManager.Instance.State == NetworkState.Server)
          NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref new MenuSelectMessage()
          {
            IntendedMenu = MenuSelectMessage.MenuType.Statistics,
            Option = 2,
            Param0I = 0
          });
        Tome.Instance.PopMenu();
      }
    }
  }

  private void OnTomeClose()
  {
    Tome.Instance.OnBackClose -= new Action(this.OnTomeClose);
    while (!(Tome.Instance.CurrentMenu is SubMenuMain))
      Tome.Instance.PopMenuInstant();
  }

  internal override unsafe void NetworkInput(ref MenuSelectMessage iMessage)
  {
    switch (iMessage.Option)
    {
      case 0:
        byte[] iLevelHashSum = new byte[32 /*0x20*/];
        fixed (int* numPtr = &iMessage.Param0I)
        {
          for (int index = 0; index < iLevelHashSum.Length; ++index)
            iLevelHashSum[index] = ((byte*) numPtr)[index];
        }
        int oIndex;
        LevelManager.Instance.GetLevel(SubMenuCharacterSelect.Instance.GameType, iLevelHashSum, out oIndex);
        if (oIndex == -1)
        {
          Tome.Instance.PopMenu();
          break;
        }
        SubMenuCutscene.Instance.Level = oIndex;
        SubMenuCutscene.Instance.Play = true;
        Tome.Instance.PushMenu((SubMenu) SubMenuCutscene.Instance, 1);
        break;
      case 1:
        PlayState.RecentPlayState.Restart((object) this, RestartType.StartOfLevel);
        Tome.Instance.CloseTomeInstant();
        GameStateManager.Instance.PushState((GameState) PlayState.RecentPlayState);
        break;
      case 2:
        Tome.Instance.PopMenu();
        break;
    }
  }

  public override void ControllerB(Controller iSender)
  {
    if (this.mScreenShot != null)
      return;
    base.ControllerB(iSender);
    Tome.Instance.PopMenu();
  }

  public override void ControllerUp(Controller iSender)
  {
    if (this.mMenuItems == null)
      return;
    --this.mSelectedPosition;
    if (this.mSelectedPosition < 0)
      this.mSelectedPosition = this.mMenuItems.Count - 1;
    if (!(!this.mIsCampaign & this.mSelectedPosition == 0))
      return;
    this.mSelectedPosition = this.mMenuItems.Count - 1;
  }

  public override void ControllerDown(Controller iSender)
  {
    if (this.mMenuItems == null)
      return;
    ++this.mSelectedPosition;
    if (this.mSelectedPosition >= this.mMenuItems.Count)
      this.mSelectedPosition = 0;
    if (!(!this.mIsCampaign & this.mSelectedPosition == 0))
      return;
    this.mSelectedPosition = 1;
  }

  public override void Draw(Viewport iLeftSide, Viewport iRightSide)
  {
    this.mEffect.GraphicsDevice.Viewport = iRightSide;
    this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    this.mEffect.Texture = (Texture) this.mScreenShot;
    Matrix matrix = new Matrix();
    matrix.M11 = matrix.M22 = matrix.M33 = matrix.M44 = 1f;
    matrix.M41 = 512f;
    matrix.M42 = 512f;
    this.mEffect.Color = new Vector4(1f);
    this.mEffect.Transform = matrix;
    this.mEffect.Begin();
    this.mEffect.CurrentTechnique.Passes[0].Begin();
    this.mEffect.GraphicsDevice.DrawUserPrimitives<Vector4>(PrimitiveType.TriangleFan, this.mVertices, 0, 2);
    this.mEffect.CurrentTechnique.Passes[0].End();
    this.mEffect.End();
  }

  public override unsafe void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (Tome.Instance.CameraMoving)
      return;
    if (Tome.Instance.CurrentState is Tome.OpenState)
    {
      if (this.CreditsEnd)
      {
        Tome.Instance.ChangeState((Tome.TomeState) Tome.CloseToBack.Instance);
        Tome.Instance.OnBackClose += new Action(this.OnTomeClose);
      }
      else if (this.mIsCampaign)
      {
        if (NetworkManager.Instance.State == NetworkState.Server)
        {
          CampaignNode campaignNode = SubMenuCharacterSelect.Instance.GameType == GameType.Mythos ? LevelManager.Instance.MythosCampaign[(int) this.mSaveSlot.Level] : LevelManager.Instance.VanillaCampaign[(int) this.mSaveSlot.Level];
          SaveSlotInfo iMessage1 = new SaveSlotInfo(this.mSaveSlot);
          NetworkManager.Instance.Interface.SendMessage<SaveSlotInfo>(ref iMessage1);
          MenuSelectMessage iMessage2 = new MenuSelectMessage();
          iMessage2.Option = 0;
          iMessage2.IntendedMenu = MenuSelectMessage.MenuType.Statistics;
          byte* numPtr = (byte*) &iMessage2.Param0I;
          byte[] combinedHash = campaignNode.GetCombinedHash();
          for (int index = 0; index < combinedHash.Length; ++index)
            numPtr[index] = combinedHash[index];
          NetworkManager.Instance.Interface.SendMessage<MenuSelectMessage>(ref iMessage2);
        }
        if (NetworkManager.Instance.State == NetworkState.Client)
          return;
        if (this.mSaveSlot != null)
        {
          SubMenuCutscene.Instance.Level = (int) this.mSaveSlot.Level;
          SubMenuCutscene.Instance.Play = true;
          Tome.Instance.PushMenu((SubMenu) SubMenuCutscene.Instance, 1);
        }
        else
          Tome.Instance.PopMenu();
      }
      else
        Tome.Instance.PopMenu();
    }
    else
    {
      if (!(Tome.Instance.CurrentState is Tome.OpenState))
        return;
      this.mScreenShot = (Texture2D) null;
    }
  }

  public override void OnEnter()
  {
    base.OnEnter();
    Tome.Instance.CrossfadeCameraAnimation(Tome.CameraAnimation.Idle, 1f);
  }

  public override void OnExit()
  {
    base.OnExit();
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Bottom);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Right);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Left);
    GamePadMenuHelp.Instance.DeactivateButton(GamePadMenuHelp.Button.Top);
    this.CreditsEnd = false;
  }

  public void Set(bool iIsCampaign, SaveData iSaveSlot)
  {
    this.mIsCampaign = iIsCampaign;
    this.mSaveSlot = iSaveSlot;
  }

  public Texture2D ScreenShot
  {
    get => this.mScreenShot;
    set
    {
      this.mScreenShot = value;
      float num1 = (float) (1.25 / ((double) this.mScreenShot.Width / (double) this.mScreenShot.Height));
      float num2 = (float) ((1.0 - (double) num1) * 0.5);
      this.mVertices[0].Z = num2;
      this.mVertices[1].Z = num2 + num1;
      this.mVertices[2].Z = num2 + num1;
      this.mVertices[3].Z = num2;
      this.mVertices[0].W = 0.0f;
      this.mVertices[1].W = 0.0f;
      this.mVertices[2].W = 1f;
      this.mVertices[3].W = 1f;
    }
  }
}
