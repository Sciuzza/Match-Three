using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MatchThree
{
    public class MThree_S : MonoBehaviour
    {

        

        Cell_S cell_linking_script_1, cell_linking_script_2, cell_linking_script_3;
        Block_S block_linking_script_1, block_linking_script_2;
        public List<GameObject> tris_cells = new List<GameObject>();
        public GameObject[,] cell_pointers = new GameObject[8, 10];
        public GameObject[,] blocks_pointers = new GameObject[8, 10];
        public enum game_phases { init, animation, sel_source, sel_dest };
        public game_phases current_gp;


        GameObject cell_linking_init, block_linking_init;
        int rows = 0;
        int cell_source_i, cell_source_j, cell_dest_i, cell_dest_j;
        public bool tris_dest = false, tris_source = false;
        public int count_right = 0, count_left = 0, count_top = 0, count_bot = 0, count_midh = 0, count_midv = 0;        
        List<int> color_possibilities = new List<int>();

        // Time
        public float time_start;
        public float time_end;

        void Awake()
        {

            time_start = Time.realtimeSinceStartup;

            for (int i = 0; i < cell_pointers.GetLength(0); i++)
            {
                for (int j = 0; j < cell_pointers.GetLength(1); j++)
                {
                    cell_linking_init = Resources.Load<GameObject>("Cell");
                    cell_linking_init = Instantiate(cell_linking_init);
                    cell_pointers[i, j] = cell_linking_init;
                    cell_linking_init.transform.position = new Vector3(j - 4.5f, i - 3.5f, 0);
                    cell_linking_init.name = "Cell" + i + "," + j;
                    cell_linking_script_1 = cell_linking_init.GetComponent<Cell_S>();
                    color_randomization();
                    cell_linking_script_1.cell_i = i;
                    cell_linking_script_1.cell_j = j;
                    fixing_tris(i, j);
                }
            }

            time_end = Time.realtimeSinceStartup;

            block_spawning_init();


        }

        public void Update()
        {
            if (current_gp == game_phases.init)
            {
                if (block_linking_init.transform.position.y < cell_pointers[7, 0].transform.position.y && rows < 7)
                {
                    rows++;
                    block_spawning_init();
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

        // Blocks Management

        public Color color_block_setting(int block_i_temp, int block_j_temp)
        {
            cell_linking_script_1 = cell_pointers[block_i_temp, block_j_temp].GetComponent<Cell_S>();
            return cell_linking_script_1.sr_array[0].color;
        }

        void block_spawning_init()
        {
            for (int j = 0; j < cell_pointers.GetLength(1); j++)
            {
                block_linking_init = Resources.Load<GameObject>("Block");
                block_linking_init = Instantiate<GameObject>(block_linking_init);
                blocks_pointers[rows, j] = block_linking_init;
                block_linking_init.transform.position = new Vector2(cell_pointers[rows, j].transform.position.x, 10);
                block_linking_script_1 = block_linking_init.GetComponent<Block_S>();
                block_linking_script_1.target_cell = cell_pointers[rows, j].transform;
                block_linking_script_1.block_i = rows;
                block_linking_script_1.block_j = j;
            }

            if (rows == 7)
                current_gp = game_phases.sel_source;
        }


        //Selection Source methods

        public void selection_visibility_source(int cell_i_temp, int cell_j_temp)
        {
            block_linking_script_1 = blocks_pointers[cell_i_temp, cell_j_temp].GetComponent<Block_S>();
            block_linking_script_1.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 255);
            cell_source_i = cell_i_temp;
            cell_source_j = cell_j_temp;
            current_gp = game_phases.sel_dest;
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
                return true;

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

        public void selection_visibility_dest(int cell_i_temp, int cell_j_temp)
        {
            block_linking_script_1 = blocks_pointers[cell_i_temp, cell_j_temp].GetComponent<Block_S>();
            block_linking_script_1.sr_array[1].color = Color.Lerp(block_linking_script_1.sr_array[1].color, new Color(0, 0, 0, 255), 1);
            cell_dest_i = cell_i_temp;
            cell_dest_j = cell_j_temp;
            current_gp = game_phases.animation;
        }

        public void animation_swap()
        {
            block_linking_script_1 = blocks_pointers[cell_source_i, cell_source_j].GetComponent<Block_S>();
            block_linking_script_2 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();

            block_linking_script_1.target_cell = cell_pointers[cell_dest_i, cell_dest_j].transform;
            block_linking_script_1.block_i = cell_source_i;
            block_linking_script_1.block_j = cell_source_j;

            block_linking_script_2.target_cell = cell_pointers[cell_source_i, cell_source_j].transform;
            block_linking_script_2.block_i = cell_dest_i;
            block_linking_script_2.block_j = cell_dest_j;

            block_linking_init = blocks_pointers[cell_source_i, cell_source_j];
            blocks_pointers[cell_source_i, cell_source_j] = blocks_pointers[cell_dest_i, cell_dest_j];
            blocks_pointers[cell_dest_i, cell_dest_j] = block_linking_init;
        }

        public void deselecting()
        {
            block_linking_script_1 = blocks_pointers[cell_source_i, cell_source_j].GetComponent<Block_S>();
            block_linking_script_2 = blocks_pointers[cell_dest_i, cell_dest_j].GetComponent<Block_S>();

            block_linking_script_1.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 0);
            block_linking_script_2.sr_array[1].color = new Color(block_linking_script_1.sr_array[1].color.r, block_linking_script_1.sr_array[1].color.g, block_linking_script_1.sr_array[1].color.b, 0);
        }

        public void destroying_tris()
        {
            for (int i = 0; i < tris_cells.Count; i++)
            {
                cell_linking_script_1 = tris_cells[i].GetComponent<Cell_S>();
                Destroy(blocks_pointers[cell_linking_script_1.cell_i, cell_linking_script_1.cell_j]);
                cell_linking_script_1.sr_array[0].color = Color.white;
            }
        }



    }
}



/*
                

            

                void randomization_fix_alghoritm_part_2()
                {
                    for (int i = 0; i < cell_pointers.GetLength(0); i++)
                    {
                        for (int j = 0; j < cell_pointers.GetLength(1); j++)
                        {
                            cell_linking_script_1 = cell_pointers[i, j].GetComponent<Cell_S>();
                            if (cell_linking_script_1.counter_tris_init > 0)
                                tris_cell_list.Add(cell_linking_script_1);
                        }
                    }

                    tris_cell_list.Sort(compare_cell_by_tris_counter);
                }

                int compare_cell_by_tris_counter(Cell_S first_cell_scr, Cell_S second_cell_scr)
                {
                    if (first_cell_scr.counter_tris_init > second_cell_scr.counter_tris_init)
                        return -1;
                    else if (first_cell_scr.counter_tris_init == second_cell_scr.counter_tris_init)
                        return 0;
                    else
                        return 1;
                }

                void randomization_fix_alghoritm_part_3()
                {
                    int cell_i_source = tris_cell_list[0].cell_i, cell_j_source = tris_cell_list[0].cell_j;

                    for (int i = 0; i < tris_cell_list.Count; i++)
                    {
                        exclude_color(tris_cell_list[i]);
                        if (i == 0)
                            color_randomization(tris_cell_list[i]);
                        else
                        {



                        }
                    }

                }



                void exclude_color(Cell_S cell_1, Cell_S cell_2)
                {
                    if (cell_2.cell_sr.color == Color.red)
                        cell_1.red_p = false;
                    else if (cell_2.cell_sr.color == Color.blue)
                        cell_1.blue_p = false;
                    else if (cell_2.cell_sr.color == Color.green)
                        cell_1.green_p = false;
                    else if (cell_2.cell_sr.color == Color.cyan)
                        cell_1.cyan_p = false;
                    else if (cell_2.cell_sr.color == Color.magenta)
                        cell_1.magenta_p = false;
                }

            */
