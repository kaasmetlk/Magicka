// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.GameSparks.GameSparksPropertySet
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using GameSparks.Api.Responses;
using GameSparks.Core;
using Magicka.CoreFramework;
using Magicka.Misc;
using System.Collections.Generic;

#nullable disable
namespace Magicka.WebTools.GameSparks;

internal abstract class GameSparksPropertySet
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.GameSparksProperties;
  private const string INVALID_PROPERTY = "{0} is not a valid GameSparks property.";
  private const string INVALID_TYPE = "GameSparks property '{0}' is not of type '{1}'.";
  private const string FAILED_GET = "GameSparks property retrieval failed. ERROR: {0}";
  protected Dictionary<string, GameSparksPropertySet.Property> mProperties = new Dictionary<string, GameSparksPropertySet.Property>();
  protected Dictionary<string, GameSparksPropertySet.Property> mDefaultProperties = new Dictionary<string, GameSparksPropertySet.Property>();

  public abstract string Name { get; }

  public GameSparksPropertySet() => this.SetDefaults();

  public T GetProperty<T>(string iKey)
  {
    object propertyRaw = this.GetPropertyRaw(iKey);
    property = default (T);
    if (!(propertyRaw is T property))
      ;
    return property;
  }

  public void Callback(GetPropertySetResponse iResponse)
  {
    if (iResponse.HasErrors)
    {
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) iResponse.Errors.BaseData)
        Logger.LogError(Logger.Source.GameSparksProperties, $"GameSparks property retrieval failed. ERROR: {keyValuePair.Key}");
    }
    else
      this.Parse(iResponse.PropertySet);
  }

  public virtual void Parse(GSData iRawData)
  {
    this.mProperties.Clear();
    GameSparksPropertySet.Property property;
    foreach (KeyValuePair<string, object> keyValuePair1 in (IEnumerable<KeyValuePair<string, object>>) iRawData.BaseData)
    {
      foreach (KeyValuePair<string, object> keyValuePair2 in (IEnumerable<KeyValuePair<string, object>>) ((GSData) keyValuePair1.Value).BaseData)
      {
        if (this.mDefaultProperties.ContainsKey(keyValuePair2.Key))
        {
          if (this.IsTypeCorrect(this.mDefaultProperties[keyValuePair2.Key].mType, keyValuePair2.Value))
          {
            property = new GameSparksPropertySet.Property(keyValuePair2.Key, this.mDefaultProperties[keyValuePair2.Key].mType, keyValuePair2.Value);
            this.mProperties.Add(keyValuePair2.Key, property);
          }
          else
          {
            property = new GameSparksPropertySet.Property(keyValuePair2.Key, this.mDefaultProperties[keyValuePair2.Key].mType, this.mDefaultProperties[keyValuePair2.Key].mValue);
            this.mProperties.Add(keyValuePair2.Key, property);
          }
        }
        else
        {
          property = new GameSparksPropertySet.Property(keyValuePair2.Key, GameSparksPropertySet.PropertyType.COUNT, keyValuePair2.Value);
          this.mProperties.Add(keyValuePair2.Key, property);
        }
      }
    }
    Singleton<GameSparksProperties>.Instance.ConfirmPropertySetReady(this);
  }

  protected abstract void SetDefaults();

  private object GetPropertyRaw(string iKey)
  {
    object propertyRaw = (object) null;
    if (this.mProperties.ContainsKey(iKey))
      propertyRaw = this.mProperties[iKey].mValue;
    else if (this.mDefaultProperties.ContainsKey(iKey))
      propertyRaw = this.mDefaultProperties[iKey].mValue;
    return propertyRaw;
  }

  private bool IsTypeCorrect(GameSparksPropertySet.PropertyType iType, object iValue)
  {
    bool flag = false;
    switch (iType)
    {
      case GameSparksPropertySet.PropertyType.BOOL:
        flag = iValue is bool;
        break;
      case GameSparksPropertySet.PropertyType.INT:
        flag = iValue is int;
        break;
      case GameSparksPropertySet.PropertyType.FLOAT:
        flag = iValue is float;
        break;
    }
    return flag;
  }

  public enum PropertyType
  {
    BOOL,
    INT,
    FLOAT,
    ARRAY,
    DICTIONARY,
    COUNT,
  }

  public struct Property(string iName, GameSparksPropertySet.PropertyType iType, object iValue)
  {
    public readonly string mName = iName;
    public readonly GameSparksPropertySet.PropertyType mType = iType;
    public readonly object mValue = iValue;
  }
}
