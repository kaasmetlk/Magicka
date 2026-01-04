// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.DLC_StatusHelper
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

public sealed class DLC_StatusHelper
{
  private const string SaveLocation = "./SaveData/DLCData.sav";
  private const string DynamicPromotions_URL = "https://s3.amazonaws.com/paradox-ads/magicka/";
  private const string DynamicPromotions_XMLFileName = "current_promotion.xml";
  private const string DynamicPromotions_NodeName = "Promotion";
  private const int DAYS_TO_FORCE_LATEST = 14;
  private const TextAlign TextAlignment = TextAlign.Center;
  public static Action OnDynamicPromotionsLoaded;
  private static SimpleFileFromURL mURL_PromotionLoader;
  private XmlDocument mURL_PromotionXML;
  private static DLC_StatusHelper mInstance = (DLC_StatusHelper) null;
  private static volatile object mSingeltonLockObj = new object();
  private static readonly List<string> hardcodedNewItems = new List<string>();
  private static readonly List<string> hardcodedFreeItems = new List<string>()
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
  private bool mIsBusy;
  private uint[] mFreeDLCIDs;
  private Dictionary<uint, uint> mPackedItems;
  private PromotionInfo mCurrentPromotionInfo;
  private Texture2D mCurrentDLC_Texture;
  private int mCurrentDLCIndex = -1;
  private static readonly Vector2 DEFAULT_SIZE = new Vector2(180f, 300f);
  private static readonly BitmapFont mFont = FontManager.Instance.GetFont(MagickaFont.MenuOption);
  private static readonly Vector2 mTextureOffset = Vector2.Zero;
  private static readonly Vector2 mTextureScale = Vector2.One;
  private bool mPromotionListIsLocked;
  private static List<PromotionInfo> mDLC_SplashInfos;
  private static List<uint> mNewDLCs;
  private static List<Texture2D> dynamicallyLoadedtextures;
  private static List<DLC_ContentUsedStatus> mItemStatuses;

  public static DLC_StatusHelper Instance
  {
    get
    {
      if (DLC_StatusHelper.mInstance == null)
      {
        lock (DLC_StatusHelper.mSingeltonLockObj)
        {
          if (DLC_StatusHelper.mInstance == null)
            DLC_StatusHelper.mInstance = new DLC_StatusHelper();
        }
      }
      return DLC_StatusHelper.mInstance;
    }
  }

  public bool HasPromotion => this.mCurrentPromotionInfo != null;

  public string CurrentPromotion_NoneStoreURL
  {
    get => this.mCurrentPromotionInfo != null ? this.mCurrentPromotionInfo.NoneStoreURL : "";
  }

  public uint CurrentPromotion_AppID
  {
    get => this.mCurrentPromotionInfo != null ? this.mCurrentPromotionInfo.AppID : 0U;
  }

  public string CurrentPromotion_Name
  {
    get => this.mCurrentPromotionInfo != null ? this.mCurrentPromotionInfo.Name : "";
  }

  public bool CurrentPromotion_IsDynamicallyLoaded
  {
    get => this.mCurrentPromotionInfo != null && this.mCurrentPromotionInfo.IsDynamicallyLoaded;
  }

  public bool PromotionListIsLocked => this.mPromotionListIsLocked;

