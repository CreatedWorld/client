/* 
*┌──────────────────────────────────┐
*│　         Copyright(C) 2017 by Antiphon.All rights reserved.       │
*│                Author:by Locke Xie 2017-04-01  　　　　　　　　  　│
*└──────────────────────────────────┘
*
* 功 能： 第三方音频压缩
* 类 名： Speex.cs
* 
* 修改历史：
* 
* 
*/

using NSpeex;
using UnityEngine;

public static class Speex
 {

    static SpeexEncoder speexEnc = new SpeexEncoder(BandMode.Narrow);
    static SpeexDecoder speexDec = new SpeexDecoder(BandMode.Narrow);

    public static byte[] SpeexCompress(float[] input, out int length)
    {
        short[] shortBuffer = new short[input.Length];
        byte[] encoded = new byte[input.Length];
        input.ToShortArray(shortBuffer);
        length = speexEnc.Encode(shortBuffer, 0, input.Length, encoded, 0, encoded.Length);
        return encoded;
    }

    public static float[] DeCompress(byte[] data, int dataLength)
    {
        float[] decoded = new float[data.Length];
        short[] shortBuffer = new short[data.Length];
        speexDec.Decode(data, 0, dataLength, shortBuffer, 0, false);
        shortBuffer.ToFloatArray(decoded, shortBuffer.Length);
        return decoded;
    }

    /// <summary>
    /// short to float
    /// </summary>
    /// <param PlayerName="input"></param>
    /// <param PlayerName="output"></param>
    public static void ToShortArray(this float[] input, short[] output)
    {
        if(output.Length < input.Length)
        {
            return;
        }

        for(int i = 0; i < input.Length; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i] * 32767.0f), short.MinValue, short.MaxValue);
        }
    }

    /// <summary>
    /// float to short
    /// </summary>
    /// <param PlayerName="input"></param>
    /// <param PlayerName="output"></param>
    /// <param PlayerName="length"></param>
    public static void ToFloatArray(this short[] input, float[] output, int length)
    {
        if(output.Length < length || input.Length < length)
        {
            return;
        }

        for(int i = 0; i < length; ++i)
        {
            output[i] = input[i] / (float)short.MaxValue;
        }
    }
}
