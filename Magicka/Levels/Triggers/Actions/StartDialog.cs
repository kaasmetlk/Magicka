using System;
using System.Collections.Generic;
using System.Globalization;
using Magicka.GameLogic.Entities;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x02000284 RID: 644
	public class StartDialog : Action
	{
		// Token: 0x06001309 RID: 4873 RVA: 0x000759B4 File Offset: 0x00073BB4
		public StartDialog(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x000759E1 File Offset: 0x00073BE1
		public override void OnTrigger(Character iArg)
		{
			base.OnTrigger(iArg);
			this.mArgs.Enqueue(iArg);
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x000759F8 File Offset: 0x00073BF8
		protected override void Execute()
		{
			Character character = (this.mArgs.Count > 0) ? (this.mArgs.Dequeue() as Character) : null;
			if (this.mIs2DPos)
			{
				DialogManager.Instance.StartDialog(this.mID, new Vector2(this.mPosition.X, this.mPosition.Y), null);
				return;
			}
			if (this.mIs3DPos)
			{
				DialogManager.Instance.StartDialog(this.mID, this.mPosition, null);
				return;
			}
			Vector3 iWorldPosition = default(Vector3);
			Character character2;
			if (this.mOwner == StartDialog.INTERACTORHASH)
			{
				character2 = character;
			}
			else
			{
				character2 = (Entity.GetByID(this.mOwner) as Character);
				if (character2 == null)
				{
					Matrix matrix;
					base.GameScene.GetLocator(this.mOwner, out matrix);
					iWorldPosition = matrix.Translation;
				}
			}
			if (character2 == null)
			{
				DialogManager.Instance.StartDialog(this.mID, iWorldPosition, null);
				return;
			}
			if (character2 is Avatar)
			{
				DialogManager.Instance.StartDialog(this.mID, character2, (character2 as Avatar).Player.Controller);
				return;
			}
			DialogManager.Instance.StartDialog(this.mID, character2, null);
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x00075B18 File Offset: 0x00073D18
		public override void QuickExecute()
		{
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x0600130D RID: 4877 RVA: 0x00075B1A File Offset: 0x00073D1A
		// (set) Token: 0x0600130E RID: 4878 RVA: 0x00075B22 File Offset: 0x00073D22
		public string Dialog
		{
			get
			{
				return this.mDialog;
			}
			set
			{
				this.mDialog = value;
				this.mID = this.mDialog.GetHashCodeCustom();
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x0600130F RID: 4879 RVA: 0x00075B3C File Offset: 0x00073D3C
		// (set) Token: 0x06001310 RID: 4880 RVA: 0x00075B44 File Offset: 0x00073D44
		public bool ForceOnScreen
		{
			get
			{
				return this.mForceOnScreen;
			}
			set
			{
				this.mForceOnScreen = value;
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x00075B4D File Offset: 0x00073D4D
		// (set) Token: 0x06001312 RID: 4882 RVA: 0x00075B58 File Offset: 0x00073D58
		public string Position
		{
			get
			{
				return this.mPositionStr;
			}
			set
			{
				this.mPositionStr = value;
				string[] array = value.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					this.mPosition.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
					this.mPosition.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
					this.mIs2DPos = true;
					this.mIs3DPos = false;
					return;
				}
				if (array.Length == 3)
				{
					this.mPosition.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
					this.mPosition.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
					this.mPosition.Y = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
					this.mIs2DPos = false;
					this.mIs3DPos = true;
					return;
				}
				this.mOwner = this.mPositionStr.GetHashCodeCustom();
				this.mIs2DPos = false;
				this.mIs3DPos = false;
			}
		}

		// Token: 0x040014C9 RID: 5321
		public static readonly int INTERACTORHASH = "interactor".GetHashCodeCustom();

		// Token: 0x040014CA RID: 5322
		private string mDialog;

		// Token: 0x040014CB RID: 5323
		private int mID;

		// Token: 0x040014CC RID: 5324
		private string mPositionStr = "interactor";

		// Token: 0x040014CD RID: 5325
		private int mOwner = StartDialog.INTERACTORHASH;

		// Token: 0x040014CE RID: 5326
		private Vector3 mPosition;

		// Token: 0x040014CF RID: 5327
		private bool mForceOnScreen;

		// Token: 0x040014D0 RID: 5328
		private bool mIs3DPos;

		// Token: 0x040014D1 RID: 5329
		private bool mIs2DPos;

		// Token: 0x040014D2 RID: 5330
		private Queue<object> mArgs = new Queue<object>(10);
	}
}
