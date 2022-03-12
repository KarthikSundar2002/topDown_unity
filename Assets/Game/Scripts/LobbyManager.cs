using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager localLobbyManager;

    public NetworkManager myNetworkManager;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex; 
    // Start is called before the first frame update
    void Start()
    {
        localLobbyManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void HostGame() {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID) {
        matchID = _matchID;
        Debug.Log("Hi from CMDHostGame");
        if(MatchMaker.instance.HostGame(matchID, gameObject, out playerIndex)){
            Debug.Log("Game Hosted Successfully");
            Debug.Log(GameObject.Find("NetworkManager"));
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().OnServerAddPlayer(NetworkClient.connection);
            Player.localPlayer.networkMatch.matchId = matchID.ToGuid();
            Player.localPlayer.TargetHostGame(true, matchID);
        }else{
            Debug.Log("Game not hosted");
            Player.localPlayer.TargetHostGame(false, matchID);
        }
    }

    public void JoinGame(string _inputID) {
        string matchID = _inputID;
        CmdJoinGame(matchID);
    }

    [Command]
    void CmdJoinGame(string _matchID) {
        matchID = _matchID;
        if(MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex)){
            Debug.Log("Game Joined Successfully");
            Debug.Log(GameObject.Find("NetworkManager"));
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().OnServerAddPlayer(NetworkClient.connection);
            Player.localPlayer.networkMatch.matchId = _matchID.ToGuid();
           
            Player.localPlayer.TargetJoinGame(true, _matchID);
        }else{
            Debug.Log("Game not joined");
            Player.localPlayer.TargetJoinGame(false, _matchID);
        }
    }

}
