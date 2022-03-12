using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UILobby : MonoBehaviour
{

    public static UILobby instance;
    [SerializeField] InputField JoinMatchInput;
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] Canvas lobbyCanvas;

    [SerializeField] Text matchIdText;

    [SerializeField] GameObject beginGameButton;

    [SerializeField] Scene gameScene;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Host(){
        JoinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.HostGame();
    }

    public void HostSuccess (bool success, string _matchID) {
        if ( success) {
            Debug.Log(success);
            lobbyCanvas.enabled = true;
            Player.localPlayer.matchID = _matchID;
            Player.localPlayer.controllable = true;
            Player.localPlayer.rb = Player.localPlayer.GetComponent<Rigidbody2D>();
            Player.localPlayer.movementJoyStick = GameObject.Find("MovementLobby").GetComponent<MovementJoyStick>();
            Player.localPlayer.shootJoystick = GameObject.Find("ShootLobby").GetComponent<ShootJoystick>();
            Player.localPlayer.firePoint = Player.localPlayer.transform.GetChild(0);
            Player.localPlayer.bulletPrefab = (GameObject)Resources.Load("Bullet");
            if(!Player.localPlayer.Cam){
                Player.localPlayer.Cam = GameObject.FindGameObjectWithTag("MainCamera");
                Player.localPlayer.Cam.AddComponent<camFollow>();
                Player.localPlayer.Cam.GetComponent<camFollow>().target = Player.localPlayer.transform;
            }
            Player.localPlayer.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Connect").SetActive(false);
            matchIdText.text = Player.localPlayer.matchID;
            beginGameButton.SetActive(true);
        } else {
            JoinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void Join(){
        JoinMatchInput.interactable = false;
        joinButton.interactable = false;
        hostButton.interactable = false;

        Player.localPlayer.JoinGame(JoinMatchInput.text);

    }

    public void JoinSuccess (bool success, string _matchID) {
        if ( success) {
            Debug.Log(success);
            Player.localPlayer.matchID = _matchID;
            lobbyCanvas.enabled = true;
            Player.localPlayer.controllable = true;
            Player.localPlayer.rb = Player.localPlayer.GetComponent<Rigidbody2D>();
            Player.localPlayer.movementJoyStick = GameObject.Find("MovementLobby").GetComponent<MovementJoyStick>();
            Player.localPlayer.shootJoystick = GameObject.Find("ShootLobby").GetComponent<ShootJoystick>();
            Player.localPlayer.firePoint = Player.localPlayer.transform.GetChild(0);
            Player.localPlayer.bulletPrefab = (GameObject)Resources.Load("Bullet");
            if(!Player.localPlayer.Cam){
                Player.localPlayer.Cam = GameObject.FindGameObjectWithTag("MainCamera");
                Player.localPlayer.Cam.AddComponent<camFollow>();
                Player.localPlayer.Cam.GetComponent<camFollow>().target = Player.localPlayer.transform;
            }
            Player.localPlayer.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Connect").SetActive(false);
            matchIdText.text = Player.localPlayer.matchID;
            beginGameButton.SetActive(true);
        } else {
            JoinMatchInput.interactable = true;
            joinButton.interactable = true;
            hostButton.interactable = true;
        }
    }

    public void BeginGame() {
        Player.localPlayer.BeginGame(gameScene);
           
    }
}
