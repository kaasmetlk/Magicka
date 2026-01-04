// Decompiled with JetBrains decompiler
// Type: Magicka.GameLogic.UI.DialogCollection
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using PolygonHead;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#nullable disable
namespace Magicka.GameLogic.UI;

internal class DialogCollection : Dictionary<int, Dialog>
{
  private Dictionary<int, bool> mFinishedDialogs = new Dictionary<int, bool>();

  public DialogCollection(string iFileName)
  {
    this.LoadDialogs(iFileName);
    this.LoadDialogs(Path.Combine("content", "Levels/Dialog_Fairy.xml"));
    foreach (KeyValuePair<int, Dialog> keyValuePair in (Dictionary<int, Dialog>) this)
      this.mFinishedDialogs[keyValuePair.Key] = false;
  }

  private void LoadDialogs(string iFileName)
  {
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(iFileName);
    XmlNode xmlNode = (XmlNode) null;
    for (int i = 0; i < xmlDocument.ChildNodes.Count; ++i)
    {
      XmlNode childNode = xmlDocument.ChildNodes[i];
      if (childNode.Name.Equals("Dialogs", StringComparison.OrdinalIgnoreCase))
      {
        xmlNode = childNode;
        break;
      }
    }
    if (xmlNode == null)
      throw new Exception("No dialogs found!");
    for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
    {
      XmlNode childNode = xmlNode.ChildNodes[i];
      if (!(childNode is XmlComment))
      {
        Dialog dialog = childNode.Name.Equals("Dialog", StringComparison.OrdinalIgnoreCase) ? new Dialog(childNode) : throw new NotImplementedException();
        this.Add(dialog.ID, dialog);
      }
    }
  }

  public void Initialize(Scene iScene)
  {
    foreach (Dialog dialog in this.Values)
      dialog.Initialize(iScene);
  }

  public void DialogFinished(int iId) => this.mFinishedDialogs[iId] = true;

  public bool IsDialogDone(int iId) => this.mFinishedDialogs[iId];

  public void Reset()
  {
    foreach (KeyValuePair<int, Dialog> keyValuePair in (Dictionary<int, Dialog>) this)
    {
      keyValuePair.Value.Reset();
      this.mFinishedDialogs[keyValuePair.Key] = false;
    }
  }

  public class State
  {
    private Dictionary<int, bool> mFinishedDialogs = new Dictionary<int, bool>();
    private List<Dialog.State> mDialogStates = new List<Dialog.State>();
    private DialogCollection mOwner;

    public State(DialogCollection iOwner)
    {
      this.mOwner = iOwner != null ? iOwner : throw new ArgumentNullException(nameof (iOwner));
      foreach (KeyValuePair<int, Dialog> keyValuePair in (Dictionary<int, Dialog>) this.mOwner)
        this.mDialogStates.Add(new Dialog.State(keyValuePair.Value));
      this.UpdateState();
    }

    public void UpdateState()
    {
      this.mFinishedDialogs.Clear();
      foreach (KeyValuePair<int, bool> mFinishedDialog in this.mOwner.mFinishedDialogs)
        this.mFinishedDialogs.Add(mFinishedDialog.Key, mFinishedDialog.Value);
      for (int index = 0; index < this.mDialogStates.Count; ++index)
        this.mDialogStates[index].UpdateState();
    }

    public void ApplyState()
    {
      this.mOwner.mFinishedDialogs.Clear();
      foreach (KeyValuePair<int, bool> mFinishedDialog in this.mFinishedDialogs)
        this.mOwner.mFinishedDialogs.Add(mFinishedDialog.Key, mFinishedDialog.Value);
      for (int index = 0; index < this.mDialogStates.Count; ++index)
        this.mDialogStates[index].ApplayState();
    }

    internal void Write(BinaryWriter iWriter)
    {
      iWriter.Write(this.mFinishedDialogs.Count);
      foreach (KeyValuePair<int, bool> mFinishedDialog in this.mFinishedDialogs)
      {
        iWriter.Write(mFinishedDialog.Key);
        iWriter.Write(mFinishedDialog.Value);
      }
      iWriter.Write(this.mDialogStates.Count);
      foreach (Dialog.State mDialogState in this.mDialogStates)
      {
        iWriter.Write(mDialogState.Owner.ID);
        mDialogState.Write(iWriter);
      }
    }

    internal void Read(BinaryReader iReader)
    {
      this.mFinishedDialogs.Clear();
      int num1 = iReader.ReadInt32();
      for (int index = 0; index < num1; ++index)
        this.mFinishedDialogs[iReader.ReadInt32()] = iReader.ReadBoolean();
      int num2 = iReader.ReadInt32();
      for (int index1 = 0; index1 < num2; ++index1)
      {
        int num3 = iReader.ReadInt32();
        for (int index2 = 0; index2 < this.mDialogStates.Count; ++index2)
        {
          if (num3 == this.mDialogStates[index2].Owner.ID)
            this.mDialogStates[index2].Read(iReader);
        }
      }
    }
  }
}
