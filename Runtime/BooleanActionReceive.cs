using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class BooleanActionReceive : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<bool> m_onEnable = new UnityEvent<bool>();

        [FormerlySerializedAs("onReceive")]
        [SerializeField]
        private UnityEvent<bool> m_onDisable = new UnityEvent<bool>();

        public void Receive(bool state)
        {
            if (this.enabled)
            {
                if (state)
                {
                    m_onEnable.Invoke(true);
                }
                else
                {
                    m_onDisable.Invoke(false);
                }
            }
        }

    }
}