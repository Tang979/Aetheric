---
title : "Tạo S3 Bucket cho Asset Game"
date :  "2025-07-08" 
weight : 1 
chapter : false
pre : " <b> 5.1. </b> "
---

### Truy cập vào Amazon S3

1. Truy cập vào trang [S3](https://ap-southeast-2.console.aws.amazon.com/s3/home?region=ap-southeast-2#)
2. Nhấn vào nút **Create bucket**

![S3Console](/images/4.s3/s3-console.png)

### Cấu hình bucket

1. **Thông tin cơ bản**:
   - **Bucket name**: Nhập tên bucket của bạn (ví dụ: `mygame-assets`)

   ![S3Name](/images/4.s3/s3-name.png)

2. **Object Ownership**: Giữ mặc định "ACLs disabled"

3. **Block Public Access settings**: Mặc định, S3 sẽ chặn tất cả quyền truy cập công khai. Để cho phép asset được tải từ game, bỏ chọn "Block all public access"

   ![S3Public](/images/4.s3/s3-public.png)

4. Nhấn **Create bucket**

### Cấu hình CORS (Cross-Origin Resource Sharing)

CORS cho phép game Unity của bạn tải asset từ S3 bucket:

1. Chọn bucket vừa tạo
2. Chọn tab **Permissions**
3. Cuộn xuống phần **Cross-origin resource sharing (CORS)**
4. Nhấn **Edit**
5. Nhập cấu hình CORS sau:

    ```json
    [
        {
            "AllowedHeaders": [
                "*"
            ],
            "AllowedMethods": [
                "GET",
                "HEAD"
            ],
            "AllowedOrigins": [
                "*"
            ],
            "ExposeHeaders": []
        }
    ]
    ```

6. Nhấn **Save changes**

![S3CORS](/images/4.s3/s3-cors.png)

### Tạo chính sách bucket (Bucket Policy)

Để cho phép truy cập công khai vào asset:

1. Vẫn trong tab **Permissions**, cuộn đến phần **Bucket policy**
2. Nhấn **Edit**
3. Nhập chính sách sau (thay `mygame-assets` bằng tên bucket của bạn):

    ```json
    {
        "Version": "2012-10-17",
        "Statement": [
            {
            "Sid": "PublicReadForGetBucketObjects",
            "Effect": "Allow",
            "Principal": "*",
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::mygame-assets/*"
            }
        ]
    }
    ```

4. Nhấn **Save changes**

![S3Policy](/images/4.s3/s3-policy.png)

### Tải asset lên S3

1. Quay lại tab **Objects**
2. Nhấn **Create folder** để tạo cấu trúc thư mục (ví dụ: `textures`, `models`, `audio`, `bundles`)
3. Để tải asset lên:
   - Chọn thư mục đích
   - Nhấn **Upload**
   - Chọn file từ máy tính của bạn
   - Nhấn **Upload**

### Lấy URL của asset

Sau khi tải lên, bạn có thể lấy URL của asset để sử dụng trong Unity:

1. Chọn asset đã tải lên
2. Chọn đến file mà bạn đã upload sẽ hiển thị **Object URL**
3. URL này có dạng: `https://mygame-assets.s3.amazonaws.com/textures/character.png`

### Lưu ý bảo mật

Cấu hình trên cho phép bất kỳ ai cũng có thể truy cập asset của bạn. Trong môi trường sản xuất, bạn nên:

1. Sử dụng CloudFront với OAI (Origin Access Identity) để hạn chế truy cập trực tiếp vào S3
2. Sử dụng Signed URLs hoặc Cookies để kiểm soát quyền truy cập
3. Cân nhắc sử dụng AWS WAF để bảo vệ khỏi các cuộc tấn công
