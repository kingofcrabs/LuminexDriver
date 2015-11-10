using Lx100IS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuminexController
{
    class LuminexSDK
    {
        Application objLXIS = null;
        static LuminexSDK instance = null;
        static public LuminexSDK Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new LuminexSDK();
                }
                return instance;
            }
        }


        public void Close()
        {
            if (objLXIS != null)
                objLXIS.Disconnect();
        }
        private LuminexSDK()
        {
            var connect = new Connector();
            objLXIS = connect.Application["A0095978-3906-4ba5-AC19-3DE880EE6A01"];
            if(objLXIS == null)
            {
                throw new Exception("Cannot connect to the Luminex IS App!");
            }
        }
        public void Eject()
        {
            objLXIS.Eject();
        }

        public void Retract()
        {
            objLXIS.Retract();
        }

    }
}
