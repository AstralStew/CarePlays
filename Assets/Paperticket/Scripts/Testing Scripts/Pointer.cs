using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{

    RaycastHit hitInfo;
    [SerializeField] LayerMask layers;
    [SerializeField] float maxDistance;

    [SerializeField] SpriteRenderer hitSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, layers)) {
            
            hitSprite.transform.position = hitInfo.point;
            hitSprite.transform.position += (transform.position - hitSprite.transform.position).normalized * 0.1f;

            hitSprite.transform.forward = hitInfo.normal;
            

            if (hitSprite.color.a == 0) {
                hitSprite.color = new Color(hitSprite.color.r, hitSprite.color.b, hitSprite.color.g, 1);
            }

        } else {

            if (hitSprite.color.a == 1) {
                hitSprite.color = new Color(hitSprite.color.r, hitSprite.color.b, hitSprite.color.g, 0);
            }
        }
        
    }
}
