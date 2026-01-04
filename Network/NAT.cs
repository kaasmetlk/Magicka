// Decompiled with JetBrains decompiler
// Type: Magicka.Network.NAT
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

#nullable disable
namespace Magicka.Network;

public class NAT
{
  private static string sDescUrl;
  private static string sServiceUrl;
  private static string sEventUrl;
  private static long sTimeout = TimeSpan.FromSeconds(3.0).Ticks;

  public static TimeSpan TimeOut
  {
    get => TimeSpan.FromTicks(NAT.sTimeout);
    set => NAT.sTimeout = value.Ticks;
  }

  public static bool Discover()
  {
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
    byte[] bytes = Encoding.ASCII.GetBytes("M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nST:upnp:rootdevice\r\nMAN:\"ssdp:discover\"\r\nMX:3\r\n\r\n");
    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, 1900);
    byte[] numArray = new byte[4096 /*0x1000*/];
    long ticks = DateTime.Now.Ticks;
    do
    {
      socket.SendTo(bytes, (EndPoint) remoteEP);
      socket.SendTo(bytes, (EndPoint) remoteEP);
      socket.SendTo(bytes, (EndPoint) remoteEP);
      do
      {
        if (socket.Available > 0)
        {
          int count = socket.Receive(numArray);
          string lowerInvariant = Encoding.ASCII.GetString(numArray, 0, count).ToLowerInvariant();
          if (lowerInvariant.Contains("upnp:rootdevice"))
          {
            string str = lowerInvariant.Substring(lowerInvariant.ToLowerInvariant().IndexOf("location:") + 9);
            string iResp = str.Substring(0, str.IndexOf("\r")).Trim();
            if (!string.IsNullOrEmpty(NAT.sServiceUrl = NAT.GetServiceUrl(iResp)))
            {
              NAT.sDescUrl = iResp;
              return true;
            }
          }
        }
      }
      while (DateTime.Now.Ticks - ticks < NAT.sTimeout);
    }
    while (DateTime.Now.Ticks - ticks < NAT.sTimeout);
    return false;
  }

  private static string GetServiceUrl(string iResp)
  {
    try
    {
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        WebRequest webRequest = WebRequest.Create(iResp);
        webRequest.Timeout = 2000;
        Stream responseStream = webRequest.GetResponse().GetResponseStream();
        responseStream.ReadTimeout = 2000;
        xmlDocument.Load(responseStream);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
        nsmgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
        if (!xmlDocument.SelectSingleNode("//tns:device/tns:deviceType/text()", nsmgr).Value.Contains("InternetGatewayDevice"))
          return (string) null;
        XmlNode xmlNode1 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", nsmgr);
        if (xmlNode1 == null)
          return (string) null;
        XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", nsmgr);
        NAT.sEventUrl = NAT.CombineUrls(iResp, xmlNode2.Value);
        return NAT.CombineUrls(iResp, xmlNode1.Value);
      }
      catch
      {
        return (string) null;
      }
    }
    catch (Exception ex)
    {
      return (string) null;
    }
  }

  private static string CombineUrls(string iResp, string iP)
  {
    int num = iResp.IndexOf("://");
    int length = iResp.IndexOf('/', num + 3);
    return iResp.Substring(0, length) + iP;
  }

  public static bool ForwardPort(int iPort, ProtocolType iProtocol, string iDescription)
  {
    return NAT.ForwardPort(iPort, iProtocol, iDescription, out XmlDocument _);
  }

  public static bool ForwardPort(
    int iPort,
    ProtocolType iProtocol,
    string iDescription,
    out XmlDocument oResponse)
  {
    if (string.IsNullOrEmpty(NAT.sServiceUrl))
      throw new Exception("No UPnP service available or Discover() has not been called");
    if (iProtocol != ProtocolType.Tcp && iProtocol != ProtocolType.Udp)
      throw new ArgumentException("Invalid ProtocolType! Must be Tcp or Udp.", nameof (iProtocol));
    IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
    IPAddress ipAddress = (IPAddress) null;
    for (int index = 0; index < hostAddresses.Length; ++index)
    {
      if (hostAddresses[index].AddressFamily == AddressFamily.InterNetwork)
      {
        ipAddress = hostAddresses[index];
        break;
      }
    }
    if (ipAddress == null)
    {
      oResponse = (XmlDocument) null;
      return false;
    }
    return NAT.SOAPRequest(NAT.sServiceUrl, $"<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>{iPort.ToString()}</NewExternalPort><NewProtocol>{iProtocol.ToString().ToUpper()}</NewProtocol><NewInternalPort>{iPort.ToString()}</NewInternalPort><NewInternalClient>{(object) ipAddress}</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>{iDescription}</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>", "AddPortMapping", out oResponse);
  }

  public static bool DeleteForwardingRule(int iPort, ProtocolType iProtocol)
  {
    return NAT.DeleteForwardingRule(iPort, iProtocol, out XmlDocument _);
  }

  public static bool DeleteForwardingRule(
    int iPort,
    ProtocolType iProtocol,
    out XmlDocument oResponse)
  {
    if (string.IsNullOrEmpty(NAT.sServiceUrl))
      throw new Exception("No UPnP service available or Discover() has not been called");
    return NAT.SOAPRequest(NAT.sServiceUrl, $"<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>{(object) iPort}</NewExternalPort><NewProtocol>{iProtocol.ToString().ToUpper()}</NewProtocol></u:DeletePortMapping>", "DeletePortMapping", out oResponse);
  }

  public static IPAddress GetExternalIP()
  {
    if (string.IsNullOrEmpty(NAT.sServiceUrl))
      throw new Exception("No UPnP service available or Discover() has not been called");
    XmlDocument oResponse;
    NAT.SOAPRequest(NAT.sServiceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"></u:GetExternalIPAddress>", "GetExternalIPAddress", out oResponse);
    XmlNamespaceManager nsmgr = new XmlNamespaceManager(oResponse.NameTable);
    nsmgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
    return IPAddress.Parse(oResponse.SelectSingleNode("//NewExternalIPAddress/text()", nsmgr).Value);
  }

  private static bool SOAPRequest(
    string iUrl,
    string iSoap,
    string iFunction,
    out XmlDocument oResponse)
  {
    string s = $"<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>{iSoap}</s:Body></s:Envelope>";
    WebRequest webRequest = WebRequest.Create(iUrl);
    webRequest.Method = "POST";
    byte[] bytes = Encoding.UTF8.GetBytes(s);
    webRequest.Headers.Add("SOAPACTION", $"\"urn:schemas-upnp-org:service:WANIPConnection:1#{iFunction}\"");
    webRequest.ContentType = "text/xml; charset=\"utf-8\"";
    webRequest.ContentLength = (long) bytes.Length;
    webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
    try
    {
      Stream responseStream = webRequest.GetResponse().GetResponseStream();
      oResponse = new XmlDocument();
      oResponse.Load(responseStream);
      responseStream.Close();
      return true;
    }
    catch (WebException ex)
    {
      Stream responseStream = ex.Response.GetResponseStream();
      oResponse = new XmlDocument();
      oResponse.Load(responseStream);
      responseStream.Close();
      return false;
    }
  }
}
