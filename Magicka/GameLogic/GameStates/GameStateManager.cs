using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates.Persistent;
using PolygonHead;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x020002C2 RID: 706
	public sealed class GameStateManager
	{
		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001564 RID: 5476 RVA: 0x0008A0F8 File Offset: 0x000882F8
		public static GameStateManager Instance
		{
			get
			{
				if (GameStateManager.mSingelton == null)
				{
					lock (GameStateManager.mSingeltonLock)
					{
						if (GameStateManager.mSingelton == null)
						{
							GameStateManager.mSingelton = new GameStateManager();
						}
					}
				}
				return GameStateManager.mSingelton;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001565 RID: 5477 RVA: 0x0008A14C File Offset: 0x0008834C
		public PersistentGameState PersistentState
		{
			get
			{
				return this.mPersistentGameState;
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x0008A154 File Offset: 0x00088354
		private GameStateManager()
		{
			this.mStack = new Stack<GameState>();
			this.mPersistentGameState = new PersistentGameState();
			this.mPersistentGameState.OnEnter();
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x0008A193 File Offset: 0x00088393
		public void PushState(GameState iGameState)
		{
			this.mPushStates.Enqueue(iGameState);
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x0008A1A4 File Offset: 0x000883A4
		private void InternalPushState(GameState iGameState)
		{
			lock (this.mChangeStateLock)
			{
				if (this.mStack.Count > 0)
				{
					this.mStack.Peek().OnExit();
				}
				iGameState.OnEnter();
				this.mStack.Push(iGameState);
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0008A208 File Offset: 0x00088408
		public void PopState()
		{
			this.mPopState = true;
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0008A214 File Offset: 0x00088414
		private void InternalPopState()
		{
			lock (this.mChangeStateLock)
			{
				this.mStack.Pop().OnExit();
				if (this.mStack.Count > 0)
				{
					this.mStack.Peek().OnEnter();
				}
				else
				{
					Game.Instance.Exit();
				}
			}
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0008A284 File Offset: 0x00088484
		public void ChangeState(GameState iGameState)
		{
			lock (this.mChangeStateLock)
			{
				this.mStack.Pop().OnExit();
				iGameState.OnEnter();
				this.mStack.Push(iGameState);
			}
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x0008A2DC File Offset: 0x000884DC
		public GameState CurrentState
		{
			get
			{
				if (this.mStack.Count != 0)
				{
					return this.mStack.Peek();
				}
				return null;
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600156D RID: 5485 RVA: 0x0008A2F8 File Offset: 0x000884F8
		public bool IsStackEmpty
		{
			get
			{
				return this.mStack.Count == 0;
			}
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x0008A308 File Offset: 0x00088508
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mPopState)
			{
				this.mPopState = false;
				this.InternalPopState();
			}
			while (this.mPushStates.Count > 0)
			{
				this.InternalPushState(this.mPushStates.Dequeue());
			}
			lock (this.mChangeStateLock)
			{
				this.mStack.Peek().Update(iDataChannel, iDeltaTime);
				if (this.mPersistentGameState != null)
				{
					this.mPersistentGameState.Update(iDataChannel, iDeltaTime);
				}
			}
		}

		// Token: 0x040016E6 RID: 5862
		private Stack<GameState> mStack;

		// Token: 0x040016E7 RID: 5863
		private PersistentGameState mPersistentGameState;

		// Token: 0x040016E8 RID: 5864
		private object mChangeStateLock = new object();

		// Token: 0x040016E9 RID: 5865
		private static GameStateManager mSingelton;

		// Token: 0x040016EA RID: 5866
		private static volatile object mSingeltonLock = new object();

		// Token: 0x040016EB RID: 5867
		private bool mPopState;

		// Token: 0x040016EC RID: 5868
		private Queue<GameState> mPushStates = new Queue<GameState>();
	}
}
