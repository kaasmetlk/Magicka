using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PolygonHead;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200056B RID: 1387
	internal class DialogCollection : Dictionary<int, Dialog>
	{
		// Token: 0x06002958 RID: 10584 RVA: 0x00144630 File Offset: 0x00142830
		public DialogCollection(string iFileName)
		{
			this.LoadDialogs(iFileName);
			string iFileName2 = Path.Combine("content", "Levels/Dialog_Fairy.xml");
			this.LoadDialogs(iFileName2);
			foreach (KeyValuePair<int, Dialog> keyValuePair in this)
			{
				this.mFinishedDialogs[keyValuePair.Key] = false;
			}
		}

		// Token: 0x06002959 RID: 10585 RVA: 0x001446BC File Offset: 0x001428BC
		private void LoadDialogs(string iFileName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(iFileName);
			XmlNode xmlNode = null;
			for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
			{
				XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
				if (xmlNode2.Name.Equals("Dialogs", StringComparison.OrdinalIgnoreCase))
				{
					xmlNode = xmlNode2;
					break;
				}
			}
			if (xmlNode == null)
			{
				throw new Exception("No dialogs found!");
			}
			for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
			{
				XmlNode xmlNode3 = xmlNode.ChildNodes[j];
				if (!(xmlNode3 is XmlComment))
				{
					if (!xmlNode3.Name.Equals("Dialog", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotImplementedException();
					}
					Dialog dialog = new Dialog(xmlNode3);
					base.Add(dialog.ID, dialog);
				}
			}
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x00144788 File Offset: 0x00142988
		public void Initialize(Scene iScene)
		{
			foreach (Dialog dialog in base.Values)
			{
				dialog.Initialize(iScene);
			}
		}

		// Token: 0x0600295B RID: 10587 RVA: 0x001447DC File Offset: 0x001429DC
		public void DialogFinished(int iId)
		{
			this.mFinishedDialogs[iId] = true;
		}

		// Token: 0x0600295C RID: 10588 RVA: 0x001447EB File Offset: 0x001429EB
		public bool IsDialogDone(int iId)
		{
			return this.mFinishedDialogs[iId];
		}

		// Token: 0x0600295D RID: 10589 RVA: 0x001447FC File Offset: 0x001429FC
		public void Reset()
		{
			foreach (KeyValuePair<int, Dialog> keyValuePair in this)
			{
				keyValuePair.Value.Reset();
				this.mFinishedDialogs[keyValuePair.Key] = false;
			}
		}

		// Token: 0x04002CBF RID: 11455
		private Dictionary<int, bool> mFinishedDialogs = new Dictionary<int, bool>();

		// Token: 0x0200056C RID: 1388
		public class State
		{
			// Token: 0x0600295E RID: 10590 RVA: 0x00144864 File Offset: 0x00142A64
			public State(DialogCollection iOwner)
			{
				if (iOwner == null)
				{
					throw new ArgumentNullException("iOwner");
				}
				this.mOwner = iOwner;
				foreach (KeyValuePair<int, Dialog> keyValuePair in this.mOwner)
				{
					Dialog.State item = new Dialog.State(keyValuePair.Value);
					this.mDialogStates.Add(item);
				}
				this.UpdateState();
			}

			// Token: 0x0600295F RID: 10591 RVA: 0x00144900 File Offset: 0x00142B00
			public void UpdateState()
			{
				this.mFinishedDialogs.Clear();
				foreach (KeyValuePair<int, bool> keyValuePair in this.mOwner.mFinishedDialogs)
				{
					this.mFinishedDialogs.Add(keyValuePair.Key, keyValuePair.Value);
				}
				for (int i = 0; i < this.mDialogStates.Count; i++)
				{
					this.mDialogStates[i].UpdateState();
				}
			}

			// Token: 0x06002960 RID: 10592 RVA: 0x0014499C File Offset: 0x00142B9C
			public void ApplyState()
			{
				this.mOwner.mFinishedDialogs.Clear();
				foreach (KeyValuePair<int, bool> keyValuePair in this.mFinishedDialogs)
				{
					this.mOwner.mFinishedDialogs.Add(keyValuePair.Key, keyValuePair.Value);
				}
				for (int i = 0; i < this.mDialogStates.Count; i++)
				{
					this.mDialogStates[i].ApplayState();
				}
			}

			// Token: 0x06002961 RID: 10593 RVA: 0x00144A40 File Offset: 0x00142C40
			internal void Write(BinaryWriter iWriter)
			{
				iWriter.Write(this.mFinishedDialogs.Count);
				foreach (KeyValuePair<int, bool> keyValuePair in this.mFinishedDialogs)
				{
					iWriter.Write(keyValuePair.Key);
					iWriter.Write(keyValuePair.Value);
				}
				iWriter.Write(this.mDialogStates.Count);
				foreach (Dialog.State state in this.mDialogStates)
				{
					iWriter.Write(state.Owner.ID);
					state.Write(iWriter);
				}
			}

			// Token: 0x06002962 RID: 10594 RVA: 0x00144B1C File Offset: 0x00142D1C
			internal void Read(BinaryReader iReader)
			{
				this.mFinishedDialogs.Clear();
				int num = iReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int key = iReader.ReadInt32();
					bool value = iReader.ReadBoolean();
					this.mFinishedDialogs[key] = value;
				}
				num = iReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					int num2 = iReader.ReadInt32();
					for (int k = 0; k < this.mDialogStates.Count; k++)
					{
						if (num2 == this.mDialogStates[k].Owner.ID)
						{
							this.mDialogStates[k].Read(iReader);
						}
					}
				}
			}

			// Token: 0x04002CC0 RID: 11456
			private Dictionary<int, bool> mFinishedDialogs = new Dictionary<int, bool>();

			// Token: 0x04002CC1 RID: 11457
			private List<Dialog.State> mDialogStates = new List<Dialog.State>();

			// Token: 0x04002CC2 RID: 11458
			private DialogCollection mOwner;
		}
	}
}