  public bool IsBusy
  {
    get => this.mIsBusy;
    private set => this.mIsBusy = value;
  }

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
    this.mFreeDLCIDs = new uint[6]
    {
      73111U,
      73112U,
      73113U,
      73114U,
      73118U,
      73032U
    };
    SteamAPI.DlcInstalled += new Action<SteamWrapper.DlcInstalled>(this.DlcInstalled);
    Magicka.Game.Instance.AddLoadTask(new Action(this.TryLoadExtraFromFTP));
  }

  private void TryLoadExtraFromFTP()
  {
    List<DLC_StatusHelper.DynamicPromotionLoadData> promotionLoadDataList = new List<DLC_StatusHelper.DynamicPromotionLoadData>();
    DLC_StatusHelper.mURL_PromotionLoader = new SimpleFileFromURL(new Uri("https://s3.amazonaws.com/paradox-ads/magicka/"));
    DLC_StatusHelper.mURL_PromotionLoader.Connect();
    this.mURL_PromotionXML = new XmlDocument();
    bool flag1 = DLC_StatusHelper.mURL_PromotionLoader.GetXML("current_promotion.xml", out this.mURL_PromotionXML);
    if (flag1 && this.mURL_PromotionXML != null)
    {
      XmlNodeList xmlNodeList = this.mURL_PromotionXML.SelectNodes("root/Promotion");
      if (xmlNodeList != null && xmlNodeList.Count > 0)
      {
        foreach (XmlNode xmlNode1 in xmlNodeList)
        {
          if (!(xmlNode1 is XmlComment))
          {
            uint result = 0;
            bool flag2 = false;
            XmlAttribute attribute1 = xmlNode1.Attributes["AppID"];
            if (attribute1 != null)
            {
              flag2 = uint.TryParse(attribute1.Value, out result);
              if (!flag2)
                result = 0U;
            }
            string name = "";
            XmlAttribute attribute2 = xmlNode1.Attributes["Name"];
            if (attribute2 != null)
              name = attribute2.Value;
            XmlNode xmlNode2 = xmlNode1.SelectSingleNode("img");
            XmlNode xmlNode3 = xmlNode1.SelectSingleNode("url");
            if (xmlNode2 == null)
            {
              flag1 = false;
              promotionLoadDataList.Clear();
              break;
            }
            string url = "";
            if (xmlNode3 == null)
            {
              if (!flag2 || result == 0U)
              {
                flag1 = false;
                promotionLoadDataList.Clear();
                break;
              }
            }
            else
            {
              url = xmlNode3.InnerText;
              if (string.IsNullOrEmpty(url))
              {
                flag1 = false;
                promotionLoadDataList.Clear();
                break;
              }
            }
            promotionLoadDataList.Add(new DLC_StatusHelper.DynamicPromotionLoadData(result, name, xmlNode2.InnerText, url));
          }
        }
        if (!flag1)
          return;
      }
      else
        flag1 = false;
    }
    else
      flag1 = false;
    bool flag3 = false;
    if (flag1 && promotionLoadDataList != null && promotionLoadDataList.Count > 0)
    {
      foreach (DLC_StatusHelper.DynamicPromotionLoadData promotionLoadData in promotionLoadDataList)
      {
        Texture2D tex;
        DLC_StatusHelper.mURL_PromotionLoader.GetTexture(promotionLoadData.imgUrl, Magicka.Game.Instance.GraphicsDevice, out tex);
        if (tex != null)
        {
          flag3 = true;
          if (DLC_StatusHelper.dynamicallyLoadedtextures == null)
            DLC_StatusHelper.dynamicallyLoadedtextures = new List<Texture2D>();
          DLC_StatusHelper.dynamicallyLoadedtextures.Add(tex);
          DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
          {
            IsDynamicallyLoaded = true,
            AppID = promotionLoadData.appID,
            DynamicTextureID = DLC_StatusHelper.dynamicallyLoadedtextures.Count - 1,
            IsNewToPlayer = true,
            IsOwndByPlayer = false,
            Name = promotionLoadData.name,
            ReleasedDate = DateTime.Now,
            NoneStoreURL = promotionLoadData.gotoUrl
          });
        }
      }
    }
    DLC_StatusHelper.mURL_PromotionLoader.Disconnect();
    if (!flag3)
      return;
    this.SortSplashList();
    this.Promotion_SelectLatestNotOwndDLC();
    if (DLC_StatusHelper.OnDynamicPromotionsLoaded == null)
      return;
    DLC_StatusHelper.OnDynamicPromotionsLoaded();
  }

  private void FindNewDLCs()
  {
    foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
    {
      if ((DateTime.Now.Date - mDlcSplashInfo.ReleasedDate).Days <= 14)
      {
        if (DLC_StatusHelper.mNewDLCs == null)
          DLC_StatusHelper.mNewDLCs = new List<uint>();
        DLC_StatusHelper.mNewDLCs.Add(mDlcSplashInfo.AppID);
      }
    }
  }

  public bool AppID_IsNew(uint appID) => this.AppID_IsNew(appID, "");

  public bool AppID_IsNew(uint appID, string objectName)
  {
    if (DLC_StatusHelper.mNewDLCs == null || DLC_StatusHelper.mNewDLCs.Count == 0)
      return false;
    if (DLC_StatusHelper.mNewDLCs.Contains(appID))
      return true;
    return !string.IsNullOrEmpty(objectName) && DLC_StatusHelper.hardcodedNewItems != null && DLC_StatusHelper.hardcodedNewItems.Count != 0 && DLC_StatusHelper.hardcodedNewItems.Contains(objectName);
  }

  private void DlcInstalled(SteamWrapper.DlcInstalled obj)
  {
    this.Item_SetAllInDLC_UsedStatus(obj.mAppID, false);
  }

  public bool IsFreeDLC(uint appID) => this.IsFreeDLC(appID, (string) null);

  public bool IsFreeDLC(uint appID, string itemName)
  {
    for (int index = 0; index < this.mFreeDLCIDs.Length; ++index)
    {
      if ((int) this.mFreeDLCIDs[index] == (int) appID)
        return true;
    }
    return !string.IsNullOrEmpty(itemName) && DLC_StatusHelper.hardcodedFreeItems != null && DLC_StatusHelper.hardcodedFreeItems.Count != 0 && DLC_StatusHelper.hardcodedFreeItems.Contains(itemName);
  }

  private void Promotion_UpdateLockStatus()
  {
    this.mPromotionListIsLocked = false;
    if (this.mCurrentPromotionInfo == null || (DateTime.Now.Date - this.mCurrentPromotionInfo.ReleasedDate).Days > 14)
      return;
    this.mPromotionListIsLocked = true;
  }

  private void SortSplashList()
  {
    DLC_StatusHelper.mDLC_SplashInfos.Sort(new Comparison<PromotionInfo>(PromotionInfo.CompareByDate));
  }

  private void Promotion_SelectLatestNotOwndDLC()
  {
    this.mCurrentPromotionInfo = (PromotionInfo) null;
    int num = -1;
    if (DLC_StatusHelper.mDLC_SplashInfos != null && DLC_StatusHelper.mDLC_SplashInfos.Count > 0)
    {
      bool flag = false;
      foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
      {
        if (mDlcSplashInfo.IsDynamicallyLoaded)
        {
          flag = true;
          this.mCurrentPromotionInfo = mDlcSplashInfo;
          ++num;
          break;
        }
        ++num;
      }
      if (!flag)
      {
        num = -1;
        foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
        {
          if (!mDlcSplashInfo.IsOwndByPlayer)
          {
            this.mCurrentPromotionInfo = mDlcSplashInfo;
            ++num;
            break;
          }
          ++num;
        }
      }
    }
    this.mCurrentDLCIndex = this.mCurrentPromotionInfo != null ? num : -1;
    this.Splash_TryLoadTextureForCurrent();
  }

  public void Splash_TrySelectNextDLC()
  {
    if (DLC_StatusHelper.mDLC_SplashInfos == null || DLC_StatusHelper.mDLC_SplashInfos.Count == 0)
    {
      this.mCurrentDLCIndex = -1;
      this.mCurrentPromotionInfo = (PromotionInfo) null;
    }
    else
    {
      if (this.mPromotionListIsLocked)
        return;
      ++this.mCurrentDLCIndex;
      if (this.mCurrentDLCIndex == -1)
        this.mCurrentDLCIndex = 0;
      else if (this.mCurrentDLCIndex > DLC_StatusHelper.mDLC_SplashInfos.Count - 1)
        this.mCurrentDLCIndex = 0;
      for (int mCurrentDlcIndex = this.mCurrentDLCIndex; mCurrentDlcIndex < DLC_StatusHelper.mDLC_SplashInfos.Count; ++mCurrentDlcIndex)
      {
        if (!DLC_StatusHelper.mDLC_SplashInfos[mCurrentDlcIndex].IsOwndByPlayer || DLC_StatusHelper.mDLC_SplashInfos[mCurrentDlcIndex].IsDynamicallyLoaded)
        {
          this.mCurrentPromotionInfo = DLC_StatusHelper.mDLC_SplashInfos[mCurrentDlcIndex];
          this.mCurrentDLCIndex = mCurrentDlcIndex;
          break;
        }
      }
      this.Splash_TryLoadTextureForCurrent();
    }
  }

  private void Splash_TryLoadTextureForCurrent()
  {
    Texture2D texture2D = this.mCurrentDLC_Texture;
    if (this.mCurrentPromotionInfo != null && this.mCurrentPromotionInfo.IsDynamicallyLoaded && DLC_StatusHelper.dynamicallyLoadedtextures != null && this.mCurrentPromotionInfo.DynamicTextureID < DLC_StatusHelper.dynamicallyLoadedtextures.Count)
    {
      this.mCurrentDLC_Texture = DLC_StatusHelper.dynamicallyLoadedtextures[this.mCurrentPromotionInfo.DynamicTextureID];
    }
    else
    {
      bool flag1 = true;
      if (this.mCurrentPromotionInfo != null && this.mCurrentDLCIndex > -1)
      {
        lock (Magicka.Game.Instance.GraphicsDevice)
        {
          try
          {
            texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/Splashes/" + (object) this.mCurrentPromotionInfo.AppID);
          }
          catch (Exception ex)
          {
            flag1 = false;
          }
        }
      }
      else
        flag1 = false;
      if (!flag1)
      {
        bool flag2 = true;
        lock (Magicka.Game.Instance.GraphicsDevice)
        {
          try
          {
            texture2D = Magicka.Game.Instance.Content.Load<Texture2D>("UI/DLCFeedback/Splashes/default");
          }
          catch (Exception ex)
          {
            flag2 = false;
          }
        }
      }
      this.mCurrentDLC_Texture = texture2D;
    }
  }

  public void Splash_Set_DLC_NotNew(uint appID)
  {
    foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
    {
      if ((int) mDlcSplashInfo.AppID == (int) appID && mDlcSplashInfo.IsOwndByPlayer)
        mDlcSplashInfo.IsNewToPlayer = false;
    }
    this.SaveLocalData();
    if ((int) this.mCurrentPromotionInfo.AppID != (int) appID)
      return;
    this.mCurrentPromotionInfo = (PromotionInfo) null;
    this.Promotion_SelectLatestNotOwndDLC();
  }

  private void Splash_ConstructHardCodedAppIDs()
  {
    if (DLC_StatusHelper.mDLC_SplashInfos != null)
      return;
    DLC_StatusHelper.mDLC_SplashInfos = new List<PromotionInfo>();
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 255980U,
      ReleasedDate = new DateTime(2013, 10, 31 /*0x1F*/),
      Name = "Dungeons and Gargoyles",
      IsOwndByPlayer = Helper.CheckDLCID(73116U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73118U,
      ReleasedDate = new DateTime(2013, 1, 17),
      Name = "Free Jolnir's Workshop",
      IsOwndByPlayer = Helper.CheckDLCID(73118U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73120U,
      ReleasedDate = new DateTime(2012, 10, 29),
      Name = "Grimnir's Laboratory",
      IsOwndByPlayer = Helper.CheckDLCID(73120U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73115U,
      ReleasedDate = new DateTime(2012, 10, 11),
      Name = "Dungeons and Daemons",
      IsOwndByPlayer = Helper.CheckDLCID(73115U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73095U,
      ReleasedDate = new DateTime(2012, 7, 24),
      Name = "Mega Villain Robes",
      IsOwndByPlayer = Helper.CheckDLCID(73095U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73096U,
      ReleasedDate = new DateTime(2012, 7, 24),
      Name = "Aspiring Musician Robes",
      IsOwndByPlayer = Helper.CheckDLCID(73096U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73097U,
      ReleasedDate = new DateTime(2012, 7, 24),
      Name = "Peculiar Gadgets Item Pack",
      IsOwndByPlayer = Helper.CheckDLCID(73097U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73098U,
      ReleasedDate = new DateTime(2012, 7, 24),
      Name = "Heirlooms Item Pack",
      IsOwndByPlayer = Helper.CheckDLCID(73098U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73093U,
      ReleasedDate = new DateTime(2012, 6, 19),
      Name = "The Other Side of the Coin",
      IsOwndByPlayer = Helper.CheckDLCID(73093U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73039U,
      ReleasedDate = new DateTime(2012, 2, 16 /*0x10*/),
      Name = "Lonely Island Cruise",
      IsOwndByPlayer = Helper.CheckDLCID(73039U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73058U,
      ReleasedDate = new DateTime(2011, 11, 30),
      Name = "The Stars Are Left",
      IsOwndByPlayer = Helper.CheckDLCID(73058U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73091U,
      ReleasedDate = new DateTime(2011, 11, 30),
      Name = "Holiday Spirit Item Pack",
      IsOwndByPlayer = Helper.CheckDLCID(73091U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73092U,
      ReleasedDate = new DateTime(2011, 11, 30),
      Name = "Horror Props Item Pack",
      IsOwndByPlayer = Helper.CheckDLCID(73092U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 42918U,
      ReleasedDate = new DateTime(2011, 4, 12),
      Name = "Vietnam",
      IsOwndByPlayer = Helper.CheckDLCID(42918U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73030U,
      ReleasedDate = new DateTime(2011, 3, 8),
      Name = "Wizard's Survival Kit",
      IsOwndByPlayer = Helper.CheckDLCID(73030U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73033U,
      ReleasedDate = new DateTime(2011, 4, 26),
      Name = "Marshlands",
      IsOwndByPlayer = Helper.CheckDLCID(73033U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73031U,
      ReleasedDate = new DateTime(2011, 6, 1),
      Name = "Nippon",
      IsOwndByPlayer = Helper.CheckDLCID(73031U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73035U,
      ReleasedDate = new DateTime(2011, 6, 21),
      Name = "Final Frontier",
      IsOwndByPlayer = Helper.CheckDLCID(73035U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73036U,
      ReleasedDate = new DateTime(2011, 6, 21),
      Name = "The Watchtower",
      IsOwndByPlayer = Helper.CheckDLCID(73036U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73037U,
      ReleasedDate = new DateTime(2011, 6, 21),
      Name = "Frozen Lake",
      IsOwndByPlayer = Helper.CheckDLCID(73037U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 901679U,
      ReleasedDate = new DateTime(2011, 6, 21),
      Name = "Party Robes",
      IsOwndByPlayer = Helper.CheckDLCID(901679U),
      IsNewToPlayer = true
    });
    DLC_StatusHelper.mDLC_SplashInfos.Add(new PromotionInfo()
    {
      AppID = 73057U,
      ReleasedDate = new DateTime(2011, 9, 18),
      Name = "Gamer Bundle",
      IsOwndByPlayer = Helper.CheckDLCID(73057U),
      IsNewToPlayer = true
    });
  }

  public MenuImageTextItem Splash_GetMenuItem(Vector2 pos)
  {
    bool isNew = false;
    return this.Splash_GetMenuItem(pos, DLC_StatusHelper.DEFAULT_SIZE, out isNew);
  }

  public MenuImageTextItem Splash_GetMenuItem(Vector2 pos, out bool isNew)
  {
    return this.Splash_GetMenuItem(pos, DLC_StatusHelper.DEFAULT_SIZE, out isNew);
  }

  public MenuImageTextItem Splash_GetMenuItem(Vector2 pos, Vector2 size, out bool isNew)
  {
    isNew = false;
    MenuImageTextItem menuItem = new MenuImageTextItem(pos, this.mCurrentDLC_Texture, DLC_StatusHelper.mTextureOffset, DLC_StatusHelper.mTextureScale, 0, Vector2.Zero, TextAlign.Center, DLC_StatusHelper.mFont, size);
    menuItem.SetTitle("");
    isNew = this.mCurrentPromotionInfo != null && this.AppID_IsNew(this.mCurrentPromotionInfo.AppID);
    return menuItem;
  }

  public List<PromotionInfo> Splash_GetAllOwned()
  {
    List<PromotionInfo> allOwned = new List<PromotionInfo>();
    lock (DLC_StatusHelper.mDLC_SplashInfos)
    {
      foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
      {
        if (mDlcSplashInfo.IsOwndByPlayer)
          allOwned.Add(mDlcSplashInfo);
      }
    }
    return allOwned;
  }

  public List<PromotionInfo> Splash_GetAllOwnedAndNew()
  {
    List<PromotionInfo> allOwnedAndNew = new List<PromotionInfo>();
    lock (DLC_StatusHelper.mDLC_SplashInfos)
    {
      foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
      {
        if (mDlcSplashInfo.IsOwndByPlayer && mDlcSplashInfo.IsNewToPlayer)
          allOwnedAndNew.Add(mDlcSplashInfo);
      }
    }
    return allOwnedAndNew;
  }

  public List<PromotionInfo> Splash_GetAllOwnedButNotNew()
  {
    List<PromotionInfo> allOwnedButNotNew = new List<PromotionInfo>();
    lock (DLC_StatusHelper.mDLC_SplashInfos)
    {
      foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
      {
        if (mDlcSplashInfo.IsOwndByPlayer && !mDlcSplashInfo.IsNewToPlayer)
          allOwnedButNotNew.Add(mDlcSplashInfo);
      }
    }
    return allOwnedButNotNew;
  }

  public List<PromotionInfo> Splash_GetAllNotOwned()
  {
    List<PromotionInfo> allNotOwned = new List<PromotionInfo>();
    lock (DLC_StatusHelper.mDLC_SplashInfos)
    {
      foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
      {
        if (!mDlcSplashInfo.IsOwndByPlayer)
          allNotOwned.Add(mDlcSplashInfo);
      }
    }
    return allNotOwned;
  }

  private void SortItemsList()
  {
    if (DLC_StatusHelper.mItemStatuses == null || DLC_StatusHelper.mItemStatuses.Count <= 0)
      return;
    DLC_StatusHelper.mItemStatuses.Sort(new Comparison<DLC_ContentUsedStatus>(DLC_ContentUsedStatus.CompareByType));
  }

  public void Item_TrySetUsed(string type, string name, bool forceSave)
  {
    int index = -1;
    if (!this.Item_HasUsedStatus(type, name, 0U, out index))
      return;
    int num = DLC_StatusHelper.mItemStatuses[index].IsUsed ? 1 : 0;
    DLC_StatusHelper.mItemStatuses[index].IsUsed = true;
    if (!forceSave)
      return;
    this.SaveLocalData();
  }

  public List<DLC_ContentUsedStatus> Item_GetAllForDLC(uint appID)
  {
    List<DLC_ContentUsedStatus> allForDlc = new List<DLC_ContentUsedStatus>();
    foreach (DLC_ContentUsedStatus mItemStatuse in DLC_StatusHelper.mItemStatuses)
    {
      if ((int) mItemStatuse.AppID == (int) appID)
        allForDlc.Add(mItemStatuse);
    }
    return allForDlc;
  }

  public List<DLC_ContentUsedStatus> Item_GetAllNewForDLC(uint appID)
  {
    List<DLC_ContentUsedStatus> allNewForDlc = new List<DLC_ContentUsedStatus>();
    foreach (DLC_ContentUsedStatus mItemStatuse in DLC_StatusHelper.mItemStatuses)
    {
      if ((int) mItemStatuse.AppID == (int) appID && mItemStatuse.IsUsed)
        allNewForDlc.Add(mItemStatuse);
    }
    return allNewForDlc;
  }

  private void Item_SetAllInDLC_UsedStatus(uint appID, bool newStatus)
  {
    bool flag = false;
    foreach (DLC_ContentUsedStatus mItemStatuse in DLC_StatusHelper.mItemStatuses)
    {
      if ((int) mItemStatuse.AppID == (int) appID)
      {
        mItemStatuse.IsUsed = newStatus;
        flag = true;
      }
    }
    if (!flag)
      return;
    this.SaveLocalData();
  }

  public bool Item_IsUnused(string type, string name) => this.Item_IsUnused(type, name, 0U, true);

  public bool Item_IsUnused(string type, string name, uint appID)
  {
    return this.Item_IsUnused(type, name, appID, true);
  }

  public bool Item_IsUnused(string type, string name, bool forceSaveIfNewEntry)
  {
    return this.Item_IsUnused(type, name, 0U, forceSaveIfNewEntry);
  }

  public bool Item_IsUnused(string type, string name, uint appID, bool forceSaveIfNewEntry)
  {
    int index = -1;
    if (this.Item_HasUsedStatus(type, name, appID, out index))
      return !DLC_StatusHelper.mItemStatuses[index].IsUsed;
    this.Item_CreateEntry(type, name, appID, false, forceSaveIfNewEntry);
    return true;
  }

  private void Item_CreateEntry(string type, string name, bool failSafe)
  {
    this.Item_CreateEntry(type, name, 0U, failSafe, true);
  }

  private void Item_CreateEntry(string type, string name, uint appID, bool failSafe)
  {
    this.Item_CreateEntry(type, name, appID, failSafe, true);
  }

  private void Item_CreateEntry(
    string type,
    string name,
    uint appID,
    bool failSafe,
    bool forceSave)
  {
    if (DLC_StatusHelper.mItemStatuses == null)
      DLC_StatusHelper.mItemStatuses = new List<DLC_ContentUsedStatus>();
    if (failSafe)
    {
      int index = -1;
      if (this.Item_HasUsedStatus(type, name, appID, out index))
      {
        DLC_StatusHelper.mItemStatuses[index].IsUsed = false;
      }
      else
      {
        DLC_ContentUsedStatus target = new DLC_ContentUsedStatus()
        {
          Name = name,
          IsUsed = false,
          ItemType = type,
          AppID = appID
        };
        this.SetStoreAppID(ref target);
        DLC_StatusHelper.mItemStatuses.Add(target);
        if (!forceSave)
          return;
        this.SaveLocalData();
      }
    }
    else
    {
      DLC_ContentUsedStatus target = new DLC_ContentUsedStatus()
      {
        Name = name,
        IsUsed = false,
        ItemType = type,
        AppID = appID
      };
      this.SetStoreAppID(ref target);
      DLC_StatusHelper.mItemStatuses.Add(target);
      if (!forceSave)
        return;
      this.SaveLocalData();
    }
  }

  private void SetStoreAppID(ref DLC_ContentUsedStatus target)
  {
    if (!this.mPackedItems.ContainsKey(target.AppID))
      return;
    foreach (KeyValuePair<uint, uint> mPackedItem in this.mPackedItems)
    {
      if ((int) mPackedItem.Key == (int) target.AppID)
      {
        target.StoreAppID = mPackedItem.Value;
        break;
      }
    }
  }

  public uint GetStorePageAppID(uint appID)
  {
    if (!this.mPackedItems.ContainsKey(appID))
      return appID;
    foreach (KeyValuePair<uint, uint> mPackedItem in this.mPackedItems)
    {
      if ((int) mPackedItem.Key == (int) appID)
        return mPackedItem.Value;
    }
    return appID;
  }

  private bool Item_HasUsedStatus(string type, string name, uint appID, out int index)
  {
    index = -1;
    if (DLC_StatusHelper.mItemStatuses == null || DLC_StatusHelper.mItemStatuses.Count == 0)
      return false;
    for (int index1 = 0; index1 < DLC_StatusHelper.mItemStatuses.Count; ++index1)
    {
      bool flag = appID == 0U || (int) DLC_StatusHelper.mItemStatuses[index1].AppID == (int) appID;
      if (string.Compare(DLC_StatusHelper.mItemStatuses[index1].ItemType, type) == 0 && string.Compare(DLC_StatusHelper.mItemStatuses[index1].Name, name) == 0 && flag)
      {
        index = index1;
        return true;
      }
    }
    return false;
  }

  public void SaveLocalData()
  {
    Magicka.Game.Instance.Form.BeginInvoke((Delegate) (() =>
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<?xml version=\"1.0\"?>\n");
      stringBuilder.Append($"<root>\n<LatestSave>{(object) DateTime.Now}</LatestSave>\n");
      stringBuilder.Append("<DLC_ContentNewStatuses>\n");
      if (DLC_StatusHelper.mItemStatuses != null && DLC_StatusHelper.mItemStatuses.Count > 0)
      {
        foreach (DLC_ContentUsedStatus mItemStatuse in DLC_StatusHelper.mItemStatuses)
          stringBuilder.Append(mItemStatuse.ToXML());
      }
      stringBuilder.Append("</DLC_ContentNewStatuses>\n");
      stringBuilder.Append("<DLC_Splash_Statuses>\n");
      if (DLC_StatusHelper.mDLC_SplashInfos != null && DLC_StatusHelper.mDLC_SplashInfos.Count > 0)
      {
        foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
          stringBuilder.Append(mDlcSplashInfo.ToXML());
      }
      stringBuilder.Append("</DLC_Splash_Statuses>\n");
      stringBuilder.Append("</root>");
      StreamWriter streamWriter = new StreamWriter("./SaveData/DLCData.sav", false, Encoding.UTF8);
      streamWriter.Write(stringBuilder.ToString());
      streamWriter.Close();
    }));
  }

  private void SyncWithLocalData()
  {
    Magicka.Game.Instance.Form.BeginInvoke((Delegate) (() =>
    {
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load("./SaveData/DLCData.sav");
      }
      catch (Exception ex)
      {
        return;
      }
      if (xmlDocument == null)
        return;
      XmlNode xmlNode = xmlDocument.SelectSingleNode("root");
      DLC_StatusHelper.mItemStatuses = new List<DLC_ContentUsedStatus>();
      if (xmlNode != null)
      {
        XmlNodeList xmlNodeList = xmlNode.SelectNodes("DLC_ContentNewStatuses/DLC_ContentNewStatus");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
        {
          foreach (XmlNode n in xmlNodeList)
          {
            DLC_ContentUsedStatus contentUsedStatus = DLC_ContentUsedStatus.FromXml(n);
            if (contentUsedStatus != null && (string.Compare(contentUsedStatus.ItemType, "level") == 0 || string.Compare(contentUsedStatus.ItemType, "robe") == 0 || string.Compare(contentUsedStatus.ItemType, "item") == 0 || string.Compare(contentUsedStatus.ItemType, "ItemPack") == 0 || string.Compare(contentUsedStatus.ItemType, "MagickPack") == 0))
            {
              int index = -1;
              int num = 0;
              foreach (DLC_ContentUsedStatus mItemStatuse in DLC_StatusHelper.mItemStatuses)
              {
                if (string.Compare(mItemStatuse.Name, contentUsedStatus.Name) == 0)
                {
                  if (mItemStatuse.AppID != 0U)
                  {
                    if (contentUsedStatus.AppID == 0U)
                      break;
                  }
                  if (mItemStatuse.AppID == 0U && contentUsedStatus.AppID != 0U || (int) mItemStatuse.AppID == (int) contentUsedStatus.AppID)
                  {
                    index = num;
                    break;
                  }
                }
                ++num;
              }
              if (index != -1)
                DLC_StatusHelper.mItemStatuses[index] = contentUsedStatus;
              else
                DLC_StatusHelper.mItemStatuses.Add(contentUsedStatus);
            }
          }
        }
      }
      List<PromotionInfo> promotionInfoList = new List<PromotionInfo>();
      if (xmlNode != null)
      {
        XmlNodeList xmlNodeList = xmlNode.SelectNodes("DLC_Splash_Statuses/DLC_SplashInfo");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
        {
          foreach (XmlNode n in xmlNodeList)
          {
            PromotionInfo promotionInfo = PromotionInfo.FromXml(n);
            if (promotionInfo != null)
              promotionInfoList.Add(promotionInfo);
          }
        }
      }
      if (promotionInfoList.Count <= 0)
        return;
      foreach (PromotionInfo promotionInfo in promotionInfoList)
      {
        foreach (PromotionInfo mDlcSplashInfo in DLC_StatusHelper.mDLC_SplashInfos)
        {
          if ((int) mDlcSplashInfo.AppID == (int) promotionInfo.AppID)
          {
            mDlcSplashInfo.IsNewToPlayer = (DateTime.Now.Date - mDlcSplashInfo.ReleasedDate).Days <= 14 && !mDlcSplashInfo.IsOwndByPlayer || !promotionInfo.IsOwndByPlayer && mDlcSplashInfo.IsOwndByPlayer;
            if (mDlcSplashInfo.IsNewToPlayer)
            {
              this.Item_SetAllInDLC_UsedStatus(mDlcSplashInfo.AppID, true);
              break;
            }
            break;
          }
        }
      }
    }));
  }

  public static bool HasAnyUnusedContent_Challange()
  {
    return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Challenge) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Challenge);
  }

  public static bool HasAnyUnusedContent_StoryChallange()
  {
    return DLC_StatusHelper.HasAnyUnusedLevels(GameType.StoryChallange) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.StoryChallange);
  }

  public static bool HasAnyUnusedContent_Versus()
  {
    return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Versus) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Versus);
  }

  public static bool HasAnyUnusedContent_Campaing_Vanilla()
  {
    return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Campaign) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Campaign);
  }

  public static bool HasAnyUnusedContent_Campaing_Mythos()
  {
    return DLC_StatusHelper.HasAnyUnusedLevels(GameType.Mythos) || DLC_StatusHelper.HasAnyUnusedRobe(GameType.Mythos);
  }

  public static List<string> RobesThatIgnoreUsedStatus()
  {
    return DLC_StatusHelper.RobesThatIgnoreUsedStatus(GameType.Any);
  }

  public static List<string> RobesThatIgnoreUsedStatus(GameType gameType)
  {
    List<string> stringList = new List<string>()
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
        stringList.Add("wizardcul");
    }
    else
      stringList.Add("wizardcul");
    return stringList;
  }

  public static bool HasAnyUnusedRobe(GameType gameType)
  {
    IList<Profile.PlayableAvatar> values = Profile.Instance.Avatars.Values;
    uint appID = 0;
    List<string> stringList = DLC_StatusHelper.RobesThatIgnoreUsedStatus();
    foreach (Profile.PlayableAvatar playableAvatar in (IEnumerable<Profile.PlayableAvatar>) values)
    {
      if (!stringList.Contains(playableAvatar.Name))
      {
        HackHelper.License license = HackHelper.CheckLicense(playableAvatar);
        if (license != HackHelper.License.Custom && !DLC_StatusHelper.ValidateRobeLocked(license, playableAvatar, out appID) && (gameType != GameType.StoryChallange && gameType != GameType.Challenge || playableAvatar.AllowChallenge) && (gameType != GameType.Mythos && gameType != GameType.Campaign || playableAvatar.AllowCampaign) && (gameType != GameType.Versus || playableAvatar.AllowPVP) && DLC_StatusHelper.Instance.Item_IsUnused("robe", playableAvatar.Name, appID, false))
          return true;
      }
    }
    return false;
  }

  public static bool HasAnyUnusedItemPacks()
  {
    foreach (ItemPack itemPack in PackMan.Instance.ItemPacks)
    {
      if (!itemPack.IsUsed && itemPack.License == HackHelper.License.Yes)
        return true;
    }
    return false;
  }

  public static bool HasAnyUnusedMagicPacks()
  {
    foreach (MagickPack magickPack in PackMan.Instance.MagickPacks)
    {
      if (!magickPack.IsUsed && magickPack.License == HackHelper.License.Yes)
        return true;
    }
    return false;
  }

  public static void TrySetAllItemsAndMagicsUsed()
  {
    foreach (ItemPack itemPack in PackMan.Instance.ItemPacks)
    {
      if (!itemPack.IsUsed)
        itemPack.SetUsed(false);
    }
    foreach (MagickPack magickPack in PackMan.Instance.MagickPacks)
    {
      if (!magickPack.IsUsed)
        magickPack.SetUsed(false);
    }
    DLC_StatusHelper.Instance.SaveLocalData();
  }

  public static bool HasAnyUnusedLevels(GameType gameType)
  {
    List<LevelNode> levelNodeList = new List<LevelNode>();
    GameType gameType1 = gameType;
    if ((uint) gameType1 <= 8U)
    {
      switch (gameType1 - (byte) 1)
      {
        case (GameType) 0:
          levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.VanillaCampaign);
          goto label_12;
        case GameType.Campaign:
          levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.Challenges);
          goto label_12;
        case GameType.Challenge:
          break;
        case GameType.Campaign | GameType.Challenge:
          levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.Versus);
          goto label_12;
        default:
          if (gameType1 == GameType.Mythos)
          {
            levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.MythosCampaign);
            goto label_12;
          }
          break;
      }
    }
    else if (gameType1 != GameType.Any)
    {
      if (gameType1 == GameType.StoryChallange)
      {
        levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.StoryChallanges);
        goto label_12;
      }
    }
    else
    {
      levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.VanillaCampaign);
      levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.Challenges);
      levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.MythosCampaign);
      levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.StoryChallanges);
      levelNodeList.AddRange((IEnumerable<LevelNode>) LevelManager.Instance.Versus);
      goto label_12;
    }
    return false;
