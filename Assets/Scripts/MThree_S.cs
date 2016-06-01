using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class MThree_S : MonoBehaviour
    {

        GameObject cell_linking_init, block_linking_init;
        Cell_S cell_linking_script;
        Block_S block_linking_script;
        int rows = 0;
        public GameObject[,] cell_pointers = new GameObject[8, 10];
        public GameObject[,] blocks_pointers = new GameObject[8, 10];

        public enum game_phases { animation, sel_source, sel_dest };
        public game_phases current_gp = game_phases.animation;

        void Awake()
        {
            cell_linking_init = Resources.Load<GameObject>("Cell");

            for (int i = 0; i < cell_pointers.GetLength(0); i++)
            {
                for (int j = 0; j < cell_pointers.GetLength(1); j++)
                {

                    cell_linking_init = Instantiate(cell_linking_init);
                    cell_pointers[i, j] = cell_linking_init;
                    cell_linking_init.transform.position = new Vector3(j - 4.5f, i - 3.5f, -12);
                    cell_linking_init.name = "Cell" + i + "," + j;
                    cell_linking_script = cell_linking_init.GetComponent<Cell_S>();
                    cell_linking_script.cell_sr.color = color_randomization();
                    cell_linking_script.cell_i = i;
                    cell_linking_script.cell_j = j;
                }
            }



        }

        void Start()
        {
            block_linking_init = Resources.Load<GameObject>("Block");

            block_spawning_init();
        }

        public void Update()
        {          
                if (block_linking_init.transform.position.y < cell_pointers[7, 0].transform.position.y && rows < 7)
                {
                    rows++;
                    block_spawning_init();
                }
            
        }

        Color color_randomization()
        {
            switch ((Random.Range(0, 5)))
            {
                case 0:
                    return Color.red;

                case 1:
                    return Color.blue;

                case 2:
                    return Color.green;

                case 3:
                    return Color.cyan;

                default:
                    return Color.magenta;

            }
        }
        public Color color_block_setting(int block_i_temp, int block_j_temp)
        {
            cell_linking_script = cell_pointers[block_i_temp, block_j_temp].GetComponent<Cell_S>();
            return cell_linking_script.cell_sr.color;
        }

        void block_spawning_init()
        {
            for (int j = 0; j < cell_pointers.GetLength(1); j++)
            {

                block_linking_init = Instantiate<GameObject>(block_linking_init);
                blocks_pointers[rows, j] = block_linking_init;
                block_linking_init.transform.position = new Vector2(cell_pointers[rows, j].transform.position.x, 10);
                block_linking_script = block_linking_init.GetComponent<Block_S>();
                block_linking_script.target_cell = cell_pointers[rows, j].transform;
                block_linking_script.block_i = rows;
                block_linking_script.block_j = j;
            }

            if (rows == 7)
                current_gp = game_phases.sel_source;
        }

        
    }
}