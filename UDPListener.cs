using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PunchServerUDPReceiver
{
    internal class UDPListener
    {
        internal delegate void RecvCallback(string data);
        internal Action<string> OnRecvCallback;
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public UDPListener(Action<string> rxCallback)
        {
            OnRecvCallback = rxCallback;
        }

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                OnRecvCallback($"RECV: {epFrom.ToString()}: {bytes}, {Encoding.ASCII.GetString(so.buffer, 0, bytes)}");
            }, state);
        }
    }
}
