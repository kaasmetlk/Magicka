using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200056D RID: 1389
	internal class Dialog
	{
		// Token: 0x06002963 RID: 10595 RVA: 0x00144BCC File Offset: 0x00142DCC
		public Dialog(XmlNode iInput)
		{
			InteractType iDefaultIconText = InteractType.Talk;
			List<Interact> list = new List<Interact>();
			for (int i = 0; i < iInput.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = iInput.Attributes[i];
				if (xmlAttribute.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
				{
					this.mID = xmlAttribute.Value.ToLowerInvariant().GetHashCodeCustom();
				}
				else if (xmlAttribute.Name.Equals("iconText", StringComparison.OrdinalIgnoreCase))
				{
					iDefaultIconText = (InteractType)Enum.Parse(typeof(InteractType), xmlAttribute.Value, true);
				}
				else
				{
					if (!xmlAttribute.Name.Equals("repeatFrom", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					this.mRepeatFrom = int.Parse(xmlAttribute.Value);
				}
			}
			for (int j = 0; j < iInput.ChildNodes.Count; j++)
			{
				XmlNode xmlNode = iInput.ChildNodes[j];
				if (!(xmlNode is XmlComment))
				{
					if (!xmlNode.Name.Equals("Interact", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					list.Add(new Interact(xmlNode, iDefaultIconText, this));
				}
			}
			this.mMessages = list.ToArray();
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x00144D04 File Offset: 0x00142F04
		public void Initialize(Scene iScene)
		{
			this.mScene = iScene;
			for (int i = 0; i < this.mMessages.Length; i++)
			{
				this.mMessages[i].Initialize();
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06002965 RID: 10597 RVA: 0x00144D38 File Offset: 0x00142F38
		public InteractType IconText
		{
			get
			{
				if (this.mNext < this.mMessages.Length)
				{
					return this.mMessages[this.mNext].IconText;
				}
				return InteractType.Talk;
			}
		}

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06002966 RID: 10598 RVA: 0x00144D5E File Offset: 0x00142F5E
		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06002967 RID: 10599 RVA: 0x00144D66 File Offset: 0x00142F66
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
		}

		// Token: 0x06002968 RID: 10600 RVA: 0x00144D70 File Offset: 0x00142F70
		public Interact Pop()
		{
			if (this.mNext < this.mMessages.Length)
			{
				Interact interact = this.mMessages[this.mNext];
				if (!interact.RepeatInfinitly && interact.Repetitions <= 1)
				{
					this.mNext++;
					this.mInteracted = Math.Min(this.mInteracted + 1, this.mMessages.Length);
				}
				if (this.mNext >= this.mMessages.Length && this.mRepeatFrom >= 0)
				{
					this.mNext = this.mRepeatFrom;
					for (int i = 0; i < this.mMessages.Length; i++)
					{
						this.mMessages[i].Reset();
					}
				}
				return interact;
			}
			return null;
		}

		// Token: 0x06002969 RID: 10601 RVA: 0x00144E1F File Offset: 0x0014301F
		public Interact Peek()
		{
			if (this.mNext < this.mMessages.Length)
			{
				return this.mMessages[this.mNext];
			}
			return null;
		}

		// Token: 0x0600296A RID: 10602 RVA: 0x00144E40 File Offset: 0x00143040
		internal void Reset()
		{
			this.mNext = 0;
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x0600296B RID: 10603 RVA: 0x00144E49 File Offset: 0x00143049
		public int Next
		{
			get
			{
				return this.mNext;
			}
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x0600296C RID: 10604 RVA: 0x00144E51 File Offset: 0x00143051
		public int InteractedMessages
		{
			get
			{
				return this.mInteracted;
			}
		}

		// Token: 0x04002CC3 RID: 11459
		private int mRepeatFrom;

		// Token: 0x04002CC4 RID: 11460
		private int mNext;

		// Token: 0x04002CC5 RID: 11461
		private int mInteracted;

		// Token: 0x04002CC6 RID: 11462
		private Interact[] mMessages;

		// Token: 0x04002CC7 RID: 11463
		private int mID;

		// Token: 0x04002CC8 RID: 11464
		private Scene mScene;

		// Token: 0x0200056E RID: 1390
		public class State
		{
			// Token: 0x0600296D RID: 10605 RVA: 0x00144E59 File Offset: 0x00143059
			public State(Dialog iOwner)
			{
				this.mOwner = iOwner;
				this.mMessages = new Interact.State[this.mOwner.mMessages.Length];
				this.UpdateState();
			}

			// Token: 0x0600296E RID: 10606 RVA: 0x00144E88 File Offset: 0x00143088
			public void UpdateState()
			{
				this.mNext = this.mOwner.mNext;
				this.mFinished = this.mOwner.mInteracted;
				for (int i = 0; i < this.mOwner.mMessages.Length; i++)
				{
					this.mMessages[i] = new Interact.State(this.mOwner.mMessages[i]);
				}
			}

			// Token: 0x0600296F RID: 10607 RVA: 0x00144EF4 File Offset: 0x001430F4
			public void ApplayState()
			{
				this.mOwner.mNext = this.mNext;
				this.mOwner.mInteracted = this.mFinished;
				for (int i = 0; i < this.mMessages.Length; i++)
				{
					this.mMessages[i].ApplyState();
				}
			}

			// Token: 0x170009C3 RID: 2499
			// (get) Token: 0x06002970 RID: 10608 RVA: 0x00144F47 File Offset: 0x00143147
			internal Dialog Owner
			{
				get
				{
					return this.mOwner;
				}
			}

			// Token: 0x06002971 RID: 10609 RVA: 0x00144F50 File Offset: 0x00143150
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mNext);
				iWriter.Write(this.mFinished);
				for (int i = 0; i < this.mMessages.Length; i++)
				{
					this.mMessages[i].Write(iWriter);
				}
			}

			// Token: 0x06002972 RID: 10610 RVA: 0x00144F9C File Offset: 0x0014319C
			internal void Read(BinaryReader iReader)
			{
				this.mNext = iReader.ReadInt32();
				this.mFinished = iReader.ReadInt32();
				for (int i = 0; i < this.mMessages.Length; i++)
				{
					this.mMessages[i].Read(iReader);
				}
			}

			// Token: 0x04002CC9 RID: 11465
			private int mNext;

			// Token: 0x04002CCA RID: 11466
			private int mFinished;

			// Token: 0x04002CCB RID: 11467
			private Interact.State[] mMessages;

			// Token: 0x04002CCC RID: 11468
			private Dialog mOwner;
		}
	}
}
