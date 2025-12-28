using System;
using System.Xml;
using JigLibX.Geometry;
using Magicka.GameLogic.Entities;
using Microsoft.Xna.Framework;

namespace Magicka.Levels.Triggers.Actions
{
	// Token: 0x020002CA RID: 714
	public class SetPosition : Action
	{
		// Token: 0x060015B9 RID: 5561 RVA: 0x0008ACC8 File Offset: 0x00088EC8
		public SetPosition(Trigger iTrigger, GameScene iScene, XmlNode iNode) : base(iTrigger, iScene)
		{
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x0008ACD2 File Offset: 0x00088ED2
		public override void Initialize()
		{
			base.Initialize();
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0008ACDC File Offset: 0x00088EDC
		protected override void Execute()
		{
			Entity entity = null;
			int num = -1;
			if (this.mID.Equals(SetPosition.PLAYER1, StringComparison.OrdinalIgnoreCase))
			{
				num = 0;
			}
			else if (this.mID.Equals(SetPosition.PLAYER2, StringComparison.OrdinalIgnoreCase))
			{
				num = 1;
			}
			else if (this.mID.Equals(SetPosition.PLAYER3, StringComparison.OrdinalIgnoreCase))
			{
				num = 2;
			}
			else if (this.mID.Equals(SetPosition.PLAYER4, StringComparison.OrdinalIgnoreCase))
			{
				num = 3;
			}
			if (num != -1)
			{
				if (Game.Instance.Players[num].Playing && Game.Instance.Players[num].Avatar != null)
				{
					entity = Game.Instance.Players[num].Avatar;
				}
			}
			else
			{
				entity = Entity.GetByID(this.mIDHash);
			}
			if (entity == null)
			{
				return;
			}
			Matrix orientation;
			this.mScene.GetLocator(this.mAreaHash, out orientation);
			Vector3 vector = orientation.Translation;
			Vector3 forward = orientation.Forward;
			orientation.Translation = default(Vector3);
			Segment seg = default(Segment);
			seg.Origin = vector;
			seg.Origin.Y = seg.Origin.Y + 1f;
			seg.Delta.Y = -5f;
			float num2;
			Vector3 vector2;
			Vector3 vector3;
			if (base.GameScene.LevelModel.CollisionSkin.SegmentIntersect(out num2, out vector2, out vector3, seg))
			{
				vector = vector2;
				Character character = entity as Character;
				if (character != null)
				{
					vector.Y -= character.HeightOffset;
					if (this.mFacingDirection)
					{
						character.CharacterBody.DesiredDirection = forward;
					}
					else
					{
						orientation = entity.Body.Orientation;
						orientation.Translation = default(Vector3);
					}
				}
			}
			entity.Body.Velocity = default(Vector3);
			entity.Body.MoveTo(vector, orientation);
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x0008AE9D File Offset: 0x0008909D
		public override void QuickExecute()
		{
			this.Execute();
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x060015BD RID: 5565 RVA: 0x0008AEA5 File Offset: 0x000890A5
		// (set) Token: 0x060015BE RID: 5566 RVA: 0x0008AEAD File Offset: 0x000890AD
		public string ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				this.mIDHash = this.mID.GetHashCodeCustom();
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x0008AEC7 File Offset: 0x000890C7
		// (set) Token: 0x060015C0 RID: 5568 RVA: 0x0008AECF File Offset: 0x000890CF
		public bool FacingDirection
		{
			get
			{
				return this.mFacingDirection;
			}
			set
			{
				this.mFacingDirection = value;
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x0008AED8 File Offset: 0x000890D8
		// (set) Token: 0x060015C2 RID: 5570 RVA: 0x0008AEE0 File Offset: 0x000890E0
		public string Area
		{
			get
			{
				return this.mArea;
			}
			set
			{
				this.mArea = value;
				this.mAreaHash = this.mArea.GetHashCodeCustom();
			}
		}

		// Token: 0x0400170A RID: 5898
		public static readonly string PLAYER1 = "player1";

		// Token: 0x0400170B RID: 5899
		public static readonly string PLAYER2 = "player2";

		// Token: 0x0400170C RID: 5900
		public static readonly string PLAYER3 = "player3";

		// Token: 0x0400170D RID: 5901
		public static readonly string PLAYER4 = "player4";

		// Token: 0x0400170E RID: 5902
		protected string mID;

		// Token: 0x0400170F RID: 5903
		protected int mIDHash;

		// Token: 0x04001710 RID: 5904
		protected bool mFacingDirection;

		// Token: 0x04001711 RID: 5905
		protected string mArea;

		// Token: 0x04001712 RID: 5906
		protected int mAreaHash;
	}
}
