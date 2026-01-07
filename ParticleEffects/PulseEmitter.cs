// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.PulseEmitter
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

#nullable disable
namespace PolygonHead.ParticleEffects;

public class PulseEmitter : IParticleEmitter
{
  protected int mNrOfParticles;
  protected float mPositionX;
  protected float mPositionY;
  protected float mPositionZ;
  protected float mPositionXOffset;
  protected float mPositionYOffset;
  protected float mPositionZOffset;
  protected float mRotationY;
  protected byte mParticle;
  protected float mGravity;
  protected float mDrag;
  protected bool mAlphaBlended;
  protected bool mHSV;
  protected float mColorR;
  protected float mColorG;
  protected float mColorB;
  protected float mColorAMin;
  protected float mColorAMax;
  protected float mColorADistribution;
  protected bool mColorize;
  protected float mHueMin;
  protected float mHueMax;
  protected float mHueDistribution;
  protected float mSatMin;
  protected float mSatMax;
  protected float mSatDistribution;
  protected float mValueMin;
  protected float mValueMax;
  protected float mValueDistribution;
  protected float mAlphaMin;
  protected float mAlphaMax;
  protected float mAlphaDistribution;
  protected float mLifeTimeMin;
  protected float mLifeTimeMax;
  protected float mLifeTimeDistribution;
  protected float mSizeStartMin;
  protected float mSizeStartMax;
  protected float mSizeStartDistribution;
  protected float mSizeEndMin;
  protected float mSizeEndMax;
  protected float mSizeEndDistribution;
  protected float mVelocityMin;
  protected float mVelocityMax;
  protected float mVelocityDistribution;
  protected bool mRotationAligned;
  protected float mRotationMin;
  protected float mRotationMax;
  protected float mRotationSpeedMin;
  protected float mRotationSpeedMax;
  protected float mRotationPCCW;
  protected bool mConeSpread;
  protected float mSpreadConeAngle;
  protected float mSpreadConeDistribution;
  protected float mSpreadArcHorizontalAngle;
  protected float mSpreadArcHorizontalDistribution;
  protected float mSpreadArcVerticalMin;
  protected float mSpreadArcVerticalMax;
  protected float mSpreadArcVerticalDistribution;
  protected float mTriggerTime;
  protected bool mRelativeVelocity;
  protected bool mSpawnLights;
  protected bool mLightSizeRelative;
  protected float mLightRadius;
  protected float mLightDiffuseR;
  protected float mLightDiffuseG;
  protected float mLightDiffuseB;
  protected float mLightAmbientR;
  protected float mLightAmbientG;
  protected float mLightAmbientB;
  protected float mLightSpecular;
  private ushort mRandomOffset;

  protected PulseEmitter() => this.mRandomOffset = (ushort) DateTime.Now.Ticks;

  public PulseEmitter(XmlNode iNode, int iKeyFramesPerSecond, int iVersion)
    : this()
  {
    if (iVersion >= 2)
      this.ReadVersion2(iNode);
    else
      this.ReadVersion1(iNode);
  }

