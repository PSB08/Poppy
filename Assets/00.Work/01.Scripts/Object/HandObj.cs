using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandObj : MonoBehaviour
{
    [SerializeField] private RightHand rightHand;
    [SerializeField] private GameObject handObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            gameObject.SetActive(false);
            if (!rightHand.hands.Contains(handObject.transform))
            {
                rightHand.hands.Add(handObject.transform);
            }
        }
        
    }
}
