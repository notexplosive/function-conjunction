using System;

namespace gmtk2021
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Game1(args))
                game.Run();
        }
    }
}
