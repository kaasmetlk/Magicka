using System;
using Microsoft.Xna.Framework.Content;

namespace Magicka.GameLogic.Entities.Items
{
	// Token: 0x020004AA RID: 1194
	public struct CallbackEvent
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06002412 RID: 9234 RVA: 0x00102FD6 File Offset: 0x001011D6
		// (remove) Token: 0x06002413 RID: 9235 RVA: 0x00102FED File Offset: 0x001011ED
		public static event CallbackEvent.ItemCallbackFn callbackFn;

		// Token: 0x06002414 RID: 9236 RVA: 0x00103004 File Offset: 0x00101204
		public CallbackEvent(ContentReader iInput)
		{
			throw new Exception("Unhandled callback event! CallbackEvent cannot be called from script.");
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x00103010 File Offset: 0x00101210
		public CallbackEvent(CallbackEvent.ItemCallbackFn func)
		{
			CallbackEvent.callbackFn = func;
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x00103018 File Offset: 0x00101218
		public void Execute(Entity iItem, Entity iTarget)
		{
			if (iItem is Character)
			{
				(iItem as Character).Terminate(true, false);
				return;
			}
			CallbackEvent.callbackFn(iItem, iTarget);
		}

		// Token: 0x020004AB RID: 1195
		// (Invoke) Token: 0x06002418 RID: 9240
		public delegate void ItemCallbackFn(Entity iItem, Entity iTarget);
	}
}
