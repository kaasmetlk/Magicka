// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Triggers.Actions.Action
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Triggers.Actions;

public abstract class Action
{
  protected float mDelay;
  protected float mDelayCountdown;
  protected int mQueue;
  protected Trigger mTrigger;
  protected GameScene mScene;
  private static List<Action> sInstances = new List<Action>();
  private readonly ushort mHandle;

  protected Action(Trigger iTrigger, GameScene iScene)
  {
    this.mHandle = (ushort) Action.sInstances.Count;
    if (Action.sInstances.Count >= (int) ushort.MaxValue)
      throw new Exception("To many actions! Max allowed per level is " + (object) ushort.MaxValue);
    Action.sInstances.Add(this);
    this.mTrigger = iTrigger;
    this.mScene = iScene;
  }

  public ushort Handle => this.mHandle;

  public static void ClearInstances() => Action.sInstances.Clear();

  public static Action GetByHandle(ushort iHandel)
  {
    return (int) iHandel < Action.sInstances.Count ? Action.sInstances[(int) iHandel] : (Action) null;
  }

  public virtual void Initialize()
  {
  }

  protected abstract void Execute();

  public abstract void QuickExecute();

  protected static Type GetType(string name)
  {
    Type[] types;
    try
    {
      types = Assembly.GetExecutingAssembly().GetModules()[0].GetTypes();
    }
    catch (Exception ex)
    {
      throw ex;
    }
    for (int index = 0; index < types.Length; ++index)
    {
      if (types[index].BaseType == typeof (Action) && types[index].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return types[index];
    }
    return (Type) null;
  }

  public virtual void OnTrigger(Character iArg)
  {
    if (this.mQueue <= 0)
    {
      this.mQueue = 1;
      this.mDelayCountdown = this.mDelay;
    }
    else
      ++this.mQueue;
  }

  public bool HasFinishedExecuting() => this.mQueue == 0;

  public virtual void Update(float iDeltaTime)
  {
    this.mDelayCountdown -= iDeltaTime;
    while ((double) this.mDelayCountdown <= 0.0 && this.mQueue > 0)
    {
      --this.mQueue;
      this.mDelayCountdown += this.mDelay;
      this.Execute();
      this.GameScene.ActionExecute(this);
    }
  }

  public void ClearDelayed() => this.mQueue = 0;

  public static Action Read(GameScene iScene, Trigger iTrigger, XmlNode iNode)
  {
    Type type = Action.GetType(iNode.Name);
    ConstructorInfo constructor = type.GetConstructor(new Type[3]
    {
      typeof (Trigger),
      typeof (GameScene),
      typeof (XmlNode)
    });
    Action action;
    if (constructor == null)
      action = (Action) type.GetConstructor(new Type[2]
      {
        typeof (Trigger),
        typeof (GameScene)
      }).Invoke(new object[2]
      {
        (object) iTrigger,
        (object) iScene
      });
    else
      action = (Action) constructor.Invoke(new object[3]
      {
        (object) iTrigger,
        (object) iScene,
        (object) iNode
      });
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
      if (propertyInfo != null && propertyInfo != null)
      {
        if (propertyInfo.PropertyType.IsEnum)
          propertyInfo.SetValue((object) action, Enum.Parse(propertyInfo.PropertyType, attribute.Value, true), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (bool))
          propertyInfo.SetValue((object) action, (object) bool.Parse(attribute.Value), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (int))
          propertyInfo.SetValue((object) action, (object) int.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (float))
          propertyInfo.SetValue((object) action, (object) float.Parse(attribute.Value, (IFormatProvider) CultureInfo.InvariantCulture), (object[]) null);
        else if (propertyInfo.PropertyType == typeof (Vector2))
        {
          string[] strArray = attribute.Value.Split(',');
          propertyInfo.SetValue((object) action, (object) new Vector2()
          {
            X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture),
            Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture)
          }, (object[]) null);
        }
        else if (propertyInfo.PropertyType == typeof (Vector3))
        {
          string[] strArray = attribute.Value.Split(',');
          propertyInfo.SetValue((object) action, (object) new Vector3()
          {
            X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture),
            Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture),
            Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture)
          }, (object[]) null);
        }
        else if (propertyInfo.PropertyType == typeof (Vector4))
        {
          string[] strArray = attribute.Value.Split(',');
          propertyInfo.SetValue((object) action, (object) new Vector4()
          {
            X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture),
            Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture),
            Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture),
            W = float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture)
          }, (object[]) null);
        }
        else if (propertyInfo.PropertyType == typeof (Color))
        {
          string[] strArray = attribute.Value.Split(',');
          Color color = new Color(new Vector4()
          {
            X = float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture),
            Y = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture),
            Z = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture),
            W = float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture)
          });
          propertyInfo.SetValue((object) action, (object) color, (object[]) null);
        }
        else if (propertyInfo.PropertyType == typeof (string))
          propertyInfo.SetValue((object) action, (object) attribute.Value.ToLowerInvariant(), (object[]) null);
        else
          throw new NotSupportedException($"Invalid type \"{propertyInfo.PropertyType.Name}\" trying to parse attribute \"{attribute.Name}\" in XML node \"{iNode.Name}\"!");
      }
    }
    return action;
  }

  public float Delay
  {
    get => this.mDelay;
    set => this.mDelay = value;
  }

  public GameScene GameScene => this.mScene;

  public Trigger Trigger => this.mTrigger;

  protected virtual object Tag { get; set; }

  protected virtual void WriteTag(BinaryWriter iWriter, object mTag)
  {
    throw new NotImplementedException();
  }

  protected virtual object ReadTag(BinaryReader iReader) => throw new NotImplementedException();

  public struct State
  {
    private float mDelayCountdown;
    private int mQueue;
    private object mTag;

    public State(Action iAction)
    {
      this.mDelayCountdown = iAction.mDelayCountdown;
      this.mQueue = iAction.mQueue;
      this.mTag = iAction.Tag;
    }

    public State(BinaryReader iReader, Action iAction)
    {
      this.mDelayCountdown = iReader.ReadSingle();
      this.mQueue = iReader.ReadInt32();
      this.mTag = (object) null;
      if (!iReader.ReadBoolean())
        return;
      this.mTag = iAction.ReadTag(iReader);
    }

    public void AssignToAction(Action iAction)
    {
      iAction.mDelayCountdown = this.mDelayCountdown;
      iAction.mQueue = this.mQueue;
      iAction.Tag = this.mTag;
    }

    public void Reset(Action iAction)
    {
      iAction.mDelayCountdown = 0.0f;
      iAction.mQueue = 0;
    }

    internal void Write(BinaryWriter iWriter, Action iAction)
    {
      iWriter.Write(this.mDelayCountdown);
      iWriter.Write(this.mQueue);
      iWriter.Write(this.mTag != null);
      if (this.mTag == null)
        return;
      iAction.WriteTag(iWriter, this.mTag);
    }
  }
}
