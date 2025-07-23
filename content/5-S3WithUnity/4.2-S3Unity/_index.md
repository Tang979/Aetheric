---
title : "Loading Dynamic Assets from S3"
date :  "2025-07-08" 
weight : 2
chapter : false
pre : " <b> 5.2 </b> "
---

## Loading Dynamic Assets from Amazon S3 into Unity

In this section, we will learn how to load dynamic assets (such as images, audio, 3D models) from Amazon S3 into your Unity game using S3 URLs.

### 1. Preparation

Before starting, make sure you have:

- Created an S3 bucket and configured it as shown in the previous section
- Uploaded your assets to your S3 bucket
- Ensured that assets are publicly accessible or have appropriate CORS configuration

### 2. Create a Script to Load Assets from S3 using URL

In Unity, we'll create a simple script to load assets from S3 using direct URLs:

```csharp
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class S3AssetLoader : MonoBehaviour
{
    // URL of the asset on S3
    public string assetUrl = "https://your-bucket-name.s3.amazonaws.com/path-to-your-asset.png";
    
    // References to components that will use the asset
    public UnityEngine.UI.RawImage imageDisplay;
    public AudioSource audioSource;
    
    void Start()
    {
        // Automatically load asset on startup
        StartCoroutine(LoadAssetFromS3());
    }
    
    // Function to load asset from URL
    public IEnumerator LoadAssetFromS3()
    {
        Debug.Log("Starting to load asset from: " + assetUrl);
        
        // Determine asset type from URL
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
            Debug.LogError("File format not supported!");
        }
    }
    
    // Load image from URL
    private IEnumerator LoadImageFromUrl(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                
                // Display image if RawImage component exists
                if (imageDisplay != null)
                {
                    imageDisplay.texture = texture;
                    Debug.Log("Successfully loaded and displayed image!");
                }
                else
                {
                    Debug.Log("Successfully loaded image, but no RawImage to display it!");
                }
            }
            else
            {
                Debug.LogError("Error loading image: " + www.error);
            }
        }
    }
    
    // Load audio from URL
    private IEnumerator LoadAudioFromUrl(string url)
    {
        // Determine audio type from URL
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
                
                // Play audio if AudioSource exists
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("Successfully loaded and played audio!");
                }
                else
                {
                    Debug.Log("Successfully loaded audio, but no AudioSource to play it!");
                }
            }
            else
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
        }
    }
    
    // Function to load asset on demand (can be called from a button)
    public void LoadAssetOnDemand(string url)
    {
        assetUrl = url;
        StartCoroutine(LoadAssetFromS3());
    }
}
```

### 3. Using the Script in Unity

#### Step 1: Create GameObject and Attach Script

1. In Unity, create a new GameObject (GameObject > Create Empty)
2. Name it "S3AssetManager"
3. Add the S3AssetLoader component to the GameObject (Add Component > Scripts > S3AssetLoader)

#### Step 2: Set Up URL and References

1. In the Inspector, enter the URL of the asset on S3 in the "Asset Url" field
   - Example: `https://your-bucket-name.s3.amazonaws.com/images/character.png`
2. Drag the necessary components into the corresponding fields:
   - Drag a GameObject with a RawImage component into the "Image Display" field to display images
   - Drag a GameObject with an AudioSource component into the "Audio Source" field to play audio

#### Step 3: Create UI to Load Assets on Demand

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
            Debug.LogError("Invalid or empty URL!");
        }
    }
}
```

### 4. Security Notes

1. **Don't store sensitive information**: Don't embed AWS credentials in Unity code
2. **Use backend as intermediary**: It's best to use a backend server to create pre-signed URLs
3. **Limit access rights**: Only allow access to necessary assets

### 5. Conclusion

With this guide, you can easily load dynamic assets from Amazon S3 into your Unity game using URLs. This allows you to:

- Update game content without releasing a new version
- Reduce initial application size
- Increase flexibility in content management

Experiment with different types of assets and optimize the loading process for the best user experience!
