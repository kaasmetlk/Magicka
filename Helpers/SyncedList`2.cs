// Decompiled with JetBrains decompiler
// Type: PolygonHead.Helpers.SyncedList`2
// Assembly: PolygonHead, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AF997701-4383-4FB0-841F-9C71849233DF
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\PolygonHead.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace PolygonHead.Helpers;

public class SyncedList<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable
{
  private List<TKey> mTKeys;
  private List<TValue> mTValues;
  private ReadOnlyCollection<TKey> mReadOnlyKeys;
  private ReadOnlyCollection<TValue> mReadOnlyValues;

  public SyncedList(int iCapacity)
  {
    this.mTKeys = new List<TKey>(iCapacity);
    this.mTValues = new List<TValue>(iCapacity);
    this.mReadOnlyKeys = new ReadOnlyCollection<TKey>((IList<TKey>) this.mTKeys);
    this.mReadOnlyValues = new ReadOnlyCollection<TValue>((IList<TValue>) this.mTValues);
  }

  public TValue this[TKey key]
  {
    get => this.mTValues[this.mTKeys.IndexOf(key)];
    set
    {
      int index = this.mTKeys.IndexOf(key);
      if (index < 0)
        this.Add(key, value);
      else
        this.mTValues[index] = value;
    }
  }

  public void Add(TKey key, TValue value)
  {
    this.mTKeys.Add(key);
    this.mTValues.Add(value);
  }

  public bool ContainsKey(TKey key) => this.mTKeys.Contains(key);

  public ICollection<TKey> Keys => (ICollection<TKey>) this.mReadOnlyKeys;

  public bool Remove(TKey key)
  {
    int index = this.mTKeys.IndexOf(key);
    if (index < 0)
      return false;
    this.mTKeys.RemoveAt(index);
    this.mTValues.RemoveAt(index);
    return true;
  }

  public void RemoveAt(int index)
  {
    this.mTKeys.RemoveAt(index);
    this.mTValues.RemoveAt(index);
  }

  public bool TryGetValue(TKey key, out TValue value)
  {
    int index = this.mTKeys.IndexOf(key);
    if (index < 0)
    {
      value = default (TValue);
      return false;
    }
    value = this.mTValues[index];
    return true;
  }

  public ICollection<TValue> Values => (ICollection<TValue>) this.mReadOnlyValues;

  public void Add(KeyValuePair<TKey, TValue> item)
  {
    this.mTKeys.Add(item.Key);
    this.mTValues.Add(item.Value);
  }

  public KeyValuePair<TKey, TValue> GetKeyValuePair(int iIndex)
  {
    return new KeyValuePair<TKey, TValue>(this.mTKeys[iIndex], this.mTValues[iIndex]);
  }

  public void Clear()
  {
    this.mTKeys.Clear();
    this.mTValues.Clear();
  }

  public bool Contains(KeyValuePair<TKey, TValue> item)
  {
    return this.mTKeys.Contains(item.Key) && this.mTValues.Contains(item.Value);
  }

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    for (int index = 0; index < this.mTKeys.Count; ++index)
      array[index + arrayIndex] = new KeyValuePair<TKey, TValue>(this.mTKeys[index], this.mTValues[index]);
  }

  public int Count => this.mTKeys.Count;

  public bool IsReadOnly => false;

  public bool Remove(KeyValuePair<TKey, TValue> item)
  {
    int index = this.mTKeys.IndexOf(item.Key);
    if (index < 0 || !this.mTValues[index].Equals((object) item.Value))
      return false;
    this.mTKeys.RemoveAt(index);
    this.mTValues.RemoveAt(index);
    return true;
  }

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<TKey, TValue>>) new SyncedList<TKey, TValue>.Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new SyncedList<TKey, TValue>.Enumerator();
  }

  private struct Enumerator(SyncedList<TKey, TValue> iSyncedList) : 
    IEnumerator<KeyValuePair<TKey, TValue>>,
    IDisposable,
    IEnumerator
  {
    private int mCurrentIndex = -1;
    private SyncedList<TKey, TValue> mSyncedList = iSyncedList;

    public KeyValuePair<TKey, TValue> Current
    {
      get
      {
        return this.mCurrentIndex < 0 || this.mCurrentIndex > this.mSyncedList.Count ? new KeyValuePair<TKey, TValue>() : this.mSyncedList.GetKeyValuePair(this.mCurrentIndex);
      }
    }

    public void Dispose()
    {
      this.mSyncedList = (SyncedList<TKey, TValue>) null;
      this.mCurrentIndex = -1;
    }

    object IEnumerator.Current => throw new NotImplementedException();

    public bool MoveNext() => this.mSyncedList.Count < ++this.mCurrentIndex;

    public bool MoveBack() => 0 <= --this.mCurrentIndex;

    public void Reset() => this.mCurrentIndex = -1;
  }
}
