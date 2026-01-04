// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.DecalManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using System;

#nullable disable
namespace Magicka.Graphics;

public sealed class DecalManager
{
  private static DecalManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private Matrix[] mAlphaTransforms;
  private Matrix[] mAlphaInvTransforms;
  private Vector3[] mAlphaArgs;
  private Vector4[] mAlphaColors;
  private int[] mAlphaUniqueIDs;
  private int mLastAlphaBlendedDecal = -1;
  private VertexBuffer mVertices;
  private IndexBuffer mIndices;
  private VertexDeclaration mVertexDeclaration;
  private IntHeap mFreeAlphaBlendedDecals;
  private Random mRandom = new Random();
  private DecalManager.RenderData[] mAlphaBlendedRenderData;
  private Scene mScene;
  private Texture2D mAlphaDecals;
  private static readonly Vector2 sDecals = new Vector2(8f, 8f);

  public static DecalManager Instance
  {
    get
    {
      if (DecalManager.mSingelton == null)
      {
        lock (DecalManager.mSingeltonLock)
        {
          if (DecalManager.mSingelton == null)
            DecalManager.mSingelton = new DecalManager();
        }
      }
      return DecalManager.mSingelton;
    }
  }

  private DecalManager()
  {
    RenderManager.Instance.RegisterEffect((Effect) new HardwareInstancedProjectionEffect(Magicka.Game.Instance.GraphicsDevice, RenderManager.Instance.GlobalDummyEffect.EffectPool));
    this.mAlphaDecals = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/decals");
    this.mAlphaBlendedRenderData = new DecalManager.RenderData[3];
  }

  public void Initialize(Scene iScene, int iMaxDecals)
  {
    this.mScene = iScene;
    if (iScene == null)
      return;
    this.mLastAlphaBlendedDecal = -1;
    this.mFreeAlphaBlendedDecals = new IntHeap(iMaxDecals);
    this.mAlphaUniqueIDs = new int[iMaxDecals];
    this.Clear();
    this.mAlphaTransforms = new Matrix[iMaxDecals];
    this.mAlphaInvTransforms = new Matrix[iMaxDecals];
    this.mAlphaArgs = new Vector3[iMaxDecals];
    this.mAlphaColors = new Vector4[iMaxDecals];
    this.mAlphaUniqueIDs = new int[iMaxDecals];
    for (int index = 0; index < 3; ++index)
    {
      DecalManager.RenderData renderData = new DecalManager.RenderData(iMaxDecals);
      this.mAlphaBlendedRenderData[index] = renderData;
      renderData.mTextureScale = DecalManager.sDecals;
      renderData.mTexture = this.mAlphaDecals;
    }
    this.CreateVertexBuffer();
  }

  internal void Clear()
  {
    this.mLastAlphaBlendedDecal = -1;
    this.mFreeAlphaBlendedDecals.Clear();
    for (int iValue = 0; iValue < this.mAlphaUniqueIDs.Length; ++iValue)
    {
      this.mFreeAlphaBlendedDecals.Push(iValue);
      this.mAlphaUniqueIDs[iValue] = -1;
    }
  }

  public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

  public Scene Scene => this.mScene;

