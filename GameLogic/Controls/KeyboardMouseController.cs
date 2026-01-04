// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.KeyboardMouseController
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Geometry;
using Magicka.Achievements;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.GameStates.Menu.Main;
using Magicka.GameLogic.GameStates.Persistent;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Magicka.Network;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal class KeyboardMouseController : Controller
{
  private static readonly int[] MOUSE_BUTTONS = new int[8]
  {
    0,
    "#mouse_left".GetHashCodeCustom(),
    "#mouse_middle".GetHashCodeCustom(),
    "#mouse_right".GetHashCodeCustom(),
    "#mouse_4".GetHashCodeCustom(),
    "#mouse_5".GetHashCodeCustom(),
    "#mouse_scroll_up".GetHashCodeCustom(),
    "#mouse_scroll_down".GetHashCodeCustom()
  };
  private static readonly float ONEOVERSQRT2 = 1f / (float) Math.Sqrt(2.0);
  private static readonly float[] LOOKUP_ANGLES = new float[9]
  {
    0.0f,
    0.7853982f,
    1.57079637f,
    2.3561945f,
    3.14159274f,
    3.926991f,
    4.712389f,
    5.49778748f,
    6.28318548f
  };
  internal static KeyboardMouseController.Binding[] mKeyboardBindings = new KeyboardMouseController.Binding[17];
  private KeyboardState mLastKeyboardState;
  private MouseState mLastMouseState;
  private char mLastChar;
  private float[] mFadeTimers;
  private float[] mMoveTimers;
  private bool mInteractMoveLock;
  private bool mIsGameActive;
  private object mCursorPressedTarget;
  private object mLockedTarget;
  private bool mStillPressing;
  private Point mLockedMousePos;
  private Vector2 mMouseOffset;
  private KeyboardHUD mHUD = KeyboardHUD.Instance;
  private Vector3 mCursorLockPosition;
  public static bool mCatchKeyActive;
  public static KeyboardBindings mCatchKeyIndex;

  public event Action AnyKeyPress;

  static KeyboardMouseController() => KeyboardMouseController.LoadDefaults();

  public KeyboardMouseController(InputMessageFilter iMessageFilter)
  {
    this.mFadeTimers = new float[4];
    this.mMoveTimers = new float[4];
    this.mCursorPressedTarget = (object) null;
    iMessageFilter.KeyDown += new Action<KeyData>(this.OnKeyDown);
    iMessageFilter.KeyPress += new Action<char, KeyModifiers>(this.OnKeyPress);
    iMessageFilter.KeyUp += new Action<KeyData>(this.OnKeyUp);
    this.mInteractMoveLock = false;
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    DialogManager instance1 = DialogManager.Instance;
    Tome instance2 = Tome.Instance;
    GameState currentState = GameStateManager.Instance.CurrentState;
    ControlManager instance3 = ControlManager.Instance;
    TutorialManager instance4 = TutorialManager.Instance;
    Point screenSize = RenderManager.Instance.ScreenSize;
    PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
    KeyboardMouseController.InputState iState = new KeyboardMouseController.InputState();
    iState.NewKeyboardState = Magicka.Game.Instance.KeyboardState;
    iState.OldKeyboardState = this.mLastKeyboardState;
    this.mLastKeyboardState = iState.NewKeyboardState;
    iState.NewMouseState = Magicka.Game.Instance.MouseState;
    iState.OldMouseState = this.mLastMouseState;
    this.mLastMouseState = iState.NewMouseState;
    bool focused = Magicka.Game.Instance.Focused;
    if (focused & !this.mIsGameActive)
      this.mInteractMoveLock = false;
    this.mIsGameActive = focused;
    PlayState playState = currentState as PlayState;
    if (!this.mIsGameActive)
    {
      if (playState == null || playState.IsInCutscene || this.Player == null || !this.Player.Playing)
        return;
      this.mHUD.Update(iDataChannel, iDeltaTime);
    }
    else
    {
      if (this.AnyKeyPress != null)
      {
        if (iState.NewKeyboardState != iState.OldKeyboardState && iState.NewKeyboardState.GetPressedKeys().Length > 0)
        {
          this.AnyKeyPress();
          return;
        }
        if (iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.NewMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.NewMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.NewMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed || iState.NewMouseState.ScrollWheelValue != iState.OldMouseState.ScrollWheelValue)
        {
          this.AnyKeyPress();
          return;
        }
      }
      if (KeyboardMouseController.mCatchKeyActive)
      {
        if (this.IsPressed(KeyboardMouseController.MouseButton.Left, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Left, KeyboardMouseController.mCatchKeyIndex);
        if (this.IsPressed(KeyboardMouseController.MouseButton.Middle, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Middle, KeyboardMouseController.mCatchKeyIndex);
        if (this.IsPressed(KeyboardMouseController.MouseButton.Right, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Right, KeyboardMouseController.mCatchKeyIndex);
        if (this.IsPressed(KeyboardMouseController.MouseButton.X1, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.X1, KeyboardMouseController.mCatchKeyIndex);
        if (this.IsPressed(KeyboardMouseController.MouseButton.X2, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.X2, KeyboardMouseController.mCatchKeyIndex);
        if (this.IsPressed(KeyboardMouseController.MouseButton.ScrollUp, ref iState))
          KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.ScrollUp, KeyboardMouseController.mCatchKeyIndex);
        if (!this.IsPressed(KeyboardMouseController.MouseButton.ScrollDown, ref iState))
          return;
        KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.ScrollDown, KeyboardMouseController.mCatchKeyIndex);
      }
      else
      {
        bool flag1 = this.IsPressed(Keys.Enter, ref iState);
        bool flag2 = this.IsPressed(Keys.Back, ref iState);
        bool flag3 = this.IsPressed(Keys.Escape, ref iState);
        bool flag4 = this.IsPressed(Keys.Space, ref iState);
        bool flag5 = this.IsPressed(Keys.Delete, ref iState);
        bool flag6 = iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed & iState.OldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        bool flag7 = iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released & iState.OldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        bool flag8 = iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed & iState.OldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        bool flag9 = iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released & iState.OldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        ControllerDirection direction1 = KeyboardMouseController.GetDirection(ref iState.NewKeyboardState, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
        ControllerDirection direction2 = KeyboardMouseController.GetDirection(ref iState.OldKeyboardState, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
        Cursors iCursor = Cursors.Default;
        bool iActive = true;
        bool flag10 = iState.NewMouseState.X != iState.OldMouseState.X | iState.NewMouseState.Y != iState.OldMouseState.Y;
        Vector2 iMousePos = new Vector2();
        iMousePos.X = (float) iState.NewMouseState.X;
        iMousePos.Y = (float) iState.NewMouseState.Y;
        if (PdxLoginWindow.Instance.Visible)
        {
          Magicka.Game.Instance.IsMouseVisible = true;
          if (flag6)
            PdxLoginWindow.Instance.OnMouseDown(ref iMousePos);
          if (flag7)
            PdxLoginWindow.Instance.OnMouseUp(ref iMousePos);
          if (flag10)
            PdxLoginWindow.Instance.OnMouseMove(ref iMousePos);
        }
        else if (PdxWidget.Instance.Visible)
        {
          Magicka.Game.Instance.IsMouseVisible = true;
          if (flag6)
            PdxWidget.Instance.OnMouseDown(ref iMousePos);
          if (flag7)
            PdxWidget.Instance.OnMouseUp(ref iMousePos);
          if (flag10)
            PdxWidget.Instance.OnMouseMove(ref iMousePos);
        }
        else if (currentState is CompanyState & (flag3 | flag1 | flag2 | flag4 | flag8 | flag6))
          (currentState as CompanyState).SkipScreen();
        else if (persistentState.IsolateControls())
        {
          if (flag3)
            persistentState.ControllerB((Controller) this);
          if (direction1 != direction2 && direction1 != ControllerDirection.Center && direction1 != ControllerDirection.Dead)
            persistentState.ControllerMovement((Controller) this, direction1);
          persistentState.ControllerEvent((Controller) this, iState.NewKeyboardState, iState.OldKeyboardState);
          if (flag1 || flag4)
            persistentState.ControllerA((Controller) this);
          else if (flag7 || flag6 || flag8 || flag9 || iState.NewMouseState.ScrollWheelValue != iState.OldMouseState.ScrollWheelValue)
            persistentState.ControllerMouseAction((Controller) this, screenSize, iState.NewMouseState, iState.OldMouseState);
          else if (flag10)
          {
            Magicka.Game.Instance.IsMouseVisible = true;
            persistentState.ControllerMouseMove((Controller) this, screenSize, iState.NewMouseState, iState.OldMouseState);
          }
        }
        else if (instance1.MessageBoxActive)
        {
          if (flag3)
            instance1.ControllerEsc((Controller) this);
          else if (flag6 | flag7 | iState.NewMouseState.ScrollWheelValue != iState.OldMouseState.ScrollWheelValue)
            instance1.ControllerMouseClick((Controller) this, iState.NewMouseState, iState.OldMouseState);
          else if (flag10)
          {
            Magicka.Game.Instance.IsMouseVisible = true;
            instance1.ControllerMouseMove((Controller) this, iState.NewMouseState, iState.OldMouseState);
          }
        }
        else if (currentState is MenuState)
        {
          if (MenuState.Instance.TomeTakesInput)
          {
            if (flag3)
              instance2.ControllerB((Controller) this);
            if (flag5 & Tome.Instance.CurrentMenu is SubMenuCampaignSelect_SaveSlotSelect)
              (Tome.Instance.CurrentMenu as SubMenuCampaignSelect_SaveSlotSelect).Delete((Controller) this);
            if (direction1 != direction2 & direction1 != ControllerDirection.Center & direction1 != ControllerDirection.Dead)
              instance2.ControllerMovement((Controller) this, direction1);
            instance2.ControllerEvent((Controller) this, iState.OldKeyboardState, iState.NewKeyboardState);
            if (Tome.Instance.CurrentState is Tome.ClosedBack && flag6 | flag1 | flag4)
              instance2.ControllerA((Controller) this);
            else if (flag7 | flag6 | flag8 | flag9 | iState.NewMouseState.ScrollWheelValue != iState.OldMouseState.ScrollWheelValue)
              instance2.ControllerMouseAction(this, screenSize, iState.NewMouseState, iState.OldMouseState);
            else if (flag10)
            {
              Magicka.Game.Instance.IsMouseVisible = true;
              instance2.ControllerMouseMove(this, screenSize, iState.NewMouseState, iState.OldMouseState);
            }
          }
        }
        else if (playState != null)
        {
          if (playState.IsInCutscene && !playState.IsPaused)
            Magicka.Game.Instance.IsMouseVisible = false;
          else if (this.Player != null && this.Player.Playing)
            this.mHUD.Update(iDataChannel, iDeltaTime);
          if (playState.IsPaused && InGameMenu.IsControllerAllowed((Controller) this))
          {
            if (flag3)
              playState.TogglePause((Controller) this);
            if (flag1 | flag4)
              InGameMenu.ControllerSelect((Controller) this);
            if (flag2)
              InGameMenu.ControllerBack((Controller) this);
            if (direction1 != direction2 & direction1 != ControllerDirection.Center & direction1 != ControllerDirection.Dead)
              instance2.ControllerMovement((Controller) this, direction1);
            if (flag10)
            {
              Magicka.Game.Instance.IsMouseVisible = true;
              InGameMenu.MouseMove((Controller) this, ref iMousePos);
            }
            if (flag6)
              InGameMenu.MouseDown((Controller) this, ref iMousePos);
            else if (flag7)
              InGameMenu.MouseUp((Controller) this, ref iMousePos);
            else if (iState.NewMouseState.ScrollWheelValue != iState.OldMouseState.ScrollWheelValue)
              InGameMenu.MouseScroll((Controller) this, ref iMousePos, iState.NewMouseState.ScrollWheelValue - iState.OldMouseState.ScrollWheelValue);
            if (direction1 != ControllerDirection.Dead & direction1 != ControllerDirection.Center & direction1 != direction2)
              InGameMenu.ControllerMove((Controller) this, direction1);
          }
          else
          {
            if (flag3)
              playState.TogglePause((Controller) this);
            else if (flag6 | flag8)
              instance4.RemoveDialogHint();
            if (flag1 | flag4 | flag6 && instance1.CanAdvance((Controller) this) && !playState.IsInCutscene)
            {
              instance1.Advance((Controller) this);
              this.mAvatar.MouseMoveStop();
            }
            if (iState.NewKeyboardState.IsKeyDown(Keys.Enter) && iState.OldKeyboardState.IsKeyUp(Keys.Enter))
              playState.ToggleChat();
            if (this.Player != null && this.Player.Playing)
            {
              this.mAvatar = this.mPlayer.Avatar;
              if (this.mAvatar != null && !this.mAvatar.Dead && (currentState as PlayState).Level.CurrentScene != null)
              {
                if (!playState.IsInCutscene)
                  Magicka.Game.Instance.IsMouseVisible = true;
                Matrix oMatrix;
                GameStateManager.Instance.CurrentState.Scene.Camera.GetViewProjectionMatrix(iDataChannel, out oMatrix);
                if (this.mCursorPressedTarget != null && this.IsDown(KeyboardBindings.Move, ref iState))
                {
                  Vector2 screenPosition = MagickaMath.WorldToScreenPosition(ref this.mCursorLockPosition, ref oMatrix);
                  screenPosition.X += this.mMouseOffset.X;
                  screenPosition.Y += this.mMouseOffset.Y;
                  this.mLockedMousePos.X = (int) ((double) screenPosition.X + 0.5);
                  this.mLockedMousePos.Y = (int) ((double) screenPosition.Y + 0.5);
                  Mouse.SetPosition(this.mLockedMousePos.X, this.mLockedMousePos.Y);
                }
                Vector3 oPlanePosition;
                Segment mousePick = this.GetMousePick(ref oMatrix, ref screenSize, ref iState.NewMouseState, out oPlanePosition);
                bool flag11 = this.IsDown(KeyboardBindings.Shift, ref iState);
                bool flag12 = false;
                Magicka.GameLogic.Entities.Character character1 = (Magicka.GameLogic.Entities.Character) null;
                Pickable pickable = (Pickable) null;
                Interactable interactable1 = (Interactable) null;
                if (this.mStillPressing)
                {
                  if (this.mLockedTarget is Magicka.GameLogic.Entities.Character)
                    character1 = this.mLockedTarget as Magicka.GameLogic.Entities.Character;
                  else if (this.mLockedTarget is Pickable)
                    pickable = this.mLockedTarget as Pickable;
                  else if (this.mLockedTarget is Interactable)
                    interactable1 = this.mLockedTarget as Interactable;
                }
                if (!(this.mAvatar.Chanting | playState.IsInCutscene) && !flag11 && !this.mAvatar.Polymorphed)
                {
                  if (!this.mStillPressing)
                  {
                    character1 = this.FindCharacter(ref mousePick, ref oPlanePosition);
                    pickable = this.FindPickUp(ref mousePick, ref oPlanePosition);
                    interactable1 = this.FindInteractable(ref mousePick);
                    Vector3 iWorldPosition = new Vector3();
                    if (pickable != null)
                      iWorldPosition = pickable.Position;
                    else if (character1 != null)
                      iWorldPosition = character1.Position;
                    else if (interactable1 != null)
                      iWorldPosition = interactable1.Locator.Transform.Translation;
                    this.mLockedMousePos.X = iState.NewMouseState.X;
                    this.mLockedMousePos.Y = iState.NewMouseState.Y;
                    Vector2 screenPosition = MagickaMath.WorldToScreenPosition(ref iWorldPosition, ref oMatrix);
                    Vector2.Subtract(ref iMousePos, ref screenPosition, out this.mMouseOffset);
                  }
                  Magicka.GameLogic.Entities.Character character2 = this.mAvatar.FindCharacter(false);
                  Interactable interactable2 = this.mAvatar.FindInteractable(false);
                  if (character1 != null && character1.Dialog != 0 && (character1.Faction & this.mAvatar.Faction) != Factions.NONE)
                  {
                    character1.Highlight();
                    flag12 = character2 == character1;
                    iActive = flag12;
                    this.mStillPressing = true;
                    iCursor = Cursors.Talk;
                  }
                  else if (pickable != null)
                  {
                    pickable.Highlight();
                    iActive = flag12 = this.mAvatar.FindPickUp(false, pickable);
                    iCursor = Cursors.PickUp;
                    this.mStillPressing = true;
                  }
                  else if (interactable1 != null)
                  {
                    interactable1.Highlight();
                    iActive = flag12 = interactable2 == interactable1;
                    switch (interactable1.InteractType)
                    {
                      case InteractType.Pick_Up:
                        iCursor = Cursors.PickUp;
                        break;
                      case InteractType.Examine:
                        iCursor = Cursors.Examine;
                        break;
                      case InteractType.Talk:
                        iCursor = Cursors.Talk;
                        break;
                      default:
                        iCursor = Cursors.Interact;
                        break;
                    }
                    this.mStillPressing = true;
                  }
                }
                this.mAvatar.Player.NotifierButton.Hide();
                if (!(instance1.AwaitingInput | instance1.HoldoffInput))
                {
                  if (this.mAvatar.NotifySpecialAbility)
                    this.mAvatar.Player.NotifierButton.Show(this.mAvatar.SpecialAbilityName, ButtonChar.None, iMousePos);
                  else if (this.mAvatar.ChantingMagick)
                    this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int) this.mAvatar.MagickType]), ButtonChar.SpaceBar, iMousePos);
                  else if (!this.mAvatar.Chanting)
                  {
                    if (this.mAvatar.IsGripped)
                    {
                      this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[12]), ButtonChar.SpaceBar, iMousePos);
                      if (this.mAvatar.Boosts > 0)
                        this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
                    }
                    else if (!flag11 && !this.mAvatar.Polymorphed)
                    {
                      if (pickable != null)
                      {
                        string iText = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[1]);
                        if (!(pickable is Item) || ((pickable as Item).WeaponClass != WeaponClass.Staff || !this.mAvatar.Equipment[1].Item.IsBound) && (!Defines.IsWeapon((pickable as Item).WeaponClass) || !this.mAvatar.Equipment[0].Item.IsBound))
                          this.mAvatar.Player.NotifierButton.Show(iText, ButtonChar.None, iMousePos);
                      }
                      else if (interactable1 != null)
                        this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int) interactable1.InteractType]), ButtonChar.None, iMousePos);
                      else if (BoostState.Instance.ShieldToBoost((Magicka.GameLogic.Entities.Character) this.mAvatar) != null || this.mAvatar.IsSelfShielded && !this.mAvatar.IsSolidSelfShielded)
                      {
                        this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[6]), ButtonChar.None, iMousePos);
                        if (this.mAvatar.Boosts > 0)
                          this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
                      }
                      else if (character1 != null && character1.InteractText != InteractType.None && !AudioManager.Instance.Threat)
                        this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int) character1.InteractText]), ButtonChar.None, iMousePos);
                    }
                  }
                }
                this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
                if (instance3.IsInputLimited || instance3.IsPlayerInputLocked(this.Player.ID))
                {
                  this.mAvatar.UpdateMouseDirection(Vector2.Zero, false);
                }
                else
                {
                  this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
                  Vector2 iValue = new Vector2();
                  if (this.mLockedTarget == null)
                  {
                    iValue.X = oPlanePosition.X - this.mAvatar.Position.X;
                    iValue.Y = oPlanePosition.Z - this.mAvatar.Position.Z;
                  }
                  else
                  {
                    iValue.X = this.mCursorLockPosition.X - this.mAvatar.Position.X;
                    iValue.Y = this.mCursorLockPosition.Z - this.mAvatar.Position.Z;
                  }
                  this.mAvatar.UpdateMouseDirection(iValue, this.mInverted);
                  if (!this.IsDown(KeyboardBindings.Move, ref iState))
                  {
                    this.mInteractMoveLock = false;
                    this.mAvatar.MouseMoveStop();
                    this.mCursorPressedTarget = (object) null;
                    this.mLockedTarget = (object) null;
                    this.mStillPressing = false;
                  }
                  if (!playState.IsInCutscene)
                  {
                    if (flag11)
                    {
                      this.mAvatar.ForceReleased();
                      if (this.IsDown(KeyboardBindings.Cast, ref iState))
                        this.mAvatar.AreaPressed();
                      else
                        this.mAvatar.AreaReleased();
                      if (this.IsPressed(KeyboardBindings.Move, ref iState))
                        this.mAvatar.Attack();
                      else if (this.IsDown(KeyboardBindings.Move, ref iState) & (this.mAvatar.WieldingGun | this.mAvatar.Equipment[0].Item.SpellCharged))
                        this.mAvatar.Attack();
                      else if (this.IsReleased(KeyboardBindings.Move, ref iState))
                        this.mAvatar.AttackRelease();
                      iActive = true;
                      iCursor = Cursors.Attack;
                    }
                    else
                    {
                      this.mAvatar.AreaReleased();
                      if (this.IsDown(KeyboardBindings.Cast, ref iState))
                        this.mAvatar.ForcePressed();
                      else
                        this.mAvatar.ForceReleased();
                      if (this.mAvatar.Attacking)
                        this.mAvatar.AttackRelease();
                      if (this.IsPressed(KeyboardBindings.Move, ref iState))
                      {
                        if (!this.mAvatar.Chanting)
                        {
                          if (flag12)
                          {
                            if (pickable != null)
                              this.mAvatar.PickUp(pickable);
                            else if (character1 != null)
                              this.mAvatar.Action();
                            else if (interactable1 != null)
                              this.mAvatar.Interact();
                            this.mInteractMoveLock = true;
                          }
                          else
                          {
                            if (pickable != null)
                            {
                              this.mCursorPressedTarget = (object) pickable;
                              this.mCursorLockPosition = pickable.Position;
                            }
                            else if (character1 != null)
                            {
                              this.mCursorPressedTarget = (object) character1;
                              this.mCursorLockPosition = character1.Position;
                            }
                            else if (interactable1 != null)
                            {
                              this.mCursorPressedTarget = (object) interactable1;
                              this.mCursorLockPosition = interactable1.Locator.Transform.Translation;
                            }
                            else
                              this.mCursorPressedTarget = (object) null;
                            this.mLockedTarget = this.mCursorPressedTarget;
                          }
                        }
                      }
                      else if (this.IsDown(KeyboardBindings.Move, ref iState) & !this.mInteractMoveLock)
                      {
                        this.mAvatar.MouseMove();
                        if (!this.mAvatar.Chanting && this.mCursorPressedTarget != null && flag12)
                        {
                          if (this.mCursorPressedTarget is Magicka.GameLogic.Entities.Character)
                            this.mAvatar.Action();
                          else if (this.mCursorPressedTarget is Pickable)
                          {
                            this.mAvatar.PickUp(pickable);
                            this.mStillPressing = false;
                            this.mLockedTarget = (object) null;
                          }
                          else if (this.mCursorPressedTarget is Interactable)
                          {
                            this.mAvatar.Interact();
                            this.mStillPressing = false;
                            this.mLockedTarget = (object) null;
                          }
                          this.mCursorPressedTarget = (object) null;
                        }
                      }
                    }
                  }
                  if (this.IsPressed(KeyboardBindings.CastSelf, ref iState))
                    this.mAvatar.Special();
                  else if (this.IsReleased(KeyboardBindings.CastSelf, ref iState))
                    this.mAvatar.SpecialRelease();
                  if (this.IsPressed(KeyboardBindings.PrevMagick, ref iState))
                  {
                    MagickType iMagick = this.Player.IconRenderer.TomeMagick - 1;
                    if (iMagick == MagickType.NrOfMagicks | iMagick < MagickType.None)
                      iMagick = MagickType.Amalgameddon;
                    while (iMagick != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, iMagick))
                      --iMagick;
                    this.Player.IconRenderer.TomeMagick = iMagick;
                  }
                  else if (this.IsPressed(KeyboardBindings.NextMagick, ref iState))
                  {
                    MagickType iMagick = this.Player.IconRenderer.TomeMagick + 1;
                    while (iMagick != MagickType.NrOfMagicks && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, iMagick))
                      ++iMagick;
                    if (iMagick == MagickType.NrOfMagicks)
                      iMagick = MagickType.None;
                    this.Player.IconRenderer.TomeMagick = iMagick;
                  }
                  if (!NetworkChat.Instance.Active)
                  {
                    if (this.IsPressed(KeyboardBindings.Inventory, ref iState))
                      this.mAvatar.CheckInventory();
                    if (this.IsPressed(KeyboardBindings.Boost, ref iState))
                      this.mAvatar.Boost();
                    this.mAvatar.IsBlocking = this.IsDown(KeyboardBindings.Block, ref iState);
                    if (!this.mAvatar.Polymorphed)
                    {
                      if (this.IsPressed(KeyboardBindings.Water, ref iState) && this.mHUD.IconCoolDown(Elements.Water))
                      {
                        this.mAvatar.ConjureWater();
                        this.mHUD.CoolDown(Elements.Water);
                      }
                      if (this.IsPressed(KeyboardBindings.Lightning, ref iState) && this.mHUD.IconCoolDown(Elements.Lightning))
                      {
                        this.mAvatar.ConjureLightning();
                        this.mHUD.CoolDown(Elements.Lightning);
                      }
                      if (this.IsPressed(KeyboardBindings.Life, ref iState) && this.mHUD.IconCoolDown(Elements.Life))
                      {
                        this.mAvatar.ConjureLife();
                        this.mHUD.CoolDown(Elements.Life);
                      }
                      if (this.IsPressed(KeyboardBindings.Arcane, ref iState) && this.mHUD.IconCoolDown(Elements.Arcane))
                      {
                        this.mAvatar.ConjureArcane();
                        this.mHUD.CoolDown(Elements.Arcane);
                      }
                      if (this.IsPressed(KeyboardBindings.Shield, ref iState) && this.mHUD.IconCoolDown(Elements.Shield))
                      {
                        this.mAvatar.ConjureShield();
                        this.mHUD.CoolDown(Elements.Shield);
                      }
                      if (this.IsPressed(KeyboardBindings.Earth, ref iState) && this.mHUD.IconCoolDown(Elements.Earth))
                      {
                        this.mAvatar.ConjureEarth();
                        this.mHUD.CoolDown(Elements.Earth);
                      }
                      if (this.IsPressed(KeyboardBindings.Cold, ref iState) && this.mHUD.IconCoolDown(Elements.Cold))
                      {
                        this.mAvatar.ConjureCold();
                        this.mHUD.CoolDown(Elements.Cold);
                      }
                      if (this.IsPressed(KeyboardBindings.Fire, ref iState) && this.mHUD.IconCoolDown(Elements.Fire))
                      {
                        this.mAvatar.ConjureFire();
                        this.mHUD.CoolDown(Elements.Fire);
                      }
                    }
                  }
                }
              }
            }
            else
              Magicka.Game.Instance.IsMouseVisible = false;
          }
        }
        Magicka.Game.Instance.SetCursor(iActive, iCursor);
      }
    }
  }

  public override void Invert(bool iInvert)
  {
    if (!this.mInverted)
    {
      if (!iInvert)
        return;
      this.mInverted = true;
      Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[0], ref KeyboardMouseController.mKeyboardBindings[7]);
      Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[1], ref KeyboardMouseController.mKeyboardBindings[4]);
      Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[2], ref KeyboardMouseController.mKeyboardBindings[3]);
      Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[5], ref KeyboardMouseController.mKeyboardBindings[6]);
    }
    else
    {
      if (iInvert)
        return;
      SaveManager.Instance.KeyBindings.CopyTo((Array) KeyboardMouseController.mKeyboardBindings, 0);
      this.mInverted = false;
    }
  }

  protected char GetKeyboardInput(KeyboardState iNewKeyboardState)
  {
    Keys[] pressedKeys = iNewKeyboardState.GetPressedKeys();
    char c = char.MinValue;
    for (int index = 0; index < pressedKeys.Length; ++index)
    {
      Keys keys = pressedKeys[index];
      if ((ushort) keys >= (ushort) 96 /*0x60*/ & (ushort) keys <= (ushort) 105)
        pressedKeys[index] = keys = keys - 96 /*0x60*/ + 48 /*0x30*/;
      char ch1 = (char) pressedKeys[index];
      if (ch1 >= '0' & ch1 <= 'Z')
      {
        c = ch1;
        break;
      }
      char ch2;
      switch (keys)
      {
        case Keys.Back:
          ch2 = '\b';
          break;
        case Keys.Space:
          ch2 = ' ';
          break;
        case Keys.Separator:
          ch2 = '.';
          break;
        case Keys.Decimal:
          ch2 = ',';
          break;
        case Keys.Divide:
          ch2 = '/';
          break;
      }
    }
    if ((int) this.mLastChar != (int) c)
      this.mLastChar = c;
    else
      c = char.MinValue;
    return !(iNewKeyboardState.IsKeyDown(Keys.LeftShift) | iNewKeyboardState.IsKeyDown(Keys.RightShift)) ? char.ToLowerInvariant(c) : c;
  }

  protected Segment GetMousePick(
    ref Matrix iViewProjectionMatrix,
    ref Point iScreenSize,
    ref MouseState iMouseState,
    out Vector3 oPlanePosition)
  {
    Camera camera = GameStateManager.Instance.CurrentState.Scene.Camera;
    Matrix result1;
    Matrix.Invert(ref iViewProjectionMatrix, out result1);
    Vector4 result2 = new Vector4((float) (2.0 * (double) iMouseState.X / (double) iScreenSize.X - 1.0), (float) (1.0 - 2.0 * (double) iMouseState.Y / (double) iScreenSize.Y), 1f, 1f);
    Vector4.Transform(ref result2, ref result1, out result2);
    Vector3 result3 = new Vector3();
    result3.X = result2.X;
    result3.Y = result2.Y;
    result3.Z = result2.Z;
    Vector3.Divide(ref result3, result2.W, out result3);
    Microsoft.Xna.Framework.Plane plane = new Microsoft.Xna.Framework.Plane(Vector3.Up, -this.Player.Avatar.Position.Y);
    Vector3 result4 = camera.Position;
    Vector3.Subtract(ref result3, ref result4, out result4);
    Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray(camera.Position, result4);
    float? result5;
    ray.Intersects(ref plane, out result5);
    Vector3 result6 = ray.Position;
    if (result5.HasValue)
    {
      Vector3 result7 = ray.Direction;
      Vector3.Multiply(ref result7, result5.Value, out result7);
      Vector3.Add(ref result6, ref result7, out result6);
    }
    oPlanePosition = result6;
    Vector3 position = camera.Position;
    Vector3 result8;
    Vector3.Subtract(ref result3, ref position, out result8);
    return new Segment(position, result8);
  }

  protected Magicka.GameLogic.Entities.Character FindCharacter(Avatar iAvatar)
  {
    if (AudioManager.Instance.Threat)
      return (Magicka.GameLogic.Entities.Character) null;
    float num1 = float.MaxValue;
    Magicka.GameLogic.Entities.Character character1 = (Magicka.GameLogic.Entities.Character) null;
    List<Entity> entities = iAvatar.PlayState.EntityManager.GetEntities(iAvatar.Position, 3f, false);
    entities.Remove((Entity) iAvatar);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Magicka.GameLogic.Entities.Character character2 && character2.InteractText != InteractType.None)
      {
        Vector3 position = iAvatar.Position with
        {
          Y = 0.0f
        };
        Vector3 result = character2.Position with
        {
          Y = 0.0f
        };
        Vector3.Subtract(ref result, ref position, out result);
        float num2 = result.LengthSquared();
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          character1 = character2;
        }
      }
    }
    iAvatar.PlayState.EntityManager.ReturnEntityList(entities);
    return character1;
  }

  protected Magicka.GameLogic.Entities.Character FindCharacter(
    ref Segment iSegment,
    ref Vector3 iPlanePosition)
  {
    if (AudioManager.Instance.Threat)
      return (Magicka.GameLogic.Entities.Character) null;
    List<Entity> entities = this.mAvatar.PlayState.EntityManager.GetEntities(iPlanePosition, 6f, false);
    entities.Remove((Entity) this.mAvatar);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Magicka.GameLogic.Entities.Character character && character.Body.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSegment) && character.InteractText != InteractType.None)
      {
        this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
        return character;
      }
    }
    this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
    return (Magicka.GameLogic.Entities.Character) null;
  }

  protected Pickable FindPickUp(Avatar iAvatar)
  {
    float maxValue = float.MaxValue;
    Pickable pickUp = (Pickable) null;
    List<Entity> entities = iAvatar.PlayState.EntityManager.GetEntities(iAvatar.Position, 2.5f, false);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Pickable pickable && pickable.IsPickable)
      {
        Vector3 position1 = pickable.Position;
        Vector3 position2 = pickable.Position;
        float result;
        Vector3.DistanceSquared(ref position2, ref position1, out result);
        if ((double) result < (double) maxValue)
        {
          result = maxValue;
          pickUp = pickable;
        }
      }
    }
    iAvatar.PlayState.EntityManager.ReturnEntityList(entities);
    return pickUp;
  }

  protected Pickable FindPickUp(ref Segment iSegment, ref Vector3 iPlanePosition)
  {
    Pickable pickUp1 = (Pickable) null;
    List<Entity> entities = this.mAvatar.PlayState.EntityManager.GetEntities(iPlanePosition, 6f, false);
    entities.Remove((Entity) this.mAvatar);
    if (this.mAvatar.Equipment[0].Item != null)
      entities.Remove((Entity) this.mAvatar.Equipment[0].Item);
    if (this.mAvatar.Equipment[1].Item != null)
      entities.Remove((Entity) this.mAvatar.Equipment[1].Item);
    for (int index = 0; index < entities.Count; ++index)
    {
      if (entities[index] is Pickable pickUp2 && pickUp2.IsPickable && pickUp2.Body.CollisionSkin.SegmentIntersect(out float _, out Vector3 _, out Vector3 _, iSegment))
      {
        this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
        return pickUp2;
      }
    }
    this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
    return pickUp1;
  }

  protected Interactable FindInteractable(Avatar iAvatar)
  {
    if (AudioManager.Instance.Threat)
      return (Interactable) null;
    foreach (Trigger trigger in (IEnumerable<Trigger>) iAvatar.PlayState.Level.CurrentScene.Triggers.Values)
    {
      if (trigger is Interactable interactable && interactable.Enabled)
      {
        Vector3 position = iAvatar.Position with
        {
          Y = 0.0f
        };
        Locator locator = interactable.Locator;
        Vector3 translation = locator.Transform.Translation with
        {
          Y = 0.0f
        };
        float result;
        Vector3.DistanceSquared(ref translation, ref position, out result);
        float radius = iAvatar.Radius;
        if ((double) result <= (double) radius * (double) radius || (double) result <= (double) locator.Radius * (double) locator.Radius)
          return interactable;
      }
    }
    return (Interactable) null;
  }

  protected Interactable FindInteractable(ref Segment iSegment)
  {
    SortedList<int, Trigger> triggers = this.mAvatar.PlayState.Level.CurrentScene.Triggers;
    Interactable interactable1 = (Interactable) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < triggers.Count; ++index)
    {
      if (triggers.Values[index] is Interactable interactable2 && interactable2.Enabled)
      {
        Locator locator = interactable2.Locator;
        float num2 = Distance.PointSegmentDistanceSq(locator.Transform.Translation, iSegment);
        if ((double) num2 <= (double) locator.Radius * (double) locator.Radius && (double) num2 <= (double) num1)
        {
          num1 = num2;
          interactable1 = interactable2;
        }
      }
    }
    return interactable1 != null && AudioManager.Instance.Threat && interactable1.InteractType == InteractType.Talk ? (Interactable) null : interactable1;
  }

  private bool IsValidText(char iChar)
  {
    return char.IsLetterOrDigit(iChar) | iChar == '\b' | iChar == ' ' | char.IsPunctuation(iChar) | char.IsSymbol(iChar);
  }

  private void OnKeyDown(KeyData iData)
  {
    if (KeyboardMouseController.mCatchKeyActive)
    {
      KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(iData.Key, KeyboardMouseController.mCatchKeyIndex);
      if ((iData.Key & Keys.Escape) != Keys.Escape)
        return;
      KeyboardMouseController.mCatchKeyActive = false;
    }
    else if (PdxLoginWindow.Instance.Visible)
      PdxLoginWindow.Instance.OnKeyDown(iData);
    else if (PdxWidget.Instance.Visible)
      PdxLoginWindow.Instance.OnKeyDown(iData);
    else if (DialogManager.Instance.MessageBoxActive)
    {
      if (iData.Key == Keys.Enter)
        DialogManager.Instance.ControllerSelect((Controller) this);
      else if (iData.Key == Keys.Up)
        DialogManager.Instance.ControllerMove((Controller) this, ControllerDirection.Up);
      else if (iData.Key == Keys.Down)
        DialogManager.Instance.ControllerMove((Controller) this, ControllerDirection.Down);
      else if (iData.Key == Keys.Left)
      {
        DialogManager.Instance.ControllerMove((Controller) this, ControllerDirection.Left);
      }
      else
      {
        if (iData.Key != Keys.Right)
          return;
        DialogManager.Instance.ControllerMove((Controller) this, ControllerDirection.Right);
      }
    }
    else if ((iData.Key & Keys.Escape) == Keys.None && GameStateManager.Instance.CurrentState is PlayState && !(GameStateManager.Instance.CurrentState as PlayState).IsPaused)
    {
      if ((GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
        (GameStateManager.Instance.CurrentState as PlayState).SkipCutscene();
      else
        TutorialManager.Instance.RemoveDialogHint();
    }
    else
    {
      if (!(Tome.Instance.CurrentState is Tome.OpenState & NetworkManager.Instance.State != NetworkState.Offline) || iData.Key != Keys.Enter)
        return;
      NetworkChat.Instance.SendMessage();
    }
  }

  private void OnKeyPress(char iChar, KeyModifiers iModifiers)
  {
    if (!this.IsValidText(iChar))
      return;
    if (PdxLoginWindow.Instance.Visible)
      PdxLoginWindow.Instance.OnKeyPress(iChar);
    else if (PdxWidget.Instance.Visible)
      PdxLoginWindow.Instance.OnKeyPress(iChar);
    else if (DialogManager.Instance.MessageBoxActive)
    {
      DialogManager.Instance.ControllerType((Controller) this, iChar);
    }
    else
    {
      if (!(Tome.Instance.CurrentState is Tome.OpenState & NetworkManager.Instance.State != NetworkState.Offline))
        return;
      NetworkChat.Instance.TakeInput((Controller) this, iChar);
    }
  }

  private void OnKeyUp(KeyData iData)
  {
  }

  private static ControllerDirection GetDirection(
    ref KeyboardState iState,
    Keys iUp,
    Keys iDown,
    Keys iLeft,
    Keys iRight)
  {
    ControllerDirection direction = ControllerDirection.Center;
    if (iState.IsKeyDown(iUp))
      direction |= ControllerDirection.Up;
    if (iState.IsKeyDown(iDown))
      direction |= ControllerDirection.Down;
    if (iState.IsKeyDown(iLeft))
      direction |= ControllerDirection.Left;
    if (iState.IsKeyDown(iRight))
      direction |= ControllerDirection.Right;
    if ((direction & (ControllerDirection.Up | ControllerDirection.Down)) == (ControllerDirection.Up | ControllerDirection.Down))
      direction &= ~(ControllerDirection.Up | ControllerDirection.Down);
    if ((direction & (ControllerDirection.Right | ControllerDirection.Left)) == (ControllerDirection.Right | ControllerDirection.Left))
      direction &= ~(ControllerDirection.Right | ControllerDirection.Left);
    return direction;
  }

  public override void Rumble(float iLeft, float iRight)
  {
  }

  public override float LeftRumble() => 0.0f;

  public override float RightRumble() => 0.0f;

  public override void Clear()
  {
  }

  public void SetKey(KeyboardBindings iIndex, Keys iKey)
  {
    KeyboardMouseController.mKeyboardBindings[(int) iIndex] = new KeyboardMouseController.Binding(iKey);
  }

  internal KeyboardMouseController.Binding GetBinding(KeyboardBindings iIndex)
  {
    return KeyboardMouseController.mKeyboardBindings[(int) iIndex];
  }

  public static void LoadDefaults()
  {
    KeyboardMouseController.mKeyboardBindings[6] = new KeyboardMouseController.Binding(Keys.D);
    KeyboardMouseController.mKeyboardBindings[0] = new KeyboardMouseController.Binding(Keys.Q);
    KeyboardMouseController.mKeyboardBindings[3] = new KeyboardMouseController.Binding(Keys.R);
    KeyboardMouseController.mKeyboardBindings[7] = new KeyboardMouseController.Binding(Keys.F);
    KeyboardMouseController.mKeyboardBindings[4] = new KeyboardMouseController.Binding(Keys.A);
    KeyboardMouseController.mKeyboardBindings[5] = new KeyboardMouseController.Binding(Keys.S);
    KeyboardMouseController.mKeyboardBindings[1] = new KeyboardMouseController.Binding(Keys.W);
    KeyboardMouseController.mKeyboardBindings[2] = new KeyboardMouseController.Binding(Keys.E);
    KeyboardMouseController.mKeyboardBindings[14] = new KeyboardMouseController.Binding(Keys.Tab);
    KeyboardMouseController.mKeyboardBindings[13] = new KeyboardMouseController.Binding(Keys.LeftShift);
    KeyboardMouseController.mKeyboardBindings[12] = new KeyboardMouseController.Binding(Keys.LeftControl);
    KeyboardMouseController.mKeyboardBindings[10] = new KeyboardMouseController.Binding(Keys.Space);
    KeyboardMouseController.mKeyboardBindings[9] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.Middle);
    KeyboardMouseController.mKeyboardBindings[11] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.Left);
    KeyboardMouseController.mKeyboardBindings[8] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.Right);
    KeyboardMouseController.mKeyboardBindings[15] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.ScrollUp);
    KeyboardMouseController.mKeyboardBindings[16 /*0x10*/] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.ScrollDown);
    KeyboardMouseController.mKeyboardBindings.CopyTo((Array) SaveManager.Instance.KeyBindings, 0);
  }

  public static bool IsValidKey(Keys iKey, KeyboardBindings iFuction)
  {
    if (iKey == Keys.Escape | iKey == Keys.LeftWindows | iKey == Keys.RightWindows | iKey == Keys.Apps)
      return false;
    for (int index = 0; index < KeyboardMouseController.mKeyboardBindings.Length; ++index)
    {
      if (!KeyboardMouseController.mKeyboardBindings[index].IsMouse && (Keys) KeyboardMouseController.mKeyboardBindings[index].Button == iKey)
        KeyboardMouseController.mKeyboardBindings[index] = new KeyboardMouseController.Binding();
    }
    KeyboardMouseController.mKeyboardBindings[(int) iFuction] = new KeyboardMouseController.Binding(iKey);
    return true;
  }

  public static bool IsValidKey(
    KeyboardMouseController.MouseButton iButton,
    KeyboardBindings iFuction)
  {
    for (int index = 0; index < KeyboardMouseController.mKeyboardBindings.Length; ++index)
    {
      if (KeyboardMouseController.mKeyboardBindings[index].IsMouse && (KeyboardMouseController.MouseButton) KeyboardMouseController.mKeyboardBindings[index].Button == iButton)
        KeyboardMouseController.mKeyboardBindings[index] = new KeyboardMouseController.Binding();
    }
    KeyboardMouseController.mKeyboardBindings[(int) iFuction] = new KeyboardMouseController.Binding(iButton);
    return true;
  }

  private bool IsDown(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
  {
    if (!KeyboardMouseController.mKeyboardBindings[(int) iFuction].IsMouse)
      return iState.NewKeyboardState.IsKeyDown((Keys) KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button);
    switch (KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button)
    {
      case 1:
        return iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      case 2:
        return iState.NewMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      case 3:
        return iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      case 4:
        return iState.NewMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      case 5:
        return iState.NewMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      default:
        return false;
    }
  }

  private bool IsPressed(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
  {
    if (KeyboardMouseController.mKeyboardBindings[(int) iFuction].IsMouse)
    {
      switch (KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button)
      {
        case 1:
          return iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        case 2:
          return iState.NewMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        case 3:
          return iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
        case 4:
          return iState.NewMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Released;
        case 5:
          return iState.NewMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Released;
        case 6:
          return iState.NewMouseState.ScrollWheelValue > iState.OldMouseState.ScrollWheelValue;
        case 7:
          return iState.NewMouseState.ScrollWheelValue < iState.OldMouseState.ScrollWheelValue;
        default:
          return false;
      }
    }
    else
      return iState.NewKeyboardState.IsKeyDown((Keys) KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button) && iState.OldKeyboardState.IsKeyUp((Keys) KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button);
  }

  private bool IsReleased(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
  {
    if (KeyboardMouseController.mKeyboardBindings[(int) iFuction].IsMouse)
    {
      switch (KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button)
      {
        case 1:
          return iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iState.OldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        case 2:
          return iState.NewMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iState.OldMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        case 3:
          return iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released && iState.OldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        case 4:
          return iState.NewMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Released && iState.OldMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        case 5:
          return iState.NewMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Released && iState.OldMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
        default:
          return false;
      }
    }
    else
      return iState.NewKeyboardState.IsKeyUp((Keys) KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button) && iState.OldKeyboardState.IsKeyDown((Keys) KeyboardMouseController.mKeyboardBindings[(int) iFuction].Button);
  }

  private bool IsPressed(Keys iKey, ref KeyboardMouseController.InputState iState)
  {
    return iState.NewKeyboardState.IsKeyDown(iKey) && iState.OldKeyboardState.IsKeyUp(iKey);
  }

  private bool IsPressed(
    KeyboardMouseController.MouseButton iButton,
    ref KeyboardMouseController.InputState iState)
  {
    switch (iButton)
    {
      case KeyboardMouseController.MouseButton.Left:
        return iState.NewMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
      case KeyboardMouseController.MouseButton.Middle:
        return iState.NewMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
      case KeyboardMouseController.MouseButton.Right:
        return iState.NewMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released;
      case KeyboardMouseController.MouseButton.X1:
        return iState.NewMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Released;
      case KeyboardMouseController.MouseButton.X2:
        return iState.NewMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Pressed && iState.OldMouseState.XButton2 == Microsoft.Xna.Framework.Input.ButtonState.Released;
      case KeyboardMouseController.MouseButton.ScrollUp:
        return iState.NewMouseState.ScrollWheelValue > iState.OldMouseState.ScrollWheelValue;
      case KeyboardMouseController.MouseButton.ScrollDown:
        return iState.NewMouseState.ScrollWheelValue < iState.OldMouseState.ScrollWheelValue;
      default:
        return false;
    }
  }

  public static string KeyToString(KeyboardBindings iKey)
  {
    KeyboardMouseController.Binding mKeyboardBinding = KeyboardMouseController.mKeyboardBindings[(int) iKey];
    if (mKeyboardBinding.Button == (byte) 0)
      return " ";
    if (mKeyboardBinding.IsMouse)
      return LanguageManager.Instance.GetString(KeyboardMouseController.MOUSE_BUTTONS[(int) mKeyboardBinding.Button]);
    Keys button = (Keys) mKeyboardBinding.Button;
    return button >= Keys.D0 && button <= Keys.Divide || button >= Keys.OemSemicolon && button <= Keys.OemBackslash ? char.ToUpper((char) InputMessageFilter.MapVirtualKey((uint) button, 2U)).ToString() : button.ToString();
  }

  private struct InputState
  {
    public KeyboardState NewKeyboardState;
    public KeyboardState OldKeyboardState;
    public MouseState NewMouseState;
    public MouseState OldMouseState;
  }

  internal enum MouseButton
  {
    Invalid,
    Left,
    Middle,
    Right,
    X1,
    X2,
    ScrollUp,
    ScrollDown,
  }

  internal struct Binding
  {
    internal bool IsMouse;
    internal byte Button;

    public Binding(Keys iKey)
    {
      this.IsMouse = false;
      this.Button = (byte) iKey;
    }

    public Binding(KeyboardMouseController.MouseButton iButton)
    {
      this.IsMouse = true;
      this.Button = (byte) iButton;
    }
  }
}
