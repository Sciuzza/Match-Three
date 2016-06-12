using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MatchThree
{

    public class Level_S : MonoBehaviour
    {

        public Text timerLabel, current_level_text;
        MThree_S brain_linking;
        int minutes;
        int seconds;
        public int next_level = 0;

        void Awake()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            timerLabel = this.GetComponent<Text>();
            current_level_text = GameObject.Find("Level Number").GetComponent<Text>();
            next_level = brain_linking.level + 1;
            minutes = (int)brain_linking.time_to_next / 60;
            seconds = (int)brain_linking.time_to_next % 60;
            timerLabel.text = string.Format("{0:00}:{1:00} to level {2}", minutes, seconds, next_level);
        }

        void Update()
        {
            if (brain_linking.current_gp >= MThree_S.game_phases.sel_source)
            {
                brain_linking.time_to_next -= Time.deltaTime;

                minutes = (int)brain_linking.time_to_next / 60;
                seconds = (int)brain_linking.time_to_next % 60;
                next_level = brain_linking.level + 1;
                current_level_text.text = "Level " + brain_linking.level;

                if (brain_linking.level < 4)
                    timerLabel.text = string.Format("{0:00}:{1:00} to level {2}", minutes, seconds, next_level);
                else
                    timerLabel.text = string.Format("{0:00}:{1:00} to Victory", minutes, seconds);
            }
        }
    }

}
