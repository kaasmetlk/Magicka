using System;
using System.Collections.Generic;
using JigLibX.Geometry;
using JigLibX.Math;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.Spells;
using Magicka.Graphics;
using Magicka.Localization;
using Magicka.Network;
using Magicka.WebTools.Paradox.Telemetry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020005A8 RID: 1448
	public class BookOfMagick : Pickable
	{
		// Token: 0x06002B4D RID: 11085 RVA: 0x00155F34 File Offset: 0x00154134
		public static BookOfMagick GetInstance(PlayState iPlayState)
		{
			BookOfMagick bookOfMagick = BookOfMagick.sCache.Dequeue();
			BookOfMagick.sCache.Enqueue(bookOfMagick);
			return bookOfMagick;
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x00155F58 File Offset: 0x00154158
		public static void InitializeCache(int iNr, PlayState iPlayState)
		{
			BookOfMagick.sCache = new Queue<BookOfMagick>(iNr);
			for (int i = 0; i < iNr; i++)
			{
				BookOfMagick.sCache.Enqueue(new BookOfMagick(iPlayState));
			}
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x00155F8C File Offset: 0x0015418C
		public BookOfMagick(PlayState iPlayState) : base(iPlayState)
		{
			if (BookOfMagick.sPickUpStrings == null)
			{
				BookOfMagick.sPickUpStrings = new string[35];
				for (int i = 1; i < BookOfMagick.sPickUpStrings.Length; i++)
				{
					if (i >= Magicka.GameLogic.Spells.Magick.NAME_LOCALIZATION.Length)
					{
						Console.WriteLine("Magick name out of range! Fix MMagick.NAME_LOCALIZATION.");
					}
					else
					{
						BookOfMagick.sPickUpStrings[i] = LanguageManager.Instance.GetString(BookOfMagick.MAGICK_PICKUP_LOC);
						BookOfMagick.sPickUpStrings[i] = BookOfMagick.sPickUpStrings[i].Replace("#1;", "[c=1,1,1]" + LanguageManager.Instance.GetString(Magicka.GameLogic.Spells.Magick.NAME_LOCALIZATION[i]) + "[/c]");
					}
				}
			}
			if (BookOfMagick.sBookModel == null)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					BookOfMagick.sBookModel = Game.Instance.Content.Load<Model>("Models/Items_Wizard/magickbook_major");
				}
			}
			base.Model = BookOfMagick.sBookModel;
			VertexElement[] vertexElements;
			lock (Game.Instance.GraphicsDevice)
			{
				vertexElements = BookOfMagick.sBookModel.Meshes[0].MeshParts[0].VertexDeclaration.GetVertexElements();
			}
			int num = -1;
			for (int j = 0; j < vertexElements.Length; j++)
			{
				if (vertexElements[j].VertexElementUsage == VertexElementUsage.Position)
				{
					num = (int)vertexElements[j].Offset;
					break;
				}
			}
			if (num < 0)
			{
				throw new Exception("No positions found");
			}
			Vector3[] array = new Vector3[BookOfMagick.sBookModel.Meshes[0].MeshParts[0].NumVertices];
			BookOfMagick.sBookModel.Meshes[0].VertexBuffer.GetData<Vector3>(num, array, BookOfMagick.sBookModel.Meshes[0].MeshParts[0].StartIndex, array.Length, BookOfMagick.sBookModel.Meshes[0].MeshParts[0].VertexStride);
			this.mBoundingBox = BoundingBox.CreateFromPoints(array);
			Vector3 sideLengths;
			Vector3.Subtract(ref this.mBoundingBox.Max, ref this.mBoundingBox.Min, out sideLengths);
			sideLengths.Y *= 2f;
			(this.mCollision.GetPrimitiveLocal(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveOldWorld(0) as Box).SideLengths = sideLengths;
			(this.mCollision.GetPrimitiveNewWorld(0) as Box).SideLengths = sideLengths;
			Vector3 vector = (this.mBoundingBox.Min + this.mBoundingBox.Max) * 0.5f;
			Vector3 vector2 = base.SetMass(50f);
			Transform transform = default(Transform);
			Vector3.Negate(ref vector2, out transform.Position);
			Vector3.Add(ref transform.Position, ref vector, out transform.Position);
			transform.Orientation = Matrix.Identity;
			this.mCollision.ApplyLocalTransform(transform);
			this.mBody.Immovable = true;
			this.mPickable = true;
			this.mPickUpString = BookOfMagick.sPickUpStrings[0];
			this.mDead = false;
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x001562C4 File Offset: 0x001544C4
		public void Initialize(Vector3 iPosition, Matrix iOrientation, MagickType iMagick, bool iImmovable, Vector3 iVelocity, float iTimeout, int iUniqueID)
		{
			base.Initialize(iUniqueID);
			this.mRestingTimer = 1f;
			this.mDead = false;
			iOrientation.Translation = default(Vector3);
			this.mBody.MoveTo(iPosition, iOrientation);
			this.mTimeOutTimer = iTimeout;
			this.mMagick = iMagick;
			this.mPickUpString = BookOfMagick.sPickUpStrings[(int)this.mMagick];
			iOrientation.Translation = iPosition;
			EffectManager.Instance.StartEffect(BookOfMagick.GLIMMER_EFFECT, ref iOrientation, out this.mEffect);
			this.mBody.Immovable = iImmovable;
			if (!iImmovable)
			{
				this.mBody.Velocity = iVelocity;
				this.mBody.EnableBody();
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06002B51 RID: 11089 RVA: 0x00156372 File Offset: 0x00154572
		public MagickType Magick
		{
			get
			{
				return this.mMagick;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06002B52 RID: 11090 RVA: 0x0015637A File Offset: 0x0015457A
		public string PickUpString
		{
			get
			{
				return this.mPickUpString;
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06002B53 RID: 11091 RVA: 0x00156382 File Offset: 0x00154582
		public bool Resting
		{
			get
			{
				return this.mRestingTimer < 0f;
			}
		}

		// Token: 0x06002B54 RID: 11092 RVA: 0x00156394 File Offset: 0x00154594
		public override void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			base.Update(iDataChannel, iDeltaTime);
			if (this.mTimeOutTimer > 0f)
			{
				this.mTimeOutTimer -= iDeltaTime;
				Vector3 position = this.Position;
				Vector3 right = Vector3.Right;
				bool flag = EffectManager.Instance.UpdatePositionDirection(ref this.mTimeOutEffect, ref position, ref right);
				if (this.mTimeOutTimer <= 0f)
				{
					VisualEffectReference visualEffectReference;
					EffectManager.Instance.StartEffect(BookOfMagick.DISAPPEAR_EFFECT, ref position, ref right, out visualEffectReference);
					if (NetworkManager.Instance.State != NetworkState.Client)
					{
						this.Kill();
					}
				}
				else if (!flag && this.mTimeOutTimer <= 4f)
				{
					EffectManager.Instance.StartEffect(BookOfMagick.TIMEOUT_EFFECT, ref position, ref right, out this.mTimeOutEffect);
				}
			}
			if (this.mBody.IsActive)
			{
				this.mRestingTimer = 1f;
			}
			else
			{
				this.mRestingTimer -= iDeltaTime;
			}
			Matrix orientation = this.mBody.Orientation;
			orientation.Translation = this.mBody.Position;
			EffectManager.Instance.UpdateOrientation(ref this.mEffect, ref orientation);
		}

		// Token: 0x06002B55 RID: 11093 RVA: 0x001564A8 File Offset: 0x001546A8
		public void Unlock(Player iPlayer)
		{
			if (this.mPlayState.GameType == GameType.Versus)
			{
				if ((iPlayer.Team & Factions.TEAM_RED) == Factions.TEAM_RED)
				{
					Player[] players = Game.Instance.Players;
					for (int i = 0; i < players.Length; i++)
					{
						if ((players[i].Team & Factions.TEAM_RED) == Factions.TEAM_RED)
						{
							SpellManager.Instance.UnlockMagick(players[i], this.mMagick);
						}
					}
				}
				else if ((iPlayer.Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
				{
					Player[] players2 = Game.Instance.Players;
					for (int j = 0; j < players2.Length; j++)
					{
						if ((players2[j].Team & Factions.TEAM_BLUE) == Factions.TEAM_BLUE)
						{
							SpellManager.Instance.UnlockMagick(players2[j], this.mMagick);
						}
					}
				}
				else
				{
					SpellManager.Instance.UnlockMagick(iPlayer, this.mMagick);
				}
			}
			else
			{
				SpellManager.Instance.UnlockMagick(this.mMagick, this.mPlayState.GameType);
				if (this.mPlayState.GameType == GameType.Campaign)
				{
					TelemetryUtils.SendCollectSpellbook(this.mMagick);
				}
			}
			this.Kill();
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06002B56 RID: 11094 RVA: 0x001565C2 File Offset: 0x001547C2
		public override bool Dead
		{
			get
			{
				return this.mDead;
			}
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06002B57 RID: 11095 RVA: 0x001565CA File Offset: 0x001547CA
		public override bool Removable
		{
			get
			{
				return this.Dead;
			}
		}

		// Token: 0x06002B58 RID: 11096 RVA: 0x001565D2 File Offset: 0x001547D2
		public override void Kill()
		{
			this.mDead = true;
			this.mTimeOutTimer = 0f;
			EffectManager.Instance.Stop(ref this.mEffect);
			EffectManager.Instance.Stop(ref this.mTimeOutEffect);
		}

		// Token: 0x06002B59 RID: 11097 RVA: 0x00156608 File Offset: 0x00154808
		protected override void IGetNetworkUpdate(out EntityUpdateMessage oMsg, float iPrediction)
		{
			oMsg = default(EntityUpdateMessage);
			if (this.mBody.Immovable || !this.mBody.IsActive)
			{
				iPrediction = 0f;
			}
			if (!this.Resting)
			{
				Transform transform = this.mBody.Transform;
				TransformRate transformRate = this.mBody.TransformRate;
				transform.ApplyTransformRate(ref transformRate, iPrediction);
				oMsg.Features |= EntityFeatures.Position;
				oMsg.Position = transform.Position;
				oMsg.Features |= EntityFeatures.Orientation;
				Quaternion.CreateFromRotationMatrix(ref transform.Orientation, out oMsg.Orientation);
				oMsg.Features |= EntityFeatures.Velocity;
				oMsg.Velocity = this.mBody.Velocity;
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06002B5A RID: 11098 RVA: 0x001566C4 File Offset: 0x001548C4
		public override bool Permanent
		{
			get
			{
				return this.mTimeOutTimer <= 0f && !this.Dead;
			}
		}

		// Token: 0x04002EF5 RID: 12021
		private static Queue<BookOfMagick> sCache;

		// Token: 0x04002EF6 RID: 12022
		private static Model sBookModel;

		// Token: 0x04002EF7 RID: 12023
		private MagickType mMagick;

		// Token: 0x04002EF8 RID: 12024
		public static readonly int TIMEOUT_EFFECT = "tome_timing_out".GetHashCodeCustom();

		// Token: 0x04002EF9 RID: 12025
		public static readonly int DISAPPEAR_EFFECT = "tome_disappear".GetHashCodeCustom();

		// Token: 0x04002EFA RID: 12026
		public static readonly int MAGICK_PICKUP_LOC = "#magick_pick_up".GetHashCodeCustom();

		// Token: 0x04002EFB RID: 12027
		private static readonly int GLIMMER_EFFECT = "bookofmagick_glimmer".GetHashCodeCustom();

		// Token: 0x04002EFC RID: 12028
		private VisualEffectReference mEffect;

		// Token: 0x04002EFD RID: 12029
		protected float mRestingTimer = 1f;

		// Token: 0x04002EFE RID: 12030
		private string mPickUpString;

		// Token: 0x04002EFF RID: 12031
		private float mTimeOutTimer;

		// Token: 0x04002F00 RID: 12032
		private VisualEffectReference mTimeOutEffect;

		// Token: 0x04002F01 RID: 12033
		private static string[] sPickUpStrings;
	}
}
