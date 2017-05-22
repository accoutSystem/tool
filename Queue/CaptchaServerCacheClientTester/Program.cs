#region source code header

// solution:CaptchaServer
// created:2015-04-08
// modify:2015-04-08
// copyright fangbian.com 2015

#endregion

#region

using System;
using System.Threading;
using CaptchaServerCacheClient;
using CaptchaServerCacheClientTester.Properties;

#endregion

namespace CaptchaServerCacheClientTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stackCli = new StackClient
            {
                Auth = Settings.Default.Auth,
                Name = Settings.Default.Name,
                Url = Settings.Default.Url
            };

            for (var i = 0; i < 10; i++)
            {
                stackCli.Push("stack_" + i);
                Console.WriteLine("push " + i);
            }

            for (var n = 0; n < 10; n++)
                for (var i = 0; i < 1; i++)
                {
                    Console.WriteLine(stackCli.Pop());
                }

            Console.ReadLine();

            var queueCli = new QueueClient
            {
                Auth = Settings.Default.Auth,
                Name = Settings.Default.Name,
                Url = Settings.Default.Url
            };

            for (var i = 0; i < 10; i++)
            {
                queueCli.Enqueue("queue_" + i);
                Console.WriteLine("enqueue " + i);
            }

            for (var n = 0; n < 10; n++)
                for (var i = 0; i < 1; i++)
                {
                    Console.WriteLine(queueCli.Dequeue());
                }

            Console.ReadLine();


            var cacheCli = new CacheClient
            {
                Auth = Settings.Default.Auth,
                Name = Settings.Default.Name,
                Url = Settings.Default.Url
            };

            for (var i = 0; i < 10; i++)
            {
                cacheCli.Add("cache_" + i, i + "", 1);
                Console.WriteLine("add " + i);
            }

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(cacheCli.Exist("cache_" + i));
                Console.WriteLine(cacheCli.Get("cache_" + i));
            }

            Thread.Sleep(500);

            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine("cache_" + i + " exist " + cacheCli.Exist("cache_" + i) + " then remove.");
                Console.WriteLine(cacheCli.Remove("cache_" + i));
                Console.WriteLine("cache_" + i + " exist " + cacheCli.Exist("cache_" + i) + ".");
            }

            Thread.Sleep(2000);
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(cacheCli.Exist("cache_" + i));
            }

            for (var i = 0; i < 1000; i++)
            {
                Console.WriteLine("get " + cacheCli.Get("cache_" + i));
            }

            for (var i = 0; i < 1000; i++)
            {
                cacheCli.Add("cache_" + i, i + "");
                Console.WriteLine("add " + i);
            }
            Console.ReadLine();
        }
    }
}