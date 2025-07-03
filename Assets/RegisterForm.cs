using UnityEngine;
using TMPro;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput;
    public TMP_InputField phoneInput;
    public GameObject loginPanel;
    public GameObject registerPanel;

    public void OnRegisterClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string email = emailInput.text;
        string phone = phoneInput.text;

        if (string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(phone))
        {
            Debug.LogWarning("Vui lòng điền đầy đủ thông tin!");
            return;
        }

        Debug.Log("Đăng ký thành công!");

        // TODO: Tích hợp Cognito đăng ký tại đây nếu cần
    }

}
