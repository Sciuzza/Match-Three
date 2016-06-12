using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MatchThree
{

    public class Timer_S : MonoBehaviour
    {

        public Text timerLabel;
        MThree_S brain_linking;
        int minutes;
        int seconds;

        void Awake()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            timerLabel = this.GetComponent<Text>();
            minutes = (int)brain_linking.time / 60;
            seconds = (int)brain_linking.time % 60;
            timerLabel.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }

        void Update()
        {
            if (!brain_linking.updating_time && brain_linking.time >= 0 && brain_linking.current_gp >= MThree_S.game_phases.sel_source)
            {
                brain_linking.time -= Time.deltaTime;

                minutes = (int)brain_linking.time / 60;
                seconds = (int)brain_linking.time % 60;



                timerLabel.text = string.Format("{0:00} : {1:00}", minutes, seconds);
            }
        }
    }

}