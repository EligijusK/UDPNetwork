using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
namespace UDPNetwork
{
    public class UDPPlayerManager : MonoBehaviour, ObjectStateHandler
    {

        private static List<UDPPlayer> freePlayerManagers = new List<UDPPlayer>();
        private static Dictionary<string, UDPPlayer> playerById = new Dictionary<string, UDPPlayer>();
        private static List<string> playersID = new List<string>();
        private static bool playerCreated = false;
        private static GameObject playerPrefabRef;
        private static GameObject refGameObject;
        [SerializeField]
        private GameObject playerPrefab;
        private ObjectState state;
        private bool oneTime = true;
        private void Setup()
        {
            playerPrefabRef = playerPrefab;
            playerById = new Dictionary<string, UDPPlayer>();
            freePlayerManagers = new List<UDPPlayer>();
            playersID = new List<string>();
            playerCreated = false;

        }

        // Start is called before the first frame update
        void Awake()
        {
            if (oneTime)
            {
                Setup();
                oneTime = false;
            }
            //freePlayerManagers.Add(this); change this
        }

        void Start()
        {
            if (state == null)
            {
                state = new ObjectState(new List<object>() { playerPrefabRef, playerById, freePlayerManagers, playersID, playerCreated });
            }
            else
            {
                state.AddVariables(new List<object>() { playerPrefabRef, playerById, freePlayerManagers, playersID, playerCreated });
            }
        }

        void Update()
        {
            if (refGameObject == null)
            {
                refGameObject = this.gameObject;
            }
        }

        public static void ActivatePlayer(string id)
        {

            if (!playerCreated)
            {
                CreateMaxPlayer(UDPConnectionManager.GetMaxPlayerCount());
                playerCreated = true;
            }

            if (freePlayerManagers.Count > 0 && !playersID.Contains(id))
            {
                Debug.Log("Activated user: " + id);
                UDPPlayer player = freePlayerManagers[0];
                int playerId = int.Parse(id);
                player.SetPlayerId(playerId);
                player.ActivatePlayerReceivers();
                player.ActivateSender();
                playerById.Add(id, player);
                freePlayerManagers.Remove(player);
                playersID.Add(id);
                player.gameObject.transform.position = UDPConnectionManager.singleton.transform.position;
                player.gameObject.SetActive(true);

            }

        }



        public static void DeactivatePlayer(string id)
        {


            if (playerById.ContainsKey(id))
            {
                UDPPlayer player = playerById[id];
                player.SetPlayerId(0);
                freePlayerManagers.Add(player);
                playerById.Remove(id);
                playersID.Remove(id.ToString());
                player.gameObject.SetActive(false);
            }


        }

        public static bool ContainsIdInConnected(string id)
        {
            return playersID.Contains(id);
        }

        public static bool ContainsKeyInMessageHandler(string prefix, string playerId)
        {

            if (playerById.ContainsKey(playerId) && playerById[playerId].ContainsKeyInMessageHandler(prefix))
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        public static void AddMessage(string prefix, string playerId, PacketHeader receivedMessage)
        {

            if (playerById.ContainsKey(playerId) && playerById[playerId].ContainsKeyInMessageHandler(prefix))
            {
                //Debug.Log("sender id: " + playerId);
                Packet packet = new Packet(receivedMessage);
                if (playerId == UDPConnectionManager.singleton.GetPlayerId().ToString() && UDPConnectionManager.PlayerIsAlive())
                {
                    playerById[playerId].AddMessage(prefix, packet);
                    if (playerById[playerId].ConstainsKeyInBothHandler(prefix) && UDPConnectionManager.ContainsPrefix(prefix))
                    {
                        Packet secPacket = new Packet(receivedMessage);
                        UDPConnectionManager.AddMessageToMainPlayer(prefix, secPacket);
                    }
                }
                else if (playerId != UDPConnectionManager.singleton.GetPlayerId().ToString())
                {
                    if (playerById[playerId].PlayerIsAlive())
                    {
                        playerById[playerId].AddMessage(prefix, packet);
                    }
                }

            }
            else
            {
                //Debug.Log("NonPlayerMessage");
                Packet packet = new Packet(receivedMessage);
                UDPConnectionManager.AddMssageToOther(prefix, packet);
            }

        }

        private static void CreateMaxPlayer(int maxPlayer)
        {
            for (int i = 0; i < maxPlayer; i++)
            {
                GameObject go = Instantiate(playerPrefabRef);
                go.transform.parent = refGameObject.transform;
                UDPPlayer player = go.GetComponent<UDPPlayer>();
                freePlayerManagers.Add(player);
                go.SetActive(false);
            }
        }

        public static GameObject CurrentOnlinePlayer()
        {
            string playerId = UDPConnectionManager.singleton.GetPlayerId().ToString();
            if (playerById.ContainsKey(playerId))
            {
                return playerById[playerId].gameObject;
            }
            else
                return null;
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

                if (playerById != null && playerById.Count > 0)
                {
                    for (int i = 0; i < playerById.Keys.Count; i++)
                    {
                        if (playerById != null && playerById.Values.ElementAt(i) != null && playerById.Values.ElementAt(i).gameObject != null)
                        {
                            Destroy(playerById.Values.ElementAt(i).gameObject);
                        }
                    }

                }

                if (freePlayerManagers != null && freePlayerManagers.Count > 0)
                {
                    for (int i = 0; i < freePlayerManagers.Count; i++)
                    {
                        if (freePlayerManagers != null && freePlayerManagers[i] != null && freePlayerManagers[i].gameObject != null)
                        {
                            Destroy(freePlayerManagers[i].gameObject);
                        }
                    }
                }

                Dictionary<string, UDPPlayer> playerByIdCopy = new Dictionary<string, UDPPlayer>((Dictionary<string, UDPPlayer>)variables[1]);
                List<UDPPlayer> freePlayerManagersCopy = new List<UDPPlayer>((List<UDPPlayer>)variables[2]);
                List<string> playersIDCopy = new List<string>((List<string>)variables[3]);
                playerPrefabRef = (GameObject)variables[0];
                playerById = playerByIdCopy; //
                freePlayerManagers = freePlayerManagersCopy; //
                playersID = playersIDCopy; //
                playerCreated = (bool)variables[4];
            }
        }

        public object DeepCopy(object copying)
        {
            throw new System.NotImplementedException();
        }
    }
}