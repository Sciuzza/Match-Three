using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Score_in_place_S : MonoBehaviour
    {
        TextMesh opacity;
        float timer_ani = 0;
        MThree_S brain_linking;

        void Awake()
        {
            opacity = this.GetComponent<TextMesh>();
            brain_linking = GameObject.Find("MatchFreeBrain").GetComponent<MThree_S>();
        }

        void Start()
        {
            opacity.text = "+ " + ((int)(((1 + ((float)brain_linking.c_counter / 10)) * 10 * (1 + ((float)brain_linking.level / 10)))));
        }

        void Update()
        {
            if (timer_ani < 1)
            {
                opacity.color = new Color(opacity.color.r, opacity.color.g, opacity.color.b, opacity.color.a - (1 * Time.deltaTime));
                timer_ani += 1 * Time.deltaTime;
            }
            else
                Destroy(this.gameObject);

        }
    }
}
