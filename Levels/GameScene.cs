// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.GameScene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.AI;
using Magicka.Audio;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Graphics;
using Magicka.Graphics.Effects;
using Magicka.Levels.Triggers;
using Magicka.Levels.Triggers.Actions;
using Magicka.Levels.Versus;
using Magicka.Network;
using Magicka.PathFinding;
using Magicka.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using PolygonHead.Effects;
using PolygonHead.Lights;
using PolygonHead.Models;
using PolygonHead.ParticleEffects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

#nullable disable
namespace Magicka.Levels;

public class GameScene : IPreRenderRenderer, IDisposable
{
  private const float PROJECTIONWIDTH = 50f;
  private const float OFFSETLENGTH = 35.35534f;
  private const float OFFSETDEVISOR = 0.0141421361f;
  private static readonly string VOLUME_VAR_NAME = "Volume";
  public static readonly int ANYAREA = "any".GetHashCodeCustom();
  private SharedContentManager mContent;
  private PlayState mPlayState;
  private Level mLevel;
  private int mLastGeneratedEffectID;
  private List<Magicka.Levels.Triggers.Actions.Action> mTriggeredActions;
  private List<Magicka.Levels.Triggers.Actions.Action> mStartupActions = new List<Magicka.Levels.Triggers.Actions.Action>(64 /*0x40*/);
  private SortedList<int, GameScene.GlobalAudio> mGlobalSounds = new SortedList<int, GameScene.GlobalAudio>();
  private SortedList<int, AudioLocator> mSounds = new SortedList<int, AudioLocator>();
  private SortedList<int, Trigger> mTriggers;
  private string mModelName;
  private LevelModel mModel;
  private Liquid[] mLiquids;
  private string mName;
  private IRuleset mRuleset;
  private bool mIndoor;
  private bool mForceNavMesh;
  private bool mForceCamera;
  private bool mFirstStart = true;
  private bool mIsInitialized;
  private List<Entity> mSavedEntities = new List<Entity>();
  private Dictionary<int, AnimatedLevelPart.AnimationState> mSavedAnimations = new Dictionary<int, AnimatedLevelPart.AnimationState>();
  private Fog mFog;
  private float mBrightness = 1f;
  private float mContrast = 1f;
  private float mSaturation = 1f;
  private float mRimLightGlow = 0.8f;
  private float mRimLightPower = 1.5f;
  private float mRimLightBias = 0.2f;
  private float mBloomThreshold = 0.8f;
  private float mBloomMultiplier = 1f;
  private float mBlurSigma = 2.5f;
  private Matrix mVisualAidViewProjection;
  private bool mUseVertexTexturing;
  private SurfaceFormat mSwaySurfaceFormat;
  private RenderTarget2D mSwayTarget;
  private Texture2D mSwayTexture;
  private Texture2D mCharacterDisplacementTexture;
  private SwayEffect mSwayEffect;
  private VertexDeclaration mSwayVertexDeclaration;
  private VertexBuffer mSwayVertices;
  private float mWindPosition;
  private Vector2 mVindDirection = Vector2.Normalize(new Vector2(1f, 1f));
  private float mVindSpeed = 0.05f;
  private float mReverbMix;
  private RoomType mRoomType;
  private bool mCloudWorldRelative;
  private float mCloudIntensity;
  private Vector2 mCloudScale;
  private Vector2 mCloudMovement;
  private Vector2 mCloudOffset;
  private Texture2D mCloudTexture;
  private Texture2D mSkyMap;
  private string mSkyMapFileName;
  private Vector3 mSkyMapColor;
  private float mLightIntensity = 1f;
  private float mTargetLightIntensity = 1f;
  private Vector3 mLightDiffuse = new Vector3(1f);
  private Vector3 mTargetLightDiffuse = new Vector3(1f);
  private GameScene.LightSettings[] mDirectionalLightSettings;
  private SortedList<int, GameScene.EffectStorage> mEffects = new SortedList<int, GameScene.EffectStorage>();
  private int mId;
  private CollisionSkin mKillPlane;
  private byte[] mShaHash;

