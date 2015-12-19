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
            Console.WriteLine("version is : {0}", strings.version);
            if(args.Count() == 0)
            {
                Console.WriteLine("Argument not defined, press any key to exit!");
                Console.ReadKey();
                return;
            }
            string sArg = args[0];
            WindowOp winOp = new WindowOp();
            bool bok = true;
            if(sArg == "e")
            {
                winOp.Eject();
                return;
            }


            int batchID = int.Parse(sArg);
            
            try
            {
                winOp.SelectLastBatch(batchID);
                winOp.ClickRun();
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
        

    }



}
