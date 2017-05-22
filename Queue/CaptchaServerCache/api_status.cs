using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptchaServerCache
{
    /*
HTTP Simple Queue Service v1.7
------------------------------
Queue Name: fuck12306PassCode
Maximum number of queues: 1000000
Put position of queue (1st lap): 249102
Get position of queue (1st lap): 249102
Number of unread queue: 0
     */
    public class api_status
    {
        public string version { get; set; }
        public int unread { get; set; }
        public int get { get; set; }
        public int put { get; set; }
        public string name { get; set; }
    }
}