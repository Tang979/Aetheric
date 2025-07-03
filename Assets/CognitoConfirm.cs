using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class CognitoConfirm : MonoBehaviour
{
    public TMP_InputField codeInput;
    public Button submitButton;

    [Header("AWS Cognito configs")]
    public string region = "ap-southeast-2"; // vùng AWS
    public string clientId = "1iv1l3avi8ah3gn2b3ua03i81v"; // App client ID

    [Header("UI Panels")]
    public GameObject confirmationPanel;
    public GameObject loginPanel; // 👈 thêm cái này

    void Start()
    {
        submitButton.onClick.AddListener(() => StartCoroutine(ConfirmUser()));
    }

    IEnumerator ConfirmUser()
    {
        string url = $"https://cognito-idp.{region}.amazonaws.com/";

        string username = PlayerPrefs.GetString("PendingUsername");
        string code = codeInput.text.Trim();

        string jsonPayload = $@"
        {{
            ""ClientId"": ""{clientId}"",
            ""Username"": ""{username}"",
            ""ConfirmationCode"": ""{code}""
        }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
        request.SetRequestHeader("X-Amz-Target", "AWSCognitoIdentityProviderService.ConfirmSignUp");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Xác nhận thành công!");

            // Ẩn panel xác nhận
            if (confirmationPanel != null) confirmationPanel.SetActive(false);

            // Hiện panel login
            if (loginPanel != null) loginPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ Xác nhận thất bại: " + request.downloadHandler.text);
        }

    }



}
