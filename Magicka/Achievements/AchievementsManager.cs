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
using Magicka.DRM;
using Magicka.GameLogic.GameStates;
using Magicka.Localization;
using Microsoft.Xna.Framework.Graphics;
using PolygonHead;
using SteamWrapper;

namespace Magicka.Achievements
{
	// Token: 0x0200061C RID: 1564
	internal class AchievementsManager
	{
		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x0017B53C File Offset: 0x0017973C
		public static AchievementsManager Instance
		{
			get
			{
				if (AchievementsManager.sSingelton == null)
				{
					lock (AchievementsManager.sSingeltonLock)
					{
						if (AchievementsManager.sSingelton == null)
						{
							AchievementsManager.sSingelton = new AchievementsManager();
						}
					}
				}
				return AchievementsManager.sSingelton;
			}
		}

		// Token: 0x06002EC8 RID: 11976 RVA: 0x0017B590 File Offset: 0x00179790
		private AchievementsManager()
		{
			this.mQueuedAchievements = new Queue<int>(32);
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
			FileStream inputStream = File.OpenRead(Application.ExecutablePath);
			this.mGameHash = BitConverter.ToString(this.mSHA256.ComputeHash(inputStream)).Replace("-", "").ToLowerInvariant();
		}

		// Token: 0x06002EC9 RID: 11977 RVA: 0x0017B6E0 File Offset: 0x001798E0
		private void Worker()
		{
			bool working = true;
			Game.Instance.Exiting += delegate(object A_1, EventArgs A_2)
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
						httpWebResponse = (HttpWebResponse)workerTask.Request.GetResponse();
					}
					catch
					{
						httpWebResponse = null;
					}
					if (workerTask.ResponseCallback != null)
					{
						workerTask.ResponseCallback.Invoke(httpWebResponse, workerTask.CallbackArgument);
					}
				}
			}
		}

		// Token: 0x06002ECA RID: 11978 RVA: 0x0017B79C File Offset: 0x0017999C
		public void SetLanguage(Language iLanguage)
		{
			string requestUriString = "https://connect.paradoxplaza.com/api/uistrings/?langCode=" + ((AchievementsManager.PdxLanguage)iLanguage).ToString();
			AchievementsManager.WorkerTask item;
			item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
			item.ResponseCallback = this.mGetLanguageCallback;
			item.CallbackArgument = null;
			this.mWorkerQueue.Enqueue(item);
			this.mServerStatus = ServerRequestResult.PENDING;
			this.mBusy = true;
		}

		// Token: 0x06002ECB RID: 11979 RVA: 0x0017B800 File Offset: 0x00179A00
		private void GetLanguageCallback(HttpWebResponse iResponse, object iArg)
		{
			if (iResponse == null)
			{
				this.LoadDefaultLanguage();
				this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
				AchievementsManager.Instance.OnLanguageChanged();
				return;
			}
			if (iResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = iResponse.GetResponseStream();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(responseStream);
				responseStream.Close();
				responseStream.Dispose();
				XmlNode xmlNode = null;
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
					if (!(xmlNode2 is XmlComment) && xmlNode2.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode = xmlNode2;
						break;
					}
				}
				for (int j = 0; j < xmlNode.Attributes.Count; j++)
				{
					XmlAttribute xmlAttribute = xmlNode.Attributes[j];
					if (xmlAttribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !xmlAttribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
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
						for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
						{
							XmlNode xmlNode3 = xmlNode.ChildNodes[k];
							if (!(xmlNode3 is XmlComment))
							{
								if (xmlNode3.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
								{
									string text = null;
									string value = null;
									for (int l = 0; l < xmlNode3.Attributes.Count; l++)
									{
										XmlAttribute xmlAttribute2 = xmlNode3.Attributes[l];
										if (xmlAttribute2.Name.Equals("key"))
										{
											text = xmlAttribute2.Value;
										}
										else if (xmlAttribute2.Name.Equals("value"))
										{
											value = xmlAttribute2.Value;
										}
									}
									this.mStrings[text.ToLowerInvariant().GetHashCodeCustom()] = value;
								}
								else if (xmlNode3.Name.Equals("language", StringComparison.OrdinalIgnoreCase))
								{
									for (int m = 0; m < xmlNode3.Attributes.Count; m++)
									{
										XmlAttribute xmlAttribute3 = xmlNode3.Attributes[m];
										if (xmlAttribute3.Name.Equals("code"))
										{
											this.mLanguage = (AchievementsManager.PdxLanguage)Enum.Parse(typeof(AchievementsManager.PdxLanguage), xmlAttribute3.Value, true);
										}
									}
								}
							}
						}
					}
				}
				this.mServerStatus = ServerRequestResult.SUCCESS;
				AchievementsManager.Instance.OnLanguageChanged();
				return;
			}
			this.LoadDefaultLanguage();
			this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
			AchievementsManager.Instance.OnLanguageChanged();
		}

		// Token: 0x06002ECC RID: 11980 RVA: 0x0017BAC0 File Offset: 0x00179CC0
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

		// Token: 0x06002ECD RID: 11981 RVA: 0x0017BCF8 File Offset: 0x00179EF8
		public string GetTranslation(int iHash)
		{
			string result;
			lock (this.mStrings)
			{
				result = this.mStrings[iHash];
			}
			return result;
		}

		// Token: 0x06002ECE RID: 11982 RVA: 0x0017BD3C File Offset: 0x00179F3C
		internal bool HasAchievement(string iAchievement)
		{
			bool result = false;
			SteamUserStats.GetAchievement(iAchievement, out result);
			return result;
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x0017BD58 File Offset: 0x00179F58
		internal void AwardAchievement(PlayState iPlayState, string iAchievementCode)
		{
			if (HackHelper.LicenseStatus == HackHelper.Status.Hacked)
			{
				return;
			}
			bool flag;
			if (SteamUserStats.GetAchievement(iAchievementCode, out flag) && !flag)
			{
				SteamUserStats.SetAchievement(iAchievementCode);
				SteamUserStats.StoreStats();
			}
			int i = 0;
			while (i < this.mAchievements.Count)
			{
				AchievementData achievementData = this.mAchievements[i];
				if (achievementData.Code.Equals(iAchievementCode))
				{
					if (achievementData.Achieved)
					{
						return;
					}
					string s = string.Format("jtp93qs{0}kkvq3oa{1}ijrstwn", this.mSessionKey, iAchievementCode);
					byte[] bytes = Encoding.ASCII.GetBytes(s);
					string text = BitConverter.ToString(this.mSHA256.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
					string requestUriString = string.Format("{0}award_achievement/?sessionKey={1}&achievementCode={2}&signature={3}", new object[]
					{
						"https://connect.paradoxplaza.com/api/",
						this.mSessionKey,
						iAchievementCode,
						text
					});
					AchievementsManager.WorkerTask item = default(AchievementsManager.WorkerTask);
					item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
					item.ResponseCallback = this.mAwardAchievementCallback;
					item.CallbackArgument = i;
					this.mWorkerQueue.Enqueue(item);
					return;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x0017BE8C File Offset: 0x0017A08C
		private void AwardAchievementCallback(HttpWebResponse iResponse, object iArg)
		{
			if (iResponse == null || iResponse.StatusCode != HttpStatusCode.OK)
			{
				return;
			}
			try
			{
				Stream responseStream = iResponse.GetResponseStream();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(responseStream);
				responseStream.Close();
				responseStream.Dispose();
				XmlNode xmlNode = null;
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
					if (!(xmlNode2 is XmlComment) && xmlNode2.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode = xmlNode2;
						break;
					}
				}
				for (int j = 0; j < xmlNode.Attributes.Count; j++)
				{
					XmlAttribute xmlAttribute = xmlNode.Attributes[j];
					if (xmlAttribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && xmlAttribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
					{
						int num = (int)iArg;
						if (!this.mAchievements[num].Achieved)
						{
							AchievementData value = this.mAchievements[num];
							value.Achieved = true;
							this.mTotalPoints += value.Points;
							this.mQueuedAchievements.Enqueue(num);
							this.mAchievements[num] = value;
						}
						break;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x0017BFF0 File Offset: 0x0017A1F0
		public void Update(DataChannel iDataChannel, float iDeltaTime)
		{
			if (this.mPopupWindow.Visible)
			{
				this.mPopupWindow.Update(iDataChannel, iDeltaTime);
			}
			else if (this.mQueuedAchievements.Count > 0)
			{
				int index = this.mQueuedAchievements.Dequeue();
				if (this.mAchievements.Count > 0)
				{
					this.mPopupWindow.Show(this.mAchievements[index]);
				}
			}
			if (PdxLoginWindow.Instance.Visible)
			{
				PdxLoginWindow.Instance.Update(iDataChannel, iDeltaTime);
			}
			if (PdxWidget.Instance.Visible)
			{
				PdxWidget.Instance.Update(iDataChannel, iDeltaTime);
			}
		}

		// Token: 0x06002ED2 RID: 11986 RVA: 0x0017C088 File Offset: 0x0017A288
		public void OnLanguageChanged()
		{
			this.mPopupWindow.OnLanguageChanged();
			PdxLoginWindow.Instance.OnLanguageChanged();
		}

		// Token: 0x06002ED3 RID: 11987 RVA: 0x0017C0A0 File Offset: 0x0017A2A0
		public void LogIn(string iUsername, string iPassword)
		{
			this.mBusy = true;
			this.mLogInError = null;
			this.mUsername = iUsername;
			string uriString = string.Format("{0}login/?username={1}&password={2}&gameCode={3}", new object[]
			{
				"https://connect.paradoxplaza.com/api/",
				iUsername,
				HttpUtility.UrlEncode(iPassword),
				"magicka"
			});
			Uri requestUri = new Uri(uriString, UriKind.Absolute);
			AchievementsManager.WorkerTask item = default(AchievementsManager.WorkerTask);
			item.Request = (HttpWebRequest)WebRequest.Create(requestUri);
			item.ResponseCallback = this.mLogInCallback;
			this.mWorkerQueue.Enqueue(item);
		}

		// Token: 0x06002ED4 RID: 11988 RVA: 0x0017C130 File Offset: 0x0017A330
		private void LogInCallback(HttpWebResponse iResponse, object iArgs)
		{
			if (iResponse == null)
			{
				this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
				this.mBusy = false;
				return;
			}
			if (iResponse.StatusCode != HttpStatusCode.OK)
			{
				this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
				this.mBusy = false;
				return;
			}
			if (iResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = iResponse.GetResponseStream();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(responseStream);
				responseStream.Close();
				responseStream.Dispose();
				XmlNode xmlNode = null;
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
					if (!(xmlNode2 is XmlComment) && xmlNode2.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode = xmlNode2;
						break;
					}
				}
				for (int j = 0; j < xmlNode.Attributes.Count; j++)
				{
					XmlAttribute xmlAttribute = xmlNode.Attributes[j];
					if (xmlAttribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !xmlAttribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
					{
						this.mLogInError = xmlAttribute.Value;
						this.mServerStatus = ServerRequestResult.SUCCESS;
						return;
					}
				}
				for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
				{
					XmlNode xmlNode3 = xmlNode.ChildNodes[k];
					if (!(xmlNode3 is XmlComment))
					{
						if (xmlNode3.Name.Equals("session", StringComparison.OrdinalIgnoreCase))
						{
							for (int l = 0; l < xmlNode3.Attributes.Count; l++)
							{
								XmlAttribute xmlAttribute2 = xmlNode3.Attributes[l];
								if (xmlAttribute2.Name.Equals("key", StringComparison.OrdinalIgnoreCase))
								{
									this.mSessionKey = xmlAttribute2.Value;
								}
							}
						}
						else if (xmlNode3.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
						{
							for (int m = 0; m < xmlNode3.Attributes.Count; m++)
							{
								XmlAttribute xmlAttribute3 = xmlNode3.Attributes[m];
								if (xmlAttribute3.Name.Equals("name", StringComparison.OrdinalIgnoreCase) && !xmlAttribute3.Value.Equals(this.mUsername, StringComparison.OrdinalIgnoreCase))
								{
									this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
									this.mSessionKey = null;
									return;
								}
							}
						}
						else if (xmlNode3.Name.Equals("game", StringComparison.OrdinalIgnoreCase))
						{
							for (int n = 0; n < xmlNode3.Attributes.Count; n++)
							{
								XmlAttribute xmlAttribute4 = xmlNode3.Attributes[n];
								if (xmlAttribute4.Name.Equals("code", StringComparison.OrdinalIgnoreCase) && !xmlAttribute4.Value.Equals("magicka", StringComparison.OrdinalIgnoreCase))
								{
									this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
									this.mSessionKey = null;
									return;
								}
							}
						}
					}
				}
				this.mServerStatus = ServerRequestResult.SUCCESS;
				this.mRank = 0;
				string requestUriString = string.Format("{0}getprofile/?username={1}&langCode={2}", "https://connect.paradoxplaza.com/api/", this.mUsername, AchievementsManager.Instance.CurrentLanguage.ToString());
				AchievementsManager.WorkerTask item = default(AchievementsManager.WorkerTask);
				item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
				item.ResponseCallback = this.mGetProfileCallback;
				this.mWorkerQueue.Enqueue(item);
				this.mBusy = true;
				return;
			}
			this.mSessionKey = null;
			this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x0017C45C File Offset: 0x0017A65C
		private void GetProfileCallback(HttpWebResponse iResponse, object iArg)
		{
			if (iResponse == null)
			{
				this.mServerStatus = ServerRequestResult.ERR_SERVICE_UNAVAILABLE;
				return;
			}
			if (iResponse.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = iResponse.GetResponseStream();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(responseStream);
				responseStream.Close();
				responseStream.Dispose();
				XmlNode xmlNode = null;
				for (int i = 0; i < xmlDocument.ChildNodes.Count; i++)
				{
					XmlNode xmlNode2 = xmlDocument.ChildNodes[i];
					if (!(xmlNode2 is XmlComment) && xmlNode2.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
					{
						xmlNode = xmlNode2;
						break;
					}
				}
				for (int j = 0; j < xmlNode.Attributes.Count; j++)
				{
					XmlAttribute xmlAttribute = xmlNode.Attributes[j];
					if (xmlAttribute.Name.Equals("result", StringComparison.OrdinalIgnoreCase) && !xmlAttribute.Value.Equals("success", StringComparison.OrdinalIgnoreCase))
					{
						this.mLogInError = xmlAttribute.Value;
						this.mServerStatus = ServerRequestResult.SUCCESS;
						return;
					}
				}
				for (int k = 0; k < xmlNode.ChildNodes.Count; k++)
				{
					XmlNode xmlNode3 = xmlNode.ChildNodes[k];
					if (!(xmlNode3 is XmlComment))
					{
						if (xmlNode3.Name.Equals("game", StringComparison.OrdinalIgnoreCase))
						{
							GameData item;
							GameData.ParseXml(xmlNode3, out item);
							this.mGames.Add(item);
							if (item.Code.Equals("magicka", StringComparison.OrdinalIgnoreCase))
							{
								for (int l = 0; l < xmlNode3.ChildNodes.Count; l++)
								{
									XmlNode xmlNode4 = xmlNode3.ChildNodes[l];
									if (!(xmlNode4 is XmlComment) && xmlNode4.Name.Equals("achievement", StringComparison.OrdinalIgnoreCase))
									{
										AchievementData item2;
										AchievementData.ParseXml(xmlNode4, out item2);
										this.mAchievements.Add(item2);
									}
								}
							}
						}
						else if (xmlNode3.Name.Equals("user", StringComparison.OrdinalIgnoreCase))
						{
							for (int m = 0; m < xmlNode3.Attributes.Count; m++)
							{
								XmlAttribute xmlAttribute2 = xmlNode3.Attributes[m];
								if (xmlAttribute2.Name.Equals("name", StringComparison.OrdinalIgnoreCase) && !xmlAttribute2.Value.Equals(this.mUsername, StringComparison.OrdinalIgnoreCase))
								{
									this.mServerStatus = ServerRequestResult.ERR_UNKNOWN;
									this.mSessionKey = null;
									return;
								}
							}
						}
						else if (xmlNode3.Name.Equals("rank", StringComparison.OrdinalIgnoreCase))
						{
							for (int n = 0; n < xmlNode3.Attributes.Count; n++)
							{
								XmlAttribute xmlAttribute3 = xmlNode3.Attributes[n];
								if (xmlAttribute3.Name.Equals("position", StringComparison.OrdinalIgnoreCase))
								{
									if (string.IsNullOrEmpty(xmlAttribute3.Value))
									{
										this.mRank = 0;
									}
									else
									{
										this.mRank = int.Parse(xmlAttribute3.Value);
									}
								}
							}
						}
					}
				}
				this.mAchievements.Sort();
				int num = 0;
				int num2 = 0;
				for (int num3 = 0; num3 < this.mAchievements.Count; num3++)
				{
					if (this.mAchievements[num3].Achieved)
					{
						num++;
						num2 += this.mAchievements[num3].Points;
					}
				}
				this.mAwardedAchievements = num;
				this.mTotalPoints = num2;
				this.OnProfileUpdate();
				this.mServerStatus = ServerRequestResult.SUCCESS;
				return;
			}
			this.mServerStatus = ServerRequestResult.ERR_SERVER_ERROR_CODE;
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06002ED6 RID: 11990 RVA: 0x0017C7AA File Offset: 0x0017A9AA
		public ServerRequestResult Status
		{
			get
			{
				return this.mServerStatus;
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x06002ED7 RID: 11991 RVA: 0x0017C7B2 File Offset: 0x0017A9B2
		public string LogInError
		{
			get
			{
				return this.mLogInError;
			}
		}

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x06002ED8 RID: 11992 RVA: 0x0017C7BA File Offset: 0x0017A9BA
		public List<AchievementData> Achievements
		{
			get
			{
				return this.mAchievements;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x06002ED9 RID: 11993 RVA: 0x0017C7C2 File Offset: 0x0017A9C2
		public int AwardedAchievements
		{
			get
			{
				return this.mAwardedAchievements;
			}
		}

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x06002EDA RID: 11994 RVA: 0x0017C7CA File Offset: 0x0017A9CA
		public int TotalPoints
		{
			get
			{
				return this.mTotalPoints;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (get) Token: 0x06002EDB RID: 11995 RVA: 0x0017C7D2 File Offset: 0x0017A9D2
		public List<GameData> Games
		{
			get
			{
				return this.mGames;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06002EDC RID: 11996 RVA: 0x0017C7DA File Offset: 0x0017A9DA
		public int Rank
		{
			get
			{
				return this.mRank;
			}
		}

		// Token: 0x06002EDD RID: 11997 RVA: 0x0017C7E2 File Offset: 0x0017A9E2
		private void OnProfileUpdate()
		{
			PdxWidget.Instance.OnProfileUpdate();
		}

		// Token: 0x06002EDE RID: 11998 RVA: 0x0017C7F0 File Offset: 0x0017A9F0
		public Texture2D GetAchievementImage(string iAchievementCode)
		{
			Texture2D result;
			lock (this.mAchievementTextures)
			{
				AchievementsManager.TextureReference textureReference;
				if (this.mAchievementTextures.TryGetValue(iAchievementCode, out textureReference))
				{
					result = textureReference.Texture;
				}
				else
				{
					AchievementsManager.TextureReference textureReference2 = new AchievementsManager.TextureReference();
					textureReference2.Name = iAchievementCode;
					string requestUriString = string.Format("{0}{1}_igw.png", "http://connect.paradoxplaza.com/media/achievements/", iAchievementCode);
					AchievementsManager.WorkerTask item;
					item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
					item.CallbackArgument = textureReference2;
					item.ResponseCallback = this.mGetImageCallback;
					this.mWorkerQueue.Enqueue(item);
					this.mAchievementTextures.Add(textureReference2.Name, textureReference2);
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x0017C8A8 File Offset: 0x0017AAA8
		public Texture2D GetGameImage(string iGameCode)
		{
			Texture2D result;
			lock (this.mGameTextures)
			{
				AchievementsManager.TextureReference textureReference;
				if (this.mGameTextures.TryGetValue(iGameCode, out textureReference))
				{
					result = textureReference.Texture;
				}
				else
				{
					AchievementsManager.TextureReference textureReference2 = new AchievementsManager.TextureReference();
					textureReference2.Name = iGameCode;
					string requestUriString = string.Format("{0}{1}_igw.png", "http://connect.paradoxplaza.com/media/games/", iGameCode);
					AchievementsManager.WorkerTask item;
					item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
					item.CallbackArgument = textureReference2;
					item.ResponseCallback = this.mGetImageCallback;
					this.mWorkerQueue.Enqueue(item);
					this.mGameTextures.Add(textureReference2.Name, textureReference2);
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x0017C960 File Offset: 0x0017AB60
		private void GetImageCallback(HttpWebResponse iResponse, object iArg)
		{
			if (iResponse == null || iResponse.StatusCode != HttpStatusCode.OK)
			{
				return;
			}
			try
			{
				AchievementsManager.TextureReference textureReference = iArg as AchievementsManager.TextureReference;
				Stream responseStream = iResponse.GetResponseStream();
				MemoryStream memoryStream = new MemoryStream();
				byte[] array = new byte[16384];
				for (;;)
				{
					int num = responseStream.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					memoryStream.Write(array, 0, num);
				}
				responseStream.Close();
				responseStream.Dispose();
				memoryStream.Position = 0L;
				GraphicsDevice graphicsDevice = Game.Instance.GraphicsDevice;
				lock (graphicsDevice)
				{
					textureReference.Texture = Texture2D.FromFile(graphicsDevice, memoryStream);
				}
				memoryStream.Close();
				memoryStream.Dispose();
			}
			catch (WebException)
			{
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06002EE1 RID: 12001 RVA: 0x0017CA2C File Offset: 0x0017AC2C
		public bool Busy
		{
			get
			{
				return this.mBusy;
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06002EE2 RID: 12002 RVA: 0x0017CA34 File Offset: 0x0017AC34
		public bool LoggedIn
		{
			get
			{
				return this.mSessionKey != null;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x0017CA42 File Offset: 0x0017AC42
		public AchievementsManager.PdxLanguage CurrentLanguage
		{
			get
			{
				return this.mLanguage;
			}
		}

		// Token: 0x06002EE4 RID: 12004 RVA: 0x0017CA4C File Offset: 0x0017AC4C
		internal void LogOut()
		{
			if (!string.IsNullOrEmpty(this.mSessionKey))
			{
				string requestUriString = string.Format("{0}logout/?sessionKey=", "https://connect.paradoxplaza.com/api/", this.mSessionKey);
				this.mSessionKey = null;
				AchievementsManager.WorkerTask item;
				item.CallbackArgument = null;
				item.Request = (HttpWebRequest)WebRequest.Create(requestUriString);
				item.ResponseCallback = null;
				this.mAchievements.Clear();
				this.mGames.Clear();
				this.mWorkerQueue.Enqueue(item);
			}
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x0017CAC8 File Offset: 0x0017ACC8
		internal void Request(string iURI, object iCallbackArgument, Action<HttpWebResponse, object> iCallback)
		{
			AchievementsManager.WorkerTask item;
			item.CallbackArgument = iCallbackArgument;
			item.Request = (HttpWebRequest)WebRequest.Create(iURI);
			item.ResponseCallback = iCallback;
			this.mWorkerQueue.Enqueue(item);
		}

		// Token: 0x040032E5 RID: 13029
		public const string GameCode = "magicka";

		// Token: 0x040032E6 RID: 13030
		public const string APIServerPath = "https://connect.paradoxplaza.com/api/";

		// Token: 0x040032E7 RID: 13031
		public const string AchievementsImagePath = "http://connect.paradoxplaza.com/media/achievements/";

		// Token: 0x040032E8 RID: 13032
		public const string GamesImagePath = "http://connect.paradoxplaza.com/media/games/";

		// Token: 0x040032E9 RID: 13033
		private static AchievementsManager sSingelton;

		// Token: 0x040032EA RID: 13034
		private static volatile object sSingeltonLock = new object();

		// Token: 0x040032EB RID: 13035
		public static readonly int ACHIEVEMENT_EARNED = "achievement-earned".GetHashCodeCustom();

		// Token: 0x040032EC RID: 13036
		public static readonly int ACHIEVEMENT_UNLOCKED = "achievement-unlocked".GetHashCodeCustom();

		// Token: 0x040032ED RID: 13037
		public static readonly int BTN_CLOSE = "btn-close".GetHashCodeCustom();

		// Token: 0x040032EE RID: 13038
		public static readonly int BTN_ENTER = "btn-enter".GetHashCodeCustom();

		// Token: 0x040032EF RID: 13039
		public static readonly int BTN_LOGIN = "btn-login".GetHashCodeCustom();

		// Token: 0x040032F0 RID: 13040
		public static readonly int BTN_LOGOUT = "btn-logout".GetHashCodeCustom();

		// Token: 0x040032F1 RID: 13041
		public static readonly int EARNED_THIS_SESSION = "earned-this-session".GetHashCodeCustom();

		// Token: 0x040032F2 RID: 13042
		public static readonly int EMBED = "embed".GetHashCodeCustom();

		// Token: 0x040032F3 RID: 13043
		public static readonly int ERROR_BAD_LOGIN = "error-bad-login".GetHashCodeCustom();

		// Token: 0x040032F4 RID: 13044
		public static readonly int ERROR_SERVICE_UNAVAILABLE = "error-service-unavailable".GetHashCodeCustom();

		// Token: 0x040032F5 RID: 13045
		public static readonly int ERROR_WRONG_PROFILE = "error-wrong-profile".GetHashCodeCustom();

		// Token: 0x040032F6 RID: 13046
		public static readonly int LOGIN = "login".GetHashCodeCustom();

		// Token: 0x040032F7 RID: 13047
		public static readonly int LOGOUT = "logout".GetHashCodeCustom();

		// Token: 0x040032F8 RID: 13048
		public static readonly int MENU_ACHIEVEMENTS = "menu-achievements".GetHashCodeCustom();

		// Token: 0x040032F9 RID: 13049
		public static readonly int MENU_GAMES = "menu-games".GetHashCodeCustom();

		// Token: 0x040032FA RID: 13050
		public static readonly int MYACHIEVEMENTS = "myachievements".GetHashCodeCustom();

		// Token: 0x040032FB RID: 13051
		public static readonly int MYGAMES = "mygames".GetHashCodeCustom();

		// Token: 0x040032FC RID: 13052
		public static readonly int NUM_PP = "num-pp".GetHashCodeCustom();

		// Token: 0x040032FD RID: 13053
		public static readonly int PASSWORD = "password".GetHashCodeCustom();

		// Token: 0x040032FE RID: 13054
		public static readonly int SHARE = "share".GetHashCodeCustom();

		// Token: 0x040032FF RID: 13055
		public static readonly int USERNAME = "username".GetHashCodeCustom();

		// Token: 0x04003300 RID: 13056
		public static readonly int YOUR_RANK = "your-rank".GetHashCodeCustom();

		// Token: 0x04003301 RID: 13057
		public static readonly int LOGIN_HEADER = "login-header".GetHashCodeCustom();

		// Token: 0x04003302 RID: 13058
		private AchievementPopupWindow mPopupWindow;

		// Token: 0x04003303 RID: 13059
		private Queue<int> mQueuedAchievements;

		// Token: 0x04003304 RID: 13060
		private string mUsername;

		// Token: 0x04003305 RID: 13061
		private string mSessionKey;

		// Token: 0x04003306 RID: 13062
		private List<GameData> mGames = new List<GameData>(50);

		// Token: 0x04003307 RID: 13063
		private List<AchievementData> mAchievements = new List<AchievementData>(50);

		// Token: 0x04003308 RID: 13064
		private int mAwardedAchievements;

		// Token: 0x04003309 RID: 13065
		private int mTotalPoints;

		// Token: 0x0400330A RID: 13066
		private int mRank;

		// Token: 0x0400330B RID: 13067
		private SHA256 mSHA256 = SHA256.Create();

		// Token: 0x0400330C RID: 13068
		private Dictionary<string, AchievementsManager.TextureReference> mAchievementTextures = new Dictionary<string, AchievementsManager.TextureReference>();

		// Token: 0x0400330D RID: 13069
		private Dictionary<string, AchievementsManager.TextureReference> mGameTextures = new Dictionary<string, AchievementsManager.TextureReference>();

		// Token: 0x0400330E RID: 13070
		private AchievementsManager.PdxLanguage mLanguage = (AchievementsManager.PdxLanguage)(-1);

		// Token: 0x0400330F RID: 13071
		private Dictionary<int, string> mStrings = new Dictionary<int, string>();

		// Token: 0x04003310 RID: 13072
		private ServerRequestResult mServerStatus;

		// Token: 0x04003311 RID: 13073
		private string mLogInError;

		// Token: 0x04003312 RID: 13074
		private bool mBusy;

		// Token: 0x04003313 RID: 13075
		private Queue<AchievementsManager.WorkerTask> mWorkerQueue = new Queue<AchievementsManager.WorkerTask>();

		// Token: 0x04003314 RID: 13076
		private Action<HttpWebResponse, object> mGetLanguageCallback;

		// Token: 0x04003315 RID: 13077
		private Action<HttpWebResponse, object> mGetImageCallback;

		// Token: 0x04003316 RID: 13078
		private Action<HttpWebResponse, object> mAwardAchievementCallback;

		// Token: 0x04003317 RID: 13079
		private Action<HttpWebResponse, object> mLogInCallback;

		// Token: 0x04003318 RID: 13080
		private Action<HttpWebResponse, object> mGetProfileCallback;

		// Token: 0x04003319 RID: 13081
		private readonly string mGameHash;

		// Token: 0x0200061D RID: 1565
		internal enum PdxLanguage
		{
			// Token: 0x0400331B RID: 13083
			en_US = 9,
			// Token: 0x0400331C RID: 13084
			sv_SE = 29,
			// Token: 0x0400331D RID: 13085
			es = 10
		}

		// Token: 0x0200061E RID: 1566
		private class TextureReference
		{
			// Token: 0x0400331E RID: 13086
			public string Name;

			// Token: 0x0400331F RID: 13087
			public Texture2D Texture;
		}

		// Token: 0x0200061F RID: 1567
		private struct WorkerTask
		{
			// Token: 0x04003320 RID: 13088
			public HttpWebRequest Request;

			// Token: 0x04003321 RID: 13089
			public Action<HttpWebResponse, object> ResponseCallback;

			// Token: 0x04003322 RID: 13090
			public object CallbackArgument;
		}
	}
}
