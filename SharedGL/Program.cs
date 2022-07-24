using System;
using Shared;

namespace SharedGL
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            using var game = new Game1();
            game.Run();
        }
    }
}