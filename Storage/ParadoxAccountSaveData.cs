// Decompiled with JetBrains decompiler
// Type: Magicka.Storage.ParadoxAccountSaveData
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Magicka.CoreFramework;
using Magicka.Misc;
using SteamWrapper;
using System;
using System.IO;
using System.Text;
using System.Web.Security;

#nullable disable
namespace Magicka.Storage;

public class ParadoxAccountSaveData : Singleton<ParadoxAccountSaveData>
{
  private const Logger.Source LOGGER_SOURCE = Logger.Source.ParadoxAccountSaveData;
  private const char DATA_SEPARATOR = '|';
  private string mShadowUniqueId = string.Empty;
  private string mAuthToken = string.Empty;
  private bool mLinkedToSteam;
  private object mSaveLoadLock = new object();

  public string ShadowUniqueId => this.mShadowUniqueId;

  public bool HasShadowUniqueId => !string.IsNullOrEmpty(this.mShadowUniqueId);

  public string AuthToken => this.mAuthToken;

  public bool HasAuthToken => !string.IsNullOrEmpty(this.mAuthToken);

  public bool IsShadow => this.HasShadowUniqueId && !this.HasAuthToken;

  public ParadoxAccountSaveData() => this.Load();

  public void SetShadowUniqueId(string iUniqueId)
  {
    this.mShadowUniqueId = iUniqueId;
    this.Save();
  }

  public void ClearShadowUniqueId()
  {
    this.mShadowUniqueId = string.Empty;
    this.Save();
  }

  public void SetAuthToken(string iAuthToken)
  {
    this.mAuthToken = iAuthToken;
    this.Save();
  }

  public void ClearAuthToken()
  {
    this.mAuthToken = string.Empty;
    this.Save();
  }

  public void Promote(string iAuthToken)
  {
    this.mShadowUniqueId = string.Empty;
    this.SetAuthToken(iAuthToken);
  }

  public void Write(BinaryWriter iWriter)
  {
    string[] strArray = new string[2]
    {
      this.mShadowUniqueId,
      this.mAuthToken
    };
    string iInput = string.Join('|'.ToString(), strArray);
    string salt = this.GetSalt();
    string base64String1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(iInput + salt, "MD5")));
    string base64String2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Encryption.Vigenere(iInput, this.GetSalt(), false)));
    iWriter.Write(base64String1);
    iWriter.Write(base64String2);
  }

  public void Read(BinaryReader iReader)
  {
    string salt = this.GetSalt();
    string str1 = Encoding.UTF8.GetString(Convert.FromBase64String(iReader.ReadString()));
    string str2 = Encryption.Vigenere(Encoding.UTF8.GetString(Convert.FromBase64String(iReader.ReadString())), salt, true);
    string strB = FormsAuthentication.HashPasswordForStoringInConfigFile(str2 + salt, "MD5");
    if (str1.CompareTo(strB) == 0)
    {
      string[] strArray1 = str2.Split('|');
      try
      {
        int num1 = 0;
        string[] strArray2 = strArray1;
        int index1 = num1;
        int num2 = index1 + 1;
        this.mShadowUniqueId = strArray2[index1];
        string[] strArray3 = strArray1;
        int index2 = num2;
        int num3 = index2 + 1;
        this.mAuthToken = strArray3[index2];
      }
      catch (Exception ex)
      {
        this.Reset();
      }
    }
    else
    {
      Logger.LogError(Logger.Source.ParadoxAccountSaveData, "Checksum fail.");
      this.Reset();
    }
  }

  public string GetSalt()
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(SteamUser.GetSteamID().AsUInt64.ToString() + SteamUtils.GetAppID().ToString()));
  }

  public void Load()
  {
    lock (this.mSaveLoadLock)
      SaveManager.Instance.LoadPOPSData(this);
  }

  public void Save()
  {
    lock (this.mSaveLoadLock)
      SaveManager.Instance.SavePOPSData(this);
  }

  public void Reset()
  {
    this.mShadowUniqueId = string.Empty;
    this.mAuthToken = string.Empty;
    this.mLinkedToSteam = false;
    this.Save();
  }
}
