using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Core
{
    /// <summary>
    /// OPC功能包装类
    /// </summary>
    public class OpcUtilHelper
    {
        #region 私有变量
        private bool is_groups_active = true, is_group_active = true, is_group_subscribed = true; //OPC组集合活动状态，OPC组激活状态、订阅状态
        private float groups_deadband = 0; //OPC组集合不敏感区
        private int group_update_rate = 250; //OPC组更新速度
        private const string DEFAULT_GROUP_NAME = "OPCDOTNETGROUP"; //默认OPC组的名称
        #endregion

        #region 公共属性
        /// <summary>
        /// OPC重连线程
        /// </summary>
        public Thread Thread_Reconn { get; private set; }

        /// <summary>
        /// 是否重连
        /// </summary>
        public bool ReconnEnabled { get; set; }

        /// <summary>
        /// OPC服务
        /// </summary>
        public OPCServer OpcServer { get; set; }

        /// <summary>
        /// OPC服务IP
        /// </summary>
        public string OpcServerIp { get; set; }

        /// <summary>
        /// OPC服务名称
        /// </summary>
        public string OpcServerName { get; set; }

        /// <summary>
        /// 默认OPC组信息
        /// </summary>
        public OpcGroupInfo DefaultGroupInfo { get; set; }

        /// <summary>
        /// OPC组信息List，包含OPC组名称，OPC项信息等信息，OPCServer连接前设置此属性可在连接时（ConnectRemoteServer方法）直接添加组
        /// </summary>
        public List<OpcGroupInfo> ListGroupInfo { get; set; }

        /// <summary>
        /// OPC读取速率（毫秒）
        /// </summary>
        public int OpcUpdateRate { get; set; }

        /// <summary>
        /// 标签名称_默认
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务连接状态
        /// </summary>
        public bool OpcConnected { get; set; }

        /// <summary>
        /// 客户端句柄_默认
        /// </summary>
        public int ItemHandleClient { get; set; }

        /// <summary>
        /// 服务端句柄_默认
        /// </summary>
        public int ItemHandleServer { get; set; }

        /// <summary>
        /// OPC组集合活动状态
        /// </summary>
        public bool IsGroupsActive
        {
            get { return this.is_groups_active; }
            set
            {
                this.is_groups_active = value;
                this.OpcServer.OPCGroups.DefaultGroupIsActive = is_groups_active;
            }
        }

        /// <summary>
        /// OPC组集合不敏感区
        /// </summary>
        public float GroupsDeadband
        {
            get { return this.groups_deadband; }
            set
            {
                this.groups_deadband = value;
                this.OpcServer.OPCGroups.DefaultGroupDeadband = groups_deadband;
            }
        }

        /// <summary>
        /// OPC组激活状态
        /// </summary>
        public bool IsGroupActive
        {
            get { return this.is_group_active; }
            set
            {
                this.is_group_active = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsActive = this.is_group_active);
            }
        }

        /// <summary>
        /// OPC组订阅状态
        /// </summary>
        public bool IsGroupSubscribed
        {
            get { return this.is_group_subscribed; }
            set
            {
                this.is_group_subscribed = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.IsSubscribed = this.is_group_subscribed);
            }
        }

        /// <summary>
        /// OPC组更新速度
        /// </summary>
        public int GroupUpdateRate
        {
            get { return this.group_update_rate; }
            set
            {
                this.group_update_rate = value;
                this.ListGroupInfo.ForEach(groupInfo => groupInfo.OpcGroup.UpdateRate = this.group_update_rate);
            }
        }

        /// <summary>
        /// OPC服务启动时间
        /// </summary>
        public string ServerStartTime { get; set; }

        /// <summary>
        /// OPC服务版本
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// OPC服务状态
        /// </summary>
        public string ServerState { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="updateRate">OPC读取速率（毫秒）</param>
        /// <param name="reconn_enabled">是否重连</param>
        public OpcUtilHelper(int updateRate, bool reconn_enabled)
        {
            this.ReconnEnabled = reconn_enabled;
            this.OpcServer = new OPCServer();
            this.OpcUpdateRate = updateRate;
            this.ListGroupInfo = new List<OpcGroupInfo>();
            this.ItemId = string.Empty;
        }

        /// <summary>
        /// 构造器，OPC读取速率1000毫秒，默认不重连
        /// </summary>
        public OpcUtilHelper() : this(1000, false) { }

        /// <summary>
        /// 更新OPC服务信息，包括启动时间、版本与状态
        /// </summary>
        public void UpdateServerInfo()
        {
            this.ServerStartTime = this.OpcServer == null ? string.Empty : string.Format("启动时间:{0}", this.OpcServer.StartTime.ToString());
            this.ServerVersion = this.OpcServer == null ? string.Empty : string.Format("版本:{0}.{1}.{2}", this.OpcServer.MajorVersion, this.OpcServer.MinorVersion, this.OpcServer.BuildNumber);
            this.ServerState = this.OpcServer == null ? string.Empty : (this.OpcServer.ServerState == (int)OPCServerState.OPCRunning ? string.Format("已连接:{0}", this.OpcServer.ServerName) : string.Format("状态：{0}", this.OpcServer.ServerState.ToString()));
        }

        /// <summary>
        /// OPC服务枚举
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public string[] ServerEnum(string ipAddress, out string message)
        {
            Array array = null;
            message = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    message = "IP地址为空";
                    return null;
                }

                if (this.OpcServer == null)
                    this.OpcServer = new OPCServer();
                array = (Array)(object)this.OpcServer.GetOPCServers(ipAddress);
            }
            //假如获取OPC Server过程中引发COMException，即代表无法连接此IP的OPC Server
            catch (Exception ex) { message = "无法连接此IP地址的OPC Server！" + ex.Message; }
            return array == null ? null : array.Cast<string>().ToArray();
        }

        /// <summary>
        /// 连接OPC服务器，连接成功后刷新OPC服务信息并创建默认组，同时根据ListGroupInfo属性（OPC组信息List）创建OPC组
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        /// <param name="message">返回的错误消息</param>
        /// <returns></returns>
        public bool ConnectRemoteServer(string remoteServerIP, string remoteServerName, out string message)
        {
            message = string.Empty;
            try
            {
                this.OpcServer.Connect(remoteServerName, remoteServerIP);
                this.OpcServerName = remoteServerName;
                this.OpcServerIp = remoteServerIP;
                this.OpcConnected = true;
                this.UpdateServerInfo(); //刷新OPC服务信息
                this.SetGroupsProperty(this.IsGroupsActive, this.GroupsDeadband); //设置组集合属性
                this.CreateDefaultGroup(out message); //创建默认OPC组
                //TODO 根据对象自身具有的OPC组信息List创建OPC组，假如连接前未在ListGroupInfo属性中设置OPC组信息，则在连接后用CreateGroups方法创建OPC组
                this.CreateGroups(this.ListGroupInfo, out message);
                //try { if (this.Thread_Reconn != null) this.Thread_Reconn.Abort(); }
                //catch (Exception e) { }
                //假如线程为空，初始化重连线程；假如线程不为空，则线程已经开始运行
                if (this.Thread_Reconn == null)
                {
                    this.Thread_Reconn = new Thread(new ThreadStart(this.Reconn_Recursive)) { IsBackground = true };
                    this.Thread_Reconn.Start();
                }
            }
            catch (Exception ex)
            {
                message = "连接远程服务器出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 与OPC服务断开
        /// </summary>
        public void DisconnectRemoteServer()
        {
            if (this.Thread_Reconn != null)
                this.Thread_Reconn.Abort();
            if (!this.OpcConnected)
                return;

            if (this.OpcServer != null)
            {
                this.OpcServer.OPCGroups.RemoveAll();
                this.OpcServer.Disconnect();
                this.OpcServer = null;
                this.ListGroupInfo.ForEach(g => g.Dispose());
                this.ListGroupInfo.Clear();
            }

            this.OpcConnected = false;
        }

        /// <summary>
        /// 循环连接OPC，用于重连OPC线程
        /// </summary>
        private void Reconn_Recursive()
        {
            string info;
            while (true)
            {
                if (!this.ReconnEnabled)
                    break;
                Thread.Sleep(5000);
                this.Reconn(out info);
            }
        }

        /// <summary>
        /// 重新连接OPC，返回连接信息
        /// </summary>
        public void Reconn(out string info)
        {
            info = string.Empty;
            try
            {
                if (this.OpcServer.ServerState != (int)OPCServerState.OPCRunning)
                    this.ReconnDetail(out info);
            }
            //假如捕捉到COMException
            catch (COMException)
            {
                try { this.ReconnDetail(out info); }
                catch { }
            }
            catch (Exception e) { info = string.Format("准备重连OPC服务{0} (IP {1}) 时出现异常: {2}", this.OpcServerName, this.OpcServerIp, e.Message); }
        }

        /// <summary>
        /// 重新连接OPC
        /// </summary>
        /// <param name="info">返回信息</param>
        public void ReconnDetail(out string info)
        {
            info = string.Empty;
            try
            {
                this.OpcServer = new OPCServer();
                info = string.Format("OPC服务{0} (IP {1}) 连接失败，尝试重连", this.OpcServerName, this.OpcServerIp);
                this.ConnectRemoteServer(this.OpcServerIp, this.OpcServerName, out info);
                //this.OpcServer.Connect(this.OpcServerName, this.OpcServerIp);
                info = string.Format("OPC服务{0} (IP {1}) 重连成功", this.OpcServerName, this.OpcServerIp);
                //this.OpcServer.OPCGroups.RemoveAll();
                //if (this.CreateDefaultGroup(out info))
                //    info = string.Format("OPC服务{0} (IP {1}) 的OPC组创建成功", this.OpcServerName, this.OpcServerIp);
            }
            catch (Exception e) { info = string.Format("OPC服务{0} (IP {1}) 重连失败: {2}", this.OpcServerName, this.OpcServerIp, e.Message); }
        }

        /// <summary>
        /// 创建默认OPC组
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool CreateDefaultGroup(out string message)
        {
            message = string.Empty;
            try
            {
                try { this.OpcServer.OPCGroups.Remove(DEFAULT_GROUP_NAME); } catch (Exception e) { } //试着移除已存在组
                this.DefaultGroupInfo = new OpcGroupInfo(this.OpcServer.OPCGroups, DEFAULT_GROUP_NAME);
                this.DefaultGroupInfo.SetGroupProperty(this.GroupUpdateRate, this.IsGroupActive, this.IsGroupSubscribed);
            }
            catch (Exception ex)
            {
                message = "创建组出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据若干个OPC组信息创建OPC组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CreateGroups(IEnumerable<OpcGroupInfo> groups, out string message)
        {
            message = string.Empty;
            if (groups == null || groups.Count() == 0)
            {
                message = "未提供任何OPC组信息，无法创建OPC组";
                return false;
            }
            try
            {
                List<OpcGroupInfo> groupList = groups.ToList(); //转换为新List对象，防止枚举改变对象时出现未知影响
                foreach (var groupInfo in groupList)
                {
                    if (groupInfo == null)
                        continue;
                    string name = groupInfo.GroupName; //OPC组名称
                    List<OpcItemInfo> itemInfos = groupInfo.ListItemInfo; //OPC项信息集合
                    try { this.OpcServer.OPCGroups.Remove(name); } catch (Exception) { } //试着移除已存在组
                    //初始化OPC组信息并设置OPC组属性、添加OPC项
                    OpcGroupInfo group = new OpcGroupInfo(this.OpcServer.OPCGroups, name);
                    group.SetGroupProperty(this.GroupUpdateRate, this.IsGroupActive, this.IsGroupSubscribed);
                    //TODO 假如OPC组信息中已设置OPC项信息，则根据这些OPC项信息添加OPC项，否则创建组之后调用SetTags方法
                    group.SetItems(itemInfos, out message);

                    //假如List中已存在，则移除
                    if (this.ListGroupInfo.Contains(groupInfo))
                    {
                        this.ListGroupInfo.Remove(groupInfo);
                        groupInfo.Dispose();
                    }
                    this.ListGroupInfo.Add(group);
                }
            }
            catch (Exception ex)
            {
                message = "创建组出现错误：" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 创建给定名字的OPC组，不添加OPC项
        /// </summary>
        /// <param name="groupNames">待创建的OPC组名称</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>
        public bool CreateGroups(IEnumerable<string> groupNames, out string message)
        {
            IEnumerable<OpcGroupInfo> groupInfos = groupNames == null ? null : groupNames.Select(n => new OpcGroupInfo(null, n));
            return this.CreateGroups(groupInfos, out message);
        }

        /// <summary>
        /// 设置组集合属性
        /// </summary>
        /// <param name="isGroupsActive">OPC组集合活动状态</param>
        /// <param name="deadband">OPC组集合不敏感区</param>
        public void SetGroupsProperty(bool isGroupsActive, float deadband)
        {
            if (this.OpcServer.OPCGroups != null)
            {
                this.OpcServer.OPCGroups.DefaultGroupIsActive = isGroupsActive;
                this.OpcServer.OPCGroups.DefaultGroupDeadband = deadband;
            }
        }

        /// <summary>
        /// 设置默认的OPC项，假如已添加，则移除后再重新添加（同一时刻默认标签只有一个）
        /// </summary>
        /// <param name="itemId">标签ID</param>
        /// <param name="clientHandle">标签的客户端句柄</param>
        /// <param name="message">返回的错误信息</param>
        /// <returns></returns>
        public bool SetItem(string itemId, int clientHandle, out string message)
        {
            try
            {
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                //初始化OPC项信息并在默认OPC组中添加
                List<OpcItemInfo> list = new List<OpcItemInfo>() { new OpcItemInfo(itemId, clientHandle) };
                this.DefaultGroupInfo.SetItems(list, out message);
                if (this.DefaultGroupInfo.ItemCount > 0)
                {
                    OpcItemInfo item = this.DefaultGroupInfo.ListItemInfo.Last();
                    //保存默认OPC项的客户端句柄，服务端句柄，标签名称
                    this.ItemHandleClient = item.ClientHandle;
                    this.ItemId = item.ItemId;
                    this.ItemHandleServer = item.ServerHandle;
                }
            }
            catch (Exception ex)
            {
                this.ItemHandleClient = 0;
                message = "移除或添加标签时发生错误:" + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从默认的OPC项读取值
        /// </summary>
        /// <param name="value">待读取的值</param>
        /// <param name="message">返回的错误信息</param>
        public bool ReadItemValue(out string value, out string message)
        {
            value = string.Empty;
            try
            {
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (!this.DefaultGroupInfo.ReadValues(out message))
                    return false;
                if (this.DefaultGroupInfo.ItemCount > 0)
                    value = this.DefaultGroupInfo.ListItemInfo.Last().Value;
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("从服务端句柄为{0}、标签ID为{1}的标签读取值失败：{2}", this.ItemHandleServer, this.ItemId, ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向默认的OPC项写入值
        /// </summary>
        /// <param name="value">待写入的值</param>
        /// <param name="message">返回的错误信息</param>
        public bool WriteItemValue(string value, out string message)
        {
            try
            {
                if (this.DefaultGroupInfo == null)
                {
                    message = "未找到默认组";
                    return false;
                }

                if (this.DefaultGroupInfo.ItemCount > 0)
                    this.DefaultGroupInfo.ListItemInfo.Last().Value = value;
                if (!this.DefaultGroupInfo.WriteValues(out message))
                    return false;
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("向服务端句柄为{0}的标签写入值{1}失败：{2}", this.ItemHandleServer, value, ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从对应指定客户端句柄的指定OPC项读取值（先根据OPC项ID与客户端句柄添加OPC项，然后再读取）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool ReadOpc(string itemName, int clientHandle, out string value, out string message)
        {
            value = string.Empty;
            return SetItem(itemName, clientHandle, out message) && ReadItemValue(out value, out message);
        }

        /// <summary>
        /// 向对应指定客户端句柄的指定OPC项写入值（先根据OPC项ID与客户端句柄添加OPC项，然后再写入）
        /// </summary>
        /// <param name="itemName">标签ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        /// <param name="value">待写入值</param>
        /// <param name="message">返回的错误信息</param>
        public bool WriteOpc(string itemName, int clientHandle, string value, out string message)
        {
            return SetItem(itemName, clientHandle, out message) && WriteItemValue(value, out message);
        }
    }

    /// <summary>
    /// OPC项信息实体类，每个OPC项信息实体对应单独的OPC项ID、服务端句柄、客户端句柄以及值（供读取或写入）
    /// 可根据这些信息为OPC组添加OPC项（OpcGroupInfo中的SetItems方法）
    /// </summary>
    public class OpcItemInfo
    {
        private int client_handle;

        /// <summary>
        /// OPC项ID（名称）
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// OPC服务端句柄
        /// </summary>
        public int ServerHandle { get; set; }

        /// <summary>
        /// OPC客户端句柄
        /// </summary>
        public int ClientHandle
        {
            get { return this.client_handle; }
            set { this.client_handle = value; }
        }

        /// <summary>
        /// 读取或待写入的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="itemId">OPC项ID</param>
        /// <param name="clientHandle">客户端句柄</param>
        public OpcItemInfo(string itemId, int clientHandle)
        {
            this.ItemId = itemId;
            this.ClientHandle = clientHandle;
            this.Value = string.Empty;
        }
    }

    /// <summary>
    /// OPC组信息实体类，每个OPC组信息实体对应单独的OPCGroup基本信息、标签ID Array、服务端句柄Array、客户端句柄Array以及OPC项信息List（供添加OPC项或添加OPC项后保存信息）
    /// 可根据这些信息为OPC服务添加OPC组（OpcUtilHelper中的CreateGroups方法）
    /// </summary>
    public class OpcGroupInfo : IDisposable
    {
        #region 私有变量
        private Array item_ids, server_handles, client_handles, errors;
        private readonly OpcItemInfo opc_pack_basic = new OpcItemInfo(string.Empty, 0);
        #endregion

        #region 属性
        /// <summary>
        /// OPCGroup对象
        /// </summary>
        public OPCGroup OpcGroup { get; private set; }

        /// <summary>
        /// OPC组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// OPC组所拥有的OPC项数量
        /// </summary>
        public int ItemCount { get { return this.OpcGroup == null ? 0 : this.OpcGroup.OPCItems.Count; } }

        /// <summary>
        /// OPC项ID Array，添加OPC项时变化
        /// </summary>
        public Array ItemIds
        {
            get { return this.item_ids; }
            private set { this.item_ids = value; }
        }

        /// <summary>
        /// OPC项服务端句柄 Array，添加、移除OPC项时变化
        /// </summary>
        public Array ServerHandles
        {
            get { return this.server_handles; }
            private set { this.server_handles = value; }
        }

        /// <summary>
        /// OPC项客户端句柄 Array，添加OPC项时变化
        /// </summary>
        public Array ClientHandles
        {
            get { return this.client_handles; }
            private set { this.client_handles = value; }
        }

        /// <summary>
        /// 错误信息Array，添加、移除OPC项，读取、写入值时变化
        /// </summary>
        public Array Errors
        {
            get { return this.errors; }
            set { this.errors = value; }
        }

        /// <summary>
        /// OPC项信息List，包含OPC项ID、客户端句柄、服务端句柄、值等信息，添加OPC组前设置此属性可在添加OPC组（OpcUtilHelper.CreateGroups方法）时直接添加OPC项
        /// </summary>
        public List<OpcItemInfo> ListItemInfo { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="groups">待创建的OPC组所属于的组集合，为null则无法创建</param>
        /// <param name="name">OPC组名称</param>
        public OpcGroupInfo(OPCGroups groups, string name)
        {
            this.GroupName = name;
            this.ListItemInfo = new List<OpcItemInfo>();
            if (groups == null)
                return;

            this.OpcGroup = groups.Add(name);
        }

        /// <summary>
        /// 设置OPC组属性
        /// </summary>
        /// <param name="updateRate">OPC组更新速度</param>
        /// <param name="isGroupActive">OPC组激活状态</param>
        /// <param name="isGroupSubscribed">OPC组订阅状态</param>
        public void SetGroupProperty(int updateRate, bool isGroupActive, bool isGroupSubscribed)
        {
            if (this.OpcGroup != null)
            {
                this.OpcGroup.UpdateRate = updateRate;
                this.OpcGroup.IsActive = isGroupActive;
                this.OpcGroup.IsSubscribed = isGroupSubscribed;
            }
        }

        /// <summary>
        /// 获取所有OPC项的值并转换为Array
        /// </summary>
        /// <returns></returns>
        private Array GetValues()
        {
            return this.GetValues(null);
        }

        /// <summary>
        /// 获取在给定服务端句柄中存在对应OPC项的值并转换为Array，假如给定的服务端句柄为空，则给出所有值
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有值</param>
        /// <returns></returns>
        private Array GetValues(IEnumerable<int> serverHandles)
        {
            bool flag = serverHandles == null || serverHandles.Count() == 0;
            return this.ListItemInfo == null ? new object[0] : this.ListItemInfo.Select((item, index) => new { item, index }).Where(p => (flag || serverHandles.Contains(p.item.ServerHandle)) || p.index == 0).Select(p => (object)p.item.Value).ToArray();
            //if (serverHandles == null || serverHandles.Count() == 0)
            //    return this.ListItemInfo == null ? new object[0] : this.ListItemInfo.Select(p => (object)p.Value).ToArray();
            //else
            //    return this.ListItemInfo == null ? new object[0] : this.ListItemInfo.Where(item => serverHandles.Contains(item.ServerHandle)).Select(p => (object)p.Value).ToArray();
        }

        /// <summary>
        /// 获取在给定服务端句柄中存在对应OPC项的服务端句柄并转换为Array，假如给定的服务端句柄为空，则给出所有服务端句柄
        /// </summary>
        /// <param name="serverHandles">给定的服务端句柄数组，假如给定的服务端句柄为空，则给出所有服务端句柄</param>
        /// <returns></returns>
        private Array GetServerHandles(IEnumerable<int> serverHandles)
        {
            bool flag = serverHandles == null || serverHandles.Count() == 0;
            return this.ListItemInfo == null ? new int[0] : this.ListItemInfo.Select((item, index) => new { item, index }).Where(p => (flag || serverHandles.Contains(p.item.ServerHandle)) || p.index == 0).Select(p => p.item.ServerHandle).ToArray();
            //if (serverHandles == null || serverHandles.Count() == 0)
            //    return this.ListItemInfo == null ? new int[0] : this.ListItemInfo.Select(item => item.ServerHandle).ToArray();
            //else
            //    return this.ListItemInfo == null ? new int[0] : this.ListItemInfo.Where(item => serverHandles.Contains(item.ServerHandle)).Select(p => p.ServerHandle).ToArray();
        }

        /// <summary>
        /// 根据给定的OPC项集合信息添加OPC项
        /// </summary>
        /// <param name="items">给出添加OPC项时所需信息的集合</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool SetItems(IEnumerable<OpcItemInfo> items, out string message)
        {
            message = string.Empty;
            if (items == null || items.Count() == 0)
                return false;

            try
            {
                //假如已存在OPC项，先移除
                if (this.ItemCount > 0)
                    this.OpcGroup.OPCItems.Remove(this.ItemCount, ref this.server_handles, out this.errors);
                List<OpcItemInfo> itemList = items.ToList(); //转换为新List对象，防止枚举改变对象时出现未知影响
                this.ListItemInfo.Clear();
                this.ListItemInfo.Add(this.opc_pack_basic);
                itemList.Insert(0, this.opc_pack_basic);
                this.item_ids = itemList.Select(p => p.ItemId).ToArray();
                this.client_handles = itemList.Select(p => p.ClientHandle).ToArray();
                this.OpcGroup.OPCItems.AddItems(itemList.Count - 1, ref this.item_ids, ref this.client_handles, out this.server_handles, out this.errors);
                //添加OPC项后根据ID找到OPC项信息对象并设置服务端句柄，向OPC项信息List中添加
                if (this.item_ids.Length > 1)
                    for (int i = 1; i < this.item_ids.Length; i++)
                    {
                        OpcItemInfo itemInfo = itemList.Find(p => p.ItemId.Equals(this.item_ids.GetValue(i)));
                        itemInfo.ServerHandle = int.Parse(this.server_handles.GetValue(i).ToString());
                        this.ListItemInfo.Add(itemInfo);
                    }
                //假如添加后的数量对不上，则至少有一个OPC项未添加成功
                if (this.ListItemInfo.Count < itemList.Count)
                    message = "至少有1个OPC项未添加成功";
            }
            catch (Exception ex)
            {
                message = string.Format("OPC组{0}移除或添加标签时发生错误:{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return this.ListItemInfo.Count > 1; //假如至少有1个添加成功，返回true
        }

        /// <summary>
        /// 为OPC组的所有OPC项读取数据
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool ReadValues(out string message)
        {
            //return this.ReadValues(this.server_handles, out message);
            return this.ReadValues(null, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内给定数量的、与给定服务端句柄对应的OPC项读取数据
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="itemCount">读取的OPC项数量</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool ReadValues(Array serverHandles, /*int itemCount, */out string message)
        {
            message = string.Empty;
            object qualities, timeStamps;
            Array values;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = this.GetServerHandles(temp);
                int itemCount = handles.Length - 1;
                this.OpcGroup.SyncRead((short)OPCDataSource.OPCDevice, itemCount, ref handles, out values, out this.errors, out qualities, out timeStamps);
                //假如至少读取到1个值，根据服务端句柄找到OPC项信息并更新值
                if (values.Length > 0)
                    for (int i = 1; i <= values.Length; i++)
                    {
                        OpcItemInfo itemInfo = this.ListItemInfo.Find(item => item.ServerHandle.Equals(handles.GetValue(i)));
                        if (itemInfo != null)
                            itemInfo.Value = values.GetValue(i).ToString();
                    }
                if (values.Length < itemCount)
                    message = "至少有1个OPC项的值未找到";
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("从名称为{0}的OPC组读取值失败：{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return values.Length > 0;
        }

        /// <summary>
        /// 为OPC组的所有OPC项写入数据
        /// </summary>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(out string message)
        {
            //return this.WriteValues(this.server_handles, out message);
            return this.WriteValues(null, out message);
        }

        /// <summary>
        /// 为OPC组OPC项List内给定数量的、与给定服务端句柄对应的OPC项写入数据
        /// </summary>
        /// <param name="serverHandles">服务端句柄Array</param>
        /// <param name="itemCount">写入的OPC项数量</param>
        /// <param name="message">返回信息</param>
        /// <returns></returns>
        public bool WriteValues(Array serverHandles, out string message)
        {
            message = string.Empty;
            try
            {
                IEnumerable<int> temp = serverHandles == null || serverHandles.Length == 0 ? null : serverHandles.Cast<int>();
                Array handles = this.GetServerHandles(temp), values = this.GetValues(temp);
                int itemCount = handles.Length - 1;
                this.OpcGroup.SyncWrite(itemCount, ref handles, ref values, out this.errors);
                GC.Collect();
            }
            catch (Exception ex)
            {
                message = string.Format("向名称为{0}的OPC组写入值失败：{1}", this.OpcGroup.Name, ex.Message);
                return false;
            }
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">为true时释放所有资源，否则仅释放非托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.OpcGroup = null;
                    this.ItemIds = null;
                    this.ClientHandles = null;
                    this.Errors = null;
                    this.ServerHandles = null;
                    this.ListItemInfo.Clear();
                    this.ListItemInfo = null;
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~OpcGroupInfo()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
