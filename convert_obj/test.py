



MaxPoint = 100000;

class Point:
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def sub(self, point):   
        return Point(self.x - point.x, self.y - point.y)

    def __str__(self) -> str:
        return "[{},{}]".format(self.x, self.y)
        


class Edge:
    def __init__(self, p1, p2):
        self.p1 = p1
        self.p2 = p2

    def __str__(self) -> str:
        return "[{},{}]".format(self.p1, self.p2)


    def other(self, p):
        return self.p1 if self.p2 == p else self.p2
    

def EdgeIndex(p1, p2):
    if p1 < p2:
        return p1 * MaxPoint + p2
    return p2 * MaxPoint + p1


# 根据edgeindex生成Edge对象
def markEdge(edgeindex):
    p1 = edgeindex / MaxPoint;
    p2 = edgeindex % MaxPoint;
    return Edge(p1, p2)




class Polygon:
    
    def __init__(self, Points):
        self.Points = self.Merge( Points )
        p0 = Points[0];
        minx = p0.x;
        miny = p0.y;
        maxx = p0.x;
        maxy = p0.y;
        maxxn = 0;   # maxx的索引
        cnt = len(Points) - 1

        for i in range(1,cnt):
            x = Points[i].x
            y = Points[i].y
            if (x < minx):
                minx = x
            if (x > maxx):
                maxx = x
                maxxn = i
            if (y < miny):
                miny = y
            if (y> maxy):
                maxy = y

        # 左上角为坐标原点，向下向右坐标为坐标轴的正方向
        self.lt.x = minx;  # left top
        self.lt.y = miny;
        self.rb.x = maxx;  # right bottom
        self.rb.y = maxy;
      
        p = Points[maxxn];
        pre = Points[ cnt if maxxn-1<0 else maxxn-1];
        nxt = Points[ 0 if maxxn + 1 > cnt else maxxn + 1];

        # 判断点的集合是否是顺时针
        for i in range(cnt + 2):   # 最大到 cnt + 1
            if (i == cnt + 1):
                raise Exception("Clock Detect Fail")

            n = maxxn + i;
            if (n > cnt):
                n -= (cnt+1);
            p = Points[n];
            pre = Points[cnt if n - 1 < 0 else n - 1];
            nxt = Points[0 if n + 1 > cnt else n + 1];
            v1 = p.sub(pre);
            v2 = nxt.sub(p);
            dxy =  v1.x* v2.y - v2.x * v1.y;
            if (dxy != 0):
                self.clock = dxy > 0;
                break;

        self.polygons = [];


    # 把特别靠近的顶点合并
    def Merge(Points):
        # todo 之后实现
        return Points


    # 多边形是否包含另一个多边形p
    def Contain(self, p):  # p:Polygon 
        plt = p.lt;
        prb = p.rb;
        if (self.lt.x > plt.x or self.lt.y > plt.y):
            return False;
        if (self.rb.x < prb.x or self.rb.y <  prb.y):
            return False;
        return True;

    # 自己被包含在其他多边形中
    def Contained(self, polygons ): # polygons: list<Polygon>
        for i in range(len(polygons)):
            polygon = polygons[i];
            if (polygon != self and polygon.Contain(self)):
                return polygon;
      
        return None


    def Add(self, p):  #p:Polygon
        self.polygons.Add(p);


    def ToList(self, clock, list ):
        change = self.clock == clock;
        if (self.Points.Count - 1 < 4): return False;
        list.Add(self.Points.Count-1);      
        if (not change):
            for i in range(len(self.Points - 2), -1, -1):
                x = self.Points[i].x 
                y = self.Points[i].y
                list.Add(x);
                list.Add(y)
        else:
            for i in range(len(self.Points - 1)):
                x = self.Points[i].x 
                y = self.Points[i].y
                list.Add(x);
                list.Add(y);
        return True;



def GenTxt(vertices, indices):

    # 找到所有没有被两个三角形给引用的边，用于找回环
    for i in range(0, len(indices), 3):
        p1 = indices[i];
        p2 = indices[i+1];
        p3 = indices[i+2];

        edge1 = EdgeIndex(p1, p2);
        edge2 = EdgeIndex(p2, p3);
        edge3 = EdgeIndex(p3, p1);

        edge2triangle = {};  # key是边的index，value是以该边为三角形的数量

        if (edge1 in edge2triangle):
            edge2triangle[edge1] = edge2triangle[edge1] + 1;
        else:
            edge2triangle[edge1] = 1;

        if (edge2 in edge2triangle):
            edge2triangle[edge2] = edge2triangle[edge2] + 1;
        else:
            edge2triangle[edge2] = 1;

        if (edge3 in edge2triangle):
            edge2triangle[edge3] = edge2triangle[edge3] + 1;
        else:
            edge2triangle[edge3] = 1;

    # 点所在边集合
    border = []  # 多边形边界，每个边界边只有一个三角形
    point2edge = {}   # {point_index:[border中的边]} 点到边的映射,一个点可以映射到两个边
    for item in edge2triangle:
        if (edge2triangle[item] == 1):
            i = len(border)
            edge = markEdge(item)   # 通过边的索引获得边的对象,边的对象包含两个点
            border.append(edge);
            
            p1 = edge.p1;
            p2 = edge.p2;

            if p1 not in point2edge:
                point2edge[p1] = []
            point2edge[p1].append(i)

            if p2 not in point2edge:
                point2edge[p2] = []
            point2edge[p2].append(i)

        elif edge2triangle[item] >= 3:
            print("error: edge2triangle[item] != 1")


    # 从任意边出发，找到回环，，回环有最外层的回环和内部包含的回环
    visited = {}
    for i in range(0, len(border)):
        if(i not in visited):
            edge = border[i]
            p = edge.p2
            b = True
            polygon = []
            pindexs = []
            polygon.append(vertices[edge.p1])
            polygon.append(vertices[edge.p2])
            visited[i] = True

            while True:
                edge_list = point2edge[p]


    # 判断单个连线是否在别的多边形内

    pass


# ==========test=============
'''
p1 = Point(6,6)
p2 = Point(2,2,)
print(p1.sub(p2))


e1 = Edge(1, 2)
print(e1)
print(e1.other(1))
print(e1.other(2))

'''


vertices = [
    [1,1],
    [2,3],
    [6,5],
    [9,1],
    [5,5],
    [4,4]
]
indices = [
1,2,6,
2,3,4,
4,5,6
]
GenTxt(vertices, indices)





