using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Magicka.Levels.Triggers;
using Magicka.Levels.Triggers.Actions;

namespace Magicka.Levels
{
	// Token: 0x020000B3 RID: 179
	public class WaveActions
	{
		// Token: 0x06000547 RID: 1351 RVA: 0x0001F9F9 File Offset: 0x0001DBF9
		public WaveActions(GameScene iGameScene, Wave iWave)
		{
			this.mWave = iWave;
			this.mGameScene = iGameScene;
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001FA10 File Offset: 0x0001DC10
		public void Initialize(SurvivalRuleset iRules)
		{
			this.mSpawnedHitPoins = 0f;
			this.mHasExecuted = false;
			this.mSpawnedHitPoins = 0f;
			this.mDelay = this.mInitalDelay;
			if (this.mArea == null || this.mArea.Equals("any", StringComparison.OrdinalIgnoreCase))
			{
				this.mArea = iRules.GetAreas()[MagickaMath.Random.Next(0, iRules.GetAreas().Count)];
			}
			for (int i = 0; i < this.mActions.Count; i++)
			{
				this.mActions[i].Initialize();
				Type type = this.mActions[i].GetType();
				PropertyInfo[] properties = type.GetProperties();
				for (int j = 0; j < properties.Length; j++)
				{
					if (properties[j].Name.Equals("Area"))
					{
						properties[j].SetValue(this.mActions[i], this.mArea, null);
						break;
					}
				}
				if (this.mActions[i] is Spawn)
				{
					this.mSpawnedHitPoins += (this.mActions[i] as Spawn).GetTotalHitPoins();
				}
				if (this.mActions[i] is SpawnDispenser)
				{
					this.mSpawnedHitPoins += (this.mActions[i] as SpawnDispenser).GetTotalHitPoins();
				}
			}
			this.mWave.PreReadHitPoints += this.mSpawnedHitPoins;
			this.mWave.TotalHitPoints += this.mSpawnedHitPoins;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001FBAC File Offset: 0x0001DDAC
		public void Update(float iDeltaTime, GameScene iScene, SurvivalRuleset iRules, ref bool oExecuting)
		{
			this.mDelay -= iDeltaTime;
			if (this.mDelay <= 0f && !this.mHasExecuted)
			{
				for (int i = 0; i < this.mActions.Count; i++)
				{
					this.mActions[i].OnTrigger(null);
				}
				this.mHasExecuted = true;
			}
			for (int j = 0; j < this.mActions.Count; j++)
			{
				this.mActions[j].Update(iDeltaTime);
				if (this.mDelay > 0f || !this.mActions[j].HasFinishedExecuting())
				{
					oExecuting = true;
				}
			}
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001FC58 File Offset: 0x0001DE58
		public void Read(GameScene iGameScene, XmlNode iNode)
		{
			foreach (object obj in iNode.Attributes)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)obj;
				if (xmlAttribute.Name.Equals("area", StringComparison.OrdinalIgnoreCase))
				{
					if (!xmlAttribute.Value.Equals("any", StringComparison.OrdinalIgnoreCase))
					{
						this.mArea = xmlAttribute.Value;
					}
				}
				else if (xmlAttribute.Name.Equals("delay", StringComparison.OrdinalIgnoreCase))
				{
					this.mDelay = (this.mInitalDelay = float.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			Action[][] array = Trigger.ReadActions(iGameScene, null, iNode);
			if (array.Length > 1)
			{
				throw new Exception("Can't use RANDOM in survivalmode, you're making the leaderboards unbalanced!");
			}
			this.mActions = new List<Action>(array[0].Length);
			for (int i = 0; i < array[0].Length; i++)
			{
				this.mActions.Add(array[0][i]);
			}
		}

		// Token: 0x04000418 RID: 1048
		private List<Action> mActions;

		// Token: 0x04000419 RID: 1049
		private bool mHasExecuted;

		// Token: 0x0400041A RID: 1050
		private float mDelay;

		// Token: 0x0400041B RID: 1051
		private float mInitalDelay;

		// Token: 0x0400041C RID: 1052
		private string mArea;

		// Token: 0x0400041D RID: 1053
		private float mSpawnedHitPoins;

		// Token: 0x0400041E RID: 1054
		private GameScene mGameScene;

		// Token: 0x0400041F RID: 1055
		private Wave mWave;
	}
}
