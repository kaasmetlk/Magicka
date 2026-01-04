// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.GameStates.GameStateManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.GameLogic.GameStates.Persistent;
using PolygonHead;
using System.Collections.Generic;

#nullable disable
namespace Magicka.GameLogic.GameStates;

public sealed class GameStateManager
{
  private Stack<GameState> mStack;
  private PersistentGameState mPersistentGameState;
  private object mChangeStateLock = new object();
  private static GameStateManager mSingelton;
  private static volatile object mSingeltonLock = new object();
  private bool mPopState;
  private Queue<GameState> mPushStates = new Queue<GameState>();

  public static GameStateManager Instance
  {
    get
    {
      if (GameStateManager.mSingelton == null)
      {
        lock (GameStateManager.mSingeltonLock)
        {
          if (GameStateManager.mSingelton == null)
            GameStateManager.mSingelton = new GameStateManager();
        }
      }
      return GameStateManager.mSingelton;
    }
  }

  public PersistentGameState PersistentState => this.mPersistentGameState;

  private GameStateManager()
  {
    this.mStack = new Stack<GameState>();
    this.mPersistentGameState = new PersistentGameState();
    this.mPersistentGameState.OnEnter();
  }

  public void PushState(GameState iGameState) => this.mPushStates.Enqueue(iGameState);

  private void InternalPushState(GameState iGameState)
  {
    lock (this.mChangeStateLock)
    {
      if (this.mStack.Count > 0)
        this.mStack.Peek().OnExit();
      iGameState.OnEnter();
      this.mStack.Push(iGameState);
    }
  }

  public void PopState() => this.mPopState = true;

  private void InternalPopState()
  {
    lock (this.mChangeStateLock)
    {
      this.mStack.Pop().OnExit();
      if (this.mStack.Count > 0)
        this.mStack.Peek().OnEnter();
      else
        Game.Instance.Exit();
    }
  }

  public void ChangeState(GameState iGameState)
  {
    lock (this.mChangeStateLock)
    {
      this.mStack.Pop().OnExit();
      iGameState.OnEnter();
      this.mStack.Push(iGameState);
    }
  }

  public GameState CurrentState => this.mStack.Count != 0 ? this.mStack.Peek() : (GameState) null;

  public bool IsStackEmpty => this.mStack.Count == 0;

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mPopState)
    {
      this.mPopState = false;
      this.InternalPopState();
    }
    while (this.mPushStates.Count > 0)
      this.InternalPushState(this.mPushStates.Dequeue());
    lock (this.mChangeStateLock)
    {
      this.mStack.Peek().Update(iDataChannel, iDeltaTime);
      if (this.mPersistentGameState == null)
        return;
      this.mPersistentGameState.Update(iDataChannel, iDeltaTime);
    }
  }
}
