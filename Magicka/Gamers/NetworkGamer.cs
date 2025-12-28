using System;
using Microsoft.Xna.Framework.Graphics;
using SteamWrapper;

namespace Magicka.Gamers
{
	// Token: 0x020001B4 RID: 436
	internal class NetworkGamer : Gamer
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0004D0A1 File Offset: 0x0004B2A1
		// (set) Token: 0x06000D54 RID: 3412 RVA: 0x0004D0A9 File Offset: 0x0004B2A9
		public SteamID ClientID
		{
			get
			{
				return this.mClientID;
			}
			set
			{
				this.mClientID = value;
			}
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x0004D0B4 File Offset: 0x0004B2B4
		public NetworkGamer(string iGamerTag, byte iColor, string iAvatarThumb, string iAvatarType, SteamID iClientID) : base(iGamerTag)
		{
			this.mAvatar.ThumbPath = iAvatarThumb;
			this.mAvatar.Thumb = Game.Instance.Content.Load<Texture2D>(this.mAvatar.ThumbPath);
			this.mAvatar.TypeName = iAvatarType;
			this.mAvatar.Type = this.mAvatar.TypeName.GetHashCodeCustom();
			this.mClientID = iClientID;
			this.mColor = iColor;
		}

		// Token: 0x04000C03 RID: 3075
		private SteamID mClientID;
	}
}
