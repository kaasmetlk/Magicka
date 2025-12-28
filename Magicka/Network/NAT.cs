using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Magicka.Network
{
	// Token: 0x02000624 RID: 1572
	public class NAT
	{
		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x06002F23 RID: 12067 RVA: 0x0017EAF0 File Offset: 0x0017CCF0
		// (set) Token: 0x06002F24 RID: 12068 RVA: 0x0017EAFC File Offset: 0x0017CCFC
		public static TimeSpan TimeOut
		{
			get
			{
				return TimeSpan.FromTicks(NAT.sTimeout);
			}
			set
			{
				NAT.sTimeout = value.Ticks;
			}
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x0017EB0C File Offset: 0x0017CD0C
		public static bool Discover()
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			string s = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nST:upnp:rootdevice\r\nMAN:\"ssdp:discover\"\r\nMX:3\r\n\r\n";
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, 1900);
			byte[] array = new byte[4096];
			long ticks = DateTime.Now.Ticks;
			string text;
			for (;;)
			{
				socket.SendTo(bytes, remoteEP);
				socket.SendTo(bytes, remoteEP);
				socket.SendTo(bytes, remoteEP);
				do
				{
					if (socket.Available > 0)
					{
						int count = socket.Receive(array);
						text = Encoding.ASCII.GetString(array, 0, count).ToLowerInvariant();
						if (text.Contains("upnp:rootdevice"))
						{
							text = text.Substring(text.ToLowerInvariant().IndexOf("location:") + 9);
							text = text.Substring(0, text.IndexOf("\r")).Trim();
							if (!string.IsNullOrEmpty(NAT.sServiceUrl = NAT.GetServiceUrl(text)))
							{
								goto Block_3;
							}
						}
					}
				}
				while (DateTime.Now.Ticks - ticks < NAT.sTimeout);
				if (DateTime.Now.Ticks - ticks >= NAT.sTimeout)
				{
					return false;
				}
			}
			Block_3:
			NAT.sDescUrl = text;
			return true;
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x0017EC54 File Offset: 0x0017CE54
		private static string GetServiceUrl(string iResp)
		{
			string result;
			try
			{
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					WebRequest webRequest = WebRequest.Create(iResp);
					webRequest.Timeout = 2000;
					WebResponse response = webRequest.GetResponse();
					Stream responseStream = response.GetResponseStream();
					responseStream.ReadTimeout = 2000;
					xmlDocument.Load(responseStream);
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
					xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
					XmlNode xmlNode = xmlDocument.SelectSingleNode("//tns:device/tns:deviceType/text()", xmlNamespaceManager);
					if (!xmlNode.Value.Contains("InternetGatewayDevice"))
					{
						result = null;
					}
					else
					{
						XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", xmlNamespaceManager);
						if (xmlNode2 == null)
						{
							result = null;
						}
						else
						{
							XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", xmlNamespaceManager);
							NAT.sEventUrl = NAT.CombineUrls(iResp, xmlNode3.Value);
							result = NAT.CombineUrls(iResp, xmlNode2.Value);
						}
					}
				}
				catch
				{
					result = null;
				}
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06002F27 RID: 12071 RVA: 0x0017ED54 File Offset: 0x0017CF54
		private static string CombineUrls(string iResp, string iP)
		{
			int num = iResp.IndexOf("://");
			num = iResp.IndexOf('/', num + 3);
			return iResp.Substring(0, num) + iP;
		}

		// Token: 0x06002F28 RID: 12072 RVA: 0x0017ED88 File Offset: 0x0017CF88
		public static bool ForwardPort(int iPort, ProtocolType iProtocol, string iDescription)
		{
			XmlDocument xmlDocument;
			return NAT.ForwardPort(iPort, iProtocol, iDescription, out xmlDocument);
		}

		// Token: 0x06002F29 RID: 12073 RVA: 0x0017EDA0 File Offset: 0x0017CFA0
		public static bool ForwardPort(int iPort, ProtocolType iProtocol, string iDescription, out XmlDocument oResponse)
		{
			if (string.IsNullOrEmpty(NAT.sServiceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			if (iProtocol != ProtocolType.Tcp && iProtocol != ProtocolType.Udp)
			{
				throw new ArgumentException("Invalid ProtocolType! Must be Tcp or Udp.", "iProtocol");
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
			IPAddress ipaddress = null;
			for (int i = 0; i < hostAddresses.Length; i++)
			{
				if (hostAddresses[i].AddressFamily == AddressFamily.InterNetwork)
				{
					ipaddress = hostAddresses[i];
					break;
				}
			}
			if (ipaddress == null)
			{
				oResponse = null;
				return false;
			}
			return NAT.SOAPRequest(NAT.sServiceUrl, string.Concat(new object[]
			{
				"<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
				iPort.ToString(),
				"</NewExternalPort><NewProtocol>",
				iProtocol.ToString().ToUpper(),
				"</NewProtocol><NewInternalPort>",
				iPort.ToString(),
				"</NewInternalPort><NewInternalClient>",
				ipaddress,
				"</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>",
				iDescription,
				"</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>"
			}), "AddPortMapping", out oResponse);
		}

		// Token: 0x06002F2A RID: 12074 RVA: 0x0017EE90 File Offset: 0x0017D090
		public static bool DeleteForwardingRule(int iPort, ProtocolType iProtocol)
		{
			XmlDocument xmlDocument;
			return NAT.DeleteForwardingRule(iPort, iProtocol, out xmlDocument);
		}

		// Token: 0x06002F2B RID: 12075 RVA: 0x0017EEA8 File Offset: 0x0017D0A8
		public static bool DeleteForwardingRule(int iPort, ProtocolType iProtocol, out XmlDocument oResponse)
		{
			if (string.IsNullOrEmpty(NAT.sServiceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			return NAT.SOAPRequest(NAT.sServiceUrl, string.Concat(new object[]
			{
				"<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
				iPort,
				"</NewExternalPort><NewProtocol>",
				iProtocol.ToString().ToUpper(),
				"</NewProtocol></u:DeletePortMapping>"
			}), "DeletePortMapping", out oResponse);
		}

		// Token: 0x06002F2C RID: 12076 RVA: 0x0017EF20 File Offset: 0x0017D120
		public static IPAddress GetExternalIP()
		{
			if (string.IsNullOrEmpty(NAT.sServiceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			XmlDocument xmlDocument;
			NAT.SOAPRequest(NAT.sServiceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"></u:GetExternalIPAddress>", "GetExternalIPAddress", out xmlDocument);
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
			string value = xmlDocument.SelectSingleNode("//NewExternalIPAddress/text()", xmlNamespaceManager).Value;
			return IPAddress.Parse(value);
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x0017EF90 File Offset: 0x0017D190
		private static bool SOAPRequest(string iUrl, string iSoap, string iFunction, out XmlDocument oResponse)
		{
			string s = "<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + iSoap + "</s:Body></s:Envelope>";
			WebRequest webRequest = WebRequest.Create(iUrl);
			webRequest.Method = "POST";
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			webRequest.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + iFunction + "\"");
			webRequest.ContentType = "text/xml; charset=\"utf-8\"";
			webRequest.ContentLength = (long)bytes.Length;
			webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			bool result;
			try
			{
				WebResponse response = webRequest.GetResponse();
				Stream responseStream = response.GetResponseStream();
				oResponse = new XmlDocument();
				oResponse.Load(responseStream);
				responseStream.Close();
				result = true;
			}
			catch (WebException ex)
			{
				WebResponse response2 = ex.Response;
				Stream responseStream2 = response2.GetResponseStream();
				oResponse = new XmlDocument();
				oResponse.Load(responseStream2);
				responseStream2.Close();
				result = false;
			}
			return result;
		}

		// Token: 0x0400334B RID: 13131
		private static string sDescUrl;

		// Token: 0x0400334C RID: 13132
		private static string sServiceUrl;

		// Token: 0x0400334D RID: 13133
		private static string sEventUrl;

		// Token: 0x0400334E RID: 13134
		private static long sTimeout = TimeSpan.FromSeconds(3.0).Ticks;
	}
}
