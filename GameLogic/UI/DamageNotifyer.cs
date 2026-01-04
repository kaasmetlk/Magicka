// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.DamageNotifyer
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using System;

#nullable disable
namespace Magicka.GameLogic.UI;

public sealed class DamageNotifyer
{
  public const int MAXNUMBERS = 1024 /*0x0400*/;
  private static DamageNotifyer mSingelton;
  private static volatile object mSingeltonLock = new object();
  public static readonly Vector2 DIGITSIZE = new Vector2(11f, 16f);
  public readonly Vector2 DIGITSIZEUV;
  private VertexBuffer mVertices;
  private VertexDeclaration mVertexDeclaration;
  private GUIHardwareInstancingEffect mEffect;
  private Heap<int> mFreeInstances;
  private int mLastNumber;
  private DamageNotifyer.Number[] mNumbers;
  private Scene mScene;
  private DamageNotifyer.RenderData[] mRenderData;

  public static DamageNotifyer Instance
  {
    get
    {
      if (DamageNotifyer.mSingelton == null)
      {
        lock (DamageNotifyer.mSingeltonLock)
        {
          if (DamageNotifyer.mSingelton == null)
            DamageNotifyer.mSingelton = new DamageNotifyer();
        }
      }
      return DamageNotifyer.mSingelton;
    }
  }

