using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class RotationReceive : ReceiveBase
    {

        //private Vector3 position = Vector3.zero;
        //private HubConnection connection;
        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<Vector3> m_onReceive = new UnityEvent<Vector3>();
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
                        object receivedMessage1 = Vector3.zero;
                        object receivedMessageBuff = Vector3.zero;
                        object receivedMessageBuff2 = Vector3.zero;
                        object receivedMessageBuff3 = Vector3.zero;
                        object receivedMessageBuff4 = Vector3.zero;


                        if (packet.TryReadFloat())
                        {
                            float tempX = packet.GetFloat();
                            if (packet.TryReadFloat())
                            {
                                float tempY = packet.GetFloat();
                                if (packet.TryReadFloat())
                                {
                                    float tempZ = packet.GetFloat();
                                    receivedMessage1 = new Vector3(tempX, tempY, tempZ);
                                }

                            }
                        }

                        if (packet.TryReadFloat())
                        {
                            float tempX = packet.GetFloat();
                            if (packet.TryReadFloat())
                            {
                                float tempY = packet.GetFloat();
                                if (packet.TryReadFloat())
                                {
                                    float tempZ = packet.GetFloat();
                                    receivedMessageBuff = new Vector3(tempX, tempY, tempZ);
                                }

                            }
                        }

                        if (packet.TryReadFloat())
                        {
                            float tempX = packet.GetFloat();
                            if (packet.TryReadFloat())
                            {
                                float tempY = packet.GetFloat();
                                if (packet.TryReadFloat())
                                {
                                    float tempZ = packet.GetFloat();
                                    receivedMessageBuff2 = new Vector3(tempX, tempY, tempZ);
                                }

                            }

                        }

                        if (packet.TryReadFloat())
                        {
                            float tempX = packet.GetFloat();
                            if (packet.TryReadFloat())
                            {
                                float tempY = packet.GetFloat();
                                if (packet.TryReadFloat())
                                {
                                    float tempZ = packet.GetFloat();
                                    receivedMessageBuff3 = new Vector3(tempX, tempY, tempZ);
                                }

                            }
                        }

                        if (packet.TryReadFloat())
                        {
                            float tempX = packet.GetFloat();
                            if (packet.TryReadFloat())
                            {
                                float tempY = packet.GetFloat();
                                if (packet.TryReadFloat())
                                {
                                    float tempZ = packet.GetFloat();
                                    receivedMessageBuff4 = new Vector3(tempX, tempY, tempZ);
                                }

                            }
                        }

                        if (receivedMessage1 != (object)Vector3.zero)
                        {
                            Message receivedMessage = new Message(key, packet.GetSenderId(), receivedMessage1);

                            base.AddObject(receivedMessage, key);
                        }



                        if (messages.Count >= 3 && receivedMessageBuff != (object)Vector3.zero)
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

                        if (messages.Count >= 4 && receivedMessageBuff2 != (object)Vector3.zero)
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

                        if (messages.Count >= 5 && receivedMessageBuff3 != (object)Vector3.zero)
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

                        if (messages.Count >= 6 && receivedMessageBuff4 != (object)Vector3.zero)
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
                    Vector3 result = (Vector3)message.messageObject;
                    m_onReceive.Invoke(result);
                    m_onReceiveText.Invoke(result.ToString());
                    //Debug.Log(angle.ToString());
                    transform.rotation = Quaternion.Euler(result.x, result.y, result.z);
                    //Debug.Log(transform.name.ToString());
                }
            }

        }

        public void DebugReceived(object value)
        {
            Debug.Log(value.ToString());
        }


    }
}