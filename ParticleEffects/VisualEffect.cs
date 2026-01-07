// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.VisualEffect
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using PolygonHead.Lights;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

#nullable disable
namespace PolygonHead.ParticleEffects;

public struct VisualEffect
{
  private PointLightBatcher.PointLightReference mLightReference;
  private bool mLightEnabled;
  private Vector3 mLightOffset;
  private PointLightBatcher.BatchedPointLight mLight;
  private LightTransitionType mLightSpawnTransition;
  private float mLightSpawnTransitionTime;
  private LightTransitionType mLightDespawnTransition;
  private float mLightDespawnTransitionTime;
  private static Dictionary<string, System.Type> mEmitterTypes;
  private bool mActive;
  private float mTotalTime;
  private float mTimeLinePos;
  private float mDuration;
  private VisualEffectType mType;
  private ParticleEmitterCollection mEmitters;
  public Matrix Transform;
  public Matrix LastTransform;

  public VisualEffect(float iDuration, VisualEffectType iType, ParticleEmitterCollection iEmitters)
  {
    this.mActive = false;
    this.mTimeLinePos = 0.0f;
    this.mTotalTime = 0.0f;
    this.mDuration = iDuration;
    this.mType = iType;
    this.mEmitters = iEmitters;
    this.Transform = Matrix.Identity;
    this.LastTransform = this.Transform;
    this.mLightEnabled = false;
    this.mLightReference = new PointLightBatcher.PointLightReference();
    this.mLightOffset = new Vector3();
    this.mLight = new PointLightBatcher.BatchedPointLight();
    this.mLightSpawnTransition = LightTransitionType.None;
    this.mLightSpawnTransitionTime = 0.0f;
    this.mLightDespawnTransition = LightTransitionType.None;
    this.mLightDespawnTransitionTime = 0.0f;
  }

  public void Start(ref Matrix iTransform)
  {
    this.mActive = true;
    this.Transform = iTransform;
    this.LastTransform = iTransform;
    this.mTimeLinePos = 0.0f;
    this.mTotalTime = 0.0f;
  }

  public void Stop()
  {
    this.mActive = false;
    PointLightBatcher.Instance.DisableLight(ref this.mLightReference, this.mLightDespawnTransition, this.mLightDespawnTransitionTime);
  }

  public void Reset()
  {
    this.mTotalTime = 0.0f;
    this.mTimeLinePos = 0.0f;
  }

  public void Update(float iDeltaTime)
  {
    if (this.mLightEnabled)
    {
      Vector3 result;
      Vector3.Transform(ref this.mLightOffset, ref this.Transform, out result);
      if (!PointLightBatcher.Instance.IsLightActive(ref this.mLightReference))
        PointLightBatcher.Instance.SpawnLight(ref result, ref this.mLight, out this.mLightReference, this.mLightSpawnTransition, this.mLightSpawnTransitionTime);
      else
        PointLightBatcher.Instance.SetLightPosition(ref result, ref this.mLightReference);
    }
    if ((double) iDeltaTime < 1.4012984643248171E-45 || this.mEmitters == null)
      return;
    if (this.mActive)
    {
      float mTimeLinePos = this.mTimeLinePos;
      float mTotalTime = this.mTotalTime;
      this.mTotalTime += iDeltaTime;
      this.mTimeLinePos += iDeltaTime;
      switch (this.mType)
      {
        case VisualEffectType.Looping:
          this.mTimeLinePos %= this.mDuration;
          break;
        case VisualEffectType.Infinite:
          if ((double) this.mTimeLinePos > (double) this.mDuration)
          {
            this.mTimeLinePos = this.mDuration;
            break;
          }
          break;
        default:
          if ((double) this.mTimeLinePos > (double) this.mDuration)
          {
            this.Stop();
            break;
          }
          break;
      }
      this.mEmitters.Update(iDeltaTime, mTotalTime, this.mTotalTime, this.mTimeLinePos, mTimeLinePos, ref this.Transform, ref this.LastTransform);
    }
    this.LastTransform = this.Transform;
  }

  public bool IsActive => this.mActive;

  public float Time => this.mTimeLinePos;

  public float Duration => this.mDuration;

  public VisualEffectType Type => this.mType;

  public ParticleEmitterCollection Emitters => this.mEmitters;

  public static void Clone(ref VisualEffect iSource, ref VisualEffect oTarget)
  {
    oTarget.mEmitters.Clear();
    for (int index = 0; index < iSource.mEmitters.Count; ++index)
      oTarget.mEmitters.Add(iSource.mEmitters[index]);
    oTarget.mActive = iSource.mActive;
    oTarget.mTimeLinePos = iSource.mTimeLinePos;
    oTarget.mDuration = iSource.mDuration;
    oTarget.mType = iSource.mType;
    oTarget.Transform = iSource.Transform;
  }

