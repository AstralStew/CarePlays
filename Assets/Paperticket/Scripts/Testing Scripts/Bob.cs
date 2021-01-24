using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    [SerializeField] Vector3 iniPos;
    [SerializeField] float speed;
    [SerializeField] float maxY;

    // Start is called before the first frame update
    void Start()
    {
        iniPos = transform.position;
        StartCoroutine(Bobbing());
    }
    
    IEnumerator Bobbing() {
        float dir = 1;

        while (true) {

            transform.position += Vector3.up * speed * dir * Time.deltaTime;

            if (transform.position.y >= iniPos.y + maxY) {
                dir = -1;
            } else if (transform.position.y < iniPos.y) {
                dir = 1;
            }

            yield return null;
        }
    }



}
