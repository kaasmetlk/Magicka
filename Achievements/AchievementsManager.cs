// Decompiled with JetBrains decompiler
// Type: Magicka.Achievements.AchievementsManager
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.DRM;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using SteamWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace Magicka.Achievements;

internal class AchievementsManager
{
  public const string GameCode = "magicka";
  public const string APIServerPath = "https://connect.paradoxplaza.com/api/";
  public const string AchievementsImagePath = "http://connect.paradoxplaza.com/media/achievements/";
  public const string GamesImagePath = "http://connect.paradoxplaza.com/media/games/";
  private static AchievementsManager sSingelton;
  private static volatile object sSingeltonLock = new object();
  public static readonly int ACHIEVEMENT_EARNED = "achievement-earned".GetHashCodeCustom();
  public static readonly int ACHIEVEMENT_UNLOCKED = "achievement-unlocked".GetHashCodeCustom();
  public static readonly int BTN_CLOSE = "btn-close".GetHashCodeCustom();
  public static readonly int BTN_ENTER = "btn-enter".GetHashCodeCustom();
  public static readonly int BTN_LOGIN = "btn-login".GetHashCodeCustom();
  public static readonly int BTN_LOGOUT = "btn-logout".GetHashCodeCustom();
  public static readonly int EARNED_THIS_SESSION = "earned-this-session".GetHashCodeCustom();
  public static readonly int EMBED = "embed".GetHashCodeCustom();
  public static readonly int ERROR_BAD_LOGIN = "error-bad-login".GetHashCodeCustom();
  public static readonly int ERROR_SERVICE_UNAVAILABLE = "error-service-unavailable".GetHashCodeCustom();
  public static readonly int ERROR_WRONG_PROFILE = "error-wrong-profile".GetHashCodeCustom();
  public static readonly int LOGIN = "login".GetHashCodeCustom();
  public static readonly int LOGOUT = "logout".GetHashCodeCustom();
  public static readonly int MENU_ACHIEVEMENTS = "menu-achievements".GetHashCodeCustom();
  public static readonly int MENU_GAMES = "menu-games".GetHashCodeCustom();
  public static readonly int MYACHIEVEMENTS = "myachievements".GetHashCodeCustom();
  public static readonly int MYGAMES = "mygames".GetHashCodeCustom();
  public static readonly int NUM_PP = "num-pp".GetHashCodeCustom();
  public static readonly int PASSWORD = "password".GetHashCodeCustom();
  public static readonly int SHARE = "share".GetHashCodeCustom();
  public static readonly int USERNAME = "username".GetHashCodeCustom();
  public static readonly int YOUR_RANK = "your-rank".GetHashCodeCustom();
  public static readonly int LOGIN_HEADER = "login-header".GetHashCodeCustom();
  private AchievementPopupWindow mPopupWindow;
  private Queue<int> mQueuedAchievements;
  private string mUsername;
  private string mSessionKey;
  private List<GameData> mGames = new List<GameData>(50);
  private List<AchievementData> mAchievements = new List<AchievementData>(50);
  private int mAwardedAchievements;
  private int mTotalPoints;
  private int mRank;
  private SHA256 mSHA256 = SHA256.Create();
  private Dictionary<string, AchievementsManager.TextureReference> mAchievementTextures = new Dictionary<string, AchievementsManager.TextureReference>();
  private Dictionary<string, AchievementsManager.TextureReference> mGameTextures = new Dictionary<string, AchievementsManager.TextureReference>();
  private AchievementsManager.PdxLanguage mLanguage = (AchievementsManager.PdxLanguage) -1;
  private Dictionary<int, string> mStrings = new Dictionary<int, string>();
  private ServerRequestResult mServerStatus;
  private string mLogInError;
  private bool mBusy;
  private Queue<AchievementsManager.WorkerTask> mWorkerQueue = new Queue<AchievementsManager.WorkerTask>();
  private Action<HttpWebResponse, object> mGetLanguageCallback;
  private Action<HttpWebResponse, object> mGetImageCallback;
  private Action<HttpWebResponse, object> mAwardAchievementCallback;
  private Action<HttpWebResponse, object> mLogInCallback;
  private Action<HttpWebResponse, object> mGetProfileCallback;
  private readonly string mGameHash;

