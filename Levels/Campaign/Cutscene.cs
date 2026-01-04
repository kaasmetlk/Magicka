// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Campaign.Cutscene
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Campaign;

internal class Cutscene
{
  private Cutscene.TimeFloat[] mZoomKeys;
  private Cutscene.TimeVector2[] mPositionKeys;
  private string mDialog;
  private Banks mDialogBank = Banks.WaveBank;
  private int mDialogHash;
  private string mSubTitles;
  private int mSubTitlesHash;
  private float mDuration;

  public Cutscene(XmlNode iNode)
  {
    List<Cutscene.TimeFloat> timeFloatList = new List<Cutscene.TimeFloat>();
    List<Cutscene.TimeVector2> timeVector2List = new List<Cutscene.TimeVector2>();
    for (int i1 = 0; i1 < iNode.ChildNodes.Count; ++i1)
    {
      XmlNode childNode1 = iNode.ChildNodes[i1];
      if (!(childNode1 is XmlComment))
      {
        if (childNode1.Name.Equals("camera", StringComparison.OrdinalIgnoreCase))
        {
          for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
          {
            XmlNode childNode2 = childNode1.ChildNodes[i2];
            if (!(childNode2 is XmlComment))
            {
              if (childNode2.Name.Equals("zoom", StringComparison.OrdinalIgnoreCase))
              {
                Cutscene.TimeFloat timeFloat = new Cutscene.TimeFloat();
                for (int i3 = 0; i3 < childNode2.Attributes.Count; ++i3)
                {
                  XmlAttribute attribute = childNode2.Attributes[i3];
                  if (attribute.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                    timeFloat.Time = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                }
                timeFloat.Value = float.Parse(childNode2.InnerText, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                timeFloatList.Add(timeFloat);
              }
              else if (childNode2.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
              {
                Cutscene.TimeVector2 timeVector2 = new Cutscene.TimeVector2();
                for (int i4 = 0; i4 < childNode2.Attributes.Count; ++i4)
                {
                  XmlAttribute attribute = childNode2.Attributes[i4];
                  if (attribute.Name.Equals("time", StringComparison.OrdinalIgnoreCase))
                    timeVector2.Time = float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                }
                string[] strArray = childNode2.InnerText.Split(',');
                timeVector2.Value.X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                timeVector2.Value.Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
                timeVector2List.Add(timeVector2);
              }
            }
          }
        }
        else if (childNode1.Name.Equals("dialog", StringComparison.OrdinalIgnoreCase))
        {
          for (int i5 = 0; i5 < childNode1.Attributes.Count; ++i5)
          {
            XmlAttribute attribute = childNode1.Attributes[i5];
            if (attribute.Name.Equals("subtitles", StringComparison.OrdinalIgnoreCase))
            {
              this.mSubTitles = attribute.Value.ToLowerInvariant();
              this.mSubTitlesHash = this.mSubTitles.GetHashCodeCustom();
            }
          }
          this.mDialog = childNode1.InnerText.ToLowerInvariant();
          string[] strArray = this.mDialog.Split('/');
          if (strArray.Length == 1)
          {
            this.mDialogBank = Banks.WaveBank;
            this.mDialogHash = this.mDialog.GetHashCodeCustom();
          }
          else
          {
            this.mDialogBank = strArray.Length == 2 ? (Banks) Enum.Parse(typeof (Banks), strArray[0], true) : throw new Exception("Invalid sound description: " + this.mDialog);
            this.mDialogHash = strArray[1].GetHashCodeCustom();
          }
        }
      }
    }
    timeFloatList.Sort();
    this.mZoomKeys = timeFloatList.ToArray();
    timeVector2List.Sort();
    this.mPositionKeys = timeVector2List.ToArray();
    this.mDuration = Math.Max(this.mPositionKeys[this.mPositionKeys.Length - 1].Time, this.mZoomKeys[this.mZoomKeys.Length - 1].Time);
  }

  public int SubTitles => this.mSubTitlesHash;

  public Banks DialogBank => this.mDialogBank;

  public int Dialog => this.mDialogHash;

  public float Duration => this.mDuration;

  public void GetCamera(float iTime, out Vector2 oPosition, out float oZoom)
  {
    Cutscene.TimeFloat.Lerp(this.mZoomKeys, iTime, out oZoom);
    Cutscene.TimeVector2.Lerp(this.mPositionKeys, iTime, out oPosition);
  }

  private struct TimeVector2 : IComparable<Cutscene.TimeVector2>
  {
    public float Time;
    public Vector2 Value;

    public static void Lerp(ref Vector2 iA, ref Vector2 iB, float iAmount, out Vector2 oValue)
    {
      Vector2.SmoothStep(ref iA, ref iB, iAmount, out oValue);
    }

    public static void Lerp(Cutscene.TimeVector2[] iKeys, float iTime, out Vector2 oValue)
    {
      int length = iKeys.Length;
      for (int index = 0; index < length; ++index)
      {
        if ((double) iTime < (double) iKeys[index].Time)
        {
          if (index == 0)
          {
            oValue = iKeys[0].Value;
            return;
          }
          float iAmount = (float) (((double) iTime - (double) iKeys[index - 1].Time) / ((double) iKeys[index].Time - (double) iKeys[index - 1].Time));
          Cutscene.TimeVector2.Lerp(ref iKeys[index - 1].Value, ref iKeys[index].Value, iAmount, out oValue);
          return;
        }
      }
      int index1 = length - 1;
      oValue = iKeys[index1].Value;
    }

    public int CompareTo(Cutscene.TimeVector2 other)
    {
      if ((double) this.Time < (double) other.Time)
        return -1;
      return (double) this.Time > (double) other.Time ? 1 : 0;
    }
  }

  private struct TimeFloat : IComparable<Cutscene.TimeFloat>
  {
    public float Time;
    public float Value;

    public static void Lerp(float iA, float iB, float iAmount, out float oValue)
    {
      oValue = MathHelper.SmoothStep(iA, iB, iAmount);
    }

    public static void Lerp(Cutscene.TimeFloat[] iKeys, float iTime, out float oValue)
    {
      int length = iKeys.Length;
      for (int index = 0; index < length; ++index)
      {
        if ((double) iTime < (double) iKeys[index].Time)
        {
          if (index == 0)
          {
            oValue = iKeys[0].Value;
            return;
          }
          float iAmount = (float) (((double) iTime - (double) iKeys[index - 1].Time) / ((double) iKeys[index].Time - (double) iKeys[index - 1].Time));
          Cutscene.TimeFloat.Lerp(iKeys[index - 1].Value, iKeys[index].Value, iAmount, out oValue);
          return;
        }
      }
      int index1 = length - 1;
      oValue = iKeys[index1].Value;
    }

    public int CompareTo(Cutscene.TimeFloat other)
    {
      if ((double) this.Time < (double) other.Time)
        return -1;
      return (double) this.Time > (double) other.Time ? 1 : 0;
    }
  }
}
