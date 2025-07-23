---
title : "Cấu hình API Gateway để sử dụng Lambda tương tác với DynamoDB"
date :  "2025-07-08" 
weight : 3 
chapter : false
pre : " <b> 4.3. </b> "
---

### Tạo HTTP API Gateway

1. Truy cập vào [giao diện của dịch vụ API Gateway](https://ap-southeast-2.console.aws.amazon.com/apigateway/main/apis?region=ap-southeast-2) và chọn **Create API**.

2. Chọn **HTTP API** và nhấn **Build**.

   ![apigateway](/images/3.connect/apigateway1.png)

3. Ở màn hình tạo API:
   + Đặt tên cho API của bạn trong trường **API name**
   + Trong phần **Integrations**, chọn **Add integration**
   + Chọn **Lambda** làm loại tích hợp
   + Chọn Lambda function đã tạo ở bước trước
   + Ở phần **API endpoint type**, chọn **Regional**
   + Ở phần **CORS**, để mặc định hoặc bật tùy theo nhu cầu của bạn
   + Nhấn **Next**

   ![apigateway](/images/3.connect/httpapi1.png)

### Cấu hình Routes (Tuyến đường)

1. Ở màn hình Configure routes:
   + Chọn phương thức **POST**
   + Nhập đường dẫn **/scores**
   + Chọn Lambda function của bạn làm integration target (Mặc định sẽ là Lambda function mà bạn đã chọn ở bước trên)
   + Nhấn **Next**

   ![apigateway](/images/3.connect/httpapi2.png)

### Cấu hình Stage

1. Ở màn hình Configure stages:
   + Giữ stage mặc định **$default**
   + Bạn có thể thêm các stage khác nếu cần
   + Nhấn **Next**

   ![apigateway](/images/3.connect/httpapi3.png)

2. Xem lại các thiết lập và nhấn **Create**.

### CORS (Cross-Origin Resource Sharing)

Một trong những ưu điểm của HTTP API là nó đã hỗ trợ CORS mặc định. Khi tạo API, bạn có thể đã cấu hình CORS trong quá trình tạo. Nếu cần điều chỉnh thêm:

1. Chọn API của bạn và chọn tab **CORS**.

2. Ở đây bạn có thể cấu hình:
   + **Access-Control-Allow-Origins**: Mặc định là '*' (cho phép tất cả các nguồn)
   + **Access-Control-Allow-Headers**: Mặc định đã bao gồm 'Content-Type,Authorization'
   + **Access-Control-Allow-Methods**: Mặc định đã bao gồm các phương thức cơ bản

{{% notice info %}}
Với ứng dụng Unity, CORS thường không phải là vấn đề lớn vì các yêu cầu không được gọi từ trình duyệt web. Tuy nhiên, cấu hình CORS vẫn hữu ích nếu bạn cũng sử dụng API này cho các ứng dụng web.
{{% /notice %}}

### Lấy URL của API

1. Chọn tab **Stages** và bạn sẽ thấy URL của API của bạn trong phần **Invoke URL**.

   ![apigateway](/images/3.connect/httpapi6.png)

2. URL này sẽ có dạng: `https://api-id.execute-api.ap-southeast-2.amazonaws.com`

3. Để gọi API của bạn, bạn sẽ sử dụng URL này kết hợp với đường dẫn đã định nghĩa (ví dụ: `https://api-id.execute-api.ap-southeast-2.amazonaws.com/scores`)

### Kiểm tra API

1. Bạn có thể sử dụng công cụ như Postman hoặc curl để kiểm tra API của bạn.

2. Ví dụ sử dụng curl:

   ```bash
   curl -X POST \
     https://your-api-id.execute-api.ap-southeast-2.amazonaws.com/scores \
     -H 'Content-Type: application/json' \
     -d '{
       "userId": "user123",
       "score": 100,
       "level": 5
     }'
   ```

3. Hoặc bạn có thể sử dụng một đoạn code JavaScript đơn giản để gọi API:

   ```javascript
   fetch('https://your-api-id.execute-api.ap-southeast-2.amazonaws.com/scores', {
     method: 'POST',
     headers: {
       'Content-Type': 'application/json',
     },
     body: JSON.stringify({
       userId: 'user123',
       score: 100,
       level: 5
     }),
   })
   .then(response => response.json())
   .then(data => console.log(data))
   .catch(error => console.error('Error:', error));
   ```

### Sử dụng API trong Unity

Bạn có thể sử dụng UnityWebRequest để gọi API từ game Unity của bạn:

```csharp
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    private readonly string apiUrl = "https://your-api-id.execute-api.ap-southeast-2.amazonaws.com/scores";

    public void SaveScore(string userId, int score, int level)
    {
        StartCoroutine(PostScore(userId, score, level));
    }

    private IEnumerator PostScore(string userId, int score, int level)
    {
        // Tạo JSON data
        string jsonData = JsonUtility.ToJson(new ScoreData
        {
            userId = userId,
            score = score,
            level = level
        });

        // Tạo request
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Gửi request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score saved successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error saving score: " + request.error);
        }
    }
}

[System.Serializable]
public class ScoreData
{
    public string userId;
    public int score;
    public int level;
}
```

### Kết luận

Bây giờ bạn đã có một HTTP API hoàn chỉnh để lưu trữ dữ liệu game vào DynamoDB thông qua Lambda function. API này được tối ưu hóa cho ứng dụng di động và game Unity, với độ trễ thấp và hiệu suất cao.

{{% notice tip %}}
Khi sử dụng API trong môi trường sản xuất, bạn nên cân nhắc việc thêm xác thực và giới hạn tốc độ để bảo vệ API của bạn.
{{% /notice %}}
