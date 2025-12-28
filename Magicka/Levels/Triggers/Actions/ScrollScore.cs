using System;
using System.Globalization;
using Magicka.GameLogic.UI;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020001AA RID: 426
	internal class ScrollScore : Action
	{
		// Token: 0x06000CBE RID: 3262 RVA: 0x0004B6A4 File Offset: 0x000498A4
		public ScrollScore(Trigger iTrigger, GameScene iScene) : base(iTrigger, iScene)
		{
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0004B6B0 File Offset: 0x000498B0
		protected override void Execute()
		{
			Vector3 vector = default(Vector3);
			TriggerArea triggerArea;
			Matrix matrix;
			if (this.mScene.Level.CurrentScene.TryGetTriggerArea(this.mAreaID, out triggerArea))
			{
				vector = triggerArea.GetRandomLocation();
			}
			else if (this.mScene.Level.CurrentScene.TryGetLocator(this.mAreaID, out matrix))
			{
				vector = matrix.Translation;
			}
			Vector3.Add(ref vector, ref this.mOffsetV, out vector);
			Vector3 one = Vector3.One;
			DamageNotifyer.Instance.AddNumber(this.mValuef, ref vector, this.mTTL, false, ref one);
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x0004B745 File Offset: 0x00049945
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x0004B74D File Offset: 0x0004994D
		// (set) Token: 0x06000CC2 RID: 3266 RVA: 0x0004B755 File Offset: 0x00049955
		public string Value
		{
			get
			{
				return this.mValueName;
			}
			set
			{
				this.mValueName = value;
				this.mValuef = float.Parse(this.mValueName, CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06000CC3 RID: 3267 RVA: 0x0004B779 File Offset: 0x00049979
		// (set) Token: 0x06000CC4 RID: 3268 RVA: 0x0004B781 File Offset: 0x00049981
		public float TTL
		{
			get
			{
				return this.mTTL;
			}
			set
			{
				this.mTTL = value;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x0004B78A File Offset: 0x0004998A
		// (set) Token: 0x06000CC6 RID: 3270 RVA: 0x0004B792 File Offset: 0x00049992
		public string Area
		{
			get
			{
				return this.mAreaName;
			}
			set
			{
				this.mAreaName = value;
				this.mAreaID = this.mAreaName.ToLowerInvariant().GetHashCodeCustom();
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06000CC7 RID: 3271 RVA: 0x0004B7B1 File Offset: 0x000499B1
		// (set) Token: 0x06000CC8 RID: 3272 RVA: 0x0004B7BC File Offset: 0x000499BC
		public string Offset
		{
			get
			{
				return this.mOffset;
			}
			set
			{
				this.mOffset = value;
				string[] array = this.mOffset.Split(new char[]
				{
					','
				});
				if (array.Length == 3)
				{
					this.mOffsetV.X = float.Parse(array[0], CultureInfo.InvariantCulture.NumberFormat);
					this.mOffsetV.Y = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
					this.mOffsetV.Z = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
				}
			}
		}

		// Token: 0x04000B9E RID: 2974
		private string mValueName;

		// Token: 0x04000B9F RID: 2975
		private string mAreaName;

		// Token: 0x04000BA0 RID: 2976
		private int mAreaID;

		// Token: 0x04000BA1 RID: 2977
		private string mOffset;

		// Token: 0x04000BA2 RID: 2978
		private Vector3 mOffsetV;

		// Token: 0x04000BA3 RID: 2979
		private float mTTL;

		// Token: 0x04000BA4 RID: 2980
		private float mValuef;
	}
}
