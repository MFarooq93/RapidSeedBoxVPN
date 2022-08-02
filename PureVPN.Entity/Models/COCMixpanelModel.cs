using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models
{
    public class COCMixpanelModel
    {
        public ConnectingFrom ConnectVia { get; set; }

        public ConnectedTo SelectedInterfaceType { get; set; }
        
        public SelectedInterfaceScreen SelectedInterfaceScreenType { get; set; }
        
        public string SelectedLocation { get; set; }
        
        public string CurrentConnectedLocation { get; set; }
    }
}