  private void ReadVersion1(XmlNode iNode)
  {
    Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
    FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
    for (int index = 0; index < fields.Length; ++index)
    {
      if (fields[index].FieldType == typeof (float))
        dictionary.Add(fields[index].Name.Substring(1), fields[index]);
    }
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = iNode.ChildNodes[i1];
      FieldInfo fieldInfo;
      if (dictionary.TryGetValue(childNode.Name, out fieldInfo))
      {
        float num = 0.0f;
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode.Attributes[i2];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
            num = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        fieldInfo.SetValue((object) this, (object) num);
      }
      else
      {
        for (int i3 = 0; i3 < childNode.Attributes.Count; ++i3)
        {
          XmlAttribute attribute = childNode.Attributes[i3];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
          {
            if (childNode.Name.Equals("BlendMode", StringComparison.OrdinalIgnoreCase))
              this.mAlphaBlended = attribute.Value.Equals("Alpha", StringComparison.OrdinalIgnoreCase);
            else if (childNode.Name.Equals("SpreadType", StringComparison.OrdinalIgnoreCase))
              this.mConeSpread = attribute.Value.Equals("Cone", StringComparison.OrdinalIgnoreCase);
            else if (childNode.Name.Equals("RotationAligned", StringComparison.OrdinalIgnoreCase))
              this.mRotationAligned = bool.Parse(attribute.Value);
            else if (!childNode.Name.Equals("RelativeVelocity", StringComparison.OrdinalIgnoreCase))
            {
              if (childNode.Name.Equals("ColorControlAlpha", StringComparison.OrdinalIgnoreCase))
                this.mHSV = !bool.Parse(attribute.Value);
              else if (childNode.Name.Equals("HSV", StringComparison.OrdinalIgnoreCase))
                this.mHSV = bool.Parse(attribute.Value);
              else if (childNode.Name.Equals("Colorize", StringComparison.OrdinalIgnoreCase))
                this.mColorize = bool.Parse(attribute.Value);
              else if (childNode.Name.Equals("Particle", StringComparison.OrdinalIgnoreCase))
                this.mParticle = byte.Parse(attribute.Value);
              else if (childNode.Name.Equals("NrOfParticles", StringComparison.OrdinalIgnoreCase))
                this.mNrOfParticles = int.Parse(attribute.Value);
            }
          }
        }
      }
    }
    this.mColorAMin = this.mAlphaMin;
    this.mColorAMax = this.mAlphaMax;
    this.mColorADistribution = this.mAlphaDistribution;
    this.mColorR = 1f;
    this.mColorG = 1f;
    this.mColorB = 1f;
    this.mValueMin = 1f;
    this.mValueMax = 1f;
    this.mValueDistribution = 1f;
    this.mAlphaMin = 1f;
    this.mAlphaMax = 1f;
    this.mAlphaDistribution = 1f;
    this.mRotationY = MathHelper.ToRadians(this.mRotationY);
    this.mHueMin = MathHelper.ToRadians(this.mHueMin);
    this.mHueMax = MathHelper.ToRadians(this.mHueMax);
    this.mRotationMin = MathHelper.ToRadians(this.mRotationMin);
    this.mRotationMax = MathHelper.ToRadians(this.mRotationMax);
    this.mRotationSpeedMin = MathHelper.ToRadians(this.mRotationSpeedMin);
    this.mRotationSpeedMax = MathHelper.ToRadians(this.mRotationSpeedMax);
    this.mSpreadConeAngle = MathHelper.ToRadians(this.mSpreadConeAngle);
    this.mSpreadArcHorizontalAngle = MathHelper.ToRadians(this.mSpreadArcHorizontalAngle);
    this.mSpreadArcVerticalMin = MathHelper.ToRadians(this.mSpreadArcVerticalMin);
    this.mSpreadArcVerticalMax = MathHelper.ToRadians(this.mSpreadArcVerticalMax);
  }

  private void ReadVersion2(XmlNode iNode)
  {
    Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
    FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
    for (int index = 0; index < fields.Length; ++index)
    {
      if (fields[index].FieldType == typeof (float))
        dictionary.Add(fields[index].Name.Substring(1), fields[index]);
    }
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode = iNode.ChildNodes[i1];
      FieldInfo fieldInfo;
      if (dictionary.TryGetValue(childNode.Name, out fieldInfo))
      {
        float num = 0.0f;
        for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
        {
          XmlAttribute attribute = childNode.Attributes[i2];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
            num = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
        }
        fieldInfo.SetValue((object) this, (object) num);
      }
      else
      {
        for (int i3 = 0; i3 < childNode.Attributes.Count; ++i3)
        {
          XmlAttribute attribute = childNode.Attributes[i3];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
          {
            if (childNode.Name.Equals("BlendMode", StringComparison.OrdinalIgnoreCase))
              this.mAlphaBlended = attribute.Value.Equals("Alpha", StringComparison.OrdinalIgnoreCase);
            else if (childNode.Name.Equals("SpreadType", StringComparison.OrdinalIgnoreCase))
              this.mConeSpread = attribute.Value.Equals("Cone", StringComparison.OrdinalIgnoreCase);
            else if (childNode.Name.Equals("RotationAligned", StringComparison.OrdinalIgnoreCase))
              this.mRotationAligned = bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("ColorControlAlpha", StringComparison.OrdinalIgnoreCase))
              this.mHSV = !bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("HSV", StringComparison.OrdinalIgnoreCase))
              this.mHSV = bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("Colorize", StringComparison.OrdinalIgnoreCase))
              this.mColorize = bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("Particle", StringComparison.OrdinalIgnoreCase))
              this.mParticle = byte.Parse(attribute.Value);
            else if (childNode.Name.Equals("NrOfParticles", StringComparison.OrdinalIgnoreCase))
              this.mNrOfParticles = int.Parse(attribute.Value);
            else if (childNode.Name.Equals("RelativeVelocity", StringComparison.OrdinalIgnoreCase))
              this.mRelativeVelocity = bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("SpawnLights", StringComparison.OrdinalIgnoreCase))
              this.mSpawnLights = bool.Parse(attribute.Value);
            else if (childNode.Name.Equals("LightSizeRelative", StringComparison.OrdinalIgnoreCase))
              this.mLightSizeRelative = bool.Parse(attribute.Value);
          }
        }
      }
    }
    this.mRotationY = MathHelper.ToRadians(this.mRotationY);
    this.mHueMin = MathHelper.ToRadians(this.mHueMin);
    this.mHueMax = MathHelper.ToRadians(this.mHueMax);
    this.mRotationMin = MathHelper.ToRadians(this.mRotationMin);
    this.mRotationMax = MathHelper.ToRadians(this.mRotationMax);
    this.mRotationSpeedMin = MathHelper.ToRadians(this.mRotationSpeedMin);
    this.mRotationSpeedMax = MathHelper.ToRadians(this.mRotationSpeedMax);
    this.mSpreadConeAngle = MathHelper.ToRadians(this.mSpreadConeAngle);
    this.mSpreadArcHorizontalAngle = MathHelper.ToRadians(this.mSpreadArcHorizontalAngle);
    this.mSpreadArcVerticalMin = MathHelper.ToRadians(this.mSpreadArcVerticalMin);
    this.mSpreadArcVerticalMax = MathHelper.ToRadians(this.mSpreadArcVerticalMax);
  }

  public void Update(
    float iDeltaTime,
    float iPreviousTotalTime,
    float iTotalTime,
    float iTimeLinePosition,
    float iPreviousTimeLinePosition,
    ref Matrix iTransform,
    ref Matrix iPreviousTransform)
  {
    float mTriggerTime = this.mTriggerTime;
    if ((double) iTimeLinePosition <= (double) mTriggerTime || !((double) iPreviousTimeLinePosition <= (double) mTriggerTime | (double) iPreviousTimeLinePosition > (double) iTimeLinePosition))
      return;
    float num1 = 1f / iDeltaTime;
    Vector3 vector3 = new Vector3();
    vector3.X = (iTransform.M41 - iPreviousTransform.M41) * num1;
    vector3.Y = (iTransform.M42 - iPreviousTransform.M42) * num1;
    vector3.Z = (iTransform.M43 - iPreviousTransform.M43) * num1;
    Matrix result;
    Matrix.CreateRotationY(this.mRotationY, out result);
    result.M41 = this.mPositionX;
    result.M42 = this.mPositionY;
    result.M43 = this.mPositionZ;
    Matrix.Multiply(ref result, ref iTransform, out result);
    int mRotationPccw = (int) this.mRotationPCCW;
    Particle iParticle = new Particle();
    iParticle.HSV = this.mHSV;
    if (!this.mHSV)
    {
      iParticle.Color.X = this.mColorR;
      iParticle.Color.Y = this.mColorG;
      iParticle.Color.Z = this.mColorB;
    }
    iParticle.Sprite = this.mParticle;
    iParticle.Drag = this.mDrag;
    iParticle.Gravity = this.mGravity;
    Vector3 vector1 = new Vector3();
    vector1.X = -(float) Math.Sin((double) this.mRotationY);
    vector1.Z = -(float) Math.Cos((double) this.mRotationY);
    Vector3 up = Vector3.Up;
    Vector3.Cross(ref vector1, ref up, out Vector3 _);
    Vector3 oDirection = new Vector3();
    ParticleSystem instance = ParticleSystem.Instance;
    ParticleLightProperties iLight = new ParticleLightProperties();
    if (this.mSpawnLights)
    {
      iLight.DiffuseColor.X = this.mLightDiffuseR;
      iLight.DiffuseColor.Y = this.mLightDiffuseG;
      iLight.DiffuseColor.Z = this.mLightDiffuseB;
      iLight.AmbientColor.X = this.mLightAmbientR;
      iLight.AmbientColor.Y = this.mLightAmbientG;
      iLight.AmbientColor.Z = this.mLightAmbientB;
      iLight.SpecularAmount = this.mLightSpecular;
      iLight.RadiusStart = this.mLightRadius;
      iLight.RadiusEnd = this.mLightRadius;
    }
    iParticle.RotationAligned = this.mRotationAligned;
    if (this.mRotationAligned)
    {
      iParticle.Rotation = 0.0f;
      iParticle.RotationVelocity = 0.0f;
    }
    if (!this.mRelativeVelocity)
    {
      vector3.X = 0.0f;
      vector3.Y = 0.0f;
      vector3.Z = 0.0f;
    }
    int num2 = (int) ((double) this.mNrOfParticles * (double) ParticleSystem.GetSpawnMultiplyer((float) this.mNrOfParticles));
    for (int index = 0; index < num2; ++index)
    {
      Vector3 position;
      position.X = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * this.mPositionXOffset;
      position.Y = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * this.mPositionYOffset;
      position.Z = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * this.mPositionZOffset;
      Vector3.Transform(ref position, ref result, out iParticle.Position);
      iParticle.StartSize = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mSizeStartMin, this.mSizeStartMax, this.mSizeStartDistribution);
      iParticle.EndSize = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mSizeEndMin, this.mSizeEndMax, this.mSizeEndDistribution);
      if (this.mConeSpread)
        ParticleHelper.GetRandomDirectionCone(ref this.mRandomOffset, ref result, this.mSpreadConeAngle, this.mSpreadConeDistribution, out oDirection);
      else
        ParticleHelper.GetRandomDirectionArc(ref this.mRandomOffset, ref result, this.mSpreadArcHorizontalAngle, this.mSpreadArcHorizontalDistribution, this.mSpreadArcVerticalMin, this.mSpreadArcVerticalMax, this.mSpreadArcVerticalDistribution, out oDirection);
      float randomValue = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mVelocityMin, this.mVelocityMax, this.mVelocityDistribution);
      iParticle.Velocity.X = oDirection.X * randomValue + vector3.X;
      iParticle.Velocity.Y = oDirection.Y * randomValue + vector3.Y;
      iParticle.Velocity.Z = oDirection.Z * randomValue + vector3.Z;
      iParticle.TTL = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mLifeTimeMin, this.mLifeTimeMax, this.mLifeTimeDistribution);
      iParticle.AlphaBlended = this.mAlphaBlended;
      if (!this.mRotationAligned)
      {
        iParticle.Rotation = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mRotationMin, this.mRotationMax);
        float num3 = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mRotationSpeedMin, this.mRotationSpeedMax) * (float) ParticleHelper.GetRandomSign(ref this.mRandomOffset, mRotationPccw);
        iParticle.RotationVelocity = num3;
      }
      if (this.mHSV)
      {
        iParticle.Colorize = this.mColorize;
        iParticle.Color.X = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mHueMin, this.mHueMax, this.mHueDistribution);
        iParticle.Color.Y = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mSatMin, this.mSatMax, this.mSatDistribution);
        iParticle.Color.Z = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mValueMin, this.mValueMax, this.mValueDistribution);
        iParticle.Color.W = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mAlphaMin, this.mAlphaMax, this.mAlphaDistribution);
      }
      else
        iParticle.Color.W = ParticleHelper.GetRandomValue(ref this.mRandomOffset, this.mAlphaMin, this.mAlphaMax, this.mAlphaDistribution);
      instance.SpawnParticle(ref iParticle);
      if (this.mSpawnLights)
      {
        if (this.mLightSizeRelative)
        {
          iLight.RadiusStart = iParticle.StartSize * this.mLightRadius;
          iLight.RadiusEnd = iParticle.EndSize * this.mLightRadius;
        }
        ParticleHelper.SpawnLight(ref iParticle, ref iLight);
      }
    }
  }
}
