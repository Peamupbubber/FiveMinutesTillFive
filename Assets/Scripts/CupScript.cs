using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupScript : MonoBehaviour
{
    public bool isFilled = false;
    public GameObject myLiquid;
    public Material water;
    public Material coffee;

    CarryableScript carryableScript;
    Quaternion normalRotation;

    [SerializeField] private Sprite cupEmptySprite;
    [SerializeField] private Sprite cupFilledWaterSprite;
    [SerializeField] private Sprite cupFilledCoffeeSprite;

    void Start()
    {
        carryableScript = gameObject.GetComponent<CarryableScript>();
        myLiquid.SetActive(false);
        normalRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        transform.parent = null;
    }

    public void Fill(string Liquid)
    {
        isFilled = true;
        myLiquid.SetActive(true);
        if(Liquid == "Water")
        {
            myLiquid.GetComponent<MeshRenderer>().material = water;
            carryableScript.SetSprite(cupFilledWaterSprite);
        } else if(Liquid == "Coffee")
        {
            myLiquid.GetComponent<MeshRenderer>().material = coffee;
            carryableScript.SetSprite(cupFilledCoffeeSprite);
        }
    }

    public void Normalize()
    {
        transform.rotation = normalRotation;
    }

    private void FixedUpdate()
    {
        if (isFilled && transform.rotation != normalRotation)
        {
            isFilled = false;
            myLiquid.SetActive(false);
            carryableScript.SetSprite(cupEmptySprite);
        }
    }
}
