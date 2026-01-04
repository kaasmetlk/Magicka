// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Controls.DirectInputController
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls.DirectInput;
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
using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.Controls;

internal class DirectInputController : Controller
{
  public static readonly string COLDKEY = new string('̠', 1) + new string('̣', 1);
  public static readonly string SELFKEY = new string('̧', 1);
  public static readonly string WEAPONKEY = new string('̤', 1);
  private Device mDevice;
  private DirectInputController.State mOldState;
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
  private DirectInputController.Binding[] mBindings;
  private bool mConfigured;
  private bool mAcquire;

  public event Action<DirectInputController.Binding> OnChange;

  public DirectInputController(Guid iGamepadInstanceGuid)
  {
    this.mConfigured = GlobalSettings.Instance.DInputBindings.TryGetValue(iGamepadInstanceGuid, out this.mBindings);
    if (!this.mConfigured)
    {
      this.mBindings = new DirectInputController.Binding[24];
      GlobalSettings.Instance.DInputBindings.Add(iGamepadInstanceGuid, this.mBindings);
    }
    this.mDevice = new Device(iGamepadInstanceGuid);
    this.mDevice.SetDataFormat(DeviceDataFormat.Joystick);
    this.mDevice.Acquire();
    this.mHelpActive = false;
    this.mFadeTimers = new float[4];
    this.mMoveTimers = new float[4];
  }

