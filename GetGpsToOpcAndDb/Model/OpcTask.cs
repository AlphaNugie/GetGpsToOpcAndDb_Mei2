using CommonLib.Clients.Tasks;
using CommonLib.Extensions.Property;
using CommonLib.Function;
using GetGpsToOpcAndDb.Core;
using OpcLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GetGpsToOpcAndDb.Model
{

    public class OpcTask : Task
    {
        private readonly OpcUtilHelper _opcHelper = new OpcUtilHelper(1000, true);

        /// <summary>
        /// OPC操作对象
        /// </summary>
        public OpcUtilHelper OpcHelper { get { return _opcHelper; } }

        ///// <summary>
        ///// 最新错误信息
        ///// </summary>
        //public string LastErrorMessage { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public OpcTask() : base() { }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Init()
        {
            Interval = BaseConst.OpcUpdateRate;
            //if (this.Shiploader == null)
            //    _errorMessage = "装船机信息为空，OPC服务无法初始化";
            if (string.IsNullOrWhiteSpace(BaseConst.OpcServerName))
                _errorMessage = "OPC服务端的名称为空";

            OpcInit();
            SetOpcGroupsDataSource();
        }

        /// <summary>
        /// 循环体内容
        /// </summary>
        public override void LoopContent()
        {
            Interval = BaseConst.OpcUpdateRate;
            if (BaseConst.PostureMode == PostureMode.Beidou)
            {
                BaseConst.OpcDataSource.CopyPropertyValueFrom(BaseConst.GnssInfo);
                OpcWriteValues();
            }
            else if (BaseConst.PostureMode == PostureMode.OPC)
            {
                OpcReadValues();
                GnssInfoObject info = BaseConst.GnssInfo;
                BaseConst.OpcDataSource.CopyPropertyValueTo(ref info);
                BaseConst.GnssInfo = info;
            }
        }

        /// <summary>
        /// OPC初始化
        /// </summary>
        private void OpcInit()
        {
            BaseConst.Log.WriteLogsToFile(string.Format("开始连接IP地址为{0}的OPC SERVER {1}...", BaseConst.OpcServerIp, BaseConst.OpcServerName));
            string[] servers = _opcHelper.ServerEnum(BaseConst.OpcServerIp, out _errorMessage);
            if (!string.IsNullOrWhiteSpace(_errorMessage))
            {
                BaseConst.Log.WriteLogsToFile(string.Format("枚举过程中出现问题：{0}", _errorMessage));
                goto END_OF_OPC;
            }
            if (servers == null || !servers.Contains(BaseConst.OpcServerName))
            {
                BaseConst.Log.WriteLogsToFile(string.Format("无法找到指定OPC SERVER：{0}", BaseConst.OpcServerName));
                goto END_OF_OPC;
            }
            DataTable table = new DataService_Opc().GetOpcInfo();
            if (table == null || table.Rows.Count == 0)
            {
                BaseConst.Log.WriteLogsToFile(string.Format("在表中未找到任何OPC记录，将不进行读取或写入", BaseConst.OpcServerName));
                goto END_OF_OPC;
            }
            List<OpcGroupInfo> groups = new List<OpcGroupInfo>();
            List<DataRow> dataRows = table.Rows.Cast<DataRow>().ToList();
            List<OpcItemInfo> items = null;
            int id = 0;
            foreach (var row in dataRows)
            {
                string itemId = row["item_id"].ConvertType<string>();
                if (string.IsNullOrWhiteSpace(itemId))
                    continue;
                int groupId = row["group_id"].ConvertType<int>(), clientHandle = row["record_id"].ConvertType<int>();
                string groupName = row["group_name"].ConvertType<string>(), fieldName = row["field_name"].ConvertType<string>();
                GroupType type = (GroupType)row["group_type"].ConvertType<int>();
                if (groupId != id)
                {
                    id = groupId;
                    groups.Add(new OpcGroupInfo(null, groupName/*, OpcDatasource*/) { GroupType = type, ListItemInfo = new List<OpcItemInfo>() });
                    OpcGroupInfo groupInfo = groups.Last();
                    items = groupInfo.ListItemInfo;
                }
                items.Add(new OpcItemInfo(itemId, clientHandle, fieldName));
            }
            _opcHelper.ListGroupInfo = groups;
            _opcHelper.ConnectRemoteServer(BaseConst.OpcServerIp, BaseConst.OpcServerName, out _errorMessage);
            BaseConst.Log.WriteLogsToFile(string.Format("OPC连接状态：{0}", _opcHelper.OpcConnected));
            if (!string.IsNullOrWhiteSpace(_errorMessage))
                BaseConst.Log.WriteLogsToFile(string.Format("连接过程中出现问题：{0}", _errorMessage));
            END_OF_OPC:;
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        private void SetOpcGroupsDataSource()
        {
            if (_opcHelper != null && _opcHelper.ListGroupInfo != null)
                _opcHelper.ListGroupInfo.ForEach(group => group.DataSource = BaseConst.OpcDataSource);
        }

        /// <summary>
        /// 读取值
        /// </summary>
        private void OpcReadValues()
        {
            _opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.READ)
                    return;

                if (!group.ReadValues(out _errorMessage))
                    BaseConst.Log.WriteLogsToFile(string.Format("读取PLC失败，读取过程中出现问题：{0}", _errorMessage));
            });
        }

        /// <summary>
        /// 写入值
        /// </summary>
        private void OpcWriteValues()
        {
            //if (!BaseConst.WriteItemValue)
            //    return;

            _opcHelper.ListGroupInfo.ForEach(group =>
            {
                if (group.GroupType != GroupType.WRITE)
                    return;

                //BaseConst.OpcDataSource.CopyPropertyValueFrom(BaseConst.GnssInfo);
                if (!group.WriteValues(out _errorMessage))
                    BaseConst.Log.WriteLogsToFile(string.Format("写入PLC失败，写入过程中出现问题：{0}", _errorMessage));
            });
        }
    }
}
