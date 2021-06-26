using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawnerScript : MonoBehaviour
{
    public GameObject platform;
    public bool gameOver;
    Vector3 lastPos; //qui registro la posizione dell'ultima piattaforma inserita
    float size;
    int direction; //per sapere direzione attuale della piattaforma (la piattaforma può cambiare direzione)
    private int counterUp; //Serve a indicare il numero di piattaforme da creare
    public static PlatformSpawnerScript current; //per interfecciare con altri script, in moda da richiamarla

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
    }


    // Start is called before the first frame update
    void Start()
    {
        gameOver = true;
        
        direction = 1; //up

        lastPos = new Vector3(platform.transform.position.x, platform.transform.position.y,
            platform.transform.position.z -0.2f); //devo aggiungere - 0.2f dopo poition.z
        //usare i bound quando ho degli oggetti legati tra loro. prendo le misure del bound, ovvero l'oggetto più grande 
        //dell'insieme di oggetti

        Bounds momSize = GetMaxBounds(platform); //caricato il valore più grande. però ora voglio solo la profondità z per
                                                //capire dove mettere la piattaforma
        
        size = momSize.size.z; //prendo solo l'asse z di momSize perchè è l'unica dimensione che mi serve
        
        counterUp = 5; //numero piattaforme iniziale
        
        InvokeRepeating("SpawnInitialVertical", 0.1f, 0.1f); //comanda che lancia una void ripetutamente



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnInitialVertical()
    {

        Vector3 pos = lastPos;
        pos.z += size;

        lastPos = new Vector3(pos.x, pos.y, pos.z -0.2f); //devo agg -0.2f dopo pos.z

        GameObject newObject = GroundVerticalPoolerScript.current.GetPooledObject();
        //con il codice sopra creiamo una nuovo gameobject, lo chiamiamo newobject e richiamo la funzione
        //che mi ritorna un newobject
        //inoltre la variabile current è quella che ci permette di agganciarci allo script groundvertical..
        //ora però devo gestire il caso in cui non mi ritorna niente, il caso null

        if (newObject == null) return;
        //il return serve ad uscire nel caso null

        newObject.transform.position =
            pos; //come posizione all'oggetto creato gli diamo pos. pos è l'ultima posizione +size
        //dunque metto l'oggetto nell'ultima posizione quando finisce la piattaforma
        newObject.transform.rotation = Quaternion.identity; //rimane la stessa rotazione
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato

        //essendo che tale void viene richiamata ogni volta bisogna controllare il conteggio
        if (--counterUp <= 0) {
            CancelInvoke("SpawnInitialVertical"); 
            //con questo comando decremento counterup di 1. se quello che rimane è minore uguale a zero usciamo dal cancelinvoke,
            //che cancella l'invocazione, ossia la ripetizione
                                                            
            
    }
}

    Bounds GetMaxBounds(GameObject g) //è una void con il nome della variabile (bounds) perchè ritorna un risultato quando la chiamo
    {
        var b = new Bounds(g.transform.position, Vector3.zero); //viene creata una variabile con la posizone di platform
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>()) //per ogni componente children dell'oggetto che processo,prendo l'oggetto più grande
        {
            b.Encapsulate(r.bounds);
        }

        return b;

    }
}