  public static AchievementsManager Instance
  {
    get
    {
      if (AchievementsManager.sSingelton == null)
      {
        lock (AchievementsManager.sSingeltonLock)
        {
          if (AchievementsManager.sSingelton == null)
            AchievementsManager.sSingelton = new AchievementsManager();
        }
      }
      return AchievementsManager.sSingelton;
    }
  }

  private AchievementsManager()
  {
    this.mQueuedAchievements = new Queue<int>(32 /*0x20*/);
    this.mPopupWindow = new AchievementPopupWindow();
    this.mGetLanguageCallback = new Action<HttpWebResponse, object>(this.GetLanguageCallback);
    this.mAwardAchievementCallback = new Action<HttpWebResponse, object>(this.AwardAchievementCallback);
    this.mGetImageCallback = new Action<HttpWebResponse, object>(this.GetImageCallback);
    this.mLogInCallback = new Action<HttpWebResponse, object>(this.LogInCallback);
    this.mGetProfileCallback = new Action<HttpWebResponse, object>(this.GetProfileCallback);
    this.LoadDefaultLanguage();
    new Thread(new ThreadStart(this.Worker))
    {
      Name = "WebCalls"
    }.Start();
    this.mGameHash = BitConverter.ToString(this.mSHA256.ComputeHash((Stream) System.IO.File.OpenRead(Application.ExecutablePath))).Replace("-", "").ToLowerInvariant();
  }

  private void Worker()
  {
    bool working = true;
    Game.Instance.Exiting += (EventHandler) delegate
    {
      working = false;
    };
    while (working)
    {
      if (this.mWorkerQueue.Count == 0)
      {
        this.mBusy = false;
        Thread.Sleep(1);
      }
      else
      {
        AchievementsManager.WorkerTask workerTask = this.mWorkerQueue.Dequeue();
        HttpWebResponse httpWebResponse;
        try
        {
          workerTask.Request.Timeout = 5000;
          httpWebResponse = (HttpWebResponse) workerTask.Request.GetResponse();
        }
        catch
        {
          httpWebResponse = (HttpWebResponse) null;
        }
        if (workerTask.ResponseCallback != null)
          workerTask.ResponseCallback(httpWebResponse, workerTask.CallbackArgument);
      }
    }
  }

  public void SetLanguage(Language iLanguage)
  {
    string requestUriString = "https://connect.paradoxplaza.com/api/uistrings/?langCode=" + ((AchievementsManager.PdxLanguage) iLanguage).ToString();
    AchievementsManager.WorkerTask workerTask;
    workerTask.Request = (HttpWebRequest) WebRequest.Create(requestUriString);
    workerTask.ResponseCallback = this.mGetLanguageCallback;
    workerTask.CallbackArgument = (object) null;
    this.mWorkerQueue.Enqueue(workerTask);
    this.mServerStatus = ServerRequestResult.PENDING;
    this.mBusy = true;
  }

