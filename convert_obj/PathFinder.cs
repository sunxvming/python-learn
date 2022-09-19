using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Security;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public struct Point{
    public double x;
    public double y;
    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public Point sub( Point p )
    {
        return new Point(this.x - p.x, this.y - p.y);
    }
}

public class Edge{
    public int p1;
    public int p2;

    public Edge( int p1, int p2 )
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public int other( int p )
    {
        return p2 == p ? p1 : p2;
    }
}

/*public class Triangle{
    public int p1;
    public int p2;
    public int p3;
    public List<int> eges;
}*/

public class Polygon{
    public List<Point> Points;
    public List<int> Pointsindex;
    public List<Polygon> polygons;
    private Point lt;
    private Point rb;
    private bool clock;
    public Polygon( List<Point> Points )
    {
        this.Points = Merge( Points );
        var p0 = Points[0];
        var minx = p0.x;
        var miny = p0.y;
        var maxx = p0.x;
        var maxy = p0.y;
        int maxxn = 0;
        int cnt = Points.Count-1;
        for (int i = 1; i < cnt; i++)
        {
            var x = Points[i].x;
            var y = Points[i].y;
            if (x < minx)
                minx = x;
            if (x > maxx)
            {
                maxx = x;
                maxxn = i;
            }
            if (y < miny)
                miny = y;
            if (y> maxy)
                maxy = y;
        }
        lt.x = minx;
        lt.y = miny;
        rb.x = maxx;
        rb.y = maxy;
        var p = Points[maxxn];
        var pre = Points[maxxn-1<0?cnt:maxxn-1];
        var nxt = Points[maxxn + 1 > cnt ? 0 : maxxn + 1];

        Log.Sys("-----Points----count--------", Points.Count);
        for ( int i = 0; i<= (cnt + 1); i++)
        {
            if (i == cnt + 1)
                throw (new Exception("Clock Detect Fail"));
            var n = maxxn + i;
            if (n > cnt)
                n -= (cnt+1);
            p = Points[n];
            pre = Points[n - 1 < 0 ? cnt : n - 1];
            nxt = Points[n + 1 > cnt ? 0 : n + 1];
            var v1 = p.sub(pre);
            var v2 = nxt.sub(p);
            var dxy =  v1.x* v2.y - v2.x * v1.y;
            if (dxy != 0)
            {
                clock = dxy > 0;
                break;
            }
        }
        polygons = new List<Polygon>();
    }

    public bool Contain( Polygon p )//不考虑相交
    {
        var plt = p.lt;
        var prb = p.rb;
        if (lt.x > plt.x || lt.y > plt.y)
            return false;
        if (rb.x < prb.x || rb.y <  prb.y)
            return false;
        return true;
    }

    public Polygon Contained( List<Polygon> polygons )//不考虑相交
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            var polygon = polygons[i];
            if (polygon != this && polygon.Contain(this))
                return polygon;
        }
        return null;
    }

    private List<Point> Merge( List<Point> Points )//把特别靠近的顶点合并
    {
        var points = new List<Point>();
        var px = Points[0].x;
        var py = Points[0].y;
        points.Add(Points[0]);
        for (int i = 1; i < Points.Count; i++)
        {
            var x = Points[i].x;
            var y = Points[i].y;
            var dx = px - x;
            var dy = py - y;
            if( dx*dx + dy*dy > 0.1 ) 
            {
                points.Add(Points[i]);
                px = x;
                py = y;
            }
        }
        //Log.Sys("-------------pointscount--------", points.Count);
        if (points.Count < 3) {
            for (int i = 0; i < Points.Count; i++)
            {
                Log.Sys("==点{0}:=={1}={2}=",i+1, Points[i].x, Points[i].y);
            }
            //throw (new Exception("point too close"));
        }
        return points;
    }

    public void Add( Polygon p )
    {
        polygons.Add(p);
    }

    public bool ToList( bool clock, List<double> list )
    {        
        var change = this.clock == clock;
        //var change = true;
        if (Points.Count - 1 < 4) return false;
        list.Add(Points.Count-1);      
        if (!change)
        {
            for (int i = Points.Count-2; i >=0; i--)
            {
                int x = (int)(Points[i].x * 1000);
                double xx = x / 1000.0f;
                int y = (int)(Points[i].y * 1000);
                double yy = y / 1000.0f;
                list.Add(xx);
                list.Add(yy);
            }
        }
        else
        {
            for (int i = 0; i < Points.Count-1; i++)
            {
                int x = (int)(Points[i].x * 1000);
                double xx = x / 1000.0f;
                int y = (int)(Points[i].y * 1000);
                double yy = y / 1000.0f;   
                list.Add(xx);
                list.Add(yy);
            }
        }
        return true;
    }
}