  internal GameScene(
    Level iLevel,
    string iFileName,
    XmlDocument iInput,
    ContentManager iContent,
    VersusRuleset.Settings iSettings)
  {
    this.mContent = new SharedContentManager(iContent.ServiceProvider);
    try
    {
      using (FileStream inputStream = File.OpenRead(iFileName))
        this.mShaHash = SHA256.Create().ComputeHash((Stream) inputStream);
    }
    catch (Exception ex)
    {
      this.mShaHash = new byte[32 /*0x20*/];
    }
    this.mPlayState = iLevel.PlayState;
    this.mLevel = iLevel;
    this.mLastGeneratedEffectID = 0;
    float num = -50f;
    string directoryName = Path.GetDirectoryName(iFileName);
    this.mName = Path.GetFileNameWithoutExtension(iFileName).ToLowerInvariant();
    this.mId = this.mName.GetHashCodeCustom();
    XmlNode xmlNode = (XmlNode) null;
    for (int i1 = 0; i1 < iInput.ChildNodes.Count; ++i1)
    {
      if (iInput.ChildNodes[i1].Name.Equals(nameof (Scene), StringComparison.OrdinalIgnoreCase))
      {
        for (int i2 = 0; i2 < iInput.ChildNodes[i1].Attributes.Count; ++i2)
        {
          XmlAttribute attribute = iInput.ChildNodes[i1].Attributes[i2];
          if (attribute.Name.Equals("forcenavmesh", StringComparison.OrdinalIgnoreCase))
            this.mForceNavMesh = bool.Parse(attribute.Value);
          else if (attribute.Name.Equals("forcecamera", StringComparison.OrdinalIgnoreCase))
            this.mForceCamera = bool.Parse(attribute.Value);
        }
        xmlNode = iInput.ChildNodes[i1];
        break;
      }
    }
    if (xmlNode == null)
      throw new Exception("No Scene node found in scene XML!");
    this.mTriggers = new SortedList<int, Trigger>();
    for (int i3 = 0; i3 < xmlNode.ChildNodes.Count; ++i3)
    {
      XmlNode childNode = xmlNode.ChildNodes[i3];
      if (!(childNode is XmlComment))
      {
        if (childNode.Name.Equals("Indoor", StringComparison.OrdinalIgnoreCase))
          this.mIndoor = bool.Parse(childNode.InnerText);
        else if (childNode.Name.Equals("Magnify", StringComparison.OrdinalIgnoreCase))
          iLevel.PlayState.Camera.DefaultMagnification = float.Parse(childNode.InnerText, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        else if (childNode.Name.Equals("reverb", StringComparison.OrdinalIgnoreCase))
        {
          for (int i4 = 0; i4 < childNode.Attributes.Count; ++i4)
          {
            XmlAttribute attribute = childNode.Attributes[i4];
            if (attribute.Name.Equals("roomType", StringComparison.OrdinalIgnoreCase))
              this.mRoomType = (RoomType) Enum.Parse(typeof (RoomType), attribute.Value, true);
            else if (attribute.Name.Equals("mix", StringComparison.OrdinalIgnoreCase))
              this.mReverbMix = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        else if (childNode.Name.Equals("Ruleset", StringComparison.OrdinalIgnoreCase))
        {
          for (int i5 = 0; i5 < childNode.Attributes.Count; ++i5)
          {
            XmlAttribute attribute = childNode.Attributes[i5];
            switch (attribute.Name.ToLowerInvariant())
            {
              case "type":
                if (attribute.Value.Equals("survival", StringComparison.OrdinalIgnoreCase))
                {
                  this.mRuleset = (IRuleset) new SurvivalRuleset(this, childNode);
                  break;
                }
                if (attribute.Value.Equals("timedobjective", StringComparison.OrdinalIgnoreCase))
                {
                  this.mRuleset = (IRuleset) new TimedObjectiveRuleset(this, childNode);
                  break;
                }
                if (attribute.Value.Equals("versus", StringComparison.OrdinalIgnoreCase))
                {
                  switch (iSettings)
                  {
                    case DeathMatch.Settings _:
                      this.mRuleset = (IRuleset) new DeathMatch(this, childNode, iSettings as DeathMatch.Settings);
                      continue;
                    case Brawl.Settings _:
                      this.mRuleset = (IRuleset) new Brawl(this, childNode, iSettings as Brawl.Settings);
                      continue;
                    case Pyrite.Settings _:
                      this.mRuleset = (IRuleset) new Pyrite(this, childNode, iSettings as Pyrite.Settings);
                      continue;
                    case Krietor.Settings _:
                      this.mRuleset = (IRuleset) new Krietor(this, childNode, iSettings as Krietor.Settings);
                      continue;
                    case King.Settings _:
                      this.mRuleset = (IRuleset) new King(this, childNode, iSettings as King.Settings);
                      continue;
                    default:
                      continue;
                  }
                }
                else
                  break;
            }
          }
        }
        else if (this.RuleSet != null && childNode.Name.Equals("Wave", StringComparison.OrdinalIgnoreCase) && this.mRuleset is SurvivalRuleset)
          ((SurvivalRuleset) this.mRuleset).ReadWave(childNode);
        else if (childNode.Name.Equals("Fog", StringComparison.OrdinalIgnoreCase))
        {
          this.mFog.Enabled = true;
          for (int i6 = 0; i6 < childNode.Attributes.Count; ++i6)
          {
            XmlAttribute attribute = childNode.Attributes[i6];
            if (attribute.Name.Equals("Start", StringComparison.OrdinalIgnoreCase))
              this.mFog.Start = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("End", StringComparison.OrdinalIgnoreCase))
              this.mFog.End = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("Color", StringComparison.OrdinalIgnoreCase))
            {
              string[] strArray = attribute.Value.Split(',');
              this.mFog.Color.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
              this.mFog.Color.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
              this.mFog.Color.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
              this.mFog.Color.W = strArray.Length <= 3 ? 1f : float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            }
          }
        }
        else if (childNode.Name.Equals("Filter", StringComparison.OrdinalIgnoreCase))
        {
          for (int i7 = 0; i7 < childNode.Attributes.Count; ++i7)
          {
            XmlAttribute attribute = childNode.Attributes[i7];
            if (attribute.Name.Equals(nameof (Brightness), StringComparison.OrdinalIgnoreCase))
              this.mBrightness = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals(nameof (Contrast), StringComparison.OrdinalIgnoreCase))
              this.mContrast = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals(nameof (Saturation), StringComparison.OrdinalIgnoreCase))
              this.mSaturation = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        else if (childNode.Name.Equals("Bloom", StringComparison.OrdinalIgnoreCase))
        {
          for (int i8 = 0; i8 < childNode.Attributes.Count; ++i8)
          {
            XmlAttribute attribute = childNode.Attributes[i8];
            if (attribute.Name.Equals("Threshold", StringComparison.OrdinalIgnoreCase))
              this.mBloomThreshold = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("Multiplyer", StringComparison.OrdinalIgnoreCase) || attribute.Name.Equals("Multiplier", StringComparison.OrdinalIgnoreCase))
              this.mBloomMultiplier = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("Blur", StringComparison.OrdinalIgnoreCase))
              this.mBlurSigma = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        else if (childNode.Name.Equals(nameof (SkyMap), StringComparison.OrdinalIgnoreCase))
        {
          this.mSkyMapColor = new Vector3(1f);
          for (int i9 = 0; i9 < childNode.Attributes.Count; ++i9)
          {
            XmlAttribute attribute = childNode.Attributes[i9];
            if (attribute.Name.Equals("color"))
            {
              string[] strArray = attribute.Value.Split(',');
              this.mSkyMapColor.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
              this.mSkyMapColor.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
              this.mSkyMapColor.Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            }
          }
          this.mSkyMapFileName = Path.Combine(directoryName, childNode.InnerText);
        }
        else if (childNode.Name.Equals("RimLight", StringComparison.OrdinalIgnoreCase))
        {
          for (int i10 = 0; i10 < childNode.Attributes.Count; ++i10)
          {
            XmlAttribute attribute = childNode.Attributes[i10];
            if (attribute.Name.Equals("Glow", StringComparison.OrdinalIgnoreCase))
              this.mRimLightGlow = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("Power", StringComparison.OrdinalIgnoreCase))
              this.mRimLightPower = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            if (attribute.Name.Equals("Bias", StringComparison.OrdinalIgnoreCase))
              this.mRimLightBias = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
        }
        else if (childNode.Name.Equals("Clouds", StringComparison.OrdinalIgnoreCase))
        {
          for (int i11 = 0; i11 < childNode.Attributes.Count; ++i11)
          {
            XmlAttribute attribute = childNode.Attributes[i11];
            if (attribute.Name.Equals("Intensity", StringComparison.OrdinalIgnoreCase))
              this.mCloudIntensity = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            else if (attribute.Name.Equals("Scale", StringComparison.OrdinalIgnoreCase))
            {
              string[] strArray = attribute.Value.Split(',');
              if (strArray.Length == 1)
              {
                this.mCloudScale = new Vector2(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
              }
              else
              {
                if (strArray.Length != 2)
                  throw new Exception("Invalid Syntax in; Clouds, Scale!");
                this.mCloudScale = new Vector2(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
              }
            }
            else if (attribute.Name.Equals("Movement", StringComparison.OrdinalIgnoreCase))
            {
              string[] strArray = attribute.Value.Split(',');
              if (strArray.Length != 2)
                throw new Exception("Invalid Syntax in; Clouds, Scale!");
              this.mCloudMovement = new Vector2(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat));
            }
            else if (attribute.Name.Equals("Texture", StringComparison.OrdinalIgnoreCase))
              this.mCloudTexture = iContent.Load<Texture2D>(Path.Combine(directoryName, attribute.Value));
            else if (attribute.Name.Equals("WorldRelative", StringComparison.InvariantCultureIgnoreCase))
              this.mCloudWorldRelative = bool.Parse(attribute.Value);
          }
        }
        else if (childNode.Name.Equals(nameof (Model), StringComparison.OrdinalIgnoreCase))
          this.mModelName = Path.Combine(directoryName, childNode.InnerText);
        else if (childNode.Name.Equals("Trigger", StringComparison.OrdinalIgnoreCase))
        {
          Trigger trigger = Trigger.Read(this, childNode);
          this.mTriggers.Add(trigger.ID, trigger);
        }
        else if (childNode.Name.Equals("OnInteract", StringComparison.OrdinalIgnoreCase))
        {
          Interactable interactable = new Interactable(childNode, this);
          this.mTriggers.Add(interactable.ID, (Trigger) interactable);
        }
        else
        {
          if (!childNode.Name.Equals("KillPlanePosition", StringComparison.OrdinalIgnoreCase))
            throw new NotImplementedException(childNode.Name);
          num = float.Parse(childNode.InnerText, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
      }
    }
    this.mTriggeredActions = new List<Magicka.Levels.Triggers.Actions.Action>();
    this.mSwaySurfaceFormat = SurfaceFormat.HalfVector4;
    this.mUseVertexTexturing = Magicka.Game.Instance.GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(Magicka.Game.Instance.GraphicsDevice.GraphicsDeviceCapabilities.DeviceType, Magicka.Game.Instance.GraphicsDevice.DisplayMode.Format, TextureUsage.None, QueryUsages.VertexTexture, ResourceType.Texture2D, this.mSwaySurfaceFormat);
    if (!this.mUseVertexTexturing)
    {
      this.mSwaySurfaceFormat = SurfaceFormat.Vector4;
      this.mUseVertexTexturing = Magicka.Game.Instance.GraphicsDevice.CreationParameters.Adapter.CheckDeviceFormat(Magicka.Game.Instance.GraphicsDevice.GraphicsDeviceCapabilities.DeviceType, Magicka.Game.Instance.GraphicsDevice.DisplayMode.Format, TextureUsage.None, QueryUsages.VertexTexture, ResourceType.Texture2D, this.mSwaySurfaceFormat);
    }
    if (this.mUseVertexTexturing)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
      {
        this.mSwayTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/SwayTexture");
        this.mCharacterDisplacementTexture = Magicka.Game.Instance.Content.Load<Texture2D>("EffectTextures/CharacterDisplacement");
        this.mSwayEffect = new SwayEffect(Magicka.Game.Instance.GraphicsDevice, Magicka.Game.Instance.Content);
        this.mSwayVertexDeclaration = new VertexDeclaration(Magicka.Game.Instance.GraphicsDevice, VertexPositionTexture.VertexElements);
      }
    }
    this.mKillPlane = new CollisionSkin();
    this.mKillPlane.AddPrimitive((Primitive) new JigLibX.Geometry.Plane(Vector3.Up, -num), 1, new MaterialProperties());
    this.mKillPlane.callbackFn += new CollisionCallbackFn(this.mKillPlane_callbackFn);
  }

  public byte[] ShaHash => this.mShaHash;

  internal void AddEffect(int iId, ref LevelModel.VisualEffectStorage iEffect)
  {
    if (this.mEffects.ContainsKey(iId) && this.mEffects[iId].Effect.IsActive)
      return;
    GameScene.EffectStorage effectStorage;
    effectStorage.Effect = EffectManager.Instance.GetEffect(iEffect.Effect);
    new Vector3().Y = 1f;
    effectStorage.Transform = iEffect.Transform;
    effectStorage.Effect.Start(ref effectStorage.Transform);
    effectStorage.Animation = (AnimatedLevelPart) null;
    effectStorage.Range = iEffect.Range;
    this.mEffects[iId] = effectStorage;
  }

  internal int GetGeneratedEffectID()
  {
    int key = this.mLastGeneratedEffectID + 1;
    while (this.mEffects.ContainsKey(key) && key != 0)
      ++key;
    this.mLastGeneratedEffectID = key;
    return key;
  }

  public void StartEffect(int iId)
  {
    GameScene.EffectStorage effectStorage;
    if (!this.mEffects.TryGetValue(iId, out effectStorage) || effectStorage.Effect.IsActive)
      return;
    effectStorage.Effect.Start(ref effectStorage.Effect.Transform);
    this.mEffects[iId] = effectStorage;
  }

  public bool StopEffect(int iId)
  {
    GameScene.EffectStorage effectStorage;
    if (!this.mEffects.TryGetValue(iId, out effectStorage))
      return false;
    effectStorage.Effect.Stop();
    this.mEffects[iId] = effectStorage;
    return true;
  }

  private bool mKillPlane_callbackFn(
    CollisionSkin skin0,
    int prim0,
    CollisionSkin skin1,
    int prim1)
  {
    if (skin1.Owner != null)
    {
      if (skin1.Owner.Tag is Magicka.GameLogic.Entities.Character)
        (skin1.Owner.Tag as Magicka.GameLogic.Entities.Character).Terminate(true, true, false);
      else if (skin1.Owner.Tag is Entity)
        (skin1.Owner.Tag as Entity).Kill();
    }
    return false;
  }

  public float LightTargetIntensity
  {
    get => this.mTargetLightIntensity;
    set => this.mTargetLightIntensity = value;
  }

  public GameScene.LightSettings[] DirectionalLightSettings => this.mDirectionalLightSettings;

  public TriggerArea GetTriggerArea(int iAreaID) => this.mModel.TriggerAreas[iAreaID];

  public bool TryGetTriggerArea(int iAreaID, out TriggerArea oArea)
  {
    return this.mModel.TriggerAreas.TryGetValue(iAreaID, out oArea);
  }

  public bool LiquidSegmentIntersect(
    out float frac,
    out Vector3 pos,
    out Vector3 nrm,
    ref Segment seg,
    bool ignoreBackfaces,
    bool ignoreWater,
    bool ignoreIce)
  {
    frac = 0.0f;
    pos = new Vector3();
    nrm = new Vector3();
    foreach (Liquid liquid in this.Liquids)
    {
      if (liquid.SegmentIntersect(out frac, out pos, out nrm, ref seg, ignoreBackfaces, ignoreWater, ignoreIce))
        return true;
    }
    return false;
  }

  public bool SegmentIntersect(out float oFrac, out Vector3 oPos, out Vector3 oNrm, Segment iSeg)
  {
    return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out AnimatedLevelPart _, iSeg);
  }

  public bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPos,
    out Vector3 oNrm,
    out AnimatedLevelPart oAnimatedLevelPart,
    Segment iSeg)
  {
    return this.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out int _, iSeg);
  }

