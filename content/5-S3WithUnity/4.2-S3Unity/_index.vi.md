---
title : "Tải Asset Động từ S3"
date :  "2025-07-08" 
weight : 2
chapter : false
pre : " <b> 5.2 </b> "
---

## Tải Asset Động từ Amazon S3 vào Unity

Trong phần này, chúng ta sẽ tìm hiểu cách tải các asset động (như hình ảnh, âm thanh, mô hình 3D) từ Amazon S3 vào game Unity của bạn bằng cách sử dụng URL của S3.

### 1. Chuẩn bị

Trước khi bắt đầu, hãy đảm bảo bạn đã:

- Tạo bucket S3 và cấu hình như trong phần trước
- Tải các asset lên bucket S3 của bạn
- Đảm bảo các asset có thể truy cập công khai hoặc có cấu hình CORS phù hợp

### 2. Tạo Script Tải Asset từ S3 bằng URL

Trong Unity, chúng ta sẽ tạo một script đơn giản để tải asset từ S3 bằng cách sử dụng URL trực tiếp:

```csharp
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class S3AssetLoader : MonoBehaviour
{
    // URL của asset trên S3
    public string assetUrl = "https://your-bucket-name.s3.amazonaws.com/path-to-your-asset.png";
    
    // Tham chiếu đến các component sẽ sử dụng asset
    public UnityEngine.UI.RawImage imageDisplay;
    public AudioSource audioSource;
    
    void Start()
    {
        // Tự động tải asset khi khởi động
        StartCoroutine(LoadAssetFromS3());
    }
    
    // Hàm để tải asset từ URL
    public IEnumerator LoadAssetFromS3()
    {
        Debug.Log("Bắt đầu tải asset từ: " + assetUrl);
        
        // Xác định loại asset từ URL
        if (assetUrl.EndsWith(".png") || assetUrl.EndsWith(".jpg") || assetUrl.EndsWith(".jpeg"))
        {
            yield return StartCoroutine(LoadImageFromUrl(assetUrl));
        }
        else if (assetUrl.EndsWith(".mp3") || assetUrl.EndsWith(".wav") || assetUrl.EndsWith(".ogg"))
        {
            yield return StartCoroutine(LoadAudioFromUrl(assetUrl));
        }
        else
        {
            Debug.LogError("Định dạng file không được hỗ trợ!");
        }
    }
    
    // Tải hình ảnh từ URL
    private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                
                // Hiển thị hình ảnh nếu có component RawImage
                if (imageDisplay != null)
                {
                    imageDisplay.texture = texture;
                    Debug.Log("Đã tải và hiển thị hình ảnh thành công!");
                }
                else
                {
                    Debug.Log("Đã tải hình ảnh thành công, nhưng không có RawImage để hiển thị!");
                }
            }
            else
            {
                Debug.LogError("Lỗi khi tải hình ảnh: " + www.error);
            }
        }
    }
    
    // Tải âm thanh từ URL
    private IEnumerator LoadAudioFromUrl(string url)
    {
        // Xác định loại audio từ URL
        AudioType audioType = AudioType.UNKNOWN;
        if (url.EndsWith(".mp3")) audioType = AudioType.MPEG;
        else if (url.EndsWith(".wav")) audioType = AudioType.WAV;
        else if (url.EndsWith(".ogg")) audioType = AudioType.OGGVORBIS;
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                
                // Phát âm thanh nếu có AudioSource
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("Đã tải và phát âm thanh thành công!");
                }
                else
                {
                    Debug.Log("Đã tải âm thanh thành công, nhưng không có AudioSource để phát!");
                }
            }
            else
            {
                Debug.LogError("Lỗi khi tải âm thanh: " + www.error);
            }
        }
    }
    
    // Hàm để tải asset theo yêu cầu (có thể gọi từ button)
    public void LoadAssetOnDemand(string url)
    {
        assetUrl = url;
        StartCoroutine(LoadAssetFromS3());
    }
}
```

### 3. Sử dụng Script trong Unity

#### Bước 1: Tạo GameObject và gắn Script

1. Trong Unity, tạo một GameObject mới (GameObject > Create Empty)
2. Đặt tên là "S3AssetManager"
3. Thêm component S3AssetLoader vào GameObject (Add Component > Scripts > S3AssetLoader)

#### Bước 2: Thiết lập URL và tham chiếu

1. Trong Inspector, nhập URL của asset trên S3 vào trường "Asset Url"
   - Ví dụ: `https://your-bucket-name.s3.amazonaws.com/images/character.png`
2. Kéo các component cần thiết vào các trường tương ứng:
   - Kéo một GameObject có component RawImage vào trường "Image Display" để hiển thị hình ảnh
   - Kéo một GameObject có component AudioSource vào trường "Audio Source" để phát âm thanh

#### Bước 3: Tạo UI để tải asset theo yêu cầu

```csharp
using UnityEngine;
using UnityEngine.UI;

public class AssetLoadButton : MonoBehaviour
{
    public S3AssetLoader assetLoader;
    public InputField urlInput;
    
    public void LoadAssetFromInput()
    {
        if (urlInput != null && !string.IsNullOrEmpty(urlInput.text))
        {
            assetLoader.LoadAssetOnDemand(urlInput.text);
        }
        else
        {
            Debug.LogError("URL không hợp lệ hoặc trống!");
        }
    }
}
```

### 4. Lưu ý bảo mật

1. **Không lưu trữ thông tin nhạy cảm**: Không nhúng AWS credentials vào code Unity
2. **Sử dụng backend làm trung gian**: Tốt nhất là sử dụng backend server để tạo pre-signed URL
3. **Giới hạn quyền truy cập**: Chỉ cho phép truy cập đến các asset cần thiết

### 5. Kết luận

Với hướng dẫn này, bạn có thể dễ dàng tải các asset động từ Amazon S3 vào game Unity của mình bằng cách sử dụng URL. Điều này cho phép bạn:

- Cập nhật nội dung game mà không cần phát hành phiên bản mới
- Giảm kích thước ứng dụng ban đầu
- Tăng tính linh hoạt trong việc quản lý nội dung

Hãy thử nghiệm với các loại asset khác nhau và tối ưu hóa quá trình tải để có trải nghiệm người dùng tốt nhất!
