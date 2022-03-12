using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine;
using Mirror;

[System.Serializable]
public class Match {
    
    public string matchID;
    public List<GameObject> players = new List<GameObject>();

    public Match(string matchID, GameObject player){
        this.matchID = matchID;
        players.Add(player);
    }

    public Match() {}
}

public static class MatchExtensions {
    public static Guid ToGuid(this string id) {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes (id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    } 
}

public class MatchMaker : NetworkBehaviour
{

    public static MatchMaker instance;

    public SyncList<Match> matches = new SyncList<Match>();
    public SyncList<string> matchIDs = new SyncList<string>();
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public bool JoinGame(string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;
        Debug.Log("Id in Matchmaker:" + _matchID);
        if(matchIDs.Contains(_matchID)){
            for(int i = 0; i < matches.Count; i++){
                if(matches[i].matchID == _matchID) {
                    matches[i].players.Add(_player);
                    _player.GetComponent<Player>().matchID = _matchID;
                    _player.GetComponent<SpriteRenderer>().enabled = true;
                    playerIndex = matches[i].players.Count - 1;
                    break;
                }
            }
            Debug.Log("Match Joined");
            return true;
        } else {
            Debug.Log("Match ID does not exists");
            return false;
        }
    }

    public bool HostGame(string _matchID, GameObject _player, out int playerIndex) {
        playerIndex = -1;
        if(!matchIDs.Contains(_matchID)){
            matchIDs.Add(_matchID);
            _player.GetComponent<SpriteRenderer>().enabled = true;
            _player.GetComponent<Player>().matchID = _matchID;
            matches.Add(new Match(_matchID, _player));
            Debug.Log("Match Made");
            playerIndex = 0;
            return true;
        } else {
            Debug.Log("Match ID already exists");
            return false;
        }
    }
    public static string GetRandomMatchID() {
        Debug.Log("hi from Matchmaker");
        string _id = string.Empty;
        for(int i = 0; i < 5; i++){
            int random = UnityEngine.Random.Range(0, 36);
            if(random < 26){
                _id += (char)(random + 65);
            }else{
                _id += (random - 26).ToString();
            }
        }
        Debug.Log(_id);
        return _id;
    }

    public void BeginGame(string _matchID) {
        for(int i = 0; i < matches.Count; i++){
            if(matches[i].matchID == _matchID){
                foreach(var player in matches[i].players){
                    player.GetComponent<SpriteRenderer>().enabled = true;
                    Player _player = player.GetComponent<Player>();
                    _player.StartGame();
                }
                break;
            }
        }
    }

    

}
