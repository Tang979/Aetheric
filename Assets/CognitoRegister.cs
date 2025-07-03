using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;
using System;

public class CognitoRegister : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput;
    public TMP_InputField phoneInput;

    public GameObject registerPanel;
    public GameObject confirmationPanel;

    public GameObject loginPanel;

    private string clientId = "1iv1l3avi8ah3gn2b3ua03i81v"; // Thay bằng Client ID thật
    private string region = "ap-southeast-2";   // Region của bạn


    public void OnRegisterClick()
    {
        StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        string url = $"https://cognito-idp.{region}.amazonaws.com/";

        // Xử lý số điện thoại: loại bỏ số 0 đầu và thêm +84
        string rawPhone = phoneInput.text.Trim();
        string formattedPhone = "";

        if (rawPhone.StartsWith("0"))
            formattedPhone = "+84" + rawPhone.Substring(1);
        else if (rawPhone.StartsWith("+84"))
            formattedPhone = rawPhone;
        else
            formattedPhone = "+84" + rawPhone;

        string username = usernameInput.text.Trim(); // 👉 Đặt biến riêng cho username

        string jsonPayload = $@"
{{
    ""ClientId"": ""{clientId}"",
    ""Username"": ""{username}"",
    ""Password"": ""{passwordInput.text}"",
    ""UserAttributes"": [
        {{ ""Name"": ""email"", ""Value"": ""{emailInput.text}"" }},
        {{ ""Name"": ""phone_number"", ""Value"": ""{formattedPhone}"" }}
    ]
}}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
        request.SetRequestHeader("X-Amz-Target", "AWSCognitoIdentityProviderService.SignUp");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Đăng ký thành công: " + request.downloadHandler.text);

            // 👉 Lưu username vào PlayerPrefs để dùng khi xác nhận mã
            PlayerPrefs.SetString("PendingUsername", username);
            PlayerPrefs.Save(); // luôn gọi Save để chắc chắn dữ liệu được ghi

            // Ẩn panel đăng ký
            registerPanel.SetActive(false);

            // Hiện panel xác nhận mã OTP
            confirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ Đăng ký thất bại: " + request.downloadHandler.text);
        }
    }

    public void BackToLogin()
    {
        if (registerPanel != null) registerPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(true);
    }


}
