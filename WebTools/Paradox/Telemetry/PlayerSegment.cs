// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.PlayerSegment
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public class PlayerSegment
{
  private const string EXCEPTION_INDEX_OUT_OF_RANGE = "Index out of range. Provided index {0} with a maximum allowed of {1}.";
  private const string EXCEPTION_UNEXPECTED_SECTION_VALUE = "The section at index {0} have an unexpected value {1}. Expecting only -1, 0 or 1.";
  private const string EXCEPTION_NULL_SECTIONS_ARRAY = "Cannot pass a null to a PlayerSegment.";
  private const char NOT_APPLICABLE_CHAR = 'N';
  private const char TRUE_CHAR = 'T';
  private const char FALSE_CHAR = 'F';
  private const string SEGMENT_STRING_ALLOWED_CHAR = "NTF";
  private static readonly Dictionary<PlayerSegment.Section, char> sConvertionDictionary = new Dictionary<PlayerSegment.Section, char>()
  {
    {
      PlayerSegment.Section.NotApplicable,
      'N'
    },
    {
      PlayerSegment.Section.True,
      'T'
    },
    {
      PlayerSegment.Section.False,
      'F'
    }
  };
  private PlayerSegment.Section[] mSections;

  public PlayerSegment.Section this[int iIndex]
  {
    get
    {
      if (this.mSections == null)
        return PlayerSegment.Section.NotApplicable;
      if (iIndex >= this.mSections.Length)
        throw new IndexOutOfRangeException($"Index out of range. Provided index {iIndex} with a maximum allowed of {this.mSections.Length - 1}.");
      return this.mSections[iIndex];
    }
  }

  public int Length
  {
    get => this.mSections == null ? 0 : this.mSections.Length;
    set => Array.Resize<PlayerSegment.Section>(ref this.mSections, value);
  }

  private PlayerSegment()
  {
  }

  public PlayerSegment(int iSize)
  {
    this.mSections = new PlayerSegment.Section[iSize];
    for (int index = 0; index < iSize; ++index)
      this.mSections[index] = PlayerSegment.Section.NotApplicable;
  }

  public PlayerSegment(int iSize, PlayerSegment.Section iDefault)
  {
    this.mSections = new PlayerSegment.Section[iSize];
    for (int index = 0; index < iSize; ++index)
      this.mSections[index] = iDefault;
  }

  public PlayerSegment(params PlayerSegment.Section[] iSections)
  {
    this.mSections = iSections != null ? iSections : throw new NullReferenceException("Cannot pass a null to a PlayerSegment.");
  }

  public PlayerSegment(params int[] iSections)
  {
    this.mSections = iSections != null ? new PlayerSegment.Section[iSections.Length] : throw new NullReferenceException("Cannot pass a null to a PlayerSegment.");
    for (int index = 0; index < iSections.Length; ++index)
    {
      int iSection = iSections[index];
      this.mSections[index] = iSection >= -1 && iSection <= 1 ? (PlayerSegment.Section) iSection : throw new Exception($"The section at index {index} have an unexpected value {iSection}. Expecting only -1, 0 or 1.");
    }
  }

  public string ToSegmentString() => PlayerSegment.ToSegmentString(this.mSections);

  public static string ToSegmentString(params PlayerSegment.Section[] iSections)
  {
    char[] chArray = new char[iSections.Length];
    for (int index = 0; index < iSections.Length; ++index)
      chArray[index] = PlayerSegment.sConvertionDictionary[iSections[index]];
    return new string(chArray);
  }

  public static bool IsSegmentString(string iString)
  {
    foreach (char ch in iString)
    {
      if (!"NTF".Contains(ch.ToString()))
        return false;
    }
    return true;
  }

  public enum Section
  {
    NotApplicable = -1, // 0xFFFFFFFF
    False = 0,
    True = 1,
  }
}