public class NavPath
{
    public List<Polygon> polygons;
    public NavPath( )
    {
        polygons = new List<Polygon>();
    }

    public void Add( Polygon p )
    {
        polygons.Add(p);
    }

    private void GetCoords() {

    }

    public double[] GetBytes( )
    {
        List<double> coords = new List<double>();
        Polygon poly;        
        for (int i = 0; i < polygons.Count; i++)
        {
            poly = polygons[i];
            if (poly.polygons.Count > 0)
            {
                int a = 0;
                for (int j = 0; j < poly.polygons.Count; j++)
                {
                    if (poly.polygons[j].Points.Count-1 > 2) {
                        a++;
                    }
                }
                coords.Add(a);
                if (!poly.ToList(true, coords)) {
                    coords.RemoveAt(coords.Count-1);
                }
                for (int j = 0; j < poly.polygons.Count; j++)
                {
                    coords.Add(0);
                    if (!poly.polygons[j].ToList(false, coords)) {
                        coords.RemoveAt(coords.Count-1);
                    }
                }
            }
            else {
                coords.Add(0);
                if (!poly.ToList(true, coords)) {
                    coords.RemoveAt(coords.Count-1);
                }
            }
        }
        return coords.ToArray();
    }
}

public class CreatScenePath{
    static int MaxPoint = 100000;
    static int EdgeIndex(int p1, int p2)
    {
        if (p1 < p2)
            return p1 * MaxPoint + p2;
        return p2 * MaxPoint + p1;
    }

    static Edge markEdge(int edgeindex)
    {
        int p1 = edgeindex / MaxPoint;
        int p2 = edgeindex % MaxPoint;
        if (p2 < p1)
            return new Edge(p2, p1);
        return new Edge(p1, p2);
    }

