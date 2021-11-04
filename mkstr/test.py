

def read_file_to_list(file_name):
    fileHandler  =  open(file_name,  "r")
    line_list = []
    for line in fileHandler.readlines():                          #依次读取每行  
        line = line.strip()                             #去掉每行头尾空白  
        line_list.append(line)
    fileHandler.close()
    return line_list



words = read_file_to_list("word.txt")


param_str = ""
for word in words:
    param_str +=  word + "=" + "{" + word + "}" + "&"

param_str = param_str[:-1]    
print(param_str)