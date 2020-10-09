using System;
using Lsquared.SmartHome.Zigbee;
using Lsquared.SmartHome.Zigbee.Protocol.Zigate;
using Lsquared.SmartHome.Zigbee.Transports.Serial;

var protocol = new ZigateProtocol();
var transport = SerialTransportFactory.FromSerial("COM5");
var network = new ZigbeeNetwork(transport, protocol);

network.Extensions.Add(new ZigbeeNodeDiscovery());
network.Extensions.Add(new ZigbeeGroupDiscovery());

//network.RegisterCluster<BasicCluster>(0x0000);
//network.RegisterCluster<IdentifyCluster>(0x0003);
//network.RegisterCluster<GroupsCluster>(0x0004);
//network.RegisterCluster<ScenesCluster>(0x0005);
//network.RegisterCluster<OnOffCluster>(0x0006);
//network.RegisterCluster<LevelCluster>(0x0008);
//network.RegisterCluster<DoorLockCluster>(0x0101);
//network.RegisterCluster<WindowCoveringCluster>(0x0102);
//network.RegisterCluster<ColorControlCluster>(0x0300);
//network.RegisterCluster<IlluminanceCluster>(0x0400);
//network.RegisterCluster<IlluminanceLevelCluster>(0x0401);
//network.RegisterCluster<TemperatureCluster>(0x0402);
//network.RegisterCluster<PressureCluster>(0x0403);
//network.RegisterCluster<RelativeHumidityCluster>(0x0405);

await network.SendAndReceiveAsync(new Lsquared.SmartHome.Zigbee.ZDO.Mgmt.GetVersionRequestPayload());
//await network.SendAndReceiveAsync(new Lsquared.SmartHome.Zigbee.ZDO.GetDevicesRequestPayload());

var invoker = new CommandInvoker(network);
var @continue = true;
while (@continue)
{
    Console.Write("> ");
    var cmd = Console.ReadLine();
    @continue = await invoker.InvokeAsync(cmd);
}

await network.DisposeAsync();
await transport.DisposeAsync();
