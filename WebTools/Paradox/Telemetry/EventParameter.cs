// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.Paradox.Telemetry.EventParameter
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.WebTools.Paradox.Telemetry.TypeValidators;
using System;
using System.Collections.Generic;

#nullable disable
namespace Magicka.WebTools.Paradox.Telemetry;

public static class EventParameter
{
  private const string EXCEPTION_NULL_VALIDATOR = "Cannot register a null type validator.";
  private const string EXCEPTION_MISSING_VALIDATOR = "Missing validator for type {0}.";
  private const int MAX_ALLOWED_CHARACTERS = 128 /*0x80*/;
  private static readonly Dictionary<EventParameter.Type, ITypeValidator> sTypeValidators = new Dictionary<EventParameter.Type, ITypeValidator>();

  static EventParameter()
  {
    EventParameter.RegisterValidator(EventParameter.Type.Boolean, (ITypeValidator) new BaseTypeValidator<bool>());
    EventParameter.RegisterValidator(EventParameter.Type.Int, (ITypeValidator) new BaseTypeValidator<int>());
    EventParameter.RegisterValidator(EventParameter.Type.Int64, (ITypeValidator) new BaseTypeValidator<long>());
    EventParameter.RegisterValidator(EventParameter.Type.UInt, (ITypeValidator) new BaseTypeValidator<uint>());
    EventParameter.RegisterValidator(EventParameter.Type.UInt64, (ITypeValidator) new BaseTypeValidator<ulong>());
    EventParameter.RegisterValidator(EventParameter.Type.Float, (ITypeValidator) new BaseTypeValidator<float>());
    EventParameter.RegisterValidator(EventParameter.Type.String, (ITypeValidator) new BaseTypeValidator<string>());
    EventParameter.RegisterValidator(EventParameter.Type.PlayerSegment, (ITypeValidator) new PlayerSegmentTypeValidator());
    EventParameter.RegisterValidator(EventParameter.Type.TutorialActionEnum, (ITypeValidator) new BaseTypeValidator<EventEnums.TutorialAction>());
    EventParameter.RegisterValidator(EventParameter.Type.ControllerTypeEnum, (ITypeValidator) new BaseTypeValidator<EventEnums.ControllerType>());
    EventParameter.RegisterValidator(EventParameter.Type.NetworkStateEnum, (ITypeValidator) new NetworkStateTypeValidator());
    EventParameter.RegisterValidator(EventParameter.Type.MagickTypeEnum, (ITypeValidator) new MagickTypeTypeValidator());
    EventParameter.RegisterValidator(EventParameter.Type.PlayerDeath, (ITypeValidator) new BaseTypeValidator<EventEnums.DeathCategory>());
  }

  private static void RegisterValidator(EventParameter.Type iType, ITypeValidator iValidator)
  {
    if (iValidator == null)
      throw new NullReferenceException("Cannot register a null type validator.");
    EventParameter.sTypeValidators.Add(iType, iValidator);
  }

  public static bool MatchType(object iObject, EventParameter.Type iExpectedType)
  {
    if (EventParameter.sTypeValidators.ContainsKey(iExpectedType))
      return EventParameter.sTypeValidators[iExpectedType].MatchType(iObject);
    throw new Exception($"Missing validator for type {iExpectedType.ToString()}.");
  }

  public static string ToString(object iObject, EventParameter.Type iExpectedType)
  {
    string str = string.Empty;
    if (EventParameter.MatchType(iObject, iExpectedType))
    {
      str = EventParameter.sTypeValidators[iExpectedType].GetFormattedString(iObject);
      if (str.Length > 128 /*0x80*/)
      {
        Logger.LogWarning(Logger.Source.EventParameter, $"The data sent for this param is too big and will be truncated. Truncated string will be : {str.Substring(0, 128 /*0x80*/)}##Truncated##");
        Logger.LogWarning(Logger.Source.EventParameter, $"Truncated string will be : \"{str.Substring(0, 128 /*0x80*/)}\"##Truncated##");
      }
    }
    return str;
  }

  public static System.Type ToSystemType(EventParameter.Type iType)
  {
    return EventParameter.sTypeValidators.ContainsKey(iType) ? EventParameter.sTypeValidators[iType].GetSystemType() : throw new Exception($"Missing validator for type {iType.ToString()}.");
  }

  public enum Type
  {
    Boolean,
    Int,
    Int64,
    UInt,
    UInt64,
    Float,
    String,
    PlayerSegment,
    TutorialActionEnum,
    ControllerTypeEnum,
    NetworkStateEnum,
    MagickTypeEnum,
    PlayerDeath,
  }
}
