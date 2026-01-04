// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.EventData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public class EventData
{
  private const string EXCEPTION_NO_EVENT_NAME = "Cannot send a telemetry event without name.";
  private const string EXCEPTION_NULL_PARAMETER_ARRAY = "Cannot access a parameter from an event data with an empty parameter list.";
  private const string EXCEPTION_PARAMETER_INDEX_OUT_OF_RANGE = "Index out of range when trying to access a parameter from an event data.";
  private readonly string mName = string.Empty;
  private readonly object[] mParameters;

  public EventData(string iEventName, params object[] iParameters)
  {
    this.mName = !string.IsNullOrEmpty(iEventName) ? iEventName : throw new Exception("Cannot send a telemetry event without name.");
    this.mParameters = iParameters;
  }

  public string Name => this.mName;

  public int ParameterCount => this.mParameters == null ? 0 : this.mParameters.Length;

  public object[] Parameters => this.mParameters == null ? new object[0] : this.mParameters;

  public object GetParameter(int iIndex)
  {
    if (this.mParameters == null)
      throw new Exception("Cannot access a parameter from an event data with an empty parameter list.");
    if (iIndex < 0 || iIndex >= this.mParameters.Length)
      throw new IndexOutOfRangeException("Index out of range when trying to access a parameter from an event data.");
    return this.mParameters[iIndex];
  }
}
