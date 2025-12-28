using System;
using Magicka.Audio;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020000A9 RID: 169
	public class PlayMusic : Action
	{
		// Token: 0x060004CD RID: 1229 RVA: 0x0001BD1F File Offset: 0x00019F1F
		public PlayMusic(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001BD30 File Offset: 0x00019F30
		protected override void Execute()
		{
			base.GameScene.PlayState.PlayMusic(this.mSoundBank, this.mCueID, this.mFocusValue);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001BD54 File Offset: 0x00019F54
		public override void QuickExecute()
		{
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0001BD56 File Offset: 0x00019F56
		// (set) Token: 0x060004D1 RID: 1233 RVA: 0x0001BD60 File Offset: 0x00019F60
		public string Cue
		{
			get
			{
				return this.mCue;
			}
			set
			{
				this.mCue = value;
				string[] array = this.mCue.Split(new char[]
				{
					'/'
				});
				if (array != null && array.Length > 1)
				{
					this.mSoundBank = (Banks)Enum.Parse(typeof(Banks), array[0], true);
					this.mCueID = array[1].GetHashCodeCustom();
					return;
				}
				this.mCueID = this.mCue.GetHashCodeCustom();
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060004D2 RID: 1234 RVA: 0x0001BDD4 File Offset: 0x00019FD4
		// (set) Token: 0x060004D3 RID: 1235 RVA: 0x0001BDE6 File Offset: 0x00019FE6
		public float FocusValue
		{
			get
			{
				return this.mFocusValue.GetValueOrDefault(1f);
			}
			set
			{
				this.mFocusValue = new float?(value);
			}
		}

		// Token: 0x0400037C RID: 892
		private Banks mSoundBank = Banks.Music;

		// Token: 0x0400037D RID: 893
		private string mCue;

		// Token: 0x0400037E RID: 894
		private int mCueID;

		// Token: 0x0400037F RID: 895
		private float? mFocusValue;
	}
}
