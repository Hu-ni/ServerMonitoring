using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Models
{
    public class Friend
    {
        public string ID { get; set; }
        public string UUID { get; set; }
        public bool Favorite { get; set; }
        public string Profile_Nickname { get; set; }
        public string Profile_Thumbnail_Image { get; set; }

    }
}
