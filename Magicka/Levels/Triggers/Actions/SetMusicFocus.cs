using System;
using Magicka.Audio;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x0200001A RID: 26
	internal class SetMusicFocus : Action
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00006CD0 File Offset: 0x00004ED0
		public SetMusicFocus(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006CE8 File Offset: 0x00004EE8
		protected override void Execute()
		{
			Matrix matrix;
			base.GameScene.GetLocator(this.mFocusJointID, out matrix);
			AudioManager.Instance.SetMusicFocus(matrix.Translation, this.mFocusRadius, this.mDefaultValue);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006D25 File Offset: 0x00004F25
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00006D2D File Offset: 0x00004F2D
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00006D35 File Offset: 0x00004F35
		public string ID
		{
			get
			{
				return this.mFocusJoint;
			}
			set
			{
				this.mFocusJoint = value;
				this.mFocusJointID = this.mFocusJoint.GetHashCodeCustom();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00006D4F File Offset: 0x00004F4F
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00006D57 File Offset: 0x00004F57
		public float Radius
		{
			get
			{
				return this.mFocusRadius;
			}
			set
			{
				this.mFocusRadius = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00006D60 File Offset: 0x00004F60
		// (set) Token: 0x060000CB RID: 203 RVA: 0x00006D68 File Offset: 0x00004F68
		public float ClearTo
		{
			get
			{
				return this.mDefaultValue;
			}
			set
			{
				this.mDefaultValue = value;
			}
		}

		// Token: 0x04000094 RID: 148
		private string mFocusJoint;

		// Token: 0x04000095 RID: 149
		private int mFocusJointID;

		// Token: 0x04000096 RID: 150
		private float mFocusRadius;

		// Token: 0x04000097 RID: 151
		private float mDefaultValue = 1f;
	}
}
