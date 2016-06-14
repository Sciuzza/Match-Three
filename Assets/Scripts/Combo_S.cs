using UnityEngine;
using System.Collections;

namespace MatchThree {
    public class Combo_S : MonoBehaviour {

        public TextMesh[] tm_array = new TextMesh[3];
        MThree_S brain_linking;
        float timer_ani = 0;

        // Use this for initialization
        void Awake() {

            tm_array[0] = GetComponent<TextMesh>();
            tm_array[1] = this.transform.Find("Combo Number Score Gain").GetComponent<TextMesh>();
            tm_array[2] = this.transform.Find("Time Gain").GetComponent<TextMesh>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
        }


        void Start()
        {
            tm_array[0].text = "Combo x" + (brain_linking.c_counter + 1);
            tm_array[1].text = "x " + brain_linking.tris_cells.Count + " -> + " + ((int)(((1 + ((float)brain_linking.c_counter / 10)) * (brain_linking.tris_cells.Count * 10)) * (1 + ((float)brain_linking.level / 10)))) + " Score";
            tm_array[2].text = "             + " + (brain_linking.tris_cells.Count * (((float)5 - brain_linking.level) / 2)) + " Time";
        }

        void Update()
        {
            if (timer_ani < 1.7f)
            {
                tm_array[0].color = new Color(tm_array[0].color.r, tm_array[0].color.g, tm_array[0].color.b, tm_array[0].color.a - (1 * Time.deltaTime));
                tm_array[1].color = new Color(tm_array[1].color.r, tm_array[1].color.g, tm_array[1].color.b, tm_array[1].color.a - (1 * Time.deltaTime));
                tm_array[2].color = new Color(tm_array[2].color.r, tm_array[2].color.g, tm_array[2].color.b, tm_array[2].color.a - (1 * Time.deltaTime));
                timer_ani += 1 * Time.deltaTime;
            }
            else
                Destroy(this.gameObject);

        }
    }
}
