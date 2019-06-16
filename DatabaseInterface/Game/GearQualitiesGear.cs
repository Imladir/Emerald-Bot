using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Game
{
    public class GearQualitiesGear
    {
        public int GearID { get; set; }
        public virtual Gear Gear { get; set; }

        public int GearQualityID { get; set; }
        public virtual GearQuality GearQuality { get; set; }
    }
}
