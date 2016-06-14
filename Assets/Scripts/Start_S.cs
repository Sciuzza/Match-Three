using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Start_S : MonoBehaviour
    {
        
        Sound_S sound_linking;


        // Use this for initialization
        void Awake()
        {
            sound_linking = GameObject.Find("Sound Controller").GetComponent<Sound_S>();
           
        }


        

        void OnMouseEnter()
        {
            sound_linking.play_environment(6);
           
        }

       
    }
}
