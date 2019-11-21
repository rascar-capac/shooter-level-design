using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    // Start is called before the first frame update
    private float minimum;
    public float maximum = 5;
    [Range(0,3)]public float speed; 
    Light myLight;
    float t = 0;
    bool isShrinking;

    void Start()
    {
       myLight = GetComponent<Light>();
       minimum = myLight.intensity;
       isShrinking = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(t);
        if (!isShrinking)
        {
            myLight.intensity = Mathf.Lerp(minimum, maximum, t += speed * Time.deltaTime);
            if (t >= 1)
            {
                isShrinking = true;
                t = 0;
            }
                
        }
        else
        {
            
            myLight.intensity = Mathf.Lerp(maximum, minimum, t += speed * Time.deltaTime);
            if(t >= 1)
            {
                isShrinking = false;
                t = 0;
            }
        }
    }
}
