using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Collider c;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClicked()
    {
        Debug.Log("OnClicked");
        c.enabled = false;
    }
}
