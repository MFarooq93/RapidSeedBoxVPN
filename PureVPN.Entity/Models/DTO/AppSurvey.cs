using System.Collections.Generic;

namespace PureVPN.Entity.Models.DTO
{
    public class AppSurvey : BaseNetwork
    {
        public AppSurveyBody body { get; set; }
    }

    public class AppSurveyBody
    {
        public InAppSurvey in_app_survey { get; set; }
    }

    public class InAppSurvey
    {
        public string content { get; set; }
        public string cta { get; set; }
        public string url { get; set; }
    }


}
