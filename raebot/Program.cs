using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace raebot
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            if (args.Length != 1)
            {
                Console.WriteLine("bot cannot start, needs token param");
                Console.Read();
            }
            else
            {
                Dictionary<string, string> tokens = new Dictionary<string, string>();
                StreamReader tokens_file = new StreamReader(Path.Combine(dir, "tokens.tokens"));
                string line;
                while((line = tokens_file.ReadLine()) != null)
                {
                    string[] parsed = line.Split('\t');
                    tokens.Add(parsed[0], parsed[1]);
                }
                if (tokens.ContainsKey(args[0]))
                {
                    MyBot bot = new MyBot(tokens[args[0]]);
                }
                else
                {
                    Console.WriteLine("Bot could not start; token key not found in .tokens file");
                }
            }
        }
    }
}
