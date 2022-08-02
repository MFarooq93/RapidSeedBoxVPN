using PureVPN.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Entity.Models.DTO
{
    public class PureAction
    {
        public PureLinksActions Action;
        public IEnumerable<string> parameters;
    }
}
