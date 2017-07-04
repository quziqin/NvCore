using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
public class NativeStreamReader 
{
    private IntPtr dataPtr;
    private IntPtr cuPtr; 
    private Int32 dataSize;
    private Int32 remainSize;
    public NativeStreamReader()
    {
        dataSize = 0;
    }

    public void Init(IntPtr ptr)
    {
        dataSize = Marshal.ReadInt32(ptr);
        dataPtr=ptr;
        Reset();
    }

    public Int32 Size()
    {
        return remainSize;
    }

    public bool Remain(int size)
    {
        return remainSize >= size;
    }
    public void Reset()
    {
        cuPtr = dataPtr;
        remainSize = dataSize ;
        if(remainSize >= 4)
            Step(4);
    }

    private bool Step ( int s )
    {
        remainSize=remainSize-s; 
        //DebugUtil.Assert(remainSize>=0 ,"Native stream out of bound!!");
        if (remainSize>=0){
            cuPtr = new IntPtr(cuPtr.ToInt64() + s);
            return true;
        }
       
        return false;
    }

     public int  ReadInt()
    {
#if DEBUG
         if(!Remain(4))
         {
             //DebugUtil.LogError("[jl - SR]: read int exception in native stream reader.");
             return -1;
         }
#endif
        int i=Marshal.ReadInt32(cuPtr);
        Step(4);
        return i;
    }

    public int  ReadByte()
    {
#if DEBUG
        if (!Remain(1))
        {
            //DebugUtil.LogError("[jl - SR]: read byte exception in native stream reader.");
            return -1;
        }
#endif
        byte i = Marshal.ReadByte(cuPtr);
        Step(1);
        return i;
    }
    

    //public static void Copy(IntPtr source, byte[] destination, int startIndex, int length);
    public int ReadBytes(byte[] bytes,  int length)
    {
#if DEBUG
        if (!Remain(length))
        {
            //DebugUtil.LogError("[jl - SR]: read bytes exception in native stream reader.");
            return -1;
        }
#endif
        Marshal.Copy(cuPtr, bytes, 0, length);
        Step(length);
        return length;
    }

    public string ReadUTFBytes(int length)
    {
        byte[] temp = new byte[length + 1];
        if (ReadBytes(temp, length) == length)
        {
            temp[length]        = 0;
            System.Text.Encoding ec = System.Text.Encoding.UTF8;
            return ec.GetString(temp, 0, (int)length);
        }
        return null;
    }

    public string ReadString()
    {
#if DEBUG
        if (!Remain(4))
        {
            //DebugUtil.LogError("[jl - SR]: read string exception in native stream reader.");
            return "";
        }
#endif
        Int32 l;
        l=ReadInt();
        string s;
        if (l > 0)
        {
            s = ReadUTFBytes(l-1);
            ReadByte();
        }
        else s = "";
        return s; 
    }

    public Vector3 ReadVector2()
    {
#if DEBUG
        if (!Remain(8))
        {
            //DebugUtil.LogError("[jl - SR]: read vector2 exception in native stream reader.");
            return Vector3.zero;
        }
#endif
        Int32 x; 
        Int32 y; 
        x=ReadInt(); 
        y=ReadInt();
        return new Vector2(x, y);
    }

};