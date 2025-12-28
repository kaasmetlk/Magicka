using System;
using System.Collections.Generic;
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

namespace Magicka.GameLogic.Controls
{
	// Token: 0x020004E3 RID: 1251
	internal class KeyboardMouseController : Controller
	{
		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600251F RID: 9503 RVA: 0x0010CA75 File Offset: 0x0010AC75
		// (remove) Token: 0x06002520 RID: 9504 RVA: 0x0010CA8E File Offset: 0x0010AC8E
		public event Action AnyKeyPress;

		// Token: 0x06002521 RID: 9505 RVA: 0x0010CACC File Offset: 0x0010ACCC
		static KeyboardMouseController()
		{
			KeyboardMouseController.LoadDefaults();
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x0010CB84 File Offset: 0x0010AD84
		public KeyboardMouseController(InputMessageFilter iMessageFilter)
		{
			this.mFadeTimers = new float[4];
			this.mMoveTimers = new float[4];
			this.mCursorPressedTarget = null;
			iMessageFilter.KeyDown += this.OnKeyDown;
			iMessageFilter.KeyPress += new Action<char, KeyModifiers>(this.OnKeyPress);
			iMessageFilter.KeyUp += this.OnKeyUp;
			this.mInteractMoveLock = false;
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x0010CC00 File Offset: 0x0010AE00
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			DialogManager instance = DialogManager.Instance;
			Tome instance2 = Tome.Instance;
			GameState currentState = GameStateManager.Instance.CurrentState;
			ControlManager instance3 = ControlManager.Instance;
			TutorialManager instance4 = TutorialManager.Instance;
			Point screenSize = RenderManager.Instance.ScreenSize;
			PersistentGameState persistentState = GameStateManager.Instance.PersistentState;
			KeyboardMouseController.InputState inputState = default(KeyboardMouseController.InputState);
			inputState.NewKeyboardState = Game.Instance.KeyboardState;
			inputState.OldKeyboardState = this.mLastKeyboardState;
			this.mLastKeyboardState = inputState.NewKeyboardState;
			inputState.NewMouseState = Game.Instance.MouseState;
			inputState.OldMouseState = this.mLastMouseState;
			this.mLastMouseState = inputState.NewMouseState;
			bool focused = Game.Instance.Focused;
			if (focused & !this.mIsGameActive)
			{
				this.mInteractMoveLock = false;
			}
			this.mIsGameActive = focused;
			PlayState playState = currentState as PlayState;
			if (!this.mIsGameActive)
			{
				if (playState != null && !playState.IsInCutscene && base.Player != null && base.Player.Playing)
				{
					this.mHUD.Update(iDataChannel, iDeltaTime);
				}
				return;
			}
			if (this.AnyKeyPress != null)
			{
				if (inputState.NewKeyboardState != inputState.OldKeyboardState && inputState.NewKeyboardState.GetPressedKeys().Length > 0)
				{
					this.AnyKeyPress.Invoke();
					return;
				}
				if (inputState.NewMouseState.LeftButton == ButtonState.Pressed || inputState.NewMouseState.MiddleButton == ButtonState.Pressed || inputState.NewMouseState.RightButton == ButtonState.Pressed || inputState.NewMouseState.XButton1 == ButtonState.Pressed || inputState.NewMouseState.XButton2 == ButtonState.Pressed || inputState.NewMouseState.ScrollWheelValue != inputState.OldMouseState.ScrollWheelValue)
				{
					this.AnyKeyPress.Invoke();
					return;
				}
			}
			if (KeyboardMouseController.mCatchKeyActive)
			{
				if (this.IsPressed(KeyboardMouseController.MouseButton.Left, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Left, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.Middle, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Middle, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.Right, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.Right, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.X1, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.X1, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.X2, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.X2, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.ScrollUp, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.ScrollUp, KeyboardMouseController.mCatchKeyIndex);
				}
				if (this.IsPressed(KeyboardMouseController.MouseButton.ScrollDown, ref inputState))
				{
					KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(KeyboardMouseController.MouseButton.ScrollDown, KeyboardMouseController.mCatchKeyIndex);
				}
				return;
			}
			bool flag = this.IsPressed(Keys.Enter, ref inputState);
			bool flag2 = this.IsPressed(Keys.Back, ref inputState);
			bool flag3 = this.IsPressed(Keys.Escape, ref inputState);
			bool flag4 = this.IsPressed(Keys.Space, ref inputState);
			bool flag5 = this.IsPressed(Keys.Delete, ref inputState);
			bool flag6 = inputState.NewMouseState.LeftButton == ButtonState.Pressed & inputState.OldMouseState.LeftButton == ButtonState.Released;
			bool flag7 = inputState.NewMouseState.LeftButton == ButtonState.Released & inputState.OldMouseState.LeftButton == ButtonState.Pressed;
			bool flag8 = inputState.NewMouseState.RightButton == ButtonState.Pressed & inputState.OldMouseState.RightButton == ButtonState.Released;
			bool flag9 = inputState.NewMouseState.RightButton == ButtonState.Released & inputState.OldMouseState.RightButton == ButtonState.Pressed;
			ControllerDirection direction = KeyboardMouseController.GetDirection(ref inputState.NewKeyboardState, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
			ControllerDirection direction2 = KeyboardMouseController.GetDirection(ref inputState.OldKeyboardState, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
			Cursors iCursor = Cursors.Default;
			bool iActive = true;
			bool flag10 = inputState.NewMouseState.X != inputState.OldMouseState.X | inputState.NewMouseState.Y != inputState.OldMouseState.Y;
			Vector2 iPosition = default(Vector2);
			iPosition.X = (float)inputState.NewMouseState.X;
			iPosition.Y = (float)inputState.NewMouseState.Y;
			if (PdxLoginWindow.Instance.Visible)
			{
				Game.Instance.IsMouseVisible = true;
				if (flag6)
				{
					PdxLoginWindow.Instance.OnMouseDown(ref iPosition);
				}
				if (flag7)
				{
					PdxLoginWindow.Instance.OnMouseUp(ref iPosition);
				}
				if (flag10)
				{
					PdxLoginWindow.Instance.OnMouseMove(ref iPosition);
				}
			}
			else if (PdxWidget.Instance.Visible)
			{
				Game.Instance.IsMouseVisible = true;
				if (flag6)
				{
					PdxWidget.Instance.OnMouseDown(ref iPosition);
				}
				if (flag7)
				{
					PdxWidget.Instance.OnMouseUp(ref iPosition);
				}
				if (flag10)
				{
					PdxWidget.Instance.OnMouseMove(ref iPosition);
				}
			}
			else if (currentState is CompanyState & (flag3 || flag || flag2 || flag4 || flag8 || flag6))
			{
				(currentState as CompanyState).SkipScreen();
			}
			else if (persistentState.IsolateControls())
			{
				if (flag3)
				{
					persistentState.ControllerB(this);
				}
				if (direction != direction2 && direction != ControllerDirection.Center && direction != ControllerDirection.Dead)
				{
					persistentState.ControllerMovement(this, direction);
				}
				persistentState.ControllerEvent(this, inputState.NewKeyboardState, inputState.OldKeyboardState);
				if (flag || flag4)
				{
					persistentState.ControllerA(this);
				}
				else if (flag7 || flag6 || flag8 || flag9 || inputState.NewMouseState.ScrollWheelValue != inputState.OldMouseState.ScrollWheelValue)
				{
					persistentState.ControllerMouseAction(this, screenSize, inputState.NewMouseState, inputState.OldMouseState);
				}
				else if (flag10)
				{
					Game.Instance.IsMouseVisible = true;
					persistentState.ControllerMouseMove(this, screenSize, inputState.NewMouseState, inputState.OldMouseState);
				}
			}
			else if (instance.MessageBoxActive)
			{
				if (flag3)
				{
					instance.ControllerEsc(this);
				}
				else if ((flag6 || flag7) | inputState.NewMouseState.ScrollWheelValue != inputState.OldMouseState.ScrollWheelValue)
				{
					instance.ControllerMouseClick(this, inputState.NewMouseState, inputState.OldMouseState);
				}
				else if (flag10)
				{
					Game.Instance.IsMouseVisible = true;
					instance.ControllerMouseMove(this, inputState.NewMouseState, inputState.OldMouseState);
				}
			}
			else if (currentState is MenuState)
			{
				if (MenuState.Instance.TomeTakesInput)
				{
					if (flag3)
					{
						instance2.ControllerB(this);
					}
					if (flag5 & Tome.Instance.CurrentMenu is SubMenuCampaignSelect_SaveSlotSelect)
					{
						(Tome.Instance.CurrentMenu as SubMenuCampaignSelect_SaveSlotSelect).Delete(this);
					}
					if (direction != direction2 & direction != ControllerDirection.Center & direction != ControllerDirection.Dead)
					{
						instance2.ControllerMovement(this, direction);
					}
					instance2.ControllerEvent(this, inputState.OldKeyboardState, inputState.NewKeyboardState);
					if (Tome.Instance.CurrentState is Tome.ClosedBack && (flag6 || flag || flag4))
					{
						instance2.ControllerA(this);
					}
					else if ((flag7 || flag6 || flag8 || flag9) | inputState.NewMouseState.ScrollWheelValue != inputState.OldMouseState.ScrollWheelValue)
					{
						instance2.ControllerMouseAction(this, screenSize, inputState.NewMouseState, inputState.OldMouseState);
					}
					else if (flag10)
					{
						Game.Instance.IsMouseVisible = true;
						instance2.ControllerMouseMove(this, screenSize, inputState.NewMouseState, inputState.OldMouseState);
					}
				}
			}
			else if (playState != null)
			{
				if (playState.IsInCutscene && !playState.IsPaused)
				{
					Game.Instance.IsMouseVisible = false;
				}
				else if (base.Player != null && base.Player.Playing)
				{
					this.mHUD.Update(iDataChannel, iDeltaTime);
				}
				if (playState.IsPaused && InGameMenu.IsControllerAllowed(this))
				{
					if (flag3)
					{
						playState.TogglePause(this);
					}
					if (flag || flag4)
					{
						InGameMenu.ControllerSelect(this);
					}
					if (flag2)
					{
						InGameMenu.ControllerBack(this);
					}
					if (direction != direction2 & direction != ControllerDirection.Center & direction != ControllerDirection.Dead)
					{
						instance2.ControllerMovement(this, direction);
					}
					if (flag10)
					{
						Game.Instance.IsMouseVisible = true;
						InGameMenu.MouseMove(this, ref iPosition);
					}
					if (flag6)
					{
						InGameMenu.MouseDown(this, ref iPosition);
					}
					else if (flag7)
					{
						InGameMenu.MouseUp(this, ref iPosition);
					}
					else if (inputState.NewMouseState.ScrollWheelValue != inputState.OldMouseState.ScrollWheelValue)
					{
						InGameMenu.MouseScroll(this, ref iPosition, inputState.NewMouseState.ScrollWheelValue - inputState.OldMouseState.ScrollWheelValue);
					}
					if (direction != ControllerDirection.Dead & direction != ControllerDirection.Center & direction != direction2)
					{
						InGameMenu.ControllerMove(this, direction);
					}
				}
				else
				{
					if (flag3)
					{
						playState.TogglePause(this);
					}
					else if (flag6 || flag8)
					{
						instance4.RemoveDialogHint();
					}
					if ((flag || flag4 || flag6) && instance.CanAdvance(this) && !playState.IsInCutscene)
					{
						instance.Advance(this);
						this.mAvatar.MouseMoveStop();
					}
					if (inputState.NewKeyboardState.IsKeyDown(Keys.Enter) && inputState.OldKeyboardState.IsKeyUp(Keys.Enter))
					{
						playState.ToggleChat();
					}
					if (base.Player != null && base.Player.Playing)
					{
						this.mAvatar = this.mPlayer.Avatar;
						if (this.mAvatar != null && !this.mAvatar.Dead && (currentState as PlayState).Level.CurrentScene != null)
						{
							if (!playState.IsInCutscene)
							{
								Game.Instance.IsMouseVisible = true;
							}
							Matrix matrix;
							GameStateManager.Instance.CurrentState.Scene.Camera.GetViewProjectionMatrix(iDataChannel, out matrix);
							if (this.mCursorPressedTarget != null && this.IsDown(KeyboardBindings.Move, ref inputState))
							{
								Vector2 vector = MagickaMath.WorldToScreenPosition(ref this.mCursorLockPosition, ref matrix);
								vector.X += this.mMouseOffset.X;
								vector.Y += this.mMouseOffset.Y;
								this.mLockedMousePos.X = (int)(vector.X + 0.5f);
								this.mLockedMousePos.Y = (int)(vector.Y + 0.5f);
								Mouse.SetPosition(this.mLockedMousePos.X, this.mLockedMousePos.Y);
							}
							Vector3 vector2;
							Segment mousePick = this.GetMousePick(ref matrix, ref screenSize, ref inputState.NewMouseState, out vector2);
							bool flag11 = this.IsDown(KeyboardBindings.Shift, ref inputState);
							bool flag12 = false;
							Magicka.GameLogic.Entities.Character character = null;
							Pickable pickable = null;
							Interactable interactable = null;
							if (this.mStillPressing)
							{
								if (this.mLockedTarget is Magicka.GameLogic.Entities.Character)
								{
									character = (this.mLockedTarget as Magicka.GameLogic.Entities.Character);
								}
								else if (this.mLockedTarget is Pickable)
								{
									pickable = (this.mLockedTarget as Pickable);
								}
								else if (this.mLockedTarget is Interactable)
								{
									interactable = (this.mLockedTarget as Interactable);
								}
							}
							if (!(this.mAvatar.Chanting | playState.IsInCutscene) && !flag11 && !this.mAvatar.Polymorphed)
							{
								if (!this.mStillPressing)
								{
									character = this.FindCharacter(ref mousePick, ref vector2);
									pickable = this.FindPickUp(ref mousePick, ref vector2);
									interactable = this.FindInteractable(ref mousePick);
									Vector3 vector3 = default(Vector3);
									if (pickable != null)
									{
										vector3 = pickable.Position;
									}
									else if (character != null)
									{
										vector3 = character.Position;
									}
									else if (interactable != null)
									{
										Matrix transform = interactable.Locator.Transform;
										vector3 = transform.Translation;
									}
									this.mLockedMousePos.X = inputState.NewMouseState.X;
									this.mLockedMousePos.Y = inputState.NewMouseState.Y;
									Vector2 vector4 = MagickaMath.WorldToScreenPosition(ref vector3, ref matrix);
									Vector2.Subtract(ref iPosition, ref vector4, out this.mMouseOffset);
								}
								Magicka.GameLogic.Entities.Character character2 = this.mAvatar.FindCharacter(false);
								Interactable interactable2 = this.mAvatar.FindInteractable(false);
								if (character != null && character.Dialog != 0 && (character.Faction & this.mAvatar.Faction) != Factions.NONE)
								{
									character.Highlight();
									flag12 = (character2 == character);
									iActive = flag12;
									this.mStillPressing = true;
									iCursor = Cursors.Talk;
								}
								else if (pickable != null)
								{
									pickable.Highlight();
									flag12 = (iActive = this.mAvatar.FindPickUp(false, pickable));
									iCursor = Cursors.PickUp;
									this.mStillPressing = true;
								}
								else if (interactable != null)
								{
									interactable.Highlight();
									flag12 = (iActive = (interactable2 == interactable));
									InteractType interactType = interactable.InteractType;
									if (interactType != InteractType.Pick_Up)
									{
										switch (interactType)
										{
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
									}
									else
									{
										iCursor = Cursors.PickUp;
									}
									this.mStillPressing = true;
								}
							}
							this.mAvatar.Player.NotifierButton.Hide();
							if (!(instance.AwaitingInput | instance.HoldoffInput))
							{
								if (this.mAvatar.NotifySpecialAbility)
								{
									this.mAvatar.Player.NotifierButton.Show(this.mAvatar.SpecialAbilityName, ButtonChar.None, iPosition);
								}
								else if (this.mAvatar.ChantingMagick)
								{
									string @string = LanguageManager.Instance.GetString(Magick.NAME_LOCALIZATION[(int)this.mAvatar.MagickType]);
									this.mAvatar.Player.NotifierButton.Show(@string, ButtonChar.SpaceBar, iPosition);
								}
								else if (!this.mAvatar.Chanting)
								{
									if (this.mAvatar.IsGripped)
									{
										string string2 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[12]);
										this.mAvatar.Player.NotifierButton.Show(string2, ButtonChar.SpaceBar, iPosition);
										if (this.mAvatar.Boosts > 0)
										{
											this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
										}
									}
									else if (!flag11 && !this.mAvatar.Polymorphed)
									{
										if (pickable != null)
										{
											string string3 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[1]);
											if (!(pickable is Item) || (((pickable as Item).WeaponClass != WeaponClass.Staff || !this.mAvatar.Equipment[1].Item.IsBound) && (!Defines.IsWeapon((pickable as Item).WeaponClass) || !this.mAvatar.Equipment[0].Item.IsBound)))
											{
												this.mAvatar.Player.NotifierButton.Show(string3, ButtonChar.None, iPosition);
											}
										}
										else if (interactable != null)
										{
											string string4 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int)interactable.InteractType]);
											this.mAvatar.Player.NotifierButton.Show(string4, ButtonChar.None, iPosition);
										}
										else if (BoostState.Instance.ShieldToBoost(this.mAvatar) != null || (this.mAvatar.IsSelfShielded && !this.mAvatar.IsSolidSelfShielded))
										{
											string string5 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[6]);
											this.mAvatar.Player.NotifierButton.Show(string5, ButtonChar.None, iPosition);
											if (this.mAvatar.Boosts > 0)
											{
												this.mAvatar.Player.NotifierButton.SetButtonIntensity(0.5f);
											}
										}
										else if (character != null && character.InteractText != InteractType.None && !AudioManager.Instance.Threat)
										{
											string string6 = LanguageManager.Instance.GetString(Defines.INTERACTSTRINGS[(int)character.InteractText]);
											this.mAvatar.Player.NotifierButton.Show(string6, ButtonChar.None, iPosition);
										}
									}
								}
							}
							this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
							if (instance3.IsInputLimited || instance3.IsPlayerInputLocked(base.Player.ID))
							{
								this.mAvatar.UpdateMouseDirection(Vector2.Zero, false);
							}
							else
							{
								this.mAvatar.Player.NotifierButton.Update(iDataChannel, iDeltaTime);
								Vector2 iValue = default(Vector2);
								if (this.mLockedTarget == null)
								{
									iValue.X = vector2.X - this.mAvatar.Position.X;
									iValue.Y = vector2.Z - this.mAvatar.Position.Z;
								}
								else
								{
									iValue.X = this.mCursorLockPosition.X - this.mAvatar.Position.X;
									iValue.Y = this.mCursorLockPosition.Z - this.mAvatar.Position.Z;
								}
								this.mAvatar.UpdateMouseDirection(iValue, this.mInverted);
								if (!this.IsDown(KeyboardBindings.Move, ref inputState))
								{
									this.mInteractMoveLock = false;
									this.mAvatar.MouseMoveStop();
									this.mCursorPressedTarget = null;
									this.mLockedTarget = null;
									this.mStillPressing = false;
								}
								if (!playState.IsInCutscene)
								{
									if (flag11)
									{
										this.mAvatar.ForceReleased();
										if (this.IsDown(KeyboardBindings.Cast, ref inputState))
										{
											this.mAvatar.AreaPressed();
										}
										else
										{
											this.mAvatar.AreaReleased();
										}
										if (this.IsPressed(KeyboardBindings.Move, ref inputState))
										{
											this.mAvatar.Attack();
										}
										else if (this.IsDown(KeyboardBindings.Move, ref inputState) & (this.mAvatar.WieldingGun | this.mAvatar.Equipment[0].Item.SpellCharged))
										{
											this.mAvatar.Attack();
										}
										else if (this.IsReleased(KeyboardBindings.Move, ref inputState))
										{
											this.mAvatar.AttackRelease();
										}
										iActive = true;
										iCursor = Cursors.Attack;
									}
									else
									{
										this.mAvatar.AreaReleased();
										if (this.IsDown(KeyboardBindings.Cast, ref inputState))
										{
											this.mAvatar.ForcePressed();
										}
										else
										{
											this.mAvatar.ForceReleased();
										}
										if (this.mAvatar.Attacking)
										{
											this.mAvatar.AttackRelease();
										}
										if (this.IsPressed(KeyboardBindings.Move, ref inputState))
										{
											if (!this.mAvatar.Chanting)
											{
												if (flag12)
												{
													if (pickable != null)
													{
														this.mAvatar.PickUp(pickable);
													}
													else if (character != null)
													{
														this.mAvatar.Action();
													}
													else if (interactable != null)
													{
														this.mAvatar.Interact();
													}
													this.mInteractMoveLock = true;
												}
												else
												{
													if (pickable != null)
													{
														this.mCursorPressedTarget = pickable;
														this.mCursorLockPosition = pickable.Position;
													}
													else if (character != null)
													{
														this.mCursorPressedTarget = character;
														this.mCursorLockPosition = character.Position;
													}
													else if (interactable != null)
													{
														this.mCursorPressedTarget = interactable;
														Matrix transform2 = interactable.Locator.Transform;
														this.mCursorLockPosition = transform2.Translation;
													}
													else
													{
														this.mCursorPressedTarget = null;
													}
													this.mLockedTarget = this.mCursorPressedTarget;
												}
											}
										}
										else if (this.IsDown(KeyboardBindings.Move, ref inputState) & !this.mInteractMoveLock)
										{
											this.mAvatar.MouseMove();
											if (!this.mAvatar.Chanting && this.mCursorPressedTarget != null && flag12)
											{
												if (this.mCursorPressedTarget is Magicka.GameLogic.Entities.Character)
												{
													this.mAvatar.Action();
												}
												else if (this.mCursorPressedTarget is Pickable)
												{
													this.mAvatar.PickUp(pickable);
													this.mStillPressing = false;
													this.mLockedTarget = null;
												}
												else if (this.mCursorPressedTarget is Interactable)
												{
													this.mAvatar.Interact();
													this.mStillPressing = false;
													this.mLockedTarget = null;
												}
												this.mCursorPressedTarget = null;
											}
										}
									}
								}
								if (this.IsPressed(KeyboardBindings.CastSelf, ref inputState))
								{
									this.mAvatar.Special();
								}
								else if (this.IsReleased(KeyboardBindings.CastSelf, ref inputState))
								{
									this.mAvatar.SpecialRelease();
								}
								if (this.IsPressed(KeyboardBindings.PrevMagick, ref inputState))
								{
									MagickType magickType = base.Player.IconRenderer.TomeMagick - 1;
									if (magickType == MagickType.NrOfMagicks | magickType < MagickType.None)
									{
										magickType = MagickType.Amalgameddon;
									}
									while (magickType != MagickType.None && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, magickType))
									{
										magickType--;
									}
									base.Player.IconRenderer.TomeMagick = magickType;
								}
								else if (this.IsPressed(KeyboardBindings.NextMagick, ref inputState))
								{
									MagickType magickType2 = base.Player.IconRenderer.TomeMagick + 1;
									while (magickType2 != MagickType.NrOfMagicks && !SpellManager.Instance.IsMagickAllowed(this.mPlayer, this.mAvatar.PlayState.GameType, magickType2))
									{
										magickType2++;
									}
									if (magickType2 == MagickType.NrOfMagicks)
									{
										magickType2 = MagickType.None;
									}
									base.Player.IconRenderer.TomeMagick = magickType2;
								}
								if (!NetworkChat.Instance.Active)
								{
									if (this.IsPressed(KeyboardBindings.Inventory, ref inputState))
									{
										this.mAvatar.CheckInventory();
									}
									if (this.IsPressed(KeyboardBindings.Boost, ref inputState))
									{
										this.mAvatar.Boost();
									}
									this.mAvatar.IsBlocking = this.IsDown(KeyboardBindings.Block, ref inputState);
									if (!this.mAvatar.Polymorphed)
									{
										if (this.IsPressed(KeyboardBindings.Water, ref inputState) && this.mHUD.IconCoolDown(Elements.Water))
										{
											this.mAvatar.ConjureWater();
											this.mHUD.CoolDown(Elements.Water);
										}
										if (this.IsPressed(KeyboardBindings.Lightning, ref inputState) && this.mHUD.IconCoolDown(Elements.Lightning))
										{
											this.mAvatar.ConjureLightning();
											this.mHUD.CoolDown(Elements.Lightning);
										}
										if (this.IsPressed(KeyboardBindings.Life, ref inputState) && this.mHUD.IconCoolDown(Elements.Life))
										{
											this.mAvatar.ConjureLife();
											this.mHUD.CoolDown(Elements.Life);
										}
										if (this.IsPressed(KeyboardBindings.Arcane, ref inputState) && this.mHUD.IconCoolDown(Elements.Arcane))
										{
											this.mAvatar.ConjureArcane();
											this.mHUD.CoolDown(Elements.Arcane);
										}
										if (this.IsPressed(KeyboardBindings.Shield, ref inputState) && this.mHUD.IconCoolDown(Elements.Shield))
										{
											this.mAvatar.ConjureShield();
											this.mHUD.CoolDown(Elements.Shield);
										}
										if (this.IsPressed(KeyboardBindings.Earth, ref inputState) && this.mHUD.IconCoolDown(Elements.Earth))
										{
											this.mAvatar.ConjureEarth();
											this.mHUD.CoolDown(Elements.Earth);
										}
										if (this.IsPressed(KeyboardBindings.Cold, ref inputState) && this.mHUD.IconCoolDown(Elements.Cold))
										{
											this.mAvatar.ConjureCold();
											this.mHUD.CoolDown(Elements.Cold);
										}
										if (this.IsPressed(KeyboardBindings.Fire, ref inputState) && this.mHUD.IconCoolDown(Elements.Fire))
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
					{
						Game.Instance.IsMouseVisible = false;
					}
				}
			}
			Game.Instance.SetCursor(iActive, iCursor);
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x0010E25C File Offset: 0x0010C45C
		public override void Invert(bool iInvert)
		{
			if (!this.mInverted)
			{
				if (iInvert)
				{
					this.mInverted = true;
					Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[0], ref KeyboardMouseController.mKeyboardBindings[7]);
					Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[1], ref KeyboardMouseController.mKeyboardBindings[4]);
					Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[2], ref KeyboardMouseController.mKeyboardBindings[3]);
					Helper.Swap<KeyboardMouseController.Binding>(ref KeyboardMouseController.mKeyboardBindings[5], ref KeyboardMouseController.mKeyboardBindings[6]);
					return;
				}
			}
			else if (!iInvert)
			{
				SaveManager.Instance.KeyBindings.CopyTo(KeyboardMouseController.mKeyboardBindings, 0);
				this.mInverted = false;
			}
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0010E30C File Offset: 0x0010C50C
		protected char GetKeyboardInput(KeyboardState iNewKeyboardState)
		{
			Keys[] pressedKeys = iNewKeyboardState.GetPressedKeys();
			char c = '\0';
			for (int i = 0; i < pressedKeys.Length; i++)
			{
				Keys keys = pressedKeys[i];
				if ((ushort)keys >= 96 & (ushort)keys <= 105)
				{
					keys = (pressedKeys[i] = keys - 96 + 48);
				}
				char c2 = (char)pressedKeys[i];
				if (c2 >= '0' & c2 <= 'Z')
				{
					c = c2;
					break;
				}
				if (keys != Keys.Back)
				{
					if (keys != Keys.Space)
					{
						if (keys != Keys.Separator)
						{
							if (keys != Keys.Decimal)
							{
								if (keys == Keys.Divide)
								{
								}
							}
						}
					}
				}
			}
			if (this.mLastChar != c)
			{
				this.mLastChar = c;
			}
			else
			{
				c = '\0';
			}
			if (!(iNewKeyboardState.IsKeyDown(Keys.LeftShift) | iNewKeyboardState.IsKeyDown(Keys.RightShift)))
			{
				return char.ToLowerInvariant(c);
			}
			return c;
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0010E3E4 File Offset: 0x0010C5E4
		protected Segment GetMousePick(ref Matrix iViewProjectionMatrix, ref Point iScreenSize, ref MouseState iMouseState, out Vector3 oPlanePosition)
		{
			Camera camera = GameStateManager.Instance.CurrentState.Scene.Camera;
			Matrix matrix;
			Matrix.Invert(ref iViewProjectionMatrix, out matrix);
			Vector4 vector = new Vector4(-1f + 2f * (float)iMouseState.X / (float)iScreenSize.X, 1f - 2f * (float)iMouseState.Y / (float)iScreenSize.Y, 1f, 1f);
			Vector4.Transform(ref vector, ref matrix, out vector);
			Vector3 vector2 = default(Vector3);
			vector2.X = vector.X;
			vector2.Y = vector.Y;
			vector2.Z = vector.Z;
			Vector3.Divide(ref vector2, vector.W, out vector2);
			Vector3 position = base.Player.Avatar.Position;
			Microsoft.Xna.Framework.Plane plane = new Microsoft.Xna.Framework.Plane(Vector3.Up, -position.Y);
			Vector3 position2 = camera.Position;
			Vector3.Subtract(ref vector2, ref position2, out position2);
			Microsoft.Xna.Framework.Ray ray = new Microsoft.Xna.Framework.Ray(camera.Position, position2);
			float? num;
			ray.Intersects(ref plane, out num);
			Vector3 position3 = ray.Position;
			if (num != null)
			{
				Vector3 direction = ray.Direction;
				Vector3.Multiply(ref direction, num.Value, out direction);
				Vector3.Add(ref position3, ref direction, out position3);
			}
			oPlanePosition = position3;
			Vector3 position4 = camera.Position;
			Vector3 delta;
			Vector3.Subtract(ref vector2, ref position4, out delta);
			return new Segment(position4, delta);
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0010E55C File Offset: 0x0010C75C
		protected Magicka.GameLogic.Entities.Character FindCharacter(Avatar iAvatar)
		{
			if (AudioManager.Instance.Threat)
			{
				return null;
			}
			float num = float.MaxValue;
			Magicka.GameLogic.Entities.Character result = null;
			List<Entity> entities = iAvatar.PlayState.EntityManager.GetEntities(iAvatar.Position, 3f, false);
			entities.Remove(iAvatar);
			for (int i = 0; i < entities.Count; i++)
			{
				Magicka.GameLogic.Entities.Character character = entities[i] as Magicka.GameLogic.Entities.Character;
				if (character != null && character.InteractText != InteractType.None)
				{
					Vector3 position = iAvatar.Position;
					position.Y = 0f;
					Vector3 position2 = character.Position;
					position2.Y = 0f;
					Vector3.Subtract(ref position2, ref position, out position2);
					float num2 = position2.LengthSquared();
					if (num2 < num)
					{
						num = num2;
						result = character;
					}
				}
			}
			iAvatar.PlayState.EntityManager.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x0010E62C File Offset: 0x0010C82C
		protected Magicka.GameLogic.Entities.Character FindCharacter(ref Segment iSegment, ref Vector3 iPlanePosition)
		{
			if (AudioManager.Instance.Threat)
			{
				return null;
			}
			List<Entity> entities = this.mAvatar.PlayState.EntityManager.GetEntities(iPlanePosition, 6f, false);
			entities.Remove(this.mAvatar);
			for (int i = 0; i < entities.Count; i++)
			{
				Magicka.GameLogic.Entities.Character character = entities[i] as Magicka.GameLogic.Entities.Character;
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (character != null && character.Body.CollisionSkin.SegmentIntersect(out num, out vector, out vector2, iSegment))
				{
					InteractType interactText = character.InteractText;
					if (interactText != InteractType.None)
					{
						this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
						return character;
					}
				}
			}
			this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
			return null;
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x0010E6F0 File Offset: 0x0010C8F0
		protected Pickable FindPickUp(Avatar iAvatar)
		{
			float maxValue = float.MaxValue;
			Pickable result = null;
			List<Entity> entities = iAvatar.PlayState.EntityManager.GetEntities(iAvatar.Position, 2.5f, false);
			for (int i = 0; i < entities.Count; i++)
			{
				Pickable pickable = entities[i] as Pickable;
				if (pickable != null && pickable.IsPickable)
				{
					Vector3 position = pickable.Position;
					Vector3 position2 = pickable.Position;
					float num;
					Vector3.DistanceSquared(ref position2, ref position, out num);
					if (num < maxValue)
					{
						num = maxValue;
						result = pickable;
					}
				}
			}
			iAvatar.PlayState.EntityManager.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x0010E788 File Offset: 0x0010C988
		protected Pickable FindPickUp(ref Segment iSegment, ref Vector3 iPlanePosition)
		{
			Pickable result = null;
			List<Entity> entities = this.mAvatar.PlayState.EntityManager.GetEntities(iPlanePosition, 6f, false);
			entities.Remove(this.mAvatar);
			if (this.mAvatar.Equipment[0].Item != null)
			{
				entities.Remove(this.mAvatar.Equipment[0].Item);
			}
			if (this.mAvatar.Equipment[1].Item != null)
			{
				entities.Remove(this.mAvatar.Equipment[1].Item);
			}
			for (int i = 0; i < entities.Count; i++)
			{
				Pickable pickable = entities[i] as Pickable;
				float num;
				Vector3 vector;
				Vector3 vector2;
				if (pickable != null && pickable.IsPickable && pickable.Body.CollisionSkin.SegmentIntersect(out num, out vector, out vector2, iSegment))
				{
					this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
					return pickable;
				}
			}
			this.mAvatar.PlayState.EntityManager.ReturnEntityList(entities);
			return result;
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0010E898 File Offset: 0x0010CA98
		protected Interactable FindInteractable(Avatar iAvatar)
		{
			if (AudioManager.Instance.Threat)
			{
				return null;
			}
			SortedList<int, Trigger> triggers = iAvatar.PlayState.Level.CurrentScene.Triggers;
			foreach (Trigger trigger in triggers.Values)
			{
				Interactable interactable = trigger as Interactable;
				if (interactable != null && interactable.Enabled)
				{
					Vector3 position = iAvatar.Position;
					position.Y = 0f;
					Locator locator = interactable.Locator;
					Vector3 translation = locator.Transform.Translation;
					translation.Y = 0f;
					float num;
					Vector3.DistanceSquared(ref translation, ref position, out num);
					float radius = iAvatar.Radius;
					if (num <= radius * radius)
					{
						return interactable;
					}
					if (num <= locator.Radius * locator.Radius)
					{
						return interactable;
					}
				}
			}
			return null;
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0010E994 File Offset: 0x0010CB94
		protected Interactable FindInteractable(ref Segment iSegment)
		{
			SortedList<int, Trigger> triggers = this.mAvatar.PlayState.Level.CurrentScene.Triggers;
			Interactable interactable = null;
			float num = float.MaxValue;
			for (int i = 0; i < triggers.Count; i++)
			{
				Interactable interactable2 = triggers.Values[i] as Interactable;
				if (interactable2 != null && interactable2.Enabled)
				{
					Locator locator = interactable2.Locator;
					float num2 = Distance.PointSegmentDistanceSq(locator.Transform.Translation, iSegment);
					if (num2 <= locator.Radius * locator.Radius && num2 <= num)
					{
						num = num2;
						interactable = interactable2;
					}
				}
			}
			if (interactable != null && AudioManager.Instance.Threat && interactable.InteractType == InteractType.Talk)
			{
				return null;
			}
			return interactable;
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x0010EA50 File Offset: 0x0010CC50
		private bool IsValidText(char iChar)
		{
			return char.IsLetterOrDigit(iChar) | iChar == '\b' | iChar == ' ' | char.IsPunctuation(iChar) | char.IsSymbol(iChar);
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x0010EA74 File Offset: 0x0010CC74
		private void OnKeyDown(KeyData iData)
		{
			if (KeyboardMouseController.mCatchKeyActive)
			{
				KeyboardMouseController.mCatchKeyActive = !KeyboardMouseController.IsValidKey(iData.Key, KeyboardMouseController.mCatchKeyIndex);
				if ((iData.Key & Keys.Escape) == Keys.Escape)
				{
					KeyboardMouseController.mCatchKeyActive = false;
					return;
				}
			}
			else
			{
				if (PdxLoginWindow.Instance.Visible)
				{
					PdxLoginWindow.Instance.OnKeyDown(iData);
					return;
				}
				if (PdxWidget.Instance.Visible)
				{
					PdxLoginWindow.Instance.OnKeyDown(iData);
					return;
				}
				if (DialogManager.Instance.MessageBoxActive)
				{
					if (iData.Key == Keys.Enter)
					{
						DialogManager.Instance.ControllerSelect(this);
						return;
					}
					if (iData.Key == Keys.Up)
					{
						DialogManager.Instance.ControllerMove(this, ControllerDirection.Up);
						return;
					}
					if (iData.Key == Keys.Down)
					{
						DialogManager.Instance.ControllerMove(this, ControllerDirection.Down);
						return;
					}
					if (iData.Key == Keys.Left)
					{
						DialogManager.Instance.ControllerMove(this, ControllerDirection.Left);
						return;
					}
					if (iData.Key == Keys.Right)
					{
						DialogManager.Instance.ControllerMove(this, ControllerDirection.Right);
						return;
					}
				}
				else if ((iData.Key & Keys.Escape) == Keys.None && GameStateManager.Instance.CurrentState is PlayState && !(GameStateManager.Instance.CurrentState as PlayState).IsPaused)
				{
					if ((GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
					{
						(GameStateManager.Instance.CurrentState as PlayState).SkipCutscene();
						return;
					}
					TutorialManager.Instance.RemoveDialogHint();
					return;
				}
				else if ((Tome.Instance.CurrentState is Tome.OpenState & NetworkManager.Instance.State != NetworkState.Offline) && iData.Key == Keys.Enter)
				{
					NetworkChat.Instance.SendMessage();
				}
			}
		}

		// Token: 0x0600252F RID: 9519 RVA: 0x0010EC14 File Offset: 0x0010CE14
		private void OnKeyPress(char iChar, KeyModifiers iModifiers)
		{
			bool flag = this.IsValidText(iChar);
			if (flag)
			{
				if (PdxLoginWindow.Instance.Visible)
				{
					PdxLoginWindow.Instance.OnKeyPress(iChar);
					return;
				}
				if (PdxWidget.Instance.Visible)
				{
					PdxLoginWindow.Instance.OnKeyPress(iChar);
					return;
				}
				if (DialogManager.Instance.MessageBoxActive)
				{
					DialogManager.Instance.ControllerType(this, iChar);
					return;
				}
				if (Tome.Instance.CurrentState is Tome.OpenState & NetworkManager.Instance.State != NetworkState.Offline)
				{
					NetworkChat.Instance.TakeInput(this, iChar);
				}
			}
		}

		// Token: 0x06002530 RID: 9520 RVA: 0x0010ECA6 File Offset: 0x0010CEA6
		private void OnKeyUp(KeyData iData)
		{
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x0010ECA8 File Offset: 0x0010CEA8
		private static ControllerDirection GetDirection(ref KeyboardState iState, Keys iUp, Keys iDown, Keys iLeft, Keys iRight)
		{
			ControllerDirection controllerDirection = ControllerDirection.Center;
			if (iState.IsKeyDown(iUp))
			{
				controllerDirection |= ControllerDirection.Up;
			}
			if (iState.IsKeyDown(iDown))
			{
				controllerDirection |= ControllerDirection.Down;
			}
			if (iState.IsKeyDown(iLeft))
			{
				controllerDirection |= ControllerDirection.Left;
			}
			if (iState.IsKeyDown(iRight))
			{
				controllerDirection |= ControllerDirection.Right;
			}
			if ((byte)(controllerDirection & (ControllerDirection.Up | ControllerDirection.Down)) == 10)
			{
				controllerDirection &= ~(ControllerDirection.Up | ControllerDirection.Down);
			}
			if ((byte)(controllerDirection & (ControllerDirection.Right | ControllerDirection.Left)) == 5)
			{
				controllerDirection &= ~(ControllerDirection.Right | ControllerDirection.Left);
			}
			return controllerDirection;
		}

		// Token: 0x06002532 RID: 9522 RVA: 0x0010ED13 File Offset: 0x0010CF13
		public override void Rumble(float iLeft, float iRight)
		{
		}

		// Token: 0x06002533 RID: 9523 RVA: 0x0010ED15 File Offset: 0x0010CF15
		public override float LeftRumble()
		{
			return 0f;
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x0010ED1C File Offset: 0x0010CF1C
		public override float RightRumble()
		{
			return 0f;
		}

		// Token: 0x06002535 RID: 9525 RVA: 0x0010ED23 File Offset: 0x0010CF23
		public override void Clear()
		{
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x0010ED25 File Offset: 0x0010CF25
		public void SetKey(KeyboardBindings iIndex, Keys iKey)
		{
			KeyboardMouseController.mKeyboardBindings[(int)iIndex] = new KeyboardMouseController.Binding(iKey);
		}

		// Token: 0x06002537 RID: 9527 RVA: 0x0010ED3D File Offset: 0x0010CF3D
		internal KeyboardMouseController.Binding GetBinding(KeyboardBindings iIndex)
		{
			return KeyboardMouseController.mKeyboardBindings[(int)iIndex];
		}

		// Token: 0x06002538 RID: 9528 RVA: 0x0010ED50 File Offset: 0x0010CF50
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
			KeyboardMouseController.mKeyboardBindings[16] = new KeyboardMouseController.Binding(KeyboardMouseController.MouseButton.ScrollDown);
			KeyboardMouseController.mKeyboardBindings.CopyTo(SaveManager.Instance.KeyBindings, 0);
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x0010EF04 File Offset: 0x0010D104
		public static bool IsValidKey(Keys iKey, KeyboardBindings iFuction)
		{
			if (iKey == Keys.Escape | iKey == Keys.LeftWindows | iKey == Keys.RightWindows | iKey == Keys.Apps)
			{
				return false;
			}
			for (int i = 0; i < KeyboardMouseController.mKeyboardBindings.Length; i++)
			{
				if (!KeyboardMouseController.mKeyboardBindings[i].IsMouse && (Keys)KeyboardMouseController.mKeyboardBindings[i].Button == iKey)
				{
					KeyboardMouseController.mKeyboardBindings[i] = default(KeyboardMouseController.Binding);
				}
			}
			KeyboardMouseController.mKeyboardBindings[(int)iFuction] = new KeyboardMouseController.Binding(iKey);
			return true;
		}

		// Token: 0x0600253A RID: 9530 RVA: 0x0010EF8C File Offset: 0x0010D18C
		public static bool IsValidKey(KeyboardMouseController.MouseButton iButton, KeyboardBindings iFuction)
		{
			for (int i = 0; i < KeyboardMouseController.mKeyboardBindings.Length; i++)
			{
				if (KeyboardMouseController.mKeyboardBindings[i].IsMouse && (KeyboardMouseController.MouseButton)KeyboardMouseController.mKeyboardBindings[i].Button == iButton)
				{
					KeyboardMouseController.mKeyboardBindings[i] = default(KeyboardMouseController.Binding);
				}
			}
			KeyboardMouseController.mKeyboardBindings[(int)iFuction] = new KeyboardMouseController.Binding(iButton);
			return true;
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x0010EFF8 File Offset: 0x0010D1F8
		private bool IsDown(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
		{
			if (!KeyboardMouseController.mKeyboardBindings[(int)iFuction].IsMouse)
			{
				return iState.NewKeyboardState.IsKeyDown((Keys)KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button);
			}
			switch (KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button)
			{
			case 1:
				return iState.NewMouseState.LeftButton == ButtonState.Pressed;
			case 2:
				return iState.NewMouseState.MiddleButton == ButtonState.Pressed;
			case 3:
				return iState.NewMouseState.RightButton == ButtonState.Pressed;
			case 4:
				return iState.NewMouseState.XButton1 == ButtonState.Pressed;
			case 5:
				return iState.NewMouseState.XButton2 == ButtonState.Pressed;
			default:
				return false;
			}
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x0010F0B0 File Offset: 0x0010D2B0
		private bool IsPressed(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
		{
			if (!KeyboardMouseController.mKeyboardBindings[(int)iFuction].IsMouse)
			{
				return iState.NewKeyboardState.IsKeyDown((Keys)KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button) && iState.OldKeyboardState.IsKeyUp((Keys)KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button);
			}
			switch (KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button)
			{
			case 1:
				return iState.NewMouseState.LeftButton == ButtonState.Pressed && iState.OldMouseState.LeftButton == ButtonState.Released;
			case 2:
				return iState.NewMouseState.MiddleButton == ButtonState.Pressed && iState.OldMouseState.MiddleButton == ButtonState.Released;
			case 3:
				return iState.NewMouseState.RightButton == ButtonState.Pressed && iState.OldMouseState.RightButton == ButtonState.Released;
			case 4:
				return iState.NewMouseState.XButton1 == ButtonState.Pressed && iState.OldMouseState.XButton1 == ButtonState.Released;
			case 5:
				return iState.NewMouseState.XButton2 == ButtonState.Pressed && iState.OldMouseState.XButton2 == ButtonState.Released;
			case 6:
				return iState.NewMouseState.ScrollWheelValue > iState.OldMouseState.ScrollWheelValue;
			case 7:
				return iState.NewMouseState.ScrollWheelValue < iState.OldMouseState.ScrollWheelValue;
			default:
				return false;
			}
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x0010F218 File Offset: 0x0010D418
		private bool IsReleased(KeyboardBindings iFuction, ref KeyboardMouseController.InputState iState)
		{
			if (!KeyboardMouseController.mKeyboardBindings[(int)iFuction].IsMouse)
			{
				return iState.NewKeyboardState.IsKeyUp((Keys)KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button) && iState.OldKeyboardState.IsKeyDown((Keys)KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button);
			}
			switch (KeyboardMouseController.mKeyboardBindings[(int)iFuction].Button)
			{
			case 1:
				return iState.NewMouseState.LeftButton == ButtonState.Released && iState.OldMouseState.LeftButton == ButtonState.Pressed;
			case 2:
				return iState.NewMouseState.MiddleButton == ButtonState.Released && iState.OldMouseState.MiddleButton == ButtonState.Pressed;
			case 3:
				return iState.NewMouseState.RightButton == ButtonState.Released && iState.OldMouseState.RightButton == ButtonState.Pressed;
			case 4:
				return iState.NewMouseState.XButton1 == ButtonState.Released && iState.OldMouseState.XButton1 == ButtonState.Pressed;
			case 5:
				return iState.NewMouseState.XButton2 == ButtonState.Released && iState.OldMouseState.XButton2 == ButtonState.Pressed;
			default:
				return false;
			}
		}

		// Token: 0x0600253E RID: 9534 RVA: 0x0010F33E File Offset: 0x0010D53E
		private bool IsPressed(Keys iKey, ref KeyboardMouseController.InputState iState)
		{
			return iState.NewKeyboardState.IsKeyDown(iKey) && iState.OldKeyboardState.IsKeyUp(iKey);
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x0010F35C File Offset: 0x0010D55C
		private bool IsPressed(KeyboardMouseController.MouseButton iButton, ref KeyboardMouseController.InputState iState)
		{
			switch (iButton)
			{
			case KeyboardMouseController.MouseButton.Left:
				return iState.NewMouseState.LeftButton == ButtonState.Pressed && iState.OldMouseState.LeftButton == ButtonState.Released;
			case KeyboardMouseController.MouseButton.Middle:
				return iState.NewMouseState.MiddleButton == ButtonState.Pressed && iState.OldMouseState.MiddleButton == ButtonState.Released;
			case KeyboardMouseController.MouseButton.Right:
				return iState.NewMouseState.RightButton == ButtonState.Pressed && iState.OldMouseState.RightButton == ButtonState.Released;
			case KeyboardMouseController.MouseButton.X1:
				return iState.NewMouseState.XButton1 == ButtonState.Pressed && iState.OldMouseState.XButton1 == ButtonState.Released;
			case KeyboardMouseController.MouseButton.X2:
				return iState.NewMouseState.XButton2 == ButtonState.Pressed && iState.OldMouseState.XButton2 == ButtonState.Released;
			case KeyboardMouseController.MouseButton.ScrollUp:
				return iState.NewMouseState.ScrollWheelValue > iState.OldMouseState.ScrollWheelValue;
			case KeyboardMouseController.MouseButton.ScrollDown:
				return iState.NewMouseState.ScrollWheelValue < iState.OldMouseState.ScrollWheelValue;
			default:
				return false;
			}
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x0010F464 File Offset: 0x0010D664
		public static string KeyToString(KeyboardBindings iKey)
		{
			KeyboardMouseController.Binding binding = KeyboardMouseController.mKeyboardBindings[(int)iKey];
			if (binding.Button == 0)
			{
				return " ";
			}
			if (binding.IsMouse)
			{
				return LanguageManager.Instance.GetString(KeyboardMouseController.MOUSE_BUTTONS[(int)binding.Button]);
			}
			Keys button = (Keys)binding.Button;
			if ((button >= Keys.D0 && button <= Keys.Divide) || (button >= Keys.OemSemicolon && button <= Keys.OemBackslash))
			{
				char c = (char)InputMessageFilter.MapVirtualKey((uint)button, 2U);
				return char.ToUpper(c).ToString();
			}
			return button.ToString();
		}

		// Token: 0x04002882 RID: 10370
		private static readonly int[] MOUSE_BUTTONS = new int[]
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

		// Token: 0x04002883 RID: 10371
		private static readonly float ONEOVERSQRT2 = 1f / (float)Math.Sqrt(2.0);

		// Token: 0x04002884 RID: 10372
		private static readonly float[] LOOKUP_ANGLES = new float[]
		{
			0f,
			0.7853982f,
			1.5707964f,
			2.3561945f,
			3.1415927f,
			3.926991f,
			4.712389f,
			5.4977875f,
			6.2831855f
		};

		// Token: 0x04002885 RID: 10373
		internal static KeyboardMouseController.Binding[] mKeyboardBindings = new KeyboardMouseController.Binding[17];

		// Token: 0x04002886 RID: 10374
		private KeyboardState mLastKeyboardState;

		// Token: 0x04002887 RID: 10375
		private MouseState mLastMouseState;

		// Token: 0x04002889 RID: 10377
		private char mLastChar;

		// Token: 0x0400288A RID: 10378
		private float[] mFadeTimers;

		// Token: 0x0400288B RID: 10379
		private float[] mMoveTimers;

		// Token: 0x0400288C RID: 10380
		private bool mInteractMoveLock;

		// Token: 0x0400288D RID: 10381
		private bool mIsGameActive;

		// Token: 0x0400288E RID: 10382
		private object mCursorPressedTarget;

		// Token: 0x0400288F RID: 10383
		private object mLockedTarget;

		// Token: 0x04002890 RID: 10384
		private bool mStillPressing;

		// Token: 0x04002891 RID: 10385
		private Point mLockedMousePos;

		// Token: 0x04002892 RID: 10386
		private Vector2 mMouseOffset;

		// Token: 0x04002893 RID: 10387
		private KeyboardHUD mHUD = KeyboardHUD.Instance;

		// Token: 0x04002894 RID: 10388
		private Vector3 mCursorLockPosition;

		// Token: 0x04002895 RID: 10389
		public static bool mCatchKeyActive;

		// Token: 0x04002896 RID: 10390
		public static KeyboardBindings mCatchKeyIndex;

		// Token: 0x020004E4 RID: 1252
		private struct InputState
		{
			// Token: 0x04002897 RID: 10391
			public KeyboardState NewKeyboardState;

			// Token: 0x04002898 RID: 10392
			public KeyboardState OldKeyboardState;

			// Token: 0x04002899 RID: 10393
			public MouseState NewMouseState;

			// Token: 0x0400289A RID: 10394
			public MouseState OldMouseState;
		}

		// Token: 0x020004E5 RID: 1253
		internal enum MouseButton
		{
			// Token: 0x0400289C RID: 10396
			Invalid,
			// Token: 0x0400289D RID: 10397
			Left,
			// Token: 0x0400289E RID: 10398
			Middle,
			// Token: 0x0400289F RID: 10399
			Right,
			// Token: 0x040028A0 RID: 10400
			X1,
			// Token: 0x040028A1 RID: 10401
			X2,
			// Token: 0x040028A2 RID: 10402
			ScrollUp,
			// Token: 0x040028A3 RID: 10403
			ScrollDown
		}

		// Token: 0x020004E6 RID: 1254
		internal struct Binding
		{
			// Token: 0x06002541 RID: 9537 RVA: 0x0010F4F7 File Offset: 0x0010D6F7
			public Binding(Keys iKey)
			{
				this.IsMouse = false;
				this.Button = (byte)iKey;
			}

			// Token: 0x06002542 RID: 9538 RVA: 0x0010F508 File Offset: 0x0010D708
			public Binding(KeyboardMouseController.MouseButton iButton)
			{
				this.IsMouse = true;
				this.Button = (byte)iButton;
			}

			// Token: 0x040028A4 RID: 10404
			internal bool IsMouse;

			// Token: 0x040028A5 RID: 10405
			internal byte Button;
		}
	}
}
