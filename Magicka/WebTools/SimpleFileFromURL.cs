using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Magicka.WebTools
{
	// Token: 0x02000620 RID: 1568
	public class SimpleFileFromURL
	{
		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06002EE8 RID: 12008 RVA: 0x0017CC7E File Offset: 0x0017AE7E
		public bool Connected
		{
			get
			{
				return this.connected;
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06002EE9 RID: 12009 RVA: 0x0017CC86 File Offset: 0x0017AE86
		public WebClient ActiveConnection
		{
			get
			{
				return this.request;
			}
		}

		// Token: 0x06002EEA RID: 12010 RVA: 0x0017CC8E File Offset: 0x0017AE8E
		public SimpleFileFromURL(Uri inServerURI, string FTP_UserName, string FTP_Password)
		{
			this.serverURI = inServerURI;
			this.userName = FTP_UserName;
			this.psw = FTP_Password;
			this.connected = false;
			this.isFTP = true;
		}

		// Token: 0x06002EEB RID: 12011 RVA: 0x0017CCB9 File Offset: 0x0017AEB9
		public SimpleFileFromURL(Uri inServerURI)
		{
			this.isFTP = false;
			this.serverURI = inServerURI;
			this.connected = false;
		}

		// Token: 0x06002EEC RID: 12012 RVA: 0x0017CCD8 File Offset: 0x0017AED8
		public SimpleFileFromURL(WebClient existingConnection)
		{
			this.request = existingConnection;
			this.connected = (this.request != null);
			WebClient webClient = new WebClient();
			webClient.BaseAddress = existingConnection.BaseAddress;
			this.isFTP = (this.serverURI.Scheme == Uri.UriSchemeFtp);
		}

		// Token: 0x06002EED RID: 12013 RVA: 0x0017CD34 File Offset: 0x0017AF34
		public void Connect()
		{
			if (this.connected)
			{
				return;
			}
			if (this.isFTP && this.serverURI.Scheme != Uri.UriSchemeFtp)
			{
				this.connected = false;
				return;
			}
			this.connected = true;
			try
			{
				this.request = new WebClient();
				if (this.isFTP)
				{
					this.request.Credentials = new NetworkCredential(this.userName, this.psw);
				}
			}
			catch
			{
				this.connected = false;
			}
		}

		// Token: 0x06002EEE RID: 12014 RVA: 0x0017CDC4 File Offset: 0x0017AFC4
		public void Disconnect()
		{
			this.connected = false;
			if (this.request == null)
			{
				return;
			}
			this.request.Dispose();
			this.request = null;
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x0017CDE8 File Offset: 0x0017AFE8
		public bool GetXML(string fileName, out XmlDocument doc)
		{
			doc = null;
			if (!this.connected)
			{
				return false;
			}
			bool flag = true;
			string text = "";
			try
			{
				string text2 = this.serverURI.ToString().Trim();
				if (text2[text2.Length - 1] != '/')
				{
					text2 += "/";
				}
				text2 += fileName;
				byte[] bytes = this.request.DownloadData(text2);
				text = Encoding.UTF8.GetString(bytes);
			}
			catch (WebException)
			{
				flag = false;
			}
			if (!flag || string.IsNullOrEmpty(text))
			{
				return false;
			}
			doc = new XmlDocument();
			try
			{
				doc.InnerXml = text;
			}
			catch (Exception)
			{
				doc = null;
				flag = false;
			}
			return flag;
		}

		// Token: 0x06002EF0 RID: 12016 RVA: 0x0017CEA4 File Offset: 0x0017B0A4
		public bool GetTexture(string fileName, GraphicsDevice device, out Texture2D tex)
		{
			tex = null;
			if (!this.connected)
			{
				return false;
			}
			bool flag = true;
			byte[] array = null;
			try
			{
				string text = this.serverURI.ToString().Trim();
				if (text[text.Length - 1] != '/')
				{
					text += "/";
				}
				text += fileName;
				array = this.request.DownloadData(text);
			}
			catch (WebException)
			{
				flag = false;
			}
			if (!flag || array == null)
			{
				return false;
			}
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				MemoryStream memoryStream2;
				memoryStream = (memoryStream2 = new MemoryStream(array));
				try
				{
					tex = Texture2D.FromFile(device, memoryStream, array.Length);
				}
				finally
				{
					if (memoryStream2 != null)
					{
						((IDisposable)memoryStream2).Dispose();
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				memoryStream.Close();
				memoryStream.Dispose();
			}
			return tex != null && flag;
		}

		// Token: 0x04003323 RID: 13091
		private string userName;

		// Token: 0x04003324 RID: 13092
		private string psw;

		// Token: 0x04003325 RID: 13093
		private Uri serverURI;

		// Token: 0x04003326 RID: 13094
		private WebClient request;

		// Token: 0x04003327 RID: 13095
		private bool connected;

		// Token: 0x04003328 RID: 13096
		private bool isFTP;
	}
}
