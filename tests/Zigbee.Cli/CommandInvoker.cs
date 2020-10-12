using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Color;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Groups;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Identify;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.Level;
using Lsquared.SmartHome.Zigbee.ZCL.Clusters.OnOff;

namespace Lsquared.SmartHome.Zigbee
{
    internal sealed class CommandInvoker
    {
        public CommandInvoker(ZigbeeNetwork network)
        {
            _network = network;
            _groups = network.Extensions.Find<ZigbeeGroupDiscovery>();
        }

        public ValueTask<bool> InvokeAsync(string cmd)
        {
            var parts = cmd.ToLowerInvariant().Split(' ');
            return parts[0] switch
            {
                "" => new ValueTask<bool>(true),
                "clear" => InvokeAsync(Console.Clear),
                "quit" or "exit" => new ValueTask<bool>(false),
                "permit" => parts[1] switch
                {
                    "join" => parts[2] switch
                    {
                        "status" => GetPermitJoinStatusAsync(parts.Skip(3).ToArray()),
                        "on" => PermitJoinAsync(parts.Skip(3).ToArray(), true),
                        "off" => PermitJoinAsync(parts.Skip(3).ToArray(), false),
                        _ => PermitJoinDurationAsync(parts.Skip(2).ToArray())
                    },
                    _ => UnknownCommand(cmd)
                },
                "create" => parts[1] switch
                {
                    "group" => CreateGroupAsync(parts.Skip(2).ToArray()),
                    _ => UnknownCommand(cmd)
                },
                "get" => parts[1] switch
                {
                    "devices" => GetDevicesAsync(parts.Skip(2).ToArray()),
                    "endpoints" => GetActiveEndpointsAsync(parts.Skip(2).ToArray()),
                    "nd" => GetNodeDescriptorAsync(parts.Skip(2).ToArray()),
                    "cd" => GetComplexDescriptorAsync(parts.Skip(2).ToArray()),
                    "sd" => GetSimpleDescriptorAsync(parts.Skip(2).ToArray()),
                    "attr" or "attribute" => ReadAttributeAsync(parts.Skip(2).ToArray()),
                    "groups" => GetGroupsAsync(parts.Skip(2).ToArray()),
                    _ => UnknownCommand(cmd)
                },
                "list" => parts[1] switch
                {
                    "devices" => ListDevices(parts.Skip(2).ToArray()),
                    "groups" => ListGroups(parts.Skip(2).ToArray()),
                    _ => UnknownCommand(cmd)
                },
                "move" => parts[1] switch
                {
                    "l" or "level" => parts[2] switch
                    {
                        "g" or "grp" or "group" => MoveToLevelGroup(parts.Skip(3).ToArray()),
                        _ => MoveToLevel(parts.Skip(2).ToArray())
                    },
                    "t" or "temp" or "temperature" => parts[2] switch
                    {
                        "g" or "grp" or "group" => MoveToColorTempGroup(parts.Skip(3).ToArray()),
                        _ => MoveToColorTemp(parts.Skip(2).ToArray())
                    },
                    _ => UnknownCommand(cmd)
                },
                "identify" => IdentifyAsync(parts.Skip(1).ToArray()),
                "on" or "off" or "toggle" => OnOffAsync(parts[0], parts.Skip(1).ToArray()),
                _ => UnknownCommand(cmd)
            };
        }

        ValueTask<bool> InvokeAsync(Action action)
        {
            action();
            return new ValueTask<bool>(true);
        }

        ValueTask<bool> ListDevices(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Usage: list devices");
            }
            else
            {
                Console.WriteLine("Devices:");
                Console.WriteLine(string.Join("\n", _network.Nodes.Select(n => $"ExtAddr: {n.ExtAddr:X16}, NwkAddr: {n.NwkAddr:X4}")));
            }
            return new ValueTask<bool>(true);
        }

