// Decompiled with JetBrains decompiler
// Type: Magicka.Levels.Campaign.LevelNode
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

#nullable disable
namespace Magicka.Levels.Campaign;

internal class LevelNode
{
  private string mFileName;
  private string mName;
  private string mFullFileName;
  private int mDescription;
  private int mShortDescription;
  private string mLoadingImage;
  private byte[] mHashSum;
  private LevelNode.SceneNode[] mScenes;
  private string mPreferredAvatar;
  private List<string> mAllowedAvatars;

  public string FullFileName => this.mFullFileName;

  public string FileName => this.mFileName;

  public string LoadingImage => this.mLoadingImage;

  public string Name => this.mName;

  public int Description => this.mDescription;

  public int ShortDescription => this.mShortDescription;

  public byte[] HashSum => this.mHashSum;

  public LevelNode.SceneNode[] Scenes => this.mScenes;

  public string PreferredAvatar => this.mPreferredAvatar;

  public List<string> AllowedAvatars => this.mAllowedAvatars;

  public Rulesets RulesetType
  {
    get
    {
      if (this.mScenes != null && this.mScenes.Length > 0)
      {
        for (int index = 0; index < this.mScenes.Length; ++index)
        {
          if (this.mScenes[index].RulesetType != Rulesets.None)
            return this.mScenes[index].RulesetType;
        }
      }
      return Rulesets.None;
    }
  }

