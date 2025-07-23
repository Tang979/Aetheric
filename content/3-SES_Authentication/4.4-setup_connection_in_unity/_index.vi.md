---
title : "Kết nối vào Unity"
date : "2025-07-08"
weight : 4
chapter : false
pre : " <b> 3.4. </b> "
---

1. Mở lại phần **API GateWay** vào lại cái API mình mới tạo.
    + Ở bên phải dưới phần **Deploy** ta chọn vào **Stages**
    + Trong phần **Stages** tích vào cái **$default** để mở bảng **Stages Detail**
    + trong phần **Stages Detail** copy cái URL.
![Connect](/images/SES/4.4/4.4.1.png)

2. Mở lại **Lambda** kéo xuống phần code tìm 2 mục **sender** và **to_email** ghi email của mình vào đó.
![Connect](/images/SES/4.4/4.4.3.png)

3. Vào trong game tạo 1 cái feedback đơn giản để test thử. Đăng nhập bằng 1 tài khoản khác của mình.

![Connect](/images/SES/4.4/4.4.2.png)

4. Tạo 1 scrip để sử lý feedback

![Connect](/images/SES/4.4/4.4.5.png)
    Dán cái URL copy ở API GateWay vào trong đoạn code

5. sau khi thiết lập xong thì mình test thử
![Connect](/images/SES/4.4/4.4.4.png)
    Sau khi đã gửi feedback thì vào trong email chính của chúng ta xem đã có feedback trả về hay chưa
![Connect](/images/SES/4.4/4.4.6.png)
    Vậy là chúng ta đã thành công sử dụng dịch vụ SES để gửi feedback từ unity về email