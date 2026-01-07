// Decompiled with JetBrains decompiler
// Type: PolygonHead.IHeap`1
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

#nullable disable
namespace PolygonHead;

public interface IHeap<T>
{
  bool IsEmpty { get; }

  void Push(T iValue);

  T Pop();

  T Peek();

  void Clear();

  bool Contains(T iValue);

  int Count { get; }
}
