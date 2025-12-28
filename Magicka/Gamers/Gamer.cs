using System;
using System.IO;
using Magicka.Achievements;
using Magicka.GameLogic;
using Magicka.GameLogic.GameStates;

namespace Magicka.Gamers
{
	// Token: 0x020001B3 RID: 435
	public class Gamer
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000D41 RID: 3393 RVA: 0x0004CD6E File Offset: 0x0004AF6E
		// (set) Token: 0x06000D42 RID: 3394 RVA: 0x0004CD76 File Offset: 0x0004AF76
		public virtual Profile.PlayableAvatar Avatar
		{
			get
			{
				return this.mAvatar;
			}
			set
			{
				this.mAvatar = value;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000D43 RID: 3395 RVA: 0x0004CD7F File Offset: 0x0004AF7F
		// (set) Token: 0x06000D44 RID: 3396 RVA: 0x0004CD87 File Offset: 0x0004AF87
		public uint VersusWins
		{
			get
			{
				return this.mVersusWins;
			}
			set
			{
				this.mVersusWins = value;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000D45 RID: 3397 RVA: 0x0004CD90 File Offset: 0x0004AF90
		// (set) Token: 0x06000D46 RID: 3398 RVA: 0x0004CD98 File Offset: 0x0004AF98
		public uint VersusDefeats
		{
			get
			{
				return this.mVersusDefeats;
			}
			set
			{
				this.mVersusDefeats = value;
			}
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0004CDA1 File Offset: 0x0004AFA1
		public void IncrementVersusWinStreak(PlayState iPlayState)
		{
			this.mVersusWinStreak += 1U;
			if (this.mVersusWinStreak >= 20U)
			{
				AchievementsManager.Instance.AwardAchievement(iPlayState, "nothingbutaman");
			}
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x0004CDCB File Offset: 0x0004AFCB
		public void ResetVersusWinStreak()
		{
			this.mVersusWinStreak = 0U;
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06000D49 RID: 3401 RVA: 0x0004CDD4 File Offset: 0x0004AFD4
		// (set) Token: 0x06000D4A RID: 3402 RVA: 0x0004CDDC File Offset: 0x0004AFDC
		public string GamerTag
		{
			get
			{
				return this.mGamerTag;
			}
			set
			{
				this.mGamerTag = value;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000D4B RID: 3403 RVA: 0x0004CDE5 File Offset: 0x0004AFE5
		// (set) Token: 0x06000D4C RID: 3404 RVA: 0x0004CDED File Offset: 0x0004AFED
		public virtual byte Color
		{
			get
			{
				return this.mColor;
			}
			set
			{
				this.mColor = value;
			}
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0004CDF6 File Offset: 0x0004AFF6
		private Gamer()
		{
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x0004CE00 File Offset: 0x0004B000
		public Gamer(string iGamerTag)
		{
			if (iGamerTag.Length > 15)
			{
				iGamerTag = iGamerTag.Substring(0, 15);
			}
			this.mGamerTag = iGamerTag;
			this.mAvatar = Profile.Instance.DefaultAvatar;
			this.mColor = (byte)Gamer.sRandom.Next(Defines.PLAYERCOLORS_UNLOCKED);
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000D4F RID: 3407 RVA: 0x0004CE58 File Offset: 0x0004B058
		public bool InUse
		{
			get
			{
				Player[] players = Game.Instance.Players;
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].Playing & players[i].Gamer == this)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x0004CE98 File Offset: 0x0004B098
		public static Gamer Read(BinaryReader iReader)
		{
			Gamer gamer = new Gamer();
			gamer.mGamerTag = iReader.ReadString();
			if (gamer.mGamerTag.Length > 15)
			{
				gamer.mGamerTag = gamer.mGamerTag.Substring(0, 15);
			}
			gamer.mColor = iReader.ReadByte();
			string iName = iReader.ReadString();
			gamer.mAvatar = Profile.Instance.GetAvatar(iName);
			gamer.Kills = iReader.ReadUInt32();
			gamer.Deaths = iReader.ReadUInt32();
			gamer.OverKilled = iReader.ReadUInt32();
			gamer.OverKills = iReader.ReadUInt32();
			gamer.HealingDone = iReader.ReadUInt64();
			gamer.HealingReceived = iReader.ReadUInt64();
			gamer.DamageDone = iReader.ReadUInt64();
			gamer.DamageReceived = iReader.ReadUInt64();
			gamer.Suicides = iReader.ReadUInt32();
			gamer.TeamKills = iReader.ReadUInt32();
			gamer.TeamKilled = iReader.ReadUInt32();
			gamer.mVersusWins = iReader.ReadUInt32();
			gamer.mVersusWinStreak = iReader.ReadUInt32();
			gamer.mVersusDefeats = iReader.ReadUInt32();
			return gamer;
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x0004CFA8 File Offset: 0x0004B1A8
		public void Write(BinaryWriter iWriter)
		{
			iWriter.Write(this.mGamerTag);
			iWriter.Write(this.mColor);
			iWriter.Write(this.mAvatar.Name);
			iWriter.Write(this.Kills);
			iWriter.Write(this.Deaths);
			iWriter.Write(this.OverKilled);
			iWriter.Write(this.OverKills);
			iWriter.Write(this.HealingDone);
			iWriter.Write(this.HealingReceived);
			iWriter.Write(this.DamageDone);
			iWriter.Write(this.DamageReceived);
			iWriter.Write(this.Suicides);
			iWriter.Write(this.TeamKills);
			iWriter.Write(this.TeamKilled);
			iWriter.Write(this.mVersusWins);
			iWriter.Write(this.mVersusWinStreak);
			iWriter.Write(this.mVersusDefeats);
		}

		// Token: 0x04000BF0 RID: 3056
		private static Random sRandom = new Random();

		// Token: 0x04000BF1 RID: 3057
		public static readonly Gamer INVALID_GAMER = new Gamer("");

		// Token: 0x04000BF2 RID: 3058
		protected string mGamerTag;

		// Token: 0x04000BF3 RID: 3059
		protected byte mColor;

		// Token: 0x04000BF4 RID: 3060
		protected Profile.PlayableAvatar mAvatar;

		// Token: 0x04000BF5 RID: 3061
		public uint Kills;

		// Token: 0x04000BF6 RID: 3062
		public uint Deaths;

		// Token: 0x04000BF7 RID: 3063
		public uint OverKilled;

		// Token: 0x04000BF8 RID: 3064
		public uint OverKills;

		// Token: 0x04000BF9 RID: 3065
		public ulong HealingDone;

		// Token: 0x04000BFA RID: 3066
		public ulong HealingReceived;

		// Token: 0x04000BFB RID: 3067
		public ulong DamageDone;

		// Token: 0x04000BFC RID: 3068
		public ulong DamageReceived;

		// Token: 0x04000BFD RID: 3069
		public uint Suicides;

		// Token: 0x04000BFE RID: 3070
		public uint TeamKills;

		// Token: 0x04000BFF RID: 3071
		public uint TeamKilled;

		// Token: 0x04000C00 RID: 3072
		protected uint mVersusWins;

		// Token: 0x04000C01 RID: 3073
		protected uint mVersusWinStreak;

		// Token: 0x04000C02 RID: 3074
		protected uint mVersusDefeats;
	}
}
