using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinimport_TUI
{
    internal class Program
    {
        static string[][] text_fields = new string[3][];
        static string[][] text_inputs = new string[8][];
        static int[,] field_size = new int[3, 2];
        static int[,] pos_of_inputs = new int[8, 2];
        static int current_windowwidth = 0;
        static int current_windowheight = 0;
        static int[] default_cursor_pos = { 0, 0 };
        static string bottom_bar;

        static void err_msg(string msg, int which = 1)
        {
            Console.WriteLine("ERROR: {1}", msg );
            Environment.Exit(which);
        }
        static void set_and_write(int where, string[] what)
            //TODO: text wrapping
        {
            text_inputs[where] = what; //for regeneraton af ui
            for (int n = 0; n < what.Length; ++n)
            {
                Console.SetCursorPosition(pos_of_inputs[where, 0] + 1, pos_of_inputs[where, 1] + n);
                Console.Write(what[n]);
            }
            Console.SetCursorPosition(default_cursor_pos[0], default_cursor_pos[1]);
        }
        static void input_fields(string where, string[] what)
        {
            switch (where)
            {
                case "temp_og_fugt_lager":
                    set_and_write(0, what);
                    break;
                case "temp_og_fugt_udenfor":
                    set_and_write(1, what);
                    break;
                case "date_kobenhavn":
                    set_and_write(2, what);
                    break;
                case "date_london":
                    set_and_write(3, what);
                    break;
                case "date_singapore":
                    set_and_write(4, what);
                    break;
                case "lager_min":
                    set_and_write(5, what);
                    break;
                case "lager_max":
                    set_and_write(6, what);
                    break;
                case "lager_mest":
                    set_and_write(7, what);
                    break;
                default:
                    err_msg("No such action exists.");
                    break;
            }
        }
        static int odd_number(int input)
        {
            if (input % 2 == 0)
                return input + 1;
            return input;
        }
        static bool odd_numbered_window_size()
        // Den del skal gør sikker, at terminal vindue er altid symmetrisk. Fordi vi har brug for 1 linje, som dividere vores app i halv, 2 gange, vi har brug for "odd" nummere.
        {

            if (Math.Abs(odd_number(Console.WindowWidth) - current_windowwidth) < 1 || Math.Abs(odd_number(Console.WindowWidth) - current_windowwidth) < 1)
            {
                Console.WindowWidth = odd_number(Console.WindowWidth);
                current_windowwidth = Console.WindowWidth;
                Console.WindowHeight = odd_number(Console.WindowHeight);
                current_windowheight = Console.WindowHeight;
                return false;
            }
            else if (Console.WindowWidth % 2 == 0)
            {
                Console.WindowWidth = odd_number(Console.WindowWidth);
            }
            else if (Console.WindowHeight % 2 == 0)
            {
                Console.WindowHeight = odd_number(Console.WindowHeight);
            }
            return true;
        }
        static void generate_ui()
        {

            if (odd_numbered_window_size())
                return; //Hvis størrelsen er ikke meget forskellig, fortsæt ikke. Ellers generere nyt layout.

            //resten af kode
            Console.Clear();
            Console.SetCursorPosition(0, 0);


        }
        static void Main(string[] args)
        {
            //"sep" står for separation, som ligner sådan: "----------------------"
            text_fields[0] = new string[] { "Temperatur og fugtighed", "Lager:", "Udenfor:" };
            text_fields[1] = new string[] { "Dato / tid", "København:", "London:", "Singapore:" };
            text_fields[2] = new string[] { "Lagerstatus", "Varer under minimum", "sep", "Varer over maksimum", "sep", "Mest solgte i dag", "sep" };
            bottom_bar = "Skulpturen Favntag er ikke at finde på den faste plads, og det satte spekulationer";
            generate_ui();


            while (true)
            {
                if (current_windowwidth != Console.WindowHeight || current_windowheight != Console.WindowWidth)
                    generate_ui(); //regeneration af ui

                /* API:
                Du skal bruge "input_fields(where, what)" til at få ting på skærmen,
                "what" er string array, som bevæge cursor til specifiske plads og skrive hvad der er i array.
                "where" referere til en switch statement, som tjekke, om en af de følgene tekstfælde passer, som du kende som:
                "temp_og_fugt_lager"    = "Lager:"
                "temp_og_fugt_udenfor"  = "Udenfor:"
                "date_kobenhavn"        = "København:"
                "date_london"           = "London:"
                "date_singapore"        = "Singapore:"
                "lager_min"             = "Varer under minimum"
                "lager_max"             = "Varer over maksimum"
                "lager_mest"            = "Mest solgte i dag"

                VÆR OPMARKSOM: input_fields tjekke ikke for hvor ofte sender du requester til API

                */

            }
        }
    }
}
