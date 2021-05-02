using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;
namespace UDPNetwork
{
    public class UDPPlayer : MonoBehaviour, ObjectStateHandler
    {

        private int playerId = 0;
        [SerializeField]
        private List<ReceiveBase> listOfReceivers;
        [SerializeField]
        private List<ReceiveBase> listOfReceiversForBoth;
        [SerializeField]
        private List<SendBase> listOfSenders;
        private Dictionary<string, ReceiveBase> messageHandler = new Dictionary<string, ReceiveBase>();
        private Dictionary<string, ReceiveBase> messageHandlerForBoth = new Dictionary<string, ReceiveBase>();
        private bool reset = false;
        private bool isAlive = true;
        private ObjectState state;
        private bool oneTime = true;
        // Start is called before the first frame update
        void OnEnable()
        {
            if (oneTime)
            {
                messageHandler = new Dictionary<string, ReceiveBase>();
                messageHandlerForBoth = new Dictionary<string, ReceiveBase>();
                reset = false;
                isAlive = true;
                oneTime = false;
            }
        }

        void Start()
        {
            if (state == null)
            {
                state = new ObjectState(new List<object>() { new Dictionary<string, ReceiveBase>(messageHandler), new Dictionary<string, ReceiveBase>(messageHandlerForBoth), reset, isAlive });
            }
            else
            {
                state.AddVariables(new List<object>() { new Dictionary<string, ReceiveBase>(messageHandler), new Dictionary<string, ReceiveBase>(messageHandlerForBoth), reset, isAlive });
            }
        }

        public void SetPlayerId(int id)
        {
            playerId = id;
        }

        public int GetPlayerId()
        {
            return playerId;
        }

        public void ActivatePlayerReceivers()
        {
            if (listOfReceivers.Count > 0)
            {


                for (int i = 0; i < listOfReceivers.Count; i++)
                {
                    //string errorMessage = "An prefix = '" + listOfReceivers[i].GetPrefix() + "' already exists.";
                    string key = listOfReceivers[i].GetPrefix();
                    if (!messageHandler.ContainsKey(key))
                    {

                        messageHandler.Add(key, listOfReceivers[i]);

                    }
                    //else
                    //{
                    //    Debug.LogError(errorMessage);
                    //}
                }

                for (int i = 0; i < listOfReceiversForBoth.Count; i++)
                {
                    string key = listOfReceiversForBoth[i].GetPrefix();
                    if (!messageHandler.ContainsKey(key))
                    {
                        messageHandler.Add(key, listOfReceiversForBoth[i]);
                        messageHandlerForBoth.Add(key, listOfReceiversForBoth[i]);
                    }
                }

            }
            else
            {
                Debug.LogError("There are no receivers to activate");
            }
        }

        public void ActivateSender()
        {
            if (listOfSenders.Count > 0)
            {
                for (int i = 0; i < listOfSenders.Count; i++)
                {
                    listOfSenders[i].SetSenderId(playerId);
                }
            }
        }

        //public static void addMessage(Packet packet, int id)
        //{
        //    messageList.Add()
        //}

        public void RemoveFromMessageHandler(string key)
        {
            string errorMessage = "Listener Already removed";
            if (messageHandler.ContainsKey(key))
            {


                messageHandler.Remove(key);

            }
            else
            {
                Debug.LogError(errorMessage);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPlayerAlive(bool alive)
        {
            this.isAlive = alive;
        }

        public bool PlayerIsAlive()
        {
            return isAlive;
        }

        public void AddMessage(string key, Packet message)
        {

            string madeKey = key;
            messageHandler[madeKey].AddMessage(message);
        }

        public bool ContainsKeyInMessageHandler(string prefix)
        {

            return messageHandler.ContainsKey(prefix);
        }

        public bool ConstainsKeyInBothHandler(string prefix)
        {
            return messageHandlerForBoth.ContainsKey(prefix);
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

                Dictionary<string, ReceiveBase> copyMessageHandler = new Dictionary<string, ReceiveBase>((Dictionary<string, ReceiveBase>)variables[0]);
                Dictionary<string, ReceiveBase> copyMessageHandlerForBoth = new Dictionary<string, ReceiveBase>((Dictionary<string, ReceiveBase>)variables[1]);

                this.messageHandler = copyMessageHandler;
                this.messageHandlerForBoth = copyMessageHandlerForBoth;
                this.reset = (bool)variables[2];
                this.isAlive = (bool)variables[3];
            }
        }

        public object DeepCopy(object copying)
        {
            throw new NotImplementedException();
        }
    }
}