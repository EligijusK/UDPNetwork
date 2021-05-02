using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class ServerPacket
    {

        public enum ServerCommand : int // Server must have same enum for communication
        {
            Connect = 1,
            Disconnect = 2,
            Disconnected = 3,
            DisconnectedFromOther = 4,
            Connected = 5,
            Full = 6,
            Start = 7,
            Timer = 8,
            ConnectOther = 9,
            ConnectedOther = 10

        }

        string message = "";
        string header = "";
        int commandLen = 0;

        UnityEvent<bool> startEvent;

        bool connectToMap = false;

        object data;

        public ServerPacket()
        {

            connectToMap = false;
            this.commandLen = (int)StaicBitFunctions.CountBits((int)ServerCommand.ConnectedOther); // count bit size of last index in command enum

            int sendLen = (int)StaicBitFunctions.CountBits((int)Packet.PacketType.NumberOfValues);

            header = header + Convert.ToString((int)Packet.PacketType.ServerCommand, toBase: 2).PadLeft(sendLen, '0');
            message = header;
        }

        public void AddStartEvent(UnityEvent<bool> startEvent)
        {
            this.startEvent = startEvent;
        }

        public void Command(ServerCommand commandIndex)
        {
            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod(commandIndex.ToString());
            theMethod.Invoke(this, null);
        }

        public byte[] CommandPacket(ServerCommand commandIndex)
        {
            Debug.Log((int)commandIndex);
            message = header;
            message = message + Convert.ToString((int)commandIndex, toBase: 2).PadLeft(commandLen, '0');

            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod(commandIndex.ToString() + "Packet");
            theMethod.Invoke(this, null);

            bool[] bits = this.message.Select(c => c == '1').ToArray();

            return StaicBitFunctions.PackBoolsInByteArray(bits, bits.Length);
        }

        public void ConnectPacket()
        {

        }

        public void DisconnectPacket()
        {
            int maxPlayerBitCount = (int)StaicBitFunctions.CountBits(UDPConnectionManager.GetMaxPlayerCount());
            message = message + Convert.ToString(UDPConnectionManager.singleton.GetPlayerId(), toBase: 2).PadLeft(maxPlayerBitCount, '0');
        }

        public void DisconnectedPacket()
        {

        }

        public void StartPacket()
        {

        }

        public void Start()
        {
            if (!UDPConnectionManager.gameStarted && !connectToMap)
            {
                bool check = (bool)data;
                connectToMap = check;

                MainThreadManager.ExecuteOnMainThread(delegate
                {


                    startEvent.Invoke(check);
                    //Debug.Log("connecedToGame" + connectToMap.ToString());

                });
            }

        }

        public void Timer()
        {
            uint time = (uint)data;
            UDPConnectionManager.singleton.SetTime(time);

        }

        public void DisconnectedFromOtherPacket()
        {

        }

        public void ConnectedPacket()
        {

        }

        public void ConnectedOtherPacket()
        {

        }

        public void FullPacket()
        {

        }

        public void AddData(object dat)
        {
            data = dat;
        }

    }
}