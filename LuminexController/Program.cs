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
            WindowOp winOp = new WindowOp();
            try
            {
                //winOp.SelectLastBatch(batchID);
                winOp.ClickRun();
            }
            catch(Exception ex)
            {
                bok = false;
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit!");
                Console.ReadKey();
            }
            finally
            {
                LuminexSDK.Instance.Close();
            }
            Folders.WriteResult(bok);
        }

        static void OnError()
        {
            Folders.WriteResult(false);
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }

    }



}
