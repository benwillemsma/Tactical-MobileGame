#if ENABLE_UNET

namespace UnityEngine.Networking
{
    using UI;

    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class NetworkManagerHUD : MonoBehaviour
    {
        public static NetworkManager manager;

        public Text ipText;
        public Text nameText;
        public int matchIndex;

        // Runtime variable
        bool showServer = false;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                StartHost();
            }

            //manager.networkAddress = ipText.text;
        }

        // LAN Connections
        public void StartServer()
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
                manager.StartServer();
        }
        public void StartHost()
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
                manager.StartHost();
        }
        public void StartClient()
        {
            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
                manager.StartClient();
        }

        public void StopServer()
        {
            manager.StopServer();
        }
        public void StopHost()
        {
            manager.StopHost();
        }
        public void StopClient()
        {
            manager.StopClient();
        }

        // MatchMaking
        public void StartMatchMaking()
        {
            if(manager.matchMaker == null)
                manager.StartMatchMaker();
        }
        public void StopMatchMaking()
        {
            if (manager.matchMaker != null)
                manager.StopMatchMaker();
        }

        public void CreateMatch()
        {
            if (!NetworkServer.active && !NetworkClient.active)
            {
                if (manager.matchMaker != null)
                {
                    manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 1, 1, manager.OnMatchCreate);
                    //manager.matchName = nameText.text;
                }
            }
        }
        public void ListMatches()
        {
            if (!NetworkServer.active && !NetworkClient.active)
            {
                if (manager.matchMaker != null)
                    manager.matchMaker.ListMatches(0, 20, "", true, 1, 1, manager.OnMatchList);
            }
        }
        public void JoinMatch()
        {
            if (!NetworkServer.active && !NetworkClient.active)
            {
                if (manager.matchMaker != null && manager.matches != null && manager.matches.Count > 0)
                {
                    manager.matchName = manager.matches[matchIndex].name;
                    manager.matchSize = (uint)manager.matches[matchIndex].currentSize;
                    manager.matchMaker.JoinMatch(manager.matches[matchIndex].networkId, "", "", "", 1, 1, manager.OnMatchJoined);
                }
            }
        }
    }
};
#endif //ENABLE_UNET