        ValueTask<bool> ListGroups(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Usage: list groups");
            }
            else if (_groups is not null)
            {
                Console.WriteLine("Groups:");
                Console.WriteLine(string.Join("\n", _groups.Groups.Select(g => $"GrpAddr: {g:X4}")));
            }
            return new ValueTask<bool>(true);
        }

        async ValueTask<bool> GetPermitJoinStatusAsync(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Usage: permit join status");
            }
            else
                await _network.SendAndReceiveAsync(new ZDO.Mgmt.GetPermitJoinStatusRequestPayload());
            return true;
        }

        async ValueTask<bool> PermitJoinAsync(string[] args, bool enable)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: permit join <on|off>");
                Console.WriteLine("       permit join <on|off> <short address>");
            }
            else
            {
                var nwkAddr = NWK.Address.AllRouters;
                if (args.Length == 1)
                    nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                await _network.SendAndReceiveAsync(new ZDO.Mgmt.PermitJoinRequestPayload(nwkAddr, enable));
            }
            return true;
        }

        async ValueTask<bool> PermitJoinDurationAsync(string[] args)
        {
            if (args.Length != 2)
                Console.WriteLine("Usage: permit join on <short address> <duration in seconds>");
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                byte duration = 30;
                if (args.Length > 1)
                    duration = byte.Parse(args[4], NumberStyles.HexNumber);
                await _network.SendAndReceiveAsync(new ZDO.Mgmt.PermitJoinRequestPayload(nwkAddr, duration));
            }
            return true;
        }

        async ValueTask<bool> CreateGroupAsync(string[] args)
        {
            if (args.Length < 3)
                Console.WriteLine("Usage: create group <name> <group address> <short address> [<short address...>]");
            else
            {
                var name = args[0];
                NWK.GroupAddress grpAddr = ushort.Parse(args[1], NumberStyles.HexNumber);
                var index = 2;
                while (index < args.Length)
                {
                    NWK.Address nwkAddr = ushort.Parse(args[index++], NumberStyles.HexNumber);
                    var payload = new ZCL.Command<AddGroupRequestPayload>(nwkAddr, 1, 1, new AddGroupRequestPayload(grpAddr, name));
                    await _network.SendAndReceiveAsync(payload);
                }
            }
            return true;
        }

        async ValueTask<bool> GetDevicesAsync(string[] args)
        {
            if (args.Length != 0)
                Console.WriteLine("Usage: get devices");
            else
            {
                var payload = new ZDO.GetDevicesRequestPayload();
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> GetActiveEndpointsAsync(string[] args)
        {
            if (args.Length != 1)
                Console.WriteLine("Usage: get endpoints <short address>");
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                var payload = new ZDO.GetActiveEndpointsRequestPayload(nwkAddr);
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> GetNodeDescriptorAsync(string[] args)
        {
            if (args.Length != 1)
                Console.WriteLine("Usage: get nd <short address>");
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                var payload = new ZDO.GetNodeDescriptorRequestPayload(nwkAddr);
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> GetComplexDescriptorAsync(string[] args)
        {
            if (args.Length != 1)
                Console.WriteLine("Usage: get cd <short address>");
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                var payload = new ZDO.GetComplexDescriptorRequestPayload(nwkAddr);
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> GetSimpleDescriptorAsync(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
                Console.WriteLine("Usage: get sd <short address> [<endpoint=1>]");
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                APP.Endpoint endpoint = 1;
                if (args.Length > 1)
                    endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var payload = new ZDO.GetSimpleDescriptorRequestPayload(nwkAddr, endpoint);
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> ReadAttributeAsync(string[] args)
        {
            // TODO handle endpoint...
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: get attribute <short address> <endpoint> <clusterID> <attributeID>");
                Console.WriteLine("       get attribute <IEEE address> <endpoint> <clusterID> <attributeID>");
            }
            else
            {
                APP.Address address;
                if (args[0].Length > 4)
                {
                    MAC.Address extAddr = ulong.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(extAddr);
                }
                else
                {
                    NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(nwkAddr);
                }
                APP.Endpoint endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var clusterID = ushort.Parse(args[2], NumberStyles.HexNumber);
                var attributeID = ushort.Parse(args[3], NumberStyles.HexNumber);
                var payload = new ZCL.Command<ZCL.ReadAttributesRequestPayload>(
                    address, 1, endpoint, new ZCL.ReadAttributesRequestPayload(clusterID, attributeID));
                await _network.SendAndReceiveAsync(payload);
            }
            return true;
        }

        async ValueTask<bool> GetGroupsAsync(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: get groups <short address> [<endpoint=1>]");
                Console.WriteLine("       get groups <IEEE address> [<endpoint=1>]");
            }
            else
            {
                APP.Address address;
                if (args[0].Length > 4)
                {
                    MAC.Address extAddr = ulong.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(extAddr);
                }
                else
                {
                    NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(nwkAddr);
                }

                APP.Endpoint endpoint = 1;
                if (args.Length > 1)
                    endpoint = byte.Parse(args[1], NumberStyles.HexNumber);

                await _network.SendAndReceiveAsync(
                    new ZCL.Command<GetGroupMembershipRequestPayload>(
                        address, 1, endpoint, new GetGroupMembershipRequestPayload()));
            }
            return true;
        }

        async ValueTask<bool> MoveToLevel(string[] args)
        {
            if (args.Length < 1 || args.Length > 5)
            {
                Console.WriteLine("Usage: move level <short address> <endpoint> <level> [<duration=5>]");
                Console.WriteLine("       move level <IEEE address> <endpoint> <level> [<duration=5>]");
            }
            else
            {
                APP.Address address;
                if (args[0].Length > 4)
                {
                    MAC.Address extAddr = ulong.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(extAddr);
                }
                else
                {
                    NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(nwkAddr);
                }

                APP.Endpoint endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var level = byte.Parse(args[2], NumberStyles.HexNumber);
                int duration = 5 * 10;
                if (args.Length == 4)
                    duration = int.Parse(args[3]) * 10;

                await _network.SendAndReceiveAsync(
                    new ZCL.Command<MoveToLevelRequestTimedPayload>(
                        address, 1, endpoint, new MoveToLevelRequestTimedPayload(true, level, (ushort)duration)));
            }
            return true;
        }

        async ValueTask<bool> MoveToLevelGroup(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
                Console.WriteLine("Usage: move level group <group address> <endpoint> <level> [<duration=5>]");
            else
            {
                NWK.GroupAddress grpAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                var address = new APP.Address(grpAddr);

                APP.Endpoint endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var level = byte.Parse(args[2], NumberStyles.HexNumber);
                int duration = 5 * 10;
                if (args.Length == 4)
                    duration = int.Parse(args[3]) * 10;

                await _network.SendAndReceiveAsync(
                    new ZCL.Command<MoveToLevelRequestTimedPayload>(
                        address, 1, endpoint, new MoveToLevelRequestTimedPayload(true, level, (ushort)duration)));
            }
            return true;
        }

        async ValueTask<bool> MoveToColorTemp(string[] args)
        {
            if (args.Length < 1 || args.Length > 5)
            {
                Console.WriteLine("Usage: move temperature <short address> <endpoint> <temperature in K> [<duration=5>]");
                Console.WriteLine("       move temperature <IEEE address> <endpoint> <temperature in K> [<duration=5>]");
            }
            else
            {
                APP.Address address;
                if (args[0].Length > 4)
                {
                    MAC.Address extAddr = ulong.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(extAddr);
                }
                else
                {
                    NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(nwkAddr);
                }

                APP.Endpoint endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var temperatureInKelvin = int.Parse(args[2]);
                int duration = 5 * 10;
                if (args.Length == 4)
                    duration = int.Parse(args[3]) * 10;

                await _network.SendAndReceiveAsync(
                    new ZCL.Command<MoveToColorTemperatureRequestPayload>(
                        address, 1, endpoint, new MoveToColorTemperatureRequestPayload(temperatureInKelvin, (ushort)duration)));
            }
            return true;
        }

        async ValueTask<bool> MoveToColorTempGroup(string[] args)
        {
            if (args.Length < 3 || args.Length > 4)
                Console.WriteLine("Usage: move temperature group <group address> <endpoint> <temperature in K> [<duration=5>]");
            else
            {
                NWK.GroupAddress grpAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                var address = new APP.Address(grpAddr);

                APP.Endpoint endpoint = byte.Parse(args[1], NumberStyles.HexNumber);
                var temperatureInKelvin = int.Parse(args[2]);
                int duration = 5 * 10;
                if (args.Length == 4)
                    duration = int.Parse(args[3]) * 10;

                await _network.SendAndReceiveAsync(
                    new ZCL.Command<MoveToColorTemperatureRequestPayload>(
                        address, 1, endpoint, new MoveToColorTemperatureRequestPayload(temperatureInKelvin, (ushort)duration)));
            }
            return true;
        }

        async ValueTask<bool> IdentifyAsync(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: identify <short address>");
                // TODO add duration
            }
            else
            {
                NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                await _network.SendAndReceiveAsync(
                    new ZCL.Command<IdentifyRequestPayload>(
                        nwkAddr, 1, 1, new IdentifyRequestPayload(TimeSpan.FromSeconds(5))));
            }
            return true;
        }

        async ValueTask<bool> OnOffAsync(string cmd, string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine($"Usage: {cmd} <IEEE address>");
                Console.WriteLine($"       {cmd} <short address>");
                Console.WriteLine($"       {cmd} group <group address>");
            }
            else
            {
                APP.Address address;
                if (args[0] == "g" || args[0] == "grp" || args[0] == "group")
                {
                    NWK.GroupAddress grpAddr = ushort.Parse(args[1], NumberStyles.HexNumber);
                    address = new APP.Address(grpAddr);
                }
                else if (args[0].Length > 4)
                {
                    MAC.Address extAddr = ulong.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(extAddr);
                }
                else
                {
                    NWK.Address nwkAddr = ushort.Parse(args[0], NumberStyles.HexNumber);
                    address = new APP.Address(nwkAddr);
                }

                var zclCommandPayloadPayload = cmd switch
                {
                    "off" => OnOff.Off,
                    "on" => OnOff.On,
                    _ => OnOff.Toggle
                };

                var zclCommandPayload = new OnOffRequestPayload(zclCommandPayloadPayload);
                var zclCommand = new ZCL.Command<OnOffRequestPayload>(address, 1, 1, zclCommandPayload);
                await _network.SendAndReceiveAsync(zclCommand);
            }
            return true;
        }

        ValueTask<bool> UnknownCommand(string cmd)
        {
            Console.WriteLine($"Unknown command: {cmd}");
            return new ValueTask<bool>(true);
        }

        private readonly ZigbeeNetwork _network;
        private readonly ZigbeeGroupDiscovery? _groups;
    }
}
