using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class GrabReceive : ReceiveBase
    {


        private string message = "";
        private bool grabbed = false;


        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<bool> m_onReceive = new UnityEvent<bool>();
        [FormerlySerializedAs("onReceiveText")]
        [SerializeField]
        private UnityEvent<string> m_onReceiveText = new UnityEvent<string>();
        object lockReceive = new object();
        int currentPlayerId = 0;

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
                    currentPlayerId = UDPConnectionManager.singleton.GetPlayerId();
                    if (!ContainsKey(key) && packet.GetSenderId() != currentPlayerId)
                    {
                        bool receivedMessage1 = packet.GetBool();
                        bool? receivedMessageBuff = null;
                        if (packet.TryReadBool())
                        {
                            receivedMessageBuff = packet.GetBool();
                        }
                        bool? receivedMessageBuff2 = null;
                        if (packet.TryReadBool())
                        {
                            receivedMessageBuff2 = packet.GetBool();
                        }
                        bool? receivedMessageBuff3 = null;
                        if (packet.TryReadBool())
                        {
                            receivedMessageBuff3 = packet.GetBool();
                        }

                        bool? receivedMessageBuff4 = null;
                        if (packet.TryReadBool())
                        {
                            receivedMessageBuff4 = packet.GetBool();
                        }

                        Message receivedMessage = new Message(key, packet.GetSenderId(), receivedMessage1);
                        base.AddObject(receivedMessage, key);


                        if (messages.Count >= 3 && receivedMessageBuff != null)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 2);
                            if ((temp != null && temp.messageObject != (object)receivedMessageBuff) || temp == null)
                            {
                                int tempKey = key - 1;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }

                        }

                        if (messages.Count >= 4 && receivedMessageBuff2 != null)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 3);
                            if ((temp != null && temp.messageObject != (object)receivedMessageBuff2) || temp == null)
                            {
                                int tempKey = key - 2;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff2);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }

                        }

                        if (messages.Count >= 5 && receivedMessageBuff3 != null)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 4);
                            if ((temp != null && temp.messageObject != (object)receivedMessageBuff3) || temp == null)
                            {
                                int tempKey = key - 3;
                                Message tempReceived = new Message(tempKey, packet.GetSenderId(), receivedMessageBuff3);
                                if (ContainsKey(tempKey))
                                {
                                    AddObject(tempReceived, tempKey);
                                }
                            }
                        }

                        if (messages.Count >= 6 && receivedMessageBuff4 != null)
                        {
                            Message temp = (Message)messages.GetByIndex(messages.Count - 5);
                            if ((temp != null && temp.messageObject != (object)receivedMessageBuff4) || temp == null)
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
                    //throw new System.NotImplementedException();
                    //Debug.Log(prefix);
                    grabbed = (bool)message.messageObject;
                    m_onReceive.Invoke(grabbed);
                    m_onReceiveText.Invoke(grabbed.ToString());
                }
            }


        }

        public bool IsGrabbed()
        {
            return grabbed;
        }

        public void DebugReceived(object value)
        {
            Debug.Log(value.ToString());
        }

    }
}