  public bool SegmentIntersect(
    out float oFrac,
    out Vector3 oPos,
    out Vector3 oNrm,
    out AnimatedLevelPart oAnimatedLevelPart,
    out int oPrim,
    Segment iSeg)
  {
    if (this.mModel != null)
      return this.mModel.SegmentIntersect(out oFrac, out oPos, out oNrm, out oAnimatedLevelPart, out oPrim, iSeg);
    oFrac = float.MaxValue;
    oPos = new Vector3();
    oNrm = new Vector3();
    oAnimatedLevelPart = (AnimatedLevelPart) null;
    oPrim = 0;
    return false;
  }

  public void UpdateAnimatedLevelParts(DataChannel iDataChannel, float iDeltaTime)
  {
    this.mModel.UpdateAnimatedLevelParts(iDataChannel, iDeltaTime, this);
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if ((double) this.mTargetLightIntensity < (double) this.mLightIntensity)
      this.mLightIntensity -= Math.Max(iDeltaTime, this.mTargetLightIntensity - this.mLightIntensity);
    else
      this.mLightIntensity += Math.Min(iDeltaTime, this.mTargetLightIntensity - this.mLightIntensity);
    for (int index = 0; index < this.mDirectionalLightSettings.Length; ++index)
      this.mDirectionalLightSettings[index].Assign(this.mLightIntensity);
    for (int index = 0; index < this.mSounds.Count; ++index)
    {
      this.mSounds.Values[index].Update(this);
      Cue cue = this.mSounds.Values[index].Cue;
      if (cue.IsStopped || cue.IsStopping)
      {
        this.mSounds.RemoveAt(index);
        --index;
      }
    }
    for (int index = 0; index < this.mTriggers.Count; ++index)
      this.mTriggers.Values[index].Update(iDeltaTime);
    if (this.mRuleset != null)
    {
      if (NetworkManager.Instance.State == NetworkState.Client)
        this.mRuleset.LocalUpdate(iDeltaTime, iDataChannel);
      else
        this.mRuleset.Update(iDeltaTime, iDataChannel);
    }
    if (iDataChannel != DataChannel.None)
      this.PlayState.Scene.AddPreRenderRenderers(iDataChannel, (IPreRenderRenderer) this);
    this.mModel.Update(iDataChannel, iDeltaTime, this);
    for (int index = 0; index < this.mEffects.Count; ++index)
    {
      GameScene.EffectStorage effectStorage = this.mEffects.Values[index];
      if (effectStorage.Effect.IsActive)
      {
        float iScale = 1f;
        Matrix result;
        if (effectStorage.Animation != null)
        {
          result = effectStorage.Animation.AbsoluteTransform;
          Matrix.Multiply(ref effectStorage.Transform, ref result, out result);
        }
        else
          result = effectStorage.Transform;
        if ((double) effectStorage.Range > 0.0)
        {
          Segment segment;
          segment.Origin = result.Translation;
          Vector3 forward = result.Forward;
          Vector3.Multiply(ref forward, effectStorage.Range, out segment.Delta);
          float num;
          if (this.mModel.SegmentIntersect(out num, segment))
            iScale = num;
          foreach (Entity shield in this.PlayState.EntityManager.Shields)
          {
            if (shield.Body.CollisionSkin.SegmentIntersect(out num, out Vector3 _, out Vector3 _, segment) && (double) num < (double) iScale)
              iScale = num;
          }
          MagickaMath.UniformMatrixScale(ref result, iScale);
        }
        effectStorage.Effect.Transform = result;
        effectStorage.Effect.Update(iDeltaTime);
        this.mEffects[this.mEffects.Keys[index]] = effectStorage;
      }
    }
    this.ClearTriggerAreas();
  }

