---
title : "kiểm tra kết nối đến unity"
date : "2025-07-08"
weight : 2
chapter : false
pre : " <b> 2.2. </b> "
---

1. Vào tài khoản mình mới tạo trong user pool, ở mục **Overview** copy ```ap-southeast-2```
    ![Connect](/images/2.prerequisite/2.2/2.2.2.png)

2. Trong phần app client user copy cái **Client ID**
    ![Connect](/images/2.prerequisite/2.2/2.2.1.png)

3. Vào trong project của mình, vào đoạn code sử lý trang Register và Confirm tạo ra 2 biến clientId và region. Gán cái client ID và region trong **User pool** của mình vô trong.
    ![Connect](/images/2.prerequisite/2.2/2.2.3.png)

4. Nhập thông tin của bạn vào và ấn register
    ![Connect](/images/2.prerequisite/2.2/2.2.4.png)

5. Sau khi Register thành công thì email sẽ gửi cho bạn 1 đoạn mã xác nhận, copy đoạn mã đó
    ![Connect](/images/2.prerequisite/2.2/2.2.5.png)

6. Quay lại vào phần Verify Email
    - Nhập mã Confirmation bạn đã ghi lại.
    - Nhấp vào nút Submit

    ![Connect](/images/2.prerequisite/2.2/2.2.6.png)

7. Quay lại **Amazon Cognito** vào phần **User** trong User **User Management**
    ![Connect](/images/2.prerequisite/2.2/2.2.7.png)

8. Ấn vào user vừa mới tạo, kéo xuống phần **User attributes** sẽ thấy thông tin của mình
    ![Connect](/images/2.prerequisite/2.2/2.2.8.png)
