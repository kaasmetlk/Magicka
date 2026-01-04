// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Flash
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic;
using Magicka.GameLogic.Spells;
using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.Graphics;

public class Flash : IAbilityEffect, IRenderableAdditiveObject, IPreRenderRenderer
{
  private static Flash sSingelton;
  private static volatile object sSingeltonLock = new object();
  private float mIntensity;
  private float mTTL;
  private Scene mScene;
  private float[] mIntensities = new float[3];
  private int mEffectHash;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private int mVerticesHash;
  private DataChannel mCurrentDataChannel;

  public static Flash Instance
  {
    get
    {
      if (Flash.sSingelton == null)
      {
        lock (Flash.sSingeltonLock)
        {
          if (Flash.sSingelton == null)
            Flash.sSingelton = new Flash();
        }
      }
      return Flash.sSingelton;
    }
  }

  private Flash()
  {
    this.mEffectHash = RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) new FlashEffect());
    Vector2[] data = new Vector2[4];
    data[0].X = -1f;
    data[0].Y = 1f;
    data[1].X = 1f;
    data[1].Y = 1f;
    data[2].X = 1f;
    data[2].Y = -1f;
    data[3].X = -1f;
    data[3].Y = -1f;
    this.mVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 32 /*0x20*/, BufferUsage.WriteOnly);
    this.mVertices.SetData<Vector2>(data);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, new VertexElement[1]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0)
    });
    this.mVerticesHash = this.mVertices.GetHashCode();
  }

  public void Execute(Scene iScene, float iTime)
  {
    this.mTTL = this.mIntensity = Math.Max(iTime, this.mIntensity);
    this.mScene = iScene;
    SpellManager.Instance.AddSpellEffect((IAbilityEffect) this);
  }

  public int Effect => this.mEffectHash;

  public int Technique => 0;

  public VertexBuffer Vertices => this.mVertices;

  public int VerticesHashCode => this.mVerticesHash;

  public int VertexStride => 8;

  public IndexBuffer Indices => (IndexBuffer) null;

  public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public bool Cull(BoundingFrustum iViewFrustum) => false;

  public void Draw(Microsoft.Xna.Framework.Graphics.Effect iEffect, BoundingFrustum iViewFrustum)
  {
    FlashEffect flashEffect = iEffect as FlashEffect;
    Vector4 vector4 = new Vector4();
    vector4.X = vector4.Y = vector4.Z = this.mIntensities[(int) this.mCurrentDataChannel];
    vector4.W = 1f;
    flashEffect.Color = vector4;
    flashEffect.CommitChanges();
    flashEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
  }

  public void PreRenderUpdate(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Matrix iViewProjectionMatrix,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    this.mCurrentDataChannel = iDataChannel;
  }

  public bool IsDead => (double) this.mIntensity <= 0.0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mIntensity -= iDeltaTime;
    this.mIntensities[(int) iDataChannel] = (float) ((double) this.mIntensity / (double) this.mTTL * 0.75);
    this.mScene.AddRenderableAdditiveObject(iDataChannel, (IRenderableAdditiveObject) this);
  }

  public void OnRemove()
  {
  }
}
