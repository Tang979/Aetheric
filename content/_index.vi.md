---
title : "Modernizing Mobile Game Backend with AWS Cloud Services"
date :  "2025-07-08" 
weight : 1 
chapter : false
---
# Hiện đại hóa hệ thống backend cho game di động bằng các dịch vụ AWS

### Tổng quan

 Trong khuôn khổ workshop này, nhóm chúng tôi trình bày quá trình tích hợp các dịch vụ cốt lõi của AWS vào một dự án game di động thể loại thủ thành, được phát triển trên nền tảng Unity 6. Do **Unity 6 đã ngừng hỗ trợ chính thức AWS SDK**, chúng tôi xây dựng toàn bộ kiến trúc backend theo hướng **serverless** và thực hiện giao tiếp với các dịch vụ AWS thông qua **REST API** được ký bằng **AWS Signature Version 4** hoặc xác thực bằng **JWT từ Amazon Cognito**.

Việc cấu hình và tích hợp được thực hiện theo từng khối chức năng riêng biệt, đảm bảo hệ thống vừa an toàn, vừa có khả năng mở rộng và dễ dàng kiểm thử độc lập. Toàn bộ quá trình triển khai được thiết kế theo hướng thực chiến – ưu tiên hiệu quả, đơn giản, nhưng vẫn tuân thủ theo các nguyên tắc kiến trúc chuẩn của AWS (Well-Architected Framework).

<!-- ![ConnectPrivate](/images/arc-log.png)  -->

### Nội dung

 1. [Giới thiệu](1-introduce/)
 2. [Xác thực tài khoản Cognito](2-Prerequiste/)
 3. [Đẩy dữ liệu từ Unity lên Amazon DynamoDB](3-Accessibilitytoinstance/)
 <!-- 4. [Quản lý session logs](4-s3log/)
 5. [Port Forwarding](5-Portfwd/)
 6. [Dọn dẹp tài nguyên](6-cleanup/) -->
