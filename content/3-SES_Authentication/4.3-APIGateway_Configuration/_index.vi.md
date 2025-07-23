---
title : "Cấu hình API GateWay"
date : "2025-07-08"
weight : 3
chapter : false
pre : " <b> 3.3. </b> "
---

1. Mở [Amazon API GateWay](https://ap-southeast-2.console.aws.amazon.com/apigateway/main/apis?region=ap-southeast-2) nhấn vào nút **Create an API** để để bắt đầu cấu hinh
 
![Connect](/images/SES/4.3/4.3.1.png)

2. Tiếp theo ta sẽ cấu hình phần **Configue API**
    + Trong phần **API name** tên đặt tên như ta muốn
    + Dưới phần **Integrations** ta chọn vào phần **Lambda**. Trong phần **Lambda funtion** chọn vào cái **Lambda** ta mới tạo.
    + sau khi xong ta ấn **next** để tiếp tục cấu hình.
![Connect](/images/SES/4.3/4.3.2.png)

3. Trong phần **Configure routes** ở dưới phần **Method** ta chọn vào phần **POST**. Sau khi xong ta chọn **next**
![Connect](/images/SES/4.3/4.3.3.png)

4. Ở bước cuối ta coi lại các bước ta cấu hình xem đã đầy đủ thông tin hay chưa. Sau khi đã kiểm tra ta chọn **create** để tạo API GateWay
![Connect](/images/SES/4.3/4.3.4.png)

Như thế ta đã cấu hình xong **API GateWay**, giờ ta sẽ bắt đầu tích hợp dịch vụ vào trong game.