// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.CheckBox
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

internal class CheckBox : MenuItem
{
  private bool mChecked;
  private Vector4 mBackColor;
  private Vector4 mBackColorHover;
  private Vector4 mTickColor;
  private Vector4 mTickColorHover;
  private CheckBox.CheckBoxStyle mStyle;
  private bool mHover;
  private Texture2D mTexture;
  private VertexBuffer mVertexBuffer;
  private VertexDeclaration mVertexDeclaration;
  private int mWidth;
  private int mHeight;

  public event Action<CheckBox> OnCheckedChanged;

  public int Width
  {
    get => this.mWidth;
    set
    {
      this.mWidth = value;
      this.UpdateVertices();
    }
  }

  public int Height
  {
    get => this.mHeight;
    set
    {
      this.mHeight = value;
      this.UpdateVertices();
    }
  }

  public CheckBox(bool iChecked)
  {
    this.mBackColor = Vector4.One;
    this.mBackColorHover = new Vector4(1.5f, 1.5f, 1.5f, 1f);
    this.mTickColor = new Vector4(0.1f, 0.9f, 0.1f, 1f);
    this.mTickColorHover = new Vector4(0.5f, 1.5f, 0.5f, 1f);
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    this.mTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
    this.mVertexBuffer = new VertexBuffer(graphicsDevice, 128 /*0x80*/, BufferUsage.WriteOnly);
    this.mVertexDeclaration = RenderManager.Instance.CreateVertexDeclaration(new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    });
    this.Width = 32 /*0x20*/;
    this.Height = 32 /*0x20*/;
    this.UpdateVertices();
    this.Checked = iChecked;
    if (this.OnCheckedChanged == null)
      return;
    this.OnCheckedChanged(this);
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y * this.mScale;
    this.mBottomRight.X = this.mPosition.X + (float) this.mWidth * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + (float) this.mHeight * this.mScale;
  }

  public override void LanguageChanged()
  {
  }

  public CheckBox.CheckBoxStyle Style
  {
    get => this.mStyle;
    set
    {
      this.mStyle = value;
      this.UpdateVertices();
    }
  }

  private void UpdateVertices()
  {
    Vector4[] vector4Array = new Vector4[8];
    float num = (float) Math.Min(this.Width, this.Height);
    if (this.mStyle == CheckBox.CheckBoxStyle.RadioButton)
    {
      QuadHelper.CreateQuadFan(vector4Array, 0, new Vector2(), new Vector2(num), new Vector2(992f / (float) this.mTexture.Width, 160f / (float) this.mTexture.Height), new Vector2(32f / (float) this.mTexture.Width, 32f / (float) this.mTexture.Height));
      QuadHelper.CreateQuadFan(vector4Array, 4, new Vector2(), new Vector2(num), new Vector2(960f / (float) this.mTexture.Width, 160f / (float) this.mTexture.Height), new Vector2(32f / (float) this.mTexture.Width, 32f / (float) this.mTexture.Height));
    }
    else
    {
      QuadHelper.CreateQuadFan(vector4Array, 0, new Vector2(), new Vector2(num), new Vector2(992f / (float) this.mTexture.Width, 128f / (float) this.mTexture.Height), new Vector2(32f / (float) this.mTexture.Width, 32f / (float) this.mTexture.Height));
      QuadHelper.CreateQuadFan(vector4Array, 4, new Vector2(), new Vector2(num), new Vector2(960f / (float) this.mTexture.Width, 128f / (float) this.mTexture.Height), new Vector2(32f / (float) this.mTexture.Width, 32f / (float) this.mTexture.Height));
    }
    this.mVertexBuffer.SetData<Vector4>(vector4Array);
  }

  public bool Checked
  {
    get => this.mChecked;
    set
    {
      this.mChecked = value;
      if (this.OnCheckedChanged == null)
        return;
      this.OnCheckedChanged(this);
    }
  }

  public Vector4 BackgroundColor
  {
    get => this.mBackColor;
    set => this.mBackColor = value;
  }

  public Vector4 BackgroundColorHover
  {
    get => this.mBackColorHover;
    set => this.mBackColorHover = value;
  }

  public Vector4 TickColor
  {
    get => this.mBackColor;
    set => this.mBackColor = value;
  }

  public Vector4 TickColorHover
  {
    get => this.mTickColorHover;
    set => this.mTickColorHover = value;
  }

  public void DoMouseClick() => this.Toggle();

  public void Toggle() => this.Checked = !this.Checked;

  public void SetMouseHover(bool iHover) => this.mHover = iHover;

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    iEffect.Texture = (Texture) this.mTexture;
    iEffect.TextureEnabled = true;
    iEffect.Transform = Matrix.Identity;
    Matrix identity = Matrix.Identity with
    {
      M41 = this.Position.X,
      M42 = this.Position.Y
    };
    iEffect.Transform = identity;
    iEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertexBuffer, 0, 16 /*0x10*/);
    iEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
    Vector4 vector4 = this.mHover ? this.mBackColorHover : this.mBackColor;
    if (!this.mEnabled)
      vector4.W *= 0.5f;
    iEffect.Color = vector4;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    if (!this.mChecked)
      return;
    if (this.mStyle != CheckBox.CheckBoxStyle.RadioButton)
      vector4 = this.mHover ? this.mTickColorHover : this.mTickColor;
    if (!this.mEnabled)
      vector4.W *= 0.5f;
    iEffect.Color = vector4;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 4, 2);
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, this.mScale);

  public enum CheckBoxStyle
  {
    Checkbox,
    RadioButton,
  }
}
