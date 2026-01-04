// Decompiled with JetBrains decompiler
// Type: Magicka.WebTools.InternalSteamUtils
// Assembly: Magicka, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 20B30093-0B41-4B13-B130-C3B04DD4E3C2
// Assembly location: C:\SteamLibrary\steamapps\common\Magicka\Magicka.exe

using SteamWrapper;
using System;

#nullable disable
namespace Magicka.WebTools;

public static class InternalSteamUtils
{
  public const int STEAM_AUTH_BUFFER_SIZE = 1024 /*0x0400*/;
  private static InternalSteamUtils.BufferClass mBufferClass = new InternalSteamUtils.BufferClass();

  public static uint GetSteamAppID() => SteamUtils.GetAppID();

  public static unsafe string GetSteamAuthToken()
  {
    uint length = 0;
    byte[] numArray;
    fixed (byte* pTicket = InternalSteamUtils.mBufferClass.mSteamBuffer.mBuffer)
    {
      int authSessionTicket = (int) SteamUser.GetAuthSessionTicket((void*) pTicket, 1024 /*0x0400*/, &length);
      numArray = new byte[(IntPtr) length];
      for (int index = 0; (long) index < (long) length; ++index)
        numArray[index] = pTicket[index];
    }
    return BitConverter.ToString(numArray).Replace("-", string.Empty);
  }

  private struct SteamAuthBuffer
  {
    public unsafe fixed byte mBuffer[1024];
  }

  private class BufferClass
  {
    public InternalSteamUtils.SteamAuthBuffer mSteamBuffer = new InternalSteamUtils.SteamAuthBuffer();
  }
}
