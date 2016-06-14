using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MatchThree
{
    public class Sound_S : MonoBehaviour
    {

        AudioSource[] Music = new AudioSource[1];
        AudioSource[] Environment = new AudioSource[6];
        AudioSource[] Interaction = new AudioSource[9];
        AudioSource[] Special = new AudioSource[2];

        

        void Awake()
        {
            DontDestroyOnLoad(this);
            Music = this.gameObject.transform.Find("Music").GetComponents<AudioSource>();
            Music[0].Play();
            Music[0].loop = true;

            Environment = this.gameObject.transform.Find("Environment").GetComponents<AudioSource>();
            Interaction = this.gameObject.transform.Find("Interaction").GetComponents<AudioSource>();
            Special = this.gameObject.transform.Find("Special").GetComponents<AudioSource>();
        }

        public void play_interaction(int number)
        {
            Interaction[number].Play();
        }

        public void play_music(int number)
        {
            Music[number].Play();
        }

        public void play_environment(int number)
        {
            Environment[number].Play();
        }

        public void play_special(int number)
        {
            Special[number].Play();
        }

        public void stop_play_environment()
        {
            Environment[6].Stop();
        }

    }
}