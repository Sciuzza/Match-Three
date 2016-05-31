using UnityEngine;
using System.Collections;

namespace MatchThree
{
    public class Cell_S : MonoBehaviour
    {
        public SpriteRenderer sr;
        // define the enumerator for cells color
        public enum block { red, blue, green, cyan, magenta };

        // get and initialize sprite renderer componet
        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            // call the enumerator and get randomize the starting color
            switch ((block)(Random.Range(0, 5)))
            {
                case block.red:
                    sr.color = Color.red;
                    break;

                case block.blue:
                    sr.color = Color.blue;
                    break;

                case block.green:
                    sr.color = Color.green;
                    break;

                case block.cyan:
                    sr.color = Color.cyan;
                    break;

                case block.magenta:
                    sr.color = Color.magenta;
                    break;
            }
        }
    }
}