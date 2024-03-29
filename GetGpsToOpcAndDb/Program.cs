﻿using GetGpsToOpcAndDb.Core;
using GetGpsToOpcAndDb.Model;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GetGpsToOpcAndDb
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            BaseFunc.InitConfigs();
            #region test
            //double inputd1 = 300.3;
            //string inputs1 = "400.s4";
            //double.TryParse(.TryParse(inputs1, out inputd1);
            //return;

            //double lat = 38.3579708, lon = 117.90819113, alt = 0.0897, x = 0, y = 0, z = 0;
            //BaseFunc.GpsChange(lat, lon, alt, ref x, ref y, ref z);
            //BaseFunc.GetCoordinates(lat, lon, ref x, ref y);

            ////SqlGeography graphy1 = SqlGeography.Point(38.37030552, 117.9177707, BaseConst.SystemId), graphy2 = SqlGeography.Point(38.36387604, 117.90369888, BaseConst.SystemId);
            //SqlGeography graphy1 = SqlGeography.Point(38.22218331, 117.55066242, BaseConst.SystemId), graphy2 = SqlGeography.Point(38.21832563, 117.54221933, BaseConst.SystemId);
            //double dist = graphy1.STDistance(graphy2).Value;
            //return;

            //double test = Math.Atan(0.0615568);
            //test = test * 180 / Math.PI;
            //return;

            //double x = 0, y = 0, z = -1.3965;
            //BaseFunc.GetCoordinates(38.966372, 118.453625, ref x, ref y);
            //Coordinate coor = new Coordinate();
            //coor.Update(x, y, z);
            //string result = coor.ToString("prime");
            //double t = Math.Atan(1)*180/Math.PI;
            //double lat = 38.305164, lon = 117.862279, alt = 0;
            //Coordinate local_ante = new Coordinate(), local_center = new Coordinate();
            //BaseFunc.GetLocalCoordinates(lat, lon, alt, ref local_ante, ref local_center);
            //SqlGeography geo1 = SqlGeography.Point(38.97619063, 118.45381294, BaseConst.SystemId), geo2 = SqlGeography.Point(38.97613785, 118.45374736, BaseConst.SystemId);
            //double distance = geo1.STDistance(geo2).Value;
            //Coordinate point = new Coordinate();
            //List<double[]> list = new List<double[]>() {
            //new double[] { 38.96636077, 118.45417260 },
            //new double[] { 38.96636054, 118.45406854 },
            //new double[] { 38.97758795, 118.45416477 },
            //new double[] { 38.97758735, 118.45406074 },
            //new double[] { 38.96637582, 118.45332502 },
            //new double[] { 38.96637448, 118.45318600 },
            //new double[] { 38.97750114, 118.45331807 },
            //new double[] { 38.97750316, 118.45317850 },
            //new double[] { 38.97764795, 118.45399180 },
            //new double[] { 38.96632684, 118.45399938 },
            //new double[] { 38.97765839, 118.44904021 },
            //new double[] { 38.96632501, 118.44880929 }
            //            };
            //double x = 0, y = 0;
            //foreach (var d in list)
            //{
            //    BaseFunc.GetCoordinates(d[0], d[1], ref x, ref y);
            //    point.X = x - 100;
            //    point.Y = y;
            //    Console.WriteLine(string.Format("x: {0}, y: {1}", point.XPrime, point.YPrime));
            //}
            //int test = int.Parse("+100");
            //string test = string.Format("{0:f3}", 9.9);
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BaseConst.Log.WriteLogsToFile("进入程序入口点，开始初始化主窗体");
            Application.Run(new FormMain());
        }
    }
}
