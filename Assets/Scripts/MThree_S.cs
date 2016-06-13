using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace MatchThree
{
    public class MThree_S : MonoBehaviour
    {

        // The two references matrix to manage cells and blocks all over the game phases
        public GameObject[,] cell_pointers = new GameObject[8, 10];
        public GameObject[,] blocks_pointers = new GameObject[8, 10];

        // Game phases 
        public enum game_phases { init, sel_source, sel_dest, animation, tris_p_animation, cyclyng_animation, waiting, game_over};
        public game_phases current_gp;

        //Score
        public int score = 0;

        //Timer
        public float time = 10;
        public bool updating_time = false;

        //Time to next level and level variables
        public float time_to_next = 45;
        public int level = 1;
        bool level_1_sound = false, level_2_sound = false, level_3_sound = false, level_4_sound = false;

        // First one is used in the initialization process only, the second one is used to update the blocks pointer matrix too 
        GameObject cell_linking_init, block_linking_init;

        //Variable used during the initialization by the block spawning phase
        int rows = 0;

        // Used for the Smart Randomization Color System
        List<int> color_possibilities = new List<int>();

        // Game over Boolean
        bool game_over = false;

        //Debugging Variables

        // Time
        public float time_start;
        public float time_end;

        //Boolean to check if player is simply switching the selection box or not
        public bool is_switching = false;

        // Variables to reset each sel source

        // Tris Direction Counting
        public int count_right = 0, count_left = 0, count_top = 0, count_bot = 0, count_midh = 0, count_midv = 0;

        //Cycling Counter
        public int c_counter = -1;

        //Boolean to proc the sound "Suffer" when below 10 seconds
        bool is_below_10 = false;

        //Cycling animation Boolean call
        bool tris_p_called = false;

        //Boolean to call tris_destroying only once
        bool tris_destroying_called = false;

        //Scripts references needed in almost all methods
        Cell_S cell_linking_script_1, cell_linking_script_2, cell_linking_script_3;
        Block_S block_linking_script_1, block_linking_script_2;
        Sound_S sound_linking_script;

        //Lists needed to save the cells to be destroyed and the columns to check for tris
        public List<GameObject> tris_cells = new List<GameObject>();
        public List<int> mg_columns = new List<int>();

        // variables needed to save the row and column position of the blocks/cells involved in the source and dest phases
        int cell_source_i, cell_source_j, cell_dest_i, cell_dest_j;

        //booleans needed to understand which cell is generating a tris during the swap
        public bool tris_dest = false, tris_source = false;



        void Awake()
        {

            time_start = Time.realtimeSinceStartup;

            sound_linking_script = GameObject.Find("Sound Controller").GetComponent<Sound_S>();


            //Here the invisible matrix will be generated
            for (int i = 0; i < cell_pointers.GetLength(0); i++)
            {
                for (int j = 0; j < cell_pointers.GetLength(1); j++)
                {
                    cell_linking_init = Resources.Load<GameObject>("Cell");
                    cell_linking_init = Instantiate(cell_linking_init);
                    cell_pointers[i, j] = cell_linking_init;
                    cell_linking_init.transform.position = new Vector3(j - 4.5f, i - 3.5f, cell_linking_init.transform.position.z);
                    cell_linking_init.name = "Cell" + i + "," + j;
                    cell_linking_script_1 = cell_linking_init.GetComponent<Cell_S>();
                    color_randomization();
                    cell_linking_script_1.cell_i = i;
                    cell_linking_script_1.cell_j = j;
                    fixing_tris(i, j);
                }
            }

            time_end = Time.realtimeSinceStartup;

            //Blocks will start spawning by top
            block_spawning_init();


        }

        public void Update()
        {

            //Sound level Management
            if (level == 1 && !level_1_sound)
            {
                sound_linking_script.play_environment(0);
                level_1_sound = true;
            }
            else if (level == 1 && time_to_next <= 0 && !level_2_sound)
            {
                sound_linking_script.play_environment(1);
                level_2_sound = true;
                level++;
                time_to_next = 60;
            }
            else if (level == 2 && time_to_next <= 0 && !level_3_sound)
            {
                sound_linking_script.play_environment(2);
                level_3_sound = true;
                level++;
                time_to_next = 90;
            }
            else if (level == 3 && time_to_next <= 0 && !level_4_sound)
            {
                sound_linking_script.play_environment(3);
                level_4_sound = true;
                time_to_next = 150;
            }

            //Suffer sound proc
            if (!is_below_10 && time <= 10)
            {
                suffer_sound();
                is_below_10 = true;
            }

            if (is_below_10 && time > 10)
                is_below_10 = false;


            //Game Over , to be implemented
            if (time <= 0 && !game_over)
            {
                game_over = true;
                current_gp = game_phases.game_over;
                if (level < 3)
                    sound_linking_script.play_environment(5);
                else
                    sound_linking_script.play_environment(4);

                Time.timeScale = 0;
            }
            // Will spawn another row when the current one is surpassing the top matrix row plus a value that will increase over time to deacrease the spawn time
            if (current_gp == game_phases.init && rows < 7)
            {
                if (block_linking_init.transform.position.y < (cell_pointers[7, 0].transform.position.y + rows / 2.3f) && rows < 7)
                {
                    rows++;
                    block_spawning_init();
                }
            }
            //When all rows are spawned will check if all blocks are in position, if the condition is true the player will be able to start playing
            else if (current_gp == game_phases.init && rows == 7)
            {
                if (blocks_in_position())
                    current_gp = game_phases.sel_source;
            }
            //
            else if (current_gp == game_phases.tris_p_animation)
            {
                if (!tris_destroying_called)
                {
                    destroying_tris();
                    tris_destroying_called = true;
                }
                else if (blocks_are_destroyed())
                    current_gp = game_phases.cyclyng_animation;
            }
            //Every time the game phase will be on cycling animation destroying tris will be called only once and no more because in the end the method will switch to waiting phase
            else if (current_gp == game_phases.cyclyng_animation)
            {
                tris_destroying_called = false;
                set_new_targets();
                current_gp = game_phases.waiting;
            }

            //In this phase the system will check every update if block are in position, when the condition is true will then check if tris have been generated, if the condition is true 
            //the game phase will be switched again to ciclyng animation, otherwise will be switched to sel source and player will be able to play
            else if (current_gp == game_phases.waiting)
            {
                if (blocks_in_position() && !tris_p_called)
                {
                    auto_tris_check();
                    tris_p_called = true;
                }



            }

        }



        // System Tris Fix 

        void color_randomization()
        {

            if (cell_linking_script_1.red_p)
                color_possibilities.Add(0);
            if (cell_linking_script_1.blue_p)
                color_possibilities.Add(1);
            if (cell_linking_script_1.green_p)
                color_possibilities.Add(2);
            if (cell_linking_script_1.cyan_p)
                color_possibilities.Add(3);
            if (cell_linking_script_1.magenta_p)
                color_possibilities.Add(4);

            switch (color_possibilities[Random.Range(0, color_possibilities.Count)])
            {
                case 0:
                    cell_linking_script_1.sr_array[0].color = Color.red;
                    break;

                case 1:
                    cell_linking_script_1.sr_array[0].color = Color.blue;
                    break;

                case 2:
                    cell_linking_script_1.sr_array[0].color = Color.green;
                    break;

                case 3:
                    cell_linking_script_1.sr_array[0].color = Color.cyan;
                    break;

                default:
                    cell_linking_script_1.sr_array[0].color = Color.magenta;
                    break;

            }
            color_possibilities.TrimExcess();
            color_possibilities.Clear();

        }

        void fixing_tris(int i_temp, int j_temp)
        {
            // LEft Tris Checking
            if (j_temp > 1)
            {
                cell_linking_script_2 = cell_pointers[i_temp, j_temp - 1].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[i_temp, j_temp - 2].GetComponent<Cell_S>();
                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color) && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    exclude_color();
                    color_randomization();
                }
                else if ((cell_linking_script_1.sr_array[0].color != cell_linking_script_2.sr_array[0].color) && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                    exclude_color_second_step_tris();

            }
            //Bottom Tris Checking
            if (i_temp > 1)
            {
                cell_linking_script_2 = cell_pointers[i_temp - 1, j_temp].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[i_temp - 2, j_temp].GetComponent<Cell_S>();
                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color) && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    exclude_color();
                    color_randomization();
                }
            }

            reset_boolean_color();
        }

        void exclude_color()
        {
            if (cell_linking_script_1.sr_array[0].color == Color.red)
                cell_linking_script_1.red_p = false;
            else if (cell_linking_script_1.sr_array[0].color == Color.blue)
                cell_linking_script_1.blue_p = false;
            else if (cell_linking_script_1.sr_array[0].color == Color.green)
                cell_linking_script_1.green_p = false;
            else if (cell_linking_script_1.sr_array[0].color == Color.cyan)
                cell_linking_script_1.cyan_p = false;
            else if (cell_linking_script_1.sr_array[0].color == Color.magenta)
                cell_linking_script_1.magenta_p = false;
        }

        void exclude_color_second_step_tris()
        {
            if (cell_linking_script_2.sr_array[0].color == Color.red)
                cell_linking_script_1.red_p = false;
            else if (cell_linking_script_2.sr_array[0].color == Color.blue)
                cell_linking_script_1.blue_p = false;
            else if (cell_linking_script_2.sr_array[0].color == Color.green)
                cell_linking_script_1.green_p = false;
            else if (cell_linking_script_2.sr_array[0].color == Color.cyan)
                cell_linking_script_1.cyan_p = false;
            else if (cell_linking_script_2.sr_array[0].color == Color.magenta)
                cell_linking_script_1.magenta_p = false;
        }

        void reset_boolean_color()
        {
            cell_linking_script_1.red_p = true;
            cell_linking_script_1.blue_p = true;
            cell_linking_script_1.green_p = true;
            cell_linking_script_1.cyan_p = true;
            cell_linking_script_1.magenta_p = true;

        }


        //Blocks Management methods

        public Sprite color_block_setting(int block_i_temp, int block_j_temp)
        {
            cell_linking_script_1 = cell_pointers[block_i_temp, block_j_temp].GetComponent<Cell_S>();
            Sprite block_sprite;

            if (cell_linking_script_1.sr_array[0].color == Color.red)
            {
                block_sprite = Resources.Load<Sprite>("Red");
                return block_sprite;
            }
            else if (cell_linking_script_1.sr_array[0].color == Color.blue)
            {
                block_sprite = Resources.Load<Sprite>("Blue");
                return block_sprite;
            }
            else if (cell_linking_script_1.sr_array[0].color == Color.green)
            {
                block_sprite = Resources.Load<Sprite>("Green");
                return block_sprite;
            }
            else if (cell_linking_script_1.sr_array[0].color == Color.cyan)
            {
                block_sprite = Resources.Load<Sprite>("Cyan");
                return block_sprite;
            }
            else
            {
                block_sprite = Resources.Load<Sprite>("Magenta");
                return block_sprite;
            }


        }

        void block_spawning_init()
        {
            for (int j = 0; j < cell_pointers.GetLength(1); j++)
            {
                block_linking_init = Resources.Load<GameObject>("Block");
                block_linking_init = Instantiate<GameObject>(block_linking_init);
                blocks_pointers[rows, j] = block_linking_init;
                block_linking_init.transform.position = new Vector3(cell_pointers[rows, j].transform.position.x, 10, block_linking_init.transform.position.z);
                block_linking_script_1 = block_linking_init.GetComponent<Block_S>();
                block_linking_script_1.target_cell = cell_pointers[rows, j].transform;
                block_linking_script_1.block_i = rows;
                block_linking_script_1.block_j = j;
            }

        }


        //Selection Source methods

        public void reset_all()
        {
            cell_linking_script_1 = null;
            cell_linking_script_2 = null;
            cell_linking_script_3 = null;
            block_linking_script_1 = null;
            block_linking_script_2 = null;
            tris_cells.Clear();
            tris_cells.TrimExcess();
            mg_columns.Clear();
            mg_columns.TrimExcess();
            cell_source_i = -1;
            cell_source_j = -1;
            cell_dest_i = -1;
            cell_dest_j = -1;
            tris_dest = false;
            tris_source = false;
            count_bot = 0;
            count_left = 0;
            count_midh = 0;
            count_midv = 0;
            count_right = 0;
            count_top = 0;
        }

        public void selection_visibility_source(int cell_i_temp, int cell_j_temp)
        {
            cell_dest_j = -1;
            cell_dest_i = -1;

            block_linking_script_1 = blocks_pointers[cell_i_temp, cell_j_temp].GetComponent<Block_S>();
            block_linking_script_1.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 255);
            cell_source_i = cell_i_temp;
            cell_source_j = cell_j_temp;
        }


        // Selection Dest Methods

        public bool is_next_to(int cell_i_temp, int cell_j_temp)
        {
            if (cell_i_temp == cell_source_i)
            {
                if ((cell_j_temp == cell_source_j + 1) || (cell_j_temp == cell_source_j - 1))
                    return true;
                else
                    return false;
            }
            else if (cell_j_temp == cell_source_j)
            {
                if ((cell_i_temp == cell_source_i + 1) || (cell_i_temp == cell_source_i - 1))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool is_sel_generating_tris(int cell_i_temp, int cell_j_temp)
        {
            cell_dest_i = cell_i_temp;
            cell_dest_j = cell_j_temp;

            cell_linking_script_1 = cell_pointers[cell_dest_i, cell_dest_j].GetComponent<Cell_S>();
            cell_linking_script_2 = cell_pointers[cell_source_i, cell_source_j].GetComponent<Cell_S>();

            Color color_temp = cell_linking_script_1.sr_array[0].color;

            cell_linking_script_1.sr_array[0].color = cell_linking_script_2.sr_array[0].color;
            cell_linking_script_2.sr_array[0].color = color_temp;

            tris_dest = tris_checking(cell_dest_i, cell_dest_j);
            tris_source = tris_checking(cell_source_i, cell_source_j);

            if (!tris_dest && !tris_source)
            {
                cell_linking_script_1 = cell_pointers[cell_dest_i, cell_dest_j].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_source_i, cell_source_j].GetComponent<Cell_S>();
                color_temp = cell_linking_script_1.sr_array[0].color;
                cell_linking_script_1.sr_array[0].color = cell_linking_script_2.sr_array[0].color;
                cell_linking_script_2.sr_array[0].color = color_temp;
                return false;
            }
            else
            {
                return true;
            }
        }

        bool tris_checking(int cell_i_temp, int cell_j_temp)
        {

            bool tris_found = false;
            bool tris_longer = true;
            bool right_tris_found = false, top_tris_found = false, left_tris_found = false, bottom_tris_found = false;
            int i_searching = 0, j_searching = 0;

            if (cell_j_temp < cell_pointers.GetLength(1) - 2)
            {
                //Right tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp, cell_j_temp + 1].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp, cell_j_temp + 2].GetComponent<Cell_S>();

                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
            && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_right++;
                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {
                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp + 1]);
                    tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp + 2]);

                    right_tris_found = true;
                    j_searching = 3;

                    while ((cell_j_temp + j_searching) < cell_pointers.GetLength(1) && tris_longer)
                    {
                        cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp + j_searching].GetComponent<Cell_S>();
                        if (cell_linking_script_1.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                        {
                            tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp + j_searching]);
                        }
                        else
                            tris_longer = false;
                        j_searching++;
                    }

                    tris_longer = true;
                    tris_found = true;
                }

            }
            if (cell_j_temp >= 2)
            {
                //Left Tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp, cell_j_temp - 1].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp, cell_j_temp - 2].GetComponent<Cell_S>();



                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
           && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_left++;
                    left_tris_found = true;

                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {

                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp - 1]);
                    tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp - 2]);
                    j_searching = -3;

                    while ((cell_j_temp + j_searching) > 0 && tris_longer)
                    {
                        cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp + j_searching].GetComponent<Cell_S>();
                        if (cell_linking_script_1.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                        {
                            tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp + j_searching]);
                        }
                        else
                            tris_longer = false;
                        j_searching--;
                    }

                    tris_longer = true;
                    tris_found = true;
                }

            }
            if (cell_i_temp < cell_pointers.GetLength(0) - 2)
            {
                //Top Tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp + 1, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp + 2, cell_j_temp].GetComponent<Cell_S>();



                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
           && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_top++;
                    top_tris_found = true;

                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {

                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    tris_cells.Add(cell_pointers[cell_i_temp + 1, cell_j_temp]);
                    tris_cells.Add(cell_pointers[cell_i_temp + 2, cell_j_temp]);

                    i_searching = 3;

                    while ((cell_i_temp + i_searching) < cell_pointers.GetLength(0) && tris_longer)
                    {
                        cell_linking_script_1 = cell_pointers[cell_i_temp + i_searching, cell_j_temp].GetComponent<Cell_S>();
                        if (cell_linking_script_1.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                        {
                            tris_cells.Add(cell_pointers[cell_i_temp + i_searching, cell_j_temp]);
                        }
                        else
                            tris_longer = false;
                        i_searching++;
                    }

                    tris_longer = true;
                    tris_found = true;
                }

            }
            if (cell_i_temp >= 2)
            {
                //Bottom Tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp - 1, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp - 2, cell_j_temp].GetComponent<Cell_S>();



                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
          && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_bot++;
                    bottom_tris_found = true;

                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {

                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    tris_cells.Add(cell_pointers[cell_i_temp - 1, cell_j_temp]);
                    tris_cells.Add(cell_pointers[cell_i_temp - 2, cell_j_temp]);

                    i_searching = -3;

                    while ((cell_i_temp + i_searching) > 0 && tris_longer)
                    {
                        cell_linking_script_1 = cell_pointers[cell_i_temp + i_searching, cell_j_temp].GetComponent<Cell_S>();
                        if (cell_linking_script_1.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                        {
                            tris_cells.Add(cell_pointers[cell_i_temp + i_searching, cell_j_temp]);
                        }
                        else
                            tris_longer = false;
                        i_searching--;
                    }

                    tris_longer = true;
                    tris_found = true;
                }
            }
            if ((cell_j_temp >= 1) && (cell_j_temp < cell_pointers.GetLength(1) - 1))
            {
                //Mid Horizontal Tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp, cell_j_temp - 1].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp, cell_j_temp + 1].GetComponent<Cell_S>();



                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
          && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_midh++;
                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {

                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    if (!left_tris_found)
                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp - 1]);
                    if (!right_tris_found)
                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp + 1]);



                    tris_found = true;
                }
            }
            if ((cell_i_temp >= 1) && (cell_i_temp < cell_pointers.GetLength(0) - 1))
            {
                //Mid Vertical Tris Checking
                cell_linking_script_1 = cell_pointers[cell_i_temp, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_2 = cell_pointers[cell_i_temp - 1, cell_j_temp].GetComponent<Cell_S>();
                cell_linking_script_3 = cell_pointers[cell_i_temp + 1, cell_j_temp].GetComponent<Cell_S>();



                if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
           && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                {
                    count_midv++;
                    if (!tris_cells.Contains(cell_pointers[cell_i_temp, cell_j_temp]))
                    {

                        tris_cells.Add(cell_pointers[cell_i_temp, cell_j_temp]);
                    }
                    if (!bottom_tris_found)
                        tris_cells.Add(cell_pointers[cell_i_temp - 1, cell_j_temp]);
                    if (!top_tris_found)
                        tris_cells.Add(cell_pointers[cell_i_temp + 1, cell_j_temp]);

                    tris_found = true;
                }
            }

            return tris_found;

        }


        //Animations Methods

        public void animation_swap()
        {

            //Taking the two blocks involved in the swap
            block_linking_script_1 = blocks_pointers[cell_source_i, cell_source_j].GetComponent<Block_S>();
            block_linking_script_2 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();

            // Moving Blocks to their new cells and changing their identifiers 
            block_linking_script_1.target_cell = cell_pointers[cell_dest_i, cell_dest_j].transform;
            block_linking_script_1.block_i = cell_dest_i;
            block_linking_script_1.block_j = cell_dest_j;

            block_linking_script_2.target_cell = cell_pointers[cell_source_i, cell_source_j].transform;
            block_linking_script_2.block_i = cell_source_i;
            block_linking_script_2.block_j = cell_source_j;

            //Changing their linkings so the the game controller knows their new name too
            block_linking_init = blocks_pointers[cell_source_i, cell_source_j];
            blocks_pointers[cell_source_i, cell_source_j] = blocks_pointers[cell_dest_i, cell_dest_j];
            blocks_pointers[cell_dest_i, cell_dest_j] = block_linking_init;

            //Changing the names of the blocks accordingly in Unity
            blocks_pointers[cell_source_i, cell_source_j].name = "Block " + block_linking_script_2.block_i + "," + block_linking_script_2.block_j;
            blocks_pointers[cell_dest_i, cell_dest_j].name = "Block " + block_linking_script_1.block_i + "," + block_linking_script_1.block_j;
        }

        public void reverse_swap()
        {

            int cell_i_temp, cell_j_temp;

            cell_i_temp = cell_source_i;
            cell_j_temp = cell_source_j;

            cell_source_i = cell_dest_i;
            cell_source_j = cell_dest_j;

            cell_dest_i = cell_i_temp;
            cell_dest_j = cell_j_temp;

            //Taking the two blocks involved in the swap
            block_linking_script_1 = blocks_pointers[cell_source_i, cell_source_j].GetComponent<Block_S>();
            block_linking_script_2 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();

            // Moving Blocks to their new cells and changing their identifiers 
            block_linking_script_1.target_cell = cell_pointers[cell_dest_i, cell_dest_j].transform;
            block_linking_script_1.block_i = cell_dest_i;
            block_linking_script_1.block_j = cell_dest_j;

            block_linking_script_2.target_cell = cell_pointers[cell_source_i, cell_source_j].transform;
            block_linking_script_2.block_i = cell_source_i;
            block_linking_script_2.block_j = cell_source_j;

            //Changing their linkings so the the game controller knows their new name too
            block_linking_init = blocks_pointers[cell_source_i, cell_source_j];
            blocks_pointers[cell_source_i, cell_source_j] = blocks_pointers[cell_dest_i, cell_dest_j];
            blocks_pointers[cell_dest_i, cell_dest_j] = block_linking_init;

            //Changing the names of the blocks accordingly in Unity
            blocks_pointers[cell_source_i, cell_source_j].name = "Block " + block_linking_script_2.block_i + "," + block_linking_script_2.block_j;
            blocks_pointers[cell_dest_i, cell_dest_j].name = "Block " + block_linking_script_1.block_i + "," + block_linking_script_1.block_j;

            cell_source_i = cell_i_temp;
            cell_source_j = cell_j_temp;

            cell_dest_i = -1;
            cell_dest_j = -1;

            current_gp = game_phases.sel_dest;
        }

        public void deselecting()
        {
            if (!is_switching)
            {
                if (tris_dest || (!tris_dest && !tris_source))
                {
                    block_linking_script_1 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();
                    block_linking_script_1.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 0);
                }
                else if (tris_source)
                {
                    block_linking_script_2 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();
                    block_linking_script_2.sr_array[1].color = new Color(block_linking_script_2.sr_array[1].color.r, block_linking_script_2.sr_array[1].color.g, block_linking_script_2.sr_array[1].color.b, 0);
                }
                tris_p_called = true;
                Invoke("switch_to_tris_p_animation", 1);
            }
            else
            {
                block_linking_script_1 = blocks_pointers[cell_source_i, cell_source_j].GetComponent<Block_S>();
                block_linking_script_1.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 0);
            }
        }


        //Tris animation

        public void destroying_tris()
        {
            c_counter++;
            score +=(int)(((1 + ((float)c_counter / 10)) * (tris_cells.Count * 10)) * (1 + ((float)level / 10)));
            updating_time = true;
            time += tris_cells.Count * (((float)5 - level) / 3);
            updating_time = false;

            sound_linking_script.play_interaction(9);

            for (int i = 0; i < tris_cells.Count; i++)
            {
                cell_linking_script_1 = tris_cells[i].GetComponent<Cell_S>();
                block_linking_script_1 = blocks_pointers[cell_linking_script_1.cell_i, cell_linking_script_1.cell_j].GetComponent<Block_S>();
                block_linking_script_1.ps_link.Play();
                block_linking_script_1.is_dying = true;
                cell_linking_script_1.sr_array[0].color = Color.white;
            }
            generating_mg_columns();
        }

        void generating_mg_columns()
        {
            mg_columns.Clear();
            mg_columns.TrimExcess();

            for (int i = 0; i < tris_cells.Count; i++)
            {
                cell_linking_script_1 = tris_cells[i].GetComponent<Cell_S>();
                if (!mg_columns.Contains(cell_linking_script_1.cell_j))
                    mg_columns.Add(cell_linking_script_1.cell_j);
            }
            current_gp = game_phases.tris_p_animation;
        }

        bool blocks_are_destroyed()
        {
            Cell_S temp_cell_linking;

            for (int i = 0; i < tris_cells.Count; i++)
            {
                temp_cell_linking = tris_cells[i].GetComponent<Cell_S>();
                if (blocks_pointers[temp_cell_linking.cell_i, temp_cell_linking.cell_j] != null)
                    return false;

            }
            return true;
        }

        //Cycling animation methods : the first one will call the next in cascade, the last will set in the end the game phase to waiting

        void set_new_targets()
        {
            int k = 0;
            bool found;


            while (k < mg_columns.Count)
            {
                for (int i = 1; i < cell_pointers.GetLength(0); i++)
                {
                    cell_linking_script_1 = cell_pointers[i - 1, mg_columns[k]].GetComponent<Cell_S>();
                    if (cell_linking_script_1.sr_array[0].color == Color.white)
                    {
                        found = false;
                        for (int searching = i; !found && searching < cell_pointers.GetLength(0); searching++)
                        {
                            cell_linking_script_2 = cell_pointers[searching, mg_columns[k]].GetComponent<Cell_S>();
                            if (cell_linking_script_2.sr_array[0].color != Color.white)
                            {
                                found = true;

                                cell_linking_script_1.sr_array[0].color = cell_linking_script_2.sr_array[0].color;
                                cell_linking_script_2.sr_array[0].color = Color.white;

                                blocks_pointers[i - 1, mg_columns[k]] = blocks_pointers[searching, mg_columns[k]];
                                blocks_pointers[searching, mg_columns[k]] = null;
                                blocks_pointers[i - 1, mg_columns[k]].name = "Block" + (i - 1) + "," + mg_columns[k];

                                block_linking_script_1 = blocks_pointers[i - 1, mg_columns[k]].GetComponent<Block_S>();
                                block_linking_script_1.target_cell = cell_pointers[i - 1, mg_columns[k]].transform;
                                block_linking_script_1.block_i = i - 1;
                                block_linking_script_1.block_j = mg_columns[k];
                            }
                        }
                    }
                }
                k++;
            }


            falling_blocks_generation();
        }

        void falling_blocks_generation()
        {
            int k = 0, white_counter;
            bool no_more_white = false;


            while (k < mg_columns.Count)
            {
                white_counter = 0;
                no_more_white = false;
                for (int i = cell_pointers.GetLength(0) - 1; i >= 0 && !no_more_white; i--)
                {
                    cell_linking_script_1 = cell_pointers[i, mg_columns[k]].GetComponent<Cell_S>();
                    if (cell_linking_script_1.sr_array[0].color == Color.white)
                        white_counter++;
                    else
                        no_more_white = true;
                }

                for (int j = 0; j < white_counter; j++)
                {
                    cell_linking_script_1 = cell_pointers[cell_pointers.GetLength(0) - white_counter + j, mg_columns[k]].GetComponent<Cell_S>();
                    color_randomization();

                    block_linking_init = Resources.Load<GameObject>("Block");
                    block_linking_init = Instantiate<GameObject>(block_linking_init);
                    blocks_pointers[cell_pointers.GetLength(0) - white_counter + j, mg_columns[k]] = block_linking_init;
                    block_linking_init.transform.position = new Vector2(cell_pointers[0, mg_columns[k]].transform.position.x, 10 + j);

                    block_linking_script_1 = block_linking_init.GetComponent<Block_S>();
                    block_linking_script_1.target_cell = cell_pointers[cell_pointers.GetLength(0) - white_counter + j, mg_columns[k]].transform;
                    block_linking_script_1.block_i = cell_pointers.GetLength(0) - white_counter + j;
                    block_linking_script_1.block_j = mg_columns[k];
                }

                k++;
            }

        }


        //Waiting Methods : First one will be checked every update til it becomes true
        //when it becomes true will be checked the true condition of the second one: if it will be true the game phase will be set to cycling animation, otherwise will be set to sel_source 
        bool blocks_in_position()
        {
            bool in_position = true;


            for (int i = 0; i < cell_pointers.GetLength(0) && in_position; i++)
            {
                for (int j = 0; j < cell_pointers.GetLength(1) && in_position; j++)
                {
                    block_linking_script_1 = blocks_pointers[i, j].GetComponent<Block_S>();

                    if (blocks_pointers[i, j].transform.position.y != block_linking_script_1.target_cell.position.y)
                        in_position = false;
                }
            }

            return in_position;
        }

        public void auto_tris_check()
        {
            int k = 0;
            tris_cells.Clear();
            tris_cells.TrimExcess();
            bool tris_found = false;
            bool tris_longer = true;
            bool right_tris_found = false, top_tris_found = false, left_tris_found = false, bottom_tris_found = false;
            int i_searching = 0, j_searching = 0;


            while (k < mg_columns.Count)
            {
                for (int i = 0; i < cell_pointers.GetLength(0); i++)
                {
                    cell_linking_script_1 = cell_pointers[i, mg_columns[k]].GetComponent<Cell_S>();




                    if (mg_columns[k] < cell_pointers.GetLength(1) - 2)
                    {
                        //Right tris Checking
                        cell_linking_script_2 = cell_pointers[i, mg_columns[k] + 1].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i, mg_columns[k] + 2].GetComponent<Cell_S>();

                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                    && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {


                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] + 1]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] + 1]);
                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] + 2]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] + 2]);

                            //  cell_linking_script_2.tris_checked = true;
                            //  cell_linking_script_3.tris_checked = true;
                            right_tris_found = true;
                            j_searching = 3;

                            while ((mg_columns[k] + j_searching) < cell_pointers.GetLength(1) && tris_longer)
                            {
                                cell_linking_script_2 = cell_pointers[i, mg_columns[k] + j_searching].GetComponent<Cell_S>();
                                if (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                                {
                                    if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] + j_searching]))
                                        tris_cells.Add(cell_pointers[i, mg_columns[k] + j_searching]);
                                }
                                else
                                    tris_longer = false;
                                j_searching++;
                            }

                            tris_longer = true;
                            tris_found = true;
                        }

                    }
                    if (mg_columns[k] >= 2)
                    {
                        //Left Tris Checking
                        cell_linking_script_2 = cell_pointers[i, mg_columns[k] - 1].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i, mg_columns[k] - 2].GetComponent<Cell_S>();



                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                   && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {

                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] - 1]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] - 1]);
                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] - 2]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] - 2]);

                            //  cell_linking_script_2.tris_checked = true;
                            //  cell_linking_script_3.tris_checked = true;
                            left_tris_found = true;
                            j_searching = -3;

                            while ((mg_columns[k] + j_searching) > 0 && tris_longer)
                            {
                                cell_linking_script_2 = cell_pointers[i, mg_columns[k] + j_searching].GetComponent<Cell_S>();
                                if (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                                {
                                    if (!tris_cells.Contains(cell_pointers[i, mg_columns[k] + j_searching]))
                                        tris_cells.Add(cell_pointers[i, mg_columns[k] + j_searching]);
                                }
                                else
                                    tris_longer = false;
                                j_searching--;
                            }

                            tris_longer = true;
                            tris_found = true;
                        }

                    }
                    if (i < cell_pointers.GetLength(0) - 2)
                    {
                        //Top Tris Checking
                        cell_linking_script_2 = cell_pointers[i + 1, mg_columns[k]].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i + 2, mg_columns[k]].GetComponent<Cell_S>();



                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                   && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {

                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i + 1, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i + 1, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i + 2, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i + 2, mg_columns[k]]);

                            // cell_linking_script_2.tris_checked = true;
                            // cell_linking_script_3.tris_checked = true;
                            top_tris_found = true;
                            i_searching = 3;

                            while ((i + i_searching) < cell_pointers.GetLength(0) && tris_longer)
                            {
                                cell_linking_script_2 = cell_pointers[i + i_searching, mg_columns[k]].GetComponent<Cell_S>();
                                if (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                                {
                                    if (!tris_cells.Contains(cell_pointers[i + i_searching, mg_columns[k]]))
                                        tris_cells.Add(cell_pointers[i + i_searching, mg_columns[k]]);
                                }
                                else
                                    tris_longer = false;
                                i_searching++;
                            }

                            tris_longer = true;
                            tris_found = true;
                        }

                    }
                    if (i >= 2)
                    {
                        //Bottom Tris Checking
                        cell_linking_script_2 = cell_pointers[i - 1, mg_columns[k]].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i - 2, mg_columns[k]].GetComponent<Cell_S>();



                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                  && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {

                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i - 1, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i - 1, mg_columns[k]]);
                            if (!tris_cells.Contains(cell_pointers[i - 2, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i - 2, mg_columns[k]]);

                            // cell_linking_script_2.tris_checked = true;
                            //  cell_linking_script_3.tris_checked = true;
                            bottom_tris_found = true;
                            i_searching = -3;

                            while ((i + i_searching) > 0 && tris_longer)
                            {
                                cell_linking_script_2 = cell_pointers[i + i_searching, mg_columns[k]].GetComponent<Cell_S>();
                                if (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color)
                                {
                                    if (!tris_cells.Contains(cell_pointers[i + i_searching, mg_columns[k]]))
                                        tris_cells.Add(cell_pointers[i + i_searching, mg_columns[k]]);
                                }
                                else
                                    tris_longer = false;
                                i_searching--;
                            }

                            tris_longer = true;
                            tris_found = true;
                        }
                    }
                    if ((mg_columns[k] >= 1) && (mg_columns[k] < cell_pointers.GetLength(1) - 1))
                    {
                        //Mid Horizontal Tris Checking
                        cell_linking_script_2 = cell_pointers[i, mg_columns[k] - 1].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i, mg_columns[k] + 1].GetComponent<Cell_S>();



                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                  && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {

                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!left_tris_found && !tris_cells.Contains(cell_pointers[i, mg_columns[k] - 1]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] - 1]);
                            if (!right_tris_found && !tris_cells.Contains(cell_pointers[i, mg_columns[k] + 1]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k] + 1]);

                            // cell_linking_script_2.tris_checked = true;
                            // cell_linking_script_3.tris_checked = true;

                            tris_found = true;
                        }
                    }
                    if ((i >= 1) && (i < cell_pointers.GetLength(0) - 1))
                    {
                        //Mid Vertical Tris Checking
                        cell_linking_script_2 = cell_pointers[i - 1, mg_columns[k]].GetComponent<Cell_S>();
                        cell_linking_script_3 = cell_pointers[i + 1, mg_columns[k]].GetComponent<Cell_S>();



                        if ((cell_linking_script_1.sr_array[0].color == cell_linking_script_2.sr_array[0].color)
                   && (cell_linking_script_2.sr_array[0].color == cell_linking_script_3.sr_array[0].color))
                        {

                            if (!tris_cells.Contains(cell_pointers[i, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i, mg_columns[k]]);
                            if (!bottom_tris_found && !tris_cells.Contains(cell_pointers[i - 1, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i - 1, mg_columns[k]]);
                            if (!top_tris_found && !tris_cells.Contains(cell_pointers[i + 1, mg_columns[k]]))
                                tris_cells.Add(cell_pointers[i + 1, mg_columns[k]]);

                            // cell_linking_script_2.tris_checked = true;
                            // cell_linking_script_3.tris_checked = true;

                            tris_found = true;
                        }
                    }
                }
                k++;
            }
            //reset_tris_checked_boolean();

            if (!tris_found)
            {
                c_counter = -1;
                current_gp = game_phases.sel_source;
            }
            else
            {
                Invoke("switch_to_tris_p_animation", 1);
            }


        }

        //Sound Methods

        void suffer_sound()
        {
            sound_linking_script.play_special(0);
        }


        //Switch to cyclying

        void switch_to_tris_p_animation()
        {
            current_gp = game_phases.tris_p_animation;
            tris_p_called = false;
        }

    }
}