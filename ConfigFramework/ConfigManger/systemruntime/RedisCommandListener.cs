using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZF.Redis;
using ServiceStack.Redis;

namespace ConfigFramework.ConfigManger.systemruntime
{
    //redis网络及时监听器
    public class RedisCommandListener
    {
        private List<string> RedisServerIps;
        private CancellationTokenSource cancellation;
        private string redis_channel;
        public RedisCommandListener(string ips)
        {
            RedisServerIps = ips.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void Register(Action<string,string> action, CancellationTokenSource tokensource, string channel)
        {
            cancellation = tokensource;
            redis_channel = channel;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                ConnectSubscribe(action, channel);
            }, cancellation.Token);
        }

        private void ConnectSubscribe(Action<string, string> action,string channle)
        {
            while (!cancellation.Token.IsCancellationRequested)
            {
                RedisSub(action, channle);
            }
        }

        public void RedisSub(Action<string,string> action, string channle)
        {
            
            RedisManager redismanger = new RedisManager();


            using (IRedisClient redis = redismanger.GetClient(RedisServerIps, RedisServerIps))
            {
                using (var subscription = redis.CreateSubscription())
                {
                    subscription.OnUnSubscribe = channel =>
                    {

                    };
                    subscription.OnMessage = (channel, msg) =>
                    {
                        if (msg == "RedisCommand_Closed")
                        {

                            try
                            {
                                subscription.UnSubscribeFromAllChannels();   //关闭redis订阅
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                        else
                        {
                            action.Invoke(channel, msg);
                        }
                    };

                    subscription.SubscribeToChannels(channle); //blocks thread
                }
            }
        }
    }
}
