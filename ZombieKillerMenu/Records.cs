using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieKillerMenu
{
    class Records
    {
        public string GameName { get; set; }
        public int Highscore{ get; set; }
        public int SurvivalTime{ get; set; }

        public Records(string gamename, int highscore, int survivaltime)
        {
            GameName = gamename;
            Highscore = highscore;
            SurvivalTime = survivaltime;
        }
    }


}
