using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerManager : MonoBehaviour
{
    public static PoolerManager current; //per interfecciare con altri script, in moda da richiamarla
    public GameObject pooledObject;
    public int pooledAmount = 5; //totale di oggetti che metteremo di piattaforme verticali
    public bool willGrow = true; //ci serve per dire se deve creare o meno oggetti

    //List<GameObject> pooledObjects;

    [SerializeField]
    private LevelPlatform[] pooledObjects;
    private GameObject folder;

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in pooledObjects)
        {
            for (int i = 0; i < obj.probability; i++)
            {
                GameObject newObject = (GameObject)Instantiate(obj.prefab); //dentro newGameObject istanzio un nuovo oggetto
                newObject.SetActive(false); //ho creato l'oggetto e finchè è false non appare
                obj.GetPLatforms().Add(newObject);

                newObject.transform.SetParent(folder.transform);
            }
        }
    }

    public GameObject GetPooledObject(Object obj)
    {
        //LevelPlatform

        //for (int i = 0; i < pooledObjects.Count; i++)
        //{
        //    if (!pooledObjects[i].activeInHierarchy) //se l'oggetto in posizone i-esima, se l'oggetto non è attivo, ritrona l'oggetto i
        //    {
        //        return pooledObjects[i];
        //    }
        //}

        //if (willGrow) //Se willgrow è true
        //{
        //    GameObject newObject = (GameObject)Instantiate(pooledObject); //istanzio il gameobject del prefab 
        //    pooledObjects.Add(newObject); //si aggiunte il nuovo oggetto alla lista
        //    return (newObject); //Ritorna l'oggetto istanziato
        //}

        return null;
    }
}

[System.Serializable]
public class LevelPlatform
{
    public string name;
    public GameObject prefab;
    public int probability = 1;
    public bool willGrow;
    private static List<Object> listPlatforms;

    public List<Object> GetPLatforms()
    {
        return listPlatforms;
    }
}

[System.Serializable]
public class GroundVerticalPooler
{
    public GameObject gameObject;
    public int probability = 1;
    public bool willGrow;
}

[System.Serializable]
public class LevelObstacle
{
    public string name;
    public GameObject prefab;
    public int probability = 1;
    public string type;
}