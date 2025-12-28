using System;
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

namespace Magicka.GameLogic.Controls
{
	// Token: 0x02000546 RID: 1350
	internal class DirectInputController : Controller
	{
		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600281A RID: 10266 RVA: 0x00125E1A File Offset: 0x0012401A
		// (remove) Token: 0x0600281B RID: 10267 RVA: 0x00125E33 File Offset: 0x00124033
		public event Action<DirectInputController.Binding> OnChange;

		// Token: 0x0600281D RID: 10269 RVA: 0x00125E9C File Offset: 0x0012409C
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

		// Token: 0x0600281E RID: 10270 RVA: 0x00125F38 File Offset: 0x00124138
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (!this.mDevice.Caps.Attatched)
			{
				this.mOldState = default(DirectInputController.State);
				this.mAcquire = false;
				return;
			}
			if (!this.mAcquire)
			{
				this.mDevice.Acquire();
				this.mAcquire = true;
				DirectInputController.State.FromDevice(this.mDevice, out this.mOldState);
				return;
			}
			DialogManager instance = DialogManager.Instance;
			Tome instance2 = Tome.Instance;
			GameState currentState = GameStateManager.Instance.CurrentState;
			TutorialManager instance3 = TutorialManager.Instance;
			PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
			this.mAvatar = ((base.Player == null) ? null : base.Player.Avatar);
			this.mDevice.Poll();
			DirectInputController.State state;
			DirectInputController.State.FromDevice(this.mDevice, out state);
			if (this.OnChange != null)
			{
				DirectInputController.Binding obj = default(DirectInputController.Binding);
				for (int i = 0; i < state.Buttons.NrOfButtons; i++)
				{
					if (state.Buttons[i] & !this.mOldState.Buttons[i])
					{
						obj.Type = DirectInputController.Binding.BindingType.Button;
						obj.BindingIndex = i;
						break;
					}
				}
				if (obj.Type == DirectInputController.Binding.BindingType.None)
				{
					for (int j = 0; j < 4; j++)
					{
						if (state.DPad[j] & !this.mOldState.DPad[j])
						{
							obj.Type = DirectInputController.Binding.BindingType.POV;
							obj.BindingIndex = j;
							break;
						}
					}
				}
				if (obj.Type == DirectInputController.Binding.BindingType.None)
				{
					for (int k = 0; k < 6; k++)
					{
						if (state.Axes[k] >= 0.5f & this.mOldState.Axes[k] < 0.5f)
						{
							obj.Type = DirectInputController.Binding.BindingType.PositiveAxis;
							obj.BindingIndex = k;
							break;
						}
						if (state.Axes[k] <= -0.5f & this.mOldState.Axes[k] > -0.5f)
						{
							obj.Type = DirectInputController.Binding.BindingType.NegativeAxis;
							obj.BindingIndex = k;
							break;
						}
					}
				}
				if (obj.Type != DirectInputController.Binding.BindingType.None)
				{
					Action<DirectInputController.Binding> onChange = this.OnChange;
					this.OnChange = null;
					onChange(obj);
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
				for (int l = 0; l < this.mFadeTimers.Length; l++)
				{
					if (num2 == l)
					{
						this.mFadeTimers[l] += iDeltaTime;
						if (this.mFadeTimers[l] > 0.2f)
						{
							this.mFadeTimers[l] = 0.2f;
						}
						this.mMoveTimers[l] += iDeltaTime;
						if (this.mMoveTimers[l] > 0.2f)
						{
							this.mMoveTimers[l] = 0.2f;
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
					for (int m = 0; m < 4; m++)
					{
						if (this.mFadeTimers[m] <= 0f)
						{
							this.mMoveTimers[m] = 0f;
						}
					}
				}
				for (int n = 0; n < this.mFadeTimers.Length; n++)
				{
					if (num2 == n)
					{
						this.mFadeTimers[n] += iDeltaTime;
						if (this.mFadeTimers[n] > 0.2f)
						{
							this.mFadeTimers[n] = 0.2f;
						}
					}
					else
					{
						this.mFadeTimers[n] -= iDeltaTime;
						if (this.mFadeTimers[n] < 0f)
						{
							this.mFadeTimers[n] = 0f;
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

		// Token: 0x0600281F RID: 10271 RVA: 0x00127062 File Offset: 0x00125262
		public override void Invert(bool iInvert)
		{
			this.mInverted = iInvert;
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06002820 RID: 10272 RVA: 0x0012706B File Offset: 0x0012526B
		public DirectInputController.Binding[] Bindings
		{
			get
			{
				return this.mBindings;
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06002821 RID: 10273 RVA: 0x00127074 File Offset: 0x00125274
		public bool IsConnected
		{
			get
			{
				return this.mDevice.Caps.Attatched;
			}
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06002822 RID: 10274 RVA: 0x00127094 File Offset: 0x00125294
		// (set) Token: 0x06002823 RID: 10275 RVA: 0x0012709C File Offset: 0x0012529C
		public bool Configured
		{
			get
			{
				return this.mConfigured;
			}
			set
			{
				this.mConfigured = (this.mConfigured || value);
			}
		}

		// Token: 0x06002824 RID: 10276 RVA: 0x001270AC File Offset: 0x001252AC
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

		// Token: 0x06002825 RID: 10277 RVA: 0x0012732C File Offset: 0x0012552C
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

		// Token: 0x06002826 RID: 10278 RVA: 0x00127378 File Offset: 0x00125578
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

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06002827 RID: 10279 RVA: 0x001273BD File Offset: 0x001255BD
		// (set) Token: 0x06002828 RID: 10280 RVA: 0x001273C5 File Offset: 0x001255C5
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

		// Token: 0x06002829 RID: 10281 RVA: 0x001273CE File Offset: 0x001255CE
		public void SetFadeTime(int iIndex, float iValue)
		{
			this.mFadeTimers[iIndex] = iValue;
		}

		// Token: 0x0600282A RID: 10282 RVA: 0x001273D9 File Offset: 0x001255D9
		public void SetMoveTime(int iIndex, float iValue)
		{
			this.mMoveTimers[iIndex] = iValue;
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x0600282B RID: 10283 RVA: 0x001273E4 File Offset: 0x001255E4
		public Device Device
		{
			get
			{
				return this.mDevice;
			}
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x001273EC File Offset: 0x001255EC
		public override void Rumble(float iLeft, float iRight)
		{
			this.mLeftRumble = Math.Max(iLeft, this.mLeftRumble);
			this.mRightRumble = Math.Max(iRight * 1.5f, this.mRightRumble);
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x00127418 File Offset: 0x00125618
		public override float LeftRumble()
		{
			return this.mLeftRumble;
		}

		// Token: 0x0600282E RID: 10286 RVA: 0x00127420 File Offset: 0x00125620
		public override float RightRumble()
		{
			return this.mRightRumble;
		}

		// Token: 0x0600282F RID: 10287 RVA: 0x00127428 File Offset: 0x00125628
		public override void Clear()
		{
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x0012742C File Offset: 0x0012562C
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

		// Token: 0x06002831 RID: 10289 RVA: 0x00127528 File Offset: 0x00125728
		private float GetBoundValueF(ControllerFunction iFunction, DirectInputController.State iState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case DirectInputController.Binding.BindingType.Button:
				if (!iState.Buttons[this.mBindings[(int)iFunction].BindingIndex])
				{
					return 0f;
				}
				return 1f;
			case DirectInputController.Binding.BindingType.POV:
				if (!iState.DPad[this.mBindings[(int)iFunction].BindingIndex])
				{
					return 0f;
				}
				return 1f;
			case DirectInputController.Binding.BindingType.PositiveAxis:
				return Math.Max(iState.Axes[this.mBindings[(int)iFunction].BindingIndex], 0f);
			case DirectInputController.Binding.BindingType.NegativeAxis:
				return Math.Max(-iState.Axes[this.mBindings[(int)iFunction].BindingIndex], 0f);
			default:
				return 0f;
			}
		}

		// Token: 0x06002832 RID: 10290 RVA: 0x00127610 File Offset: 0x00125810
		private bool GetBoundValueB(ControllerFunction iFunction, DirectInputController.State iState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case DirectInputController.Binding.BindingType.Button:
				return iState.Buttons[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.POV:
				return iState.DPad[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.PositiveAxis:
				return iState.Axes[this.mBindings[(int)iFunction].BindingIndex] >= 0.5f;
			case DirectInputController.Binding.BindingType.NegativeAxis:
				return iState.Axes[this.mBindings[(int)iFunction].BindingIndex] <= -0.5f;
			default:
				return false;
			}
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x001276DC File Offset: 0x001258DC
		private bool GetBoundValuePressed(ControllerFunction iFunction, DirectInputController.State iNewState, DirectInputController.State iOldState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case DirectInputController.Binding.BindingType.Button:
				return iNewState.Buttons[this.mBindings[(int)iFunction].BindingIndex] & !iOldState.Buttons[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.POV:
				return iNewState.DPad[this.mBindings[(int)iFunction].BindingIndex] & !iOldState.DPad[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.PositiveAxis:
				return iNewState.Axes[this.mBindings[(int)iFunction].BindingIndex] >= 0.5f & iOldState.Axes[this.mBindings[(int)iFunction].BindingIndex] < 0.5f;
			case DirectInputController.Binding.BindingType.NegativeAxis:
				return iNewState.Axes[this.mBindings[(int)iFunction].BindingIndex] <= -0.5f & iOldState.Axes[this.mBindings[(int)iFunction].BindingIndex] > -0.5f;
			default:
				return false;
			}
		}

		// Token: 0x06002834 RID: 10292 RVA: 0x00127834 File Offset: 0x00125A34
		private bool GetBoundValueReleased(ControllerFunction iFunction, DirectInputController.State iNewState, DirectInputController.State iOldState)
		{
			switch (this.mBindings[(int)iFunction].Type)
			{
			case DirectInputController.Binding.BindingType.Button:
				return iOldState.Buttons[this.mBindings[(int)iFunction].BindingIndex] & !iNewState.Buttons[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.POV:
				return iOldState.DPad[this.mBindings[(int)iFunction].BindingIndex] & !iNewState.DPad[this.mBindings[(int)iFunction].BindingIndex];
			case DirectInputController.Binding.BindingType.PositiveAxis:
				return iOldState.Axes[this.mBindings[(int)iFunction].BindingIndex] >= 0.5f & iNewState.Axes[this.mBindings[(int)iFunction].BindingIndex] < 0.5f;
			case DirectInputController.Binding.BindingType.NegativeAxis:
				return iOldState.Axes[this.mBindings[(int)iFunction].BindingIndex] <= -0.5f & iNewState.Axes[this.mBindings[(int)iFunction].BindingIndex] > -0.5f;
			default:
				return false;
			}
		}

		// Token: 0x04002BA3 RID: 11171
		public static readonly string COLDKEY = new string('̠', 1) + new string('̣', 1);

		// Token: 0x04002BA4 RID: 11172
		public static readonly string SELFKEY = new string('̧', 1);

		// Token: 0x04002BA5 RID: 11173
		public static readonly string WEAPONKEY = new string('̤', 1);

		// Token: 0x04002BA7 RID: 11175
		private Device mDevice;

		// Token: 0x04002BA8 RID: 11176
		private DirectInputController.State mOldState;

		// Token: 0x04002BA9 RID: 11177
		private float mLeftRumble;

		// Token: 0x04002BAA RID: 11178
		private float mRightRumble;

		// Token: 0x04002BAB RID: 11179
		private float mMenuMoveCooldown;

		// Token: 0x04002BAC RID: 11180
		private ControllerDirection mOldNavDirection;

		// Token: 0x04002BAD RID: 11181
		private Vector2 mPreviousPosition;

		// Token: 0x04002BAE RID: 11182
		private ControllerDirection mCurrentDirection;

		// Token: 0x04002BAF RID: 11183
		private ControllerDirection mStoredDirection;

		// Token: 0x04002BB0 RID: 11184
		private ControllerDirection mVisualDirection;

		// Token: 0x04002BB1 RID: 11185
		private float[] mFadeTimers;

		// Token: 0x04002BB2 RID: 11186
		private float[] mMoveTimers;

		// Token: 0x04002BB3 RID: 11187
		private bool mInputLocked;

		// Token: 0x04002BB4 RID: 11188
		private bool mHelpActive;

		// Token: 0x04002BB5 RID: 11189
		private float mHelpTime;

		// Token: 0x04002BB6 RID: 11190
		private DirectInputController.Binding[] mBindings;

		// Token: 0x04002BB7 RID: 11191
		private bool mConfigured;

		// Token: 0x04002BB8 RID: 11192
		private bool mAcquire;

		// Token: 0x02000547 RID: 1351
		private struct State
		{
			// Token: 0x06002835 RID: 10293 RVA: 0x0012798C File Offset: 0x00125B8C
			public static void FromDevice(Device iDevice, out DirectInputController.State oState)
			{
				JoystickState currentJoystickState = iDevice.CurrentJoystickState;
				oState.Axes = new DirectInputAxes(iDevice, currentJoystickState);
				oState.DPad = new DirectInputDPad(currentJoystickState.GetPointOfView()[0]);
				oState.Buttons = new DirectInputButtons(iDevice, currentJoystickState);
			}

			// Token: 0x04002BB9 RID: 11193
			public DirectInputAxes Axes;

			// Token: 0x04002BBA RID: 11194
			public DirectInputDPad DPad;

			// Token: 0x04002BBB RID: 11195
			public DirectInputButtons Buttons;
		}

		// Token: 0x02000548 RID: 1352
		public struct Binding
		{
			// Token: 0x06002836 RID: 10294 RVA: 0x001279CE File Offset: 0x00125BCE
			public Binding(DirectInputController.Binding.BindingType iType, int iIndex)
			{
				this.Type = iType;
				this.BindingIndex = iIndex;
			}

			// Token: 0x04002BBC RID: 11196
			public DirectInputController.Binding.BindingType Type;

			// Token: 0x04002BBD RID: 11197
			public int BindingIndex;

			// Token: 0x02000549 RID: 1353
			public enum BindingType
			{
				// Token: 0x04002BBF RID: 11199
				None,
				// Token: 0x04002BC0 RID: 11200
				Button,
				// Token: 0x04002BC1 RID: 11201
				POV,
				// Token: 0x04002BC2 RID: 11202
				PositiveAxis,
				// Token: 0x04002BC3 RID: 11203
				NegativeAxis
			}
		}
	}
}
