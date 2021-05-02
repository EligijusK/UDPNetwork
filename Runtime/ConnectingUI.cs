using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{

    public class ConnectingUI : MonoBehaviour
    {
        [SerializeField]
        private UDPConnectionManager connectionManager;
        public UDPConnectionManager[] foundManagers;
        [FormerlySerializedAs("connectedToServer")]
        [SerializeField]
        private UnityEvent<bool> m_connectedToServer = new UnityEvent<bool>();
        // Start is called before the first frame update
        void Start()
        {
            if (connectionManager == null)
            {
                UDPConnectionManager[] connectionManagers = FindObjectsOfType<UDPConnectionManager>();
                foundManagers = connectionManagers;
                connectionManager = connectionManagers[0].gameObject.GetComponent<UDPConnectionManager>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (connectionManager.ServerState() != UDPConnectionManager.ConnectionState.Disconnected && connectionManager.ServerState() != UDPConnectionManager.ConnectionState.Connecting && connectionManager.ServerState() != UDPConnectionManager.ConnectionState.Connected)
            {
                m_connectedToServer.Invoke(false);
            }
            else if (connectionManager.ServerState() == UDPConnectionManager.ConnectionState.Connected)
            {
                m_connectedToServer.Invoke(true);
            }

        }

        public void ConnectToServer()
        {
            connectionManager.UserConnectToServer();
        }
    }
}