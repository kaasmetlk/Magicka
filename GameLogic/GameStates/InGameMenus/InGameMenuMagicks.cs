// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.InGameMenus.InGameMenuMagicks
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.Controls;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.InGameMenus;

internal class InGameMenuMagicks : InGameMenu
{
  private const string OPTION_BACK = "Back";
  private const int NONE_INDEX = 28;
  private const int VISIBLE_ITEMS = 12;
  private static InGameMenuMagicks sSingelton;
  private static volatile object sSingeltonLock = new object();
  private static readonly Vector2[] MAGICK_TEXTURE_LOOKUP = new Vector2[33]
  {
    new Vector2(0.0f, 0.0f),
    new Vector2(25f / 128f, 0.0f),
    new Vector2(25f / 64f, 0.0f),
    new Vector2(75f / 128f, 0.0f),
    new Vector2(25f / 32f, 0.0f),
    new Vector2(0.0f, 0.125f),
    new Vector2(25f / 128f, 0.122070313f),
    new Vector2(25f / 64f, 0.122070313f),
    new Vector2(75f / 128f, 0.122070313f),
    new Vector2(25f / 32f, 0.122070313f),
    new Vector2(0.0f, 0.25f),
    new Vector2(25f / 128f, 125f / 512f),
    new Vector2(25f / 64f, 125f / 512f),
    new Vector2(75f / 128f, 125f / 512f),
    new Vector2(25f / 32f, 125f / 512f),
    new Vector2(0.0f, 0.375f),
    new Vector2(25f / 128f, 0.366210938f),
    new Vector2(25f / 64f, 0.366210938f),
    new Vector2(75f / 128f, 0.366210938f),
    new Vector2(25f / 32f, 0.366210938f),
    new Vector2(0.0f, 125f / 256f),
    new Vector2(25f / 128f, 125f / 256f),
    new Vector2(0.0f, 375f / 512f),
    new Vector2(25f / 128f, 375f / 512f),
    new Vector2(25f / 32f, 375f / 512f),
    new Vector2(25f / 128f, 0.8544922f),
    new Vector2(0.0f, 0.8544922f),
    new Vector2(25f / 64f, 375f / 512f),
    new Vector2(75f / 128f, 375f / 512f),
    new Vector2(25f / 128f, 0.0f),
    new Vector2(25f / 64f, 0.0f),
    new Vector2(25f / 32f, 0.0f),
    new Vector2(75f / 128f, 0.0f)
  };
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private Texture2D mTexture;
  private Texture2D mTexture2;
  private Texture2D mIconTexture;
  private string[] mDescriptions;
  private int[] mDescriptionsHash;
  private MenuScrollBar mScrollBar;
  private BitmapFont mFont;
  private int mMarkedItem;
  private Text mDescription;

  public static InGameMenuMagicks Instance
  {
    get
    {
      if (InGameMenuMagicks.sSingelton == null)
      {
        lock (InGameMenuMagicks.sSingeltonLock)
        {
          if (InGameMenuMagicks.sSingelton == null)
            InGameMenuMagicks.sSingelton = new InGameMenuMagicks();
        }
      }
      return InGameMenuMagicks.sSingelton;
    }
  }

