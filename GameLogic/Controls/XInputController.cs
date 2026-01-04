// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.XInputController
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.Entities.CharacterStates;
using Magicka.GameLogic.Entities.Items;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.InGameMenus;
using Magicka.GameLogic.GameStates.Persistent;
using Magicka.GameLogic.Spells;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal class XInputController : Controller
{
  public static readonly int[] BUTTON_NAMES;
  public static readonly string COLDKEY = new string('̠', 1) + new string('̣', 1);
  public static readonly string SELFKEY = new string('̧', 1);
  public static readonly string WEAPONKEY = new string('̤', 1);
  private PlayerIndex mPlayerIndex;
  private GamePadState mOldState;
  private float mLeftRumble;
  private float mRightRumble;
  private float mMenuMoveCooldown;
  private ControllerDirection mOldNavDirection;
  private Vector2 mPreviousPosition;
  private ControllerDirection mCurrentDirection;
  private ControllerDirection mStoredDirection;
  private ControllerDirection mVisualDirection;
  private float[] mFadeTimers;
  private float[] mMoveTimers;
  private bool mInputLocked;
  private bool mHelpActive;
  private float mHelpTime;
  private XInputController.Binding[] mBindings;

  public event Action<XInputController.Binding> OnChange;

  static XInputController()
  {
    XInputController.BUTTON_NAMES = new int[32 /*0x20*/];
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(1U)] = "#xb_dpad_up".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(2U)] = "#xb_dpad_down".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4U)] = "#xb_dpad_left".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8U)] = "#xb_dpad_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16U /*0x10*/)] = "#xb_start".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(32U /*0x20*/)] = "#xb_back".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(64U /*0x40*/)] = "#xb_thumb_left".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(128U /*0x80*/)] = "#xb_thumb_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(256U /*0x0100*/)] = "#xb_bumper_left".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(512U /*0x0200*/)] = "#xb_bumper_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4096U /*0x1000*/)] = "#xb_a".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8192U /*0x2000*/)] = "#xb_b".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16384U /*0x4000*/)] = "#xb_x".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(32768U /*0x8000*/)] = "#xb_y".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4194304U /*0x400000*/)] = "#xb_trigger_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8388608U /*0x800000*/)] = "#xb_trigger_left".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16777216U /*0x01000000*/)] = "#xb_thumb_right_up".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(33554432U /*0x02000000*/)] = "#xb_thumb_right_down".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(67108864U /*0x04000000*/)] = "#xb_thumb_right_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(134217728U /*0x08000000*/)] = "#xb_thumb_right_left".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(268435456U /*0x10000000*/)] = "#xb_thumb_left_up".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(536870912U /*0x20000000*/)] = "#xb_thumb_left_down".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(1073741824U /*0x40000000*/)] = "#xb_thumb_left_right".GetHashCodeCustom();
    XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(2097152U /*0x200000*/)] = "#xb_thumb_left_left".GetHashCodeCustom();
  }

  public static string GetButtonName(Buttons iButton)
  {
    return LanguageManager.Instance.GetString(XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits((uint) iButton)]);
  }

  public XInputController(PlayerIndex iPlayerIndex)
  {
    this.mBindings = GlobalSettings.Instance.XInputBindings[(int) iPlayerIndex];
    if (this.mBindings == null)
    {
      this.mBindings = new XInputController.Binding[24];
      GlobalSettings.Instance.XInputBindings[(int) iPlayerIndex] = this.mBindings;
      this.LoadDefaults();
    }
    this.mPlayerIndex = iPlayerIndex;
    this.mHelpActive = false;
    this.mFadeTimers = new float[4];
    this.mMoveTimers = new float[4];
  }

  public void LoadDefaults()
  {
    this.mBindings[0] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 0);
    this.mBindings[1] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 1);
    this.mBindings[2] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 0);
    this.mBindings[3] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 1);
    this.mBindings[4] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 4096 /*0x1000*/);
    this.mBindings[5] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192 /*0x2000*/);
    this.mBindings[6] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 2);
    this.mBindings[7] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 3);
    this.mBindings[8] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 2);
    this.mBindings[9] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 3);
    this.mBindings[10] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 2);
    this.mBindings[11] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 1);
    this.mBindings[12] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 4096 /*0x1000*/);
    this.mBindings[13] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32768 /*0x8000*/);
    this.mBindings[14] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 512 /*0x0200*/);
    this.mBindings[15] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 16384 /*0x4000*/);
    this.mBindings[16 /*0x10*/] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192 /*0x2000*/);
    this.mBindings[17] = new XInputController.Binding(XInputController.Binding.BindingType.Trigger, 1);
    this.mBindings[18] = new XInputController.Binding(XInputController.Binding.BindingType.Trigger, 0);
    this.mBindings[19] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32768 /*0x8000*/);
    this.mBindings[20] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192 /*0x2000*/);
    this.mBindings[21] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 16 /*0x10*/);
    this.mBindings[22] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32 /*0x20*/);
    this.mBindings[23] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 128 /*0x80*/);
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    DialogManager instance1 = DialogManager.Instance;
    Tome instance2 = Tome.Instance;
    GameState currentState = GameStateManager.Instance.CurrentState;
    PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
    TutorialManager instance3 = TutorialManager.Instance;
    this.mAvatar = this.Player == null ? (Avatar) null : this.Player.Avatar;
    GamePadState state;
    try
    {
      state = GamePad.GetState(this.mPlayerIndex);
    }
    catch (InvalidOperationException ex)
    {
      this.mOldState = new GamePadState();
      return;
    }
    if (this.OnChange != null)
    {
      XInputController.Binding binding = new XInputController.Binding();
      for (int index = 0; index < 16 /*0x10*/; ++index)
      {
        Buttons button = (Buttons) (1 << index);
        if (state.IsButtonDown(button) & this.mOldState.IsButtonUp(button))
        {
          binding.Type = XInputController.Binding.BindingType.Button;
          binding.BindingIndex = (int) button;
          break;
        }
      }
      if (binding.Type == XInputController.Binding.BindingType.None)
      {
        if ((double) state.Triggers.Left >= 0.5 & (double) this.mOldState.Triggers.Left < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.Trigger;
          binding.BindingIndex = 0;
        }
        else if ((double) state.Triggers.Right >= 0.5 & (double) this.mOldState.Triggers.Right < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.Trigger;
          binding.BindingIndex = 1;
        }
      }
      if (binding.Type == XInputController.Binding.BindingType.None)
      {
        if ((double) state.ThumbSticks.Left.X >= 0.5 & (double) this.mOldState.ThumbSticks.Left.X < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.PositiveStick;
          binding.BindingIndex = 0;
        }
        else if ((double) state.ThumbSticks.Left.Y >= 0.5 & (double) this.mOldState.ThumbSticks.Left.Y < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.PositiveStick;
          binding.BindingIndex = 1;
        }
        else if ((double) state.ThumbSticks.Right.X >= 0.5 & (double) this.mOldState.ThumbSticks.Right.X < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.PositiveStick;
          binding.BindingIndex = 2;
        }
        else if ((double) state.ThumbSticks.Right.Y >= 0.5 & (double) this.mOldState.ThumbSticks.Right.Y < 0.5)
        {
          binding.Type = XInputController.Binding.BindingType.PositiveStick;
          binding.BindingIndex = 3;
        }
        else if ((double) state.ThumbSticks.Left.X <= -0.5 & (double) this.mOldState.ThumbSticks.Left.X > -0.5)
        {
          binding.Type = XInputController.Binding.BindingType.NegativeStick;
          binding.BindingIndex = 0;
        }
        else if ((double) state.ThumbSticks.Left.Y <= -0.5 & (double) this.mOldState.ThumbSticks.Left.Y > -0.5)
        {
          binding.Type = XInputController.Binding.BindingType.NegativeStick;
          binding.BindingIndex = 1;
        }
        else if ((double) state.ThumbSticks.Right.X <= -0.5 & (double) this.mOldState.ThumbSticks.Right.X > -0.5)
        {
          binding.Type = XInputController.Binding.BindingType.NegativeStick;
          binding.BindingIndex = 2;
        }
        else if ((double) state.ThumbSticks.Right.Y <= -0.5 & (double) this.mOldState.ThumbSticks.Right.Y > -0.5)
        {
          binding.Type = XInputController.Binding.BindingType.NegativeStick;
          binding.BindingIndex = 3;
        }
      }
      if (binding.Type != XInputController.Binding.BindingType.None)
      {
        this.OnChange(binding);
        this.OnChange = (Action<XInputController.Binding>) null;
        this.mOldState = state;
        this.mMenuMoveCooldown = float.PositiveInfinity;
        return;
      }
    }
    ControllerDirection controllerDirection = ControllerDirection.Center;
    if (this.GetBoundValueB(ControllerFunction.Move_Right, state))
      controllerDirection |= ControllerDirection.Right;
    if (this.GetBoundValueB(ControllerFunction.Move_Up, state))
      controllerDirection |= ControllerDirection.Up;
    if (this.GetBoundValueB(ControllerFunction.Move_Left, state))
      controllerDirection |= ControllerDirection.Left;
    if (this.GetBoundValueB(ControllerFunction.Move_Down, state))
      controllerDirection |= ControllerDirection.Down;
    GamePad.SetVibration(this.mPlayerIndex, MathHelper.Clamp(this.mLeftRumble, 0.0f, 1f), MathHelper.Clamp(this.mRightRumble, 0.0f, 1f));
    this.mLeftRumble -= iDeltaTime * 3f;
    this.mRightRumble -= iDeltaTime * 3f;
    if (currentState is CompanyState & this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
      (currentState as CompanyState).SkipScreen();
    else if (persistentState.IsolateControls())
    {
      this.mMenuMoveCooldown -= iDeltaTime;
      if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
        persistentState.ControllerA((Controller) this);
      if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
        persistentState.ControllerB((Controller) this);
      if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
        persistentState.ControllerX((Controller) this);
      if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, state, this.mOldState))
        persistentState.ControllerY((Controller) this);
      if (controllerDirection == ControllerDirection.Center)
        this.mMenuMoveCooldown = 0.25f;
      else if (controllerDirection != this.mOldNavDirection || (double) this.mMenuMoveCooldown <= 0.0)
      {
        persistentState.ControllerMovement((Controller) this, controllerDirection);
        this.mMenuMoveCooldown += 0.25f;
      }
    }
    else if (instance1.MessageBoxActive)
    {
      this.mMenuMoveCooldown -= iDeltaTime;
      if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
        instance1.ControllerSelect((Controller) this);
      if (state.Buttons.B == ButtonState.Pressed)
        instance1.ControllerEsc((Controller) this);
      if (controllerDirection == ControllerDirection.Center)
        this.mMenuMoveCooldown = 0.25f;
      else if (controllerDirection != this.mOldNavDirection || (double) this.mMenuMoveCooldown <= 0.0)
      {
        instance1.ControllerMove((Controller) this, controllerDirection);
        this.mMenuMoveCooldown += 0.25f;
      }
    }
    else
    {
      switch (currentState)
      {
        case MenuState _:
          this.mMenuMoveCooldown -= iDeltaTime;
          if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
            instance2.ControllerA((Controller) this);
          if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
            instance2.ControllerB((Controller) this);
          if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
            instance2.ControllerX((Controller) this);
          if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, state, this.mOldState))
            instance2.ControllerY((Controller) this);
          if (controllerDirection == ControllerDirection.Center)
          {
            this.mMenuMoveCooldown = 0.25f;
            break;
          }
          if (controllerDirection != this.mOldNavDirection || (double) this.mMenuMoveCooldown <= 0.0)
          {
            instance2.ControllerMovement((Controller) this, controllerDirection);
            this.mMenuMoveCooldown += 0.25f;
            break;
          }
          break;
        case PlayState _:
          if (this.Player != null && this.Player.Playing)
          {
            PlayState playState = currentState as PlayState;
            if (playState.IsPaused)
            {
              this.mMenuMoveCooldown -= iDeltaTime;
              if (this.GetBoundValuePressed(ControllerFunction.Pause, state, this.mOldState))
                playState.TogglePause((Controller) this);
              if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
                InGameMenu.ControllerSelect((Controller) this);
              if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
                InGameMenu.ControllerBack((Controller) this);
              if (controllerDirection == ControllerDirection.Center)
                this.mMenuMoveCooldown = 0.25f;
              else if (controllerDirection != this.mOldNavDirection || (double) this.mMenuMoveCooldown <= 0.0)
              {
                InGameMenu.ControllerMove((Controller) this, controllerDirection);
                this.mMenuMoveCooldown += 0.25f;
              }
            }
            else
            {
              if (this.GetBoundValuePressed(ControllerFunction.Pause, state, this.mOldState))
                playState.TogglePause((Controller) this);
              else if (this.GetBoundValuePressed(ControllerFunction.Attack, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Boost, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Special, state, this.mOldState))
              {
                if (playState.IsInCutscene)
                  playState.SkipCutscene();
                instance3.RemoveDialogHint();
              }
              if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState) && instance1.CanAdvance((Controller) this) && !playState.IsInCutscene)
                instance1.Advance((Controller) this);
            }
            if (this.mAvatar != null && !this.mAvatar.Dead && (currentState as PlayState).Level.CurrentScene != null)
            {
              this.mAvatar.Player.NotifierButton.Hide();
              if (!DialogManager.Instance.AwaitingInput && !DialogManager.Instance.HoldoffInput)
              {
                if (this.mAvatar.NotifySpecialAbility)
                  this.mAvatar.Player.NotifierButton.Show(this.mAvatar.SpecialAbilityName, ButtonChar.Y, (Entity) this.mAvatar);
                else if (this.mAvatar.ChantingMagick)
                  this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int) this.mAvatar.MagickType]), ButtonChar.B, (Entity) this.mAvatar);
                else if (!this.mAvatar.Chanting)
                {
                  if (this.mAvatar.IsGripped)
                  {
                    this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[12]), ButtonChar.B, (Entity) this.mAvatar);
                    if (this.mAvatar.Boosts > 0)
                      this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
                  }
                  else if (!this.mAvatar.Polymorphed)
                  {
                    Pickable pickUp = this.mAvatar.FindPickUp(true);
                    if (pickUp != null)
                    {
                      string iText = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[1]);
                      if (pickUp is Item && ((pickUp as Item).WeaponClass != WeaponClass.Staff || !this.mAvatar.Equipment[1].Item.IsBound) && (!Defines.IsWeapon((pickUp as Item).WeaponClass) || !this.mAvatar.Equipment[0].Item.IsBound))
                      {
                        this.mAvatar.Player.NotifierButton.Show(iText, ButtonChar.X, (Entity) this.mAvatar);
                        pickUp.Highlight();
                      }
                      else if (!(pickUp is Item))
                      {
                        this.mAvatar.Player.NotifierButton.Show(iText, ButtonChar.X, (Entity) this.mAvatar);
                        pickUp.Highlight();
                      }
                    }
                    else
                    {
                      Interactable interactable = this.mAvatar.FindInteractable(true);
                      if (interactable != null && (!AudioManager.Instance.Threat || interactable.InteractType != InteractType.Talk))
                      {
                        this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int) interactable.InteractType]), ButtonChar.X, (Entity) this.mAvatar);
                        interactable.Highlight();
                      }
                      else if (BoostState.Instance.ShieldToBoost((Magicka.GameLogic.Entities.Character) this.mAvatar) != null || this.mAvatar.IsSelfShielded && !this.mAvatar.IsSolidSelfShielded)
                      {
                        this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[6]), ButtonChar.B, (Entity) this.mAvatar);
                        if (this.mAvatar.Boosts > 0)
                          this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
                      }
                      else
                      {
                        Magicka.GameLogic.Entities.Character character = this.mAvatar.FindCharacter(true);
                        if (character != null && character.InteractText != InteractType.None && !AudioManager.Instance.Threat)
                        {
                          this.mAvatar.Player.NotifierButton.Show(LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int) character.InteractText]), ButtonChar.X, (Entity) this.mAvatar);
                          character.Highlight();
                        }
                      }
                    }
                  }
                }
              }
              if (KeyboardHUD.Instance.UIEnabled)
                this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
              if (!ControlManager.Instance.IsInputLimited && !ControlManager.Instance.IsPlayerInputLocked(this.mAvatar.Player.ID))
              {
                if (this.GetBoundValuePressed(ControllerFunction.Attack, state, this.mOldState) | this.GetBoundValueB(ControllerFunction.Attack, state) & (this.mAvatar.WieldingGun | this.mAvatar.Equipment[0].Item.SpellCharged))
                  this.mAvatar.Attack();
                else if (this.GetBoundValueReleased(ControllerFunction.Attack, state, this.mOldState))
                  this.mAvatar.AttackRelease();
                if (this.GetBoundValuePressed(ControllerFunction.Boost, state, this.mOldState))
                  this.mAvatar.Boost();
                if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
                  this.mAvatar.Action();
                if (this.GetBoundValuePressed(ControllerFunction.Special, state, this.mOldState))
                  this.mAvatar.Special();
                else if (this.GetBoundValueReleased(ControllerFunction.Special, state, this.mOldState))
                  this.mAvatar.SpecialRelease();
                if (this.GetBoundValuePressed(ControllerFunction.Inventory, state, this.mOldState))
                  this.mAvatar.CheckInventory();
                this.mHelpActive = this.GetBoundValueB(ControllerFunction.Spell_Wheel, state);
                this.mAvatar.IsBlocking = this.GetBoundValueB(ControllerFunction.Block, state);
                Vector2 iValue = new Vector2();
                iValue.X += this.GetBoundValueF(ControllerFunction.Spell_Right, state);
                iValue.X -= this.GetBoundValueF(ControllerFunction.Spell_Left, state);
                iValue.Y += this.GetBoundValueF(ControllerFunction.Spell_Up, state);
                iValue.Y -= this.GetBoundValueF(ControllerFunction.Spell_Down, state);
                if (this.mAvatar.Polymorphed)
                  this.RightStickUpdate(new Vector2());
                else
                  this.RightStickUpdate(iValue);
                Vector2 result = new Vector2();
                result.X += this.GetBoundValueF(ControllerFunction.Move_Right, state);
                result.X -= this.GetBoundValueF(ControllerFunction.Move_Left, state);
                result.Y += this.GetBoundValueF(ControllerFunction.Move_Up, state);
                result.Y -= this.GetBoundValueF(ControllerFunction.Move_Down, state);
                float divider = result.Length();
                if ((double) divider > 1.0)
                  Vector2.Divide(ref result, divider, out result);
                else if ((double) divider < 0.25)
                  result = new Vector2();
                this.mAvatar.UpdatePadDirection(result, this.mInverted);
                if (this.GetBoundValueB(ControllerFunction.Cast_Area, state))
                  this.mAvatar.AreaPressed();
                else
                  this.mAvatar.AreaReleased();
                if (this.GetBoundValueB(ControllerFunction.Cast_Force, state))
                  this.mAvatar.ForcePressed();
                else
                  this.mAvatar.ForceReleased();
                if (this.GetBoundValuePressed(ControllerFunction.Magick_Next, state, this.mOldState))
                {
                  MagickType iMagick = this.Player.IconRenderer.TomeMagick + 1;
                  while (iMagick != MagickType.NrOfMagicks && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, iMagick))
                    ++iMagick;
                  if (iMagick == MagickType.NrOfMagicks)
                    iMagick = MagickType.None;
                  this.Player.IconRenderer.TomeMagick = iMagick;
                  break;
                }
                if (this.GetBoundValuePressed(ControllerFunction.Magick_Prev, state, this.mOldState))
                {
                  MagickType iMagick = this.Player.IconRenderer.TomeMagick - 1;
                  if (iMagick == MagickType.NrOfMagicks | iMagick < MagickType.None)
                    iMagick = MagickType.Amalgameddon;
                  while (iMagick != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, iMagick))
                    --iMagick;
                  this.Player.IconRenderer.TomeMagick = iMagick;
                  break;
                }
                break;
              }
              this.mAvatar.UpdatePadDirection(Vector2.Zero, false);
              break;
            }
            break;
          }
          break;
      }
    }
    this.mOldNavDirection = controllerDirection;
    this.mOldState = state;
    if (ControlManager.Instance.IsInputLimited || this.mAvatar == null || this.mAvatar.Dead)
    {
      this.mHelpActive = false;
      this.mInputLocked = false;
    }
    else
    {
      if (this.Player == null)
        return;
      SpellWheel spellWheel = this.Player.SpellWheel;
      int num = -1;
      if (!this.mInputLocked && this.mCurrentDirection != ControllerDirection.Center)
      {
        int directionIndex = SpellWheel.GetDirectionIndex(this.mCurrentDirection);
        for (int index = 0; index < this.mFadeTimers.Length; ++index)
        {
          if (directionIndex == index)
          {
            this.mFadeTimers[index] += iDeltaTime;
            if ((double) this.mFadeTimers[index] > 0.20000000298023224)
              this.mFadeTimers[index] = 0.2f;
            this.mMoveTimers[index] += iDeltaTime;
            if ((double) this.mMoveTimers[index] > 0.20000000298023224)
              this.mMoveTimers[index] = 0.2f;
          }
          else
          {
            this.mFadeTimers[index] -= iDeltaTime;
            if ((double) this.mFadeTimers[index] < 0.0)
              this.mFadeTimers[index] = 0.0f;
          }
        }
        if (this.mStoredDirection != ControllerDirection.Center)
          spellWheel.Direction(this.mStoredDirection);
      }
      else
      {
        if (!this.mInputLocked && this.mVisualDirection != ControllerDirection.Center)
        {
          num = SpellWheel.GetDirectionIndex(this.mVisualDirection);
          spellWheel.Direction(this.mVisualDirection);
        }
        else
        {
          for (int index = 0; index < 4; ++index)
          {
            if ((double) this.mFadeTimers[index] <= 0.0)
              this.mMoveTimers[index] = 0.0f;
          }
        }
        for (int index = 0; index < this.mFadeTimers.Length; ++index)
        {
          if (num == index)
          {
            this.mFadeTimers[index] += iDeltaTime;
            if ((double) this.mFadeTimers[index] > 0.20000000298023224)
              this.mFadeTimers[index] = 0.2f;
          }
          else
          {
            this.mFadeTimers[index] -= iDeltaTime;
            if ((double) this.mFadeTimers[index] < 0.0)
              this.mFadeTimers[index] = 0.0f;
          }
        }
      }
      spellWheel.FadeTimers(this.mFadeTimers);
      spellWheel.MoveTimers(this.mMoveTimers);
      this.mHelpTime = !this.mHelpActive || (double) this.mHelpTime > 0.20000000298023224 ? MathHelper.Max(this.mHelpTime - iDeltaTime, 0.0f) : MathHelper.Min(this.mHelpTime + iDeltaTime, 0.2f);
      spellWheel.HelpTimer(this.mHelpTime);
    }
  }

  public override void Invert(bool iInvert) => this.mInverted = iInvert;

  public PlayerIndex PlayerIndex => this.mPlayerIndex;

  public XInputController.Binding[] Bindings => this.mBindings;

  protected void RightStickUpdate(Vector2 iValue)
  {
    if (this.mInverted)
      Vector2.Negate(ref iValue, out iValue);
    ControllerDirection iDirection = ControllerDirection.Center;
    if (this.mHelpActive)
      return;
    if ((double) iValue.Length() < 0.30000001192092896)
    {
      this.mInputLocked = false;
      this.mVisualDirection = ControllerDirection.Center;
      this.mCurrentDirection = ControllerDirection.Center;
    }
    else
    {
      if (this.mInputLocked)
        return;
      if ((double) iValue.Length() >= 0.30000001192092896 && (double) iValue.Length() < 0.60000002384185791)
      {
        if ((double) Math.Abs(iValue.X) >= (double) Math.Abs(iValue.Y))
        {
          if ((double) iValue.X >= 0.0)
            this.mVisualDirection = ControllerDirection.Right;
          else
            this.mVisualDirection = ControllerDirection.Left;
        }
        else if ((double) iValue.Y >= 0.0)
          this.mVisualDirection = ControllerDirection.Up;
        else
          this.mVisualDirection = ControllerDirection.Down;
      }
      else
      {
        if (this.mCurrentDirection == ControllerDirection.Center)
        {
          Vector2 vector2 = new Vector2(1f, 0.0f);
          float result;
          Vector2.Distance(ref vector2, ref iValue, out result);
          if ((double) result < 0.5)
            iDirection = ControllerDirection.Right;
          vector2 = new Vector2(-1f, 0.0f);
          Vector2.Distance(ref vector2, ref iValue, out result);
          if ((double) result < 0.5)
            iDirection = ControllerDirection.Left;
          vector2 = new Vector2(0.0f, 1f);
          Vector2.Distance(ref vector2, ref iValue, out result);
          if ((double) result < 0.5)
            iDirection = ControllerDirection.Up;
          vector2 = new Vector2(0.0f, -1f);
          Vector2.Distance(ref vector2, ref iValue, out result);
          if ((double) result < 0.5)
            iDirection = ControllerDirection.Down;
        }
        else
        {
          iDirection = this.mCurrentDirection;
          switch (this.mCurrentDirection)
          {
            case ControllerDirection.Right:
            case ControllerDirection.Left:
              if ((double) Math.Abs(iValue.Y) > 1.0 * (double) Math.Abs(iValue.X))
              {
                iDirection = (double) iValue.Y <= 0.0 ? ControllerDirection.Down : ControllerDirection.Up;
                break;
              }
              break;
            case ControllerDirection.Up:
            case ControllerDirection.Down:
              if ((double) Math.Abs(iValue.X) > 1.0 * (double) Math.Abs(iValue.Y))
              {
                iDirection = (double) iValue.X <= 0.0 ? ControllerDirection.Left : ControllerDirection.Right;
                break;
              }
              break;
          }
        }
        Vector2.Subtract(ref iValue, ref this.mPreviousPosition, out Vector2 _);
        if (iDirection != this.mCurrentDirection && this.mCurrentDirection != ControllerDirection.Center)
        {
          this.mAvatar.HandleCombo(this.mCurrentDirection);
          this.mAvatar.HandleCombo(iDirection);
          this.mInputLocked = true;
        }
        else
        {
          this.mCurrentDirection = iDirection;
          this.mStoredDirection = this.mCurrentDirection;
          this.mPreviousPosition = iValue;
        }
      }
    }
  }

  protected ControllerDirection GetInterpolatedDirection(
    ControllerDirection iLastDirection,
    Vector2 iOldDirection,
    Vector2 iCurrentDirection)
  {
    for (float amount = 0.0f; (double) amount <= 1.0; amount += 0.125f)
    {
      ControllerDirection direction = this.GetDirection(Vector2.Lerp(iOldDirection, iCurrentDirection, amount));
      if (direction != ControllerDirection.Dead && direction != iLastDirection)
        return direction;
    }
    return ControllerDirection.Dead;
  }

  protected ControllerDirection GetInterpolatedDirection(
    Vector2 iOldDirection,
    Vector2 iCurrentDirection)
  {
    for (float amount = 0.0f; (double) amount <= 1.0; amount += 0.125f)
    {
      ControllerDirection direction = this.GetDirection(Vector2.Lerp(iOldDirection, iCurrentDirection, amount));
      if (direction != ControllerDirection.Dead)
        return direction;
    }
    return ControllerDirection.Dead;
  }

  public float HelpTimer
  {
    get => this.mHelpTime;
    set => this.mHelpTime = value;
  }

  public void SetFadeTime(int iIndex, float iValue) => this.mFadeTimers[iIndex] = iValue;

  public void SetMoveTime(int iIndex, float iValue) => this.mMoveTimers[iIndex] = iValue;

  public override void Rumble(float iLeft, float iRight)
  {
    this.mLeftRumble = Math.Max(iLeft, this.mLeftRumble);
    this.mRightRumble = Math.Max(iRight * 1.5f, this.mRightRumble);
  }

  public override float LeftRumble() => this.mLeftRumble;

  public override float RightRumble() => this.mRightRumble;

  public bool IsConnected => this.mOldState.IsConnected;

  public override void Clear()
  {
  }

  private float GetClampedPointerRotation(Vector2 iPosition, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
      case ControllerDirection.Left:
        iPosition.Y *= 0.35f;
        break;
      case ControllerDirection.Up:
      case ControllerDirection.Down:
        iPosition.X *= 0.35f;
        break;
    }
    iPosition.Normalize();
    float num = MagickaMath.Angle(new Vector2(iPosition.X, -iPosition.Y));
    float min = 0.0f;
    float max = 1f;
    switch (iDirection)
    {
      case ControllerDirection.Right:
        min = -0.3926991f;
        max = 0.3926991f;
        break;
      case ControllerDirection.Up:
        min = -1.96349549f;
        max = -3f * (float) Math.PI / 8f;
        break;
      case ControllerDirection.Left:
        if ((double) num < 0.0)
          num += 6.28318548f;
        min = 2.74889374f;
        max = 3.53429174f;
        break;
      case ControllerDirection.Down:
        min = 3f * (float) Math.PI / 8f;
        max = 1.96349549f;
        break;
    }
    return MathHelper.Clamp(num, min, max);
  }

  private float GetBoundValueF(ControllerFunction iFunction, GamePadState iState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case XInputController.Binding.BindingType.Button:
        return !iState.IsButtonDown((Buttons) this.mBindings[(int) iFunction].BindingIndex) ? 0.0f : 1f;
      case XInputController.Binding.BindingType.Trigger:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return iState.Triggers.Left;
          case 1:
            return iState.Triggers.Right;
          default:
            return 0.0f;
        }
      case XInputController.Binding.BindingType.PositiveStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return Math.Max(iState.ThumbSticks.Left.X, 0.0f);
          case 1:
            return Math.Max(iState.ThumbSticks.Left.Y, 0.0f);
          case 2:
            return Math.Max(iState.ThumbSticks.Right.X, 0.0f);
          case 3:
            return Math.Max(iState.ThumbSticks.Right.Y, 0.0f);
          default:
            return 0.0f;
        }
      case XInputController.Binding.BindingType.NegativeStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return Math.Max(-iState.ThumbSticks.Left.X, 0.0f);
          case 1:
            return Math.Max(-iState.ThumbSticks.Left.Y, 0.0f);
          case 2:
            return Math.Max(-iState.ThumbSticks.Right.X, 0.0f);
          case 3:
            return Math.Max(-iState.ThumbSticks.Right.Y, 0.0f);
          default:
            return 0.0f;
        }
      default:
        return 0.0f;
    }
  }

  private bool GetBoundValueB(ControllerFunction iFunction, GamePadState iState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case XInputController.Binding.BindingType.Button:
        return iState.IsButtonDown((Buttons) this.mBindings[(int) iFunction].BindingIndex);
      case XInputController.Binding.BindingType.Trigger:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iState.Triggers.Left >= 0.5;
          case 1:
            return (double) iState.Triggers.Right >= 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.PositiveStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iState.ThumbSticks.Left.X >= 0.5;
          case 1:
            return (double) iState.ThumbSticks.Left.Y >= 0.5;
          case 2:
            return (double) iState.ThumbSticks.Right.X >= 0.5;
          case 3:
            return (double) iState.ThumbSticks.Right.Y >= 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.NegativeStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iState.ThumbSticks.Left.X <= -0.5;
          case 1:
            return (double) iState.ThumbSticks.Left.Y <= -0.5;
          case 2:
            return (double) iState.ThumbSticks.Right.X <= -0.5;
          case 3:
            return (double) iState.ThumbSticks.Right.Y <= -0.5;
          default:
            return false;
        }
      default:
        return false;
    }
  }

  private bool GetBoundValuePressed(
    ControllerFunction iFunction,
    GamePadState iNewState,
    GamePadState iOldState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case XInputController.Binding.BindingType.Button:
        return iNewState.IsButtonDown((Buttons) this.mBindings[(int) iFunction].BindingIndex) & iOldState.IsButtonUp((Buttons) this.mBindings[(int) iFunction].BindingIndex);
      case XInputController.Binding.BindingType.Trigger:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iNewState.Triggers.Left >= 0.5 & (double) iOldState.Triggers.Left < 0.5;
          case 1:
            return (double) iNewState.Triggers.Right >= 0.5 & (double) iOldState.Triggers.Right < 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.PositiveStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iNewState.ThumbSticks.Left.X >= 0.5 & (double) iOldState.ThumbSticks.Left.X < 0.5;
          case 1:
            return (double) iNewState.ThumbSticks.Left.Y >= 0.5 & (double) iOldState.ThumbSticks.Left.Y < 0.5;
          case 2:
            return (double) iNewState.ThumbSticks.Right.X >= 0.5 & (double) iOldState.ThumbSticks.Right.X < 0.5;
          case 3:
            return (double) iNewState.ThumbSticks.Right.Y >= 0.5 & (double) iOldState.ThumbSticks.Right.Y < 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.NegativeStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iNewState.ThumbSticks.Left.X <= -0.5 & (double) iOldState.ThumbSticks.Left.X > -0.5;
          case 1:
            return (double) iNewState.ThumbSticks.Left.Y <= -0.5 & (double) iOldState.ThumbSticks.Left.Y > -0.5;
          case 2:
            return (double) iNewState.ThumbSticks.Right.X <= -0.5 & (double) iOldState.ThumbSticks.Right.X > -0.5;
          case 3:
            return (double) iNewState.ThumbSticks.Right.Y <= -0.5 & (double) iOldState.ThumbSticks.Right.Y > -0.5;
          default:
            return false;
        }
      default:
        return false;
    }
  }

  private bool GetBoundValueReleased(
    ControllerFunction iFunction,
    GamePadState iNewState,
    GamePadState iOldState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case XInputController.Binding.BindingType.Button:
        return iOldState.IsButtonDown((Buttons) this.mBindings[(int) iFunction].BindingIndex) & iNewState.IsButtonUp((Buttons) this.mBindings[(int) iFunction].BindingIndex);
      case XInputController.Binding.BindingType.Trigger:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iOldState.Triggers.Left >= 0.5 & (double) iNewState.Triggers.Left < 0.5;
          case 1:
            return (double) iOldState.Triggers.Right >= 0.5 & (double) iNewState.Triggers.Right < 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.PositiveStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iOldState.ThumbSticks.Left.X >= 0.5 & (double) iNewState.ThumbSticks.Left.X < 0.5;
          case 1:
            return (double) iOldState.ThumbSticks.Left.Y >= 0.5 & (double) iNewState.ThumbSticks.Left.Y < 0.5;
          case 2:
            return (double) iOldState.ThumbSticks.Right.X >= 0.5 & (double) iNewState.ThumbSticks.Right.X < 0.5;
          case 3:
            return (double) iOldState.ThumbSticks.Right.Y >= 0.5 & (double) iNewState.ThumbSticks.Right.Y < 0.5;
          default:
            return false;
        }
      case XInputController.Binding.BindingType.NegativeStick:
        switch (this.mBindings[(int) iFunction].BindingIndex)
        {
          case 0:
            return (double) iOldState.ThumbSticks.Left.X <= -0.5 & (double) iNewState.ThumbSticks.Left.X > -0.5;
          case 1:
            return (double) iOldState.ThumbSticks.Left.Y <= -0.5 & (double) iNewState.ThumbSticks.Left.Y > -0.5;
          case 2:
            return (double) iOldState.ThumbSticks.Right.X <= -0.5 & (double) iNewState.ThumbSticks.Right.X > -0.5;
          case 3:
            return (double) iOldState.ThumbSticks.Right.Y <= -0.5 & (double) iNewState.ThumbSticks.Right.Y > -0.5;
          default:
            return false;
        }
      default:
        return false;
    }
  }

  public struct Binding(XInputController.Binding.BindingType iType, int iIndex)
  {
    public XInputController.Binding.BindingType Type = iType;
    public int BindingIndex = iIndex;

    public enum BindingType
    {
      None,
      Button,
      Trigger,
      PositiveStick,
      NegativeStick,
    }
  }
}
