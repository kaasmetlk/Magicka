// Decompiled with JetBrains decompiler
// Type: PolygonHead.ProjectionObject
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead.Effects;

#nullable disable
namespace PolygonHead;

public class ProjectionObject : IProjectionObject
{
  private static VertexBuffer sVertices;
  private static VertexDeclaration sVertexDeclaration;
  private static IndexBuffer sIndices;
  private static int sVerticesHash;
  protected Matrix mTransform;
  protected Matrix mProjection;
  protected Vector2 mTextureOffset;
  protected Vector2 mTextureScale;
  protected float mGlow;
  protected Texture2D mTexture;
  protected Texture2D mNormalMap;

  public ProjectionObject(GraphicsDevice iDevice, Texture2D iTexture, Texture2D iNormalMap)
  {
    if (ProjectionObject.sVertices == null)
    {
      Vector3[] data1 = new Vector3[8];
      for (int index = 0; index < data1.Length; ++index)
        data1[index] = new Vector3(index % 2 < 1 ? -1f : 1f, index % 4 < 2 ? -1f : 1f, index % 8 < 4 ? -1f : 1f);
      ProjectionObject.sVertices = new VertexBuffer(iDevice, data1.Length * 3 * 4, BufferUsage.WriteOnly);
      ProjectionObject.sVertices.SetData<Vector3>(data1);
      ushort[] data2 = new ushort[36]
      {
        (ushort) 0,
        (ushort) 2,
        (ushort) 1,
        (ushort) 1,
        (ushort) 2,
        (ushort) 3,
        (ushort) 4,
        (ushort) 5,
        (ushort) 6,
        (ushort) 5,
        (ushort) 7,
        (ushort) 6,
        (ushort) 0,
        (ushort) 4,
        (ushort) 2,
        (ushort) 2,
        (ushort) 4,
        (ushort) 6,
        (ushort) 2,
        (ushort) 6,
        (ushort) 3,
        (ushort) 3,
        (ushort) 6,
        (ushort) 7,
        (ushort) 0,
        (ushort) 1,
        (ushort) 4,
        (ushort) 4,
        (ushort) 1,
        (ushort) 5,
        (ushort) 1,
        (ushort) 3,
        (ushort) 7,
        (ushort) 7,
        (ushort) 5,
        (ushort) 1
      };
      ProjectionObject.sIndices = new IndexBuffer(iDevice, data2.Length * 2, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      ProjectionObject.sIndices.SetData<ushort>(data2);
      ProjectionObject.sVertexDeclaration = new VertexDeclaration(iDevice, new VertexElement[1]
      {
        new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0)
      });
      ProjectionObject.sVerticesHash = ProjectionObject.sVertices.GetHashCode();
      if (!(RenderManager.Instance.GetEffect(ProjectionEffect.TYPEHASH) is ProjectionEffect))
        RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) new ProjectionEffect(iDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
    }
    this.mTexture = iTexture;
    this.mNormalMap = iNormalMap;
    this.mTransform = Matrix.Identity;
    this.mProjection = Matrix.Identity;
    this.mTextureScale = Vector2.One;
  }

  public void SetPosition(ref Matrix iTransform)
  {
    this.mTransform = iTransform;
    Matrix.Invert(ref iTransform, out this.mProjection);
  }

  public Vector2 TextureScale
  {
    get => this.mTextureScale;
    set => this.mTextureScale = value;
  }

  public Vector2 TextureOffset
  {
    get => this.mTextureOffset;
    set => this.mTextureOffset = value;
  }

  public int Effect => ProjectionEffect.TYPEHASH;

  public int Technique => 0;

  public VertexBuffer Vertices => ProjectionObject.sVertices;

  public int VerticesHashCode => ProjectionObject.sVerticesHash;

  public int VertexStride => 12;

  public IndexBuffer Indices => ProjectionObject.sIndices;

  public VertexDeclaration VertexDeclaration => ProjectionObject.sVertexDeclaration;

  public float Glow
  {
    get => this.mGlow;
    set => this.mGlow = value;
  }

  public bool Cull(BoundingFrustum iViewFrustum) => false;

  public virtual void Draw(Microsoft.Xna.Framework.Graphics.Effect iEffect, Texture2D iDepthMap)
  {
    ProjectionEffect projectionEffect = iEffect as ProjectionEffect;
    projectionEffect.World = this.mTransform;
    projectionEffect.Texture = this.mTexture;
    projectionEffect.NormalMap = this.mNormalMap;
    projectionEffect.NormalMapEnabled = this.mNormalMap != null;
    projectionEffect.DepthMap = iDepthMap;
    projectionEffect.InvProjectedMatrix = this.mProjection;
    projectionEffect.Glow = this.mGlow;
    projectionEffect.TextureOffset = this.mTextureOffset;
    projectionEffect.TextureScale = this.mTextureScale;
    projectionEffect.BumpScale = 1f;
    projectionEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
    projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.None;
    projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.None;
    projectionEffect.CommitChanges();
    projectionEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
    projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels1 = ColorWriteChannels.All;
    projectionEffect.GraphicsDevice.RenderState.ColorWriteChannels2 = ColorWriteChannels.All;
    projectionEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
  }
}
