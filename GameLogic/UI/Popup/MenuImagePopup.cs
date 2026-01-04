// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.Popup.MenuImagePopup
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.UI.Popup;

public class MenuImagePopup : MenuBasePopup
{
  private const string PAGES_TEXTURE = "UI/Menu/Pages";
  private const string PAGES_TEXTURE_DEMO = "UI/Menu/Pages_demo";
  protected static readonly VertexBuffer sVertices = (VertexBuffer) null;
  protected static readonly VertexDeclaration sDeclaration = (VertexDeclaration) null;
  protected static readonly Texture2D sPagesTexture;
  protected Texture2D mTexture;
  protected Vector2 mTextureOffset = Vector2.Zero;
  protected Vector2 mTextureScale = Vector2.One;
  private PopupDrawMode mDrawMode;
  private RenderModeSettings mRenderModeSettings;
  private List<RenderSection> mRenderSections;

  static MenuImagePopup()
  {
    VertexPositionTexture[] data = new VertexPositionTexture[4]
    {
      new VertexPositionTexture(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0.0f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 0.0f, 0.0f), new Vector2(1f, 0.0f)),
      new VertexPositionTexture(new Vector3(1f, 1f, 0.0f), new Vector2(1f, 1f)),
      new VertexPositionTexture(new Vector3(0.0f, 1f, 0.0f), new Vector2(0.0f, 1f))
    };
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      MenuImagePopup.sVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
      MenuImagePopup.sVertices.SetData<VertexPositionTexture>(data);
      MenuImagePopup.sDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
    }
    MenuImagePopup.sPagesTexture = Magicka.Game.Instance.Content.Load<Texture2D>("UI/Menu/Pages");
  }

  public MenuImagePopup(Texture2D iTexture, Vector2 iTextureUV, Vector2 iTextureSize)
  {
    this.mTexture = iTexture;
    Vector2 vector2 = new Vector2((float) this.mTexture.Width, (float) this.mTexture.Height);
    this.mTextureOffset = iTextureUV / vector2;
    this.mTextureScale = iTextureSize / vector2;
    this.mSize = iTextureSize;
    this.mHitBox.Width = (int) this.mSize.X;
    this.mHitBox.Height = (int) this.mSize.Y;
    this.UpdateBoundingBox();
    this.mTransform = Matrix.Identity;
    this.mTransform.M11 = this.mSize.X;
    this.mTransform.M22 = this.mSize.Y;
    this.mTransform.M41 = this.mPosition.X;
    this.mTransform.M42 = this.mPosition.Y;
    this.mRenderSections = new List<RenderSection>();
    this.mRenderSections.Add(new RenderSection(Vector2.Zero, Vector2.One, Vector2.Zero, Vector2.One));
  }

  public void EnableThreePatch(Vector2 iInsets, Vector2 iProportions)
  {
    this.mDrawMode = PopupDrawMode.ThreePatch;
    this.mRenderModeSettings = new RenderModeSettings(new Vector4(iInsets, 0.0f, 0.0f), new Vector4(iProportions, 0.0f, 0.0f));
    this.mRenderSections.Clear();
    this.CreateThreePatch();
  }

  private void CreateThreePatch()
  {
    float x = (float) (1.0 - ((double) this.mRenderModeSettings.Proportions.X + (double) this.mRenderModeSettings.Proportions.Y));
    this.mRenderSections.Add(new RenderSection(Vector2.Zero, new Vector2(this.mRenderModeSettings.Insets.X, 1f), Vector2.Zero, new Vector2(this.mRenderModeSettings.Proportions.X, 1f)));
    this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 0.0f), new Vector2(this.mRenderModeSettings.Insets.X + this.mRenderModeSettings.Insets.Y, 1f), new Vector2(this.mRenderModeSettings.Proportions.X, 0.0f), new Vector2(x, 1f)));
    this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 0.0f), new Vector2(this.mRenderModeSettings.Insets.Y, 1f), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 0.0f), new Vector2(this.mRenderModeSettings.Proportions.Y, 1f)));
  }

  public void EnableNinePatch(Vector4 iInsets, Vector4 iProportions)
  {
    this.mDrawMode = PopupDrawMode.NinePatch;
    this.mRenderModeSettings = new RenderModeSettings(iInsets, iProportions);
    this.mRenderSections.Clear();
    this.CreateNinePatch();
  }

  private void CreateNinePatch()
  {
    float x1 = (float) (1.0 - ((double) this.mRenderModeSettings.Insets.X + (double) this.mRenderModeSettings.Insets.Y));
    float y1 = (float) (1.0 - ((double) this.mRenderModeSettings.Insets.Z + (double) this.mRenderModeSettings.Insets.W));
    float x2 = (float) (1.0 - ((double) this.mRenderModeSettings.Proportions.X + (double) this.mRenderModeSettings.Proportions.Y));
    float y2 = (float) (1.0 - ((double) this.mRenderModeSettings.Proportions.Z + (double) this.mRenderModeSettings.Proportions.W));
    this.mRenderSections.Add(new RenderSection(Vector2.Zero, new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.Z), Vector2.Zero, new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.Z)));
    this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 0.0f), new Vector2(x1, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Proportions.X, 0.0f), new Vector2(x2, this.mRenderModeSettings.Proportions.Z)));
    this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 0.0f), new Vector2(this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.Z), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 0.0f), new Vector2(this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.Z)));
    this.mRenderSections.Add(new RenderSection(new Vector2(0.0f, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Insets.X, y1), new Vector2(0.0f, this.mRenderModeSettings.Proportions.Z), new Vector2(this.mRenderModeSettings.Proportions.X, y2)));
    this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.Z), new Vector2(x1, y1), new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.Z), new Vector2(x2, y2)));
    this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.Z), new Vector2(this.mRenderModeSettings.Insets.Y, y1), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.Z), new Vector2(this.mRenderModeSettings.Proportions.Y, y2)));
    this.mRenderSections.Add(new RenderSection(new Vector2(0.0f, 1f - this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Insets.X, this.mRenderModeSettings.Insets.W), new Vector2(0.0f, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(this.mRenderModeSettings.Proportions.X, this.mRenderModeSettings.Proportions.W)));
    this.mRenderSections.Add(new RenderSection(new Vector2(this.mRenderModeSettings.Insets.X, 1f - this.mRenderModeSettings.Insets.W), new Vector2(x1, this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Proportions.X, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(x2, this.mRenderModeSettings.Proportions.W)));
    this.mRenderSections.Add(new RenderSection(new Vector2(1f - this.mRenderModeSettings.Insets.Y, 1f - this.mRenderModeSettings.Insets.W), new Vector2(this.mRenderModeSettings.Insets.Y, this.mRenderModeSettings.Insets.W), new Vector2(1f - this.mRenderModeSettings.Proportions.Y, 1f - this.mRenderModeSettings.Proportions.W), new Vector2(this.mRenderModeSettings.Proportions.Y, this.mRenderModeSettings.Proportions.W)));
  }

  public override void Draw(GUIBasicEffect iEffect)
  {
    iEffect.GraphicsDevice.Vertices[0].SetSource(MenuImagePopup.sVertices, 0, VertexPositionTexture.SizeInBytes);
    iEffect.GraphicsDevice.VertexDeclaration = MenuImagePopup.sDeclaration;
    iEffect.VertexColorEnabled = false;
    iEffect.Color = this.mColour;
    iEffect.Texture = (Texture) this.mTexture;
    iEffect.TextureEnabled = true;
    iEffect.Saturation = 1f;
    foreach (RenderSection mRenderSection in this.mRenderSections)
      this.DrawSection(iEffect, mRenderSection.Position, mRenderSection.Size, mRenderSection.TextureOffset, mRenderSection.TextureSize);
    iEffect.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) null, 0, 0);
  }

  protected void DrawSection(
    GUIBasicEffect iEffect,
    Vector2 iPositionOffset,
    Vector2 iSize,
    Vector2 iTextureOffset,
    Vector2 iTextureSize)
  {
    Vector2 positionFromAlignment = this.GetPositionFromAlignment();
    Matrix mTransform = this.mTransform with
    {
      M11 = this.mSize.X * iSize.X * this.mScale,
      M22 = this.mSize.Y * iSize.Y * this.mScale,
      M41 = positionFromAlignment.X + this.mSize.X * iPositionOffset.X * this.mScale,
      M42 = positionFromAlignment.Y + this.mSize.Y * iPositionOffset.Y * this.mScale
    };
    iEffect.Transform = mTransform;
    iEffect.TextureOffset = this.mTextureOffset + this.mTextureScale * iTextureOffset;
    iEffect.TextureScale = this.mTextureScale * iTextureSize;
    iEffect.CommitChanges();
    iEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  internal override void ControllerA(Controller iSender)
  {
    this.Dismiss();
    if (this.mOnPositiveClickDelegate == null)
      return;
    this.mOnPositiveClickDelegate();
    this.mOnPositiveClickDelegate = (Action) null;
  }

  internal override void ControllerMouseAction(
    Controller iSender,
    Point iScreenSize,
    MouseState iState,
    MouseState iOldState)
  {
    this.Dismiss();
    if (this.mOnPositiveClickDelegate == null)
      return;
    this.mOnPositiveClickDelegate();
    this.mOnPositiveClickDelegate = (Action) null;
  }
}
