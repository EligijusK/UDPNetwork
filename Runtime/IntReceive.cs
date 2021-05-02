using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UDPNetwork
{
    public class IntReceive : ReceiveBase
    {

        private string message = "";
        private int value = 0;


        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<int> m_onReceive = new UnityEvent<int>();

        [FormerlySerializedAs("onReceiveText")]
        [SerializeField]
        private UnityEvent<string> m_onReceiveText = new UnityEvent<string>();

        object lockReceive = new object();

        new void Start()
        {
            base.Start();
        }


        public override void AddMessage(Packet message)
        {
            lock (lockReceive)
            {
                if (scriptEnabled)
                {
                    Packet packet = message;
                    int key = packet.GetPackageIndex();
                    if (!ContainsKey(key))
                    {
                        int receivedMessage1 = packet.GetInt();
                        int receivedMessageBuff = -1;
                        if (packet.TryReadInt())
                        {
                            receivedMessageBuff = packet.GetInt();
                        }
                        int receivedMessageBuff2 = -1;
                        if (packet.TryReadInt())
                        {
                            receivedMessageBuff2 = packet.GetInt();
                        }
                        int receivedMessageBuff3 = -1;
                        if (packet.TryReadInt())
                        {
                            receivedMessageBuff3 = packet.GetInt();
                        }
                        int receivedMessageBuff4 = -1;
                        if (packet.TryReadInt())
                        {
                            receivedMessageBuff4 = packet.GetInt();
                        }

                        Message receivedMessage = new Message(key, packet.GetSenderId(), receivedMessage1);
                        base.AddObject(receivedMessage, key);



                        if (messages.Count >= 3 && receivedMessageBuff != -1)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 2);
                            if (temp.messageObject != (object)receivedMessageBuff)
                            {
                                int tempKey = key - 1;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }
                        }

                        if (messages.Count >= 4 && receivedMessageBuff2 != -1)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 3);
                            if (temp.messageObject != (object)receivedMessageBuff2)
                            {
                                int tempKey = key - 2;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff2);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }
                        }

                        if (messages.Count >= 5 && receivedMessageBuff3 != -1)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 4);
                            if (temp.messageObject != (object)receivedMessageBuff3)
                            {
                                int tempKey = key - 3;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff3);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }
                        }

                        if (messages.Count >= 6 && receivedMessageBuff4 != -1)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 5);
                            if (temp.messageObject != (object)receivedMessageBuff4)
                            {
                                int tempKey = key - 4;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff4);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }
                        }

                    }
                }
            }


        }


        protected override void MessageReceived()
        {
            if (scriptEnabled)
            {
                Message message = (Message)GetMessageFromBuffer();
                if (message != null)
                {
                    value = (int)message.messageObject;
                    m_onReceive.Invoke((int)message.messageObject);
                    m_onReceiveText.Invoke(value.ToString());
                }
            }


        }

        public void DebugReceived(int value)
        {
            Debug.Log(value.ToString());
        }

    }
}