  private void CreateVertexBuffer()
  {
    if (this.mVertices != null && !this.mVertices.IsDisposed)
      this.mVertices.Dispose();
    if (this.mVertexDeclaration != null && !this.mVertexDeclaration.IsDisposed)
      this.mVertexDeclaration.Dispose();
    if (this.mIndices != null && !this.mIndices.IsDisposed)
      this.mIndices.Dispose();
    DecalManager.Vertex[] data1 = new DecalManager.Vertex[240 /*0xF0*/];
    ushort[] data2 = new ushort[1080];
    for (int index1 = 0; index1 < 30; ++index1)
    {
      int num1 = index1 * 8;
      for (int index2 = 0; index2 < 8; ++index2)
      {
        data1[num1 + index2].Position.X = index2 % 2 < 1 ? -0.5f : 0.5f;
        data1[num1 + index2].Position.Y = index2 % 4 < 2 ? -0.5f : 0.5f;
        data1[num1 + index2].Position.Z = index2 % 8 < 4 ? -0.5f : 0.5f;
        data1[num1 + index2].Instance = (float) index1;
      }
      int num2 = index1 * 36;
      ushort[] numArray1 = data2;
      int index3 = num2;
      int num3 = index3 + 1;
      int num4 = (int) (ushort) num1;
      numArray1[index3] = (ushort) num4;
      ushort[] numArray2 = data2;
      int index4 = num3;
      int num5 = index4 + 1;
      int num6 = (int) (ushort) (num1 + 2);
      numArray2[index4] = (ushort) num6;
      ushort[] numArray3 = data2;
      int index5 = num5;
      int num7 = index5 + 1;
      int num8 = (int) (ushort) (num1 + 1);
      numArray3[index5] = (ushort) num8;
      ushort[] numArray4 = data2;
      int index6 = num7;
      int num9 = index6 + 1;
      int num10 = (int) (ushort) (num1 + 1);
      numArray4[index6] = (ushort) num10;
      ushort[] numArray5 = data2;
      int index7 = num9;
      int num11 = index7 + 1;
      int num12 = (int) (ushort) (num1 + 2);
      numArray5[index7] = (ushort) num12;
      ushort[] numArray6 = data2;
      int index8 = num11;
      int num13 = index8 + 1;
      int num14 = (int) (ushort) (num1 + 3);
      numArray6[index8] = (ushort) num14;
      ushort[] numArray7 = data2;
      int index9 = num13;
      int num15 = index9 + 1;
      int num16 = (int) (ushort) (num1 + 1);
      numArray7[index9] = (ushort) num16;
      ushort[] numArray8 = data2;
      int index10 = num15;
      int num17 = index10 + 1;
      int num18 = (int) (ushort) num1;
      numArray8[index10] = (ushort) num18;
      ushort[] numArray9 = data2;
      int index11 = num17;
      int num19 = index11 + 1;
      int num20 = (int) (ushort) (num1 + 4);
      numArray9[index11] = (ushort) num20;
      ushort[] numArray10 = data2;
      int index12 = num19;
      int num21 = index12 + 1;
      int num22 = (int) (ushort) (num1 + 4);
      numArray10[index12] = (ushort) num22;
      ushort[] numArray11 = data2;
      int index13 = num21;
      int num23 = index13 + 1;
      int num24 = (int) (ushort) (num1 + 5);
      numArray11[index13] = (ushort) num24;
      ushort[] numArray12 = data2;
      int index14 = num23;
      int num25 = index14 + 1;
      int num26 = (int) (ushort) (num1 + 1);
      numArray12[index14] = (ushort) num26;
      ushort[] numArray13 = data2;
      int index15 = num25;
      int num27 = index15 + 1;
      int num28 = (int) (ushort) num1;
      numArray13[index15] = (ushort) num28;
      ushort[] numArray14 = data2;
      int index16 = num27;
      int num29 = index16 + 1;
      int num30 = (int) (ushort) (num1 + 2);
      numArray14[index16] = (ushort) num30;
      ushort[] numArray15 = data2;
      int index17 = num29;
      int num31 = index17 + 1;
      int num32 = (int) (ushort) (num1 + 4);
      numArray15[index17] = (ushort) num32;
      ushort[] numArray16 = data2;
      int index18 = num31;
      int num33 = index18 + 1;
      int num34 = (int) (ushort) (num1 + 4);
      numArray16[index18] = (ushort) num34;
      ushort[] numArray17 = data2;
      int index19 = num33;
      int num35 = index19 + 1;
      int num36 = (int) (ushort) (num1 + 2);
      numArray17[index19] = (ushort) num36;
      ushort[] numArray18 = data2;
      int index20 = num35;
      int num37 = index20 + 1;
      int num38 = (int) (ushort) (num1 + 6);
      numArray18[index20] = (ushort) num38;
      ushort[] numArray19 = data2;
      int index21 = num37;
      int num39 = index21 + 1;
      int num40 = (int) (ushort) (num1 + 4);
      numArray19[index21] = (ushort) num40;
      ushort[] numArray20 = data2;
      int index22 = num39;
      int num41 = index22 + 1;
      int num42 = (int) (ushort) (num1 + 6);
      numArray20[index22] = (ushort) num42;
      ushort[] numArray21 = data2;
      int index23 = num41;
      int num43 = index23 + 1;
      int num44 = (int) (ushort) (num1 + 5);
      numArray21[index23] = (ushort) num44;
      ushort[] numArray22 = data2;
      int index24 = num43;
      int num45 = index24 + 1;
      int num46 = (int) (ushort) (num1 + 5);
      numArray22[index24] = (ushort) num46;
      ushort[] numArray23 = data2;
      int index25 = num45;
      int num47 = index25 + 1;
      int num48 = (int) (ushort) (num1 + 6);
      numArray23[index25] = (ushort) num48;
      ushort[] numArray24 = data2;
      int index26 = num47;
      int num49 = index26 + 1;
      int num50 = (int) (ushort) (num1 + 7);
      numArray24[index26] = (ushort) num50;
      ushort[] numArray25 = data2;
      int index27 = num49;
      int num51 = index27 + 1;
      int num52 = (int) (ushort) (num1 + 1);
      numArray25[index27] = (ushort) num52;
      ushort[] numArray26 = data2;
      int index28 = num51;
      int num53 = index28 + 1;
      int num54 = (int) (ushort) (num1 + 5);
      numArray26[index28] = (ushort) num54;
      ushort[] numArray27 = data2;
      int index29 = num53;
      int num55 = index29 + 1;
      int num56 = (int) (ushort) (num1 + 3);
      numArray27[index29] = (ushort) num56;
      ushort[] numArray28 = data2;
      int index30 = num55;
      int num57 = index30 + 1;
      int num58 = (int) (ushort) (num1 + 3);
      numArray28[index30] = (ushort) num58;
      ushort[] numArray29 = data2;
      int index31 = num57;
      int num59 = index31 + 1;
      int num60 = (int) (ushort) (num1 + 5);
      numArray29[index31] = (ushort) num60;
      ushort[] numArray30 = data2;
      int index32 = num59;
      int num61 = index32 + 1;
      int num62 = (int) (ushort) (num1 + 7);
      numArray30[index32] = (ushort) num62;
      ushort[] numArray31 = data2;
      int index33 = num61;
      int num63 = index33 + 1;
      int num64 = (int) (ushort) (num1 + 2);
      numArray31[index33] = (ushort) num64;
      ushort[] numArray32 = data2;
      int index34 = num63;
      int num65 = index34 + 1;
      int num66 = (int) (ushort) (num1 + 3);
      numArray32[index34] = (ushort) num66;
      ushort[] numArray33 = data2;
      int index35 = num65;
      int num67 = index35 + 1;
      int num68 = (int) (ushort) (num1 + 6);
      numArray33[index35] = (ushort) num68;
      ushort[] numArray34 = data2;
      int index36 = num67;
      int num69 = index36 + 1;
      int num70 = (int) (ushort) (num1 + 6);
      numArray34[index36] = (ushort) num70;
      ushort[] numArray35 = data2;
      int index37 = num69;
      int num71 = index37 + 1;
      int num72 = (int) (ushort) (num1 + 3);
      numArray35[index37] = (ushort) num72;
      ushort[] numArray36 = data2;
      int index38 = num71;
      int num73 = index38 + 1;
      int num74 = (int) (ushort) (num1 + 7);
      numArray36[index38] = (ushort) num74;
    }
    this.mVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 3840 /*0x0F00*/, BufferUsage.WriteOnly);
    this.mVertices.SetData<DecalManager.Vertex>(data1);
    this.mIndices = new IndexBuffer(Magicka.Game.Instance.GraphicsDevice, 2160, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
    this.mIndices.SetData<ushort>(data2);
    this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, DecalManager.Vertex.VertexElements);
    for (int index = 0; index < 3; ++index)
    {
      DecalManager.RenderData renderData = this.mAlphaBlendedRenderData[index];
      renderData.mVertices = this.mVertices;
      renderData.mVerticesHash = this.mVertices.GetHashCode();
      renderData.mIndices = this.mIndices;
      renderData.mVertexDeclaration = this.mVertexDeclaration;
    }
  }

  public void AddAlphaBlendedDecal(
    Decal iDecal,
    AnimatedLevelPart iAnimation,
    float iScale,
    ref Vector3 iPosition,
    ref Vector3 iNormal,
    float iTTL)
  {
    Vector2 iScale1 = new Vector2();
    iScale1.X = iScale1.Y = iScale;
    this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale1, ref iPosition, new Vector3?(), ref iNormal, iTTL, 1f, out DecalManager.DecalReference _);
  }

  public void AddAlphaBlendedDecal(
    Decal iDecal,
    AnimatedLevelPart iAnimation,
    ref Vector2 iScale,
    ref Vector3 iPosition,
    Vector3? iDirection,
    ref Vector3 iNormal,
    float iTTL)
  {
    this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, 1f, out DecalManager.DecalReference _);
  }

  public void AddAlphaBlendedDecal(
    Decal iDecal,
    AnimatedLevelPart iAnimation,
    ref Vector2 iScale,
    ref Vector3 iPosition,
    Vector3? iDirection,
    ref Vector3 iNormal,
    float iTTL,
    float iAlpha)
  {
    this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, iAlpha, out DecalManager.DecalReference _);
  }

  public void AddAlphaBlendedDecal(
    Decal iDecal,
    AnimatedLevelPart iAnimation,
    ref Vector2 iScale,
    ref Vector3 iPosition,
    Vector3? iDirection,
    ref Vector3 iNormal,
    float iTTL,
    float iAlpha,
    out DecalManager.DecalReference oReference)
  {
    Vector4 iColor = new Vector4();
    iColor.X = iColor.Y = iColor.Z = 1f;
    iColor.W = iAlpha;
    this.AddAlphaBlendedDecal(iDecal, iAnimation, ref iScale, ref iPosition, iDirection, ref iNormal, iTTL, ref iColor, out oReference);
  }

  public void AddAlphaBlendedDecal(
    Decal iDecal,
    AnimatedLevelPart iAnimation,
    ref Vector2 iScale,
    ref Vector3 iPosition,
    Vector3? iDirection,
    ref Vector3 iNormal,
    float iTTL,
    ref Vector4 iColor,
    out DecalManager.DecalReference oReference)
  {
    int index1;
    if (this.mFreeAlphaBlendedDecals.IsEmpty)
    {
      float num = float.MaxValue;
      index1 = -1;
      for (int index2 = 0; index2 < this.mAlphaArgs.Length; ++index2)
      {
        float z = this.mAlphaArgs[index2].Z;
        if ((double) z < (double) num)
        {
          num = z;
          index1 = index2;
        }
      }
      if (index1 < 0)
      {
        oReference = new DecalManager.DecalReference();
        oReference.Index = -1;
        return;
      }
    }
    else
      index1 = this.mFreeAlphaBlendedDecals.Pop();
    Matrix matrix = new Matrix();
    if (iDirection.HasValue)
    {
      Vector3 result1 = iDirection.Value;
      Vector3 result2;
      Vector3.Cross(ref result1, ref iNormal, out result2);
      result2.Normalize();
      Vector3.Cross(ref iNormal, ref result2, out result1);
      matrix.Backward = iNormal;
      matrix.Right = result2;
      matrix.Up = result1;
    }
    else
    {
      Vector3 result3 = new Vector3();
      for (float result4 = 1f; (double) result4 > 0.99000000953674316 | (double) result4 < -0.99000000953674316 | float.IsNaN(result4); Vector3.Dot(ref result3, ref iNormal, out result4))
      {
        result3.X = (float) this.mRandom.NextDouble() - 0.5f;
        result3.Y = (float) this.mRandom.NextDouble() - 0.5f;
        result3.Z = (float) this.mRandom.NextDouble() - 0.5f;
        result3.Normalize();
      }
      Vector3 result5;
      Vector3.Cross(ref result3, ref iNormal, out result5);
      result5.Normalize();
      Vector3.Cross(ref iNormal, ref result5, out result3);
      matrix.Backward = iNormal;
      matrix.Right = result5;
      matrix.Up = result3;
    }
    matrix.M11 *= iScale.X;
    matrix.M12 *= iScale.X;
    matrix.M13 *= iScale.X;
    matrix.M21 *= iScale.Y;
    matrix.M22 *= iScale.Y;
    matrix.M23 *= iScale.Y;
    matrix.M44 = 1f;
    matrix.Translation = iPosition;
    this.mAlphaTransforms[index1] = matrix;
    Matrix.Invert(ref matrix, out this.mAlphaInvTransforms[index1]);
    this.mAlphaArgs[index1] = new Vector3((float) ((int) iDecal % 8), (float) ((int) iDecal / 8), iTTL);
    this.mAlphaColors[index1] = iColor;
    if (index1 > this.mLastAlphaBlendedDecal)
      this.mLastAlphaBlendedDecal = index1;
    oReference.Index = index1;
    this.mAlphaUniqueIDs[index1] = (int) DateTime.Now.Ticks;
    if (this.mAlphaUniqueIDs[index1] == -1)
      this.mAlphaUniqueIDs[index1] = 0;
    oReference.Hash = this.mAlphaUniqueIDs[index1];
    iAnimation?.AddDecal(ref oReference);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mLastAlphaBlendedDecal >= 0)
    {
      DecalManager.RenderData iObject = this.mAlphaBlendedRenderData[(int) iDataChannel];
      iObject.mNrOfDecals = this.mLastAlphaBlendedDecal + 1;
      Array.Copy((Array) this.mAlphaTransforms, 0, (Array) iObject.mTransforms, 0, iObject.mNrOfDecals);
      Array.Copy((Array) this.mAlphaInvTransforms, 0, (Array) iObject.mInvTransforms, 0, iObject.mNrOfDecals);
      Array.Copy((Array) this.mAlphaArgs, 0, (Array) iObject.mArgs, 0, iObject.mNrOfDecals);
      Array.Copy((Array) this.mAlphaColors, 0, (Array) iObject.mColors, 0, iObject.mNrOfDecals);
      this.mScene.AddProjection(iDataChannel, (IProjectionObject) iObject);
    }
    float num1 = (float) Math.Pow(1.0 - (double) this.mFreeAlphaBlendedDecals.Count / (double) this.mAlphaUniqueIDs.Length, 8.0) * 19f;
    float num2 = iDeltaTime * (1f + num1);
    for (int iValue = 0; iValue <= this.mLastAlphaBlendedDecal; ++iValue)
    {
      float z = this.mAlphaArgs[iValue].Z;
      float num3 = z - num2;
      this.mAlphaArgs[iValue].Z = num3;
      if ((double) num3 < 1.4012984643248171E-45 && (double) z >= 1.4012984643248171E-45)
      {
        this.mAlphaUniqueIDs[iValue] = -1;
        this.mFreeAlphaBlendedDecals.Push(iValue);
      }
    }
    while (this.mLastAlphaBlendedDecal >= 0 && (double) this.mAlphaArgs[this.mLastAlphaBlendedDecal].Z < 0.0)
      --this.mLastAlphaBlendedDecal;
  }

  public bool AddAlphaBlendedDecalAlpha(
    ref DecalManager.DecalReference iReference,
    float iAdditionalAlpha)
  {
    if (iReference.Index < 0)
      return false;
    if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
    {
      this.mAlphaColors[iReference.Index].W = MathHelper.Clamp(this.mAlphaColors[iReference.Index].W + iAdditionalAlpha, 0.0f, 1f);
      return true;
    }
    iReference.Index = -1;
    return false;
  }

  public bool SetDecalTTL(ref DecalManager.DecalReference iReference, float iTTL)
  {
    if (iReference.Index < 0)
      return false;
    if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
    {
      this.mAlphaArgs[iReference.Index].Z = iTTL;
      return true;
    }
    iReference.Index = -1;
    return false;
  }

  public bool SetDecal(
    ref DecalManager.DecalReference iReference,
    float iTTL,
    ref Matrix iTransform)
  {
    if (iReference.Index < 0)
      return false;
    if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
    {
      this.mAlphaTransforms[iReference.Index] = iTransform;
      Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
      this.mAlphaArgs[iReference.Index].Z = iTTL;
      return true;
    }
    iReference.Index = -1;
    return false;
  }

  public bool SetDecal(
    ref DecalManager.DecalReference iReference,
    float iTTL,
    ref Matrix iTransform,
    float iAlpha)
  {
    if (iReference.Index < 0)
      return false;
    if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
    {
      this.mAlphaTransforms[iReference.Index] = iTransform;
      Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
      this.mAlphaArgs[iReference.Index].Z = iTTL;
      this.mAlphaColors[iReference.Index].W = iAlpha;
      return true;
    }
    iReference.Index = -1;
    return false;
  }

  public bool SetDecal(
    ref DecalManager.DecalReference iReference,
    ref Matrix iTransform,
    ref Vector4 iColor)
  {
    if (iReference.Index < 0)
      return false;
    if (iReference.Index < this.mAlphaTransforms.Length && this.mAlphaUniqueIDs[iReference.Index] == iReference.Hash)
    {
      this.mAlphaTransforms[iReference.Index] = iTransform;
      Matrix.Invert(ref iTransform, out this.mAlphaInvTransforms[iReference.Index]);
      this.mAlphaColors[iReference.Index] = iColor;
      return true;
    }
    iReference.Index = -1;
    return false;
  }

  public bool GetDecalTTL(ref DecalManager.DecalReference iReference, out float oTTL)
  {
    oTTL = 0.0f;
    if (iReference.Index < 0 || iReference.Index >= this.mAlphaTransforms.Length || this.mAlphaUniqueIDs[iReference.Index] != iReference.Hash)
      return false;
    oTTL = this.mAlphaArgs[iReference.Index].Z;
    return true;
  }

  public bool GetDecalAlpha(ref DecalManager.DecalReference iReference, out float oAlpha)
  {
    oAlpha = 0.0f;
    if (iReference.Index < 0 || iReference.Index >= this.mAlphaTransforms.Length || this.mAlphaUniqueIDs[iReference.Index] != iReference.Hash)
      return false;
    oAlpha = this.mAlphaColors[iReference.Index].W;
    return true;
  }

  public bool TransformDecal(ref DecalManager.DecalReference iReference, ref Matrix iTransform)
  {
    if (iReference.Index < 0 || iReference.Index >= this.mAlphaTransforms.Length || this.mAlphaUniqueIDs[iReference.Index] != iReference.Hash)
      return false;
    Matrix.Multiply(ref this.mAlphaTransforms[iReference.Index], ref iTransform, out this.mAlphaTransforms[iReference.Index]);
    Matrix.Invert(ref this.mAlphaTransforms[iReference.Index], out this.mAlphaInvTransforms[iReference.Index]);
    return true;
  }

  public struct DecalReference
  {
    public int Index;
    public int Hash;
  }

  private struct Vertex
  {
    public const int SIZEINBYTES = 16 /*0x10*/;
    public Vector3 Position;
    public float Instance;
    public static readonly VertexElement[] VertexElements = new VertexElement[2]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.BlendIndices, (byte) 0)
    };
  }

  private class RenderData : IProjectionObject
  {
    public VertexBuffer mVertices;
    public IndexBuffer mIndices;
    public VertexDeclaration mVertexDeclaration;
    public Texture2D mTexture;
    public Vector2 mTextureScale;
    public int mNrOfDecals;
    public int mVerticesHash;
    public Matrix[] mTransforms;
    public Matrix[] mInvTransforms;
    public Vector3[] mArgs;
    public Vector4[] mColors;
    private Matrix[] mBatchTransforms = new Matrix[30];
    private Matrix[] mBatchInvTransforms = new Matrix[30];
    private Vector3[] mBatchArgs = new Vector3[30];
    private Vector4[] mBatchColors = new Vector4[30];

    public RenderData(int iMaxDecals)
    {
      this.mTransforms = new Matrix[iMaxDecals];
      this.mInvTransforms = new Matrix[iMaxDecals];
      this.mArgs = new Vector3[iMaxDecals];
      this.mColors = new Vector4[iMaxDecals];
    }

    public int Effect => HardwareInstancedProjectionEffect.TYPEHASH;

    public int Technique => 0;

    public VertexBuffer Vertices => this.mVertices;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => 16 /*0x10*/;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum) => false;

    public void Draw(Effect iEffect, Texture2D iDepthMap)
    {
      HardwareInstancedProjectionEffect projectionEffect = iEffect as HardwareInstancedProjectionEffect;
      projectionEffect.DepthMap = iDepthMap;
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
      projectionEffect.GraphicsDevice.RenderState.ReferenceStencil = 1;
      projectionEffect.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
      projectionEffect.PixelSize = new Vector2(1f / (float) iDepthMap.Width, 1f / (float) iDepthMap.Height);
      projectionEffect.Texture = this.mTexture;
      projectionEffect.TextureScale = this.mTextureScale;
      for (int sourceIndex = 0; sourceIndex < this.mNrOfDecals; sourceIndex += 30)
      {
        int length = Math.Min(Math.Min(this.mTransforms.Length - sourceIndex, 30), this.mNrOfDecals - sourceIndex);
        Array.Copy((Array) this.mTransforms, sourceIndex, (Array) this.mBatchTransforms, 0, length);
        Array.Copy((Array) this.mInvTransforms, sourceIndex, (Array) this.mBatchInvTransforms, 0, length);
        Array.Copy((Array) this.mArgs, sourceIndex, (Array) this.mBatchArgs, 0, length);
        Array.Copy((Array) this.mColors, sourceIndex, (Array) this.mBatchColors, 0, length);
        projectionEffect.WorldTransforms = this.mBatchTransforms;
        projectionEffect.InvWorldTransforms = this.mBatchInvTransforms;
        projectionEffect.Args = this.mBatchArgs;
        projectionEffect.Colors = this.mBatchColors;
        projectionEffect.CommitChanges();
        projectionEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, length * 8, 0, length * 12);
      }
      projectionEffect.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
    }
  }
}