  public LevelNode(string iPath, string iFileName)
  {
    this.mFullFileName = iPath + iFileName;
    string path = "content/Levels/" + this.mFullFileName;
    FileStream input = File.OpenRead(path);
    this.mHashSum = (byte[]) null;
    this.mPreferredAvatar = "wizard";
    this.mAllowedAvatars = new List<string>();
    XmlTextReader xmlTextReader = new XmlTextReader((Stream) input);
    List<LevelNode.SceneNode> sceneNodeList = new List<LevelNode.SceneNode>();
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    while (xmlTextReader.Read())
    {
      if (xmlTextReader.NodeType == XmlNodeType.Element)
      {
        if (xmlTextReader.Name.Equals(nameof (Name), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"Name\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.NodeType == XmlNodeType.Text)
            {
              this.mName = xmlTextReader.Value.ToLowerInvariant();
              flag1 = true;
              break;
            }
            if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name.Equals(nameof (Name)))
              throw new Exception($"\"Name\" tag is empty in \"{iFileName}{(object) '"'}");
          }
        }
        else if (xmlTextReader.Name.Equals(nameof (Description), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"Description\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.NodeType == XmlNodeType.Text)
            {
              this.mDescription = xmlTextReader.Value.ToLowerInvariant().GetHashCodeCustom();
              if (!flag4)
                this.mShortDescription = this.mDescription;
              flag2 = true;
              break;
            }
            if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals(nameof (Description)))
              throw new Exception($"\"Description\" tag is empty in \"{iFileName}{(object) '"'}");
          }
        }
        else if (xmlTextReader.Name.Equals(nameof (ShortDescription), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
          {
            this.mShortDescription = this.mDescription;
          }
          else
          {
            while (xmlTextReader.Read())
            {
              if (xmlTextReader.NodeType == XmlNodeType.Text)
              {
                this.mShortDescription = xmlTextReader.Value.ToLowerInvariant().GetHashCodeCustom();
                flag4 = true;
                break;
              }
              if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals(nameof (ShortDescription)))
              {
                this.mShortDescription = this.mDescription;
                break;
              }
            }
          }
        }
        else if (xmlTextReader.Name.Equals("Loading", StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"Loading\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.NodeType == XmlNodeType.Text)
            {
              this.mLoadingImage = xmlTextReader.Value;
              flag3 = true;
              break;
            }
            if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals("Loading"))
              throw new Exception($"\"Loading\" tag is empty in \"{iFileName}{(object) '"'}");
          }
        }
        else if (xmlTextReader.Name.Equals(nameof (Scenes), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"Scenes\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.Name.Equals("Scene", StringComparison.OrdinalIgnoreCase))
            {
              if (xmlTextReader.IsEmptyElement)
                throw new Exception($"\"Scene\" tag is empty in \"{iFileName}{(object) '"'}");
              while (xmlTextReader.Read())
              {
                if (xmlTextReader.NodeType == XmlNodeType.Text)
                {
                  sceneNodeList.Add(new LevelNode.SceneNode(Path.Combine(Path.GetDirectoryName(path), xmlTextReader.Value)));
                  break;
                }
              }
            }
          }
        }
        else if (xmlTextReader.Name.Equals(nameof (PreferredAvatar), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"PreferredAvatar\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.NodeType == XmlNodeType.Text)
            {
              this.mPreferredAvatar = xmlTextReader.Value.ToLowerInvariant();
              break;
            }
            if (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.Name.Equals(nameof (PreferredAvatar)))
              throw new Exception($"\"PreferredAvatar\" tag is empty in \"{iFileName}{(object) '"'}");
          }
        }
        else if (xmlTextReader.Name.Equals(nameof (AllowedAvatars), StringComparison.OrdinalIgnoreCase))
        {
          if (xmlTextReader.IsEmptyElement)
            throw new Exception($"\"AllowedAvatars\" tag is empty in \"{iFileName}{(object) '"'}");
          while (xmlTextReader.Read())
          {
            if (xmlTextReader.Name.Equals("Avatar", StringComparison.OrdinalIgnoreCase))
            {
              if (xmlTextReader.IsEmptyElement)
                throw new Exception($"\"Avatar\" tag is empty in \"{iFileName}{(object) '"'}");
              while (xmlTextReader.Read())
              {
                if (xmlTextReader.NodeType == XmlNodeType.Text)
                {
                  this.mAllowedAvatars.Add(xmlTextReader.Value.ToLowerInvariant());
                  break;
                }
                if (xmlTextReader.NodeType == XmlNodeType.EndElement)
                  break;
              }
            }
            if (xmlTextReader.NodeType == XmlNodeType.EndElement)
              break;
          }
        }
      }
      if (flag1 && flag2 && flag3 && sceneNodeList.Count > 0)
        break;
    }
    xmlTextReader.Close();
    input.Close();
    input.Dispose();
    if (!flag1)
      throw new Exception($"No \"Name\" tag found in \"{iFileName}{(object) '"'}");
    if (!flag2)
      throw new Exception($"No \"Description\" tag found in \"{iFileName}{(object) '"'}");
    if (!flag3)
      throw new Exception($"No \"Loading\" tag found in \"{iFileName}{(object) '"'}");
    this.mScenes = sceneNodeList.Count != 0 ? sceneNodeList.ToArray() : throw new Exception($"No \"Scene\" tag found in \"{iFileName}{(object) '"'}");
    this.mFileName = Path.GetFileNameWithoutExtension(iFileName);
  }

  internal void ComputeHashSums(SHA256 iSHA)
  {
    for (int index = 0; index < this.mScenes.Length; ++index)
      this.mScenes[index].ComputeHashSums(iSHA);
    FileStream inputStream = File.OpenRead("content/Levels/" + this.mFullFileName);
    this.mHashSum = iSHA.ComputeHash((Stream) inputStream);
    inputStream.Close();
  }

  public byte[] GetCombinedHash()
  {
    if (this.mHashSum == null)
      return (byte[]) null;
    byte[] combinedHash = this.mHashSum.Clone() as byte[];
    for (int index1 = 0; index1 < this.mScenes.Length; ++index1)
    {
      for (int index2 = 0; index2 < combinedHash.Length; ++index2)
      {
        combinedHash[index2] ^= this.mScenes[index1].ScriptHashSum[index2];
        combinedHash[index2] ^= this.mScenes[index1].ModelHashSum[index2];
      }
    }
    return combinedHash;
  }

  public class SceneNode
  {
    private Rulesets mRulesetType;

    public SceneNode(string iFilename)
    {
      this.ScriptFilename = iFilename;
      FileStream input = File.OpenRead(iFilename);
      this.ScriptHashSum = (byte[]) null;
      XmlTextReader xmlTextReader = new XmlTextReader((Stream) input);
      while (xmlTextReader.Read())
      {
        if (xmlTextReader.NodeType == XmlNodeType.Element)
        {
          if (xmlTextReader.Name.Equals("model", StringComparison.OrdinalIgnoreCase))
          {
            if (xmlTextReader.IsEmptyElement)
              throw new Exception($"\"model\" tag is empty in \"{iFilename}{(object) '"'}");
            while (xmlTextReader.Read())
            {
              if (xmlTextReader.NodeType == XmlNodeType.Text)
              {
                this.ModelFilename = xmlTextReader.Value.ToLowerInvariant() + ".xnb";
                this.ModelFilename = Path.Combine(Path.GetDirectoryName(iFilename), this.ModelFilename);
                this.ModelHashSum = (byte[]) null;
                break;
              }
              if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name.Equals("model"))
                throw new Exception($"\"model\" tag is empty in \"{iFilename}{(object) '"'}");
            }
          }
          else if (xmlTextReader.Name.Equals("ruleset", StringComparison.OrdinalIgnoreCase))
          {
            string attribute = xmlTextReader.GetAttribute("type");
            if (attribute.Equals("versus", StringComparison.OrdinalIgnoreCase))
              this.mRulesetType = Rulesets.None;
            else if (!string.IsNullOrEmpty(attribute))
              this.mRulesetType = (Rulesets) Enum.Parse(typeof (Rulesets), attribute, true);
          }
        }
      }
      if (string.IsNullOrEmpty(this.ModelFilename))
        throw new Exception($"\"model\" tag missing in \"{iFilename}{(object) '"'}");
      input.Close();
    }

    internal void ComputeHashSums(SHA256 iSHA)
    {
      FileStream inputStream1 = File.OpenRead(this.ScriptFilename);
      this.ScriptHashSum = iSHA.ComputeHash((Stream) inputStream1);
      inputStream1.Close();
      FileStream inputStream2 = File.OpenRead(this.ModelFilename);
      this.ModelHashSum = iSHA.ComputeHash((Stream) inputStream2);
      inputStream2.Close();
    }

    public string ScriptFilename { get; private set; }

    public byte[] ScriptHashSum { get; private set; }

    public string ModelFilename { get; private set; }

    public byte[] ModelHashSum { get; private set; }

    public Rulesets RulesetType => this.mRulesetType;
  }
}
