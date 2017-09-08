using System;
/// <summary>
/// 经纬度工具类
/// </summary>
public class LantitudeLongitudeUtil {
    /// <summary>
    /// 地球半径
    /// </summary>
    private const double EARTH_RADIUS = 6371393;//赤道半径(单位m)  

    /** 
     * 转化为弧度(rad) 
     * */
    private static double rad(double d)
    {
        return d * Math.PI / 180.0;
    }

    /** 
     * 基于余弦定理求两经纬度距离 
     * @param lon1 第一点的经度 单位为°
     * @param lat1 第一点的纬度 单位为°
     * @param lon2 第二点的经度 单位为°
     * @param lat3 第二点的纬度 单位为°
     * @return 返回的距离，单位m 
     * */
    public static double LantitudeLongitudeDist(double lon1, double lat1, double lon2, double lat2)
    {
        double radLat1 = rad(lat1);
        double radLat2 = rad(lat2);

        double radLon1 = rad(lon1);
        double radLon2 = rad(lon2);

        if (radLat1 < 0)
            radLat1 = Math.PI / 2 + Math.Abs(radLat1);// south  
        if (radLat1 > 0)
            radLat1 = Math.PI / 2 - Math.Abs(radLat1);// north  
        if (radLon1 < 0)
            radLon1 = Math.PI * 2 - Math.Abs(radLon1);// west  
        if (radLat2 < 0)
            radLat2 = Math.PI / 2 + Math.Abs(radLat2);// south  
        if (radLat2 > 0)
            radLat2 = Math.PI / 2 - Math.Abs(radLat2);// north  
        if (radLon2 < 0)
            radLon2 = Math.PI * 2 - Math.Abs(radLon2);// west  
        double x1 = EARTH_RADIUS * Math.Cos(radLon1) * Math.Sin(radLat1);
        double y1 = EARTH_RADIUS * Math.Sin(radLon1) * Math.Sin(radLat1);
        double z1 = EARTH_RADIUS * Math.Cos(radLat1);

        double x2 = EARTH_RADIUS * Math.Cos(radLon2) * Math.Sin(radLat2);
        double y2 = EARTH_RADIUS * Math.Sin(radLon2) * Math.Sin(radLat2);
        double z2 = EARTH_RADIUS * Math.Cos(radLat2);

        double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
        //余弦定理求夹角  
        double theta = Math.Acos((EARTH_RADIUS * EARTH_RADIUS + EARTH_RADIUS * EARTH_RADIUS - d * d) / (2 * EARTH_RADIUS * EARTH_RADIUS));
        double dist = theta * EARTH_RADIUS;
        return dist;
    }
}
