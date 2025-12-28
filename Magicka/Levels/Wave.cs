using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels
{
	// Token: 0x020000B2 RID: 178
	public class Wave
	{
		// Token: 0x0600053A RID: 1338 RVA: 0x0001F79B File Offset: 0x0001D99B
		public Wave(GameScene iGameScene)
		{
			this.mGameScene = iGameScene;
			this.mCharacters = new List<WeakReference>();
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001F7B8 File Offset: 0x0001D9B8
		public void Initialize(SurvivalRuleset iRules)
		{
			this.mPreReadHitPoints = 0f;
			this.mTotalHitPoints = 0f;
			this.mExecuting = false;
			this.mCharacters.Clear();
			this.mWaveStarted = false;
			for (int i = 0; i < this.mWaveActions.Count; i++)
			{
				this.mWaveActions[i].Initialize(iRules);
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001F81C File Offset: 0x0001DA1C
		public void Update(float iDeltaTime, SurvivalRuleset iRules)
		{
			if (this.mDelay <= 0f)
			{
				this.mWaveStarted = true;
				this.mExecuting = false;
				for (int i = 0; i < this.mWaveActions.Count; i++)
				{
					this.mWaveActions[i].Update(iDeltaTime, this.mGameScene, iRules, ref this.mExecuting);
				}
				return;
			}
			this.mExecuting = true;
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001F881 File Offset: 0x0001DA81
		internal void TrackCharacter(NonPlayerCharacter iChar, bool iItemEvent)
		{
			this.mCharacters.Add(new WeakReference(iChar));
			if (!iItemEvent)
			{
				this.mPreReadHitPoints -= iChar.HitPoints;
				return;
			}
			this.mTotalHitPoints += iChar.HitPoints;
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0001F8BE File Offset: 0x0001DABE
		// (set) Token: 0x0600053F RID: 1343 RVA: 0x0001F8C6 File Offset: 0x0001DAC6
		public float PreReadHitPoints
		{
			get
			{
				return this.mPreReadHitPoints;
			}
			set
			{
				this.mPreReadHitPoints = value;
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001F8D0 File Offset: 0x0001DAD0
		public float HitPointPercentage()
		{
			float num = 0f;
			for (int i = 0; i < this.mCharacters.Count; i++)
			{
				if (this.mCharacters[i].Target != null && (this.mCharacters[i].Target as Character).Faction == Factions.EVIL)
				{
					num += Math.Max(0f, (this.mCharacters[i].Target as NonPlayerCharacter).HitPoints);
				}
			}
			return (num + this.mPreReadHitPoints) / this.mTotalHitPoints;
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x0001F961 File Offset: 0x0001DB61
		// (set) Token: 0x06000542 RID: 1346 RVA: 0x0001F969 File Offset: 0x0001DB69
		public float TotalHitPoints
		{
			get
			{
				return this.mTotalHitPoints;
			}
			set
			{
				this.mTotalHitPoints = value;
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001F972 File Offset: 0x0001DB72
		public bool HasStarted()
		{
			return this.mWaveStarted;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001F97A File Offset: 0x0001DB7A
		public void StartWave(float iDelay)
		{
			this.mDelay = iDelay;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001F983 File Offset: 0x0001DB83
		public bool IsDone()
		{
			return !this.mExecuting;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001F990 File Offset: 0x0001DB90
		public void Read(XmlNode iNode)
		{
			this.mWaveActions = new List<WaveActions>();
			for (int i = 0; i < iNode.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = iNode.ChildNodes[i];
				WaveActions waveActions = new WaveActions(this.mGameScene, this);
				if (!(xmlNode is XmlComment))
				{
					waveActions.Read(this.mGameScene, xmlNode);
					this.mWaveActions.Add(waveActions);
				}
			}
		}

		// Token: 0x04000410 RID: 1040
		private bool mWaveStarted;

		// Token: 0x04000411 RID: 1041
		private bool mExecuting;

		// Token: 0x04000412 RID: 1042
		private float mDelay;

		// Token: 0x04000413 RID: 1043
		private GameScene mGameScene;

		// Token: 0x04000414 RID: 1044
		private List<WaveActions> mWaveActions;

		// Token: 0x04000415 RID: 1045
		private List<WeakReference> mCharacters;

		// Token: 0x04000416 RID: 1046
		private float mTotalHitPoints;

		// Token: 0x04000417 RID: 1047
		private float mPreReadHitPoints;
	}
}
