using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{
    public class TransformSend : SendBase
    {

        private Vector3 prevPosition = Vector3.zero;
        private Vector3 position = Vector3.zero;
        // Start is called before the first frame update
        new void Start()
        {

            base.Start();
        }


        protected void Update()
        {
            position = transform.position;
        }

        public override void SendMessage()
        {
            if (scriptEnabled)
            {
                //Debug.Log("transformSend");
                if (prevPosition != position && packet != null)
                {

                    packet.ResetPacket();


                    if (frame != null && bufferFrame != null && bufferFrame2 != null)
                    {
                        bufferFrame3 = bufferFrame2;
                        bufferFrame2 = bufferFrame;
                        bufferFrame = frame;
                        frame = position;
                        Vector3 positionTemp = position;
                        Vector3 buffPosition = (Vector3)bufferFrame;
                        Vector3 buffPosition2 = (Vector3)bufferFrame2;
                        Vector3 buffPosition3 = (Vector3)bufferFrame3;

                        packet.AddFloat(positionTemp.x);
                        packet.AddFloat(positionTemp.y);
                        packet.AddFloat(positionTemp.z);

                        packet.AddFloat(buffPosition.x);
                        packet.AddFloat(buffPosition.y);
                        packet.AddFloat(buffPosition.z);

                        packet.AddFloat(buffPosition2.x);
                        packet.AddFloat(buffPosition2.y);
                        packet.AddFloat(buffPosition2.z);

                        packet.AddFloat(buffPosition3.x);
                        packet.AddFloat(buffPosition3.y);
                        packet.AddFloat(buffPosition3.z);

                        message = packet.CreatePacket();
                        UDPConnectionManager.SendMessage(message);

                    }
                    else if (frame != null && bufferFrame != null)
                    {
                        bufferFrame2 = bufferFrame;
                        bufferFrame = frame;
                        frame = position;
                        Vector3 positionTemp = position;
                        Vector3 buffPosition = (Vector3)bufferFrame;
                        Vector3 buffPosition2 = (Vector3)bufferFrame2;

                        packet.AddFloat(positionTemp.x);
                        packet.AddFloat(positionTemp.y);
                        packet.AddFloat(positionTemp.z);

                        packet.AddFloat(buffPosition.x);
                        packet.AddFloat(buffPosition.y);
                        packet.AddFloat(buffPosition.z);

                        packet.AddFloat(buffPosition2.x);
                        packet.AddFloat(buffPosition2.y);
                        packet.AddFloat(buffPosition2.z);

                        message = packet.CreatePacket();
                        UDPConnectionManager.SendMessage(message);

                    }
                    else if (frame != null && bufferFrame == null)
                    {
                        bufferFrame = frame;
                        frame = position;
                        Vector3 positionTemp = position;
                        Vector3 buffPosition = (Vector3)bufferFrame;

                        packet.AddFloat(positionTemp.x);
                        packet.AddFloat(positionTemp.y);
                        packet.AddFloat(positionTemp.z);

                        packet.AddFloat(buffPosition.x);
                        packet.AddFloat(buffPosition.y);
                        packet.AddFloat(buffPosition.z);

                        message = packet.CreatePacket();
                        UDPConnectionManager.SendMessage(message);

                    }
                    else if (frame == null)
                    {
                        frame = position;
                        Vector3 positionTemp = position;

                        packet.AddFloat(positionTemp.x);
                        packet.AddFloat(positionTemp.y);
                        packet.AddFloat(positionTemp.z);

                        message = packet.CreatePacket();
                        UDPConnectionManager.SendMessage(message);
                    }

                    prevPosition = position;

                }
            }
        }
    }
}