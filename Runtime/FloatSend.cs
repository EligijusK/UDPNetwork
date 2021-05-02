using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace UDPNetwork
{
    public class FloatSend : SendBase
    {

        //private HubConnection connection;
        private float value = 0f;
        private float mutipleCount = 300;
        // Start is called before the first frame update
        new void Start()
        {

            base.Start();
        }


        public void SendMultiple(float value)
        {
            Thread mutipleFloats = new Thread(() => SendMutipleThread(value));
            mutipleFloats.Name = "multipleFloats";
            mutipleFloats.Start();
        }

        private void SendMutipleThread(float value)
        {
            for (int i = 0; i < 10; i++)
            {
                SendFloat(value);
            }
        }

        public void SendFloat(float value)
        {
            if (this.enabled)
            {
                try
                {
                    this.value = value;

                    if (sendManualy && UDPConnectionManager.ConectedToServer())
                    {
                        SendMessage();
                    }

                    //await connection.InvokeAsync("Send", connection.GetHashCode().ToString(), msg);
                    //Debug.Log(msg);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void SendMessage()
        {

            packet.ResetPacket();

            if (frame != null && bufferFrame != null && bufferFrame2 != null && bufferFrame3 != null)
            {
                bufferFrame4 = bufferFrame3;
                bufferFrame3 = bufferFrame2;
                bufferFrame2 = bufferFrame;
                bufferFrame = frame;
                frame = this.value;
                float frameFloat = this.value;
                float buffFloat = (float)bufferFrame;
                float buffFloat2 = (float)bufferFrame2;
                float buffFloat3 = (float)bufferFrame3;
                float buffFloat4 = (float)bufferFrame4;

                packet.AddFloat(frameFloat);

                packet.AddFloat(buffFloat);

                packet.AddFloat(buffFloat2);

                packet.AddFloat(buffFloat3);

                packet.AddFloat(buffFloat4);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame != null && bufferFrame2 != null)
            {
                bufferFrame3 = bufferFrame2;
                bufferFrame2 = bufferFrame;
                bufferFrame = frame;
                frame = this.value;
                float frameFloat = this.value;
                float buffFloat = (float)bufferFrame;
                float buffFloat2 = (float)bufferFrame2;
                float buffFloat3 = (float)bufferFrame3;

                packet.AddFloat(frameFloat);

                packet.AddFloat(buffFloat);

                packet.AddFloat(buffFloat2);

                packet.AddFloat(buffFloat3);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame != null)
            {
                bufferFrame2 = bufferFrame;
                bufferFrame = frame;
                frame = this.value;
                float frameFloat = this.value;
                float buffFloat = (float)bufferFrame;
                float buffFloat2 = (float)bufferFrame2;

                packet.AddFloat(frameFloat);

                packet.AddFloat(buffFloat);

                packet.AddFloat(buffFloat2);


                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame == null)
            {

                bufferFrame = frame;
                frame = this.value;
                float frameFloat = this.value;
                float buffFloat = (float)bufferFrame;

                packet.AddFloat(frameFloat);

                packet.AddFloat(buffFloat);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame == null)
            {
                frame = this.value;
                float frameFloat = this.value;

                packet.AddFloat(frameFloat);


                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);
            }

        }
    }
}