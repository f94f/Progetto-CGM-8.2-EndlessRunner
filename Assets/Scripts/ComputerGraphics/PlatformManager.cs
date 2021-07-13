using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager: MonoBehaviour
{
    [SerializeField]
    private GameObject plane, corner;

    private GameObject lastObject;
    private Quaternion rotation;
    private int direction; //per sapere direzione attuale della piattaforma (la piattaforma può cambiare direzione

    public GameObject platform,corner1,corner2,corner3,corner4;
    public bool gameOver;
    Vector3 lastPos; //qui registro la posizione dell'ultima piattaforma inserita
    float size,sizeCorner;
    private int counterUp,counterHor; //Serve a indicare il numero di piattaforme da creare
    //Questa float serve per dare un tempo un tempo di attesa per creare una nuova piattaforma
    float timeForCreation = 0.8f;

    public GameObject diamond;
    public static PlatformManager current; //per interfecciare con altri script, in moda da richiamarla

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
    }


    // Start is called before the first frame update
    void Start()
    {
        lastPos = plane.transform.position;
        lastObject = plane;
        rotation = Quaternion.identity;
        direction = 1; //asse Z

        //Bounds momSize = GetMaxBounds(platform); //caricato il valore più grande. però ora voglio solo la profondità z per
        //                                        //capire dove mettere la piattaforma

        //size = momSize.size.z; //prendo solo l'asse z di momSize perchè è l'unica dimensione che mi serve

        //counterUp = 20; //numero piattaforme iniziale

        //InvokeRepeating("SpawnInitialVertical", 0.1f, 0.1f); //comanda che lancia una void ripetutamente

        for (int i = 0; i < 40; i++)
        {
            CreatePLatform();
        }

        //CreateCorner();

        //for (int i = 0; i < 4; i++)
        //{
        //    CreatePLatform();
        //}

        //CreateCorner();

        //for (int i = 0; i < 4; i++)
        //{
        //    CreatePLatform();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatePLatform()
    {
        Bounds bounds = GetMaxBounds(lastObject);

        Vector3 newPos = lastPos;
        if (direction == 1) //foward
            newPos.z += (bounds.size.z);
        else if (direction == 2) //left
            newPos.x -= (bounds.size.x);
        else if (direction == 3) //rigth
            newPos.x += (bounds.size.x);
        else if (direction == 4) //back
            newPos.z -= (bounds.size.z);

        GameObject go = Instantiate(plane, newPos, rotation);
        lastPos = go.transform.position;
        lastObject = go;
    }

    private void CreateCorner()
    {
        Bounds bounds = GetMaxBounds(lastObject);
        Bounds boundsMe = GetMaxBounds(corner);
        Vector3 newPos = lastPos;
        if (direction == 1)
            newPos.z += (bounds.size.z / 2 + boundsMe.size.z / 2);
        else if (direction == 2)
            newPos.x -= (bounds.size.z / 2 + boundsMe.size.z / 2);
        else if (direction == 3)
            newPos.x += (bounds.size.z / 2 + boundsMe.size.z / 2);
        else if (direction == 4)
            newPos.z -= (bounds.size.z / 2 + boundsMe.size.z / 2);

        GameObject go = Instantiate(corner, newPos, rotation);
        lastPos = go.transform.position;

        rotation = Quaternion.Euler(0f, -90f, 0f);
        direction = 2; //asse X
        lastObject = go;

        bounds = GetMaxBounds(lastObject);
        Debug.Log(bounds.size);
        Vector3 newPos2 = lastPos;
        Debug.Log(newPos2);
        newPos2.x = newPos2.x - boundsMe.size.z - (boundsMe.size.z/2);

        GameObject go2 = Instantiate(plane, newPos2, rotation);
        lastPos = go2.transform.position;
        lastObject = go2;
    }

    void CreateDiamonds (Vector3 pos)
    {
        int rand = UnityEngine.Random.Range(0,4);
        if (rand < 1)
        {
            Instantiate(diamond, new Vector3(pos.x,pos.y + 1f ,pos.z),diamond.transform.rotation);
        }
    }

    void SpawnInitialVertical()
    {

        Vector3 pos = lastPos;
        pos.z += size;

        lastPos = new Vector3(pos.x, pos.y, pos.z - 0.2f); //devo agg -0.2f dopo pos.z

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

        CreateDiamonds(pos);

        //essendo che tale void viene richiamata ogni volta bisogna controllare il conteggio
        if (--counterUp <= 0) {
            CancelInvoke("SpawnInitialVertical"); 
            //con questo comando decremento counterup di 1. se quello che rimane è minore uguale a zero usciamo dal cancelinvoke,
            //che cancella l'invocazione, ossia la ripetizione
        }
    }

    public void BeginToSpawn()
    {
        direction = 1;//up

        gameOver = false;

        Bounds momSize = GetMaxBounds(platform); //caricato il valore più grande. però ora voglio solo la profondità z per
                                                //capire dove mettere la piattaforma
        size = momSize.size.z; //prendo solo l'asse z di momSize perchè è l'unica dimensione che mi serve



        momSize = GetMaxBounds(corner1);


        sizeCorner = momSize.size.z;
        counterUp = 1;

        //0.1f è il tempo di attesa prima di farla partire
        //timeforCreation è il tempo di attesa per le altre piattaforme
        InvokeRepeating("SpawnVertical", 0.1f, timeForCreation);


    }

    void SpawnVertical()
    {
        Vector3 pos = lastPos;
        pos.z += size;

        lastPos = new Vector3(pos.x, pos.y, pos.z - 0.2f); //devo agg -0.2f dopo pos.z

        GameObject newObject = GroundVerticalPoolerScript.current.GetPooledObject();
        //con il codice sopra creiamo una nuovo gameobject, lo chiamiamo newobject e richiamo la funzione
        //che mi ritorna un newobject
        //inoltre la variabile current è quella che ci permette di agganciarci allo script groundvertical..
        //ora però devo gestire il caso in cui non mi ritorna niente, il caso null

        if (newObject == null) return;
        //il return serve ad uscire nel caso null

        newObject.transform.position = pos; //come posizione all'oggetto creato gli diamo pos. pos è l'ultima posizione +size
                                            //dunque metto l'oggetto nell'ultima posizione quando finisce la piattaforma
        newObject.transform.rotation = Quaternion.identity; //rimane la stessa rotazione
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato


        CreateDiamonds(pos);

        //essendo che tale void viene richiamata ogni volta bisogna controllare il conteggio
        if (--counterUp <= 0) 
        {
            CancelInvoke("SpawnVertical");
            //con questo comando decremento counterup di 1. se quello che rimane è minore uguale a zero usciamo dal cancelinvoke,
            //che cancella l'invocazione, ossia la ripetizione

            if (!gameOver)
            {
                //CreateCombination creiamo delle piattaforme verticale e delle orizzontali
                CreateCombination();
                SpawnCornersHorizontal();
            }


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

    void CreateCombination()
    {
        SpawnInitialVertical();
        SpawnInitialVertical();
        int rand = UnityEngine.Random.Range(0, 10);

        //0-1
        if(rand >= 0 && rand <2)
        {
            //mettiamo una piattaforma vuota
            SpawnObstacolVertical(GroundEmptyVerticalPoolerScript.current.GetPooledObject());

            //ostacolo 1
            SpawnObstacolVertical(GroundVerticalObstacole1PoolerScript.current.GetPooledObject());
        }

        //2-3
        if(rand>1 && rand < 4)
        {
            //mettiamo una piattaforma vuota
            SpawnObstacolVertical(GroundEmptyVerticalPoolerScript.current.GetPooledObject());


            //ostacolo 2
            SpawnObstacolVertical(GroundVerticalObstacole2PoolerScript.current.GetPooledObject());
        }
        //4-5
        if (rand > 3 && rand < 6)
        {
            //ostacolo 1
            SpawnObstacolVertical(GroundVerticalObstacole1PoolerScript.current.GetPooledObject());


            //ostacolo 2
            SpawnObstacolVertical(GroundVerticalObstacole2PoolerScript.current.GetPooledObject());
        }
        //6-7
        if (rand > 5 && rand < 8)
        {
            //ostacolo 1
            SpawnObstacolVertical(GroundVerticalObstacole1PoolerScript.current.GetPooledObject());


            //ostacolo 1
            SpawnObstacolVertical(GroundVerticalObstacole1PoolerScript.current.GetPooledObject());
        }
        //8-9
        if (rand > 7 && rand < 10)
        {
            //ostacolo 2
            SpawnObstacolVertical(GroundVerticalObstacole2PoolerScript.current.GetPooledObject());


            //ostacolo 2
            SpawnObstacolVertical(GroundVerticalObstacole2PoolerScript.current.GetPooledObject());
        }
    }

    void SpawnObstacolVertical(GameObject newObj)
    {
        Vector3 pos = lastPos;
        pos.z += size;
        lastPos = new Vector3(pos.x, pos.y, pos.z - 0.2f);
        //GameObject newObject = Corner4PoolerScript.current.GetPooledObject();

        if (newObj == null) return;


        newObj.transform.position = pos;
        newObj.transform.rotation = Quaternion.identity;
        newObj.SetActive(true);


        //ne metto una normale
        pos = lastPos;
        pos.z += size;
        lastPos = new Vector3(pos.x, pos.y, pos.z - 0.2f);
        newObj = GroundVerticalPoolerScript.current.GetPooledObject();

        if (newObj == null) return;


        newObj.transform.position = pos;
        newObj.transform.rotation = Quaternion.identity;
        newObj.SetActive(true);

        CreateDiamonds(pos);


    }

    void SpawnCornersHorizontal()
    {
        int rand = UnityEngine.Random.Range(0, 2);//valori tra 0 e 1
        //rand = 0;//solo per testare----Rimuovere
        if (rand < 1)
        {
            //metto l'angolo che va a sinistra
            SpawnCornerLeft();
        }
        else
        {
            //metto l'angolo che va a destra
            SpawnCornerRight();

        }

    }

    void SpawnCornerLeft()
    {
        Vector3 pos = lastPos;
        //pos.x += (sizeCorner / 2) - 0.2f;
        //pos.z += (sizeCorner / 2) - 0.2f;

        lastPos = new Vector3(pos.x +5.9f, pos.y, pos.z +1.9f);
        GameObject newObject = Corner2PoolerScript.current.GetPooledObject();

        if (newObject == null) return;
        //il return serve ad uscire nel caso null

        newObject.transform.position = pos; //come posizione all'oggetto creato gli diamo pos. pos è l'ultima posizione +size
                                            //dunque metto l'oggetto nell'ultima posizione quando finisce la piattaforma
        newObject.transform.rotation = Quaternion.Euler(0, 180, 0); //Con Euler puoi fare la rotazione di un oggetto
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato



        if (!gameOver)
        {
            counterHor = 4;
            InvokeRepeating("SpawnHorizontalLeft", 0.1f, timeForCreation);

        }
        
    }

    void SpawnCornerRight()
    {
        Vector3 pos = lastPos;
        pos.x += (sizeCorner / 2)-0.2f;
        pos.z += (sizeCorner / 2)-0.2f;

        //questo last post era per una mia conferma (Donato)
        //lastPos = new Vector3(pos.x +1.80, pos.y, pos.z +2.20f);
        lastPos = new Vector3(pos.x -0.2f, pos.y, pos.z);
        GameObject newObject = Corner1PoolerScript.current.GetPooledObject();

        if (newObject == null) return;
        //il return serve ad uscire nel caso null

        newObject.transform.position =
            pos; //come posizione all'oggetto creato gli diamo pos. pos è l'ultima posizione +size
                 //dunque metto l'oggetto nell'ultima posizione quando finisce la piattaforma
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0); //Con Euler puoi fare la rotazione di un oggetto
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato

        

        if (!gameOver)
        {
            counterHor = 4;
            InvokeRepeating("SpawnHorizontalRight", 0.1f, timeForCreation);

        }
 
        }

    void SpawnHorizontalRight()
    {
        direction = 2;//destra

        Vector3 pos = lastPos;
        pos.x += size;

        lastPos = new Vector3(pos.x - 0.2f , pos.y, pos.z);

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
        newObject.transform.rotation = Quaternion.Euler(0,90,0); //rimane la stessa rotazione
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato

        CreateDiamonds(pos);

        //essendo che tale void viene richiamata ogni volta bisogna controllare il conteggio
        if (--counterHor <= 0)
        {
            CancelInvoke("SpawnHorizontalRight");
            //con questo comando decremento counterup di 1. se quello che rimane è minore uguale a zero usciamo dal cancelinvoke,
            //che cancella l'invocazione, ossia la ripetizione

            if (!gameOver)
            {
                int rand = UnityEngine.Random.Range(0, 10);

                rand = 2; //per testare da rimuovere

                if (rand > 1)
                {
                    //metto una piattaforma
                    SpawnEmptyHorizontalRight(); 
                }
                else
                {
                    //cambio il lastPos per il corner
                    lastPos = new Vector3(pos.x, pos.y, pos.y);

                    //lancio lo spawn del corner verso l'alto
                    SpawnCornerUp();
                }
            }


        }
    }


    void SpawnHorizontalLeft()
    {
        direction = 4;//sinistra

        Vector3 pos = lastPos;
        pos.x -= size;

        lastPos = new Vector3(pos.x + 0.2f, pos.y, pos.z);

        GameObject newObject = GroundVerticalPoolerScript.current.GetPooledObject();
        //con il codice sopra creiamo una nuovo gameobject, lo chiamiamo newobject e richiamo la funzione
        //che mi ritorna un newobject
        //inoltre la variabile current è quella che ci permette di agganciarci allo script groundvertical..
        //ora però devo gestire il caso in cui non mi ritorna niente, il caso null

        if (newObject == null) return;
        //il return serve ad uscire nel caso null

        newObject.transform.position = pos; //come posizione all'oggetto creato gli diamo pos. pos è l'ultima posizione +size
                                            //dunque metto l'oggetto nell'ultima posizione quando finisce la piattaforma
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0); //rimane la stessa rotazione
        newObject.SetActive(true); //diventa visibile. è come mettere il segno di spunta sull'oggetto appena creato

        CreateDiamonds(pos);

        //essendo che tale void viene richiamata ogni volta bisogna controllare il conteggio
        if (--counterHor <= 0)
        {
            CancelInvoke("SpawnHorizontalLeft");
            //con questo comando decremento counterup di 1. se quello che rimane è minore uguale a zero usciamo dal cancelinvoke,
            //che cancella l'invocazione, ossia la ripetizione


             
            if (!gameOver)
            {
                int rand = UnityEngine.Random.Range(0, 10);

                // rand = 0; //per testare da rimuovere

                if (rand > 1)
                {
                    //metto una piattaforma
                    SpawnEmptyHorizontalLeft();
                }
                else
                {
                    //cambio il lastPos per il corner
                    lastPos = new Vector3(pos.x - 9.5f, pos.y, pos.z + 1.9f); //da modificare

                    //lancio lo spawn del corner verso l'alto
                    SpawnCornerUp();
                }
            }
            


        }
    }

    void SpawnEmptyHorizontalRight()
    {
        direction = 2;//destra

        Vector3 pos = lastPos;
        pos.x += size;

        lastPos = new Vector3(pos.x - 0.2f, pos.y, pos.z);

        GameObject newObject = GroundEmptyVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;
        

        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0); 
        newObject.SetActive(true);

        //subito dopo metto una piattaforma normale altrimenti ci sarebbe subito l'angolo

        

        pos = lastPos;
        pos.x += size;

        lastPos = new Vector3(pos.x - 0.2f, pos.y, pos.z);

        newObject = GroundVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);

        CreateDiamonds(pos);

        //metto una piattaforma orizzontale con un ostacolo fancendo un estrazione random

        pos = lastPos;
        pos.x += size;

        lastPos = new Vector3(pos.x - 0.2f, pos.y, pos.z);

        int rand = UnityEngine.Random.Range(0, 2);

        if (rand < 1)
        {
            newObject = GroundVerticalObstacole1PoolerScript.current.GetPooledObject();    
        }
        else
        {
            newObject = GroundVerticalObstacole2PoolerScript.current.GetPooledObject();
        }

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);




        //ne metto una normale altrimenti avrei subito l'angolo


        pos = lastPos;
        pos.x += size;

        lastPos = new Vector3(pos.x - 0.2f, pos.y, pos.z);

        newObject = GroundVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);

        CreateDiamonds(pos);

        lastPos = new Vector3(pos.x, pos.y, pos.z);


        //metto l'angolo verso l'alto
        SpawnCornerUp();

    }

    void SpawnEmptyHorizontalLeft()
    {
        direction = 4;//sinistra

        Vector3 pos = lastPos;
        pos.x -= size;

        lastPos = new Vector3(pos.x - 0.2f, pos.y, pos.z);

        GameObject newObject = GroundEmptyVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);

        //subito dopo metto una piattaforma normale altrimenti ci sarebbe subito l'angolo



        pos = lastPos;
        pos.x -= size;

        lastPos = new Vector3(pos.x + 0.2f, pos.y, pos.z);

        newObject = GroundVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);

        CreateDiamonds(pos);

        //metto una piattaforma orizzontale con un ostacolo fancendo un estrazione random

        pos = lastPos;
        pos.x -= size;

        lastPos = new Vector3(pos.x + 0.2f, pos.y, pos.z);

        int rand = UnityEngine.Random.Range(0, 2);

        if (rand < 1)
        {
            newObject = GroundVerticalObstacole1PoolerScript.current.GetPooledObject();
        }
        else
        {
            newObject = GroundVerticalObstacole2PoolerScript.current.GetPooledObject();
        }

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);




        //ne metto una normale altrimenti avrei subito l'angolo


        pos = lastPos;
        pos.x -= size;

        lastPos = new Vector3(pos.x + 0.2f, pos.y, pos.z);

        newObject = GroundVerticalPoolerScript.current.GetPooledObject();

        if (newObject == null) return;


        newObject.transform.position = pos;
        newObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        newObject.SetActive(true);

        CreateDiamonds(pos);

        lastPos = new Vector3(pos.x - 9.5f, pos.y, pos.z + 1.9f); //da modificare


        //metto l'angolo verso l'alto
        SpawnCornerUp();

    }

    void SpawnCornerUp()
    {

        Vector3 pos = lastPos;
        if (direction == 2)
        {
            lastPos = new Vector3(pos.x + 1.9f, pos.y, pos.z + 1.8f); //da modificare
            GameObject newObject = Corner3PoolerScript.current.GetPooledObject();

            if (newObject == null) return;


            newObject.transform.position = pos;
            newObject.transform.rotation = Quaternion.Euler(0, -90, 0);
            newObject.SetActive(true);
        }
        else
        {
            //in questo caso sarà sicuro la direction uguale a 4. Quindi va nell'else quando direction è uguale a 4
            lastPos = new Vector3(pos.x, pos.y, pos.z - 0.2f); //da modificare
            GameObject newObject = Corner4PoolerScript.current.GetPooledObject();

            if (newObject == null) return;


            newObject.transform.position = pos;
            newObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            newObject.SetActive(true);
        }

        direction = 1; //Up
        if (!gameOver)
        {
            CreateCombination();

            counterUp = 5;
            InvokeRepeating("SpawnVertical",0.1f,timeForCreation);
        }
        
    }
}
