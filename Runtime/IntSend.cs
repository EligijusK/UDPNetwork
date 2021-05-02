using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
namespace UDPNetwork
{
    public class IntSend : SendBase
    {

        //private HubConnection connection;
        private int value = 0;
        private int mutipleCount = 300;
        // Start is called before the first frame update
        new void Start()
        {

            base.Start();
        }


        public void SendMultiple(int value)
        {
            Thread mutipleInts = new Thread(() => SendMutipleThread(value));
            mutipleInts.Name = "multipleInts";
            mutipleInts.Start();
        }

        private void SendMutipleThread(int value)
        {
            for (int i = 0; i < 10; i++)
            {
                SendInt(value);
            }
        }

        public void SendInt(int value)
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
                int frameInt = this.value;
                int buffFInt = (int)bufferFrame;
                int buffInt2 = (int)bufferFrame2;
                int buffInt3 = (int)bufferFrame3;
                int buffInt4 = (int)bufferFrame4;

                packet.AddInt(frameInt);

                packet.AddInt(buffFInt);

                packet.AddInt(buffInt2);

                packet.AddInt(buffInt3);

                packet.AddInt(buffInt4);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame != null && bufferFrame2 != null)
            {
                bufferFrame3 = bufferFrame2;
                bufferFrame2 = bufferFrame;
                bufferFrame = frame;
                frame = this.value;
                int frameInt = this.value;
                int buffFInt = (int)bufferFrame;
                int buffInt2 = (int)bufferFrame2;
                int buffInt3 = (int)bufferFrame3;

                packet.AddInt(frameInt);

                packet.AddInt(buffFInt);

                packet.AddInt(buffInt2);

                packet.AddInt(buffInt3);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame != null)
            {
                bufferFrame2 = bufferFrame;
                bufferFrame = frame;
                frame = this.value;
                int frameInt = this.value;
                int buffFInt = (int)bufferFrame;
                int buffInt2 = (int)bufferFrame2;

                packet.AddInt(frameInt);

                packet.AddInt(buffFInt);

                packet.AddInt(buffInt2);


                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame != null && bufferFrame == null)
            {

                bufferFrame = frame;
                frame = this.value;
                int frameInt = this.value;
                int buffFInt = (int)bufferFrame;

                packet.AddInt(frameInt);

                packet.AddInt(buffFInt);

                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);

            }
            else if (frame == null)
            {
                frame = this.value;
                int frameFloat = this.value;

                packet.AddInt(frameFloat);


                message = packet.CreatePacket();
                UDPConnectionManager.SendMessage(message);
            }

        }
    }
}