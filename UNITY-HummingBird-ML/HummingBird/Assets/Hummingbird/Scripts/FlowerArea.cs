using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Manages a collection of plants and attached flowers </summary>
public class FlowerArea : MonoBehaviour
{

    //The diameter of the area where the agent and flowers
    // can be used for observing relative distance from agent to flower
    public const float AREA_DIAMETER = 20f;

    //List of all flower plants in the area
    private List<GameObject> _flowerPlants;

    //A lookup dictionary for looking up a flower from a nectar collider
    private Dictionary<Collider, Flower> _nectarFlowerDictionary;

    /// <summary> List of all flowers in the flower area </summary>
    public List<Flower> Flowers { get; private set; }

    /// <summary> Reset flowers and plants </summary>
    public void ResetFlowers()
    {
        //Rotate each plant around the Y axis and subtly around X and Z
        foreach(GameObject plants in _flowerPlants)
        {
            float yRotation = UnityEngine.Random.Range(-180f,180f);
            float xRotation = UnityEngine.Random.Range(-5f,5f);
            float zRotation = UnityEngine.Random.Range(-5f,5f);
            plants.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        foreach(Flower flower in Flowers)
        {
            flower.ResetFlower();
        }
    }

    /// <summary> Gets the <see cref="Flower"/> that the nectar collider belongs to </summary>
    /// <param name="collider"> The nectar collider </param>
    /// <returns> The matching flower </returns>
    public Flower GetFlowerFromNectar(Collider collider)
    {
        return _nectarFlowerDictionary[collider];
    }

    /// <summary> Callend when the area wakes up </summary>
    private void Awake() 
    {
        //Initialize variables
        _flowerPlants = new List<GameObject>();
        _nectarFlowerDictionary = new Dictionary<Collider, Flower>();
        Flowers = new List<Flower>();
    }

    private void Start() {
        //Find all flowers children of this Game Object / Transform
        FindChildFlowers(this.transform);
    }

    /// <summary> Recursively find all children flowers of the parent transform </summary>
    /// <param name='parent'> The parent of the children flowers </param>
    private void FindChildFlowers(Transform parent)
    {
        for (int i =0; i<parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag("flower_plant"))
            {
                // found a flower plant and add it to the list
                _flowerPlants.Add(child.gameObject);

                // check if there is flowers within the plant
                FindChildFlowers(child);
            }
            else
            {
                // not a flower plant, however look for flower component
                Flower flower = child.GetComponent<Flower>();
                if (flower != null)
                {
                    //found a flower and add it to the list
                    Flowers.Add(flower);

                    //add the nectar collider to the dictionary
                    _nectarFlowerDictionary.Add(flower.NectarCollider,flower);
                }
                else
                {
                    //if not found check child
                    FindChildFlowers(child);
                }
            }
        }
    }
}
