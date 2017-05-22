using Fangbian.Data.Client;
using Fangbian.DataStruct.Business.System;
using Fangbian.DataStruct.Watch;
using FangBian.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fangbian.Ticket.Server.Trace.Watch
{
    /// <summary>
    /// 上传机器状态的监视组件
    /// </summary>
    public class ServerWatch
    {
        public static string WatchGuid { get; set; }

        //SystemConfigClient.WebInterfaceUrl "http://localhost/wf/"
        public static string Path
        {
            get
            {
                return SystemConfigClient.WebInterfaceUrl;
            }
        }// ;  SystemConfigClient.WebInterfaceUrl

        /// <summary>
        /// 上传火车票服务监控数据
        /// </summary>
        /// <param name="watch"></param>
        public static void UpLoadMachineWatch(MachineWatch watch)
        {
            try
            {
                var source = HttpHelper.Post(Path + "ServerControl/ticketServerWatch.ashx", "type=machine&data=" + Newtonsoft.Json.JsonConvert.SerializeObject(watch), "UTF-8");

                if (source != "success")
                {
                    Log.Logger.Warn("上传机器数据成功 但是处理失败:" + source);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("上传机器数据失败" + ex.Message);
            }
        }

        /// <summary>
        /// 上传进行监控明细数据
        /// </summary>
        /// <param name="data">监控数据</param>
        /// <param name="key">数据组描述</param>
        public static void UpLoadMachineWatch(string data,string type)
        {
            try
            {
                string tempPath=Path + "ServerControl/ticketServerWatch.ashx";

                Console.WriteLine(tempPath);

                var source = HttpHelper.Post(tempPath, "type=" + type + "&data=" + data, "UTF-8");

                if (source != "success")
                {
                    Log.Logger.Warn("上传机器数据成功 但是处理失败:" + source);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("上传机器数据失败" + ex.Message);
            }
        }


        /// <summary>
        /// 上传机器监控数据明细
        /// </summary>
        /// <param name="watch"></param>
        public static void UpLoadWatchDetail(string data)
        {
            try
            {

                var source = HttpHelper.Post(Path + "ServerControl/ticketServerWatch.ashx", "type=detail&data=" + data + "&machine=" + ServerWatch.WatchGuid,"UTF-8");

                if (source != "success")
                {
                    Log.Logger.Warn("上传机器数据成功 但是处理失败:" + source);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("上传机器数据失败" + ex.Message);
            }
        }

        /// <summary>
        /// 获取控制指令
        /// </summary>
        /// <returns></returns>
        public static SystemInstruct GetControlInstruct()
        {
            try
            {
                var source = HttpHelper.Post(Path + "ServerControl/ticketServerWatch.ashx", "type=getControl&machine=" + ServerWatch.WatchGuid);

                try
                {
                    SystemInstruct instruct = Newtonsoft.Json.JsonConvert.DeserializeObject<SystemInstruct>(source);

                    return instruct;
                }
                catch
                {
                    Log.Logger.Warn("读取机器数据失败" + source);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("读取机器数据失败" + ex.Message);
            }
            return new SystemInstruct { };
        }
    }
}
