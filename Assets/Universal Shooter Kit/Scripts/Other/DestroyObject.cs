using UnityEngine;
using System.Collections;

namespace GercStudio.USK.Scripts
{

    public class DestroyObject : MonoBehaviour
    {
        public float destroy_time;

        void Start()
        {
            StartCoroutine("CheckIfAlive");
        }

        IEnumerator CheckIfAlive()
        {
            while (true)
            {
                yield return new WaitForSeconds(destroy_time);
                Destroy(gameObject);
                StopCoroutine("CheckIfAlive");
            }
        }
    }
}




