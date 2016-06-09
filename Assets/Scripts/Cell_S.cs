using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Cell_S : MonoBehaviour
    {
        public SpriteRenderer[] sr_array = new SpriteRenderer[1];
        MThree_S brain_linking;
        public int cell_i, cell_j;
        public bool red_p = true, blue_p = true, green_p = true, cyan_p = true, magenta_p = true;
        public bool tris_checked = false;
        
        // get and initialize sprite renderer componet
        void Awake()
        {

            sr_array[0] = GetComponent<SpriteRenderer>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
        }

    
        
        void OnMouseUp()
        {
            

            //if current game phase is set to Animation will appear a debug message saying "animation in progress..." and click will have no effects
            if (brain_linking.current_gp == MThree_S.game_phases.init || brain_linking.current_gp >= MThree_S.game_phases.animation)
                Debug.Log("Animation in progress...");
            else if (brain_linking.current_gp == MThree_S.game_phases.sel_source)
            {
                brain_linking.selection_visibility_source(cell_i, cell_j);
            }
            else if (brain_linking.current_gp == MThree_S.game_phases.sel_dest)
            {
                if (brain_linking.is_next_to(cell_i, cell_j))
                {
                    if (brain_linking.is_sel_generating_tris(cell_i, cell_j))
                    {
                        brain_linking.animation_swap();
                        brain_linking.deselecting();
                    }
                    else
                        Debug.Log("Selection is correct but not possible because it doesn't generate tris");
                }
                else
                    Debug.Log("Wrong Selection");
            }
    
          

        }

        
    }
}