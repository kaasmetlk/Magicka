// Decompiled with JetBrains decompiler
// Type: Magicka.Graphics.Lights.CapsuleLight
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;

#nullable disable
namespace Magicka.Graphics.Lights;

public class CapsuleLight : Light
{
  private static ModelMesh sMesh;
  private float mRadius;
  private float mLength;
  private Vector3 mStart;
  private Vector3 mEnd;
  private Matrix mLightOrientation;
  private bool mMatrixDirty = true;
  private Vector3 mDiffuseColor;
  private Vector3 mAmbientColor;
  private new float mIntensity;
  private GraphicsDevice mDevice;

  public CapsuleLight(ContentManager iContent)
  {
    if (CapsuleLight.sMesh == null)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        CapsuleLight.sMesh = iContent.Load<Model>("models/effects/Capsule").Meshes[0];
    }
    if (!(RenderManager.Instance.GetEffect(CapsuleLightEffect.TYPEHASH) is CapsuleLightEffect iEffect))
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        iEffect = new CapsuleLightEffect(Magicka.Game.Instance.GraphicsDevice, iContent);
      RenderManager.Instance.RegisterEffect((Microsoft.Xna.Framework.Graphics.Effect) iEffect);
    }
    this.mDevice = iEffect.GraphicsDevice;
  }

  protected void UpdateMatrices()
  {
    this.mMatrixDirty = false;
    Vector3.Distance(ref this.mStart, ref this.mEnd, out this.mLength);
    Vector3 result;
    Vector3.Add(ref this.mStart, ref this.mEnd, out result);
    Vector3.Multiply(ref result, 0.5f, out result);
    Vector3 cameraUpVector = (double) this.mEnd.X != (double) this.mStart.X || (double) this.mEnd.Z != (double) this.mStart.Z ? Vector3.Up : Vector3.Backward;
    Matrix.CreateLookAt(ref result, ref this.mEnd, ref cameraUpVector, out this.mLightOrientation);
    Matrix.Invert(ref this.mLightOrientation, out this.mLightOrientation);
  }

  public Vector3 Start
  {
    get => this.mStart;
    set
    {
      this.mStart = value;
      this.mMatrixDirty = true;
    }
  }

  public Vector3 End
  {
    get => this.mEnd;
    set
    {
      this.mEnd = value;
      this.mMatrixDirty = true;
    }
  }

  public float Radius
  {
    get => this.mRadius;
    set => this.mRadius = value;
  }

  protected override void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
  }

  public override void Draw(
    Microsoft.Xna.Framework.Graphics.Effect iEffect,
    DataChannel iDataChannel,
    float iDeltaTime,
    Texture2D iNormalMap,
    Texture2D iDepthMap)
  {
    if (this.mMatrixDirty)
      this.UpdateMatrices();
    CapsuleLightEffect capsuleLightEffect = iEffect as CapsuleLightEffect;
    capsuleLightEffect.Length = this.mLength;
    capsuleLightEffect.World = this.mLightOrientation;
    capsuleLightEffect.Start = this.mStart;
    capsuleLightEffect.End = this.mEnd;
    capsuleLightEffect.Radius = this.mRadius;
    Vector3 result1;
    Vector3.Multiply(ref this.mDiffuseColor, this.mIntensity, out result1);
    Vector3 result2;
    Vector3.Multiply(ref this.mAmbientColor, this.mIntensity, out result2);
    capsuleLightEffect.DiffuseColor = result1;
    capsuleLightEffect.AmbientColor = result2;
    capsuleLightEffect.NormalMap = (Texture) iNormalMap;
    capsuleLightEffect.DepthMap = (Texture) iDepthMap;
    Vector2 vector2 = new Vector2();
    Point screenSize = RenderManager.Instance.ScreenSize;
    vector2.X = 0.5f / (float) screenSize.X;
    vector2.Y = 0.5f / (float) screenSize.Y;
    capsuleLightEffect.HalfPixel = vector2;
    capsuleLightEffect.CommitChanges();
    ModelMeshPart meshPart = CapsuleLight.sMesh.MeshParts[0];
    this.mDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.BaseVertex, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
  }

  public override Vector3 DiffuseColor
  {
    get => this.mDiffuseColor;
    set => this.mDiffuseColor = value;
  }

  public override Vector3 AmbientColor
  {
    get => this.mAmbientColor;
    set => this.mAmbientColor = value;
  }

  public float Intensity
  {
    get => this.mIntensity;
    set => this.mIntensity = value;
  }

  public override int Effect => CapsuleLightEffect.TYPEHASH;

  public override int Technique => 0;

  public override int VertexStride => CapsuleLight.sMesh.MeshParts[0].VertexStride;

  public override VertexBuffer VertexBuffer => CapsuleLight.sMesh.VertexBuffer;

  public override IndexBuffer IndexBuffer => CapsuleLight.sMesh.IndexBuffer;

  public override VertexDeclaration VertexDeclaration
  {
    get => CapsuleLight.sMesh.MeshParts[0].VertexDeclaration;
  }

  public override bool ShouldDraw(BoundingFrustum iViewFrustum) => true;

  public override float SpecularAmount
  {
    get => 0.0f;
    set
    {
    }
  }

  public override bool CastShadows
  {
    get => false;
    set
    {
    }
  }

  public override int ShadowMapSize
  {
    get => 0;
    set
    {
    }
  }

  public override void DisposeShadowMap()
  {
  }

  public override void CreateShadowMap()
  {
  }

  public override void DrawShadows(DataChannel iDataChannel, float iDeltaTime, Scene iScene)
  {
  }
}
