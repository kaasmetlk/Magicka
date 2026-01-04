// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.ForceField
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics.Effects;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.Levels;

internal class ForceField
{
  private ForceField.RenderData[] mRenderData;
  private ForceFieldMaterial mMaterial;
  private float mTTL;
  private Vector4[] mCollPoints = new Vector4[32 /*0x20*/];
  private VertexBuffer mVertices;
  private IndexBuffer mIndices;
  private VertexDeclaration mDeclaration;
  private int mVertexStride;
  private int mNumVertices;
  private int mPrimitiveCount;
  private CollisionSkin mCollision;
  private PlayState mPlayState;

  internal ForceField(ContentReader iInput, LevelModel iLevel)
  {
    this.mMaterial.Color = iInput.ReadVector3();
    this.mMaterial.Width = iInput.ReadSingle();
    this.mMaterial.AlphaPower = iInput.ReadSingle();
    this.mMaterial.AlphaFalloffPower = iInput.ReadSingle();
    this.mMaterial.MaxRadius = iInput.ReadSingle();
    this.mMaterial.RippleDistortion = iInput.ReadSingle();
    this.mMaterial.MapDistortion = iInput.ReadSingle();
    this.mMaterial.VertexColorEnabled = iInput.ReadBoolean();
    this.mMaterial.DisplacementMap = iInput.ReadExternalReference<Texture2D>();
    this.mTTL = iInput.ReadSingle();
    this.mVertices = iInput.ReadObject<VertexBuffer>();
    this.mIndices = iInput.ReadObject<IndexBuffer>();
    this.mDeclaration = iInput.ReadObject<VertexDeclaration>();
    this.mVertexStride = iInput.ReadInt32();
    this.mNumVertices = iInput.ReadInt32();
    this.mPrimitiveCount = iInput.ReadInt32();
    VertexElement[] vertexElements = this.mDeclaration.GetVertexElements();
    int offsetInBytes = -1;
    for (int index = 0; index < vertexElements.Length; ++index)
    {
      if (vertexElements[index].VertexElementUsage == VertexElementUsage.Position && vertexElements[index].UsageIndex == (byte) 0)
      {
        if (vertexElements[index].VertexElementFormat != VertexElementFormat.Vector3)
          throw new Exception($"Unsupported vertex format: \"{(object) vertexElements[index].VertexElementFormat}\"");
        offsetInBytes = (int) vertexElements[index].Offset;
        break;
      }
    }
    if (offsetInBytes < 0)
      throw new Exception("Vertices contain no position?!");
    Vector3[] vector3Array = new Vector3[this.mNumVertices];
    this.mVertices.GetData<Vector3>(offsetInBytes, vector3Array, 0, this.mNumVertices, this.mVertexStride);
    TriangleVertexIndices[] triangleVertexIndices1 = new TriangleVertexIndices[this.mPrimitiveCount];
    if (this.mIndices.IndexElementSize == IndexElementSize.SixteenBits)
    {
      ushort[] data = new ushort[this.mIndices.SizeInBytes / 2];
      this.mIndices.GetData<ushort>(data);
      int num1 = 0;
      while (num1 < data.Length)
      {
        int index1 = num1 / 3;
        TriangleVertexIndices triangleVertexIndices2;
        ref TriangleVertexIndices local1 = ref triangleVertexIndices2;
        ushort[] numArray1 = data;
        int index2 = num1;
        int num2 = index2 + 1;
        int num3 = (int) numArray1[index2];
        local1.I0 = num3;
        ref TriangleVertexIndices local2 = ref triangleVertexIndices2;
        ushort[] numArray2 = data;
        int index3 = num2;
        int num4 = index3 + 1;
        int num5 = (int) numArray2[index3];
        local2.I2 = num5;
        ref TriangleVertexIndices local3 = ref triangleVertexIndices2;
        ushort[] numArray3 = data;
        int index4 = num4;
        num1 = index4 + 1;
        int num6 = (int) numArray3[index4];
        local3.I1 = num6;
        triangleVertexIndices1[index1] = triangleVertexIndices2;
      }
    }
    else if (this.mIndices.IndexElementSize == IndexElementSize.ThirtyTwoBits)
    {
      int[] data = new int[this.mIndices.SizeInBytes / 4];
      this.mIndices.GetData<int>(data);
      int num7 = 0;
      while (num7 < data.Length)
      {
        int index5 = num7 / 3;
        TriangleVertexIndices triangleVertexIndices3;
        ref TriangleVertexIndices local4 = ref triangleVertexIndices3;
        int[] numArray4 = data;
        int index6 = num7;
        int num8 = index6 + 1;
        int num9 = numArray4[index6];
        local4.I0 = num9;
        ref TriangleVertexIndices local5 = ref triangleVertexIndices3;
        int[] numArray5 = data;
        int index7 = num8;
        int num10 = index7 + 1;
        int num11 = numArray5[index7];
        local5.I2 = num11;
        ref TriangleVertexIndices local6 = ref triangleVertexIndices3;
        int[] numArray6 = data;
        int index8 = num10;
        num7 = index8 + 1;
        int num12 = numArray6[index8];
        local6.I1 = num12;
        triangleVertexIndices1[index5] = triangleVertexIndices3;
      }
    }
    this.mCollision = new CollisionSkin();
    this.mCollision.Tag = (object) iLevel;
    TriangleMesh prim = new TriangleMesh();
    prim.CreateMesh(vector3Array, triangleVertexIndices1, 8, 1f);
    this.mCollision.AddPrimitive((Primitive) prim, 1, new MaterialProperties(1f, 1f, 1f));
    this.mCollision.ApplyLocalTransform(Transform.Identity);
    this.mCollision.postCollisionCallbackFn += new PostCollisionCallbackFn(this.mCollision_postCollisionCallbackFn);
    if (!(RenderManager.Instance.GetEffect(ForceFieldEffect.TYPEHASH) is ForceFieldEffect))
      RenderManager.Instance.RegisterEffect((Effect) new ForceFieldEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content));
    this.mRenderData = new ForceField.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new ForceField.RenderData(this.mVertices, this.mIndices, this.mDeclaration, this.mVertexStride, this.mNumVertices, this.mPrimitiveCount);
      this.mRenderData[index].Material = this.mMaterial;
    }
  }

  internal CollisionSkin CollisionSkin => this.mCollision;

  private void mCollision_postCollisionCallbackFn(ref CollisionInfo iInfo)
  {
    if (iInfo.NumCollPts <= 0)
      return;
    Vector3 result1 = new Vector3();
    Vector3 vector3 = new Vector3();
    if (iInfo.SkinInfo.Skin0 == this.mCollision)
    {
      for (int index = 0; index < iInfo.NumCollPts; ++index)
        Vector3.Add(ref result1, ref iInfo.PointInfo[index].info.R0, out result1);
    }
    else
    {
      for (int index = 0; index < iInfo.NumCollPts; ++index)
        Vector3.Add(ref result1, ref iInfo.PointInfo[index].info.R1, out result1);
    }
    Vector3.Divide(ref result1, (float) iInfo.NumCollPts, out result1);
    int index1 = -1;
    for (int index2 = 0; index2 < this.mCollPoints.Length; ++index2)
    {
      if (index1 < 0 && (double) this.mCollPoints[index2].W >= (double) this.mTTL)
        index1 = index2;
      else if ((double) this.mCollPoints[index2].W < (double) this.mTTL * 0.5)
      {
        vector3.X = this.mCollPoints[index2].X;
        vector3.Y = this.mCollPoints[index2].Y;
        vector3.Z = this.mCollPoints[index2].Z;
        float result2;
        Vector3.Distance(ref result1, ref vector3, out result2);
        if ((double) result2 < (double) this.mMaterial.MaxRadius * 0.5)
          return;
      }
    }
    if (index1 < 0)
      return;
    this.mCollPoints[index1] = new Vector4()
    {
      X = result1.X,
      Y = result1.Y,
      Z = result1.Z
    };
  }

  public void Initialize(PlayState iPlayState)
  {
    if (!PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollision))
      PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollision);
    for (int index = 0; index < this.mCollPoints.Length; ++index)
      this.mCollPoints[index].W = float.MaxValue;
    this.mPlayState = iPlayState;
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (iDataChannel == DataChannel.None)
      return;
    float num = iDeltaTime / this.mTTL;
    for (int index = 0; index < this.mCollPoints.Length; ++index)
      this.mCollPoints[index].W += num;
    ForceField.RenderData iObject = this.mRenderData[(int) iDataChannel];
    this.mCollPoints.CopyTo((Array) iObject.CollPoints, 0);
    this.mPlayState.Scene.AddPostEffect(iDataChannel, (IPostEffect) iObject);
  }

  private class RenderData : IPostEffect
  {
    public Vector4[] CollPoints = new Vector4[32 /*0x20*/];
    public ForceFieldMaterial Material;
    private VertexBuffer mVertices;
    private IndexBuffer mIndices;
    private VertexDeclaration mDeclaration;
    private int mVertexStride;
    private int mNumVertices;
    private int mPrimitiveCount;

    public RenderData(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      VertexDeclaration iDeclaration,
      int iVertexStride,
      int iNumVertices,
      int iPrimitiveCount)
    {
      this.mVertices = iVertices;
      this.mIndices = iIndices;
      this.mDeclaration = iDeclaration;
      this.mVertexStride = iVertexStride;
      this.mNumVertices = iNumVertices;
      this.mPrimitiveCount = iPrimitiveCount;
    }

    public int ZIndex => 0;

    public void Draw(
      float iDeltaTime,
      ref Vector2 iPixelSize,
      ref Matrix iViewMatrix,
      ref Matrix iProjectionMatrix,
      Texture2D iCandidate,
      Texture2D iDepthMap,
      Texture2D iNormalMap)
    {
      ForceFieldEffect effect = RenderManager.Instance.GetEffect(ForceFieldEffect.TYPEHASH) as ForceFieldEffect;
      this.Material.AssignToEffect(effect);
      effect.CollPoints = this.CollPoints;
      effect.View = iViewMatrix;
      effect.Projection = iProjectionMatrix;
      effect.World = Matrix.Identity;
      RenderState renderState = effect.GraphicsDevice.RenderState;
      renderState.CullMode = CullMode.None;
      renderState.AlphaBlendEnable = false;
      effect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, this.mVertexStride);
      effect.GraphicsDevice.VertexDeclaration = this.mDeclaration;
      effect.GraphicsDevice.Indices = this.mIndices;
      effect.DestinationDimentions = new Point()
      {
        X = iDepthMap.Width,
        Y = iDepthMap.Height
      };
      effect.Candidate = iCandidate;
      effect.DepthMap = iDepthMap;
      effect.Begin();
      effect.CurrentTechnique.Passes[0].Begin();
      effect.GraphicsDevice.DrawIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, 0, this.mNumVertices, 0, this.mPrimitiveCount);
      effect.CurrentTechnique.Passes[0].End();
      effect.End();
    }
  }
}
