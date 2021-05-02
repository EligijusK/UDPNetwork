using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
namespace UDPNetwork
{
    [System.Serializable]
    public class PacketHeader : BitFunctions
    {

        protected bool[] headerInBits;

        // packet creation
        protected bool[] header = new bool[768];
        protected int headerIndex = 0;

        // receive
        protected bool[] receivedMessage;
        protected byte[] received;
        protected int currentIndex = 0;
        protected int prefixIndex = 0;
        protected int playerId = 0;
        protected uint res;
        protected int packageLen = 0;
        protected int packetReceiveIndex = 0;

        protected string packetPrefix;
        protected int senderPlayerId = 0;

        protected Packet.PacketType packetType;

        public PacketHeader(string prefix)
        {
            base.Setup();
            packetPrefix = prefix;
            packetReceiveIndex = 0;
            packetType = Packet.PacketType.User;

            int byteCountMaxPlayer = UDPConnectionManager.GetMaxPlayerCount();
            byteCountMaxPlayer = (int)CountBits(byteCountMaxPlayer); // bit count for player index

            senderPlayerId = UDPConnectionManager.singleton.GetPlayerId();

            int bitLen = byteCountMaxPlayer + 5 + Packet.GetPrefixBitLen();
            headerInBits = new bool[bitLen];

            int headerIndex = 0;

            bool[] bytes = ByteArray2BitArray(IntToBytes((int)senderPlayerId));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, byteCountMaxPlayer, bytes, ref headerInBits);

            bytes = ByteArray2BitArray(IntToBytes((int)Packet.GetPrefixBitLen()));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, 5, bytes, ref headerInBits);

            int prefixIndex = UDPConnectionManager.PrefixIndex(prefix);

            bytes = ByteArray2BitArray(IntToBytes(prefixIndex));



            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, Packet.GetPrefixBitLen(), bytes, ref headerInBits);



        }

        public PacketHeader(byte[] receive)
        {
            base.Setup();
            this.currentIndex = 0;
            packetReceiveIndex = -1;

            received = receive;
            this.receivedMessage = Byte2Bool(receive);

            int typeLen = (int)CountBits((int)Packet.PacketType.NumberOfValues);

            bool[] packetConsoleBit = BitsReverseLen(receivedMessage, ref this.currentIndex, typeLen);
            res = BitArrayToUInt(packetConsoleBit);

            bool[] packetLenBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 7);

            res = BitArrayToUInt(packetLenBits); // package lenght for data check
            packageLen = (int)res;

            bool[] packetIndexBits = BitsReverseLen(receivedMessage, ref this.currentIndex, 32);
            res = BitArrayToUInt(packetIndexBits); // package lenght for data check
            packetReceiveIndex = (int)res;


            int byteCountMaxPlayer = UDPConnectionManager.GetMaxPlayerCount();
            byteCountMaxPlayer = (int)CountBits(byteCountMaxPlayer);


            packetLenBits = BitsReverseLen(receivedMessage, ref currentIndex, byteCountMaxPlayer);
            playerId = (int)BitArrayToUInt(packetLenBits);

            packetLenBits = BitsReverseLen(receivedMessage, ref currentIndex, 5);
            uint lenPrefix = BitArrayToUInt(packetLenBits);

            packetLenBits = BitsReverseLen(receivedMessage, ref currentIndex, (int)lenPrefix);
            prefixIndex = (int)BitArrayToUInt(packetLenBits);


            //PrintValues(packetLenBits, 0);
        }


        public string GetPrefix()
        {
            return UDPConnectionManager.PrefixByIndex(prefixIndex);
        }

        public int GetSenderId()
        {
            return this.playerId;
        }

        public int GetPackageIndex()
        {
            return packetReceiveIndex;
        }

        public void ResetPacketHeader(int prefixBitLen)
        {

            packetType = Packet.PacketType.User;

            int byteCountMaxPlayer = UDPConnectionManager.GetMaxPlayerCount();
            byteCountMaxPlayer = (int)CountBits(byteCountMaxPlayer); // bit count for player index

            int bitLen = byteCountMaxPlayer + 5 + prefixBitLen;
            headerInBits = new bool[bitLen];

            int headerIndex = 0;

            senderPlayerId = UDPConnectionManager.singleton.GetPlayerId();

            bool[] bytes = ByteArray2BitArray(IntToBytes((int)senderPlayerId));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, byteCountMaxPlayer, bytes, ref headerInBits);

            bytes = ByteArray2BitArray(IntToBytes(prefixBitLen));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, 5, bytes, ref headerInBits);

            int prefixIndex = UDPConnectionManager.PrefixIndex(packetPrefix);

            bytes = ByteArray2BitArray(IntToBytes(prefixIndex));

            AppendBitsFromBoolArrayToBoolComplete(ref headerIndex, prefixBitLen, bytes, ref headerInBits);

        }

        public void SetPacketType(Packet.PacketType packetType)
        {
            this.packetType = packetType;
        }

        public int GetPacketType()
        {
            return (int)packetType;
        }

        public void SetSenderId(int senderId)
        {
            senderPlayerId = senderId;
        }

        public bool[] ReturnHeader()
        {
            return headerInBits;
        }


        public int GetPackageLen()
        {
            return packageLen;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        public bool[] GetReceived()
        {
            return receivedMessage;
        }


    }
}