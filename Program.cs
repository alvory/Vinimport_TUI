using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vinimport_TUI
{
    /*
     * Som koden er, den mangler disse vigtige funktionaliteter:
     * - API kode
     * Og disse ikke-så-vigtig funktionaliteter:
     * - Text wrapping i set_and_write funktion (Ikke vigtig. UI generation kode er vigtiger end det.)
     * - Magisk kode for offset af begyndelse af text_input linje i venstre sub-vindue ("Lagerstatus").
     * - fjern hacks i generate_ui() funktionen
     * Og selvfølgelig:
     * - Masse af testning
     */
    internal class Program
    {
        static string[][] text_fields = new string[3][];
        static string[][] text_inputs = new string[9][];
        static int[,] field_size = new int[3, 2]; // Mest højeste og venstreste punktet af field.
        static int[,] pos_of_inputs = new int[9, 2];
        static int current_windowwidth = 0;
        static int current_windowheight = 0;
        static int[] default_cursor_pos = { 0, 0 };
        static int bottom_bar_height = 2;

        static void err_msg(string msg, int which = 1)
        {
            Console.WriteLine("ERROR: " + msg );
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
        static int centered_text(int length, string what)
        {
            if (what.Length % 2 == 0)
                return (length - length % 2 - what.Length) / 2;
            else
                return (length - what.Length) / 2;
        }
        static void input_fields(string where, string[] what)
        {
            switch (where) //Kunne forkortes ved brug af array og array.length men de 3 sidste caser har brug magisk kode.
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
                case "newsfeed":
                    set_and_write(8, what);
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
        // Efter testning, fundet jeg ud, at der kan ske layout problemmer, derfor skal funktionen være mere aggressiv.
        {
            if (current_windowwidth != Console.WindowWidth || current_windowheight != Console.WindowHeight)
            {
                if (Math.Abs(odd_number(Console.WindowWidth) - current_windowwidth) > 1 || Math.Abs(odd_number(Console.WindowHeight) - current_windowheight) > 1)
                {
                    Console.WindowWidth = odd_number(Console.WindowWidth);
                    current_windowwidth = Console.WindowWidth;
                    Console.WindowHeight = odd_number(Console.WindowHeight);
                    current_windowheight = Console.WindowHeight;
                }
                else if (Console.WindowWidth % 2 == 0)
                {
                    Console.WindowWidth = odd_number(Console.WindowWidth);
                }
                else if (Console.WindowHeight % 2 == 0)
                {
                    Console.WindowHeight = odd_number(Console.WindowHeight);
                }
                return false;
            }
            else
                return true;
        }
        static void generate_ui()
        {

            //Skal være først i funktionen
            if (odd_numbered_window_size())
                return; //Hvis størrelsen er ikke meget forskellig, fortsæt ikke. Ellers generere nyt layout.

            int y_accounted_for_bar = current_windowheight - bottom_bar_height;
            int horizontally_middle = (y_accounted_for_bar - 1) / 2;
            int vertically_middle = (current_windowwidth - 1) / 2;
            int where_to_write = 0;

            // Mest højeste og venstreste punktet af fielder.
            field_size = new int[3, 2] { { 0, 0 }, { 0, horizontally_middle + 1 }, { vertically_middle + 1, 0 } };
            /* // Langere alternativ:
            field_size[0, 0] = 0; 
            field_size[0, 1] = 0;
            field_size[1, 0] = 0;
            field_size[1, 1] = horizontally_middle + 1;
            field_size[2, 0] = vertically_middle + 1;
            field_size[2, 1] = 0;
            //*/



            Console.Clear();
            Console.SetCursorPosition(0, 0);

            //Lave vandret og lodret linje. Vær opmærksom, at der skal tage hensyn af bottom_bar_height
            for (int y = 0; y < y_accounted_for_bar; ++y)
            {
                if (horizontally_middle == y)
                {
                    Console.SetCursorPosition(0, y);
                    Console.WriteLine(new string('-', vertically_middle) + "|");
                }
                else
                {
                    Console.SetCursorPosition(vertically_middle, y);
                    Console.WriteLine("|");
                }
            }

            //Sætte ind text_fields

            for (int first_dim = 0; first_dim < text_fields.Length; ++first_dim)
            {
                for (int second_dim = 0; second_dim < text_fields[first_dim].Length; ++second_dim)
                {
                    if (second_dim == 0)
                    {
                        Console.SetCursorPosition(centered_text(vertically_middle, text_fields[first_dim][second_dim]) + field_size[first_dim, 0] + 1, field_size[first_dim, 1] + 1);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(text_fields[first_dim][second_dim] + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (text_fields[first_dim][second_dim] == "sep")
                    {
                        Console.SetCursorPosition(field_size[first_dim, 0] + 1, Console.CursorTop + 1);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(new string('-', vertically_middle - 1));
                        Console.ForegroundColor = ConsoleColor.White;
                        //Snyd, sep er kun brugt i tredje subvindue, derfor den har ikke brug for switch
                        Console.Write(new string('\n', 5));

                    }
                    else
                    {
                        Console.SetCursorPosition(field_size[first_dim, 0] + 1, Console.CursorTop);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(text_fields[first_dim][second_dim]);
                        Console.ForegroundColor = ConsoleColor.White;
                        pos_of_inputs[where_to_write, 0] = field_size[first_dim, 0];
                        if (second_dim + 1 < text_fields[first_dim].Length && first_dim == 2) //også snyd
                        {
                            if (text_fields[first_dim][second_dim + 1] == "sep")
                            {
                                pos_of_inputs[where_to_write, 1] = Console.CursorTop + 2;
                                where_to_write += 1;
                            }
                            else
                            {
                                pos_of_inputs[where_to_write, 1] = Console.CursorTop + 1;
                                where_to_write += 1;
                            }
                        }
                        else
                        {
                            pos_of_inputs[where_to_write, 1] = Console.CursorTop + 1;
                            where_to_write += 1;
                            //Snyd, den skal bare virke
                            if (first_dim == 0)
                                Console.Write(new string('\n', 4));
                            else if (first_dim == 1)
                                Console.Write(new string('\n', 3));
                        }
                    }
                }
            }

            pos_of_inputs[8,0] = 0;
            pos_of_inputs[8,1] = Console.WindowHeight - 1;
            /* //Buggy code, alt det prøve at røre text_inputs[inputs], generere sådan error:
            //Unhandled Exception: System.NullReferenceException: Object reference not set to an instance of an object.
            for (int inputs = 0; inputs < text_inputs.Length; ++inputs)
            {
                if (text_inputs[inputs].GetLength(1) != 0)
                {
                    for (int n = 0; n < text_inputs[inputs].Length; ++n)
                    {
                        Console.SetCursorPosition(pos_of_inputs[inputs, 0] + 1, pos_of_inputs[inputs, 1] + n);
                        Console.Write(text_inputs[inputs][n]);
                    }
                    Console.SetCursorPosition(default_cursor_pos[0], default_cursor_pos[1]);
                }
            }
            //*/
        }
        static void Main(string[] args)
        {
            //"sep" står for separation, som ligner sådan: "----------------------"
            // Først string er titlen, og resten er enten separationer eller sub-titler.
            text_fields[0] = new string[] { "Temperatur og fugtighed", "Lager:", "Udenfor:" };
            text_fields[1] = new string[] { "Dato / tid", "København:", "London:", "Singapore:" };
            text_fields[2] = new string[] { "Lagerstatus", "Varer under minimum", "sep", "Varer over maksimum", "sep", "Mest solgte i dag", "sep" };
            generate_ui();


            while (true)
            {
                if (current_windowwidth != Console.WindowHeight || current_windowheight != Console.WindowWidth)
                    generate_ui(); //regeneration af ui

                /* //Debug
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                Console.Write(Console.WindowWidth + " " + Console.WindowHeight);
                //*/
                //input_fields("newsfeed", new string[1] { "lol" });
                /* //Forskellig debug
                Console.Clear();
                for (int a = 0; a < pos_of_inputs.GetLength(0); ++ a)
                {
                    Console.WriteLine(a + " = " + pos_of_inputs[a,0] + ", " + pos_of_inputs[a,1] );
                }
                break;
                //*/

                /* API:
                Du skal bruge "input_fields("where", what)" til at få ting på skærmen,
                "what" er string array, som bevæge cursor til specifiske plads og skrive hvad der er i array.
                "where" referere til et switch statement, som tjekke, om en af de følgene tekstfælde passer, som du kende som:
                0. "temp_og_fugt_lager"    = "Lager:"
                1. "temp_og_fugt_udenfor"  = "Udenfor:"
                2. "date_kobenhavn"        = "København:"
                3. "date_london"           = "London:"
                4. "date_singapore"        = "Singapore:"
                5. "lager_min"             = "Varer under minimum"
                6. "lager_max"             = "Varer over maksimum"
                7. "lager_mest"            = "Mest solgte i dag"
                8. "newsfeed"              = "Newsfeed fra Nordjyske"

                EKSEMPLER:
                    uden string af arrayer:
                        input_fields("newsfeed", new string[1] { "lol" });
                        set_and_write(8, new string[1] { "lol" });
                    med string af arrayer:
                        string[] var = new string[] {"lol"};

                        input_fields("newsfeed", var);
                        set_and_write(8, var);
                
                VÆR OPMARKSOM: input_fields tjekke ikke for hvor ofte sender du requester til API
                */

            }
        }
    }
}
