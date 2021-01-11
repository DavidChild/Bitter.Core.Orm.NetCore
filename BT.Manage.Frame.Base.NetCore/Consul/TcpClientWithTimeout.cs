using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BT.Manage.Frame.Base.NetCore.Consul
{
    public class TcpClientWithTimeout
    {
        protected string _hostname;
        protected int _port;
        protected int _timeout_milliseconds;
        protected TcpClient connection;
        protected bool connected { get; set; }
        protected Exception exception;
        private TaskCompletionSource<bool> taskSource;
        public TcpClientWithTimeout(string hostname, int port, int timeout_milliseconds)
        {
            _hostname = hostname;
            _port = port;
            _timeout_milliseconds = timeout_milliseconds;
        }
        public TcpClientWithTimeout()
        {
            connected = false;
        }
        public bool Connect()
        {

            var task = Task.Factory.StartNew(BeginConnect);
            if (_timeout_milliseconds != 0 && _timeout_milliseconds > 0)
            {

                RegisetiOverTime(task, _timeout_milliseconds);

            }
            else
            {
                RegisetiOverTime(task, 500);
            }
            return this.connected;

        }

        protected void BeginConnect()
        {
            try
            {
                connection = new TcpClient(_hostname, _port);

            }
            catch (Exception ex)
            {


                connected = false;
            }
        }




        /// <summary>
        /// 超时，二次提交处理超时问题
        /// </summary>
        /// <param name="timeOut"></param>
        private void RegisetiOverTime(Task dotask, int timeOut)
        {


            var taskTimeOut = Task.Delay(timeOut);

            var tk = Task.WhenAny(dotask, taskTimeOut).Result;
            if (taskTimeOut == tk)
            {
                this.connected = false;
                if (this.connection != null && this.connection.Connected)
                {
                    this.connection.Close();
                }
                this.connection = null;
                return;
            }
            if (this.connection != null && this.connection.Connected)
            {
                this.connection.Close();
                this.connected = true;

            }
            else
            {
                this.connected = false;

            }
            this.connection = null;




        }
    }
}
