using CommonLib.DataUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Core
{
    /// <summary>
    /// GNSS信息数据库操作类
    /// </summary>
    public class DataService_GnssInfo
    {
        private readonly OracleProvider provider = new OracleProvider("ORCL", null); //Oracle操作基础类

        /// <summary>
        /// 根据大机ID更新大机的GNSS信息
        /// </summary>
        /// <returns></returns>
        public int UpdateClaimerGnssInfo(string claimerId, double lat, string lat_dir, double lon, string lon_dir, double alt)
        {
            string sqlString = string.Format("update t_gps_time_test set longitude = {1}, latitude = {2}, longitude_direction = '{3}', latitude_direction = '{4}', antenna_altitude = {5}, time = sysdate where claimer_id = '{0}'", claimerId, lon, lat, lon_dir, lat_dir, alt);
            return this.provider.ExecuteSql(sqlString);
        }
    }
}
