//https://github.com/AbrahamQll/BigNumbersNet
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace BigNumbersNet
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 
             * WARNING: While this function has been extensively tested,
             * for multiple hours, to look clear of any 'apparent' bugs,
             * 
             * BUT
             * This has not been proven to be MATHEMATICALLY SOUND
             * 
             * IT MAY HAVE SERIOUS BUGS
             * 
             * DON'T USE IN PRODUCTION ENVIRONMENTS,
             * 
			 * The speed of the code is very good,
			 * For really really big numbers, like 500 digits, it took the code 5 ms.
			 *
			 * If you do endup using this code, please cosider a link back to this repository
			 * thanky ou for playing nice! :)
			 * https://github.com/AbrahamQll/BigNumbersNet
			 *
             */



            //This section has been commented out so that you can later use this part to ask the user for input.
            /*
            Console.WriteLine("| ----------------------------------------------------------");
            Console.WriteLine("| This application will add or subtract very big numbers.");
            Console.WriteLine("| accepted number formats are: +9865, -5641, 46565, 0000415");
            Console.WriteLine("| to add use '+' and to subtract use '-'");
            Console.WriteLine("| ----------------------------------------------------------");
            Console.WriteLine("");

            string bignumber_one;
            string bignumber_two;
            string operation;

            bignumber_one = input("Enter the first number: ");
            bignumber_two = input("Enter the second number: ");

            string Numerical_pattern = "^[-+]?\\d+$";
            if (!Regex.IsMatch(bignumber_one, Numerical_pattern) || !Regex.IsMatch(bignumber_two, Numerical_pattern))
            {
                Console.WriteLine("Please only enter digits for numbers!");
                Console.WriteLine("Acceptable number formats are: +9865, -5641, 46565, 0000415");
                Console.WriteLine("TERMINATING!");
                Console.ReadLine();
                Application.Exit();
            }

            operation = input("Enter the operation to be performed ['+' for addition '-' for subtraction]: ");
            
            if (operation != "+" && operation != "-")
            {
                Console.WriteLine("Only \"+\" or \"-\" are acceptable for operation! TERMINATING!");
                Console.ReadLine();
                Application.Exit();
            }

            string out_put_result;
            out_put_result = BigNumberOperation(bignumber_one, bignumber_two,operation);
            Console.WriteLine(out_put_result);
            string[] ss = new string[] { };
            Main(ss);
            */



            /*
             * 
             * This section can be used to test if the code does the calculations accurately or not
             * 
             * IT will generate random numbers and then does the arithmetic calculations and then use 
             * the designed function's output to check and see if the function outputs the right calculations
             * 
             * 
            string[] oprtion = new string[] { "+", "-", "+", "-", "+", "-", "+", "-", "+", "-", "+", "-" };
            Random rnd = new Random();
            string random_operation = "";
            long r1;
            long r2;
            long sumsum = 0;
            string MyBigNumberOutPut;
            int counter = 0;
            do
            {
                int index = rnd.Next(oprtion.Length);
                random_operation = oprtion[index];
                Console.WriteLine(random_operation);
                r1 = LongRandom(-223372036854775807, 223372036854775807, new Random());

                r2 = LongRandom(-220372036854775807, 223372036854775807, new Random());
                r2 = LongRandom(-420372036854775807, 223372036854775807, new Random());
                r2 = LongRandom(-523372036854775807, 523372036854775807, new Random());

                switch (random_operation)
                {
                    case "+":
                        sumsum = r1 + r2;
                        break;

                    case "-":
                        sumsum = r1 - r2;
                        break;
                }


                MyBigNumberOutPut = BigNumberOperation(r1.ToString(), r2.ToString(), random_operation);

                if (sumsum.ToString() == MyBigNumberOutPut)
                {
                    counter++;
                    Console.WriteLine(counter.ToString() + "Tries OK!");
                    Console.WriteLine("First number:                " + r1.ToString());
                    Console.WriteLine("Second number:               " + r2.ToString());
                    Console.WriteLine("Operation:                   " + random_operation);
                    Console.WriteLine("______________________________________");
                    Console.WriteLine("Result should have been:     " + sumsum.ToString());
                    Console.WriteLine("This Algo gave:              " + MyBigNumberOutPut);


                }
                else
                {
                    Console.WriteLine("F A I L E D!");
                    Console.WriteLine("First number:                " + r1.ToString());
                    Console.WriteLine("Second number:               " + r2.ToString());
                    Console.WriteLine("Operation:                   " + random_operation);
                    Console.WriteLine("______________________________________");
                    Console.WriteLine("Result should have been:     " + sumsum.ToString());
                    Console.WriteLine("This Algo gave:             " + MyBigNumberOutPut);
                    Console.WriteLine(" ;( ");
                    Console.ReadLine();
                    break;
                }

            }
            while (true);

            */

            /*This line can evaluate how fast a very very big integer adding operation lasts using this algorithm,
             * You can then compare this result with python
             * 
             * Also, you can use python to verify if the output is right or not
             */
            var watch = new System.Diagnostics.Stopwatch();
            Console.ReadLine();
            string var1 = "549584093549984497140898040998047598842598427980249980042986499845970419805409898249680498498092647925841980471980495548910975494094558403946570935564190846903854792041923868409879410948709409404049069040950790899084479384547968310447102346071742561980654010854010910059885041009580000895008956008596700856849834798577928719874195687497879887999998979948997840947988888759744098799578974987458498640749874928354928401928574198584935499389049879409854699580489854780449586804495868045967894958080697889409458784495688404598840546978945989040509884954794034987409856430495846039484983849879495840056988498409435495840935499844971408980409980475988425984279802499800429864998459704198054098982496804984980926479258419804719804955489109754940945584039465709355641908469038547920419238684098794109487094094040490690409507908990844793845479683104471023460717425619806540108540109100598850410095800008950089560085967008568498347985779287198741956874978798879999989799489978409479888887597440987995789749874584986407498749283549284019285741985849354993890498794098546995804898547804495868044958680459678949580806978894094587844956884045988405469789459890405098849547940349874098564304958460394849838498794958400569884984094354958409354998449714089804099804759884259842798024998004298649984597041980540989824968049849809264792584198047198049554891097549409455840394657093556419084690385479204192386840987941094870940940404906904095079089908447938454796831044710234607174256198065401085401091005988504100958000089500895600859670085684983479857792871987419568749787988799999897994899784094798888875974409879957897498745849864074987492835492840192857419858493549938904987940985469958048985478044958680449586804596789495808069788940945878449568840459884054697894598904050988495479403498740985643049584603948498384987949584005698849840943";
            string var2 = "845984455041398468040997508849543840298047928042798460898658940958080447988406598804368987462394784409984804698094495780848340498524318079864205987049798408980407908408985648998758493687047986235469183416984569806478947984988004586980048599047908834647954380493474398978048984099804098879405986790494877449686452981204965849824609374098768940968809045890847588070856941625619084657581456671056403984607493487434986740799880779805867998048755840367265047250427680487988047998684099588684096454680469885059690988090876049687989940893449824562984560985047880409984801098805098569804998568495067584567849095847896805467960789985904884469804569800008845984455041398468040997508849543840298047928042798460898658940958080447988406598804368987462394784409984804698094495780848340498524318079864205987049798408980407908408985648998758493687047986235469183416984569806478947984988004586980048599047908834647954380493474398978048984099804098879405986790494877449686452981204965849824609374098768940968809045890847588070856941625619084657581456671056403984607493487434986740799880779805867998048755840367265047250427680487988047998684099588684096454680469885059690988090876049687989940884598445504139846804099750884954384029804792804279846089865894095808044798840659880436898746239478440998480469809449578084834049852431807986420598704979840898040790840898564899875849368704798623546918341698456980647894798498800458698004859904790883464795438049347439897804898409980409887940598679049487744968645298120496584982460937409876894096880904589084758807085694162561908465758145667105640398460749348743498674079988077980586799804875584036726504725042768048798804799868409958868409645468046988505969098809087604968798994089344982456298456098504788040998480109880509856980499856849506758456784909584789680546796078998590488446980456980000849480984546870458984976980400597408996484938743987439934498245629845609850478804099848010988050985698049985684950675845678490958478968054679607899859048844698045698000084948098454687045898497698040059740899648493874398743949480984546870458984976980400597408996484938743987439";
            watch.Start();
            string out_put = BigNumberOperation(var1, var2, "+");
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
			Console.WriteLine("");
			Console.WriteLine("-------var1------");
			Console.WriteLine(var1);
			Console.WriteLine("-------var2------");
			Console.WriteLine(var2);
			Console.WriteLine("-------result------");
            Console.WriteLine(out_put);
			Console.WriteLine("");
			Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.ReadLine();
        }
            

        //This function can be used to generate moderately acceptable random 'long' numbers
        static long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
            





        public static string BigNumberOperation(string First_Number, string Second_Number, string Operation)
        {
            //is the first number positive or negative?
            //Also removing the + or - symbols
            string First_Number_sign = "+";
            if (First_Number.IndexOfAny("+-".ToCharArray()) != -1)
            {
                First_Number_sign = First_Number[0].ToString();
                First_Number = First_Number.Replace(First_Number_sign, "");
            }

            //is the second number positive or negative?
            //Also removing the + or - symbols
            string Second_Number_sign = "+";
            if (Second_Number.IndexOfAny("+-".ToCharArray()) != -1)
            {
                Second_Number_sign = Second_Number[0].ToString();
                Second_Number = Second_Number.Replace(Second_Number_sign, "");
            }

            //reversing the order of the digits in the array
            //so the calculations can be done easier
            First_Number = ReverseString(First_Number);
            Second_Number = ReverseString(Second_Number);


            //calculating the largest possible array size required for the operation
            int PerfArraySize = Math.Max(First_Number.Length, Second_Number.Length);


            //defining and declaring the arrays
            int[] firstNumber = new int[PerfArraySize];
            int[] secondNumber = new int[PerfArraySize];
            int[] resultNumber = new int[PerfArraySize + 1];


            //filling the arrays
            for (int i = 0; i < First_Number.Length; i++) { firstNumber[i] = (int)char.GetNumericValue(First_Number[i]); }
            for (int i = 0; i < Second_Number.Length; i++) { secondNumber[i] = (int)char.GetNumericValue(Second_Number[i]); }
            for (int i = 0; i < PerfArraySize + 1; i++) { resultNumber[i] = 0; }

            //Coefficients of the numbers
            int N1Coefficient = +1;
            int N2Coefficient = +1;
            N1Coefficient = (First_Number_sign == "+" ? +1 : -1);
            N2Coefficient = (Second_Number_sign == "+" ? +1 : -1);

            //doing in-array calculations for the operation
            switch (Operation)
            {
                case "+":
                    for (int i = 0; i < PerfArraySize; i++)
                    {
                        resultNumber[i] = (N1Coefficient * firstNumber[i]) + (N2Coefficient * secondNumber[i]);
                    }
                    break;
                case "-":
                    for (int i = 0; i < PerfArraySize; i++)
                    {
                        resultNumber[i] = (N1Coefficient * firstNumber[i]) - (N2Coefficient * secondNumber[i]);
                    }
                    break;
            }


            //after the initial calculations it is time to get the actual final result
            IntArrayProccessor(ref resultNumber);

            ///conver the result to string
            string out_put_result = IntArrayToString(resultNumber);

            //returning the output
            return out_put_result;

        }

        /*
         * 
         * This is where the actual calculations for getting the final result
         * happen
         * 
         * WARNING:
         * 
         * Although this has been tested, extensively, there is no guarantee 
         * 
         * THIS FUNCTION MAY STILL HAVE BUGS, IT IS STILL IN DEVELOPMENT
         * 
         */
        static int[] IntArrayProccessor(ref int[] IntArray)
        {
            //going through the items
            for (int i = IntArray.Length; i > 0; i--)
            {
                if (i == IntArray.Length) { continue; }
                int tempint = IntArray[i - 1];
                if (tempint % 10 == 0 && tempint != 0)
                {
                    if (tempint >= 0)
                    {
                        tempint = tempint - 1;
                        IntArray[i] = Math.DivRem(tempint, 10, out IntArray[i - 1]);
                        IntArray[i - 1] = IntArray[i - 1] + 1;
                    }

                    if (tempint < 0)
                    {
                        tempint = tempint + 1;
                        IntArray[i] = Math.DivRem(tempint, 10, out IntArray[i - 1]);
                        IntArray[i - 1] = IntArray[i - 1] - 1;
                    }
                    
                }
                else
                {
                    IntArray[i] = Math.DivRem(tempint, 10, out IntArray[i - 1]);
                }
                
                if (IntArray[i] >= 10)
                {
                    IntArray[i] = IntArray[i] - 10;
                    IntArray[i + 1] = IntArray[i + 1] + 1;
                }
                if (IntArray[i] <= -10)
                {
                    IntArray[i] = IntArray[i] + 10;
                    IntArray[i + 1] = IntArray[i + 1] - 1;
                }
                if (i == 1){break;}
                IntArray[i - 2] = (IntArray[i - 1] * 10) + IntArray[i - 2];
                IntArray[i - 1] = 0;
            }
            do
            {
                var FoundIndex = Array.FindIndex(IntArray, element => element % 10 == 0 && Math.Abs(element) > 0);
                if (FoundIndex == -1) { break; }
                IntArray[FoundIndex+1] = IntArray[FoundIndex+1] + (IntArray[FoundIndex] / 10);
                IntArray[FoundIndex] = 0;
            } while (true);
            return IntArray;
        }


        //this function converts the output to string
        public static string IntArrayToString(int[] IntArrayX)
        {
            Boolean Starting_zeros = true;
            Boolean IsNegative = false;
            string OutPutString = "";
            for (int i = IntArrayX.Length; i > 0; i--)
            {
                if (IntArrayX[i - 1] == 0 && Starting_zeros)
                {
                    continue;
                }
                Starting_zeros = false;
                if (IntArrayX[i - 1] < 0)
                {
                    IsNegative = true;
                }
                OutPutString += Math.Abs(IntArrayX[i - 1]);
            }
            if (IsNegative)
            {
                OutPutString = "-" + OutPutString;
            }
            if (OutPutString == "")
            {
                OutPutString = "0";
            }
            return OutPutString;

        }


        //a clear custom input function
        static string input(string message_to_User) {
            string out_put_string;
            Console.Write(message_to_User);
            out_put_string = Console.ReadLine();
            return out_put_string;

        }

        //a function to reverse the order of elements in a string
        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
