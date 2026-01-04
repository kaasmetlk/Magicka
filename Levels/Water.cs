// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Water
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Levels;

public class Water : Liquid
{
  private CollisionSkin mCollisionSkin;
  private IceMesh mIceCollisionMesh;
  private WaterMesh mWaterCollisionMesh;
  private RenderDeferredLiquidMaterial mMaterial;
  private VertexBuffer mWaterVertices;
  private DynamicVertexBuffer mWaterFreezeVertexBuffer;
  private float[] mWaterFreezeVertices;
  private IndexBuffer mWaterIndices;
  private VertexDeclaration mVertexDeclaration;
  private int mVertexStride;
  private int mNumVertices;
  private int mPrimitiveCount;
  private float mTime;
  private Matrix mTransform;
  private Matrix mInvTransform;
  private bool mFreezable;
  private bool mAutoFreeze;
  private Water.RenderData[] mRenderData;

  public Water(
    RenderDeferredLiquidEffect iEffect,
    ContentReader iInput,
    LevelModel iLevel,
    AnimatedLevelPart iParent)
    : base(iParent)
  {
    this.mMaterial.FetchFromEffect(iEffect);
    this.mWaterVertices = iInput.ReadObject<VertexBuffer>();
    this.mWaterIndices = iInput.ReadObject<IndexBuffer>();
    VertexElement[] vertexElements = iInput.ReadObject<VertexDeclaration>().GetVertexElements();
    VertexElement[] elements = new VertexElement[vertexElements.Length + 1];
    vertexElements.CopyTo((Array) elements, 0);
    elements[elements.Length - 1] = new VertexElement((short) 1, (short) 0, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 1);
    GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
    lock (graphicsDevice)
      this.mVertexDeclaration = new VertexDeclaration(graphicsDevice, elements);
    this.mVertexStride = iInput.ReadInt32();
    this.mNumVertices = iInput.ReadInt32();
    this.mPrimitiveCount = iInput.ReadInt32();
    int offsetInBytes1 = 0;
    int offsetInBytes2 = 0;
    for (int index = 0; index < vertexElements.Length; ++index)
    {
      if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position)
        offsetInBytes1 = (int) vertexElements[index].Offset;
      else if (vertexElements[index].VertexElementUsage == VertexElementUsage.Color)
        offsetInBytes2 = (int) vertexElements[index].Offset;
    }
    Vector3[] vector3Array = new Vector3[this.mNumVertices];
    this.mWaterVertices.GetData<Vector3>(offsetInBytes1, vector3Array, 0, this.mNumVertices, this.mVertexStride);
    float[] data1 = new float[this.mNumVertices];
    this.mWaterVertices.GetData<float>(offsetInBytes2, data1, 0, this.mNumVertices, this.mVertexStride);
    List<TriangleVertexIndices> triangleVertexIndicesList = new List<TriangleVertexIndices>();
    if (this.mWaterIndices.IndexElementSize == IndexElementSize.SixteenBits)
    {
      short[] data2 = new short[this.mPrimitiveCount * 3];
      this.mWaterIndices.GetData<short>(data2);
      int num1 = 0;
      while (num1 < data2.Length)
      {
        TriangleVertexIndices triangleVertexIndices;
        ref TriangleVertexIndices local1 = ref triangleVertexIndices;
        short[] numArray1 = data2;
        int index1 = num1;
        int num2 = index1 + 1;
        int num3 = (int) numArray1[index1];
        local1.I0 = num3;
        ref TriangleVertexIndices local2 = ref triangleVertexIndices;
        short[] numArray2 = data2;
        int index2 = num2;
        int num4 = index2 + 1;
        int num5 = (int) numArray2[index2];
        local2.I2 = num5;
        ref TriangleVertexIndices local3 = ref triangleVertexIndices;
        short[] numArray3 = data2;
        int index3 = num4;
        num1 = index3 + 1;
        int num6 = (int) numArray3[index3];
        local3.I1 = num6;
        triangleVertexIndicesList.Add(triangleVertexIndices);
      }
    }
    else
    {
      int[] data3 = new int[this.mPrimitiveCount * 3];
      this.mWaterIndices.GetData<int>(data3);
      int num7 = 0;
      while (num7 < data3.Length)
      {
        TriangleVertexIndices triangleVertexIndices;
        ref TriangleVertexIndices local4 = ref triangleVertexIndices;
        int[] numArray4 = data3;
        int index4 = num7;
        int num8 = index4 + 1;
        int num9 = numArray4[index4];
        local4.I0 = num9;
        ref TriangleVertexIndices local5 = ref triangleVertexIndices;
        int[] numArray5 = data3;
        int index5 = num8;
        int num10 = index5 + 1;
        int num11 = numArray5[index5];
        local5.I2 = num11;
        ref TriangleVertexIndices local6 = ref triangleVertexIndices;
        int[] numArray6 = data3;
        int index6 = num10;
        num7 = index6 + 1;
        int num12 = numArray6[index6];
        local6.I1 = num12;
        triangleVertexIndicesList.Add(triangleVertexIndices);
      }
    }
    this.mWaterFreezeVertices = new float[this.mNumVertices];
    lock (graphicsDevice)
      this.mWaterFreezeVertexBuffer = new DynamicVertexBuffer((iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice, 4 * this.mNumVertices, BufferUsage.WriteOnly);
    Transform identity = Transform.Identity;
    if (iInput.ReadBoolean())
    {
      this.mIceCollisionMesh = new IceMesh();
      this.mIceCollisionMesh.CreateMesh(vector3Array, this.mWaterFreezeVertices, triangleVertexIndicesList.ToArray(), 10, 1f);
      this.mIceCollisionMesh.SetTransform(ref identity);
      this.mWaterCollisionMesh = new WaterMesh();
      this.mWaterCollisionMesh.CreateMesh(vector3Array, this.mWaterFreezeVertices, triangleVertexIndicesList.ToArray(), 10, 1f);
      this.mWaterCollisionMesh.SetTransform(ref identity);
      this.mCollisionSkin = new CollisionSkin((Body) null);
      this.mCollisionSkin.callbackFn += new CollisionCallbackFn(this.OnCollision);
      this.mCollisionSkin.AddPrimitive((Primitive) this.mIceCollisionMesh, 1, new MaterialProperties(0.0f, 10f, 10f));
      this.mCollisionSkin.AddPrimitive((Primitive) this.mWaterCollisionMesh, 1, new MaterialProperties(0.0f, 10f, 10f));
      this.mCollisionSkin.ApplyLocalTransform(identity);
      this.mCollisionSkin.Tag = (object) this;
    }
    this.mRenderData = new Water.RenderData[3];
    int num = this.mMaterial.ReflectionMap != null ? 1 : 0;
    for (int index = 0; index < 3; ++index)
    {
      Water.RenderData renderData = new Water.RenderData(this.mNumVertices);
      this.mRenderData[index] = renderData;
      renderData.mPrimitiveCount = this.mPrimitiveCount;
      renderData.mNumVertices = this.mNumVertices;
      renderData.mVertexStride = this.mVertexStride;
      renderData.mVertices = this.mWaterVertices;
      renderData.mVerticesHash = this.mWaterVertices.GetHashCode();
      renderData.mFrozenVertexBuffer = (VertexBuffer) this.mWaterFreezeVertexBuffer;
      renderData.mIndices = this.mWaterIndices;
      renderData.mVertexDeclaration = this.mVertexDeclaration;
      renderData.mTechnique = num;
      renderData.mMaterial = this.mMaterial;
    }
    this.mFreezable = iInput.ReadBoolean();
    this.mAutoFreeze = iInput.ReadBoolean();
    if (!this.mAutoFreeze)
      return;
    for (int index = 0; index < this.mWaterFreezeVertices.Length; ++index)
      this.mWaterFreezeVertices[index] = 1f;
  }

