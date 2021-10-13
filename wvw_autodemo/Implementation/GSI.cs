using CSGSI;
using System;
using System.IO;

namespace wvw_autodemo
{
    public class GSI
    {
        public static GameStateListener GSL;

        public static bool Setup(NewGameStateHandler OnNewGameState)
        {
            GSL = new GameStateListener(3333);
            GSL.NewGameState += OnNewGameState;

            return GSL.Start();
        }
    }
}