using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDPNetwork
{
    public class Disconnect : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> setupOnDisconnect = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RestoreStates()
        {
            for (int i = 0; i < setupOnDisconnect.Count; i++)
            {
                ObjectStateHandler[] parentHandlers = setupOnDisconnect[i].GetComponents<ObjectStateHandler>();
                ObjectStateHandler[] handlers = setupOnDisconnect[i].GetComponentsInChildren<ObjectStateHandler>();


                for (int a = 0; a < parentHandlers.Length; a++)
                {
                    parentHandlers[a].RestoreToState();
                }

                for (int a = 0; a < handlers.Length; a++)
                {
                    handlers[a].RestoreToState();
                }

            }
        }
    }
}