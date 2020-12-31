using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> This scripts handles a single flower and its nectar </summary>
public class Flower : MonoBehaviour
{
    #region Flower Properties

    [Tooltip("The color when the flower is full of nectar")]
    public Color FullFlowerColor = new Color(1f, 0f, .3f);
    [Tooltip("The color when the flower is empty of nectar")]
    public Color EmptyFlowerColor = new Color(.5f, 0f, 1f);

    /// <summary> The trigger collider representing the nectar </summary>
    [HideInInspector]
    public Collider NectarCollider;

    //The flower's collider which represents the petals
    private Collider _flowerCollider;

    //The flower's material to be manipulated 
    private Material _flowerMaterial;

    /// <summary> A vector pointing straight out of the flower (flower orientation) </summary>
    public Vector3 FlowerUpVector { get => NectarCollider.transform.up; }

    /// <summary> The center position of the nectar collider </summary>
    public Vector3 FlowerCenterPosition { get => NectarCollider.transform.position; }

    /// <summary> The amount of the nectar remaining in the flower </summary>
    public float NectarAmount { get; private set; } //makes the set to be private only

    /// <summary> Return if the amount of nectar is greater than 0 </summary>
    public bool HasNectar { get => NectarAmount > 0f; }

    #endregion

    private void Awake() 
    {
        //Find flower mesh renderer and get material
        _flowerMaterial = GetComponent<MeshRenderer>().material;

        //Find flower and nectar colliders
        _flowerCollider = transform.Find("FlowerCollider").GetComponent<Collider>();
        NectarCollider = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }

    /// <summary> Attempts to get nectar from the flower </summary>
    /// <param name="amount"> The amount of nectar to be removed </param>
    /// <returns> The actual amount successfully removed </returns>
    public float Feed(float amount)
    {
        //Track how much nectar was successfully taken
        float nectarTaken = Mathf.Clamp(amount, 0, NectarAmount);

        //Subtract the nectar 
        NectarAmount -= amount;

        if (NectarAmount <= 0)
        {
            NectarAmount = 0;

            _flowerCollider.gameObject.SetActive(false);
            NectarCollider.gameObject.SetActive(false);

            _flowerMaterial.SetColor("_BaseColor", EmptyFlowerColor);
        }

        return nectarTaken;
    }

    /// <summary> Resets the flower </summary>
    public void ResetFlower()
    {
        //Refill the nectar
        NectarAmount = 1f;

        //Enable Flower and colliders
        _flowerCollider.gameObject.SetActive(true);
        NectarCollider.gameObject.SetActive(true);

        //Change flower color back to normal
        _flowerMaterial.SetColor("_BaseColor", FullFlowerColor);
    }
}
