#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;


public class GenScenePath : MonoBehaviour
{
    public void CreatePoly() {
        var go = gameObject.GetComponent<MeshFilter>();
        var mesh = go.sharedMesh;
        var angle = go.transform.eulerAngles.x;
        var offTrans = go.transform.position;
        var trans = go.transform;
        var navpath = CreatScenePath.GenPolygon(mesh.vertices, mesh.triangles,go.transform.right, angle, offTrans, trans);
        //Log.Sys("--------------------",mesh.vertices.Length );

        string name = SceneManager.GetActiveScene().name.ToLower() + gameObject.name;
        var bytes = navpath.GetBytes();
        string pointsPath = Application.dataPath + "/Scenes/points_" + name + ".txt";
        StreamWriter stream = new StreamWriter(pointsPath);
        for (int i = 0; i < bytes.Length; i++)
        {
            stream.WriteLine(bytes[i]);
        }
        stream.Flush();
        stream.Close();

        var pf = new PathFinder(navpath);
        pf.SavePath(Application.dataPath + "/Resources/qpf/" + name + ".qpf");
        pf.SavePath("../server/qpf/" + name + ".qpf");
        pf.Dispose();
        Log.Sys("=========export path qpf done==========");
    }

    public void CreatePoly1() {        
        //UnityEngine.AI.NavMeshTriangulation tmpNavMeshTriangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();

        ////�½��ļ�
        //string tmpPath = Application.dataPath + "/Scenes/" + SceneManager.GetActiveScene().name + ".obj";
        //StreamWriter tmpStreamWriter = new StreamWriter(tmpPath);

        ////����  
        //for (int i = 0; i < tmpNavMeshTriangulation.vertices.Length; i++)
        //{
        //    tmpStreamWriter.WriteLine("v  " + tmpNavMeshTriangulation.vertices[i].x + " " + tmpNavMeshTriangulation.vertices[i].y + " " + tmpNavMeshTriangulation.vertices[i].z);
        //}

        //tmpStreamWriter.WriteLine("g pPlane1");

        ////����  
        //for (int i = 0; i < tmpNavMeshTriangulation.indices.Length;)
        //{
        //    tmpStreamWriter.WriteLine("f " + (tmpNavMeshTriangulation.indices[i] + 1) + " " + (tmpNavMeshTriangulation.indices[i + 1] + 1) + " " + (tmpNavMeshTriangulation.indices[i + 2] + 1));
        //    i = i + 3;
        //}

        //tmpStreamWriter.Flush();
        //tmpStreamWriter.Close();

        //var navpath = CreatScenePath.GenPolygon(tmpNavMeshTriangulation.vertices, tmpNavMeshTriangulation.indices,Vector3.zero,0);
        //string name = SceneManager.GetActiveScene().name.ToLower() + gameObject.name;
        //var bytes = navpath.GetBytes();
        //string pointsPath = Application.dataPath + "/Scenes/points_" + name + ".txt";
        //StreamWriter stream = new StreamWriter(pointsPath);
        //for (int i = 0; i < bytes.Length; i++)
        //{
        //    stream.WriteLine(bytes[i]);
        //}
        //stream.Flush();
        //stream.Close();
        //var pf = new PathFinder(navpath);
        //pf.SavePath(Application.dataPath + "/Resources/qpf/" + name + ".qpf");
        //pf.SavePath("../server/qpf/" + name + ".qpf");
        //pf.Dispose();
        //Log.Sys("=========export path qpf done==========");
    }
}
#endif