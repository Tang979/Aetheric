---
title : "Cấu hinh Amazon SES"
date : "2025-07-08"
weight : 1
chapter : false
pre : " <b> 3.1. </b> "
---

1. Mở [Amazon SES](https://ap-southeast-2.console.aws.amazon.com/ses/home?region=ap-southeast-2#/homepage)
    + Ở bên trái ở phân Configuration chọn Identities
![Connect](</images/SES/4.1.png>)

2. Tại trang Identities nhấp chọn vào phần create identities
![Connect](</images/SES/4.2.png>)

3. Tại trang **Create Identities**
    + Trong phần **Identity detaills** chọn mục **Email Address**
    + Dưới phần **Email address** ghi địa chỉ email của mình vô. Cuối cùng ấn **Create Identity**
![Connect](</images/SES/4.3.png>)

4. Sau đó ta vào email của mình nhấn vào đường link trong email của Amazon Web Service gửi về để xác thực tạo
![Connect](</images/SES/4.4.png>)

5. Sau đó ta quay trở lại trang Identities để coi xem cái identity mình vừa tạo đã Verified hay chưa. Nếu đã Verified coi như bạn đã cấu hình thành công Amazon SES
![Connect](</images/SES/4.5.png>)
tiếp theo ta sẽ đi cấu hình Lambda để sử lý logic gửi email
