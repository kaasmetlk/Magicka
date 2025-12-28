using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Magicka.Achievements;
using Magicka.DRM;
using Magicka.GameLogic.GameStates;
using Magicka.GameLogic.GameStates.Menu;
using Magicka.Graphics;
using Magicka.Levels.Campaign;
using Magicka.Levels.Packs;
using Magicka.WebTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using SteamWrapper;

namespace Magicka.GameLogic.UI
{
	// Token: 0x0200063E RID: 1598
	public sealed class DLC_StatusHelper
	{
		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x0600303C RID: 12348 RVA: 0x0018BB68 File Offset: 0x00189D68
		public static DLC_StatusHelper Instance
		{
			get
			{
				if (DLC_StatusHelper.mInstance == null)
				{
					lock (DLC_StatusHelper.mSingeltonLockObj)
					{
						if (DLC_StatusHelper.mInstance == null)
						{
							DLC_StatusHelper.mInstance = new DLC_StatusHelper();
						}
					}
				}
				return DLC_StatusHelper.mInstance;
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x0600303D RID: 12349 RVA: 0x0018BBBC File Offset: 0x00189DBC
		public bool HasPromotion
		{
			get
			{
				return this.mCurrentPromotionInfo != null;
			}
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x0600303E RID: 12350 RVA: 0x0018BBCA File Offset: 0x00189DCA
		public string CurrentPromotion_NoneStoreURL
		{
			get
			{
				if (this.mCurrentPromotionInfo != null)
				{
					return this.mCurrentPromotionInfo.NoneStoreURL;
				}
				return "";
			}
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x0600303F RID: 12351 RVA: 0x0018BBE5 File Offset: 0x00189DE5
		public uint CurrentPromotion_AppID
		{
			get
			{
				if (this.mCurrentPromotionInfo != null)
				{
					return this.mCurrentPromotionInfo.AppID;
				}
				return 0U;
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06003040 RID: 12352 RVA: 0x0018BBFC File Offset: 0x00189DFC
		public string CurrentPromotion_Name
		{
			get
			{
				if (this.mCurrentPromotionInfo != null)
				{
					return this.mCurrentPromotionInfo.Name;
				}
				return "";
			}
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06003041 RID: 12353 RVA: 0x0018BC17 File Offset: 0x00189E17
		public bool CurrentPromotion_IsDynamicallyLoaded
		{
			get
			{
				return this.mCurrentPromotionInfo != null && this.mCurrentPromotionInfo.IsDynamicallyLoaded;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06003042 RID: 12354 RVA: 0x0018BC2E File Offset: 0x00189E2E
		public bool PromotionListIsLocked
		{
			get
			{
				return this.mPromotionListIsLocked;
			}
		}

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x06003043 RID: 12355 RVA: 0x0018BC36 File Offset: 0x00189E36
		// (set) Token: 0x06003044 RID: 12356 RVA: 0x0018BC3E File Offset: 0x00189E3E
		public bool IsBusy
		{
			get
			{
				return this.mIsBusy;
			}
			private set
			{
				this.mIsBusy = value;
			}
		}

		// Token: 0x06003045 RID: 12357 RVA: 0x0018BC60 File Offset: 0x00189E60
		private DLC_StatusHelper()
		{
			this.mPackedItems = new Dictionary<uint, uint>();
			this.mPackedItems.Add(73054U, 901679U);
			this.mPackedItems.Add(73055U, 901679U);
			this.mPackedItems.Add(73056U, 901679U);
			this.Splash_ConstructHardCodedAppIDs();
			this.SyncWithLocalData();
			this.SortSplashList();
			this.SortItemsList();
			this.SaveLocalData();
			this.FindNewDLCs();
			this.Promotion_SelectLatestNotOwndDLC();
			this.Promotion_UpdateLockStatus();
			this.mFreeDLCIDs = new uint[]
			{
				73111U,
				73112U,
				73113U,
				73114U,
				73118U,
				73032U
			};
			SteamAPI.DlcInstalled += this.DlcInstalled;
			Game.Instance.AddLoadTask(new Action(this.TryLoadExtraFromFTP));
		}

		// Token: 0x06003046 RID: 12358 RVA: 0x0018BD34 File Offset: 0x00189F34
		private void TryLoadExtraFromFTP()
		{
			List<DLC_StatusHelper.DynamicPromotionLoadData> list = new List<DLC_StatusHelper.DynamicPromotionLoadData>();
			DLC_StatusHelper.mURL_PromotionLoader = new SimpleFileFromURL(new Uri("https://s3.amazonaws.com/paradox-ads/magicka/"));
			DLC_StatusHelper.mURL_PromotionLoader.Connect();
			bool flag = false;
			this.mURL_PromotionXML = new XmlDocument();
			flag = DLC_StatusHelper.mURL_PromotionLoader.GetXML("current_promotion.xml", out this.mURL_PromotionXML);
			if (flag && this.mURL_PromotionXML != null)
			{
				XmlNodeList xmlNodeList = this.mURL_PromotionXML.SelectNodes("root/Promotion");
				if (xmlNodeList != null && xmlNodeList.Count > 0)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (!(xmlNode is XmlComment))
						{
							uint num = 0U;
							bool flag2 = false;
							XmlAttribute xmlAttribute = xmlNode.Attributes["AppID"];
							if (xmlAttribute != null)
							{
								flag2 = uint.TryParse(xmlAttribute.Value, out num);
								if (!flag2)
								{
									num = 0U;
								}
							}
							string name = "";
							XmlAttribute xmlAttribute2 = xmlNode.Attributes["Name"];
							if (xmlAttribute2 != null)
							{
								name = xmlAttribute2.Value;
							}
							XmlNode xmlNode2 = xmlNode.SelectSingleNode("img");
							XmlNode xmlNode3 = xmlNode.SelectSingleNode("url");
							if (xmlNode2 == null)
							{
								flag = false;
								list.Clear();
								break;
							}
							string text = "";
							if (xmlNode3 == null)
							{
								if (!flag2 || num == 0U)
								{
									flag = false;
									list.Clear();
									break;
								}
							}
							else
							{
								text = xmlNode3.InnerText;
								if (string.IsNullOrEmpty(text))
								{
									flag = false;
									list.Clear();
									break;
								}
							}
							list.Add(new DLC_StatusHelper.DynamicPromotionLoadData(num, name, xmlNode2.InnerText, text));
						}
					}
					if (!flag)
					{
						return;
					}
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			bool flag3 = false;
			if (flag && list != null && list.Count > 0)
			{
				foreach (DLC_StatusHelper.DynamicPromotionLoadData dynamicPromotionLoadData in list)
				{
					Texture2D texture2D;
					DLC_StatusHelper.mURL_PromotionLoader.GetTexture(dynamicPromotionLoadData.imgUrl, Game.Instance.GraphicsDevice, out texture2D);
					if (texture2D != null)
					{
						flag3 = true;
						if (DLC_StatusHelper.dynamicallyLoadedtextures == null)
						{
							DLC_StatusHelper.dynamicallyLoadedtextures = new List<Texture2D>();
						}
						DLC_StatusHelper.dynamicallyLoadedtextures.Add(texture2D);
						PromotionInfo promotionInfo = new PromotionInfo();
						promotionInfo.IsDynamicallyLoaded = true;
						promotionInfo.AppID = dynamicPromotionLoadData.appID;
						promotionInfo.DynamicTextureID = DLC_StatusHelper.dynamicallyLoadedtextures.Count - 1;
						promotionInfo.IsNewToPlayer = true;
						promotionInfo.IsOwndByPlayer = false;
						promotionInfo.Name = dynamicPromotionLoadData.name;
						promotionInfo.ReleasedDate = DateTime.Now;
						promotionInfo.NoneStoreURL = dynamicPromotionLoadData.gotoUrl;
						DLC_StatusHelper.mDLC_SplashInfos.Add(promotionInfo);
					}
				}
			}
			DLC_StatusHelper.mURL_PromotionLoader.Disconnect();
			if (flag3)
			{
				this.SortSplashList();
				this.Promotion_SelectLatestNotOwndDLC();
				if (DLC_StatusHelper.OnDynamicPromotionsLoaded != null)
				{
					DLC_StatusHelper.OnDynamicPromotionsLoaded.Invoke();
				}
			}
		}

		// Token: 0x06003047 RID: 12359 RVA: 0x0018C048 File Offset: 0x0018A248
		private void FindNewDLCs()
		{
			foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
			{
				if ((DateTime.Now.Date - promotionInfo.ReleasedDate).Days <= 14)
				{
					if (DLC_StatusHelper.mNewDLCs == null)
					{
						DLC_StatusHelper.mNewDLCs = new List<uint>();
					}
					DLC_StatusHelper.mNewDLCs.Add(promotionInfo.AppID);
				}
			}
		}

		// Token: 0x06003048 RID: 12360 RVA: 0x0018C0D8 File Offset: 0x0018A2D8
		public bool AppID_IsNew(uint appID)
		{
			return this.AppID_IsNew(appID, "");
		}

		// Token: 0x06003049 RID: 12361 RVA: 0x0018C0E8 File Offset: 0x0018A2E8
		public bool AppID_IsNew(uint appID, string objectName)
		{
			return DLC_StatusHelper.mNewDLCs != null && DLC_StatusHelper.mNewDLCs.Count != 0 && (DLC_StatusHelper.mNewDLCs.Contains(appID) || (!string.IsNullOrEmpty(objectName) && DLC_StatusHelper.hardcodedNewItems != null && DLC_StatusHelper.hardcodedNewItems.Count != 0 && DLC_StatusHelper.hardcodedNewItems.Contains(objectName)));
		}

		// Token: 0x0600304A RID: 12362 RVA: 0x0018C143 File Offset: 0x0018A343
		private void DlcInstalled(DlcInstalled obj)
		{
			this.Item_SetAllInDLC_UsedStatus(obj.mAppID, false);
		}

		// Token: 0x0600304B RID: 12363 RVA: 0x0018C153 File Offset: 0x0018A353
		public bool IsFreeDLC(uint appID)
		{
			return this.IsFreeDLC(appID, null);
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x0018C160 File Offset: 0x0018A360
		public bool IsFreeDLC(uint appID, string itemName)
		{
			for (int i = 0; i < this.mFreeDLCIDs.Length; i++)
			{
				if (this.mFreeDLCIDs[i] == appID)
				{
					return true;
				}
			}
			return !string.IsNullOrEmpty(itemName) && DLC_StatusHelper.hardcodedFreeItems != null && DLC_StatusHelper.hardcodedFreeItems.Count != 0 && DLC_StatusHelper.hardcodedFreeItems.Contains(itemName);
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x0018C1B8 File Offset: 0x0018A3B8
		private void Promotion_UpdateLockStatus()
		{
			this.mPromotionListIsLocked = false;
			if (this.mCurrentPromotionInfo != null && (DateTime.Now.Date - this.mCurrentPromotionInfo.ReleasedDate).Days <= 14)
			{
				this.mPromotionListIsLocked = true;
			}
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x0018C204 File Offset: 0x0018A404
		private void SortSplashList()
		{
			DLC_StatusHelper.mDLC_SplashInfos.Sort(new Comparison<PromotionInfo>(PromotionInfo.CompareByDate));
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x0018C21C File Offset: 0x0018A41C
		private void Promotion_SelectLatestNotOwndDLC()
		{
			this.mCurrentPromotionInfo = null;
			int num = -1;
			if (DLC_StatusHelper.mDLC_SplashInfos != null && DLC_StatusHelper.mDLC_SplashInfos.Count > 0)
			{
				bool flag = false;
				foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
				{
					if (promotionInfo.IsDynamicallyLoaded)
					{
						flag = true;
						this.mCurrentPromotionInfo = promotionInfo;
						num++;
						break;
					}
					num++;
				}
				if (!flag)
				{
					num = -1;
					foreach (PromotionInfo promotionInfo2 in DLC_StatusHelper.mDLC_SplashInfos)
					{
						if (!promotionInfo2.IsOwndByPlayer)
						{
							this.mCurrentPromotionInfo = promotionInfo2;
							num++;
							break;
						}
						num++;
					}
				}
			}
			if (this.mCurrentPromotionInfo == null)
			{
				this.mCurrentDLCIndex = -1;
			}
			else
			{
				this.mCurrentDLCIndex = num;
			}
			this.Splash_TryLoadTextureForCurrent();
		}

		// Token: 0x06003050 RID: 12368 RVA: 0x0018C320 File Offset: 0x0018A520
		public void Splash_TrySelectNextDLC()
		{
			if (DLC_StatusHelper.mDLC_SplashInfos == null || DLC_StatusHelper.mDLC_SplashInfos.Count == 0)
			{
				this.mCurrentDLCIndex = -1;
				this.mCurrentPromotionInfo = null;
				return;
			}
			if (this.mPromotionListIsLocked)
			{
				return;
			}
			this.mCurrentDLCIndex++;
			if (this.mCurrentDLCIndex == -1)
			{
				this.mCurrentDLCIndex = 0;
			}
			else if (this.mCurrentDLCIndex > DLC_StatusHelper.mDLC_SplashInfos.Count - 1)
			{
				this.mCurrentDLCIndex = 0;
			}
			for (int i = this.mCurrentDLCIndex; i < DLC_StatusHelper.mDLC_SplashInfos.Count; i++)
			{
				if (!DLC_StatusHelper.mDLC_SplashInfos[i].IsOwndByPlayer || DLC_StatusHelper.mDLC_SplashInfos[i].IsDynamicallyLoaded)
				{
					this.mCurrentPromotionInfo = DLC_StatusHelper.mDLC_SplashInfos[i];
					this.mCurrentDLCIndex = i;
					break;
				}
			}
			this.Splash_TryLoadTextureForCurrent();
		}

		// Token: 0x06003051 RID: 12369 RVA: 0x0018C3F4 File Offset: 0x0018A5F4
		private void Splash_TryLoadTextureForCurrent()
		{
			Texture2D texture2D = this.mCurrentDLC_Texture;
			if (this.mCurrentPromotionInfo != null && this.mCurrentPromotionInfo.IsDynamicallyLoaded && DLC_StatusHelper.dynamicallyLoadedtextures != null && this.mCurrentPromotionInfo.DynamicTextureID < DLC_StatusHelper.dynamicallyLoadedtextures.Count)
			{
				this.mCurrentDLC_Texture = DLC_StatusHelper.dynamicallyLoadedtextures[this.mCurrentPromotionInfo.DynamicTextureID];
				return;
			}
			bool flag = true;
			if (this.mCurrentPromotionInfo != null && this.mCurrentDLCIndex > -1)
			{
				lock (Game.Instance.GraphicsDevice)
				{
					try
					{
						texture2D = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/Splashes/" + this.mCurrentPromotionInfo.AppID);
					}
					catch (Exception)
					{
						flag = false;
					}
					goto IL_B6;
				}
			}
			flag = false;
			IL_B6:
			if (!flag)
			{
				flag = true;
				lock (Game.Instance.GraphicsDevice)
				{
					try
					{
						texture2D = Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/Splashes/default");
					}
					catch (Exception)
					{
						flag = false;
					}
				}
			}
			this.mCurrentDLC_Texture = texture2D;
		}

		// Token: 0x06003052 RID: 12370 RVA: 0x0018C530 File Offset: 0x0018A730
		public void Splash_Set_DLC_NotNew(uint appID)
		{
			foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
			{
				if (promotionInfo.AppID == appID && promotionInfo.IsOwndByPlayer)
				{
					promotionInfo.IsNewToPlayer = false;
				}
			}
			this.SaveLocalData();
			if (this.mCurrentPromotionInfo.AppID == appID)
			{
				this.mCurrentPromotionInfo = null;
				this.Promotion_SelectLatestNotOwndDLC();
			}
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x0018C5B4 File Offset: 0x0018A7B4
		private void Splash_ConstructHardCodedAppIDs()
		{
			if (DLC_StatusHelper.mDLC_SplashInfos != null)
			{
				return;
			}
			DLC_StatusHelper.mDLC_SplashInfos = new List<PromotionInfo>();
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 255980U,
				ReleasedDate = new DateTime(2013, 10, 31),
				Name = "Dungeons and Gargoyles",
				IsOwndByPlayer = Helper.CheckDLCID(73116U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73118U,
				ReleasedDate = new DateTime(2013, 1, 17),
				Name = "Free Jolnir's Workshop",
				IsOwndByPlayer = Helper.CheckDLCID(73118U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73120U,
				ReleasedDate = new DateTime(2012, 10, 29),
				Name = "Grimnir's Laboratory",
				IsOwndByPlayer = Helper.CheckDLCID(73120U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73115U,
				ReleasedDate = new DateTime(2012, 10, 11),
				Name = "Dungeons and Daemons",
				IsOwndByPlayer = Helper.CheckDLCID(73115U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73095U,
				ReleasedDate = new DateTime(2012, 7, 24),
				Name = "Mega Villain Robes",
				IsOwndByPlayer = Helper.CheckDLCID(73095U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73096U,
				ReleasedDate = new DateTime(2012, 7, 24),
				Name = "Aspiring Musician Robes",
				IsOwndByPlayer = Helper.CheckDLCID(73096U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73097U,
				ReleasedDate = new DateTime(2012, 7, 24),
				Name = "Peculiar Gadgets Item Pack",
				IsOwndByPlayer = Helper.CheckDLCID(73097U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73098U,
				ReleasedDate = new DateTime(2012, 7, 24),
				Name = "Heirlooms Item Pack",
				IsOwndByPlayer = Helper.CheckDLCID(73098U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73093U,
				ReleasedDate = new DateTime(2012, 6, 19),
				Name = "The Other Side of the Coin",
				IsOwndByPlayer = Helper.CheckDLCID(73093U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73039U,
				ReleasedDate = new DateTime(2012, 2, 16),
				Name = "Lonely Island Cruise",
				IsOwndByPlayer = Helper.CheckDLCID(73039U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73058U,
				ReleasedDate = new DateTime(2011, 11, 30),
				Name = "The Stars Are Left",
				IsOwndByPlayer = Helper.CheckDLCID(73058U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73091U,
				ReleasedDate = new DateTime(2011, 11, 30),
				Name = "Holiday Spirit Item Pack",
				IsOwndByPlayer = Helper.CheckDLCID(73091U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73092U,
				ReleasedDate = new DateTime(2011, 11, 30),
				Name = "Horror Props Item Pack",
				IsOwndByPlayer = Helper.CheckDLCID(73092U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 42918U,
				ReleasedDate = new DateTime(2011, 4, 12),
				Name = "Vietnam",
				IsOwndByPlayer = Helper.CheckDLCID(42918U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73030U,
				ReleasedDate = new DateTime(2011, 3, 8),
				Name = "Wizard's Survival Kit",
				IsOwndByPlayer = Helper.CheckDLCID(73030U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73033U,
				ReleasedDate = new DateTime(2011, 4, 26),
				Name = "Marshlands",
				IsOwndByPlayer = Helper.CheckDLCID(73033U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73031U,
				ReleasedDate = new DateTime(2011, 6, 1),
				Name = "Nippon",
				IsOwndByPlayer = Helper.CheckDLCID(73031U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73035U,
				ReleasedDate = new DateTime(2011, 6, 21),
				Name = "Final Frontier",
				IsOwndByPlayer = Helper.CheckDLCID(73035U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73036U,
				ReleasedDate = new DateTime(2011, 6, 21),
				Name = "The Watchtower",
				IsOwndByPlayer = Helper.CheckDLCID(73036U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73037U,
				ReleasedDate = new DateTime(2011, 6, 21),
				Name = "Frozen Lake",
				IsOwndByPlayer = Helper.CheckDLCID(73037U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 901679U,
				ReleasedDate = new DateTime(2011, 6, 21),
				Name = "Party Robes",
				IsOwndByPlayer = Helper.CheckDLCID(901679U),
				IsNewToPlayer = true
			});
			DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo
			{
				AppID = 73057U,
				ReleasedDate = new DateTime(2011, 9, 18),
				Name = "Gamer Bundle",
				IsOwndByPlayer = Helper.CheckDLCID(73057U),
				IsNewToPlayer = true
			});
		}

		// Token: 0x06003054 RID: 12372 RVA: 0x0018CD4C File Offset: 0x0018AF4C
		public MenuImageTextItem Splash_GetMenuItem(Vector2 pos)
		{
			bool flag = false;
			return this.Splash_GetMenuItem(pos, DLC_StatusHelper.DEFAULT_SIZE, out flag);
		}

		// Token: 0x06003055 RID: 12373 RVA: 0x0018CD69 File Offset: 0x0018AF69
		public MenuImageTextItem Splash_GetMenuItem(Vector2 pos, out bool isNew)
		{
			return this.Splash_GetMenuItem(pos, DLC_StatusHelper.DEFAULT_SIZE, out isNew);
		}

		// Token: 0x06003056 RID: 12374 RVA: 0x0018CD78 File Offset: 0x0018AF78
		public MenuImageTextItem Splash_GetMenuItem(Vector2 pos, Vector2 size, out bool isNew)
		{
			isNew = false;
			MenuImageTextItem menuImageTextItem = new MenuImageTextItem(pos, this.mCurrentDLC_Texture, DLC_StatusHelper.mTextureOffset, DLC_StatusHelper.mTextureScale, 0, Vector2.Zero, TextAlign.Center, DLC_StatusHelper.mFont, size);
			menuImageTextItem.SetTitle("");
			if (this.mCurrentPromotionInfo == null)
			{
				isNew = false;
			}
			else
			{
				isNew = this.AppID_IsNew(this.mCurrentPromotionInfo.AppID);
			}
			return menuImageTextItem;
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x0018CDD8 File Offset: 0x0018AFD8
		public List<PromotionInfo> Splash_GetAllOwned()
		{
			List<PromotionInfo> list = new List<PromotionInfo>();
			lock (DLC_StatusHelper.mDLC_SplashInfos)
			{
				foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
				{
					if (promotionInfo.IsOwndByPlayer)
					{
						list.Add(promotionInfo);
					}
				}
			}
			return list;
		}

		// Token: 0x06003058 RID: 12376 RVA: 0x0018CE5C File Offset: 0x0018B05C
		public List<PromotionInfo> Splash_GetAllOwnedAndNew()
		{
			List<PromotionInfo> list = new List<PromotionInfo>();
			lock (DLC_StatusHelper.mDLC_SplashInfos)
			{
				foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
				{
					if (promotionInfo.IsOwndByPlayer && promotionInfo.IsNewToPlayer)
					{
						list.Add(promotionInfo);
					}
				}
			}
			return list;
		}

		// Token: 0x06003059 RID: 12377 RVA: 0x0018CEE8 File Offset: 0x0018B0E8
		public List<PromotionInfo> Splash_GetAllOwnedButNotNew()
		{
			List<PromotionInfo> list = new List<PromotionInfo>();
			lock (DLC_StatusHelper.mDLC_SplashInfos)
			{
				foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
				{
					if (promotionInfo.IsOwndByPlayer && !promotionInfo.IsNewToPlayer)
					{
						list.Add(promotionInfo);
					}
				}
			}
			return list;
		}

		// Token: 0x0600305A RID: 12378 RVA: 0x0018CF74 File Offset: 0x0018B174
		public List<PromotionInfo> Splash_GetAllNotOwned()
		{
			List<PromotionInfo> list = new List<PromotionInfo>();
			lock (DLC_StatusHelper.mDLC_SplashInfos)
			{
				foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
				{
					if (!promotionInfo.IsOwndByPlayer)
					{
						list.Add(promotionInfo);
					}
				}
			}
			return list;
		}

		// Token: 0x0600305B RID: 12379 RVA: 0x0018CFF8 File Offset: 0x0018B1F8
		private void SortItemsList()
		{
			if (DLC_StatusHelper.mItemStatuses != null && DLC_StatusHelper.mItemStatuses.Count > 0)
			{
				DLC_StatusHelper.mItemStatuses.Sort(new Comparison<DLC_ContentUsedStatus>(DLC_ContentUsedStatus.CompareByType));
			}
		}

		// Token: 0x0600305C RID: 12380 RVA: 0x0018D024 File Offset: 0x0018B224
		public void Item_TrySetUsed(string type, string name, bool forceSave)
		{
			int index = -1;
			bool flag = this.Item_HasUsedStatus(type, name, 0U, out index);
			if (flag)
			{
				bool isUsed = DLC_StatusHelper.mItemStatuses[index].IsUsed;
				DLC_StatusHelper.mItemStatuses[index].IsUsed = true;
				if (forceSave)
				{
					this.SaveLocalData();
				}
			}
		}

		// Token: 0x0600305D RID: 12381 RVA: 0x0018D070 File Offset: 0x0018B270
		public List<DLC_ContentUsedStatus> Item_GetAllForDLC(uint appID)
		{
			List<DLC_ContentUsedStatus> list = new List<DLC_ContentUsedStatus>();
			foreach (DLC_ContentUsedStatus dlc_ContentUsedStatus in DLC_StatusHelper.mItemStatuses)
			{
				if (dlc_ContentUsedStatus.AppID == appID)
				{
					list.Add(dlc_ContentUsedStatus);
				}
			}
			return list;
		}

		// Token: 0x0600305E RID: 12382 RVA: 0x0018D0D4 File Offset: 0x0018B2D4
		public List<DLC_ContentUsedStatus> Item_GetAllNewForDLC(uint appID)
		{
			List<DLC_ContentUsedStatus> list = new List<DLC_ContentUsedStatus>();
			foreach (DLC_ContentUsedStatus dlc_ContentUsedStatus in DLC_StatusHelper.mItemStatuses)
			{
				if (dlc_ContentUsedStatus.AppID == appID && dlc_ContentUsedStatus.IsUsed)
				{
					list.Add(dlc_ContentUsedStatus);
				}
			}
			return list;
		}

		// Token: 0x0600305F RID: 12383 RVA: 0x0018D140 File Offset: 0x0018B340
		private void Item_SetAllInDLC_UsedStatus(uint appID, bool newStatus)
		{
			bool flag = false;
			foreach (DLC_ContentUsedStatus dlc_ContentUsedStatus in DLC_StatusHelper.mItemStatuses)
			{
				if (dlc_ContentUsedStatus.AppID == appID)
				{
					dlc_ContentUsedStatus.IsUsed = newStatus;
					flag = true;
				}
			}
			if (flag)
			{
				this.SaveLocalData();
			}
		}

		// Token: 0x06003060 RID: 12384 RVA: 0x0018D1A8 File Offset: 0x0018B3A8
		public bool Item_IsUnused(string type, string name)
		{
			return this.Item_IsUnused(type, name, 0U, true);
		}

		// Token: 0x06003061 RID: 12385 RVA: 0x0018D1B4 File Offset: 0x0018B3B4
		public bool Item_IsUnused(string type, string name, uint appID)
		{
			return this.Item_IsUnused(type, name, appID, true);
		}

		// Token: 0x06003062 RID: 12386 RVA: 0x0018D1C0 File Offset: 0x0018B3C0
		public bool Item_IsUnused(string type, string name, bool forceSaveIfNewEntry)
		{
			return this.Item_IsUnused(type, name, 0U, forceSaveIfNewEntry);
		}

		// Token: 0x06003063 RID: 12387 RVA: 0x0018D1CC File Offset: 0x0018B3CC
		public bool Item_IsUnused(string type, string name, uint appID, bool forceSaveIfNewEntry)
		{
			int index = -1;
			bool flag = this.Item_HasUsedStatus(type, name, appID, out index);
			if (flag)
			{
				return !DLC_StatusHelper.mItemStatuses[index].IsUsed;
			}
			this.Item_CreateEntry(type, name, appID, false, forceSaveIfNewEntry);
			return true;
		}

		// Token: 0x06003064 RID: 12388 RVA: 0x0018D20B File Offset: 0x0018B40B
		private void Item_CreateEntry(string type, string name, bool failSafe)
		{
			this.Item_CreateEntry(type, name, 0U, failSafe, true);
		}

		// Token: 0x06003065 RID: 12389 RVA: 0x0018D218 File Offset: 0x0018B418
		private void Item_CreateEntry(string type, string name, uint appID, bool failSafe)
		{
			this.Item_CreateEntry(type, name, appID, failSafe, true);
		}

		// Token: 0x06003066 RID: 12390 RVA: 0x0018D228 File Offset: 0x0018B428
		private void Item_CreateEntry(string type, string name, uint appID, bool failSafe, bool forceSave)
		{
			if (DLC_StatusHelper.mItemStatuses == null)
			{
				DLC_StatusHelper.mItemStatuses = new List<DLC_ContentUsedStatus>();
			}
			if (failSafe)
			{
				int index = -1;
				bool flag = this.Item_HasUsedStatus(type, name, appID, out index);
				if (flag)
				{
					DLC_StatusHelper.mItemStatuses[index].IsUsed = false;
					return;
				}
				DLC_ContentUsedStatus item = new DLC_ContentUsedStatus
				{
					Name = name,
					IsUsed = false,
					ItemType = type,
					AppID = appID
				};
				this.SetStoreAppID(ref item);
				DLC_StatusHelper.mItemStatuses.Add(item);
				if (forceSave)
				{
					this.SaveLocalData();
					return;
				}
			}
			else
			{
				DLC_ContentUsedStatus item2 = new DLC_ContentUsedStatus
				{
					Name = name,
					IsUsed = false,
					ItemType = type,
					AppID = appID
				};
				this.SetStoreAppID(ref item2);
				DLC_StatusHelper.mItemStatuses.Add(item2);
				if (forceSave)
				{
					this.SaveLocalData();
				}
			}
		}

		// Token: 0x06003067 RID: 12391 RVA: 0x0018D2F8 File Offset: 0x0018B4F8
		private void SetStoreAppID(ref DLC_ContentUsedStatus target)
		{
			if (this.mPackedItems.ContainsKey(target.AppID))
			{
				foreach (KeyValuePair<uint, uint> keyValuePair in this.mPackedItems)
				{
					if (keyValuePair.Key == target.AppID)
					{
						target.StoreAppID = keyValuePair.Value;
						break;
					}
				}
			}
		}

		// Token: 0x06003068 RID: 12392 RVA: 0x0018D378 File Offset: 0x0018B578
		public uint GetStorePageAppID(uint appID)
		{
			if (this.mPackedItems.ContainsKey(appID))
			{
				foreach (KeyValuePair<uint, uint> keyValuePair in this.mPackedItems)
				{
					if (keyValuePair.Key == appID)
					{
						return keyValuePair.Value;
					}
				}
				return appID;
			}
			return appID;
		}

		// Token: 0x06003069 RID: 12393 RVA: 0x0018D3EC File Offset: 0x0018B5EC
		private bool Item_HasUsedStatus(string type, string name, uint appID, out int index)
		{
			index = -1;
			if (DLC_StatusHelper.mItemStatuses == null || DLC_StatusHelper.mItemStatuses.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < DLC_StatusHelper.mItemStatuses.Count; i++)
			{
				bool flag = appID == 0U || DLC_StatusHelper.mItemStatuses[i].AppID == appID;
				if (string.Compare(DLC_StatusHelper.mItemStatuses[i].ItemType, type) == 0 && string.Compare(DLC_StatusHelper.mItemStatuses[i].Name, name) == 0 && flag)
				{
					index = i;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600306A RID: 12394 RVA: 0x0018D5D8 File Offset: 0x0018B7D8
		public void SaveLocalData()
		{
			Game.Instance.Form.BeginInvoke(new Action(delegate()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<?xml version=\"1.0\"?>\n");
				stringBuilder.Append("<root>\n<LatestSave>" + DateTime.Now + "</LatestSave>\n");
				stringBuilder.Append("<DLC_ContentNewStatuses>\n");
				if (DLC_StatusHelper.mItemStatuses != null && DLC_StatusHelper.mItemStatuses.Count > 0)
				{
					foreach (DLC_ContentUsedStatus dlc_ContentUsedStatus in DLC_StatusHelper.mItemStatuses)
					{
						stringBuilder.Append(dlc_ContentUsedStatus.ToXML());
					}
				}
				stringBuilder.Append("</DLC_ContentNewStatuses>\n");
				stringBuilder.Append("<DLC_Splash_Statuses>\n");
				if (DLC_StatusHelper.mDLC_SplashInfos != null && DLC_StatusHelper.mDLC_SplashInfos.Count > 0)
				{
					foreach (PromotionInfo promotionInfo in DLC_StatusHelper.mDLC_SplashInfos)
					{
						stringBuilder.Append(promotionInfo.ToXML());
					}
				}
				stringBuilder.Append("</DLC_Splash_Statuses>\n");
				stringBuilder.Append("</root>");
				StreamWriter streamWriter = new StreamWriter("./SaveData/DLCData.sav", false, Encoding.UTF8);
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
				stringBuilder = null;
			}));
		}

		// Token: 0x0600306B RID: 12395 RVA: 0x0018D9C0 File Offset: 0x0018BBC0
		private void SyncWithLocalData()
		{
			Game.Instance.Form.BeginInvoke(new Action(delegate()
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load("./SaveData/DLCData.sav");
				}
				catch (Exception)
				{
					return;
				}
				if (xmlDocument != null)
				{
					XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
					DLC_StatusHelper.mItemStatuses = new List<DLC_ContentUsedStatus>();
					if (xmlNode != null)
					{
						XmlNodeList xmlNodeList = xmlNode.SelectNodes("DLC_ContentNewStatuses/DLC_ContentNewStatus");
						if (xmlNodeList != null && xmlNodeList.Count > 0)
						{
							foreach (object obj in xmlNodeList)
							{
								XmlNode n = (XmlNode)obj;
								DLC_ContentUsedStatus dlc_ContentUsedStatus = DLC_ContentUsedStatus.FromXml(n);
								if (dlc_ContentUsedStatus != null && (string.Compare(dlc_ContentUsedStatus.ItemType, "level") == 0 || string.Compare(dlc_ContentUsedStatus.ItemType, "robe") == 0 || string.Compare(dlc_ContentUsedStatus.ItemType, "item") == 0 || string.Compare(dlc_ContentUsedStatus.ItemType, "ItemPack") == 0 || string.Compare(dlc_ContentUsedStatus.ItemType, "MagickPack") == 0))
								{
									int num = -1;
									int num2 = 0;
									foreach (DLC_ContentUsedStatus dlc_ContentUsedStatus2 in DLC_StatusHelper.mItemStatuses)
									{
										if (string.Compare(dlc_ContentUsedStatus2.Name, dlc_ContentUsedStatus.Name) == 0)
										{
											if (dlc_ContentUsedStatus2.AppID != 0U && dlc_ContentUsedStatus.AppID == 0U)
											{
												break;
											}
											if ((dlc_ContentUsedStatus2.AppID == 0U && dlc_ContentUsedStatus.AppID != 0U) || dlc_ContentUsedStatus2.AppID == dlc_ContentUsedStatus.AppID)
											{
												num = num2;
												break;
											}
										}
										num2++;
									}
									if (num != -1)
									{
										DLC_StatusHelper.mItemStatuses[num] = dlc_ContentUsedStatus;
									}
									else
									{
										DLC_StatusHelper.mItemStatuses.Add(dlc_ContentUsedStatus);
									}
								}
							}
						}
					}
					List<PromotionInfo> list = new List<PromotionInfo>();
					if (xmlNode != null)
					{
						XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("DLC_Splash_Statuses/DLC_SplashInfo");
						if (xmlNodeList2 != null && xmlNodeList2.Count > 0)
						{
							foreach (object obj2 in xmlNodeList2)
							{
								XmlNode n2 = (XmlNode)obj2;
								PromotionInfo promotionInfo = PromotionInfo.FromXml(n2);
								if (promotionInfo != null)
								{
									list.Add(promotionInfo);
								}
							}
						}
					}
					if (list.Count > 0)
					{
						foreach (PromotionInfo promotionInfo2 in list)
						{
							foreach (PromotionInfo promotionInfo3 in DLC_StatusHelper.mDLC_SplashInfos)
							{
								if (promotionInfo3.AppID == promotionInfo2.AppID)
								{
									if ((DateTime.Now.Date - promotionInfo3.ReleasedDate).Days <= 14 && !promotionInfo3.IsOwndByPlayer)
									{
										promotionInfo3.IsNewToPlayer = true;
									}
									else
									{
										promotionInfo3.IsNewToPlayer = (!promotionInfo2.IsOwndByPlayer && promotionInfo3.IsOwndByPlayer);
									}
									if (promotionInfo3.IsNewToPlayer)
									{
										this.Item_SetAllInDLC_UsedStatus(promotionInfo3.AppID, true);
										break;
									}
									break;
								}
							}
						}
					}
					return;
				}
			}));
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x0018D9DE File Offset: 0x0018BBDE
		public static bool HasAnyUnusedContent_Challange()
		{
			return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Challenge) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Challenge);
		}

		// Token: 0x0600306D RID: 12397 RVA: 0x0018D9F0 File Offset: 0x0018BBF0
		public static bool HasAnyUnusedContent_StoryChallange()
		{
			return DLC_StatusHelper.HasAnyUnusedLevels(GameType.StoryChallange) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.StoryChallange);
		}

		// Token: 0x0600306E RID: 12398 RVA: 0x0018DA04 File Offset: 0x0018BC04
		public static bool HasAnyUnusedContent_Versus()
		{
			return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Versus) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Versus);
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x0018DA16 File Offset: 0x0018BC16
		public static bool HasAnyUnusedContent_Campaing_Vanilla()
		{
			return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Campaign) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Campaign);
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x0018DA28 File Offset: 0x0018BC28
		public static bool HasAnyUnusedContent_Campaing_Mythos()
		{
			return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Mythos) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Mythos);
		}

		// Token: 0x06003071 RID: 12401 RVA: 0x0018DA3A File Offset: 0x0018BC3A
		public static List<string> RobesThatIgnoreUsedStatus()
		{
			return DLC_StatusHelper.RobesThatIgnoreUsedStatus(GameType.Any);
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x0018DA44 File Offset: 0x0018BC44
		public static List<string> RobesThatIgnoreUsedStatus(GameType gameType)
		{
			List<string> list = new List<string>
			{
				"wizard",
				"wizardalu",
				"wizarddra",
				"wizardnec",
				"wlad_diplomat"
			};
			if (gameType == GameType.Mythos || gameType == GameType.Any)
			{
				if (!AchievementsManager.Instance.HasAchievement("fhtagnoncemore") && !AchievementsManager.Instance.HasAchievement("drivenmad"))
				{
					list.Add("wizardcul");
				}
			}
			else
			{
				list.Add("wizardcul");
			}
			return list;
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x0018DAD4 File Offset: 0x0018BCD4
		public static bool HasAnyUnusedRobe(GameType gameType)
		{
			IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
			uint appID = 0U;
			List<string> list = DLC_StatusHelper.RobesThatIgnoreUsedStatus();
			foreach (Profile.PlayableAvatar playableAvatar in values)
			{
				if (!list.Contains(playableAvatar.Name))
				{
					HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
					if (license != HackHelper.License.Custom && !DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out appID) && ((gameType != GameType.StoryChallange && gameType != GameType.Challenge) || playableAvatar.AllowChallenge) && ((gameType != GameType.Mythos && gameType != GameType.Campaign) || playableAvatar.AllowCampaign) && (gameType != GameType.Versus || playableAvatar.AllowPVP) && DLC_StatusHelper.Instance.Item_IsUnused("robe", playableAvatar.Name, appID, false))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003074 RID: 12404 RVA: 0x0018DBB0 File Offset: 0x0018BDB0
		public static bool HasAnyUnusedItemPacks()
		{
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			foreach (ItemPack itemPack in itemPacks)
			{
				if (!itemPack.IsUsed && itemPack.License == HackHelper.License.Yes)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003075 RID: 12405 RVA: 0x0018DBFC File Offset: 0x0018BDFC
		public static bool HasAnyUnusedMagicPacks()
		{
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			foreach (MagickPack magickPack in magickPacks)
			{
				if (!magickPack.IsUsed && magickPack.License == HackHelper.License.Yes)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003076 RID: 12406 RVA: 0x0018DC48 File Offset: 0x0018BE48
		public static void TrySetAllItemsAndMagicsUsed()
		{
			ItemPack[] itemPacks = PackMan.Instance.ItemPacks;
			foreach (ItemPack itemPack in itemPacks)
			{
				if (!itemPack.IsUsed)
				{
					itemPack.SetUsed(false);
				}
			}
			MagickPack[] magickPacks = PackMan.Instance.MagickPacks;
			foreach (MagickPack magickPack in magickPacks)
			{
				if (!magickPack.IsUsed)
				{
					magickPack.SetUsed(false);
				}
			}
			DLC_StatusHelper.Instance.SaveLocalData();
		}

		// Token: 0x06003077 RID: 12407 RVA: 0x0018DCCC File Offset: 0x0018BECC
		public static bool HasAnyUnusedLevels(GameType gameType)
		{
			List<LevelNode> list = new List<LevelNode>();
			if (gameType <= GameType.Mythos)
			{
				switch (gameType)
				{
				case GameType.Campaign:
					list.AddRange(LevelManager.Instance.VanillaCampaign);
					goto IL_F6;
				case GameType.Challenge:
					list.AddRange(LevelManager.Instance.Challenges);
					goto IL_F6;
				case (GameType)3:
					break;
				case GameType.Versus:
					list.AddRange(LevelManager.Instance.Versus);
					goto IL_F6;
				default:
					if (gameType == GameType.Mythos)
					{
						list.AddRange(LevelManager.Instance.MythosCampaign);
						goto IL_F6;
					}
					break;
				}
			}
			else
			{
				if (gameType == GameType.Any)
				{
					list.AddRange(LevelManager.Instance.VanillaCampaign);
					list.AddRange(LevelManager.Instance.Challenges);
					list.AddRange(LevelManager.Instance.MythosCampaign);
					list.AddRange(LevelManager.Instance.StoryChallanges);
					list.AddRange(LevelManager.Instance.Versus);
					goto IL_F6;
				}
				if (gameType == GameType.StoryChallange)
				{
					list.AddRange(LevelManager.Instance.StoryChallanges);
					goto IL_F6;
				}
			}
			return false;
			IL_F6:
			if (list.Count == 0)
			{
				return false;
			}
			uint appID = 0U;
			foreach (LevelNode levelNode in list)
			{
				HackHelper.License license = HackHelper.CheckLicense(levelNode);
				if (license != HackHelper.License.Custom && !DLC_StatusHelper.ValidateLevelLocked(license, levelNode, out appID) && DLC_StatusHelper.Instance.Item_IsUnused("level", levelNode.Name, appID, false))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003078 RID: 12408 RVA: 0x0018DE50 File Offset: 0x0018C050
		internal static bool ValidateLevelLocked(HackHelper.License license, LevelNode lvl, out uint appID)
		{
			uint num = 0U;
			return DLC_StatusHelper.ValidateLevelLocked(license, lvl, out appID, out num);
		}

		// Token: 0x06003079 RID: 12409 RVA: 0x0018DE6C File Offset: 0x0018C06C
		internal static bool ValidateLevelLocked(HackHelper.License license, LevelNode lvl, out uint appID, out uint storePageAppID)
		{
			appID = (storePageAppID = 0U);
			return license != HackHelper.License.Custom && DLC_StatusHelper.ValidateLevelLocked(lvl, out appID, out storePageAppID);
		}

		// Token: 0x0600307A RID: 12410 RVA: 0x0018DE90 File Offset: 0x0018C090
		internal static bool ValidateLevelLocked(LevelNode lvl, out uint appID)
		{
			appID = 0U;
			uint num = 0U;
			return DLC_StatusHelper.ValidateLevelLocked(lvl, out appID, out num);
		}

		// Token: 0x0600307B RID: 12411 RVA: 0x0018DEAC File Offset: 0x0018C0AC
		internal static bool ValidateLevelLocked(LevelNode lvl, out uint appID, out uint storePageAppId)
		{
			appID = (storePageAppId = 0U);
			if (lvl == null)
			{
				return false;
			}
			byte[] hashSum = lvl.HashSum;
			if (hashSum == null || hashSum.Length == 0)
			{
				return false;
			}
			bool appID2 = HashTable.GetAppID(hashSum, out appID);
			storePageAppId = appID;
			if (!appID2)
			{
				return false;
			}
			if (appID == 42910U)
			{
				return false;
			}
			bool result = !Helper.CheckDLCID(appID);
			storePageAppId = DLC_StatusHelper.Instance.GetStorePageAppID(appID);
			return result;
		}

		// Token: 0x0600307C RID: 12412 RVA: 0x0018DF10 File Offset: 0x0018C110
		internal static bool ValidateRobeLocked(HackHelper.License license, Profile.PlayableAvatar avatar, out uint appID)
		{
			appID = 0U;
			uint num = 0U;
			return DLC_StatusHelper.ValidateRobeLocked(license, avatar, out appID, out num);
		}

		// Token: 0x0600307D RID: 12413 RVA: 0x0018DF2C File Offset: 0x0018C12C
		internal static bool ValidateRobeLocked(HackHelper.License license, Profile.PlayableAvatar avatar, out uint appID, out uint storePageAppID)
		{
			appID = (storePageAppID = 0U);
			return license != HackHelper.License.Custom && DLC_StatusHelper.ValidateRobeLocked(avatar, out appID, out storePageAppID);
		}

		// Token: 0x0600307E RID: 12414 RVA: 0x0018DF50 File Offset: 0x0018C150
		internal static bool ValidateRobeLocked(Profile.PlayableAvatar avatar, out uint appID)
		{
			appID = 0U;
			uint num = 0U;
			return DLC_StatusHelper.ValidateRobeLocked(avatar, out appID, out num);
		}

		// Token: 0x0600307F RID: 12415 RVA: 0x0018DF6C File Offset: 0x0018C16C
		internal static bool ValidateRobeLocked(Profile.PlayableAvatar avatar, out uint appID, out uint storePageAppID)
		{
			appID = (storePageAppID = 0U);
			byte[] hashSum = avatar.HashSum;
			if (hashSum == null || hashSum.Length == 0)
			{
				return false;
			}
			bool appID2 = HashTable.GetAppID(hashSum, out appID);
			storePageAppID = appID;
			if (!appID2)
			{
				return false;
			}
			if (appID == 42910U)
			{
				return false;
			}
			bool result = !Helper.CheckDLCID(appID);
			storePageAppID = DLC_StatusHelper.Instance.GetStorePageAppID(appID);
			return result;
		}

		// Token: 0x0400346D RID: 13421
		private const string SaveLocation = "./SaveData/DLCData.sav";

		// Token: 0x0400346E RID: 13422
		private const string DynamicPromotions_URL = "https://s3.amazonaws.com/paradox-ads/magicka/";

		// Token: 0x0400346F RID: 13423
		private const string DynamicPromotions_XMLFileName = "current_promotion.xml";

		// Token: 0x04003470 RID: 13424
		private const string DynamicPromotions_NodeName = "Promotion";

		// Token: 0x04003471 RID: 13425
		private const int DAYS_TO_FORCE_LATEST = 14;

		// Token: 0x04003472 RID: 13426
		private const TextAlign TextAlignment = TextAlign.Center;

		// Token: 0x04003473 RID: 13427
		public static Action OnDynamicPromotionsLoaded;

		// Token: 0x04003474 RID: 13428
		private static SimpleFileFromURL mURL_PromotionLoader;

		// Token: 0x04003475 RID: 13429
		private XmlDocument mURL_PromotionXML;

		// Token: 0x04003476 RID: 13430
		private static DLC_StatusHelper mInstance = null;

		// Token: 0x04003477 RID: 13431
		private static volatile object mSingeltonLockObj = new object();

		// Token: 0x04003478 RID: 13432
		private static readonly List<string> hardcodedNewItems = new List<string>();

		// Token: 0x04003479 RID: 13433
		private static readonly List<string> hardcodedFreeItems = new List<string>
		{
			"wizardpat",
			"wizardspa",
			"wizardtron",
			"wizardred",
			"wizardcru",
			"wizardres",
			"ch_woot_eye_sockey_rink",
			"ch_woot_elemental_roulette",
			"ch_woot_bout_of_madness",
			"vs_woot_diamond_ring",
			"vs_woot_elemental_roulette",
			"vs_woot_halls_of_moisture",
			"vs_woot_prize_podium"
		};

		// Token: 0x0400347A RID: 13434
		private bool mIsBusy;

		// Token: 0x0400347B RID: 13435
		private uint[] mFreeDLCIDs;

		// Token: 0x0400347C RID: 13436
		private Dictionary<uint, uint> mPackedItems;

		// Token: 0x0400347D RID: 13437
		private PromotionInfo mCurrentPromotionInfo;

		// Token: 0x0400347E RID: 13438
		private Texture2D mCurrentDLC_Texture;

		// Token: 0x0400347F RID: 13439
		private int mCurrentDLCIndex = -1;

		// Token: 0x04003480 RID: 13440
		private static readonly Vector2 DEFAULT_SIZE = new Vector2(180f, 300f);

		// Token: 0x04003481 RID: 13441
		private static readonly BitmapFont mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);

		// Token: 0x04003482 RID: 13442
		private static readonly Vector2 mTextureOffset = Vector2.Zero;

		// Token: 0x04003483 RID: 13443
		private static readonly Vector2 mTextureScale = Vector2.One;

		// Token: 0x04003484 RID: 13444
		private bool mPromotionListIsLocked;

		// Token: 0x04003485 RID: 13445
		private static List<PromotionInfo> mDLC_SplashInfos;

		// Token: 0x04003486 RID: 13446
		private static List<uint> mNewDLCs;

		// Token: 0x04003487 RID: 13447
		private static List<Texture2D> dynamicallyLoadedtextures;

		// Token: 0x04003488 RID: 13448
		private static List<DLC_ContentUsedStatus> mItemStatuses;

		// Token: 0x0200063F RID: 1599
		private struct DynamicPromotionLoadData
		{
			// Token: 0x06003083 RID: 12419 RVA: 0x0018E0C8 File Offset: 0x0018C2C8
			public DynamicPromotionLoadData(uint appID, string name, string img, string url)
			{
				this.appID = appID;
				this.name = name;
				this.imgUrl = img;
				this.gotoUrl = url;
			}

			// Token: 0x0400348A RID: 13450
			public uint appID;

			// Token: 0x0400348B RID: 13451
			public string imgUrl;

			// Token: 0x0400348C RID: 13452
			public string gotoUrl;

			// Token: 0x0400348D RID: 13453
			public string name;
		}
	}
}