  public void ChangeAmbientSoundVolume(int iID, float iVolume)
  {
    AudioLocator audioLocator;
    if (!this.mSounds.TryGetValue(iID, out audioLocator))
    {
      GameScene.GlobalAudio globalAudio;
      if (!this.mGlobalSounds.TryGetValue(iID, out globalAudio))
        return;
      globalAudio.SetVolume(iVolume);
      this.mGlobalSounds[iID] = globalAudio;
    }
    else
      audioLocator.Cue.SetVariable(GameScene.VOLUME_VAR_NAME, iVolume);
  }

  public void PlayAmbientSound(
    int iID,
    Banks iBank,
    int iCue,
    float iVolume,
    int iLocator,
    float iRadius,
    bool iApply3D)
  {
    if (this.mSounds.ContainsKey(iID))
      return;
    AudioLocator audioLocator = new AudioLocator(iID, iBank, iCue, iVolume, iLocator, iRadius, iApply3D);
    audioLocator.Play();
    this.mSounds.Add(audioLocator.ID, audioLocator);
  }

  public void PlayAmbientSound(int iID, Banks iBank, int iCue, float iVolume)
  {
    GameScene.GlobalAudio globalAudio;
    if (this.mGlobalSounds.TryGetValue(iID, out globalAudio))
    {
      globalAudio.SetVolume(iVolume);
      globalAudio.Play();
      this.mGlobalSounds[iID] = globalAudio;
    }
    else
    {
      globalAudio = new GameScene.GlobalAudio(iBank, iCue, iVolume);
      globalAudio.Play();
      this.mGlobalSounds.Add(iID, globalAudio);
    }
  }

  public void StopSound(int iID, bool mInstant)
  {
    AudioLocator audioLocator;
    if (this.mSounds.TryGetValue(iID, out audioLocator))
    {
      if (mInstant)
        audioLocator.Stop(AudioStopOptions.Immediate);
      else
        audioLocator.Stop(AudioStopOptions.AsAuthored);
      this.mSounds.Remove(iID);
    }
    else
    {
      GameScene.GlobalAudio globalAudio;
      if (!this.mGlobalSounds.TryGetValue(iID, out globalAudio))
        return;
      if (mInstant)
        globalAudio.Stop(AudioStopOptions.Immediate);
      else
        globalAudio.Stop(AudioStopOptions.AsAuthored);
    }
  }

  private void ClearTriggerAreas()
  {
    foreach (TriggerArea triggerArea in this.mModel.TriggerAreas.Values)
      triggerArea.Reset();
    (this.mModel.TriggerAreas[TriggerArea.ANYID] as AnyTriggerArea).Count(this.PlayState.EntityManager);
  }

  public void GetLocator(int iId, out Locator oLocator)
  {
    if (iId == GameScene.ANYAREA)
      iId = this.mRuleset.GetAnyArea();
    if (this.mModel.Locators.TryGetValue(iId, out oLocator))
      return;
    Matrix identity = Matrix.Identity;
    foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
    {
      if (animatedLevelPart.TryGetLocator(iId, ref identity, out oLocator))
        return;
    }
    oLocator = new Locator();
    oLocator.Transform = Matrix.Identity;
  }

  public void GetLocator(int iId, out Matrix oLocator)
  {
    if (iId == GameScene.ANYAREA)
      iId = this.mRuleset.GetAnyArea();
    Locator oLocator1;
    if (this.mModel.Locators.TryGetValue(iId, out oLocator1))
    {
      oLocator = oLocator1.Transform;
    }
    else
    {
      TriggerArea triggerArea;
      if (this.mModel.TriggerAreas.TryGetValue(iId, out triggerArea))
      {
        Matrix.CreateRotationY(MagickaMath.RandomBetween(0.0f, 6.28318548f), out oLocator);
        oLocator.Translation = triggerArea.GetRandomLocation();
      }
      else
      {
        Matrix identity = Matrix.Identity;
        foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
        {
          if (animatedLevelPart.TryGetLocator(iId, ref identity, out oLocator1))
          {
            oLocator = oLocator1.Transform;
            return;
          }
        }
        throw new Exception($"No area with key \"{(object) iId}\" in scene {this.mName}");
      }
    }
  }

  public bool TryGetLocator(int iId, out Matrix oLocator)
  {
    Locator oLocator1;
    if (this.mModel.Locators.TryGetValue(iId, out oLocator1))
    {
      oLocator = oLocator1.Transform;
      return true;
    }
    Matrix identity = Matrix.Identity;
    foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
    {
      if (animatedLevelPart.TryGetLocator(iId, ref identity, out oLocator1))
      {
        oLocator = oLocator1.Transform;
        return true;
      }
    }
    oLocator = Matrix.Identity;
    return false;
  }

  public CollisionSkin CollisionSkin => this.mModel.CollisionSkin;

  public bool ForceNavMesh => this.mForceNavMesh;

  public bool ForceCamera => this.mForceCamera;

  public bool Indoors => this.mIndoor;

  public Level Level
  {
    get => this.mLevel;
    set => this.mLevel = value;
  }

  public Scene Scene => this.mLevel.PlayState.Scene;

  public IRuleset RuleSet => this.mRuleset;

  public string Name => this.mName;

  public int ID => this.mId;

  public BiTreeModel Model => this.mModel.Model;

  public LevelModel LevelModel => this.mModel;

  public Liquid[] Liquids => this.mLiquids;

  public SortedList<int, Trigger> Triggers => this.mTriggers;

  internal NavMesh NavMesh => this.mModel.NavMesh;

  public PlayState PlayState => this.mPlayState;

  public RoomType RoomType => this.mRoomType;

  public float ReverbMix => this.mReverbMix;

  public float Brightness => this.mBrightness;

  public float Contrast => this.mContrast;

  public float Saturation => this.mSaturation;

  public Texture2D SkyMap => this.mSkyMap;

  public Vector3 SkyMapColor => this.mSkyMapColor;

