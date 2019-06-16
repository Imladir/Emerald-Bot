﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class ActionTypeAction
    {
        public int ActionTypeID { get; set; }
        public virtual ActionType ActionType { get; set; }

        public int ActionID { get; set; }
        public virtual Action Action { get; set; }
    }
}
