using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class Try_S : MonoBehaviour
    {
        MThree_S brain_linking;
        public BoxCollider2D clickable;
        public TextMesh opacity;
        General_functions scene_m_linking;

        // Use this for initialization
        void Awake()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            clickable = this.GetComponent<BoxCollider2D>();
            opacity = this.GetComponent<TextMesh>();
            scene_m_linking = GameObject.Find("Scene Manager").GetComponent<General_functions>();
        }
        
        void OnMouseUp()
        {
            scene_m_linking.restart_scene();
        }
    }
}