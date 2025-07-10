---
title : "Giới thiệu các dịch vụ sử dụng"
date :  "2025-07-08" 
weight : 1 
chapter : false
pre : " <b> 1. </b> "
---
### Dịch vụ Amazon Cognito ###
**Amazon Cognito** là một dịch vụ của AWS giúp quản lý và xác thực người dùng một cách an toàn mà **không cần tự xây dựng hệ thống đăng ký, đăng nhập, hay xử lý mật khẩu**. Cognito hỗ trợ đăng nhập bằng email, số điện thoại, mạng xã hội (Google, Facebook, Apple), hoặc tài khoản doanh nghiệp thông qua SAML, đồng thời sử dụng token bảo mật theo chuẩn OAuth2 để xác thực khi truy cập các dịch vụ backend.

Cognito cho phép quản lý người dùng tập trung thông qua IAM, hỗ trợ xác thực đa yếu tố (MFA), phân quyền truy cập theo nhóm và cung cấp khả năng mở rộng linh hoạt không giới hạn người dùng. Ngoài ra, các sự kiện đăng nhập và lỗi cũng được ghi log để phục vụ kiểm tra và bảo mật

- Với việc sử dụng Amazon Cognito, bạn sẽ có được những ưu điểm sau:
- Không cần tự viết hệ thống đăng nhập, quên mật khẩu hay xác thực email.
- Hỗ trợ đăng nhập đa dạng: email, mạng xã hội, tài khoản doanh nghiệp.
- Quản lý user tập trung bằng IAM và phân quyền rõ ràng.
- Tích hợp dễ dàng với API Gateway, Lambda, DynamoDB, S3...
- Ghi log hoạt động người dùng, tăng cường bảo mật.
- Tự động mở rộng quy mô mà không cần quản lý server.

Với những ưu điểm trên, bạn có thể sử dụng Session Manager thay vì sử dụng kỹ thuật Bastion host giúp chúng ta tiết kiệm được thời gian và chi phí khi quản lý server Bastion.

### Dịch vụ Amazon SES ###
**Amazon SES** (Simple Email Service) là dịch vụ gửi email chuyên nghiệp của AWS, cho phép bạn **gửi email tự động từ ứng dụng hoặc game mà không cần xây dựng hệ thống email server riêng**. SES hỗ trợ gửi email thông báo, xác nhận, khôi phục mật khẩu, và thu thập phản hồi từ người dùng một cách nhanh chóng, bảo mật và đáng tin cậy.

SES giúp bạn dễ dàng cấu hình để gửi email từ một địa chỉ tùy chọn (ví dụ: support@yourgame.com) và hỗ trợ xác thực SPF, DKIM nhằm đảm bảo email không bị đánh dấu spam. Ngoài ra, bạn có thể kết hợp với AWS Lambda hoặc API Gateway để gửi email từ client như Unity mà không lộ thông tin nhạy cảm.

- Với việc sử dụng Amazon SES, bạn sẽ có được những ưu điểm sau:
- Gửi email xác nhận, reset password, hoặc feedback tự động.
- Không cần dựng hoặc quản lý máy chủ gửi mail riêng.
- Tích hợp dễ dàng với các dịch vụ như Lambda, API Gateway, S3.
- Hỗ trợ xác thực tên miền để tăng độ tin cậy của email.
- Theo dõi được tỷ lệ gửi thành công, email bị từ chối hoặc báo spam.
- Chi phí thấp, có thể gửi hàng nghìn email mỗi tháng miễn phí (trong AWS).

Với những ưu điểm trên, Amazon SES là một giải pháp hiệu quả, bảo mật và tiết kiệm để bạn xây dựng hệ thống gửi email trong game hoặc ứng dụng mà không cần lo lắng về hạ tầng gửi mail.

### Dịch vụ Amazon DynamoDB ###
**Amazon DynamoDB** là dịch vụ cơ sở dữ liệu NoSQL của AWS, cho phép bạn **lưu trữ và truy xuất dữ liệu nhanh chóng, linh hoạt mà không cần quản lý hạ tầng máy chủ. Với DynamoDB**, bạn có thể dễ dàng lưu các thông tin như tài khoản người chơi, điểm số, tiến độ game, vật phẩm, hoặc dữ liệu nâng cấp tháp trong game.

DynamoDB hoạt động theo mô hình key-value và document, cho phép truy cập dữ liệu gần như tức thì, ngay cả khi có hàng triệu người chơi cùng lúc. Dịch vụ này tự động mở rộng quy mô và cung cấp khả năng sao lưu, phục hồi, đồng bộ dữ liệu đa vùng và kiểm soát truy cập chi tiết thông qua IAM.

- Với việc sử dụng Amazon DynamoDB, bạn sẽ có được những ưu điểm sau:
- Lưu trữ dữ liệu người chơi ổn định, truy xuất nhanh với độ trễ thấp.
- Không cần cài đặt, bảo trì hoặc mở rộng cơ sở dữ liệu thủ công.
- Tích hợp dễ dàng với Lambda, API Gateway và Cognito.
- Hỗ trợ truy vấn có điều kiện và chỉ số phụ để tăng tốc tìm kiếm.
- Có thể sao lưu, khôi phục và phân quyền truy cập rõ ràng.
- Dùng tốt cho cả backend game, mobile, web với chi phí tối ưu.

Với những ưu điểm trên, DynamoDB là lựa chọn lý tưởng để lưu trữ dữ liệu game quy mô lớn, đảm bảo hiệu năng cao và tiết kiệm chi phí vận hành.