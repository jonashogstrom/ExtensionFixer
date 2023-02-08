using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtensionFixer.Shared;

namespace ExtensionFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            var verbose = args.Contains("-v");
            var doRename = args.Contains("-r");

            var tool = new ExtensionFixerTool(Console.WriteLine);
            foreach (var p in args)
                if (Directory.Exists(p))
                    tool.Main(p, verbose, doRename, new NullProgress());


            Console.WriteLine("Done, press enter to quit.");
            Console.ReadLine();
        }
    }

    internal class NullProgress : IMyProgress<double>
    {
        public void Report(double value)
        {

        }

        public void SetMax(int maxValue)
        {
        }

        public void SetIndefinite()
        {
        }
    }
}
