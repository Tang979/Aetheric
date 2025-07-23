using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UILocalizer : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public TMP_Text textUI;
        [TextArea] public string originalText;  // Nội dung gốc (tiếng Anh)
    }

    [Header("UI Elements To Translate")]
    public List<UIElement> elementsToTranslate;

    [Header("Translate Button")]
    public Button translateToVietnameseButton;

    private void Start()
    {
        if (translateToVietnameseButton != null)
        {
            translateToVietnameseButton.onClick.AddListener(() =>
            {
                StartCoroutine(TranslateAllUI("vi"));
            });
        }
    }

    IEnumerator TranslateAllUI(string targetLang)
    {
        foreach (var item in elementsToTranslate)
        {
            if (!string.IsNullOrEmpty(item.originalText))
            {
                yield return StartCoroutine(TranslateText(item.originalText, "auto", targetLang, (translated) =>
                {
                    item.textUI.text = translated;
                }));
            }
        }
    }

    IEnumerator TranslateText(string text, string sourceLang, string targetLang, System.Action<string> onTranslated)
    {
        string url = "https://<API-ID>.execute-api.ap-southeast-2.amazonaws.com/TranslateTextFunction";

        var payload = new
        {
            text = text,
            source = sourceLang,
            target = targetLang
        };

        string json = JsonUtility.ToJson(payload);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseJson = request.downloadHandler.text;
            var result = JsonUtility.FromJson<TranslateResponse>(responseJson);
            onTranslated?.Invoke(result.translatedText);
        }
        else
        {
            Debug.LogError("❌ Lỗi dịch: " + request.error);
        }
    }

    [System.Serializable]
    public class TranslateResponse
    {
        public string translatedText;
    }
}
