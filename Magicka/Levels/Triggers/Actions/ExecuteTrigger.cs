using System;
using System.Collections.Generic;
using System.IO;
using Magicka.GameLogic.Entities;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000481 RID: 1153
	public class ExecuteTrigger : Action
	{
		// Token: 0x060022E3 RID: 8931 RVA: 0x000FB715 File Offset: 0x000F9915
		public ExecuteTrigger(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000FB72C File Offset: 0x000F992C
		public override void OnTrigger(Character iArg)
		{
			base.OnTrigger(iArg);
			this.mArgs.Enqueue(iArg);
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x000FB741 File Offset: 0x000F9941
		protected override void Execute()
		{
			base.GameScene.ExecuteTrigger(this.mTriggerId, this.mArgs.Dequeue(), false);
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x000FB760 File Offset: 0x000F9960
		public override void QuickExecute()
		{
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x060022E7 RID: 8935 RVA: 0x000FB762 File Offset: 0x000F9962
		// (set) Token: 0x060022E8 RID: 8936 RVA: 0x000FB76A File Offset: 0x000F996A
		public new string Trigger
		{
			get
			{
				return this.mTriggerName;
			}
			set
			{
				this.mTriggerName = value;
				this.mTriggerId = this.mTriggerName.GetHashCodeCustom();
			}
		}

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x060022E9 RID: 8937 RVA: 0x000FB784 File Offset: 0x000F9984
		// (set) Token: 0x060022EA RID: 8938 RVA: 0x000FB791 File Offset: 0x000F9991
		protected override object Tag
		{
			get
			{
				return new Queue<Character>(this.mArgs);
			}
			set
			{
				this.mArgs = new Queue<Character>(value as Queue<Character>);
			}
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x000FB7A4 File Offset: 0x000F99A4
		protected override void WriteTag(BinaryWriter iWriter, object iTag)
		{
			Queue<Character> queue = iTag as Queue<Character>;
			iWriter.Write(queue.Count);
			foreach (Character character in queue)
			{
				if (character != null)
				{
					iWriter.Write(character.UniqueID);
				}
				else
				{
					iWriter.Write(0);
				}
			}
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000FB818 File Offset: 0x000F9A18
		protected override object ReadTag(BinaryReader iReader)
		{
			int num = iReader.ReadInt32();
			Queue<Character> queue = new Queue<Character>(num);
			for (int i = 0; i < num; i++)
			{
				int iID = iReader.ReadInt32();
				queue.Enqueue(Entity.GetByID(iID) as Character);
			}
			return queue;
		}

		// Token: 0x04002614 RID: 9748
		private string mTriggerName;

		// Token: 0x04002615 RID: 9749
		private int mTriggerId;

		// Token: 0x04002616 RID: 9750
		private Queue<Character> mArgs = new Queue<Character>(10);
	}
}
