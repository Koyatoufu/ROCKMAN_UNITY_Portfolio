using UnityEngine;
using System.Collections;


public class BillBoard : MonoBehaviour
{
	// Use this for initialization
	void Start () {
        StartCoroutine(RotateCoroutine());
	}
	
    IEnumerator RotateCoroutine()
    {
        yield return null;

        while(true)
        {
            yield return null;

            if(Camera.main==null)
            {
                continue;
            }

            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

}
