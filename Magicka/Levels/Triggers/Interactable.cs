using System;
using System.Collections.Generic;
using System.Xml;
using Magicka.GameLogic.Entities;
using Magicka.Levels.Triggers.Actions;
using Magicka.Levels.Triggers.Conditions;
using Magicka.Network;

namespace Magicka.Levels.Triggers
{
	// Token: 0x02000011 RID: 17
	public class Interactable : Trigger
	{
		// Token: 0x06000065 RID: 101 RVA: 0x000056FC File Offset: 0x000038FC
		public Interactable(XmlNode iNode, GameScene iScene) : base(iScene)
		{
			this.mAnimHighlightNames = new List<string>(1);
			this.mPhysHighlightNames = new List<string>(1);
			for (int i = 0; i < iNode.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iNode.Attributes[i];
				if (xmlAttribute.Name.Equals("locator", StringComparison.OrdinalIgnoreCase))
				{
					this.mIDString = xmlAttribute.Value.ToLowerInvariant();
					this.mID = this.mIDString.GetHashCodeCustom();
				}
				else if (xmlAttribute.Name.Equals("type", StringComparison.OrdinalIgnoreCase))
				{
					this.mInteractType = (InteractType)Enum.Parse(typeof(InteractType), xmlAttribute.Value, true);
				}
				else if (xmlAttribute.Name.Equals("enabled", StringComparison.OrdinalIgnoreCase))
				{
					this.mEnabled = bool.Parse(xmlAttribute.Value);
				}
				else if (xmlAttribute.Name.Equals("animhighlight", StringComparison.OrdinalIgnoreCase))
				{
					string[] array = xmlAttribute.Value.ToLowerInvariant().Replace(" ", "").Split(new char[]
					{
						','
					});
					for (int j = 0; j < array.Length; j++)
					{
						this.mAnimHighlightNames.Add(array[j]);
					}
				}
				else
				{
					if (!xmlAttribute.Name.Equals("physhighlight", StringComparison.OrdinalIgnoreCase))
					{
						throw new Exception(string.Concat(new string[]
						{
							"Unexpected attribute \"",
							xmlAttribute.Name,
							"\" in \"",
							iNode.Name,
							"\"!"
						}));
					}
					string[] array2 = xmlAttribute.Value.ToLowerInvariant().Replace(" ", "").Split(new char[]
					{
						','
					});
					for (int k = 0; k < array2.Length; k++)
					{
						this.mPhysHighlightNames.Add(array2[k]);
					}
				}
			}
			this.mAnimHighlightIDs = new int[this.mAnimHighlightNames.Count][];
			for (int l = 0; l < this.mAnimHighlightNames.Count; l++)
			{
				string[] array3 = this.mAnimHighlightNames[l].Split(new char[]
				{
					'/'
				});
				this.mAnimHighlightIDs[l] = new int[array3.Length];
				for (int m = 0; m < array3.Length; m++)
				{
					this.mAnimHighlightIDs[l][m] = array3[m].GetHashCodeCustom();
				}
			}
			this.mPhysHighlightIDs = new int[this.mPhysHighlightNames.Count];
			for (int n = 0; n < this.mPhysHighlightNames.Count; n++)
			{
				this.mPhysHighlightIDs[n] = this.mPhysHighlightNames[n].GetHashCodeCustom();
			}
			this.mConditions = new Condition[0][];
			this.mAutoRun = false;
			this.mActions = Trigger.ReadActions(iScene, this, iNode);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005A00 File Offset: 0x00003C00
		public void Interact(Character iCharacter)
		{
			if (!this.mEnabled)
			{
				return;
			}
			if (NetworkManager.Instance.State == NetworkState.Client)
			{
				TriggerRequestMessage triggerRequestMessage = default(TriggerRequestMessage);
				triggerRequestMessage.Handle = ((iCharacter != null) ? iCharacter.Handle : ushort.MaxValue);
				triggerRequestMessage.Id = this.mID;
				triggerRequestMessage.Scene = this.mGameScene.ID;
				NetworkManager.Instance.Interface.SendMessage<TriggerRequestMessage>(ref triggerRequestMessage, 0);
				return;
			}
			int num = Trigger.sRandom.Next(this.mActions.Length);
			Action[] array = this.mActions[num];
			if (NetworkManager.Instance.State == NetworkState.Server)
			{
				TriggerActionMessage triggerActionMessage = default(TriggerActionMessage);
				triggerActionMessage.ActionType = TriggerActionType.TriggerExecute;
				triggerActionMessage.Id = this.mID;
				triggerActionMessage.Scene = this.mGameScene.ID;
				triggerActionMessage.Arg = num;
				triggerActionMessage.Handle = ((iCharacter != null) ? iCharacter.Handle : ushort.MaxValue);
				NetworkManager.Instance.Interface.SendMessage<TriggerActionMessage>(ref triggerActionMessage);
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnTrigger(iCharacter);
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005B18 File Offset: 0x00003D18
		public void Highlight()
		{
			for (int i = 0; i < this.mAnimHighlightIDs.Length; i++)
			{
				int[] array = this.mAnimHighlightIDs[i];
				AnimatedLevelPart animatedLevelPart = this.mGameScene.LevelModel.GetAnimatedLevelPart(array[0]);
				for (int j = 1; j < array.Length; j++)
				{
					animatedLevelPart = animatedLevelPart.GetChild(array[j]);
				}
				animatedLevelPart.Highlight(0f);
			}
			for (int k = 0; k < this.mPhysHighlightIDs.Length; k++)
			{
				PhysicsEntity physicsEntity = Entity.GetByID(this.mPhysHighlightIDs[k]) as PhysicsEntity;
				if (physicsEntity != null)
				{
					physicsEntity.Highlight(0f);
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00005BB4 File Offset: 0x00003DB4
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00005BBC File Offset: 0x00003DBC
		public bool Enabled
		{
			get
			{
				return this.mEnabled;
			}
			set
			{
				this.mEnabled = value;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00005BC5 File Offset: 0x00003DC5
		public InteractType InteractType
		{
			get
			{
				return this.mInteractType;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00005BD0 File Offset: 0x00003DD0
		public Locator Locator
		{
			get
			{
				Locator result;
				this.mGameScene.GetLocator(this.mID, out result);
				return result;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005BF1 File Offset: 0x00003DF1
		public override Trigger.State GetState()
		{
			return new Interactable.State(this);
		}

		// Token: 0x04000068 RID: 104
		private InteractType mInteractType;

		// Token: 0x04000069 RID: 105
		private bool mEnabled = true;

		// Token: 0x0400006A RID: 106
		private List<string> mAnimHighlightNames;

		// Token: 0x0400006B RID: 107
		private int[][] mAnimHighlightIDs;

		// Token: 0x0400006C RID: 108
		private List<string> mPhysHighlightNames;

		// Token: 0x0400006D RID: 109
		private int[] mPhysHighlightIDs;

		// Token: 0x02000012 RID: 18
		public new class State : Trigger.State
		{
			// Token: 0x0600006D RID: 109 RVA: 0x00005BF9 File Offset: 0x00003DF9
			public State(Interactable iInteractable) : base(iInteractable)
			{
			}

			// Token: 0x0600006E RID: 110 RVA: 0x00005C02 File Offset: 0x00003E02
			public override void UpdateState()
			{
				base.UpdateState();
				this.mEnabled = (this.mTrigger as Interactable).mEnabled;
			}

			// Token: 0x0600006F RID: 111 RVA: 0x00005C20 File Offset: 0x00003E20
			public override void ApplyState()
			{
				base.ApplyState();
				(this.mTrigger as Interactable).mEnabled = this.mEnabled;
			}

			// Token: 0x0400006E RID: 110
			private bool mEnabled;
		}
	}
}
