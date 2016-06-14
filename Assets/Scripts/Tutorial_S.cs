using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Tutorial_S : MonoBehaviour
    {
        MThree_S brain_linking;
        TextMesh changing_t, space_changing;
        int tutorial_phase = 0;

        void Awake()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            changing_t = this.GetComponent<TextMesh>();
            space_changing = GameObject.Find("Space to Continue").GetComponent<TextMesh>();
        }


        // Update is called once per frame
        void Update()
        {


            if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 0)
            {
                tutorial_phase = 1;
                changing_t.text = "First Select a Weapon\n then select an adjacent one to swap";
                space_changing.text = "\n" + space_changing.text;
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 1)
            {
                tutorial_phase = 2;
                changing_t.text = "The swap will occur only\nif a tris will be generated";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 2)
            {
                tutorial_phase = 3;
                changing_t.text = "Every weapon destroyed\n will increase the score and time";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 3)
            {
                tutorial_phase = 4;
                changing_t.text = "Every Combo and Level beyond the first\nwill increase Score even further";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 4)
            {
                tutorial_phase = 5;
                changing_t.text = "Every Level beyond the first\nwill decrease Time gain";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 5)
            {
                tutorial_phase = 6;
                changing_t.text = "You have to survive 4 levels\nto defeat Illidan";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 6)
            {
                tutorial_phase = 7;
                changing_t.text = "The time necessary to reach the next level\nis on your left";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 7)
            {
                tutorial_phase = 8;
                changing_t.text = "If you win any spare time left\nwill be converted into additional score";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 8)
            {
                tutorial_phase = 9;
                space_changing.text = "Press Space to Begin";
                changing_t.text = "Good Luck Hero!!!";
            }
            else if (Input.GetKeyDown(KeyCode.Space) && tutorial_phase == 9)
            {
                tutorial_phase = 10;
                brain_linking.current_gp = MThree_S.game_phases.init;
                Time.timeScale = 1;
                Destroy(this.gameObject);
            }
        }
    }
}