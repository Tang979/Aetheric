---
title : "Tạo bảng dữ liệu trên DynamoDB và cấu hình các phần cần thiết"
date :  "2025-07-08" 
weight : 1 
chapter : false
pre : " <b> 4.1. </b> "
---

### Tạo bảng dữ liệu trên DynamoDB

1. Truy cập vào [giao diện quản trị của dịch vụ DynamoDB](https://ap-southeast-2.console.aws.amazon.com/dynamodbv2/home) chọn **Create table**.

   ![dynamodb](/images/3.connect/create-table.png)

2. Thiết lập thông tin bảng.

   + Nhập tên table mà bạn muốn tạo ở mục đầu tiên **Table name**.
   + Nhập **Partition key** đây chính là primary key cho table của bạn bạn có thể **chọn kiểu dữ liệu** của key ở bên cạnh.
   + Click **Create table**.

   ![dynamodb](/images/3.connect/config-table.png)

3. Truy cập vào [Cognito](https://ap-southeast-2.console.aws.amazon.com/cognito/).

   + Click chọn **Identity pools**.
   + Click chọn **Create identity pool**.
   ![connectprivate](/images/3.connect/create-identitypools.png)
   + Chọn **Authenticated access** sau đó chọn những loại đăng nhập mà bạn muốn xác thực sau đó chọn **Next**.
   ![connectprivate](/images/3.connect/identitypool1.png)
   + Đặt tên cho **IAM role name** sau đó chọn **Next**.
   ![connectprivate](/images/3.connect/identitypool2.png)
   + Ở đậy chúng ta sẽ chọn **User pool ID**  mà bạn muốn xác thực và chọn **App client ID** tương ứng sau đó chọn **Next**.
   + Đặt tên cho **Identity pool** sau đó bạn có thể chọn **Next** xem lại các thông tin đã cấu hình và chọn **Create identity pool**.

4. Tạo [Policy](https://us-east-1.console.aws.amazon.com/iam/home?region=ap-southeast-2#/policies) cấp quyền cho Cognito tương tác với DynamoDB.

   + Chọn **Create plolicy**.
   + ![connectprivate](/images/3.connect/policy1.png)
   + Chuyển sang chỉnh sửa Json và cập nhật lại quyền truy cập vào DynamoDB như sau:
   ![connectprivate](/images/3.connect/policy2.png)

      ```json
      {
        "Version": "2012-10-17",
        "Statement": [
            {
                "Effect": "Allow",
                "Action": [
                    "dynamodb:GetItem",
                    "dynamodb:PutItem",
                    "dynamodb:UpdateItem"
                ],
                "Resource": "arn:aws:dynamodb:[Region]:*:table/[Name table]"
            }
        ]
      }
      ```

   + Sau đó chọn **Next**.
   + Đặt tên cho Policy và **Create policy**.

5. Truy cập vào [giao diện quản trị của dịch vụ IAM](https://us-east-1.console.aws.amazon.com/iam/home)

   + Chọn **Roles** ở thanh trượt bên trái chọn vào Role mà bạn đã đặt tên lúc tạo **Identity pool**.
   ![connectprivate](/images/3.connect/role.png)
   + Chọn **Add permissions** sau đó chọn **Attach polichies**.
   ![connectprivate](/images/3.connect/role3.png)
   + **Tìm tên** và chọn **Policy** mà bạn vừa tạo sau đó chọn **Add permissions**.
   + Sau đó chọn tab **Trust relationships** chọn **Edit tust policy**.
   ![connectprivate](/images/3.connect/role2.png)
   + Cập nhật lại quyền mới như sau:

    ```json
    {
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Principal": {
                "Federated": "cognito-identity.amazonaws.com"
            },
            "Action": "sts:AssumeRoleWithWebIdentity",
            "Condition": {
                "StringEquals": {
                    "cognito-identity.amazonaws.com:aud": "[Identity pool ID]"
                },
                "ForAnyValue:StringLike": {
                    "cognito-identity.amazonaws.com:amr": "authenticated"
                }
            }
        }
      ]
    }
    ```
