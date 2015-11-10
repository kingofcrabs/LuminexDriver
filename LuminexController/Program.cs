using LCLuminexController;
using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminexController
{
    class Program
    {
        static void Main(string[] args)
        {
<<<<<<< .mine
            string sArg = args[0];
            bool bok = true;
            if(sArg == "e")
            {
                Console.WriteLine("Ejecting Plate.");
                try
                {
                    LuminexSDK.Instance.Eject();
                    LuminexSDK.Instance.Close();
                }
                catch(Exception ex)
                {
                    bok = false;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press any key to exit!");
                    Console.ReadKey();
                }
                Folders.WriteResult(bok);
                return;
            }

            int batchID = int.Parse(sArg);
=======
            int batchID = int.Parse(args[0]);
>>>>>>> .r4
            WindowOp winOp = new WindowOp();
<<<<<<< .mine
            try
            {
                //winOp.SelectLastBatch(batchID);
                winOp.ClickRun();
                LuminexSDK.Instance.Close();
            }
            catch(Exception ex)
            {
                bok = false;
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
            }
            Folders.WriteResult(bok);
        }
=======
            bool bok = winOp.SelectLastBatch(batchID);
            if(!bok)
            {
                OnError();
                return;
            }
        }
>>>>>>> .r4

<<<<<<< .mine
=======
        static void OnError()
        {
            Folders.WriteResult(false);
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
>>>>>>> .r4
    }



}
