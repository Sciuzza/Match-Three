using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Block_S : MonoBehaviour
    {


        public float speed;
        public Vector2 direction, distance = new Vector2 (-100, -100);
        public Transform target_cell;
        public SpriteRenderer[] sr_array = new SpriteRenderer[2];
        public int block_i, block_j;
        MThree_S brain_linking;
        public ParticleSystem ps_link;
        public bool is_dying = false;
       

        void Awake()
        {
            sr_array[0] = GetComponent<SpriteRenderer>();
            sr_array[1] = this.transform.Find("Selection").GetComponent<SpriteRenderer>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            ps_link = GetComponent<ParticleSystem>();
        }

        void Start()
        {
            sr_array[0].sprite = brain_linking.color_block_setting(block_i, block_j);
            
            this.name = "Block " + block_i + "," + block_j;
        }

        // Update is called once per frame
        void Update()
        {

            
                distance = this.transform.position - target_cell.position;
                direction = distance.normalized;


                this.transform.position = (Vector2)(this.transform.position) + direction * speed * Time.deltaTime;
            
            if (distance.magnitude <= 0.1f)
            {
                transform.position = new Vector3(target_cell.position.x, target_cell.position.y, 7);
            }

            if (!ps_link.isPlaying && is_dying)
            {
                GameObject score_in_place = Resources.Load<GameObject>("Score in Place");
                score_in_place = Instantiate(score_in_place);
                score_in_place.transform.position = this.transform.position;
                Destroy(this.gameObject);
            }

        }
     

    }
}