// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.Menu.MenuItemList
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates.Menu;

public class MenuItemList : MenuItem
{
  private const float MIN_WIDTH = 128f;
  private const float MIN_HEIGHT = 32f;
  private static VertexBuffer sVertexBuffer;
  private static VertexDeclaration sVertexDeclaration;
  private List<Text> mItems;
  private Vector2 mSize;
  private BitmapFont mFont;
  private float mLineHeight;

  static MenuItemList()
  {
    VertexPositionColor[] data = new VertexPositionColor[4]
    {
      new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), Microsoft.Xna.Framework.Graphics.Color.Black),
      new VertexPositionColor(new Vector3(1f, 0.0f, 0.0f), Microsoft.Xna.Framework.Graphics.Color.Black),
      new VertexPositionColor(new Vector3(1f, 1f, 0.0f), Microsoft.Xna.Framework.Graphics.Color.Black),
      new VertexPositionColor(new Vector3(0.0f, 1f, 0.0f), Microsoft.Xna.Framework.Graphics.Color.Black)
    };
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      MenuItemList.sVertexBuffer = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionColor.SizeInBytes, BufferUsage.WriteOnly);
      MenuItemList.sVertexBuffer.SetData<VertexPositionColor>(data);
      MenuItemList.sVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionColor.VertexElements);
    }
  }

  public MenuItemList(Vector2 iPosition, BitmapFont iFont, Vector2 iSize)
  {
    this.mFont = iFont;
    this.mLineHeight = (float) iFont.LineHeight;
    this.mItems = new List<Text>(8);
    for (int index = 0; index < 6; ++index)
    {
      this.mItems.Add(new Text(100, iFont, TextAlign.Left, false));
      if (index == 0)
        this.mItems[index].SetText("Create new profile(noloc)");
      else
        this.mItems[index].SetText("Dummmy");
      this.mItems[index].DefaultColor = new Vector4(1f, 1f, 1f, 1f);
    }
    this.mPosition = iPosition;
    this.mSize = iSize;
    this.mTransform = Matrix.Identity;
    this.mTransform.M41 = iPosition.X;
    this.mTransform.M42 = iPosition.Y;
    this.mTransform.M11 = this.mSize.X;
    this.mTransform.M22 = this.mSize.Y;
  }

  protected override void UpdateBoundingBox()
  {
    this.mTopLeft.X = this.mPosition.X - this.mSize.X * 0.5f * this.mScale;
    this.mTopLeft.Y = this.mPosition.Y - this.mSize.Y * 0.5f * this.mScale;
    this.mBottomRight.X = this.mPosition.X + this.mSize.X * 0.5f * this.mScale;
    this.mBottomRight.Y = this.mPosition.Y + this.mSize.Y * 0.5f * this.mScale;
  }

  public override void LanguageChanged()
  {
  }

  public override void Draw(GUIBasicEffect iEffect) => this.Draw(iEffect, 1f);

  public override void Draw(GUIBasicEffect iEffect, float iScale)
  {
    if (!this.mEnabled)
      return;
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuItemList.sVertexBuffer, 0, VertexPositionColor.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = MenuItemList.sVertexDeclaration;
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
    this.mTransform.M11 = this.mSize.X;
    this.mTransform.M22 = this.mSize.Y;
    iEffect.Transform = this.mTransform;
    iEffect.Color = new Vector4(1f);
    iEffect.TextureEnabled = false;
    iEffect.OverlayTextureEnabled = false;
    iEffect.VertexColorEnabled = true;
    iEffect.Saturation = 1f;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
    this.mTransform.M11 = iScale;
    this.mTransform.M22 = iScale;
    this.mTransform.M42 = this.mPosition.Y;
    iEffect.VertexColorEnabled = false;
    for (int index = 0; index < this.mItems.Count; ++index)
    {
      this.mItems[index].Draw(iEffect, ref this.mTransform);
      this.mTransform.M42 += this.mLineHeight;
    }
  }
}
