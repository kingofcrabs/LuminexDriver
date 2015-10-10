using LCLuminexController;
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
            int handle = int.Parse(args[0]);
            Bitmap bmp = CaptureHelper.CaptureControl((IntPtr)handle);
            Console.WriteLine("snapshot saved at f:\\test.jpg");
            bmp.Save("f:\\test.jpg");

        }
    }



}
