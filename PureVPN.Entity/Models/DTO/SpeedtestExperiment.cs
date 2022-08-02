using System.Collections.Generic;

namespace PureVPN.Entity.Models.DTO
{
    public class SpeedtestExperiment : BaseNetwork
    {
        public ExperimentBody body { get; set; }
    }

    public class ExperimentBody
    {
        public List<Experiment> experiments { get; set; }
    }
    public class Experiment
    {
        public string name { get; set; }
        public string group { get; set; }
    }


}
