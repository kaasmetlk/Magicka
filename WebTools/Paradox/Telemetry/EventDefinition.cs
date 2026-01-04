// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.EventDefinition
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public class EventDefinition
{
  public const int MAX_EVENT_PARAM = 9;
  private const string EXCEPTION_TOO_MANY_PARAMETERS = "Too many parameters ({0}) were passed to a definition with a maximum allowed of {1}.";
  private const string EXCEPTION_TOO_MANY_ARGUMENTS = "Too many arguments ({0}) were passed to a definition with a maximum allowed of {1}.";
  private const string EXCEPTION_ARGUMENT_COUNT_ODD = "The provided number of arguments are odd. It should be even. Are you missing a parameter name?";
  private const string EXCEPTION_EXPECTED_TYPE_STRING = "The argument at position {0} is expected to be a parameter name of type String, received {1} instead.";
  private const string EXCEPTION_EXPECTED_TYPE_TYPE = "The argument at position {0} is expected to be a parameter name of type EventParameter.Type, received {1} instead.";
  private const string EXCEPTION_INDEX_OUT_OF_RANGE = "Index out of range. Provided index {0} with a maximum allowed of {1}.";
  private const string EXCEPTION_PARAMETER_NOT_FOUND = "Couldn't find a parameter with name {0}.";
  private readonly EventDefinition.Parameter[] mParameters;

  public int Count => this.mParameters.Length;

  public EventDefinition(params EventDefinition.Parameter[] iParameters)
  {
    this.mParameters = iParameters.Length <= 9 ? iParameters : throw new Exception($"Too many arguments ({iParameters.Length}) were passed to a definition with a maximum allowed of {9}.");
  }

  public EventDefinition(params object[] iArgs)
  {
    int length1 = iArgs.Length;
    if (length1 % 2 != 0)
      throw new Exception("The provided number of arguments are odd. It should be even. Are you missing a parameter name?");
    if (length1 > 18)
      throw new Exception($"Too many arguments ({length1}) were passed to a definition with a maximum allowed of {18}.");
    int length2 = length1 / 2;
    this.mParameters = new EventDefinition.Parameter[length2];
    if (length2 <= 0)
      return;
    for (int index1 = 0; index1 < length2; ++index1)
    {
      int index2 = index1 * 2;
      int index3 = index1 * 2 + 1;
      object iArg1 = iArgs[index2];
      object iArg2 = iArgs[index3];
      System.Type type1 = iArg1.GetType();
      System.Type type2 = iArg2.GetType();
      if (type1 != typeof (string))
        throw new Exception($"The argument at position {index2} is expected to be a parameter name of type String, received {type1.ToString()} instead.");
      if (type2 != typeof (EventParameter.Type))
        throw new Exception($"The argument at position {index3} is expected to be a parameter name of type EventParameter.Type, received {type2.ToString()} instead.");
      this.mParameters[index1] = new EventDefinition.Parameter((string) iArg1, (EventParameter.Type) iArg2);
    }
  }

  public string GetParameterName(int iParameterIndex)
  {
    if (iParameterIndex >= this.mParameters.Length)
      throw new IndexOutOfRangeException($"Index out of range. Provided index {iParameterIndex} with a maximum allowed of {this.mParameters.Length - 1}.");
    return this.mParameters.Length > 0 ? this.mParameters[iParameterIndex].Name : string.Empty;
  }

  public EventParameter.Type GetParameterType(int iParameterIndex)
  {
    if (iParameterIndex < 0 || iParameterIndex >= this.mParameters.Length)
      throw new IndexOutOfRangeException($"Index out of range. Provided index {iParameterIndex} with a maximum allowed of {this.mParameters.Length - 1}.");
    return this.mParameters[iParameterIndex].Type;
  }

  public System.Type GetParameterSystemType(int iParameterIndex)
  {
    if (iParameterIndex < 0 || iParameterIndex >= this.mParameters.Length)
      throw new IndexOutOfRangeException($"Index out of range. Provided index {iParameterIndex} with a maximum allowed of {this.mParameters.Length - 1}.");
    return EventParameter.ToSystemType(this.mParameters[iParameterIndex].Type);
  }

  public bool IsValid(object[] iValues)
  {
    if (this.mParameters == null)
      return iValues.Length == 0;
    if (iValues.Length != this.mParameters.Length)
      return false;
    for (int iParameterIndex = 0; iParameterIndex < iValues.Length; ++iParameterIndex)
    {
      if (!this.IsValid(iParameterIndex, iValues[iParameterIndex]))
        return false;
    }
    return true;
  }

  public bool IsValid(int iParameterIndex, EventParameter.Type iParameterType)
  {
    if (this.mParameters.Length == 0)
      return false;
    if (iParameterIndex < this.mParameters.Length)
      return this.mParameters[iParameterIndex].Type == iParameterType;
    throw new IndexOutOfRangeException($"Index out of range. Provided index {iParameterIndex} with a maximum allowed of {this.mParameters.Length - 1}.");
  }

  public bool IsValid(int iParameterIndex, object iParamObject)
  {
    if (this.mParameters.Length == 0)
      return false;
    if (iParameterIndex < this.mParameters.Length)
      return EventParameter.MatchType(iParamObject, this.mParameters[iParameterIndex].Type);
    throw new IndexOutOfRangeException($"Index out of range. Provided index {iParameterIndex} with a maximum allowed of {this.mParameters.Length - 1}.");
  }

  public bool IsValid(string iParameterName, EventParameter.Type iParameterType)
  {
    int index = Array.FindIndex<EventDefinition.Parameter>(this.mParameters, (Predicate<EventDefinition.Parameter>) (row => row.Name == iParameterName));
    return index >= 0 ? this.IsValid(index, iParameterType) : throw new Exception($"Couldn't find a parameter with name {iParameterName}.");
  }

  public bool IsValid(string iParameterName, object iParameterObject)
  {
    int index = Array.FindIndex<EventDefinition.Parameter>(this.mParameters, (Predicate<EventDefinition.Parameter>) (row => row.Name == iParameterName));
    return index >= 0 ? this.IsValid(index, iParameterObject) : throw new Exception($"Couldn't find a parameter with name {iParameterName}.");
  }

  public struct Parameter(string iName, EventParameter.Type iType)
  {
    public readonly string Name = iName;
    public readonly EventParameter.Type Type = iType;
  }
}
