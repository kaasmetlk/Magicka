using System;
using System.Diagnostics;
using Magicka.GameLogic.Controls;
using Microsoft.Xna.Framework.Input;

namespace Magicka.GameLogic.GameStates.Menu.Main
{
	// Token: 0x020003AA RID: 938
	internal class DirectionalKeyboardHelper
	{
		// Token: 0x06001CD5 RID: 7381 RVA: 0x000CA99C File Offset: 0x000C8B9C
		internal DirectionalKeyboardHelper()
		{
			this.mWatch = new Stopwatch();
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x000CA9B0 File Offset: 0x000C8BB0
		internal void Update(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
		{
			if (iOldState == iNewState)
			{
				if (this.mWatch.ElapsedMilliseconds > (this.mFirstPress ? 500L : 100L))
				{
					this.mWatch.Reset();
					this.mWatch.Start();
					this.mFirstPress = false;
					this.Press(iSender, iNewState);
					return;
				}
			}
			else if (!this.CheckNewPresses(iSender, iOldState, iNewState))
			{
				this.CheckReleases(iOldState, iNewState);
			}
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x000CAA20 File Offset: 0x000C8C20
		private bool CheckNewPresses(Controller iSender, KeyboardState iOldState, KeyboardState iNewState)
		{
			bool flag = (iOldState.IsKeyUp(Keys.Down) && iNewState.IsKeyDown(Keys.Down)) || (iOldState.IsKeyUp(Keys.Up) && iNewState.IsKeyDown(Keys.Up)) || (iOldState.IsKeyUp(Keys.Right) && iNewState.IsKeyDown(Keys.Right)) || (iOldState.IsKeyUp(Keys.Left) && iNewState.IsKeyDown(Keys.Left));
			if (flag)
			{
				this.mFirstPress = true;
				this.mWatch.Reset();
				this.mWatch.Start();
			}
			return flag;
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x000CAAAC File Offset: 0x000C8CAC
		private void CheckReleases(KeyboardState iOldState, KeyboardState iNewState)
		{
			bool flag = (iOldState.IsKeyDown(Keys.Down) && iNewState.IsKeyUp(Keys.Down)) || (iOldState.IsKeyDown(Keys.Up) && iNewState.IsKeyUp(Keys.Up)) || (iOldState.IsKeyDown(Keys.Right) && iNewState.IsKeyUp(Keys.Right)) || (iOldState.IsKeyDown(Keys.Left) && iNewState.IsKeyUp(Keys.Left));
			if (flag)
			{
				this.mWatch.Stop();
				this.mWatch.Reset();
			}
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x000CAB30 File Offset: 0x000C8D30
		private void Press(Controller iSender, KeyboardState iState)
		{
			if (iState.IsKeyDown(Keys.Down))
			{
				if (this.DownPressed != null)
				{
					this.DownPressed(iSender);
				}
				return;
			}
			if (iState.IsKeyDown(Keys.Up))
			{
				if (this.UpPressed != null)
				{
					this.UpPressed(iSender);
				}
				return;
			}
			if (iState.IsKeyDown(Keys.Right))
			{
				if (this.RightPressed != null)
				{
					this.RightPressed(iSender);
				}
				return;
			}
			if (iState.IsKeyDown(Keys.Left) && this.LeftPressed != null)
			{
				this.LeftPressed(iSender);
			}
		}

		// Token: 0x04001F79 RID: 8057
		private Stopwatch mWatch;

		// Token: 0x04001F7A RID: 8058
		internal Action<Controller> UpPressed;

		// Token: 0x04001F7B RID: 8059
		internal Action<Controller> DownPressed;

		// Token: 0x04001F7C RID: 8060
		internal Action<Controller> RightPressed;

		// Token: 0x04001F7D RID: 8061
		internal Action<Controller> LeftPressed;

		// Token: 0x04001F7E RID: 8062
		private bool mFirstPress;
	}
}