label_12:
    if (levelNodeList.Count == 0)
      return false;
    uint appID = 0;
    foreach (LevelNode levelNode in levelNodeList)
    {
      HackHelper.License license = HackHelper.CheckLicense(levelNode);
      if (license != HackHelper.License.Custom && !DLC_StatusHelper.ValidateLevelLocked(license, levelNode, out appID) && DLC_StatusHelper.Instance.Item_IsUnused("level", levelNode.Name, appID, false))
        return true;
    }
    return false;
  }

  internal static bool ValidateLevelLocked(
    HackHelper.License license,
    LevelNode lvl,
    out uint appID)
  {
    uint storePageAppID = 0;
    return DLC_StatusHelper.ValidateLevelLocked(license, lvl, out appID, out storePageAppID);
  }

  internal static bool ValidateLevelLocked(
    HackHelper.License license,
    LevelNode lvl,
    out uint appID,
    out uint storePageAppID)
  {
    appID = storePageAppID = 0U;
    return license != HackHelper.License.Custom && DLC_StatusHelper.ValidateLevelLocked(lvl, out appID, out storePageAppID);
  }

  internal static bool ValidateLevelLocked(LevelNode lvl, out uint appID)
  {
    appID = 0U;
    uint storePageAppId = 0;
    return DLC_StatusHelper.ValidateLevelLocked(lvl, out appID, out storePageAppId);
  }

  internal static bool ValidateLevelLocked(LevelNode lvl, out uint appID, out uint storePageAppId)
  {
    appID = storePageAppId = 0U;
    if (lvl == null)
      return false;
    byte[] hashSum = lvl.HashSum;
    if (hashSum == null || hashSum.Length == 0)
      return false;
    bool appId = HashTable.GetAppID(hashSum, out appID);
    storePageAppId = appID;
    if (!appId || appID == 42910U)
      return false;
    bool flag = !Helper.CheckDLCID(appID);
    storePageAppId = DLC_StatusHelper.Instance.GetStorePageAppID(appID);
    return flag;
  }

  internal static bool ValidateRobeLocked(
    HackHelper.License license,
    Profile.PlayableAvatar avatar,
    out uint appID)
  {
    appID = 0U;
    uint storePageAppID = 0;
    return DLC_StatusHelper.ValidateRobeLocked(license, avatar, out appID, out storePageAppID);
  }

  internal static bool ValidateRobeLocked(
    HackHelper.License license,
    Profile.PlayableAvatar avatar,
    out uint appID,
    out uint storePageAppID)
  {
    appID = storePageAppID = 0U;
    return license != HackHelper.License.Custom && DLC_StatusHelper.ValidateRobeLocked(avatar, out appID, out storePageAppID);
  }

  internal static bool ValidateRobeLocked(Profile.PlayableAvatar avatar, out uint appID)
  {
    appID = 0U;
    uint storePageAppID = 0;
    return DLC_StatusHelper.ValidateRobeLocked(avatar, out appID, out storePageAppID);
  }

  internal static bool ValidateRobeLocked(
    Profile.PlayableAvatar avatar,
    out uint appID,
    out uint storePageAppID)
  {
    appID = storePageAppID = 0U;
    byte[] hashSum = avatar.HashSum;
    if (hashSum == null || hashSum.Length == 0)
      return false;
    bool appId = HashTable.GetAppID(hashSum, out appID);
    storePageAppID = appID;
    if (!appId || appID == 42910U)
      return false;
    bool flag = !Helper.CheckDLCID(appID);
    storePageAppID = DLC_StatusHelper.Instance.GetStorePageAppID(appID);
    return flag;
  }

  private struct DynamicPromotionLoadData(uint appID, string name, string img, string url)
  {
    public uint appID = appID;
    public string imgUrl = img;
    public string gotoUrl = url;
    public string name = name;
  }
}
