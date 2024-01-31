import pyshark
import binascii

# 指定 pcap 文件路径
pcap_file = 'ais_udp.pcapng'
output_file = 'ais_data_wireshark.txt'

# 使用 pyshark 打开 pcap 文件
cap = pyshark.FileCapture(pcap_file, display_filter='udp')


# 保存第一个数据包的时间戳
first_timestamp = None

i = 0
# 打开输出文件
with open(output_file, 'w') as file:
    # 遍历每个 UDP 数据包
    for packet in cap:
        # 获取 UDP 数据包的内容
        udp_payload = bytes.fromhex(packet.data.data)
        udp_payload = udp_payload.decode('ascii')
        timestamp = packet.sniff_time
        milliseconds = int(timestamp.timestamp() * 1000)
        udp_payload = udp_payload.strip()

        # print(milliseconds)
        pos = udp_payload.find('!AIVDM')
        if pos != -1:
            udp_payload = udp_payload[pos:]
            # print(udp_payload)

            if first_timestamp is None:
                first_timestamp = milliseconds

            relative_timestamp = milliseconds - first_timestamp

            # 将内容和时间写入文件        
            file.write(f"{udp_payload}\n")
            file.write(f"{relative_timestamp}\n")
            # file.write(f"{milliseconds}\n")

            # i += 1
            # if i > 5:
            #     break

# 关闭 pcap 文件
cap.close()
