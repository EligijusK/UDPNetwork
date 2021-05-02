using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
namespace UDPNetwork
{
    public abstract class ReceiveBase : MonoBehaviour, ObjectStateHandler
    {


        public class Message : IComparable, IComparable<Message>
        {

            public int index { get; private set; }

            public int senderId { get; private set; }

            public object messageObject { get; private set; }

            public Message(int index, int sender, object message)
            {
                this.index = index;
                this.senderId = sender;
                this.messageObject = message;
            }

            public override bool Equals(object obj)
            {
                //Check for null and compare run-time types.
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    Message p = (Message)obj;
                    return (index == p.index) && (messageObject.Equals(p.messageObject));
                }
            }

            public override int GetHashCode()
            {
                return (index << 2) ^ 2;
            }



            public static bool operator <(Message left, Message right)
            {
                return (Compare(left, right) < 0);
            }
            public static bool operator >(Message left, Message right)
            {
                return (Compare(left, right) > 0);
            }

            public static int Compare(Message left, Message right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return 0;
                }
                if (left is null)
                {
                    return -1;
                }
                return left.CompareTo(right);
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                {
                    return 1;
                }

                Message other = obj as Message; // avoid double casting
                if (other == null)
                {
                    throw new ArgumentException("A RatingInformation object is required for comparison.", "obj");
                }

                return CompareTo(other);
            }

            public int CompareTo(Message other)
            {
                if (other is null)
                {
                    return 1;
                }

                if (other.index > this.index)
                {
                    return 1;
                }
                else if (other.index < this.index)
                {
                    return -1;
                }

                return 0;

            }

        }


        [SerializeField]
        //[Range(0, 199)]
        protected string prefix = "";
        [SerializeField]
        protected bool gameObjectSpecificPrefix = false;
        [SerializeField]
        protected int minFrameBuffered = 20;
        [SerializeField]
        protected bool manualReceive = false;
        protected SortedList messages { get; private set; }
        //protected List<Message> messages {  get; private set; }
        protected static byte[] wrong;
        protected byte[] wrongRepresentation;
        protected int packageIndex = 0;
        protected bool scriptEnabled = false;
        protected ObjectState state;
        private bool oneTime = true;
        // Start is called before the first frame update

        protected void Awake()
        {
            if (oneTime)
            {

                messages = new SortedList();
                scriptEnabled = true;
                packageIndex = 0;
                if (gameObjectSpecificPrefix)
                {
                    Vector3 position = gameObject.transform.position;
                    Vector3 rotation = gameObject.transform.rotation.eulerAngles;
                    Vector3 scale = gameObject.transform.lossyScale;

                    Vector3 index = position + scale + rotation;

                    prefix = prefix + gameObject.name + (gameObject.transform.childCount + index.magnitude).ToString();
                }

                if (!UDPConnectionManager.CheckIfPrefixExist(prefix))
                {
                    UDPConnectionManager.AddPrefix(prefix);
                }
                UDPConnectionManager.AddToMessageHandler(prefix, this);
                oneTime = false;
            }
        }

        protected void Start()
        {
            //Debug.Log("Handler added");
            if (state == null)
            {
                state = new ObjectState(new List<object>() { new SortedList(), scriptEnabled, prefix, packageIndex });
            }
            else
            {
                state.AddVariables(new List<object>() { new SortedList(), scriptEnabled, prefix, packageIndex });
            }

        }



        public string GetPrefix()
        {
            return prefix;
        }

        protected void FixedUpdate()
        {
            if (!manualReceive)
            {
                if (messages.Count > minFrameBuffered)
                {
                    MessageReceived();
                }
                wrongRepresentation = wrong;
            }
        }

        protected void Update()
        {
            //if (messages.Count > minFrameBuffered)
            //{
            //    MessageReceived();
            //}
        }

        protected void AddObject(Message message, int index)
        {

            messages.Add(index, message);

        }

        protected bool ContainsKey(int index)
        {
            return messages.ContainsKey(index);
        }

        protected void ReplaceObject(int index, Message message)
        {
            messages[index] = message;
        }

        protected object GetMessageFromBuffer()
        {
            if (messages != null && messages.Count > 0)
            {
                object message = messages.GetByIndex(0);
                messages.RemoveAt(0); // PADARYTI ANALIZE KOKS LISTO ILGIS IR KAIP VEIKIA JO REMOVES

                return message;
            }
            else
            {
                return null;
            }
        }

        protected object GetMessageFromBufferNewest()
        {
            if (messages != null && messages.Count > 0)
            {
                object message = messages.GetByIndex(messages.Count - 1);

                return message;
            }
            else
            {
                return null;
            }
        }


        protected bool Compare(int index, object obj)
        {
            if (index < messages.Count)
            {
                if (messages.GetByIndex(index).Equals(obj))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public static void WrongPacket(byte[] wrongPacket)
        {
            wrong = wrongPacket;
        }

        protected abstract void MessageReceived();

        public abstract void AddMessage(Packet message);

        void OnDisable()
        {
            scriptEnabled = false;
        }

        void OnEnable()
        {
            scriptEnabled = true;
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
                SortedList coping = (SortedList)variables[0];
                SortedList copy = new SortedList(coping);
                messages = copy;
                scriptEnabled = (bool)variables[1];
                prefix = (string)variables[2];
                packageIndex = (int)variables[3];
                if (!UDPConnectionManager.CheckIfPrefixExist(prefix))
                {
                    UDPConnectionManager.AddPrefix(prefix);
                }
                UDPConnectionManager.AddToMessageHandler(prefix, this);
            }
        }

        public object DeepCopy(object copying)
        {
            throw new NotImplementedException();
        }

        public void ClearReceivedMessages()
        {
            if (messages != null)
            {
                messages.Clear();
            }
        }


    }
}