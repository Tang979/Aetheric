---
title : "Cấu hình Lambda"
date : "2025-07-08"
weight : 2
chapter : false
pre : " <b> 3.2. </b> "
---

1. Mở [Lambda](https://ap-southeast-2.console.aws.amazon.com/lambda/home?region=ap-southeast-2#/discover). trong phần **Dashboard** nhấn vào **Create function**
![Connect](</images/SES/4.2/4.2.1.png>)

2. Trong trang **Create function** làm các bước sau:
    + ở mục **Function name** thì ghi tên mà mình muốn đặt
    + Ở dưới phần **Runtime** thì chọn ngôn ngữ để viết code sử lý gửi email. Như ở trong bài thì chọn **Python 3.12**
    + Ở phần **Permissions** ta tích vào **Create a new role with basic Lambda permissions** 
    + sau khi làm xong các bước trên kéo xuống cuối và chọn **Create function**
![Connect](</images/SES/4.2/4.2.2.png>)

3. Sau khi tạo xong Lambda kéo xuống dưới, ở phần code thì dán đoạn code này vô. Sau khi thêm vô thì nhấn vào nút **deploy** ở bên trái

    import boto3

    import json

    ses = boto3.client('ses', region_name='ap-southeast-2')

    def lambda_handler(event, context):

        body = json.loads(event['body'])

        sender = "minhallk.vk@gmail.com"  # Email đã xác minh
        to_email = "minhallk.vk@gmail.com"  # Email nhận feedback
        subject = body.get("subject", "Phản hồi từ người dùng")
        message = body.get("message", "Không có nội dung")
        user_email = body.get("user_email", "Không cung cấp")

        # Gửi email bằng SES
        response = ses.send_email(
            Source=sender,
            Destination={
                'ToAddresses': [to_email]
            },
            Message={
                'Subject': {
                    'Data': subject
                },
                'Body': {
                    'Text': {
                        'Data': f"Từ: {user_email}\n\nNội dung:\n{message}"
                    }
                }
            }
        )

        return {
            'statusCode': 200,
            'body': json.dumps({'message': 'Gửi feedback thành công!'})
        }
![Connect](</images/SES/4.2/4.2.3.png>)

4. Tiếp theo ta sẽ cấp quyền SES cho Lambda
    + Vào **Configuration** nhấn vào **Permissions**
    + Bấm vào tên Role dưới dòng Execution role
    + Bấm **Add permissions** rồi ấn thêm **Attach policies**
    + Tìm **AmazonSESFullAccess** tích vào nó rồi ấn **Add permissions**
![Connect](</images/SES/4.2/4.2.4.png>)
![Connect](</images/SES/4.2/4.2.5.png>)
![Connect](</images/SES/4.2/4.2.6.png>)

Như thế là đã cấu hình xong Lambda, tiếp theo là cấu hình API GateWay
    