    public static NavPath GenPolygon(Vector3[] vertices, int[] indices, Vector3 cent, float angle, Vector3 offTrans, Transform trans)
    {
        Log.Sys("-----------trans angle----------", angle, offTrans.x, offTrans.z);
        if (vertices.Length >= MaxPoint)
            throw (new System.Exception("too many vertices"));
        //去掉重复顶点
        //List<Point> Points2 = new List<Point>();
        List<Point> Points = new List<Point>();
        Dictionary<int, int> adpts = new Dictionary<int, int>();

    
        for (int i = 0; i < vertices.Length; i++)
        {
            //var v = vertices[i];
            ////v = new Vector3(v.x + offTrans.x, v.y, v.z + offTrans.z);
            //var newV = v;

            //var angleAbs = Math.Abs(angle);
            //var dis = Math.Abs(angleAbs - 270);
            //if (dis < 10)
            //    newV = Quaternion.AngleAxis(-90, cent)*v;

            //var pt = new Point(newV.x+offTrans.x, newV.z+offTrans.z);

            var v = trans.TransformPoint(vertices[i]);
            var pt = new Point(v.x , v.z);
            int index = Points.FindIndex((Point p) => (
                ((Math.Floor(pt.x * 10000) / 10000f) == (Math.Floor(p.x * 10000) / 10000f)) && ((Math.Floor(pt.y * 10000) / 10000f) == (Math.Floor(p.y * 10000) / 10000f))
            ));

            if (index > -1)
            {//重复的点
                adpts.Add(i, index);
            }
            else
            {
                adpts.Add(i, Points.Count);
                Points.Add(pt);             
            }
        }

        var indexs = new List<int>();
        for (int i = 0; i < indices.Length; i++)
        {
            var n = indices[i];
            if (adpts.ContainsKey(n))
                n = adpts[n];
            indexs.Add(n);
        }

        #region //画出点索引
        int length = Points.Count;
        string pointpath = Application.dataPath + "/Scenes/" + "/points1.txt";
        StreamWriter stream = new StreamWriter(pointpath);
        for (int i = 0; i < length; i++)
        {
            stream.WriteLine(Points[i].x);
            stream.WriteLine(Points[i].y);
        }
        stream.Flush();
        stream.Close();

        string indexpath = Application.dataPath + "/Scenes/" + "/points2.txt";
        StreamWriter stream2 = new StreamWriter(indexpath);
        int length2 = indexs.Count;
        for (int i = 0; i < length2; i++)
        {
            stream2.WriteLine(indexs[i]);
        }
        stream2.Flush();
        stream2.Close();
        #endregion

        //Points, indexs新的顶点和索引,
        //todo对比跟之前是否不一致

        //Dictionary<int, int> reindexs = new Dictionary<int, int>();
        //for (int i = 0; i < indexs.Count; i++)
        //{
        //    if (reindexs.ContainsKey(indexs[i])) {
        //        reindexs[indexs[i]]++;
        //        //if (reindexs[indexs[i]] > 5)
        //            //Log.Sys("=====reindex>>>>>>>>>>>>>>555==");
        //    }
        //    else
        //        reindexs.Add(indexs[i],1);
        //}

        //找到所有没有被2个三角形给引用的边，用于找回环
        Dictionary<int, int> edge2triangle = new Dictionary<int, int>();
        for (int i = 0; i < indexs.Count-2; i=i+3)
        {
            var p1 = indexs[i];
            var p2 = indexs[i + 1];
            var p3 = indexs[i + 2];

            int edge1 = EdgeIndex(p1, p2);
            int edge2 = EdgeIndex(p2, p3);
            int edge3 = EdgeIndex(p3, p1);

            if (edge2triangle.ContainsKey(edge1))
            {
                edge2triangle[edge1]++;
            }
            else
            {
                edge2triangle.Add(edge1, 1);                              
            }

            if (edge2triangle.ContainsKey(edge2))
            {
                edge2triangle[edge2]++;
            }
            else
            {
                edge2triangle.Add(edge2, 1);              
            }

            if (edge2triangle.ContainsKey(edge3))
            {
                edge2triangle[edge3]++;
            }
            else
            {
                edge2triangle.Add(edge3, 1);               
            }
        }

        //点所在边集合
        List<Edge> border = new List<Edge>();//回环边界
        Dictionary<int, List<int>> point2edge = new Dictionary<int, List<int>>();
        foreach (var item in edge2triangle)
        {
            if (item.Value == 1)
            {
                int i = border.Count;
                var edge = markEdge(item.Key);//第几条边  
                border.Add(edge);
                var p1 = edge.p1;//第几个点
                var p2 = edge.p2;
                if (!point2edge.ContainsKey(p1))
                {
                    point2edge.Add(p1, new List<int>());
                }
                point2edge[p1].Add(i);

                if (!point2edge.ContainsKey(p2))
                {
                    point2edge.Add(p2, new List<int>());
                }
                point2edge[p2].Add(i);
              
                //if (deledges.Contains(item.Key))
                //    stream3.WriteLine(i);                             
            }
            else if (item.Value >= 3)
            {
                var n = item.Value;
                //Log.Sys("--------xxxxxxx-------",n);
            }
        }

        //stream3.Flush();
        //stream3.Close();        

        //删除无用边478,847,2,168,65,186,483,849      83,219,415,42,46, 103,320
        //List<int> noUseBorders = new List<int> { };

        //todo检查有没有1个点连2个以上边的情况
        #region//画出寻找到的边界
        int length3 = border.Count;
        Log.Sys("==========bordercount=======", length3);
        string borderpath = Application.dataPath + "/Scenes/" + "/points3.txt";
        Dictionary<int, int> reborders = new Dictionary<int, int>();
        StreamWriter streamborder = new StreamWriter(borderpath);
        for (int i = 0; i < length3; i++)
        {
            var edge = border[i];
            var p1 = edge.p1;
            var p2 = edge.p2;
            streamborder.WriteLine(p1);            
            streamborder.WriteLine(p2);
            if (reborders.ContainsKey(p1))
            {
                reborders[p1]++;
            }
            else
                reborders.Add(p1, 1);

            if (reborders.ContainsKey(p2))
            {
                reborders[p2]++;
                //if (reborders[p2] > 2)
                //{
                //    noUseBorders.Add(i);
                //}
            }
            else
                reborders.Add(p2, 1);
        }
        streamborder.Flush();
        streamborder.Close();
        #endregion

        Dictionary<int, int> visited = new Dictionary<int, int>();
        //for (int i = 0; i < border.Count; i++)
        //{
        //    if (!visited.ContainsKey(i) && !noUseBorders.Contains(i))
        //    {
        //        var edge = border[i];
        //        int p = edge.p2;
        //        visited.Add(i, 1);
        //        while (true)
        //        {
        //            var list = point2edge[p];//点所在边的index集合
        //            var e0 = list[0];

        //            if (point2edge[p].Count < 2)
        //            {
        //                break;
        //            }

        //            var e1 = list[1];
        //            if (list.Count > 3)
        //            {
        //                //Log.Sys("========点所在边的list>3========", Points[p].x, Points[p].y, p, list.Count, e0, e1, list[2], list[3]);
        //            }
        //            else if (list.Count > 2)
        //            {
        //                //Log.Sys("========点所在边的list>2========", Points[p].x, Points[p].y, p, list.Count, e0, e1, list[2]);
        //            }
        //            if (list.Count > 2)
        //            {
        //                if (!noUseBorders.Contains(e0))
        //                    noUseBorders.Add(e0);
        //                if (list.Count>3&&!noUseBorders.Contains(list[3]))
        //                    noUseBorders.Add(list[3]);
        //            }
        //            Edge e = null;
        //            if (!visited.ContainsKey(e0))
        //            {
        //                e = border[e0];
        //                visited.Add(e0, 1);
        //            }
        //            else if (!visited.ContainsKey(e1))
        //            {
        //                e = border[e1];
        //                visited.Add(e1, 1);
        //            }
        //            if (e == null)
        //                break;
        //            var nxtp = e.other(p);
        //            p = nxtp;
        //        }
        //    }
        //}

        //for (int i = 0; i < noUseBorders.Count; i++)
        //{            
        //    foreach (var item in point2edge)
        //    {
        //        if (item.Value.Contains(noUseBorders[i])) item.Value.Remove(noUseBorders[i]);
        //    }
        //}

        //从任意边出发，找到最大回环，如果找不到回环的话，就是有问题

        List<Polygon> polygons = new List<Polygon>();
        visited.Clear();
        for (int i = 0; i < border.Count; i++)
        {
            if (!visited.ContainsKey(i))
            {
                var edge = border[i];
                int p = edge.p2;
                var b = true;
                var polygon = new List<Point>();
                var pindexs = new List<int>();
                polygon.Add(Points[edge.p1]);
                polygon.Add(Points[p]);
                visited.Add(i, 1);
                while (true)
                {
                    var list = point2edge[p];//点所在边的index集合
                    var e0 = list[0];
                    
                    if (point2edge[p].Count < 2) {
                        //Log.Sys("========点所在边的=list=1===========", Points[e0].x, Points[e0].y, Points[p].x, Points[p].y, p, list.Count, e0);
                        b = false;
                        break;
                    }                     

                    var e1 = list[1];
                    if (list.Count > 3)
                    {
                        Log.Sys("========点所在边的list>3========", Points[p].x, Points[p].y, p, list.Count, e0, e1, list[2], list[3]);
                    }
                    else if (list.Count > 2) {
                        Log.Sys("========点所在边的list>2========", Points[p].x, Points[p].y, p, list.Count, e0, e1, list[2]);
                    }
                    if (list.Count > 2)
                    {
                        e0 = list[1];
                        e1 = list[2];                        
                    }
                    Edge e = null;
                    if (!visited.ContainsKey(e0))
                    {
                        e = border[e0];
                        visited.Add(e0, 1);
                    }
                    else if (!visited.ContainsKey(e1))
                    {
                        e = border[e1];
                        visited.Add(e1, 1);
                    }
                    if (e == null)
                        break;
                    var nxtp = e.other(p);
                    //if (!polygon.Contains(Points[nxtp]))           
                        polygon.Add(Points[nxtp]);
                    //streamborder.WriteLine(nxtp);
                    pindexs.Add(nxtp);
                    p = nxtp;
                }
                if (polygon.Count <= 3)
                {
                    for (int k = 0; k < polygon.Count; k++)
                    {
                        Log.Sys("==点{0}:=={1}={2}=", k + 1, polygon[k].x, polygon[k].y);
                    }
                    //throw (new System.Exception("single edge"));
                }

                bool b1 = false;
                bool b2 = false;
                var statx = polygon[0].x;
                foreach (var item in polygon)
                {
                    if (item.x != statx) b1 = true;
                }
                var staty = polygon[0].y;
                foreach (var item in polygon)
                {
                    if (item.y != staty) b2 = true;
                }
                if (b1&&b2&&b) {
                    var ps = new Polygon(polygon);
                    if (ps.Points.Count > 3) {
                        ps.Pointsindex = pindexs;
                        polygons.Add(ps);
                    } 
                }  
            }
        }

        //判断单个连线是否在别的多边形内
        //todo把
        int expcept = 0;
        var path = new NavPath();
        for (int i = 0; i < polygons.Count; i++)
        {
            var polygon = polygons[i];           
            var container = polygon.Contained(polygons);
            if (container == null)
            {
                path.Add(polygon);
                //var pindexs = polygon.Pointsindex;
                //for (int j = 0; j < pindexs.Count; j++)
                //{
                //    //streamborder.WriteLine(pindexs[j]);
                //}
            }
            else
            {
                container.Add(polygon);
                expcept++;
            }
            //polygon.ToList(!polygon.Contained(polygons), listPoints);//被包含就逆时针，否则顺时针
        }
        return path;
    }
}

