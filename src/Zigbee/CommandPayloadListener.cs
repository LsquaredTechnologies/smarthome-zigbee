//using System.Collections.Generic;
//using System.Linq;
//using Lsquared.SmartHome.Zigbee.Protocol;

//namespace Lsquared.SmartHome.Zigbee
//{
//    internal sealed class CommandPayloadListener : IPayloadListener
//    {
//        public CommandPayloadListener(IList<IPayloadListener> listeners) => _listeners = listeners;

//        void IPayloadListener.OnNext(ICommandPayload command)
//        {
//            switch (command)
//            {
//                case ZCL.ICommand zclCommand:
//                    foreach (var listener in _listeners.ToArray())
//                        listener?.OnNext(zclCommand);
//                    break;

//                default:
//                    // ignore
//                    break;
//            }
//        }

//        private readonly IList<IPayloadListener> _listeners;
//    }
//}
