using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //�Ѿ�
    private void Start()
    {
        StartCoroutine(DeleteBullet());
    }

    IEnumerator DeleteBullet()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
