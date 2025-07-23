---
title : "Giới thiệu"
date :  "2025-07-08" 
weight : 1 
chapter : false
pre : " <b> 1. </b> "
---
Đề tài "Hiện đại hóa hệ thống backend cho game di động bằng các dịch vụ đám mây AWS" tập trung vào việc xây dựng một **kiến trúc backend thực tiễn** phục vụ trò chơi di động được phát triển bằng **Unity 6**. Đây là loại kiến trúc ứng dụng mô hình **serverless cloud-native**, không sử dụng máy chủ truyền thống, mà tận dụng các dịch vụ AWS **có khả năng tự động mở rộng và tính phí theo mức sử dụng thực tế**.

## Ưu điểm của giải pháp

| Ưu điểm                     | Mô tả                                                                 |
|----------------------------|----------------------------------------------------------------------|
| Chi phí thấp               | Không cần thuê máy chủ EC2; toàn bộ dịch vụ là serverless, tối ưu chi phí vận hành khi số lượng người dùng thấp |
| Tự động mở rộng            | Lambda, DynamoDB, và API Gateway có khả năng scale tự động theo số lượng người dùng, không cần DevOps |
| Tích hợp trực tiếp với Unity | Giao tiếp bằng UnityWebRequest, không cần SDK, dễ kiểm soát request/response |
| Bảo mật mạnh mẽ            | Xác thực bằng JWT (Amazon Cognito), phân quyền bằng IAM theo nguyên tắc "Least Privilege" |
| Dễ kiểm thử và debug      | Sử dụng Postman test API trước khi tích hợp với Unity; log lỗi tập trung tại CloudWatch |

## Hạn chế và thách thức

| Hạn chế / Thách thức       | Mô tả                                                                 |
|----------------------------|----------------------------------------------------------------------|
| Không có SDK chính thức cho Unity 6 | Phải tự ký AWS Signature V4 thủ công hoặc dùng JWT; dễ phát sinh lỗi xác thực nếu sai định dạng |
| Lập trình viên phải hiểu kỹ về AWS IAM, Security Token | Nếu không nắm rõ quyền, rất dễ cấu hình sai, gây lỗi bảo mật hoặc không truy cập được dữ liệu |
| Khó mô phỏng backend offline | Vì server phụ thuộc vào nhiều dịch vụ cloud, không dễ mô phỏng toàn bộ offline trong quá trình phát triển |
| Chi phí có thể tăng khi scale lớn | Dù chi phí thấp ở giai đoạn đầu, nhưng nếu lên hàng chục ngàn user, cần đánh giá lại mô hình lưu trữ/tối ưu API |

## Đề tài phù hợp với ai?

- Lập trình viên game muốn học về kiến trúc cloud thực tế, dễ áp dụng cho game mobile.
- Nhóm sinh viên muốn xây dựng MVP nhanh, không cần tốn công DevOps.
- Người học AWS cần một bài toán "real-world" để thực hành Lambda, Cognito, API Gateway, DynamoDB một cách liền mạch và có ý nghĩa.