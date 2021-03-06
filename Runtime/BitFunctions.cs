using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{
    public class BitFunctions
    {

        protected const byte BZ = 0, B0 = 1 << 0, B1 = 1 << 1, B2 = 1 << 2, B3 = 1 << 3, B4 = 1 << 4, B5 = 1 << 5, B6 = 1 << 6, B7 = 1 << 7;

        protected byte[] bytesForint;
        protected bool[] bytes2Bits;
        // Start is called before the first frame update

        public void Setup()
        {
            bytesForint = new byte[4];
            bytes2Bits = new bool[64];
        }

        public bool[] BitsReverseLen(bool[] bits, ref int start, int lenght)
        {

            int startTemp = start;
            bool[] a = new bool[lenght];
            bool[] b = new bool[lenght];

            for (int i = 0, j = lenght - 1; i < lenght; ++i, --j)
            {
                a[i] = bits[start + i];
                b[j] = bits[start + j];

                a[i] = a[i] ^ b[j];
                b[j] = a[i] ^ b[j];
                a[i] = a[i] ^ b[j];
            }
            start += lenght;
            return a;

        }

        public bool[] Byte2Bool(byte[] bytes)
        {

            bool[] res = new bool[bytes.Length * 8];
            int index = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                index = i * 8;
                res[index + 0] = (bytes[i] & (1 << 0)) == 0 ? false : true;
                res[index + 1] = (bytes[i] & (1 << 1)) == 0 ? false : true;
                res[index + 2] = (bytes[i] & (1 << 2)) == 0 ? false : true;
                res[index + 3] = (bytes[i] & (1 << 3)) == 0 ? false : true;
                res[index + 4] = (bytes[i] & (1 << 4)) == 0 ? false : true;
                res[index + 5] = (bytes[i] & (1 << 5)) == 0 ? false : true;
                res[index + 6] = (bytes[i] & (1 << 6)) == 0 ? false : true;
                res[index + 7] = (bytes[i] & (1 << 7)) == 0 ? false : true;

            }
            return res;

        }


        public uint BitArrayToUInt(bool[] array)
        {

            uint res = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                {
                    res |= (uint)(1 << i);
                }
            }
            return res;


        }


        public uint CountBits(int n)
        {

            int count = 0;
            while (n != 0)
            {
                count++;
                n >>= 1;
            }

            return (uint)count;


        }

        public void AppendBitsFromBoolArrayToBoolComplete(ref int index, int len, bool[] bitArray, ref bool[] bitBuilder)
        {

            for (int i = len - 1; i > -1; i--)
            {
                bitBuilder[index] = bitArray[i];
                index++;
            }

        }

        public void AppendBitsFromBoolArrayToBool(ref int index, int len, bool[] bitArray, ref bool[] bitBuilder)
        {

            for (int i = 0; i < len; i++, index++)
            {
                bitBuilder[index] = bitArray[i];
            }

        }


        public bool[] ByteArray2BitArray(byte[] bytes)
        {

            Array.Clear(bytes2Bits, 0, bytes2Bits.Length);

            //Array.Resize(ref bytes2Bits, bytes2Bits.Length);


            for (int i = 0; i < bytes.Length * 8; i++)
            {


                if ((bytes.Length * 8) - i == 1)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i == 2)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i == 3)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;

                }
                if ((bytes.Length * 8) - i == 4)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i == 5)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i == 6)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i == 7)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }
                if ((bytes.Length * 8) - i >= 8)
                {
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                    i++;
                    if ((bytes[i / 8] & (1 << ((i % 8)))) > 0) // (7 - (i % 8)) non reverse
                        bytes2Bits[i] = true;
                }

            }

            return bytes2Bits;

        }

        public byte[] PackBoolsInByteArray(bool[] bools, int len)
        {

            int rem = len & 0x07; // hint: rem = len % 8.


            byte[] byteArr = rem == 0 // length is a multiple of 8? (no remainder?)
                ? new byte[len >> 3] // -yes-
                : new byte[(len >> 3) + 1]; // -no-


            byte b;
            int i = 0;
            for (int mul = len & ~0x07; i < mul; i += 8) // hint: len = mul + rem.
            {
                b = bools[i] ? B0 : BZ;
                if (bools[i + 1]) b |= B1;
                if (bools[i + 2]) b |= B2;
                if (bools[i + 3]) b |= B3;
                if (bools[i + 4]) b |= B4;
                if (bools[i + 5]) b |= B5;
                if (bools[i + 6]) b |= B6;
                if (bools[i + 7]) b |= B7;

                byteArr[i >> 3] = b;
                //yield return b;
            }

            if (rem != 0) // take care of the remainder...
            {
                b = bools[i] ? B0 : BZ; // (there is at least one more bool.)

                switch (rem) // rem is [1:7] (fall-through switch!)
                {
                    case 7:
                        if (bools[i + 6]) b |= B6;
                        goto case 6;
                    case 6:
                        if (bools[i + 5]) b |= B5;
                        goto case 5;
                    case 5:
                        if (bools[i + 4]) b |= B4;
                        goto case 4;
                    case 4:
                        if (bools[i + 3]) b |= B3;
                        goto case 3;
                    case 3:
                        if (bools[i + 2]) b |= B2;
                        goto case 2;
                    case 2:
                        if (bools[i + 1]) b |= B1;
                        break;
                        // case 1 is the statement above the switch!
                }

                byteArr[i >> 3] = b; // write the last byte to the array.
                                     //yield return b; // yield the last byte.
            }

            return byteArr;

        }


        public byte[] IntToBytes(int value)
        {

            //Array.Clear(bytesForint, 0, bytesForint.Length);
            unchecked
            {
                bytesForint[3] = (byte)(value >> 24);
                bytesForint[2] = (byte)(value >> 16);
                bytesForint[1] = (byte)(value >> 8);
                bytesForint[0] = (byte)value;
            }

            return bytesForint;

        }

    }
}