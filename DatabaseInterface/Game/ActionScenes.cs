using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class ActionScenes
    {
        public int ActionID { get; set; }
        public virtual Action Action { get; set; }
        public int SceneTypeID { get; set; }
        public virtual SceneType SceneType { get; set; }
    }
}
