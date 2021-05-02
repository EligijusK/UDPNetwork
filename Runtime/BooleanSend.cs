using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
namespace UDPNetwork
{
    public class BooleanSend : SendBase
    {
        // Start is called before the first frame update
        [SerializeField]
        private int messageCount = 200;
        private bool value = false;

        //// Start is called before the first frame update
        new void Start()
        {
            base.Start();
        }

        public void SendBoolean(bool value)
        {

            if (scriptEnabled)
            {
                try
                {
                    this.value = value;
                    if (sendManualy && UDPConnectionManager.ConectedToServer())
                    {
                        SendMessage();
                    }

                    //Debug.Log(msg);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }

        }

        public void SendMultipleMessages(bool value)
        {
            if (scriptEnabled && UDPConnectionManager.ConectedToServer())
            {
                this.value = value;
                Thread thread = new Thread(() => SendMessage());
                thread.Name = "Object State";
                thread.Start();
            }
        }

        public void MultipleMessages()
        {
            for (int i = 0; i < messageCount; i++)
            {
                SendMessage();
            }
        }

        public override void SendMessage()
        {
            if (scriptEnabled)
            {

                packet.ResetPacket();

                if (frame != null && bufferFrame != null && bufferFrame2 != null && bufferFrame3 != null)
                {
                    bufferFrame4 = bufferFrame3;
                    bufferFrame3 = bufferFrame2;
                    bufferFrame2 = bufferFrame;
                    bufferFrame = frame;
                    frame = this.value;
                    bool frameFloat = this.value;
                    bool buffFloat = (bool)bufferFrame;
                    bool buffFloat2 = (bool)bufferFrame2;
                    bool buffFloat3 = (bool)bufferFrame3;
                    bool buffFloat4 = (bool)bufferFrame4;

                    packet.AddBool(frameFloat);

                    packet.AddBool(buffFloat);

                    packet.AddBool(buffFloat2);

                    packet.AddBool(buffFloat3);

                    packet.AddBool(buffFloat4);

                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);

                }
                else if (frame != null && bufferFrame != null && bufferFrame2 != null)
                {
                    bufferFrame3 = bufferFrame2;
                    bufferFrame2 = bufferFrame;
                    bufferFrame = frame;
                    frame = this.value;
                    bool frameFloat = this.value;
                    bool buffFloat = (bool)bufferFrame;
                    bool buffFloat2 = (bool)bufferFrame2;
                    bool buffFloat3 = (bool)bufferFrame3;

                    packet.AddBool(frameFloat);

                    packet.AddBool(buffFloat);

                    packet.AddBool(buffFloat2);

                    packet.AddBool(buffFloat3);

                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);

                }
                else if (frame != null && bufferFrame != null)
                {
                    bufferFrame2 = bufferFrame;
                    bufferFrame = frame;
                    frame = this.value;
                    bool frameFloat = this.value;
                    bool buffFloat = (bool)bufferFrame;
                    bool buffFloat2 = (bool)bufferFrame2;

                    packet.AddBool(frameFloat);

                    packet.AddBool(buffFloat);

                    packet.AddBool(buffFloat2);


                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);

                }
                else if (frame != null && bufferFrame == null)
                {

                    bufferFrame = frame;
                    frame = this.value;
                    bool frameFloat = this.value;
                    bool buffFloat = (bool)bufferFrame;

                    packet.AddBool(frameFloat);

                    packet.AddBool(buffFloat);

                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);

                }
                else if (frame == null)
                {
                    frame = this.value;
                    bool frameFloat = this.value;

                    packet.AddBool(frameFloat);


                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}