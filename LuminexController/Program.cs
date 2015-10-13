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
            int batchID = int.Parse(args[0]);
            WindowOp winOp = new WindowOp();
            bool bok = winOp.SelectLastBatch(batchID);
            if(!bok)
            {
                OnError();
                return;
            }
        }

        static void OnError()
        {
            Folders.WriteResult(false);
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }
    }



}
