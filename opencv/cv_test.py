import cv2

# RTSP URL（替换为你的视频流地址）
rtsp_url = 'rtsp://127.0.0.1:8554/test_video'

# 创建 VideoCapture 对象
cap = cv2.VideoCapture(rtsp_url)

# 检查视频流是否成功打开
if not cap.isOpened():
    print("Error: Could not open video stream")
    exit()


frame_rate = cap.get(cv2.CAP_PROP_FPS)
frame_rate = int(frame_rate) 
print("frame_rate is:", frame_rate)

while True:
    # 读取帧
    ret, frame = cap.read()

    # 检查是否成功读取帧
    if not ret:
        print("Error: Could not read frame")
        break

    frame_number = cap.get(cv2.CAP_PROP_POS_FRAMES)
    frame_number = int(frame_number)
    print("当前帧:", frame_number)   

    # 在窗口中显示帧
    cv2.imshow("RTSP Video Stream", frame)



    # 按 'q' 键退出循环
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# 释放 VideoCapture 对象和销毁所有窗口
cap.release()
cv2.destroyAllWindows()











