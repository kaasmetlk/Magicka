// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Conditions.Condition
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Conditions;

public abstract class Condition
{
  public static readonly int ANYID = "any".GetHashCodeCustom();
  private bool mInvert;
  private GameScene mScene;

  protected Condition(GameScene iScene) => this.mScene = iScene;

  public virtual void Initialize()
  {
  }

  protected static Type GetType(string name)
  {
    Type[] types = Assembly.GetExecutingAssembly().GetTypes();
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].BaseType == typeof (Condition) && types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  protected abstract bool InternalMet(Character iSender);

  public virtual bool IsMet(Character iSender) => this.mInvert ^ this.InternalMet(iSender);

  public static Condition Read(GameScene iParent, XmlNode iNode)
  {
    Type type = Condition.GetType(iNode.Name);
    Condition condition = (Condition) type.GetConstructor(new Type[1]
    {
      typeof (GameScene)
    }).Invoke(new object[1]{ (object) iParent });
    PropertyInfo[] properties = type.GetProperties();
    for (int i = 0; i < iNode.Attributes.Count; ++i)
    {
      XmlAttribute attribute = iNode.Attributes[i];
      PropertyInfo propertyInfo = (PropertyInfo) null;
      for (int index = 0; index < properties.Length; ++index)
      {
        if (properties[index].CanWrite && properties[index].Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase))
        {
          propertyInfo = properties[index];
          break;
        }
      }
      if (propertyInfo != null)
      {
        if (propertyInfo.PropertyType.IsEnum)
          propertyInfo.SetValue((object) condition, Enum.Parse(propertyInfo.PropertyType, attribute.Value, true), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (bool))
          propertyInfo.SetValue((object) condition, (object) bool.Parse(attribute.Value), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (int))
          propertyInfo.SetValue((object) condition, (object) int.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (float))
          propertyInfo.SetValue((object) condition, (object) float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (string))
          propertyInfo.SetValue((object) condition, (object) attribute.Value.ToLowerInvariant(), (object[]) null);
        else
          throw new NotSupportedException($"Invalid type \"{propertyInfo.PropertyType.Name}\" trying to parse attribute \"{attribute.Name}\" in XML node \"{iNode.Name}\"!");
      }
    }
    return condition;
  }

  public GameScene Scene => this.mScene;

  public bool Invert
  {
    get => this.mInvert;
    set => this.mInvert = value;
  }
}
