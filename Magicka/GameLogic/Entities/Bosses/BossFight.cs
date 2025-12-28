using System;
using System.Collections.Generic;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.UI;
using Magicka.Network;
using Microsoft.Xna.Framework;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Bosses
{
	// Token: 0x020005AA RID: 1450
	public class BossFight
	{
		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06002B5E RID: 11102 RVA: 0x0015672C File Offset: 0x0015492C
		public static BossFight Instance
		{
			get
			{
				if (BossFight.mSingelton == null)
				{
					lock (BossFight.mSingeltonLock)
					{
						if (BossFight.mSingelton == null)
						{
							BossFight.mSingelton = new BossFight();
						}
					}
				}
				return BossFight.mSingelton;
			}
		}

		// Token: 0x06002B5F RID: 11103 RVA: 0x00156780 File Offset: 0x00154980
		private BossFight()
		{
			this.mHealthbar = new BossHealthBar(null);
			this.mTitleRenderer = new TitleRenderer();
			this.mBossNameSet = false;
		}

		// Token: 0x06002B60 RID: 11104 RVA: 0x001567E0 File Offset: 0x001549E0
		public void PassMessage(BossMessages iMessage)
		{
			for (int i = 0; i < this.mBosses.Count; i++)
			{
				this.mBosses[i].ScriptMessage(iMessage);
			}
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x00156815 File Offset: 0x00154A15
		public void SetTitles(string iBossTitle, string iBossSubTitle, float iDisplayTime, float iFadeIn, float iFadeOut)
		{
			this.mTitleRenderer.SetTitles(iBossTitle, iBossSubTitle, iDisplayTime, iFadeIn, iFadeOut);
			this.mTitleRenderer.Start();
			this.mBossNameSet = true;
		}

		// Token: 0x06002B62 RID: 11106 RVA: 0x0015683C File Offset: 0x00154A3C
		public void Setup(PlayState iPlayState, float iFreezeTime, float iHealthAppearDelay, float iHealthbarWidth)
		{
			this.mTitleRenderer.ResetTimer();
			this.mHealthbar.Reset();
			this.mHealthbar.Scene = iPlayState.Scene;
			this.mPlayState = iPlayState;
			this.mHealthAppearDelay = 0f;
			this.mHealthBarWidth = iHealthbarWidth;
			this.mHealthbar.SetWidth(iHealthbarWidth);
			this.mFreezeTime = iFreezeTime;
			this.mGotPlayState = true;
		}

		// Token: 0x06002B63 RID: 11107 RVA: 0x001568A8 File Offset: 0x00154AA8
		public void Clear()
		{
			this.mBosses.Clear();
			this.mGotPlayState = false;
			this.mDead = false;
			this.mBossNameSet = false;
			this.mTitleRenderer.ResetTimer();
			this.mNetworkMessageQueue.Clear();
			this.mNetworkInitializeMessageQueue.Clear();
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x001568F8 File Offset: 0x00154AF8
		public void Initialize(IBoss iBoss, int iAreaHash, int iUniqueID)
		{
			for (int i = 0; i < this.mNetworkInitializeMessageQueue.Count; i++)
			{
				BossInitializeMessage bossInitializeMessage = this.mNetworkInitializeMessageQueue[i];
				if (iBoss.GetBossType() == bossInitializeMessage.BossID)
				{
					iBoss.NetworkInitialize(ref bossInitializeMessage);
					this.mNetworkInitializeMessageQueue.RemoveAt(i--);
				}
			}
			Matrix matrix;
			this.mPlayState.Level.CurrentScene.GetLocator(iAreaHash, out matrix);
			iBoss.Initialize(ref matrix, iUniqueID);
			if (!this.mBosses.Contains(iBoss))
			{
				this.mBosses.Add(iBoss);
			}
			this.mPlayState.BossFight = this;
			this.mDead = false;
			float num = 0f;
			float num2 = 0f;
			foreach (IBoss boss in this.mBosses)
			{
				num += boss.HitPoints;
				num2 += boss.MaxHitPoints;
			}
			this.mNormalizedHealth = MathHelper.Clamp(num / num2, 0f, 1f);
			this.mRunning = false;
			this.mHealthAppearDelay = 0f;
			this.mHealthbar.Destroy = false;
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x00156A38 File Offset: 0x00154C38
		public void Start()
		{
			this.mRunning = true;
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x00156A44 File Offset: 0x00154C44
		public void Reset()
		{
			if (this.mPlayState != null)
			{
				this.mBosses.Clear();
				this.mPlayState.BossFight = null;
				this.mNormalizedHealth = 0f;
				this.mDead = false;
				this.mHealthbar.Reset();
				this.mBossNameSet = false;
				this.mTitleRenderer.ResetTimer();
			}
			this.mNetworkMessageQueue.Clear();
			this.mNetworkInitializeMessageQueue.Clear();
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x00156AB8 File Offset: 0x00154CB8
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mBossNameSet)
			{
				TitleRenderData titleRenderData = this.mTitleRenderer.Update((int)iDataChannel, iDeltaTime);
				if (titleRenderData == null)
				{
					this.mBossNameSet = false;
				}
				else
				{
					this.mPlayState.Scene.AddRenderableGUIObject(iDataChannel, titleRenderData);
				}
			}
			if (this.mRunning)
			{
				float num = 0f;
				float num2 = 0f;
				bool flag = true;
				foreach (IBoss boss in this.mBosses)
				{
					if (!boss.Dead)
					{
						flag = false;
					}
					num += Math.Max(boss.HitPoints, 0f);
					num2 += boss.MaxHitPoints;
				}
				this.mDead = (flag && this.mBosses.Count > 0);
				this.mNormalizedHealth = MathHelper.Clamp(num / num2, 0f, 1f);
				if (!this.mHealthbar.Destroy && num <= 0f)
				{
					this.mHealthbar.Destroy = true;
				}
				this.mHealthbar.SetNormalizedHealth(this.mNormalizedHealth);
				this.mHealthAppearDelay -= iDeltaTime;
				if (this.mHealthAppearDelay <= 0f)
				{
					this.mHealthbar.Update(iDataChannel, iDeltaTime);
				}
			}
			else
			{
				this.mNormalizedHealth = 0.0001f;
				this.mHealthbar.SetNormalizedHealth(this.mNormalizedHealth);
			}
			for (int i = 0; i < this.mNetworkMessageQueue.Count; i++)
			{
				BossUpdateMessage bossUpdateMessage = this.mNetworkMessageQueue[i];
				for (int j = 0; j < this.mBosses.Count; j++)
				{
					if (this.mBosses[j].GetBossType() == bossUpdateMessage.BossID)
					{
						this.mNetworkMessageQueue.RemoveAt(i--);
						this.mBosses[j].NetworkUpdate(ref bossUpdateMessage);
						break;
					}
				}
			}
			foreach (IBoss boss2 in this.mBosses)
			{
				boss2.UpdateBoss(iDataChannel, iDeltaTime, this.mRunning);
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06002B68 RID: 11112 RVA: 0x00156CFC File Offset: 0x00154EFC
		public bool IsSetup
		{
			get
			{
				return this.mGotPlayState;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06002B69 RID: 11113 RVA: 0x00156D04 File Offset: 0x00154F04
		public bool IsRunning
		{
			get
			{
				return this.mBosses.Count > 0 && this.mRunning;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06002B6A RID: 11114 RVA: 0x00156D1C File Offset: 0x00154F1C
		public bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06002B6B RID: 11115 RVA: 0x00156D24 File Offset: 0x00154F24
		public float NormalizedHealth
		{
			get
			{
				return this.mNormalizedHealth;
			}
		}

		// Token: 0x06002B6C RID: 11116 RVA: 0x00156D2C File Offset: 0x00154F2C
		internal void NetworkInitialize(ref BossInitializeMessage iMsg)
		{
			bool flag = false;
			for (int i = 0; i < this.mBosses.Count; i++)
			{
				if (this.mBosses[i].GetBossType() == iMsg.BossID)
				{
					this.mBosses[i].NetworkInitialize(ref iMsg);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.mNetworkInitializeMessageQueue.Add(iMsg);
			}
		}

		// Token: 0x06002B6D RID: 11117 RVA: 0x00156D94 File Offset: 0x00154F94
		internal unsafe void SendInitializeMessage<T>(IBoss iSender, ushort iType, void* iMessage) where T : struct
		{
			BossInitializeMessage bossInitializeMessage;
			BossInitializeMessage.ConvertFrom<T>(iType, iMessage, out bossInitializeMessage);
			bossInitializeMessage.BossID = iSender.GetBossType();
			NetworkManager.Instance.Interface.SendMessage<BossInitializeMessage>(ref bossInitializeMessage);
		}

		// Token: 0x06002B6E RID: 11118 RVA: 0x00156DC8 File Offset: 0x00154FC8
		internal unsafe void SendInitializeMessage<T>(IBoss iSender, ushort iType, void* iMessage, int iClientIndex) where T : struct
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				throw new Exception("This overload may only be called if this is the game server!");
			}
			BossInitializeMessage bossInitializeMessage;
			BossInitializeMessage.ConvertFrom<T>(iType, iMessage, out bossInitializeMessage);
			bossInitializeMessage.BossID = iSender.GetBossType();
			networkServer.SendMessage<BossInitializeMessage>(ref bossInitializeMessage, iClientIndex);
		}

		// Token: 0x06002B6F RID: 11119 RVA: 0x00156E14 File Offset: 0x00155014
		internal void NetworkUpdate(ref BossUpdateMessage iMsg)
		{
			bool flag = false;
			for (int i = 0; i < this.mBosses.Count; i++)
			{
				if (this.mBosses[i].GetBossType() == iMsg.BossID)
				{
					this.mBosses[i].NetworkUpdate(ref iMsg);
					flag = true;
				}
			}
			if (!flag)
			{
				this.mNetworkMessageQueue.Add(iMsg);
			}
		}

		// Token: 0x06002B70 RID: 11120 RVA: 0x00156E7C File Offset: 0x0015507C
		internal unsafe void SendMessage<T>(IBoss iSender, ushort iType, void* iMessage, bool iReliable) where T : struct
		{
			BossUpdateMessage bossUpdateMessage;
			BossUpdateMessage.ConvertFrom<T>(iType, iMessage, out bossUpdateMessage);
			bossUpdateMessage.BossID = iSender.GetBossType();
			if (iReliable)
			{
				NetworkManager.Instance.Interface.SendMessage<BossUpdateMessage>(ref bossUpdateMessage);
				return;
			}
			NetworkManager.Instance.Interface.SendUdpMessage<BossUpdateMessage>(ref bossUpdateMessage);
		}

		// Token: 0x06002B71 RID: 11121 RVA: 0x00156EC8 File Offset: 0x001550C8
		internal unsafe void SendMessage<T>(IBoss iSender, ushort iType, void* iMessage, bool iReliable, int iClientIndex) where T : struct
		{
			NetworkServer networkServer = NetworkManager.Instance.Interface as NetworkServer;
			if (networkServer == null)
			{
				throw new Exception("This overload may only be called if this is the game server!");
			}
			BossUpdateMessage bossUpdateMessage;
			BossUpdateMessage.ConvertFrom<T>(iType, iMessage, out bossUpdateMessage);
			bossUpdateMessage.BossID = iSender.GetBossType();
			if (iReliable)
			{
				networkServer.SendMessage<BossUpdateMessage>(ref bossUpdateMessage, iClientIndex);
				return;
			}
			networkServer.SendUdpMessage<BossUpdateMessage>(ref bossUpdateMessage, iClientIndex);
		}

		// Token: 0x06002B72 RID: 11122 RVA: 0x00156F22 File Offset: 0x00155122
		public void UpdateResolution()
		{
			this.mHealthbar.SetWidth(this.mHealthBarWidth);
		}

		// Token: 0x04002F02 RID: 12034
		private static BossFight mSingelton;

		// Token: 0x04002F03 RID: 12035
		private static volatile object mSingeltonLock = new object();

		// Token: 0x04002F04 RID: 12036
		private PlayState mPlayState;

		// Token: 0x04002F05 RID: 12037
		private List<IBoss> mBosses = new List<IBoss>();

		// Token: 0x04002F06 RID: 12038
		private float mNormalizedHealth;

		// Token: 0x04002F07 RID: 12039
		private float mHealthAppearDelay;

		// Token: 0x04002F08 RID: 12040
		private float mFreezeTime;

		// Token: 0x04002F09 RID: 12041
		private BossHealthBar mHealthbar;

		// Token: 0x04002F0A RID: 12042
		private bool mGotPlayState;

		// Token: 0x04002F0B RID: 12043
		private bool mDead;

		// Token: 0x04002F0C RID: 12044
		private bool mRunning;

		// Token: 0x04002F0D RID: 12045
		private bool mBossNameSet;

		// Token: 0x04002F0E RID: 12046
		private TitleRenderer mTitleRenderer;

		// Token: 0x04002F0F RID: 12047
		private List<BossUpdateMessage> mNetworkMessageQueue = new List<BossUpdateMessage>(4);

		// Token: 0x04002F10 RID: 12048
		private List<BossInitializeMessage> mNetworkInitializeMessageQueue = new List<BossInitializeMessage>(4);

		// Token: 0x04002F11 RID: 12049
		private List<KeyValuePair<BossEnum, int>> mInitializeParamQueue = new List<KeyValuePair<BossEnum, int>>(4);

		// Token: 0x04002F12 RID: 12050
		private float mHealthBarWidth;
	}
}
