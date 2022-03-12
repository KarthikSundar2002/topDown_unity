using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float destroyAfter = 4;
    public float bulletForce = 5f;

    public uint bulletParent; 
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    void Start()
    {
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        
    }

    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.GetComponent<Player>() != null && bulletParent != co.transform.GetComponent<NetworkIdentity>().netId)
        {
                --(co.transform.GetComponent<Player>().health);
                if (co.transform.GetComponent<Player>().health == 0){
                    NetworkServer.Destroy(co.gameObject);
                }
                NetworkServer.Destroy(gameObject);
                return;
        }
        if(!co.gameObject.CompareTag("Shootable") && !co.gameObject.CompareTag("Player")){
            NetworkServer.Destroy(gameObject);
        }
        
    }
}
