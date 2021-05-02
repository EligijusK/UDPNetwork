using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{
    [System.Serializable]
    public class Packet : BitFunctions
    {

        public enum PacketType
        {
            User = 0,
            ServerCommand = 1,
            UserServer = 2,
            NumberOfValues,
        }

        protected static int prefixBitLen = 0;
        protected int packetPrefixBitLen = 0;
        protected int preFixBitLenFloat = 4; // 13 in bits len
        protected int preFixBitLenInt = 5; // 20 in bits len

        protected bool[] resultsInBits = new bool[768];
        protected bool[] headerInBits;
        protected int indexRefBits = 0;

        // bit count initialization;
        protected static int count = 0;

        // indexes for types
        protected uint boolIndex = 3;
        protected uint floatIndex = 1;
        protected uint intIndex = 2;

        // packet creation
        protected bool[] header = new bool[768];
        protected int headerIndex = 0;
        protected float messageLen = 0;
        protected int packetIndex = 0;
        protected bool increaseIndexManualy = false;

        // receive
        protected bool[] receivedMessage;
        protected byte[] received;
        protected int currentIndex = 0;
        protected List<int> prevIndexArray;
        //protected int prefixIndex = 0;
        //protected int playerId = 0;
        protected uint res;
        protected int packageLen = 0;
        // protected int packetReceiveIndex = 0;


        //bool to byte array

        protected int lenghtlen = 0;
        protected int bytes = 0;
        protected byte[] resultInBytes;


        protected string packetPrefix;

        protected PacketType packetType;

        protected PacketHeader packetHeader;

        public Packet(string prefix)
        {
            base.Setup();
            packetPrefix = prefix;
            indexRefBits = 0;
            this.increaseIndexManualy = false;

            packetHeader = new PacketHeader(prefix);
            headerInBits = packetHeader.ReturnHeader();

            AppendBitsFromBoolArrayToBool(ref indexRefBits, headerInBits.Length, headerInBits, ref resultsInBits); // fishy

        }

        public Packet(string prefix, bool increaseIndexManualy)
        {
            base.Setup();
            packetPrefix = prefix;
            indexRefBits = 0;
            this.increaseIndexManualy = increaseIndexManualy;

            packetHeader = new PacketHeader(prefix);
            headerInBits = packetHeader.ReturnHeader();

            AppendBitsFromBoolArrayToBool(ref indexRefBits, headerInBits.Length, headerInBits, ref resultsInBits); // fishy

        }

        public Packet(byte[] receive)
        {
            base.Setup();
            this.currentIndex = 0;

            received = receive;
            this.receivedMessage = Byte2Bool(receive);

            packetHeader = new PacketHeader(receive);

            packageLen = packetHeader.GetPackageLen();

            currentIndex = packetHeader.GetCurrentIndex();

            prevIndexArray = new List<int>();

            //PrintValues(packetLenBits, 0);
        }

        public Packet(PacketHeader packetHeader)
        {
            base.Setup();
            this.currentIndex = 0;

            this.packetHeader = packetHeader;

            this.receivedMessage = this.packetHeader.GetReceived();


            packageLen = this.packetHeader.GetPackageLen();

            currentIndex = this.packetHeader.GetCurrentIndex();

            prevIndexArray = new List<int>();
        }

        public Packet()
        {
            base.Setup();
        }

        public string GetPrefix()
        {
            return packetHeader.GetPrefix();
        }

        public void Next()
        {
            // calculate next index by checking typr and adding indexes
            prevIndexArray.Add(this.currentIndex);

            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

            if (tempRes == floatIndex)
            {
                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, preFixBitLenFloat);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                this.currentIndex += (int)tempRes;
                this.currentIndex++;
                this.currentIndex += 10;

            }
            else if (tempRes == intIndex)
            {
                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, preFixBitLenInt);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                this.currentIndex += (int)tempRes;
                this.currentIndex++;
                //this.currentIndex += preFixBitLenInt;
            }
            else if (tempRes == boolIndex)
            {
                this.currentIndex++;
            }
        }

        public void Previous()
        {
            // list with prev current index
            this.currentIndex = prevIndexArray[prevIndexArray.Count - 1];
            prevIndexArray.Remove(this.currentIndex);

        }

        public bool TryReadFloat()
        {
            int index = this.currentIndex;
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref index, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                                                          //Debug.Log("index of start in float: " +  this.currentIndex);

            if (tempRes == floatIndex && this.currentIndex < (packageLen * 8))
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryReadVector3()
        {
            int index = this.currentIndex;
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref index, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                                                          //Debug.Log("index of start in float: " +  this.currentIndex);

            if (tempRes == floatIndex && index < (packageLen * 8) && (index + 11 + 1 + preFixBitLenFloat) < (packageLen * 8))
            {

                packetLenBits = BitsReverseLen(receivedMessage, ref index, preFixBitLenFloat);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                index += (int)tempRes;
                index += 11;

                if ((index + 2) < (packageLen * 8) && (index + 2 + 11 + 1 + preFixBitLenFloat) < (packageLen * 8))
                {

                    packetLenBits = BitsReverseLen(receivedMessage, ref index, preFixBitLenFloat);
                    tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                    index += (int)tempRes;
                    index += 11;

                    if ((index + 2) < (packageLen * 8) && (index + 2 + 11 + 1 + preFixBitLenFloat) < (packageLen * 8))
                    {

                        return true;
                    }

                    else
                    {
                        return false;
                    }


                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public float GetFloat()
        {
            // read float or throw message that it can't be read
            prevIndexArray.Add(this.currentIndex);
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                                                          //Debug.Log("index of start in float: " +  this.currentIndex);
            if (tempRes == floatIndex)
            {
                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, preFixBitLenFloat);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

                bool sign = receivedMessage[this.currentIndex];
                this.currentIndex++;

                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, (int)tempRes);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

                float res = (float)tempRes;

                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 10);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

                float fraction = ((float)tempRes) / 1000;
                res = res + fraction;

                //Debug.Log(gg);



                if (sign)
                {
                    res = res * -1;
                }

                return res;

            }
            else
            {
                Previous();
                //PrintValues(receivedMessage, 0);
                throw new ArgumentException("Float cannot be read");

            }
        }

        public bool TryReadInt()
        {
            int index = this.currentIndex;
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref index, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                                                          //Debug.Log("index of start in float: " +  this.currentIndex);

            if (tempRes == intIndex && this.currentIndex < (packageLen * 8))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetInt()
        {
            // read int or throw message that it can't be read
            prevIndexArray.Add(this.currentIndex);
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
            if (tempRes == intIndex)
            {
                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, preFixBitLenInt);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

                bool sign = receivedMessage[this.currentIndex];
                this.currentIndex++;

                packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, (int)tempRes);
                tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check

                int result = (int)tempRes;

                if (sign)
                {
                    result = result * -1;
                }

                return result;

            }
            else
            {
                Previous();
                //PrintValues(receivedMessage, 0);
                throw new ArgumentException("Int cannot be read");

            }
        }

        public bool TryReadBool()
        {
            int index = this.currentIndex;
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref index, 2);
            uint tempRes = BitArrayToUInt(packetLenBits); // package lenght for data check
                                                          //Debug.Log("index of start in float: " +  this.currentIndex);

            if (tempRes == intIndex && this.currentIndex < (packageLen * 8))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetBool()
        {
            // read bool or throw message that it can't be read
            prevIndexArray.Add(this.currentIndex);
            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 2);

            uint res = BitArrayToUInt(packetLenBits); // package lenght for data check
            if (res == boolIndex)
            {
                bool value = receivedMessage[this.currentIndex];
                this.currentIndex++;
                return value;
            }
            else
            {
                Previous();
                //throw new ArgumentException("Bool cannot be read");
                return false;
            }
        }

        public int GetSenderId()
        {
            return packetHeader.GetSenderId();
        }

        public int GetPackageIndex()
        {
            return packetHeader.GetPackageIndex();
        }


        public static void SetPRefixBitLen(int prefixListLen)
        {

            if (prefixListLen <= 7)
            {
                prefixBitLen = 3;
            }
            else if (prefixListLen <= 15)
            {
                prefixBitLen = 4;
            }
            else if (prefixListLen <= 31)
            {
                prefixBitLen = 5;
            }
            else if (prefixListLen <= 63)
            {
                prefixBitLen = 6;
            }
            else if (prefixListLen <= 127)
            {
                prefixBitLen = 7;
            }
            else if (prefixListLen <= 255)
            {
                prefixBitLen = 8;
            }
            else if (prefixListLen <= 512)
            {
                prefixBitLen = 9;
            }
            else if (prefixListLen <= 1023)
            {
                prefixBitLen = 10;
            }
            else if (prefixListLen <= 2047)
            {
                prefixBitLen = 11;
            }
            else if (prefixListLen <= 4095)
            {
                prefixBitLen = 12;
            }
            else if (prefixListLen <= 8191)
            {
                prefixBitLen = 13;
            }
            else if (prefixListLen <= 16383)
            {
                prefixBitLen = 14;
            }
            else if (prefixListLen <= 32767)
            {
                prefixBitLen = 15;
            }
            else if (prefixListLen <= 65535)
            {
                prefixBitLen = 16;
            }




        }

        public static int GetPrefixBitLen()
        {
            return prefixBitLen;
        }



        public void AddFloat(float value)
        {

            int start = (int)Math.Truncate(value);
            int sign = 0;

            if (value >= 0)
            {
                sign = 0;
            }
            else
            {
                sign = 1;
            }

            start = Math.Abs(start);


            if (start > 8191) // max 13 bits
            {
                start = 8191;
            }

            int result = (int)((Math.Abs(value) % 1) * 1000); // max 10 bits

            if (result > 999)
            {
                result = 999;
            }

            bool[] lenBitsTest = ByteArray2BitArray(IntToBytes((int)floatIndex));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 2, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes((int)CountBits(start)));
            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, preFixBitLenFloat, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes((int)sign));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 1, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes(start));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, (int)CountBits(start), lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes(result));
            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 10, lenBitsTest, ref resultsInBits);


        }

        public void AddInt(int value)
        {

            int result = value;
            int sign = 0;

            if (value >= 0)
            {
                sign = 0;
            }
            else
            {
                sign = 1;
            }

            result = Math.Abs(result);
            if (result > 1048575) // max 20 bits
            {
                result = 1048575;
            }

            bool[] lenBitsTest = ByteArray2BitArray(IntToBytes((int)intIndex));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 2, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes((int)CountBits(result)));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, preFixBitLenInt, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes((int)sign));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 1, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes(result));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, (int)CountBits(result), lenBitsTest, ref resultsInBits);

        }

        public void AddBool(bool value)
        {

            int result = value ? 1 : 0;

            bool[] lenBitsTest = ByteArray2BitArray(IntToBytes((int)boolIndex));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 2, lenBitsTest, ref resultsInBits);

            lenBitsTest = ByteArray2BitArray(IntToBytes((int)result));

            AppendBitsFromBoolArrayToBoolComplete(ref indexRefBits, 1, lenBitsTest, ref resultsInBits);


        }

        public void ResetPacket()
        {

            packetHeader.ResetPacketHeader(prefixBitLen);
            headerInBits = packetHeader.ReturnHeader();

            Array.Clear(resultsInBits, 0, resultsInBits.Length);
            indexRefBits = 0;
            AppendBitsFromBoolArrayToBool(ref indexRefBits, headerInBits.Length, headerInBits, ref resultsInBits);
        }

        public byte[] CreatePacket()
        {

            Array.Clear(header, 0, header.Length);

            headerIndex = 0;
            int bitCount = (int)CountBits((int)PacketType.NumberOfValues);
            messageLen = indexRefBits + bitCount + 7 + 32; // + 7 for packet length, 1 + for console check, 1 + for lenght, 32 + for packet index
            messageLen = messageLen / 8;


            if (messageLen - Math.Truncate(messageLen) > 0)
            {
                messageLen++;
            }


            if (!increaseIndexManualy)
            {
                packetIndex = packetIndex + 7;
            }

            //Debug.Log("index milisecs: " + packetIndex + " index ticks: " + lastTwoTics);

            messageLen = (int)Math.Truncate(messageLen);

            int tempLen = (int)messageLen;

            bool[] lenBitsTest = ByteArray2BitArray(IntToBytes(packetHeader.GetPacketType()));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, bitCount, lenBitsTest, ref header);


            lenBitsTest = ByteArray2BitArray(IntToBytes(Math.Max(1, (int)messageLen)));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, 7, lenBitsTest, ref header);


            lenBitsTest = ByteArray2BitArray(IntToBytes(packetIndex));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, 32, lenBitsTest, ref header);



            AppendBitsFromBoolArrayToBool(ref headerIndex, indexRefBits, resultsInBits, ref header);

            //StringBuilder sb = new StringBuilder();
            //foreach (var bit in header)
            //{
            //    sb.Append(bit ? "1" : "0");
            //}

            //Debug.Log("------------------------------------------------");
            //Debug.Log(sb.ToString());
            //Debug.Log(res);
            //Debug.Log("------------------------------------------------");

            //Debug.Log("packet length: " + (tempLen) + " actual package len: " + (headerIndex/8));

            return PackBoolsInByteArray(header, headerIndex);
        }

        public void SetIndex(int index)
        {
            packetIndex = index;
        }

        public int ReturnCurrentSendPacketIndex()
        {
            return packetIndex;
        }

        public void SetManualyIncresedIndex(bool value)
        {
            increaseIndexManualy = value;
        }

        public bool IncresaseManulayIndexCheck()
        {
            return increaseIndexManualy;
        }

        public void SetPacketType(PacketType packetType)
        {
            packetHeader.SetPacketType(packetType);
        }

        public static void PrintValues(BitArray b, int realRes)
        {

            IEnumerator enumerator = b.GetEnumerator();
            string res = "";
            while (enumerator.MoveNext())
            {
                //Debug.Log(enumerator.Current);
                if (bool.Parse(enumerator.Current.ToString()))
                {
                    res = res + "1";
                }
                else
                {
                    res = res + "0";
                }
            }
            Debug.Log(res);



        }

        public void SetSenderId(int senderId)
        {
            packetHeader.SetSenderId(senderId);
        }





    }
}