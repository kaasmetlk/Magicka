// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuOptionsSound
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuOptionsSound : InGameMenu
{
  private const string OPTION_BACK = "back";
  private static InGameMenuOptionsSound sSingelton;
  private static volatile object sSingeltonLock = new object();
  protected static readonly int SOUND_THUD = "misc_thud01".GetHashCodeCustom();
  private MenuScrollSlider mMusicScrollSlider;
  private MenuScrollSlider mSFXScrollSlider;
  private int mMusicLevel;
  private int mSfxLevel;
  private BitmapFont mFont;

  public static InGameMenuOptionsSound Instance
  {
    get
    {
      if (InGameMenuOptionsSound.sSingelton == null)
      {
        lock (InGameMenuOptionsSound.sSingeltonLock)
        {
          if (InGameMenuOptionsSound.sSingelton == null)
            InGameMenuOptionsSound.sSingelton = new InGameMenuOptionsSound();
        }
      }
      return InGameMenuOptionsSound.sSingelton;
    }
  }

  private InGameMenuOptionsSound()
  {
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    this.AddMenuTextItem("#menu_opt_sfx_01".GetHashCodeCustom(), this.mFont, TextAlign.Right);
    this.AddMenuTextItem("#menu_opt_sfx_02".GetHashCodeCustom(), this.mFont, TextAlign.Right);
    this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), this.mFont, TextAlign.Center);
    this.mMusicScrollSlider = new MenuScrollSlider(new Vector2(), 240f, 10);
    this.mMusicScrollSlider.TextureOffset = new Vector2(-384f, 224f);
    this.mSFXScrollSlider = new MenuScrollSlider(new Vector2(), 240f, 10);
    this.mSFXScrollSlider.TextureOffset = new Vector2(-384f, 224f);
    this.mBackgroundSize = new Vector2(400f, 200f);
    Vector2 vector2 = new Vector2();
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - 60.0 * (double) InGameMenu.sScale);
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 30.0 * (double) InGameMenu.sScale);
    this.mMusicScrollSlider.Position = vector2 + new Vector2(120f * InGameMenu.sScale, 0.0f);
    vector2.Y += (float) this.mFont.LineHeight;
    this.mSFXScrollSlider.Position = vector2 + new Vector2(120f * InGameMenu.sScale, 0.0f);
    this.mMusicScrollSlider.Scale = 0.7f;
    this.mSFXScrollSlider.Scale = 0.7f;
  }

  public override void UpdatePositions()
  {
    Vector2 vector2 = new Vector2();
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - 60.0 * (double) InGameMenu.sScale);
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 30.0 * (double) InGameMenu.sScale);
    for (int index = 0; index < 2; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Scale = InGameMenu.sScale;
      mMenuItem.Position = vector2;
      vector2.Y += mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
    }
    vector2.X = InGameMenu.sScreenSize.X * 0.5f;
    vector2.Y += 10f * InGameMenu.sScale;
    this.mMenuItems[2].Scale = InGameMenu.sScale;
    this.mMenuItems[2].Position = vector2;
    vector2 = new Vector2();
    vector2.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - 60.0 * (double) InGameMenu.sScale);
    vector2.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - 30.0 * (double) InGameMenu.sScale);
    this.mMusicScrollSlider.Position = vector2 + new Vector2(120f * InGameMenu.sScale, 0.0f);
    vector2.Y += (float) this.mFont.LineHeight;
    this.mSFXScrollSlider.Position = vector2 + new Vector2(120f * InGameMenu.sScale, 0.0f);
  }

  protected override string IGetHighlightedButtonName() => "back";

  protected override void IControllerSelect(Controller iSender)
  {
    if (this.mSelectedItem != 2)
      return;
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override void IControllerBack(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    base.IControllerMove(iSender, iDirection);
    switch (iDirection)
    {
      case ControllerDirection.Right:
        if (this.mSelectedItem == 0 && this.mMusicScrollSlider.Value < 10)
        {
          ++this.mMusicScrollSlider.Value;
          AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
          break;
        }
        if (this.mSelectedItem != 1 || this.mSFXScrollSlider.Value >= 10)
          break;
        ++this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
        break;
      case ControllerDirection.Left:
        if (this.mSelectedItem == 0 && this.mMusicScrollSlider.Value > 0)
        {
          --this.mMusicScrollSlider.Value;
          AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
          break;
        }
        if (this.mSelectedItem != 1 || this.mSFXScrollSlider.Value <= 0)
          break;
        --this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
        break;
    }
  }

  protected override void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mMusicScrollSlider.InsideBounds(ref iMousePosition))
    {
      if (this.mMusicScrollSlider.InsideDragBounds(iMousePosition))
      {
        this.mMusicScrollSlider.Grabbed = true;
        this.mSelectedItem = -1;
      }
      else
      {
        if (!(!this.mMusicScrollSlider.InsideLeftBounds(iMousePosition) & !this.mMusicScrollSlider.InsideRightBounds(iMousePosition)))
          return;
        int num = this.mMusicScrollSlider.Value;
        this.mMusicScrollSlider.ScrollTo(iMousePosition.X);
        if (num == this.mMusicScrollSlider.Value)
          return;
        AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
      }
    }
    else if (this.mSFXScrollSlider.InsideBounds(ref iMousePosition))
    {
      if (this.mSFXScrollSlider.InsideDragBounds(iMousePosition))
      {
        this.mSFXScrollSlider.Grabbed = true;
        this.mSelectedItem = -1;
      }
      else
      {
        if (!(!this.mSFXScrollSlider.InsideLeftBounds(iMousePosition) & !this.mSFXScrollSlider.InsideRightBounds(iMousePosition)))
          return;
        int num = this.mSFXScrollSlider.Value;
        this.mSFXScrollSlider.ScrollTo(iMousePosition.X);
        if (num == this.mSFXScrollSlider.Value)
          return;
        AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
      }
    }
    else
      base.IMouseDown(iSender, ref iMousePosition);
  }

  protected override void IMouseScroll(Controller iSender, ref Vector2 iMousePosition, int iValue)
  {
    if (this.mMusicScrollSlider.InsideBounds(ref iMousePosition))
    {
      if (iValue > 0)
      {
        --this.mMusicScrollSlider.Value;
      }
      else
      {
        if (iValue >= 0)
          return;
        ++this.mMusicScrollSlider.Value;
      }
    }
    else
    {
      if (!this.mSFXScrollSlider.InsideBounds(ref iMousePosition))
        return;
      if (iValue > 0)
      {
        --this.mSFXScrollSlider.Value;
      }
      else
      {
        if (iValue >= 0)
          return;
        ++this.mSFXScrollSlider.Value;
      }
    }
  }

  protected override void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mMusicScrollSlider.Grabbed | this.mSFXScrollSlider.Grabbed)
    {
      this.mMusicScrollSlider.Grabbed = false;
      this.mSFXScrollSlider.Grabbed = false;
    }
    else if (this.mMusicScrollSlider.Value > 0 & this.mMusicScrollSlider.InsideLeftBounds(iMousePosition))
    {
      --this.mMusicScrollSlider.Value;
      AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else if (this.mMusicScrollSlider.Value < 10 & this.mMusicScrollSlider.InsideRightBounds(iMousePosition))
    {
      ++this.mMusicScrollSlider.Value;
      AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else if (this.mSFXScrollSlider.Value > 0 & this.mSFXScrollSlider.InsideLeftBounds(iMousePosition))
    {
      --this.mSFXScrollSlider.Value;
      AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else if (this.mSFXScrollSlider.Value < 10 & this.mSFXScrollSlider.InsideRightBounds(iMousePosition))
    {
      ++this.mSFXScrollSlider.Value;
      AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else
      base.IMouseUp(iSender, ref iMousePosition);
  }

  protected override void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mMusicScrollSlider.Grabbed)
    {
      if (this.mMusicScrollSlider.Value > 0 & this.mMusicScrollSlider.InsideDragLeftBounds(iMousePosition))
      {
        --this.mMusicScrollSlider.Value;
        AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
      }
      if (!(this.mMusicScrollSlider.Value < 10 & this.mMusicScrollSlider.InsideDragRightBounds(iMousePosition)))
        return;
      ++this.mMusicScrollSlider.Value;
      AudioManager.Instance.VolumeMusic(this.mMusicScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else if (this.mSFXScrollSlider.Grabbed)
    {
      if (this.mSFXScrollSlider.Value > 0 & this.mSFXScrollSlider.InsideDragLeftBounds(iMousePosition))
      {
        --this.mSFXScrollSlider.Value;
        AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
        AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
      }
      if (!(this.mSFXScrollSlider.Value < 10 & this.mSFXScrollSlider.InsideDragRightBounds(iMousePosition)))
        return;
      ++this.mSFXScrollSlider.Value;
      AudioManager.Instance.VolumeSound(this.mSFXScrollSlider.Value);
      AudioManager.Instance.PlayCue(Banks.Misc, InGameMenuOptionsSound.SOUND_THUD);
    }
    else if (this.mMenuItems[2].InsideBounds(ref iMousePosition))
    {
      if (this.mSelectedItem != 2)
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
      this.mSelectedItem = 2;
    }
    else
      this.mSelectedItem = -1;
  }

  protected override void OnEnter()
  {
    if (InGameMenu.sController is KeyboardMouseController)
      this.mSelectedItem = -1;
    else
      this.mSelectedItem = 0;
    this.mMusicScrollSlider.Value = this.mMusicLevel = AudioManager.Instance.VolumeMusic();
    this.mSFXScrollSlider.Value = this.mSfxLevel = AudioManager.Instance.VolumeSound();
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    this.UpdatePositions();
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    vector4_1.W = this.mAlpha;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 0.0f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.4f;
    vector4_3.W = this.mAlpha;
    for (int index = 0; index < this.mMenuItems.Count; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Color = vector4_1;
      mMenuItem.ColorSelected = vector4_2;
      mMenuItem.ColorDisabled = vector4_3;
      mMenuItem.Selected = mMenuItem.Enabled & this.mSelectedItem == index;
      if (mMenuItem.Selected)
      {
        InGameMenu.sEffect.Transform = new Matrix()
        {
          M44 = 1f,
          M11 = iBackgroundSize.X * InGameMenu.sScale,
          M22 = mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y,
          M41 = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - (double) iBackgroundSize.X * 0.5 * (double) InGameMenu.sScale),
          M42 = mMenuItem.TopLeft.Y
        };
        Vector4 vector4_4 = new Vector4();
        vector4_4.X = vector4_4.Y = vector4_4.Z = 1f;
        vector4_4.W = 0.8f * this.mAlpha;
        InGameMenu.sEffect.Color = vector4_4;
        InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
        InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
        InGameMenu.sEffect.CommitChanges();
        InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
    }
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Draw(InGameMenu.sEffect);
    this.mMusicScrollSlider.Color = vector4_1;
    this.mSFXScrollSlider.Color = vector4_1;
    this.mMusicScrollSlider.Draw(InGameMenu.sEffect);
    this.mSFXScrollSlider.Draw(InGameMenu.sEffect);
  }

  protected override void OnExit()
  {
    GlobalSettings instance = GlobalSettings.Instance;
    instance.VolumeMusic = AudioManager.Instance.VolumeMusic();
    instance.VolumeSound = AudioManager.Instance.VolumeSound();
    if (!(this.mMusicLevel != instance.VolumeMusic | this.mSfxLevel != instance.VolumeSound))
      return;
    SaveManager.Instance.SaveSettings();
  }
}
