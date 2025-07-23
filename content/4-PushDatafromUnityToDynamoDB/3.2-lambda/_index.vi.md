---
title : "Tạo lambda để tương tác với DynamoDB"
date :  "2025-07-08" 
weight : 2 
chapter : false
pre : " <b> 4.2. </b> "
---

### Tạo Lambda

1. Truy cập vào [giao diện của dịch vụ Lambda](https://us-east-1.console.aws.amazon.com/lambda/home?region=us-east-1#/begin) chọn **Create a function**.

   ![conectprivate](/images/3.connect/lambda.png)

2. Thiết lập function cho Lambda.

   + Nhập tên function của bạn **Function name**.
   + Ở **Runtime** bạn có thể chọn ngôn ngữ lập trình mà bạn muốn sử dụng.
   ![connnectprivate](/images/3.connect/lambda1.png)
   + Chọn **Create function**

### Cấu hình Permissions cho Role của Lambda

1. Truy cập vào function của [Lambda](https://ap-southeast-2.console.aws.amazon.com/lambda/home?region=ap-southeast-2#/functions?sb=LastModified&so=descending).

    {{% notice info %}}
Cần chỉnh region đúng với region lúc tạo function
{{% /notice %}}
   + Bạn sẽ thấy function mà chúng ta vừa tạo chọn vào function đó.
   ![lambda](/images/3.connect/lamda2.png)
   + Chọn **Configuration** sau đó chọn vào **Permissions** bạn sẽ thấy role của function chọn vào role đó để chuyển nhanh sang trang cấu hình của role đó.
   ![lambda](/images/3.connect/lambda3.png)
2. Cấu hình cho role của lambda.
   + Tại trang này chúng ta sẽ thêm policy cho role.
   ![lambda](/images/3.connect/role-lambda.png)
   + Sau đó chúng ta sẽ chọn policy mà chúng ta đã tạo ở phần trước.
   + Sau khi chọn policy kéo xuống cuối trang và chọn **Add permissions**.

### Viết code Lambda để tương tác với DynamoDB

1. Quay lại trang Lambda function của bạn và chọn tab **Code**.

2. Trong phần editor, bạn có thể viết code để tương tác với DynamoDB. Dưới đây là một ví dụ sử dụng Node.js với AWS SDK v3:

   ```javascript
   // Import AWS SDK v3
   import { DynamoDBClient } from "@aws-sdk/client-dynamodb";
   import { DynamoDBDocumentClient, PutCommand, GetCommand } from "@aws-sdk/lib-dynamodb";
   
   // Khởi tạo DynamoDB client
   const client = new DynamoDBClient({ region: "ap-southeast-2" }); // đảm bảo trùng với region của DynamoDB
   const docClient = DynamoDBDocumentClient.from(client);
   
   export const handler = async (event) => {
       console.log('=== FULL EVENT ===');
       console.log(JSON.stringify(event, null, 2));
       console.log('==================');
       
       // Cấu hình headers cho CORS
       const headers = {
           'Access-Control-Allow-Origin': '*',
           'Access-Control-Allow-Headers': 'Content-Type, User-Agent',
           'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
           'Content-Type': 'application/json',
           'Cache-Control': 'no-cache'
       };
       
       try {
           // Lấy dữ liệu từ event
           const data = JSON.parse(event.body || '{}');
           const { userId, score, level } = data;
           
           if (!userId) {
               return {
                   statusCode: 400,
                   headers,
                   body: JSON.stringify({ message: 'userId là bắt buộc' })
               };
           }
           
           // Tham số để lưu vào DynamoDB
           const params = {
               TableName: 'GameScores',
               Item: {
                   userId: userId,
                   timestamp: new Date().toISOString(),
                   score: score || 0,
                   level: level || 1,
               }
           };
           
           // Lưu dữ liệu vào DynamoDB sử dụng PutCommand
           const command = new PutCommand(params);
           await docClient.send(command);
           
           // Trả về kết quả thành công
           return {
               statusCode: 200,
               headers,
               body: JSON.stringify({
                   message: 'Dữ liệu đã được lưu thành công',
                   data: params.Item
               })
           };
       } catch (error) {
           // Xử lý lỗi
           console.error('Lỗi:', error);
           return {
               statusCode: 500,
               headers,
               body: JSON.stringify({
                   message: 'Đã xảy ra lỗi khi xử lý yêu cầu',
                   error: error.message
               })
           };
       }
   };
   ```

3. Sau khi viết xong code, nhấn **Deploy** để lưu và triển khai function.

    ![lambda](/images/3.connect/lambda-code.png)

### Kiểm tra Lambda function

1. Sau khi triển khai function, bạn có thể kiểm tra nó bằng cách tạo một test event.

2. Nhấn vào nút **Test** ở phía trên editor.

    ![lambda](/images/3.connect/lambda-test.png)

3. Tạo một test event mới với dữ liệu JSON như sau:

   ```json
   {
     "body": "{\"userId\":\"user123\",\"score\":100,\"level\":5}"
   }
   ```

4. Đặt tên cho test event và nhấn **Save**.

5. Nhấn **Test** để chạy function với test event đã tạo.

6. Kiểm tra kết quả thực thi và log để đảm bảo function hoạt động đúng.

{{% notice tip %}}
Đảm bảo rằng bảng DynamoDB đã được tạo trước khi chạy Lambda function.
{{% /notice %}}
