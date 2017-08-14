using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayerController : MonoBehaviour
{
    private NetworkLobbyPlayer player;

	void Start ()
    {
        player = GetComponent<NetworkLobbyPlayer>();
	}
	
	void Update () {
		
	}
}
