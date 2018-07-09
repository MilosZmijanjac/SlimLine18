using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InLine18
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length>0)
            (new Inline18()).Run(args[0]);
        }
    }
}
