// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.LevelModel
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Physics;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.Graphics;
using Magicka.Levels.Triggers;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Lights;
using PolygonHead.Models;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.Levels;

public class LevelModel : IDisposable
{
  private const float NETWORK_UPDATE_PERIOD = 0.1f;
  protected float mNetworkUpdateTimer;
  private BiTreeModel mModel;
  private Dictionary<int, AnimatedLevelPart> mAnimatedLevelParts;
  private Dictionary<int, Light> mLights;
  private LevelModel.VisualEffectStorage[] mEffects;
  private LevelModel.PhysicsEntityStorage[] mPhysEntities;
  private Liquid[] mWaters;
  private ForceField[] mForceFields;
  private CollisionSkin mCollisionSkin;
  private Dictionary<int, TriggerArea> mTriggerAreas = new Dictionary<int, TriggerArea>();
  private Dictionary<int, Locator> mLocators = new Dictionary<int, Locator>();
  private NavMesh mNavMesh;
  private TriangleMesh mCameraMesh;

  public LevelModel(ContentReader iInput)
  {
    GraphicsDevice graphicsDevice = (iInput.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceManager)) as GraphicsDeviceManager).GraphicsDevice;
    this.mModel = iInput.ReadObject<BiTreeModel>();
    int capacity1 = iInput.ReadInt32();
    this.mAnimatedLevelParts = new Dictionary<int, AnimatedLevelPart>(capacity1);
    for (int index = 0; index < capacity1; ++index)
    {
      AnimatedLevelPart animatedLevelPart = new AnimatedLevelPart(iInput, this);
      this.mAnimatedLevelParts.Add(animatedLevelPart.ID, animatedLevelPart);
    }
    int capacity2 = iInput.ReadInt32();
    this.mLights = new Dictionary<int, Light>(capacity2);
    for (int index = 0; index < capacity2; ++index)
    {
      string iString = iInput.ReadString();
      Vector3 vector3 = iInput.ReadVector3();
      Vector3 iDirection = iInput.ReadVector3();
      LightType lightType = (LightType) iInput.ReadInt32();
      LightVariationType lightVariationType = (LightVariationType) iInput.ReadInt32();
      float num1 = iInput.ReadSingle();
      bool flag = iInput.ReadBoolean();
      float num2 = iInput.ReadSingle();
      float num3 = iInput.ReadSingle();
      Light light;
      switch (lightType)
      {
        case LightType.Point:
          light = (Light) new PointLight(graphicsDevice)
          {
            Position = vector3,
            Radius = num1
          };
          break;
        case LightType.Directional:
          light = (Light) new DirectionalLight(graphicsDevice, iDirection, false);
          break;
        case LightType.Spot:
          light = (Light) new SpotLight(graphicsDevice)
          {
            Position = vector3,
            Range = num1,
            Direction = iDirection,
            CutoffAngle = num2,
            Sharpness = num3,
            UseAttenuation = flag
          };
          break;
        default:
          throw new Exception("Unknown light type!");
      }
      light.Name = iString;
      light.VariationType = lightVariationType;
      light.DiffuseColor = iInput.ReadVector3();
      light.AmbientColor = iInput.ReadVector3();
      light.SpecularAmount = iInput.ReadSingle();
      light.VariationSpeed = iInput.ReadSingle();
      light.VariationAmount = iInput.ReadSingle();
      light.ShadowMapSize = GlobalSettings.Instance.ModShadowResolution(iInput.ReadInt32());
      light.CastShadows = iInput.ReadBoolean();
      this.mLights.Add(iString.GetHashCodeCustom(), light);
    }
    int length1 = iInput.ReadInt32();
    this.mEffects = new LevelModel.VisualEffectStorage[length1];
    Vector3 up = new Vector3();
    up.Y = 1f;
    for (int index = 0; index < length1; ++index)
    {
      string lowerInvariant = iInput.ReadString().ToLowerInvariant();
      LevelModel.VisualEffectStorage visualEffectStorage;
      visualEffectStorage.ID = lowerInvariant.GetHashCodeCustom();
      Vector3 position = iInput.ReadVector3();
      Vector3 forward = iInput.ReadVector3();
      visualEffectStorage.Range = iInput.ReadSingle();
      string iString = iInput.ReadString();
      visualEffectStorage.Effect = iString.GetHashCodeCustom();
      Matrix.CreateWorld(ref position, ref forward, ref up, out visualEffectStorage.Transform);
      this.mEffects[index] = visualEffectStorage;
    }
    int length2 = iInput.ReadInt32();
    this.mPhysEntities = new LevelModel.PhysicsEntityStorage[length2];
    for (int index = 0; index < length2; ++index)
    {
      LevelModel.PhysicsEntityStorage physicsEntityStorage;
      physicsEntityStorage.StartTransform = iInput.ReadMatrix();
      physicsEntityStorage.Template = iInput.ContentManager.Load<PhysicsEntityTemplate>("Data/PhysicsEntities/" + iInput.ReadString());
      this.mPhysEntities[index] = physicsEntityStorage;
    }
    int length3 = iInput.ReadInt32();
    this.mWaters = new Liquid[length3];
    for (int index = 0; index < length3; ++index)
      this.mWaters[index] = Liquid.Read(iInput, this, (AnimatedLevelPart) null);
    int length4 = iInput.ReadInt32();
    this.mForceFields = new ForceField[length4];
    for (int index = 0; index < length4; ++index)
      this.mForceFields[index] = new ForceField(iInput, this);
    this.mCollisionSkin = new CollisionSkin((Body) null);
    for (int index1 = 0; index1 < 10; ++index1)
    {
      TriangleMesh prim = new TriangleMesh();
      List<Vector3> vertices;
      List<TriangleVertexIndices> triangleVertexIndices1;
      if (iInput.ReadBoolean())
      {
        vertices = iInput.ReadObject<List<Vector3>>();
        int capacity3 = iInput.ReadInt32();
        triangleVertexIndices1 = new List<TriangleVertexIndices>(capacity3);
        for (int index2 = 0; index2 < capacity3; ++index2)
        {
          TriangleVertexIndices triangleVertexIndices2;
          triangleVertexIndices2.I0 = iInput.ReadInt32();
          triangleVertexIndices2.I1 = iInput.ReadInt32();
          triangleVertexIndices2.I2 = iInput.ReadInt32();
          triangleVertexIndices1.Add(triangleVertexIndices2);
        }
      }
      else
      {
        vertices = new List<Vector3>();
        triangleVertexIndices1 = new List<TriangleVertexIndices>();
      }
      prim.CreateMesh(vertices, triangleVertexIndices1, 16 /*0x10*/, 2f);
      this.mCollisionSkin.AddPrimitive((Primitive) prim, 1, new MaterialProperties(1f, 1f, 1f));
    }
    this.mCollisionSkin.ApplyLocalTransform(Transform.Identity);
    this.mCollisionSkin.Tag = (object) this;
    if (iInput.ReadBoolean())
    {
      this.mCameraMesh = new TriangleMesh();
      List<Vector3> vertices = iInput.ReadObject<List<Vector3>>();
      int capacity4 = iInput.ReadInt32();
      List<TriangleVertexIndices> triangleVertexIndices3 = new List<TriangleVertexIndices>(capacity4);
      for (int index = 0; index < capacity4; ++index)
      {
        TriangleVertexIndices triangleVertexIndices4;
        triangleVertexIndices4.I0 = iInput.ReadInt32();
        triangleVertexIndices4.I1 = iInput.ReadInt32();
        triangleVertexIndices4.I2 = iInput.ReadInt32();
        triangleVertexIndices3.Add(triangleVertexIndices4);
      }
      this.mCameraMesh.CreateMesh(vertices, triangleVertexIndices3, 4, 2f);
    }
    int num4 = iInput.ReadInt32();
    for (int index = 0; index < num4; ++index)
      this.mTriggerAreas.Add(iInput.ReadString().ToLowerInvariant().GetHashCodeCustom(), TriggerArea.Read(iInput));
    this.mTriggerAreas.Add(TriggerArea.ANYID, (TriggerArea) new AnyTriggerArea());
    int num5 = iInput.ReadInt32();
    for (int index = 0; index < num5; ++index)
    {
      string str = iInput.ReadString();
      Locator locator = new Locator(str, iInput);
      this.mLocators.Add(str.GetHashCodeCustom(), locator);
    }
    this.mNavMesh = new NavMesh(this, iInput);
  }

  public void UpdateAnimatedLevelParts(
    DataChannel iDataChannel,
    float iDeltaTime,
    GameScene iScene)
  {
    Matrix identity = Matrix.Identity;
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
      animatedLevelPart.Update(iDataChannel, iDeltaTime, ref identity, iScene);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime, GameScene iScene)
  {
    this.mModel.SwayEnabled = true;
    this.mModel.AddToScene(iDataChannel, iScene.PlayState.Scene);
    if (NetworkManager.Instance.State == NetworkState.Server)
    {
      this.mNetworkUpdateTimer -= iDeltaTime;
      if ((double) this.mNetworkUpdateTimer <= 0.0)
      {
        this.mNetworkUpdateTimer = 0.1f;
        if (NetworkManager.Instance.Interface is NetworkServer iServer)
        {
          foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
            animatedLevelPart.NetworkUpdate(iServer);
        }
      }
    }
    for (int index = 0; index < this.mWaters.Length; ++index)
      this.mWaters[index].Update(iDataChannel, iDeltaTime, iScene.PlayState.Scene);
    for (int index = 0; index < this.mForceFields.Length; ++index)
      this.mForceFields[index].Update(iDataChannel, iDeltaTime);
  }

  public Dictionary<int, AnimatedLevelPart> AnimatedLevelParts => this.mAnimatedLevelParts;

  public AnimatedLevelPart GetAnimatedLevelPart(int iId) => this.mAnimatedLevelParts[iId];

  public void Initialize(PlayState iPlayState)
  {
    Matrix identity = Matrix.Identity;
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
      animatedLevelPart.Initialize(ref identity);
    for (int index = 0; index < this.mForceFields.Length; ++index)
      this.mForceFields[index].Initialize(iPlayState);
  }

  public Dictionary<int, Light> Lights => this.mLights;

  public Liquid[] Waters => this.mWaters;

  public Dictionary<int, TriggerArea> TriggerAreas => this.mTriggerAreas;

  public Dictionary<int, Locator> Locators => this.mLocators;

  public CollisionSkin CollisionSkin => this.mCollisionSkin;

  public TriangleMesh CameraMesh => this.mCameraMesh;

  public BiTreeModel Model => this.mModel;

  internal NavMesh NavMesh => this.mNavMesh;

  public void RegisterCollisionSkin()
  {
    if (!PhysicsManager.Instance.Simulator.CollisionSystem.CollisionSkins.Contains(this.mCollisionSkin))
      PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mCollisionSkin);
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
      animatedLevelPart.RegisterCollisionSkin();
  }

  public void CreatePhysicsEntities(List<Entity> iEntities, PlayState iPlayState)
  {
    for (int index = 0; index < this.mPhysEntities.Length; ++index)
    {
      PhysicsEntity physicsEntity = this.mPhysEntities[index].Template.MaxHitpoints <= 0 ? new PhysicsEntity(iPlayState) : (PhysicsEntity) new DamageablePhysicsEntity(iPlayState);
      physicsEntity.Initialize(this.mPhysEntities[index].Template, this.mPhysEntities[index].StartTransform, 0);
      iEntities.Add((Entity) physicsEntity);
    }
  }

  public void Dispose()
  {
    foreach (Light light in this.mLights.Values)
      light.DisposeShadowMap();
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
      animatedLevelPart.Dispose();
    this.mModel.Dispose();
    this.mNavMesh = (NavMesh) null;
  }

  internal bool SegmentIntersect(out float oFrac, Segment iSeg)
  {
    return this.SegmentIntersect(out oFrac, out Vector3 _, out Vector3 _, out AnimatedLevelPart _, out int _, iSeg, false);
  }

  internal bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPos,
    out Vector3 oNrm,
    out AnimatedLevelPart oAnimatedLevelPart,
    out int oPrim,
    Segment iSeg)
  {
    return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out oPrim, iSeg, false);
  }

  internal bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPos,
    out Vector3 oNrm,
    out AnimatedLevelPart oAnimatedLevelPart,
    out int oPrim,
    Segment iSeg,
    bool iIgnoreBackfaces)
  {
    oFrac = float.MaxValue;
    oPos = new Vector3();
    oNrm = new Vector3();
    oAnimatedLevelPart = (AnimatedLevelPart) null;
    oPrim = 0;
    float num;
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (this.mCollisionSkin.SegmentIntersect(out num, out vector3_1, out vector3_2, out oPrim, iIgnoreBackfaces, iSeg))
    {
      oFrac = num;
      oPos = vector3_1;
      oNrm = vector3_2;
    }
    for (int index = 0; index < this.mForceFields.Length; ++index)
    {
      if (this.mForceFields[index].CollisionSkin.SegmentIntersect(out num, out vector3_1, out vector3_2, iSeg) && (double) num < (double) oFrac)
      {
        oFrac = num;
        oPos = vector3_1;
        oNrm = vector3_2;
        oPrim = 0;
      }
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
    {
      AnimatedLevelPart oAnim;
      if (animatedLevelPart.SegmentIntersect(out num, out vector3_1, out vector3_2, out oAnim, iSeg) && (double) num < (double) oFrac)
      {
        oFrac = num;
        oPos = vector3_1;
        oNrm = vector3_2;
        oAnimatedLevelPart = oAnim;
        oPrim = (int) oAnimatedLevelPart.CollisionMaterial;
      }
    }
    return (double) oFrac <= 1.0;
  }

  public void GetAllEffects(SortedList<int, GameScene.EffectStorage> iEffects)
  {
    for (int index = 0; index < this.mEffects.Length; ++index)
    {
      GameScene.EffectStorage effectStorage;
      effectStorage.Effect = EffectManager.Instance.GetEffect(this.mEffects[index].Effect);
      effectStorage.Transform = this.mEffects[index].Transform;
      effectStorage.Animation = (AnimatedLevelPart) null;
      effectStorage.Range = this.mEffects[index].Range;
      effectStorage.Effect.Start(ref effectStorage.Transform);
      iEffects.Add(this.mEffects[index].ID, effectStorage);
    }
    foreach (AnimatedLevelPart animatedLevelPart in this.mAnimatedLevelParts.Values)
      animatedLevelPart.GetAllEffects(iEffects);
  }

  protected struct PhysicsEntityStorage
  {
    public Matrix StartTransform;
    public PhysicsEntityTemplate Template;
  }

  internal struct VisualEffectStorage
  {
    public int ID;
    public Matrix Transform;
    public float Range;
    public int Effect;
  }
}
