using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLib
{
    public static class Communication
    {
        public static void ParseMessage(Messages message)
        {

        }
        public enum Messages
        {
            TurnOff, 
            Echo, 
            AddNumbers, 
            ReturnToSender
        }
        public static void TurnOff() 
        {
            Console.WriteLine("Closing socket...");
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

        }
        public static void Echo()
        {
             
        }
        public static void AddNumbers()
        {
            
        }
    }
}
