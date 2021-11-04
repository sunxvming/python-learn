#!/usr/bin/python
# coding=utf-8
from email.mime.base import MIMEBase
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
import smtplib

__author__ = 'sunxvming'

SMTP = "smtp.163.com"
USER = "sunxvming@163.com"
PASS = "ming1618v"


def send_mail(to, subject, txt, attach_file_name = None):
    msg = MIMEMultipart('alternative')
    # 注意包含了非ASCII字符，需要使用unicode
    # if not isinstance(subject, unicode):   # Linux下需要
    # subject = unicode(subject)
    msg['Subject'] = subject
    msg['From'] = USER
    msg['To'] = ','.join(to)
    part = MIMEText(txt, 'plain', 'utf-8')
    msg.attach(part)

    # 附件
    if attach_file_name is None:
        pass
    else:
        data = open(attach_file_name, "rb").read()
        contype = 'application/octet-stream'
        maintype, subtype = contype.split('/', 1)
        file_msg = MIMEBase(maintype, subtype)
        file_msg.set_payload(data, 'base64')
        file_msg.add_header('Content-Disposition', 'attachment', filename=attach_file_name)
        msg.attach(file_msg)

    '''
    发送
    默认smtp是25端口，在windows上测试的没问题，弄到腾讯云的linux服务器上就卡死在smtplib.SMTP(SMTP) 这一步
    死活connect不上，后来查询到是腾讯云为防止用户乱发垃圾邮件，然后把25端口直接给禁掉了，解封了就可以用了
    '''
    server = smtplib.SMTP(SMTP)   
    server.login(USER, PASS)
    server.sendmail(USER, to, msg.as_string())
    server.quit()

if __name__ == '__main__':
    send_mail(['466205048@qq.com'],'mailtest','hello,bbbbbbb')
