using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class Give_S : MonoBehaviour
    {
        public BoxCollider2D clickable;
        public TextMesh opacity;
        General_functions scene_m_linking;
        Sound_S sound_linking;


        // Use this for initialization
        void Awake()
        {
            sound_linking = GameObject.Find("Sound Controller").GetComponent<Sound_S>();
            clickable = this.GetComponent<BoxCollider2D>();
            opacity = this.GetComponent<TextMesh>();
            scene_m_linking = GameObject.Find("Scene Manager").GetComponent<General_functions>();
        }


        void OnMouseUp()
        {
            scene_m_linking.Application_quit();
        }

        void OnMouseEnter()
        {
            sound_linking.play_special(2);
            opacity.color = Color.green;
        }

        void OnMouseExit()
        {
            opacity.color = Color.white;
        }
    }
}