  private InGameMenuMagicks()
  {
    this.mBackgroundSize = new Vector2(1000f, 550f);
    this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks");
    this.mTexture2 = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Magicks_2");
    this.mIconTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/HUD");
    Vector2 vector2 = new Vector2();
    vector2.X = 1f / (float) this.mTexture.Width;
    vector2.Y = 1f / (float) this.mTexture.Height;
    Vector4[] data = new Vector4[8];
    data[0].X = -200f;
    data[0].Y = -125f;
    data[0].Z = 0.0f;
    data[0].W = 0.0f;
    data[1].X = 200f;
    data[1].Y = -125f;
    data[1].Z = 400f * vector2.X;
    data[1].W = 0.0f;
    data[2].X = 200f;
    data[2].Y = 125f;
    data[2].Z = 400f * vector2.X;
    data[2].W = 250f * vector2.Y;
    data[3].X = -200f;
    data[3].Y = 125f;
    data[3].Z = 0.0f;
    data[3].W = 250f * vector2.Y;
    vector2.X = 1f / (float) this.mIconTexture.Width;
    vector2.Y = 1f / (float) this.mIconTexture.Height;
    data[4].X = -25f;
    data[4].Y = -25f;
    data[4].Z = 0.0f * vector2.X;
    data[4].W = 156f * vector2.Y;
    data[5].X = 25f;
    data[5].Y = -25f;
    data[5].Z = 50f * vector2.X;
    data[5].W = 156f * vector2.Y;
    data[6].X = 25f;
    data[6].Y = 25f;
    data[6].Z = 50f * vector2.X;
    data[6].W = 206f * vector2.Y;
    data[7].X = -25f;
    data[7].Y = 25f;
    data[7].Z = 0.0f * vector2.X;
    data[7].W = 206f * vector2.Y;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    lock (graphicsDevice)
    {
      this.mVertices = new VertexBuffer(graphicsDevice, 128 /*0x80*/, BufferUsage.WriteOnly);
      this.mVertices.SetData<Vector4>(data);
      this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, new VertexElement[2]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
        new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
      });
    }
    this.mFont = FontManager.Instance.GetFont(MagickaFont.Maiandra18);
    int length = 34;
    for (int index = 0; index < length; ++index)
      this.AddMenuTextItem("PLACEHOLDEROFDOOMLOLOLROFLCOPTER", this.mFont, TextAlign.Center);
    this.AddMenuTextItem("#menu_back".GetHashCodeCustom(), this.mFont, TextAlign.Center);
    this.mDescriptions = new string[length];
    this.mDescriptionsHash = new int[length];
    this.mScrollBar = new MenuScrollBar(new Vector2(), (float) (this.mFont.LineHeight * 12), this.mMenuItems.Count - 12 - 1);
    this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
    this.mDescription = new Text(1024 /*0x0400*/, this.mFont, TextAlign.Center, false);
  }

  public override void LanguageChanged()
  {
    base.LanguageChanged();
    for (int index = 0; index < this.mDescriptions.Length; ++index)
      this.mDescriptions[index] = this.mFont.Wrap(LanguageManager.Instance.GetString(this.mDescriptionsHash[index]), 500, true);
    this.mDescription.SetText(this.mDescriptions[this.mMarkedItem]);
  }

  public override void UpdatePositions()
  {
    for (int index = 0; index < this.mMenuItems.Count; ++index)
      this.mMenuItems[index].Scale = InGameMenu.sScale;
    this.mScrollBar.Position = new Vector2((float) ((double) InGameMenu.sScreenSize.X * 0.5 - 100.0 * (double) InGameMenu.sScale), InGameMenu.sScreenSize.Y * 0.5f);
    this.mScrollBar.Scale = InGameMenu.sScale;
  }

  protected override void IMouseScroll(Controller iSender, ref Vector2 iMousePos, int iValue)
  {
    if (this.mScrollBar.InsideBounds(ref iMousePos))
    {
      if (iValue > 0)
      {
        --this.mScrollBar.Value;
      }
      else
      {
        if (iValue >= 0)
          return;
        ++this.mScrollBar.Value;
      }
    }
    else
    {
      for (int index = 0; index < this.mMenuItems.Count - 1; ++index)
      {
        if (this.mMenuItems[index].InsideBounds(ref iMousePos))
        {
          if (iValue > 0)
          {
            --this.mScrollBar.Value;
            break;
          }
          if (iValue >= 0)
            break;
          ++this.mScrollBar.Value;
          break;
        }
      }
    }
  }

  protected override void IMouseMove(Controller iSender, ref Vector2 iMousePosition)
  {
    if (this.mScrollBar.Grabbed)
    {
      this.mSelectedItem = -1;
      if (this.mScrollBar.InsideDragUpBounds(iMousePosition))
      {
        --this.mScrollBar.Value;
      }
      else
      {
        if (!this.mScrollBar.InsideDragDownBounds(iMousePosition))
          return;
        ++this.mScrollBar.Value;
      }
    }
    else
    {
      int num = -1;
      for (int index = this.mScrollBar.Value; index < this.mScrollBar.Value + 12; ++index)
      {
        if (this.mMenuItems[index].Enabled && this.mMenuItems[index].InsideBounds(ref iMousePosition))
        {
          num = index;
          break;
        }
      }
      if (num == -1 && this.mMenuItems[this.mMenuItems.Count - 1].InsideBounds(ref iMousePosition))
        num = this.mMenuItems.Count - 1;
      if (this.mSelectedItem != num & num >= 0)
        AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_MOVE);
      this.mSelectedItem = num;
    }
  }

  protected override void IMouseDown(Controller iSender, ref Vector2 iMousePosition)
  {
    base.IMouseDown(iSender, ref iMousePosition);
    if (this.mScrollBar.InsideDragBounds(iMousePosition))
    {
      this.mScrollBar.Grabbed = true;
    }
    else
    {
      if (this.mScrollBar.InsideUpBounds(iMousePosition) || this.mScrollBar.InsideDownBounds(iMousePosition) || !this.mScrollBar.InsideBounds(ref iMousePosition))
        return;
      this.mScrollBar.ScrollTo(iMousePosition.Y);
    }
  }

  protected override void IMouseUp(Controller iSender, ref Vector2 iMousePosition)
  {
    base.IMouseUp(iSender, ref iMousePosition);
    if (!this.mScrollBar.Grabbed)
    {
      if (this.mScrollBar.InsideUpBounds(iMousePosition))
        --this.mScrollBar.Value;
      else if (this.mScrollBar.InsideDownBounds(iMousePosition))
        ++this.mScrollBar.Value;
    }
    this.mScrollBar.Grabbed = false;
  }

  protected override void IControllerMove(Controller iSender, ControllerDirection iDirection)
  {
    switch (iDirection)
    {
      case ControllerDirection.Right:
        if (this.mSelectedItem == this.mMenuItems.Count - 1)
        {
          this.mSelectedItem = 0;
          break;
        }
        this.mSelectedItem = this.mMenuItems.Count - 1;
        break;
      case ControllerDirection.Left:
        if (this.mSelectedItem == this.mMenuItems.Count - 1)
        {
          this.mSelectedItem = 0;
          break;
        }
        this.mSelectedItem = this.mMenuItems.Count - 1;
        break;
      default:
        base.IControllerMove(iSender, iDirection);
        break;
    }
    while (this.mSelectedItem >= this.mScrollBar.Value + 12 && this.mSelectedItem != this.mMenuItems.Count - 1)
      ++this.mScrollBar.Value;
    while (this.mSelectedItem < this.mScrollBar.Value)
      --this.mScrollBar.Value;
    if (this.mSelectedItem >= 0 && this.mSelectedItem < this.mMenuItems.Count - 1)
    {
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
      this.mDescription.SetText(this.mDescriptions[this.mSelectedItem]);
      this.mMarkedItem = this.mSelectedItem;
    }
    else
      this.mMarkedItem = -1;
  }

  protected override void IControllerSelect(Controller iSender)
  {
    if (this.mSelectedItem == this.mMenuItems.Count - 1)
    {
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
      InGameMenu.PopMenu();
    }
    else
    {
      if (this.mSelectedItem < 0)
        return;
      AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_INCREASE);
      if (this.mSelectedItem >= 0 && this.mSelectedItem < this.mMenuItems.Count - 1)
        this.mDescription.SetText(this.mDescriptions[this.mSelectedItem]);
      this.mMarkedItem = this.mSelectedItem;
    }
  }

  protected override void IControllerBack(Controller iSender)
  {
    AudioManager.Instance.PlayCue(Banks.UI, InGameMenu.SOUND_DECREASE);
    InGameMenu.PopMenu();
  }

  protected override string IGetHighlightedButtonName()
  {
    return this.mSelectedItem != this.mMenuItems.Count - 1 ? ((MagickType) this.mSelectedItem).ToString() : "Back";
  }

  protected override void OnEnter()
  {
    this.mScrollBar.Value = 0;
    this.mSelectedItem = -1;
    this.mMarkedItem = -1;
    LanguageManager instance = LanguageManager.Instance;
    Player player = InGameMenu.sController.Player;
    if (player == null && InGameMenu.sPlayState.GameType != GameType.Versus)
    {
      for (int index = 0; index < Magicka.Game.Instance.Players.Length; ++index)
      {
        if (Magicka.Game.Instance.Players[index].Playing)
        {
          player = Magicka.Game.Instance.Players[index];
          index = Magicka.Game.Instance.Players.Length;
        }
      }
    }
    int num = 34;
    for (int index = 0; index < num; ++index)
    {
      MagickType iMagick = (MagickType) (index + 1);
      if (iMagick >= (MagickType) Magick.DESC_LOCALIZATION.Length)
      {
        Console.WriteLine("Magick description out of range! Fix DESC_LOCALIZATION.");
      }
      else
      {
        this.mDescriptionsHash[index] = Magick.DESC_LOCALIZATION[(int) iMagick];
        this.mDescriptions[index] = this.mFont.Wrap(instance.GetString(this.mDescriptionsHash[index]), 500, true);
      }
      if (SpellManager.Instance.IsMagickAllowed(player, InGameMenu.sPlayState.GameType, iMagick) && player != null)
      {
        this.mMenuItems[index].Enabled = true;
        (this.mMenuItems[index] as MenuTextItem).SetText(Magick.NAME_LOCALIZATION[(int) iMagick]);
      }
      else
      {
        this.mMenuItems[index].Enabled = false;
        (this.mMenuItems[index] as MenuTextItem).SetText("???");
      }
    }
  }

  protected override void IUpdate(DataChannel iDataChannel, float iDeltaTime)
  {
    base.IUpdate(iDataChannel, iDeltaTime);
  }

  protected override void IDraw(float iDeltaTime, ref Vector2 iBackgroundSize)
  {
    Matrix matrix = new Matrix();
    matrix.M44 = 1f;
    Vector4 vector4_1 = new Vector4();
    vector4_1.X = vector4_1.Y = vector4_1.Z = 1f;
    Vector4 vector4_2 = new Vector4();
    vector4_2.X = vector4_2.Y = vector4_2.Z = 1f;
    vector4_2.W = this.mAlpha;
    Vector4 vector4_3 = new Vector4();
    vector4_3.X = vector4_3.Y = vector4_3.Z = 0.0f;
    vector4_3.W = this.mAlpha;
    Vector4 vector4_4 = new Vector4();
    vector4_4.X = vector4_4.Y = vector4_4.Z = 0.4f;
    vector4_4.W = this.mAlpha;
    float lineHeight = (float) this.mFont.LineHeight;
    Vector2 vector2_1 = new Vector2();
    vector2_1.X = (float) ((double) InGameMenu.sScreenSize.X * 0.5 - 250.0 * (double) InGameMenu.sScale);
    vector2_1.Y = (float) ((double) InGameMenu.sScreenSize.Y * 0.5 - (double) lineHeight * 11.0 * 0.5 * (double) InGameMenu.sScale);
    for (int index = this.mScrollBar.Value; index < this.mScrollBar.Value + 12; ++index)
    {
      MenuItem mMenuItem = this.mMenuItems[index];
      mMenuItem.Position = vector2_1;
      mMenuItem.Color = vector4_2;
      mMenuItem.ColorSelected = vector4_3;
      mMenuItem.ColorDisabled = vector4_4;
      mMenuItem.Selected = mMenuItem.Enabled & (this.mSelectedItem == index | this.mMarkedItem == index);
      if (mMenuItem.Selected)
      {
        matrix.M11 = 300f * InGameMenu.sScale;
        matrix.M22 = mMenuItem.BottomRight.Y - mMenuItem.TopLeft.Y;
        matrix.M41 = mMenuItem.Position.X - 150f * InGameMenu.sScale;
        matrix.M42 = mMenuItem.TopLeft.Y;
        InGameMenu.sEffect.Transform = matrix;
        vector4_1.W = this.mSelectedItem != index ? 0.5f * this.mAlpha : 0.8f * this.mAlpha;
        InGameMenu.sEffect.Color = vector4_1;
        InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
        InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
        InGameMenu.sEffect.CommitChanges();
        InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
      }
      vector2_1.Y += lineHeight * InGameMenu.sScale;
    }
    this.mMenuItems[this.mMenuItems.Count - 1].Position = vector2_1;
    this.mMenuItems[this.mMenuItems.Count - 1].Color = vector4_2;
    this.mMenuItems[this.mMenuItems.Count - 1].ColorSelected = vector4_3;
    this.mMenuItems[this.mMenuItems.Count - 1].ColorDisabled = vector4_4;
    this.mMenuItems[this.mMenuItems.Count - 1].Selected = this.mSelectedItem == this.mMenuItems.Count - 1;
    if (this.mMenuItems[this.mMenuItems.Count - 1].Selected)
    {
      matrix.M11 = 100f * InGameMenu.sScale;
      matrix.M22 = this.mMenuItems[this.mMenuItems.Count - 1].BottomRight.Y - this.mMenuItems[this.mMenuItems.Count - 1].TopLeft.Y;
      matrix.M41 = this.mMenuItems[this.mMenuItems.Count - 1].Position.X - 50f * InGameMenu.sScale;
      matrix.M42 = this.mMenuItems[this.mMenuItems.Count - 1].TopLeft.Y;
      InGameMenu.sEffect.Transform = matrix;
      vector4_1.W = this.mSelectedItem != this.mMenuItems.Count - 1 ? 0.5f * this.mAlpha : 0.8f * this.mAlpha;
      InGameMenu.sEffect.Color = vector4_1;
      InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(InGameMenu.sBackground, 0, 8);
      InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = InGameMenu.sBackgroundDeclaration;
      InGameMenu.sEffect.CommitChanges();
      InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    }
    for (int index = this.mScrollBar.Value; index < this.mScrollBar.Value + 12; ++index)
      this.mMenuItems[index].Draw(InGameMenu.sEffect);
    this.mMenuItems[this.mMenuItems.Count - 1].Draw(InGameMenu.sEffect);
    vector4_1.W = this.mAlpha;
    this.mScrollBar.Color = vector4_1;
    this.mScrollBar.TextureOffset = new Vector2(-384f, 224f);
    this.mScrollBar.Draw(InGameMenu.sEffect);
    matrix.M11 = InGameMenu.sScale;
    matrix.M22 = InGameMenu.sScale;
    matrix.M41 = (float) ((double) InGameMenu.sScreenSize.X * 0.5 + 180.0 * (double) InGameMenu.sScale);
    matrix.M42 = 270f * InGameMenu.sScale;
    int mMarkedItem = this.mMarkedItem;
    MagickType iType = (MagickType) (mMarkedItem + 1);
    InGameMenu.sEffect.Transform = matrix;
    InGameMenu.sEffect.Color = vector4_1;
    if (iType >= MagickType.Confuse)
    {
      InGameMenu.sEffect.TextureScale = new Vector2(1f, 4f);
      InGameMenu.sEffect.Texture = (Texture) this.mTexture2;
    }
    else
      InGameMenu.sEffect.Texture = (Texture) this.mTexture;
    if (!(mMarkedItem < 0 | mMarkedItem == this.mMenuItems.Count - 1))
    {
      try
      {
        InGameMenu.sEffect.TextureOffset = InGameMenuMagicks.MAGICK_TEXTURE_LOOKUP[mMarkedItem];
      }
      catch (Exception ex)
      {
        InGameMenu.sEffect.TextureOffset = InGameMenuMagicks.MAGICK_TEXTURE_LOOKUP[0];
      }
      InGameMenu.sEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 16 /*0x10*/);
      InGameMenu.sEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      InGameMenu.sEffect.CommitChanges();
      InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    }
    InGameMenu.sEffect.TextureScale = Vector2.One;
    if (this.mMarkedItem >= 0)
    {
      Elements[] magickCombo = SpellManager.Instance.GetMagickCombo(iType);
      if (magickCombo != null)
      {
        Vector2 vector2_2 = new Vector2();
        InGameMenu.sEffect.Texture = (Texture) this.mIconTexture;
        matrix.M11 = InGameMenu.sScale * 0.75f;
        matrix.M22 = InGameMenu.sScale * 0.75f;
        matrix.M42 = 380f * InGameMenu.sScale;
        for (int index = 0; index < magickCombo.Length; ++index)
        {
          int num = MagickaMath.CountTrailingZeroBits((uint) magickCombo[index]);
          matrix.M41 = (float) ((double) InGameMenu.sScreenSize.X * 0.5 + (180.0 - (double) magickCombo.Length * 0.5 * 38.0 + ((double) index + 0.5) * 38.0) * (double) InGameMenu.sScale);
          InGameMenu.sEffect.Transform = matrix;
          vector2_2.X = (float) (num % 5) * 50f / (float) this.mIconTexture.Width;
          vector2_2.Y = (float) (num / 5) * 50f / (float) this.mIconTexture.Height;
          InGameMenu.sEffect.TextureOffset = vector2_2;
          InGameMenu.sEffect.CommitChanges();
          InGameMenu.sEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
        }
      }
      this.mDescription.Draw(InGameMenu.sEffect, (float) ((double) InGameMenu.sScreenSize.X * 0.5 + 180.0 * (double) InGameMenu.sScale), (float) (270.0 * (double) InGameMenu.sScale + 125.0 * (double) InGameMenu.sScale), InGameMenu.sScale);
    }
    InGameMenu.sEffect.TextureOffset = new Vector2();
  }

  protected override void OnExit()
  {
  }
}
