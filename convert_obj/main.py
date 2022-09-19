
import getopt
import sys
import os


import parse_obj
import navmesh

# #############################################################################
# Helpers
# #############################################################################
def usage():
    print("Usage: {} -i filename.obj -o filename.js".format(os.path.basename(sys.argv[0])))




def convert_obj2txt(infile, outfile):
    if not parse_obj.file_exists(infile):
        print("Couldn't find [%s]" % infile)
        return

    vertices, triangles, _, _, _,_,_  = parse_obj.parse_obj(infile)

    # print(type(vertices[0][0]))   # 存储着所有的点的坐标的list
    # print(type(triangles[0]))  # 存储着所有的三角形的顶点索引的list

    # print((vertices))   
    # print((triangles))  

    mesh = navmesh.NavMash(vertices, triangles)

    pols = mesh.find_loopback()
    return pols
    # print(pols)



# #####################################################
# Main
# #####################################################
if __name__ == "__main__":

    # get parameters from the command line
    try:
        opts, args = getopt.getopt(sys.argv[1:], "hi:o:", ["help", "input=",  "output="])

    except getopt.GetoptError:
        usage()
        sys.exit(2)

    infile = outfile = ""

    for o, a in opts:
        if o in ("-h", "--help"):
            usage()
            sys.exit()

        elif o in ("-i", "--input"):
            infile = a

        elif o in ("-o", "--output"):
            outfile = a
    print("infile is:" + infile)
    print("outfile is:" + outfile)
    if infile == "":
        usage()
        sys.exit(2)

    convert_obj2txt(infile, outfile)
