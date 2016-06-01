using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Cell_S : MonoBehaviour
    {
        public SpriteRenderer sr;
        // define the enumerator for cells color
        public enum block { red, blue, green, cyan, magenta };
        
        // get and initialize sprite renderer componet
        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            // call the enumerator and get randomize the starting color
            switch ((block)(Random.Range(0, 5)))
            {
                case block.red:
                    sr.color = Color.red;
                    break;

                case block.blue:
                    sr.color = Color.blue;
                    break;

                case block.green:
                    sr.color = Color.green;
                    break;

                case block.cyan:
                    sr.color = Color.cyan;
                    break;

                case block.magenta:
                    sr.color = Color.magenta;
                    break;
            }


        }

        void OnMouseUp()
        {

          //if current game phase is set to Animation will appear a debug message saying "animation in progress..." and click will have no effects

          //else if current game phase is set to sel_source the player will be able to select 1 block

          //else if current game phase is set to sel_dest the player will be able to select an adiacent block to the first one selected in the previous game phase
            
            // if the selected block is not adiacent to the previous one will appear a message on the screen indicating that the selection is forbidden
            
            // else if the selected block is adiacent but the swap will not produce any combo will appear a message on the screen that the selection is forbidden
            
            // else the game phase will be set to animation (to prevent any click during the animation), blocks will be swapped, 
            // every tris will be destroyed updating the current score that will be visible on scene and in the end new blocks will appears to the top and will fall down til they reach their position
            // when the last action will be completed the game phase will be set again to sel_source

        }
    }
}