  public override void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (!this.mDevice.Caps.Attatched)
    {
      this.mOldState = new DirectInputController.State();
      this.mAcquire = false;
    }
    else if (!this.mAcquire)
    {
      this.mDevice.Acquire();
      this.mAcquire = true;
      DirectInputController.State.FromDevice(this.mDevice, out this.mOldState);
    }
    else
    {
      DialogManager instance1 = DialogManager.Instance;
      Tome instance2 = Tome.Instance;
      GameState currentState = GameStateManager.Instance.CurrentState;
      TutorialManager instance3 = TutorialManager.Instance;
      PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
      this.mAvatar = this.Player == null ? (Avatar) null : this.Player.Avatar;
      this.mDevice.Poll();
      DirectInputController.State oState;
      DirectInputController.State.FromDevice(this.mDevice, out oState);
      if (this.OnChange != null)
      {
        DirectInputController.Binding binding = new DirectInputController.Binding();
        for (int index = 0; index < oState.Buttons.NrOfButtons; ++index)
        {
          if (oState.Buttons[index] & !this.mOldState.Buttons[index])
          {
            binding.Type = DirectInputController.Binding.BindingType.Button;
            binding.BindingIndex = index;
            break;
          }
        }
        if (binding.Type == DirectInputController.Binding.BindingType.None)
        {
          for (int iIndex = 0; iIndex < 4; ++iIndex)
          {
            if (oState.DPad[iIndex] & !this.mOldState.DPad[iIndex])
            {
              binding.Type = DirectInputController.Binding.BindingType.POV;
              binding.BindingIndex = iIndex;
              break;
            }
          }
        }
        if (binding.Type == DirectInputController.Binding.BindingType.None)
        {
          for (int index = 0; index < 6; ++index)
          {
            if ((double) oState.Axes[index] >= 0.5 & (double) this.mOldState.Axes[index] < 0.5)
            {
              binding.Type = DirectInputController.Binding.BindingType.PositiveAxis;
              binding.BindingIndex = index;
              break;
            }
            if ((double) oState.Axes[index] <= -0.5 & (double) this.mOldState.Axes[index] > -0.5)
            {
              binding.Type = DirectInputController.Binding.BindingType.NegativeAxis;
              binding.BindingIndex = index;
              break;
            }
          }
        }
        if (binding.Type != DirectInputController.Binding.BindingType.None)
        {
          Action<DirectInputController.Binding> onChange = this.OnChange;
          this.OnChange = (Action<DirectInputController.Binding>) null;
          onChange(binding);
          this.mOldState = oState;
          this.mMenuMoveCooldown = float.PositiveInfinity;
          return;
        }
      }
      ControllerDirection controllerDirection = ControllerDirection.Center;
      if (this.GetBoundValueB(ControllerFunction.Move_Right, oState))
        controllerDirection |= ControllerDirection.Right;
      if (this.GetBoundValueB(ControllerFunction.Move_Up, oState))
        controllerDirection |= ControllerDirection.Up;
      if (this.GetBoundValueB(ControllerFunction.Move_Left, oState))
        controllerDirection |= ControllerDirection.Left;
      if (this.GetBoundValueB(ControllerFunction.Move_Down, oState))
        controllerDirection |= ControllerDirection.Down;
      this.mLeftRumble -= iDeltaTime * 3f;
      this.mRightRumble -= iDeltaTime * 3f;
      if (currentState is CompanyState & this.GetBoundValuePressed(ControllerFunction.Menu_Select, oState, this.mOldState))
        (currentState as CompanyState).SkipScreen();
      else if (persistentState.IsolateControls())
      {
        this.mMenuMoveCooldown -= iDeltaTime;
        if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, oState, this.mOldState))
          persistentState.ControllerA((Controller) this);
        if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, oState, this.mOldState))
          persistentState.ControllerB((Controller) this);
        if (this.GetBoundValuePressed(ControllerFunction.Interact, oState, this.mOldState))
          persistentState.ControllerX((Controller) this);
        if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, oState, this.mOldState))
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
        if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, oState, this.mOldState))
          instance1.ControllerSelect((Controller) this);
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
            if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, oState, this.mOldState))
              instance2.ControllerA((Controller) this);
            if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, oState, this.mOldState))
              instance2.ControllerB((Controller) this);
            if (this.GetBoundValuePressed(ControllerFunction.Interact, oState, this.mOldState))
              instance2.ControllerX((Controller) this);
            if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, oState, this.mOldState))
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
                if (this.GetBoundValuePressed(ControllerFunction.Pause, oState, this.mOldState))
                  playState.TogglePause((Controller) this);
                if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, oState, this.mOldState))
                  InGameMenu.ControllerSelect((Controller) this);
                if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, oState, this.mOldState))
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
                if (this.GetBoundValuePressed(ControllerFunction.Pause, oState, this.mOldState))
                  playState.TogglePause((Controller) this);
                else if (this.GetBoundValuePressed(ControllerFunction.Attack, oState, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Boost, oState, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Interact, oState, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Special, oState, this.mOldState))
                {
                  if (playState.IsInCutscene)
                    playState.SkipCutscene();
                  instance3.RemoveDialogHint();
                }
                if (this.GetBoundValuePressed(ControllerFunction.Interact, oState, this.mOldState) && instance1.CanAdvance((Controller) this) && !playState.IsInCutscene)
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
                  if (this.GetBoundValuePressed(ControllerFunction.Attack, oState, this.mOldState) | this.GetBoundValueB(ControllerFunction.Attack, oState) & (this.mAvatar.WieldingGun | this.mAvatar.Equipment[0].Item.SpellCharged))
                    this.mAvatar.Attack();
                  else if (this.GetBoundValueReleased(ControllerFunction.Attack, oState, this.mOldState))
                    this.mAvatar.AttackRelease();
                  if (this.GetBoundValuePressed(ControllerFunction.Boost, oState, this.mOldState))
                    this.mAvatar.Boost();
                  if (this.GetBoundValuePressed(ControllerFunction.Interact, oState, this.mOldState))
                    this.mAvatar.Action();
                  if (this.GetBoundValuePressed(ControllerFunction.Special, oState, this.mOldState))
                    this.mAvatar.Special();
                  else if (this.GetBoundValueReleased(ControllerFunction.Special, oState, this.mOldState))
                    this.mAvatar.SpecialRelease();
                  if (this.GetBoundValuePressed(ControllerFunction.Inventory, oState, this.mOldState))
                    this.mAvatar.CheckInventory();
                  this.mHelpActive = this.GetBoundValueB(ControllerFunction.Spell_Wheel, oState);
                  this.mAvatar.IsBlocking = this.GetBoundValueB(ControllerFunction.Block, oState);
                  Vector2 iValue = new Vector2();
                  iValue.X += this.GetBoundValueF(ControllerFunction.Spell_Right, oState);
                  iValue.X -= this.GetBoundValueF(ControllerFunction.Spell_Left, oState);
                  iValue.Y += this.GetBoundValueF(ControllerFunction.Spell_Up, oState);
                  iValue.Y -= this.GetBoundValueF(ControllerFunction.Spell_Down, oState);
                  if (this.mAvatar.Polymorphed)
                    this.RightStickUpdate(new Vector2());
                  else
                    this.RightStickUpdate(iValue);
                  Vector2 result = new Vector2();
                  result.X += this.GetBoundValueF(ControllerFunction.Move_Right, oState);
                  result.X -= this.GetBoundValueF(ControllerFunction.Move_Left, oState);
                  result.Y += this.GetBoundValueF(ControllerFunction.Move_Up, oState);
                  result.Y -= this.GetBoundValueF(ControllerFunction.Move_Down, oState);
                  float divider = result.Length();
                  if ((double) divider > 1.0)
                    Vector2.Divide(ref result, divider, out result);
                  else if ((double) divider < 0.25)
                    result = new Vector2();
                  this.mAvatar.UpdatePadDirection(result, this.mInverted);
                  if (this.GetBoundValueB(ControllerFunction.Cast_Area, oState))
                    this.mAvatar.AreaPressed();
                  else
                    this.mAvatar.AreaReleased();
                  if (this.GetBoundValueB(ControllerFunction.Cast_Force, oState))
                    this.mAvatar.ForcePressed();
                  else
                    this.mAvatar.ForceReleased();
                  if (this.GetBoundValuePressed(ControllerFunction.Magick_Next, oState, this.mOldState))
                  {
                    MagickType iMagick = this.Player.IconRenderer.TomeMagick + 1;
                    while (iMagick != MagickType.NrOfMagicks && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, iMagick))
                      ++iMagick;
                    if (iMagick == MagickType.NrOfMagicks)
                      iMagick = MagickType.None;
                    this.Player.IconRenderer.TomeMagick = iMagick;
                    break;
                  }
                  if (this.GetBoundValuePressed(ControllerFunction.Magick_Prev, oState, this.mOldState))
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
      this.mOldState = oState;
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
  }

  public override void Invert(bool iInvert) => this.mInverted = iInvert;

  public DirectInputController.Binding[] Bindings => this.mBindings;

  public bool IsConnected => this.mDevice.Caps.Attatched;

  public bool Configured
  {
    get => this.mConfigured;
    set => this.mConfigured |= value;
  }

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

  public Device Device => this.mDevice;

  public override void Rumble(float iLeft, float iRight)
  {
    this.mLeftRumble = Math.Max(iLeft, this.mLeftRumble);
    this.mRightRumble = Math.Max(iRight * 1.5f, this.mRightRumble);
  }

  public override float LeftRumble() => this.mLeftRumble;

  public override float RightRumble() => this.mRightRumble;

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

  private float GetBoundValueF(ControllerFunction iFunction, DirectInputController.State iState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case DirectInputController.Binding.BindingType.Button:
        return !iState.Buttons[this.mBindings[(int) iFunction].BindingIndex] ? 0.0f : 1f;
      case DirectInputController.Binding.BindingType.POV:
        return !iState.DPad[this.mBindings[(int) iFunction].BindingIndex] ? 0.0f : 1f;
      case DirectInputController.Binding.BindingType.PositiveAxis:
        return Math.Max(iState.Axes[this.mBindings[(int) iFunction].BindingIndex], 0.0f);
      case DirectInputController.Binding.BindingType.NegativeAxis:
        return Math.Max(-iState.Axes[this.mBindings[(int) iFunction].BindingIndex], 0.0f);
      default:
        return 0.0f;
    }
  }

  private bool GetBoundValueB(ControllerFunction iFunction, DirectInputController.State iState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case DirectInputController.Binding.BindingType.Button:
        return iState.Buttons[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.POV:
        return iState.DPad[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.PositiveAxis:
        return (double) iState.Axes[this.mBindings[(int) iFunction].BindingIndex] >= 0.5;
      case DirectInputController.Binding.BindingType.NegativeAxis:
        return (double) iState.Axes[this.mBindings[(int) iFunction].BindingIndex] <= -0.5;
      default:
        return false;
    }
  }

  private bool GetBoundValuePressed(
    ControllerFunction iFunction,
    DirectInputController.State iNewState,
    DirectInputController.State iOldState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case DirectInputController.Binding.BindingType.Button:
        return iNewState.Buttons[this.mBindings[(int) iFunction].BindingIndex] & !iOldState.Buttons[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.POV:
        return iNewState.DPad[this.mBindings[(int) iFunction].BindingIndex] & !iOldState.DPad[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.PositiveAxis:
        return (double) iNewState.Axes[this.mBindings[(int) iFunction].BindingIndex] >= 0.5 & (double) iOldState.Axes[this.mBindings[(int) iFunction].BindingIndex] < 0.5;
      case DirectInputController.Binding.BindingType.NegativeAxis:
        return (double) iNewState.Axes[this.mBindings[(int) iFunction].BindingIndex] <= -0.5 & (double) iOldState.Axes[this.mBindings[(int) iFunction].BindingIndex] > -0.5;
      default:
        return false;
    }
  }

  private bool GetBoundValueReleased(
    ControllerFunction iFunction,
    DirectInputController.State iNewState,
    DirectInputController.State iOldState)
  {
    switch (this.mBindings[(int) iFunction].Type)
    {
      case DirectInputController.Binding.BindingType.Button:
        return iOldState.Buttons[this.mBindings[(int) iFunction].BindingIndex] & !iNewState.Buttons[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.POV:
        return iOldState.DPad[this.mBindings[(int) iFunction].BindingIndex] & !iNewState.DPad[this.mBindings[(int) iFunction].BindingIndex];
      case DirectInputController.Binding.BindingType.PositiveAxis:
        return (double) iOldState.Axes[this.mBindings[(int) iFunction].BindingIndex] >= 0.5 & (double) iNewState.Axes[this.mBindings[(int) iFunction].BindingIndex] < 0.5;
      case DirectInputController.Binding.BindingType.NegativeAxis:
        return (double) iOldState.Axes[this.mBindings[(int) iFunction].BindingIndex] <= -0.5 & (double) iNewState.Axes[this.mBindings[(int) iFunction].BindingIndex] > -0.5;
      default:
        return false;
    }
  }

  private struct State
  {
    public DirectInputAxes Axes;
    public DirectInputDPad DPad;
    public DirectInputButtons Buttons;

    public static void FromDevice(Device iDevice, out DirectInputController.State oState)
    {
      JoystickState iState = iDevice.CurrentJoystickState;
      oState.Axes = new DirectInputAxes(iDevice, iState);
      oState.DPad = new DirectInputDPad(iState.GetPointOfView()[0]);
      oState.Buttons = new DirectInputButtons(iDevice, iState);
    }
  }

  public struct Binding(DirectInputController.Binding.BindingType iType, int iIndex)
  {
    public DirectInputController.Binding.BindingType Type = iType;
    public int BindingIndex = iIndex;

    public enum BindingType
    {
      None,
      Button,
      POV,
      PositiveAxis,
      NegativeAxis,
    }
  }
}