  private void GetLanguageCallback(HttpWebResponse iResponse, object iArg)
  {
    if (iResponse == null)
    {
      this.LoadDefaultLanguage();
      this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
      AchievementsManager.Instance.OnLanguageChanged();
    }
    else if (iResponse.StatusCode == HttpStatusCode.OK)
    {
      Stream responseStream = iResponse.GetResponseStream();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(responseStream);
      responseStream.Close();
      responseStream.Dispose();
      XmlNode xmlNode = (XmlNode) null;
      for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlDocument.ChildNodes[i];
        if (!(childNode is XmlComment) && childNode.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
        {
          xmlNode = childNode;
          break;
        }
      }
      for (int i = 0; i < xmlNode.Attributes.Count; ++i)
      {
        XmlAttribute attribute = xmlNode.Attributes[i];
        if (attribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
          this.LoadDefaultLanguage();
          this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
          AchievementsManager.Instance.OnLanguageChanged();
          return;
        }
      }
      this.LoadDefaultLanguage();
      lock (this.mStrings)
      {
        if (xmlNode != null)
        {
          for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
          {
            XmlNode childNode = xmlNode.ChildNodes[i1];
            if (!(childNode is XmlComment))
            {
              if (childNode.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
              {
                string str1 = (string) null;
                string str2 = (string) null;
                for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
                {
                  XmlAttribute attribute = childNode.Attributes[i2];
                  if (attribute.Name.Equals("key"))
                    str1 = attribute.Value;
                  else if (attribute.Name.Equals("value"))
                    str2 = attribute.Value;
                }
                this.mStrings[str1.ToLowerInvariant().GetHashCodeCustom()] = str2;
              }
              else if (childNode.Name.Equals("language", StringComparison.OrdinalIgnoreCase))
              {
                for (int i3 = 0; i3 < childNode.Attributes.Count; ++i3)
                {
                  XmlAttribute attribute = childNode.Attributes[i3];
                  if (attribute.Name.Equals("code"))
                    this.mLanguage = (AchievementsManager.PdxLanguage) System.Enum.Parse(typeof (AchievementsManager.PdxLanguage), attribute.Value, true);
                }
              }
            }
          }
        }
      }
      this.mServerStatus = ServerRequestResult.SUCCESS;
      AchievementsManager.Instance.OnLanguageChanged();
    }
    else
    {
      this.LoadDefaultLanguage();
      this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
      AchievementsManager.Instance.OnLanguageChanged();
    }
  }

  private void LoadDefaultLanguage()
  {
    this.mLanguage = AchievementsManager.PdxLanguage.en_US;
    lock (this.mStrings)
    {
      this.mStrings.Clear();
      this.mStrings.Add(AchievementsManager.ACHIEVEMENT_EARNED, "Earned %s");
      this.mStrings.Add(AchievementsManager.ACHIEVEMENT_UNLOCKED, "ACHIEVEMENT UNLOCKED:");
      this.mStrings.Add(AchievementsManager.BTN_CLOSE, "CLOSE");
      this.mStrings.Add(AchievementsManager.BTN_ENTER, "ENTER");
      this.mStrings.Add(AchievementsManager.BTN_LOGIN, "LOGIN");
      this.mStrings.Add(AchievementsManager.BTN_LOGOUT, "LOGOUT");
      this.mStrings.Add(AchievementsManager.EARNED_THIS_SESSION, "THIS SESSION");
      this.mStrings.Add(AchievementsManager.EMBED, "Embed");
      this.mStrings.Add(AchievementsManager.ERROR_BAD_LOGIN, "Wrong Forum username or password");
      this.mStrings.Add(AchievementsManager.ERROR_SERVICE_UNAVAILABLE, "Service is temporarily unavailable.\nPlease try again later.");
      this.mStrings.Add(AchievementsManager.ERROR_WRONG_PROFILE, "This profile is locked to a different login");
      this.mStrings.Add(AchievementsManager.LOGIN, "Login");
      this.mStrings.Add(AchievementsManager.LOGOUT, "Logout");
      this.mStrings.Add(AchievementsManager.MENU_ACHIEVEMENTS, "MY ACHIEVEMENTS (%d/%d)");
      this.mStrings.Add(AchievementsManager.MENU_GAMES, "MY GAMES (%d)");
      this.mStrings.Add(AchievementsManager.MYACHIEVEMENTS, "My Achievements");
      this.mStrings.Add(AchievementsManager.MYGAMES, "My Games");
      this.mStrings.Add(AchievementsManager.NUM_PP, "%d PP");
      this.mStrings.Add(AchievementsManager.PASSWORD, "Password");
      this.mStrings.Add(AchievementsManager.SHARE, "Share");
      this.mStrings.Add(AchievementsManager.USERNAME, "Username");
      this.mStrings.Add(AchievementsManager.YOUR_RANK, "YOUR RANK:");
      this.mStrings.Add(AchievementsManager.LOGIN_HEADER, "Log in using your Paradox Forum username");
    }
  }

  public string GetTranslation(int iHash)
  {
    lock (this.mStrings)
      return this.mStrings[iHash];
  }

  internal bool HasAchievement(string iAchievement)
  {
    bool pbAchieved = false;
    SteamUserStats.GetAchievement(iAchievement, out pbAchieved);
    return pbAchieved;
  }

  internal void AwardAchievement(PlayState iPlayState, string iAchievementCode)
  {
    if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
      return;
    bool pbAchieved;
    if (SteamUserStats.GetAchievement(iAchievementCode, out pbAchieved) && !pbAchieved)
    {
      SteamUserStats.SetAchievement(iAchievementCode);
      SteamUserStats.StoreStats();
    }
    for (int index = 0; index < this.mAchievements.Count; ++index)
    {
      AchievementData mAchievement = this.mAchievements[index];
      if (mAchievement.Code.Equals(iAchievementCode))
      {
        if (mAchievement.Achieved)
          break;
        string lowerInvariant = BitConverter.ToString(this.mSHA256.ComputeHash(Encoding.ASCII.GetBytes($"jtp93qs{this.mSessionKey}kkvq3oa{iAchievementCode}ijrstwn"))).Replace("-", "").ToLowerInvariant();
        string requestUriString = $"{"https://connect.paradoxplaza.com/api/"}award_achievement/?sessionKey={this.mSessionKey}&achievementCode={iAchievementCode}&signature={lowerInvariant}";
        this.mWorkerQueue.Enqueue(new AchievementsManager.WorkerTask()
        {
          Request = (HttpWebRequest) WebRequest.Create(requestUriString),
          ResponseCallback = this.mAwardAchievementCallback,
          CallbackArgument = (object) index
        });
        break;
      }
    }
  }

  private void AwardAchievementCallback(HttpWebResponse iResponse, object iArg)
  {
    if (iResponse == null)
      return;
    if (iResponse.StatusCode != HttpStatusCode.OK)
      return;
    try
    {
      Stream responseStream = iResponse.GetResponseStream();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(responseStream);
      responseStream.Close();
      responseStream.Dispose();
      XmlNode xmlNode = (XmlNode) null;
      for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlDocument.ChildNodes[i];
        if (!(childNode is XmlComment) && childNode.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
        {
          xmlNode = childNode;
          break;
        }
      }
      for (int i = 0; i < xmlNode.Attributes.Count; ++i)
      {
        XmlAttribute attribute = xmlNode.Attributes[i];
        if (attribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && attribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
          int index = (int) iArg;
          if (this.mAchievements[index].Achieved)
            break;
          AchievementData mAchievement = this.mAchievements[index] with
          {
            Achieved = true
          };
          this.mTotalPoints += mAchievement.Points;
          this.mQueuedAchievements.Enqueue(index);
          this.mAchievements[index] = mAchievement;
          break;
        }
      }
    }
    catch
    {
    }
  }

  public void Update(DataChannel iDataChannel, float iDeltaTime)
  {
    if (this.mPopupWindow.Visible)
      this.mPopupWindow.Update(iDataChannel, iDeltaTime);
    else if (this.mQueuedAchievements.Count > 0)
    {
      int index = this.mQueuedAchievements.Dequeue();
      if (this.mAchievements.Count > 0)
        this.mPopupWindow.Show(this.mAchievements[index]);
    }
    if (PdxLoginWindow.Instance.Visible)
      PdxLoginWindow.Instance.Update(iDataChannel, iDeltaTime);
    if (!PdxWidget.Instance.Visible)
      return;
    PdxWidget.Instance.Update(iDataChannel, iDeltaTime);
  }

  public void OnLanguageChanged()
  {
    this.mPopupWindow.OnLanguageChanged();
    PdxLoginWindow.Instance.OnLanguageChanged();
  }

  public void LogIn(string iUsername, string iPassword)
  {
    this.mBusy = true;
    this.mLogInError = (string) null;
    this.mUsername = iUsername;
    Uri requestUri = new Uri($"{"https://connect.paradoxplaza.com/api/"}login/?username={iUsername}&password={HttpUtility.UrlEncode(iPassword)}&gameCode={"magicka"}", UriKind.Absolute);
    this.mWorkerQueue.Enqueue(new AchievementsManager.WorkerTask()
    {
      Request = (HttpWebRequest) WebRequest.Create(requestUri),
      ResponseCallback = this.mLogInCallback
    });
  }

  private void LogInCallback(HttpWebResponse iResponse, object iArgs)
  {
    if (iResponse == null)
    {
      this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
      this.mBusy = false;
    }
    else if (iResponse.StatusCode != HttpStatusCode.OK)
    {
      this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
      this.mBusy = false;
    }
    else if (iResponse.StatusCode == HttpStatusCode.OK)
    {
      Stream responseStream = iResponse.GetResponseStream();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(responseStream);
      responseStream.Close();
      responseStream.Dispose();
      XmlNode xmlNode = (XmlNode) null;
      for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlDocument.ChildNodes[i];
        if (!(childNode is XmlComment) && childNode.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
        {
          xmlNode = childNode;
          break;
        }
      }
      for (int i = 0; i < xmlNode.Attributes.Count; ++i)
      {
        XmlAttribute attribute = xmlNode.Attributes[i];
        if (attribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
          this.mLogInError = attribute.Value;
          this.mServerStatus = ServerRequestResult.SUCCESS;
          return;
        }
      }
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlNode childNode = xmlNode.ChildNodes[i1];
        if (!(childNode is XmlComment))
        {
          if (childNode.Name.Equals("session", StringComparison.OrdinalIgnoreCase))
          {
            for (int i2 = 0; i2 < childNode.Attributes.Count; ++i2)
            {
              XmlAttribute attribute = childNode.Attributes[i2];
              if (attribute.Name.Equals("key", StringComparison.OrdinalIgnoreCase))
                this.mSessionKey = attribute.Value;
            }
          }
          else if (childNode.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
          {
            for (int i3 = 0; i3 < childNode.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode.Attributes[i3];
              if (attribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals(this.mUsername, StringComparison.OrdinalIgnoreCase))
              {
                this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
                this.mSessionKey = (string) null;
                return;
              }
            }
          }
          else if (childNode.Name.Equals("game", StringComparison.OrdinalIgnoreCase))
          {
            for (int i4 = 0; i4 < childNode.Attributes.Count; ++i4)
            {
              XmlAttribute attribute = childNode.Attributes[i4];
              if (attribute.Name.Equals("code", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals("magicka", StringComparison.OrdinalIgnoreCase))
              {
                this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
                this.mSessionKey = (string) null;
                return;
              }
            }
          }
        }
      }
      this.mServerStatus = ServerRequestResult.SUCCESS;
      this.mRank = 0;
      string requestUriString = $"{"https://connect.paradoxplaza.com/api/"}getprofile/?username={this.mUsername}&langCode={AchievementsManager.Instance.CurrentLanguage.ToString()}";
      this.mWorkerQueue.Enqueue(new AchievementsManager.WorkerTask()
      {
        Request = (HttpWebRequest) WebRequest.Create(requestUriString),
        ResponseCallback = this.mGetProfileCallback
      });
      this.mBusy = true;
    }
    else
    {
      this.mSessionKey = (string) null;
      this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
    }
  }

  private void GetProfileCallback(HttpWebResponse iResponse, object iArg)
  {
    if (iResponse == null)
      this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
    else if (iResponse.StatusCode == HttpStatusCode.OK)
    {
      Stream responseStream = iResponse.GetResponseStream();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(responseStream);
      responseStream.Close();
      responseStream.Dispose();
      XmlNode xmlNode = (XmlNode) null;
      for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
      {
        XmlNode childNode = xmlDocument.ChildNodes[i];
        if (!(childNode is XmlComment) && childNode.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
        {
          xmlNode = childNode;
          break;
        }
      }
      for (int i = 0; i < xmlNode.Attributes.Count; ++i)
      {
        XmlAttribute attribute = xmlNode.Attributes[i];
        if (attribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
          this.mLogInError = attribute.Value;
          this.mServerStatus = ServerRequestResult.SUCCESS;
          return;
        }
      }
      for (int i1 = 0; i1 < xmlNode.ChildNodes.Count; ++i1)
      {
        XmlNode childNode1 = xmlNode.ChildNodes[i1];
        if (!(childNode1 is XmlComment))
        {
          if (childNode1.Name.Equals("game", StringComparison.OrdinalIgnoreCase))
          {
            GameData oData1;
            GameData.ParseXml(childNode1, out oData1);
            this.mGames.Add(oData1);
            if (oData1.Code.Equals("magicka", StringComparison.OrdinalIgnoreCase))
            {
              for (int i2 = 0; i2 < childNode1.ChildNodes.Count; ++i2)
              {
                XmlNode childNode2 = childNode1.ChildNodes[i2];
                if (!(childNode2 is XmlComment) && childNode2.Name.Equals("achievement", StringComparison.OrdinalIgnoreCase))
                {
                  AchievementData oData2;
                  AchievementData.ParseXml(childNode2, out oData2);
                  this.mAchievements.Add(oData2);
                }
              }
            }
          }
          else if (childNode1.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
          {
            for (int i3 = 0; i3 < childNode1.Attributes.Count; ++i3)
            {
              XmlAttribute attribute = childNode1.Attributes[i3];
              if (attribute.Name.Equals("name", StringComparison.OrdinalIgnoreCase) && !attribute.Value.Equals(this.mUsername, StringComparison.OrdinalIgnoreCase))
              {
                this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
                this.mSessionKey = (string) null;
                return;
              }
            }
          }
          else if (childNode1.Name.Equals("rank", StringComparison.OrdinalIgnoreCase))
          {
            for (int i4 = 0; i4 < childNode1.Attributes.Count; ++i4)
            {
              XmlAttribute attribute = childNode1.Attributes[i4];
              if (attribute.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
                this.mRank = !string.IsNullOrEmpty(attribute.Value) ? int.Parse(attribute.Value) : 0;
            }
          }
        }
      }
      this.mAchievements.Sort();
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.mAchievements.Count; ++index)
      {
        if (this.mAchievements[index].Achieved)
        {
          ++num1;
          num2 += this.mAchievements[index].Points;
        }
      }
      this.mAwardedAchievements = num1;
      this.mTotalPoints = num2;
      this.OnProfileUpdate();
      this.mServerStatus = ServerRequestResult.SUCCESS;
    }
    else
      this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
  }

  public ServerRequestResult Status => this.mServerStatus;

  public string LogInError => this.mLogInError;

  public List<AchievementData> Achievements => this.mAchievements;

  public int AwardedAchievements => this.mAwardedAchievements;

  public int TotalPoints => this.mTotalPoints;

  public List<GameData> Games => this.mGames;

  public int Rank => this.mRank;

  private void OnProfileUpdate() => PdxWidget.Instance.OnProfileUpdate();

  public Texture2D GetAchievementImage(string iAchievementCode)
  {
    lock (this.mAchievementTextures)
    {
      AchievementsManager.TextureReference textureReference1;
      if (this.mAchievementTextures.TryGetValue(iAchievementCode, out textureReference1))
        return textureReference1.Texture;
      AchievementsManager.TextureReference textureReference2 = new AchievementsManager.TextureReference();
      textureReference2.Name = iAchievementCode;
      string requestUriString = $"{"http://connect.paradoxplaza.com/media/achievements/"}{iAchievementCode}_igw.png";
      AchievementsManager.WorkerTask workerTask;
      workerTask.Request = (HttpWebRequest) WebRequest.Create(requestUriString);
      workerTask.CallbackArgument = (object) textureReference2;
      workerTask.ResponseCallback = this.mGetImageCallback;
      this.mWorkerQueue.Enqueue(workerTask);
      this.mAchievementTextures.Add(textureReference2.Name, textureReference2);
      return (Texture2D) null;
    }
  }

  public Texture2D GetGameImage(string iGameCode)
  {
    lock (this.mGameTextures)
    {
      AchievementsManager.TextureReference textureReference1;
      if (this.mGameTextures.TryGetValue(iGameCode, out textureReference1))
        return textureReference1.Texture;
      AchievementsManager.TextureReference textureReference2 = new AchievementsManager.TextureReference();
      textureReference2.Name = iGameCode;
      string requestUriString = $"{"http://connect.paradoxplaza.com/media/games/"}{iGameCode}_igw.png";
      AchievementsManager.WorkerTask workerTask;
      workerTask.Request = (HttpWebRequest) WebRequest.Create(requestUriString);
      workerTask.CallbackArgument = (object) textureReference2;
      workerTask.ResponseCallback = this.mGetImageCallback;
      this.mWorkerQueue.Enqueue(workerTask);
      this.mGameTextures.Add(textureReference2.Name, textureReference2);
      return (Texture2D) null;
    }
  }

  private void GetImageCallback(HttpWebResponse iResponse, object iArg)
  {
    if (iResponse == null)
      return;
    if (iResponse.StatusCode != HttpStatusCode.OK)
      return;
    try
    {
      AchievementsManager.TextureReference textureReference = iArg as AchievementsManager.TextureReference;
      Stream responseStream = iResponse.GetResponseStream();
      MemoryStream textureStream = new MemoryStream();
      byte[] buffer = new byte[16384 /*0x4000*/];
      while (true)
      {
        int count = responseStream.Read(buffer, 0, buffer.Length);
        if (count > 0)
          textureStream.Write(buffer, 0, count);
        else
          break;
      }
      responseStream.Close();
      responseStream.Dispose();
      textureStream.Position = 0L;
      GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
      lock (graphicsDevice)
        textureReference.Texture = Texture2D.FromFile(graphicsDevice, (Stream) textureStream);
      textureStream.Close();
      textureStream.Dispose();
    }
    catch (WebException ex)
    {
    }
  }

  public bool Busy => this.mBusy;

  public bool LoggedIn => this.mSessionKey != null;

  public AchievementsManager.PdxLanguage CurrentLanguage => this.mLanguage;

  internal void LogOut()
  {
    if (string.IsNullOrEmpty(this.mSessionKey))
      return;
    string requestUriString = string.Format("{0}logout/?sessionKey=", (object) "https://connect.paradoxplaza.com/api/", (object) this.mSessionKey);
    this.mSessionKey = (string) null;
    AchievementsManager.WorkerTask workerTask;
    workerTask.CallbackArgument = (object) null;
    workerTask.Request = (HttpWebRequest) WebRequest.Create(requestUriString);
    workerTask.ResponseCallback = (Action<HttpWebResponse, object>) null;
    this.mAchievements.Clear();
    this.mGames.Clear();
    this.mWorkerQueue.Enqueue(workerTask);
  }

  internal void Request(
    string iURI,
    object iCallbackArgument,
    Action<HttpWebResponse, object> iCallback)
  {
    AchievementsManager.WorkerTask workerTask;
    workerTask.CallbackArgument = iCallbackArgument;
    workerTask.Request = (HttpWebRequest) WebRequest.Create(iURI);
    workerTask.ResponseCallback = iCallback;
    this.mWorkerQueue.Enqueue(workerTask);
  }

  internal enum PdxLanguage
  {
    en_US = 9,
    es = 10, // 0x0000000A
    sv_SE = 29, // 0x0000001D
  }

  private class TextureReference
  {
    public string Name;
    public Texture2D Texture;
  }

  private struct WorkerTask
  {
    public HttpWebRequest Request;
    public Action<HttpWebResponse, object> ResponseCallback;
    public object CallbackArgument;
  }
}