#endif
public class PathFinder:ScriptObject
{
    public override ScriptClass typeid()
    {
        return ScriptClass.CLASS_PathFinder;
    }

#if UNITY_IPHONE && !UNITY_EDITOR
        const string PATHDLL = "__Internal";
#else
        const string PATHDLL = "NavPath";
#endif
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreatePath(double[] cont, int count);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SavePath(IntPtr handle, string resname );
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr LoadPath(string resname);//字符串用byte[]
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FreePath(IntPtr handle);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FindNear(IntPtr handle, double x, double y);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FindPath(IntPtr handle, double startx, double starty, double endx, double endy, int far, out int size);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FindCross(IntPtr handle, double startx, double starty, double facex, double facey);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CheckPath(IntPtr handle, double startx, double starty, double endx, double endy);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr GetPoints(IntPtr cont, out int length);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetIndexs(IntPtr cont, out int length);
    [DllImport(PATHDLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetSize(IntPtr cont, out int length);


    private IntPtr handle;
    private double[] point;
    private ResHandle res;
    private string name;
    private Material material;
    private GameObject meshgo;

    [LuaAttr("pathfinder构造","加载文件名")]
    public PathFinder( string name )
    {
        Log.Assert(name != null, "Argument #1 Can Not Be Null");
        handle = IntPtr.Zero;
        point = new double[2]{0.0f, 0.0f};
        this.name = name;
        res = ResourceManager.Instance.AsyncLoadAsset( name, LoadType.qpf, OnLoaded);
    }

#if UNITY_EDITOR
    public PathFinder( NavPath path )
    {
        var bytes = path.GetBytes();
        if (bytes == null) {
            Log.Sys("------------getbytes   end-----------------");
            return;
        }
        handle = CreatePath( bytes, bytes.Length );
        res = null;
    }
#endif

    public void OnLoaded( ResHandle reshandle )
    {
        /*if (Config.UseResources)
            handle = LoadPath("Assets/Resources/" + reshandle.GetPath());
        else
        {
            var path = Config.ABPath + "/" + reshandle.GetLongName();
            Log.Sys("PathFinder.OnLoaded#####", path);
            
            Log.Sys("PathFinder.BeginLoadPath#####", path);
        }*/
        var path = ResourceManager.Instance.LoadAsset(name);
        handle = LoadPath(path);
        reshandle.Dispose();
        reshandle = null;
    }
    
    [LuaAttr("资源名")]
    public string resname
    {
        get {
            if(res==null) return null;
            return res.name;
        }
    }

    [LuaAttr("path加载完毕")]
    public bool Ready( )
    {
        return handle != IntPtr.Zero;
    }

    [LuaAttr("两点寻路")]
    public double[] FindPath(double startx, double starty, double endx, double endy, bool far)
    {
        if (!Ready()) return null;
        int size = 0;
        var ptr = FindPath(handle, startx, starty, endx, endy, far?1:0, out size);
        if (size < 4) return null;
        if (ptr == IntPtr.Zero) return null;
        var path = new double[size];
        Marshal.Copy(ptr, path, 0, size);
        return path;
    }
    [LuaAttr("找到一个目标点")]
    public Var2 FindCross(double startx, double starty, double facex, double facey )
    {
        if (!Ready()) return new Var2(Var.nil, Var.nil);
        var ptr =  FindCross(handle, startx, starty, facex, facey);           
        if (ptr == IntPtr.Zero) return new Var2(Var.nil, Var.nil);  
        Marshal.Copy(ptr, point, 0, 2);
        return new Var2(point[0], point[1]);
    }
    [LuaAttr("检查目标点")]
    public bool CheckPath(double startx, double starty, double endx, double endy )
    {
        if (!Ready()) return false;
        return CheckPath(handle, startx, starty, endx, endy) != 0;
    }
    [LuaAttr("找到阻挡内最近点", "x坐标", "y坐标")]
    public Var2 FindNear(double x, double y)
    {
        if (!Ready()) return new Var2(Var.nil, Var.nil);
        var ptr = FindNear(handle, x, y);
        if (ptr == IntPtr.Zero) return new Var2(Var.nil, Var.nil);
        Marshal.Copy(ptr, point, 0, 2);
        return new Var2(point[0], point[1]);
    }
    [LuaAttr("drawMesh导出")]
    public void DrawMesh() {
#if UNITY_EDITOR
        int len = 0;
        List<Vector3> lists = ListPool<Vector3>.Pop();
        var heights = GameObject.Find("Scene").GetComponentsInChildren<MeshCollider>();
        MeshCollider height=null;
        foreach (var mesh in heights)
        {
            if (mesh.gameObject.layer != LayerMask.NameToLayer("camera"))
                height = mesh;
        }
        Log.Assert(height, "the height is null!!!!!!!!!!");
        var cpoints = GetPoints(handle, out len);
        if (len <= 0) return;
        var points = new double[len];
        Marshal.Copy(cpoints, points, 0, len);
        for (int i = 0; i < points.Length-1; i=i+2)
        {
            var temp = new Vector3((float)points[i], 0,(float)points[i + 1]);
            lists.Add(temp);
        }
       var ptr =  GetSize(handle, out len);
        int[] sizes;
        if (len <= 0) return;
        sizes = new int[len];
        Marshal.Copy(ptr, sizes, 0, len);
        int pre = 0;
        for ( int i = 0; i<len; i++)
        {
            List<Vector3> drawPoints = ListPool<Vector3>.Pop();
            int size = sizes[i];
            for( int j = 0; j<size; j++)
            {
                var nowpoint = lists[pre+j];
                float h = GetHeight(height, (float)nowpoint.x, (float)nowpoint.z);
                var temp = new Vector3((float)nowpoint.x, h + 1, (float)nowpoint.z);
                drawPoints.Add(temp);
            }
            pre += size;
            Factory.getInstance().DrawLines(drawPoints);
            ListPool<Vector3>.Push(drawPoints);
        }
        ListPool<Vector3>.Push(lists);
#endif
    }    

    private float GetHeight(MeshCollider heightcollider, float x, float z)
    {                       
        var bounds = heightcollider.bounds;
        var y = bounds.max.y;
        var ray = new Ray(new Vector3(x, y + 10, z), new Vector3(0, -1, 0));
        RaycastHit hit;
        if (heightcollider.Raycast(ray, out hit, bounds.size.y + 20))
            return hit.point.y;
        return 0;
    }

#if UNITY_EDITOR
    public void SavePath(string resname)
    {
        SavePath(handle, resname);
    }
#endif

    [LuaAttr("销毁")]
    public override void Dispose( )
    {
		if (disposed) return;
        base.Dispose();
        if (res != null)
        {
            res.Dispose();
            res = null;
        }
        if (handle != IntPtr.Zero)
        {
            FreePath(handle);
            handle = IntPtr.Zero;
        }
        if (material != null) {
            GameObject.Destroy(material);
            material = null;
        }
        if (meshgo != null)
            GameObject.Destroy(meshgo);
            meshgo = null;
    }

}

