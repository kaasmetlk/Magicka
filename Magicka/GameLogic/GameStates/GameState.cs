using System;
using Magicka.GameLogic.Controls;
using PolygonHead;

namespace Magicka.GameLogic.GameStates
{
	// Token: 0x02000156 RID: 342
	public abstract class GameState
	{
		// Token: 0x06000A32 RID: 2610 RVA: 0x0003DFAA File Offset: 0x0003C1AA
		public GameState(Camera iCamera)
		{
			this.mScene = new Scene(Game.Instance.GraphicsDevice, 512, iCamera);
		}

		// Token: 0x06000A33 RID: 2611
		public abstract void OnEnter();

		// Token: 0x06000A34 RID: 2612
		public abstract void OnExit();

		// Token: 0x06000A35 RID: 2613 RVA: 0x0003DFCD File Offset: 0x0003C1CD
		public virtual void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			this.mScene.ClearObjects(iDataChannel);
			ControlManager.Instance.HandleInput(iDataChannel, iDeltaTime);
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000A36 RID: 2614 RVA: 0x0003DFE7 File Offset: 0x0003C1E7
		public Scene Scene
		{
			get
			{
				return this.mScene;
			}
		}

		// Token: 0x0400094A RID: 2378
		protected Scene mScene;
	}
}
