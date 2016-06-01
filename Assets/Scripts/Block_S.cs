using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Block_S : MonoBehaviour
    {


        public float speed = -1f;
        public Transform target_cell;
        public SpriteRenderer block_sr;
        public int block_i, block_j;
        MThree_S brain_linking;


        void Awake()
        {
            block_sr = GetComponent<SpriteRenderer>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            
        }

        void Start()
        {
            block_sr.color = brain_linking.color_block_setting(block_i, block_j);
            this.name = "Block " + block_i + "," + block_j;
        }

        // Update is called once per frame
        void Update()
        {

            if (transform.position.y > target_cell.position.y)
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
            else {
                transform.position = new Vector3(target_cell.position.x, target_cell.position.y, -1);
          
            }
            
        }
    }
}