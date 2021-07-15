using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoteriManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private BulletManager bullet;

    [SerializeField] private Material invisibleMaterial;
    private Material defaultMaterial;
    private bool isInvisible;
    public float timeInvisible = 1f;

    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material bilocazioneMaterial;
    public float timeBilocazione = 4f;

    public bool canBullet = true;
    public bool canInvisibile = true;
    public bool canBilocazione = true;

    public int passiBullet = 50;
    public int passiBilocazione = 100;
    public int maxInvisibilita = 3;

    public static PoteriManager current; //per interfecciare con altri script, in moda da richiamarla

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
        defaultMaterial = player.GetComponent<Renderer>().material; //Salvo il materiale principale come default
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
            isInvisible = true;
            maxInvisibilita--;
            player.GetComponent<Renderer>().material = invisibleMaterial;
            Invoke("TurnNormal", timeInvisible);

            if (maxInvisibilita == 0)
                canInvisibile = false;
        }
    }

    public void TurnNormal()
    {
        isInvisible = false;
        player.GetComponent<Renderer>().material = defaultMaterial;
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }

    //Bilocazione
    public void StartBilocazione()
    {
        //var all = GameObject.FindGameObjectsWithTag("terrain");
        //foreach(var g in all)
        //{
        //    g.GetComponent<Renderer>().material = bilocazioneMaterial;
        //}
        //Invoke("EndBilocazione", 5f);

        if (canBilocazione && !BilocazioneManager.current.bilocazione)
        {
            canBilocazione = false;
            BilocazioneManager.current.StartBilocazione();
            Invoke("EndBilocazione", timeBilocazione);
        }
    }

    public void EndBilocazione()
    {
        //var all = GameObject.FindGameObjectsWithTag("terrain");
        //foreach (var g in all)
        //{
        //    g.GetComponent<Renderer>().material = normalMaterial;
        //}

        BilocazioneManager.current.EndBilocazione();
    }
}
