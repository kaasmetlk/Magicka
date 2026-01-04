// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.DialogManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Network;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI;

internal sealed class DialogManager
{
  public const int MAXDIALOGS = 8;
  public const float INPUT_COOLDOWN = 0.2f;
  private static DialogManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private TextBox[] mTextBoxes;
  private DialogCollection mDialogs;
  private Interact[] mActiveInteracts;
  private bool mAwaitingInput;
  private float mHoldoffInputTimer;
  private List<TextBox> mAdditionalTextBoxes = new List<TextBox>(10);
  private List<MessageBox> mMessageBoxes = new List<MessageBox>(10);
  private Text mSubtitles;
  private float mSubtitleHeight;
  private bool mDrawSubtitles;
  private DialogManager.RenderData[] mRenderData;
  private bool mDrawCutsceneText;
  private int mCutsceneInteract;
  private CutsceneText mCutsceneText;

  public static DialogManager Instance
  {
    get
    {
      if (DialogManager.mSingelton == null)
      {
        lock (DialogManager.mSingeltonLock)
        {
          if (DialogManager.mSingelton == null)
            DialogManager.mSingelton = new DialogManager();
        }
      }
      return DialogManager.mSingelton;
    }
  }

  private DialogManager()
  {
    this.mTextBoxes = new TextBox[8];
    this.mActiveInteracts = new Interact[8];
    for (int index = 0; index < 8; ++index)
      this.mTextBoxes[index] = new TextBox();
    this.mCutsceneText = new CutsceneText();
    this.mSubtitles = new Text(1024 /*0x0400*/, FontManager.Instance.GetFont(MagickaFont.Maiandra14), TextAlign.Center, false);
    this.mSubtitles.DrawShadows = true;
    this.mSubtitles.ShadowsOffset = new Vector2(1f);
    this.mSubtitles.ShadowAlpha = 0.8f;
    GUIBasicEffect guiBasicEffect;
    lock (Magicka.Game.Instance.GraphicsDevice)
      guiBasicEffect = new GUIBasicEffect(Magicka.Game.Instance.GraphicsDevice, (EffectPool) null);
    this.mRenderData = new DialogManager.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      DialogManager.RenderData renderData = new DialogManager.RenderData();
      this.mRenderData[index] = renderData;
      renderData.Effect = guiBasicEffect;
      renderData.Text = this.mSubtitles;
    }
  }

  public void SetDialogs(DialogCollection iDialogs)
  {
    this.mDialogs = iDialogs;
    for (int index = 0; index < 8; ++index)
      this.mActiveInteracts[index] = (Interact) null;
  }

  internal void StartDialog(int iDialog, Vector2 iScreePosition, Controller iInteractor)
  {
    if (this.DialogActive(iDialog))
      return;
    Interact interact = this.mDialogs[iDialog].Pop();
    if (interact == null)
      return;
    int index = 0;
    while (index < 8 && (this.mActiveInteracts[index] != null || this.mTextBoxes[index].Visible))
      ++index;
    if (index == 8)
      index = 0;
    this.mActiveInteracts[index] = interact;
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
    {
      this.mDrawCutsceneText = true;
      this.mActiveInteracts[index].Advance((TextBox) this.mCutsceneText, iScreePosition, iInteractor);
      this.mCutsceneInteract = index;
    }
    else
    {
      this.mActiveInteracts[index].Advance(this.mTextBoxes[index], iScreePosition, iInteractor);
      this.mDrawCutsceneText = false;
    }
    this.mAwaitingInput = false;
    this.mHoldoffInputTimer = 0.2f;
    if (!interact.LockGame)
      return;
    PhysicsManager.Instance.Freeze();
    ControlManager.Instance.LimitInput((object) this);
  }

  internal void StartDialog(int iDialog, Vector3 iWorldPosition, Controller iInteractor)
  {
    if (this.DialogActive(iDialog))
      return;
    Interact interact = this.mDialogs[iDialog].Pop();
    if (interact == null)
      return;
    int index = 0;
    while (index < 8 && (this.mActiveInteracts[index] != null || this.mTextBoxes[index].Visible))
      ++index;
    if (index == 8)
      index = 0;
    this.mActiveInteracts[index] = interact;
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
    {
      this.mDrawCutsceneText = true;
      this.mActiveInteracts[index].Advance((TextBox) this.mCutsceneText, iWorldPosition, iInteractor);
      this.mCutsceneInteract = index;
    }
    else
    {
      this.mActiveInteracts[index].Advance(this.mTextBoxes[index], iWorldPosition, iInteractor);
      this.mDrawCutsceneText = false;
    }
    this.mAwaitingInput = false;
    this.mHoldoffInputTimer = 0.2f;
    if (!interact.LockGame)
      return;
    PhysicsManager.Instance.Freeze();
    ControlManager.Instance.LimitInput((object) this);
  }

  public void StartDialog(int iDialog, Entity iOwner, Controller iInteractor)
  {
    if (this.DialogActive(iDialog))
      return;
    Interact interact = !this.mDialogs.ContainsKey(iDialog) ? (Interact) null : this.mDialogs[iDialog].Pop();
    if (interact == null)
      return;
    int index = 0;
    while (index < 8 && (this.mActiveInteracts[index] != null || this.mTextBoxes[index].Visible))
      ++index;
    if (index == 8)
      index = 0;
    this.mActiveInteracts[index] = interact;
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
    {
      this.mDrawCutsceneText = true;
      this.mActiveInteracts[index].Advance((TextBox) this.mCutsceneText, iOwner, iInteractor);
      this.mCutsceneInteract = index;
    }
    else
    {
      this.mActiveInteracts[index].Advance(this.mTextBoxes[index], iOwner, iInteractor);
      this.mDrawCutsceneText = false;
    }
    this.mAwaitingInput = false;
    this.mHoldoffInputTimer = 0.2f;
    if (!interact.LockGame)
      return;
    PhysicsManager.Instance.Freeze();
    ControlManager.Instance.LimitInput((object) this);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime, ref Matrix iViewProjectionMatrix)
  {
    this.mHoldoffInputTimer -= iDeltaTime;
    for (int index = 0; index < 8; ++index)
    {
      this.mTextBoxes[index].Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
      if (this.mTextBoxes[index].Visible && this.mTextBoxes[index].Owner != null && this.mTextBoxes[index].Owner.Dead)
      {
        if (this.mActiveInteracts[index] != null)
        {
          this.mActiveInteracts[index].End();
          this.mActiveInteracts[index] = (Interact) null;
        }
        this.mTextBoxes[index].Hide();
      }
    }
    for (int index = 0; index < this.mAdditionalTextBoxes.Count; ++index)
    {
      this.mAdditionalTextBoxes[index].Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
      if (!this.mAdditionalTextBoxes[index].Visible)
      {
        this.mAdditionalTextBoxes.RemoveAt(index);
        --index;
      }
    }
    for (int index = 0; index < this.mMessageBoxes.Count; ++index)
    {
      this.mMessageBoxes[index].Update(iDataChannel, iDeltaTime);
      if (this.mMessageBoxes[index].Dead)
      {
        this.mMessageBoxes.RemoveAt(index);
        --index;
      }
    }
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene && this.mDrawCutsceneText)
      this.mCutsceneText.Update(iDeltaTime, iDataChannel, ref iViewProjectionMatrix);
    if (!this.mDrawSubtitles)
      return;
    DialogManager.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.SubtitleHeight = this.mSubtitleHeight;
    GameStateManager.Instance.CurrentState.Scene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public void Advance(Controller iSender)
  {
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene || (double) this.mHoldoffInputTimer > 0.0)
      return;
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Interactor == iSender)
      {
        if (NetworkManager.Instance.State != NetworkState.Offline)
          NetworkManager.Instance.Interface.SendMessage<DialogAdvanceMessage>(ref new DialogAdvanceMessage()
          {
            Interact = this.mActiveInteracts[index].Current
          });
        if (this.mTextBoxes[index].Animating)
          this.mTextBoxes[index].FinishAnimation(true);
        else if (this.mActiveInteracts[index].Advance(this.mTextBoxes[index], iSender))
        {
          this.mDialogs.DialogFinished(this.mActiveInteracts[index].Parent.ID);
          this.mAwaitingInput = false;
          this.mHoldoffInputTimer = 0.2f;
          if (this.mActiveInteracts[index].LockGame)
          {
            PhysicsManager.Instance.UnFreeze();
            ControlManager.Instance.UnlimitInput((object) this);
          }
          this.mActiveInteracts[index] = (Interact) null;
        }
      }
    }
  }

  public void NetworkAdvance(int iInteract)
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Current == iInteract)
      {
        if (this.mTextBoxes[index].Animating)
          this.mTextBoxes[index].FinishAnimation(true);
        else if (this.mActiveInteracts[index].Advance(this.mTextBoxes[index], (Controller) null))
        {
          this.mDialogs.DialogFinished(this.mActiveInteracts[index].Parent.ID);
          this.mAwaitingInput = false;
          this.mHoldoffInputTimer = 0.2f;
          if (this.mActiveInteracts[index].LockGame)
          {
            PhysicsManager.Instance.UnFreeze();
            ControlManager.Instance.UnlimitInput((object) this);
          }
          this.mActiveInteracts[index] = (Interact) null;
        }
      }
    }
  }

  public void Advance(TextBox iSender)
  {
    if (GameStateManager.Instance.CurrentState is PlayState && (GameStateManager.Instance.CurrentState as PlayState).IsInCutscene)
    {
      if (this.mCutsceneText == iSender && this.mActiveInteracts[this.mCutsceneInteract] != null && this.mActiveInteracts[this.mCutsceneInteract].Advance(iSender, (Controller) null))
      {
        this.mDialogs.DialogFinished(this.mActiveInteracts[this.mCutsceneInteract].Parent.ID);
        if (this.mActiveInteracts[this.mCutsceneInteract].LockGame)
        {
          PhysicsManager.Instance.UnFreeze();
          ControlManager.Instance.UnlimitInput((object) this);
        }
        this.mActiveInteracts[this.mCutsceneInteract] = (Interact) null;
        this.mDrawCutsceneText = false;
      }
    }
    else
    {
      for (int index = 0; index < 8; ++index)
      {
        if (this.mTextBoxes[index] == iSender && this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Advance(iSender, (Controller) null))
        {
          this.mDialogs.DialogFinished(this.mActiveInteracts[index].Parent.ID);
          if (this.mActiveInteracts[index].LockGame)
          {
            PhysicsManager.Instance.UnFreeze();
            ControlManager.Instance.UnlimitInput((object) this);
          }
          this.mActiveInteracts[index] = (Interact) null;
        }
      }
    }
    for (int index = 0; index < this.mAdditionalTextBoxes.Count; ++index)
      this.mAdditionalTextBoxes[index].Hide();
  }

  public void ControllerSelect(Controller iSender)
  {
    if ((double) this.mHoldoffInputTimer > 0.0)
      return;
    if (this.mMessageBoxes.Count > 0)
      this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnSelect(iSender);
    else
      this.Advance(iSender);
  }

  public void ControllerMove(Controller iSender, ControllerDirection iValue)
  {
    if ((double) this.mHoldoffInputTimer > 0.0 | this.mMessageBoxes.Count == 0)
      return;
    Magicka.Game.Instance.IsMouseVisible = false;
    this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnMove(iSender, iValue);
  }

  public void ControllerType(Controller iSender, char iValue)
  {
    if ((double) this.mHoldoffInputTimer > 0.0 | this.mMessageBoxes.Count == 0)
      return;
    this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnTextInput(iValue);
  }

  public void ControllerMouseClick(Controller iSender, MouseState iNewState, MouseState iOldState)
  {
    this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnMouseClick(iNewState, iOldState);
  }

  public void ControllerEsc(Controller iSender)
  {
    this.mMessageBoxes[this.mMessageBoxes.Count - 1].ControllerEsc(iSender);
  }

  public void ControllerMouseRelease(
    Controller iSender,
    MouseState iNewState,
    MouseState iOldState)
  {
  }

  public void ControllerMouseMove(Controller iSender, MouseState iNewState, MouseState iOldState)
  {
    this.mMessageBoxes[this.mMessageBoxes.Count - 1].OnMouseMove(iNewState, iOldState);
  }

  public void ShowSubtitles(string iSubtitles)
  {
    this.mSubtitles.SetText(iSubtitles);
    this.mSubtitleHeight = this.mSubtitles.Font.MeasureText(iSubtitles, true).Y;
    this.mDrawSubtitles = true;
  }

  public void HideSubtitles() => this.mDrawSubtitles = false;

  public bool IsDialogActive
  {
    get
    {
      for (int index = 0; index < this.mActiveInteracts.Length; ++index)
      {
        if (this.mActiveInteracts[index] != null)
          return true;
      }
      return false;
    }
  }

  public bool AwaitingInput
  {
    get
    {
      return this.mAwaitingInput | this.mMessageBoxes.Count > 0 | (double) this.mHoldoffInputTimer > 0.0;
    }
  }

  public bool CanAdvance(Controller iSender)
  {
    for (int index = 0; index < this.mActiveInteracts.Length; ++index)
    {
      if (this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Interactor == iSender)
        return true;
    }
    return false;
  }

  public bool MessageBoxActive => this.mMessageBoxes.Count > 0;

  public BitmapFont SubtitleFont => this.mSubtitles.Font;

  public DialogCollection Dialogs => this.mDialogs;

  public bool HoldoffInput => (double) this.mHoldoffInputTimer > 0.0;

  public bool IsDialogDone(int iID, int iInteractIndex)
  {
    return this.mDialogs.IsDialogDone(iID) && this.mDialogs[iID].InteractedMessages > iInteractIndex;
  }

  public InteractType GetDialogIconText(int iDialog)
  {
    if (!this.mDialogs.ContainsKey(iDialog))
      return InteractType.Talk;
    Interact interact = this.mDialogs[iDialog].Peek();
    return interact == null ? InteractType.Talk : interact.IconText;
  }

  public TextBox GetTextBox()
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null)
        return this.mTextBoxes[index];
    }
    return (TextBox) null;
  }

  public void EndAll()
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null)
        this.mActiveInteracts[index].End();
      this.mActiveInteracts[index] = (Interact) null;
      this.mTextBoxes[index].FinishAnimation(false);
      this.mTextBoxes[index].Hide();
    }
    this.mCutsceneText.Hide();
    this.mCutsceneText.FinishAnimation(false);
    this.mAwaitingInput = false;
    PhysicsManager.Instance.UnFreeze();
    ControlManager.Instance.UnlimitInput((object) this);
  }

  public void End(Avatar iAvatar)
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null && iAvatar.Player.Controller != null && this.mActiveInteracts[index].Interactor == iAvatar.Player.Controller)
      {
        this.mActiveInteracts[index].End();
        this.mActiveInteracts[index] = (Interact) null;
        this.mTextBoxes[index].Hide();
      }
    }
  }

  public void End(int iDialog)
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Parent.ID == iDialog)
      {
        this.mActiveInteracts[index].End();
        this.mActiveInteracts[index] = (Interact) null;
        this.mTextBoxes[index].Hide();
      }
    }
  }

  public void Reset()
  {
    this.EndAll();
    if (this.mDialogs == null)
      return;
    this.mDialogs.Reset();
  }

  public void AddTextBox(TextBox iTextBox) => this.mAdditionalTextBoxes.Add(iTextBox);

  public void AddMessageBox(MessageBox iMessageBox)
  {
    if (!(GameStateManager.Instance.CurrentState is PlayState) || !(GameStateManager.Instance.CurrentState as PlayState).IsPaused)
      this.mHoldoffInputTimer = 0.2f;
    this.mMessageBoxes.Add(iMessageBox);
  }

  public bool DialogActive(int iDialog)
  {
    for (int index = 0; index < 8; ++index)
    {
      if (this.mActiveInteracts[index] != null && this.mActiveInteracts[index].Parent.ID == iDialog)
        return true;
    }
    return false;
  }

  private class RenderData : IRenderableGUIObject
  {
    public GUIBasicEffect Effect;
    public Text Text;
    public float SubtitleHeight;

    public void Draw(float iDeltaTime)
    {
      Point screenSize = RenderManager.Instance.ScreenSize;
      this.Effect.SetScreenSize(screenSize.X, screenSize.Y);
      this.Effect.Color = new Vector4(1f);
      this.Effect.Begin();
      this.Effect.CurrentTechnique.Passes[0].Begin();
      this.Text.Draw(this.Effect, (float) screenSize.X * 0.5f, (float) screenSize.Y - (this.SubtitleHeight + 32f));
      this.Effect.CurrentTechnique.Passes[0].End();
      this.Effect.End();
    }

    public int ZIndex => 2000;
  }
}
