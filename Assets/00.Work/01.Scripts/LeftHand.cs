using DG.Tweening;
using UnityEngine;

public class LeftHand : MonoBehaviour
{
    public int endValue;
    public int duration;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ¿ÞÂÊ ÇÚµå ¹ß»ç
        if (Input.GetMouseButtonDown(0)) // ÁÂÅ¬¸¯
        {
            transform.DOMoveZ(endValue, duration).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
        }
    }

}
