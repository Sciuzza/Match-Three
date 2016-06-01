using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class MThree_S : MonoBehaviour
    {

        GameObject cell_linking_init;
        GameObject[,] cell_pointers = new GameObject[8, 10];

        public enum game_phases { animation, sel_source, sel_dest};
        public game_phases current_gp = game_phases.sel_source;

        void Awake()
        {
            cell_linking_init = Resources.Load<GameObject>("Cell");

            for (int i = 0; i < cell_pointers.GetLength(0); i++)
            {
                for (int j = 0; j < cell_pointers.GetLength(1); j++)
                {
                    cell_linking_init = Instantiate(cell_linking_init);
                    cell_linking_init.transform.position = new Vector2(j - 4.5f, i - 3.5f);
                    cell_linking_init.name = "Cell" + i + "," + j;
                }
            }

            

        }



    }
}