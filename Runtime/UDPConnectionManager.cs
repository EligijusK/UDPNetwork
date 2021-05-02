using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace UDPNetwork
{
    public class UDPConnectionManager : MonoBehaviour, ObjectStateHandler
    {

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            ServerIsFull,
            ServerTimeout
        }

        public static UDPConnectionManager singleton; // re do index don't use singelton

        private static ConnectionState serverConnectionState = ConnectionState.Disconnected;

        [SerializeField]
        private float timeIntervalFoEveryAtempt = 1.5f;
        [SerializeField]
        private float connectionTimeOut = 10f;
        [SerializeField]
        private UDPPlayer mainPlayer;
        [SerializeField]
        private string serverIp = "141.136.44.126";
        [SerializeField]
        private bool connectOnStart = true;
        private static Socket socket;
        private IPAddress send_to_address;
        private IPEndPoint sending_end_point;
        private EndPoint epFrom;
        private AsyncCallback recv = null;
        private int id = 0;
        private State state = new State();
        private static bool messageIsSend = false;
        private float timeIsPassed = 0; // padaryti kitaip
        private float timePassedTrying = 0; // padaryti kitaip

        private static SortedDictionary<string, ReceiveBase> messageHandler = new SortedDictionary<string, ReceiveBase>();
        private static StringComparer stringComparer = StringComparer.InvariantCulture;
        private static SortedList prefixList = new SortedList(stringComparer);
        private static int maxPlayers = 10;
        private ServerPacket packetForServer;
        private int bytesLen = 0;
        private static bool isAlive = true;
        private static object objectLoadSceneLock = new object();
        private int timer = -1;
        private int spawnId = -1;
        private Thread connectionThread;
        private bool connectionThreadJoin = false;
        public static bool gameStarted { get; private set; }
        public bool isDisconnected = true;
        [FormerlySerializedAs("gameStarts")]
        [SerializeField]
        private UnityEvent<bool> m_gameStarts = new UnityEvent<bool>();

        [FormerlySerializedAs("connectedToServer")]
        [SerializeField]
        private UnityEvent<bool> m_connectedToServer = new UnityEvent<bool>();

        [FormerlySerializedAs("disconnectedFromServer")]
        [SerializeField]
        private UnityEvent m_disconnectedFromServer = new UnityEvent();

        private ObjectState objectState;
        public class State
        {
            public static int BufSize = 1500;
            public static byte[] receive_buffer = new byte[BufSize];
            public StringBuilder sb = new StringBuilder();

        }

        public void Setup()
        {
            isAlive = true;
            objectLoadSceneLock = new object();
            messageHandler = new SortedDictionary<string, ReceiveBase>();
            if (prefixList == null)
            {
                prefixList = new SortedList(stringComparer);
            }
            maxPlayers = 10;
            messageIsSend = false;
            gameStarted = false;
            timer = -1;
            id = 0;
            state = new State();
            timeIsPassed = 0;
            timePassedTrying = 0;
            bytesLen = 0;
            spawnId = -1;
            isDisconnected = true;
            connectionThreadJoin = false;

        }
        private void OnEnable()
        {

            if (singleton == null)
            {
                Setup();
                singleton = this;
                if (connectOnStart)
                {
                    UserConnectToServer();
                }
                

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (objectState == null)
            {
                objectState = new ObjectState(new List<object>() { isAlive, objectLoadSceneLock, new SortedDictionary<string, ReceiveBase>(messageHandler), new SortedList(prefixList), maxPlayers, messageIsSend, gameStarted, timer, id, new State(), timeIsPassed, timePassedTrying, bytesLen, spawnId, serverIp, isDisconnected, connectionThreadJoin });
            }
            else
            {
                objectState.AddVariables(new List<object>() { isAlive, objectLoadSceneLock, new SortedDictionary<string, ReceiveBase>(messageHandler), new SortedList(prefixList), maxPlayers, messageIsSend, gameStarted, timer, id, new State(), timeIsPassed, timePassedTrying, bytesLen, spawnId, serverIp, isDisconnected, connectionThreadJoin });
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (ConnectionState.Connected == serverConnectionState)
            {
                socket.BeginReceiveFrom(State.receive_buffer, 0, State.BufSize, SocketFlags.None, ref epFrom, recv, state);
            }
            else if (ConnectionState.Connecting == serverConnectionState)
            {
                socket.BeginReceiveFrom(State.receive_buffer, 0, State.BufSize, SocketFlags.None, ref epFrom, recv, state);
                timeIsPassed += Time.deltaTime;
                //Debug.LogError("in update: " + recv.Method.Name);
            }
            if (ConnectionState.Connected == serverConnectionState && !connectionThreadJoin)
            {
                connectionThread.Join();
                connectionThreadJoin = true;
            }


        }

        public void SetTime(uint time)
        {
            this.timer = (int)time;
        }

        public int Passedtime()
        {
            return (int)this.timer;
        }

        public bool DoorsUnlocked()
        {
            if (timer > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetIpAddress(string ip)
        {

            serverIp = ip;
        }

        public void UserConnectToServer()
        {

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            send_to_address = IPAddress.Parse(singleton.serverIp); // server ip: 141.136.44.126
            sending_end_point = new IPEndPoint(singleton.send_to_address, 5002);
            epFrom = new IPEndPoint(singleton.send_to_address, 5002);
            recv = new AsyncCallback(ReceiveCallbackConnection);
            packetForServer = new ServerPacket();
            packetForServer.AddStartEvent(singleton.m_gameStarts);
            serverConnectionState = ConnectionState.Connecting;

            AttemtToConnect();
        }


        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {

                //State.receive_buffer = new byte[State.BufSize];

                socket.BeginReceiveFrom(State.receive_buffer, 0, State.BufSize, SocketFlags.None, ref epFrom, recv, state);


                //string receivedMessage = Encoding.ASCII.GetString(State.receive_buffer, 0, bytesLen);
                //Debug.Log(receivedMessage);

                int currentIndex = 0;

                bool[] receivedMessageBits = StaicBitFunctions.Byte2Bool(State.receive_buffer);

                int bitLenType = (int)StaicBitFunctions.CountBits((int)Packet.PacketType.NumberOfValues);

                bool[] packetLenBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, bitLenType);
                int packetType = (int)StaicBitFunctions.BitArrayToUInt(packetLenBits);

                //PrintValues(receivedMessage, 0);
                packetLenBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 7);
                uint res = StaicBitFunctions.BitArrayToUInt(packetLenBits); // package lenght for data check

                bytesLen = socket.EndReceiveFrom(ar, ref epFrom);

                if (bytesLen > 0 && res == bytesLen)
                {
                    if (packetType == (int)Packet.PacketType.User)
                    {

                        PacketHeader receivedMessage = new PacketHeader(State.receive_buffer);
                        string prefix = receivedMessage.GetPrefix();
                        int playerId = receivedMessage.GetSenderId();

                        //Debug.Log(prefix);
                        if (messageHandler.ContainsKey(prefix))
                        {
                            //Thread thread;
                            //thread = new Thread(() => UDPPManager.AddMessage(prefix, playerId.ToString(), receivedMessage));
                            //thread.Start();
                            //Debug.LogError("it should work");
                            ThreadManager.AddOneTimeAction(delegate { UDPPlayerManager.AddMessage(prefix, playerId.ToString(), receivedMessage); });

                        }
                    }
                    else
                    {
                        int commandLen = (int)StaicBitFunctions.CountBits((int)ServerPacket.ServerCommand.ConnectedOther); // last element of enum
                        bool[] packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, commandLen);
                        res = StaicBitFunctions.BitArrayToUInt(packetInBits); // command num

                        if ((int)ServerPacket.ServerCommand.ConnectOther == (int)res) // new command for server other connections
                        {
                            int bitLen = (int)StaicBitFunctions.CountBits(GetMaxPlayerCount());
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, bitLen);
                            uint lenRes = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            int playerCountConnected = (int)lenRes;
                            Debug.Log("connected players: " + playerCountConnected);
                            if (playerCountConnected > 0)
                            {
                                int len = (int)StaicBitFunctions.CountBits(maxPlayers);
                                for (int length = 0; length < playerCountConnected; length++) // 0 cant exist as player index
                                {

                                    packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, len);
                                    res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                                    int custId = (int)res;
                                    //Debug.Log("id :? " + custId);
                                    if (UDPPlayerManager.ContainsIdInConnected(custId.ToString()))
                                    {
                                        byte[] send_buffer = packetForServer.CommandPacket(ServerPacket.ServerCommand.ConnectedOther);
                                        socket.Send(send_buffer);
                                        break;
                                    }

                                    //UDPPManager.ActivatePlayer(custId.ToString());
                                    MainThreadManager.ExecuteOnMainThread(delegate
                                    {
                                        UDPPlayerManager.ActivatePlayer(custId.ToString());
                                    });

                                    Debug.Log("connection other id:" + custId);
                                }

                            }
                        }
                        if ((int)ServerPacket.ServerCommand.DisconnectedFromOther == (int)res)
                        {
                            int bitLen = (int)StaicBitFunctions.CountBits(GetMaxPlayerCount());
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, bitLen);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            int custId = (int)res;

                            Debug.Log("Disconnected id: " + custId);
                            byte[] send_buffer = packetForServer.CommandPacket(ServerPacket.ServerCommand.DisconnectedFromOther);
                            socket.Send(send_buffer);

                            MainThreadManager.ExecuteOnMainThread(delegate
                            {
                                UDPPlayerManager.DeactivatePlayer(custId.ToString());
                            });

                        }
                        if ((int)ServerPacket.ServerCommand.Start == (int)res && !gameStarted) // new command for server other connections
                        {
                            lock (objectLoadSceneLock)
                            {
                                packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 1);
                                packetForServer.AddData(packetInBits[0]);
                                packetForServer.Command(ServerPacket.ServerCommand.Start);
                                gameStarted = packetInBits[0];

                            }

                        }
                        if ((int)ServerPacket.ServerCommand.Timer == (int)res) // new command for server other connections
                        {

                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 12);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            packetForServer.AddData(res);
                            packetForServer.Command(ServerPacket.ServerCommand.Timer);

                        }
                    }
                }
                else
                {
                    //Debug.Log("result in packet len: " + res + " received packet: " + bytesLen);
                    //Debug.Log("received packet len: " + bytesLen);
                    //Debug.LogError("Packet Dropped"); // harmeless error
                    //Packet.PrintValues(receivedMessageBits, 0);
                }


            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public ConnectionState ServerState()
        {
            return serverConnectionState;
        }

        void ReceiveCallbackConnection(IAsyncResult ar)
        {
            try
            {


                socket.BeginReceiveFrom(State.receive_buffer, 0, State.BufSize, SocketFlags.None, ref epFrom, recv, state);
                //Debug.LogError("Attemp Connect function");
                //string receivedMessage = Encoding.ASCII.GetString(State.receive_buffer, 0, bytesLen);
                //Debug.Log(receivedMessage);

                bool[] receivedMessageBits = StaicBitFunctions.Byte2Bool(State.receive_buffer);
                //PrintValues(receivedMessage, 0);
                int currentIndex = 0;

                int bitLenType = (int)StaicBitFunctions.CountBits((int)Packet.PacketType.NumberOfValues);

                bool[] packetLenBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, bitLenType);
                int packetType = (int)StaicBitFunctions.BitArrayToUInt(packetLenBits);

                bool[] packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 7);
                uint res = StaicBitFunctions.BitArrayToUInt(packetInBits); // package lenght for data check

                //Debug.Log(res);

                bytesLen = socket.EndReceiveFrom(ar, ref epFrom);


                if (bytesLen > 0 && res == bytesLen)
                {
                    if (packetType == (int)Packet.PacketType.ServerCommand)
                    {


                        int commandLen = (int)StaicBitFunctions.CountBits((int)ServerPacket.ServerCommand.ConnectedOther); // last element of enum
                        packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, commandLen);
                        res = StaicBitFunctions.BitArrayToUInt(packetInBits); // command num

                        //Debug.Log("result: " + res);

                        if ((int)ServerPacket.ServerCommand.Connected == (int)res)
                        {
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 7);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            UDPConnectionManager.maxPlayers = (int)res;
                            //Debug.Log("max players: " + res);
                            int len = (int)StaicBitFunctions.CountBits((int)res);
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, len);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            int custId = (int)res;

                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 7);
                            len = (int)StaicBitFunctions.BitArrayToUInt(packetInBits);

                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, len);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits);

                            spawnId = (int)res;

                            //UDPPManager.ActivatePlayer(custId.ToString());

                            id = custId;
                            recv = new AsyncCallback(ReceiveCallback);
                            serverConnectionState = ConnectionState.Connected;

                            Debug.Log("connected");

                            MainThreadManager.ExecuteOnMainThread(delegate
                            {
                                mainPlayer.SetPlayerId(custId);
                                mainPlayer.ActivatePlayerReceivers();
                                mainPlayer.ActivateSender();
                                UDPPlayerManager.ActivatePlayer(custId.ToString());
                                m_connectedToServer.Invoke(true);
                            });
                            isDisconnected = false;
                            //Debug.Log("Should print");
                            //Debug.Log("connection id:" + custId);

                        }
                        else if ((int)ServerPacket.ServerCommand.Full == (int)res)
                        {
                            Debug.Log("Server Is full");
                            MainThreadManager.ExecuteOnMainThread(delegate
                            {
                                m_connectedToServer.Invoke(false);
                            });
                            serverConnectionState = ConnectionState.ServerIsFull;
                        }
                        else if ((int)ServerPacket.ServerCommand.ConnectOther == (int)res) // new command for server other connections
                        {
                            int len = (int)StaicBitFunctions.CountBits(UDPConnectionManager.maxPlayers);
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, len);
                            uint lenRes = StaicBitFunctions.BitArrayToUInt(packetInBits);
                            int playerCountConnected = (int)lenRes;
                            //Debug.Log("connected players: " + playerCountConnected);
                            if (playerCountConnected > 0)
                            {

                                for (int length = 0; length < playerCountConnected; length++) // 0 cant exist as player index
                                {

                                    packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, len);
                                    res = StaicBitFunctions.BitArrayToUInt(packetInBits);
                                    int custId = (int)res;

                                    //UDPPManager.ActivatePlayer(custId.ToString());
                                    MainThreadManager.ExecuteOnMainThread(delegate
                                    {
                                        UDPPlayerManager.ActivatePlayer(custId.ToString());
                                    });

                                    //Debug.Log("connect other id:" + custId);
                                }
                            }
                        }

                    }
                }
                else
                {
                    //Debug.LogError("Packet Dropped"); // harmeless error

                }

            }
            catch (Exception e)
            {
                //string str = e.ToString();
                MainThreadManager.ExecuteOnMainThread(delegate
                {
                    m_connectedToServer.Invoke(false);
                });
                serverConnectionState = ConnectionState.ServerTimeout;
                Debug.Log(e);
            }
        }

        void ReceiveCallbackDisconnect()
        {
            while (ConnectionState.Connected == serverConnectionState)
            {
                int byteCount = socket.ReceiveFrom(State.receive_buffer, State.BufSize, SocketFlags.None, ref epFrom);

                if (byteCount > 0)
                {
                    bool[] receivedMessageBits = StaicBitFunctions.Byte2Bool(State.receive_buffer);
                    //PrintValues(receivedMessage, 0);
                    int currentIndex = 0;

                    int bitLenType = (int)StaicBitFunctions.CountBits((int)Packet.PacketType.NumberOfValues);

                    bool[] packetLenBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, bitLenType);
                    int packetType = (int)StaicBitFunctions.BitArrayToUInt(packetLenBits);

                    bool[] packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, 7);
                    uint res = StaicBitFunctions.BitArrayToUInt(packetInBits); // package lenght for data check

                    if (res == byteCount)
                    {
                        if (packetType == (int)Packet.PacketType.ServerCommand)
                        {


                            int commandLen = (int)StaicBitFunctions.CountBits((int)ServerPacket.ServerCommand.ConnectedOther); // last element of enum
                            packetInBits = StaicBitFunctions.BitsReverseLen(receivedMessageBits, ref currentIndex, commandLen);
                            res = StaicBitFunctions.BitArrayToUInt(packetInBits); // command num

                            //Debug.Log("result: " + res);

                            if ((int)ServerPacket.ServerCommand.Disconnected == (int)res && serverConnectionState != ConnectionState.Disconnected && !isDisconnected)
                            {

                                socket.Close();
                                Debug.Log("Disconnected");
                                MainThreadManager.ExecuteOnMainThread(delegate
                                {
                                    m_disconnectedFromServer.Invoke();
                                });

                                serverConnectionState = ConnectionState.Disconnected;
                                isDisconnected = true;
                            }


                        }
                    }
                }

            }
        }


        public static void AddMssageToOther(string key, Packet message)
        {
            messageHandler[key].AddMessage(message);
        }

        public static void AddMessageToMainPlayer(string key, Packet message)
        {
            singleton.mainPlayer.AddMessage(key, message);
        }

        public UDPPlayer GetMainPlayer()
        {
            return mainPlayer;
        }

        public static bool ContainsPrefix(string prefix)
        {
            return messageHandler.ContainsKey(prefix);
        }



        public static void SendMessage(byte[] message)
        {

            if (ConnectionState.Connected == serverConnectionState)
            {
                socket.Send(message);
            }

        }

        public int GetSpawnId()
        {
            return spawnId;
        }

        public void ConnectToServer()
        {
            try
            {
                while (ConnectionState.Connecting == serverConnectionState)
                {

                    if (!messageIsSend)
                    {


                        byte[] send_buffer = packetForServer.CommandPacket(ServerPacket.ServerCommand.Connect);
                        socket.Send(send_buffer);
                        messageIsSend = true;

                    }
                    else if (timeIsPassed > timeIntervalFoEveryAtempt)
                    {

                        timeIsPassed = 0;
                        messageIsSend = false;
                        timePassedTrying++;

                    }
                    if (timePassedTrying > connectionTimeOut)
                    {
                        MainThreadManager.ExecuteOnMainThread(delegate
                        {
                            m_connectedToServer.Invoke(false);
                        });
                        serverConnectionState = ConnectionState.ServerTimeout;
                    }


                }
            }
            catch (Exception ex)
            {
                Debug.LogError("connection to server exeption: " + ex.ToString());
            }

        }

        void DisconnectFromServer(Thread thread)
        {

            double timeIsPassed = 0;
            bool disconnectMessageIsSend = false;
            float timeIntervalFoEveryAtempt = this.timeIntervalFoEveryAtempt;
            ServerPacket packetForServer = new ServerPacket();
            DateTime currentDateTime = DateTime.Now;
            bool serverTimeOut = false;
            timePassedTrying = 0;

            while (ConnectionState.Connected == serverConnectionState && !serverTimeOut)
            {

                timeIsPassed = (DateTime.Now - currentDateTime).TotalSeconds;
                if (!disconnectMessageIsSend)
                {
                    byte[] send_buffer = packetForServer.CommandPacket(ServerPacket.ServerCommand.Disconnect);
                    socket.Send(send_buffer);
                    disconnectMessageIsSend = true;

                }
                if (timeIsPassed > timeIntervalFoEveryAtempt)
                {
                    currentDateTime = DateTime.Now;
                    disconnectMessageIsSend = false;
                    timePassedTrying++;
                }
                if (timePassedTrying > connectionTimeOut)
                {
                    serverConnectionState = ConnectionState.Disconnected;
                    serverTimeOut = true;

                }


            }
            if (ConnectionState.Disconnected == serverConnectionState)
            {
                thread.Abort();
                socket.Close();
            }

        }

        public void AttemtToDisconnect()
        {

            try
            {
                Thread thread = new Thread(() => ReceiveCallbackDisconnect());
                thread.Start();
                DisconnectFromServer(thread);
            }
            catch (Exception send_exception)
            {

                Console.WriteLine(" Exception {0}", send_exception.Message);

            }

        }

        public void AttemtToConnect()
        {
            if (ConnectionState.Connecting == serverConnectionState)
            {
                timeIsPassed = 0;
                timePassedTrying = 0;


                try
                {
                    socket.Connect(sending_end_point);
                    connectionThread = new Thread(() => ConnectToServer());
                    connectionThread.Start();

                }
                catch (Exception send_exception)
                {

                    Console.WriteLine(" Exception {0}", send_exception.Message);

                }

            }

        }


        public static void AddToMessageHandler(string prefix, ReceiveBase receiverBase)
        {
            try
            {
                messageHandler.Add(prefix, receiverBase);
            }
            catch (ArgumentException)
            {
                //Debug.LogError("An prefix = '" + prefix + "' already exists.");
            }

        }

        public static void AddPrefix(string prefix)
        {
            try
            {
                prefixList.Add(prefix, prefix);
                Packet.SetPRefixBitLen(prefixList.Count);
            }
            catch (Exception ex)
            {
                //Debug.Log("Prefix already exists: " + prefix);
            }
        }

        public static bool CheckIfPrefixExist(string prefix)
        {
            return prefixList.ContainsKey(prefix);
        }


        public static int PrefixIndex(string prefix)
        {

            return prefixList.IndexOfKey(prefix);
        }

        public static string PrefixByIndex(int index)
        {
            return (string)prefixList.GetByIndex(index);
        }


        public static int GetMaxPlayerCount()
        {
            return maxPlayers;
        }

        public int GetPlayerId()
        {
            return id;
        }

        public static bool ConectedToServer()
        {
            if (ConnectionState.Connected == serverConnectionState)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsAlive()
        {
            return isAlive;
        }

        public void SetPlayerAlive(bool playerIsAlive)
        {
            isAlive = playerIsAlive;
        }

        void OnApplicationQuit()
        {
            AttemtToDisconnect();
            Debug.Log("App Quit");
        }

        public void SetState(ObjectState state)
        {
            this.objectState = state;
        }

        public void RestoreToState()
        {
            if (objectState != null && objectState.StateIsSaved())
            {
                List<object> variables = objectState.GetVariables();

                SortedDictionary<string, ReceiveBase> messageHandlerCopy = new SortedDictionary<string, ReceiveBase>((SortedDictionary<string, ReceiveBase>)variables[2]);
                SortedList prefixListCopy = new SortedList((SortedList)variables[3]);

                State.receive_buffer = new byte[State.BufSize];

                isAlive = (bool)variables[0];
                objectLoadSceneLock = variables[1];
                messageHandler = messageHandlerCopy; //
                prefixList = prefixListCopy; //
                maxPlayers = (int)variables[4];
                messageIsSend = (bool)variables[5];
                gameStarted = (bool)variables[6];
                this.timer = (int)variables[7];
                this.id = (int)variables[8];
                this.state = new State(); // make copy (State)variables[9];
                this.timeIsPassed = (float)variables[10];
                this.timePassedTrying = (float)variables[11];
                this.bytesLen = (int)variables[12];
                this.spawnId = (int)variables[13];
                this.serverIp = (string)variables[14];
                this.isDisconnected = (bool)variables[15];
                this.connectionThreadJoin = (bool)variables[16];

            }
        }

        public object DeepCopy(object copying)
        {
            throw new NotImplementedException();
        }
    }
}