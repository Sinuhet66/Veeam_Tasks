
using System;
using System.Diagnostics;

namespace Monitor
{
    class Program
    {

        static Process[] processlist;
        static void Main(string[] args)
        {
            const string strArgumentText = "Too few arguments! Try to enter again? Yes/No - [y]/[n])";
            const string strRetryText = "Wrong answer! Allowed [y] OR [n] (Yes OR No)";
            const string strProcessText = "Process name: ";
            const string strTimeText = "Maximum lifetime [min]: ";
            const string strCycleText = "Monitoring frequency [min]: ";
            string strLine;
            int iProcess;
            Boolean bEnd = false;
            string strProcess;
            Double dProcessTime;
            Double dFrequency;
            int iFrequency;

            //write start time of program to log file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("monitor.log", true))
            {
                file.WriteLine("----------------------------------------------------");
                file.WriteLine("Start of monitoring program: " + DateTime.Now);
            }

            //check count of passed arguments, if less, ask for user to enter them correctly
            if (args.Length < 3)
            {
                  //ask user if he wants to enter correct arguments
                Console.WriteLine(strArgumentText);
                strLine = Console.ReadLine();
                while ((strLine.ToUpper() != "Y") && (strLine.ToUpper() != "N"))
                {
                    Console.Clear();
                    Console.WriteLine(strRetryText);
                    Console.WriteLine(strArgumentText);
                    strLine = Console.ReadLine();
                }

                if (strLine.ToUpper() == "Y")
                {
                    //ask user for process name to monitor
                    Console.WriteLine(strProcessText);
                    strProcess = Console.ReadLine();

                    //ask user for maximum life time of the process to monitor
                    dProcessTime = GetUserTime(strTimeText);

                    //ask user for monitoring frequency time of the process to monitor
                    dFrequency = GetUserTime(strCycleText);
                }
                else
                {
                    //end program after user don't want to enter correct arguments, and write it into log file
                    Console.WriteLine("Ending program");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("monitor.log", true))
                    {
                        file.WriteLine("Arguments not presented in required format. Ending program.");
                    }
                    System.Threading.Thread.Sleep(2000);
                    return;
                }
            }
            else
            {
                //count of passed arguments is OK
                //process name
                strProcess = args[0];
                try
                {
                    //maximum lifetime
                    dProcessTime = Double.Parse(args[1]);
                    //monitoring frequency
                    dFrequency = Double.Parse(args[2]);
                }
                catch (FormatException)
                {
                    //maximum lifetime or monitoring frequency not in correct format - end of program
                    Console.WriteLine("Inavlid format of passed arguments!");
                    Console.WriteLine("Reqired arguments: STRING(name of the process) NUMBER(maximum lifetime - minutes) NUMBER(monitoring frequency - minutes).");
                    Console.WriteLine("Ending program");
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("monitor.log", true))
                    {
                        file.WriteLine("Inavlid format of passed arguments! Ending program.");
                    }
                    System.Threading.Thread.Sleep(2000);
                    return;
                }
            }

            try
            {
                //convert monitoring fraquency to miliseconds
                iFrequency = Convert.ToInt32(dFrequency * 60 * 1000);
            }
            catch (OverflowException)
            {
                //monitoring frequency not in correct format - end of progarm
                Console.WriteLine("Wrong value for monitoring frequency: " + dFrequency.ToString());
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("monitor.log", true))
                {
                    file.WriteLine("Wrong value for monitoring frequency: " + dFrequency.ToString() + " Ending program.");
                }
                return;
            }

            while (!(bEnd))
            {
                iProcess = 0;
                Console.Clear();
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("--- Pres CTRL+C to end the program. ---");
                Console.WriteLine("---------------------------------------");
                Console.WriteLine(strProcessText + " " + strProcess);
                Console.WriteLine(strTimeText + " " + dProcessTime);
                Console.WriteLine(strCycleText + " " + dFrequency);
                Console.WriteLine("---------------------------------------");
                //GetUserTime all running processes
                processlist = Process.GetProcesses();
                foreach (Process process in processlist)
                {
                    if (process.ProcessName.ToUpper().Equals(strProcess.ToUpper()))
                    {
                        try
                        {
                            //found monitored process
                            Console.WriteLine("ID of the process: " + process.Id);
                            Console.WriteLine("Startime of the process: " + process.StartTime);
                            Console.WriteLine("Runnig time of the process in minutes: " + (DateTime.Now - process.StartTime).TotalMinutes.ToString());
                            if ((DateTime.Now - process.StartTime).TotalMinutes.CompareTo(dProcessTime) > 0)
                            {
                                //monitored process maximum lifetime exceeded - kill and write to log file information
                                process.Kill();
                                Console.WriteLine("Process killed.");
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter("monitor.log", true))
                                {
                                    file.WriteLine(strProcessText + " " + strProcess);
                                    file.WriteLine("ID of the process: " + process.Id);
                                    file.WriteLine("Process started at: " + process.StartTime);
                                    file.WriteLine("Process killed at: " + DateTime.Now);
                                    file.WriteLine("----------------------------------------------------");
                                }
                            }
                            else
                            {
                                //monitored process maximum lifetime not exceeded - conitnue monitoring
                                Console.WriteLine("Process is running less than maximum lifetime.");
                                iProcess = 1;
                            }
                            Console.WriteLine();
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                            //startime of the process not accesible, not able to kill it - continue
                            Console.WriteLine("Process starttime not accesible!");
                            Console.WriteLine("Process will not be killed!");
                        }
                    }
                }

                if (iProcess == 0)
                {
                    //no running process to monitor
                    Console.WriteLine("No running process with entered name.");
                }

                //wait till next monitoring - time=monitoring frequency
                System.Threading.Thread.Sleep(iFrequency);

            }
            Console.WriteLine("End of program.");
        }

        //get user input for maximum lifetime and monitoring frequency
        //try to parse user input into number
        //if parsing failed, try to get input from user again
        private static Double GetUserTime(String strUserText)
        {
            Boolean bCorrect = false;
            string strTime;
            Double result = 0;

            while (!(bCorrect))
            {
                Console.WriteLine(strUserText);
                strTime = Console.ReadLine();
                try
                {
                    result = Double.Parse(strTime);
                    Console.WriteLine(result);
                    bCorrect = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{strTime}'");
                    Console.WriteLine("Enter only numbers!");
                }
            }
            return result;
        }


    }
}