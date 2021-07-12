using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoteriManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private BulletManager bullet;

    [SerializeField]
    private Material invisibleMaterial;
    private Material defaultMaterial;
    public float timeInvisible = 1f;

    private bool canBullet = true;
    private bool canInvisibile = true;

    public static PoteriManager current; //per interfecciare con altri script, in moda da richiamarla

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
        defaultMaterial = player.GetComponent<Renderer>().material; //Salvo il materiale principale come default
    }

    //Ricaricare del potere
    public void ChargePower(bool variabile)
    {
        variabile = true;
    }

    //Pugno distruttore
    public void ShootBullet(GameObject player)
    {
        if (canBullet)
        {
            bullet.ShootBullet(player);
            canBullet = false;
        }
    }

    //Invisibilità
    public void SetDefaultMaterial(Material material)
    {
        defaultMaterial = material;
    }

    public void TurnInvisible()
    {
        if (canInvisibile)
        {
            player.GetComponent<Renderer>().material = invisibleMaterial;
            canInvisibile = false;
            Invoke("TurnNormal", timeInvisible);
        }
    }

    public void TurnNormal()
    {
        player.GetComponent<Renderer>().material = defaultMaterial;
    }
}
