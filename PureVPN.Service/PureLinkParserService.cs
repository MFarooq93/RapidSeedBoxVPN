using PureVPN.Entity.Enums;
using PureVPN.Entity.Models.DTO;
using PureVPN.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureVPN.Service
{
    public class PureLinkParserService : IPureLinkParser
    {
        public PureAction ParseActions(string Path)
        {
            PureLinksActions pureLinksEnum;
            List<PureAction> result = new List<PureAction>();
            Uri uri = new Uri(Path);
            Path = uri.PathAndQuery;
            string[] Actions = Path.Split('/').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (Actions?.Length > 0 && Enum.TryParse(Actions[0], true, out pureLinksEnum))
            {
                return new PureAction { Action = pureLinksEnum, parameters = Actions.Skip(1).ToArray() };

            }
            return default;
        }//ParseActions
    }
}