  public override void Initialize()
  {
    if (this.mCollisionSkin == null || PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
      return;
    PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
  }

  private bool OnCollision(CollisionSkin skin0, int prim0, CollisionSkin skin1, int prim1)
  {
    return prim0 == 0 || skin1 != null && skin1.Owner != null && skin1.Owner.Tag is Magicka.GameLogic.Entities.Character;
  }

  public override CollisionSkin CollisionSkin => this.mCollisionSkin;

  internal override bool AutoFreeze => this.mAutoFreeze;

  public override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    Scene scene,
    ref Matrix iTransform,
    ref Matrix iInvTransform)
  {
    this.mTransform = iTransform;
    this.mInvTransform = iInvTransform;
    if (this.mCollisionSkin != null)
    {
      Transform newTransform = this.mCollisionSkin.NewTransform;
      Transform transformNew;
      transformNew.Position = iTransform.Translation;
      transformNew.Orientation = iTransform;
      transformNew.Orientation.Translation = new Vector3();
      this.mCollisionSkin.SetTransform(ref newTransform, ref transformNew);
    }
    if (iDataChannel == DataChannel.None)
      return;
    Water.RenderData iObject = this.mRenderData[(int) iDataChannel];
    float num = iDeltaTime * 0.1f;
    for (int index = 0; index < this.mWaterFreezeVertices.Length; ++index)
      this.mWaterFreezeVertices[index] = !this.mAutoFreeze ? MathHelper.Clamp(this.mWaterFreezeVertices[index] - num, 0.0f, 2f) : MathHelper.Clamp(this.mWaterFreezeVertices[index] + num, -1f, 1f);
    this.mTime += iDeltaTime;
    iObject.mMaterial.WorldTransform = iTransform;
    iObject.mTime = this.mTime;
    iObject.SetFrozenVertices(this.mWaterFreezeVertices);
    scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  public override unsafe bool SegmentIntersect(
    out float frac,
    out Vector3 pos,
    out Vector3 normal,
    ref Segment seg,
    bool ignoreBackfaces,
    bool ignoreWater,
    bool ignoreIce)
  {
    if (this.mCollisionSkin == null || ignoreIce && ignoreWater)
    {
      frac = 0.0f;
      pos = new Vector3();
      normal = new Vector3();
      return false;
    }
    Segment seg1;
    Vector3.Transform(ref seg.Origin, ref this.mInvTransform, out seg1.Origin);
    Vector3.TransformNormal(ref seg.Delta, ref this.mInvTransform, out seg1.Delta);
    BoundingBox initialBox = BoundingBoxHelper.InitialBox;
    BoundingBoxHelper.AddSegment(seg1, ref initialBox);
    int[] alloced = DetectFunctor.IntStackAlloc();
    fixed (int* triangles = alloced)
    {
      int intersectingtAaBox = this.mIceCollisionMesh.GetAllTrianglesIntersectingtAABox(triangles, 2048 /*0x0800*/, ref initialBox);
      pos = Vector3.Zero;
      normal = Vector3.Zero;
      float num = float.MaxValue;
      bool flag = false;
      IndexedTriangle indexedTriangle = new IndexedTriangle();
      for (int index = 0; index < intersectingtAaBox; ++index)
      {
        IndexedTriangle triangle1 = this.mIceCollisionMesh.GetTriangle(triangles[index]);
        int i0;
        int i1;
        int i2;
        triangle1.GetVertexIndices(out i0, out i1, out i2);
        if (ignoreWater)
        {
          if ((double) this.mWaterFreezeVertices[i0] < 0.5 && (double) this.mWaterFreezeVertices[i1] < 0.5 && (double) this.mWaterFreezeVertices[i2] < 0.5)
            continue;
        }
        else if (ignoreIce && ((double) this.mWaterFreezeVertices[i0] >= 0.5 || (double) this.mWaterFreezeVertices[i1] >= 0.5 || (double) this.mWaterFreezeVertices[i2] >= 0.5))
          continue;
        Vector3 result1;
        this.mIceCollisionMesh.GetVertex(i0, out result1);
        Vector3 result2;
        this.mIceCollisionMesh.GetVertex(i1, out result2);
        Vector3 result3;
        this.mIceCollisionMesh.GetVertex(i2, out result3);
        Triangle triangle2 = new Triangle(ref result1, ref result2, ref result3);
        float tS;
        if (Intersection.SegmentTriangleIntersection(out tS, out float _, out float _, ref seg1, ref triangle2, ignoreBackfaces) && (double) tS < (double) num)
        {
          flag = true;
          num = tS;
          indexedTriangle = triangle1;
          Vector3.Multiply(ref seg1.Delta, tS, out seg1.Delta);
        }
      }
      frac = num;
      if (flag)
      {
        seg1.GetEnd(out pos);
        normal = indexedTriangle.Plane.Normal;
      }
      DetectFunctor.FreeStackAlloc(alloced);
      return flag;
    }
  }

  protected override unsafe void Freeze(
    ref Vector3 iOrigin,
    ref Vector3 iDirection,
    float iSpread,
    float iMagnitude)
  {
    if (!this.mFreezable)
      return;
    Vector3 result1;
    Vector3.Transform(ref iOrigin, ref this.mInvTransform, out result1);
    Vector3 result2;
    Vector3.TransformNormal(ref iDirection, ref this.mInvTransform, out result2);
    int[] alloced = DetectFunctor.IntStackAlloc();
    fixed (int* numPtr = alloced)
    {
      int num1 = 0;
      int num2 = (int) ((double) iSpread / 0.78539818525314331);
      float iSpread1 = iSpread / (float) (num2 * 2 + 1);
      Segment seg;
      seg.Origin = result1;
      for (int index1 = -num2; index1 <= num2; ++index1)
      {
        Quaternion result3;
        Quaternion.CreateFromYawPitchRoll((float) index1 * iSpread1, 0.0f, 0.0f, out result3);
        Vector3.Transform(ref result2, ref result3, out seg.Delta);
        if (GameStateManager.Instance.CurrentState is PlayState currentState)
        {
          List<Shield> shields = currentState.EntityManager.Shields;
          for (int index2 = 0; index2 < shields.Count; ++index2)
          {
            float frac;
            if (shields[index2].Body.CollisionSkin.SegmentIntersect(out frac, out Vector3 _, out Vector3 _, seg))
              Vector3.Multiply(ref seg.Delta, frac, out seg.Delta);
          }
        }
        int num3 = num1;
        num1 += this.mIceCollisionMesh.GetVerticesIntersectingArc(numPtr + num1, 2048 /*0x0800*/ - num1, ref result1, ref seg.Delta, iSpread1);
        for (int index3 = 0; index3 < num3; ++index3)
        {
          for (int index4 = num3 + 1; index4 < num1; ++index4)
          {
            if (numPtr[index3] == numPtr[index4])
              numPtr[index4] = -1;
          }
        }
      }
      for (int index = 0; index < num1; ++index)
      {
        if (numPtr[index] >= 0)
          this.mWaterFreezeVertices[numPtr[index]] += iMagnitude;
      }
    }
    DetectFunctor.FreeStackAlloc(alloced);
  }

  public override void FreezeAll(float iMagnitude)
  {
    if (!this.mFreezable)
      return;
    for (int index = 0; index < this.mWaterFreezeVertices.Length; ++index)
      this.mWaterFreezeVertices[index] += iMagnitude;
  }

  protected class RenderData : IRenderableObject
  {
    public float mTime;
    public int mTechnique;
    public RenderDeferredLiquidMaterial mMaterial;
    public VertexBuffer mVertices;
    public VertexBuffer mFrozenVertexBuffer;
    public int mVerticesHash;
    public IndexBuffer mIndices;
    public VertexDeclaration mVertexDeclaration;
    public int mVertexStride;
    public int mNumVertices;
    public int mPrimitiveCount;
    private float[] mFrozenVertices;

    public RenderData(int iVertexCount) => this.mFrozenVertices = new float[iVertexCount];

    public void SetFrozenVertices(float[] iFrozenVertices)
    {
      iFrozenVertices.CopyTo((Array) this.mFrozenVertices, 0);
    }

    public int Effect => RenderDeferredLiquidEffect.TYPEHASH;

    public int DepthTechnique => 2;

    public int Technique => this.mTechnique;

    public int ShadowTechnique => 3;

    public VertexBuffer Vertices => this.mVertices;

    public int VertexStride => this.mVertexStride;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public int VerticesHashCode => this.mVerticesHash;

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredLiquidEffect iEffect1 = iEffect as RenderDeferredLiquidEffect;
      this.mFrozenVertexBuffer.SetData<float>(this.mFrozenVertices);
      iEffect1.GraphicsDevice.Vertices[1].SetSource(this.mFrozenVertexBuffer, 0, 4);
      iEffect1.GraphicsDevice.RenderState.ReferenceStencil = 3;
      this.mMaterial.AssignToEffect(iEffect1);
      iEffect1.Time = this.mTime;
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
      iEffect1.GraphicsDevice.Vertices[1].SetSource((VertexBuffer) null, 0, 0);
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      RenderDeferredLiquidEffect iEffect1 = iEffect as RenderDeferredLiquidEffect;
      this.mMaterial.AssignOpacityToEffect(iEffect1);
      iEffect1.CommitChanges();
      iEffect1.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
    }
  }
}
