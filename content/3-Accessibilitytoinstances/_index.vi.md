---
title : "Đẩy dữ liệu từ Unity lên Amazon DynamoDB"
date :  "2025-07-08" 
weight : 3 
chapter : false
pre : " <b> 3. </b> "
---

Trong phần này, chúng ta sẽ xây dựng quy trình cho phép **client game Unity 6** gửi dữ liệu game (ví dụ: team tháp, cấp độ, tiến trình) lên **Amazon DynamoDB**, thông qua **API Gateway và AWS Lambda**, với lớp xác thực bảo mật sử dụng **Amazon Cognito**.

Do Unity 6 không hỗ trợ AWS SDK, tất cả tương tác với backend được thực hiện bằng **HTTP request** thông qua `UnityWebRequest`. Vì vậy, tính bảo mật và kiểm soát truy cập phải được thực hiện nghiêm ngặt từ phía API.

Trước khi có thể gửi và lưu dữ liệu, bạn cần cấu hình các dịch vụ backend, cấu hình **User Pool (Amazon Cognito)** ở phần trước đây là nơi quản lý người dùng, cung cấp JWT để xác thực với API Gateway.

### Nội dung
3.1. [Tạo bảng dữ liệu trên DynamoDB và cấu hình các phần cần thiết](3.1-public-instance/) \
<!-- 3.2. [Tạo Kết nối đến máy chủ EC2 Private](3.2-private-instance/)  -->