  private DamageNotifyer()
  {
    this.mNumbers = new DamageNotifyer.Number[1024 /*0x0400*/];
    this.mFreeInstances = new Heap<int>(1024 /*0x0400*/);
    for (int iValue = 0; iValue < 1024 /*0x0400*/; ++iValue)
      this.mFreeInstances.Push(iValue);
    Texture2D texture2D;
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      this.mEffect = new GUIHardwareInstancingEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
      texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/HUD/Digits");
    }
    this.mEffect.Texture = (Texture) texture2D;
    this.DIGITSIZEUV = DamageNotifyer.DIGITSIZE / new Vector2((float) texture2D.Width, (float) texture2D.Height);
    Point point;
    point.X = GlobalSettings.Instance.Resolution.Width;
    point.Y = GlobalSettings.Instance.Resolution.Height;
    this.mEffect.SetScreenSize(point.X, point.Y);
    this.mEffect.SetTechnique(GUIHardwareInstancingEffect.Technique.Numbers);
    this.mEffect.DigitWidth = this.DIGITSIZEUV.X;
    this.mRenderData = new DamageNotifyer.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      DamageNotifyer.RenderData renderData = new DamageNotifyer.RenderData();
      this.mRenderData[index] = renderData;
      renderData.mEffect = this.mEffect;
    }
    this.CreateVertices();
  }

  private void CreateVertices()
  {
    DamageNotifyer.VertexPositionTextureIndex[] data = new DamageNotifyer.VertexPositionTextureIndex[240 /*0xF0*/];
    for (int index = 0; index < 40; ++index)
    {
      DamageNotifyer.VertexPositionTextureIndex positionTextureIndex;
      positionTextureIndex.Index = (float) index;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, 0.0f);
      data[index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(1f, 0.0f);
      data[1 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, 1f);
      data[2 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * -0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(1f, 0.0f);
      data[3 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * 0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(1f, 1f);
      data[4 + index * 6] = positionTextureIndex;
      positionTextureIndex.Position = new Vector3(DamageNotifyer.DIGITSIZE.X * -0.5f, DamageNotifyer.DIGITSIZE.Y * 0.5f, 0.0f);
      positionTextureIndex.TexCoord = new Vector2(0.0f, 1f);
      data[5 + index * 6] = positionTextureIndex;
    }
    if (this.mVertices == null || this.mVertices.IsDisposed)
      this.mVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, 24 * data.Length, BufferUsage.WriteOnly);
    this.mVertices.SetData<DamageNotifyer.VertexPositionTextureIndex>(data);
    if (this.mVertexDeclaration == null || this.mVertexDeclaration.IsDisposed)
      this.mVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, DamageNotifyer.VertexPositionTextureIndex.VertexElements);
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index].mVertices = this.mVertices;
      this.mRenderData[index].mVertexDeclaration = this.mVertexDeclaration;
    }
  }

  public void Clear()
  {
    this.mFreeInstances.Clear();
    for (int iValue = 0; iValue < 1024 /*0x0400*/; ++iValue)
      this.mFreeInstances.Push(iValue);
    this.mLastNumber = -1;
  }

  public int AddNumber(float iValue, ref Vector3 iPosition, float iTTL, bool iLocked)
  {
    if (GlobalSettings.Instance.DamageNumbers == SettingOptions.Off)
      return 0;
    if (this.mFreeInstances.IsEmpty)
      return -1;
    int val2 = this.mFreeInstances.Pop();
    DamageNotifyer.Number number = new DamageNotifyer.Number();
    number.Locked = iLocked;
    number.Position = iPosition;
    number.Scale.Y = 1f;
    number.Scale.X = 1f;
    number.Value = iValue;
    Vector3 oColor;
    DamageNotifyer.GetDefaultColors(iValue, out oColor);
    number.Color = oColor;
    number.TTL.X = iTTL;
    number.TTL.Y = iTTL;
    number.Velocity = 0.02f;
    this.mNumbers[val2] = number;
    this.mLastNumber = Math.Max(this.mLastNumber, val2);
    return val2;
  }

  public int AddNumber(
    float iValue,
    ref Vector3 iPosition,
    float iTTL,
    bool iLocked,
    ref Vector3 iColor)
  {
    if (GlobalSettings.Instance.DamageNumbers == SettingOptions.Off)
      return 0;
    if (this.mFreeInstances.IsEmpty)
      return -1;
    int val2 = this.mFreeInstances.Pop();
    this.mNumbers[val2] = new DamageNotifyer.Number()
    {
      Locked = iLocked,
      Position = iPosition,
      Scale = {
        Y = 1f,
        X = 1f
      },
      Value = iValue,
      Color = iColor,
      TTL = {
        X = iTTL,
        Y = iTTL
      },
      Velocity = 0.02f
    };
    this.mLastNumber = Math.Max(this.mLastNumber, val2);
    return val2;
  }

  public void AddToNumber(int iIndex, float iValue) => this.mNumbers[iIndex].Value += iValue;

  public void UpdateNumberPosition(int iIndex, ref Vector3 iPosition)
  {
    this.mNumbers[iIndex].Position = iPosition;
  }

  public void ReleasNumber(int iIndex) => this.mNumbers[iIndex].Locked = false;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    DamageNotifyer.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.mNrOfNumbers = this.mLastNumber + 1;
    DamageNotifyer.Number.CopyToArrays(this.mNumbers, 0, 0, this.mLastNumber + 1, iObject.mWorldPositions, iObject.mScales, iObject.mValues, iObject.mColors);
    for (int iValue = 0; iValue <= this.mLastNumber; ++iValue)
    {
      DamageNotifyer.Number mNumber = this.mNumbers[iValue];
      mNumber.Scale.Y = 1f + Math.Min(2f, Math.Abs(mNumber.Value / 1000f));
      mNumber.Scale.X = 1f + Math.Min(2f, Math.Abs(mNumber.Value / 1000f));
      if (mNumber.Locked)
      {
        this.mNumbers[iValue] = mNumber;
      }
      else
      {
        float y = mNumber.TTL.Y;
        mNumber.TTL.Y = y - iDeltaTime;
        mNumber.Velocity -= iDeltaTime * 0.03f;
        mNumber.Position.Y += mNumber.Velocity;
        this.mNumbers[iValue] = mNumber;
        if ((double) mNumber.TTL.Y < 1.4012984643248171E-45 && (double) y >= 1.4012984643248171E-45)
          this.mFreeInstances.Push(iValue);
      }
    }
    while (this.mLastNumber >= 0 && (double) this.mNumbers[this.mLastNumber].TTL.Y < 0.0)
      --this.mLastNumber;
    this.mScene.AddRenderableGUIObject(iDataChannel, (IRenderableGUIObject) iObject);
  }

  public Scene Scene
  {
    get => this.mScene;
    set
    {
      this.mScene = value;
      this.mLastNumber = -1;
      this.mFreeInstances.Clear();
      for (int iValue = 0; iValue < 1024 /*0x0400*/; ++iValue)
        this.mFreeInstances.Push(iValue);
      this.UpdateResolution();
    }
  }

  public static void GetDefaultColors(float amount, out Vector3 oColor)
  {
    oColor = new Vector3();
    if ((double) amount < 0.0)
    {
      oColor.X = 0.0f;
      oColor.Y = 1f;
      oColor.Z = 0.0f;
    }
    else if ((double) amount > 0.0)
    {
      oColor.X = 1f;
      oColor.Y = 0.9f;
      oColor.Z = 0.0f;
    }
    else
    {
      oColor.X = 0.5f;
      oColor.Y = 0.5f;
      oColor.Z = 0.5f;
    }
  }

  public void UpdateResolution()
  {
    Point point;
    point.X = GlobalSettings.Instance.Resolution.Width;
    point.Y = GlobalSettings.Instance.Resolution.Height;
    this.mEffect.SetScreenSize(point.X, point.Y);
  }

  private class RenderData : IRenderableGUIObject, IPreRenderRenderer
  {
    public GUIHardwareInstancingEffect mEffect;
    public VertexBuffer mVertices;
    public VertexDeclaration mVertexDeclaration;
    public int mNrOfNumbers;
    public Vector3[] mWorldPositions = new Vector3[1024 /*0x0400*/];
    private Vector2[] mPositions = new Vector2[1024 /*0x0400*/];
    public Vector3[] mScales = new Vector3[1024 /*0x0400*/];
    public float[] mValues = new float[1024 /*0x0400*/];
    public Vector4[] mColors = new Vector4[1024 /*0x0400*/];
    private Vector2[] mBatchPositions = new Vector2[40];
    private Vector3[] mBatchScales = new Vector3[40];
    private float[] mBatchValues = new float[40];
    private Vector4[] mBatchColors = new Vector4[40];

    public void Draw(float iDeltaTime)
    {
      if (this.mNrOfNumbers <= 0)
        return;
      this.mEffect.GraphicsDevice.Vertices[0].SetSource(this.mVertices, 0, 24);
      this.mEffect.GraphicsDevice.VertexDeclaration = this.mVertexDeclaration;
      this.mEffect.GraphicsDevice.RenderState.CullMode = CullMode.None;
      for (int sourceIndex = 0; sourceIndex < this.mNrOfNumbers; sourceIndex += 40)
      {
        int length = this.mPositions.Length - sourceIndex;
        if (length > 40)
          length = 40;
        if (length > this.mNrOfNumbers)
          length = this.mNrOfNumbers;
        Array.Copy((Array) this.mPositions, sourceIndex, (Array) this.mBatchPositions, 0, length);
        Array.Copy((Array) this.mScales, sourceIndex, (Array) this.mBatchScales, 0, length);
        Array.Copy((Array) this.mValues, sourceIndex, (Array) this.mBatchValues, 0, length);
        Array.Copy((Array) this.mColors, sourceIndex, (Array) this.mBatchColors, 0, length);
        this.mEffect.Positions = this.mBatchPositions;
        this.mEffect.Scales = this.mBatchScales;
        this.mEffect.Values = this.mBatchValues;
        this.mEffect.Colors = this.mBatchColors;
        this.mEffect.Begin();
        for (int index = 0; index < this.mEffect.CurrentTechnique.Passes.Count; ++index)
        {
          this.mEffect.CurrentTechnique.Passes[index].Begin();
          this.mEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, length * 2);
          this.mEffect.CurrentTechnique.Passes[index].End();
        }
        this.mEffect.End();
      }
    }

    public int ZIndex => 20;

    public void PreRenderUpdate(
      DataChannel iDataChannel,
      float iDeltaTime,
      ref Matrix iViewProjectionMatrix,
      ref Vector3 iCameraPosition,
      ref Vector3 iCameraDirection)
    {
      for (int index = 0; index < this.mNrOfNumbers; ++index)
        this.mPositions[index] = MagickaMath.WorldToScreenPosition(ref this.mWorldPositions[index], ref iViewProjectionMatrix);
    }
  }

  private struct VertexPositionTextureIndex
  {
    public const int SIZEINBYTES = 24;
    public Vector3 Position;
    public Vector2 TexCoord;
    public float Index;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 20, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1)
    };
  }

  private struct Number
  {
    public bool Locked;
    public float Velocity;
    public Vector3 Position;
    public float Value;
    public Vector3 Color;
    public Vector2 Scale;
    public Vector2 TTL;

    public static void CopyToArrays(
      DamageNotifyer.Number[] iSource,
      int iSourceStartIndex,
      int iTargetStartIndex,
      int iCount,
      Vector3[] iTargetPositions,
      Vector3[] iTargetScales,
      float[] iTargetValues,
      Vector4[] iTargetColors)
    {
      Vector4 vector4 = new Vector4();
      for (int index1 = 0; index1 < iCount; ++index1)
      {
        DamageNotifyer.Number number = iSource[index1 + iSourceStartIndex];
        vector4.X = number.Color.X;
        vector4.Y = number.Color.Y;
        vector4.Z = number.Color.Z;
        vector4.W = Math.Max(1f - (float) Math.Pow(1.0 - (double) number.TTL.Y / (double) number.TTL.X, 5.0), 0.0f);
        int index2 = index1 + iTargetStartIndex;
        iTargetPositions[index2] = number.Position;
        iTargetScales[index2] = new Vector3(number.Scale, 1f);
        iTargetValues[index2] = number.Value + 0.5f;
        iTargetColors[index2] = vector4;
      }
    }
  }
}
