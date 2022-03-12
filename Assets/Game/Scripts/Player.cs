using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;

    public bool controllable;

    public string matchID;
    [SyncVar] public int playerIndex; 

    public NetworkMatch networkMatch;
    public MovementJoyStick movementJoyStick = null;
    public ShootJoystick shootJoystick;
    public float playerSpeed;
    public float rotationSpeed;

    public GameObject Cam;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float bulletForce = 5f;
    public float firingDelay = 0f;
    public Rigidbody2D rb;

    private bool onGameScene = false;
    public Scene gameScene; 

    [SyncVar]
    public int health = 4;
    // Start is called before the first frame update
    void Start()
    {   
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(isLocalPlayer){
            localPlayer = this;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            
        }
        if(controllable){
            onGameScene = true;
           
            movementJoyStick = GameObject.Find("MovementLobby").GetComponent<MovementJoyStick>();
            shootJoystick = GameObject.Find("ShootLobby").GetComponent<ShootJoystick>();
            if(isLocalPlayer){
                Cam = GameObject.FindGameObjectWithTag("MainCamera");
                if(!Cam.GetComponent<camFollow>()){
                    Cam.AddComponent<camFollow>();
                    Cam.GetComponent<camFollow>().target = transform;
                }
                
            }
            
        }
    
        networkMatch = GetComponent<NetworkMatch> ();
        firePoint = transform.GetChild(0);
        bulletPrefab = (GameObject)Resources.Load("Bullet");
        
         rb = GetComponent<Rigidbody2D>();
        bulletForce = 5f;
        
        
        
    }


    void Update(){
        if(isLocalPlayer && controllable){
            if(movementJoyStick.joystickVec.y != 0){
                transform.Translate(new Vector3(movementJoyStick.joystickVec.x * playerSpeed * Time.deltaTime, movementJoyStick.joystickVec.y * playerSpeed * Time.deltaTime, 0), Space.World);
            }
            
            if(shootJoystick.joystickVec.y != 0){
                    var angle = Mathf.Rad2Deg * Mathf.Atan2(shootJoystick.joystickVec.y,shootJoystick.joystickVec.x);
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
                    if(firingDelay < 0f){
                        CmdShoot();
                        firingDelay = 1f;
                    }
                    firingDelay -= Time.deltaTime;
                    
         }

        }
       
    }



    [Command]
    void CmdShoot() {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().bulletParent = transform.GetComponent<NetworkIdentity>().netId;
        bullet.GetComponent<SpriteRenderer>().sortingLayerName = transform.GetComponent<SpriteRenderer>().sortingLayerName;
        NetworkServer.Spawn(bullet);
    }

    public void HostGame() {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID);
    }

    [Command]
    void CmdHostGame(string _matchID) {
        matchID = _matchID;
        if(MatchMaker.instance.HostGame(matchID, gameObject, out playerIndex)){
            Debug.Log("Game Hosted Successfully");
            networkMatch.matchId = matchID.ToGuid();
            matchID = _matchID;
            transform.GetComponent<SpriteRenderer>().enabled = true;
            // Player.localPlayer.controllable = true;
            // Player.localPlayer.rb = Player.localPlayer.GetComponent<Rigidbody2D>();
            // Player.localPlayer.movementJoyStick = GameObject.Find("MovementLobby").GetComponent<MovementJoyStick>();
            // Player.localPlayer.shootJoystick = GameObject.Find("ShootLobby").GetComponent<ShootJoystick>();
            // Player.localPlayer.firePoint = Player.localPlayer.transform.GetChild(0);
            // Player.localPlayer.bulletPrefab = (GameObject)Resources.Load("Bullet");
            // Player.localPlayer.Cam = GameObject.FindGameObjectWithTag("MainCamera");
            // Player.localPlayer.Cam.AddComponent<camFollow>();
            // Player.localPlayer.Cam.GetComponent<camFollow>().target = Player.localPlayer.transform;
            RpcHostGame();
            TargetHostGame(true, matchID);
        }else{
            Debug.Log("Game not hosted");
            TargetHostGame(false, matchID);
        }
    }

    [TargetRpc]
    public void TargetHostGame(bool success, string _matchID) {
        Debug.Log($"MatchID: {matchID}");
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        UILobby.instance.HostSuccess(success, _matchID);
        
    }
    [ClientRpc]
    public void RpcHostGame(){
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
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
            networkMatch.matchId = _matchID.ToGuid();
            RpcJoinGame();
            TargetJoinGame(true, _matchID);
        }else{
            Debug.Log("Game not joined");
            TargetJoinGame(false, _matchID);
        }
    }

    [TargetRpc]
    public void TargetJoinGame(bool success, string _matchID) {
        Debug.Log($"MatchID: {matchID}");
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        UILobby.instance.JoinSuccess(success, _matchID);
    }

    [ClientRpc]
    public void RpcJoinGame(){
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
    

    public void BeginGame(Scene _gameScene) {
        gameScene = _gameScene;
        GameObject.Find("LobbyCamera").SetActive(false);
        GameObject.Find("LobbyEventSystem").SetActive(false);
        CmdBeginGame();
    }

    [Command]
    public void CmdBeginGame() {
        MatchMaker.instance.BeginGame(matchID);
    }

    [ClientRpc]
    public void RpcBeginGame() {
        if(GameObject.Find("LobbyCamera")){
            GameObject.Find("LobbyCamera").SetActive(false);
        }
        if(GameObject.Find("LobbyEventSystem")){
            GameObject.Find("LobbyEventSystem").SetActive(false);
        }
        if(GameObject.Find("LobbyCanvas")){
            GameObject.Find("LobbyCanvas").SetActive(false);
        }
    }

    [TargetRpc]
    public async void TargetBeginGame(){
        Debug.Log(gameScene.name);
        
        SceneManager.LoadScene(2 ,LoadSceneMode.Additive);
        

        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.name == "Game"){
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            SceneManager.SetActiveScene(scene);
            Player.localPlayer.movementJoyStick = GameObject.Find("Movement").GetComponent<MovementJoyStick>();
            Player.localPlayer.shootJoystick = GameObject.Find("Shoot").GetComponent<ShootJoystick>();
            if(!Player.localPlayer.Cam.GetComponent<camFollow>()){
                Player.localPlayer.Cam = GameObject.FindGameObjectWithTag("MainCamera");
                Player.localPlayer.Cam.AddComponent<camFollow>();
                Player.localPlayer.Cam.GetComponent<camFollow>().target = Player.localPlayer.transform;
            }
            Player.localPlayer.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            Player.localPlayer.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        }
       
    }

    public void StartGame () {
        RpcBeginGame();
        
        TargetBeginGame();
       
    }




}
