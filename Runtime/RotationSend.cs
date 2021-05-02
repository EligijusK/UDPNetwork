using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
namespace UDPNetwork
{
    public class RotationSend : SendBase
    {

        private Vector3 prevRotation = Vector3.zero;
        private Vector3 rotation = Vector3.zero;
        // Start is called before the first frame update
        new void Start()
        {

            base.Start();
        }


        private void Update()
        {
            rotation = transform.rotation.eulerAngles;
        }

        public override void SendMessage()
        {
            if (prevRotation != rotation && Mathf.Abs(rotation.magnitude - prevRotation.magnitude) > 0.001)
            {

                packet.ResetPacket();



                if (frame != null && bufferFrame != null && bufferFrame2 != null)
                {
                    bufferFrame3 = bufferFrame2;
                    bufferFrame2 = bufferFrame;
                    bufferFrame = frame;
                    frame = rotation;
                    Vector3 position = rotation;
                    Vector3 buffPosition = (Vector3)bufferFrame;
                    Vector3 buffPosition2 = (Vector3)bufferFrame2;
                    Vector3 buffPosition3 = (Vector3)bufferFrame3;

                    packet.AddFloat(position.x);
                    packet.AddFloat(position.y);
                    packet.AddFloat(position.z);

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
                    frame = rotation;
                    Vector3 frameRotation = rotation;
                    Vector3 buffPosition = (Vector3)bufferFrame;
                    Vector3 buffPosition2 = (Vector3)bufferFrame2;

                    packet.AddFloat(frameRotation.x);
                    packet.AddFloat(frameRotation.y);
                    packet.AddFloat(frameRotation.z);

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
                    frame = rotation;

                    Vector3 frameRotation = rotation;
                    Vector3 buffPosition = (Vector3)bufferFrame;

                    packet.AddFloat(frameRotation.x);
                    packet.AddFloat(frameRotation.y);
                    packet.AddFloat(frameRotation.z);

                    packet.AddFloat(buffPosition.x);
                    packet.AddFloat(buffPosition.y);
                    packet.AddFloat(buffPosition.z);

                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);

                }
                else if (frame == null)
                {
                    frame = rotation;
                    Vector3 frameRotation = rotation;

                    packet.AddFloat(frameRotation.x);
                    packet.AddFloat(frameRotation.y);
                    packet.AddFloat(frameRotation.z);


                    message = packet.CreatePacket();
                    UDPConnectionManager.SendMessage(message);
                }
                prevRotation = rotation;
            }

        }
    }
}