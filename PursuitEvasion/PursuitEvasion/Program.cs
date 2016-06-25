using System;

namespace PursuitEvasion
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PursuitEvasion game = new PursuitEvasion())
            {
                game.Run();
            }
        }
    }
#endif
}

