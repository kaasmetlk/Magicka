// Decompiled with JetBrains decompiler
// Type: PolygonHead.ParticleEffects.ContinuousEmitter
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

public class ContinuousEmitter : IParticleEmitter
{
  protected KeyedValue[] mParticlesPerSecond;
  protected KeyedValue[] mPositionX;
  protected KeyedValue[] mPositionY;
  protected KeyedValue[] mPositionZ;
  protected KeyedValue[] mPositionXOffset;
  protected KeyedValue[] mPositionYOffset;
  protected KeyedValue[] mPositionZOffset;
  protected KeyedValue[] mRotationY;
  protected byte mParticle;
  protected KeyedValue[] mGravity;
  protected KeyedValue[] mDrag;
  protected bool mAlphaBlended;
  protected bool mHSV;
  protected KeyedValue[] mColorR;
  protected KeyedValue[] mColorG;
  protected KeyedValue[] mColorB;
  protected KeyedValue[] mColorAMin;
  protected KeyedValue[] mColorAMax;
  protected KeyedValue[] mColorADistribution;
  protected bool mColorize;
  protected KeyedValue[] mHueMin;
  protected KeyedValue[] mHueMax;
  protected KeyedValue[] mHueDistribution;
  protected KeyedValue[] mSatMin;
  protected KeyedValue[] mSatMax;
  protected KeyedValue[] mSatDistribution;
  protected KeyedValue[] mValueMin;
  protected KeyedValue[] mValueMax;
  protected KeyedValue[] mValueDistribution;
  protected KeyedValue[] mAlphaMin;
  protected KeyedValue[] mAlphaMax;
  protected KeyedValue[] mAlphaDistribution;
  protected KeyedValue[] mLifeTimeMin;
  protected KeyedValue[] mLifeTimeMax;
  protected KeyedValue[] mLifeTimeDistribution;
  protected KeyedValue[] mSizeStartMin;
  protected KeyedValue[] mSizeStartMax;
  protected KeyedValue[] mSizeStartDistribution;
  protected KeyedValue[] mSizeEndMin;
  protected KeyedValue[] mSizeEndMax;
  protected KeyedValue[] mSizeEndDistribution;
  protected KeyedValue[] mVelocityMin;
  protected KeyedValue[] mVelocityMax;
  protected KeyedValue[] mVelocityDistribution;
  protected bool mRotationAligned;
  protected KeyedValue[] mRotationMin;
  protected KeyedValue[] mRotationMax;
  protected KeyedValue[] mRotationSpeedMin;
  protected KeyedValue[] mRotationSpeedMax;
  protected KeyedValue[] mRotationPCCW;
  protected bool mConeSpread;
  protected KeyedValue[] mSpreadConeAngle;
  protected KeyedValue[] mSpreadConeDistribution;
  protected KeyedValue[] mSpreadArcHorizontalAngle;
  protected KeyedValue[] mSpreadArcHorizontalDistribution;
  protected KeyedValue[] mSpreadArcVerticalMin;
  protected KeyedValue[] mSpreadArcVerticalMax;
  protected KeyedValue[] mSpreadArcVerticalDistribution;
  protected bool mRelativeVelocity;
  protected bool mSpawnLights;
  protected bool mLightSizeRelative;
  protected KeyedValue[] mLightRadius;
  protected KeyedValue[] mLightDiffuseR;
  protected KeyedValue[] mLightDiffuseG;
  protected KeyedValue[] mLightDiffuseB;
  protected KeyedValue[] mLightAmbientR;
  protected KeyedValue[] mLightAmbientG;
  protected KeyedValue[] mLightAmbientB;
  protected KeyedValue[] mLightSpecular;
  private ushort mRandomOffset;

  protected ContinuousEmitter() => this.mRandomOffset = (ushort) DateTime.Now.Ticks;

  public ContinuousEmitter(XmlNode iNode, int iKeyFramesPerSecond, int iVersion)
    : this()
  {
    if (iVersion >= 2)
      this.ReadVersion2(iNode, iKeyFramesPerSecond);
    else
      this.ReadVersion1(iNode, iKeyFramesPerSecond);
  }

