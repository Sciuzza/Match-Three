using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MatchThree
{
    public class Score_s : MonoBehaviour
    {

        Text score_updating;
        MThree_S brain_linking;

        // Use this for initialization
        void Awake()
        {
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
            score_updating = this.GetComponent<Text>();
            score_updating.text = "" + brain_linking.score;
        }

        // Update is called once per frame
        void Update()
        {
            if (brain_linking.current_gp >= MThree_S.game_phases.animation)
            score_updating.text = "" + brain_linking.score;
        }

    }

}