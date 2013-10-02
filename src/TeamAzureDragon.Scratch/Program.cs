using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamAzureDragon.Utils;

namespace TeamAzureDragon.Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            //var obj = new { A = "a", B = "b", C = new { A = "a" } };

            //var dict = Misc.SerializeToDictionary(obj);

            var executer = new Rossie.Engine.CodeExecuter();
            Console.WriteLine(executer.Execute("return 1 + 2;"));
        }


    }
}
