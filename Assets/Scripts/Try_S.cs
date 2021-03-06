﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class Try_S : MonoBehaviour
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
            scene_m_linking.restart_scene();
        }

        void OnMouseEnter()
        {
            opacity.color = Color.green;
            sound_linking.play_special(1);
        }

        void OnMouseExit()
        {
            opacity.color = Color.white;
        }
    }
}