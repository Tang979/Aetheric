---
title : "Tạo user pool"
date : "2025-07-08"
weight : 1
chapter : false
pre : " <b> 2.1. </b> "
---

1. Mở [Amazon Cognito cosole](https://ap-southeast-2.console.aws.amazon.com/cognito/v2/idp/user-pools?region=ap-southeast-2)
    + Chọn User pools trên menu bên trái.
    + Nhấp vào Create user pool.
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 100815.png>)

2. tại trang Set up resources for your application
    + Ở phần **Define your application** chọn Mobile app
    + Dưới phần **Configure options** chọn **Emai**, **Username**
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 101949.png>)
    + Ở dưới **Required attributes for sign-up** chọn **Email**
    + Cuối cùng ấn Create User Directory
![Connect](</images/2.prerequisite/user_pool/Screenshot 2025-07-09 102420.png>)    

3. Quay trở lại [Amazon Cognito cosole](https://ap-southeast-2.console.aws.amazon.com/cognito/v2/idp/user-pools?region=ap-southeast-2) click vào user pool mới tạo
![Connect](/images/2.prerequisite/user_pool/buoc4.png)

4. Ở bên trái phần Application click vào app client
![Connect](/images/2.prerequisite/user_pool/buoc5.png)

5. Ở **App clients and analytics ** chọn vào cái app clients name mới tạo
![Connect](/images/2.prerequisite/user_pool/buoc6.png)
    + trong **App client** chọn vào edit

6. Ở trong **Edit app client information** chọn vào các mục sau
    + Choice-based sign-in: ALLOW_USER_AUTH
    + Sign in with username and password: ALLOW_USER_PASSWORD_AUTH
    + Get new user tokens from existing authenticated sessions: ALLOW_REFRESH_TOKEN_AUTH
rồi kéo xuống cuối chọn **Save Change**
![Connect](/images/2.prerequisite/user_pool/buoc7.png)

vậy là hoàn thành bước tạo User Pool