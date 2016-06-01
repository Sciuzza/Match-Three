using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Block_S : MonoBehaviour
    {


        public float speed = -1f;
        public Transform targetTr;
        MThree_S brain_linking;


        void Start()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            targetTr = brain_linking.cell_pointers[0, 0].transform;
        }

        // Update is called once per frame
        void Update()
        {

            if (transform.position.y > targetTr.position.y)
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
            else
                transform.position = targetTr.position;
        }
    }
}