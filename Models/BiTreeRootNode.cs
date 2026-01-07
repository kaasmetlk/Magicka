// Decompiled with JetBrains decompiler
// Type: PolygonHead.Models.BiTreeRootNode
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Models;

public class BiTreeRootNode : BiTreeNode
{
  protected bool mVisible;
  protected bool mCastShadows;
  protected float mSway;
  protected float mEntityInfluence;
  protected float mGroundLevel;
  protected Effect mEffect;
  protected VertexDeclaration mVertexDeclaration;
  protected VertexBuffer mVertexBuffer;
  protected IndexBuffer mIndexBuffer;

  public BiTreeRootNode(ContentReader iInput, GraphicsDevice iDevice)
  {
    this.mDevice = iDevice;
    this.mVisible = iInput.ReadBoolean();
    this.mCastShadows = iInput.ReadBoolean();
    this.mSway = iInput.ReadSingle();
    this.mEntityInfluence = iInput.ReadSingle();
    this.mGroundLevel = iInput.ReadSingle();
    this.mNumVertices = iInput.ReadInt32();
    this.mVertexStride = iInput.ReadInt32();
    VertexElement[] vertexElementArray = new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    };
    lock (iDevice)
    {
      this.mVertexDeclaration = iInput.ReadObject<VertexDeclaration>();
      this.mVertexBuffer = iInput.ReadObject<VertexBuffer>();
      this.mIndexBuffer = iInput.ReadObject<IndexBuffer>();
      this.mEffect = iInput.ReadObject<Effect>();
    }
    this.mPrimitiveCount = iInput.ReadInt32();
    this.mStartIndex = iInput.ReadInt32();
    this.mBoundingBox.Min = iInput.ReadVector3();
    this.mBoundingBox.Max = iInput.ReadVector3();
    if (iInput.ReadBoolean())
      this.mChildA = new BiTreeNode(iInput, this.mNumVertices, iDevice);
    if (!iInput.ReadBoolean())
      return;
    this.mChildB = new BiTreeNode(iInput, this.mNumVertices, iDevice);
  }

  public Effect Effect => this.mEffect;

  public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public VertexBuffer VertexBuffer => this.mVertexBuffer;

  public IndexBuffer IndexBuffer => this.mIndexBuffer;

  public bool Visible => this.mVisible;

  public bool CastShadows => this.mCastShadows;

  public float Sway => this.mSway;

  public float EntityInfluence => this.mEntityInfluence;

  public float GroundLevel => this.mGroundLevel;
}
