---
title : "Xác thực tài khoản bằng Amazon Cognito"
date :  "2025-07-08" 
weight : 2 
chapter : false
pre : " <b> 2. </b> "
---

{{% notice info %}}
Bạn cần tạo sẵn 1 Linux instance thuộc public subnet và 1 Window instance thuộc private subnet để thực hiện bài thực hành này.
{{% /notice %}}

Để tìm hiểu cách tạo user pool các bạn có thể tham khảo bài lab :

- [Xác thực với Amazon Cognito](https://000081.awsstudygroup.com/vi/)

Để sử dụng System Manager để quản lý window instance nói riêng và các instance nói chung của chúng ta trên AWS, ta cần phải cung cấp quyền cho các instance của chúng ta có thể làm việc với System Manager.Trong phần chuẩn bị này, chúng ta cũng sẽ tiến hành tạo IAM Role để cấp quyền cho các instance có thể làm việc với System Manager.

### Nội dung

- [Tạo User Pool](2.1-createuserpool/)
