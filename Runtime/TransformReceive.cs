using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class TransformReceive : ReceiveBase
    {



        [SerializeField]
        private bool useLerping = true;
        [SerializeField]
        private float lerpTime = 0.04f;
        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<Vector3> m_onReceive = new UnityEvent<Vector3>();
        [FormerlySerializedAs("onReceiveText")]
        [SerializeField]
        private UnityEvent<string> m_onReceiveText = new UnityEvent<string>();
        private float tempTimeCount = 0.04f;
        private float interpolateStepVal = 0f;
        private string message = "";

        private Vector3 receivedFourth;
        private Vector3 receivedThird;
        private Vector3 receivedSec;
        private Vector3 received;
        object lockReceive = new object();

        new void Start()
        {
            base.Start();

        }

        new void FixedUpdate()
        {
            if (useLerping)
            {

                if (tempTimeCount >= lerpTime)
                {
                    if (messages.Count > minFrameBuffered)
                    {
                        Message message = (Message)GetMessageFromBuffer();
                        Message messageSec = (Message)GetMessageFromBuffer();
                        Message messageThird = (Message)GetMessageFromBuffer();
                        Message messageFourth = (Message)GetMessageFromBuffer();

                        if (message != null && messageSec != null)
                        {

                            received = (Vector3)message.messageObject;
                            if (messageSec != null && messageThird != null && messageFourth != null)
                            {
                                receivedSec = (Vector3)messageSec.messageObject;
                                receivedThird = (Vector3)messageThird.messageObject;
                                receivedFourth = (Vector3)messageFourth.messageObject;

                                interpolateStepVal = 0;

                                Vector3 result = new Vector3(Lerp5(transform.position.x, received.x, receivedSec.x, receivedThird.x, receivedFourth.x, interpolateStepVal), Lerp5(transform.position.y, received.y, receivedSec.y, receivedThird.y, receivedFourth.y, interpolateStepVal), Lerp5(transform.position.z, received.z, receivedSec.z, receivedThird.z, receivedFourth.z, interpolateStepVal));

                                transform.position = result;
                            }
                            //packageIndex = message.GetPacketId();


                        }
                        tempTimeCount = 0;
                    }

                }
                else
                {

                    interpolateStepVal = (1 / lerpTime) * tempTimeCount;
                    Vector3 result = new Vector3(Lerp5(transform.position.x, received.x, receivedSec.x, receivedThird.x, receivedFourth.x, interpolateStepVal), Lerp5(transform.position.y, received.y, receivedSec.y, receivedThird.y, receivedFourth.y, interpolateStepVal), Lerp5(transform.position.z, received.z, receivedSec.z, receivedThird.z, receivedFourth.z, interpolateStepVal));
                    transform.position = result;

                    tempTimeCount += Time.fixedDeltaTime;
                }
            }
            else if (!useLerping)
            {
                base.FixedUpdate();
            }

        }


        public override void AddMessage(Packet message)
        {
            //Debug.Log("transform received");
            lock (lockReceive)
            {
                if (scriptEnabled)
                {

                    Packet packet = message;
                    int key = packet.GetPackageIndex();

                    if (!ContainsKey(key))
                    {

                        Vector3 receivedMessage1 = Vector3.zero;


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

                        Vector3 receivedMessageBuff = Vector3.zero;
                        Vector3 receivedMessageBuff2 = Vector3.zero;
                        Vector3 receivedMessageBuff3 = Vector3.zero;
                        Vector3 receivedMessageBuff4 = Vector3.zero;
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

                        if (receivedMessage1 != Vector3.zero)
                        {
                            Message receivedMessage = new Message(key, packet.GetSenderId(), receivedMessage1);
                            AddObject(receivedMessage, key);
                        }


                        if (messages.Count >= 3 && receivedMessageBuff != Vector3.zero)
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

                        if (messages.Count >= 4 && receivedMessageBuff2 != Vector3.zero)
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

                        if (messages.Count >= 5 && receivedMessageBuff3 != Vector3.zero)
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

                        if (messages.Count >= 6 && receivedMessageBuff4 != Vector3.zero)
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

                    Vector3 received = (Vector3)message.messageObject;

                    Vector3 result = new Vector3(received.x, received.y, received.z);
                    m_onReceive.Invoke(result);
                    m_onReceiveText.Invoke(result.ToString());

                    transform.position = result;

                    //packageIndex = message.GetPacketId();


                }
            }

        }

        float Lerp3(float a, float b, float c, float t)
        {
            if (t <= 0.5f)
            {
                return Mathf.Lerp(a, b, t);
            }
            else
            {
                return Mathf.Lerp(b, c, t);
            }
        }

        float Lerp4(float a, float b, float c, float d, float t)
        {
            if (t <= 0.33f)
            {
                return Mathf.Lerp(a, b, t);
            }
            else if (t > 0.33 && t <= 0.66)
            {
                return Mathf.Lerp(b, c, t);
            }
            else
            {
                return Mathf.Lerp(c, d, t);
            }
        }

        float Lerp5(float a, float b, float c, float d, float e, float t)
        {
            if (t <= 0.25f)
            {
                return Mathf.Lerp(a, b, t);
            }
            else if (t > 0.25 && t <= 0.5)
            {
                return Mathf.Lerp(b, c, t);
            }
            else if (t > 0.5 && t <= 0.75)
            {
                return Mathf.Lerp(c, d, t);
            }
            else
            {
                return Mathf.Lerp(d, e, t);
            }
        }

        public void DebugReceived(object value)
        {
            Debug.Log(value.ToString());
        }

    }
}