using System;
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

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000590 RID: 1424
	internal class XInputController : Controller
	{
		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06002A81 RID: 10881 RVA: 0x0014DC01 File Offset: 0x0014BE01
		// (remove) Token: 0x06002A82 RID: 10882 RVA: 0x0014DC1A File Offset: 0x0014BE1A
		public event Action<XInputController.Binding> OnChange;

		// Token: 0x06002A83 RID: 10883 RVA: 0x0014DC34 File Offset: 0x0014BE34
		static XInputController()
		{
			XInputController.BUTTON_NAMES = new int[32];
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(1U)] = "#xb_dpad_up".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(2U)] = "#xb_dpad_down".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4U)] = "#xb_dpad_left".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8U)] = "#xb_dpad_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16U)] = "#xb_start".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(32U)] = "#xb_back".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(64U)] = "#xb_thumb_left".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(128U)] = "#xb_thumb_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(256U)] = "#xb_bumper_left".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(512U)] = "#xb_bumper_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4096U)] = "#xb_a".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8192U)] = "#xb_b".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16384U)] = "#xb_x".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(32768U)] = "#xb_y".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(4194304U)] = "#xb_trigger_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(8388608U)] = "#xb_trigger_left".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(16777216U)] = "#xb_thumb_right_up".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(33554432U)] = "#xb_thumb_right_down".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(67108864U)] = "#xb_thumb_right_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(134217728U)] = "#xb_thumb_right_left".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(268435456U)] = "#xb_thumb_left_up".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(536870912U)] = "#xb_thumb_left_down".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(1073741824U)] = "#xb_thumb_left_right".GetHashCodeCustom();
			XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits(2097152U)] = "#xb_thumb_left_left".GetHashCodeCustom();
		}

		// Token: 0x06002A84 RID: 10884 RVA: 0x0014DEE4 File Offset: 0x0014C0E4
		public static string GetButtonName(Buttons iButton)
		{
			return LanguageManager.Instance.GetString(XInputController.BUTTON_NAMES[MagickaMath.CountTrailingZeroBits((uint)iButton)]);
		}

		// Token: 0x06002A85 RID: 10885 RVA: 0x0014DEFC File Offset: 0x0014C0FC
		public XInputController(PlayerIndex iPlayerIndex)
		{
			this.mBindings = GlobalSettings.Instance.XInputBindings[(int)iPlayerIndex];
			if (this.mBindings == null)
			{
				this.mBindings = new XInputController.Binding[24];
				GlobalSettings.Instance.XInputBindings[(int)iPlayerIndex] = this.mBindings;
				this.LoadDefaults();
			}
			this.mPlayerIndex = iPlayerIndex;
			this.mHelpActive = false;
			this.mFadeTimers = new float[4];
			this.mMoveTimers = new float[4];
		}

		// Token: 0x06002A86 RID: 10886 RVA: 0x0014DF74 File Offset: 0x0014C174
		public void LoadDefaults()
		{
			this.mBindings[0] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 0);
			this.mBindings[1] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 1);
			this.mBindings[2] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 0);
			this.mBindings[3] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 1);
			this.mBindings[4] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 4096);
			this.mBindings[5] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192);
			this.mBindings[6] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 2);
			this.mBindings[7] = new XInputController.Binding(XInputController.Binding.BindingType.PositiveStick, 3);
			this.mBindings[8] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 2);
			this.mBindings[9] = new XInputController.Binding(XInputController.Binding.BindingType.NegativeStick, 3);
			this.mBindings[10] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 2);
			this.mBindings[11] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 1);
			this.mBindings[12] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 4096);
			this.mBindings[13] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32768);
			this.mBindings[14] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 512);
			this.mBindings[15] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 16384);
			this.mBindings[16] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192);
			this.mBindings[17] = new XInputController.Binding(XInputController.Binding.BindingType.Trigger, 1);
			this.mBindings[18] = new XInputController.Binding(XInputController.Binding.BindingType.Trigger, 0);
			this.mBindings[19] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32768);
			this.mBindings[20] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 8192);
			this.mBindings[21] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 16);
			this.mBindings[22] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 32);
			this.mBindings[23] = new XInputController.Binding(XInputController.Binding.BindingType.Button, 128);
		}

		// Token: 0x06002A87 RID: 10887 RVA: 0x0014E1FC File Offset: 0x0014C3FC
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			DialogManager instance = DialogManager.Instance;
			Tome instance2 = Tome.Instance;
			GameState currentState = GameStateManager.Instance.CurrentState;
			PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
			TutorialManager instance3 = TutorialManager.Instance;
			this.mAvatar = ((base.Player == null) ? null : base.Player.Avatar);
			GamePadState state;
			try
			{
				state = GamePad.GetState(this.mPlayerIndex);
			}
			catch (InvalidOperationException)
			{
				this.mOldState = default(GamePadState);
				return;
			}
			if (this.OnChange != null)
			{
				XInputController.Binding obj = default(XInputController.Binding);
				for (int i = 0; i < 16; i++)
				{
					Buttons buttons = (Buttons)(1 << i);
					if (state.IsButtonDown(buttons) & this.mOldState.IsButtonUp(buttons))
					{
						obj.Type = XInputController.Binding.BindingType.Button;
						obj.BindingIndex = (int)buttons;
						break;
					}
				}
				if (obj.Type == XInputController.Binding.BindingType.None)
				{
					if (state.Triggers.Left >= 0.5f & this.mOldState.Triggers.Left < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.Trigger;
						obj.BindingIndex = 0;
					}
					else if (state.Triggers.Right >= 0.5f & this.mOldState.Triggers.Right < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.Trigger;
						obj.BindingIndex = 1;
					}
				}
				if (obj.Type == XInputController.Binding.BindingType.None)
				{
					if (state.ThumbSticks.Left.X >= 0.5f & this.mOldState.ThumbSticks.Left.X < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.PositiveStick;
						obj.BindingIndex = 0;
					}
					else if (state.ThumbSticks.Left.Y >= 0.5f & this.mOldState.ThumbSticks.Left.Y < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.PositiveStick;
						obj.BindingIndex = 1;
					}
					else if (state.ThumbSticks.Right.X >= 0.5f & this.mOldState.ThumbSticks.Right.X < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.PositiveStick;
						obj.BindingIndex = 2;
					}
					else if (state.ThumbSticks.Right.Y >= 0.5f & this.mOldState.ThumbSticks.Right.Y < 0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.PositiveStick;
						obj.BindingIndex = 3;
					}
					else if (state.ThumbSticks.Left.X <= -0.5f & this.mOldState.ThumbSticks.Left.X > -0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.NegativeStick;
						obj.BindingIndex = 0;
					}
					else if (state.ThumbSticks.Left.Y <= -0.5f & this.mOldState.ThumbSticks.Left.Y > -0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.NegativeStick;
						obj.BindingIndex = 1;
					}
					else if (state.ThumbSticks.Right.X <= -0.5f & this.mOldState.ThumbSticks.Right.X > -0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.NegativeStick;
						obj.BindingIndex = 2;
					}
					else if (state.ThumbSticks.Right.Y <= -0.5f & this.mOldState.ThumbSticks.Right.Y > -0.5f)
					{
						obj.Type = XInputController.Binding.BindingType.NegativeStick;
						obj.BindingIndex = 3;
					}
				}
				if (obj.Type != XInputController.Binding.BindingType.None)
				{
					this.OnChange(obj);
					this.OnChange = null;
					this.mOldState = state;
					this.mMenuMoveCooldown = float.PositiveInfinity;
					return;
				}
			}
			ControllerDirection controllerDirection = ControllerDirection.Center;
			if (this.GetBoundValueB(ControllerFunction.Move_Right, state))
			{
				controllerDirection |= ControllerDirection.Right;
			}
			if (this.GetBoundValueB(ControllerFunction.Move_Up, state))
			{
				controllerDirection |= ControllerDirection.Up;
			}
			if (this.GetBoundValueB(ControllerFunction.Move_Left, state))
			{
				controllerDirection |= ControllerDirection.Left;
			}
			if (this.GetBoundValueB(ControllerFunction.Move_Down, state))
			{
				controllerDirection |= ControllerDirection.Down;
			}
			GamePad.SetVibration(this.mPlayerIndex, MathHelper.Clamp(this.mLeftRumble, 0f, 1f), MathHelper.Clamp(this.mRightRumble, 0f, 1f));
			this.mLeftRumble -= iDeltaTime * 3f;
			this.mRightRumble -= iDeltaTime * 3f;
			if (currentState is CompanyState & this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
			{
				(currentState as CompanyState).SkipScreen();
			}
			else if (persistentState.IsolateControls())
			{
				this.mMenuMoveCooldown -= iDeltaTime;
				if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
				{
					persistentState.ControllerA(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
				{
					persistentState.ControllerB(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
				{
					persistentState.ControllerX(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, state, this.mOldState))
				{
					persistentState.ControllerY(this);
				}
				if (controllerDirection == ControllerDirection.Center)
				{
					this.mMenuMoveCooldown = 0.25f;
				}
				else if (controllerDirection != this.mOldNavDirection || this.mMenuMoveCooldown <= 0f)
				{
					persistentState.ControllerMovement(this, controllerDirection);
					this.mMenuMoveCooldown += 0.25f;
				}
			}
			else if (instance.MessageBoxActive)
			{
				this.mMenuMoveCooldown -= iDeltaTime;
				if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
				{
					instance.ControllerSelect(this);
				}
				if (state.Buttons.B == ButtonState.Pressed)
				{
					instance.ControllerEsc(this);
				}
				if (controllerDirection == ControllerDirection.Center)
				{
					this.mMenuMoveCooldown = 0.25f;
				}
				else if (controllerDirection != this.mOldNavDirection || this.mMenuMoveCooldown <= 0f)
				{
					instance.ControllerMove(this, controllerDirection);
					this.mMenuMoveCooldown += 0.25f;
				}
			}
			else if (currentState is MenuState)
			{
				this.mMenuMoveCooldown -= iDeltaTime;
				if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
				{
					instance2.ControllerA(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
				{
					instance2.ControllerB(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
				{
					instance2.ControllerX(this);
				}
				if (this.GetBoundValuePressed(ControllerFunction.Cast_Self, state, this.mOldState))
				{
					instance2.ControllerY(this);
				}
				if (controllerDirection == ControllerDirection.Center)
				{
					this.mMenuMoveCooldown = 0.25f;
				}
				else if (controllerDirection != this.mOldNavDirection || this.mMenuMoveCooldown <= 0f)
				{
					instance2.ControllerMovement(this, controllerDirection);
					this.mMenuMoveCooldown += 0.25f;
				}
			}
			else if (currentState is PlayState && base.Player != null && base.Player.Playing)
			{
				PlayState playState = currentState as PlayState;
				if (playState.IsPaused)
				{
					this.mMenuMoveCooldown -= iDeltaTime;
					if (this.GetBoundValuePressed(ControllerFunction.Pause, state, this.mOldState))
					{
						playState.TogglePause(this);
					}
					if (this.GetBoundValuePressed(ControllerFunction.Menu_Select, state, this.mOldState))
					{
						InGameMenu.ControllerSelect(this);
					}
					if (this.GetBoundValuePressed(ControllerFunction.Menu_Back, state, this.mOldState))
					{
						InGameMenu.ControllerBack(this);
					}
					if (controllerDirection == ControllerDirection.Center)
					{
						this.mMenuMoveCooldown = 0.25f;
					}
					else if (controllerDirection != this.mOldNavDirection || this.mMenuMoveCooldown <= 0f)
					{
						InGameMenu.ControllerMove(this, controllerDirection);
						this.mMenuMoveCooldown += 0.25f;
					}
				}
				else
				{
					if (this.GetBoundValuePressed(ControllerFunction.Pause, state, this.mOldState))
					{
						playState.TogglePause(this);
					}
					else if (this.GetBoundValuePressed(ControllerFunction.Attack, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Boost, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState) | this.GetBoundValuePressed(ControllerFunction.Special, state, this.mOldState))
					{
						if (playState.IsInCutscene)
						{
							playState.SkipCutscene();
						}
						instance3.RemoveDialogHint();
					}
					if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState) && instance.CanAdvance(this) && !playState.IsInCutscene)
					{
						instance.Advance(this);
					}
				}
				if (this.mAvatar != null && !this.mAvatar.Dead && (currentState as PlayState).Level.CurrentScene != null)
				{
					this.mAvatar.Player.NotifierButton.Hide();
					if (!DialogManager.Instance.AwaitingInput && !DialogManager.Instance.HoldoffInput)
					{
						if (this.mAvatar.NotifySpecialAbility)
						{
							this.mAvatar.Player.NotifierButton.Show(this.mAvatar.SpecialAbilityName, ButtonChar.Y, this.mAvatar);
						}
						else if (this.mAvatar.ChantingMagick)
						{
							string @string = LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int)this.mAvatar.MagickType]);
							this.mAvatar.Player.NotifierButton.Show(@string, ButtonChar.B, this.mAvatar);
						}
						else if (!this.mAvatar.Chanting)
						{
							if (this.mAvatar.IsGripped)
							{
								string string2 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[12]);
								this.mAvatar.Player.NotifierButton.Show(string2, ButtonChar.B, this.mAvatar);
								if (this.mAvatar.Boosts > 0)
								{
									this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
								}
							}
							else if (!this.mAvatar.Polymorphed)
							{
								Pickable pickable = this.mAvatar.FindPickUp(true);
								if (pickable != null)
								{
									string string3 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[1]);
									if (pickable is Item && ((pickable as Item).WeaponClass != WeaponClass.Staff || !this.mAvatar.Equipment[1].Item.IsBound) && (!Defines.IsWeapon((pickable as Item).WeaponClass) || !this.mAvatar.Equipment[0].Item.IsBound))
									{
										this.mAvatar.Player.NotifierButton.Show(string3, ButtonChar.X, this.mAvatar);
										pickable.Highlight();
									}
									else if (!(pickable is Item))
									{
										this.mAvatar.Player.NotifierButton.Show(string3, ButtonChar.X, this.mAvatar);
										pickable.Highlight();
									}
								}
								else
								{
									Interactable interactable = this.mAvatar.FindInteractable(true);
									if (interactable != null && (!AudioManager.Instance.Threat || interactable.InteractType != InteractType.Talk))
									{
										string string4 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int)interactable.InteractType]);
										this.mAvatar.Player.NotifierButton.Show(string4, ButtonChar.X, this.mAvatar);
										interactable.Highlight();
									}
									else if (BoostState.Instance.ShieldToBoost(this.mAvatar) != null || (this.mAvatar.IsSelfShielded && !this.mAvatar.IsSolidSelfShielded))
									{
										string string5 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[6]);
										this.mAvatar.Player.NotifierButton.Show(string5, ButtonChar.B, this.mAvatar);
										if (this.mAvatar.Boosts > 0)
										{
											this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
										}
									}
									else
									{
										Magicka.GameLogic.Entities.Character character = this.mAvatar.FindCharacter(true);
										if (character != null && character.InteractText != InteractType.None && !AudioManager.Instance.Threat)
										{
											string string6 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int)character.InteractText]);
											this.mAvatar.Player.NotifierButton.Show(string6, ButtonChar.X, this.mAvatar);
											character.Highlight();
										}
									}
								}
							}
						}
					}
					if (KeyboardHUD.Instance.UIEnabled)
					{
						this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
					}
					if (!ControlManager.Instance.IsInputLimited && !ControlManager.Instance.IsPlayerInputLocked(this.mAvatar.Player.ID))
					{
						if (this.GetBoundValuePressed(ControllerFunction.Attack, state, this.mOldState) | (this.GetBoundValueB(ControllerFunction.Attack, state) & (this.mAvatar.WieldingGun | this.mAvatar.Equipment[0].Item.SpellCharged)))
						{
							this.mAvatar.Attack();
						}
						else if (this.GetBoundValueReleased(ControllerFunction.Attack, state, this.mOldState))
						{
							this.mAvatar.AttackRelease();
						}
						if (this.GetBoundValuePressed(ControllerFunction.Boost, state, this.mOldState))
						{
							this.mAvatar.Boost();
						}
						if (this.GetBoundValuePressed(ControllerFunction.Interact, state, this.mOldState))
						{
							this.mAvatar.Action();
						}
						if (this.GetBoundValuePressed(ControllerFunction.Special, state, this.mOldState))
						{
							this.mAvatar.Special();
						}
						else if (this.GetBoundValueReleased(ControllerFunction.Special, state, this.mOldState))
						{
							this.mAvatar.SpecialRelease();
						}
						if (this.GetBoundValuePressed(ControllerFunction.Inventory, state, this.mOldState))
						{
							this.mAvatar.CheckInventory();
						}
						if (this.GetBoundValueB(ControllerFunction.Spell_Wheel, state))
						{
							this.mHelpActive = true;
						}
						else
						{
							this.mHelpActive = false;
						}
						this.mAvatar.IsBlocking = this.GetBoundValueB(ControllerFunction.Block, state);
						Vector2 iValue = default(Vector2);
						iValue.X += this.GetBoundValueF(ControllerFunction.Spell_Right, state);
						iValue.X -= this.GetBoundValueF(ControllerFunction.Spell_Left, state);
						iValue.Y += this.GetBoundValueF(ControllerFunction.Spell_Up, state);
						iValue.Y -= this.GetBoundValueF(ControllerFunction.Spell_Down, state);
						if (this.mAvatar.Polymorphed)
						{
							this.RightStickUpdate(default(Vector2));
						}
						else
						{
							this.RightStickUpdate(iValue);
						}
						Vector2 iValue2 = default(Vector2);
						iValue2.X += this.GetBoundValueF(ControllerFunction.Move_Right, state);
						iValue2.X -= this.GetBoundValueF(ControllerFunction.Move_Left, state);
						iValue2.Y += this.GetBoundValueF(ControllerFunction.Move_Up, state);
						iValue2.Y -= this.GetBoundValueF(ControllerFunction.Move_Down, state);
						float num = iValue2.Length();
						if (num > 1f)
						{
							Vector2.Divide(ref iValue2, num, out iValue2);
						}
						else if (num < 0.25f)
						{
							iValue2 = default(Vector2);
						}
						this.mAvatar.UpdatePadDirection(iValue2, this.mInverted);
						if (this.GetBoundValueB(ControllerFunction.Cast_Area, state))
						{
							this.mAvatar.AreaPressed();
						}
						else
						{
							this.mAvatar.AreaReleased();
						}
						if (this.GetBoundValueB(ControllerFunction.Cast_Force, state))
						{
							this.mAvatar.ForcePressed();
						}
						else
						{
							this.mAvatar.ForceReleased();
						}
						if (this.GetBoundValuePressed(ControllerFunction.Magick_Next, state, this.mOldState))
						{
							MagickType magickType = base.Player.IconRenderer.TomeMagick + 1;
							while (magickType != MagickType.NrOfMagicks && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, magickType))
							{
								magickType++;
							}
							if (magickType == MagickType.NrOfMagicks)
							{
								magickType = MagickType.None;
							}
							base.Player.IconRenderer.TomeMagick = magickType;
						}
						else if (this.GetBoundValuePressed(ControllerFunction.Magick_Prev, state, this.mOldState))
						{
							MagickType magickType2 = base.Player.IconRenderer.TomeMagick - 1;
							if (magickType2 == MagickType.NrOfMagicks | magickType2 < MagickType.None)
							{
								magickType2 = MagickType.Amalgameddon;
							}
							while (magickType2 != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, magickType2))
							{
								magickType2--;
							}
							base.Player.IconRenderer.TomeMagick = magickType2;
						}
					}
					else
					{
						this.mAvatar.UpdatePadDirection(Vector2.Zero, false);
					}
				}
			}
			this.mOldNavDirection = controllerDirection;
			this.mOldState = state;
			if (ControlManager.Instance.IsInputLimited || this.mAvatar == null || this.mAvatar.Dead)
			{
				this.mHelpActive = false;
				this.mInputLocked = false;
				return;
			}
			if (base.Player == null)
			{
				return;
			}
			SpellWheel spellWheel = base.Player.SpellWheel;
			int num2 = -1;
			if (!this.mInputLocked && this.mCurrentDirection != ControllerDirection.Center)
			{
				num2 = SpellWheel.GetDirectionIndex(this.mCurrentDirection);
				for (int j = 0; j < this.mFadeTimers.Length; j++)
				{
					if (num2 == j)
					{
						this.mFadeTimers[j] += iDeltaTime;
						if (this.mFadeTimers[j] > 0.2f)
						{
							this.mFadeTimers[j] = 0.2f;
						}
						this.mMoveTimers[j] += iDeltaTime;
						if (this.mMoveTimers[j] > 0.2f)
						{
							this.mMoveTimers[j] = 0.2f;
						}
					}
					else
					{
						this.mFadeTimers[j] -= iDeltaTime;
						if (this.mFadeTimers[j] < 0f)
						{
							this.mFadeTimers[j] = 0f;
						}
					}
				}
				if (this.mStoredDirection != ControllerDirection.Center)
				{
					spellWheel.Direction(this.mStoredDirection);
				}
			}
			else
			{
				if (!this.mInputLocked && this.mVisualDirection != ControllerDirection.Center)
				{
					num2 = SpellWheel.GetDirectionIndex(this.mVisualDirection);
					spellWheel.Direction(this.mVisualDirection);
				}
				else
				{
					for (int k = 0; k < 4; k++)
					{
						if (this.mFadeTimers[k] <= 0f)
						{
							this.mMoveTimers[k] = 0f;
						}
					}
				}
				for (int l = 0; l < this.mFadeTimers.Length; l++)
				{
					if (num2 == l)
					{
						this.mFadeTimers[l] += iDeltaTime;
						if (this.mFadeTimers[l] > 0.2f)
						{
							this.mFadeTimers[l] = 0.2f;
						}
					}
					else
					{
						this.mFadeTimers[l] -= iDeltaTime;
						if (this.mFadeTimers[l] < 0f)
						{
							this.mFadeTimers[l] = 0f;
						}
					}
				}
			}
			spellWheel.FadeTimers(this.mFadeTimers);
			spellWheel.MoveTimers(this.mMoveTimers);
			if (this.mHelpActive && this.mHelpTime <= 0.2f)
			{
				this.mHelpTime = MathHelper.Min(this.mHelpTime + iDeltaTime, 0.2f);
			}
			else
			{
				this.mHelpTime = MathHelper.Max(this.mHelpTime - iDeltaTime, 0f);
			}
			spellWheel.HelpTimer(this.mHelpTime);
		}

		// Token: 0x06002A88 RID: 10888 RVA: 0x0014F580 File Offset: 0x0014D780
		public override void Invert(bool iInvert)
		{
			this.mInverted = iInvert;
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06002A89 RID: 10889 RVA: 0x0014F589 File Offset: 0x0014D789
		public PlayerIndex PlayerIndex
		{
			get
			{
				return this.mPlayerIndex;
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x06002A8A RID: 10890 RVA: 0x0014F591 File Offset: 0x0014D791
		public XInputController.Binding[] Bindings
		{
			get
			{
				return this.mBindings;
			}
		}

		// Token: 0x06002A8B RID: 10891 RVA: 0x0014F59C File Offset: 0x0014D79C
		protected void RightStickUpdate(Vector2 iValue)
		{
			if (this.mInverted)
			{
				Vector2.Negate(ref iValue, out iValue);
			}
			ControllerDirection controllerDirection = ControllerDirection.Center;
			if (this.mHelpActive)
			{
				return;
			}
			if (iValue.Length() < 0.3f)
			{
				this.mInputLocked = false;
				this.mVisualDirection = ControllerDirection.Center;
				this.mCurrentDirection = ControllerDirection.Center;
				return;
			}
			if (this.mInputLocked)
			{
				return;
			}
			if (iValue.Length() >= 0.3f && iValue.Length() < 0.6f)
			{
				if (Math.Abs(iValue.X) >= Math.Abs(iValue.Y))
				{
					if (iValue.X >= 0f)
					{
						this.mVisualDirection = ControllerDirection.Right;
						return;
					}
					this.mVisualDirection = ControllerDirection.Left;
					return;
				}
				else
				{
					if (iValue.Y >= 0f)
					{
						this.mVisualDirection = ControllerDirection.Up;
						return;
					}
					this.mVisualDirection = ControllerDirection.Down;
					return;
				}
			}
			else
			{
				if (this.mCurrentDirection == ControllerDirection.Center)
				{
					Vector2 vector = new Vector2(1f, 0f);
					float num;
					Vector2.Distance(ref vector, ref iValue, out num);
					if (num < 0.5f)
					{
						controllerDirection = ControllerDirection.Right;
					}
					vector = new Vector2(-1f, 0f);
					Vector2.Distance(ref vector, ref iValue, out num);
					if (num < 0.5f)
					{
						controllerDirection = ControllerDirection.Left;
					}
					vector = new Vector2(0f, 1f);
					Vector2.Distance(ref vector, ref iValue, out num);
					if (num < 0.5f)
					{
						controllerDirection = ControllerDirection.Up;
					}
					vector = new Vector2(0f, -1f);
					Vector2.Distance(ref vector, ref iValue, out num);
					if (num < 0.5f)
					{
						controllerDirection = ControllerDirection.Down;
					}
				}
				else
				{
					controllerDirection = this.mCurrentDirection;
					ControllerDirection controllerDirection2 = this.mCurrentDirection;
					switch (controllerDirection2)
					{
					case ControllerDirection.Right:
					case ControllerDirection.Left:
						if (Math.Abs(iValue.Y) <= 1f * Math.Abs(iValue.X))
						{
							goto IL_211;
						}
						if (iValue.Y > 0f)
						{
							controllerDirection = ControllerDirection.Up;
							goto IL_211;
						}
						controllerDirection = ControllerDirection.Down;
						goto IL_211;
					case ControllerDirection.Up:
						break;
					case ControllerDirection.UpRight:
						goto IL_211;
					default:
						if (controllerDirection2 != ControllerDirection.Down)
						{
							goto IL_211;
						}
						break;
					}
					if (Math.Abs(iValue.X) > 1f * Math.Abs(iValue.Y))
					{
						if (iValue.X > 0f)
						{
							controllerDirection = ControllerDirection.Right;
						}
						else
						{
							controllerDirection = ControllerDirection.Left;
						}
					}
				}
				IL_211:
				Vector2 vector2;
				Vector2.Subtract(ref iValue, ref this.mPreviousPosition, out vector2);
				if (controllerDirection != this.mCurrentDirection && this.mCurrentDirection != ControllerDirection.Center)
				{
					this.mAvatar.HandleCombo(this.mCurrentDirection);
					this.mAvatar.HandleCombo(controllerDirection);
					this.mInputLocked = true;
					return;
				}
				this.mCurrentDirection = controllerDirection;
				this.mStoredDirection = this.mCurrentDirection;
				this.mPreviousPosition = iValue;
				return;
			}
		}

		// Token: 0x06002A8C RID: 10892 RVA: 0x0014F81C File Offset: 0x0014DA1C
		protected ControllerDirection GetInterpolatedDirection(ControllerDirection iLastDirection, Vector2 iOldDirection, Vector2 iCurrentDirection)
		{
			for (float num = 0f; num <= 1f; num += 0.125f)
			{
				Vector2 iValue = Vector2.Lerp(iOldDirection, iCurrentDirection, num);
				ControllerDirection direction = base.GetDirection(iValue);
				if (direction != ControllerDirection.Dead && direction != iLastDirection)
				{
					return direction;
				}
			}
			return ControllerDirection.Dead;
		}

		// Token: 0x06002A8D RID: 10893 RVA: 0x0014F868 File Offset: 0x0014DA68
		protected ControllerDirection GetInterpolatedDirection(Vector2 iOldDirection, Vector2 iCurrentDirection)
		{
			for (float num = 0f; num <= 1f; num += 0.125f)
			{
				Vector2 iValue = Vector2.Lerp(iOldDirection, iCurrentDirection, num);
				ControllerDirection direction = base.GetDirection(iValue);
				if (direction != ControllerDirection.Dead)
				{
					return direction;
				}
			}
			return ControllerDirection.Dead;
		}

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x06002A8E RID: 10894 RVA: 0x0014F8AD File Offset: 0x0014DAAD
		// (set) Token: 0x06002A8F RID: 10895 RVA: 0x0014F8B5 File Offset: 0x0014DAB5
		public float HelpTimer
		{
			get
			{
				return this.mHelpTime;
			}
			set
			{
				this.mHelpTime = value;
			}
		}

		// Token: 0x06002A90 RID: 10896 RVA: 0x0014F8BE File Offset: 0x0014DABE
		public void SetFadeTime(int iIndex, float iValue)
		{
			this.mFadeTimers[iIndex] = iValue;
		}

		// Token: 0x06002A91 RID: 10897 RVA: 0x0014F8C9 File Offset: 0x0014DAC9
		public void SetMoveTime(int iIndex, float iValue)
		{
			this.mMoveTimers[iIndex] = iValue;
		}

		// Token: 0x06002A92 RID: 10898 RVA: 0x0014F8D4 File Offset: 0x0014DAD4
		public override void Rumble(float iLeft, float iRight)
		{
			this.mLeftRumble = Math.Max(iLeft, this.mLeftRumble);
			this.mRightRumble = Math.Max(iRight * 1.5f, this.mRightRumble);
		}

		// Token: 0x06002A93 RID: 10899 RVA: 0x0014F900 File Offset: 0x0014DB00
		public override float LeftRumble()
		{
			return this.mLeftRumble;
		}

		// Token: 0x06002A94 RID: 10900 RVA: 0x0014F908 File Offset: 0x0014DB08
		public override float RightRumble()
		{
			return this.mRightRumble;
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x06002A95 RID: 10901 RVA: 0x0014F910 File Offset: 0x0014DB10
		public bool IsConnected
		{
			get
			{
				return this.mOldState.IsConnected;
			}
		}

		// Token: 0x06002A96 RID: 10902 RVA: 0x0014F91D File Offset: 0x0014DB1D
		public override void Clear()
		{
		}

		// Token: 0x06002A97 RID: 10903 RVA: 0x0014F920 File Offset: 0x0014DB20
		private float GetClampedPointerRotation(Vector2 iPosition, ControllerDirection iDirection)
		{
			switch (iDirection)
			{
			case ControllerDirection.Right:
			case ControllerDirection.Left:
				iPosition.Y *= 0.35f;
				goto IL_49;
			case ControllerDirection.Up:
				break;
			case ControllerDirection.UpRight:
				goto IL_49;
			default:
				if (iDirection != ControllerDirection.Down)
				{
					goto IL_49;
				}
				break;
			}
			iPosition.X *= 0.35f;
			IL_49:
			iPosition.Normalize();
			Vector2 vector = new Vector2(iPosition.X, -iPosition.Y);
			float num = MagickaMath.Angle(vector);
			float min = 0f;
			float max = 1f;
			switch (iDirection)
			{
			case ControllerDirection.Right:
				min = -0.3926991f;
				max = 0.3926991f;
				break;
			case ControllerDirection.Up:
				min = -1.9634955f;
				max = -1.1780972f;
				break;
			case ControllerDirection.UpRight:
				break;
			case ControllerDirection.Left:
				if (num < 0f)
				{
					num += 6.2831855f;
				}
				min = 2.7488937f;
				max = 3.5342917f;
				break;
			default:
				if (iDirection == ControllerDirection.Down)
				{
					min = 1.1780972f;
					max = 1.9634955f;
				}
				break;
			}
			return MathHelper.Clamp(num, min, max);
		}

		// Token: 0x06002A98 RID: 10904 RVA: 0x0014FA1C File Offset: 0x0014DC1C
		private float GetBoundValueF(ControllerFunction iFunction, GamePadState iState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case XInputController.Binding.BindingType.Button:
				if (!iState.IsButtonDown((Buttons)this.mBindings[(int)iFunction].BindingIndex))
				{
					return 0f;
				}
				return 1f;
			case XInputController.Binding.BindingType.Trigger:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iState.Triggers.Left;
				case 1:
					return iState.Triggers.Right;
				default:
					return 0f;
				}
				break;
			case XInputController.Binding.BindingType.PositiveStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return Math.Max(iState.ThumbSticks.Left.X, 0f);
				case 1:
					return Math.Max(iState.ThumbSticks.Left.Y, 0f);
				case 2:
					return Math.Max(iState.ThumbSticks.Right.X, 0f);
				case 3:
					return Math.Max(iState.ThumbSticks.Right.Y, 0f);
				default:
					return 0f;
				}
				break;
			case XInputController.Binding.BindingType.NegativeStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return Math.Max(-iState.ThumbSticks.Left.X, 0f);
				case 1:
					return Math.Max(-iState.ThumbSticks.Left.Y, 0f);
				case 2:
					return Math.Max(-iState.ThumbSticks.Right.X, 0f);
				case 3:
					return Math.Max(-iState.ThumbSticks.Right.Y, 0f);
				default:
					return 0f;
				}
				break;
			default:
				return 0f;
			}
		}

		// Token: 0x06002A99 RID: 10905 RVA: 0x0014FC3C File Offset: 0x0014DE3C
		private bool GetBoundValueB(ControllerFunction iFunction, GamePadState iState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case XInputController.Binding.BindingType.Button:
				return iState.IsButtonDown((Buttons)this.mBindings[(int)iFunction].BindingIndex);
			case XInputController.Binding.BindingType.Trigger:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iState.Triggers.Left >= 0.5f;
				case 1:
					return iState.Triggers.Right >= 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.PositiveStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iState.ThumbSticks.Left.X >= 0.5f;
				case 1:
					return iState.ThumbSticks.Left.Y >= 0.5f;
				case 2:
					return iState.ThumbSticks.Right.X >= 0.5f;
				case 3:
					return iState.ThumbSticks.Right.Y >= 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.NegativeStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iState.ThumbSticks.Left.X <= -0.5f;
				case 1:
					return iState.ThumbSticks.Left.Y <= -0.5f;
				case 2:
					return iState.ThumbSticks.Right.X <= -0.5f;
				case 3:
					return iState.ThumbSticks.Right.Y <= -0.5f;
				default:
					return false;
				}
				break;
			default:
				return false;
			}
		}

		// Token: 0x06002A9A RID: 10906 RVA: 0x0014FE4C File Offset: 0x0014E04C
		private bool GetBoundValuePressed(ControllerFunction iFunction, GamePadState iNewState, GamePadState iOldState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case XInputController.Binding.BindingType.Button:
				return iNewState.IsButtonDown((Buttons)this.mBindings[(int)iFunction].BindingIndex) & iOldState.IsButtonUp((Buttons)this.mBindings[(int)iFunction].BindingIndex);
			case XInputController.Binding.BindingType.Trigger:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iNewState.Triggers.Left >= 0.5f & iOldState.Triggers.Left < 0.5f;
				case 1:
					return iNewState.Triggers.Right >= 0.5f & iOldState.Triggers.Right < 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.PositiveStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iNewState.ThumbSticks.Left.X >= 0.5f & iOldState.ThumbSticks.Left.X < 0.5f;
				case 1:
					return iNewState.ThumbSticks.Left.Y >= 0.5f & iOldState.ThumbSticks.Left.Y < 0.5f;
				case 2:
					return iNewState.ThumbSticks.Right.X >= 0.5f & iOldState.ThumbSticks.Right.X < 0.5f;
				case 3:
					return iNewState.ThumbSticks.Right.Y >= 0.5f & iOldState.ThumbSticks.Right.Y < 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.NegativeStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iNewState.ThumbSticks.Left.X <= -0.5f & iOldState.ThumbSticks.Left.X > -0.5f;
				case 1:
					return iNewState.ThumbSticks.Left.Y <= -0.5f & iOldState.ThumbSticks.Left.Y > -0.5f;
				case 2:
					return iNewState.ThumbSticks.Right.X <= -0.5f & iOldState.ThumbSticks.Right.X > -0.5f;
				case 3:
					return iNewState.ThumbSticks.Right.Y <= -0.5f & iOldState.ThumbSticks.Right.Y > -0.5f;
				default:
					return false;
				}
				break;
			default:
				return false;
			}
		}

		// Token: 0x06002A9B RID: 10907 RVA: 0x00150190 File Offset: 0x0014E390
		private bool GetBoundValueReleased(ControllerFunction iFunction, GamePadState iNewState, GamePadState iOldState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case XInputController.Binding.BindingType.Button:
				return iOldState.IsButtonDown((Buttons)this.mBindings[(int)iFunction].BindingIndex) & iNewState.IsButtonUp((Buttons)this.mBindings[(int)iFunction].BindingIndex);
			case XInputController.Binding.BindingType.Trigger:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iOldState.Triggers.Left >= 0.5f & iNewState.Triggers.Left < 0.5f;
				case 1:
					return iOldState.Triggers.Right >= 0.5f & iNewState.Triggers.Right < 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.PositiveStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iOldState.ThumbSticks.Left.X >= 0.5f & iNewState.ThumbSticks.Left.X < 0.5f;
				case 1:
					return iOldState.ThumbSticks.Left.Y >= 0.5f & iNewState.ThumbSticks.Left.Y < 0.5f;
				case 2:
					return iOldState.ThumbSticks.Right.X >= 0.5f & iNewState.ThumbSticks.Right.X < 0.5f;
				case 3:
					return iOldState.ThumbSticks.Right.Y >= 0.5f & iNewState.ThumbSticks.Right.Y < 0.5f;
				default:
					return false;
				}
				break;
			case XInputController.Binding.BindingType.NegativeStick:
				switch (this.mBindings[(int)iFunction].BindingIndex)
				{
				case 0:
					return iOldState.ThumbSticks.Left.X <= -0.5f & iNewState.ThumbSticks.Left.X > -0.5f;
				case 1:
					return iOldState.ThumbSticks.Left.Y <= -0.5f & iNewState.ThumbSticks.Left.Y > -0.5f;
				case 2:
					return iOldState.ThumbSticks.Right.X <= -0.5f & iNewState.ThumbSticks.Right.X > -0.5f;
				case 3:
					return iOldState.ThumbSticks.Right.Y <= -0.5f & iNewState.ThumbSticks.Right.Y > -0.5f;
				default:
					return false;
				}
				break;
			default:
				return false;
			}
		}

		// Token: 0x04002DDD RID: 11741
		public static readonly int[] BUTTON_NAMES;

		// Token: 0x04002DDE RID: 11742
		public static readonly string COLDKEY = new string('̠', 1) + new string('̣', 1);

		// Token: 0x04002DDF RID: 11743
		public static readonly string SELFKEY = new string('̧', 1);

		// Token: 0x04002DE0 RID: 11744
		public static readonly string WEAPONKEY = new string('̤', 1);

		// Token: 0x04002DE2 RID: 11746
		private PlayerIndex mPlayerIndex;

		// Token: 0x04002DE3 RID: 11747
		private GamePadState mOldState;

		// Token: 0x04002DE4 RID: 11748
		private float mLeftRumble;

		// Token: 0x04002DE5 RID: 11749
		private float mRightRumble;

		// Token: 0x04002DE6 RID: 11750
		private float mMenuMoveCooldown;

		// Token: 0x04002DE7 RID: 11751
		private ControllerDirection mOldNavDirection;

		// Token: 0x04002DE8 RID: 11752
		private Vector2 mPreviousPosition;

		// Token: 0x04002DE9 RID: 11753
		private ControllerDirection mCurrentDirection;

		// Token: 0x04002DEA RID: 11754
		private ControllerDirection mStoredDirection;

		// Token: 0x04002DEB RID: 11755
		private ControllerDirection mVisualDirection;

		// Token: 0x04002DEC RID: 11756
		private float[] mFadeTimers;

		// Token: 0x04002DED RID: 11757
		private float[] mMoveTimers;

		// Token: 0x04002DEE RID: 11758
		private bool mInputLocked;

		// Token: 0x04002DEF RID: 11759
		private bool mHelpActive;

		// Token: 0x04002DF0 RID: 11760
		private float mHelpTime;

		// Token: 0x04002DF1 RID: 11761
		private XInputController.Binding[] mBindings;

		// Token: 0x02000591 RID: 1425
		public struct Binding
		{
			// Token: 0x06002A9C RID: 10908 RVA: 0x001504D1 File Offset: 0x0014E6D1
			public Binding(XInputController.Binding.BindingType iType, int iIndex)
			{
				this.Type = iType;
				this.BindingIndex = iIndex;
			}

			// Token: 0x04002DF2 RID: 11762
			public XInputController.Binding.BindingType Type;

			// Token: 0x04002DF3 RID: 11763
			public int BindingIndex;

			// Token: 0x02000592 RID: 1426
			public enum BindingType
			{
				// Token: 0x04002DF5 RID: 11765
				None,
				// Token: 0x04002DF6 RID: 11766
				Button,
				// Token: 0x04002DF7 RID: 11767
				Trigger,
				// Token: 0x04002DF8 RID: 11768
				PositiveStick,
				// Token: 0x04002DF9 RID: 11769
				NegativeStick
			}
		}
	}
}
