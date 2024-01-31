
import cv2

# video_path = "original_data/2022.09.05 16_57_38.206.mp4"
# video_path = "original_data/20230605_20230603011515_20230603012442_193156.mp4"
video_path = "original_data/2022.09.02 17_17_42.065.mp4"



# video_path = "test.mp4"
output_folder = "out_img/"

cap = cv2.VideoCapture(video_path)
frame_rate = cap.get(cv2.CAP_PROP_FPS)
interval = int(frame_rate)  # 每秒提取一帧
print("frame_rate is:", frame_rate)
print("interval is:", interval)

count = 0

while cap.isOpened():
    # print(count)
    ret, frame = cap.read()
    
    if not ret:
        break

    if count % interval == 0:  # 每隔interval帧提取一帧
        print("save frame:", count)
        second = int(count / interval)
        output_file = output_folder + f"second_{second}.jpg"
        # print(output_file, frame)
        is_ok = cv2.imwrite(output_file, frame)
        if is_ok:
            print("image saved")
        else:
            print("Failed saving the image" )
    count += 1

cap.release()
