using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AutoConnect : MonoBehaviour
{
    [SerializeField]
    NetworkManager networkManager;
    [SerializeField]
    string networkAddress;

    public void joinLocal() {
        networkManager.networkAddress = networkAddress;
        networkManager.StartClient();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(!Application.isBatchMode ){
            joinLocal();
        }else{
            networkManager.StartServer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
