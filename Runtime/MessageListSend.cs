using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UDPNetwork
{

    public class MessageListSend : SendBase
    {
        [SerializeField]
        private SendBase[] baseObjects;
        private static List<SendBase> interactivePlayerObjects = new List<SendBase>();
        private static int index = 0;
        private static int lastElement = -1;

        private void Setup()
        {
            interactivePlayerObjects = new List<SendBase>();
            index = 0;
            lastElement = -1;
        }


        new void Start()
        {
            Setup();
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void SendMessage()
        {
            int loopLength = Mathf.Max(interactivePlayerObjects.Count, baseObjects.Length);
            //Debug.Log(loopLength);
            for (int i = 0; i < loopLength; i++)
            {

                SendInteractivePlayerObjectMessage(i);
                SendPlayerObjectMessage(i);
            }
            index += 7;
        }

        private void SendInteractivePlayerObjectMessage(int i)
        {
            if (i < interactivePlayerObjects.Count && interactivePlayerObjects[i] != null)
            {
                interactivePlayerObjects[i].IncreaseSyncIndex(index);
                interactivePlayerObjects[i].SendMessage();
            }
        }

        private void SendPlayerObjectMessage(int i)
        {
            if (i < baseObjects.Length)
            {
                baseObjects[i].IncreaseSyncIndex(index);
                baseObjects[i].SendMessage();
            }
        }

        public static void AddSendBase(SendBase sendBase)
        {
            interactivePlayerObjects.Add(sendBase);
            lastElement++;
        }

        public static void InsertSendBase(SendBase sendBase, int insertIndex)
        {
            interactivePlayerObjects.Insert(insertIndex, sendBase);
            lastElement++;
        }

        public static void RemoveLastSendBase()
        {
            interactivePlayerObjects.RemoveAt(lastElement);
        }

        public static void RemoveSendBase(SendBase sendBase)
        {
            interactivePlayerObjects.Remove(sendBase);
        }

        public static void RemoveSendBaseAtIndex(int removeIndex)
        {
            interactivePlayerObjects.RemoveAt(removeIndex);
        }

    }
}