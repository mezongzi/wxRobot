﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using wxRobot.Https;

namespace wxRobot.Services
{
    public class WXServices
    {
        private static Dictionary<string, string> _syncKey = new Dictionary<string, string>();
        /// <summary>
        /// 微信初始化url
        /// </summary>

        private static string _init_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=884513456464";
        
        /// <summary>
        /// 发送消息url
        /// </summary>
        private static string _sendmsg_url = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg?sid=";

        /// <summary>
        /// 上传文件
        /// </summary>
        private static string _uplpadFileUrl = "https://file.wx.qq.com/cgi-bin/mmwebwx-bin/webwxuploadmedia?f=json";

        /// <summary>
        /// 发送图片消息
        /// </summary>
        private static string _sendmsgimg = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsgimg?fun=async&f=json";

        /// <summary>
        /// 发送视频消息
        /// </summary>
        private static string _sendvideomsg = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsendvideomsg?fun=async&f=json";

        /// <summary>
        /// 微信初始化
        /// </summary>
        /// <returns></returns>
        public JObject WxInit()
        {
            string init_json = "{{\"BaseRequest\":{{\"Uin\":\"{0}\",\"Sid\":\"{1}\",\"Skey\":\"{2}\",\"DeviceID\":\"e1615250492\"}}}}";
            Cookie sid = HttpServer.GetCookie("wxsid");
            Cookie uin = HttpServer.GetCookie("wxuin");

            if (sid != null && uin != null)
            {
                init_json = string.Format(init_json, uin.Value, sid.Value,LoginService.SKey);
                byte[] bytes = HttpServer.SendPostRequest(_init_url + "&lang=zh_CN&pass_ticket=" + LoginService.Pass_Ticket, init_json);
                string init_str = Encoding.UTF8.GetString(bytes);

                JObject init_result = JsonConvert.DeserializeObject(init_str) as JObject;

                foreach (JObject synckey in init_result["SyncKey"]["List"])  //同步键值
                {
                    _syncKey.Add(synckey["Key"].ToString(), synckey["Val"].ToString());
                }
                return init_result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="msg">内容</param>
        /// <param name="from">来</param>
        /// <param name="to">到</param>
        /// <param name="type">类型(文本为1)</param>
        public void SendMessage(string msg, string from, string to, int type)
        {
            string msg_json = "{{" +
           "\"BaseRequest\":{{" +
               "\"DeviceID\" : \"e441551176\"," +
               "\"Sid\" : \"{0}\"," +
               "\"Skey\" : \"{6}\"," +
               "\"Uin\" : \"{1}\"" +
           "}}," +
           "\"Msg\" : {{" +
               "\"ClientMsgId\" : {8}," +
               "\"Content\" : \"{2}\"," +
               "\"FromUserName\" : \"{3}\"," +
               "\"LocalID\" : {9}," +
               "\"ToUserName\" : \"{4}\"," +
               "\"Type\" : {5}" +
           "}}," +
           "\"rr\" : {7}" +
           "}}";

            Cookie sid = HttpServer.GetCookie("wxsid");
            Cookie uin = HttpServer.GetCookie("wxuin");

            if (sid != null && uin != null)
            {
                msg_json = string.Format(msg_json, sid.Value, uin.Value, msg, from, to, type, LoginService.SKey, DateTime.Now.Millisecond, DateTime.Now.Millisecond, DateTime.Now.Millisecond);

                byte[] bytes = HttpServer.SendPostRequest(_sendmsg_url + sid.Value + "&lang=zh_CN&pass_ticket=" + LoginService.Pass_Ticket, msg_json);

                string send_result = Encoding.UTF8.GetString(bytes);
            }
        }


        public void SendVideo(string MediaId, string from, string to)
        {
           string msg_json= "{{\"BaseRequest\":{{\"Uin\":{0},\"Sid\":\"{1}\",\"Skey\":\"{2}\",\"DeviceID\":\"e513037603731077\"}},"+
                "\"Msg\":{{"+
                "\"Type\":43,"+
                "\"MediaId\":\"{3}\"," +
                "\"Content\":\"\"," +
                "\"FromUserName\":\"{4}\"," +
                "\"ToUserName\":\"{5}\"," +
                "\"LocalID\":\"{6}\"," +
                "\"ClientMsgId\":\"{7}\"}}," + 
                "\"Scene\":0"+
                "}}";
            Cookie sid = HttpServer.GetCookie("wxsid");
            Cookie uin = HttpServer.GetCookie("wxuin");
            if (sid != null && uin != null)
            {
                msg_json = string.Format(msg_json, uin.Value,sid.Value, LoginService.SKey, MediaId, from, to, DateTime.Now.Millisecond, DateTime.Now.Millisecond);

                byte[] bytes = HttpServer.SendPostRequest(_sendvideomsg + "&lang=zh_CN&pass_ticket=" + LoginService.Pass_Ticket, msg_json);
                string send_result = Encoding.UTF8.GetString(bytes);
            }


        }

    }
}
