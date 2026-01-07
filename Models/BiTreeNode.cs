// Decompiled with JetBrains decompiler
// Type: PolygonHead.Models.BiTreeNode
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace PolygonHead.Models;

public class BiTreeNode
{
  protected int mNumVertices;
  protected int mPrimitiveCount;
  protected int mStartIndex;
  protected int mVertexStride;
  protected BoundingBox mBoundingBox;
  protected BiTreeNode mChildA;
  protected BiTreeNode mChildB;
  protected GraphicsDevice mDevice;

  protected BiTreeNode()
  {
  }

  public BiTreeNode(ContentReader iInput, int iNumVertices, GraphicsDevice iDevice)
  {
    this.mDevice = iDevice;
    this.mNumVertices = iNumVertices;
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

  public virtual ContainmentType Draw(BoundingFrustum iFrustum)
  {
    ContainmentType result;
    iFrustum.Contains(ref this.mBoundingBox, out result);
    switch (result)
    {
      case ContainmentType.Contains:
        this.Draw();
        break;
      case ContainmentType.Intersects:
        if (this.mChildA != null && this.mChildB != null)
        {
          int num1 = (int) this.mChildA.Draw(iFrustum);
          int num2 = (int) this.mChildB.Draw(iFrustum);
          break;
        }
        this.Draw();
        break;
    }
    return result;
  }

  public virtual void Draw()
  {
    this.mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
  }

  public BoundingBox BoundingBox => this.mBoundingBox;

  public int VertexStride => this.mVertexStride;
}
