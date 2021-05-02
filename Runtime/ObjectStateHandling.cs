using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{
    public class ObjectStateHandling : MonoBehaviour
    {

        enum ObjectType
        {
            Item,
            Door
        }
        enum LocalState
        {
            Grabbed,
            Ungrabbed,
            RBTracking,
            Resting
        }
        [SerializeField]
        ObjectType type = ObjectType.Item;
        [SerializeField]
        private RotationReceive rotation;
        [SerializeField]
        private TransformReceive transformPosition;
        [SerializeField]
        private GrabReceive boolState;
        [SerializeField]
        private GameObject objectHandeling;
        [SerializeField]
        private TransformSend sendTransform;
        [SerializeField]
        private RotationSend sendRotation;
        [SerializeField]
        private BooleanSend sendTracking;
        [SerializeField]
        private Rigidbody rb;
        private LocalState localState = LocalState.Resting;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (LocalState.RBTracking == localState && rb.velocity.magnitude == 0)
            {
                localState = LocalState.Resting;
                rotation.ClearReceivedMessages();
                rotation.enabled = true;
                if (transformPosition != null)
                {
                    transformPosition.ClearReceivedMessages();
                    transformPosition.enabled = true;
                }

                if (sendTransform != null)
                {
                    MessageListSend.RemoveSendBase(sendTransform);
                }
                MessageListSend.RemoveSendBase(sendRotation);
                sendTracking.SendMultipleMessages(false);
            }
        }

        public void ReceiveState(bool state)
        {
            if (state)
            {

                if (type == ObjectType.Item)
                {
                    objectHandeling.SetActive(false);
                }
                else if (type == ObjectType.Door && UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(false);
                }
                else if (type == ObjectType.Door && !UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(false);
                }
                Tracking(state);
            }
            else
            {

                if (type == ObjectType.Item)
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && !UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(false);
                }

            }
        }

        public void Tracking(bool state)
        {
            if (state)
            {
                rotation.enabled = true;
                if (transformPosition != null)
                {
                    transformPosition.enabled = true;
                }
                rb.detectCollisions = true;
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            else
            {
                rotation.enabled = false;
                if (transformPosition != null)
                {
                    transformPosition.enabled = false;
                }
                rb.detectCollisions = true;
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        public void HandleLocalTracking(bool state)
        {
            if (state)
            {
                localState = LocalState.Grabbed;
                rotation.ClearReceivedMessages();
                rotation.enabled = false;
                if (transformPosition != null)
                {
                    transformPosition.ClearReceivedMessages();
                    transformPosition.enabled = false;
                }
                boolState.enabled = false;
                if (type == ObjectType.Item)
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && !UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(false);
                }
                if (sendTransform != null)
                {
                    MessageListSend.AddSendBase(sendTransform);
                }
                MessageListSend.AddSendBase(sendRotation);

            }
            else
            {
                localState = LocalState.Ungrabbed;

                boolState.enabled = true;
                if (type == ObjectType.Item)
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(true);
                }
                else if (type == ObjectType.Door && !UDPConnectionManager.singleton.DoorsUnlocked())
                {
                    objectHandeling.SetActive(false);
                }

                if (rb.velocity.magnitude == 0.0f)
                {
                    localState = LocalState.Resting;
                    rotation.ClearReceivedMessages();
                    rotation.enabled = true;
                    if (transformPosition != null)
                    {
                        transformPosition.ClearReceivedMessages();
                        transformPosition.enabled = true;
                    }

                    if (sendTransform != null)
                    {
                        MessageListSend.RemoveSendBase(sendTransform);
                    }
                    MessageListSend.RemoveSendBase(sendRotation);
                }
                else
                {
                    localState = LocalState.RBTracking;
                }
            }

        }

        public void DebugState(bool state)
        {
            Debug.Log(state);
        }
    }
}