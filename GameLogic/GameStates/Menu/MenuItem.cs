// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuItem
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PolygonHead.Effects;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public abstract class MenuItem
{
  public static Vector4 COLOR_DISABLED = new Vector4(0.0f, 0.0f, 0.0f, 0.4f);
  public static Vector4 COLOR = new Vector4(0.1f, 0.05f, 0.0f, 0.7f);
  public static Vector4 COLOR_SELECTED = new Vector4(1.5f, 1.5f, 1.5f, 1f);
  protected bool mEnabled;
  protected bool mSelected;
  protected Vector2 mPosition;
  protected Matrix mTransform;
  protected Vector4 mColor;
  protected Vector4 mColorSelected;
  protected Vector4 mColorDisabled;
  protected float mAlpha;
  protected Vector2 mTopLeft;
  protected Vector2 mBottomRight;
  protected float mScale = 1f;

  public MenuItem()
  {
    this.mEnabled = true;
    this.mColor = MenuItem.COLOR;
    this.mColorSelected = MenuItem.COLOR_SELECTED;
    this.mColorDisabled = MenuItem.COLOR_DISABLED;
    this.mTransform = Matrix.Identity;
    this.mAlpha = 1f;
  }

  public Vector2 Position
  {
    get => this.mPosition;
    set
    {
      this.mPosition = value;
      this.mTransform.M41 = this.mPosition.X;
      this.mTransform.M42 = this.mPosition.Y;
      this.UpdateBoundingBox();
    }
  }

  public float Scale
  {
    get => this.mScale;
    set
    {
      this.mScale = value;
      this.UpdateBoundingBox();
    }
  }

  public float Alpha
  {
    get => this.mAlpha;
    set => this.mAlpha = value;
  }

  public virtual bool Enabled
  {
    get => this.mEnabled;
    set => this.mEnabled = value;
  }

  public virtual bool Selected
  {
    get => this.mSelected;
    set => this.mSelected = value;
  }

  public bool InsideBounds(float iX, float iY)
  {
    return (double) iX >= (double) this.mTopLeft.X & (double) iY >= (double) this.mTopLeft.Y & (double) iX <= (double) this.mBottomRight.X & (double) iY <= (double) this.mBottomRight.Y;
  }

  public bool InsideBounds(MouseState iState)
  {
    return (double) iState.X >= (double) this.mTopLeft.X & (double) iState.Y >= (double) this.mTopLeft.Y & (double) iState.X <= (double) this.mBottomRight.X & (double) iState.Y <= (double) this.mBottomRight.Y;
  }

  public bool InsideBounds(ref Vector2 iPoint)
  {
    return (double) iPoint.X >= (double) this.mTopLeft.X & (double) iPoint.Y >= (double) this.mTopLeft.Y & (double) iPoint.X <= (double) this.mBottomRight.X & (double) iPoint.Y <= (double) this.mBottomRight.Y;
  }

  public Vector2 TopLeft => this.mTopLeft;

  public Vector2 BottomRight => this.mBottomRight;

  public Vector4 Color
  {
    get => this.mColor;
    set => this.mColor = value;
  }

  public Vector4 ColorSelected
  {
    get => this.mColorSelected;
    set => this.mColorSelected = value;
  }

  public Vector4 ColorDisabled
  {
    get => this.mColorDisabled;
    set => this.mColorDisabled = value;
  }

  public float ColorAlphas
  {
    get => this.mColor.W;
    set
    {
      this.mColor.W = value;
      this.mColorSelected.W = value;
      this.mColorDisabled.W = value;
    }
  }

  protected abstract void UpdateBoundingBox();

  public abstract void Draw(GUIBasicEffect iEffect);

  public abstract void Draw(GUIBasicEffect iEffect, float iScale);

  public abstract void LanguageChanged();

  public virtual object Tag { get; set; }
}
