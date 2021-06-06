using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBackgrounds : MonoBehaviour
{

    [SerializeField]
    List<Transform> backgrounds = null;
    [SerializeField] float speed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < backgrounds.Count; i++)
        //{
        //    float sizeX = backgrounds[i].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        //    GameObject go = Instantiate(backgrounds[i].gameObject, transform);
        //    go.transform.localPosition = new Vector3(go.transform.localPosition.x + sizeX, go.transform.localPosition.y, go.transform.localPosition.z);

        //    GameObject go2 = Instantiate(backgrounds[i].gameObject, transform);
        //    go2.transform.localPosition = new Vector3(go2.transform.localPosition.x + (sizeX * 3), go2.transform.localPosition.y, go2.transform.localPosition.z);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}