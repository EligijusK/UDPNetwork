using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
namespace UDPNetwork
{
    public abstract class SendBase : MonoBehaviour, ObjectStateHandler
    {
        [SerializeField]
        protected string prefix = "";
        [SerializeField]
        protected bool gameObjectSpecificPrefix = false;
        [SerializeField]
        protected bool sendManualy = false;
        protected Packet packet;
        protected object frame;
        protected object bufferFrame;
        protected object bufferFrame2;
        protected object bufferFrame3;
        protected object bufferFrame4;
        private bool firstTimeInConnection = true;
        protected byte[] message;
        protected bool scriptEnabled = false;
        private int senderId = -1;
        protected ObjectState state;
        private bool oneTime = true;
        // Start is called before the first frame update

        protected void Start()
        {
            if (state == null)
            {
                state = new ObjectState(new List<object>() { prefix, scriptEnabled });
            }
            else
            {
                state.AddVariables(new List<object>() { prefix, scriptEnabled });
            }
            ThreadManager.AddAction(delegate { CustomUpdate(); });
        }

        protected void CustomUpdate()
        {

            if (!sendManualy && UDPConnectionManager.ConectedToServer() && !firstTimeInConnection)
            {
                packet.SetManualyIncresedIndex(false);
                SendMessage();
            }

        }

        // Update is called once per frame
        protected void FixedUpdate()
        {

            if (UDPConnectionManager.ConectedToServer() && firstTimeInConnection)
            {
                packet = new Packet(prefix);
                firstTimeInConnection = false;
                if (senderId != -1)
                {
                    packet.SetSenderId(senderId);
                }
            }

            //if (!sendManualy && UDPConnectionManager.ConectedToServer() && !firstTimeInConnection)
            //{
            //    SendMessage();
            //}
        }

        public void IncreaseSyncIndex(int index)
        {
            if (packet != null)
            {
                if (packet.IncresaseManulayIndexCheck())
                {
                    packet.SetIndex(index);
                }
                else
                {
                    packet.SetManualyIncresedIndex(true);
                    packet.SetIndex(index);
                }
            }
        }

        public abstract void SendMessage();

        void OnApplicationQuit()
        {
            //Debug.Log("App Quit");
        }

        void OnDisable()
        {
            scriptEnabled = false;
        }

        void OnEnable()
        {
            scriptEnabled = true;

            if (oneTime)
            {
                if (gameObjectSpecificPrefix)
                {
                    Vector3 position = gameObject.transform.position;
                    Vector3 rotation = gameObject.transform.rotation.eulerAngles;
                    Vector3 scale = gameObject.transform.lossyScale;

                    Vector3 index = position + scale + rotation;

                    prefix = prefix + gameObject.name + (gameObject.transform.childCount + index.magnitude).ToString();
                }
                UDPConnectionManager.AddPrefix(prefix);
                scriptEnabled = true;
                Debug.Log("Handler added");
                oneTime = false;
            }

        }

        public int CurrentSendIndex()
        {
            return packet.ReturnCurrentSendPacketIndex();
        }

        public void SetSenderId(int id)
        {
            if (packet != null)
            {
                packet.SetSenderId(id);
                senderId = id;
            }
            else
            {
                senderId = id;
            }
        }

        public void SetState(ObjectState state)
        {
            this.state = state;
        }

        public void RestoreToState()
        {
            if (state != null && state.StateIsSaved())
            {
                List<object> variables = state.GetVariables();
                this.prefix = (string)variables[0];
                this.scriptEnabled = (bool)variables[1];
            }
        }

        public object DeepCopy(object copying)
        {
            throw new NotImplementedException();
        }
    }
}