  public void Destroy(bool iSaveNPCs)
  {
    TutorialManager.Instance.HideAll();
    EffectManager.Instance.Clear();
    PhysicsManager.Instance.Clear();
    AudioManager.Instance.ClearMusicFocus();
    foreach (AudioLocator audioLocator in (IEnumerable<AudioLocator>) this.mSounds.Values)
    {
      if (iSaveNPCs)
        audioLocator.Pause();
      else
        audioLocator.Stop(AudioStopOptions.AsAuthored);
    }
    foreach (GameScene.GlobalAudio globalAudio in (IEnumerable<GameScene.GlobalAudio>) this.mGlobalSounds.Values)
    {
      if (iSaveNPCs)
        globalAudio.Pause();
      else
        globalAudio.Stop(AudioStopOptions.AsAuthored);
    }
    this.mSavedAnimations.Clear();
    foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
      animatedLevelPart.AddStateTo(this.mSavedAnimations);
    this.mSavedEntities.Clear();
    this.PlayState.EntityManager.ClearAndStore(iSaveNPCs ? this.mSavedEntities : (List<Entity>) null);
    if (this.mSwayTarget != null && !this.mSwayTarget.IsDisposed)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        this.mSwayTarget.Dispose();
    }
    lock (Magicka.Game.Instance.GraphicsDevice)
    {
      foreach (Light light in this.mModel.Lights.Values)
      {
        light.Disable();
        light.DisposeShadowMap();
      }
      Vector3 iCameraPosition = new Vector3();
      Vector3 iCameraDirection = new Vector3();
      this.mPlayState.Scene.UpdateLights(DataChannel.None, 0.0f, ref iCameraPosition, ref iCameraDirection);
    }
  }

  public void UnloadContent() => this.mContent.Unload();

  public void LoadLevel()
  {
    AnimatedLevelPart.ClearHandles();
    this.mModel = this.mContent.Load<LevelModel>(this.mModelName);
    List<Liquid> iLiquids = new List<Liquid>();
    iLiquids.AddRange((IEnumerable<Liquid>) this.mModel.Waters);
    foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
      animatedLevelPart.GetLiquids(iLiquids);
    this.mLiquids = iLiquids.ToArray();
    if (string.IsNullOrEmpty(this.mSkyMapFileName))
      return;
    this.mSkyMap = this.mContent.Load<Texture2D>(this.mSkyMapFileName);
  }

  public void Initialize(SpawnPoint iSpawnPoint, bool iClearAudio)
  {
    this.Initialize(iSpawnPoint, iClearAudio, (Action<float>) null);
  }

  public unsafe void Initialize(
    SpawnPoint iSpawnPoint,
    bool iClearAudio,
    Action<float> reportbackAction)
  {
    this.mModel.Initialize(this.PlayState);
    ParticleSystem.Instance.Clear();
    ParticleLightBatcher.Instance.Clear();
    PointLightBatcher.Instance.Clear();
    foreach (Trigger trigger in (IEnumerable<Trigger>) this.mTriggers.Values)
      trigger.Initialize();
    this.RestoreSavedAnimations();
    List<DirectionalLight> directionalLightList = new List<DirectionalLight>();
    foreach (Light light in this.mModel.Lights.Values)
    {
      if (light is DirectionalLight directionalLight)
        directionalLightList.Add(directionalLight);
    }
    this.mDirectionalLightSettings = new GameScene.LightSettings[directionalLightList.Count];
    for (int index = 0; index < this.mDirectionalLightSettings.Length; ++index)
      this.mDirectionalLightSettings[index].GetFromLight((Light) directionalLightList[index]);
    if ((double) this.mCloudIntensity > 1.4012984643248171E-45)
    {
      foreach (Light light in this.mModel.Lights.Values)
      {
        if (light is DirectionalLight directionalLight)
        {
          directionalLight.ProjectedTextureIntensity = this.mCloudIntensity;
          directionalLight.ProjectedTextureScale = this.mCloudScale;
          directionalLight.ProjectedTexture = this.mCloudTexture;
        }
      }
    }
    DecalManager.Instance.Clear();
    ShadowBlobs.Instance.Initialize(this.PlayState.Scene);
    this.mModel.RegisterCollisionSkin();
    if (this.mRuleset != null)
      this.mRuleset.Initialize();
    PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.PlayState.Camera.CollisionSkin);
    RenderManager.Instance.Fog = this.mFog;
    RenderManager.Instance.SkyMap = this.mSkyMap;
    RenderManager.Instance.SkyMapColor = this.mSkyMapColor;
    RenderManager.Instance.Brightness = this.mBrightness;
    RenderManager.Instance.Contrast = this.mContrast;
    RenderManager.Instance.Saturation = this.mSaturation;
    RenderManager.Instance.BloomThreshold = this.mBloomThreshold;
    RenderManager.Instance.BloomMultiplier = this.mBloomMultiplier;
    RenderManager.Instance.BlurSigma = this.mBlurSigma;
    SkinnedModelDeferredEffect effect = RenderManager.Instance.GetEffect(SkinnedModelDeferredEffect.TYPEHASH) as SkinnedModelDeferredEffect;
    effect.RimLightGlow = this.mRimLightGlow;
    effect.RimLightPower = this.mRimLightPower;
    effect.RimLightBias = this.mRimLightBias;
    PlayState playState = this.PlayState;
    if (this.mSwayTarget == null || this.mSwayTarget.IsDisposed)
    {
      lock (Magicka.Game.Instance.GraphicsDevice)
        this.mSwayTarget = new RenderTarget2D(Magicka.Game.Instance.GraphicsDevice, 512 /*0x0200*/, 512 /*0x0200*/, 1, this.mSwaySurfaceFormat);
    }
    foreach (AudioLocator audioLocator in (IEnumerable<AudioLocator>) this.mSounds.Values)
    {
      if (iClearAudio)
        audioLocator.Stop(AudioStopOptions.Immediate);
      else
        audioLocator.Play();
    }
    foreach (GameScene.GlobalAudio globalAudio in (IEnumerable<GameScene.GlobalAudio>) this.mGlobalSounds.Values)
    {
      if (iClearAudio)
        globalAudio.Stop(AudioStopOptions.Immediate);
      else
        globalAudio.Play();
    }
    if (iClearAudio)
    {
      this.mSounds.Clear();
      this.mGlobalSounds.Clear();
    }
    Vector2 mVindDirection = this.mVindDirection;
    mVindDirection.X *= 35.35534f;
    mVindDirection.Y *= 35.35534f;
    Vector2 vector2 = mVindDirection;
    vector2.X = vector2.Y;
    vector2.Y = -vector2.X;
    VertexPositionTexture[] data = new VertexPositionTexture[8];
    VertexPositionTexture vertexPositionTexture = new VertexPositionTexture();
    vertexPositionTexture.Position.X = -35.35534f;
    vertexPositionTexture.Position.Z = -35.35534f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[0] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 35.35534f;
    vertexPositionTexture.Position.Z = -35.35534f;
    vertexPositionTexture.TextureCoordinate.X = 0.25f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[1] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 35.35534f;
    vertexPositionTexture.Position.Z = 35.35534f;
    vertexPositionTexture.TextureCoordinate.X = 0.25f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[2] = vertexPositionTexture;
    vertexPositionTexture.Position.X = -35.35534f;
    vertexPositionTexture.Position.Z = 35.35534f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[3] = vertexPositionTexture;
    vertexPositionTexture.Position.X = -1f;
    vertexPositionTexture.Position.Z = -1f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[4] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 1f;
    vertexPositionTexture.Position.Z = -1f;
    vertexPositionTexture.TextureCoordinate.X = 1f;
    vertexPositionTexture.TextureCoordinate.Y = 0.0f;
    data[5] = vertexPositionTexture;
    vertexPositionTexture.Position.X = 1f;
    vertexPositionTexture.Position.Z = 1f;
    vertexPositionTexture.TextureCoordinate.X = 1f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[6] = vertexPositionTexture;
    vertexPositionTexture.Position.X = -1f;
    vertexPositionTexture.Position.Z = 1f;
    vertexPositionTexture.TextureCoordinate.X = 0.0f;
    vertexPositionTexture.TextureCoordinate.Y = 1f;
    data[7] = vertexPositionTexture;
    if (this.mSwayVertices == null || this.mSwayVertices.IsDisposed)
      this.mSwayVertices = new VertexBuffer(Magicka.Game.Instance.GraphicsDevice, data.Length * VertexPositionTexture.SizeInBytes, BufferUsage.WriteOnly);
    lock (Magicka.Game.Instance.GraphicsDevice)
      this.mSwayVertices.SetData<VertexPositionTexture>(data);
    if (this.mFirstStart)
    {
      this.mFirstStart = false;
      this.mModel.CreatePhysicsEntities(this.mSavedEntities, this.PlayState);
    }
    if (!this.mIsInitialized)
      this.mModel.GetAllEffects(this.mEffects);
    this.AddSavedEntities();
    this.PlayState.Camera.SnapPrimitive = (Primitive) this.mModel.CameraMesh;
    this.PlayState.Camera.Release(0.0f);
    for (int index = 0; index < this.mModel.Waters.Length; ++index)
      this.mModel.Waters[index].Initialize();
    foreach (Light light in this.mModel.Lights.Values)
    {
      light.CreateShadowMap();
      light.Enable(playState.Scene);
    }
    foreach (Trigger trigger in (IEnumerable<Trigger>) this.mTriggers.Values)
      trigger.ResetTimers();
    foreach (TriggerArea triggerArea in this.mModel.TriggerAreas.Values)
      triggerArea.Register();
    PhysicsManager.Instance.Simulator.CollisionSystem.AddCollisionSkin(this.mKillPlane);
    if (iSpawnPoint.SpawnPlayers)
    {
      Vector3 zero = Vector3.Zero;
      for (int index1 = 0; index1 < Magicka.Game.Instance.Players.Length; ++index1)
      {
        Matrix orientation = Matrix.Identity;
        Vector3 pos = new Vector3();
        int index2;
        for (index2 = index1; index2 >= 0; --index2)
        {
          Locator locator;
          if (this.mModel.Locators.TryGetValue(iSpawnPoint.Locations[index2], out locator))
          {
            orientation = locator.Transform;
            pos = locator.Transform.Translation;
            orientation.Translation = new Vector3();
            break;
          }
        }
        pos.X += 2f * (float) (index1 - index2);
        ++pos.Y;
        zero += pos;
        Magicka.GameLogic.Player player = Magicka.Game.Instance.Players[index1];
        if (player.Playing && player.Avatar != null && !player.Avatar.Dead)
        {
          Segment iSeg = new Segment();
          iSeg.Delta.Y = -10f;
          iSeg.Origin = pos;
          ++iSeg.Origin.Y;
          Vector3 oPos;
          if (this.SegmentIntersect(out float _, out oPos, out Vector3 _, iSeg))
          {
            pos = oPos;
            pos.Y += player.Avatar.Capsule.Length * 0.5f + player.Avatar.Capsule.Radius;
          }
          player.Avatar.CharacterBody.MoveTo(pos, orientation);
          player.Avatar.CharacterBody.DesiredDirection = orientation.Forward;
          player.Avatar.Events = (AIEvent[]) null;
          player.Avatar.Path.Clear();
          player.Avatar.CharacterBody.Movement = new Vector3();
          playState.EntityManager.AddEntity((Entity) player.Avatar);
          player.Avatar.Body.EnableBody();
        }
      }
      this.PlayState.Camera.SetPosition(zero / (float) Magicka.Game.Instance.Players.Length, true);
      playState.SpawnFairies();
    }
    this.RunStartupActions(reportbackAction);
    this.mIsInitialized = true;
  }

  public void RestoreSavedAnimations()
  {
    if (this.mSavedAnimations.Count <= 0)
      return;
    foreach (AnimatedLevelPart animatedLevelPart in this.mModel.AnimatedLevelParts.Values)
    {
      animatedLevelPart.RestoreStateFrom(this.mSavedAnimations);
      Matrix identity = Matrix.Identity;
      animatedLevelPart.Update(DataChannel.None, 0.0f, ref identity, this);
    }
  }

  public void RestoreDynamicLights()
  {
    this.mLevel.PlayState.Scene.ClearLights();
    foreach (Light light in this.mModel.Lights.Values)
    {
      light.CreateShadowMap();
      light.Enable(this.mLevel.PlayState.Scene);
    }
  }

  public void AddSavedEntities()
  {
    for (int index = 0; index < this.mSavedEntities.Count; ++index)
    {
      Entity mSavedEntity = this.mSavedEntities[index];
      mSavedEntity.Body.EnableBody();
      if (mSavedEntity is NonPlayerCharacter nonPlayerCharacter)
        nonPlayerCharacter.AI.Enable();
      this.mPlayState.EntityManager.AddEntity(mSavedEntity);
    }
    this.mSavedEntities.Clear();
  }

  public int GetNumStartupActions()
  {
    if (this.mStartupActions == null)
      return 0;
    lock (this.mStartupActions)
      return this.mStartupActions.Count;
  }

  public void RunStartupActions() => this.RunStartupActions((Action<float>) null);

  public void RunStartupActions(Action<float> reportBackAction)
  {
    if (NetworkManager.Instance.State != NetworkState.Offline)
      NetworkManager.Instance.Interface.Sync();
    float count = (float) this.mStartupActions.Count;
    lock (this.mStartupActions)
    {
      for (int index = 0; index < this.mStartupActions.Count; ++index)
      {
        this.mStartupActions[index].QuickExecute();
        float num = ((float) index + 1f) / count;
        if (reportBackAction != null)
          reportBackAction(num);
      }
      this.mStartupActions.Clear();
    }
  }

  public void PreRenderUpdate(
    DataChannel iDataChannel,
    float iDeltaTime,
    ref Matrix iViewProjectionMatrix,
    ref Vector3 iCameraPosition,
    ref Vector3 iCameraDirection)
  {
    if ((double) this.mCloudIntensity > 1.4012984643248171E-45)
    {
      Vector2 result1;
      Vector2.Multiply(ref this.mCloudMovement, iDeltaTime, out result1);
      Vector2.Add(ref this.mCloudOffset, ref result1, out this.mCloudOffset);
      foreach (Light light in this.mModel.Lights.Values)
      {
        if (light is DirectionalLight directionalLight)
        {
          if (this.mCloudWorldRelative)
          {
            Vector3 position = new Vector3();
            position.X = this.mCloudOffset.X;
            position.Z = this.mCloudOffset.Y;
            Matrix lightOrientation = directionalLight.LightOrientation;
            Vector3 result2;
            Vector3.Transform(ref position, ref lightOrientation, out result2);
            directionalLight.ProjectedTextureOffset = new Vector2()
            {
              X = -result2.X,
              Y = result2.Y
            };
          }
          else
            directionalLight.ProjectedTextureOffset = this.mCloudOffset;
        }
      }
    }
    EntityManager entityManager = this.PlayState.EntityManager;
    lock (this)
    {
      if (entityManager == null)
        return;
      float scaleFactor = 224f;
      Vector3 up = Vector3.Up;
      Vector3 result3;
      Vector3.Multiply(ref iCameraDirection, scaleFactor, out result3);
      Vector3.Add(ref result3, ref iCameraPosition, out result3);
      Vector3 result4;
      Vector3.Multiply(ref up, 10f, out result4);
      Vector3.Add(ref result3, ref result4, out result4);
      Vector3 forward = Vector3.Forward;
      Matrix result5;
      Matrix.CreateLookAt(ref result4, ref result3, ref forward, out result5);
      Matrix result6;
      Matrix.CreateOrthographic(50f, 50f, 0.0f, 20f, out result6);
      Matrix.Multiply(ref result5, ref result6, out this.mVisualAidViewProjection);
      GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
      DepthStencilBuffer depthStencilBuffer = graphicsDevice.DepthStencilBuffer;
      if (this.mUseVertexTexturing)
        this.DrawSway(iDeltaTime, ref result3, entityManager);
      graphicsDevice.SetRenderTarget(0, (RenderTarget2D) null);
      graphicsDevice.DepthStencilBuffer = depthStencilBuffer;
      if (!this.mUseVertexTexturing || this.mSwayTarget == null || this.mSwayTarget.IsDisposed)
        return;
      RenderDeferredEffect effect = RenderManager.Instance.GetEffect(RenderDeferredEffect.TYPEHASH) as RenderDeferredEffect;
      effect.SwayTexture = this.mSwayTarget.GetTexture();
      effect.SwayProjection = this.mVisualAidViewProjection;
    }
  }

  private void DrawSway(float iDeltaTime, ref Vector3 iOrigo, EntityManager iEntityManager)
  {
    if (this.mSwayTarget == null || this.mSwayTarget.IsDisposed)
      return;
    GraphicsDevice graphicsDevice = Magicka.Game.Instance.GraphicsDevice;
    Matrix result1;
    Matrix.CreateRotationY(1f, out result1);
    this.mWindPosition += iDeltaTime * this.mVindSpeed;
    Vector3 result2 = new Vector3();
    result2.X = this.mWindPosition;
    Vector3.Transform(ref result2, ref result1, out result2);
    result1.Translation = result2;
    result1.M41 -= (float) ((double) iOrigo.X * 0.014142136089503765 * 0.25);
    result1.M42 -= iOrigo.Z * 0.0141421361f;
    this.mSwayEffect.TextureTransform = result1;
    result1 = Matrix.Identity with
    {
      M41 = iOrigo.X,
      M42 = iOrigo.Y,
      M43 = iOrigo.Z
    };
    this.mSwayEffect.World = result1;
    this.mSwayEffect.ViewProjection = this.mVisualAidViewProjection;
    this.mSwayEffect.Texture = this.mSwayTexture;
    this.mSwayEffect.SetTechnique(SwayEffect.Technique.Sway);
    graphicsDevice.SetRenderTarget(0, this.mSwayTarget);
    graphicsDevice.DepthStencilBuffer = (DepthStencilBuffer) null;
    graphicsDevice.VertexDeclaration = this.mSwayVertexDeclaration;
    graphicsDevice.RenderState.CullMode = CullMode.None;
    graphicsDevice.RenderState.DepthBufferEnable = false;
    graphicsDevice.RenderState.AlphaBlendEnable = false;
    graphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
    graphicsDevice.Vertices[0].SetSource(this.mSwayVertices, 0, VertexPositionTexture.SizeInBytes);
    graphicsDevice.VertexDeclaration = this.mSwayVertexDeclaration;
    this.mSwayEffect.Begin();
    this.mSwayEffect.CurrentTechnique.Passes[0].Begin();
    graphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 0, 2);
    this.mSwayEffect.CurrentTechnique.Passes[0].End();
    this.mSwayEffect.End();
    this.mSwayEffect.Texture = this.mCharacterDisplacementTexture;
    this.mSwayEffect.SetTechnique(SwayEffect.Technique.CharacterOffset);
    this.mSwayEffect.Begin();
    this.mSwayEffect.CurrentTechnique.Passes[0].Begin();
    List<Magicka.GameLogic.Entities.Character> chars = new List<Magicka.GameLogic.Entities.Character>();
    iEntityManager.GetCharacters(ref chars);
    lock (chars)
    {
      for (int index = 0; index < chars.Count; ++index)
      {
        Magicka.GameLogic.Entities.Character character = chars[index];
        Vector3 position = character.Position;
        float radius = character.Radius;
        result1.M41 = position.X;
        result1.M42 = iOrigo.Y;
        result1.M43 = position.Z;
        result1.M11 = radius;
        result1.M22 = radius;
        result1.M33 = radius;
        this.mSwayEffect.World = result1;
        this.mSwayEffect.CommitChanges();
        graphicsDevice.DrawPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleFan, 4, 2);
      }
    }
    this.mSwayEffect.CurrentTechnique.Passes[0].End();
    this.mSwayEffect.End();
    graphicsDevice.RenderState.ColorWriteChannels = ColorWriteChannels.All;
  }

  public void ActionExecute(Magicka.Levels.Triggers.Actions.Action iAction)
  {
    if (iAction is Spawn)
      this.mTriggeredActions.Remove(iAction);
    this.mTriggeredActions.Add(iAction);
  }

  public void Dispose()
  {
    lock (this)
    {
      if (this.mModel != null)
      {
        foreach (GameScene.GlobalAudio globalAudio in (IEnumerable<GameScene.GlobalAudio>) this.mGlobalSounds.Values)
          globalAudio.Stop(AudioStopOptions.Immediate);
        this.mGlobalSounds = (SortedList<int, GameScene.GlobalAudio>) null;
        foreach (AudioLocator audioLocator in (IEnumerable<AudioLocator>) this.mSounds.Values)
          audioLocator.Stop(AudioStopOptions.Immediate);
        this.mSounds = (SortedList<int, AudioLocator>) null;
        this.mModel.Dispose();
        this.mModel = (LevelModel) null;
        this.mLiquids = (Liquid[]) null;
      }
      if (this.mSwayTarget != null)
      {
        this.mSwayTarget.Dispose();
        this.mSwayTarget = (RenderTarget2D) null;
      }
      if (this.mRuleset != null)
        this.mRuleset.DeInitialize();
    }
    this.mContent.Dispose();
  }

  public void ExecuteTrigger(int iTrigger, Magicka.GameLogic.Entities.Character iArg, bool iIgnoreConditions)
  {
    if (NetworkManager.Instance.State == NetworkState.Client)
      return;
    this.mTriggers[iTrigger].Execute(iArg, iIgnoreConditions);
  }

  public struct GlobalAudio
  {
    private Cue mCue;
    private Banks mBank;
    private int mCueID;
    private float mVolume;

    public GlobalAudio(Banks iBank, int iCue, float iVolume)
    {
      this.mBank = iBank;
      this.mCueID = iCue;
      this.mCue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
      this.mVolume = iVolume;
      this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, this.mVolume);
    }

    public Banks Bank => this.mBank;

    public int CueID => this.mCueID;

    public float Volume => this.mVolume;

    public void Play()
    {
      if (this.mCue == null || this.mCue.IsStopped || this.mCue.IsStopping)
      {
        this.mCue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
        this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, this.mVolume);
      }
      if (this.mCue.IsPaused)
      {
        this.mCue.Resume();
      }
      else
      {
        if (this.mCue.IsPlaying)
          return;
        this.mCue.Play();
      }
    }

    public void Pause()
    {
      if (this.mCue == null || !this.mCue.IsPlaying)
        return;
      this.mCue.Pause();
    }

    public void Stop(AudioStopOptions iOptions)
    {
      if (this.mCue == null || this.mCue.IsStopped || this.mCue.IsStopping)
        return;
      this.mCue.Stop(iOptions);
    }

    public void SetVolume(float iValue)
    {
      this.mVolume = iValue;
      this.mCue.SetVariable(GameScene.VOLUME_VAR_NAME, iValue);
    }
  }

  public struct LightSettings
  {
    public Light Light;
    public Vector3 AmbientColor;
    public Vector3 DiffuseColor;
    public float SpecularAmount;

    public void Assign()
    {
      this.Light.AmbientColor = this.AmbientColor;
      this.Light.DiffuseColor = this.DiffuseColor;
      this.Light.SpecularAmount = this.SpecularAmount;
    }

    public void Assign(float iIntensity)
    {
      Vector3 result;
      Vector3.Multiply(ref this.AmbientColor, iIntensity, out result);
      this.Light.AmbientColor = result;
      Vector3.Multiply(ref this.DiffuseColor, iIntensity, out result);
      this.Light.DiffuseColor = result;
      this.Light.SpecularAmount = this.SpecularAmount * iIntensity;
    }

    public void GetFromLight(Light iLight)
    {
      this.Light = iLight;
      this.AmbientColor = this.Light.AmbientColor;
      this.DiffuseColor = this.Light.DiffuseColor;
      this.SpecularAmount = this.Light.SpecularAmount;
    }
  }

  public struct EffectStorage
  {
    public VisualEffect Effect;
    public Matrix Transform;
    public AnimatedLevelPart Animation;
    public float Range;
  }

  public class State
  {
    private GameScene mScene;
    private Trigger.State[] mTriggers;
    private bool mFirstStart;
    private EntityStateStorage mSavedEntities;
    private List<Magicka.Levels.Triggers.Actions.Action> mTriggeredActions;
    private Dictionary<int, GameScene.GlobalAudio> mGlobalAudio = new Dictionary<int, GameScene.GlobalAudio>();
    private Dictionary<int, AudioLocator> mAmbientAudio = new Dictionary<int, AudioLocator>();
    private Dictionary<int, AnimatedLevelPart.AnimationState> mAnimations = new Dictionary<int, AnimatedLevelPart.AnimationState>();

    public State(GameScene iScene)
    {
      this.mScene = iScene;
      this.mSavedEntities = new EntityStateStorage(iScene.PlayState);
      this.mTriggers = new Trigger.State[this.mScene.mTriggers.Count];
      int num = 0;
      foreach (Trigger trigger in (IEnumerable<Trigger>) this.mScene.mTriggers.Values)
        this.mTriggers[num++] = trigger.GetState();
      this.mTriggeredActions = new List<Magicka.Levels.Triggers.Actions.Action>(iScene.mTriggeredActions.Capacity);
      this.UpdateState();
    }

    public void UpdateState()
    {
      this.mFirstStart = this.mScene.mFirstStart;
      this.mSavedEntities.Clear();
      if (this.mScene.Level.CurrentScene == this.mScene)
        this.mSavedEntities.Store((IEnumerable<Entity>) this.mScene.PlayState.EntityManager.Entities);
      else
        this.mSavedEntities.Store((IEnumerable<Entity>) this.mScene.mSavedEntities);
      this.mTriggeredActions.Clear();
      this.mTriggeredActions.AddRange((IEnumerable<Magicka.Levels.Triggers.Actions.Action>) this.mScene.mTriggeredActions);
      this.mGlobalAudio.Clear();
      foreach (KeyValuePair<int, GameScene.GlobalAudio> mGlobalSound in this.mScene.mGlobalSounds)
        this.mGlobalAudio.Add(mGlobalSound.Key, mGlobalSound.Value);
      this.mAmbientAudio.Clear();
      foreach (KeyValuePair<int, AudioLocator> mSound in this.mScene.mSounds)
        this.mAmbientAudio.Add(mSound.Key, mSound.Value);
      this.mAnimations.Clear();
      if (this.mScene.Level.CurrentScene == this.mScene)
      {
        foreach (AnimatedLevelPart animatedLevelPart in this.mScene.mModel.AnimatedLevelParts.Values)
          animatedLevelPart.AddStateTo(this.mAnimations);
      }
      else
      {
        foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> mSavedAnimation in this.mScene.mSavedAnimations)
          this.mAnimations.Add(mSavedAnimation.Key, mSavedAnimation.Value);
      }
      for (int index = 0; index < this.mTriggers.Length; ++index)
        this.mTriggers[index].UpdateState();
    }

    public void ApplyState(List<int> iIgnoredTriggers)
    {
      this.mScene.mFirstStart = this.mFirstStart;
      this.mScene.mSavedEntities.Clear();
      this.mSavedEntities.Restore(this.mScene.mSavedEntities);
      this.mScene.mGlobalSounds.Clear();
      foreach (KeyValuePair<int, GameScene.GlobalAudio> keyValuePair in this.mGlobalAudio)
        this.mScene.mGlobalSounds.Add(keyValuePair.Key, keyValuePair.Value);
      this.mScene.mSounds.Clear();
      foreach (KeyValuePair<int, AudioLocator> keyValuePair in this.mAmbientAudio)
        this.mScene.mSounds.Add(keyValuePair.Key, keyValuePair.Value);
      this.mScene.mSavedAnimations.Clear();
      foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> mAnimation in this.mAnimations)
        this.mScene.mSavedAnimations.Add(mAnimation.Key, mAnimation.Value);
      this.mScene.mTriggeredActions.Clear();
      this.mScene.mTriggeredActions.AddRange((IEnumerable<Magicka.Levels.Triggers.Actions.Action>) this.mTriggeredActions);
      if (iIgnoredTriggers != null)
      {
        for (int index = 0; index < this.mTriggers.Length; ++index)
        {
          if (iIgnoredTriggers.Contains(this.mTriggers[index].Trigger.ID))
            this.mTriggers[index].ResetState();
          else
            this.mTriggers[index].ApplyState();
        }
        this.mScene.mStartupActions.Clear();
        for (int index = 0; index < this.mTriggeredActions.Count; ++index)
        {
          if (!iIgnoredTriggers.Contains(this.mTriggeredActions[index].Trigger.ID))
            this.mScene.mStartupActions.Add(this.mTriggeredActions[index]);
        }
      }
      else
      {
        for (int index = 0; index < this.mTriggers.Length; ++index)
          this.mTriggers[index].ApplyState();
        this.mScene.mStartupActions.Clear();
        this.mScene.mStartupActions.AddRange((IEnumerable<Magicka.Levels.Triggers.Actions.Action>) this.mTriggeredActions);
      }
    }

    public GameScene Scene => this.mScene;

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mFirstStart);
      this.mSavedEntities.Write(iWriter);
      iWriter.Write(this.mGlobalAudio.Count);
      foreach (KeyValuePair<int, GameScene.GlobalAudio> keyValuePair in this.mGlobalAudio)
      {
        iWriter.Write(keyValuePair.Key);
        iWriter.Write((ushort) keyValuePair.Value.Bank);
        iWriter.Write(keyValuePair.Value.CueID);
        iWriter.Write(keyValuePair.Value.Volume);
      }
      iWriter.Write(this.mAmbientAudio.Count);
      foreach (AudioLocator audioLocator in this.mAmbientAudio.Values)
      {
        iWriter.Write(audioLocator.ID);
        iWriter.Write((ushort) audioLocator.Bank);
        iWriter.Write(audioLocator.CueID);
        iWriter.Write(audioLocator.Volume);
        iWriter.Write(audioLocator.Locator);
        iWriter.Write(audioLocator.Radius);
        iWriter.Write(audioLocator.Apply3D);
      }
      iWriter.Write(this.mAnimations.Count);
      foreach (KeyValuePair<int, AnimatedLevelPart.AnimationState> mAnimation in this.mAnimations)
      {
        iWriter.Write(mAnimation.Key);
        mAnimation.Value.Write(iWriter);
      }
      for (int index = 0; index < this.mTriggers.Length; ++index)
        this.mTriggers[index].Write(iWriter);
      iWriter.Write(this.mTriggeredActions.Count);
      for (int index = 0; index < this.mTriggeredActions.Count; ++index)
        iWriter.Write(this.mTriggeredActions[index].Handle);
    }

    internal void Read(BinaryReader iReader)
    {
      this.mFirstStart = iReader.ReadBoolean();
      this.mSavedEntities.Clear();
      this.mSavedEntities.Read(iReader);
      this.mGlobalAudio.Clear();
      int num1 = iReader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.mGlobalAudio.Add(iReader.ReadInt32(), new GameScene.GlobalAudio((Banks) iReader.ReadUInt16(), iReader.ReadInt32(), iReader.ReadSingle()));
      this.mAmbientAudio.Clear();
      int num2 = iReader.ReadInt32();
      for (int index = 0; index < num2; ++index)
      {
        int num3 = iReader.ReadInt32();
        Banks iBank = (Banks) iReader.ReadUInt16();
        int iCue = iReader.ReadInt32();
        float iVolume = iReader.ReadSingle();
        int iLocator = iReader.ReadInt32();
        float iRadius = iReader.ReadSingle();
        bool iApply3D = iReader.ReadBoolean();
        AudioLocator audioLocator = new AudioLocator(num3, iBank, iCue, iVolume, iLocator, iRadius, iApply3D);
        this.mAmbientAudio.Add(num3, audioLocator);
      }
      this.mAnimations.Clear();
      int num4 = iReader.ReadInt32();
      for (int index = 0; index < num4; ++index)
        this.mAnimations.Add(iReader.ReadInt32(), new AnimatedLevelPart.AnimationState(iReader));
      for (int index = 0; index < this.mTriggers.Length; ++index)
        this.mTriggers[index].Read(iReader);
      this.mTriggeredActions.Clear();
      int num5 = iReader.ReadInt32();
      for (int index = 0; index < num5; ++index)
        this.mTriggeredActions.Add(Magicka.Levels.Triggers.Actions.Action.GetByHandle(iReader.ReadUInt16()));
    }
  }
}