  private void ReadVersion1(XmlNode iNode, int iKeyFramesPerSecond)
  {
    Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
    FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
    for (int index = 0; index < fields.Length; ++index)
    {
      if (fields[index].FieldType == typeof (KeyedValue[]))
        dictionary.Add(fields[index].Name.Substring(1), fields[index]);
    }
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      FieldInfo fieldInfo;
      if (dictionary.TryGetValue(childNode1.Name, out fieldInfo))
      {
        if (childNode1.HasChildNodes)
        {
          KeyedValue[] keyedValueArray = new KeyedValue[childNode1.ChildNodes.Count];
          fieldInfo.SetValue((object) this, (object) keyedValueArray);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            KeyedValue keyedValue = new KeyedValue();
            XmlNode childNode2 = childNode1.ChildNodes[i2];
            for (int i3 = 0; i3 < childNode2.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode2.Attributes[i3];
              if (attribute.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                keyedValue.Time = (float) int.Parse(attribute.Value) / (float) iKeyFramesPerSecond;
              else if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
                keyedValue.Value = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            }
            keyedValueArray[i2] = keyedValue;
          }
        }
        else
        {
          KeyedValue[] keyedValueArray = new KeyedValue[1];
          fieldInfo.SetValue((object) this, (object) keyedValueArray);
          KeyedValue keyedValue = new KeyedValue();
          for (int i4 = 0; i4 < childNode1.Attributes.Count; ++i4)
          {
            XmlAttribute attribute = childNode1.Attributes[i4];
            if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
              keyedValue.Value = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
          keyedValueArray[0] = keyedValue;
        }
      }
      else
      {
        for (int i5 = 0; i5 < childNode1.Attributes.Count; ++i5)
        {
          XmlAttribute attribute = childNode1.Attributes[i5];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
          {
            if (childNode1.Name.Equals("BlendMode", StringComparison.OrdinalIgnoreCase))
              this.mAlphaBlended = attribute.Value.Equals("Alpha", StringComparison.OrdinalIgnoreCase);
            else if (childNode1.Name.Equals("SpreadType", StringComparison.OrdinalIgnoreCase))
              this.mConeSpread = attribute.Value.Equals("Cone", StringComparison.OrdinalIgnoreCase);
            else if (childNode1.Name.Equals("RotationAligned", StringComparison.OrdinalIgnoreCase))
              this.mRotationAligned = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("ColorControlAlpha", StringComparison.OrdinalIgnoreCase))
              this.mHSV = !bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("HSV", StringComparison.OrdinalIgnoreCase))
              this.mHSV = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("Colorize", StringComparison.OrdinalIgnoreCase))
              this.mColorize = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("RelativeVelocity", StringComparison.OrdinalIgnoreCase))
              this.mRelativeVelocity = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("Particle", StringComparison.OrdinalIgnoreCase))
              this.mParticle = byte.Parse(attribute.Value);
          }
        }
      }
    }
    this.mColorAMin = this.mAlphaMin;
    this.mColorAMax = this.mAlphaMax;
    this.mColorADistribution = this.mAlphaDistribution;
    this.mColorR = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mColorG = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mColorB = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mValueMin = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mValueMax = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mValueDistribution = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mAlphaMin = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mAlphaMax = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    this.mAlphaDistribution = new KeyedValue[1]
    {
      new KeyedValue(0.0f, 1f)
    };
    for (int index = 0; index < this.mRotationY.Length; ++index)
      this.mRotationY[index].Value = MathHelper.ToRadians(this.mRotationY[index].Value);
    for (int index = 0; index < this.mHueMin.Length; ++index)
      this.mHueMin[index].Value = MathHelper.ToRadians(this.mHueMin[index].Value);
    for (int index = 0; index < this.mHueMax.Length; ++index)
      this.mHueMax[index].Value = MathHelper.ToRadians(this.mHueMax[index].Value);
    for (int index = 0; index < this.mRotationMin.Length; ++index)
      this.mRotationMin[index].Value = MathHelper.ToRadians(this.mRotationMin[index].Value);
    for (int index = 0; index < this.mRotationMax.Length; ++index)
      this.mRotationMax[index].Value = MathHelper.ToRadians(this.mRotationMax[index].Value);
    for (int index = 0; index < this.mRotationSpeedMin.Length; ++index)
      this.mRotationSpeedMin[index].Value = MathHelper.ToRadians(this.mRotationSpeedMin[index].Value);
    for (int index = 0; index < this.mRotationSpeedMax.Length; ++index)
      this.mRotationSpeedMax[index].Value = MathHelper.ToRadians(this.mRotationSpeedMax[index].Value);
    for (int index = 0; index < this.mSpreadConeAngle.Length; ++index)
      this.mSpreadConeAngle[index].Value = MathHelper.ToRadians(this.mSpreadConeAngle[index].Value);
    for (int index = 0; index < this.mSpreadArcHorizontalAngle.Length; ++index)
      this.mSpreadArcHorizontalAngle[index].Value = MathHelper.ToRadians(this.mSpreadArcHorizontalAngle[index].Value);
    for (int index = 0; index < this.mSpreadArcVerticalMin.Length; ++index)
      this.mSpreadArcVerticalMin[index].Value = MathHelper.ToRadians(this.mSpreadArcVerticalMin[index].Value);
    for (int index = 0; index < this.mSpreadArcVerticalMax.Length; ++index)
      this.mSpreadArcVerticalMax[index].Value = MathHelper.ToRadians(this.mSpreadArcVerticalMax[index].Value);
  }

  private void ReadVersion2(XmlNode iNode, int iKeyFramesPerSecond)
  {
    Dictionary<string, FieldInfo> dictionary = new Dictionary<string, FieldInfo>();
    FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
    for (int index = 0; index < fields.Length; ++index)
    {
      if (fields[index].FieldType == typeof (KeyedValue[]))
        dictionary.Add(fields[index].Name.Substring(1), fields[index]);
    }
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      FieldInfo fieldInfo;
      if (dictionary.TryGetValue(childNode1.Name, out fieldInfo))
      {
        if (childNode1.HasChildNodes)
        {
          KeyedValue[] keyedValueArray = new KeyedValue[childNode1.ChildNodes.Count];
          fieldInfo.SetValue((object) this, (object) keyedValueArray);
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            KeyedValue keyedValue = new KeyedValue();
            XmlNode childNode2 = childNode1.ChildNodes[i2];
            for (int i3 = 0; i3 < childNode2.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode2.Attributes[i3];
              if (attribute.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                keyedValue.Time = (float) int.Parse(attribute.Value) / (float) iKeyFramesPerSecond;
              else if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
                keyedValue.Value = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
            }
            keyedValueArray[i2] = keyedValue;
          }
        }
        else
        {
          KeyedValue[] keyedValueArray = new KeyedValue[1];
          fieldInfo.SetValue((object) this, (object) keyedValueArray);
          KeyedValue keyedValue = new KeyedValue();
          for (int i4 = 0; i4 < childNode1.Attributes.Count; ++i4)
          {
            XmlAttribute attribute = childNode1.Attributes[i4];
            if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
              keyedValue.Value = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
          }
          keyedValueArray[0] = keyedValue;
        }
      }
      else
      {
        for (int i5 = 0; i5 < childNode1.Attributes.Count; ++i5)
        {
          XmlAttribute attribute = childNode1.Attributes[i5];
          if (attribute.Name.Equals("value", StringComparison.OrdinalIgnoreCase))
          {
            if (childNode1.Name.Equals("BlendMode", StringComparison.OrdinalIgnoreCase))
              this.mAlphaBlended = attribute.Value.Equals("Alpha", StringComparison.OrdinalIgnoreCase);
            else if (childNode1.Name.Equals("SpreadType", StringComparison.OrdinalIgnoreCase))
              this.mConeSpread = attribute.Value.Equals("Cone", StringComparison.OrdinalIgnoreCase);
            else if (childNode1.Name.Equals("RotationAligned", StringComparison.OrdinalIgnoreCase))
              this.mRotationAligned = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("ColorControlAlpha", StringComparison.OrdinalIgnoreCase))
              this.mHSV = !bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("HSV", StringComparison.OrdinalIgnoreCase))
              this.mHSV = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("Colorize", StringComparison.OrdinalIgnoreCase))
              this.mColorize = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("RelativeVelocity", StringComparison.OrdinalIgnoreCase))
              this.mRelativeVelocity = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("Particle", StringComparison.OrdinalIgnoreCase))
              this.mParticle = byte.Parse(attribute.Value);
            else if (childNode1.Name.Equals("SpawnLights", StringComparison.OrdinalIgnoreCase))
              this.mSpawnLights = bool.Parse(attribute.Value);
            else if (childNode1.Name.Equals("LightSizeRelative", StringComparison.OrdinalIgnoreCase))
              this.mLightSizeRelative = bool.Parse(attribute.Value);
          }
        }
      }
    }
    for (int index = 0; index < this.mRotationY.Length; ++index)
      this.mRotationY[index].Value = MathHelper.ToRadians(this.mRotationY[index].Value);
    for (int index = 0; index < this.mHueMin.Length; ++index)
      this.mHueMin[index].Value = MathHelper.ToRadians(this.mHueMin[index].Value);
    for (int index = 0; index < this.mHueMax.Length; ++index)
      this.mHueMax[index].Value = MathHelper.ToRadians(this.mHueMax[index].Value);
    for (int index = 0; index < this.mRotationMin.Length; ++index)
      this.mRotationMin[index].Value = MathHelper.ToRadians(this.mRotationMin[index].Value);
    for (int index = 0; index < this.mRotationMax.Length; ++index)
      this.mRotationMax[index].Value = MathHelper.ToRadians(this.mRotationMax[index].Value);
    for (int index = 0; index < this.mRotationSpeedMin.Length; ++index)
      this.mRotationSpeedMin[index].Value = MathHelper.ToRadians(this.mRotationSpeedMin[index].Value);
    for (int index = 0; index < this.mRotationSpeedMax.Length; ++index)
      this.mRotationSpeedMax[index].Value = MathHelper.ToRadians(this.mRotationSpeedMax[index].Value);
    for (int index = 0; index < this.mSpreadConeAngle.Length; ++index)
      this.mSpreadConeAngle[index].Value = MathHelper.ToRadians(this.mSpreadConeAngle[index].Value);
    for (int index = 0; index < this.mSpreadArcHorizontalAngle.Length; ++index)
      this.mSpreadArcHorizontalAngle[index].Value = MathHelper.ToRadians(this.mSpreadArcHorizontalAngle[index].Value);
    for (int index = 0; index < this.mSpreadArcVerticalMin.Length; ++index)
      this.mSpreadArcVerticalMin[index].Value = MathHelper.ToRadians(this.mSpreadArcVerticalMin[index].Value);
    for (int index = 0; index < this.mSpreadArcVerticalMax.Length; ++index)
      this.mSpreadArcVerticalMax[index].Value = MathHelper.ToRadians(this.mSpreadArcVerticalMax[index].Value);
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
    float num1 = 1f / iDeltaTime;
    Vector3 vector3_1 = new Vector3();
    vector3_1.X = (iTransform.M41 - iPreviousTransform.M41) * num1;
    vector3_1.Y = (iTransform.M42 - iPreviousTransform.M42) * num1;
    vector3_1.Z = (iTransform.M43 - iPreviousTransform.M43) * num1;
    float iParticles = (float) (((double) ParticleHelper.GetKeyedValue(this.mParticlesPerSecond, iTimeLinePosition) + (double) ParticleHelper.GetKeyedValue(this.mParticlesPerSecond, iTimeLinePosition + iDeltaTime)) * 0.5);
    float num2 = iParticles * ParticleSystem.GetSpawnMultiplyer(iParticles);
    float num3 = iPreviousTotalTime * num2;
    int num4 = (int) (iTotalTime * num2) - (int) num3;
    if (num4 <= 0)
      return;
    Matrix result1;
    Matrix.CreateRotationY(ParticleHelper.GetKeyedValue(this.mRotationY, iPreviousTimeLinePosition), out result1);
    result1.M41 = ParticleHelper.GetKeyedValue(this.mPositionX, iPreviousTimeLinePosition);
    result1.M42 = ParticleHelper.GetKeyedValue(this.mPositionY, iPreviousTimeLinePosition);
    result1.M43 = ParticleHelper.GetKeyedValue(this.mPositionZ, iPreviousTimeLinePosition);
    Vector3 vector3_2 = new Vector3();
    vector3_2.X = ParticleHelper.GetKeyedValue(this.mPositionXOffset, iPreviousTimeLinePosition);
    vector3_2.Y = ParticleHelper.GetKeyedValue(this.mPositionYOffset, iPreviousTimeLinePosition);
    vector3_2.Z = ParticleHelper.GetKeyedValue(this.mPositionZOffset, iPreviousTimeLinePosition);
    Matrix.Multiply(ref result1, ref iPreviousTransform, out result1);
    Matrix result2;
    Matrix.CreateRotationY(ParticleHelper.GetKeyedValue(this.mRotationY, iTimeLinePosition), out result2);
    result2.M41 = ParticleHelper.GetKeyedValue(this.mPositionX, iTimeLinePosition);
    result2.M42 = ParticleHelper.GetKeyedValue(this.mPositionY, iTimeLinePosition);
    result2.M43 = ParticleHelper.GetKeyedValue(this.mPositionZ, iTimeLinePosition);
    Vector3 vector3_3 = new Vector3();
    vector3_3.X = ParticleHelper.GetKeyedValue(this.mPositionXOffset, iTimeLinePosition);
    vector3_3.Y = ParticleHelper.GetKeyedValue(this.mPositionYOffset, iTimeLinePosition);
    vector3_3.Z = ParticleHelper.GetKeyedValue(this.mPositionZOffset, iTimeLinePosition);
    Matrix.Multiply(ref result2, ref iTransform, out result2);
    float iAlphaMin = 0.0f;
    float iAlphaMax = 0.0f;
    float iAlphaDist = 0.0f;
    ContinuousEmitter.HSVParameters iParameters1 = new ContinuousEmitter.HSVParameters();
    if (this.mHSV)
    {
      iParameters1.HueMin = ParticleHelper.GetKeyedValue(this.mHueMin, iTimeLinePosition);
      iParameters1.HueMax = ParticleHelper.GetKeyedValue(this.mHueMax, iTimeLinePosition);
      iParameters1.HueDist = ParticleHelper.GetKeyedValue(this.mHueDistribution, iTimeLinePosition);
      iParameters1.SaturationMin = ParticleHelper.GetKeyedValue(this.mSatMin, iTimeLinePosition);
      iParameters1.SaturationMax = ParticleHelper.GetKeyedValue(this.mSatMax, iTimeLinePosition);
      iParameters1.SaturationDist = ParticleHelper.GetKeyedValue(this.mSatDistribution, iTimeLinePosition);
      iParameters1.ValueMin = ParticleHelper.GetKeyedValue(this.mValueMin, iTimeLinePosition);
      iParameters1.ValueMax = ParticleHelper.GetKeyedValue(this.mValueMax, iTimeLinePosition);
      iParameters1.ValueDist = ParticleHelper.GetKeyedValue(this.mValueDistribution, iTimeLinePosition);
      iParameters1.AlphaMin = ParticleHelper.GetKeyedValue(this.mAlphaMin, iTimeLinePosition);
      iParameters1.AlphaMax = ParticleHelper.GetKeyedValue(this.mAlphaMax, iTimeLinePosition);
      iParameters1.AlphaDist = ParticleHelper.GetKeyedValue(this.mAlphaDistribution, iTimeLinePosition);
    }
    else
    {
      iAlphaMin = ParticleHelper.GetKeyedValue(this.mColorAMin, iTimeLinePosition);
      iAlphaMax = ParticleHelper.GetKeyedValue(this.mColorAMax, iTimeLinePosition);
      iAlphaDist = ParticleHelper.GetKeyedValue(this.mColorADistribution, iTimeLinePosition);
    }
    float iAngle = 0.0f;
    float iDistribution = 0.0f;
    float iHorizontalAngle = 0.0f;
    float iHorizontalDistribution = 0.0f;
    float iVerticalMin = 0.0f;
    float iVerticalMax = 0.0f;
    float iVerticalDistribution = 0.0f;
    float keyedValue1 = ParticleHelper.GetKeyedValue(this.mRotationMin, iTimeLinePosition);
    float keyedValue2 = ParticleHelper.GetKeyedValue(this.mRotationMax, iTimeLinePosition);
    float keyedValue3 = ParticleHelper.GetKeyedValue(this.mRotationSpeedMin, iTimeLinePosition);
    float keyedValue4 = ParticleHelper.GetKeyedValue(this.mRotationSpeedMax, iTimeLinePosition);
    int keyedValue5 = (int) ParticleHelper.GetKeyedValue(this.mRotationPCCW, iTimeLinePosition);
    if (this.mConeSpread)
    {
      iAngle = ParticleHelper.GetKeyedValue(this.mSpreadConeAngle, iTimeLinePosition);
      iDistribution = ParticleHelper.GetKeyedValue(this.mSpreadConeDistribution, iTimeLinePosition);
    }
    else
    {
      iHorizontalAngle = ParticleHelper.GetKeyedValue(this.mSpreadArcHorizontalAngle, iTimeLinePosition);
      iHorizontalDistribution = ParticleHelper.GetKeyedValue(this.mSpreadArcHorizontalDistribution, iTimeLinePosition);
      iVerticalMin = ParticleHelper.GetKeyedValue(this.mSpreadArcVerticalMin, iTimeLinePosition);
      iVerticalMax = ParticleHelper.GetKeyedValue(this.mSpreadArcVerticalMax, iTimeLinePosition);
      iVerticalDistribution = ParticleHelper.GetKeyedValue(this.mSpreadArcVerticalDistribution, iTimeLinePosition);
    }
    ContinuousEmitter.ParticleParameters iParameters2 = new ContinuousEmitter.ParticleParameters();
    iParameters2.SizeStartMin = ParticleHelper.GetKeyedValue(this.mSizeStartMin, iTimeLinePosition);
    iParameters2.SizeStartMax = ParticleHelper.GetKeyedValue(this.mSizeStartMax, iTimeLinePosition);
    iParameters2.SizeStartDist = ParticleHelper.GetKeyedValue(this.mSizeStartDistribution, iTimeLinePosition);
    iParameters2.SizeEndMin = ParticleHelper.GetKeyedValue(this.mSizeEndMin, iTimeLinePosition);
    iParameters2.SizeEndMax = ParticleHelper.GetKeyedValue(this.mSizeEndMax, iTimeLinePosition);
    iParameters2.SizeEndDist = ParticleHelper.GetKeyedValue(this.mSizeEndDistribution, iTimeLinePosition);
    iParameters2.LifeTimeMin = ParticleHelper.GetKeyedValue(this.mLifeTimeMin, iTimeLinePosition);
    iParameters2.LifeTimeMax = ParticleHelper.GetKeyedValue(this.mLifeTimeMax, iTimeLinePosition);
    iParameters2.LifeTimeDist = ParticleHelper.GetKeyedValue(this.mLifeTimeDistribution, iTimeLinePosition);
    iParameters2.VelocityMin = ParticleHelper.GetKeyedValue(this.mVelocityMin, iTimeLinePosition);
    iParameters2.VelocityMax = ParticleHelper.GetKeyedValue(this.mVelocityMax, iTimeLinePosition);
    iParameters2.VelocityDist = ParticleHelper.GetKeyedValue(this.mVelocityDistribution, iTimeLinePosition);
    Particle iParticle = new Particle();
    iParticle.Sprite = this.mParticle;
    iParticle.AlphaBlended = this.mAlphaBlended;
    iParticle.Drag = ParticleHelper.GetKeyedValue(this.mDrag, iTimeLinePosition);
    iParticle.Gravity = ParticleHelper.GetKeyedValue(this.mGravity, iTimeLinePosition);
    iParticle.RotationAligned = this.mRotationAligned;
    if (this.mRotationAligned)
    {
      iParticle.Rotation = 0.0f;
      iParticle.RotationVelocity = 0.0f;
    }
    iParticle.HSV = this.mHSV;
    if (!this.mHSV)
    {
      iParticle.Color.X = ParticleHelper.GetKeyedValue(this.mColorR, iTimeLinePosition);
      iParticle.Color.Y = ParticleHelper.GetKeyedValue(this.mColorG, iTimeLinePosition);
      iParticle.Color.Z = ParticleHelper.GetKeyedValue(this.mColorB, iTimeLinePosition);
    }
    iParticle.Colorize = this.mColorize;
    if (!this.mRelativeVelocity)
    {
      vector3_1.X = 0.0f;
      vector3_1.Y = 0.0f;
      vector3_1.Z = 0.0f;
    }
    ParticleSystem instance = ParticleSystem.Instance;
    ParticleLightProperties iLight = new ParticleLightProperties();
    float keyedValue6 = ParticleHelper.GetKeyedValue(this.mLightRadius, iTimeLinePosition);
    if (this.mSpawnLights)
    {
      iLight.DiffuseColor.X = ParticleHelper.GetKeyedValue(this.mLightDiffuseR, iTimeLinePosition);
      iLight.DiffuseColor.Y = ParticleHelper.GetKeyedValue(this.mLightDiffuseG, iTimeLinePosition);
      iLight.DiffuseColor.Z = ParticleHelper.GetKeyedValue(this.mLightDiffuseB, iTimeLinePosition);
      iLight.AmbientColor.X = ParticleHelper.GetKeyedValue(this.mLightAmbientR, iTimeLinePosition);
      iLight.AmbientColor.Y = ParticleHelper.GetKeyedValue(this.mLightAmbientG, iTimeLinePosition);
      iLight.AmbientColor.Z = ParticleHelper.GetKeyedValue(this.mLightAmbientB, iTimeLinePosition);
      iLight.SpecularAmount = ParticleHelper.GetKeyedValue(this.mLightSpecular, iTimeLinePosition);
      iLight.RadiusStart = keyedValue6;
      iLight.RadiusEnd = keyedValue6;
    }
    float num5 = 1f / (float) num4;
    float amount = num5;
    int num6 = 0;
    while (num6 < num4)
    {
      Vector3.Lerp(ref vector3_2, ref vector3_3, amount, out iParameters2.Offset);
      Matrix.Lerp(ref result1, ref result2, amount, out iParameters2.Transform);
      if (this.mConeSpread)
        ParticleHelper.GetRandomDirectionCone(ref this.mRandomOffset, ref iParameters2.Transform, iAngle, iDistribution, out iParticle.Velocity);
      else
        ParticleHelper.GetRandomDirectionArc(ref this.mRandomOffset, ref iParameters2.Transform, iHorizontalAngle, iHorizontalDistribution, iVerticalMin, iVerticalMax, iVerticalDistribution, out iParticle.Velocity);
      this.SetCommonParameters(ref iParameters2, ref iParticle);
      iParticle.Velocity.X += vector3_1.X;
      iParticle.Velocity.Y += vector3_1.Y;
      iParticle.Velocity.Z += vector3_1.Z;
      if (!this.mRotationAligned)
        this.SetRotationSpeed(keyedValue1, keyedValue2, keyedValue3, keyedValue4, keyedValue5, ref iParticle);
      if (this.mHSV)
        this.SetHSV(ref iParameters1, ref iParticle);
      else
        this.SetColor(iAlphaMin, iAlphaMax, iAlphaDist, ref iParticle);
      instance.SpawnParticle(ref iParticle);
      if (this.mSpawnLights)
      {
        if (this.mLightSizeRelative)
        {
          iLight.RadiusStart = iParticle.StartSize * keyedValue6;
          iLight.RadiusEnd = iParticle.EndSize * keyedValue6;
        }
        ParticleHelper.SpawnLight(ref iParticle, ref iLight);
      }
      ++num6;
      amount += num5;
    }
  }

  private void SetCommonParameters(
    ref ContinuousEmitter.ParticleParameters iParameters,
    ref Particle iParticle)
  {
    iParticle.Position.X = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * iParameters.Offset.X;
    iParticle.Position.Y = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * iParameters.Offset.Y;
    iParticle.Position.Z = (float) ((double) ParticleHelper.GetRandomValue(ref this.mRandomOffset) * 2.0 - 1.0) * iParameters.Offset.Z;
    Vector3.Transform(ref iParticle.Position, ref iParameters.Transform, out iParticle.Position);
    iParticle.StartSize = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.SizeStartMin, iParameters.SizeStartMax, iParameters.SizeStartDist);
    iParticle.EndSize = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.SizeEndMin, iParameters.SizeEndMax, iParameters.SizeEndDist);
    iParticle.TTL = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.LifeTimeMin, iParameters.LifeTimeMax, iParameters.LifeTimeDist);
    float randomValue = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.VelocityMin, iParameters.VelocityMax, iParameters.VelocityDist);
    iParticle.Velocity.X = iParticle.Velocity.X * randomValue + iParameters.EmitterVelocity.X;
    iParticle.Velocity.Y = iParticle.Velocity.Y * randomValue + iParameters.EmitterVelocity.Y;
    iParticle.Velocity.Z = iParticle.Velocity.Z * randomValue + iParameters.EmitterVelocity.Z;
  }

  private void SetRotationSpeed(
    float iRotationMin,
    float iRotationMax,
    float iRotationSpeedMin,
    float iRotationSpeedMax,
    int iRotationPCCW,
    ref Particle iParticle)
  {
    iParticle.Rotation = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iRotationMin, iRotationMax);
    float num = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iRotationSpeedMin, iRotationSpeedMax) * (float) ParticleHelper.GetRandomSign(ref this.mRandomOffset, iRotationPCCW);
    iParticle.RotationVelocity = num;
  }

  private void SetColor(
    float iAlphaMin,
    float iAlphaMax,
    float iAlphaDist,
    ref Particle iParticle)
  {
    iParticle.Color.W = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iAlphaMin, iAlphaMax, iAlphaDist);
  }

  private void SetHSV(ref ContinuousEmitter.HSVParameters iParameters, ref Particle iParticle)
  {
    iParticle.Color.X = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.HueMin, iParameters.HueMax, iParameters.HueDist);
    iParticle.Color.Y = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.SaturationMin, iParameters.SaturationMax, iParameters.SaturationDist);
    iParticle.Color.Z = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.ValueMin, iParameters.ValueMax, iParameters.ValueDist);
    iParticle.Color.W = ParticleHelper.GetRandomValue(ref this.mRandomOffset, iParameters.AlphaMin, iParameters.AlphaMax, iParameters.AlphaDist);
  }

  private struct ParticleParameters
  {
    public Matrix Transform;
    public Vector3 Offset;
    public Vector3 EmitterVelocity;
    public float SizeStartMin;
    public float SizeStartMax;
    public float SizeStartDist;
    public float SizeEndMin;
    public float SizeEndMax;
    public float SizeEndDist;
    public float LifeTimeMin;
    public float LifeTimeMax;
    public float LifeTimeDist;
    public float VelocityMin;
    public float VelocityMax;
    public float VelocityDist;
  }

  private struct HSVParameters
  {
    public float HueMin;
    public float HueMax;
    public float HueDist;
    public float SaturationMin;
    public float SaturationMax;
    public float SaturationDist;
    public float ValueMin;
    public float ValueMax;
    public float ValueDist;
    public float AlphaMin;
    public float AlphaMax;
    public float AlphaDist;
  }
}
