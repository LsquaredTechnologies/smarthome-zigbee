namespace Lsquared.SmartHome.Zigbee.ZCL
{
    public abstract class Cluster
    {
        public string Name { get; }

        protected Cluster(string name) =>
            Name = name;

        public abstract void OnNext(ICommandPayload cp);
    }
}
