using System;
using Magicka.Audio;
using Microsoft.Xna.Framework.Audio;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000AC RID: 172
	public class PlaySound : Action
	{
		// Token: 0x060004FF RID: 1279 RVA: 0x0001C485 File Offset: 0x0001A685
		public PlaySound(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001C490 File Offset: 0x0001A690
		protected override void Execute()
		{
			if (!string.IsNullOrEmpty(this.mIDStr))
			{
				if (string.IsNullOrEmpty(this.mArea))
				{
					base.GameScene.PlayAmbientSound(this.mID, this.mBank, this.mCueID, this.mVolume);
					return;
				}
				base.GameScene.PlayAmbientSound(this.mID, this.mBank, this.mCueID, this.mVolume, this.mAreaID, this.mRadius, this.mApply3D);
				return;
			}
			else
			{
				if (string.IsNullOrEmpty(this.mArea))
				{
					Cue cue = AudioManager.Instance.GetCue(this.mBank, this.mCueID);
					cue.SetVariable(PlaySound.VOLUME_VAR_NAME, this.mVolume);
					cue.Play();
					return;
				}
				AudioLocator audioLocator = new AudioLocator(0, this.mBank, this.mCueID, this.mVolume, this.mAreaID, this.mRadius, this.mApply3D);
				audioLocator.Play();
				audioLocator.Update(this.mScene);
				return;
			}
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001C58D File Offset: 0x0001A78D
		public override void QuickExecute()
		{
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x0001C58F File Offset: 0x0001A78F
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x0001C597 File Offset: 0x0001A797
		public string ID
		{
			get
			{
				return this.mIDStr;
			}
			set
			{
				this.mIDStr = value;
				this.mID = this.mIDStr.GetHashCodeCustom();
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x0001C5B1 File Offset: 0x0001A7B1
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x0001C5BC File Offset: 0x0001A7BC
		public string Cue
		{
			get
			{
				return this.mCue;
			}
			set
			{
				string[] array = value.Split(new char[]
				{
					'/'
				});
				if (array.Length == 1)
				{
					this.mCue = array[0];
					this.mBank = Banks.Ambience;
				}
				else
				{
					if (array.Length != 2)
					{
						throw new Exception("Invalid Syntax!");
					}
					this.mCue = array[1];
					this.mBank = (Banks)Enum.Parse(typeof(Banks), array[0], true);
				}
				this.mCueID = this.mCue.GetHashCodeCustom();
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x0001C640 File Offset: 0x0001A840
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x0001C648 File Offset: 0x0001A848
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaID = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x0001C662 File Offset: 0x0001A862
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x0001C66A File Offset: 0x0001A86A
		public float Volume
		{
			get
			{
				return this.mVolume;
			}
			set
			{
				this.mVolume = value;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x0001C673 File Offset: 0x0001A873
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x0001C67B File Offset: 0x0001A87B
		public float Radius
		{
			get
			{
				return this.mRadius;
			}
			set
			{
				this.mRadius = value;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001C684 File Offset: 0x0001A884
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x0001C68C File Offset: 0x0001A88C
		public bool Apply3D
		{
			get
			{
				return this.mApply3D;
			}
			set
			{
				this.mApply3D = value;
			}
		}

		// Token: 0x0400039F RID: 927
		private string mIDStr;

		// Token: 0x040003A0 RID: 928
		private int mID;

		// Token: 0x040003A1 RID: 929
		private Banks mBank;

		// Token: 0x040003A2 RID: 930
		private string mCue;

		// Token: 0x040003A3 RID: 931
		private int mCueID;

		// Token: 0x040003A4 RID: 932
		private string mArea;

		// Token: 0x040003A5 RID: 933
		private int mAreaID;

		// Token: 0x040003A6 RID: 934
		private float mVolume;

		// Token: 0x040003A7 RID: 935
		private float mRadius;

		// Token: 0x040003A8 RID: 936
		private bool mApply3D;

		// Token: 0x040003A9 RID: 937
		private static readonly string VOLUME_VAR_NAME = "Volume";
	}
}