  private static System.Type GetEmitterType(string iName)
  {
    if (VisualEffect.mEmitterTypes == null)
      VisualEffect.mEmitterTypes = new Dictionary<string, System.Type>();
    System.Type emitterType;
    if (VisualEffect.mEmitterTypes.TryGetValue(iName, out emitterType))
      return emitterType;
    Assembly assembly = typeof (IParticleEmitter).Assembly;
    System.Type[] typeArray;
    try
    {
      typeArray = assembly.GetTypes();
    }
    catch
    {
      try
      {
        typeArray = assembly.GetModules()[0].GetTypes();
      }
      catch
      {
        typeArray = new System.Type[2]
        {
          typeof (ContinuousEmitter),
          typeof (PulseEmitter)
        };
      }
    }
    for (int index = 0; index < typeArray.Length; ++index)
    {
      foreach (System.Type type in typeArray[index].GetInterfaces())
      {
        if (type == typeof (IParticleEmitter))
        {
          if (typeArray[index].Name.Equals(iName))
          {
            VisualEffect.mEmitterTypes.Add(iName, typeArray[index]);
            return typeArray[index];
          }
          break;
        }
      }
    }
    return (System.Type) null;
  }

  public static VisualEffect FromFile(string iFileName)
  {
    XmlDocument iDoc = new XmlDocument();
    iDoc.Load(iFileName);
    return VisualEffect.FromFile(iDoc);
  }

  public static VisualEffect FromFile(XmlDocument iDoc)
  {
    VisualEffect visualEffect = new VisualEffect();
    visualEffect.mEmitters = new ParticleEmitterCollection();
    int num1 = 10;
    int num2 = 1;
    for (int i1 = 0; i1 < iDoc.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iDoc.ChildNodes[i1];
      if (childNode1.Name.Equals("Effect", StringComparison.OrdinalIgnoreCase))
      {
        for (int i2 = 0; i2 < childNode1.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode1.Attributes[i2];
          if (attribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
            visualEffect.mType = (VisualEffectType) Enum.Parse(typeof (VisualEffectType), attribute.Value, true);
          else if (attribute.Name.Equals("duration", StringComparison.OrdinalIgnoreCase))
            visualEffect.mDuration = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          else if (attribute.Name.Equals("keyFramesPerSecond", StringComparison.OrdinalIgnoreCase))
            num1 = int.Parse(attribute.Value);
          else if (attribute.Name.Equals("version", StringComparison.OrdinalIgnoreCase))
            num2 = int.Parse(attribute.Value);
        }
        for (int i3 = 0; i3 < childNode1.ChildNodes.Count; ++i3)
        {
          XmlNode childNode2 = childNode1.ChildNodes[i3];
          System.Type emitterType = VisualEffect.GetEmitterType(childNode2.Name);
          if (emitterType != null)
          {
            IParticleEmitter particleEmitter = emitterType.GetConstructor(new System.Type[3]
            {
              typeof (XmlNode),
              typeof (int),
              typeof (int)
            }).Invoke(new object[3]
            {
              (object) childNode2,
              (object) num1,
              (object) num2
            }) as IParticleEmitter;
            visualEffect.mEmitters.Add(particleEmitter);
          }
          else if (childNode2.Name.Equals("Light", StringComparison.OrdinalIgnoreCase))
          {
            visualEffect.mLightEnabled = true;
            foreach (XmlNode childNode3 in childNode2.ChildNodes)
            {
              if (!(childNode3 is XmlComment))
              {
                foreach (XmlAttribute attribute in (XmlNamedNodeMap) childNode3.Attributes)
                {
                  if (attribute.Name.Equals("Value", StringComparison.OrdinalIgnoreCase))
                  {
                    if (childNode3.Name.Equals("PositionX", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightOffset.X = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("PositionY", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightOffset.Y = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("PositionZ", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightOffset.Z = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("DiffuseR", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.DiffuseColor.X = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("DiffuseG", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.DiffuseColor.Y = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("DiffuseB", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.DiffuseColor.Z = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("AmbientR", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.AmbientColor.X = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("AmbientG", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.AmbientColor.Y = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("AmbientB", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.AmbientColor.Z = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("Specular", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.SpecularAmount = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("Radius", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.Radius = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("VariationType", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.VariationType = (LightVariationType) Enum.Parse(typeof (LightVariationType), attribute.Value, true);
                    else if (childNode3.Name.Equals("VariationSpeed", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.VariationSpeed = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("VariationAmount", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLight.VariationAmount = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("SpawnTransitionType", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightSpawnTransition = (LightTransitionType) Enum.Parse(typeof (LightTransitionType), attribute.Value, true);
                    else if (childNode3.Name.Equals("SpawnTransitionTime", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightSpawnTransitionTime = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                    else if (childNode3.Name.Equals("DespawnTransitionType", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightDespawnTransition = (LightTransitionType) Enum.Parse(typeof (LightTransitionType), attribute.Value, true);
                    else if (childNode3.Name.Equals("DespawnTransitionTime", StringComparison.OrdinalIgnoreCase))
                      visualEffect.mLightDespawnTransitionTime = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                  }
                }
              }
            }
          }
        }
      }
    }
    return visualEffect;
  }

  public override int GetHashCode()
  {
    return this.mEmitters == null ? (int) ((byte) (this.mActive.GetHashCode() + this.mDuration.GetHashCode()) + this.mType - (byte) 1) : (int) ((byte) (this.mActive.GetHashCode() + this.mDuration.GetHashCode()) + this.mType + (byte) this.mEmitters.GetHashCode());
  }

  public override bool Equals(object obj)
  {
    return obj is VisualEffect visualEffect && (double) visualEffect.Duration == (double) this.Duration && visualEffect.mType == this.mType && visualEffect.mEmitters == this.mEmitters && visualEffect.Transform == this.Transform;
  }
}
