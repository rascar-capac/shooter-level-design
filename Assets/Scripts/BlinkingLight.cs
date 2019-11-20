using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 minimum;
    public Vector2 maximum = new Vector2(15, 15);
    [Range(0,3)]public float speed; 
    Light light;
    float t = 0;
    bool isShrinking;

    void Start()
    {
       light = GetComponent<Light>();
       minimum = light.areaSize;
       isShrinking = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(t);
        if (!isShrinking)
        {
            light.areaSize = Vector2.Lerp(minimum, maximum, t += speed * Time.deltaTime);
            if (t >= 1)
            {
                isShrinking = true;
                t = 0;
            }
                
        }
        else
        {
            
            light.areaSize = Vector2.Lerp(maximum, minimum, t += speed * Time.deltaTime);
            if(t >= 1)
            {
                isShrinking = false;
                t = 0;
            }
        }
    }
}
