using PureVPN.Entity.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service.Contracts
{
    public interface IPureLinkParser
    {
        PureAction ParseActions(string Path);
    }
}
