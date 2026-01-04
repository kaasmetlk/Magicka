// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.Entities.Entanglement
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using XNAnimation;
using XNAnimation.Effects;

#nullable disable
namespace Magicka.GameLogic.Entities;

public class Entanglement
{
  public const float MAX_MAGNITUDE = 20f;
  private static readonly int BREAK_SOUND = "misc_spider_web_cut".GetHashCodeCustom();
  private static readonly int BREAK_EFFECT = "web_hit".GetHashCodeCustom();
  private static readonly int HIT_EFFECT = "web_hit".GetHashCodeCustom();
  private static Elements mWeakness;
  private static SkinnedModel mModels;
  private static int mRootJoint;
  private static int mHipJoint;
  private static Matrix mHipBindPose;
  private Entanglement.RenderData[] mRenderData;
  private float mMagnitude;
  private float mFadeTimer;
  private Character mOwner;
  private Matrix mSkinScaleMatrix;
  private float mOwnerRadius;
  private float mOwnerLength;
  private EntangleEffect mEffect;

  public static void DisposeModels()
  {
    if (Entanglement.mModels == null)
      return;
    Entanglement.mModels = (SkinnedModel) null;
  }

  public Entanglement(Character iOwner)
  {
    this.mOwner = iOwner;
    PlayState playState = iOwner.PlayState;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    if (Entanglement.mModels == null)
    {
      lock (graphicsDevice)
        Entanglement.mModels = playState.Content.Load<SkinnedModel>("Models/Effects/Entangle_web");
      for (int index = 0; index < Entanglement.mModels.SkeletonBones.Count; ++index)
      {
        SkinnedModelBone skeletonBone = Entanglement.mModels.SkeletonBones[index];
        if (skeletonBone.Index == (ushort) 0)
          Entanglement.mRootJoint = (int) skeletonBone.Index;
        else if (skeletonBone.Index == (ushort) 1)
        {
          Entanglement.mHipJoint = (int) skeletonBone.Index;
          Entanglement.mHipBindPose = skeletonBone.InverseBindPoseTransform;
        }
      }
      Entanglement.mWeakness = Elements.Fire;
    }
    ModelMesh mesh = Entanglement.mModels.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    lock (graphicsDevice)
      this.mEffect = new EntangleEffect(graphicsDevice, Magicka.Game.Instance.Content);
    this.mRenderData = new Entanglement.RenderData[3];
    for (int index = 0; index < 3; ++index)
    {
      this.mRenderData[index] = new Entanglement.RenderData();
      this.mRenderData[index].SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, EntangleEffect.TYPEHASH);
    }
    this.mFadeTimer = 0.0f;
    float length = iOwner.Capsule.Length;
    float radius = iOwner.Capsule.Radius;
    this.mSkinScaleMatrix = Matrix.Identity;
    this.mSkinScaleMatrix.M11 *= radius;
    this.mSkinScaleMatrix.M12 *= radius;
    this.mSkinScaleMatrix.M13 *= radius;
    this.mSkinScaleMatrix.M21 *= length;
    this.mSkinScaleMatrix.M22 *= length;
    this.mSkinScaleMatrix.M23 *= length;
    this.mSkinScaleMatrix.M31 *= radius;
    this.mSkinScaleMatrix.M32 *= radius;
    this.mSkinScaleMatrix.M33 *= radius;
  }

  public void Initialize()
  {
    this.mFadeTimer = 0.5f;
    ModelMesh mesh = Entanglement.mModels.Model.Meshes[0];
    ModelMeshPart meshPart = mesh.MeshParts[0];
    this.mEffect.DiffuseMap = (meshPart.Effect as SkinnedModelBasicEffect).DiffuseMap0;
    for (int index = 0; index < 3; ++index)
      this.mRenderData[index].SetMesh(mesh.VertexBuffer, mesh.IndexBuffer, meshPart, EntangleEffect.TYPEHASH);
    this.mOwnerLength = this.mOwner.Capsule.Length;
    this.mOwnerRadius = this.mOwner.Capsule.Radius;
    float num1 = (float) (((double) this.mOwnerRadius + (double) this.mOwnerLength) * 0.800000011920929);
    float num2 = this.mOwnerRadius * 1.33333337f;
    this.mSkinScaleMatrix = Matrix.Identity;
    this.mSkinScaleMatrix.M11 *= num2;
    this.mSkinScaleMatrix.M12 *= num2;
    this.mSkinScaleMatrix.M13 *= num2;
    this.mSkinScaleMatrix.M21 *= num1;
    this.mSkinScaleMatrix.M22 *= num1;
    this.mSkinScaleMatrix.M23 *= num1;
    this.mSkinScaleMatrix.M31 *= num2;
    this.mSkinScaleMatrix.M32 *= num2;
    this.mSkinScaleMatrix.M33 *= num2;
  }

  public void Update(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref BoundingSphere iBoundingSphere)
  {
    if ((double) this.mMagnitude <= 1.4012984643248171E-45 && (double) this.mFadeTimer <= 1.4012984643248171E-45)
      return;
    Entanglement.RenderData iObject = this.mRenderData[(int) iDataChannel];
    iObject.mBoundingSphere = iBoundingSphere;
    Matrix result1 = this.mOwner.Body.Orientation;
    Matrix result2 = this.mOwner.GetHipAttachOrientation();
    Matrix.Multiply(ref this.mSkinScaleMatrix, ref result1, out result1);
    Vector3 position = this.mOwner.Position;
    position.Y -= this.mOwner.Capsule.Radius + this.mOwner.Capsule.Length * 0.5f;
    result1.Translation = position;
    iObject.mBones[Entanglement.mRootJoint] = result1;
    Matrix.Multiply(ref result2, ref Entanglement.mHipBindPose, out result2);
    result1.Translation = result2.Translation;
    iObject.mBones[Entanglement.mHipJoint] = result1;
    if ((double) this.mMagnitude <= 0.0)
      this.mFadeTimer -= iDeltaTime;
    iObject.mVisibility = (float) (0.5 * ((double) this.mMagnitude / 20.0)) + this.mFadeTimer;
    this.mOwner.PlayState.Scene.AddRenderableObject(iDataChannel, (IRenderableObject) iObject);
  }

  public void AddEntanglement(float iMagnitude)
  {
    this.mMagnitude += iMagnitude;
    if ((double) this.mMagnitude <= 20.0)
      return;
    this.mMagnitude = 20f;
  }

  public void DecreaseEntanglement(float iMagnitude, Elements iElement)
  {
    if ((double) this.mMagnitude <= 0.0)
      return;
    Vector3 position = this.mOwner.Position;
    Vector3 direction = this.mOwner.Direction;
    EffectManager.Instance.StartEffect(Entanglement.HIT_EFFECT, ref position, ref direction, out VisualEffectReference _);
    float num = this.mMagnitude - iMagnitude;
    if ((iElement & Entanglement.mWeakness) != Elements.None || (double) num < 0.0)
    {
      this.mFadeTimer = 0.5f;
      this.Release();
    }
    else
      this.mMagnitude = num;
  }

  public void Release()
  {
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.SendMessage<CharacterActionMessage>(ref new CharacterActionMessage()
      {
        Action = ActionType.Release,
        Handle = this.mOwner.Handle
      });
    if ((double) this.mMagnitude > 0.0)
    {
      AudioManager.Instance.PlayCue(Banks.Misc, Entanglement.BREAK_SOUND, this.mOwner.AudioEmitter);
      this.mFadeTimer = 0.5f;
    }
    this.mMagnitude = 0.0f;
  }

  public float Magnitude => this.mMagnitude;

  protected class RenderData : IRenderableObject, IRenderableAdditiveObject
  {
    protected VertexBuffer mVertices;
    protected IndexBuffer mIndices;
    public BoundingSphere mBoundingSphere;
    protected int mEffect;
    protected VertexDeclaration mVertexDeclaration;
    protected int mBaseVertex;
    protected int mNumVertices;
    protected int mPrimitiveCount;
    protected int mStartIndex;
    protected int mStreamOffset;
    protected int mVertexStride;
    private Texture2D mTexture;
    public Matrix[] mBones = new Matrix[10];
    public float mVisibility;
    public int mVerticesHash;
    protected int mTechnique;
    protected int mDepthTechnique;
    protected int mShadowTechnique;

    public int Effect => this.mEffect;

    public int DepthTechnique => this.mDepthTechnique;

    public int Technique => this.mTechnique;

    public int ShadowTechnique => this.mShadowTechnique;

    public VertexBuffer Vertices => this.mVertices;

    public IndexBuffer Indices => this.mIndices;

    public VertexDeclaration VertexDeclaration => this.mVertexDeclaration;

    public int VertexStride => this.mVertexStride;

    public int VerticesHashCode => this.mVerticesHash;

    public bool Cull(BoundingFrustum iViewFrustum)
    {
      return this.mBoundingSphere.Contains(iViewFrustum) == ContainmentType.Disjoint;
    }

    public void Draw(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      EntangleEffect entangleEffect = iEffect as EntangleEffect;
      entangleEffect.MatBones = this.mBones;
      entangleEffect.SpecularPower = 20f;
      entangleEffect.SpecularAmount = 0.5f;
      entangleEffect.AlphaBias = (float) ((double) this.mVisibility * 0.5 - 1.0);
      entangleEffect.ColorTint = Vector4.One;
      entangleEffect.DiffuseMapEnabled = true;
      entangleEffect.DiffuseMap = this.mTexture;
      entangleEffect.DiffuseColor = new Vector3(1f);
      entangleEffect.CommitChanges();
      entangleEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      entangleEffect.DiffuseMapEnabled = false;
    }

    public void DrawShadow(Effect iEffect, BoundingFrustum iViewFrustum)
    {
      EntangleEffect entangleEffect = iEffect as EntangleEffect;
      entangleEffect.MatBones = this.mBones;
      entangleEffect.AlphaBias = (float) ((double) this.mVisibility * 0.5 - 1.0);
      entangleEffect.ColorTint = Vector4.One;
      entangleEffect.DiffuseMapEnabled = true;
      entangleEffect.DiffuseMap = this.mTexture;
      entangleEffect.CommitChanges();
      entangleEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.mBaseVertex, 0, this.mNumVertices, this.mStartIndex, this.mPrimitiveCount);
      entangleEffect.DiffuseMapEnabled = false;
    }

    public void SetMesh(
      VertexBuffer iVertices,
      IndexBuffer iIndices,
      ModelMeshPart iMeshPart,
      int iEffectHash)
    {
      this.mVertices = iVertices;
      this.mVerticesHash = iVertices.GetHashCode();
      this.mIndices = iIndices;
      this.mTechnique = 0;
      this.mDepthTechnique = 2;
      this.mShadowTechnique = 3;
      this.mEffect = iEffectHash;
      this.mVertexDeclaration = iMeshPart.VertexDeclaration;
      this.mBaseVertex = iMeshPart.BaseVertex;
      this.mNumVertices = iMeshPart.NumVertices;
      this.mPrimitiveCount = iMeshPart.PrimitiveCount;
      this.mStartIndex = iMeshPart.StartIndex;
      this.mStreamOffset = iMeshPart.StreamOffset;
      this.mVertexStride = iMeshPart.VertexStride;
      this.mTexture = (iMeshPart.Effect as SkinnedModelBasicEffect).DiffuseMap0;
    }
  }
}
