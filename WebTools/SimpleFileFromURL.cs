// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.SimpleFileFromURL
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

#nullable disable
namespace Magicka.WebTools;

public class SimpleFileFromURL
{
  private string userName;
  private string psw;
  private Uri serverURI;
  private WebClient request;
  private bool connected;
  private bool isFTP;

  public bool Connected => this.connected;

  public WebClient ActiveConnection => this.request;

  public SimpleFileFromURL(Uri inServerURI, string FTP_UserName, string FTP_Password)
  {
    this.serverURI = inServerURI;
    this.userName = FTP_UserName;
    this.psw = FTP_Password;
    this.connected = false;
    this.isFTP = true;
  }

  public SimpleFileFromURL(Uri inServerURI)
  {
    this.isFTP = false;
    this.serverURI = inServerURI;
    this.connected = false;
  }

  public SimpleFileFromURL(WebClient existingConnection)
  {
    this.request = existingConnection;
    this.connected = this.request != null;
    new WebClient().BaseAddress = existingConnection.BaseAddress;
    this.isFTP = this.serverURI.Scheme == Uri.UriSchemeFtp;
  }

  public void Connect()
  {
    if (this.connected)
      return;
    if (this.isFTP && this.serverURI.Scheme != Uri.UriSchemeFtp)
    {
      this.connected = false;
    }
    else
    {
      this.connected = true;
      try
      {
        this.request = new WebClient();
        if (!this.isFTP)
          return;
        this.request.Credentials = (ICredentials) new NetworkCredential(this.userName, this.psw);
      }
      catch
      {
        this.connected = false;
      }
    }
  }

  public void Disconnect()
  {
    this.connected = false;
    if (this.request == null)
      return;
    this.request.Dispose();
    this.request = (WebClient) null;
  }

  public bool GetXML(string fileName, out XmlDocument doc)
  {
    doc = (XmlDocument) null;
    if (!this.connected)
      return false;
    bool xml = true;
    string str1 = "";
    try
    {
      string str2 = this.serverURI.ToString().Trim();
      if (str2[str2.Length - 1] != '/')
        str2 += "/";
      str1 = Encoding.UTF8.GetString(this.request.DownloadData(str2 + fileName));
    }
    catch (WebException ex)
    {
      xml = false;
    }
    if (!xml || string.IsNullOrEmpty(str1))
      return false;
    doc = new XmlDocument();
    try
    {
      doc.InnerXml = str1;
    }
    catch (Exception ex)
    {
      doc = (XmlDocument) null;
      xml = false;
    }
    return xml;
  }

  public bool GetTexture(string fileName, GraphicsDevice device, out Texture2D tex)
  {
    tex = (Texture2D) null;
    if (!this.connected)
      return false;
    bool flag = true;
    byte[] buffer = (byte[]) null;
    try
    {
      string str = this.serverURI.ToString().Trim();
      if (str[str.Length - 1] != '/')
        str += "/";
      buffer = this.request.DownloadData(str + fileName);
    }
    catch (WebException ex)
    {
      flag = false;
    }
    if (!flag || buffer == null)
      return false;
    MemoryStream textureStream = new MemoryStream();
    try
    {
      using (textureStream = new MemoryStream(buffer))
        tex = Texture2D.FromFile(device, (Stream) textureStream, buffer.Length);
    }
    catch (Exception ex)
    {
      return false;
    }
    finally
    {
      textureStream.Close();
      textureStream.Dispose();
    }
    return tex != null && flag;
  }
}
