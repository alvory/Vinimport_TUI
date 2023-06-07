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
     * - Magisk kode for offset af begyndelse af text_input linje i venstre sub-vindue ("Lagerstatus").
     * - fjern hacks i generate_ui() funktionen(måske aldrig)
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
        static int y_accounted_for_bar = current_windowheight - bottom_bar_height;
        static int horizontally_middle = (y_accounted_for_bar - 1) / 2;
        static int vertically_middle = (current_windowwidth - 1) / 2;
        static int update_time = 300; //hvor hurtig skal API opdateres i sekunder

        static void err_msg(string msg, int which = 1)
        {
            Console.WriteLine("ERROR: " + msg);
            Environment.Exit(which);
        }
        static void colorfull_print(string what, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(what);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static string[] wrapper(string[] what) //Hvis linje er for stør, klip tekst kort og læg det i nyt linje
        {
            for (int line = 0; line < what.Length; ++line)
            {
                if (what[line].Length > vertically_middle - 1)
                {
                    Array.Resize(ref what, what.Length + 1);
                    for (int enil = what.Length - 1; enil > line; --enil)
                    {
                        what[enil] = what[enil - 1];
                    }
                    what[line + 1] = what[line].Substring(vertically_middle - 1);
                    what[line] = what[line].Substring(0, vertically_middle - 1);
                }
            }
            return what;
        } //*/
        static void set_and_write(int where, string[] what, ConsoleColor color = ConsoleColor.White)
        {
            if (where != 8) //newsfeed skulle ikke bruge det, ellers ting kunne gå galt.
            {
                string[] temp_what = wrapper(what);
                what = temp_what;
            }

            if (text_inputs.ElementAt(where) != null)
                if (!text_inputs[where].Equals(what)) //Hvis forskellig, opdatere.
                    text_inputs[where] = what; //for regeneraton af ui
            for (int n = 0; n < what.Length; ++n)
            {
                Console.SetCursorPosition(pos_of_inputs[where, 0] + 1, pos_of_inputs[where, 1] + n);
                colorfull_print(what[n], color);
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
                case var _ when where == "0" || where == "temp_og_fugt_lager":
                    set_and_write(0, what);
                    break;
                case var _ when where == "1" || where == "temp_og_fugt_udenfor":
                    set_and_write(1, what);
                    break;
                case var _ when where == "2" || where == "date_kobenhavn":
                    set_and_write(2, what);
                    break;
                case var _ when where == "3" || where == "date_london":
                    set_and_write(3, what);
                    break;
                case var _ when where == "4" || where == "date_singapore":
                    set_and_write(4, what);
                    break;
                case var _ when where == "5" || where == "lager_min":
                    set_and_write(5, what, ConsoleColor.Red);
                    break;
                case var _ when where == "6" || where == "lager_max":
                    set_and_write(6, what, ConsoleColor.Green);
                    break;
                case var _ when where == "7" || where == "lager_mest":
                    set_and_write(7, what);
                    break;
                case var _ when where == "8" || where == "newsfeed":
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

            y_accounted_for_bar = current_windowheight - bottom_bar_height;
            horizontally_middle = (y_accounted_for_bar - 1) / 2;
            vertically_middle = (current_windowwidth - 1) / 2;
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
                        colorfull_print(text_fields[first_dim][second_dim] + "\n\n", ConsoleColor.Cyan);
                    }
                    else if (text_fields[first_dim][second_dim] == "sep")
                    {
                        Console.SetCursorPosition(field_size[first_dim, 0] + 1, Console.CursorTop + 1);
                        colorfull_print(new string('-', vertically_middle - 1), ConsoleColor.Cyan);
                        //Snyd, sep er kun brugt i tredje subvindue, derfor den har ikke brug for switch
                        Console.Write(new string('\n', 5));

                    }
                    else
                    {
                        Console.SetCursorPosition(field_size[first_dim, 0] + 1, Console.CursorTop);
                        colorfull_print(text_fields[first_dim][second_dim], ConsoleColor.Cyan);
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

            //Altid kendt
            pos_of_inputs[8,0] = 0;
            pos_of_inputs[8,1] = Console.WindowHeight - 1;

            //* //Buggy code, alt det prøve at røre text_inputs[inputs], generere sådan error:
            //Unhandled Exception: System.NullReferenceException: Object reference not set to an instance of an object.
            for (int inputs = 0; inputs < text_inputs.Length; ++inputs)
            {
                if (text_inputs.ElementAt(inputs) != null)
                {
                    set_and_write(inputs, text_inputs[inputs]);
                }
            }
            //*/
        }
        static void Main(string[] args)
        {
            //Måske disse 2 linje nedenunder vil tillade UTF-8 encoding i stedet af hvad windows bruger som standard.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Text.Encoding.GetEncoding(65001);
            //"sep" står for separation, som ligner sådan: "----------------------"
            // Først string er titlen, og resten er enten separationer eller sub-titler.
            text_fields[0] = new string[] { "Temperatur og fugtighed", "Lager:", "Udenfor:" };
            text_fields[1] = new string[] { "Dato / tid", "København:", "London:", "Singapore:" };
            text_fields[2] = new string[] { "Lagerstatus", "Varer under minimum", "sep", "Varer over maksimum", "sep", "Mest solgte i dag", "sep" };
            generate_ui();

            long last_time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            while (true)
            {
                if (current_windowwidth != Console.WindowHeight || current_windowheight != Console.WindowWidth)
                    generate_ui(); //regeneration af ui


                    long time = DateTimeOffset.Now.ToUnixTimeSeconds();
                    if ( Math.Abs(time - last_time) >= update_time )
                    {
                    //API, som skal har forbindelse til netværk skal være her:

                    last_time = time;
                    }



                    /* //Debug
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);
                    Console.Write(Console.WindowWidth + " " + Console.WindowHeight);
                    //*/
                    //input_fields("5", new string[1] { "very very very long text designed to overflow and fuck up everything i worked for very hard and very much to the point where i lack things to write" });
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
                        med string af arrayer:
                            string[] var = new string[] {"lol"};

                            input_fields("newsfeed", var);

                    VÆR OPMARKSOM: input_fields tjekke ikke for hvor ofte sender du requester til API
                    */

                }
            }
    }
}
