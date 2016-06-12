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
        Sound_S sound_linking_script;

        // get and initialize sprite renderer componet
        void Awake()
        {
            sound_linking_script = GameObject.Find("Sound Controller").GetComponent<Sound_S>();
            sr_array[0] = GetComponent<SpriteRenderer>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
        }

    
        
        void OnMouseUp()
        {
            

            //if current game phase is set to Animation will appear a debug message saying "animation in progress..." and click will have no effects
            if (brain_linking.current_gp == MThree_S.game_phases.init || brain_linking.current_gp >= MThree_S.game_phases.animation)
                sound_linking_script.play_interaction(8);
            else if (brain_linking.current_gp == MThree_S.game_phases.sel_source)
            {
                sound_linking_script.play_interaction(0);
                brain_linking.reset_all();
                brain_linking.selection_visibility_source(cell_i, cell_j);
                brain_linking.current_gp = MThree_S.game_phases.sel_dest;
            }
            else if (brain_linking.current_gp == MThree_S.game_phases.sel_dest)
            {
                if (brain_linking.is_next_to(cell_i, cell_j))
                {
                    if (brain_linking.is_sel_generating_tris(cell_i, cell_j))
                    {
                        brain_linking.current_gp = MThree_S.game_phases.animation;
                        sound_linking_script.play_interaction(Random.Range(1, 4));
                        brain_linking.animation_swap();
                        brain_linking.Invoke("deselecting", 0.3f);
                    }
                    else
                    {
                        brain_linking.current_gp = MThree_S.game_phases.animation;
                        sound_linking_script.play_interaction(Random.Range(4,8));
                        brain_linking.animation_swap();
                        brain_linking.Invoke("reverse_swap", 0.5f);
                    }
                }
                else
                {
                    brain_linking.current_gp = MThree_S.game_phases.sel_source;
                    brain_linking.is_switching = true;
                    sound_linking_script.play_interaction(0);
                    brain_linking.deselecting();
                    brain_linking.reset_all();
                    brain_linking.selection_visibility_source(cell_i, cell_j);
                    brain_linking.is_switching = false;
                    brain_linking.current_gp = MThree_S.game_phases.sel_dest;
                }
                    
            }
    
          

        }

        
    }
}